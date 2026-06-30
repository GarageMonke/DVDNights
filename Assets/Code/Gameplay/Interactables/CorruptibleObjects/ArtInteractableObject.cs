using UnityEngine;

namespace DVDNights
{
    public class ArtInteractableObject : CorruptibleInteractableObject
    {
        [Header("Art-Configuration")]
        [SerializeField] private ArtPictureType artPictureType;
        [SerializeField] private MeshRenderer artRenderer;

        private Material _originalMaterial;

        private void Awake()
        {
            _originalMaterial = artRenderer.material;
            DisableInteraction();
        }
        
        public override string GetInteractionAction()
        {
            return "Remove corruption";
        }

        public override void Interact()
        {
            if (_isCorrupted)
            {
                ClearCorruption();
            }
        }
        
        public override void Corrupt()
        {
            base.Corrupt();
            artRenderer.material = null;
            EnableInteraction();
        }

        public override void ClearCorruption()
        {
            base.ClearCorruption();
            DisableInteraction();
            artRenderer.material = _originalMaterial;
        }
        
    }
}