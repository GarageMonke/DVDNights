using CorePatterns.Managers;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class DecoyGameRulesInteractableObject : InteractableObject
    {
        [Header("References")] 
        [SerializeField] private GameObject realGameRules;

        [Header("Slip-Sequence")] 
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private Vector3 targetRotation;
        [SerializeField] private AudioClip slipAudioClip;

        private Sequence _slipSequence;

        private void Awake()
        {
            realGameRules.SetActive(false);
        }
        
        public override string GetInteractionAction()
        {
            return null;
        }

        public void SlipTroughDoor()
        {
            _slipSequence?.Kill();
            _slipSequence = DOTween.Sequence();
            _slipSequence.AppendInterval(0.5f);
            _slipSequence.AppendCallback(() =>
            {
                AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, slipAudioClip);
                transform.DOLocalMove(targetPosition, slipAudioClip.length);
                transform.DOLocalRotate(targetRotation, slipAudioClip.length);
            });
        }

        public override void Interact()
        {
            gameObject.SetActive(false);
            realGameRules.SetActive(true);
        }
    }
}