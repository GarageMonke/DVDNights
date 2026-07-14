using System;
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

        private void Start()
        {
            DeliverNextRuleSet();
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
            decoyGameRulesInteractableObjects[_currentDeliveryIndex].SlipTroughDoor();
        }
    }

    public interface IDeliveryController
    {
        public void DeliverNextDvdBox();
        public void DeliverNextRuleSet();
    }
}