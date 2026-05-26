using UnityEngine;

namespace DVDNights
{
    public abstract class InteractableObject : MonoBehaviour, IInteractableObject
    {
        [Header("Configuration")] 
        [SerializeField] private Outline outline;
        [SerializeField] private InteractionData interactionData;
        
        public InteractionData InteractionData => interactionData;
        
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
        public InteractionData InteractionData { get; }
        public void Interact();
        public void StopInteraction();
        public void Highlight();
        public void Unhighlight();
    }
}