using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class InspectableInteractableObject : InteractableObject
    {
        [Header("Configuration")]
        [SerializeField] private InspectableDataSO inspectableDataSO;
        
        private IInspectionController _inspectionController;
        
        private void Start()
        {
            _inspectionController = ServiceLocator.GetService<IInspectionController>();
        }

        public override string GetInteractionAction()
        {
            return "Inspect";
        }

        public override void Interact()
        {
           _inspectionController.Inspect(inspectableDataSO);
           AudioManager.Instance.PlaySFX(InteractionAudioClip, volume: 1f, pitch: 2.5f);
        }

        public override void StopInteraction()
        {
            _inspectionController.StopInspection();
            AudioManager.Instance.PlaySFX(InteractionAudioClip, volume: 1f, pitch: 1.5f);
        }
    }
}