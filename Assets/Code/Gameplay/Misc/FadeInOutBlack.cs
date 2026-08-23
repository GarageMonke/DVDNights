using System;
using DG.Tweening;
using UnityEngine;

namespace Rulebound
{
    public class FadeInOutBlack : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup canvasGroup;
        
        private Tweener _tweener;
        
        public void FadeIn(float duration, Ease ease, Action onFadeCallback)
        {
            _tweener?.Kill();
            _tweener = canvasGroup.DOFade(1, duration).SetEase(ease).OnComplete(() => onFadeCallback?.Invoke());
            canvasGroup.blocksRaycasts = true;
        }
        
        public void FadeIn(float fadeValue, Action onFadeCallback)
        {
            UpdateFadeValue(fadeValue);
            
            canvasGroup.blocksRaycasts = true;

            if (Mathf.Approximately(canvasGroup.alpha, 1))
            {
                onFadeCallback?.Invoke();
            }
        }

        public void FadeOut(float fadeValue, Action onFadeCallback)
        {
            UpdateFadeValue(fadeValue);          

            if (Mathf.Approximately(canvasGroup.alpha, 0))
            {
                canvasGroup.blocksRaycasts = false;
                onFadeCallback?.Invoke();
            }
        }

        private void UpdateFadeValue(float fadeValue)
        {
            canvasGroup.alpha = Mathf.Clamp01(fadeValue);
        }
        
        public void FadeOut(float duration, Ease ease, Action onFadeCallback)
        {
            _tweener?.Kill();
            _tweener = canvasGroup.DOFade(0, duration).SetEase(ease).OnComplete(() =>
            {
                canvasGroup.blocksRaycasts = false;
                onFadeCallback?.Invoke();
            });
        }
    }
}