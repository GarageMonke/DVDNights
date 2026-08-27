using CorePatterns.ServiceLocator;
using UnityEngine;

namespace Rulebound
{
    public class DiodeHazard : Hazard
    {
        [Header("Configuration")]
        [SerializeField] private Color dangerousColor;
        [SerializeField] private Color safeColor;
        
        private bool _isDangerous;
        private ICharacter _character;

        protected override void Start()
        {
            base.Start();
            _character = ServiceLocator.GetService<ICharacter>();
            _character.CharacterJumpController.OnJump += ToggleHazard;
            MakeDangerous();
        }

        private void ToggleHazard()
        {
            _isDangerous = !_isDangerous;

            if (_isDangerous)
            {
                MakeDangerous();
                return;
            }
            
            MakeSafe();
        }

        private void MakeDangerous()
        {
            hazardRenderer.material.color = dangerousColor;
            _isDangerous = true;
        }
        
        private void MakeSafe()
        {
            hazardRenderer.material.color = safeColor;
            _isDangerous = false;
        }

        protected override void OnCollisionEnter(Collision other)
        {
            if (!_isDangerous)
            {
                return;
            }
            
            base.OnCollisionEnter(other);
        }
        
        public override void ResetHazard()
        {
            MakeDangerous();
        }

        private void OnDestroy()
        {
            _character.CharacterJumpController.OnJump -= ToggleHazard;
        }
    }
}