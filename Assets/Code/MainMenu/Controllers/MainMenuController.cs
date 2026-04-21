using System;
using CorePatterns.Scenes;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
   public class MainMenuController : MonoBehaviour, IMainMenuController
   {
      [Header("Menu")]
      [SerializeField] private SelectableTextView[] selectableTextViews;

      [Header("Scenes")] 
      [SerializeField] private SceneDataSO gameSceneDataSO;
      
      private int _currentIndex;

      private ITVNavigationController _tvNavigationController;

      private void Awake()
      {
         InstallService();
      }

      private void InstallService()
      {
         ServiceLocator.RegisterService<IMainMenuController>(this);
      }

      private void Start()
      {
         SelectFirst();
         _tvNavigationController = ServiceLocator.GetService<ITVNavigationController>();

         _tvNavigationController.OnPreviousButtonPressed += PreviousSelection;
         _tvNavigationController.OnNextButtonPressed += NextSelection;
         _tvNavigationController.OnSubmitButtonPressed += Submit;
      }

      private void Update()
      {
         if (Input.GetKeyDown(KeyCode.RightArrow))
         {
            NextSelection();
         }
         
         if (Input.GetKeyDown(KeyCode.LeftArrow))
         {
            PreviousSelection();
         }

         if (Input.GetKeyDown(KeyCode.Space))
         {
            Submit();
         }
      }
      
      private void SelectFirst()
      {
         _currentIndex = 0;

         foreach (ISelectableTextView selectableTextView in selectableTextViews)
         {
            selectableTextView.Unselect();
         }
         
         selectableTextViews[_currentIndex].Select();
      }

      private void NextSelection()
      {
         if (_currentIndex + 1 >= selectableTextViews.Length)
         {
            return;
         }
         
         selectableTextViews[_currentIndex].Unselect();
         
         _currentIndex++;
         
         selectableTextViews[_currentIndex].Select();
      }

      private void PreviousSelection()
      {
         if (_currentIndex - 1 < 0)
         {
            return;
         }
         
         selectableTextViews[_currentIndex].Unselect();
         
         _currentIndex--;
         
         selectableTextViews[_currentIndex].Select();
      }

      private void Submit()
      {
         switch (_currentIndex)
         {
            case 0:
               PlayGame();
               break;
            case 1:
               break;
            case 2:
               break;
            case 3:
               break;
         }
      }

      private void PlayGame()
      {
         SceneDataManager.Instance.OpenScene(gameSceneDataSO);
      }

      private void OnDestroy()
      {
         _tvNavigationController.OnPreviousButtonPressed -= PreviousSelection;
         _tvNavigationController.OnNextButtonPressed -= NextSelection;
         _tvNavigationController.OnSubmitButtonPressed -= Submit;
      }
   }
}

public interface IMainMenuController
{
}
