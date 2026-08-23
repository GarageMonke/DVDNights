using System;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace Rulebound
{
    public class GameUIController : MonoBehaviour, IGameUIController
    {
        [Header("References")] 
        [SerializeField] private GameObject gameUIContent;

        private void Awake()
        {
            ServiceLocator.RegisterService<IGameUIController>(this);
        }

        public void DisplayGameUI()
        {
            gameUIContent.SetActive(true);
        }

        public void HideGameUI()
        {
            gameUIContent.SetActive(false);
        }
    }

    public interface IGameUIController
    {
        public void DisplayGameUI();
        public void HideGameUI();
    }
}