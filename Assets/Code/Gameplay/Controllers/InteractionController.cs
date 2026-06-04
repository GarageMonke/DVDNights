
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
    private Tween _currentTween;

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
        crosshairImage.color = undetectedColor;
        TweenToSize(Vector2.one);
        _dialogController.HideDialog();
    }

    private void InteractWithObject(IInteractableObject interactableObject)
    {
        if (_isInteracting)
        {
            return;
        }
        
        _currentInteraction = interactableObject;
        _currentInteraction.Interact();
        _currentHighlighted.Unhighlight();

        if (_currentInteraction.HasNavigation)
        {
            _cameraController.DisableNavigation();
            _isInteracting = true;
            crosshairImage.gameObject.SetActive(false);
            _dialogController.HideDialog();
        }
    }

    private void HighlightObject(IInteractableObject interactableObject)
    {
        if (_isInteracting)
        {
            return;
        }
        
        if (_currentHighlighted != null && _currentHighlighted != interactableObject)
        {
            _currentHighlighted.Unhighlight();
        }

        _currentHighlighted = interactableObject;
        crosshairImage.color = detectedColor;
        _currentHighlighted.Highlight();
        TweenToSize(Vector2.one * 1.5f);
        
        _dialogController.DisplayDialog(_currentHighlighted.GetInteractionAction());
    }

    private void StopInteractionWithObject()
    {
        if (_currentInteraction == null)
        {
            return;
        }
        
        if (!_currentInteraction.HasNavigation)
        {
            return;
        }

        _currentInteraction.StopInteraction();
        _currentInteraction = null;
        _isInteracting = false;
        crosshairImage.gameObject.SetActive(true);
        _cameraController.EnableNavigation();
        _dialogController.HideDialog();
    }
    
    private void TweenToSize(Vector2 targetSize)
    {
        _currentTween?.Kill();
        _currentTween = crosshairImage.transform
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
}

public interface IInteractionController
{
    public void EnableInteractions();
    public void DisableInteractions();
}