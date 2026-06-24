using System;
using System.Collections;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DVDNights
{
    public class TVInteractableObject : InteractableObject, ICorruptibleObject
    {
        [Header("Configuration")]
        [SerializeField] private Vector3 cameraLockPosition;
        
        [Header("Feedback")]
        [SerializeField] private AudioClip strikeTVAudioClip;
        [SerializeField] private AudioClip TVHummingAudioClip;

        ICameraController _cameraController;
        private ITVNavigationController _tvNavigationController;
        private IMouseLayoutController _mouseLayoutController;
        private ITVStateController _tvStateController;
        private IForwardController _forwardController;
        private bool _isCorrupted;
        private bool _isInteractingWithTv;
        private bool _hasBeenHitOnce;
        private Sequence _strikeSequence;

        private void Start()
        {
            _cameraController = ServiceLocator.GetService<ICameraController>();
            _tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();
            _mouseLayoutController = ServiceLocator.GetService<IMouseLayoutController>();
            _tvStateController = ServiceLocator.GetService<ITVStateController>();
            _forwardController = ServiceLocator.GetService<IForwardController>();
        }

        public override string GetInteractionAction()
        {
            if (_isCorrupted)
            {
                if (_hasBeenHitOnce)
                {
                    return "Strike Again";
                }
                
                return "Strike";
            }
            
            return "TV";
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.C)) return;
            if (CanBeCorrupted())
            {
                Corrupt();
            }
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
            _tvNavigationController.EnableButtons();
            _mouseLayoutController.DisplayRegularLayout();
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, InteractionAudioClip, volume: 1f, pitch: 2.5f);
            _isInteractingWithTv = true;
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
            _mouseLayoutController.HideMouseLayout();
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, InteractionAudioClip, volume: 1f, pitch: 1.5f);
            _isInteractingWithTv = false;
        }

        public void Corrupt()
        {
            _isCorrupted = true;
            _tvStateController.PlayStatic(true);
            SetHasNavigation(false);
        }

        public void ClearCorruption()
        {
            _strikeSequence?.Kill();
            _strikeSequence = DOTween.Sequence()
                .Append(transform.DOLocalRotate(new Vector3(0, 0, 0), 0.05f).SetEase(Ease.Flash).OnComplete(
                    ()=>  SetHasNavigation(true)));
            AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, strikeTVAudioClip, volume: 0.65f, pitch: 1f);
            _forwardController.ResetForwardShader();
            _isCorrupted = false;
            _hasBeenHitOnce = false;
            AudioManager.Instance.StopOST(AudioChannelType.TV);
        }

        private void FirstStrike()
        {
            _hasBeenHitOnce = true;
            AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, strikeTVAudioClip, volume: 0.65f, pitch: 1f);
            _tvStateController.StrikeTV();
            _forwardController ??= ServiceLocator.GetService<IForwardController>();
            _forwardController.FlickerForward();
            float strikeAngle = Random.Range(-6, 7);
            _strikeSequence?.Kill();
            _strikeSequence = DOTween.Sequence()
                .Append(transform.DOLocalRotate(new Vector3(0, strikeAngle, 0), 0.05f).SetEase(Ease.Flash));
            AudioManager.Instance.PlayOST(AudioChannelType.TV, TVHummingAudioClip, volume: 0.65f, pitch: 1f, loop: true);
        }

        private IEnumerator FirstStrikeRoutine()
        {
            yield return new WaitForEndOfFrame();
        }

        public bool CanBeCorrupted()
        {
            return _tvStateController.IsTVOn && _tvStateController.IsPlayingGame && !_isInteractingWithTv;
        }
    }
}