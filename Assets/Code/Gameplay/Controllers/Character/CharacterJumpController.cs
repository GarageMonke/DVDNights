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

        [Header("Stamina")]
        [SerializeField] private bool useStamina = true;
        [SerializeField] private float staminaToConsume = 20f;

        private InputAction _jumpAction;

        public Action OnJump { get; set; }

        private bool _isGrounded;
        private bool _wasGrounded;

        private float _lastGroundedTime;
        private float _lastJumpPressedTime = Mathf.NegativeInfinity;

        private bool HasCoyoteTime => !_isGrounded &&
                                      _wasGrounded &&
                                      Time.time - _lastGroundedTime <= coyoteTime;

        private bool HasBufferedJump => Time.time - _lastJumpPressedTime <= jumpBufferTime;

        private bool _isEnabled;

        private ICharacterStaminaController _staminaController;

        public bool IsGrounded => _isGrounded;

        public bool CanJump()
        {
            bool baseJumpCondition = (_isGrounded || HasCoyoteTime) && _isEnabled;

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

            Vector3 origin = transform.position + Vector3.up * 0.1f;

            _isGrounded = Physics.Raycast(origin, Vector3.down, capsuleCollider.height / 2f + 0.15f);

            if (_isGrounded)
            {
                _lastGroundedTime = Time.time;
            }
            
            if (_wasGrounded && !_isGrounded)
            {
                Debug.Log($"LEFT GROUND - Coyote started at {Time.time}");
            }
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            if (!_isEnabled)
            {
                return;
            }
            
            _lastJumpPressedTime = Time.time;

            Debug.Log(
                $"JUMP PRESSED | Grounded: {_isGrounded} | " +
                $"Coyote: {HasCoyoteTime} | " +
                $"Time Since Grounded: {Time.time - _lastGroundedTime:F3}");
            
            if (CanJump())
            {
                PerformJump();
            }
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

            PerformJump();
        }

        private void PerformJump()
        {
            if (useStamina &&
                _staminaController.CurrentStamina < staminaToConsume)
            {
                return;
            }

            if (useStamina)
            {
                _staminaController.ConsumeStamina(staminaToConsume);
            }

            Vector3 velocity = rigidBody.linearVelocity;

            velocity.y = Mathf.Sqrt(
                jumpHeight * -2f * Physics.gravity.y
            );

            rigidBody.linearVelocity = velocity;
            
            _lastJumpPressedTime = Mathf.NegativeInfinity;
            
            _lastGroundedTime = Mathf.NegativeInfinity;
            
            _isGrounded = false;
            _wasGrounded = false;

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
            _lastGroundedTime = Mathf.NegativeInfinity;
        }

        public void ResetController()
        {
            _lastGroundedTime = Time.time;
            _lastJumpPressedTime = Mathf.NegativeInfinity;
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
