using System;
using System.Collections;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public abstract  class CorruptibleInteractableObject : InteractableObject, ICorruptibleObject
    {
        public Action<string> OnCooldownFinished { get; set; }
        
        public string ObjectId => gameObject.name;
        
        protected bool _isCorrupted;
        protected IDecayController _decayController;
        protected ISanityController _sanityController;
        protected IRulesViolationController _rulesViolationController;

        protected override void Start()
        {
            base.Start();
            _decayController = ServiceLocator.GetService<IDecayController>();
            _decayController.RegisterCorruptibleObject(this);
            _sanityController =  ServiceLocator.GetService<ISanityController>();
            _rulesViolationController = ServiceLocator.GetService<IRulesViolationController>();
        }
        
        public virtual void Corrupt()
        {
            _isCorrupted = true;
        }

        public virtual void ClearCorruption()
        {
            _decayController.ClearObject(ObjectId);
            _isCorrupted = false;
        }

        public virtual bool CanBeCorrupted()
        {
            return !_isCorrupted;
        }

        private IEnumerator StartCooldownRoutine()
        {
            yield return new WaitForSeconds(300f);
            OnCooldownFinished?.Invoke(ObjectId);
        }
        
        public void CooldownObject()
        {
            StartCoroutine(StartCooldownRoutine());
        }

        public bool IsCorrupted()
        {
            return _isCorrupted;
        }
    }
}