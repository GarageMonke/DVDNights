using CorePatterns.Managers;
using CorePatterns.Providers.Implementations;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace DVDNights
{
    public class EntityInteractableObject : CorruptibleInteractableObject
    {
        [Header("References")]
        [SerializeField] private Volume volume;
        [SerializeField] private Animator animatorController;
        [SerializeField] private Renderer entityRenderer;
        [SerializeField] private Renderer[] entitySubRenderers;
        
        [Header("Interactables")]
        [SerializeField] private LampInteractableObject lampInteractableObject;
        
        [Header("Bone-References")]
        [SerializeField] private Transform head1;
        [SerializeField] private Transform head2;
        [SerializeField] private Transform head3;
        [SerializeField] private Transform body;

        [Header("Audio-Feedback")] 
        [SerializeField] private AudioClip warningAudioClip;
        [SerializeField] private AudioClipProvider jumpScareAudioClipProvider;
        
        [Header("Jumpscares-Data")]
        [SerializeField] private StartToTargetTweenData[] jumpScareData;

        private const string IdleCorner = "Idle-Corner";
        private const string IdleSneaking = "Idle-Sneaking";
        private const string IdleTopDoor = "Idle-TopDoor";
        private const string IdleTopDoorIdle = "Idle-TopDoorIdle";
        private const string IdleJumpScare = "Idle-Jumpscare";
        
        private readonly string[] _idleAnimations =
        {
            IdleCorner,
            IdleSneaking,
            IdleTopDoorIdle
        };

        private int _selectedAnimationIndex;
        private bool _jumpScareEnabled;
        private bool _isJumpScareScheduled;
        private bool _playerWarned;
        
        private Bloom _bloom;
        private Tween _bloomTween;
        private Tween _jumpScareTween;
        private Tween _lampOffTween;
        
        private Camera _camera;
        private Sequence _jumpScareSequence;
        private Transform _spawnParent;
        private Vector3 _spawnPosition;
        
        private ICameraController _cameraController;
        private ISanityController _sanityController;
        

        private void Awake()
        {
            if (!volume.profile.TryGet(out _bloom))
            {
                Debug.LogError("Bloom override not found in Volume Profile.");
            }
            
            jumpScareAudioClipProvider.InitializeProvider();
            _spawnPosition = transform.localPosition;
            _spawnParent = transform.parent;
            
            DisableInteraction();
        }

        protected override void Start()
        {
            base.Start();
            _cameraController = ServiceLocator.GetService<ICameraController>();
            _sanityController = ServiceLocator.GetService<ISanityController>();
            _camera = _cameraController.Camera;
        }

        public override string GetInteractionAction()
        {
            return null;
        }

        public override void Interact()
        {
            //No interaction
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                Corrupt();
            }
        }

        public override void Highlight()
        {
            BloomIn(0.5f);

            if (IsVisible())
            {
                ScheduleJumpScare();
            }
            else if (IsPartiallyVisible())
            {
                DisplayWarning();
            }
        }

        private void ScheduleJumpScare()
        {
            if (_isJumpScareScheduled)
            {
                return;
            }
            
            _isJumpScareScheduled = true;
            _jumpScareTween?.Kill();
            _jumpScareTween = DOVirtual.DelayedCall(Random.Range(0.25f, 1f), PlayJumpScare);
        }
        
        private void ScheduleTurnOffLamp()
        {
            _lampOffTween?.Kill();
            _lampOffTween = DOVirtual.DelayedCall(Random.Range(0.5f, 1.5f), TurnOffLamp);
        }

        private void DisplayWarning()
        {
            if (_playerWarned)
            {
                return;
            }

            _playerWarned = true;
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, warningAudioClip);
        }

        public override void Unhighlight()
        {
            BloomOut(0.25f);
        }

        public void ShowEntity()
        {
            EnableInteraction();
            ScheduleTurnOffLamp();
            _jumpScareEnabled = true;
            animatorController.enabled = true;
            animatorController.speed = 0;
            animatorController.Play(GetRandomIdleAnimation());
        }

        private void TurnOffLamp()
        {
            if (_isCorrupted && lampInteractableObject.IsOn)
            {
                lampInteractableObject.Interact();
            }
        }
        
        private string GetRandomIdleAnimation()
        {
            _selectedAnimationIndex = Random.Range(0, _idleAnimations.Length);
            return _idleAnimations[_selectedAnimationIndex];
        }

        private void PlayJumpScare()
        {
            if (!_jumpScareEnabled)
            {
                return;
            }

            _jumpScareEnabled = false;
            animatorController.enabled = false;
            
            Vector3 targetPosition = jumpScareData[_selectedAnimationIndex].targetPosition;
            Vector3 targetRotation = jumpScareData[_selectedAnimationIndex].targetRotation;
            
            transform.parent = _cameraController.JumpScareSpot;
            transform.localEulerAngles = targetRotation;

            _cameraController.DisableNavigation();
            
            _jumpScareSequence?.Kill();
            _jumpScareSequence = DOTween.Sequence();

            _jumpScareSequence.AppendCallback(() =>
            {
                transform.DOLocalMove(targetPosition, 0.1f)
                    .SetEase(Ease.OutSine).OnComplete((() =>
                    {
                        AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, jumpScareAudioClipProvider.GetRandomElement());
                    }));
             
            });

            _jumpScareSequence.AppendInterval(0.25f);
            
            _jumpScareSequence.AppendCallback(() =>
                {
                    _sanityController.TakeSanityImmediate(PenaltyType.EXTREME);
                    _cameraController.EnableNavigation();
                    ClearCorruption();
                });
        }

        public void PlayAnimationClip()
        {
            animatorController.speed = 0;
            animatorController.Play(IdleTopDoor);
            DOVirtual.DelayedCall(0.8f, PlayKillSequence);
        }

        public override void Corrupt()
        {
            base.Corrupt();
            lampInteractableObject.OnLampTurnedOn += ScheduleTurnOffLamp;
            ShowEntity();
        }

        public override bool CanBeCorrupted()
        {
            return !_cameraController.IsNavigationEnabled;
        }

        public override void ClearCorruption()
        {
            base.ClearCorruption();
            lampInteractableObject.OnLampTurnedOn -= ScheduleTurnOffLamp;
            DisableInteraction();
            animatorController.enabled = false;
            _jumpScareEnabled = false;
            _playerWarned = false;
            transform.parent = _spawnParent;
            transform.localPosition = _spawnPosition;
            _isJumpScareScheduled = false;
        }

        private void PlayKillSequence()
        {
            Highlight();
            animatorController.speed = 1;
            animatorController.Play(IdleTopDoor);
            AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, InteractionAudioClip, 1f);
        }

        private void TweenBloom(float targetIntensity, float duration)
        {
            if (!_bloom)
            {
                return;   
            }

            _bloomTween?.Kill();
            _bloomTween = DOTween.To(
                    () => _bloom.intensity.value,
                    x => _bloom.intensity.value = x,
                    targetIntensity,
                    duration)
                .SetEase(Ease.InOutSine);
        }

        private void BloomIn(float duration = 1f)
        {
            TweenBloom(45f, duration);
        }

        private void BloomOut(float duration = 1f)
        {
            TweenBloom(0f, duration);
        }

        private bool IsVisible()
        {
            return IsOnScreen(head1.position) || IsOnScreen(head2.position) || IsOnScreen(head3.position);
        }

        private bool IsPartiallyVisible()
        {
            return IsPartiallyOnScreen(body.position);
        }

        private bool IsOnScreen(Vector3 worldPos)
        {
            Vector3 vp = _camera.WorldToViewportPoint(worldPos);
            return vp is { z: > 0, x: > 0.05f and < 0.95f, y: > 0.05f and < 0.95f };
        }
        
        private bool IsPartiallyOnScreen(Vector3 worldPos)
        {
            Vector3 vp = _camera.WorldToViewportPoint(worldPos);
            return vp is { z: > 0, x: > -1.05f and < 1.05f, y: > -1.05f and < 1.05f };
        }

        private void EnableEntityRenderers()
        {
            entityRenderer.enabled = true;
            foreach (Renderer entitySubRenderer in entitySubRenderers)
            {
                entitySubRenderer.enabled = true;
            }
        }
        
        private void DisableEntityRenderers()
        {
            entityRenderer.enabled = false;
            foreach (Renderer entitySubRenderer in entitySubRenderers)
            {
                entitySubRenderer.enabled = false;
            }
        }
    }
}