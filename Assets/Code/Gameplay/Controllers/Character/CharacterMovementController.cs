using CorePatterns.Managers;
using CorePatterns.Providers.Implementations;
using CorePatterns.ServiceLocator;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rulebound
{
    public class CharacterMovementController : MonoBehaviour, ICharacterMovementController
    {
        [Header("References")]
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private AudioClipProvider footstepAudioClipProvider;

        [Header("Input Actions")]
        [SerializeField] private InputActionSO moveActionSO;

        [Header("Movement")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float walkStepInterval = 0.30f;
        [SerializeField] private float sprintStepInterval = 0.15f;
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

        private float _stepTimer;

        private AudioClip _lastFootstepClip;
        
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

            footstepAudioClipProvider.InitializeProvider();
            
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

            if (_movementInput.sqrMagnitude > 0.01f)
            {
                _stepTimer += Time.fixedDeltaTime;

                float stepInterval = _sprintController.IsSprinting
                    ? sprintStepInterval
                    : walkStepInterval;

                if (_stepTimer >= stepInterval)
                {
                    _stepTimer = 0f;
                    AudioClip selectedClip = footstepAudioClipProvider.GetRandomElement();

                    while (selectedClip == _lastFootstepClip)
                    {
                        selectedClip = footstepAudioClipProvider.GetRandomElement();
                    }

                    _lastFootstepClip = selectedClip;
                    
                    AudioManager.Instance.PlaySFX(AudioChannelType.TVWORLD, _lastFootstepClip, volume: 0.5f);
                }
            }
            else
            {
                _stepTimer = 0f;
            }

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