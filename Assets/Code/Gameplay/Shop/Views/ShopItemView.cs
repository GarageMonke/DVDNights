using TMPro;
using UnityEngine;

namespace DVDNights
{
    public class ShopItemView : MonoBehaviour, IShopItemView
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemQuantity;
        [SerializeField] private TextMeshProUGUI itemPrice;

        [Header("Configuration")] 
        [SerializeField] private Color highlightColor;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color warningColor;


        private int _selectedQuantity;
        
        public void InitializeItem()
        {
        }

        public void HighlightItem()
        {
            itemName.color = highlightColor;
        }

        public void UnhighlightItem()
        {
            itemName.color = normalColor;
        }

        public void SelectItem()
        {
            itemName.color = normalColor;
            itemQuantity.color = highlightColor;
        }

        public void DeselectItem()
        {
            HighlightItem();
            itemQuantity.color = normalColor;
        }

        public void UpdatePrice(int updatedPrice, bool isAffordable)
        {
            itemPrice.text = updatedPrice.ToString();

            if (!isAffordable)
            {
                itemPrice.color =  warningColor;
                return;
            }
            
            itemPrice.color = normalColor;
        }

        public void UpdateQuantity(int updatedQuantity)
        {
            _selectedQuantity = updatedQuantity;
            itemQuantity.text = "< " + updatedQuantity + " >";
        }

        public int GetSelectedQuantity()
        {
            return _selectedQuantity;
        }
    }

    public interface IShopItemView
    {
        public void InitializeItem();
        public void HighlightItem();
        public void UnhighlightItem();
        public void SelectItem();
        public void DeselectItem();
        public void UpdatePrice(int updatedPrice, bool isAffordable);
        public void UpdateQuantity(int updatedQuantity);
        public int GetSelectedQuantity();
    }
}