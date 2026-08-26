using System;
using CorePatterns.ServiceLocator;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rulebound
{
    public class CharacterLookController : MonoBehaviour, ICharacterLookController
    {
         
        [Header("Input")]
        [SerializeField] private InputActionSO lookActionSO;

        [Header("References")] 
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private Transform cameraPivot;   
 
        [Header("Sensitivity")]
        [SerializeField] private float mouseSensitivity = 0.15f;
        [SerializeField] private float gamepadSensitivity = 120f;
        [SerializeField] private bool invertY;
 
        [Header("Pitch Clamp")]
        [SerializeField] private float minPitch = -85f;
        [SerializeField] private float maxPitch = 85f;
 
        [Header("Headbob")]
        [SerializeField] private HeadbobSettings headbob = new();
        
        private float _yaw;
        private float _pitch;
        private Vector3 _cameraRestLocalPos;
        private float _bobTimer;
        private bool _cursorLocked = true;

        private InputAction _lookAction;
        private bool _isEnabled;
        private ICharacterMovementController _characterMovementController;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            if (cameraPivot)
            {
                _cameraRestLocalPos = cameraPivot.localPosition;
            }

            _yaw = bodyTransform ? bodyTransform.eulerAngles.y : 0f;

            _lookAction = lookActionSO.GetInputAction();
            
            SetCursorLocked(true);
            
            ServiceLocator.RegisterService<ICharacterLookController>(this);
        }

        private void Start()
        {
            _characterMovementController = ServiceLocator.GetService<ICharacterMovementController>();
            EnableController();
        }

        private void LateUpdate()
        {
            if (!_isEnabled)
            {
                return;
            }
            
            ApplyLook();
            ApplyHeadbob();
        }

        private void ApplyLook()
        {
            Vector2 delta = _lookAction.ReadValue<Vector2>();

            if (delta.sqrMagnitude < 0.0001f)
            {
                return;
            }

            bool usingGamepad = _lookAction.activeControl != null && _lookAction.activeControl.device is Gamepad;
            float sens = usingGamepad ? gamepadSensitivity * Time.deltaTime : mouseSensitivity;

            _yaw += delta.x * sens;
            _pitch += (invertY ? delta.y : -delta.y) * sens;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

            if (bodyTransform)
            {
                bodyTransform.rotation = Quaternion.Euler(0f, _yaw, 0f);
            }

            if (cameraPivot)
            {
                cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
            }
        }
 
        private void ApplyHeadbob()
        {
            if (!headbob.enabled || !cameraPivot)
                return;

            if (_characterMovementController.IsMoving)
            {
                _bobTimer += Time.deltaTime * headbob.frequency;

                float bobY = Mathf.Sin(_bobTimer) * headbob.amplitude;
                float bobX = Mathf.Cos(_bobTimer * 0.5f) * headbob.amplitude;

                Vector3 targetPosition = _cameraRestLocalPos + new Vector3(bobX, bobY, 0f);

                cameraPivot.localPosition = Vector3.Lerp(
                    cameraPivot.localPosition,
                    targetPosition,
                    headbob.smoothing * Time.deltaTime);
            }
            else
            {
                cameraPivot.localPosition = Vector3.Lerp(
                    cameraPivot.localPosition,
                    _cameraRestLocalPos,
                    headbob.smoothing * Time.deltaTime);
            }
        }

        public void EnableController()
        {
            _isEnabled = true;
        }

        public void DisableController()
        {
            _isEnabled = false;
        }

        public void SetCursorLocked(bool locked)
        {
            _cursorLocked = locked;
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
 
    [Serializable]
    public class HeadbobSettings
    {
        public bool enabled = true;
        public float frequency = 8f;
        public float amplitude = 0.05f;
        public float smoothing = 10f;
    }

    public interface ICharacterLookController
    {
        public void EnableController();
        public void DisableController();
        void SetCursorLocked(bool locked);
    }
}