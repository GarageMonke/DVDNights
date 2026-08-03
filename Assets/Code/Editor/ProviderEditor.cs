namespace Rulebound.Editor
{
    using UnityEditor;
    using UnityEngine;
    using System.Collections;
    using System.Reflection;

    [CustomEditor(typeof(ScriptableObject), true)]
    public class ProviderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var type = target.GetType();
            
            while (type != null)
            {
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(CorePatterns.Providers.Provider<>))
                {
                    DrawButton();
                    break;
                }

                type = type.BaseType;
            }
        }

        private void DrawButton()
        {
            GUILayout.Space(10);

            if (GUILayout.Button("Reassign IDs"))
            {
                Undo.RecordObject(target, "Reassign Provider IDs");

                var field = target.GetType().BaseType?.GetField(
                    "assignedElements",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (field != null)
                {
                    var list = (IList)field.GetValue(target);

                    for (int i = 0; i < list.Count; i++)
                    {
                        object element = list[i];

                        var idProperty = element.GetType().GetField("Id");
                        idProperty?.SetValue(element, i.ToString());
                    }
                }

                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }
    }
}