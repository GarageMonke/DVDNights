using CorePatterns.Managers;
using UnityEngine;

namespace DVDNights
{
    public class LampInteractableObject : InteractableObject
    {
        [Header("References")] 
        [SerializeField] private Light lampLight;
        
        [Header("Feedback")]
        [SerializeField] private AudioClip turnOnLampAudioClip;
        [SerializeField] private AudioClip turnOffLampAudioClip;

        [Header("Materials")] 
        [SerializeField] private Renderer lampRenderer;
        [SerializeField] private Material shadeEmissiveMaterial;
        [SerializeField] private Material shadeMaterial;

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
                OverrideShadeMaterial(shadeEmissiveMaterial);
            }
            else
            {
                toPlay = turnOffLampAudioClip;
                OverrideShadeMaterial(shadeMaterial);
            }
            
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, toPlay, volume: 0.5f, pitch: 1.25f);
        }

        private void OverrideShadeMaterial(Material newMaterial)
        {
            Material[] mats = lampRenderer.materials;
            mats[1] = new Material(newMaterial);
            lampRenderer.materials = mats;
            
        }

        public override void StopInteraction()
        {
            //
        }
    }
}