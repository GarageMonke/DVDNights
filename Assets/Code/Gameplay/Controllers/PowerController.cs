using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class PowerController : MonoBehaviour, IPowerController
    {
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
            Debug.Log("Start Forwarding");
        }

        private void StopForward()
        {
            _isForwarding = false;
            _disksController.ResetAllDisksSpeed();
            Debug.Log("Stop Forwarding");
        }

        private void Update()
        {
            if (_isForwarding)
            {
                float drain = GameProgression.LayerDrainRate[_currentLayer] * Time.deltaTime;
                ConsumePower(drain);
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
            }
            
            Debug.Log("Current Power:" + _currentPower);
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