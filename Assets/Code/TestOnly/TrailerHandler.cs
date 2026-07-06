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
    [SerializeField] private ArtInteractableObject artInteractableObject;
    
    [Header("Camera-Configuration")]
    [SerializeField] private CameraPositionData originalCameraPositionData;
    [SerializeField] private CameraPositionData originalLightPositionData;
    [SerializeField] private CameraPositionData originalLampLightPositionData;
    
    [Header("Shoots-Data")]
    [SerializeField] private CameraPositionData[] cameraPositionsData;

    private Sequence _cameraSequence;
    private Tweener _trailerTween;
    private IInteractionController _interactionController;
    private IDisksController _disksController;
    private IPointsController _pointsController;
    private ITVStateController _tvStateController;
    private IGameEndingController _gameEndingController;


    private void Start()
    {
        if (playTrailer)
        {
            ResetCamera();
            lampInteractableObject.Interact();
            _interactionController = ServiceLocator.GetService<IInteractionController>();
            _tvStateController = ServiceLocator.GetService<ITVStateController>();
            _gameEndingController = ServiceLocator.GetService<IGameEndingController>();
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
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            ResetShoot();
            SetCameraShoot(3);
            _tvStateController.TurnOnOffTv();
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ResetShoot();
            SetCameraShoot(3);
            DOVirtual.DelayedCall(0.25f, () =>
            {
                _disksController = ServiceLocator.GetService<IDisksController>();
                _disksController.CreateDisk(DiskType.WHITE);
                _disksController.CreateDisk(DiskType.WHITE);
            });
        
            DOVirtual.DelayedCall(0.5f, ()=>
            {
                _disksController = ServiceLocator.GetService<IDisksController>();
                _disksController.ResumeAllDisksMoving();
            });
            
            DOVirtual.DelayedCall(1.2f, ()=>
            {
                _disksController = ServiceLocator.GetService<IDisksController>();
                _disksController.CheckDisksToMerge();
            });
        }
        
        if (Input.GetKeyDown(KeyCode.U))
        {
            ResetShoot();
            SetCameraShoot(3);
            
            _pointsController.UpdatePoints(8012026);
            
            DOVirtual.DelayedCall(0.25f, () =>
            {
                _disksController = ServiceLocator.GetService<IDisksController>();
                _disksController.RemoveAllDisks();
                _disksController.CreateDisk(DiskType.WHITE);
                _disksController.CreateDisk(DiskType.CYAN);
                _disksController.CreateDisk(DiskType.YELLOW);
                _disksController.CreateDisk(DiskType.ORANGE);
                _disksController.CreateDisk(DiskType.RED);
                _disksController.CreateDisk(DiskType.GREEN);
            });
            
            DOVirtual.DelayedCall(0.5f, ()=>
            {
                _disksController = ServiceLocator.GetService<IDisksController>();
                _disksController.ResumeAllDisksMoving();
            });
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            ResetShoot();
            SetCameraShoot(3);
            DOVirtual.DelayedCall(0.1f, () =>
            {
                _pointsController = ServiceLocator.GetService<IPointsController>();
                _pointsController.UpdatePoints(4292026);
                _disksController = ServiceLocator.GetService<IDisksController>();
                _disksController.RemoveAllDisks();
                _disksController.CreateDisk(DiskType.MAGENTA);
                _disksController.CreateDisk(DiskType.MAGENTA);
                _disksController.ResumeAllDisksMoving();
            });
            
            DOVirtual.DelayedCall(1.25f, () =>
            {
                _disksController = ServiceLocator.GetService<IDisksController>();
                _disksController.CreateDisk(DiskType.MAGENTA);
                _disksController.ResumeAllDisksMoving();
                _disksController.CheckDisksToMerge();
            });
            
            DOVirtual.DelayedCall(8f, () =>
            {
                _gameEndingController = ServiceLocator.GetService<IGameEndingController>();
                _gameEndingController.EjectDisk();
            });
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
            doorLight.enabled = true;
            DOVirtual.DelayedCall(3.5f, ()=>
                doorInteractableObject.ForceClose());
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ResetShoot();
            SetCameraShoot(5);
            trailerCameraLight.SetActive(false);
            cellphoneInteractableObject.Corrupt();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ResetShoot();
            SetCameraShoot(6);
            DOVirtual.DelayedCall(1f, ()=>
                artInteractableObject.Corrupt());
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            ResetShoot();
            lampInteractableObject.Corrupt();
            DOVirtual.DelayedCall(2f, ()=>  SetCameraShoot(7, null, Ease.OutExpo));
            drawingInteractableObject.Corrupt();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            ResetShoot();
            SetCameraShoot(8);
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

    private void SetCameraShoot(int index, Action onCompleteCallback = null, Ease ease = Ease.InOutSine)
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
                trailerCamera.transform
                    .DOLocalMove(cameraPositionData.cameraTargetPosition, cameraPositionData.timeToPosition)
                    .SetEase(ease);
                trailerCamera.transform
                    .DOLocalRotate(cameraPositionData.cameraTargetRotation, cameraPositionData.timeToRotation)
                    .SetEase(ease);
            });
            _cameraSequence.AppendInterval(cameraPositionData.timeToPosition);
            _cameraSequence.AppendCallback(() =>
            {
                onCompleteCallback?.Invoke();
            });
        }
    }
}
