using TMPro;
using UnityEngine;

namespace DVDNights
{
    public class ShopWindow : MonoBehaviour, IShopWindow
    {
        [Header("References")] 
        [SerializeField] private GameObject shopWindow;
        [SerializeField] private TextMeshProUGUI availablePointsText;
        [SerializeField] private ShopItemView[] shopItemViews;

        private bool _isItemSelected;
        private int _currentItemIndex;
        private int _previousItemIndex;

        public void Display()
        {
            foreach (IShopItemView shopItemView in shopItemViews)
            {
                shopItemView.UpdateQuantity(1);
            }
            
            shopWindow.SetActive(true);
        }

        public void Hide()
        {
            shopWindow.SetActive(false);
        }

        public void HighlightItem()
        {
            IShopItemView previousItemView = shopItemViews[_previousItemIndex];
            previousItemView.UnhighlightItem();
            
            IShopItemView currentItemView = shopItemViews[_currentItemIndex];
            currentItemView.HighlightItem();
        }

        public void SelectItem()
        {
            _isItemSelected = true;
            
            IShopItemView currentItemView = shopItemViews[_currentItemIndex];
            currentItemView.SelectItem();
        }

        public void DeselectItem()
        {
            _isItemSelected = false;
            
            IShopItemView currentItemView = shopItemViews[_currentItemIndex];
            currentItemView.DeselectItem();
        }

        public void TryBuyItem()
        {
            throw new System.NotImplementedException();
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
                IncreaseQuantity();
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
                DecreaseQuantity();
                return;
            }
            
            _previousItemIndex = _currentItemIndex;
            _currentItemIndex--;
            _currentItemIndex = Mathf.Max(0, _currentItemIndex);
            
            HighlightItem();
        }

        public void UpdateAvailablePoints(int availablePoints)
        {
            availablePointsText.text = "AVAILABLE POINTS: " + availablePoints.ToString("D10");
        }

        public bool IsItemSelected()
        {
            return _isItemSelected;
        }

        private void IncreaseQuantity()
        {
            IShopItemView currentItemView = shopItemViews[_currentItemIndex];
            int currentQuantity = currentItemView.GetSelectedQuantity();

            currentQuantity++;
            currentQuantity = Mathf.Min(currentQuantity, 99);
            currentItemView.UpdateQuantity(currentQuantity);
        }

        private void DecreaseQuantity()
        {
            IShopItemView currentItemView = shopItemViews[_currentItemIndex];
            int currentQuantity = currentItemView.GetSelectedQuantity();

            currentQuantity--;
            currentQuantity = Mathf.Max(1, currentQuantity);
            currentItemView.UpdateQuantity(currentQuantity);
        }
    }

    public interface IShopWindow : IWindow
    {
        public void HighlightItem();
        public void SelectItem();
        public void DeselectItem();
        public void TryBuyItem();
        public void SaveShop();
        public void LoadShop();
        public void MoveToNextItem();
        public void MoveToPreviousItem();
        public void UpdateAvailablePoints(int availablePoints);
        public bool IsItemSelected();
    }
}