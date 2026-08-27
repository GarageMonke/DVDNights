using System;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace Rulebound
{
    public class FloorIsLavaController : MonoBehaviour, IFloorIsLavaController
    {
        [Header("References")] 
        [SerializeField] private Character character;
        [SerializeField] private Transform startPoint;
        
        private IHazardController _hazardController;
        private IPlatformController _platformController;

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
            character.CharacterTransform.position = startPoint.position;
            character.ResetCharacter();
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