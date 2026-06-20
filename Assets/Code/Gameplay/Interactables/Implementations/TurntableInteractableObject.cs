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
        [SerializeField] private Vector3[] vinylToTurntablePath;
        [SerializeField] private Vector3[] vinylCasePath;
        [SerializeField] private Vector3 vinylCaseStartPosition;
        [SerializeField] private Vector3 vinylCaseStartRotation;
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
        [SerializeField] private Vector3 cameraReleaseRotation;

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
            _vinylPathToTurntable = CurveGenerator.GetCurvePoints(vinylToTurntablePath[0], vinylToTurntablePath[2], vinylToTurntablePath[4], 10);
            _vinylCasePath = CurveGenerator.GetCurvePoints(vinylCasePath[0], vinylCasePath[1], vinylCasePath[2], 5);
        }

        public override string GetInteractionAction()
        {
            return "Select Track";
        }

        public override void Interact()
        {
            Unhighlight();
            _interactionController.DisableInteractions();
            _trackSelectorController.OnTrackSelectionCloseRequested += PlayVinyl;
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
            _cameraController.TweenToRotation(Quaternion.Euler(cameraReleaseRotation), 0.5f);
            _trackSelectorController.CloseTrackSelector();

            if (!_trackSelectorController.IsPlayingTrack)
            {
                _interactionController.StopInteractionWithObject();
                return;
            }

            _spinningSequence?.Kill();
            _spinningSequence = DOTween.Sequence()
                .AppendInterval(0.5f)
                .AppendCallback(() => { vinylCase.gameObject.SetActive(true); })
                .Append(vinylCase.DOLocalMove(vinylCasePreparePosition, 0.75f).SetEase(Ease.OutExpo)
                    .OnComplete(() => { vinyl.parent = transform; }))
                .AppendInterval(0.8f)
                .AppendCallback(() =>
                {
                    vinyl.DOLocalMove(_vinylPathToTurntable[0], 1f).SetEase(Ease.InSine);
                    vinylCase.DOLocalMove(vinylCaseFirstPosition, 1f).SetEase(Ease.OutSine);
                })
                .AppendInterval(1f)
                .AppendCallback(() =>
                    {
                        vinylCase.DOLocalPath(_vinylCasePath, 10f, PathType.CatmullRom).SetEase(Ease.OutElastic).OnWaypointChange(waypointIndex =>
                        {
                            if (waypointIndex == 5)
                            {
                                vinylCase.gameObject.SetActive(false);
                            }
                        });
                        
                        vinyl.DOBlendableLocalRotateBy(new Vector3(0, 0, -360), 0.5f, RotateMode.FastBeyond360)
                            .SetEase(Ease.Linear);
                    }
                    )
                .AppendInterval(0.6f)
                .AppendCallback(() =>
                {
                    vinyl.DOLocalPath(_vinylPathToTurntable, 2f, PathType.CatmullRom).SetEase(Ease.InOutSine)
                        .OnWaypointChange(waypointIndex =>
                        {
                            if (waypointIndex == 5)
                            {
                                vinyl.DOLocalRotate(new Vector3(0, 0, 0), 0.35f).SetEase(Ease.OutSine);
                            }
                            
                            if (waypointIndex == 10)
                            {
                                //AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, DVDOnTrayAudioClip, volume: 0.5f, pitch: 1f);
                            }
                        }).OnComplete(StartSpinning);
                });

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
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                _interactionController.StopInteractionWithObject();
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