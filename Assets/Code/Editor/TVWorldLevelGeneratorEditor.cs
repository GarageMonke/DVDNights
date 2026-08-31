#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Rulebound
{
    [CustomEditor(typeof(TVWorldLevelGenerator))]
    public class ProceduralLevelGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);

            TVWorldLevelGenerator generator = (TVWorldLevelGenerator)target;

            if (GUILayout.Button("GENERATE LEVEL", GUILayout.Height(35)))
            {
                generator.GenerateLevel();
                EditorUtility.SetDirty(generator);
            }

            if (GUILayout.Button("CLEAR LEVEL"))
            {
                generator.ClearLevel();
                EditorUtility.SetDirty(generator);
            }
        }
    }
}

#endif