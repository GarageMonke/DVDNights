using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace Rulebound
{
    public class TVWorldController : MonoBehaviour, ITVWorldController
    {
        [Header("References")] 
        [SerializeField] private Transform startPoint;
        [SerializeField] private TVWorldLevelGenerator levelGenerator;

        [Header("Audio-Feedback")] 
        [SerializeField] private AudioClip tvWorldAudioClip;
        
        private IHazardController _hazardController;
        private IPlatformController _platformController;
        private ICharacter _character;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<ITVWorldController>(this);
        }

        private void Start()
        {
            _hazardController = ServiceLocator.GetService<IHazardController>();
            _hazardController.OnHazardTriggered += ResetMinigame;
            _platformController = ServiceLocator.GetService<IPlatformController>();
            _character = ServiceLocator.GetService<ICharacter>();
            levelGenerator.GenerateLevel();
            StartMinigame();
        }

        public void StartMinigame()
        {
            AudioManager.Instance.PlayOST(AudioChannelType.TVWORLD, tvWorldAudioClip, volume: 0.35f, loop: true);
            _hazardController.EnableAllHazards();
            _platformController.EnableAllPlatforms();
            ResetMinigame();
        }

        public void ResetMinigame()
        {
            _character.CharacterTransform.position = startPoint.position;
            _character.ResetCharacter();
            _platformController.ResetAllPlatforms();
        }

        public void EndMinigame()
        {
            _hazardController.DisableAllHazards();
            _platformController.DisableAllPlatforms();
        }

        private void OnDestroy()
        {
            _hazardController.OnHazardTriggered -= ResetMinigame;
        }
    }

    public interface ITVWorldController
    {
        public void StartMinigame();
        public void ResetMinigame();
        public void EndMinigame();
    }
}