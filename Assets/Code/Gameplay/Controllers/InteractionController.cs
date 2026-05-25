
using CorePatterns.ServiceLocator;
using DVDNights;
using UnityEngine;

public class InteractionController : MonoBehaviour, IInteractionController
{
    [Header("Raycast Settings")]
    [SerializeField] private float _interactionRange = 3f;
    [SerializeField] private LayerMask _interactableLayer;

    private ICameraController _cameraController;
    private IInteractableObject _currentInteraction;
    private IInteractableObject _currentHighlighted;
    private Camera _camera;

    private bool _isEnabled;

    private void Start()
    {
        _cameraController = ServiceLocator.GetService<ICameraController>();
        _cameraController.EnableNavigation();
        _camera = _cameraController.Camera;
        _isEnabled = true;
    }

    private void Update()
    {
        if (!_isEnabled)
        {
            return;
        }
        
        HandleRaycast();

        if (Input.GetMouseButton(0))
        {
            if (_currentHighlighted != null)
            {
                InteractWithObject(_currentHighlighted);
            }
        }

        if (Input.GetMouseButton(1))
        {
            StopInteractionWithObject();
        }
    }

    private void HandleRaycast()
    {
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, _interactionRange, _interactableLayer))
        {
            if (hit.collider.TryGetComponent(out IInteractableObject interactable))
            {
                HighlightObject(interactable);
                return;
            }
        }
        
        ClearHighlight();
    }

    private void ClearHighlight()
    {
        if (_currentHighlighted == null) return;

        _currentHighlighted.Unhighlight();
        _currentHighlighted = null;
    }

    private void InteractWithObject(IInteractableObject interactableObject)
    {
        _currentInteraction = interactableObject;
        _currentInteraction.Interact();

        if (_currentInteraction.OverrideCamera)
        {
            _cameraController.UpdateCameraPositionAndRotation(_currentInteraction.CameraPosition, _currentInteraction.CameraRotation);
        }
        
        _cameraController.DisableNavigation();
    }

    private void HighlightObject(IInteractableObject interactableObject)
    {
        if (_currentHighlighted != null && _currentHighlighted != interactableObject)
        {
            _currentHighlighted.Unhighlight();
        }

        _currentHighlighted = interactableObject;
        _currentHighlighted.Highlight();
    }

    private void StopInteractionWithObject()
    {
        if (_currentInteraction == null) return;

        _currentInteraction.StopInteraction();

        if (_currentInteraction.OverrideCamera)
        {
            _cameraController.RestoreCameraPositionAndRotation();
        }

        _currentInteraction = null;
        _cameraController.EnableNavigation();
    }
}

public interface IInteractionController
{
}