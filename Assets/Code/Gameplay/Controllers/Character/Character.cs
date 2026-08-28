using System;
using CorePatterns.ServiceLocator;
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
        [SerializeField] private CharacterHeadBobController _characterHeadBobController;
        
        public Transform CharacterTransform => transform;
        public Transform OriginalParent => _originalParent;
        public ICharacterMovementController CharacterMovementController => _characterMovementController;
        public ICharacterJumpController CharacterJumpController => _characterJumpController;
        public ICharacterLookController CharacterLookController => _characterLookController;
        public ICharacterSprintController CharacterSprintController => _characterSprintController;
        public ICharacterStaminaController CharacterStaminaController => _characterStaminaController;
        public ICharacterHeadBobController CharacterHeadBobController => _characterHeadBobController;

        private Transform _originalParent;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<ICharacter>(this);
            _originalParent = transform.parent;
        }

        public void ResetCharacter()
        {
            _characterMovementController.ResetController();
            _characterJumpController.ResetController();
            _characterLookController.ResetController();
            _characterSprintController.ResetController();
            _characterStaminaController.ResetController();
            _characterHeadBobController.ResetController();
        }
    }
    
    public interface ICharacter
    {
        public Transform CharacterTransform { get; }
        public Transform OriginalParent { get; }
        
        public ICharacterMovementController CharacterMovementController { get; }
        public ICharacterJumpController CharacterJumpController { get; }
        public ICharacterLookController CharacterLookController { get; }
        public ICharacterSprintController CharacterSprintController { get; }
        public ICharacterStaminaController CharacterStaminaController { get; }
        public ICharacterHeadBobController CharacterHeadBobController { get; }
        

        public void ResetCharacter();
    }
}