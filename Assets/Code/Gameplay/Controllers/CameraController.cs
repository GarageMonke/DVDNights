using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class CameraController : MonoBehaviour, ICameraController
    {
        [Header("Camera Reference")]
        [SerializeField] private Camera _camera;

        [Header("Rotation Settings")]
        [SerializeField] private float _mouseSensitivity = 2f;

        [Header("X Axis (Pitch - Up/Down)")]
        [SerializeField] private float _minPitchAngle = -30f;
        [SerializeField] private float _maxPitchAngle = 60f;

        [Header("Y Axis (Yaw - Left/Right)")]
        [SerializeField] private float _minYawAngle = -90f;
        [SerializeField] private float _maxYawAngle = 90f;

        private float _currentPitch;
        private float _currentYaw;
        private bool _isEnabled;

        private Vector3 _previousCameraPosition;
        private Quaternion _previousCameraRotation;
        
        public Camera Camera => _camera;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            if (!_camera)
            {
                _camera = Camera.main;
            }
            
            Vector3 startAngles = _camera.transform.eulerAngles;
            _currentPitch = startAngles.x;
            _currentYaw   = startAngles.y;
            
            ServiceLocator.RegisterService<ICameraController>(this);
        }
        

        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }
            
            HandleRotation();
        }

        public void HandleRotation()
        {
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

            // Yaw (Y axis, left/right)
            _currentYaw   = Mathf.Clamp(_currentYaw + mouseX, _minYawAngle, _maxYawAngle);

            // Pitch (X axis, up/down)
            _currentPitch = Mathf.Clamp(_currentPitch - mouseY, _minPitchAngle, _maxPitchAngle);
            _camera.transform.localRotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
        }

        public void SetSensitivity(float sensitivity)
        {
            _mouseSensitivity = sensitivity;
        }

        public void EnableNavigation()
        {
            _isEnabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void DisableNavigation()
        {
            _isEnabled = false;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        

        public void UpdateCameraPositionAndRotation(Vector3 newCameraPosition, Vector3 newCameraRotation)
        {
            _previousCameraPosition = transform.position;
            _previousCameraRotation = transform.rotation;
            
            transform.position = newCameraPosition;
            transform.rotation = Quaternion.Euler(newCameraRotation);
        }

        public void RestoreCameraPositionAndRotation()
        {
            transform.position = _previousCameraPosition;
            transform.rotation = _previousCameraRotation;
        }
    }

    public interface ICameraController
    {
        public Camera Camera { get; }
        public void HandleRotation();
        public void SetSensitivity(float sensitivity);
        public void EnableNavigation();
        public void DisableNavigation();

        public void UpdateCameraPositionAndRotation(Vector3 newCameraPosition, Vector3 newCameraRotation);
        public void RestoreCameraPositionAndRotation();
    }
}