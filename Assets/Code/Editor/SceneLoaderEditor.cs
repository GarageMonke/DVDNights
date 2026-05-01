using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneLoaderEditor : EditorWindow
{
    private string folderPath = "Assets/Game/Scenes"; // Set the default folder path here
    private Vector2 scrollPos;

    [MenuItem("Tools/Scene Loader")]
    public static void ShowWindow()
    {
        GetWindow<SceneLoaderEditor>("Scene Loader");
    }

    private void OnGUI()
    {
        GUILayout.Label("Load Scenes from Folder", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Folder Path:", GUILayout.Width(70));
        folderPath = GUILayout.TextField(folderPath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string selectedFolder = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
            if (!string.IsNullOrEmpty(selectedFolder))
            {
                folderPath = "Assets" + selectedFolder.Replace(Application.dataPath, "");
            }
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Refresh Scenes"))
        {
            RefreshScenes();
        }

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        string[] scenePaths = AssetDatabase.FindAssets("t:Scene", new[] { folderPath });

        foreach (var guid in scenePaths)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string sceneName = Path.GetFileNameWithoutExtension(path);

            if (GUILayout.Button(sceneName))
            {
                LoadScene(path);
            }
        }

        GUILayout.EndScrollView();
    }

    private void LoadScene(string scenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }

    private void RefreshScenes()
    {
        AssetDatabase.Refresh();
    }
}