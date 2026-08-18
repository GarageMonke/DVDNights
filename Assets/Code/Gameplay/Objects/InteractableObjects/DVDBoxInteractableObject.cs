using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class DVDBoxInteractableObject : InteractableObject
    {
        [Header("References")] 
        [SerializeField] private TVInteractableObject tvInteractableObject;
        [SerializeField] private DoorInteractableObject doorInteractableObject;
        [SerializeField] private Collider dvdBoxCollider;
        
        [Header("Configuration")] 
        [SerializeField] private Transform dvdDisk;
        [SerializeField] private Transform dvdBoxFace;
        [SerializeField] private Transform dvdPathParent;
        
        [Header("Desktop-Configuration")] 
        [SerializeField] private Vector3 dvdBoxDesktopPosition;
        [SerializeField] private Vector3 dvdBoxDesktopRotation;
        
        [Header("Position-Vectors")]
        [SerializeField] private Vector3 clickDVDPosition;
        [SerializeField] private Vector3 firstStepDVDPosition;
        [SerializeField] private Vector3 openDVDRotation;
        [SerializeField] private Vector3[] dvdPathNodes;
        
        [Header("Feedback")]
        [SerializeField] private AudioClip openDVDBoxAudioClip;
        [SerializeField] private AudioClip closeDVDBoxAudioClip;
        [SerializeField] private AudioClip DVDOnTrayAudioClip;
        [SerializeField] private AudioClip pickUpDvdBoxAudioClip;
        
        private Sequence _openDvdBoxSequence;
        private IDVDTrayController _dvdTrayController;
        private IInteractionController _interactionController;
        private Vector3[] _dvdPathToTray;
        private ICameraController _cameraController;
        private ITVStateController _tvStateController;
        private bool _isInDesktop;

        protected override void Start()
        {
            base.Start();
            _dvdTrayController = ServiceLocator.GetService<IDVDTrayController>();
            _dvdTrayController.OnTrayOpened += CheckInteractionStatus;
            _dvdTrayController.OnTrayClosed += CheckInteractionStatus;
            _interactionController = ServiceLocator.GetService<IInteractionController>();
            _dvdPathToTray = CurveGenerator.GetCurvePoints(dvdPathNodes[0], dvdPathNodes[1], dvdPathNodes[2], 10);
            _cameraController = ServiceLocator.GetService<ICameraController>();
            _tvStateController = ServiceLocator.GetService<ITVStateController>();
        }

        private void CheckInteractionStatus()
        {
            if (_dvdTrayController.IsTrayOpened)
            {
                EnableInteraction();
                IgnoreNavigation(true);
            }
            else
            {
                DisableInteraction();
                IgnoreNavigation(false);
            }
        }

        public override string GetInteractionAction()
        {
            if (!_isInDesktop)
            {
                return "Take DVD Box";
            }
            
            return "Insert DVD";
        }

        public override void Interact()
        {
            if (!IsEnabled)
            {
                return;
            }

            if (!_isInDesktop)
            {
                AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, pickUpDvdBoxAudioClip, pitch: 1f, randomizePitch: true);
                TeleportDvdBoxToDesktop();
                DisableInteraction();
                _dvdTrayController.SetCurrentDVDBox(this);
                DOVirtual.DelayedCall(0.5f, ()=> doorInteractableObject.ForceClose());
                OnInteractionPerformed?.Invoke();
                return;
            }
            
            DisableInteraction();
          
            _interactionController.DisableInteractions();
            _openDvdBoxSequence?.Kill();
            _openDvdBoxSequence = DOTween.Sequence()
                .AppendCallback(() =>
                {
                    _cameraController.DisableNavigation();
                    _cameraController.RestoreCameraPositionAndRotation();
                    AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, openDVDBoxAudioClip, pitch: 1.2f);
                })
                //Open Box
                .AppendInterval(openDVDBoxAudioClip.length * 0.5f)
                .Append(dvdBoxFace.DOLocalRotate(openDVDRotation, 0.5f).SetEase(Ease.InSine))
                .AppendInterval(0.75f)
                .AppendCallback(() =>
                {
                    AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, InteractionAudioClip, pitch: 1.2f);
                    dvdDisk.DOLocalMove(clickDVDPosition, 0.5f).SetEase(Ease.InBounce);
                })
                //Take out DVD
                .AppendInterval(1f)
                .Append(dvdDisk.DOLocalMove(firstStepDVDPosition, 0.25f).SetEase(Ease.InSine))
                .AppendInterval(0.3f)
                .AppendCallback(() =>
                {
                    dvdBoxFace.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.InSine).OnComplete(() =>
                    {
                        AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, closeDVDBoxAudioClip, pitch: 1.2f);
                    });

                })
                //Grab DVD
                .AppendCallback(() =>
                {
                    dvdDisk.parent = dvdPathParent;
                    dvdDisk.DOLocalMove(dvdPathNodes[0], 1f).SetEase(Ease.InSine);
                    dvdDisk.DOLocalRotate(new Vector3(180, 0, 0), 1f).SetEase(Ease.InSine);
                    dvdDisk.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                })
                //Spin DVD
                .AppendInterval(1.1f)
                .Append(dvdDisk.DOBlendableLocalRotateBy(new Vector3(0, 360 * 3, 0), 0.35f, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear))
                .AppendInterval(0.5f)
                //Go to Tray
                .Append(dvdDisk.DOLocalPath(_dvdPathToTray, 2f, PathType.CatmullRom).SetEase(Ease.InOutSine)
                    .OnWaypointChange(waypointIndex =>
                    {
                        if (waypointIndex == 3)
                        {
                            dvdDisk.DOLocalRotate(new Vector3(90, 0, 0), 0.25f).SetEase(Ease.OutSine);
                        }

                        if (waypointIndex == 10)
                        {
                            AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, DVDOnTrayAudioClip, volume: 0.5f, pitch: 1f);
                            dvdDisk.parent = _dvdTrayController.TrayTransform;
                            _interactionController.EnableInteractions();
                            _interactionController.ForceInteraction(tvInteractableObject);
                            _tvStateController.InsertDisk(1);
                        }
                    }));
        }
        
        public void TeleportDvdBoxToDesktop()
        {
            gameObject.SetActive(true);
            transform.localPosition = dvdBoxDesktopPosition;
            transform.localRotation = Quaternion.Euler(dvdBoxDesktopRotation);
            _isInDesktop = true;
        }

        private void OnDestroy()
        {
            _dvdTrayController.OnTrayOpened -= CheckInteractionStatus;
            _dvdTrayController.OnTrayClosed -= CheckInteractionStatus;
        }
    }
}