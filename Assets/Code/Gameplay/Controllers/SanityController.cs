using System;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class SanityController : MonoBehaviour, ISanityController
    {
        private float _currentSanity;
        private float _maxSanity;
        
        private const float DefaultSanityThreshold = 0.1f;

        public Action OnAllSanityLost { get; set; }

        private void Awake()
        {
            ServiceLocator.RegisterService<ISanityController>(this);
        }

        public void GainSanity()
        {
            if (_currentSanity >= _maxSanity)
            {
                return;
            }
            
            _currentSanity += DefaultSanityThreshold;
            
            Debug.Log("<color=green>[GainSanity] Current sanity: </color>" + _currentSanity);
        }

        public void LoseSanity(int multiplier)
        {
            float sanityToLose = DefaultSanityThreshold * multiplier;
            TakeSanityImmediate(sanityToLose);
            
            Debug.Log("<color=red>[LoseSanity] Current sanity: </color>" + _currentSanity);
        }

        public void TakeSanityImmediate(float sanityToTake)
        {
            _currentSanity -= sanityToTake;

            if (_currentSanity <= 0)
            {
                OnAllSanityLost?.Invoke();
                _currentSanity = 0;
            }
        }
    }

    public interface ISanityController
    {
        public Action OnAllSanityLost { get; set; }
        public void GainSanity();
        public void LoseSanity(int multiplier);
        public void TakeSanityImmediate(float sanityToTake);
    }
}