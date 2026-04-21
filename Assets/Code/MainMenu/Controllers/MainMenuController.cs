using CorePatterns.Scenes;
using UnityEngine;

namespace DVDNights
{
   public class MainMenuController : MonoBehaviour
   {
      [Header("Menu")]
      [SerializeField] private SelectableTextView[] selectableTextViews;

      [Header("Scenes")] 
      [SerializeField] private SceneDataSO gameSceneDataSO;
      
      private int _currentIndex;

      private void Start()
      {
         SelectFirst();
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
         selectableTextViews[_currentIndex].Unselect();
         
         _currentIndex++;
         
         if (_currentIndex >= selectableTextViews.Length)
         {
            _currentIndex = 0;
         }
         
         selectableTextViews[_currentIndex].Select();
      }

      private void PreviousSelection()
      {
         selectableTextViews[_currentIndex].Unselect();
         
         _currentIndex--;
         
         if (_currentIndex < 0)
         {
            _currentIndex = selectableTextViews.Length - 1;
         }
         
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
   }
}
