using System;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

namespace DVDNights
{
    public class CameraController : MonoBehaviour, ICameraController
    {
        [Header("Camera Reference")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private InputActionSO zoomInputActionSO;
        [SerializeField] private Transform jumpScareSpot;

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

        private InputAction _zoomInputAction;

        public Camera Camera => mainCamera;
        public Vector3 OriginPosition => _initialCameraPosition;

        public bool IsNavigationEnabled => _isEnabled;
        public Transform JumpScareSpot => jumpScareSpot;

        private DepthOfField _depthOfField;
        private IInteractionController _interactionController;
        private Tweener _fovTween;
        private Tween _enableInteractionTween;

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

            _zoomInputAction = zoomInputActionSO.GetInputAction();
            _zoomInputAction.performed += ZoomIn;
            _zoomInputAction.canceled += ZoomOut;
            
            DisableNavigation();
            _initialCameraPosition = mainCamera.transform.localPosition;
            Vector3 startAngles = mainCamera.transform.eulerAngles;
            _currentPitch = startAngles.x;
            _currentYaw   = startAngles.y;

            ServiceLocator.RegisterService<ICameraController>(this);
        }

        private void Start()
        {
            _interactionController = ServiceLocator.GetService<IInteractionController>();
            _depthOfField = PostProcessingManager.Instance.GetVolumeComponent<DepthOfField>();
        }

        private void ZoomOut(InputAction.CallbackContext context)
        {
            if (_interactionController.IsInteracting)
            {
                return;
            }

            if (!_isEnabled)
            {
                return;
            }
            
            _enableInteractionTween?.Kill();
            _fovTween?.Kill();

            _fovTween = mainCamera
                .DOFieldOfView(60, 0.5f)
                .SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    _enableInteractionTween = DOVirtual.DelayedCall(0.25f,()=> _interactionController.EnableInteractions());
                });
        }

        private void ZoomIn(InputAction.CallbackContext context)
        {
            if (_interactionController.IsInteracting)
            {
                return;
            }
            
            if (!_isEnabled)
            {
                return;
            }
            
            _enableInteractionTween?.Kill();
            _interactionController.DisableInteractions();
            
            _fovTween = mainCamera
                .DOFieldOfView(25, 0.5f)
                .SetEase(Ease.InOutSine);
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

        public void WakeUpSequence()
        {
            DisableNavigation();

            Vector3 startRotation = new Vector3(-65f, 0f, 0f); // Looking up
            float startStrength = 3f;

            float totalDuration = 12f;
            float recoverStartTime = 6f;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(
                DOVirtual.Float(0f, totalDuration, totalDuration, t =>
                    {
                        float recovery = Mathf.Clamp01((t - recoverStartTime) / (totalDuration - recoverStartTime));
                        
                        Vector3 baseRotation = Vector3.Lerp(startRotation, Vector3.zero, recovery);
                        
                        float strength = Mathf.Lerp(startStrength, 0f, recovery);
                        
                        float pitch = baseRotation.x + Mathf.Sin(t * 1.10f) * strength * 0.25f;
                        float yaw   = baseRotation.y + Mathf.Sin(t * 0.63f) * strength;
                        float roll  = baseRotation.z + Mathf.Sin(t * 1.85f) * strength * 1.25f;

                        mainCamera.transform.localRotation = Quaternion.Euler(pitch, yaw, roll);
                    })
                    .SetEase(Ease.InOutSine)
            );

            sequence.AppendCallback(() =>
            {
                mainCamera.transform.localRotation = Quaternion.identity;

                _currentPitch = 0f;
                _currentYaw = 0f;

                EnableNavigation();
            });
        }

        private void OnDestroy()
        {
            _zoomInputAction.performed -= ZoomIn;
            _zoomInputAction.canceled -= ZoomOut;
        }
    }

    public interface ICameraController
    {
        public Camera Camera { get; }
        public Vector3 OriginPosition { get; }
        public bool IsNavigationEnabled { get; }
        public Transform JumpScareSpot { get; }
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
        public void WakeUpSequence();
    }
}