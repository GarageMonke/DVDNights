using System;
using Common;
using DG.Tweening;
using DVDNights;
using TMPro;
using UnityEngine;

namespace Code.Gameplay.Dialogues
{
    public class DialogWindow : Window, IDialogWindow
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI dialogText;
        [SerializeField] private CanvasGroup canvasGroup;

        private Tweener _fadeTween;

        private void Awake()
        {
            canvasGroup.alpha = 0;
        }

        public override void Display()
        {
            base.Display();
            _fadeTween?.Kill();
            _fadeTween = canvasGroup.DOFade(1f, 0.3f);
        }

        public override void Hide()
        {
            _fadeTween?.Kill();
            _fadeTween = canvasGroup.DOFade(0f, 0.3f).OnComplete(() => base.Hide());
        }

        public void UpdateDialog(string dialog)
        {
            dialogText.text = dialog;
        }
    }

    public interface IDialogWindow : IWindow
    {
        public void UpdateDialog(string dialog);
    }
}