using UnityEditor;
using UnityEngine;

namespace Gamegaard.CursorSystem.Editor
{
    [CustomEditor(typeof(GenericCursorManagerBase<,>), true)]
    public class GenericCursorManagerEditor : UnityEditor.Editor
    {
        private SerializedProperty defaultCursorProp;
        private SerializedProperty inputHandlerProp;

        protected virtual void OnEnable()
        {
            defaultCursorProp = serializedObject.FindProperty("defaultCursorData");
            inputHandlerProp = serializedObject.FindProperty("inputHandler");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(defaultCursorProp, new GUIContent("Default Cursor"));

            if (inputHandlerProp != null)
            {
                EditorGUILayout.PropertyField(inputHandlerProp, new GUIContent("Input Handler"), true);
            }
            else
            {
                EditorGUILayout.HelpBox("Input Handler not configured properly.", MessageType.Error);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
