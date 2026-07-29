using System;
using System.Collections.Generic;
using Code.Common.Persistence;
using Common;
using CorePatterns.Managers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Code.MVP
{
    public class SettingsWindow : Window
    {
        [Header("Display")] 
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Toggle fullscreenToggle;

        [Header("Graphics")] 
        [SerializeField] private TMP_Dropdown qualityDropdown;

        [Header("Audio")] 
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider ostVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        [Header("Language")] 
        [SerializeField] private TMP_Dropdown languageDropdown;
        [SerializeField] private List<string> supportedLanguageCodes = new() { "en", "es", "fr", "de", "ja" };

        [Header("Danger Zone")] 
        [SerializeField] private Button deleteProgressButton;


        private void Awake()
        {
            deleteProgressButton.onClick.AddListener(OnDeleteProgressClicked);
        }

        private void OnEnable()
        {
            PopulateDropdowns();
            RefreshFromCurrentSettings();
            SettingsManager.Instance.OnSettingsChanged += HandleSettingsChanged;
        }

        public override void Hide()
        {
            base.Hide();
            SettingsManager.Instance.OnSettingsChanged -= HandleSettingsChanged;
        }
        

        private void PopulateDropdowns()
        {
            SettingsManager settingsManager = SettingsManager.Instance;

            // Resolutions
            resolutionDropdown.ClearOptions();
            List<string> resolutionOptions = new List<string>();

            foreach (Resolution resolution in settingsManager.AvailableResolutions)
            {
                resolutionOptions.Add($"{resolution.width} x {resolution.height} @ {resolution.refreshRateRatio.value:0}Hz");
            }

            resolutionDropdown.AddOptions(resolutionOptions);

            // Quality (URP levels defined in Project Settings > Quality)
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new List<string>(settingsManager.AvailableQualityLevels));

            // Language
            languageDropdown.ClearOptions();
            languageDropdown.AddOptions(supportedLanguageCodes);
        }

        private void RefreshFromCurrentSettings()
        {
            SettingsData data = SettingsManager.Instance.Current;

            resolutionDropdown.SetValueWithoutNotify(Mathf.Max(0, data.resolutionIndex));
            fullscreenToggle.SetIsOnWithoutNotify(data.fullscreenMode != FullScreenModeOption.Windowed);
            qualityDropdown.SetValueWithoutNotify(Mathf.Max(0, data.qualityLevel));
            masterVolumeSlider.SetValueWithoutNotify(data.masterVolume);
            sfxVolumeSlider.SetValueWithoutNotify(data.sfxVolume);
            ostVolumeSlider.SetValueWithoutNotify(data.ostVolume);

            int langIndex = supportedLanguageCodes.IndexOf(data.languageCode);
            languageDropdown.SetValueWithoutNotify(Mathf.Max(0, langIndex));
        }

        private void HandleSettingsChanged(SettingsData data) => RefreshFromCurrentSettings();

        public void OnResolutionChanged(int index) => SettingsManager.Instance.SetResolution(index);

        public void OnFullscreenToggled(bool isFullscreen) => SettingsManager.Instance.SetFullscreenMode(isFullscreen
                ? FullScreenModeOption.FullscreenWindow
                : FullScreenModeOption.Windowed);

        public void OnQualityChanged(int index) => SettingsManager.Instance.SetQualityLevel(index);

        public void OnMasterVolumeChanged(float value) => SettingsManager.Instance.SetMasterVolume(value);

        public void OnSfxVolumeChanged(float value) => SettingsManager.Instance.SetSfxVolume(value);

        public void OnLanguageChanged(int index) => SettingsManager.Instance.SetLanguage(supportedLanguageCodes[index]);

        public void OnDeleteProgressClicked()
        {
            //Open Confirm Dialog and Register to It
        }

        public void OnDeleteProgressConfirmed()
        {
            SettingsManager.Instance.DeleteProgress();
            //Close Confirm dialog?
        }

        public void OnDeleteProgressCancelled()
        {
            //Close Confirm dialog
        }
    }
}