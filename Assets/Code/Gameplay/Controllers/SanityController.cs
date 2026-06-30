using System;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class SanityController : MonoBehaviour, ISanityController
    {
        private float _currentSanity;
        private float _maxSanity;
        
        private const float DefaultSanityThreshold = 0.05f;

        public Action OnAllSanityLost { get; set; }

        private void Awake()
        {
            ServiceLocator.RegisterService<ISanityController>(this);
            _currentSanity = 100f;
            _maxSanity = _currentSanity;
        }

        public void GainSanity()
        {
            if (_currentSanity >= _maxSanity)
            {
                return;
            }
            
            _currentSanity += DefaultSanityThreshold;
            
            Debug.Log("<color=green>[GainSanity]</color> Current sanity: " + _currentSanity);
        }

        public void LoseSanity(int multiplier)
        {
            float sanityToLose = DefaultSanityThreshold * multiplier;
            TakeSanityImmediate(sanityToLose);
            
            Debug.Log("<color=red>[LoseSanity]</color> Current sanity: " + _currentSanity);
        }

        private void TakeSanityImmediate(float sanityToLose)
        {
            _currentSanity -= sanityToLose;

            if (_currentSanity <= 0)
            {
                OnAllSanityLost?.Invoke();
                _currentSanity = 0;
            }
        }

        public void TakeSanityImmediate(SanityType sanityToTake)
        {
            _currentSanity -= GetSanityAmountByType(sanityToTake);

            if (_currentSanity <= 0)
            {
                OnAllSanityLost?.Invoke();
                _currentSanity = 0;
            }
        }

        private float GetSanityAmountByType(SanityType sanityType)
        {
            switch (sanityType)
            {
                case SanityType.LOW:
                    return 1f;
                case SanityType.MID:
                    return 2f;
                case SanityType.HIGH:
                    return 5f;
                case SanityType.EXTREME:
                    return 10f;
            }

            return 1f;
        }
    }

    public interface ISanityController
    {
        public Action OnAllSanityLost { get; set; }
        public void GainSanity();
        public void LoseSanity(int multiplier);
        public void TakeSanityImmediate(SanityType sanityType);
    }
}

public enum SanityType
{
    LOW,
    MID,
    HIGH,
    EXTREME
}