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
        private IGameUIController _gameUIController;
        private bool _isEnabled;

        private void Awake()
        {
            InstallService();
        }

        private void Start()
        {
            _gameUIController = ServiceLocator.GetService<IGameUIController>();
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
            if (!_isEnabled)
            {
                return;
            }
            
            if (_isPaused)
            {
                if (IsPauseWindowOnTop())
                {
                    return;
                }
                
                Resume();
                return;
            }
            
            Pause();
        }

        private bool IsPauseWindowOnTop()
        {
            return WindowManager.Instance.HasOpenedWindows() && WindowManager.Instance.IsWindowOnTop<PauseWindow>();
        }

        public void Pause()
        {
            _isPaused = true;
            
            Time.timeScale = 0f;

            AudioManager.Instance.PauseAllAudio(); 
            
            _pauseWindow = WindowManager.Instance.OpenWindow<PauseWindow>(gameObject, openInContainer: true);
            _pauseWindow.OnResumePressed += Resume;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            _gameUIController.HideGameUI();
            
            OnPauseStateChanged?.Invoke(true);
        }

        public void Resume()
        {
            _isPaused = false;
            
            AudioManager.Instance.ResumeAllAudio();
            
            Time.timeScale = 1f;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            _pauseWindow.OnResumePressed -= Resume;
            _pauseWindow.Close();
            _pauseWindow = null;
            
            _gameUIController.DisplayGameUI();

            OnPauseStateChanged?.Invoke(false);
        }

        public void EnablePause()
        {
            _isEnabled = true;
        }

        public void DisablePause()
        {
            _isEnabled = false;
        }
    }

    public interface IPauseController
    {
        public bool IsPaused { get; }
        public Action<bool> OnPauseStateChanged { get; set; }
        public void Pause();
        public void Resume();

        public void EnablePause();
        public void DisablePause();
    }
}