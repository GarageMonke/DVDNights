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
        [SerializeField] private Material forwardMaterial;

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
        private ITVNavigationController _tvNagivationController;
        private bool _isForwarding;
        private IDisksController _disksController;
        private IDiskLevelController _diskLevelController;
        private ITVStateController _tvStateController;

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
            _tvNagivationController = ServiceLocator.GetService<ITVNavigationController>();
            _disksController = ServiceLocator.GetService<IDisksController>();
            _diskLevelController = ServiceLocator.GetService<IDiskLevelController>();
            _tvStateController = ServiceLocator.GetService<ITVStateController>();

            _tvNagivationController.OnSubmitButtonPressed += AddPower;
            _tvNagivationController.OnNextButtonHeld += GoForward;
            _tvNagivationController.OnNextButtonReleased += StopForward;
            _forwardButton = _tvNagivationController.NextButton;

            _currentPower = 100;
            AddPower();
        }

        private void OnDestroy()
        {
            _tvNagivationController.OnSubmitButtonPressed -= AddPower;
            _tvNagivationController.OnNextButtonHeld -= GoForward;
            _tvNagivationController.OnNextButtonReleased -= StopForward;
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
            GameFeel.ForwardPitch = _diskLevelController.DiskFFMultLevel * 0.2f;
            float forwardPitch = 0.8f + GameFeel.ForwardPitch;
            
            _tween?.Kill();
            _tween = DOVirtual.DelayedCall(startForwardingClip.length, () => 
            {
                AudioManager.Instance.PlayOST(AudioChannelType.DIEGETIC, forwardingClip, volume: 0.1f, loop: true, pitch: forwardPitch);
            });
            
            _isForwarding = true;
            _disksController.BoostAllDisksSpeed();
            powerView.SetActive(true);
            forwardLevelText.text = "X" + GameProgression.GetFFLevelMult(_diskLevelController.DiskFFMultLevel);
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
            
            forwardMaterial.SetFloat(ScrollSpeed, speed);
            forwardMaterial.SetFloat(DistortionStrength, distortion);
            forwardMaterial.SetFloat(ScanlineOpacity, opacity);
        }

        public void ResetForwardShader()
        {
            powerView.SetActive(false);
            forwardMaterial.SetFloat(ScrollSpeed, 0);
            forwardMaterial.SetFloat(DistortionStrength, 0);
            forwardMaterial.SetFloat(ScanlineOpacity, 0);
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
                float drain = GameProgression.GetFFDrainRate(_diskLevelController.DiskFFDrainRateLevel) * Time.deltaTime;
                ConsumePower(drain);
            }
        }
        
        public void AddPower()
        {
            if (_shopController.IsShopOpened)
            {
                return;
            }
            
            _currentPower += GameProgression.GetFFClickBonus(_diskLevelController.DiskFFBonusLevel);
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
            forwardMaterial = _tvStateController.TVScreenMaterial;
            var speed = 500f;
            var distortion = 0.1f;
            var opacity = 0.05f;

            forwardMaterial.SetFloat(ScrollSpeed, speed);
            forwardMaterial.SetFloat(DistortionStrength, distortion);
            forwardMaterial.SetFloat(ScanlineOpacity, opacity);
        }
    }

    public interface IFastForwardController
    {
        public void FlickerForward();
        public void ResetForwardShader();
    }
}