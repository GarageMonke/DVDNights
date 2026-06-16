using CorePatterns.Managers;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class DoorInteractableObject : InteractableObject
    {
        [Header("Configuration")] 
        [SerializeField] private Vector3 openAngle;
        [SerializeField] private Vector3 openHandleAngle;
        [SerializeField] private Ease openEase;
        [SerializeField] private Ease closeEase;
        [SerializeField] private AudioClip closeAudioClip;
        [SerializeField] private Transform handleTransform;

        private bool _isOpen;
        private bool _isTweening;
        private Tweener _doorTweener;
        private Tweener _handleTweener;

        private void Awake()
        {
            _isOpen = false;
        }
        
        public override string GetInteractionAction()
        {
            return _isOpen ? "Close" : "Open";
        }

        public override void Interact()
        {
            if (_isTweening)
            {
                return;
            }
            
            if (_isOpen)
            {
                Close();
                return;
            }
            
            Open();
        }

        private void Open()
        {
            AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, InteractionAudioClip);
            _isTweening = true;
            _handleTweener?.Kill();
            _handleTweener = handleTransform.DOLocalRotate(openHandleAngle, 0.15f).SetEase(openEase);
            
            _doorTweener?.Kill();
            _doorTweener = transform.DOLocalRotate(openAngle, InteractionAudioClip.length + 0.25f).SetEase(openEase).OnComplete(() =>
            {
                _isOpen = true;
                _isTweening = false;
                _handleTweener?.Kill();
                _handleTweener = handleTransform.DOLocalRotate(Vector3.zero, 0.15f).SetEase(closeEase);
            });
            
           
        }

        private void Close()
        {
            AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, closeAudioClip);
            _isTweening = true;
            
            _handleTweener?.Kill();
            _handleTweener = handleTransform.DOLocalRotate(openHandleAngle, 0.15f).SetEase(closeEase);
            
            _doorTweener?.Kill();
            _doorTweener = transform.DOLocalRotate(Vector3.zero, closeAudioClip.length - 0.25f).SetEase(closeEase).OnComplete(() =>
            {
                _isOpen = false;
                _isTweening = false;
                _handleTweener?.Kill();
                _handleTweener = handleTransform.DOLocalRotate(Vector3.zero, 0.1f).SetEase(closeEase);
            });
            
           
        }

        public override void StopInteraction()
        {
            
        }
    }
}