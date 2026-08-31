using CorePatterns.ServiceLocator;
using UnityEngine;

namespace Rulebound
{
    public class DiodePlatform : Platform
    {
        [Header("Configuration")]
        [SerializeField] protected Color invisibleColor; 
        
        private ICharacter _character;

        private bool _isVisible;

        protected override void Start()
        {
            base.Start();
            _character = ServiceLocator.GetService<ICharacter>();
            _character.CharacterJumpController.OnJump += TogglePlatform;
            _isVisible = true;
            MakeVisible();
        }

        private void TogglePlatform()
        {
            _isVisible = !_isVisible;

            if (_isVisible)
            {
                MakeVisible();
                return;
            }
            
            MakeInvisible();
        }

        protected virtual void MakeVisible()
        {
            platformRenderer.material.color = platformColor;
            platformCollider.enabled = true;
        }

        protected virtual void MakeInvisible()
        {
            platformRenderer.material.color = invisibleColor;
            platformCollider.enabled = false;
        }

        public override void ResetPlatform()
        {
            MakeVisible();
        }

        private void OnDestroy()
        {
            if (_character != null)
            {
                _character.CharacterJumpController.OnJump -= TogglePlatform;
            }
        }
    }
}