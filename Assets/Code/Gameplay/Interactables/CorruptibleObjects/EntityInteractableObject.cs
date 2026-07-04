using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace DVDNights
{
    public class EntityInteractableObject : CorruptibleInteractableObject
    {
        [SerializeField] private Volume volume;
        [SerializeField] private Animator animatorController;

        private const string IdleCorner = "Idle-Corner";
        private const string IdleSneaking = "Idle-Sneaking";
        private const string IdleTopDoor = "Idle-TopDoor";
        private const string IdleTopDoorIdle = "Idle-TopDoorIdle";

        private Bloom _bloom;
        private Tween _bloomTween;

        private void Awake()
        {
            if (!volume.profile.TryGet(out _bloom))
            {
                Debug.LogError("Bloom override not found in Volume Profile.");
            }
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
            BloomIn(0.25f);
        }

        public override void Unhighlight()
        {
            BloomOut(0.25f);
        }

        public void PlayAnimationClip()
        {
            Highlight();
            animatorController.speed = 0;
            animatorController.Play(IdleTopDoor);

            DOVirtual.DelayedCall(1f, PlayJumpScare);
        }

        private void PlayJumpScare()
        {
            animatorController.speed = 1;
            animatorController.Play(IdleTopDoor);
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
    }
}