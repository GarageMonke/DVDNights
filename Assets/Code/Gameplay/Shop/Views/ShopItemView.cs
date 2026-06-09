using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DVDNights
{
    public class ShopItemView : MonoBehaviour, IShopItemView
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemInfo;
        [SerializeField] private TextMeshProUGUI itemPrice;
        [SerializeField] private Image itemNameBackground;
        [SerializeField] private Image itemInfoBackground;

        [Header("Configuration")] 
        [SerializeField] private Color highlightColor;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color warningColor;
        

        [SerializeField] private int itemId;

        public int ItemId => itemId;

        public void HighlightItem()
        {
            itemName.color = Color.black;
            itemNameBackground.gameObject.SetActive(true);
        }

        public void UnhighlightItem()
        {
            itemName.color = normalColor;
            itemNameBackground.gameObject.SetActive(false);
        }

        public void SelectItem()
        {
            UnhighlightItem();
            itemInfo.color = Color.black;
            itemInfoBackground.gameObject.SetActive(true);
        }

        public void DeselectItem()
        {
            HighlightItem();
            itemInfo.color = normalColor;
            itemInfoBackground.gameObject.SetActive(false);
        }

        public void UpdateCost(int updatedCost, bool isAffordable)
        {
            itemPrice.text = updatedCost.ToKMB();

            if (!isAffordable)
            {
                itemPrice.color =  warningColor;
                return;
            }
            
            itemPrice.color = normalColor;
            
            if (updatedCost <= 0)
            {
                itemPrice.text = "-";
            }

            if (updatedCost >= Int32.MaxValue)
            {
                itemPrice.text = "MAX";
            }
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