using UnityEditor;
using UnityEngine;

namespace Gamegaard.CursorSystem.Editor
{
    [CustomEditor(typeof(VirtualCursorManager))]
    public class VirtualCursorManagerEditor : GenericCursorManagerEditor
    {
        SerializedProperty cursorImageProp;
        SerializedProperty cursorCanvasProp;

        protected override void OnEnable()
        {
            base.OnEnable();
            cursorImageProp = serializedObject.FindProperty("currentVirtualCursor");
            cursorCanvasProp = serializedObject.FindProperty("cursorCanvas");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Virtual Cursor Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(cursorImageProp, new GUIContent("Current Virtual Cursor", "The current virtual cursor."));
            EditorGUILayout.PropertyField(cursorCanvasProp, new GUIContent("Cursor Canvas", "The current Cursor Canvas."));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
