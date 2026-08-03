using Common;
using CorePatterns.Managers;
using DG.Tweening;
using UnityEngine;

namespace Code.MVP
{
    public class LoadingWindow : Window
    {
        private Tween _closeTween;

        public override void Display()
        {
            base.Display();
            
            _closeTween?.Kill();
            float timeToWait = Random.Range(3f, 5f);

            _closeTween = DOVirtual.DelayedCall(timeToWait, Close);
        }

        public override void Close()
        {
            _closeTween?.Kill();
            WindowManager.Instance.CloseWindow<LoadingWindow>();
        }
    }
}