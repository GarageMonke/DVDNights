using System;
using UnityEngine;

namespace Rulebound
{
    public class DiscDiodePlatform : DiodePlatform
    {
        [Header("Renderers")]
        [SerializeField] private Renderer contourRenderer;
        [SerializeField] private Renderer centerRenderer;
        [SerializeField] private Renderer backgroundRenderer;
        
        [Header("Invisible")]
        [SerializeField] private float invisibleAlpha = 0.5f;
        
        private Color _backgroundOriginalColor;
        
        private static readonly int GlobalAlphaID = Shader.PropertyToID("_Alpha");
        
        private float _centerOriginalAlpha;
        private float _contourOriginalAlpha;

        private void Awake()
        {
            _backgroundOriginalColor = backgroundRenderer.material.color;
            
            _centerOriginalAlpha = centerRenderer.material.GetFloat(GlobalAlphaID);
            _contourOriginalAlpha = contourRenderer.material.GetFloat(GlobalAlphaID);
        }

        protected override void MakeVisible()
        {
            centerRenderer.material.SetFloat(GlobalAlphaID, _centerOriginalAlpha);
            contourRenderer.material.SetFloat(GlobalAlphaID, _contourOriginalAlpha);
            backgroundRenderer.material.color = _backgroundOriginalColor;
            base.MakeVisible();
        }

        protected override void MakeInvisible()
        {
            centerRenderer.material.SetFloat(GlobalAlphaID, invisibleAlpha);
            contourRenderer.material.SetFloat(GlobalAlphaID, invisibleAlpha);
            
            backgroundRenderer.material.color = invisibleColor;
            base.MakeInvisible();
        }
    }
}