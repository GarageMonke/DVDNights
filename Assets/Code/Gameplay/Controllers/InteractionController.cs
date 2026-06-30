
using System;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using DVDNights;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour, IInteractionController
{
    [Header("Input Action")]
    [SerializeField] private InputActionSO _interactInputActionSO;
    [SerializeField] private InputActionSO _stopInteractionInputActionSO;
    
    [Header("Raycast Settings")]
    [SerializeField] private float _interactionRange = 3f;
    [SerializeField] private LayerMask _interactableLayer;

    [Header("Interaction Settings")] 
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Color detectedColor;
    [SerializeField] private Color undetectedColor;

    private ICameraController _cameraController;
    private IInteractableObject _currentInteraction;
    private IInteractableObject _currentHighlighted;
    private Camera _camera;

    private bool _isEnabled;
    private Tween _crossHairTween;

    private bool _isInteracting;

    private InputAction _interactInputAction;
    private InputAction _stopInteractionInputAction;
    private IDialogController _dialogController;

    private void Awake()
    {
        InstallService();
    }

    private void InstallService()
    {
        _interactInputAction = _interactInputActionSO.GetInputAction();
        _stopInteractionInputAction = _stopInteractionInputActionSO.GetInputAction();

        _interactInputAction.performed += ExecuteInteract;
        _stopInteractionInputAction.performed += ExecuteStopInteraction;

        ServiceLocator.RegisterService<IInteractionController>(this);
    }


    private void OnDestroy()
    {
        _interactInputAction.performed -= ExecuteInteract;
        _stopInteractionInputAction.performed -= ExecuteStopInteraction;
    }

    private void Start()
    {
        _dialogController = ServiceLocator.GetService<IDialogController>();
        _cameraController = ServiceLocator.GetService<ICameraController>();
        _cameraController.EnableNavigation();
        _camera = _cameraController.Camera;
    }

    private void Update()
    {
        if (!_isEnabled)
        {
            return;
        }
        
        HandleRaycast();
    }

    private void ExecuteInteract(InputAction.CallbackContext context)
    {
        if (!_isEnabled)
        {
            return;
        }

        if (_currentHighlighted != null)
        {
            InteractWithObject(_currentHighlighted);
        }
    }

    private void ExecuteStopInteraction(InputAction.CallbackContext context)
    {
        if (!_isEnabled)
        {
            return;
        }
        
        StopInteractionWithObject();
    }

    private void HandleRaycast()
    {
        Ray ray;

        if (_isInteracting)
        {
            ray = _camera.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            ray = new Ray(_camera.transform.position, _camera.transform.forward);
        }
  
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
        crosshairImage.color = undetectedColor;
        TweenToSize(Vector2.one);
        _dialogController.HideDialog();
    }

    private void InteractWithObject(IInteractableObject interactableObject)
    {
        if (!interactableObject.HasIgnoreNavigation)
        {
            if (_isInteracting)
            {
                return;
            }
        }
        
        if (_currentInteraction != null)
        {
            if (!_currentInteraction.IsEnabled)
            {
                return; 
            }
        }
        
        HandleInteraction(interactableObject);
    }

    private void HandleInteraction(IInteractableObject interactableObject)
    {
        if (interactableObject == null)
        {
            return;
        }
        
        _currentInteraction = interactableObject;

        if (_currentHighlighted != null)
        {
            _currentHighlighted.Unhighlight();
        }

        if (_currentInteraction.HasNavigation)
        {
            _cameraController.DisableNavigation();
            _isInteracting = true;
            crosshairImage.gameObject.SetActive(false);
            _dialogController.HideDialog();
        }
        
        _currentInteraction.Interact();

        if (!_currentInteraction.IsEnabled)
        {
            _currentInteraction = null;
            ClearHighlight();
        }
    }

    private void HighlightObject(IInteractableObject interactableObject)
    {
        if (!interactableObject.HasIgnoreNavigation)
        {
            if (_isInteracting)
            {
                return;
            }
        }

        if (_currentHighlighted != null && _currentHighlighted != interactableObject)
        {
            _currentHighlighted.Unhighlight();
        }

        _currentHighlighted = interactableObject;

        if (_currentHighlighted.IsEnabled)
        {
            crosshairImage.color = detectedColor;
            _currentHighlighted.Highlight();
            TweenToSize(Vector2.one * 1.5f);
            _dialogController.DisplayDialog(_currentHighlighted.GetInteractionAction());
        }
    }

    public void StopInteractionWithObject()
    {
        if (_currentInteraction == null)
        {
            return;
        }
        
        if (!_currentInteraction.HasNavigation)
        {
            return;
        }
        
        _isInteracting = false;
        crosshairImage.gameObject.SetActive(true);
        _cameraController.EnableNavigation();
        _currentInteraction.StopInteraction();
        _currentInteraction = null;
        _dialogController.HideDialog();
    }

    public void SetCurrentInteraction(IInteractableObject interactableObject)
    {
        _currentInteraction = interactableObject;
    }

    private void TweenToSize(Vector2 targetSize)
    {
        _crossHairTween?.Kill();
        _crossHairTween = crosshairImage.transform
            .DOScale(targetSize, 0.25f)
            .SetEase(Ease.Linear);
    }

    public void EnableInteractions()
    {
        _isEnabled = true;
        _isInteracting = false;
    }

    public void DisableInteractions()
    {
        _isEnabled = false;
    }

    public void ForceInteraction(IInteractableObject interactableObject)
    {
        HandleInteraction(interactableObject);
    }
}

public interface IInteractionController
{
    public void EnableInteractions();
    public void DisableInteractions();
    public void ForceInteraction(IInteractableObject interactableObject);
    public void StopInteractionWithObject();
    public void SetCurrentInteraction(IInteractableObject interactableObject);
}