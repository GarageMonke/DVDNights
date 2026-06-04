using System;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public abstract class InteractableObject : MonoBehaviour, IInteractableObject
    {
        [Header("Configuration")] 
        [SerializeField] private bool hasNavigation;
        
        [Header("References")] 
        [SerializeField] private Outline outline;
        
        [Header("Feedback")]
        [SerializeField] private AudioClip interactionAudioClip;

        private IOutlineController _outlineController;

        public bool HasNavigation => hasNavigation;
        public AudioClip InteractionAudioClip => interactionAudioClip;
        public abstract string GetInteractionAction();

        public abstract void Interact();

        public abstract void StopInteraction();

        private void Awake()
        {
            Unhighlight();
        }

        private void Start()
        {
            _outlineController = ServiceLocator.GetService<IOutlineController>();
            _outlineController.RegisterOutline(outline);
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
        public bool HasNavigation { get; }
        public AudioClip InteractionAudioClip { get; }

        public string GetInteractionAction();
    }
}