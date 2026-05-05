using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class ShopController : MonoBehaviour, IShopController
    {
        [Header("References")] 
        [SerializeField] private ShopWindow shopWindow;
        
        private IShopWindow _shopWindow;
        private ITVNavigationController _navigationController;
        private IPointsController _pointsController;
        private IDiskLevelController _diskLevelController;
        private IDiskFactory _diskFactory;
        private IShopItemInfoProvider _shopItemInfoProvider;
        private IDisksController _disksController;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IShopController>(this);
            _shopWindow = shopWindow;
        }

        private void Start()
        {
            _pointsController = ServiceLocator.GetService<IPointsController>();
            _navigationController = ServiceLocator.GetService<ITVNavigationController>();
            _navigationController.OnMenuButtonPressed += OpenShop;
            _pointsController.OnScoreChanged += UpdateTotalPoints;
            _shopWindow.OnItemPurchased += PurchaseItem;
            
            _diskLevelController = ServiceLocator.GetService<IDiskLevelController>();
            _diskFactory = ServiceLocator.GetService<IDiskFactory>();
            _shopItemInfoProvider = ServiceLocator.GetService<IShopItemInfoProvider>();
            _disksController = ServiceLocator.GetService<IDisksController>();
        }

        private void PurchaseItem(int itemPurchasedId)
        {
            int itemCost = _shopItemInfoProvider.GetCostByItemId(itemPurchasedId);
            int currentPoints = _pointsController.GetVisualPoints();
            
            _pointsController.UpdatePoints(currentPoints - itemCost);
            
            switch (itemPurchasedId)
            {
                //Buy White Disk
                case 0:
                    _diskFactory.CreateDisk(DiskType.WHITE);
                    break;
                //Disk Base Bonus Level
                case 1:
                    _diskLevelController.DiskBorderBonusLevel += 1;
                    break;
                //Disk Speed Level
                case 2:
                    _diskLevelController.DiskSpeedBonusLevel += 1;
                    break;
                //Disk Corner Bonus Level
                case 3:
                    _diskLevelController.DiskCornerBonusLevel += 1;
                    break;
            }
            
            _shopWindow.UpdateAvailablePoints(_pointsController.GetTotalPoints());
            _shopWindow.Display();
            _disksController.UpdateAllDisks();
        }

        public void OpenShop()
        {
            _navigationController.OnMenuButtonPressed -= OpenShop;
            SubscribeToEvents();
            _shopWindow.UpdateAvailablePoints(_pointsController.GetTotalPoints());
            _shopWindow.Display();
            _shopWindow.HighlightItem();
        }

        public void CloseShop()
        {
            if (_shopWindow.IsItemSelected())
            {
                _shopWindow.DeselectItem();
                return;
            }

            UnsubscribeFromEvents();
            
            _navigationController.OnMenuButtonPressed += OpenShop;
            _shopWindow.Hide();
        }

        public void MoveToNext()
        {
            _shopWindow.MoveToNextItem();
        }

        public void MoveToPrevious()
        {
            _shopWindow.MoveToPreviousItem();
        }

        public void SelectItem()
        {
            if (_shopWindow.IsItemSelected())
            {
                _shopWindow.TryBuyItem();
                return;
            }
            
            _shopWindow.SelectItem();
        }

        private void UpdateTotalPoints(int totalAvailablePoints)
        {
            _shopWindow.UpdateAvailablePoints(totalAvailablePoints);
        }

        private void SubscribeToEvents()
        {
            _navigationController.OnMenuButtonPressed += CloseShop;
            _navigationController.OnNextButtonPressed += MoveToNext;
            _navigationController.OnPreviousButtonPressed += MoveToPrevious;
            _navigationController.OnSubmitButtonPressed += SelectItem;
        }

        private void UnsubscribeFromEvents()
        {
            _navigationController.OnMenuButtonPressed -= CloseShop;
            _navigationController.OnNextButtonPressed -= MoveToNext;
            _navigationController.OnPreviousButtonPressed -= MoveToPrevious;
            _navigationController.OnSubmitButtonPressed -= SelectItem;
        }

        private void OnDestroy()
        {
            _pointsController.OnScoreChanged -= UpdateTotalPoints;
        }
    }

    public interface IShopController
    {
        public void OpenShop();
        public void CloseShop();
        public void MoveToNext();
        public void MoveToPrevious();
        public void SelectItem();
    }
}