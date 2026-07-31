namespace Code.Common.Persistence
{
    using System;
    
    [Serializable]
    public class SettingsData
    {
        // --- Display ---
        public int resolutionIndex;
        public bool isFullscreen = true;
        public bool vSyncEnabled;
        public int fpsLimitIndex;
        public FullScreenModeOption fullscreenMode = FullScreenModeOption.FullscreenWindow;

        // --- Graphics (URP) ---
        public int qualityLevel;
        
        // --- Input ---
        public float mouseSensitivity;
        public bool mouseInvertX;
        public bool mouseInvertY;

        // --- Audio ---
        // Stored as linear 0-1 values (what a UI slider gives you).
        public float masterVolume = 1f;
        public float sfxVolume = 1f;
        public float ostVolume = 1f;

        // --- Localization ---
        public string languageCode = "en";

        public SettingsData()
        {
            ResetToDefaults();
        }

        public SettingsData(SettingsData overrideSettingsData)
        {
            resolutionIndex = overrideSettingsData.resolutionIndex;
            isFullscreen = overrideSettingsData.isFullscreen;
            vSyncEnabled = overrideSettingsData.vSyncEnabled;
            fpsLimitIndex = overrideSettingsData.fpsLimitIndex;
            fullscreenMode = overrideSettingsData.fullscreenMode;

            mouseSensitivity = overrideSettingsData.mouseSensitivity;
            mouseInvertX = overrideSettingsData.mouseInvertX;
            mouseInvertY = overrideSettingsData.mouseInvertY;
            
            qualityLevel = overrideSettingsData.qualityLevel;
            
            masterVolume =overrideSettingsData.masterVolume;
            sfxVolume = overrideSettingsData.sfxVolume;
            ostVolume = overrideSettingsData.ostVolume;
            
            languageCode = overrideSettingsData.languageCode;
        }

        public void ResetToDefaults()
        {
            resolutionIndex = -1;
            isFullscreen = true;
            vSyncEnabled = false;
            fpsLimitIndex = 1;
            fullscreenMode = FullScreenModeOption.FullscreenWindow;

            mouseSensitivity = 1;
            mouseInvertY = false;
            mouseInvertX = false;
            
            qualityLevel = 0;
            masterVolume = 0.75f;
            sfxVolume = 0.75f;
            ostVolume = 0.75f;
            languageCode = "en";
        }
    }

    public enum FullScreenModeOption
    {
        ExclusiveFullscreen,
        FullscreenWindow,
        MaximizedWindow,
        Windowed
    }
}