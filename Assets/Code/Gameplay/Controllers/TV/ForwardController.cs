using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace DVDNights
{
    public class FastForwardController : MonoBehaviour, IFastForwardController
    {
        [Header("References")] 
        [SerializeField] private GameObject powerView;
      

        [Header("Feedback")] 
        [SerializeField] private FillView forwardFillView;
        [SerializeField] private TextMeshProUGUI forwardLevelText;
        [SerializeField] private AudioClip startForwardingClip;
        [SerializeField] private AudioClip forwardingClip;
        [SerializeField] private AudioClip stopForwardingClip;
        
        private float _currentPower;
        private float _consumedPower;
        
        private float _layerProgress;
        private IShopController _shopController;
        private ITVNavigationController _tvNavigationController;
        private bool _isForwarding;
        private IDisksController _disksController;
        private IDiskLevelController _diskLevelController;
        private ITVStateController _tvStateController;
        private Material _forwardMaterial;

        private static readonly int ScrollSpeed = Shader.PropertyToID("_ScrollSpeed");
        private static readonly int DistortionStrength = Shader.PropertyToID("_DistortionStrength");
        private static readonly int ScanlineOpacity = Shader.PropertyToID("_ScanlineOpacity");
        
        private Tween _tween;
        private ITVButton _forwardButton;

        private void Awake()
        {
            InstallService();
        }
        
        private void InstallService()
        {
            forwardFillView.InitializeView(100);
            ServiceLocator.RegisterService<IFastForwardController>(this);
        }

        private void Start()
        {
            _shopController = ServiceLocator.GetService<IShopController>();
            _tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();
            _disksController = ServiceLocator.GetService<IDisksController>();
            _diskLevelController = ServiceLocator.GetService<IDiskLevelController>();
            _tvStateController = ServiceLocator.GetService<ITVStateController>();

            _tvNavigationController.OnSubmitButtonPressed += AddPower;
            _tvNavigationController.OnNextButtonHeld += GoForward;
            _tvNavigationController.OnNextButtonReleased += StopForward;
            _forwardButton = _tvNavigationController.NextButton;
            _forwardMaterial = _tvStateController.TVScreenMaterial;

            _currentPower = 100;
            AddPower();
        }

        private void OnDestroy()
        {
            _tvNavigationController.OnSubmitButtonPressed -= AddPower;
            _tvNavigationController.OnNextButtonHeld -= GoForward;
            _tvNavigationController.OnNextButtonReleased -= StopForward;
        }

        private void GoForward()
        {
            if (!_tvStateController.IsPlayingGame)
            {
                return;
            }
            
            if (_shopController.IsShopOpened)
            {
                return;
            }
            
            if (_currentPower <= 0)
            {
                StopForward();
                return;
            }
            
            if (_isForwarding)
            {
                return;
            }
            
            AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, startForwardingClip, volume: 0.1f);
            BounceGameFeel.ForwardPitch = _diskLevelController.DiskFFMultLevel * 0.2f;
            float forwardPitch = 0.8f + BounceGameFeel.ForwardPitch;
            
            _tween?.Kill();
            _tween = DOVirtual.DelayedCall(startForwardingClip.length, () => 
            {
                AudioManager.Instance.PlayOST(AudioChannelType.DIEGETIC, forwardingClip, volume: 0.1f, loop: true, pitch: forwardPitch);
            });
            
            _isForwarding = true;
            _disksController.BoostAllDisksSpeed();
            powerView.SetActive(true);
            forwardLevelText.text = "X" + BounceGameProgression.GetFFLevelMult(_diskLevelController.DiskFFMultLevel);
            SetForwardShader();
        }
        

        private void SetForwardShader()
        {
            float speed = 0;
            float distortion = 0;
            float opacity = 0;
            
            switch (_diskLevelController.DiskFFMultLevel)
            {
                case 0:
                    speed = 0.25f;
                    distortion = 0.005f;
                    opacity = 0.025f;
                    break;
                case 1:
                case 2:
                case 3:
                    speed = 0.5f;
                    distortion = 0.01f;
                    opacity = 0.025f;
                    break;
                case 4:
                case 5:
                    speed = 0.75f;
                    distortion = 0.0125f;
                    opacity = 0.05f;
                    break;
                case 6:
                case 7:
                    speed = 1f;
                    distortion = 0.02f;
                    opacity = 0.1f;
                    break;
                case 8:
                case 9:
                    speed = 1.25f;
                    distortion = 0.035f;
                    opacity = 0.15f;
                    break;
                case 10:
                    speed = 5f;
                    distortion = 0.05f;
                    opacity = 0.5f;
                    break;
            }
            
            _forwardMaterial.SetFloat(ScrollSpeed, speed);
            _forwardMaterial.SetFloat(DistortionStrength, distortion);
            _forwardMaterial.SetFloat(ScanlineOpacity, opacity);
        }

        public void ResetForwardShader()
        {
            powerView.SetActive(false);
            _forwardMaterial.SetFloat(ScrollSpeed, 0);
            _forwardMaterial.SetFloat(DistortionStrength, 0);
            _forwardMaterial.SetFloat(ScanlineOpacity, 0);
        }

        private void StopForward()
        {
            if (!_isForwarding)
            {
                return;
            }
            
            ResetForwardShader();
            AudioManager.Instance.StopOST(AudioChannelType.DIEGETIC);
            AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, stopForwardingClip, 0.1f);
            _isForwarding = false;
            _disksController.ResetAllDisksSpeed();
        }

        private void Update()
        {
            if (_isForwarding)
            {
                float drain = BounceGameProgression.GetFFDrainRate(_diskLevelController.DiskFFDrainRateLevel) * Time.deltaTime;
                ConsumePower(drain);
            }
        }
        
        public void AddPower()
        {
            if (_shopController.IsShopOpened)
            {
                return;
            }
            
            _currentPower += BounceGameProgression.GetFFClickBonus(_diskLevelController.DiskFFBonusLevel);
            _currentPower = Mathf.Clamp(_currentPower, 0, 100);
            forwardFillView.UpdateFill(_currentPower);
        }

        private void ConsumePower(float amount)
        {
            if (_shopController.IsShopOpened)
            {
                return;
            }
            
            _currentPower = Mathf.Max(0, _currentPower - amount);
            forwardFillView.UpdateFill(_currentPower);

            if (_currentPower <= 0)
            {
                StopForward();
            }
        }

        public void FlickerForward()
        {
            _forwardMaterial = _tvStateController.TVScreenMaterial;
            var speed = 500f;
            var distortion = 0.1f;
            var opacity = 0.05f;

            _forwardMaterial.SetFloat(ScrollSpeed, speed);
            _forwardMaterial.SetFloat(DistortionStrength, distortion);
            _forwardMaterial.SetFloat(ScanlineOpacity, opacity);
        }
    }

    public interface IFastForwardController
    {
        public void FlickerForward();
        public void ResetForwardShader();
    }
}