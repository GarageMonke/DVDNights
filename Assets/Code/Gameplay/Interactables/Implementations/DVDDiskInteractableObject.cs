using UnityEngine;

namespace DVDNights
{
    public class DVDDiskInteractableObject : InteractableObject
    {
        [Header("Configuration")] 
        [SerializeField] private int diskId = 1;
        
        public int DiskId => diskId;
        
        public override string GetInteractionAction()
        {
            return "Pick up DVD";
        }

        public override void Interact()
        {
            
        }

        public override void StopInteraction()
        {
            
        }

      
    }
}