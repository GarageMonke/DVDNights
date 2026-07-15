using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class DeliveryController : MonoBehaviour, IDeliveryController
    {
        [Header("DVD-Boxes")]
        [SerializeField] private DVDBoxInteractableObject[] dvdBoxes;
        
        [Header("Rulesets")]
        [SerializeField] private GameRulesInteractableObject[] decoyGameRulesInteractableObjects;
        
        [Header("Door")]
        [SerializeField] private DoorInteractableObject doorInteractableObject;
        
        private int _currentDeliveryIndex;
        private IGameProgressionController _gameProgressionController;
        private IDVDTrayController _dvdTrayController;

        public int LastDeliveredIndex => _currentDeliveryIndex;

        private void Awake()
        {
            InstallService();
        }

        private void Start()
        {
            _dvdTrayController = ServiceLocator.GetService<IDVDTrayController>();
            LoadDeliveredObjects();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IDeliveryController>(this);
        }

        private void LoadDeliveredObjects()
        {
            //Load Delivery Index
            _currentDeliveryIndex = 2;
            
            if (_currentDeliveryIndex <= 0)
            {
                return;
            }
            
            decoyGameRulesInteractableObjects[_currentDeliveryIndex].ShowRules();
            decoyGameRulesInteractableObjects[_currentDeliveryIndex].TeleportRulesToDesktop();

            for (int i = 0; i <= _currentDeliveryIndex; i++)
            {
                dvdBoxes[i].TeleportDvdBoxToDesktop();
            }
            
            _dvdTrayController.SetCurrentDVDBox(dvdBoxes[_currentDeliveryIndex]);
        }

        public void DeliverNextDvdBox()
        {
            if (_currentDeliveryIndex >= dvdBoxes.Length)
            {
                return;
            }
            
            doorInteractableObject.KnockDoor();
            
            dvdBoxes[_currentDeliveryIndex].gameObject.SetActive(true);
            dvdBoxes[_currentDeliveryIndex].EnableInteraction();
            
            _currentDeliveryIndex++;
        }

        public void DeliverNextRuleSet()
        {
            GameRulesInteractableObject gameRulesInteractableObject = decoyGameRulesInteractableObjects[_currentDeliveryIndex];
            gameRulesInteractableObject.SlipTroughDoor();
            _gameProgressionController = ServiceLocator.GetService<IGameProgressionController>();
            _gameProgressionController.SetLastDeliveredRules(gameRulesInteractableObject);
        }
    }

    public interface IDeliveryController
    {
        public int LastDeliveredIndex { get; }
        public void DeliverNextDvdBox();
        public void DeliverNextRuleSet();
        
    }
}