using CorePatterns.Managers;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class DVDBoxInteractableObject : InteractableObject
    {
        [Header("References")] 
        [SerializeField] private DVDDiskInteractableObject dvdDiskInteractableObject;
        
        [Header("Configuration")] 
        [SerializeField] private Transform dvdDisk;
        [SerializeField] private Vector3 clickDVDPosition;
        [SerializeField] private Collider dvdBoxCollider;
        
        private bool _dvdTaken;
        private Tween _clickTween;
        
        public override string GetInteractionAction()
        {
            return "Remove DVD";
        }

        public override void Interact()
        {
            DisableInteraction();
            AudioManager.Instance.PlaySFX(InteractionAudioClip, pitch: 1.2f);
            _dvdTaken = true;
            _clickTween?.Kill();
            _clickTween = dvdDisk.DOLocalMove(clickDVDPosition, 0.5f).SetEase(Ease.Linear)
                .OnComplete(()=>
                {
                    dvdBoxCollider.enabled = false;
                    dvdDiskInteractableObject.EnableInteraction();
                    dvdDisk.parent = null;
                });
        }

        public override void StopInteraction()
        {
            //
        }
    }
}