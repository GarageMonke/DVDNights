using System;
using DG.Tweening;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class SequenceManager : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Camera gameCamera;
        [SerializeField] private Light roomLight;

        [Header("Shoots-Data")] 
        [SerializeField] private CameraPositionData[] cameraPositionsData;
        
        [Header("Interactables")]
        [SerializeField] private EntityInteractableObject entityInteractableObject;
        
        [Header("Camera-Configuration")]
        [SerializeField] private CameraPositionData originalLightPositionData;

        private Sequence _cameraSequence;
        private IInteractionController _interactionController;
        private ICameraController _cameraController;
        private Vector3 _trailerCameraOriginalPosition;


        public static SequenceManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void DisableInput()
        {
            _interactionController = ServiceLocator.GetService<IInteractionController>();
            _cameraController = ServiceLocator.GetService<ICameraController>();
            
            _interactionController.StopAllInteractions();
            _cameraController.DisableNavigation();
        }


        public void PlayGameOverSequence()
        {
            DisableInput();
            entityInteractableObject.ShowEntity();
            
            SetCameraShoot(0, () =>
            {
                entityInteractableObject.PlayAnimationClip();
                DOVirtual.DelayedCall(0.65f, () =>
                         gameCamera
                        .DOFieldOfView(0f, 0.4f)
                        .SetEase(Ease.InExpo));
            });

            roomLight.transform.localPosition = new Vector3(originalLightPositionData.cameraPosition.x,
                originalLightPositionData.cameraPosition.y, -0.8f);
        }
        

        private void SetCameraShoot(int index, Action onCompleteCallback = null, Ease ease = Ease.InOutSine)
        {
            gameCamera.DOKill();
            CameraPositionData cameraPositionData = cameraPositionsData[index];
            gameCamera.transform.localPosition = cameraPositionData.cameraPosition;
            gameCamera.transform.localRotation = Quaternion.Euler(cameraPositionData.cameraRotation);

            if (cameraPositionData.playTween)
            {
                _cameraSequence?.Kill();
                _cameraSequence = DOTween.Sequence();
                _cameraSequence.AppendCallback(() =>
                {
                    gameCamera.transform
                        .DOLocalMove(cameraPositionData.cameraTargetPosition, cameraPositionData.timeToPosition)
                        .SetEase(ease);
                    gameCamera.transform
                        .DOLocalRotate(cameraPositionData.cameraTargetRotation, cameraPositionData.timeToRotation)
                        .SetEase(ease);
                });
                _cameraSequence.AppendInterval(cameraPositionData.timeToPosition);
                _cameraSequence.AppendCallback(() => { onCompleteCallback?.Invoke(); });
            }
        }
    }
}
