using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class DecayController : MonoBehaviour, IDecayController
    {
        private Dictionary<string, ICorruptibleObject> _corruptibleObjects;
        private Dictionary<string, ICorruptibleObject> _corruptedObjects;
        private Dictionary<string, ICorruptibleObject> _coolingDownObjects;
        private Dictionary<string, ICorruptibleObject> _availableObjects;
        private Coroutine _tickCoroutine;
        private Coroutine _decayCoroutine;

        private const float CorruptedTickInterval = 1f;
        private WaitForSeconds _corruptedTickInterval;
        private ISanityController _sanityController;
        
        private float _naturalDecayMaxInterval = 900f;
        private float _naturalDecayMinInterval = 120f;

        private void Awake()
        {
            ServiceLocator.RegisterService<IDecayController>(this);
            _corruptibleObjects = new Dictionary<string, ICorruptibleObject>();
            _corruptedObjects = new Dictionary<string, ICorruptibleObject>();
            _coolingDownObjects = new Dictionary<string, ICorruptibleObject>();
            _availableObjects = new Dictionary<string, ICorruptibleObject>();
            _corruptedTickInterval = new WaitForSeconds(CorruptedTickInterval);
            _tickCoroutine = StartCoroutine(CorruptedTickRoutine());
            _decayCoroutine = StartCoroutine(DecayRoutine());
        }
        
        private void Start()
        {
            _sanityController = ServiceLocator.GetService<ISanityController>();
        }
 
        private void OnDestroy()
        {
            if (_tickCoroutine != null)
            {
                StopCoroutine(_tickCoroutine);
            }
            
            if (_decayCoroutine != null)
            {
                StopCoroutine(_tickCoroutine);
            }

            List<ICorruptibleObject> corruptibleObjects = _corruptibleObjects.Values.ToList();
            
            foreach (ICorruptibleObject corruptibleObject in corruptibleObjects)
            {
                corruptibleObject.OnCooldownFinished -= FinishCooldown;
            }
        }
        
        public void RegisterCorruptibleObject(ICorruptibleObject corruptibleObject)
        {
            _corruptibleObjects.TryAdd(corruptibleObject.ObjectId, corruptibleObject);
            _availableObjects.Add(corruptibleObject.ObjectId, corruptibleObject);
            corruptibleObject.OnCooldownFinished += FinishCooldown;
        }
        
        private void CorruptObject(string objectId)
        {
            if (!_availableObjects.Remove(objectId, out var corruptibleObject))
            {
                Debug.LogWarning($"[DecayController] Cannot corrupt '{objectId}': not available.");
                return;
            }

            _corruptedObjects[objectId] = corruptibleObject;
            corruptibleObject.Corrupt();
 
            Debug.Log($"[DecayController] Corrupted: {objectId}");
        }
        
        public void ClearObject(string objectId)
        {
            if (!_corruptedObjects.Remove(objectId, out var corruptibleObject))
            {
                return;
            }

            _coolingDownObjects[objectId] = corruptibleObject;
        }
        
        private void FinishCooldown(string objectId)
        {
            if (!_coolingDownObjects.Remove(objectId, out var corruptibleObject))
            {
                Debug.LogWarning($"[DecayController] Cannot finish cooldown for '{objectId}': not cooling.");
            }

            _availableObjects[objectId] = corruptibleObject;
 
            Debug.Log($"[DecayController] CooldownDone → Available: {objectId}");
        }

        private ICorruptibleObject GetRandomAvailableObject()
        {
            if (_availableObjects.Count == 0)
            {
                Debug.Log("[DecayController] No available objects to pick from.");
                return null;
            }
 
            int randomIndex = Random.Range(0, _availableObjects.Count);
            var randomEntry= _availableObjects.ElementAt(randomIndex);
 
            Debug.Log($"[DecayController] Random pick: {randomEntry.Key}");
            return randomEntry.Value;
        }

        private bool PickAndCorruptRandom()
        {
            ICorruptibleObject corruptibleObject = GetRandomAvailableObject();

            if (corruptibleObject == null)
            {
                return false;
            }

            if (!corruptibleObject.CanBeCorrupted())
            {
                return false;
            }
            
            CorruptObject(corruptibleObject.ObjectId);
            return true;
        }
        
        private IEnumerator CorruptedTickRoutine()
        {
            while (true)
            {
                yield return _corruptedTickInterval;
 
                if (_corruptedObjects.Count > 0)
                {
                    _sanityController.LoseSanity(_corruptedObjects.Count);
                }
                else
                {
                    _sanityController.GainSanity();
                }
            }
        }
        
        private IEnumerator DecayRoutine()
        {
            while (true)
            {
                float wait = GetNextDecayInterval();
                yield return new WaitForSeconds(wait);
                
                while (!PickAndCorruptRandom())
                {
                   yield return new WaitForSeconds(wait);
                }
            }
        }
        
        private float GetNextDecayInterval()
        {
            float t = Mathf.InverseLerp(0, 8, _corruptedObjects.Count);
            return Mathf.Lerp(_naturalDecayMaxInterval, _naturalDecayMinInterval, t);
        }
    }

    public interface IDecayController
    {
        public void RegisterCorruptibleObject(ICorruptibleObject corruptibleObject);
        public void ClearObject(string objectId);
    }
}