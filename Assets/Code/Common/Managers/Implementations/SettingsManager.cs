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
        public Action<float> OnSfxVolumeChanged;
        public Action<float> OnMusicVolumeChanged;
        public Action<string> OnLanguageChanged;
        public Action OnProgressDeleted;

        public SettingsData Current { get; private set; } = new();

        public IReadOnlyList<Resolution> AvailableResolutions { get; private set; }
        public IReadOnlyList<string> AvailableQualityLevels => QualitySettings.names;

        private const float MinDb = -80f;

        protected override void Awake()
        {
            base.Awake();

            AvailableResolutions = Screen.resolutions;
            Load();
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

            Save();
            OnSettingsChanged?.Invoke(Current);
        }

        public void SetFullscreenMode(FullScreenModeOption mode)
        {
            Current.fullscreenMode = mode;
            Screen.fullScreenMode = ToUnityFullscreenMode(mode);

            Save();
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

        // ---------------------------------------------------------------------
        // GRAPHICS / URP QUALITY
        // ---------------------------------------------------------------------

        public void SetQualityLevel(int qualityIndex)
        {
            if (qualityIndex < 0 || qualityIndex >= QualitySettings.names.Length) return;

            Current.qualityLevel = qualityIndex;
            QualitySettings.SetQualityLevel(qualityIndex, true);

            Save();
            OnSettingsChanged?.Invoke(Current);
        }

        // ---------------------------------------------------------------------
        // AUDIO
        // ---------------------------------------------------------------------

        public void SetMasterVolume(float linear01)
        {
            Current.masterVolume = Mathf.Clamp01(linear01);
            ApplyVolume(masterVolumeParam, Current.masterVolume);

            Save();
            OnMasterVolumeChanged?.Invoke(Current.masterVolume);
            OnSettingsChanged?.Invoke(Current);
        }

        public void SetSfxVolume(float linear01)
        {
            Current.sfxVolume = Mathf.Clamp01(linear01);
            ApplyVolume(sfxVolumeParam, Current.sfxVolume);

            Save();
            OnSfxVolumeChanged?.Invoke(Current.sfxVolume);
            OnSettingsChanged?.Invoke(Current);
        }

        public void SetMusicVolume(float linear01)
        {
            Current.ostVolume = Mathf.Clamp01(linear01);
            ApplyVolume(ostVolumeParam, Current.ostVolume);

            Save();
            OnMusicVolumeChanged?.Invoke(Current.ostVolume);
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

            Save();
            OnLanguageChanged?.Invoke(languageCode);
            OnSettingsChanged?.Invoke(Current);
        }

        // ---------------------------------------------------------------------
        // PERSISTENCE
        // ---------------------------------------------------------------------

        public void Save()
        {
            string json = JsonUtility.ToJson(Current);
            PlayerPrefs.SetString(playerPrefsKey, json);
            PlayerPrefs.Save();
        }

        public void Load()
        {
            if (PlayerPrefs.HasKey(playerPrefsKey))
            {
                string json = PlayerPrefs.GetString(playerPrefsKey);
                Current = JsonUtility.FromJson<SettingsData>(json) ?? new SettingsData();
            }
            else
            {
                Current = new SettingsData
                {
                    qualityLevel = QualitySettings.GetQualityLevel(),
                    languageCode = Application.systemLanguage.ToString()[..2].ToLower()
                };
            }
        }

        public void ApplyAll()
        {
            if (Current.qualityLevel >= 0)
                QualitySettings.SetQualityLevel(Current.qualityLevel, true);

            if (Current.resolutionIndex >= 0 && Current.resolutionIndex < AvailableResolutions.Count)
            {
                var res = AvailableResolutions[Current.resolutionIndex];
                Screen.SetResolution(res.width, res.height, ToUnityFullscreenMode(Current.fullscreenMode),
                    res.refreshRateRatio);
            }
            else
            {
                Screen.fullScreenMode = ToUnityFullscreenMode(Current.fullscreenMode);
            }

            ApplyVolume(masterVolumeParam, Current.masterVolume);
            ApplyVolume(sfxVolumeParam, Current.sfxVolume);
            ApplyVolume(ostVolumeParam, Current.ostVolume);

            OnSettingsChanged?.Invoke(Current);
        }

        public void ResetToDefaults()
        {
            Current = new SettingsData
            {
                qualityLevel = QualitySettings.GetQualityLevel()
            };
            ApplyAll();
            Save();
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