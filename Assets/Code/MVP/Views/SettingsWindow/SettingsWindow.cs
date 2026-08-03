using System.Collections.Generic;
using Code.Common.Localization;
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
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown fpsDropdown;
        [SerializeField] private Toggle vSyncToggle;

        [Header("Graphics")] 
        [SerializeField] private TMP_Dropdown qualityDropdown;

        [Header("Input")] 
        [SerializeField] private Slider mouseSensitivitySlider;
        [SerializeField] private Toggle invertMouseXToggle;
        [SerializeField] private Toggle invertMouseYToggle;

        [Header("Audio")] 
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider ostVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        [Header("Language")] 
        [SerializeField] private LanguageDropdown languageDropdown;

        [Header("Danger Zone")] 
        [SerializeField] private Button deleteProgressButton;
        [SerializeField] private Button restoreToDefaultsButton;

        [Header("ScrollView")] 
        [SerializeField] private ScrollRect scrollRect;
        
        protected override void Awake()
        {
            base.Awake();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            //Display
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
            fpsDropdown.onValueChanged.AddListener(OnFpsLimitChanged);
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
            vSyncToggle.onValueChanged.AddListener(OnVSyncToggled);
            
            //Input
            invertMouseXToggle.onValueChanged.AddListener(OnMouseXToggled);
            invertMouseYToggle.onValueChanged.AddListener(OnMouseYToggled);
            mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);
            
            //Quality
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
            
            //Localization
            languageDropdown.OnLanguageChanged += OnLanguageChanged;
            
            //Audio
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            ostVolumeSlider.onValueChanged.AddListener(OnOSTVolumeChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            
            //Danger Zone
            deleteProgressButton.onClick.AddListener(OnDeleteProgressClicked);
            restoreToDefaultsButton.onClick.AddListener(OnResetToDefaultsClicked);
        }

        public override void Display()
        {
            base.Display();
            
            scrollRect.verticalNormalizedPosition = 1;
            
            PopulateDropdowns();
            RefreshFromCurrentSettings();
            SettingsManager.Instance.OnSettingsChanged += HandleSettingsChanged;
        }

        public override void Hide()
        {
            base.Hide();
            SettingsManager.Instance.OnSettingsChanged -= HandleSettingsChanged;
        }

        public override void Close()
        {
            WindowManager.Instance.CloseWindow<SettingsWindow>();
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
            
            //FPS
            fpsDropdown.ClearOptions();
            List<string> fpsOptions = new List<string>();
            
            foreach (int fpsLimit in settingsManager.AvailableFPSLimits)
            {
                if (fpsLimit > -1)
                {
                    fpsOptions.Add($"{fpsLimit} FPS");
                    continue;
                }
                
                fpsOptions.Add("Unlimited");
            }

            fpsDropdown.AddOptions(fpsOptions);

            // Quality (URP levels defined in Project Settings > Quality)
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new List<string>(settingsManager.AvailableQualityLevels));
        }

        private void RefreshFromCurrentSettings()
        {
            SettingsData data = SettingsManager.Instance.Current;

            //Display
            resolutionDropdown.SetValueWithoutNotify(Mathf.Max(0, data.resolutionIndex));
            fpsDropdown.SetValueWithoutNotify(Mathf.Max(0, data.fpsLimitIndex));
            fullscreenToggle.SetIsOnWithoutNotify(data.fullscreenMode != FullScreenModeOption.Windowed);
            vSyncToggle.SetIsOnWithoutNotify(data.vSyncEnabled);
            
            //Input
            mouseSensitivitySlider.SetValueWithoutNotify(data.mouseSensitivity);
            invertMouseXToggle.SetIsOnWithoutNotify(data.mouseInvertX);
            invertMouseYToggle.SetIsOnWithoutNotify(data.mouseInvertY);
            
            //Quality
            qualityDropdown.SetValueWithoutNotify(Mathf.Max(0, data.qualityLevel));
            
            //Audio
            masterVolumeSlider.SetValueWithoutNotify(data.masterVolume);
            sfxVolumeSlider.SetValueWithoutNotify(data.sfxVolume);
            ostVolumeSlider.SetValueWithoutNotify(data.ostVolume);
        }

        private void HandleSettingsChanged(SettingsData data) => RefreshFromCurrentSettings();

        //DISPLAY
        private void OnResolutionChanged(int index) => SettingsManager.Instance.SetResolution(index);

        private void OnFullscreenToggled(bool isFullscreen) => SettingsManager.Instance.SetFullscreenMode(isFullscreen ? FullScreenModeOption.FullscreenWindow : FullScreenModeOption.Windowed);

        private void OnVSyncToggled(bool vSyncEnabled)
        {
            SettingsManager.Instance.SetVSync(vSyncEnabled);
            fpsDropdown.interactable = !vSyncEnabled;
        }

        private void OnFpsLimitChanged(int index) => SettingsManager.Instance.SetFPSLimitByIndex(index);
        
        //INPUT

        private void OnMouseSensitivityChanged(float arg0) => SettingsManager.Instance.SetMouseSensitivity(arg0);

        private void OnMouseXToggled(bool invertMouseX) => SettingsManager.Instance.InvertMouseX(invertMouseX);
        private void OnMouseYToggled(bool invertMouseY) => SettingsManager.Instance.InvertMouseY(invertMouseY);

        private void OnQualityChanged(int index) => SettingsManager.Instance.SetQualityLevel(index);

        private void OnMasterVolumeChanged(float value) => SettingsManager.Instance.SetMasterVolume(value);
        private void OnOSTVolumeChanged(float value) => SettingsManager.Instance.SetOSTVolume(value);

        private void OnSfxVolumeChanged(float value) => SettingsManager.Instance.SetSfxVolume(value);

        private void OnLanguageChanged(string languageCode)
        {
            SettingsManager.Instance.SetLanguage(languageCode);
        }

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
        
        public void OnResetToDefaultsClicked()
        {
            SettingsManager.Instance.ResetToDefaults();
            RefreshFromCurrentSettings();
        }
    }
}