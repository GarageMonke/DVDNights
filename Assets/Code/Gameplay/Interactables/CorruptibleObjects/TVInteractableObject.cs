using System.Collections;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DVDNights
{
    public class TVInteractableObject : CorruptibleInteractableObject
    {
        [Header("Configuration")]
        [SerializeField] private Vector3 cameraLockPosition;
        [SerializeField] private Light interactionLight;
        [SerializeField] private float interactionLightIntensity;
        
        [Header("Feedback")]
        [SerializeField] private AudioClip strikeTVAudioClip;
        [SerializeField] private AudioClip TVHummingAudioClip;

        ICameraController _cameraController;
        private ITVNavigationController _tvNavigationController;
        private IMouseLayoutController _mouseLayoutController;
        private ITVStateController _tvStateController;
        private IFastForwardController _fastForwardController;
        private IDisksController _disksController;
        private bool _isInteractingWithTv;
        private bool _hasBeenHitOnce;
        private Sequence _strikeSequence;

        protected override void Start()
        {
            base.Start();
            _cameraController = ServiceLocator.GetService<ICameraController>();
            _tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();
            _mouseLayoutController = ServiceLocator.GetService<IMouseLayoutController>();
            _tvStateController = ServiceLocator.GetService<ITVStateController>();
            _fastForwardController = ServiceLocator.GetService<IFastForwardController>();
            _disksController = ServiceLocator.GetService<IDisksController>();
        }

        public override string GetInteractionAction()
        {
            if (_isCorrupted)
            {
                if (_hasBeenHitOnce)
                {
                    return "Smack Again";
                }
                
                return "Smack";
            }
            
            return "TV";
        }
        

        public override void Interact()
        {
            if (_isCorrupted)
            {
                if (_hasBeenHitOnce)
                {
                    ClearCorruption();
                }
                else
                {
                    FirstStrike();
                }
                
                return;
            }
            
            Unhighlight();
            _cameraController.TweenToPosition(cameraLockPosition, 0.5f);
            _cameraController.TweenToRotation(Quaternion.identity, 0.5f);

            interactionLight.DOKill();
            interactionLight.DOIntensity(interactionLightIntensity, 0.5f).SetEase(Ease.Linear);
            _tvNavigationController.EnableButtons();
            _mouseLayoutController.DisplayRegularLayout();
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, InteractionAudioClip, volume: 1f, pitch: 2.5f);
            _isInteractingWithTv = true;
            AudioManager.Instance.SetChannelVolume(AudioChannelType.TV, 100f);
        }

        public override void StopInteraction()
        {
            if (_isCorrupted)
            {
                return;
            }
            
            Highlight();
            _tvNavigationController.DisableButtons();
            _cameraController.TweenToPosition(_cameraController.OriginPosition, 0.5f);
            _cameraController.TweenToRotation(Quaternion.identity, 0.5f);
            interactionLight.DOKill();
            interactionLight.DOIntensity(0f, 0.5f).SetEase(Ease.Linear);
            _mouseLayoutController.HideMouseLayout();
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, InteractionAudioClip, volume: 1f, pitch: 1.5f);
            _isInteractingWithTv = false;
            AudioManager.Instance.SetChannelVolume(AudioChannelType.TV, 25f);
        }

        public override void Corrupt()
        {
            base.Corrupt();
            _disksController = ServiceLocator.GetService<IDisksController>();
            _disksController.MuteAllDiscs();
            _tvStateController.PlayStatic(true);
            SetHasNavigation(false);
        }

        public override void ClearCorruption()
        {
            base.ClearCorruption();
            _strikeSequence?.Kill();
            _strikeSequence = DOTween.Sequence()
                .Append(transform.DOLocalRotate(new Vector3(0, 0, 0), 0.05f).SetEase(Ease.Flash).OnComplete(
                    ()=>  SetHasNavigation(true)));
            AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, strikeTVAudioClip, volume: 0.65f, pitch: 1f);
            _fastForwardController.ResetForwardShader();
            _hasBeenHitOnce = false;
            AudioManager.Instance.StopOST(AudioChannelType.TV);
            _disksController = ServiceLocator.GetService<IDisksController>();
            _disksController.UnmuteAllDiscs();
        }

        private void FirstStrike()
        {
            _hasBeenHitOnce = true;
            AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, strikeTVAudioClip, volume: 0.65f, pitch: 1f);
            _tvStateController.StrikeTV();
            _fastForwardController ??= ServiceLocator.GetService<IFastForwardController>();
            _fastForwardController.FlickerForward();
            float strikeAngle = Random.Range(-6, 7);
            _strikeSequence?.Kill();
            _strikeSequence = DOTween.Sequence()
                .Append(transform.DOLocalRotate(new Vector3(0, strikeAngle, 0), 0.05f).SetEase(Ease.Flash));
            AudioManager.Instance.PlayOST(AudioChannelType.TV, TVHummingAudioClip, volume: 0.65f, pitch: 1f, loop: true);
        }

        public override bool CanBeCorrupted()
        {
            return _tvStateController.IsTVOn && _tvStateController.IsPlayingGame && !_isInteractingWithTv;
        }
    }
}