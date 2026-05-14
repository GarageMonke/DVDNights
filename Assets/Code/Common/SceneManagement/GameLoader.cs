using CorePatterns.Scenes;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
   [SerializeField] private SceneDataSO _sceneToOpen;

   private void Start()
   {
      SceneDataManager.Instance.OpenScene(_sceneToOpen);
   }
}
