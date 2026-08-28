using System;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace Rulebound
{
    [RequireComponent(typeof(Collider))]
    public abstract class Platform : MonoBehaviour, IPlatform
    {
        [Header("References")] 
        [SerializeField] protected Renderer platformRenderer;
        [SerializeField] protected Collider platformCollider;
        [SerializeField] protected Color platformColor; 
        
        public Action OnPlatformTriggered { get; set; }
        
        protected bool _isEnabled;
        
        protected IPlatformController _platformController;
        
        protected virtual void Start()
        {
            platformRenderer.material.color = platformColor;
            _platformController = ServiceLocator.GetService<IPlatformController>();
            _platformController.RegisterPlatform(this);
        }
        
        public virtual void EnablePlatform()
        {
            _isEnabled = true;
        }

        public virtual void DisablePlatform()
        {
            _isEnabled = false;
        }

        public abstract void ResetPlatform();
    }

    public interface IPlatform
    {
        public void EnablePlatform();
        public void DisablePlatform();
        public void ResetPlatform();
        public Action OnPlatformTriggered { get; set; }
    }
}