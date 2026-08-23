using System;
using Common;
using CorePatterns.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Rulebound
{
    public class PauseWindow : Window
    {
        [Header("References")] 
        [SerializeField] private Button resumeButton;
        [SerializeField] private SettingsAccessPoint settingsAccessPoint;
        [SerializeField] private Button exitGameButton;
        
        public Action OnResumePressed;
        
        protected override void Awake()
        {
            base.Awake();
            resumeButton.onClick.AddListener(RaiseResumePressed);
            exitGameButton.onClick.AddListener(ExitGame);
        }

        private void ExitGame()
        {
            Application.Quit();
        }

        private void RaiseResumePressed()
        {
            OnResumePressed?.Invoke();
        }

        public override void Close()
        {
            WindowManager.Instance.CloseWindow<PauseWindow>();
        }
    }
}