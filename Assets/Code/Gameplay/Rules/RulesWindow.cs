using System;
using System.Data;
using Common;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using UnityEngine;
using UnityEngine.UI;

namespace DVDNights
{
    public class RulesWindow : Window, IRulesWindow
    {
        [Header("References")] 
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

        private void RaiseOnRulesAcknowledge()
        {
            OnRulesAcknowledge?.Invoke();
            _decayController.EnableDecay();
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, acknowledgeAudioClip, volume: 0.75f);
            Hide();
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