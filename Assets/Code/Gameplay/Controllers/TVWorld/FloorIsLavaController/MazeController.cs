using CorePatterns.ServiceLocator;
using UnityEngine;

namespace Rulebound
{
    public class MazeController : MonoBehaviour, IFloorIsLavaController
    {
        [Header("References")] 
        [SerializeField] private Transform startPoint;
        
        private IHazardController _hazardController;
        private IPlatformController _platformController;
        private ICharacter _character;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IFloorIsLavaController>(this);
        }

        private void Start()
        {
            _hazardController = ServiceLocator.GetService<IHazardController>();
            _hazardController.OnHazardTriggered += ResetMinigame;
            _platformController = ServiceLocator.GetService<IPlatformController>();
            _character = ServiceLocator.GetService<ICharacter>();
            StartMinigame();
        }

        public void StartMinigame()
        {
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

    public interface IFloorIsLavaController
    {
        public void StartMinigame();
        public void ResetMinigame();
        public void EndMinigame();
    }
}