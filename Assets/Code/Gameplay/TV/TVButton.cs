using System;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using DVDNights;
using UnityEngine;
using UnityEngine.EventSystems;

public class TVButton : MonoBehaviour, ITVButton, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuration")]
    [SerializeField] private int buttonId;
    [SerializeField] private Outline outline;
    [SerializeField] private float destinationZPosition;
    
    [Header("Feedback")]
    [SerializeField] private AudioClip feedbackClip;
    [SerializeField] private float volume = 0.5f;
    [SerializeField] private float pitch = 1f;
    [SerializeField] private bool canBeHeld = false;
    [SerializeField] private float holdThreshold = 0.15f;
    
    public Action<int> OnTvButtonPressed { get; set; }
    public Action<int> OnTvButtonHeld { get; set; }
    public Action<int> OnTvButtonReleased { get; set; }
    
    private float _originalZPosition;
    private bool _canBePressed;
    private bool _isPointerDown;
    private float _pointerDownTimer;
    private bool _holdTriggered;
    private float _heldSecondsTimer;
    
    private IDialogController _dialogController;
    private ITVButtonContextController _tvButtonContextController;

    public void EnableButton()  => _canBePressed = true;
    public void DisableButton() => _canBePressed = false;
    

    private void Awake()
    {
        _originalZPosition = transform.localPosition.z;
    }

    private void Start()
    {
        _dialogController = ServiceLocator.GetService<IDialogController>();
        _tvButtonContextController = ServiceLocator.GetService<ITVButtonContextController>();
    }

    private void Update()
    {
        if (!_isPointerDown)
        {
            return;
        }

        _pointerDownTimer += Time.deltaTime;

        if (!_holdTriggered)
        {
            if (!(_pointerDownTimer >= holdThreshold))
            {
                return;
            }

            OnTvButtonHeld?.Invoke(buttonId);
            _holdTriggered = true;
        }
    }

    public void Press()
    {
        if (!_canBePressed)
        {
            return;
        }
        
        _isPointerDown = true;
        _canBePressed = false;
        _holdTriggered = false;
        _pointerDownTimer = 0f;

        transform.DOKill();
        transform.DOLocalMoveZ(destinationZPosition, 0.15f).SetEase(Ease.Linear).OnComplete(() =>
        {
            AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, feedbackClip, volume, pitch, randomizePitch: false);

            if (!canBeHeld)
            {
                ReleaseButton();
            }
        });
    }

    public void ForceRelease()
    {
        transform.DOKill();
        transform.DOLocalMoveZ(_originalZPosition, 0.15f).SetEase(Ease.Linear).OnComplete(() =>
        {
            _canBePressed = true;
            _pointerDownTimer = 0f;
            _isPointerDown = false;
            _holdTriggered = false;
            _dialogController.HideDialog();
        });
    }

    private void ReleaseButton()
    {
        if (!_isPointerDown)
        {
            return;
        }
        
        transform.DOLocalMoveZ(_originalZPosition, 0.15f).SetEase(Ease.Linear).OnComplete(() =>
        {
            _canBePressed = true;
        });
        
        _pointerDownTimer = 0f;
        _isPointerDown = false;
                
        if (!_holdTriggered)
        {
            OnTvButtonPressed?.Invoke(buttonId);
        }
        else
        {
            OnTvButtonReleased?.Invoke(buttonId);
        }
        
        _holdTriggered = false;
        _dialogController.HideDialog();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
       Press();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ReleaseButton();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_canBePressed)
        {
            return;
        }
        
        outline.enabled = true;
        
        string contextAction = _tvButtonContextController.GetTVButtonAction(buttonId);
        
        _dialogController.DisplayDialog(contextAction);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outline.enabled = false;
        _dialogController.HideDialog();
    }
}

public interface ITVButton
{
    Action<int> OnTvButtonPressed { get; set; }
    Action<int> OnTvButtonHeld { get; set; }
    Action<int> OnTvButtonReleased { get; set; }
    
    void EnableButton();
    void DisableButton();
    void Press();
    void ForceRelease();
}
