using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class ArtInteractableObject : CorruptibleInteractableObject
    {
        private static readonly int Dissolve = Shader.PropertyToID("_Dissolve");
        private static readonly int OutlineThickness = Shader.PropertyToID("_OutlineThickness");

        [Header("Art-Configuration")] 
        [SerializeField] private ArtPictureType artPictureType;

        [SerializeField] private MeshRenderer corruptedArtRenderer;


        protected IArtCorruptionController _artCorruptionController;
        private float _originalOutlineThickness;
        private Tweener _dissolveTween;

        protected virtual void Awake()
        {
            if (corruptedArtRenderer == null)
            {
                Debug.LogError("Object with null renderer " + gameObject.name);
            }

            _originalOutlineThickness = corruptedArtRenderer.material.GetFloat(OutlineThickness);

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
        }

        public override void ClearCorruption()
        {
            base.ClearCorruption();
            HideCorruption();
        }

        private void DisplayCorruption()
        {
            _dissolveTween?.Kill();
            float targetValue = 0f;
            float duration = 2f;
            
            corruptedArtRenderer.material = new Material(_artCorruptionController.GetArtMaterialByType(artPictureType));
            corruptedArtRenderer.material.SetFloat(OutlineThickness, 0f);

            _dissolveTween = DOTween.To(
                    () => corruptedArtRenderer.material.GetFloat(Dissolve),
                    x => corruptedArtRenderer.material.SetFloat(Dissolve, x),
                    targetValue,
                    duration)
                .SetEase(Ease.InOutSine).OnComplete(EnableInteraction);
        }

        private void HideCorruption()
        {
            DisableInteraction();
            _dissolveTween?.Kill();

            corruptedArtRenderer.material.SetFloat(OutlineThickness, _originalOutlineThickness);
            AudioClip fadeAudioClip = _artCorruptionController.GetCleanCorruptionAudioClip();

            float targetValue = 1f;
            float duration = fadeAudioClip.length;
            AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, fadeAudioClip, 0.75f);

            _dissolveTween = DOTween.To(
                    () => corruptedArtRenderer.material.GetFloat(Dissolve),
                    x => corruptedArtRenderer.material.SetFloat(Dissolve, x),
                    targetValue,
                    duration)
                .SetEase(Ease.InOutSine);
        }

    }
}