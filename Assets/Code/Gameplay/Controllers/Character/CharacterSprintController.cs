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

        private InputAction _moveAction;
        private InputAction _sprintAction;

        private Vector2 _movementInput;
        private bool _isMoving;
        private bool _isSprinting;
        private bool _isEnabled;
        private bool _canSprint;

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
            
            EnableController();
        }

        private void HandleSprint(InputAction.CallbackContext context)
        {
            _isSprinting = context.ReadValue<float>() > 0;
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