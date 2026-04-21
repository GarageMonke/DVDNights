using System;
using CorePatterns.Managers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class TVButton : MonoBehaviour, ITVButton, IPointerDownHandler
{
    [Header("Configuration")]
    [SerializeField] private int buttonId;
    [SerializeField] private float destinationZPosition;
    
    [Header("Feedback")]
    [SerializeField] private AudioClip feedbackClip;
    
    public Action<int> OnTvButtonPressed { get; set; }
    
    public int ButtonId => buttonId;

    public float _originalZPosition;
    
    private bool _canBePressed;

    private void Awake()
    {
        _originalZPosition = transform.localPosition.z;
    }

    public void Press()
    {
        if (!_canBePressed)
        {
            return;
        }
        
        PlayButtonAnimation();
    }

    public void EnableButton()
    {
        _canBePressed = true;
    }

    private void PlayButtonAnimation()
    {
        _canBePressed = false;
        transform.DOKill();

        transform.DOLocalMoveZ(destinationZPosition, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
        {
            AudioManager.Instance.PlaySFX(feedbackClip, 0.5f, randomizePitch: false);
            transform.DOLocalMoveZ(_originalZPosition, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _canBePressed = true;
                OnTvButtonPressed?.Invoke(buttonId); 
            });;
        });
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Press();
    }
}

public interface ITVButton
{
    public Action<int> OnTvButtonPressed { get; set; }
    public void Press();
    public void EnableButton();
}
