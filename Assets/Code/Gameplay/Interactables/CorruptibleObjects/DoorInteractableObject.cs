using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DVDNights
{
    public class DoorInteractableObject : CorruptibleInteractableObject
    {
        [Header("Open/Close-Sequence")] 
        [SerializeField] private Vector3 minOpenAngle;
        [SerializeField] private Vector3 maxOpenAngle;
        [SerializeField] private Vector3 openHandleAngle;
        [SerializeField] private Ease openEase;
        [SerializeField] private Ease closeEase;
        [SerializeField] private Transform handleTransform;
        
        [Header("Feedback")]
        [SerializeField] private AudioClip closeAudioClip;
        [SerializeField] private AudioClip[] squeakAudioClips;
        [SerializeField] private AudioClip[] slowKnocksAudioClips;
        [SerializeField] private AudioClip[] fastKnocksAudioClips;
        [SerializeField] private AudioClip[] bruteKnocksAudioClips;

        private bool _isOpen;
        private bool _isTweening;
        private Tweener _doorTweener;
        private Tweener _handleTweener;
        private int _lastCorruptionIndex;
        private Sequence _knockingSequence;
        private Sequence _corruptionSequence;
        private bool _isFault;
        private ISanityController _sanityController;
        private PenaltyType _penaltyType;


        private void Awake()
        {
            _isOpen = false;
        }

        protected override void Start()
        {
            base.Start();
            _sanityController = ServiceLocator.GetService<ISanityController>();
            DisableInteraction();
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
            if (!IsEnabled)
            {
                return;
            }
            
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
            DisableInteraction();
            if (_isCorrupted && _isFault)
            {
                _isFault = false;
                _corruptionSequence?.Kill();
                _sanityController.TakeSanityImmediate(_penaltyType);
            }
            
            AudioManager.Instance.PlaySFX(AudioChannelType.DOOR, InteractionAudioClip);
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
                EnableInteraction();
            });
        }
        
        private void Squeak()
        {
            DisableInteraction();
            int randomSqueakClip = Random.Range(0, squeakAudioClips.Length);
            AudioClip squeakClip = squeakAudioClips[randomSqueakClip];
            AudioManager.Instance.PlaySFX(AudioChannelType.DOOR, squeakClip);
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
                EnableInteraction();
            });
        }


        private void Close()
        {
            DisableInteraction();
            
            if (_isCorrupted)
            {
                ClearCorruption();
            }
            
            AudioManager.Instance.PlaySFX(AudioChannelType.DOOR, closeAudioClip);
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

        public override void Corrupt()
        {
            _isCorrupted = true;
            int corruptionIndex = Random.Range(0, 3);

            while (corruptionIndex == _lastCorruptionIndex)
            {
                corruptionIndex = Random.Range(0, 3);
            }

            _lastCorruptionIndex = corruptionIndex;

            switch (corruptionIndex)
            {
                case 0:
                    Squeak();
                    return;
                case 1:
                    EnableInteraction();
                    SlowKnocking();
                    PlayCorruptionSequence();
                    _penaltyType = PenaltyType.MID;
                    return;
                case 2:
                    EnableInteraction();
                    HardKnocking();
                    PlayCorruptionSequence();
                    _penaltyType = PenaltyType.EXTREME;
                    return;
            }
        }

        private void PlayCorruptionSequence()
        {
            _isFault = true;
            _corruptionSequence?.Kill();
            _corruptionSequence = DOTween.Sequence();
            _corruptionSequence.AppendInterval(5f);
            _corruptionSequence.AppendCallback(() =>
            {
                _isFault = false;
            });

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                Corrupt();
            }
        }

        public void KnockDoor()
        {
            int randomKnockingClip = Random.Range(0, fastKnocksAudioClips.Length);
            AudioClip knockingClip = fastKnocksAudioClips[randomKnockingClip];
            AudioManager.Instance.PlaySFX(AudioChannelType.DOOR, knockingClip);
        }

        private void SlowKnocking()
        {
            int randomKnockingClip = Random.Range(0, slowKnocksAudioClips.Length);
            AudioClip knockingClip = slowKnocksAudioClips[randomKnockingClip];
            AudioManager.Instance.PlaySFX(AudioChannelType.DOOR, knockingClip);
        }
        
        private void HardKnocking()
        {
            PlayKnockingSequence(5f);
        }

        private void PlayKnockingSequence(float intensity)
        {
            _knockingSequence?.Kill();
            _knockingSequence = DOTween.Sequence();

            float rot = Mathf.Lerp(0.8f, 8f, intensity);
            float dur = Mathf.Lerp(0.10f, 0.06f, intensity);
            int vibrato = Mathf.RoundToInt(Mathf.Lerp(2f, 8f, intensity));

            for (int i = 0; i < bruteKnocksAudioClips.Length; i++)
            {
                float interval = bruteKnocksAudioClips[i].length * 0.925f;
                int index = i;

                _knockingSequence.AppendCallback(() =>
                {
                    AudioManager.Instance.PlaySFX(AudioChannelType.DOOR, bruteKnocksAudioClips[index]);
                    transform.DOPunchRotation(
                        new Vector3(0, rot, 0),
                        dur,
                        vibrato
                    );
                    
                  handleTransform.DOPunchRotation(
                        openHandleAngle,
                        interval,
                        vibrato);
                });

                _knockingSequence.AppendInterval(interval);
            }
        }
        
        public override bool CanBeCorrupted()
        {
            return !_isOpen && !_isCorrupted;
        }
    }
}