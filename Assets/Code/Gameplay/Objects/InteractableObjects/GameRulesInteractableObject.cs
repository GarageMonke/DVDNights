using System;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace Rulebound
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
        [SerializeField] private AudioClip acknowledgeAudioClip;
        
        public Action OnRulesAcknowledge;
        
        private RulesWindow _rulesWindow;
        private Sequence _slipSequence;
        private bool _isInDesktop;
        private ICameraController _cameraController;
        private IInteractionController _interactionController;
        private IMouseLayoutController _mouseLayoutController;

        protected override void Start()
        {
            base.Start();
            _cameraController = ServiceLocator.GetService<ICameraController>();
            _interactionController = ServiceLocator.GetService<IInteractionController>();
            _mouseLayoutController = ServiceLocator.GetService<IMouseLayoutController>();
        }

        public override string GetInteractionAction()
        {
            return "Read rules";
        }

        public void SlipTroughDoor()
        {
            ShowRules();
            
            _cameraController ??= ServiceLocator.GetService<ICameraController>();
            _cameraController.DisableNavigation();
            
            _slipSequence?.Kill();
            _slipSequence = DOTween.Sequence();
            _slipSequence.AppendInterval(0.5f);
            _slipSequence.AppendCallback(() =>
            {
                AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, slipAudioClip);
                transform.DOLocalMove(targetPosition, slipAudioClip.length);
                transform.DOLocalRotate(targetRotation, slipAudioClip.length);
            });
            _slipSequence.AppendInterval(1f);
            _slipSequence.AppendCallback(() =>
            {
                _interactionController.StopInteractionWithObject();
            });
            _slipSequence.AppendInterval(0.5f);
            _slipSequence.AppendCallback(() =>
            {
                Quaternion targetLookRotation = Quaternion.LookRotation(transform.position - _cameraController.Camera.transform.position);
                DOTween.KillAll(complete: true);
                _cameraController.TweenToRotation(targetLookRotation, 0.25f);
            });
        }

        public override void Interact()
        {
            if (!_rulesWindow)
            {
                _rulesWindow = WindowManager.Instance.OpenWindow<RulesWindow>(gameObject, openInContainer: true);
                _rulesWindow.OnRulesAcknowledge += AcknowledgeRules;
            }

            _cameraController.DisableNavigation();
            _interactionController.DisableInteractions();
            _mouseLayoutController.HideMouseLayout();
            AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, InteractionAudioClip, volume: 1f, randomizePitch: true);
        }

        public void AcknowledgeRules()
        {
            _rulesWindow.OnRulesAcknowledge -= AcknowledgeRules;
            _rulesWindow = null;
            
            _cameraController.EnableNavigation();
            _interactionController.EnableInteractions();
            
            if (_isInDesktop)
            {
                return;
            }

            TeleportRulesToDesktop();
            OnRulesAcknowledge?.Invoke();
            OnInteractionPerformed?.Invoke();
            AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, acknowledgeAudioClip, volume: 1f);
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
    }
}