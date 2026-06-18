using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class TurntableInteractableObject : InteractableObject
    {
        [Header("Head-Sequence")] 
        [SerializeField] private Transform head;
        [SerializeField] private Vector3 headLockRotation;
        [SerializeField] private Vector3 headFreeRotation;
        [SerializeField] private RotateTransform vinylRotateTransform;

        [Header("Head-Feedback")] 
        [SerializeField] private AudioClip placeHeadOnVinylAudioClip;
        [SerializeField] private AudioClip readingVinylAudioClip;
        [SerializeField] private AudioClip removeHeadOnVinylAudioClip;
        
        [Header("Configuration")]
        [SerializeField] private Vector3 cameraLockPosition;
        [SerializeField] private Vector3 cameraLockRotation;

        ICameraController _cameraController;
        private ITrackSelectionController _trackSelectorController;
        
        private Sequence _spinningSequence;
        private bool _isSpinning;

        private void Start()
        {
            _cameraController = ServiceLocator.GetService<ICameraController>();
            _trackSelectorController = ServiceLocator.GetService<ITrackSelectionController>();
        }

        public override string GetInteractionAction()
        {
            return "Select Track";
        }

        public override void Interact()
        {
            Unhighlight();
            _cameraController.TweenToPosition(cameraLockPosition, 0.5f, ()=> _trackSelectorController.OpenTrackSelector());
            _cameraController.TweenToRotation(Quaternion.Euler(cameraLockRotation), 0.5f);
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, InteractionAudioClip, volume: 1f, pitch: 2.5f);
            
            if (_trackSelectorController.IsPlayingTrack)
            {
                StopSpinning();
            }
        }

        public override void StopInteraction()
        {
            Highlight();
            _cameraController.TweenToPosition(_cameraController.OriginPosition, 0.5f);
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, InteractionAudioClip, volume: 1f, pitch: 1.5f);
            _trackSelectorController.CloseTrackSelector();
            
            if (_trackSelectorController.IsPlayingTrack)
            {
                StartSpinning();
            }
        }

        private void StartSpinning()
        {
            _spinningSequence?.Kill();
            _spinningSequence = DOTween.Sequence()
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                head.DOLocalRotate(headLockRotation, 1f).OnComplete(() =>
                {
                    AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, placeHeadOnVinylAudioClip);
                });
            })
            .AppendInterval(1f + placeHeadOnVinylAudioClip.length)
            .AppendCallback(() =>
            {
                AudioManager.Instance.PlaySFX(AudioChannelType.TURNTABLE, readingVinylAudioClip);
                vinylRotateTransform.EnableRotation();
            })
            .AppendInterval(readingVinylAudioClip.length)
            .AppendCallback(() =>
            {
                _trackSelectorController.PlaySelectedTrack();
            });
        }

        private void StopSpinning()
        {
            _spinningSequence?.Kill();
            _spinningSequence = DOTween.Sequence()
                .AppendCallback(
                    ()=>
                    {
                        AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, removeHeadOnVinylAudioClip);
                        head.DOLocalRotate(headFreeRotation, removeHeadOnVinylAudioClip.length);
                    })
                .AppendInterval(removeHeadOnVinylAudioClip.length)
                .AppendCallback(() =>
                {
                    vinylRotateTransform.DisableRotation();
                });
            
        }
    }
}