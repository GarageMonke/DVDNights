using System;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class SanityController : MonoBehaviour, ISanityController
    {
        private float _currentSanity;
        private float _maxSanity;
        private bool _isEnabled;
        
        private const float DefaultSanityThreshold = 0.1f;

        public Action OnAllSanityLost { get; set; }

        private void Awake()
        {
            ServiceLocator.RegisterService<ISanityController>(this);
            _currentSanity = 100f;
            _maxSanity = _currentSanity;
        }

        public void GainSanity()
        {
            if (!IsEnabled())
            {
                return;
            }
            
            if (_currentSanity >= _maxSanity)
            {
                return;
            }
            
            _currentSanity += DefaultSanityThreshold;
            
            Debug.Log("<color=green>[GainSanity]</color> Current sanity: " + _currentSanity);
        }

        public void LoseSanity(int multiplier)
        {
            if (!IsEnabled())
            {
                return;
            }
            
            float sanityToLose = DefaultSanityThreshold * multiplier;
            TakeSanityImmediate(sanityToLose);
            
            Debug.Log("<color=red>[LoseSanity]</color> Current sanity: " + _currentSanity);
        }

        private void TakeSanityImmediate(float sanityToLose)
        {
            if (!IsEnabled())
            {
                return;
            }
            
            _currentSanity -= sanityToLose;

            if (_currentSanity <= 0)
            {
                SequenceManager.Instance.PlayGameOverSequence();
                OnAllSanityLost?.Invoke();
                _currentSanity = 0;
            }
        }

        public void TakeSanityImmediate(PenaltyType penaltyToTake)
        {
            if (!IsEnabled())
            {
                return;
            }
            
            Debug.Log("<color=red>[PENALTY] : -</color>" + GetSanityAmountByType(penaltyToTake));
            _currentSanity -= GetSanityAmountByType(penaltyToTake);

            if (_currentSanity <= 0)
            {
                OnAllSanityLost?.Invoke();
                _currentSanity = 0;
            }
        }

        private float GetSanityAmountByType(PenaltyType penaltyType)
        {
            switch (penaltyType)
            {
                case PenaltyType.LOW:
                    return 1f;
                case PenaltyType.MID:
                    return 2f;
                case PenaltyType.HIGH:
                    return 5f;
                case PenaltyType.EXTREME:
                    return 10f;
            }

            return 1f;
        }
        
        private bool IsEnabled()
        {
            return _currentSanity > 0;
        }
    }

    public interface ISanityController
    {
        public Action OnAllSanityLost { get; set; }
        public void GainSanity();
        public void LoseSanity(int multiplier);
        public void TakeSanityImmediate(PenaltyType penaltyType);
    }
}

public enum PenaltyType
{
    LOW,
    MID,
    HIGH,
    EXTREME
}