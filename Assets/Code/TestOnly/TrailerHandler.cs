using System;
using Code.TestOnly;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using DVDNights;
using UnityEngine;

public class TrailerHandler : MonoBehaviour
{
    [Header("Debug")] 
    [SerializeField] private bool playTrailer;
    
    [Header("References")] 
    [SerializeField] private Camera trailerCamera;
    [SerializeField] private GameObject trailerCameraLight;
    [SerializeField] private Light roomLight;
    [SerializeField] private Light doorLight;
    
    [Header("Interactables")]
    [SerializeField] private BookInteractableObject bookInteractableObject;
    [SerializeField] private DoorInteractableObject doorInteractableObject;
    [SerializeField] private CellphoneInteractableObject cellphoneInteractableObject;
    [SerializeField] private LampInteractableObject lampInteractableObject;
    [SerializeField] private DrawingInteractableObject drawingInteractableObject;
    [SerializeField] private EntityInteractableObject entityInteractableObject;
    [SerializeField] private TurntableInteractableObject turntableInteractableObject;
    [SerializeField] private DecoyGameRulesInteractableObject decoyGameRulesInteractableObject;
    [SerializeField] private TVInteractableObject tvInteractableObject;
    
    [Header("Camera-Configuration")]
    [SerializeField] private CameraPositionData originalCameraPositionData;
    [SerializeField] private CameraPositionData originalLightPositionData;
    [SerializeField] private CameraPositionData originalLampLightPositionData;
    
    [Header("Shoots-Data")]
    [SerializeField] private CameraPositionData[] cameraPositionsData;


    private Sequence _cameraSequence;
    private Tweener _trailerTween;
    private IInteractionController _interactionController;
    

    private void Start()
    {
        if (playTrailer)
        {
            ResetCamera();
            lampInteractableObject.Interact();
            _interactionController = ServiceLocator.GetService<IInteractionController>();
            _interactionController.DisableInteractions();
            doorLight.enabled = false;
        }
    }

    private void ResetCamera()
    {
        trailerCamera.transform.localPosition = originalCameraPositionData.cameraPosition;
        trailerCamera.transform.localRotation = Quaternion.Euler(originalCameraPositionData.cameraRotation);
        roomLight.transform.localPosition = originalLightPositionData.cameraPosition;
    }

    private void ResetShoot()
    {
        doorLight.enabled = false;
        trailerCameraLight.SetActive(true);
        lampInteractableObject.ClearCorruption();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCamera();
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ResetShoot();
            SetCameraShoot(0);
            turntableInteractableObject.ForceSpinning();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ResetShoot();
            SetCameraShoot(1);
            decoyGameRulesInteractableObject.SlipTroughDoor();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ResetShoot();
            SetCameraShoot(2);
            lampInteractableObject.Corrupt();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ResetShoot();
            SetCameraShoot(3);
            tvInteractableObject.Corrupt();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ResetShoot();
            SetCameraShoot(4);
            doorInteractableObject.Corrupt();
            trailerCameraLight.SetActive(true);
            doorLight.enabled = true;
            DOVirtual.DelayedCall(3.5f, ()=>
                doorInteractableObject.ForceClose());
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            ResetShoot();
            SetCameraShoot(8);
            drawingInteractableObject.Corrupt();
            lampInteractableObject.SetLampIntensity(0.3f);
            trailerCameraLight.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ResetShoot();
            SetCameraShoot(9, () =>
            {
                entityInteractableObject.PlayAnimationClip();
                DOVirtual.DelayedCall(0.65f, ()=>
                _trailerTween = trailerCamera
                    .DOFieldOfView(0f, 0.4f)
                    .SetEase(Ease.InExpo));
            });
            
            roomLight.transform.localPosition = new Vector3(originalLightPositionData.cameraPosition.x, originalLightPositionData.cameraPosition.y, -0.8f);
        }
    }

    private void SetCameraShoot(int index, Action onCompleteCallback = null)
    {
        CameraPositionData cameraPositionData = cameraPositionsData[index];
        trailerCamera.transform.localPosition = cameraPositionData.cameraPosition;
        trailerCamera.transform.localRotation = Quaternion.Euler(cameraPositionData.cameraRotation);

        if (cameraPositionData.playTween)
        {
            _cameraSequence?.Kill();
            _cameraSequence = DOTween.Sequence();
            _cameraSequence.AppendCallback(() =>
            {
                trailerCamera.transform.DOLocalMove(cameraPositionData.cameraTargetPosition, cameraPositionData.timeToPosition).SetEase(Ease.InOutSine);
                trailerCamera.transform.DOLocalRotate(cameraPositionData.cameraTargetRotation, cameraPositionData.timeToRotation).SetEase(Ease.InOutSine);
            }).OnComplete(()=> onCompleteCallback?.Invoke());
        }
    }
}
