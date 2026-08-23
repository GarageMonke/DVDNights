using System;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rulebound
{
    public class PauseController : MonoBehaviour, IPauseController
    {
        [Header("References")]
        [SerializeField] private InputActionSO pauseInputActionSO;
        
        private InputAction _pauseInputAction;
        
        public bool IsPaused => _isPaused;
        public Action<bool> OnPauseStateChanged { get; set; }

        private bool _isPaused;

        private PauseWindow _pauseWindow;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IPauseController>(this);

            _pauseInputAction = pauseInputActionSO.GetInputAction();
            _pauseInputAction.performed += TogglePause;
        }

        private void OnDestroy()
        {
            _pauseInputAction.performed -= TogglePause;
        }

        private void TogglePause(InputAction.CallbackContext context)
        {
            if (_isPaused)
            {
                Resume();
                return;
            }
            
            Pause();
        }

        public void Pause()
        {
            if (IsPaused) return;

            _isPaused = true;
            
            Time.timeScale = 0f;

            AudioManager.Instance.PauseAllAudio(); 
            
            _pauseWindow = WindowManager.Instance.OpenWindow<PauseWindow>(gameObject, openInContainer: true);
            _pauseWindow.OnResumePressed += Resume;
            
            OnPauseStateChanged?.Invoke(true);
        }

        public void Resume()
        {
            if (!IsPaused)
            {
                return;
            }

            _isPaused = false;
            
            AudioManager.Instance.ResumeAllAudio();
            
            Time.timeScale = 1f;
            
            _pauseWindow.OnResumePressed -= Resume;
            _pauseWindow.Close();
            _pauseWindow = null;

            OnPauseStateChanged?.Invoke(false);
        }
    }

    public interface IPauseController
    {
        public bool IsPaused { get; }
        public Action<bool> OnPauseStateChanged { get; set; }
        public void Pause();
        public void Resume();
    }
}