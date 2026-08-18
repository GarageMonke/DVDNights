using System;
using System.Collections;
using CorePatterns.Scenes;
using CorePatterns.ServiceLocator;
using TMPro;
using UnityEngine;

namespace DVDNights
{
   public class MainMenuController : MonoBehaviour, IMainMenuController
   {
      [Header("References")] 
      [SerializeField] private GameObject loadingContent;
      [SerializeField] private TextMeshProUGUI loadingText;
      
      [Header("Scenes")] 
      [SerializeField] private SceneDataSO gameSceneDataSO;
      
      private int _currentIndex;
      private IEnumerator _loadingAnimation;
      
      private void Awake()
      {
         InstallService();
      }

      private void InstallService()
      {
         ServiceLocator.RegisterService<IMainMenuController>(this);
      }

      private IEnumerator AnimateLoadingDots()
      {
         int dotCount = 0;
         
         //Load from data
         string discNumber = $"Loading Disc-{1}";
         
         while (true)
         {
            dotCount = (dotCount + 1) % 4;
            loadingText.text =  discNumber + new string('.', dotCount);
            yield return new WaitForSeconds(1f);
         }
      }

      public void LoadGame()
      {
         loadingContent.gameObject.SetActive(true);
         StartCoroutine(AnimateLoadingDots());
      }
      
      private void StopLoadingAnimation()
      {
         if (_loadingAnimation == null)
         {
            return;
         }
         
         StopCoroutine(_loadingAnimation);
         _loadingAnimation = null;
      }

      public void GoToGame()
      {
         StopLoadingAnimation();
         loadingContent.gameObject.SetActive(false);
         SceneDataManager.Instance.OpenScene(gameSceneDataSO);
      }
   }
}

public interface IMainMenuController
{
   public void LoadGame();
   public void GoToGame();
}
