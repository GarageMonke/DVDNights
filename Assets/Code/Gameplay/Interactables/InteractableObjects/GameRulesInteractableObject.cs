using System;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class GameRulesInteractableObject : InteractableObject
    {
        [Header("Desktop-Configuration")]
        [SerializeField] private Vector3 desktopPosition;
        [SerializeField] private Vector3 desktopRotation;
        
        [Header("Slip-Sequence")] 
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private Vector3 targetRotation;
        [SerializeField] private AudioClip slipAudioClip;
        
        [Header("Rules-Window")]
        [SerializeField] private RulesWindow rulesWindow;

        private Sequence _slipSequence;
        private bool _isInDesktop;
        private ICameraController _cameraController;

        protected override void Start()
        {
            base.Start();
            _cameraController = ServiceLocator.GetService<ICameraController>();
            rulesWindow.OnRulesAcknowledge += AcknowledgeRules;
        }

        public override string GetInteractionAction()
        {
            return "Read rules";
        }

        public void SlipTroughDoor()
        {
            ShowRules();
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
            if (!_isInDesktop)
            {
                TeleportRulesToDesktop();
            }
            
            rulesWindow.Display();
            _cameraController.DisableNavigation();
            AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, InteractionAudioClip, volume: 1f, randomizePitch: true);
        }

        public void AcknowledgeRules()
        {
            _cameraController.EnableNavigation();
        }

        public void TeleportRulesToDesktop()
        {
            transform.localPosition = desktopPosition;
            transform.localRotation = Quaternion.Euler(targetRotation);
            _isInDesktop = true;
        }

        public void ShowRules()
        {
            gameObject.SetActive(true);
        }

        public void HideRules()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            rulesWindow.OnRulesAcknowledge -= AcknowledgeRules;
        }
    }
}