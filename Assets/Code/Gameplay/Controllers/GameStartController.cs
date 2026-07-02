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
            _outlinesController.DisableAllOutlines();
            mainFadeInOutBlack.FadeOut(3f, Ease.Linear, OpenEyes);
        }

        private void OpenEyes()
        {
            _cameraController.EnableNavigation();
            
            DOVirtual.DelayedCall(1f, () =>
            {
                _interactionController.EnableInteractions();
            });

        }
    }

    public interface IGameStartController
    {
        
    }
}