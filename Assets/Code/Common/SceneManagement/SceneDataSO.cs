using UnityEngine;

namespace CorePatterns.Scenes
{
    [CreateAssetMenu(fileName = "-SceneDataSO", menuName = "ScriptableObjects/Scenes/SceneDataSO")]
    public class SceneDataSO : ScriptableObject
    {
        [Header("Configuration")]
        [SerializeField] private string sceneName;
        [SerializeField] private bool openAdditive;
        [SerializeField] private SceneDataSO[] sceneDataToOpen;
        [SerializeField] private SceneDataSO[] sceneDataToClose;
        
        public  string SceneName => sceneName;
        public bool OpenAdditive => openAdditive;
        public SceneDataSO[] SceneDataToOpen => sceneDataToOpen;
        public SceneDataSO[] SceneDataToClose => sceneDataToClose;
    }
}
