using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class TVStateController : MonoBehaviour, ITVStateController
    {
        [Header("References")] 
        [SerializeField] private MeshRenderer tvScreenMesh;
        
        [Header("Configuration")]
        [SerializeField] private Material tvOffMaterial;
        [SerializeField] private Material tvScreenMaterial;
        [SerializeField] private Material tvStaticMaterial;

        [Header("Feedback")] 
        [SerializeField] private AudioClip staticAudioClip;
        [SerializeField] private AudioClip loadingDVDAudioClip;
        
        public bool IsTVOn => _isTVOn;
        public bool HasDisk => _hasDisk;
        public bool IsPlayingGame => _isPlayingGame;
        public int DiskId => _diskId;

        private bool _isTVOn;
        private bool _hasDisk;
        private bool _isPlayingGame;
        private int _diskId;
        
        private ITVNavigationController _tvNavigationController;
        private Sequence _readDiskSequence;
        private IMainMenuController _tvMainMenuController;
        private IDVDTrayController _dvdTrayController;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<ITVStateController>(this);
        }

        private void Start()
        {
            _tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();
            _tvNavigationController.OnPowerButtonPressed += TurnOnOffTv;
            _dvdTrayController = ServiceLocator.GetService<IDVDTrayController>();
        }

        public void InsertDisk(int diskId)
        {
            _diskId = diskId;
            _dvdTrayController.OnTrayClosed += ReadDisk;
        }

        public void ReadDisk()
        {
            _dvdTrayController.OnTrayClosed -= ReadDisk;
            _readDiskSequence?.Kill();
            _readDiskSequence = DOTween.Sequence()
                .AppendCallback(() =>
                {
                    AudioManager.Instance.StopOST();
                    tvScreenMesh.material = tvScreenMaterial;
                    AudioManager.Instance.PlaySFX(loadingDVDAudioClip, volume: 0.5f, pitch: 1f);
                })
                .AppendInterval(loadingDVDAudioClip.length)
                .AppendCallback(() =>
                {
                    _tvMainMenuController ??= ServiceLocator.GetService<IMainMenuController>();
                    _tvMainMenuController.DisplayMenu();
                });
        }

        public void TurnOnOffTv()
        {
            if (_hasDisk)
            {
                return;
            }
            
            _isTVOn = !_isTVOn;

            if (_isTVOn)
            {
                TurnOnTv();
            }
            else
            {
                TurnOffTv();
            }
        }

        public void StartPlayingGame()
        {
            _isPlayingGame = true;
        }

        private void TurnOnTv()
        {
            if (_hasDisk)
            {
                tvScreenMesh.material = tvScreenMaterial;
                return;
            }
            
            tvScreenMesh.material = tvStaticMaterial;
            AudioManager.Instance.PlayOST(staticAudioClip, 0.002f, true);
        }

        private void TurnOffTv()
        {
            tvScreenMesh.material = tvOffMaterial;
            AudioManager.Instance.StopOST(fadeOut: false);
        }

        private void OnDestroy()
        {
            _tvNavigationController.OnPowerButtonPressed -= TurnOnOffTv;
        }
    }

    public interface ITVStateController
    {
        public bool IsTVOn { get; }
        public bool HasDisk { get; }
        public bool IsPlayingGame { get; }
        public int DiskId { get; }
        public void ReadDisk();
        public void InsertDisk(int diskId);
        public void TurnOnOffTv();
        public void StartPlayingGame();
    }
}