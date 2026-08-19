using CorePatterns.ServiceLocator;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DVDNights
{
    public class AFKController : MonoBehaviour, IAFKController
    {
        [Header("References")]
        [SerializeField] private InputActionSO clickInputAction;
        
        public float AFKTime => Time.time - _lastActivityTime;
        private ICameraController _cameraController;
        private float _lastActivityTime;
        private bool _isEnabled;
        
        private InputAction _clickInputAction;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IAFKController>(this);
        }

        private void Start()
        {
            _cameraController = ServiceLocator.GetService<ICameraController>();
            _clickInputAction = clickInputAction.GetInputAction();
        }

        public void RecordActivity()
        {
            _lastActivityTime = Time.time;
        }
        
        public void EnableController()
        {
            _isEnabled = true;
            _cameraController.OnCameraMove += RecordActivity;
            _clickInputAction.performed += RecordClickAction;
        }

        public void DisableController()
        {
            _isEnabled = false;
            _cameraController.OnCameraMove -= RecordActivity;
            _clickInputAction.performed -= RecordClickAction;
        }

        private void RecordClickAction(InputAction.CallbackContext context)
        {
            RecordActivity();
        }

        private void OnDestroy()
        {
            if (_cameraController != null)
            {
                _cameraController.OnCameraMove -= RecordActivity;
            }

            if (_clickInputAction != null)
            {
                _clickInputAction.performed -= RecordClickAction;
            }
        }
    }

    public interface IAFKController
    {
        public float AFKTime { get; }
        public void EnableController();
        public void DisableController();
        public void RecordActivity();
    }
}