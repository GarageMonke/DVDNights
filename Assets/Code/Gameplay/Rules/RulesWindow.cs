using System;
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

        private void Awake()
        {
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
    }

    public interface IRulesWindow : IWindow
    {
    }
}