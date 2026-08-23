using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace Rulebound
{
    public class CurtainInteractableObject : InteractableObject
    {
        [Header("References")]
        [SerializeField] private Transform[] curtainPanels;
        
        private bool _isOpened;
        
        private IThunderController _thunderController;

        protected override void Start()
        {
            base.Start();
            _thunderController = ServiceLocator.GetService<IThunderController>();
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
                wind.DOLocalRotate(new Vector3(0f, 0f, -45f), 0.25f).SetEase(Ease.OutBack);
            }
            
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, InteractionAudioClip, volume: 1f, pitch: 2.5f);
            _thunderController.PlayRain();
        }

        private void CloseWinds()
        {
            foreach (Transform wind in curtainPanels)
            {
                wind.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.25f).SetEase(Ease.OutBack);
            }
            
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, InteractionAudioClip, volume: 1f, pitch: 1.5f);
            _thunderController.StopRain();
        }
    }
}