using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

[InitializeOnLoad]
public class CustomPlayButton
{
    static CustomPlayButton()
    {
        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
    }

    private static void OnToolbarGUI()
    {
        GUILayout.Space(400);
        if (GUILayout.Button(new GUIContent("Main", "Start the game from the Main Screen"), EditorStyles.toolbarButton,  GUILayout.Width(150)))
        {
            PlayFromStartScene();
        }
        GUILayout.Space(25);
    }

    private static void PlayFromStartScene()
    {
        var startScenePath = "Assets/Game/Scenes/MainMenu-InsideTV.unity";

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(startScenePath);
            EditorApplication.isPlaying = true;
        }
    }
}