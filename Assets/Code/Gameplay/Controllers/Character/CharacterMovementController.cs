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
        [SerializeField] private InputActionSO jumpActionSO;
        [SerializeField] private InputActionSO sprintActionSO;

        [Header("Movement")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintMultiplier = 2.5f;
        
        [Header("Jumping")]
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float jumpHeight = 3f;
        [SerializeField] private float gravityMultiplier = 2.5f;

        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _sprintAction;

        private Vector2 _movementInput;
        private bool _isMoving;
        private bool _isSprinting;
        
        private bool _isGrounded;

        private Coroutine _crouchRoutine;
        private bool _isEnabled;

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
            UpdateGrounded();
            ApplyGravity();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<ICharacterMovementController>(this);

            if (!rigidBody)
            {
                rigidBody = GetComponent<Rigidbody>();
            }

            rigidBody.freezeRotation = true;
            rigidBody.useGravity = true;

            _moveAction = moveActionSO.GetInputAction();
            _jumpAction = jumpActionSO.GetInputAction();
            _sprintAction = sprintActionSO.GetInputAction();

            _moveAction.performed += HandleMovement;
            _moveAction.canceled += HandleMovement;

            _sprintAction.performed += HandleSprint;
            _sprintAction.canceled += HandleSprint;
            
            _jumpAction.performed += OnJump;
            
            EnableController();
        }

        private void HandleSprint(InputAction.CallbackContext context)
        {
            _isSprinting = context.ReadValue<float>() > 0;
        }

        private void OnDestroy()
        {
            _moveAction.performed -= HandleMovement;
            _moveAction.canceled -= HandleMovement;
            _jumpAction.performed -= OnJump;
            
            _sprintAction.performed -= HandleSprint;
            _sprintAction.canceled -= HandleSprint;
        }

        private void Move()
        {
            float speed = GetMovementSpeed();
            Vector3 inputDirection = new Vector3(_movementInput.x, 0, _movementInput.y).normalized;
            Vector3 move = transform.TransformDirection(inputDirection) * (speed * Time.fixedDeltaTime);
            rigidBody.MovePosition(rigidBody.position + move);
        }

        private float GetMovementSpeed()
        {
            return _isSprinting ? walkSpeed * sprintMultiplier : walkSpeed;
        }

        private void UpdateMovementState()
        {
            _isMoving = _movementInput.sqrMagnitude > 0.01f;
        }

        private void UpdateGrounded()
        {
            Vector3 origin = transform.position + Vector3.up * 0.1f;
            _isGrounded = Physics.Raycast(origin, Vector3.down, capsuleCollider.height / 2f + 0.15f);
        }

        private void HandleMovement(InputAction.CallbackContext context)
        {
            _movementInput = context.ReadValue<Vector2>();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (!_isGrounded)
            {
                return;
            }
            
            Vector3 velocity = rigidBody.linearVelocity;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);

            rigidBody.linearVelocity = velocity;
        }
        
        private void ApplyGravity()
        {
            if (_isGrounded)
            {
                return;
            }

            rigidBody.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);
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