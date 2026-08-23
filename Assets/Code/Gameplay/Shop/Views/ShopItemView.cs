using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rulebound
{
    public class ShopItemView : MonoBehaviour, IShopItemView
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemInfo;
        [SerializeField] private TextMeshProUGUI itemPrice;
        [SerializeField] private Image itemBackground;

        [Header("Configuration")] 
        [SerializeField] private Color highlightColor;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color warningColor;
        
        [SerializeField] private int itemId;

        public int ItemId => itemId;
        
        private bool _isHighlighted = false;

        public void HighlightItem()
        {
            itemName.color = Color.black;
            itemInfo.color = Color.black;
            itemPrice.color = Color.black;
            itemBackground.gameObject.SetActive(true);
            _isHighlighted = true;
        }

        public void UnhighlightItem()
        {
            itemName.color = normalColor;
            itemInfo.color = normalColor;
            itemPrice.color = normalColor;
            itemBackground.gameObject.SetActive(false);
            _isHighlighted = false;
        }
        

        public void UpdateCost(int updatedCost, bool isAffordable)
        {
            itemPrice.text = updatedCost.ToKMB();

            if (!isAffordable)
            {
                itemBackground.color =  warningColor;
                return;
            }

            itemBackground.color = highlightColor;
            
            if (_isHighlighted)
            {
                itemPrice.color = Color.black;
            }
            else
            {
                itemPrice.color = normalColor;
            }
            
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
        public void UpdateCost(int updatedCost, bool isAffordable);
        public void UpdateInfo(string updatedInfo);
    }
}