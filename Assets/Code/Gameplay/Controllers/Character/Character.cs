using UnityEngine;

namespace Rulebound
{
    public class Character : MonoBehaviour, ICharacter
    {
        [Header("Controllers")]
        [SerializeField] private CharacterMovementController _characterMovementController;
        [SerializeField] private CharacterJumpController _characterJumpController;
        [SerializeField] private CharacterLookController _characterLookController;
        [SerializeField] private CharacterSprintController _characterSprintController;
        [SerializeField] private CharacterStaminaController _characterStaminaController;
        
        public Transform CharacterTransform => transform;
        public ICharacterMovementController CharacterMovementController => _characterMovementController;
        public ICharacterJumpController CharacterJumpController => _characterJumpController;
        public ICharacterLookController CharacterLookController => _characterLookController;
        public ICharacterSprintController CharacterSprintController => _characterSprintController;
        public ICharacterStaminaController CharacterStaminaController => _characterStaminaController;


        public void ResetCharacter()
        {
            _characterMovementController.ResetController();
            _characterJumpController.ResetController();
            _characterLookController.ResetController();
            _characterSprintController.ResetController();
            _characterStaminaController.ResetController();
        }
    }
    
    public interface ICharacter
    {
        public Transform CharacterTransform { get; }
        
        public ICharacterMovementController CharacterMovementController { get; }
        public ICharacterJumpController CharacterJumpController { get; }
        public ICharacterLookController CharacterLookController { get; }
        public ICharacterSprintController CharacterSprintController { get; }
        public ICharacterStaminaController CharacterStaminaController { get; }
        

        public void ResetCharacter();
    }
}