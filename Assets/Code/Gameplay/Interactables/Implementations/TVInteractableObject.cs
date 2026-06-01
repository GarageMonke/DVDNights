using System;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class TVInteractableObject : InteractableObject
    {
        [SerializeField] private Vector3 cameraLockPosition;

        ICameraController _cameraController;
        private ITVNavigationController _tvNavigationController;

        private void Start()
        {
            _cameraController = ServiceLocator.GetService<ICameraController>();
            _tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();
        }

        public override void Interact()
        {
            Unhighlight();
            _cameraController.TweenToPosition(cameraLockPosition, 0.5f);
            _cameraController.TweenToRotation(Quaternion.identity, 0.5f);
            _tvNavigationController.EnableButtons();
        }

        public override void StopInteraction()
        {
            Highlight();
            _tvNavigationController.DisableButtons();
            _cameraController.TweenToPosition(_cameraController.OriginPosition, 0.5f);
            _cameraController.TweenToRotation(Quaternion.identity, 0.5f);
        }
    }
}