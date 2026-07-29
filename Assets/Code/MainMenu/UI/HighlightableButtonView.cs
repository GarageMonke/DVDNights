using CorePatterns.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DVDNights
{
    public class HighlightableButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("References")]
        [SerializeField] private Button button;
        [SerializeField] protected TextMeshProUGUI buttonText;
        [SerializeField] protected Image highlightImage;
        [SerializeField] private bool hideImageOnUnhighlight = true;
        
        [Header("Highlight-Settings")]
        [SerializeField] private AudioClip highlightAudioClip;
        [SerializeField] private float highlightPitch = 1f;
        [SerializeField] private float highlightVolume = 1f;
        
        [Header("Click-Settings")]
        [SerializeField] private AudioClip clickAudioClip;
        [SerializeField] private float clickPitch = 1f;
        [SerializeField] private float clickVolume = 1f;
        
        [Header("Configuration")]
        [SerializeField] private Color highlightTextColor;
        [SerializeField] private FontStyles highlightFontStyle;

        private Color _originalImageHighlightColor; 
        private Color _originalTextHighlightColor; 
        private FontStyles _originalFontStyle;
        
        protected virtual void Awake()
        {
            if (highlightImage)
            {
                _originalImageHighlightColor = highlightImage.color;
            }

            _originalTextHighlightColor =  buttonText.color;
            _originalFontStyle = buttonText.fontStyle;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            Highlight();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
           Unhighlight();
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (highlightImage)
            {
                highlightImage.color = Color.grey;
            }

            buttonText.color = _originalTextHighlightColor;
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, clickAudioClip, volume: clickVolume, pitch: clickPitch);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (highlightImage)
            {
                highlightImage.color = _originalImageHighlightColor;
            }
            
            buttonText.color = _originalTextHighlightColor;
            buttonText.fontStyle = _originalFontStyle;
        }

        protected virtual void Highlight()
        {
            if (highlightImage)
            {
                highlightImage.gameObject.SetActive(true);
                highlightImage.color = highlightTextColor;
            }

            buttonText.color = highlightTextColor;
            buttonText.fontStyle = highlightFontStyle;
            
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, highlightAudioClip, volume: highlightVolume, pitch: highlightPitch);
        }

        protected virtual void Unhighlight()
        {
            if (highlightImage)
            {
                highlightImage.gameObject.SetActive(!hideImageOnUnhighlight);
                highlightImage.color = _originalImageHighlightColor;
            }

            buttonText.color = _originalTextHighlightColor;
            buttonText.fontStyle = _originalFontStyle;
        }
        
        protected virtual void OnDisable()
        {
            Unhighlight();
        }
    }

}