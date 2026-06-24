using CorePatterns.Scenes;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
   public class MainMenuController : MonoBehaviour, IMainMenuController
   {
      [Header("References")] 
      [SerializeField] private GameObject loadingContent;
      [SerializeField]private GameObject mainMenuContent;
     
      [Header("Menu")]
      [SerializeField] private SelectableTextView[] selectableTextViews;

      [Header("Scenes")] 
      [SerializeField] private SceneDataSO gameSceneDataSO;
      
      private int _currentIndex;

      private ITVNavigationController _tvNavigationController;
      private ITVStateController _tvStateController;

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
         _tvStateController = ServiceLocator.GetService<ITVStateController>();

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
         if (!_tvStateController.IsTVOn)
         {
            return;
         }

         if (!_tvStateController.HasDisk)
         {
            return;
         }
         
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

      public void DisplayMenu()
      {
         loadingContent.gameObject.SetActive(false);
         SceneDataManager.Instance.OpenScene(gameSceneDataSO);
         // mainMenuContent.gameObject.SetActive(true);
         // SelectFirst();
      }
   }
}

public interface IMainMenuController
{
   public void DisplayMenu();
}
