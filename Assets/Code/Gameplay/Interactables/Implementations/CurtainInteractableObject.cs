using System;
using System.Collections.Generic;
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

        [Header("Feedback")] 
        [SerializeField] private AudioClip interactionAudioClip;
        
        private bool _isOpened;
        
        private IRainController _rainController;

        private void Start()
        {
            _rainController = ServiceLocator.GetService<IRainController>();
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
            
            AudioManager.Instance.PlaySFX(interactionAudioClip, 0.5f);
            _rainController.PlayRain();
        }

        private void CloseWinds()
        {
            foreach (Transform wind in curtainPanels)
            {
                wind.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.25f).SetEase(Ease.OutBack);
            }
            
            AudioManager.Instance.PlaySFX(interactionAudioClip, 0.5f, 0.95f);
            _rainController.StopRain();
        }

        public override void StopInteraction()
        {
            //
        }
    }
}