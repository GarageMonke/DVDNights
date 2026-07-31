using Code.Common.Persistence;
using Code.MVP;

namespace CorePatterns.Managers
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Audio;

    public class SettingsManager : Manager<SettingsManager>
    {
        [Header("Audio")] 
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private string masterVolumeParam = "MasterVolume";
        [SerializeField] private string sfxVolumeParam = "SFXVolume";
        [SerializeField] private string ostVolumeParam = "OSTVolume";

        [Header("Persistence")] 
        [SerializeField] private string playerPrefsKey = "GameSettings";
        

        public Action<SettingsData> OnSettingsChanged;
        
        public Action<float> OnMasterVolumeChanged;
        public Action<float> OnOstVolumeChanged;
        public Action<float> OnSfxVolumeChanged;
        
        public Action<string> OnLanguageChanged;
        
        public Action OnProgressDeleted;

        public readonly int[] AvailableFPSLimits = { 30, 60, 90, 120, 144, 165, 240, -1};
        public SettingsData Current { get; private set; } = new();

        public IReadOnlyList<Resolution> AvailableResolutions { get; private set; }
        public IReadOnlyList<string> AvailableQualityLevels => QualitySettings.names;

        private const float MinDb = -80f;

        protected override void Awake()
        {
            base.Awake();

            AvailableResolutions = Screen.resolutions;
            LoadSettings();
            ApplyAll();
        }

        // ---------------------------------------------------------------------
        // DISPLAY
        // ---------------------------------------------------------------------

        public void SetResolution(int resolutionIndex)
        {
            if (resolutionIndex < 0 || resolutionIndex >= AvailableResolutions.Count) return;

            Current.resolutionIndex = resolutionIndex;
            var res = AvailableResolutions[resolutionIndex];
            Screen.SetResolution(res.width, res.height, ToUnityFullscreenMode(Current.fullscreenMode),
                res.refreshRateRatio);

            SaveSettings();
            OnSettingsChanged?.Invoke(Current);
        }

        public void SetFullscreenMode(FullScreenModeOption mode)
        {
            Current.fullscreenMode = mode;
            Screen.fullScreenMode = ToUnityFullscreenMode(mode);

            SaveSettings();
            OnSettingsChanged?.Invoke(Current);
        }

        private static FullScreenMode ToUnityFullscreenMode(FullScreenModeOption option)
        {
            return option switch
            {
                FullScreenModeOption.ExclusiveFullscreen => FullScreenMode.ExclusiveFullScreen,
                FullScreenModeOption.FullscreenWindow => FullScreenMode.FullScreenWindow,
                FullScreenModeOption.MaximizedWindow => FullScreenMode.MaximizedWindow,
                FullScreenModeOption.Windowed => FullScreenMode.Windowed,
                _ => FullScreenMode.FullScreenWindow
            };
        }
        
        public void SetVSync(bool enableVSync)
        {
            Current.vSyncEnabled = enableVSync;
            QualitySettings.vSyncCount = enableVSync ? 1 : 0;

            // Re-apply FPS limit since it only takes effect when vSync is off
            Application.targetFrameRate = enableVSync ? -1 : AvailableFPSLimits[Current.fpsLimitIndex];

            SaveSettings();
            OnSettingsChanged?.Invoke(Current);
        }
        
        public void SetFPSLimitByIndex(int index)
        {
            if (index < 0 || index >= AvailableFPSLimits.Length) return;

            Current.fpsLimitIndex = index;
            SetFPSLimit(AvailableFPSLimits[index]);
        }

        private void SetFPSLimit(int fps)
        {
            if (!Current.vSyncEnabled)
            {
                Application.targetFrameRate = fps > 0 ? fps : -1;
            }

            SaveSettings();
            OnSettingsChanged?.Invoke(Current);
        }
        
        public void ApplyDisplaySettings()
        {
            if (Current.resolutionIndex == -1)
            {
                Resolution resolution = AvailableResolutions[^1];
                Screen.SetResolution(resolution.width, resolution.height, ToUnityFullscreenMode(Current.fullscreenMode), resolution.refreshRateRatio);
                Current.resolutionIndex = AvailableResolutions.Count - 1;
            }
            else if (Current.resolutionIndex >= 0 && Current.resolutionIndex < AvailableResolutions.Count)
            {
                Resolution resolution = AvailableResolutions[Current.resolutionIndex];
                Screen.SetResolution(resolution.width, resolution.height, ToUnityFullscreenMode(Current.fullscreenMode), resolution.refreshRateRatio);
            }
            
            QualitySettings.vSyncCount = Current.vSyncEnabled ? 1 : 0;
            Application.targetFrameRate = Current.vSyncEnabled ? -1 : (Current.fpsLimitIndex > 0 ? AvailableFPSLimits[Current.fpsLimitIndex] : -1);
            Screen.fullScreenMode = ToUnityFullscreenMode(Current.fullscreenMode);
        }
        
        // ---------------------------------------------------------------------
        // INPUT
        // ---------------------------------------------------------------------

        public void SetMouseSensitivity(float sensitivity)
        {
            Current.mouseSensitivity = sensitivity;
            SaveSettings();
            OnSettingsChanged?.Invoke(Current);
        }
        
        public void InvertMouseX(bool invertX)
        {
            Current.mouseInvertX = invertX;
            SaveSettings();
            OnSettingsChanged?.Invoke(Current);
        }
        
        public void InvertMouseY(bool invertY)
        {
            Current.mouseInvertY = invertY;
            SaveSettings();
            OnSettingsChanged?.Invoke(Current);
        }
        
        // ---------------------------------------------------------------------
        // GRAPHICS / URP QUALITY
        // ---------------------------------------------------------------------

        public void SetQualityLevel(int qualityIndex)
        {
            if (qualityIndex < 0 || qualityIndex >= QualitySettings.names.Length) return;

            Current.qualityLevel = qualityIndex;
            QualitySettings.SetQualityLevel(qualityIndex, true);

            SaveSettings();
            OnSettingsChanged?.Invoke(Current);
        }
        
        private void ApplyQualitySettings()
        {
            if (Current.qualityLevel >= 0)
            {
                QualitySettings.SetQualityLevel(Current.qualityLevel, true);
            }
        }


        // ---------------------------------------------------------------------
        // AUDIO
        // ---------------------------------------------------------------------

        public void SetMasterVolume(float linear01)
        {
            Current.masterVolume = Mathf.Clamp01(linear01);
            ApplyVolume(masterVolumeParam, Current.masterVolume);

            SaveSettings();
            OnMasterVolumeChanged?.Invoke(Current.masterVolume);
            OnSettingsChanged?.Invoke(Current);
        }
        
        public void SetOSTVolume(float linear01)
        {
            Current.ostVolume = Mathf.Clamp01(linear01);
            ApplyVolume(ostVolumeParam, Current.ostVolume);

            SaveSettings();
            OnOstVolumeChanged?.Invoke(Current.ostVolume);
            OnSettingsChanged?.Invoke(Current);
        }

        public void SetSfxVolume(float linear01)
        {
            Current.sfxVolume = Mathf.Clamp01(linear01);
            ApplyVolume(sfxVolumeParam, Current.sfxVolume);

            SaveSettings();
            OnSfxVolumeChanged?.Invoke(Current.sfxVolume);
            OnSettingsChanged?.Invoke(Current);
        }

        private void ApplyVolume(string param, float linear01)
        {
            if (audioMixer == null || string.IsNullOrEmpty(param)) return;

            float db = linear01 <= 0.0001f ? MinDb : Mathf.Log10(linear01) * 20f;
            audioMixer.SetFloat(param, db);
        }

        // ---------------------------------------------------------------------
        // LANGUAGE
        // ---------------------------------------------------------------------


        public void SetLanguage(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode)) return;

            Current.languageCode = languageCode;

            SaveSettings();
            OnLanguageChanged?.Invoke(languageCode);
            OnSettingsChanged?.Invoke(Current);
        }

        // ---------------------------------------------------------------------
        // PERSISTENCE
        // ---------------------------------------------------------------------

        public void SaveSettings()
        {
            string json = JsonUtility.ToJson(Current);
            PlayerPrefs.SetString(playerPrefsKey, json);
            PlayerPrefs.Save();
        }

        public void LoadSettings()
        {
            //Temporal
            PlayerPrefs.DeleteAll();
            
            if (PlayerPrefs.HasKey(playerPrefsKey))
            {
                string json = PlayerPrefs.GetString(playerPrefsKey);
                Current = JsonUtility.FromJson<SettingsData>(json) ?? new SettingsData();
            }
            else
            {
                Current = new SettingsData();
            }
        }

        public void ApplyAll()
        {
            ApplyDisplaySettings();
            ApplyQualitySettings();

            ApplyVolume(masterVolumeParam, Current.masterVolume);
            ApplyVolume(sfxVolumeParam, Current.sfxVolume);
            ApplyVolume(ostVolumeParam, Current.ostVolume);

            OnSettingsChanged?.Invoke(Current);
        }
        
        public void ResetToDefaults()
        {
            Current.ResetToDefaults();
            ApplyAll();
            SaveSettings();
        }

        // ---------------------------------------------------------------------
        // DELETE PROGRESS
        // ---------------------------------------------------------------------
        
        public void DeleteProgress()
        {
            // Example if using file-based saves:
            // string path = System.IO.Path.Combine(Application.persistentDataPath, "save.dat");
            // if (System.IO.File.Exists(path)) System.IO.File.Delete(path);

            PlayerPrefs.Save();
            OnProgressDeleted?.Invoke();
        }
    }
}