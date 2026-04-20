using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DVDNights
{
    public class SelectableTextView : MonoBehaviour, ISelectableTextView
    {
        [Header("References")] 
        [SerializeField] private TMP_Text selectableText;
        [SerializeField] private Image selectImage;

        [Header("Configuration")] 
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color selectedColor;

        private void Awake()
        {
            Unselect();
        }

        public void Select()
        {
            selectableText.color = selectedColor;
            selectImage.color = selectedColor;
            selectImage.gameObject.SetActive(true);
        }

        public void Unselect()
        {
            selectableText.color = defaultColor;
            selectImage.color = defaultColor;
            selectImage.gameObject.SetActive(false);
        }
    }

    public interface ISelectableTextView
    {
        public void Select();
        public void Unselect();
    }
}