using System.Collections;
using Code.Gameplay.Misc;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class RainController : MonoBehaviour, IRainController
    {
        [Header("References")]
        [SerializeField] private ThunderLightning thunderLightning;
        
        [Header("Feedback")]
        [SerializeField] private AudioClip rainAudioClip;
        
        Coroutine _thunderCooldownRoutine;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IRainController>(this);
        }
        
        public void PlayRain()
        {
            AudioManager.Instance.PlayOST(AudioChannelType.STORM, rainAudioClip, volume: 1f, loop: true);
            _thunderCooldownRoutine ??= StartCoroutine(StartThunderCooldown());
        }

        public void StopRain()
        {
            AudioManager.Instance.StopOST(AudioChannelType.STORM);
            StopCoroutine(_thunderCooldownRoutine);
            _thunderCooldownRoutine = null;
        }

        public void PlayThunder()
        {
            thunderLightning.Strike();
        }

        private IEnumerator StartThunderCooldown()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(5, 15));
                PlayThunder();
            }
        }
    }

    public interface IRainController
    {
        public void PlayRain();
        public void StopRain();
        public void PlayThunder();
    }
}