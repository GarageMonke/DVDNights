using System;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class GameProgressionController : MonoBehaviour, IGameProgressionController
    {
        [Header("References")] 
        [SerializeField] private TVMessageWindow tvMessageWindow;
        
        private int _goldDisksAmount;
        private IDisksController _disksController;
        private ITVNavigationController _tvNavigationController;
        private ITVStateController _tvStateController;
        private IDeliveryController _deliveryController;
        private IDecayController _decayController;
        
        private GameRulesInteractableObject _currentGameRulesInteractableObject;
        private GameRulesInteractableObject _obsoleteGameRulesInteractableObject;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IGameProgressionController>(this);
        }

        private void Start()
        {
            _disksController = ServiceLocator.GetService<IDisksController>();
            _disksController.OnGoldDiskCreated += RegisterGoldenDisksCollected;
            _tvStateController = ServiceLocator.GetService<ITVStateController>();
            _tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();
            _decayController = ServiceLocator.GetService<IDecayController>();
            //ScheduleRulesDelivery();
        }

        public void ScheduleRulesDelivery()
        {
            if (!_tvStateController.HasDisk)
            {
                return;
            }
            
            DOVirtual.DelayedCall(5, DeliverRuleSet);
        }

        private void DeliverRuleSet()
        {
            _deliveryController = ServiceLocator.GetService<IDeliveryController>();
            _deliveryController.DeliverNextRuleSet();
        }

        public void RegisterGoldenDisksCollected()
        {
            _disksController.OnGoldDiskCreated -= RegisterGoldenDisksCollected;
            _goldDisksAmount++;
            DOVirtual.DelayedCall(0.75f, DisplayMessage);
        }

        private void CheckGameEnding()
        {
            if (_goldDisksAmount <  BounceGameProgression.GoldenDiscsToCollect)
            {
                //Game is not over yet
            }
            
            //Game Over Sequence
        }

        private void DisplayMessage()
        {
            tvMessageWindow.OnMessageAccepted += EjectDisk;
            tvMessageWindow.SetMessage("Congratulations! Golden Disc obtained!");
            tvMessageWindow.Display();
        }

        public void EjectDisk()
        {
            tvMessageWindow.OnMessageAccepted -= EjectDisk;
            _tvStateController.RemoveDisk();
            _tvNavigationController.OpenCloseButton.Press();
            _tvStateController.PlayStatic();
            _decayController.DisableDecay();
          
            DOVirtual.DelayedCall(1f, ()=> _deliveryController.DeliverNextDvdBox());
        }

        private void HideObsoleteRules()
        {
            _currentGameRulesInteractableObject.OnRulesAcknowledge -= HideObsoleteRules;

            if (_obsoleteGameRulesInteractableObject)
            {
                _obsoleteGameRulesInteractableObject.HideRules();
            }
        }

        public void SetLastDeliveredRules(GameRulesInteractableObject currentGameRulesInteractableObject)
        {
            _obsoleteGameRulesInteractableObject = _currentGameRulesInteractableObject;
            _currentGameRulesInteractableObject = currentGameRulesInteractableObject;
            _currentGameRulesInteractableObject.OnRulesAcknowledge += HideObsoleteRules;
        }

        private void OnDestroy()
        {
            _disksController.OnGoldDiskCreated -= RegisterGoldenDisksCollected;
        }
    }

    public interface IGameProgressionController
    {
        public void RegisterGoldenDisksCollected();
        public void EjectDisk();
        public void SetLastDeliveredRules(GameRulesInteractableObject currentGameRulesInteractableObject);
        public void ScheduleRulesDelivery();
    }
}