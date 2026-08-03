using CorePatterns.Scenes;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
   [SerializeField] private SceneDataSO sceneToOpen;
   [SerializeField] private bool openSceneOnStart;

   private void Start()
   {
      if (openSceneOnStart)
      {
         LoadScene();
      }
   }

   public void LoadScene()
   {
      SceneDataManager.Instance.OpenScene(sceneToOpen);
   }
}
