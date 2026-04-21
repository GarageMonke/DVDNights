using System;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class DVDLidController : MonoBehaviour, IDVDLidController
    {
        [Header("Configuration")]
        [SerializeField] private Transform diskLidTransform;
        [SerializeField] private float destinationZPosition;
    
        [Header("Feedback")]
        [SerializeField] private AudioClip openLidClip;
        [SerializeField] private AudioClip closeLidClip;
        
        private ITVNavigationController _tvNavigationController;
        private float _originalZPosition;

        private bool _canAnimate;
        private bool _isOpened;
        private ITVButton _tvOpenCloseButton;

        public Action OnLidOpened { get; set; }
        public Action OnLidClosed { get; set; }

        private void Awake()
        {
            _originalZPosition = diskLidTransform.localPosition.z;
            _canAnimate = true;
            _isOpened = false;
        }

        private void Start()
        {
            _tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();
            _tvNavigationController.OnOpenCloseButtonPressed += HandleLid;
            _tvOpenCloseButton = _tvNavigationController.OpenCloseButton;
        }

        private void HandleLid()
        {
            if (!_canAnimate)
            {
                return;
            }
            
            Debug.Log("lid handled");
            
            _canAnimate = false;
            _tvOpenCloseButton.DisableButton();
            
            if (_isOpened)
            {
                CloseLid();
                return;
            }
            
            OpenLid();
        }
        
        private void OpenLid()
        {
            diskLidTransform.DOKill();

            AudioManager.Instance.PlaySFX(openLidClip, 0.5f, randomizePitch: false);
            diskLidTransform.DOLocalMoveZ(destinationZPosition, openLidClip.length * 0.85f).SetEase(Ease.InSine).OnComplete(() =>
            {
                _isOpened = true;
                _canAnimate = true;
                OnLidOpened?.Invoke();
                _tvOpenCloseButton.EnableButton();
            });
        }
        
        private void CloseLid()
        {
            diskLidTransform.DOKill();

            AudioManager.Instance.PlaySFX(closeLidClip, 0.5f, randomizePitch: false);
            diskLidTransform.DOLocalMoveZ(_originalZPosition, closeLidClip.length * 0.55f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                _isOpened = false;
                _canAnimate = true;
                OnLidClosed?.Invoke();
                _tvOpenCloseButton.EnableButton();
            });
        }

        private void OnDestroy()
        {
            _tvNavigationController.OnOpenCloseButtonPressed -= HandleLid;
        }
    }
    
    public interface IDVDLidController
    {
        public Action OnLidOpened { get; set; }
        public Action OnLidClosed { get; set; }
    }
}

