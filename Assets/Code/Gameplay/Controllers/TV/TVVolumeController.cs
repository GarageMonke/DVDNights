using System;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class TVVolumeController : MonoBehaviour, ITVVolumeController
    {
        [Header("References")] 
        [SerializeField] private VolumeWindow tvVolumeWindow;
        
        [Header("Configuration")]
        [SerializeField] private float hideDelay = 3f;
        [SerializeField] private int minVolume = 0;
        [SerializeField] private int maxVolume = 99;
        
        private ITVNavigationController _tvNavigationController;
        private Tween _hideDelayTween;
        private Tween _holdRepeatTween;

        private int _currentVolume;
        private bool _isHolding;

        public Action OnVolumeChanged { get; set; }
        public bool IsHolding => _isHolding;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<ITVVolumeController>(this);
            SetVolume((int)AudioManager.Instance.GetChannelVolume(AudioChannelType.TV));
            tvVolumeWindow.SetVolumeLimits(minVolume, maxVolume);
        }

        private void Start()
        {
            _tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();
            _tvNavigationController.OnVolumeUpButtonPressed += VolumeUp;
            _tvNavigationController.OnVolumeDownButtonPressed += VolumeDown;
            _tvNavigationController.OnVolumeUpButtonHeld += HoldVolumeUp;
            _tvNavigationController.OnVolumeDownButtonHeld += HoldVolumeDown;
            _tvNavigationController.OnVolumeUpButtonReleased += StopHold;
            _tvNavigationController.OnVolumeDownButtonReleased += StopHold;
            
            SetVolume(50);
        }
        

        public int GetVolume()
        {
            return tvVolumeWindow.GetCurrentFill();
        }

        public int GetMinVolume()
        {
            return minVolume;
        }

        public int GetMaxVolume()
        {
            return maxVolume;
        }

        public void VolumeUp()
        {
            ShowAndScheduleHide();
            tvVolumeWindow.VolumeUp();
            _currentVolume++;

            if (_currentVolume > maxVolume)
            {
                _currentVolume = maxVolume;
            }
            
            AudioManager.Instance.SetChannelVolume(AudioChannelType.TV, _currentVolume);
            
            OnVolumeChanged?.Invoke();
        }

        public void VolumeDown()
        {
            ShowAndScheduleHide();
            tvVolumeWindow.VolumeDown();
            
            _currentVolume--;

            if (_currentVolume < minVolume)
            {
                _currentVolume = minVolume;
            }
            
            AudioManager.Instance.SetChannelVolume(AudioChannelType.TV, _currentVolume);
         
            OnVolumeChanged?.Invoke();
        }

        public void SetVolume(int volume)
        {
            tvVolumeWindow.SetVolume(volume);
            AudioManager.Instance.SetChannelVolume(AudioChannelType.TV, tvVolumeWindow.GetCurrentFill());
        }
        
        private void ShowAndScheduleHide()
        {
            if (!tvVolumeWindow.IsDisplaying)
            {
                tvVolumeWindow.Display();
            }

            _hideDelayTween?.Kill();
            _hideDelayTween = DOVirtual.DelayedCall(hideDelay, () => tvVolumeWindow.Hide());
        }
        
        public void HoldVolumeUp()
        {
            StartHoldLoop(VolumeUp);
        }
        
        public void HoldVolumeDown()
        {
            StartHoldLoop(VolumeDown);
        }

        public void EnableController()
        {
            _tvNavigationController.VolumeUpButton.EnableButton();
            _tvNavigationController.VolumeDownButton.EnableButton();
        }

        public void DisableController()
        {
            _tvNavigationController.VolumeUpButton.DisableButton();
            _tvNavigationController.VolumeDownButton.DisableButton();
        }

        private void StartHoldLoop(Action volumeAction)
        {
            if (_holdRepeatTween != null && _holdRepeatTween.IsActive())
            {
                return;
            }
            
            _isHolding = true;

            if (!tvVolumeWindow.IsDisplaying)
            {
                tvVolumeWindow.Display();
            }

            _holdRepeatTween = DOVirtual.DelayedCall(0.1f, () =>
                {
                    volumeAction();
                    _hideDelayTween?.Kill();
                }, ignoreTimeScale: false)
                .SetLoops(-1);
        }

        public void StopHold()
        {
            _isHolding = false;
            _holdRepeatTween?.Kill();
            _holdRepeatTween = null;
            ShowAndScheduleHide(); 
        }
        
        private void OnDestroy()
        {
            _hideDelayTween?.Kill();
            _holdRepeatTween?.Kill();
            _tvNavigationController.OnVolumeUpButtonPressed -= VolumeUp;
            _tvNavigationController.OnVolumeDownButtonPressed -= VolumeDown;
            _tvNavigationController.OnVolumeUpButtonHeld -= HoldVolumeUp;
            _tvNavigationController.OnVolumeDownButtonHeld -= HoldVolumeDown;
            _tvNavigationController.OnVolumeUpButtonReleased -= ShowAndScheduleHide;
            _tvNavigationController.OnVolumeDownButtonReleased -= ShowAndScheduleHide;
        }
    }

    public interface ITVVolumeController
    {
        public Action OnVolumeChanged { get; set; }
        public bool IsHolding { get; }
        public int GetVolume();
        public int GetMinVolume();
        public int GetMaxVolume();
        public void VolumeUp();
        public void VolumeDown();
        public void SetVolume(int volume);
        public void HoldVolumeUp();
        public void HoldVolumeDown();
        public void StopHold();
        public void EnableController();
        public void DisableController();
    }
}