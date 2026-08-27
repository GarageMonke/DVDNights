using System;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace Rulebound
{
    public class DiodePlatform : Platform
    {
        [Header("References")] 
        [SerializeField] private MeshRenderer platformRenderer;
        [SerializeField] private Collider platformCollider;
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

        private void MakeVisible()
        {
            platformRenderer.enabled = true;
            platformCollider.enabled = true;
        }

        private void MakeInvisible()
        {
            platformRenderer.enabled = false;
            platformCollider.enabled = false;
        }

        public override void ResetPlatform()
        {
            MakeVisible();
        }

        private void OnDestroy()
        {
            _character.CharacterJumpController.OnJump -= TogglePlatform;
        }
    }
}