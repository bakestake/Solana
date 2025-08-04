using UnityEditor;
using UnityEngine;

namespace Gamegaard.HierarchyMaster.Editor
{
    [CustomEditor(typeof(DevNote))]
    public class DevNoteEditor : UnityEditor.Editor
    {
        private SerializedProperty useTitleProperty;
        private SerializedProperty useBoxColorProperty;
        private SerializedProperty useFontColorProperty;
        private SerializedProperty titleProperty;
        private SerializedProperty messageProperty;
        private SerializedProperty fontSizeProperty;
        private SerializedProperty boxColorProperty;
        private SerializedProperty fontColorProperty;
        private DevNote devNote;
        private bool isEditing;

        private void OnEnable()
        {
            useTitleProperty = serializedObject.FindProperty("useTitle");
            useBoxColorProperty = serializedObject.FindProperty("useBoxColor");
            useFontColorProperty = serializedObject.FindProperty("useFontColor");
            titleProperty = serializedObject.FindProperty("title");
            messageProperty = serializedObject.FindProperty("message");
            fontSizeProperty = serializedObject.FindProperty("fontSize");
            boxColorProperty = serializedObject.FindProperty("boxColor");
            fontColorProperty = serializedObject.FindProperty("fontColor");
            devNote = (DevNote)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            string buttonName = isEditing ? "Save" : "Edit";
            if (GUILayout.Button(buttonName))
            {
                isEditing = !isEditing;
            }

            if (!isEditing)
            {
                DrawMessageContent();
            }
            else
            {
                DrawNoteEditor();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawMessageContent()
        {
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                richText = true,
                fontSize = (int)devNote.FontSize,
                padding = new RectOffset(10, 10, 5, 5),
            };

            if (devNote.UseFontColor)
            {
                labelStyle.normal.textColor = devNote.FontColor;
            }

            if (devNote.UseBoxColor)
            {
                Texture2D backgroundTexture = new Texture2D(1, 1);
                backgroundTexture.SetPixel(0, 0, devNote.BoxColor);
                backgroundTexture.Apply();

                GUIStyle boxStyle = new GUIStyle(EditorStyles.helpBox);
                boxStyle.normal.background = backgroundTexture;

                GUILayout.BeginVertical(boxStyle);
            }
            else
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
            }

            if (devNote.UseTitle)
            {
                GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = (int)devNote.FontSize + 2,
                    alignment = TextAnchor.MiddleCenter,
                    richText = true,
                };

                if (devNote.UseFontColor)
                {
                    titleStyle.normal.textColor = devNote.FontColor;
                }

                GUILayout.Label($"<b>{devNote.Title}</b>", titleStyle);
            }

            if (!string.IsNullOrEmpty(devNote.Message))
            {
                EditorGUILayout.LabelField(devNote.Message, labelStyle);
            }
            GUILayout.EndVertical();
        }

        private void DrawNoteEditor()
        {
            EditorGUILayout.PropertyField(useTitleProperty);
            if (useTitleProperty.boolValue)
            {
                EditorGUILayout.PropertyField(titleProperty);
            }

            EditorGUILayout.PropertyField(useBoxColorProperty);
            if (useBoxColorProperty.boolValue)
            {
                EditorGUILayout.PropertyField(boxColorProperty);
            }

            EditorGUILayout.PropertyField(useFontColorProperty);
            if (useFontColorProperty.boolValue)
            {
                EditorGUILayout.PropertyField(fontColorProperty);
            }

            EditorGUILayout.PropertyField(fontSizeProperty);
            EditorGUILayout.PropertyField(messageProperty);
        }
    }
}
