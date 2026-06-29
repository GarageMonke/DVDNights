using System;
using System.Collections;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public abstract  class CorruptibleInteractableObject : InteractableObject, ICorruptibleObject
    {
        [SerializeField] private string objectId;

        public Action<string> OnCooldownFinished { get; set; }
        
        public string ObjectId => objectId;
        
        protected bool _isCorrupted;
        private IDecayController _decayController;

        protected override void Start()
        {
            base.Start();
            _decayController = ServiceLocator.GetService<IDecayController>();
            _decayController.RegisterCorruptibleObject(this);
        }
        

        public virtual void Corrupt()
        {
            _isCorrupted = true;
        }

        public virtual void ClearCorruption()
        {
            _decayController.ClearObject(objectId);
            _isCorrupted = false;
        }

        public abstract bool CanBeCorrupted();

        private IEnumerator StartCooldownRoutine()
        {
            yield return new WaitForSeconds(300f);
            OnCooldownFinished?.Invoke(objectId);
        }
        
        public void CooldownObject()
        {
            StartCoroutine(StartCooldownRoutine());
        }
    }
}