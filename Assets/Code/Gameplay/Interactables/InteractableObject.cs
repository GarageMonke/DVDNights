using UnityEngine;

namespace DVDNights
{
    public abstract class InteractableObject : MonoBehaviour, IInteractableObject
    {
        [Header("Configuration")] 
        [SerializeField] private Outline outline;
        
        public abstract void Interact();

        public abstract void StopInteraction();

        private void Awake()
        {
            Unhighlight();
        }

        public void Highlight()
        {
            outline.enabled = true;
        }

        public void Unhighlight()
        {
            outline.enabled = false;
        }
    }

    public interface IInteractableObject
    {
        public void Interact();
        public void StopInteraction();
        public void Highlight();
        public void Unhighlight();
    }
}