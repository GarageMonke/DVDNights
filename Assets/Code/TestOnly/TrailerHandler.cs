using System;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using Rulebound;
using UnityEditor;
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
    [SerializeField] private GameRulesInteractableObject gameRulesInteractableObject;
    [SerializeField] private TVInteractableObject tvInteractableObject;
    [SerializeField] private ArtInteractableObject artInteractableObject;
    [SerializeField] private DVDBoxInteractableObject dvdBoxInteractableObject;
    
    [Header("Camera-Configuration")]
    [SerializeField] private CameraPositionData originalCameraPositionData;
    [SerializeField] private CameraPositionData originalLightPositionData;
    [SerializeField] private CameraPositionData originalLampLightPositionData;
    
    [Header("Shoots-Data")]
    [SerializeField] private CameraPositionData[] cameraPositionsData;

    private Sequence _cameraSequence;
    private Tweener _trailerTween;
    private Sequence _trailerSequence;
    private IInteractionController _interactionController;
    private IDisksController _disksController;
    private IPointsController _pointsController;
    private ITVStateController _tvStateController;
    private IGameProgressionController _gameProgressionController;
    private IThunderController _thunderController;
    private Vector3 _trailerCameraOriginalPosition;

    private int _internalDiscSequence;


    private void Start()
    {
        if (playTrailer)
        {
            PlayTrailerSequences();
        }
    }

    private void ResetCamera()
    {
        _cameraSequence?.Kill();
        trailerCamera.transform.DOKill();
        trailerCamera.transform.localPosition = originalCameraPositionData.cameraPosition;
        trailerCamera.transform.localRotation = Quaternion.Euler(originalCameraPositionData.cameraRotation);
        roomLight.transform.localPosition = originalLightPositionData.cameraPosition;
    }

    private void ResetShoot()
    {
        _cameraSequence?.Kill();
        doorLight.enabled = false;
        trailerCameraLight.SetActive(true);
        lampInteractableObject.ClearCorruption();
        _disksController = ServiceLocator.GetService<IDisksController>();
        _disksController?.MuteAllDiscs();
    }

    private void PlayTrailerSequences()
    {
        _internalDiscSequence = 0;
        _trailerSequence?.Kill();
        _trailerSequence = DOTween.Sequence();
        _trailerSequence.AppendInterval(2f);
        _trailerSequence.AppendCallback(() =>
        {
            ResetCamera();
            _trailerCameraOriginalPosition = trailerCamera.transform.localPosition;
            lampInteractableObject.Interact();
            _interactionController = ServiceLocator.GetService<IInteractionController>();
            _gameProgressionController = ServiceLocator.GetService<IGameProgressionController>();
            _interactionController.DisableInteractions();
            doorLight.enabled = false;
        });
        
        _trailerSequence.AppendInterval(2f);
        _trailerSequence.AppendCallback(OpenDVDBox);
        
//        
//         _trailerSequence.AppendCallback(PlayTake00);
//         _trailerSequence.AppendInterval(2f);
//         _trailerSequence.AppendCallback(PlayTake02);
//         _trailerSequence.AppendInterval(6f);
//         _trailerSequence.AppendCallback(PlayTake01);
//         _trailerSequence.AppendInterval(3f);
//         _trailerSequence.AppendCallback(PlayTake03);
//         _trailerSequence.AppendInterval(4f);
//         _trailerSequence.AppendCallback(()=>
//         {
//             _tvStateController.StrikeTV();
//             PlayTake10();
//         });
//         _trailerSequence.AppendInterval(5f);
//         _trailerSequence.AppendCallback(PlayTake02);
//         _trailerSequence.AppendInterval(8f);
//         _trailerSequence.AppendCallback(PlayTake04);
//         _trailerSequence.AppendInterval(5f);
//         _trailerSequence.AppendCallback(PlayTake02);
//         _trailerSequence.AppendInterval(8f);
//         _trailerSequence.AppendCallback(PlayTake05);
//         _trailerSequence.AppendInterval(5f);
//         _trailerSequence.AppendCallback(PlayTake02);
//         _trailerSequence.AppendInterval(8f);
//         _trailerSequence.AppendCallback(PlayTake06);
//         _trailerSequence.AppendInterval(5f);
//         _trailerSequence.AppendCallback(PlayTake02);
//         _trailerSequence.AppendInterval(11f);
//         _trailerSequence.AppendCallback(PlayTake07);
//         _trailerSequence.AppendInterval(3f);
//         _trailerSequence.AppendCallback(PlayTake08);
//         _trailerSequence.AppendInterval(4f);
//         _trailerSequence.AppendCallback(PlayTake09);
//         _trailerSequence.AppendInterval(3f);

            //Cryptic Messages
            // _trailerSequence.AppendInterval(1f);
            // _trailerSequence.AppendCallback(PlayTake02);
            // _trailerSequence.AppendInterval(0.1f);
            // _trailerSequence.AppendCallback(CrypticMessages);
            
            //Knocking Door
            // _trailerSequence.AppendInterval(4f);
            // _trailerSequence.AppendCallback(PlayTake12);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCamera();
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            OneWhiteDisc();
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
           WhiteFusionDisc();
        }
        
        if (Input.GetKeyDown(KeyCode.U))
        {
           ColorDiscs();
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
           GoldenDisc();
        }
      
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            PlayTake00();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayTake01();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
           PlayTake02();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
           PlayTake03();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayTake04();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlayTake05();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            PlayTake06();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
           PlayTake07();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            PlayTake08();
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
           PlayTake09();
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

    private void PlayTake00()
    {
        ResetCamera();
        ResetShoot();
        SetCameraShoot(0);
        turntableInteractableObject.ForceSpinning();
    }
    
    private void PlayTake01()
    {
        ResetCamera();
        ResetShoot();
        SetCameraShoot(1);
        gameRulesInteractableObject.SlipTroughDoor();
    }

    private void PlayTake02()
    {
        lampInteractableObject.RestoreLampIntensity();
        _internalDiscSequence++;
        
        ResetCamera();
        ResetShoot();
        SetCameraShoot(3);
        
        switch (_internalDiscSequence)
        {
            case 1:
                OneWhiteDisc();
                break;
            case 2:
                WhiteFusionDisc();
                break;
            case 3:
                ColorDiscs();
                break;
            case 4:
                FastForwardDiscs();
                break;
            case 5:
                GoldenDisc();
                break;
        }

        DOVirtual.DelayedCall(1f, () =>
        {
            _disksController = ServiceLocator.GetService<IDisksController>();
            _disksController.UnmuteAllDiscs();
        });
    }

    private void PlayTake03()
    {
        ResetCamera();
        ResetShoot();
        SetCameraShoot(3);
        tvInteractableObject.Corrupt();
    }
    
    private void PlayTake04()
    {
        ResetCamera();
        ResetShoot();
        SetCameraShoot(4);
        doorInteractableObject.Corrupt();
        doorLight.enabled = true;
        DOVirtual.DelayedCall(4.5f, ()=>
            doorInteractableObject.ForceClose());
    }
    
    private void PlayTake12()
    {
        ResetCamera();
        ResetShoot();
        SetCameraShoot(12);
        doorInteractableObject.Corrupt();
        doorLight.enabled = true;
    }


    private void PlayTake05()
    {
        ResetCamera();
        ResetShoot();
        SetCameraShoot(5);
        trailerCameraLight.SetActive(false);
        cellphoneInteractableObject.Corrupt();
    }
    
    private void PlayTake06()
    {
        ResetCamera();
        ResetShoot();
        SetCameraShoot(6);
        DOVirtual.DelayedCall(1f, ()=>
            artInteractableObject.Corrupt());
    }
    
    private void PlayTake07()
    {
        ResetCamera();
        ResetShoot();
        lampInteractableObject.Corrupt();
        DOVirtual.DelayedCall(2f, ()=> SetCameraShoot(7, null, Ease.OutExpo));
        drawingInteractableObject.Corrupt();
    }
    
    private void PlayTake08()
    {
        ResetCamera();
        ResetShoot();
        SetCameraShoot(8);
        lampInteractableObject.SetLampIntensity(0.3f);
        entityInteractableObject.ShowEntity();
        trailerCameraLight.SetActive(false);
    }
    
    private void PlayTake09()
    {
        entityInteractableObject.ShowEntity();
        ResetCamera();
        ResetShoot();
        SetCameraShoot(9, () =>
        {
            lampInteractableObject.RestoreLampIntensity();
            entityInteractableObject.PlayAnimationClip();
            DOVirtual.DelayedCall(0.65f, ()=>
                _trailerTween = trailerCamera
                    .DOFieldOfView(0f, 0.4f)
                    .SetEase(Ease.InExpo));
        });
            
        roomLight.transform.localPosition = new Vector3(originalLightPositionData.cameraPosition.x, originalLightPositionData.cameraPosition.y, -0.8f);
    }
    
    private void PlayTake10()
    {
        gameRulesInteractableObject.Interact();
        ResetCamera();
        ResetShoot();
        SetCameraShoot(10);
        lampInteractableObject.SetLampIntensity(0.2f);
        trailerCameraLight.SetActive(false);
    }

    private void PlayTake11()
    {
        lampInteractableObject.Interact();
        _tvStateController = ServiceLocator.GetService<ITVStateController>();
        _tvStateController.PlayStatic();
        _thunderController = ServiceLocator.GetService<IThunderController>();
        _thunderController.PlayRain();
        ResetShoot();
        SetCameraShoot(11);
    }
    

    private void OneWhiteDisc()
    {
        ResetShoot();
        SetCameraShoot(3);
        _tvStateController = ServiceLocator.GetService<ITVStateController>();
        _tvStateController.TurnOnOffTv();
    }

    private void WhiteFusionDisc()
    {
        ResetShoot();
        SetCameraShoot(3);
        
        IDiskLevelController diskLevelController = ServiceLocator.GetService<IDiskLevelController>();
        diskLevelController.DiskBorderBonusLevel = 6;
        
        DOVirtual.DelayedCall(0.25f, () =>
        {
            _disksController = ServiceLocator.GetService<IDisksController>();
            _disksController.CreateDisk(DiskType.WHITE);
        });
        
        DOVirtual.DelayedCall(0.5f, ()=>
        {
            _disksController = ServiceLocator.GetService<IDisksController>();
            _disksController.ResumeAllDisksMoving();
        });
            
        DOVirtual.DelayedCall(2.5f, ()=>
        {
            _disksController.CreateDisk(DiskType.WHITE);
            _disksController = ServiceLocator.GetService<IDisksController>();
            _disksController.CheckDisksToMerge();
        });
    }

    private void ColorDiscs()
    {
        ResetShoot();
        SetCameraShoot(3);
        
        IDiskLevelController diskLevelController = ServiceLocator.GetService<IDiskLevelController>();
        diskLevelController.DiskBorderBonusLevel = 15;
        
        _pointsController = ServiceLocator.GetService<IPointsController>();
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
    
    private void FastForwardDiscs()
    {
        cellphoneInteractableObject.MutePhone();
        IDiskLevelController diskLevelController = ServiceLocator.GetService<IDiskLevelController>();
        diskLevelController.DiskFFDrainRateLevel = BounceGameProgression.GetFFMaxLevel();
        diskLevelController.DiskFFMultLevel = BounceGameProgression.GetFFMaxLevel();
        ITVNavigationController tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();
        tvNavigationController.OnNextButtonHeld?.Invoke();
        DOVirtual.DelayedCall(5f, () => tvNavigationController.OnNextButtonReleased?.Invoke());
    }
    
    private void GoldenDisc()
    {
        ResetShoot();
        SetCameraShoot(3);
        DOVirtual.DelayedCall(0.1f, () =>
        {
            _pointsController = ServiceLocator.GetService<IPointsController>();
            _pointsController.UpdatePoints(1222024);
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
            _gameProgressionController = ServiceLocator.GetService<IGameProgressionController>();
            _gameProgressionController.EjectDisk();
            _cameraSequence?.Kill();
            _cameraSequence = DOTween.Sequence();
            _cameraSequence.AppendCallback(() =>
            {
                trailerCamera.transform
                    .DOLocalMove(_trailerCameraOriginalPosition, 3f)
                    .SetEase(Ease.InOutSine);
                trailerCamera.transform
                    .DOLocalRotate(Vector3.zero, 3f)
                    .SetEase(Ease.InOutSine);
            });
        });
    }

    private void CrypticMessages()
    {
        ResetShoot();
        SetCameraShoot(3);
        _disksController = ServiceLocator.GetService<IDisksController>();
        _disksController.RemoveAllDisks();
        
        IMessageWindow tvMessageWindow = ServiceLocator.GetService<IMessageWindow>();
        IShopController shopController = ServiceLocator.GetService<IShopController>();
        DOVirtual.DelayedCall(0.5f, ()=>
        {
            _pointsController = ServiceLocator.GetService<IPointsController>();
            _pointsController.UpdatePoints(1812022);
            tvMessageWindow = ServiceLocator.GetService<IMessageWindow>();
            tvMessageWindow.SetMessage("There's no exit.");
            tvMessageWindow.Display();
        });
        
        DOVirtual.DelayedCall(2f, ()=>
        {
            tvMessageWindow.Hide();
        });
            
        DOVirtual.DelayedCall(2.5f, ()=>
        {
            _pointsController = ServiceLocator.GetService<IPointsController>();
            _pointsController.UpdatePoints(666);
            tvMessageWindow.SetMessage("DO NOT OPEN THE DOOR.");
            tvMessageWindow.Display();
        });
        
        DOVirtual.DelayedCall(3.5f, ()=>
        {
            tvMessageWindow.Hide();
        });
            
        DOVirtual.DelayedCall(4f, ()=>
        {
            _pointsController = ServiceLocator.GetService<IPointsController>();
            _pointsController.UpdatePoints(4292026);
            tvMessageWindow.SetMessage("DO NOT LET THEM IN.");
            tvMessageWindow.Display();
        });
        
        DOVirtual.DelayedCall(5f, ()=>
        {
            tvMessageWindow.Hide();
        });
            
        DOVirtual.DelayedCall(5.5f, ()=>
        {
            _pointsController = ServiceLocator.GetService<IPointsController>();
            _pointsController.UpdatePoints(4292026);
            shopController.OpenShop();
        });
    }
    
    private void Shop()
    {
        ResetShoot();
        SetCameraShoot(3);
        IShopController shopController = ServiceLocator.GetService<IShopController>();
        ITVNavigationController tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();
        DOVirtual.DelayedCall(0.5f, ()=>
        {
            _pointsController = ServiceLocator.GetService<IPointsController>();
            _pointsController.UpdatePoints(1812022);
            IDiskLevelController diskLevelController = ServiceLocator.GetService<IDiskLevelController>();
            diskLevelController.DiskBorderBonusLevel = 12;
            diskLevelController.DiskCornerBonusLevel = 10;
            diskLevelController.DiskFFBonusLevel = 5;
            diskLevelController.DiskFFDrainRateLevel = 8;
            diskLevelController.DiskFFMultLevel = 6;
            shopController.OpenShop();
           
        });
        
        DOVirtual.DelayedCall(0.65f, ()=>
        {
            shopController.MoveToNext();
            tvNavigationController.NextButton.Press();
        });
            
        DOVirtual.DelayedCall(1.3f, ()=>
        {
            shopController.MoveToNext();
            tvNavigationController.NextButton.Press();
        });
                
        DOVirtual.DelayedCall(1.95f, ()=>
        {
            shopController.MoveToNext();
            tvNavigationController.NextButton.Press();
        });
        
        DOVirtual.DelayedCall(3, ()=>
        {
            shopController.SelectItem();
            tvNavigationController.SubmitButton.Press();
        });
    }

    private void OpenDVDBox()
    {
        ResetShoot();
        
        gameRulesInteractableObject.Interact();
        ITVNavigationController tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();
        tvNavigationController.PowerButton.Press();
        DOVirtual.DelayedCall(2, ()=>
        {
            tvNavigationController.OpenCloseButton.Press();
        });
    
        DOVirtual.DelayedCall(6, ()=>
        {
            dvdBoxInteractableObject.Interact();
        });
    }
}
