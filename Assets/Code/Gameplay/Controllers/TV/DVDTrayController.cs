using System;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace Rulebound
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
        private bool _isAnimating;
        
        private ITVButton _tvOpenCloseButton;
        private ITVStateController _tvStateController;

        public bool IsTrayOpened => _isOpened;
        public bool IsAnimating => _isAnimating;
        
        public Transform TrayTransform => diskTrayTransform;
        public Action OnTrayOpened { get; set; }
        public Action OnTrayClosed { get; set; }
        public Action OnTrayAnimation { get; set; }

        private void Awake()
        {
            _originalZPosition = diskTrayTransform.localPosition.z;
            _canAnimate = true;
            _isOpened = false;
            _isAnimating = false;
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
            
            _isAnimating = true;
            OnTrayAnimation?.Invoke();
            
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

            AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, openTrayClip, 0.5f, randomizePitch: false);
            diskTrayTransform.DOLocalMoveZ(destinationZPosition, openTrayClip.length * 0.65f).SetEase(Ease.InSine).OnComplete(() =>
            {
                _isOpened = true;
                _isAnimating = false;
                _canAnimate = true;
                OnTrayOpened?.Invoke();
                _tvOpenCloseButton.EnableButton();
            });
        }
        
        private void CloseLid()
        {
            diskTrayTransform.DOKill();

            AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, closeTrayClip, 0.5f, randomizePitch: false);
            diskTrayTransform.DOLocalMoveZ(_originalZPosition, closeTrayClip.length * 0.55f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                _isOpened = false;
                _isAnimating = false;
                _canAnimate = true;
                OnTrayClosed?.Invoke();
                _tvOpenCloseButton.EnableButton();
            });
        }
        
        public void SetCurrentDVDBox(DVDBoxInteractableObject dvdBoxInteractableObject)
        {
            dvdTrayInteractableObject.SetCurrentDvdBoxInteractableObject(dvdBoxInteractableObject);
        }


        private void OnDestroy()
        {
            _tvNavigationController.OnOpenCloseButtonPressed -= HandleLid;
        }
    }
    
    public interface IDVDTrayController
    {
        public bool IsTrayOpened { get; }
        public Transform TrayTransform { get; }
        public Action OnTrayOpened { get; set; }
        public Action OnTrayClosed { get; set; }
        public Action OnTrayAnimation { get; set; }

        public void SetCurrentDVDBox(DVDBoxInteractableObject dvdBoxInteractableObject);
    }
}

