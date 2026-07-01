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
        
        private IArtCorruptionController _artCorruptionController;
        private Tweener _dissolveTween;

        private void Awake()
        {
            if (corruptedArtRenderer == null)
            {
                Debug.LogError("Object with null renderer " + gameObject.name);
            }
    
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
            DisplayCorruption();
            corruptedArtRenderer.material = new Material(_artCorruptionController.GetArtMaterialByType(artPictureType));
            EnableInteraction();
        }

        public override void ClearCorruption()
        {
            base.ClearCorruption();
            HideCorruption();
            DisableInteraction();
        }

        private void DisplayCorruption()
        {
            Fade(true);
        }

        private void HideCorruption()
        {
            Fade(false);
        }

        private void Fade(bool fadeIn)
        {
            _dissolveTween?.Kill();

            float targetValue = fadeIn ? 0f : 1f;
            float duration = fadeIn ? 1f : 3f;

            _dissolveTween = DOTween.To(
                    () =>  corruptedArtRenderer.material.GetFloat(Dissolve),
                    x =>  corruptedArtRenderer.material.SetFloat(Dissolve, x),
                    targetValue,
                    duration)
                .SetEase(Ease.InOutSine);
        }
        
    }
}