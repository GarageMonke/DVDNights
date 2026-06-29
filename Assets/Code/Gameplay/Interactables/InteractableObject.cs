using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public abstract class InteractableObject : MonoBehaviour, IInteractableObject
    {
        [Header("Configuration")] [SerializeField]
        private bool hasNavigation;

        [SerializeField] private bool _isEnabled = true;

        [Header("References")] [SerializeField]
        private Outline outline;

        [Header("Feedback")] [SerializeField] private AudioClip interactionAudioClip;

        private IOutlineController _outlineController;
        private bool _hasIgnoreNavigation;

        public bool HasNavigation => hasNavigation;
        public AudioClip InteractionAudioClip => interactionAudioClip;
        public bool IsEnabled => _isEnabled;
        public bool HasIgnoreNavigation => _hasIgnoreNavigation;

        public void EnableInteraction()
        {
            _isEnabled = true;
        }

        public void DisableInteraction()
        {
            _isEnabled = false;
            Unhighlight();
        }

        public abstract string GetInteractionAction();

        public void IgnoreNavigation(bool ignore)
        {
            _hasIgnoreNavigation = ignore;
        }

        public void SetHasNavigation(bool overrideHasNavigation)
        {
            hasNavigation = overrideHasNavigation;
        }

        public abstract void Interact();

        public abstract void StopInteraction();

        private void Awake()
        {
            Unhighlight();
        }

        protected virtual void Start()
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
        public bool IsEnabled { get; }
        public bool HasIgnoreNavigation { get; }
        public void Interact();
        public void StopInteraction();
        public void Highlight();
        public void Unhighlight();
        public bool HasNavigation { get; }
        public AudioClip InteractionAudioClip { get; }
        public void EnableInteraction();
        public void DisableInteraction();
        public string GetInteractionAction();
        public void IgnoreNavigation(bool ignore);
        public void SetHasNavigation(bool hasNavigation);
    }
}