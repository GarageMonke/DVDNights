using System;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class DVDTrayController : MonoBehaviour, IDVDTrayController
    {
        [Header("References")]
        [SerializeField] private DVDTrayInteractableObject dvdTrayInteractableObject;
        
        [Header("Configuration")]
        [SerializeField] private Transform diskTrayTransform;
        [SerializeField] private float destinationZPosition;
    
        [Header("Feedback")]
        [SerializeField] private AudioClip openTrayClip;
        [SerializeField] private AudioClip closeTrayClip;
        
        private ITVNavigationController _tvNavigationController;
        private float _originalZPosition;

        private bool _canAnimate;
        private bool _isOpened;
        private ITVButton _tvOpenCloseButton;
        private ITVStateController _tvStateController;

        public bool IsLidOpened => _isOpened;
        public Action OnLidOpened { get; set; }
        public Action OnLidClosed { get; set; }

        private void Awake()
        {
            _originalZPosition = diskTrayTransform.localPosition.z;
            _canAnimate = true;
            _isOpened = false;
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IDVDTrayController>(this);
        }

        private void Start()
        {
            _tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();
            _tvNavigationController.OnOpenCloseButtonPressed += HandleLid;
            _tvOpenCloseButton = _tvNavigationController.OpenCloseButton;
            
            _tvStateController = ServiceLocator.GetService<ITVStateController>();
            dvdTrayInteractableObject.DisableInteraction();
        }

        private void HandleLid()
        {
            if (!_tvStateController.IsTVOn)
            {
                return;
            }
            
            if (_tvStateController.HasDisk)
            {
                return;
            }
            
            if (!_canAnimate)
            {
                return;
            }
            
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
            diskTrayTransform.DOKill();

            AudioManager.Instance.PlaySFX(openTrayClip, 0.5f, randomizePitch: false);
            diskTrayTransform.DOLocalMoveZ(destinationZPosition, openTrayClip.length * 0.85f).SetEase(Ease.InSine).OnComplete(() =>
            {
                _isOpened = true;
                _canAnimate = true;
                OnLidOpened?.Invoke();
                _tvOpenCloseButton.EnableButton();
                dvdTrayInteractableObject.EnableInteraction();
            });
        }
        
        private void CloseLid()
        {
            diskTrayTransform.DOKill();

            AudioManager.Instance.PlaySFX(closeTrayClip, 0.5f, randomizePitch: false);
            diskTrayTransform.DOLocalMoveZ(_originalZPosition, closeTrayClip.length * 0.55f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                _isOpened = false;
                _canAnimate = true;
                OnLidClosed?.Invoke();
                _tvOpenCloseButton.EnableButton();
                dvdTrayInteractableObject.DisableInteraction();
            });
        }

        private void OnDestroy()
        {
            _tvNavigationController.OnOpenCloseButtonPressed -= HandleLid;
        }
    }
    
    public interface IDVDTrayController
    {
        public bool IsLidOpened { get; }
        public Action OnLidOpened { get; set; }
        public Action OnLidClosed { get; set; }
    }
}

