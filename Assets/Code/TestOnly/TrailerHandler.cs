using Code.TestOnly;
using DG.Tweening;
using DVDNights;
using UnityEngine;

public class TrailerHandler : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Camera trailerCamera;
    [SerializeField] private GameObject trailerCameraLight;
    [SerializeField] private BookInteractableObject bookInteractableObject;
    [SerializeField] private DoorInteractableObject doorInteractableObject;
    [SerializeField] private CellphoneInteractableObject cellphoneInteractableObject;
    [SerializeField] private LampInteractableObject lampInteractableObject;
    [SerializeField] private DrawingInteractableObject drawingInteractableObject;
    [SerializeField] private EntityInteractableObject entityInteractableObject;
    [SerializeField] private TurntableInteractableObject turntableInteractableObject;
    [SerializeField] private DecoyGameRulesInteractableObject decoyGameRulesInteractableObject;
    [SerializeField] private Light roomLight;
    
    [Header("Configuration")]
    [SerializeField] private CameraPositionData originalCameraPositionData;
    [SerializeField] private CameraPositionData originalLightPositionData;
    [SerializeField] private CameraPositionData originalLampLightPositionData;
    
    [Header("Shoots-Data")]
    [SerializeField] private CameraPositionData[] cameraPositionsData;


    private Sequence _cameraSequence;

    private void Awake()
    {
        ResetCamera();
        
        lampInteractableObject.Interact();
    }

    private void ResetCamera()
    {
        trailerCamera.transform.localPosition = originalCameraPositionData.cameraPosition;
        trailerCamera.transform.localRotation = Quaternion.Euler(originalCameraPositionData.cameraRotation);
        roomLight.transform.localPosition = originalLightPositionData.cameraPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCamera();
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SetCameraShoot(0);
            trailerCameraLight.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetCameraShoot(1);
            decoyGameRulesInteractableObject.SlipTroughDoor();
            trailerCameraLight.SetActive(true);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SetCameraShoot(8);
            drawingInteractableObject.Corrupt();
            lampInteractableObject.SetLampIntensity(0.5f);
            trailerCameraLight.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SetCameraShoot(9);
            trailerCameraLight.SetActive(true);
            entityInteractableObject.PlayAnimationClip();
            roomLight.transform.localPosition = new Vector3(originalLightPositionData.cameraPosition.x, originalLightPositionData.cameraPosition.y, -0.8f);
        }
    }

    private void SetCameraShoot(int index)
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
            });
        }
    }
}
