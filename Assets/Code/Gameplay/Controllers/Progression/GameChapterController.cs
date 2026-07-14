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

            _tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();
            _tvStateController = ServiceLocator.GetService<ITVStateController>();
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
            
            DOVirtual.DelayedCall(1f, RegisterGoldenDisksCollected);
        }
    }

    public interface IGameProgressionController
    {
        public void RegisterGoldenDisksCollected();
        public void EjectDisk();
    }
}