using System;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace Rulebound
{
    [RequireComponent(typeof(Collider))]
    public abstract class Hazard : MonoBehaviour, IHazard
    {
        [Header("References")] 
        [SerializeField] protected Renderer hazardRenderer;
        [SerializeField] protected Collider hazardCollider;
        
        public Action OnHazardTriggered { get; set; }
        
        protected bool _isEnabled;
        
        protected IHazardController _hazardController;

        protected virtual void Start()
        {
            _hazardController = ServiceLocator.GetService<IHazardController>();
            _hazardController.RegisterHazard(this);
            EnableHazard();
        }

        public void EnableHazard()
        {
            _isEnabled = true;
        }

        public void DisableHazard()
        {
            _isEnabled = false;
        }

        public abstract void ResetHazard();

        protected virtual void OnCollisionEnter(Collision other)
        {
            if (!_isEnabled)
            {
                return;
            }
            
            ICharacter character = other.gameObject.GetComponent<ICharacter>();
            
            if (character != null)
            {
                OnHazardTriggered?.Invoke();
            }
        }
    }

    public interface IHazard
    {
        public Action OnHazardTriggered { get; set; }
        public void EnableHazard();
        public void DisableHazard();
        public void ResetHazard();
    }
}