using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class TurntableInteractableObject : InteractableObject
    {
        [Header("PlayVinyl-Sequence")] 
        [SerializeField] private Transform vinylCase;
        [SerializeField] private Transform vinyl;
        [SerializeField] private Vector3 vinylStartPosition;
        [SerializeField] private Vector3[] vinylToTurntablePath;
        [SerializeField] private Vector3[] vinylCasePath;
        [SerializeField] private Vector3 vinylCasePreparePosition;
        [SerializeField] private Vector3 vinylCaseFirstPosition;
        
        [Header("Head-Sequence")] 
        [SerializeField] private Transform head;
        [SerializeField] private Vector3 headLockRotation;
        [SerializeField] private Vector3 headFreeRotation;

        [Header("Head-Feedback")] 
        [SerializeField] private AudioClip placeHeadOnVinylAudioClip;
        [SerializeField] private AudioClip readingVinylAudioClip;
        [SerializeField] private AudioClip removeHeadOnVinylAudioClip;
        
        [Header("Configuration")]
        [SerializeField] private Vector3 cameraLockPosition;
        [SerializeField] private Vector3 cameraLockRotation;

        private ICameraController _cameraController;
        private ITrackSelectionController _trackSelectorController;
        private IInteractionController _interactionController;
        private RotateTransform _vinylRotateTransform;
        
        private Sequence _spinningSequence;
        private bool _isSpinning;
        private Vector3[] _vinylPathToTurntable;
        private Vector3[] _vinylCasePath;


        private void Start()
        {
            _cameraController = ServiceLocator.GetService<ICameraController>();
            _trackSelectorController = ServiceLocator.GetService<ITrackSelectionController>();
            _interactionController = ServiceLocator.GetService<IInteractionController>();
            _vinylPathToTurntable = CurveGenerator.GetCurvePoints(vinylToTurntablePath[0], vinylToTurntablePath[1], vinylToTurntablePath[2], 10);
            _vinylCasePath = CurveGenerator.GetCurvePoints(vinylCasePath[0], vinylCasePath[1], vinylCasePath[2], 5);
        }

        public override string GetInteractionAction()
        {
            return "Select Track";
        }

        public override void Interact()
        {
            _interactionController.DisableInteractions();
            _trackSelectorController.OnTrackSelectionCloseRequested += PlayVinyl;
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
            _interactionController.EnableInteractions();
            Highlight();
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, InteractionAudioClip, volume: 1f, pitch: 1.5f);          
        }

        private void PlayVinyl()
        {
            _vinylRotateTransform = vinyl.GetComponent<RotateTransform>();
            _trackSelectorController.OnTrackSelectionCloseRequested -= PlayVinyl;
            _cameraController.TweenToPosition(_cameraController.OriginPosition, 0.5f);
            _trackSelectorController.CloseTrackSelector();

            if (!_trackSelectorController.IsPlayingTrack)
            {
                _interactionController.StopInteractionWithObject();
                return;
            }

            _spinningSequence?.Kill();
            _spinningSequence = DOTween.Sequence()
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    vinylCase.gameObject.SetActive(true);
                })
                .Append(vinylCase.DOLocalMove(vinylCasePreparePosition, 1f).SetEase(Ease.InSine).OnComplete(
                    ()=>
                    {
                        vinyl.parent = transform;
                    }))
                .AppendCallback(() =>
                {
                    vinyl.DOLocalMove(_vinylPathToTurntable[0], 1f).SetEase(Ease.InSine);
                    vinylCase.DOLocalMove(vinylCaseFirstPosition, 0.6f).SetEase(Ease.OutSine);
                })
                .AppendInterval(1f)
                .Append(vinylCase.DOLocalPath(_vinylCasePath, 1f, PathType.CatmullRom).SetEase(Ease.OutSine)
                    .OnWaypointChange(waypointIndex =>
                    {
                        if (waypointIndex == 5)
                        {
                            vinylCase.gameObject.SetActive(false);
                        }
                    }))
                .Append(vinyl.DOLocalPath(_vinylPathToTurntable, 1f, PathType.CatmullRom).SetEase(Ease.InOutSine)
                    .OnWaypointChange(waypointIndex =>
                    {
                        if (waypointIndex == 3)
                        {
                            vinyl.DOLocalRotate(new Vector3(0, 0, 0), 0.25f).SetEase(Ease.OutSine);
                        }

                        if (waypointIndex == 10)
                        {
                            //AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, DVDOnTrayAudioClip, volume: 0.5f, pitch: 1f);
                        }
                    })).OnComplete(StartSpinning);

        }

        private void StartSpinning()
        {
            _spinningSequence?.Kill();
            _spinningSequence = DOTween.Sequence()
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
                _vinylRotateTransform.EnableRotation();
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
                    _vinylRotateTransform.DisableRotation();
                });
            
        }
    }
}