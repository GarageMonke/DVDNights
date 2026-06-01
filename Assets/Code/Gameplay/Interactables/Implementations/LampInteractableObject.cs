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
        
        public override void Interact()
        {
            _isOn = !_isOn;
            lampLight.enabled = _isOn;

            AudioClip toPlay;
            float pitch;
            
            if (_isOn)
            {
                toPlay = turnOnLampAudioClip;
                pitch = 1.1f;
            }
            else
            {
                toPlay = turnOffLampAudioClip;
                pitch = 0.8f;
            }
            
            AudioManager.Instance.PlaySFX(toPlay, volume: 0.5f, pitch: pitch, randomizePitch: true);
        }

        public override void StopInteraction()
        {
            //
        }
    }
}