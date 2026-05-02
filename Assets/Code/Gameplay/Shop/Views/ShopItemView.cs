using TMPro;
using UnityEngine;

namespace DVDNights
{
    public class ShopItemView : MonoBehaviour, IShopItemView
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemInfo;
        [SerializeField] private TextMeshProUGUI itemPrice;

        [Header("Configuration")] 
        [SerializeField] private Color highlightColor;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color warningColor;

        [SerializeField] private int itemId;

        public int ItemId => itemId;

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
            itemInfo.color = highlightColor;
        }

        public void DeselectItem()
        {
            HighlightItem();
            itemInfo.color = normalColor;
        }

        public void UpdateCost(int updatedCost, bool isAffordable)
        {
            itemPrice.text = updatedCost.ToString();

            if (!isAffordable)
            {
                itemPrice.color =  warningColor;
                return;
            }
            
            itemPrice.color = normalColor;
        }

        public void UpdateInfo(string updatedInfo)
        {
            itemInfo.text = updatedInfo;
        }
    }

    public interface IShopItemView
    {
        public int ItemId { get; }
        public void HighlightItem();
        public void UnhighlightItem();
        public void SelectItem();
        public void DeselectItem();
        public void UpdateCost(int updatedCost, bool isAffordable);
        public void UpdateInfo(string updatedInfo);
    }
}