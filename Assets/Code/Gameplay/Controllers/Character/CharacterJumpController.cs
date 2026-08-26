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
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private float jumpHeight = 25f;
        [SerializeField] private float gravityMultiplier = 4f;
        
        private InputAction _jumpAction;
        
        private bool _isGrounded;
        
        private bool _isEnabled;

        public bool CanJump => _isGrounded;

        private void Awake()
        {
            InstallService();
        }

        private void FixedUpdate()
        {
            if (!_isEnabled)
            {
                return;
            }

            UpdateGrounded();
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
            
            _jumpAction.performed += OnJump;
            
            EnableController();
        }

        private void OnDestroy()
        {
            _jumpAction.performed -= OnJump;
        }
        
        
        private void UpdateGrounded()
        {
            Vector3 origin = transform.position + Vector3.up * 0.1f;
            _isGrounded = Physics.Raycast(origin, Vector3.down, capsuleCollider.height / 2f + 0.15f);
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

    public interface ICharacterJumpController
    {
        bool CanJump { get; }
        void EnableController();
        void DisableController();
    }
}