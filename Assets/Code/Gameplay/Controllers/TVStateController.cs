using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
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
        public int DiskId => _diskId;

        private bool _isTVOn;
        private bool _hasDisk;
        private int _diskId;
        
        private ITVNavigationController _tvNavigationController;

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
        }

        public void InsertDisk(int diskId)
        {
            _diskId = diskId;
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

        private void TurnOnTv()
        {
            if (_hasDisk)
            {
                tvScreenMesh.material = tvScreenMaterial;
                return;
            }
            
            tvScreenMesh.material = tvStaticMaterial;
            AudioManager.Instance.PlayOST(staticAudioClip, 0.5f, true);
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
        public int DiskId { get; }
        public void InsertDisk(int diskId);
        public void TurnOnOffTv();
    }
}