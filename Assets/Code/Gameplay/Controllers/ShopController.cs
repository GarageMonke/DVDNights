using System;
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
            _shopWindow.UpdateAvailablePoints(_pointsController.GetTotalPoints());
        }

        public void OpenShop()
        {
            _navigationController.OnMenuButtonPressed -= OpenShop;
            SubscribeToEvents();
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