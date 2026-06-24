using System;
using System.Collections;
using System.Collections.Generic;
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
        
        public void OpenScene(SceneDataSO sceneDataToOpen, Action onComplete = null)
        {
            StartCoroutine(OpenSceneRoutine(sceneDataToOpen, onComplete));
        }

        private IEnumerator OpenSceneRoutine(SceneDataSO sceneDataToOpen, Action onComplete)
        {
            //Unload scenes first
            yield return CloseScenes(sceneDataToOpen.SceneDataToClose);

            //Collect all load operations (main + dependants)
            var operations = new List<AsyncOperation>();

            AsyncOperation mainOp = LoadScene(sceneDataToOpen);
            if (mainOp != null) operations.Add(mainOp);

            foreach (SceneDataSO dep in sceneDataToOpen.SceneDataToOpen)
            {
                AsyncOperation depOp = LoadScene(dep);
                if (depOp != null) operations.Add(depOp);
            }

            //Wait for all of them to finish
            foreach (AsyncOperation op in operations)
            {
                yield return op;
            }
            
            yield return null;
            onComplete?.Invoke();
        }

        private IEnumerator CloseScenes(SceneDataSO[] sceneDataToClose)
        {
            foreach (SceneDataSO sceneData in sceneDataToClose)
            {
                if (!IsSceneLoaded(sceneData.SceneName)) continue;

                yield return SceneManager.UnloadSceneAsync(sceneData.SceneName);
            }
        }

        private AsyncOperation LoadScene(SceneDataSO sceneData)
        {
            if (IsSceneLoaded(sceneData.SceneName))
            {
                Debug.Log($"[SceneDataManager] Scene '{sceneData.SceneName}' is already loaded. Skipping.");
                return null;
            }

            LoadSceneMode mode = sceneData.OpenAdditive
                ? LoadSceneMode.Additive
                : LoadSceneMode.Single;

            return SceneManager.LoadSceneAsync(sceneData.SceneName, mode);
        }

        private bool IsSceneLoaded(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            return scene.IsValid() && scene.isLoaded;
        }
    }
}