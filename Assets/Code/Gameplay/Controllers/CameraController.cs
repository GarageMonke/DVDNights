using System;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace DVDNights
{
    public class CameraController : MonoBehaviour, ICameraController
    {
        [Header("Camera Reference")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Volume postProcessing;

        [Header("Rotation Settings")]
        [SerializeField] private float _mouseSensitivity = 2f;
        [SerializeField] private bool _mouseYClamp;

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
        private Vector3 _initialCameraPosition;
        private Quaternion _previousCameraRotation;
        private Tween _currentMoveTween;
        private Tween _currentRotationTween;
        private Tween _currentDelayEnableTween;

        public Camera Camera => mainCamera;
        public Vector3 OriginPosition => _initialCameraPosition;
        
        private DepthOfField _depthOfField;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            if (!mainCamera)
            {
                mainCamera = Camera.main;
            }
            
            DisableNavigation();
            _initialCameraPosition = mainCamera.transform.localPosition;
            Vector3 startAngles = mainCamera.transform.eulerAngles;
            _currentPitch = startAngles.x;
            _currentYaw   = startAngles.y;
            postProcessing.profile.TryGet(out _depthOfField);

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
            if (_mouseYClamp)
            {
                _currentYaw = Mathf.Clamp(_currentYaw + mouseX, _minYawAngle, _maxYawAngle);
            }
            else
            {
                _currentYaw += mouseX;
            }

            // Pitch (X axis, up/down)
            _currentPitch = Mathf.Clamp(_currentPitch - mouseY, _minPitchAngle, _maxPitchAngle);
            
            mainCamera.transform.localRotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
        }

        public void SetSensitivity(float sensitivity)
        {
            _mouseSensitivity = sensitivity;
        }

        public void EnableNavigation()
        {
            _currentDelayEnableTween?.Kill();
            _currentDelayEnableTween = DOVirtual.DelayedCall
            (0.5f,
                () =>
                {
                    _isEnabled = true;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            );
           
        }

        public void DisableNavigation()
        {
            _isEnabled = false;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        
        public void UpdateCameraPositionAndRotation(Vector3 newCameraPosition, Vector3 newCameraRotation)
        {
            _previousCameraRotation = mainCamera.transform.localRotation;
            
            mainCamera.transform.localPosition = newCameraPosition;
            mainCamera.transform.localRotation = Quaternion.Euler(newCameraRotation);
        }

        public void RestoreCameraPositionAndRotation()
        {
            TweenToPosition(_initialCameraPosition);
            TweenToRotation(Quaternion.identity);
        }

        public void TweenToPosition(Vector3 position, float duration = 2f, Action callback = null)
        {
            _currentMoveTween?.Kill();
            _currentMoveTween = mainCamera.transform
                .DOLocalMove(position, duration)
                .SetEase(Ease.Linear).OnComplete(
                    () => callback?.Invoke());
        }

        public void TweenToRotation(Quaternion rotation, float duration = 1f)
        {
            _currentRotationTween?.Kill();
            Vector3 targetEuler = rotation.eulerAngles;
            
            _currentRotationTween = mainCamera.transform
                .DOLocalRotate(rotation.eulerAngles, duration)
                .SetEase(Ease.Linear).OnComplete(() =>
                {
                    _currentPitch = Mathf.DeltaAngle(0f, targetEuler.x);
                    _currentYaw = Mathf.DeltaAngle(0f, targetEuler.y);
                });;
        }

        public void Focus()
        {
            _depthOfField.active = false;
        }

        public void Unfocus()
        {
            _depthOfField.active = true;
        }
    }

    public interface ICameraController
    {
        public Camera Camera { get; }
        public Vector3 OriginPosition { get; }
        public void HandleRotation();
        public void SetSensitivity(float sensitivity);
        public void EnableNavigation();
        public void DisableNavigation();
        public void UpdateCameraPositionAndRotation(Vector3 newCameraPosition, Vector3 newCameraRotation);
        public void RestoreCameraPositionAndRotation();
        public void TweenToPosition(Vector3 position, float duration = 10f, Action callback = null);
        public void TweenToRotation(Quaternion rotation, float duration = 1f);
        public void Focus();
        public void Unfocus();
    }
}