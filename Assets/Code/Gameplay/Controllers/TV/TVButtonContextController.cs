using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class TVButtonContextController : MonoBehaviour, ITVButtonContextController
    {
        private ITVNavigationController _tvNavigationController;
        private IDVDTrayController _dvdTrayController;
        private IShopController _shopController;
        private TVButton[] _tvButtons;
        private ITVStateController _tvStateController;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<ITVButtonContextController>(this);
        }

        private void Start()
        {
            _tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();
            _tvButtons = _tvNavigationController.TvButtons;
            
            _dvdTrayController = ServiceLocator.GetService<IDVDTrayController>();
            _shopController = ServiceLocator.GetService<IShopController>();
            _tvStateController = ServiceLocator.GetService<ITVStateController>();
        }

        public string GetTVButtonAction(int buttonId)
        {
            if (buttonId < 0 || buttonId > _tvButtons.Length - 1)
            {
                return "NULL";
            }
            
            _tvStateController ??= ServiceLocator.GetService<ITVStateController>();
            _dvdTrayController ??= ServiceLocator.GetService<IDVDTrayController>();
            _shopController ??= ServiceLocator.GetService<IShopController>();

            if (!_tvStateController.IsTVOn)
            {
                return buttonId == 0 ? "Power On" : "Turn On to TV to Interact";
            }
            
            switch (buttonId)
            {
                //Power Button
                case 0:
                    return "Power Off";
                //Open/Close Button
                case 1:
                    return _dvdTrayController.IsTrayOpened ? "Close" : "Open";
                //Menu Button
                case 2:
                    if (_shopController == null)
                    {
                        return "Menu";
                    }
                    
                    if (_shopController.IsShopOpened)
                    {
                        return _shopController.IsItemSelected ? "Go Back" : "Exit Shop";
                    }

                    return "Open Shop";
                //Previous Button
                case 3:
                    return "Previous";
                //Submit Button
                case 4:
                    if (_shopController == null)
                    {
                        return "Submit";
                    }

                    if (_shopController.IsShopOpened)
                    {
                        return _shopController.IsItemSelected ? "Purchase Upgrade" : "Select Item";
                    }

                    return "FF Power";
                //Next Button
                case 5:
                    if (_shopController == null)
                    {
                        return "Fast Forward";
                    }
                    
                    if (_shopController.IsShopOpened)
                    {
                        return "Next";
                    }
                    
                    return "Fast Forward";
                   
                //Play/Pause Button
                case 6:
                    return "?";
                //Volume Down Button
                case 7:
                    return "Volume Down";
                //Volume Up Button
                case 8:
                    return "Volume Up";
            }

            return "NULL";
        }
    }

    public interface ITVButtonContextController
    {
        public string GetTVButtonAction(int buttonId);
    }
}