using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class DVDTrayInteractableObject : InteractableObject
    {
        [Header("References")]
        [SerializeField] private DVDBoxInteractableObject dvdBoxInteractableObject;
        
        private IDVDTrayController _dvdTrayController;
        private ITVNavigationController _tvNavigationController;
        private ITVStateController _tvStateController;

        public override string GetInteractionAction()
        {
            return _tvStateController.IsDiskOnTray ? "Close" : "Insert DVD";
        }

        public override void Interact()
        {
            if (_tvStateController.IsDiskOnTray)
            {
                _tvNavigationController.OnOpenCloseButtonPressed?.Invoke();
            }
            else
            {
                dvdBoxInteractableObject.Interact();
            }
        }

        public override void StopInteraction()
        {
           //
        }
        
        private void Start()
        {
            _dvdTrayController = ServiceLocator.GetService<IDVDTrayController>();
            _tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();
            _tvStateController = ServiceLocator.GetService<ITVStateController>();
            _dvdTrayController.OnTrayOpened += CheckInteractionStatus;
            _dvdTrayController.OnTrayClosed += CheckInteractionStatus;
            CheckInteractionStatus();
        }

        private void CheckInteractionStatus()
        {
            if (_dvdTrayController.IsTrayOpened)
            {
                EnableInteraction();
                IgnoreNavigation(true);
            }
            else
            {
                DisableInteraction();
                IgnoreNavigation(false);
            }
        }

        private void OnDestroy()
        {
            _dvdTrayController.OnTrayOpened -= CheckInteractionStatus;
            _dvdTrayController.OnTrayClosed -= CheckInteractionStatus;
        }
    }
}