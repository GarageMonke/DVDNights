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
        
        private ITVNavigationController _tvNavigationController;
        private Tween _hideDelayTween;
        private Tween _holdRepeatTween;

        private void Start()
        {
            _tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();
            _tvNavigationController.OnVolumeUpButtonPressed += VolumeUp;
            _tvNavigationController.OnVolumeDownButtonPressed += VolumeDown;
            _tvNavigationController.OnVolumeUpButtonHeld += HoldVolumeUp;
            _tvNavigationController.OnVolumeDownButtonHeld += HoldVolumeDown;
            _tvNavigationController.OnVolumeUpButtonReleased += StopHold;
            _tvNavigationController.OnVolumeDownButtonReleased += StopHold;
        }

        public void VolumeUp()
        {
            ShowAndScheduleHide();
            tvVolumeWindow.VolumeUp();
        }

        public void VolumeDown()
        {
            ShowAndScheduleHide();
            tvVolumeWindow.VolumeDown();
        }

        public void SetVolume(int volume)
        {
            tvVolumeWindow.SetVolume(volume);
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
        
        private void HoldVolumeUp()
        {
            StartHoldLoop(tvVolumeWindow.VolumeUp);
        }
        
        private void HoldVolumeDown()
        {
            StartHoldLoop(tvVolumeWindow.VolumeDown);
        }
        
        private void StartHoldLoop(System.Action volumeAction)
        {
            if (_holdRepeatTween != null && _holdRepeatTween.IsActive())
            {
                return;
            }

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

        private void StopHold()
        {
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
        public void VolumeUp();
        public void VolumeDown();
        public void SetVolume(int volume);
    }
}