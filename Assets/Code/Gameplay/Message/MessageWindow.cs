using System;
using CorePatterns.ServiceLocator;
using TMPro;
using UnityEngine;

namespace DVDNights
{
    public class MessageWindow : MonoBehaviour, IMessageWindow
    {
        [Header("References")] 
        [SerializeField] private GameObject messageWindow;
        [SerializeField] private TextMeshProUGUI messageText;

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

        public void Display()
        {
            _tvNavigationController.OnSubmitButtonPressed += RaiseOnMessageAccepted;
            messageWindow.SetActive(true);
        }

        public void Hide()
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