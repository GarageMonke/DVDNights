using UnityEngine;
using UnityEditor;

public class BakeMeshWindow : EditorWindow
{
    [MenuItem("CONTEXT/SkinnedMeshRenderer/Bake Static Mesh Snapshot")]
    public static void BakeSkinnedMesh(MenuCommand menuCommand)
    {
        SkinnedMeshRenderer smr = menuCommand.context as SkinnedMeshRenderer;
        if (smr == null) return;

        Mesh bakedMesh = new Mesh();
        smr.BakeMesh(bakedMesh);

        string path = EditorUtility.SaveFilePanelInProject("Save Baked Mesh", smr.name + "_Baked", "asset", "Save your static mesh asset");
        if (string.IsNullOrEmpty(path)) return;

        AssetDatabase.CreateAsset(bakedMesh, path);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"Successfully baked and saved static mesh to: {path}");
    }
}