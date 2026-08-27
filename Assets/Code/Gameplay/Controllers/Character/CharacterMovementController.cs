using CorePatterns.ServiceLocator;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rulebound
{
    public class CharacterMovementController : MonoBehaviour, ICharacterMovementController
    {
        [Header("References")]
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private CapsuleCollider capsuleCollider;

        [Header("Input Actions")]
        [SerializeField] private InputActionSO moveActionSO;

        [Header("Movement")]
        [SerializeField] private float walkSpeed = 5f;

        private InputAction _moveAction;

        private Vector2 _movementInput;
        private bool _isMoving;
        private bool _isEnabled;
        
        private ICharacterSprintController _sprintController;
        private ICharacterJumpController _jumpController;

        private float _speed;
        private Vector3 _inputDirection;
        private Vector3 _move;
        
        public bool IsMoving => _isMoving;

        private void Awake()
        {
            InstallService();
        }

        private void Update()
        {
            if (!_isEnabled)
                return;
            
            UpdateMovementState();
        }

        private void FixedUpdate()
        {
            if (!_isEnabled)
                return;

            Move();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<ICharacterMovementController>(this);

            if (!rigidBody)
            {
                rigidBody = GetComponent<Rigidbody>();
            }

            rigidBody.freezeRotation = true;

            _moveAction = moveActionSO.GetInputAction();

            _moveAction.performed += HandleMovement;
            _moveAction.canceled += HandleMovement;
        }

        private void Start()
        {
            _sprintController = ServiceLocator.GetService<ICharacterSprintController>();
            _jumpController = ServiceLocator.GetService<ICharacterJumpController>();
            EnableController();
        }

        private void OnDestroy()
        {
            _moveAction.performed -= HandleMovement;
            _moveAction.canceled -= HandleMovement;
        }

        private void Move()
        {
            if (!_jumpController.IsGrounded)
            {
                rigidBody.MovePosition(rigidBody.position + _move);
                return;
            }
            
            _speed = GetMovementSpeed();
            _inputDirection = new Vector3(_movementInput.x, 0, _movementInput.y).normalized;
            _move = transform.TransformDirection(_inputDirection) * (_speed * Time.fixedDeltaTime);
            rigidBody.MovePosition(rigidBody.position + _move);
        }

        private float GetMovementSpeed()
        {
            return _sprintController.IsSprinting ? walkSpeed * _sprintController.SprintMultiplier : walkSpeed;
        }

        private void UpdateMovementState()
        {
            _isMoving = _movementInput.sqrMagnitude > 0.01f;
        }

        private void HandleMovement(InputAction.CallbackContext context)
        {
            _movementInput = context.ReadValue<Vector2>();
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

    public interface ICharacterMovementController
    {
        bool IsMoving { get; }
        void EnableController();
        void DisableController();
    }
}