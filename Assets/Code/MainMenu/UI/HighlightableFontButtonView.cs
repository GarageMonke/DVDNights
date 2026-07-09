using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace DVDNights
{
    public class HighlightableFontButtonView : HighlightableButtonView
    {
        [Header("Text-Font")]
        [SerializeField] private TMP_FontAsset highlightTextFont;
        
        private TMP_FontAsset _originalTextFont;
        
        protected override void Awake()
        {
            base.Awake();
            _originalTextFont = buttonText.font;
        }
        
        protected override void Unhighlight()
        {
            base.Unhighlight();
            buttonText.font = _originalTextFont;
        }
        
        protected override void Highlight()
        {
            base.Highlight();
            buttonText.font = highlightTextFont;
        }
    }
}