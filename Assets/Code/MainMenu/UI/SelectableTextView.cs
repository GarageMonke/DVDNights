using System;
using CorePatterns.Managers;
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
        
        [Header("Feedback")]
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private float  volume = 0.1f;
        [SerializeField] private float  pitch = 0.4f;

        private void Awake()
        {
            Unselect();
        }

        public void Select()
        {
            selectableText.color = selectedColor;
            selectImage.color = selectedColor;
            selectImage.gameObject.SetActive(true);
            AudioManager.Instance.PlaySFX(audioClip, volume, pitch);
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