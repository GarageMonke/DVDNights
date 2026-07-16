using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class NewGameController : MonoBehaviour, INewGameController
    {
        [Header("Tutorial-References")]
        [SerializeField] private TVInteractableObject tvInteractableObject;
        [SerializeField] private DoorInteractableObject doorInteractableObject;
        [SerializeField] private DVDBoxInteractableObject dvdBoxInteractableObject;
        [SerializeField] private GameRulesInteractableObject gameRulesInteractableObject;
        
        private IInteractableController _interactableController;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<INewGameController>(this);
        }

        private void Start()
        {
            _interactableController = ServiceLocator.GetService<IInteractableController>();
            DOVirtual.DelayedCall(2f, StartTutorialSequence);
        }

        public void StartTutorialSequence()
        {
            _interactableController.DisableAllInteractables();
            _interactableController.EnableInteractable(doorInteractableObject.InteractableId);
            _interactableController.EnableInteractable(dvdBoxInteractableObject.InteractableId);
            dvdBoxInteractableObject.OnInteractionPerformed += HandleTutorialSequence01;
        }

        private void HandleTutorialSequence01()
        {
            dvdBoxInteractableObject.OnInteractionPerformed -= HandleTutorialSequence01;
            gameRulesInteractableObject.OnInteractionPerformed += HandleTutorialSequence02;
            _interactableController.EnableInteractable(tvInteractableObject.InteractableId);
        }

        private void HandleTutorialSequence02()
        {
            gameRulesInteractableObject.OnInteractionPerformed -= HandleTutorialSequence02;
            _interactableController.EnableAllInteractables();
            _interactableController.DisableInteractable(doorInteractableObject.InteractableId);
        }
    }

    public interface INewGameController
    {
        public void StartTutorialSequence();
    }
}