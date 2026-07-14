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
            mainFadeInOutBlack.FadeOut(2f, Ease.Linear, OpenEyes);
        }

        private void OpenEyes()
        {
            _cameraController.EnableNavigation();
            
            DOVirtual.DelayedCall(1f, () =>
            {
                _interactionController.EnableInteractions();
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