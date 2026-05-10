using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class PowerController : MonoBehaviour, IPowerController
    {
        private static readonly int ScrollSpeed = Shader.PropertyToID("_ScrollSpeed");
        private static readonly int DistortionStrength = Shader.PropertyToID("_DistortionStrength");
        private static readonly int ScanlineOpacity = Shader.PropertyToID("_ScanlineOpacity");

        [Header("References")] 
        [SerializeField] private GameObject powerView;
        [SerializeField] private Material forwardMaterial;
        
        private int _pressValue = 1;
        private float _currentPower;
        private float _consumedPower;
        private int _currentLayer;
        private int _powerLevel;
        
        private float _layerProgress;
        private IShopController _shopController;
        private ITVNavigationController _tvNagivationController;
        private bool _isForwarding;
        private IDisksController _disksController;

        public int PowerLevel => _currentLayer;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IPowerController>(this);
        }

        private void Start()
        {
            _shopController = ServiceLocator.GetService<IShopController>();
            _tvNagivationController = ServiceLocator.GetService<ITVNavigationController>();
            _disksController = ServiceLocator.GetService<IDisksController>();

            _tvNagivationController.OnSubmitButtonPressed += AddPower;
            _tvNagivationController.OnNextButtonHeld += GoForward;
            _tvNagivationController.OnNextButtonReleased += StopForward;
        }

        private void OnDestroy()
        {
            _tvNagivationController.OnSubmitButtonPressed -= AddPower;
            _tvNagivationController.OnNextButtonHeld -= GoForward;
            _tvNagivationController.OnNextButtonReleased -= StopForward;
        }

        private void GoForward()
        {
            _isForwarding = true;
            _disksController.BoostAllDisksSpeed();
            powerView.SetActive(true);
            SetForwardShader();
        }

        private void SetForwardShader()
        {
            float speed = 0;
            float distortion = 0;
            float opacity = 0;
            
            switch (_powerLevel)
            {
                case 0:
                    speed = 0.25f;
                    distortion = 0.005f;
                    opacity = 0.025f;
                    break;
                case 1:
                    speed = 0.5f;
                    distortion = 0.01f;
                    opacity = 0.025f;
                    break;
                case 2:
                    speed = 0.75f;
                    distortion = 0.0125f;
                    opacity = 0.05f;
                    break;
                case 3:
                    speed = 1f;
                    distortion = 0.02f;
                    opacity = 0.1f;
                    break;
                case 4:
                    speed = 1.25f;
                    distortion = 0.035f;
                    opacity = 0.15f;
                    break;
                case 5:
                    speed = 5f;
                    distortion = 0.05f;
                    opacity = 0.5f;
                    break;
            }
            
            forwardMaterial.SetFloat(ScrollSpeed, speed);
            forwardMaterial.SetFloat(DistortionStrength, distortion);
            forwardMaterial.SetFloat(ScanlineOpacity, opacity);
        }

        private void ResetForwardShader()
        {
            powerView.SetActive(false);
            forwardMaterial.SetFloat(ScrollSpeed, 0);
            forwardMaterial.SetFloat(DistortionStrength, 0);
            forwardMaterial.SetFloat(ScanlineOpacity, 0);
        }

        private void StopForward()
        {
            _isForwarding = false;
            _disksController.ResetAllDisksSpeed();
            ResetForwardShader();
        }

        private void Update()
        {
            if (_isForwarding)
            {
                float drain = GameProgression.LayerDrainRate[_currentLayer] * Time.deltaTime;
                ConsumePower(drain);
            }

            if (Input.anyKeyDown)
            {
                _powerLevel++;
            }
        }
        
        public void AddPower()
        {
            if (_shopController.IsShopOpened)
            {
                return;
            }
            
            _currentPower += _pressValue * GameProgression.GetPowerPoints(_powerLevel);
            Debug.Log("Current Power:" + _currentPower);
            RecalculateLayer();
        }

        private void ConsumePower(float amount)
        {
            if (_shopController.IsShopOpened)
            {
                return;
            }
            
            _currentPower = Mathf.Max(0, _currentPower - amount);

            if (_currentPower <= 0)
            {
                _disksController.ResetAllDisksSpeed();
                ResetForwardShader();
            }
            
            RecalculateLayer();
        }

        public void SetPressValue(int value)
        {
            _pressValue = value;
        }
        
        private void RecalculateLayer()
        {
            int newLayer = 0;
            float remaining = _currentPower;

            for (int i = 0; i < GameProgression.LayerThresholdsLength; i++)
            {
                if (remaining >= GameProgression.GetLayerThreshold(i))
                {
                    remaining -= GameProgression.GetLayerThreshold(i);
                    newLayer = i + 1;
                }
                else
                {
                    _layerProgress = remaining / GameProgression.GetLayerThreshold(i);
                    break;
                }
            }
            
            newLayer = Mathf.Min(newLayer, GameProgression.LayerThresholdsLength);

            _currentLayer = newLayer;
            
            Debug.Log("Current Layer:" + _currentLayer);
        }
    }

    public interface IPowerController
    {
        public int PowerLevel { get; }
    }
}