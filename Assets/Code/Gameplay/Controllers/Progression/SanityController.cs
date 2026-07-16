using System;
using CorePatterns.Managers;
using CorePatterns.Providers.Implementations;
using CorePatterns.ServiceLocator;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DVDNights
{
    public class SanityController : MonoBehaviour, ISanityController
    {
        [Header("Audio-Feedback")]
        [SerializeField] private AudioClipProvider heartbeatAudioClipProvider;
        [SerializeField] private AudioClipProvider breathingAudioClipProvider;
        
        private float _currentSanity;
        private float _maxSanity;
        private bool _isEnabled;
        private SanityLevel _currentSanityLevel;
        
        private const float DefaultSanityThreshold = 0.1f;

        public Action OnAllSanityLost { get; set; }

        private void Awake()
        {
            ServiceLocator.RegisterService<ISanityController>(this);
            
            //This should change depending on the loaded state
            _currentSanity = 100f;
            _currentSanityLevel = SanityLevel.HEALTHY;
            _maxSanity = _currentSanity;
            
            heartbeatAudioClipProvider.InitializeProvider();
            breathingAudioClipProvider.InitializeProvider();
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
            else
            {
                SanityLevel updatedSanityLevel = GetSanityLevel();

                if (_currentSanityLevel != updatedSanityLevel)
                {
                    _currentSanityLevel = updatedSanityLevel;
                    PlayHeartbeatFeedbackBySanityLevel();
                }
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
            PlayBreathingFeedbackByPenaltyType(penaltyToTake);

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

        private SanityLevel GetSanityLevel()
        {
            float percentage = _currentSanity / _maxSanity;

            //Healthy
            if (percentage >= 0.8f)
            {
                return SanityLevel.HEALTHY;
            }

            //Good
            if (percentage >= 0.6f)
            {
                return SanityLevel.GOOD;
            }

            //Moderate
            if (percentage >= 0.4f)
            {
               return SanityLevel.MODERATE;
            }

            //Low
            if (percentage >= 0.2f)
            {
               return SanityLevel.LOW;
            }
            
            //Critical
            return SanityLevel.CRITICAL;
        }

        private void PlayHeartbeatFeedbackBySanityLevel()
        {
            int randomIndex = Random.Range(1, 3);
            
            switch (_currentSanityLevel)
            {
                case SanityLevel.HEALTHY:
                    break;
                case SanityLevel.GOOD:
                    AudioManager.Instance.StopOST(AudioChannelType.HEARTBEAT);
                    break;
                case SanityLevel.MODERATE:
                    AudioManager.Instance.PlayOST(AudioChannelType.HEARTBEAT, heartbeatAudioClipProvider.GetElementById("slow"+randomIndex), loop: true);
                    break;
                case SanityLevel.LOW:
                    AudioManager.Instance.PlayOST(AudioChannelType.HEARTBEAT, heartbeatAudioClipProvider.GetElementById("medium"+randomIndex), loop: true);
                    break;
                case SanityLevel.CRITICAL:
                    AudioManager.Instance.PlayOST(AudioChannelType.HEARTBEAT, heartbeatAudioClipProvider.GetElementById("fast"), loop: true);
                    break;
            }
        }
        
        private void PlayBreathingFeedbackByPenaltyType(PenaltyType penaltyType)
        {
            int randomIndex = Random.Range(1, 3);
            
            switch (penaltyType)
            {
                case PenaltyType.LOW:
                    break;
                case PenaltyType.MID:
                    AudioManager.Instance.PlaySFX(AudioChannelType.BREATHING, breathingAudioClipProvider.GetElementById("sigh"));
                    break;
                case PenaltyType.HIGH:
                    AudioManager.Instance.PlaySFX(AudioChannelType.BREATHING, breathingAudioClipProvider.GetElementById("heavy"+randomIndex));
                    break;
                case PenaltyType.EXTREME:
                    AudioManager.Instance.PlaySFX(AudioChannelType.BREATHING, breathingAudioClipProvider.GetElementById("heavy"+randomIndex));
                    break;
            }
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

public enum SanityLevel
{
    HEALTHY,
    GOOD,
    MODERATE,
    LOW,
    CRITICAL
}