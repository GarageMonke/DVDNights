using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class ArtInteractableObject : InteractableObject
    {
        [Header("View")]
        [SerializeField] private ArtPreviewWindow artPreviewWindow;
        
        [Header("Configuration")]
        [SerializeField] private ArtDataSO artDataSO;
        
        IArtPreviewWindow _artPreviewWindow;
        private IDialogController _dialogController;

        private void Start()
        {
            _artPreviewWindow = artPreviewWindow;
            _dialogController = ServiceLocator.GetService<IDialogController>();
        }

        public override void Interact()
        {
            _dialogController.DisplayDialog("Artist: " + artDataSO.ArtistName);
        }

        public override void StopInteraction()
        {
            _dialogController.HideDialog();
        }
    }
}