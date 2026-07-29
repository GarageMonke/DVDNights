using System;
using System.Collections;
using Common;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DVDNights
{
    public class TVMessageWindow : Window, IMessageWindow
    {
        [Header("References")] 
        [SerializeField] private GameObject messageWindow;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private CanvasGroup canvasGroup;
        
        [Header("Feedback")]
        [SerializeField] private AudioClip audioClip;

        public Action OnMessageAccepted { get; set; }

        private ITVNavigationController _tvNavigationController;

        private void Awake()
        {
            ServiceLocator.RegisterService<IMessageWindow>(this);
        }

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
            StartCoroutine(StartRebuilding());
        }

        public override void Hide()
        {
            canvasGroup.alpha = 0;
            _tvNavigationController.OnSubmitButtonPressed -= RaiseOnMessageAccepted;
            messageWindow.SetActive(false);
        }

        private IEnumerator StartRebuilding()
        {
            canvasGroup.alpha = 0;
            int retries = 2;
            while (retries > 0)
            {
                messageWindow.SetActive(true);
                yield return new WaitForEndOfFrame();
                messageWindow.SetActive(false);
                yield return new WaitForEndOfFrame();
                retries--;
            }
            
            yield return new WaitForEndOfFrame();
            canvasGroup.alpha = 1;
            messageWindow.SetActive(true);
            AudioManager.Instance.PlaySFX(AudioChannelType.TV, audioClip);
        }
    }

    public interface IMessageWindow : IWindow
    {
        public Action OnMessageAccepted { get; set; }
        public void SetMessage(string message);
    }
}