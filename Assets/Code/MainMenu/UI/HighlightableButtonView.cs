using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DVDNights
{
    public class HighlightableButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerUpHandler
    {
        [Header("References")]
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Image highlightImage;
        
        [Header("Configuration")]
        [SerializeField] private Color highlightTextColor;
        [SerializeField] private FontStyles highlightFontStyle;

        private Color _originalImageHighlightColor; 
        private Color _originalTextHighlightColor; 
        private FontStyles _originalFontStyle;
        
        private void Awake()
        {
            _originalImageHighlightColor = highlightImage.color;
            _originalTextHighlightColor =  buttonText.color;
            _originalFontStyle = buttonText.fontStyle;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            highlightImage.gameObject.SetActive(true);
            buttonText.color = highlightTextColor;
            buttonText.fontStyle = highlightFontStyle;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            highlightImage.gameObject.SetActive(false);
            buttonText.color = _originalTextHighlightColor;
            buttonText.fontStyle =_originalFontStyle;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            highlightImage.color = Color.grey;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            highlightImage.color = _originalImageHighlightColor;
        }
    }

}