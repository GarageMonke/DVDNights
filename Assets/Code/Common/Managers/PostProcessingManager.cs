using UnityEngine;
using UnityEngine.Rendering;

namespace CorePatterns.Managers
{
    public class PostProcessingManager : MonoBehaviour
    {
        public static PostProcessingManager Instance { get; private set; }

        [Header("References")] 
        [SerializeField] private Volume postProcessing;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void EnableVolume()
        {
            postProcessing.enabled = true;
        }

        public void DisableVolume()
        {
            postProcessing.enabled = false;
        }
        
        public T GetVolumeComponent<T>() where T : VolumeComponent
        {
            if (postProcessing.profile.TryGet(out T component))
                return component;

            Debug.LogError($"{typeof(T).Name} not found in Volume Profile.");
            return null;
        }
    }
}