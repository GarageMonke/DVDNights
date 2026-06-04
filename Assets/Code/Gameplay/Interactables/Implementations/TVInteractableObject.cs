using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class TVInteractableObject : InteractableObject
    {
        [Header("Configuration")]
        [SerializeField] private Vector3 cameraLockPosition;

        ICameraController _cameraController;
        private ITVNavigationController _tvNavigationController;
        private IMouseLayoutController _mouseLayoutController;

        private void Start()
        {
            _cameraController = ServiceLocator.GetService<ICameraController>();
            _tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();
            _mouseLayoutController = ServiceLocator.GetService<IMouseLayoutController>();
        }

        public override string GetInteractionAction()
        {
            return "Play game";
        }

        public override void Interact()
        {
            Unhighlight();
            _cameraController.TweenToPosition(cameraLockPosition, 0.5f);
            _cameraController.TweenToRotation(Quaternion.identity, 0.5f);
            _tvNavigationController.EnableButtons();
            _mouseLayoutController.DisplayRegularLayout();
            AudioManager.Instance.PlaySFX(InteractionAudioClip, volume: 1f, pitch: 2.5f);
        }

        public override void StopInteraction()
        {
            Highlight();
            _tvNavigationController.DisableButtons();
            _cameraController.TweenToPosition(_cameraController.OriginPosition, 0.5f);
            _cameraController.TweenToRotation(Quaternion.identity, 0.5f);
            _mouseLayoutController.HideMouseLayout();
            AudioManager.Instance.PlaySFX(InteractionAudioClip, volume: 1f, pitch: 1.5f);
        }
    }
}