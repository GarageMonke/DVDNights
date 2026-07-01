using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class ArtInteractableObject : CorruptibleInteractableObject
    {
        private static readonly int Dissolve = Shader.PropertyToID("_Dissolve");

        [Header("Art-Configuration")]
        [SerializeField] private ArtPictureType artPictureType;
        [SerializeField] private MeshRenderer corruptedArtRenderer;

        private Material _corruptionMaterialInstance;
        private IArtCorruptionController _artCorruptionController;
        private Tweener _dissolveTween;

        private void Awake()
        {
            _corruptionMaterialInstance = corruptedArtRenderer.material;
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
            corruptedArtRenderer.material = _artCorruptionController.GetArtMaterialByType(artPictureType);
            EnableInteraction();
        }

        public override void ClearCorruption()
        {
            base.ClearCorruption();
            DisableInteraction();
            corruptedArtRenderer.material = _corruptionMaterialInstance;
        }

        private void DisplayCorruption()
        {
            
        }

        private void HideCorruption()
        {
            
        }
        
        public void Fade(bool fadeIn)
        {
            _dissolveTween?.Kill();

            float targetValue = fadeIn ? 0f : 1f;

            _dissolveTween = DOTween.To(
                    () => _corruptionMaterialInstance.GetFloat(Dissolve),
                    x => _corruptionMaterialInstance.SetFloat(Dissolve, x),
                    targetValue,
                    1f)
                .SetEase(Ease.InOutSine);
        }
        
    }
}