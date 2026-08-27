using System;
using System.Collections.Generic;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace Rulebound
{
    public class HazardController : MonoBehaviour, IHazardController
    {
        public Action OnHazardTriggered { get; set; }  
        
        private List<IHazard> _hazards;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            _hazards = new List<IHazard>();
            ServiceLocator.RegisterService<IHazardController>(this);
        }

        public void RegisterHazard(IHazard hazardToRegister)
        {
            if (_hazards.Contains(hazardToRegister))
            {
                return;
            }
            
            hazardToRegister.OnHazardTriggered += RaiseOnHazardTriggered;
            _hazards.Add(hazardToRegister);
        }

        private void RaiseOnHazardTriggered()
        {
            OnHazardTriggered?.Invoke();
        }

        public void EnableAllHazards()
        {
            foreach (IHazard hazard in _hazards)
            {
                hazard.EnableHazard();
            }
        }

        public void DisableAllHazards()
        {
            foreach (IHazard hazard in _hazards)
            {
                hazard.DisableHazard();
            }
        }

        public void ResetAllHazards()
        {
            foreach (IHazard hazard in _hazards)
            {
                hazard.ResetHazard();
            }
        }

        private void OnDestroy()
        {
            foreach (IHazard hazard in _hazards)
            {
                hazard.OnHazardTriggered -= OnHazardTriggered;
            }
        }
    }

    public interface IHazardController
    {
        public Action OnHazardTriggered { get; set; }
        public void RegisterHazard(IHazard hazardToRegister);
        public void EnableAllHazards();
        public void DisableAllHazards();
        public void ResetAllHazards();
    }
}