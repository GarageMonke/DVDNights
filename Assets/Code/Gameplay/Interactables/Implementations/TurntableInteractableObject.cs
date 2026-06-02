using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class TurntableInteractableObject : InteractableObject
    {
        [Header("Configuration")]
        [SerializeField] private Vector3 cameraLockPosition;
        [SerializeField] private Vector3 cameraLockRotation;

        ICameraController _cameraController;
        private IMouseLayoutController _mouseLayoutController;

        private void Start()
        {
            _cameraController = ServiceLocator.GetService<ICameraController>();
            _mouseLayoutController = ServiceLocator.GetService<IMouseLayoutController>();
        }

        public override void Interact()
        {
            Unhighlight();
            _cameraController.TweenToPosition(cameraLockPosition, 0.5f);
            _cameraController.TweenToRotation(Quaternion.Euler(cameraLockRotation), 0.5f);
            _mouseLayoutController.DisplayRegularLayout();
            AudioManager.Instance.PlaySFX(InteractionAudioClip, volume: 1f, pitch: 2.5f);
        }

        public override void StopInteraction()
        {
            Highlight();
            _cameraController.TweenToPosition(_cameraController.OriginPosition, 0.5f);
            _mouseLayoutController.HideMouseLayout();
            AudioManager.Instance.PlaySFX(InteractionAudioClip, volume: 1f, pitch: 1.5f);
        }
    }
}