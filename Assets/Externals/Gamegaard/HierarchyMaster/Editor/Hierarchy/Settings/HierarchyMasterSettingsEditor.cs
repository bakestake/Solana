using UnityEditor;

namespace Gamegaard.HierarchyMaster.Editor
{
    [CustomEditor(typeof(HierarchyMasterSettings))]
    public class HierarchyMasterSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            DrawPropertiesExcluding(serializedObject, "m_Script");

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                HierarchyMasterManager.ApplySettings();
                EditorApplication.RepaintHierarchyWindow();
            }
        }
    }
}
