using System;
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
        
        private int _currentDeliveryIndex;
        private IGameProgressionController _gameProgressionController;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IDeliveryController>(this);
        }

        private void LoadDeliveredObjects()
        {
            //Load Delivery Index
            _currentDeliveryIndex = 0;
            
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
        }
        
        public void DeliverNextDvdBox()
        {
            _currentDeliveryIndex++;

            if (_currentDeliveryIndex >= dvdBoxes.Length)
            {
                return;
            }
            
            dvdBoxes[_currentDeliveryIndex].gameObject.SetActive(true);
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
        public void DeliverNextDvdBox();
        public void DeliverNextRuleSet();
    }
}