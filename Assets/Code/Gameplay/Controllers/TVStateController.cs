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
        [SerializeField] private MeshRenderer tvLedMesh;
        
        [Header("Configuration")]
        [SerializeField] private Material tvOffMaterial;
        [SerializeField] private Material tvScreenMaterial;
        [SerializeField] private Material tvStaticMaterial;
        [SerializeField] private Material tvLedMaterial;

        [Header("Feedback")] 
        [SerializeField] private AudioClip staticAudioClip;
        [SerializeField] private AudioClip loadingDVDAudioClip;
        
        public bool IsTVOn => _isTVOn;
        public bool HasDisk => _hasDisk;
        public bool IsPlayingGame => _isPlayingGame;
        public bool IsDiskOnTray => _diskId > 0;
        public Material TVScreenMaterial => tvScreenMesh.material;
        public int DiskId => _diskId;

        private bool _isTVOn;
        private bool _hasDisk;
        private bool _isPlayingGame;
        private bool _isTesting;
        private int _diskId;
        
        private ITVNavigationController _tvNavigationController;
        private Sequence _readDiskSequence;
        private IDVDTrayController _dvdTrayController;
        private IMainMenuController _tvMainMenuController;
        private IInteractionController _interactionController;

        private void Awake()
        {
            InstallService();
            _isTesting = true;
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
                    tvScreenMesh.material = tvScreenMaterial;
                    AudioManager.Instance.StopOST(AudioChannelType.TV);
                    AudioManager.Instance.PlaySFX(AudioChannelType.TV, loadingDVDAudioClip, volume: 0.5f, pitch: 1f);
                })
                .AppendInterval(loadingDVDAudioClip.length)
                .AppendCallback(() =>
                {
                    _interactionController ??= ServiceLocator.GetService<IInteractionController>();
                    _interactionController.SetCurrentInteraction(_tvNavigationController.TVInteractableObject);
                    _tvMainMenuController ??= ServiceLocator.GetService<IMainMenuController>();
                    _tvMainMenuController.DisplayMenu();
                    _hasDisk = true;
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

        public void PlayStatic(bool isCorrupted = false)
        {
            tvLedMesh.material = tvOffMaterial;
            tvScreenMesh.material = tvStaticMaterial;
            float volume = isCorrupted ? 0.5f : 0.025f;
            AudioManager.Instance.PlayOST(AudioChannelType.TV, staticAudioClip, volume, true);
        }

        public void StrikeTV()
        {
            AudioManager.Instance.StopOST(AudioChannelType.TV, fadeOut: false);
            tvScreenMesh.material = tvScreenMaterial;
        }

        public void RemoveDisk()
        {
            _hasDisk = false;
            _diskId = -1;
        }

        private void TurnOnTv()
        {
            if (_isTesting)
            {
                _hasDisk = true;
                tvScreenMesh.material = tvScreenMaterial;
                _tvMainMenuController ??= ServiceLocator.GetService<IMainMenuController>();
                _tvMainMenuController.DisplayMenu();
            }
            
            tvScreenMesh.material = tvScreenMaterial;
            tvLedMesh.material = tvOffMaterial;
           
            if (!_hasDisk)
            {
                PlayStatic();
            }
        }

        private void TurnOffTv()
        {
            tvScreenMesh.material = tvOffMaterial;
            tvLedMesh.material = tvLedMaterial;
            AudioManager.Instance.StopOST(AudioChannelType.TV, fadeOut: false);
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
        public bool IsDiskOnTray { get; }
        public Material TVScreenMaterial { get; }
        public int DiskId { get; }
        public void ReadDisk();
        public void InsertDisk(int diskId);
        public void TurnOnOffTv();
        public void StartPlayingGame();
        public void PlayStatic(bool isCorrupted = false);
        public void StrikeTV();
        public void RemoveDisk();
    }
}