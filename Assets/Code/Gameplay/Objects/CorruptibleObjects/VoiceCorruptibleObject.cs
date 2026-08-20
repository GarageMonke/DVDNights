using CorePatterns.Managers;
using CorePatterns.Providers.Implementations;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace DVDNights
{
    public class VoiceCorruptibleObject : CorruptibleObject
    {
        [Header("References")] 
        [SerializeField] private AudioClipProvider voicesProvider;
        [SerializeField] private FadeInOutBlack fadeInOutBlack;
        [SerializeField] private TurntableInteractableObject turntableInteractableObject;

        private ITrackSelectionController _trackSelectorController;
        private ICameraController _cameraController;
        private IInteractionController _interactionController;
        private IStimuliController _stimuliController;
        private IInteractableController _interactableController;
        
        private DepthOfField _depthOfField;
        private LensDistortion _lensDistortion;
        private PaniniProjection _paniniProjection;
        private ChromaticAberration _chromaticAberration;
        private Tweener _depthOfFieldTweener;
        private float _originalDoF;
        private Sequence _lensDistortionSequence;
        private Sequence _depthOfFieldSequence;
        private MotionBlur _motionBlur;
        private FilmGrain _filmGrain;


        protected override void Start()
        {
            base.Start();
            _trackSelectorController = ServiceLocator.GetService<ITrackSelectionController>();
            _cameraController = ServiceLocator.GetService<ICameraController>();
            _interactionController = ServiceLocator.GetService<IInteractionController>();
            _interactableController = ServiceLocator.GetService<IInteractableController>();
            
            _stimuliController = ServiceLocator.GetService<IStimuliController>();
            _stimuliController.OnAnxietyTriggered += Corrupt;
            
            voicesProvider.InitializeProvider();
            
            _trackSelectorController.OnTrackStartPlaying += TryClearCorruption;
            
            _depthOfField = PostProcessingManager.Instance.GetVolumeComponent<DepthOfField>();
            _lensDistortion = PostProcessingManager.Instance.GetVolumeComponent<LensDistortion>();
            _paniniProjection = PostProcessingManager.Instance.GetVolumeComponent<PaniniProjection>();
            _chromaticAberration = PostProcessingManager.Instance.GetVolumeComponent<ChromaticAberration>();
            _motionBlur = PostProcessingManager.Instance.GetVolumeComponent<MotionBlur>();
            _filmGrain = PostProcessingManager.Instance.GetVolumeComponent<FilmGrain>();
        }

        private void OnDestroy()
        {
            if (_stimuliController != null)
            {
                _stimuliController.OnAnxietyTriggered -= Corrupt;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                Corrupt();
            }
        }

        public override void Corrupt()
        {
            base.Corrupt();
            
            _interactableController.DisableAllInteractables();
            _interactableController.EnableInteractable(turntableInteractableObject.InteractableId);
            
            AudioClip randomVoice = voicesProvider.GetRandomElement();
            AudioManager.Instance.PlayOST(AudioChannelType.VOICES, randomVoice, loop: true);
            DrunkEffect();
        }

        private void DrunkEffect()
        {
            _interactionController.StopInteractionWithObject();
            _cameraController.DelayCameraMovement(true);
            _originalDoF = _depthOfField.focalLength.value;
            
            _depthOfField.active = true;
            _lensDistortion.active = true;
            _paniniProjection.active = true;
            _chromaticAberration.active = true;
            _motionBlur.active = true;
            
            fadeInOutBlack.FadeOut(0.5f, Ease.InOutSine, null);
            
            _lensDistortion.intensity.value = 0.5f;
            
            _lensDistortionSequence = DOTween.Sequence().
                Append(
                DOTween.To(
                        () => _lensDistortion.intensity.value,
                        x => _lensDistortion.intensity.value = x,
                        -0.5f,
                        3f
                    )
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo));

            _depthOfFieldSequence = DOTween.Sequence()
                .Append(
                    DOTween.To(
                            () => _depthOfField.focalLength.value,
                            x => _depthOfField.focalLength.value = x,
                            30f,
                            5f)
                        .SetEase(Ease.Linear));
        }

        private void ClearDrunkEffect()
        {
            _lensDistortionSequence?.Kill();
            _lensDistortionSequence = DOTween.Sequence();
            _lensDistortionSequence
                .Append(
                    DOTween.To(
                            () => _lensDistortion.intensity.value,
                            x => _lensDistortion.intensity.value = x,
                            0f,
                            3f)
                        .SetEase(Ease.Linear));
            
            _depthOfFieldSequence?.Kill();
            _depthOfFieldSequence = DOTween.Sequence();
            _depthOfFieldSequence
                .Append(
                    DOTween.To(
                            () => _depthOfField.focalLength.value,
                            x => _depthOfField.focalLength.value = x,
                            _depthOfField.focalLength.value = _originalDoF,
                            3f)
                        .SetEase(Ease.Linear).OnComplete(() =>
                        {
                            fadeInOutBlack.FadeIn(0.5f, Ease.InOutSine, TurnOffAllEffects);
                        }));
        }

        private void TurnOffAllEffects()
        {
            _stimuliController.EnableAnxiety();
            _interactableController.EnableAllInteractables();
            _cameraController.DelayCameraMovement(false);
            _depthOfField.active = false;
            _chromaticAberration.active = false;
            _paniniProjection.active = false;
            _lensDistortion.active = false;
            _motionBlur.active = false;
            _filmGrain.active = false;
            AudioManager.Instance.ClearDistortedAudio(AudioChannelType.VOICES);
            fadeInOutBlack.FadeOut(2f, Ease.InOutSine, null);
        }

        public override bool CanBeCorrupted()
        {
            //Only manually Corrupted
            return false;
        }

        private void TryClearCorruption()
        {
            if (!_isCorrupted)
            {
                return;
            }
            
            ClearCorruption();
        }

        public override void ClearCorruption()
        {
            base.ClearCorruption();

            ClearDrunkEffect();
            AudioManager.Instance.StopOST(AudioChannelType.VOICES);
        }
    }
}