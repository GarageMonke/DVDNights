using CorePatterns.ServiceLocator;

namespace DVDNights
{
    public class DVDTrayInteractableObject : InteractableObject
    {
        private IDVDTrayController _dvdTrayController;
        private ITVNavigationController _tvNavigationController;
        private ITVStateController _tvStateController;
        private DVDBoxInteractableObject _currentDvdBoxInteractableObject;

        public override string GetInteractionAction()
        {
            return _tvStateController.IsDiskOnTray ? "Close" : "Insert DVD";
        }

        public override void Interact()
        {
            if (_tvStateController.IsDiskOnTray)
            {
                _tvNavigationController.OnOpenCloseButtonPressed?.Invoke();
                DisableInteraction();
            }
            else
            {
                _currentDvdBoxInteractableObject.Interact();
            }
        }

        public override void StopInteraction()
        {
           //
        }
        
        protected override void Start()
        {
            base.Start();
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

        public void SetCurrentDvdBoxInteractableObject(DVDBoxInteractableObject currentDvdBoxInteractableObject)
        {
           _currentDvdBoxInteractableObject = currentDvdBoxInteractableObject;
        }

        private void OnDestroy()
        {
            _dvdTrayController.OnTrayOpened -= CheckInteractionStatus;
            _dvdTrayController.OnTrayClosed -= CheckInteractionStatus;
        }
    }
}