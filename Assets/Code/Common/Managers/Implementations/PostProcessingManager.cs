using UnityEngine;
using UnityEngine.Rendering;

namespace CorePatterns.Managers
{
    public class PostProcessingManager : Manager<PostProcessingManager>
    {
        public static PostProcessingManager Instance { get; private set; }

        [Header("References")] 
        [SerializeField] private Volume postProcessing;

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