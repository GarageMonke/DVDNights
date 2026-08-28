using System;
using CorePatterns.ServiceLocator;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rulebound
{
    public class CharacterJumpController : MonoBehaviour, ICharacterJumpController
    {
        [Header("References")]
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private CapsuleCollider capsuleCollider;

        [Header("Input Actions")]
        [SerializeField] private InputActionSO jumpActionSO;

        [Header("Jumping")]
        [SerializeField] private float jumpHeight = 25f;
        [SerializeField] private float gravityMultiplier = 4f;
        [SerializeField] private float coyoteTime = 0.15f;
        [SerializeField] private float jumpBufferTime = 0.15f;
        [SerializeField] private float coyoteJumpMultiplier = 2f;

        [Header("Stamina")]
        [SerializeField] private bool useStamina = true;
        [SerializeField] private float staminaToConsume = 20f;

        private InputAction _jumpAction;

        public Action OnJump { get; set; }

        private bool _isGrounded;
        private bool _wasGrounded;
        private bool _coyoteTimeAvailable;
        private bool _jumpConsumed;

        private float _lastGroundedTime;
        private float _lastJumpPressedTime = Mathf.NegativeInfinity;

        private bool _isEnabled;

        private ICharacterStaminaController _staminaController;

        public bool IsGrounded => _isGrounded;

        private bool HasCoyoteTime => !_isGrounded &&
                                      _coyoteTimeAvailable &&
                                      Time.time - _lastGroundedTime <= coyoteTime;

        private bool HasBufferedJump => Time.time - _lastJumpPressedTime <= jumpBufferTime;

        public bool CanJump()
        {
            if (!_isEnabled || _jumpConsumed)
            {
                return false;
            }

            bool baseJumpCondition = _isGrounded || HasCoyoteTime;

            if (useStamina)
            {
                return baseJumpCondition && _staminaController.CurrentStamina >= staminaToConsume;
            }

            return baseJumpCondition;
        }

        private void Awake()
        {
            InstallService();
        }

        private void Start()
        {
            _staminaController = ServiceLocator.GetService<ICharacterStaminaController>();

            EnableController();
        }

        private void FixedUpdate()
        {
            if (!_isEnabled)
            {
                return;
            }

            UpdateGrounded();
            TryBufferedJump();
            ApplyGravity();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<ICharacterJumpController>(this);

            if (!rigidBody)
            {
                rigidBody = GetComponent<Rigidbody>();
            }

            rigidBody.useGravity = true;

            _jumpAction = jumpActionSO.GetInputAction();
            _jumpAction.performed += OnJumpPerformed;
        }

        private void OnDestroy()
        {
            if (_jumpAction != null)
            {
                _jumpAction.performed -= OnJumpPerformed;
            }
        }

        private void UpdateGrounded()
        {
            _wasGrounded = _isGrounded;
            
            if (_jumpConsumed && rigidBody.linearVelocity.y > 0.01f)
            {
                _isGrounded = false;
                return;
            }

            Vector3 origin = transform.position + Vector3.up * 0.1f;

            _isGrounded = Physics.Raycast(origin, Vector3.down, capsuleCollider.height / 2f + 0.15f
            );

            if (_isGrounded)
            {
                _jumpConsumed = false;
                _coyoteTimeAvailable = false;
                _lastGroundedTime = Time.time;
            }
            else if (_wasGrounded)
            {
                _coyoteTimeAvailable = true;
                _lastGroundedTime = Time.time;
            }

            if (_coyoteTimeAvailable &&
                Time.time - _lastGroundedTime > coyoteTime)
            {
                _coyoteTimeAvailable = false;
            }
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            if (!_isEnabled)
            {
                return;
            }
            
            _lastJumpPressedTime = Time.time;

            if (!CanJump())
            {
                return;
            }

            bool isCoyoteJump = !_isGrounded && HasCoyoteTime;

            PerformJump(isCoyoteJump);
        }

        private void TryBufferedJump()
        {
            if (!HasBufferedJump)
            {
                return;
            }

            if (!_isGrounded)
            {
                return;
            }

            if (!CanJump())
            {
                return;
            }

            PerformJump(false);
        }

        private void PerformJump(bool isCoyoteJump)
        {
            if (useStamina && _staminaController.CurrentStamina < staminaToConsume)
            {
                return;
            }

            if (useStamina)
            {
                _staminaController.ConsumeStamina(staminaToConsume);
            }

            Vector3 velocity = rigidBody.linearVelocity;

            float jumpMultiplier = isCoyoteJump
                ? coyoteJumpMultiplier
                : 1f;

            velocity.y = Mathf.Sqrt(
                jumpHeight *
                jumpMultiplier *
                -2f *
                Physics.gravity.y
            );

            rigidBody.linearVelocity = velocity;
            
            _jumpConsumed = true;
            
            _lastJumpPressedTime = Mathf.NegativeInfinity;
            _coyoteTimeAvailable = false;

            _isGrounded = false;

            OnJump?.Invoke();
        }

        private void ApplyGravity()
        {
            if (_isGrounded)
            {
                return;
            }

            if (rigidBody.linearVelocity.y > 0)
            {
                rigidBody.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);
            }
            else
            {
                rigidBody.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
            }
        }

        public void EnableController()
        {
            _isEnabled = true;
        }

        public void DisableController()
        {
            _isEnabled = false;

            _lastJumpPressedTime = Mathf.NegativeInfinity;
            _coyoteTimeAvailable = false;
            _jumpConsumed = false;
        }

        public void ResetController()
        {
            _lastGroundedTime = Time.time;
            _lastJumpPressedTime = Mathf.NegativeInfinity;
            _coyoteTimeAvailable = false;
            _jumpConsumed = false;
            _isGrounded = true;
            _wasGrounded = true;
        }
    }

    public interface ICharacterJumpController : ICharacterController
    {
        public Action OnJump { get; set; }
        public bool IsGrounded { get; }
        public bool CanJump();
    }
}

