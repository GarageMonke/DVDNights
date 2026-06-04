using CorePatterns.Managers;
using UnityEngine;

namespace DVDNights
{
    public class LampInteractableObject : InteractableObject
    {
        [Header("References")] 
        [SerializeField] private Light lampLight;
        [SerializeField] private AudioClip turnOnLampAudioClip;
        [SerializeField] private AudioClip turnOffLampAudioClip;

        private bool _isOn;

        public override string GetInteractionAction()
        {
            return _isOn ? "Turn Off" : "Turn On";
        }

        public override void Interact()
        {
            _isOn = !_isOn;
            lampLight.enabled = _isOn;

            AudioClip toPlay;
            
            if (_isOn)
            {
                toPlay = turnOnLampAudioClip;
            }
            else
            {
                toPlay = turnOffLampAudioClip;
            }
            
            AudioManager.Instance.PlaySFX(toPlay, volume: 0.5f, pitch: 1.25f);
        }

        public override void StopInteraction()
        {
            //
        }
    }
}