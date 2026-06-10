using CorePatterns.ServiceLocator;
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
        }

        public void CheckGameEnding()
        {
            _disksController.OnGoldDiskCreated -= CheckGameEnding;
            _goldAmount++;

            if (_goldAmount != _amountToReach)
            {
                return;
            }
            
            tvMessageWindow.OnMessageAccepted += EjectDisk;
            tvMessageWindow.SetMessage("Error: Something went wrong.");
            tvMessageWindow.Display();
        }

        private void EjectDisk()
        {
            tvMessageWindow.OnMessageAccepted -= EjectDisk;
            _tvNavigationController.OpenCloseButton.Press();
        }
    }

    public interface IGameEndingController
    {
        public void CheckGameEnding();
    }
}