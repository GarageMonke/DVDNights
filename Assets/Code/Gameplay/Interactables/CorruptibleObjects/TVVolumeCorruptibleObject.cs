using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DVDNights
{
    public class TVVolumeCorruptibleObject : CorruptibleObject
    {
        private ITVVolumeController _tvVolumeController;
        private ITVStateController _tvStateController;
        private int _targetVolume;

        protected override void Start()
        {
            base.Start();
            _tvStateController = ServiceLocator.GetService<ITVStateController>();
        }

        private void OnDestroy()
        {
            _tvVolumeController.OnVolumeChanged -= CheckRuleViolation;
            _tvVolumeController.OnVolumeChanged -= CheckTargetVolume;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                Corrupt();
            }
        }

        public override void Corrupt()
        {
            base.Corrupt();

            _tvVolumeController ??= ServiceLocator.GetService<ITVVolumeController>();
            
            _rulesViolationController.RemoveRuleViolation(ObjectId);
            
            _tvVolumeController.DisableController();
            
            _tvVolumeController.OnVolumeChanged -= CheckRuleViolation;
            _tvVolumeController.OnVolumeChanged += CheckTargetVolume;
            
            _targetVolume = GetRandomOddNumberInRange(_tvVolumeController.GetMinVolume(), _tvVolumeController.GetMaxVolume());

            while (_targetVolume == _tvVolumeController.GetVolume())
            {
                _targetVolume = GetRandomOddNumberInRange(_tvVolumeController.GetMinVolume(), _tvVolumeController.GetMaxVolume());
            }

            if (_tvVolumeController.GetVolume() > _targetVolume)
            {
                _tvVolumeController.HoldVolumeDown();
            }
            else
            {
                _tvVolumeController.HoldVolumeUp();
            }
        }

        private void CheckTargetVolume()
        {
            if (!_isCorrupted)
            {
                return;
            }

            if (_tvVolumeController.GetVolume() != _targetVolume)
            {
                return;
            }
            
            _tvVolumeController.StopHold();
            _tvVolumeController.EnableController();
            _tvVolumeController.OnVolumeChanged -= CheckTargetVolume;
            _tvVolumeController.OnVolumeChanged += CheckRuleViolation;

            DOVirtual.DelayedCall(0.5f, () => _tvVolumeController.StopHold());
        }
        
        private void CheckRuleViolation()
        {
            bool isValidVolume = _tvVolumeController.GetVolume() % 2 == 0;

            if (_isCorrupted)
            {
                if (isValidVolume)
                {
                    ClearCorruption();
                }
            }

            if (_tvVolumeController.IsHolding)
            {
                return;
            }

            if (isValidVolume)
            {
                _rulesViolationController.RemoveRuleViolation(ObjectId);
            }
            else
            {
                _rulesViolationController.AddRuleViolation(ObjectId);
            }
        }

        private int GetRandomOddNumberInRange(int min, int max)
        {
            int firstOdd = (min % 2 == 0) ? min + 1 : min;
            int lastOdd = (max % 2 == 0) ? max - 1 : max;

            int count = ((lastOdd - firstOdd) / 2) + 1;
            return firstOdd + Random.Range(0, count) * 2;
        }

        public override bool CanBeCorrupted()
        {
            return _tvStateController.IsPlayingGame;
        }
    }
}