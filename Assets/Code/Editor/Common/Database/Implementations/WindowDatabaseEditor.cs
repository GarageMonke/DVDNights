#if UNITY_EDITOR

using System.Collections.Generic;
using Code.Common.Database;
using Common;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WindowDatabase))]
public class WindowDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Refresh Database"))
        {
            RefreshDatabase();
        }
    }

    private void RefreshDatabase()
    {
        WindowDatabase database = (WindowDatabase)target;

        List<Window> windows = new();

        string[] guids = AssetDatabase.FindAssets("t:Prefab");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (!prefab) continue;

            Window window = prefab.GetComponent<Window>();

            if (!window) continue;

            windows.Add(window);
        }

        database.SetWindows(windows);

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();

        Debug.Log($"Registered {windows.Count} windows.");
    }
}

#endif