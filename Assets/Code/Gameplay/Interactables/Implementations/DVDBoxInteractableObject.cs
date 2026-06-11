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
        [SerializeField] private Transform dvdBoxFace;
        [SerializeField] private Vector3 clickDVDPosition;
        [SerializeField] private Vector3 openDVDRotation;
        [SerializeField] private Collider dvdBoxCollider;
        [SerializeField] private AudioClip openDVDBoxAudioClip;
        [SerializeField] private AudioClip closeDVDBoxAudioClip;
        
        private bool _dvdTaken;
        private Sequence _openDvdBoxSequence;
        private Vector3 _dvdDiskOriginPosition;

        private void Awake()
        {
            _dvdDiskOriginPosition = dvdDisk.localPosition;
        }
        
        public override string GetInteractionAction()
        {
            return "Remove DVD";
        }

        public override void Interact()
        {
            DisableInteraction();
          
            //_dvdTaken = true;
            _openDvdBoxSequence?.Kill();
            _openDvdBoxSequence = DOTween.Sequence()
                .AppendCallback(() =>
                {
                    AudioManager.Instance.PlaySFX(openDVDBoxAudioClip, pitch: 1.2f);
                })
                .AppendInterval(openDVDBoxAudioClip.length * 0.5f)
                .AppendCallback(() =>
                {
                    dvdBoxFace.DOLocalRotate(openDVDRotation, 0.5f).SetEase(Ease.Linear);
                })
                .AppendInterval(0.75f)
                .AppendCallback(() =>
                {
                    AudioManager.Instance.PlaySFX(InteractionAudioClip, pitch: 1.2f);
                    dvdDisk.DOLocalMove(clickDVDPosition, 0.5f).SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            //Add back again after testing
                            //dvdBoxCollider.enabled = false;
                            //dvdDiskInteractableObject.EnableInteraction();
                        });
                })
                .AppendInterval(0.75f)
                .AppendCallback(() =>
                {
                    dvdBoxFace.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.Linear);
                    EnableInteraction();
                })
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    AudioManager.Instance.PlaySFX(closeDVDBoxAudioClip, pitch: 1.2f);
                    dvdDisk.localPosition = _dvdDiskOriginPosition;
                });
        }

        public override void StopInteraction()
        {
            //
        }
    }
}