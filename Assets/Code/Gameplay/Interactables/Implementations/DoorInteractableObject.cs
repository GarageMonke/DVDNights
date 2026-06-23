using CorePatterns.Managers;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class DoorInteractableObject : InteractableObject, ICorruptibleObject
    {
        [Header("Configuration")] 
        [SerializeField] private Vector3 minOpenAngle;
        [SerializeField] private Vector3 maxOpenAngle;
        [SerializeField] private Vector3 openHandleAngle;
        [SerializeField] private Ease openEase;
        [SerializeField] private Ease closeEase;
        [SerializeField] private AudioClip closeAudioClip;
        [SerializeField] private AudioClip[] squeakAudioClips;
        [SerializeField] private Transform handleTransform;

        private bool _isOpen;
        private bool _isTweening;
        private Tweener _doorTweener;
        private Tweener _handleTweener;
        private bool _isCorrupted;

        private void Awake()
        {
            _isOpen = false;
        }
        
        public override string GetInteractionAction()
        {
            if (_isTweening)
            {
                if (_isOpen)
                {
                    return "Closing...";
                }
                
                return "Opening...";
            }
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
            _doorTweener = transform.DOLocalRotate(maxOpenAngle, InteractionAudioClip.length + 0.25f).SetEase(openEase).OnComplete(() =>
            {
                _isOpen = true;
                _isTweening = false;
                _handleTweener?.Kill();
                _handleTweener = handleTransform.DOLocalRotate(Vector3.zero, 0.15f).SetEase(closeEase);
            });
        }
        
        private void Squeak()
        {
            int randomSqueakClip = Random.Range(0, squeakAudioClips.Length);
            AudioClip squeakClip = squeakAudioClips[randomSqueakClip];
            AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, squeakClip);
            Vector3 randomOpenAngle = new Vector3(0, Random.Range(minOpenAngle.y, maxOpenAngle.y), 0);
            _isTweening = true;
            _handleTweener?.Kill();
            _handleTweener = handleTransform.DOLocalRotate(openHandleAngle, 0.15f).SetEase(openEase);
            
            _doorTweener?.Kill();
            _doorTweener = transform.DOLocalRotate(randomOpenAngle, squeakClip.length + 0.25f).SetEase(openEase).OnComplete(() =>
            {
                _isOpen = true;
                _isTweening = false;
                _handleTweener?.Kill();
                _handleTweener = handleTransform.DOLocalRotate(Vector3.zero, 0.15f).SetEase(closeEase);
            });
        }


        private void Close()
        {
            if (_isCorrupted)
            {
                ClearCorruption();
            }
            
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

        public void Corrupt()
        {
            _isCorrupted = true;
            Squeak();
        }

        public void ClearCorruption()
        {
            _isCorrupted = false;
        }

        public bool CanBeCorrupted()
        {
            return !_isOpen && !_isCorrupted;
        }
    }
}