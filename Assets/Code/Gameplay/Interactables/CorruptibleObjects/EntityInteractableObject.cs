using CorePatterns.Managers;
using CorePatterns.Providers.Implementations;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace DVDNights
{
    public class EntityInteractableObject : CorruptibleInteractableObject
    {
        [Header("References")]
        [SerializeField] private Volume volume;
        [SerializeField] private Animator animatorController;
        [SerializeField] private Renderer entityRenderer;
        
        [Header("Bone-References")]
        [SerializeField] private Transform head1;
        [SerializeField] private Transform head2;
        [SerializeField] private Transform head3;

        [Header("Audio-Feedback")] 
        [SerializeField] private AudioClipProvider jumpScareAudioClipProvider;

        private const string IdleCorner = "Idle-Corner";
        private const string IdleSneaking = "Idle-Sneaking";
        private const string IdleTopDoor = "Idle-TopDoor";
        private const string IdleTopDoorIdle = "Idle-TopDoorIdle";
        
        private readonly string[] _idleAnimations =
        {
            //IdleCorner,
            IdleSneaking,
            //IdleTopDoorIdle
        };

        private Bloom _bloom;
        private Tween _bloomTween;
        private Tween _jumpScareTween;
        private Camera _camera;
        private ICameraController _cameraController;
        private ISanityController _sanityController;
        private bool _jumpScareEnabled;

        private void Awake()
        {
            if (!volume.profile.TryGet(out _bloom))
            {
                Debug.LogError("Bloom override not found in Volume Profile.");
            }
            
            jumpScareAudioClipProvider.InitializeProvider();
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

        public override void Highlight()
        {
            if (IsVisible())
            {
                PlayJumpScare();
                return;
            }
            
            BloomIn(0.25f);
        }

        public override void Unhighlight()
        {
            BloomOut(0.25f);
        }

        public void ShowEntity()
        {
            _jumpScareEnabled = true;
            animatorController.speed = 0;
            animatorController.Play(GetRandomIdleAnimation());
        }
        
        private string GetRandomIdleAnimation()
        {
            return _idleAnimations[Random.Range(0, _idleAnimations.Length)];
        }

        private void PlayJumpScare()
        {
            if (!_jumpScareEnabled)
            {
                return;
            }

            _jumpScareEnabled = false;
            _sanityController.TakeSanityImmediate(PenaltyType.EXTREME);
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, jumpScareAudioClipProvider.GetRandomElement());
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
            ShowEntity();
        }

        public override bool CanBeCorrupted()
        {
            return !_cameraController.IsNavigationEnabled;
        }

        public override void ClearCorruption()
        {
            base.ClearCorruption();
            _jumpScareEnabled = false;
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

        bool IsVisible()
        {
            return IsOnScreen(head1.position) || IsOnScreen(head2.position) || IsOnScreen(head3.position);
        }

        bool IsOnScreen(Vector3 worldPos)
        {
            Vector3 vp = _camera.WorldToViewportPoint(worldPos);

            return vp is { z: > 0, x: > 0.05f and < 0.95f, y: > 0.05f and < 0.95f };
        }
    }
}