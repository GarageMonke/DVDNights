using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class CurtainInteractableObject : InteractableObject
    {
        [Header("References")]
        [SerializeField] private Transform[] curtainPanels;
        
        private bool _isOpened;
        
        private IRainController _rainController;

        private void Start()
        {
            _rainController = ServiceLocator.GetService<IRainController>();
        }

        public override string GetInteractionAction()
        {
            return _isOpened ? "Close" : "Open";
        }

        public override void Interact()
        {
            _isOpened = !_isOpened;
            
            if (_isOpened)
            {
                OpenWinds();
            }
            else
            {
                CloseWinds();   
            }
        }

        private void OpenWinds()
        {
            foreach (Transform wind in curtainPanels)
            {
                wind.DOLocalRotate(new Vector3(0f, 0f, -12f), 0.25f).SetEase(Ease.OutBack);
            }
            
            AudioManager.Instance.PlaySFX(InteractionAudioClip, volume: 1f, pitch: 2.5f);
            _rainController.PlayRain();
        }

        private void CloseWinds()
        {
            foreach (Transform wind in curtainPanels)
            {
                wind.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.25f).SetEase(Ease.OutBack);
            }
            
            AudioManager.Instance.PlaySFX(InteractionAudioClip, volume: 1f, pitch: 1.5f);
            _rainController.StopRain();
        }

        public override void StopInteraction()
        {
            //
        }
    }
}