using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseEditorOnInput : MonoBehaviour
{
    [SerializeField] private KeyCode toggleKey = KeyCode.P;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
#if UNITY_EDITOR
            EditorApplication.isPaused = !EditorApplication.isPaused;
#endif
        }
    }
}