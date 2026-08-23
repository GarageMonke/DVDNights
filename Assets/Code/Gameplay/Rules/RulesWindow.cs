using System;
using System.Collections;
using System.Data;
using Common;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using UnityEngine;
using UnityEngine.UI;

namespace Rulebound
{
    public class RulesWindow : Window, IRulesWindow
    {
        [Header("References")] 
        [SerializeField] private RectTransform rulesRectTransform;
        [SerializeField] private CanvasGroup rulesCanvasGroup;
        [SerializeField] private Button rulesAcknowledgeButton;
        [SerializeField] private AudioClip acknowledgeAudioClip;
        
        public Action OnRulesAcknowledge;
        private IDecayController _decayController;

        protected override void Awake()
        {
            base.Awake();
            rulesAcknowledgeButton.onClick.AddListener(RaiseOnRulesAcknowledge);
        }

        private void Start()
        {
            _decayController = ServiceLocator.GetService<IDecayController>();
        }

        public override void Display()
        {
            rulesCanvasGroup.alpha = 0;
           StartCoroutine(RebuildRulesWindow());
        }

        private void RaiseOnRulesAcknowledge()
        {
            OnRulesAcknowledge?.Invoke();
            _decayController.EnableDecay();
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, acknowledgeAudioClip, volume: 0.75f);
            Hide();
        }

        private IEnumerator RebuildRulesWindow()
        {
            base.Display();
            yield return null;
            rulesRectTransform.gameObject.SetActive(false);
            yield return null;
            rulesRectTransform.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rulesRectTransform);
            rulesCanvasGroup.enabled = false;
            rulesCanvasGroup.alpha = 1;
        }
        
        public override void Close()
        {
            WindowManager.Instance.CloseWindow<RulesWindow>();
        }
    }

    public interface IRulesWindow : IWindow
    {
    }
}