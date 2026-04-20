using UnityEngine;

namespace DVDNights
{
   public class MainMenuController : MonoBehaviour
   {
      [Header("Menu")]
      [SerializeField] private SelectableTextView[] selectableTextViews;

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
   }
}
