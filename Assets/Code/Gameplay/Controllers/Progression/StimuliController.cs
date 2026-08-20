using System;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class StimuliController : MonoBehaviour, IStimuliController
    {
        [Header("Stimuli")] 
        [SerializeField] private float anxiety;
        [SerializeField] private float sleep;

        [Header("Thresholds")] 
        [SerializeField] private float afkThreshold = 10f;

        [Header("Anxiety")] 
        [SerializeField] private float anxietyIncreaseRate = 0.1f;
        [SerializeField] private float anxietyDecayRate = 0.25f;
        [SerializeField] private Color anxietyColor = Color.red;
        
        [Header("Sleep")] 
        [SerializeField] private float sleepIncreaseRate = 0.1f;
        [SerializeField] private float sleepDecayRate = 0.25f;
        [SerializeField] private FadeInOutBlack fadeInOutBlack;

        private bool _isEnabled;

        private bool _anxietyEnabled;
        private bool _sleepEnabled;
        
        private IAFKController _afkController;
        private ITrackSelectionController _trackSelectionController;
        private IFastForwardController _fastForwardController;

        public Action OnSleepTriggered { get; set; }
        public Action OnAnxietyTriggered { get; set; }
        
        private void Awake()
        {
            InstallService();
        }

        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            UpdateAnxiety();
            UpdateSleep();
        }

        private void UpdateAnxiety()
        {
            bool shouldIncrease = IncreaseAnxiety();

            if (shouldIncrease)
            {
                anxiety += anxietyIncreaseRate * Time.deltaTime;
            }
            else
            {
                anxiety -= anxietyDecayRate * Time.deltaTime;
            }

            anxiety = Mathf.Clamp01(anxiety);
            Debug.Log("Anxiety: " + anxiety);

            if (anxiety >= 1f)
            {
                TriggerAnxiety();
            }
        }

        private void UpdateSleep()
        {
            bool shouldIncrease = IncreaseSleep();

            if (shouldIncrease)
            {
                sleep += sleepIncreaseRate * Time.deltaTime;
                fadeInOutBlack.FadeIn(sleep, null);
            }
            else
            {
                sleep -= sleepDecayRate * Time.deltaTime;
                fadeInOutBlack.FadeOut(sleep, null);
            }

            sleep = Mathf.Clamp01(sleep);
            
            if (sleep >= 1f)
            {
                TriggerSleep();
            }
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IStimuliController>(this);
            EnableAnxiety();
            EnableSleep();
        }

        private void Start()
        {
            GetServices();
        }

        private void GetServices()
        {
            _afkController = ServiceLocator.GetService<IAFKController>();
            _trackSelectionController = ServiceLocator.GetService<ITrackSelectionController>();
            _fastForwardController = ServiceLocator.GetService<IFastForwardController>();
        }
        
        private bool IncreaseSleep()
        {
            return _sleepEnabled && (IsPlayerAFK() || IsFastForwardingTooMuch());
        }

        private bool IncreaseAnxiety()
        {
            return _anxietyEnabled && !_trackSelectionController.IsPlayingTrack;
        }

        private bool IsPlayerAFK()
        {
            return !_fastForwardController.IsForwarding && _afkController.AFKTime >= afkThreshold;
        }

        private bool IsFastForwardingTooMuch()
        {
            return _fastForwardController.IsForwarding && _fastForwardController.FastForwardingTime >= afkThreshold;
        }

        private void TriggerAnxiety()
        {
            _anxietyEnabled = false;
            OnAnxietyTriggered?.Invoke();
        }

        private void TriggerSleep()
        {
            _sleepEnabled = false;
            OnSleepTriggered?.Invoke();
        }

        public void EnableController()
        {
            _isEnabled = true;
            GetServices();
        }

        public void DisableController()
        {
            _isEnabled = false;
        }

        public void EnableSleep()
        {
            _sleepEnabled = true;
        }

        public void EnableAnxiety()
        {
            _anxietyEnabled = true;
        }
    }

    public interface IStimuliController
    {
        public Action OnSleepTriggered { get; set; }
        public Action OnAnxietyTriggered { get; set; }
        public void EnableController();
        public void DisableController();
        public void EnableSleep();
        public void EnableAnxiety();
    }
}