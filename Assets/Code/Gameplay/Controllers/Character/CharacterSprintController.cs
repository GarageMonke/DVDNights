using System;
using CorePatterns.ServiceLocator;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rulebound
{
    public class CharacterSprintController : MonoBehaviour, ICharacterSprintController
    {
        [Header("References")]
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private CapsuleCollider capsuleCollider;

        [Header("Input Actions")]
        [SerializeField] private InputActionSO sprintActionSO;

        [Header("Movement")]
        [SerializeField] private float sprintMultiplier = 2.5f;

        [Header("Configuration")] 
        [SerializeField] private bool useStamina = true;

        private InputAction _moveAction;
        private InputAction _sprintAction;

        private Vector2 _movementInput;
        private bool _isMoving;
        private bool _isSprinting;
        private bool _isEnabled;
        private bool _canSprint;
        
        private ICharacterStaminaController _staminaController;

        public bool CanSprint => _canSprint;
        public bool IsSprinting => _isSprinting;
        public float SprintMultiplier => sprintMultiplier;

        private void Awake()
        {
            InstallService();
        }
        
        private void InstallService()
        {
            ServiceLocator.RegisterService<ICharacterSprintController>(this);
            
            _sprintAction = sprintActionSO.GetInputAction();

            _sprintAction.performed += HandleSprint;
            _sprintAction.canceled += HandleSprint;
        }

        private void Start()
        {
            _staminaController = ServiceLocator.GetService<ICharacterStaminaController>();
            EnableController();
        }
        
        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            if (!useStamina)
            {
                return;
            }
            
            if (_isSprinting)
            {
                if (!_staminaController.HasStamina)
                {
                    _isSprinting = false;
                    return;
                }

                _staminaController.ConsumeStamina();
            }
        }

        private void HandleSprint(InputAction.CallbackContext context)
        {
            bool isTryingToSprint = context.ReadValue<float>() > 0;

            if (useStamina)
            {
                _isSprinting = isTryingToSprint && _staminaController.HasStamina;
            }
            else
            {
                _isSprinting = isTryingToSprint;
            }
        }

        private void OnDestroy()
        {
            _sprintAction.performed -= HandleSprint;
            _sprintAction.canceled -= HandleSprint;
        }

        public void EnableController()
        {
            _isEnabled = true;
        }

        public void DisableController()
        {
            _isEnabled = false;
        }
    }

    public interface ICharacterSprintController
    {
        public bool CanSprint { get; }
        public bool IsSprinting { get; }
        public float SprintMultiplier { get; }
        public void EnableController();
        public void DisableController();
    }
}