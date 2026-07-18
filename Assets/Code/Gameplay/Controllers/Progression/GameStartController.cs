using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

namespace DVDNights
{
    public class GameStartController : MonoBehaviour, IGameStartController
    {
        [Header("References")] 
        [SerializeField] private FadeInOutBlack mainFadeInOutBlack;
        [SerializeField] private InputActionSO clickActionSO;
        [SerializeField] private GameObject wakeUpView;
        
        [Header("Audio-Feedback")]
        [SerializeField] private AudioClip openEyesAudioClip;
        [SerializeField] private AudioClip doorKnockingAudioClip;

        private IOutlineController _outlinesController;
        private ICameraController _cameraController;
        private IInteractionController _interactionController;
        private IDeliveryController _deliveryController;
        private InputAction _clickAction;
        
        private DepthOfField _depthOfField;
        private LensDistortion _lensDistortion;
        private Sequence _doorKnockingSequence;


        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            _clickAction = clickActionSO.GetInputAction();
            _clickAction.performed += PrepareRoom;
           ServiceLocator.RegisterService<IGameStartController>(this);
           
        }

        private void Start()
        {
            _outlinesController = ServiceLocator.GetService<IOutlineController>();
            _cameraController = ServiceLocator.GetService<ICameraController>();
            _interactionController = ServiceLocator.GetService<IInteractionController>();
            _depthOfField = PostProcessingManager.Instance.GetVolumeComponent<DepthOfField>();
            _lensDistortion = PostProcessingManager.Instance.GetVolumeComponent<LensDistortion>();
            _deliveryController = ServiceLocator.GetService<IDeliveryController>();
            
            //Check if its a new game
            AudioManager.Instance.PlayOST(AudioChannelType.DOOR, doorKnockingAudioClip, 0.75f, true);
        }

        private void PrepareRoom(InputAction.CallbackContext context)
        {
            _clickAction.performed -= PrepareRoom;
            wakeUpView.SetActive(false);
            _outlinesController.DisableAllOutlines();
            OpenEyes();
        }

        private void OpenEyes()
        {
            float originalDoF = _depthOfField.focalLength.value;
            _depthOfField.active = true;
            _lensDistortion.active = true;
            Sequence openEyesSequence =  DOTween.Sequence();
            Tweener depthOfFieldTweener = null;

            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, openEyesAudioClip, 0.75f);
            
            _cameraController.WakeUpSequence();
            mainFadeInOutBlack.FadeOut(2f, Ease.Linear, null);
            
            Sequence lensDistortionSequence = DOTween.Sequence()
                .Append(
                    DOTween.To(
                            () => _lensDistortion.intensity.value,
                            x => _lensDistortion.intensity.value = x,
                            0.5f,
                            5f)
                        .SetEase(Ease.Linear))
                .AppendInterval(1f)
                .Append(
                    DOTween.To(
                            () => _lensDistortion.intensity.value,
                            x => _lensDistortion.intensity.value = x,
                            -0.5f,
                            5f)
                        .SetEase(Ease.Linear))
                .SetLoops(-1);
            
            openEyesSequence.AppendInterval(2f);
            openEyesSequence.AppendCallback(() =>
            {
                mainFadeInOutBlack.FadeIn(1f, Ease.Linear, null);
            });
            openEyesSequence.AppendInterval(1f);
            openEyesSequence.AppendCallback(() =>
            {
                mainFadeInOutBlack.FadeOut(1f, Ease.Linear, null);
            });
            openEyesSequence.AppendInterval(2f);
            openEyesSequence.AppendCallback(() =>
            {
                mainFadeInOutBlack.FadeIn(1f, Ease.Linear, null);
            });
            openEyesSequence.AppendInterval(2f);
            openEyesSequence.AppendCallback(() =>
            {
                mainFadeInOutBlack.FadeOut(2f, Ease.Linear, null);
                depthOfFieldTweener = DOTween.To(
                        () => _depthOfField.focalLength.value,
                        x => _depthOfField.focalLength.value = x,
                        0f,
                        5f)
                    .SetEase(Ease.Linear);
                
                lensDistortionSequence?.Kill();
                lensDistortionSequence
                    .Append(
                        DOTween.To(
                                () => _lensDistortion.intensity.value,
                                x => _lensDistortion.intensity.value = x,
                                0f,
                                5f)
                            .SetEase(Ease.Linear))
                    .SetLoops(-1);
                
                AudioManager.Instance.StopOST(AudioChannelType.DOOR);
            });
            openEyesSequence.AppendInterval(5.5f);
            openEyesSequence.AppendCallback(() =>
            {
                _depthOfField.focalLength.value = originalDoF;
                _depthOfField.active = false;
                depthOfFieldTweener?.Kill();
                lensDistortionSequence?.Kill();
                _lensDistortion.active = false;
                _interactionController.EnableInteractions();
                _interactionController.ShowCrossHair();
            });
            
            openEyesSequence.AppendInterval(0.5f);
            openEyesSequence.AppendCallback(() =>
            {
                _deliveryController.DeliverNextDvdBox();
            });
        }
        

        private void OnDestroy()
        {
            _clickAction.performed -= PrepareRoom;
        }
    }

    public interface IGameStartController
    {
        
    }
}