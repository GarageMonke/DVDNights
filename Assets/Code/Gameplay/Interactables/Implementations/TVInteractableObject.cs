using System;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class TVInteractableObject : InteractableObject, ICorruptibleObject
    {
        [Header("Configuration")]
        [SerializeField] private Vector3 cameraLockPosition;

        ICameraController _cameraController;
        private ITVNavigationController _tvNavigationController;
        private IMouseLayoutController _mouseLayoutController;
        private ITVStateController _tvStateController;
        private bool _isCorrupted;
        private bool _isInteractingWithTv;

        private void Start()
        {
            _cameraController = ServiceLocator.GetService<ICameraController>();
            _tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();
            _mouseLayoutController = ServiceLocator.GetService<IMouseLayoutController>();
            _tvStateController = ServiceLocator.GetService<ITVStateController>();
        }

        public override string GetInteractionAction()
        {
            return "TV";
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.C)) return;
            if (CanBeCorrupted())
            {
                Corrupt();
            }
        }

        public override void Interact()
        {
            if (_isCorrupted)
            {
                ClearCorruption();
                return;
            }
            
            Unhighlight();
            _cameraController.TweenToPosition(cameraLockPosition, 0.5f);
            _cameraController.TweenToRotation(Quaternion.identity, 0.5f);
            _tvNavigationController.EnableButtons();
            _mouseLayoutController.DisplayRegularLayout();
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, InteractionAudioClip, volume: 1f, pitch: 2.5f);
            _isInteractingWithTv = true;
        }

        public override void StopInteraction()
        {
            if (_isCorrupted)
            {
                return;
            }
            
            Highlight();
            _tvNavigationController.DisableButtons();
            _cameraController.TweenToPosition(_cameraController.OriginPosition, 0.5f);
            _cameraController.TweenToRotation(Quaternion.identity, 0.5f);
            _mouseLayoutController.HideMouseLayout();
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, InteractionAudioClip, volume: 1f, pitch: 1.5f);
            _isInteractingWithTv = false;
        }

        public void Corrupt()
        {
            _isCorrupted = true;
            _tvStateController.PlayStatic(true);
            SetHasNavigation(false);
        }

        public void ClearCorruption()
        {
            _isCorrupted = false;
            _tvStateController.StrikeTV();
            DOVirtual.DelayedCall(0.25f, () => SetHasNavigation(true));
        }

        public bool CanBeCorrupted()
        {
            return _tvStateController.IsTVOn && _tvStateController.IsPlayingGame && !_isInteractingWithTv;
        }
    }
}