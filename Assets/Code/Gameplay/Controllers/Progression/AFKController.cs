using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class AFKController : MonoBehaviour, IAFKController
    {
        public float AFKTime => Time.time - _lastCameraMoveTime;
        private ICameraController _cameraController;
        private float _lastCameraMoveTime;
        private bool _isEnabled;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IAFKController>(this);
        }

        private void Start()
        {
            _cameraController = ServiceLocator.GetService<ICameraController>();
        }

        private void RecordCameraMovement()
        {
            _lastCameraMoveTime = Time.time;
        }
        
        public void EnableController()
        {
            _isEnabled = true;
            _cameraController.OnCameraMove += RecordCameraMovement;
        }

        public void DisableController()
        {
            _isEnabled = false;
            _cameraController.OnCameraMove -= RecordCameraMovement;
        }

        private void OnDestroy()
        {
            if (_cameraController != null)
            {
                _cameraController.OnCameraMove -= RecordCameraMovement;
            }
        }
    }

    public interface IAFKController
    {
        public float AFKTime { get; }
        public void EnableController();
        public void DisableController();
    }
}