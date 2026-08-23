using System;
using CorePatterns.ServiceLocator;
using Rulebound;
using UnityEngine;

namespace Rulebound
{
    public class TVNavigationController : MonoBehaviour, ITVNavigationController
    {
        [SerializeField] private TVInteractableObject tvInteractableObject;
        [SerializeField] private TVButton[] tvButtons;

        public TVInteractableObject TVInteractableObject => tvInteractableObject;
        public ITVButton PowerButton => tvButtons[0];
        public ITVButton OpenCloseButton => tvButtons[1];
        public ITVButton MenuButton =>  tvButtons[2];
        public ITVButton SubmitButton  =>  tvButtons[4];
        public ITVButton NextButton  =>  tvButtons[5];
        public ITVButton PlayPauseButton  =>  tvButtons[6];
        public ITVButton VolumeDownButton => tvButtons[7];
        public ITVButton VolumeUpButton => tvButtons[8];
        public Action OnPowerButtonPressed { get; set; }
        public Action OnOpenCloseButtonPressed { get; set; }
        public Action OnMenuButtonPressed { get; set; }
        public Action OnPreviousButtonPressed { get; set; }
        public Action OnSubmitButtonPressed { get; set; }
        public Action OnNextButtonPressed { get; set; }
        public Action OnPlayPauseButtonPressed { get; set; }
        public Action OnVolumeDownButtonPressed { get; set; }
        public Action OnVolumeUpButtonPressed { get; set; }
        public Action OnNextButtonHeld { get; set; }
        public Action OnVolumeUpButtonHeld { get; set; }
        public Action OnVolumeDownButtonHeld { get; set; }
        public Action OnNextButtonReleased { get; set; }
        public Action OnVolumeUpButtonReleased { get; set; }
        public Action OnVolumeDownButtonReleased { get; set; }
        
        public TVButton[] TvButtons => tvButtons;


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
                tvButton.OnTvButtonHeld += HandleButtonHeld;
                tvButton.OnTvButtonReleased += HandleButtonReleased;
                tvButton.EnableButton();
            }
        }

        public void EnableButtons()
        {
            foreach (ITVButton tvButton in tvButtons)
            {
                tvButton.EnableButton();
            }
        }

        public void DisableButtons()
        {
            foreach (ITVButton tvButton in tvButtons)
            {
                tvButton.DisableButton();
            }
        }

        private void OnDestroy()
        {
            foreach (ITVButton tvButton in tvButtons)
            {
                tvButton.OnTvButtonPressed -= HandleButtonPressed;
                tvButton.OnTvButtonHeld -= HandleButtonHeld;
                tvButton.OnTvButtonReleased -= HandleButtonReleased;
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
        
        private void HandleButtonHeld(int buttonId)
        {
            switch (buttonId)
            {
                //Next Button
                case 5:
                    OnNextButtonHeld?.Invoke();
                    break;
                //Volume Down Button
                case 7:
                    OnVolumeDownButtonHeld?.Invoke();
                    break;
                //Volume Up Button
                case 8:
                    OnVolumeUpButtonHeld?.Invoke();
                    break;
            }
        }
        
        private void HandleButtonReleased(int buttonId)
        {
            switch (buttonId)
            {
                //Next Button
                case 5:
                    OnNextButtonReleased?.Invoke();
                    break;
                //Volume Down Button
                case 7:
                    OnVolumeDownButtonReleased?.Invoke();
                    break;
                //Volume Up Button
                case 8:
                    OnVolumeUpButtonReleased?.Invoke();
                    break;
            }
        }
    }
}

public interface ITVNavigationController
{
    public TVInteractableObject TVInteractableObject { get; }
    public ITVButton PowerButton { get; }
    public ITVButton OpenCloseButton { get; }
    public ITVButton MenuButton { get; }
    public ITVButton SubmitButton { get; }
    public ITVButton NextButton { get; }
    public ITVButton PlayPauseButton { get; }
    public ITVButton VolumeUpButton { get; }
    public ITVButton VolumeDownButton { get; }
    public Action OnPowerButtonPressed { get; set; }
    public Action OnOpenCloseButtonPressed { get; set; }
    public Action OnMenuButtonPressed { get; set; }
    public Action OnPreviousButtonPressed { get; set; }
    public Action OnSubmitButtonPressed { get; set; }
    public Action OnNextButtonPressed { get; set; }
    public Action OnPlayPauseButtonPressed { get; set; }
    public Action OnVolumeDownButtonPressed { get; set; }
    public Action OnVolumeUpButtonPressed { get; set; }
    public Action OnNextButtonHeld { get; set; }
    public Action OnVolumeUpButtonHeld { get; set; }
    public Action OnVolumeDownButtonHeld { get; set; }
    public Action OnNextButtonReleased { get; set; }
    public Action OnVolumeUpButtonReleased { get; set; }
    public Action OnVolumeDownButtonReleased{ get; set; }
    
    public TVButton[] TvButtons { get; }

    public void EnableButtons();
    public void DisableButtons();
}