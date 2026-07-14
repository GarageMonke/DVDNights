using System;
using UnityEngine;
using UnityEngine.UI;

namespace DVDNights
{
    public class RulesWindow : Window, IRulesWindow
    {
        [Header("References")] 
        [SerializeField] private Button rulesAcknowledgeButton;
        
        public Action OnRulesAcknowledge;

        private void Awake()
        {
            rulesAcknowledgeButton.onClick.AddListener(RaiseOnRulesAcknowledge);
        }

        private void RaiseOnRulesAcknowledge()
        {
            OnRulesAcknowledge?.Invoke();
            Hide();
        }
    }

    public interface IRulesWindow : IWindow
    {
    }
}