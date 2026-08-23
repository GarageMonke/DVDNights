using System;
using System.Collections;
using Code.Gameplay.Misc;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rulebound
{
    public class ThunderController : MonoBehaviour, IThunderController
    {
        [Header("References")]
        [SerializeField] private ThunderLightning thunderLightning;
        [SerializeField] private EntityInteractableObject entityInteractableObject;
        
        [Header("Feedback")]
        [SerializeField] private AudioClip rainAudioClip;
        
        private Coroutine _thunderCooldownRoutine;
        private ISanityController _sanityController;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IThunderController>(this);
        }

        private void Start()
        {
            _sanityController = ServiceLocator.GetService<ISanityController>();
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

            if (entityInteractableObject.IsCorrupted())
            {
                entityInteractableObject.ClearCorruption();
            }
            else
            {
                _sanityController.TakeSanityImmediate(PenaltyType.HIGH);
            }
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

    public interface IThunderController
    {
        public void PlayRain();
        public void StopRain();
        public void PlayThunder();
    }
}