using System.Collections;
using System.Collections.Generic;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class RulesViolationController : MonoBehaviour, IRulesViolationController
    {
        private List<string> _ruleViolations;
        private ISanityController _sanityController;
        
        private const float ViolationTickInterval = 1f;
        private Coroutine _violationCoroutine;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IRulesViolationController>(this);
            _ruleViolations = new List<string>();
        }

        private void Start()
        {
            _sanityController = ServiceLocator.GetService<ISanityController>();
        }

        public void StartCheckingForRuleViolations()
        {
            if (_violationCoroutine != null)
            {
                StopCoroutine(_violationCoroutine);
            }
            
            _violationCoroutine = StartCoroutine(RuleViolationRoutine());
        }

        public void StopCheckingForRuleViolations()
        {
            StopCoroutine(_violationCoroutine);
            _violationCoroutine = null;
        }
        
        private IEnumerator RuleViolationRoutine()
        {
            while (true)
            {
                yield return ViolationTickInterval;
 
                if (_ruleViolations.Count > 0)
                {
                    _sanityController.LoseSanity(_ruleViolations.Count);
                }
            }
        }

        public void AddRuleViolation(string objectId)
        {
            if (_ruleViolations.Contains(objectId))
            {
                return;
            }
            
            Debug.Log($"<color=red>[RuleViolation Added]</color> {objectId}");
            _ruleViolations.Add(objectId);
        }

        public void RemoveRuleViolation(string objectId)
        {
            if (!_ruleViolations.Contains(objectId))
            {
                return;
            }
            
            Debug.Log($"<color=green>[RuleViolation Removed]</color> {objectId}");
            _ruleViolations.Remove(objectId);
        }
    }

    public interface IRulesViolationController
    {
        public void AddRuleViolation(string objectId);
        public void RemoveRuleViolation(string objectId);
        public void StartCheckingForRuleViolations();
        public void StopCheckingForRuleViolations();
    }
}