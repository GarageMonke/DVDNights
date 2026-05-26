using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class ArtInteractableObject : InteractableObject
    {
        [Header("Configuration")]
        [SerializeField] private InspectableDataSO inspectableDataSO;
        
        private IInspectionController _inspectionController;

        private void Start()
        {
            _inspectionController = ServiceLocator.GetService<IInspectionController>();
        }

        public override void Interact()
        {
           _inspectionController.Inspect(inspectableDataSO);
        }

        public override void StopInteraction()
        {
            _inspectionController.StopInspection();
        }
    }
}