using CorePatterns.ServiceLocator;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rulebound
{
    public class CharacterSprintController : MonoBehaviour, ICharacterSprintController
    {
        [Header("Input Actions")]
        [SerializeField] private InputActionSO sprintActionSO;

        [Header("Movement")]
        [SerializeField] private float sprintMultiplier = 2.5f;

        [Header("Configuration")]
        [SerializeField] private bool useStamina = true;

        private InputAction _sprintAction;

        private bool _sprintInputHeld;
        private bool _isSprinting;
        private bool _isEnabled;

        private ICharacterStaminaController _staminaController;
        private ICharacterJumpController _jumpController;

        public bool CanSprint => !useStamina || _staminaController.HasStamina;
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
            _jumpController = ServiceLocator.GetService<ICharacterJumpController>();

            EnableController();
        }

        private void Update()
        {
            if (!_isEnabled)
                return;

            UpdateSprint();
        }

        private void UpdateSprint()
        {
            if (_jumpController.IsGrounded)
            {
                _isSprinting = _sprintInputHeld && CanSprint;
            }

            if (!useStamina || !_isSprinting)
            {
                return;
            }

            _staminaController.ConsumeStamina();
        }

        private void HandleSprint(InputAction.CallbackContext context)
        {
            _sprintInputHeld = context.ReadValue<float>() > 0;

            if (!_jumpController.IsGrounded)
            {
                return;
            }

            _isSprinting = _sprintInputHeld && CanSprint;
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
            _isSprinting = false;
        }

        public void ResetController()
        {
            _isSprinting = false;
        }
    }

    public interface ICharacterSprintController : ICharacterController
    {
        bool CanSprint { get; }
        bool IsSprinting { get; }
        float SprintMultiplier { get; }
    }
}