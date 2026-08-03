using System;
using Common;
using CorePatterns.ServiceLocator;
using TMPro;
using UnityEngine;

namespace DVDNights
{
    public class ShopWindow : TVWindow, IShopWindow
    {
        [Header("References")] 
        [SerializeField] private TextMeshProUGUI availablePointsText;
        [SerializeField] private ShopItemView[] shopItemViews;
        [SerializeField] private GameObject shopContent;

        private bool _isItemSelected;
        private int _currentItemIndex;
        private int _previousItemIndex;
        private IShopItemView _currentItemView;
        private ShopItemView _previousItemView;
        private IShopItemInfoProvider _shopItemInfoProvider;
        private int _availablePoints;
        
        public Action<int> OnItemPurchased { get; set; }
        

        private void Start()
        {
            _shopItemInfoProvider = ServiceLocator.GetService<IShopItemInfoProvider>();
        }
        
        public override void Display()
        {
            foreach (IShopItemView shopItemView in shopItemViews)
            {
                shopItemView.UpdateInfo(_shopItemInfoProvider.GetInfoByItemId(shopItemView.ItemId));
                UpdateItemCost(shopItemView);
            }
            
            shopContent.SetActive(true);
        }

        public override void Hide()
        {
            _currentItemIndex = 0;
            shopContent.SetActive(false);
        }

        private void UpdateItemCost(IShopItemView shopItemView)
        {
            int itemCost = _shopItemInfoProvider.GetCostByItemId(shopItemView.ItemId);
            bool isAffordable = _availablePoints >= itemCost;
            shopItemView.UpdateCost(itemCost, isAffordable);
        }
        

        public void HighlightItem()
        {
           _previousItemView = shopItemViews[_previousItemIndex];
           _previousItemView.UnhighlightItem();
            
            _currentItemView = shopItemViews[_currentItemIndex];
            _currentItemView.HighlightItem();
        }

        public void ResetShop()
        {
            foreach (IShopItemView shopItemView in shopItemViews)
            {
               shopItemView.UnhighlightItem();
            }
            
            _currentItemView = shopItemViews[0];
            _currentItemView.HighlightItem();
        }
        

        public void TryBuyItem()
        {
            int itemCost = _shopItemInfoProvider.GetCostByItemId(_currentItemView.ItemId);

            if (_availablePoints >= itemCost)
            {
                OnItemPurchased?.Invoke(_currentItemView.ItemId);
            }
        }
        
        public void SaveShop()
        {
            throw new System.NotImplementedException();
        }

        public void LoadShop()
        {
            throw new System.NotImplementedException();
        }

        public void MoveToNextItem()
        {
            if (_isItemSelected)
            {
                return;
            }
            
            _previousItemIndex = _currentItemIndex;
            _currentItemIndex++;
            _currentItemIndex = Mathf.Min(_currentItemIndex, shopItemViews.Length - 1);
            
            HighlightItem();
        }

        public void MoveToPreviousItem()
        {
            if (_isItemSelected)
            {
                return;
            }
            
            _previousItemIndex = _currentItemIndex;
            _currentItemIndex--;
            _currentItemIndex = Mathf.Max(0, _currentItemIndex);
            
            HighlightItem();
        }

        public void UpdateAvailablePoints(int availablePoints)
        {
            _availablePoints = availablePoints;
            availablePointsText.text = "AVAILABLE POINTS: " + availablePoints.ToString("D10");
            
            foreach (IShopItemView shopItemView in shopItemViews)
            {
                UpdateItemCost(shopItemView);
            }
        }

        public bool IsItemSelected()
        {
            return _isItemSelected;
        }
    }

    public interface IShopWindow : IWindow
    {
        public Action<int> OnItemPurchased { get; set; }
        public void HighlightItem();
        public void TryBuyItem();
        public void ResetShop();
        public void SaveShop();
        public void LoadShop();
        public void MoveToNextItem();
        public void MoveToPreviousItem();
        public void UpdateAvailablePoints(int availablePoints);
        public bool IsItemSelected();
    }
}