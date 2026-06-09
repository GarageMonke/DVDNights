using System;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using TMPro;
using UnityEngine;

namespace DVDNights
{
    public class TVMessageWindow : Window, IMessageWindow
    {
        [Header("References")] 
        [SerializeField] private GameObject messageWindow;
        [SerializeField] private TextMeshProUGUI messageText;
        
        [Header("Feedback")]
        [SerializeField] private AudioClip audioClip;

        public Action OnMessageAccepted { get; set; }

        private ITVNavigationController _tvNavigationController;

        private void Start()
        {
            _tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();
        }
        
        public void SetMessage(string message)
        {
            messageText.text = message;
        }

        private void RaiseOnMessageAccepted()
        {
            OnMessageAccepted?.Invoke();
            Hide();
        }

        public override void Display()
        {
            _tvNavigationController.OnSubmitButtonPressed += RaiseOnMessageAccepted;
            messageWindow.SetActive(true);
            AudioManager.Instance.PlaySFX(audioClip);
        }

        public override void Hide()
        {
            _tvNavigationController.OnSubmitButtonPressed -= RaiseOnMessageAccepted;
            messageWindow.SetActive(false);
        }
    }

    public interface IMessageWindow : IWindow
    {
        public Action OnMessageAccepted { get; set; }
        public void SetMessage(string message);
    }
}