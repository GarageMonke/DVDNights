namespace Code.Common.Persistence
{
    using System;
    
    [Serializable]
    public class SettingsData
    {
        // --- Display ---
        public int resolutionIndex = -1;
        public bool isFullscreen = true;
        public FullScreenModeOption fullscreenMode = FullScreenModeOption.FullscreenWindow;

        // --- Graphics (URP) ---
        public int qualityLevel = -1; 

        // --- Audio ---
        // Stored as linear 0-1 values (what a UI slider gives you).
        public float masterVolume = 1f;
        public float sfxVolume = 1f;
        public float ostVolume = 1f;

        // --- Localization ---
        public string languageCode = "en";

        public SettingsData Clone()
        {
            return (SettingsData)MemberwiseClone();
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