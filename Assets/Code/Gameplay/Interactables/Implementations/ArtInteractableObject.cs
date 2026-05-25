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

        private void Awake()
        {
            _artPreviewWindow = artPreviewWindow;
        }

        public override void Interact()
        {
            _artPreviewWindow.Display();
            _artPreviewWindow.UpdateWindow(artDataSO);
        }

        public override void StopInteraction()
        {
            _artPreviewWindow.Hide();
        }
    }
}