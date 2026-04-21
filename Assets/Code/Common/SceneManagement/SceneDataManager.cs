using UnityEngine;
using UnityEngine.SceneManagement;

namespace CorePatterns.Scenes
{
    public class SceneDataManager : MonoBehaviour
    {
        public static SceneDataManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void OpenScene(SceneDataSO sceneDataToOpen)
        {
            CloseScenes(sceneDataToOpen.SceneDataToClose);
            OpenDependantScenes(sceneDataToOpen.SceneDataToOpen);
            OpenSingleScene(sceneDataToOpen);
        }

        private void CloseScenes(SceneDataSO[] sceneDataToClose)
        {
            foreach (SceneDataSO sceneData in sceneDataToClose)
            {
                SceneManager.UnloadSceneAsync(sceneData.SceneName);
            }
        }
        
        private void OpenDependantScenes(SceneDataSO[] sceneDataToOpen)
        {
            foreach (SceneDataSO sceneData in sceneDataToOpen)
            {
                OpenSingleScene(sceneData);
            }
        }

        private void OpenSingleScene(SceneDataSO sceneDataToOpen)
        {
            if (sceneDataToOpen.OpenAdditive)
            {
                SceneManager.LoadSceneAsync(sceneDataToOpen.SceneName, LoadSceneMode.Additive);
                return;
            }

            SceneManager.LoadSceneAsync(sceneDataToOpen.SceneName);
        }
    }
}