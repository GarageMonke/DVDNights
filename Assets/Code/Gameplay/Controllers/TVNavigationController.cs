using System;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class TVNavigationController : MonoBehaviour, ITVNavigationController
    {
        [SerializeField] private TVButton[] tvButtons;
        
        public Action OnPowerButtonPressed { get; set; }
        public Action OnOpenCloseButtonPressed { get; set; }
        public Action OnMenuButtonPressed { get; set; }
        public Action OnPreviousButtonPressed { get; set; }
        public Action OnSubmitButtonPressed { get; set; }
        public Action OnNextButtonPressed { get; set; }
        public Action OnPlayPauseButtonPressed { get; set; }
        public Action OnVolumeDownButtonPressed { get; set; }
        public Action OnVolumeUpButtonPressed { get; set; }

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<ITVNavigationController>(this);
            RegisterButtons();
        }

        private void RegisterButtons()
        {
            foreach (ITVButton tvButton in tvButtons)
            {
                tvButton.OnTvButtonPressed += HandleButtonPressed;
                tvButton.EnableButton();
            }
        }

        private void HandleButtonPressed(int buttonId)
        {
            switch (buttonId)
            {
                //Power Button
                case 0:
                    OnPowerButtonPressed?.Invoke();
                    break;
                //Open/Close Button
                case 1:
                    OnOpenCloseButtonPressed?.Invoke();
                    break;
                //Menu Button
                case 2:
                    OnMenuButtonPressed?.Invoke();
                    break;
                //Previous Button
                case 3:
                    OnPreviousButtonPressed?.Invoke();
                    break;
                //Submit Button
                case 4:
                    OnSubmitButtonPressed?.Invoke();
                    break;
                //Next Button
                case 5:
                    OnNextButtonPressed?.Invoke();
                    break;
                //Play/Pause Button
                case 6:
                    OnPlayPauseButtonPressed?.Invoke();
                    break;
                //Volume Down Button
                case 7:
                    OnVolumeDownButtonPressed?.Invoke();
                    break;
                //Volume Up Button
                case 8:
                    OnVolumeUpButtonPressed?.Invoke();
                    break;
            }
        }
    }
}

public interface ITVNavigationController
{
    public Action OnPowerButtonPressed { get; set; }
    public Action OnOpenCloseButtonPressed { get; set; }
    public Action OnMenuButtonPressed { get; set; }
    public Action OnPreviousButtonPressed { get; set; }
    public Action OnSubmitButtonPressed { get; set; }
    public Action OnNextButtonPressed { get; set; }
    public Action OnPlayPauseButtonPressed { get; set; }
    public Action OnVolumeDownButtonPressed { get; set; }
    public Action OnVolumeUpButtonPressed { get; set; }
}