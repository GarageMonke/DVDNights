using System;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace Rulebound
{
    public abstract class InteractableObject : MonoBehaviour, IInteractableObject
    {
        [Header("Configuration")] 
        [SerializeField] private bool hasNavigation;
        [SerializeField] private bool registerForTutorial;

        [SerializeField] private bool _isEnabled = true;
        [SerializeField] private bool _hasCrossHairHint = true;

        [Header("References")] 
        [SerializeField] private Outline outline;

        [Header("Feedback")] 
        [SerializeField] private AudioClip interactionAudioClip;

        private IOutlineController _outlineController;
        private IInteractableController _interactableController;
        private bool _hasIgnoreNavigation;

        public bool HasNavigation => hasNavigation;
        public AudioClip InteractionAudioClip => interactionAudioClip;
        public Action OnInteractionPerformed { get; set; }
        public string InteractableId => gameObject.name;
        public bool IsEnabled => _isEnabled;
        public bool HasIgnoreNavigation => _hasIgnoreNavigation;
        public bool HasCrossHairHint => _hasCrossHairHint;
        
        protected virtual void Start()
        {
            if (outline)
            {
                _outlineController = ServiceLocator.GetService<IOutlineController>();
                _outlineController.RegisterOutline(outline);
            }

            if (registerForTutorial)
            {
                RegisterObjectForTutorial();
            }
        }

        protected void RegisterObjectForTutorial()
        {
            _interactableController = ServiceLocator.GetService<IInteractableController>();
            _interactableController.RegisterInteractable(this);
        }
        
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

        public virtual void StopInteraction()
        {
            //
        }

        private void Awake()
        {
            Unhighlight();
        }
        

        public virtual void Highlight()
        {
            if (!outline)
            {
                return;
            }
            
            outline.enabled = true;
        }

        public virtual void Unhighlight()
        {
            if (!outline)
            {
                return;
            }
            
            outline.enabled = false;
        }
    }

    public interface IInteractableObject
    {
        public string InteractableId { get; }
        public bool IsEnabled { get; }
        public bool HasIgnoreNavigation { get; }
        public bool HasCrossHairHint { get; }
        public bool HasNavigation { get; }
        public AudioClip InteractionAudioClip { get; }
        public Action OnInteractionPerformed { get; set; }
        public void Interact();
        public void StopInteraction();
        public void Highlight();
        public void Unhighlight();
        public void EnableInteraction();
        public void DisableInteraction();
        public string GetInteractionAction();
        public void IgnoreNavigation(bool ignore);
        public void SetHasNavigation(bool hasNavigation);
    }
}