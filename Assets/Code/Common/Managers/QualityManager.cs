using UnityEngine;

public class QualityManager : MonoBehaviour
{
    private int _defaultFrameRate;
    private readonly int[] _fpsSteps = { 15, 30, 60, 120 };
    
    // Define the three options: High, Medium, and Low
    public enum QualityOption
    {
        High,
        Medium,
        Low
    }

    // Define the resolution and frame rate settings for each option
    public struct QualityConfig
    {
        public readonly int Width;
        public readonly int Height;
        public readonly int FrameRate;

        public QualityConfig(int width, int height, int frameRate)
        {
            Width = width;
            Height = height;
            FrameRate = frameRate;
        }
    }

    // Specify the quality settings for each option
    private static QualityManager _instance;
    private QualityConfig _highConfig;
    private QualityConfig _mediumConfig;
    private QualityConfig _lowConfig;
    
    public static QualityManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<QualityManager>();
            }

            return _instance;
        }
    }
    
    private void Start()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        AutoDetectPerformance();
#endif
    }

    // Check if the device is considered high performance
    public bool IsHighPerformanceDevice()
    {
        // You can customize the criteria based on your requirements
        return SystemInfo.processorCount >= 6 &&
               SystemInfo.systemMemorySize >= 4096;
    }

    bool IsMidPerformanceDevice()
    {
        return SystemInfo.processorCount >= 4 && SystemInfo.systemMemorySize >= 2048;
    }

    private void AutoDetectPerformance()
    {
        var highWidth = Mathf.FloorToInt(Screen.width * 0.75f);
        var highHeight = Mathf.FloorToInt(Screen.height * 0.75f);

        var midWidth = Mathf.FloorToInt(Screen.width * 0.5f);
        var midHeight = Mathf.FloorToInt(Screen.height * 0.5f);

        var lowWidth = Mathf.FloorToInt(Screen.width * 0.25f);
        var lowHeight = Mathf.FloorToInt(Screen.height * 0.25f);

        _highConfig = new QualityConfig(highWidth, highHeight, 60);
        _mediumConfig = new QualityConfig(midWidth, midHeight, 45);
        _lowConfig = new QualityConfig(lowWidth, lowHeight, 30);

        // Determine the resolution and frame rate option based on the device's specifications
        QualityOption selectedOption;

        if (IsHighPerformanceDevice())selectedOption = QualityOption.High;
        else if (IsMidPerformanceDevice())
        {
            selectedOption = QualityOption.Medium;
            QualitySettings.SetQualityLevel(1);
        }
        else
        {
            selectedOption = QualityOption.Low;
            QualitySettings.SetQualityLevel(2);
        }

        // Set the resolution and frame rate based on the selected option
        QualityConfig config;
        switch (selectedOption)
        {
            case QualityOption.High:
                config = _highConfig;
                break;
            case QualityOption.Medium:
                config = _mediumConfig;
                break;
            case QualityOption.Low:
                config = _lowConfig;
                break;
            default:
                config = _mediumConfig; // Default to medium settings
                break;
        }

        Screen.SetResolution(config.Width, config.Height, true);
        
        if (PlayerPrefs.GetInt("refreshRate") > 120)
        {
            Application.targetFrameRate = 121;
            return;
        }
        
        Application.targetFrameRate = config.FrameRate;
    }
    
    public void BoostPerformance()
    {
        // Determine the resolution and frame rate option based on the device's specifications
        QualityOption selectedOption;

        if (IsHighPerformanceDevice()) selectedOption = QualityOption.High;
        else if (IsMidPerformanceDevice())
        {
            selectedOption = QualityOption.Medium;
        }
        else
        {
            selectedOption = QualityOption.Low;
        }

        // Set the resolution and frame rate based on the selected option
        QualityConfig config;
        switch (selectedOption)
        {
            case QualityOption.High:
                config = _highConfig;
                break;
            case QualityOption.Medium:
                config = _mediumConfig;
                break;
            case QualityOption.Low:
                config = _lowConfig;
                break;
            default:
                config = _mediumConfig; // Default to medium settings
                break;
        }
        
        if (PlayerPrefs.GetInt("refreshRate") > 120)
        {
            Application.targetFrameRate = 121;
            return;
        }
        
        Application.targetFrameRate = config.FrameRate;
    }
}