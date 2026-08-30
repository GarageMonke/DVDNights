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
        [SerializeField] private HeadBobSettings headBob = new();
        
        private float _yaw;
        private float _pitch;
        private float _bobTimer;
        private bool _cursorLocked = true;

        private float _originalYaw;

        private InputAction _lookAction;
        private bool _isEnabled;
        
        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            _yaw = bodyTransform ? bodyTransform.eulerAngles.y : 0f;
            _originalYaw = _yaw;

            _lookAction = lookActionSO.GetInputAction();
            
            SetCursorLocked(true);
            
            ServiceLocator.RegisterService<ICharacterLookController>(this);
        }

        private void Start()
        {
            EnableController();
        }

        private void LateUpdate()
        {
            if (!_isEnabled)
            {
                return;
            }
            
            ApplyLook();
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
        

        public void EnableController()
        {
            _isEnabled = true;
        }

        public void DisableController()
        {
            _isEnabled = false;
        }

        public void ResetController()
        {
            _yaw = _originalYaw;
            _pitch = 0;
            
            if (bodyTransform)
            {
                bodyTransform.rotation = Quaternion.Euler(0f, _originalYaw, 0f);
            }

            if (cameraPivot)
            {
                cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
            }
        }

        public void SetCursorLocked(bool locked)
        {
            _cursorLocked = locked;
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }

    public interface ICharacterLookController : ICharacterController
    {
        void SetCursorLocked(bool locked);
    }
}