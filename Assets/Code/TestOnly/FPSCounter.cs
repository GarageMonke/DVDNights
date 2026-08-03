namespace Code.TestOnly
{
    using UnityEngine;
    using TMPro;

    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text fpsLabel;
        [SerializeField] private float updateInterval = 0.5f;

        private float _accumulatedTime;
        private int _frameCount;
        private float _currentFps;

        private void Update()
        {
            _accumulatedTime += Time.unscaledDeltaTime;
            _frameCount++;

            if (_accumulatedTime >= updateInterval)
            {
                _currentFps = _frameCount / _accumulatedTime;
                fpsLabel.text = $"{_currentFps:0} FPS";

                _accumulatedTime = 0f;
                _frameCount = 0;
            }
        }
    }
}