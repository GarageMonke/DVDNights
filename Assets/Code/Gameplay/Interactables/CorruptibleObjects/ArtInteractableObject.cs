using CorePatterns.ServiceLocator;
using TossBoss.Providers.Engine.Implementations.Materials;
using UnityEngine;

namespace DVDNights
{
    public class ArtInteractableObject : CorruptibleInteractableObject
    {
        [Header("Art-Configuration")]
        [SerializeField] private ArtPictureType artPictureType;
        [SerializeField] private MeshRenderer artRenderer;

        private Material _originalMaterial;
        private IArtCorruptionController _artCorruptionController;

        private void Awake()
        {
            _originalMaterial = artRenderer.material;
            DisableInteraction();
        }

        protected override void Start()
        {
            base.Start();
            _artCorruptionController = ServiceLocator.GetService<IArtCorruptionController>();
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
            artRenderer.material = _artCorruptionController.GetArtMaterialByType(artPictureType);
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