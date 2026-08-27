using CorePatterns.ServiceLocator;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rulebound
{
    public class CharacterMovementController : MonoBehaviour, ICharacterMovementController
    {
        [Header("References")]
        [SerializeField] private Rigidbody rigidBody;

        [Header("Input Actions")]
        [SerializeField] private InputActionSO moveActionSO;

        [Header("Movement")]
        [SerializeField] private float walkSpeed = 5f;

        [Header("Air Steering")]
        [SerializeField] private float airSteering = 1f;

        private InputAction _moveAction;

        private Vector2 _movementInput;
        private bool _isMoving;
        private bool _isEnabled;
        
        private ICharacterSprintController _sprintController;
        private ICharacterJumpController _jumpController;

        private float _speed;
        private Vector3 _inputDirection;
        private Vector3 _move;

        private Quaternion _lastRotation;
        
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

            _lastRotation = transform.rotation;
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
                ApplyAirSteering();
                rigidBody.MovePosition(rigidBody.position + _move);
                _lastRotation = transform.rotation;
                return;
            }
            
            _speed = GetMovementSpeed();
            _inputDirection = new Vector3(_movementInput.x, 0, _movementInput.y).normalized;
            _move = transform.TransformDirection(_inputDirection) * (_speed * Time.fixedDeltaTime);
            _lastRotation = transform.rotation;
            rigidBody.MovePosition(rigidBody.position + _move);
        }

        private void ApplyAirSteering()
        {
            Quaternion rotationDelta = transform.rotation * Quaternion.Inverse(_lastRotation);
            rotationDelta = Quaternion.Euler(0f, rotationDelta.eulerAngles.y, 0f);
            Quaternion steeringRotation = Quaternion.Slerp(Quaternion.identity, rotationDelta, airSteering);
            _move = steeringRotation * _move;
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

        public void ResetController()
        {
            _isMoving = false;
            _move = Vector3.zero;
            _lastRotation = transform.rotation;
        }
    }

    public interface ICharacterMovementController : ICharacterController
    {
        bool IsMoving { get; }
    }
}