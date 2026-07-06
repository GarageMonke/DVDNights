using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class GameEndingController : MonoBehaviour, IGameEndingController
    {
        [Header("References")] 
        [SerializeField] private TVMessageWindow tvMessageWindow;
        
        private IDisksController _disksController;
        private int _amountToReach = 1;
        private int _goldAmount;
        private ITVNavigationController _tvNavigationController;
        private ITVStateController _tvStateController;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IGameEndingController>(this);
        }

        private void Start()
        {
            _disksController = ServiceLocator.GetService<IDisksController>();
            _disksController.OnGoldDiskCreated += CheckGameEnding;

            _tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();
            _tvStateController = ServiceLocator.GetService<ITVStateController>();
        }

        public void CheckGameEnding()
        {
            _disksController.OnGoldDiskCreated -= CheckGameEnding;
            _goldAmount++;

            if (_goldAmount != _amountToReach)
            {
                return;
            }
            
            DOVirtual.DelayedCall(0.75f, DisplayMessage);
        }

        private void DisplayMessage()
        {
            tvMessageWindow.OnMessageAccepted += EjectDisk;
            tvMessageWindow.SetMessage("Error: Something went wrong.");
            tvMessageWindow.Display();
        }

        public void EjectDisk()
        {
            tvMessageWindow.OnMessageAccepted -= EjectDisk;
            _tvStateController.RemoveDisk();
            _tvNavigationController.OpenCloseButton.Press();
            _tvStateController.PlayStatic();
        }
    }

    public interface IGameEndingController
    {
        public void CheckGameEnding();
        public void EjectDisk();
    }
}