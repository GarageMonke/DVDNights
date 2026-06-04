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
        private ITrackSelectionController _trackSelectorController;

        private void Start()
        {
            _cameraController = ServiceLocator.GetService<ICameraController>();
            _trackSelectorController = ServiceLocator.GetService<ITrackSelectionController>();
        }

        public override string GetInteractionAction()
        {
            return "Select Track";
        }

        public override void Interact()
        {
            Unhighlight();
            _cameraController.TweenToPosition(cameraLockPosition, 0.5f, ()=> _trackSelectorController.OpenTrackSelector());
            _cameraController.TweenToRotation(Quaternion.Euler(cameraLockRotation), 0.5f);
            AudioManager.Instance.PlaySFX(InteractionAudioClip, volume: 1f, pitch: 2.5f);
        }

        public override void StopInteraction()
        {
            Highlight();
            _cameraController.TweenToPosition(_cameraController.OriginPosition, 0.5f);
            AudioManager.Instance.PlaySFX(InteractionAudioClip, volume: 1f, pitch: 1.5f);
            _trackSelectorController.CloseTrackSelector();
        }
    }
}