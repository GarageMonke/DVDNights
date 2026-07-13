using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class GameChapterController : MonoBehaviour, IGameChapterController
    {
        [Header("References")] 
        [SerializeField] private TVMessageWindow tvMessageWindow;
        
        private IDisksController _disksController;
        private int _amountToReach = 3;
        private int _goldAmount;
        private ITVNavigationController _tvNavigationController;
        private ITVStateController _tvStateController;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IGameChapterController>(this);
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

            if (_goldAmount == _amountToReach)
            {
                //Play game ending
                return;
            }
            
            DOVirtual.DelayedCall(0.75f, DisplayMessage);
        }

        private void DisplayMessage()
        {
            tvMessageWindow.OnMessageAccepted += EjectDisk;
            tvMessageWindow.SetMessage("Congratulations! Golden Disc obtained!.");
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

    public interface IGameChapterController
    {
        public void CheckGameEnding();
        public void EjectDisk();
    }
}