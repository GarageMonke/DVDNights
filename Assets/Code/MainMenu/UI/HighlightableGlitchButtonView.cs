using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DVDNights
{
    public class HighlightableGlitchButtonView : HighlightableButtonView
    {
        private static readonly int JitterStrength = Shader.PropertyToID("_JitterStrength");
        private static readonly int LineStrength = Shader.PropertyToID("_LineStrength");
        private static readonly int Chromatic = Shader.PropertyToID("_Chromatic");
        private static readonly int ScanlineIntensity = Shader.PropertyToID("_ScanlineIntensity");
        private static readonly int ScrollX = Shader.PropertyToID("_ScrollX");
        private static readonly int ScrollY = Shader.PropertyToID("_ScrollY");
        private static readonly int UpdateRate = Shader.PropertyToID("_UpdateRate");

        [Header("Glitch-Configuration")]
        [SerializeField, Range(0f, 0.05f)]
        private float originalJitterStrength = 0.0364f;
        private float highlightedJitterStrength = 0.0025f;

        [SerializeField, Range(0f, 0.1f)]
        private float originalHorizontalTear = 0.0441f;
        private float highlightedHorizontalTear = 0.0013f;

        [SerializeField, Range(0f, 0.02f)]
        private float originalRgbSplit = 0.00648f;
        private float highlightedRgbSplit = 0f;

        [SerializeField, Range(0f, 1f)]
        private float originalScanlines = 0.304f;
        private float highlightedScanlines = 0.304f;

        [SerializeField]
        private float originalNoiseScrollX = 0.1f;
        private float highlightedNoiseScrollX = 0.1f;

        [SerializeField]
        private float originalNoiseScrollY = 0.5f;
        private float highlightedNoiseScrollY = 0.5f;

        [SerializeField, Range(1f, 60f)]
        private float originalUpdateRate = 12f;
        private float highlightedUpdateRate = 12f;
        
        [Header("Tween")]
        [SerializeField] private float transitionDuration = 0.2f;
        [SerializeField] private Ease transitionEase = Ease.OutQuad;
        
        private Material _textMaterial;
        private Tween _glitchTween;
        
        private float _jitterStrength;
        private float _horizontalTear;
        private float _rgbSplit;
        private float _scanlines;
        private float _noiseScrollX;
        private float _noiseScrollY;
        private float _updateRate;
        
         protected override void Awake()
        {
            base.Awake();
            
            _textMaterial = buttonText.fontMaterial;

            _jitterStrength = originalJitterStrength;
            _horizontalTear = originalHorizontalTear;
            _rgbSplit = originalRgbSplit;
            _scanlines = originalScanlines;
            _noiseScrollX = originalNoiseScrollX;
            _noiseScrollY = originalNoiseScrollY;
            _updateRate = originalUpdateRate;

            UpdateGlitchValues();
        }

        protected override void Highlight()
        {
            base.Highlight();
            TweenGlitch(true);
        }

        protected override void Unhighlight()
        {
            base.Unhighlight();
            TweenGlitch(false);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            Highlight();
        }

        private void TweenGlitch(bool highlighted)
        {
            _glitchTween?.Kill();

            float start =
                Mathf.InverseLerp(
                    originalJitterStrength,
                    highlightedJitterStrength,
                    _jitterStrength);

            float target = highlighted ? 1f : 0f;

            _glitchTween = DOVirtual.Float(start, target, transitionDuration, t =>
            {
                _jitterStrength = Mathf.Lerp(originalJitterStrength, highlightedJitterStrength, t);
                _horizontalTear = Mathf.Lerp(originalHorizontalTear, highlightedHorizontalTear, t);
                _rgbSplit = Mathf.Lerp(originalRgbSplit, highlightedRgbSplit, t);
                _scanlines = Mathf.Lerp(originalScanlines, highlightedScanlines, t);
                _noiseScrollX = Mathf.Lerp(originalNoiseScrollX, highlightedNoiseScrollX, t);
                _noiseScrollY = Mathf.Lerp(originalNoiseScrollY, highlightedNoiseScrollY, t);
                _updateRate = Mathf.Lerp(originalUpdateRate, highlightedUpdateRate, t);

                UpdateGlitchValues();
            })
            .SetEase(transitionEase);
        }

        private void UpdateGlitchValues()
        {
            _textMaterial.SetFloat(JitterStrength, _jitterStrength);
            _textMaterial.SetFloat(LineStrength, _horizontalTear);
            _textMaterial.SetFloat(Chromatic, _rgbSplit);
            _textMaterial.SetFloat(ScanlineIntensity, _scanlines);
            _textMaterial.SetFloat(ScrollX, _noiseScrollX);
            _textMaterial.SetFloat(ScrollY, _noiseScrollY);
            _textMaterial.SetFloat(UpdateRate, _updateRate);
        }

        private void OnDestroy()
        {
            _glitchTween?.Kill();
        }
    }
}