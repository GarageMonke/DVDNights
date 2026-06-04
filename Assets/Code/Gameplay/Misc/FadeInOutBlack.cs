using System;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
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
        }
        
        public void FadeOut(float duration, Ease ease, Action onFadeCallback)
        {
            _tweener?.Kill();
            _tweener = canvasGroup.DOFade(0, duration).SetEase(ease).OnComplete(() => onFadeCallback?.Invoke());
        }
    }
}