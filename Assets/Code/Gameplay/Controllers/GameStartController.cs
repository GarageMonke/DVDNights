using System;
using System.Collections;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class GameStartController : MonoBehaviour, IGameStartController
    {
        [Header("References")] 
        [SerializeField] private FadeInOutBlack mainFadeInOutBlack;

        private IOutlineController _outlinesController;
        private ICameraController _cameraController;
        private IInteractionController _interactionController;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
           ServiceLocator.RegisterService<IGameStartController>(this);
        }

        private void Start()
        {
            _outlinesController = ServiceLocator.GetService<IOutlineController>();
            _cameraController = ServiceLocator.GetService<ICameraController>();
            _interactionController = ServiceLocator.GetService<IInteractionController>();
            
            PrepareRoom();
        }

        private void PrepareRoom()
        {
            _outlinesController.EnableAllOutlines();
            mainFadeInOutBlack.FadeOut(3f, Ease.Linear, OpenEyes);
        }

        private void OpenEyes()
        {
            _interactionController.EnableInteractions();
            _outlinesController.DisableAllOutlines();
            _cameraController.EnableNavigation();
        }
    }

    public interface IGameStartController
    {
        
    }
}