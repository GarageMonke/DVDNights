using System;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class InspectionController : MonoBehaviour, IInspectionController
    {
        [Header("References")] 
        [SerializeField] private Transform inspectionOrigin;
        [SerializeField] private InspectionWindow inspectionWindow;

        [Header("Configuration")]
        [SerializeField] private float rotationSpeed = 50f;
        [SerializeField] private float rotationSmoothTime = 0.1f;
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float zoomSmoothTime = 0.1f;
        
        private GameObject _inspectedObject;
        private IInspectionWindow _inspectionWindow;

        private bool _isInspecting;
        private Vector3 _lastMousePosition;
        private float _currentXAngle;
        private float _currentYAngle;
        
        
        private float _startXAngle;
        private float _startYAngle;
        
        private float _targetXAngle;
        private float _targetYAngle;
        private float _xVelocity;
        private float _yVelocity;

        private float _maxXAngle;
        private float _maxYAngle;
        private Vector2 _inspectionMaxAngle;
        private float _inspectionMaxZoom;
        private float _inspectionMinZoom;
        
        private float _targetZoom = 1f;
        private float _currentZoom = 1f;
        private float _zoomVelocity;
        private float _inspectionStartSize;
        
        private IMouseLayoutController _mouseLayoutController;
        private ICameraController _cameraController;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            _isInspecting = false;
            _inspectionWindow = inspectionWindow;
            ServiceLocator.RegisterService<IInspectionController>(this);
        }

        private void Start()
        {
            _mouseLayoutController = ServiceLocator.GetService<IMouseLayoutController>();
            _cameraController = ServiceLocator.GetService<ICameraController>();
        }

        public void Inspect(InspectableDataSO inspectableDataSO)
        {
            if (_isInspecting)
            {
                return;
            }

            _inspectionMaxAngle = inspectableDataSO.InspectionMaxAngle;
            _maxXAngle = _inspectionMaxAngle.x;
            _maxYAngle = _inspectionMaxAngle.y;
            _inspectionMaxZoom = inspectableDataSO.InspectionMaxZoom;
            _inspectionMinZoom = inspectableDataSO.InspectionMinZoom;
            _inspectionStartSize = inspectableDataSO.InspectionStartSize;
            _inspectedObject = Instantiate(inspectableDataSO.InspectableObject, inspectionOrigin);

            Quaternion startRotation = Quaternion.Euler(inspectableDataSO.InspectionStartRotation);
            _startXAngle = startRotation.eulerAngles.x;
            _startYAngle = startRotation.eulerAngles.y;
            _inspectionWindow.Display();
            _inspectionWindow.UpdateInspectableInfo(inspectableDataSO.InspectableTitle,  inspectableDataSO.InspectableSubTitle, inspectableDataSO.InspectableDescription);
            ResetInspection();
            _isInspecting = true;
            _mouseLayoutController.DisplayInspectionLayout();
            _cameraController.Unfocus();
        }

        public void StopInspection()
        {
            _isInspecting = false;
            Destroy(_inspectedObject);
            _inspectionWindow.Hide();
            _mouseLayoutController.HideMouseLayout();
            _cameraController.Focus();
        }

        private void Update()
        {
            if (!_isInspecting)
            {
                return;
            }

            if (!_inspectedObject)
            {
                return;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                _lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 delta = Input.mousePosition - _lastMousePosition;

                float deltaX = -delta.y * rotationSpeed * Time.deltaTime;
                float deltaY = -delta.x * rotationSpeed * Time.deltaTime;
                
                _maxXAngle = _inspectionMaxAngle.x;
                _maxYAngle = _inspectionMaxAngle.y;

                if (_maxYAngle == 360f)
                {
                    _targetYAngle += deltaY;
                }
                else
                {
                    _targetXAngle = Mathf.Clamp(_targetXAngle + deltaX, -_maxXAngle, _maxXAngle);
                    _targetYAngle = Mathf.Clamp(_targetYAngle + deltaY, -_maxYAngle, _maxYAngle);
                }

              

                _lastMousePosition = Input.mousePosition;
            }
            
            _currentXAngle = Mathf.SmoothDamp(_currentXAngle, _targetXAngle, ref _xVelocity, rotationSmoothTime);
            _currentYAngle = Mathf.SmoothDamp(_currentYAngle, _targetYAngle, ref _yVelocity, rotationSmoothTime);
                
            _inspectedObject.transform.localRotation = Quaternion.Euler(_currentXAngle, _currentYAngle, 0f);
            
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            
            if (scroll != 0f)
            {
                _targetZoom = Mathf.Clamp(_targetZoom + scroll * zoomSpeed, _inspectionMinZoom, _inspectionMaxZoom);
            }

            _currentZoom = Mathf.SmoothDamp(_currentZoom, _targetZoom, ref _zoomVelocity, zoomSmoothTime);
            _inspectedObject.transform.localScale = Vector3.one * _currentZoom;

            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetInspection();
            }
        }

        private void ResetInspection()
        {
            _targetXAngle = _startXAngle;
            _targetYAngle = _startYAngle;
            _currentXAngle = _startXAngle;
            _currentYAngle = _startYAngle;
            _lastMousePosition = Vector3.zero;
            _currentZoom = _inspectionStartSize;
            _targetZoom = _currentZoom;

            if (_inspectedObject)
            {
                _inspectedObject.transform.localRotation = Quaternion.Euler(_startXAngle, _startYAngle, 0f);
            }
        }
    }

    public interface IInspectionController
    {
        public void Inspect(InspectableDataSO inspectableDataSO);
        public void StopInspection();
    }
}