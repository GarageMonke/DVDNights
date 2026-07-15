using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

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

        private IOutlineController _outlinesController;
        private ICameraController _cameraController;
        private IInteractionController _interactionController;
        private InputAction _clickAction;

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
            Sequence openEyesSequence =  DOTween.Sequence();
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, openEyesAudioClip, 0.75f);
            _cameraController.WakeUpSequence();
            
            mainFadeInOutBlack.FadeOut(2f, Ease.Linear, null);
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
              
            });
            openEyesSequence.AppendInterval(5f);
            openEyesSequence.AppendCallback(() =>
            {
                _interactionController.EnableInteractions();
                _interactionController.ShowCrossHair();
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