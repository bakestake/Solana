using UnityEditor;
using UnityEngine;

namespace Gamegaard.HierarchyMaster.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HierarchyStyle))]
    public class HierarchyStyleEditor : UnityEditor.Editor
    {
        private SerializedProperty backgroundStyle;
        private SerializedProperty backgroundColor;
        private SerializedProperty backgroundGradient;
        private SerializedProperty gradientAngle;

        private SerializedProperty fontColorStyle;
        private SerializedProperty textColor;
        private SerializedProperty font;
        private SerializedProperty fontSize;
        private SerializedProperty fontStyle;
        private SerializedProperty alignment;
        private SerializedProperty textDropShadow;
        private SerializedProperty enableTextOutline;
        private SerializedProperty textOutlineColor;

        private SerializedProperty containerLines;
        private SerializedProperty containerLinesColor;
        private SerializedProperty useCustomIcon;
        private SerializedProperty customIcon;
        private SerializedProperty enableRichText;

        private bool isEditing;
        private bool showTextSettings = true;
        private bool showBackgroundSettings = true;
        private bool showDecorationSettings = true;

        private void OnEnable()
        {
            backgroundStyle = serializedObject.FindProperty("backgroundStyle");
            backgroundColor = serializedObject.FindProperty("backgroundColor");
            backgroundGradient = serializedObject.FindProperty("backgroundGradient");
            gradientAngle = serializedObject.FindProperty("gradientAngle");

            fontColorStyle = serializedObject.FindProperty("fontColorStyle");
            textColor = serializedObject.FindProperty("textColor");
            font = serializedObject.FindProperty("font");
            fontSize = serializedObject.FindProperty("fontSize");
            fontStyle = serializedObject.FindProperty("fontStyle");
            alignment = serializedObject.FindProperty("alignment");
            textDropShadow = serializedObject.FindProperty("textDropShadow");
            enableTextOutline = serializedObject.FindProperty("enableTextOutline");
            textOutlineColor = serializedObject.FindProperty("textOutlineColor");

            containerLines = serializedObject.FindProperty("containerLines");
            useCustomIcon = serializedObject.FindProperty("useCustomIcon");
            customIcon = serializedObject.FindProperty("customIcon");
            enableRichText = serializedObject.FindProperty("enableRichText");
            containerLinesColor = serializedObject.FindProperty("containerLineColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (GUILayout.Button(isEditing ? "Save" : "Edit"))
            {
                isEditing = !isEditing;
            }

            if (isEditing)
            {
                DrawEditMode();
            }
            else
            {
                DrawViewMode();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawEditMode()
        {
            showBackgroundSettings = EditorGUILayout.Foldout(showBackgroundSettings, "Background Settings", true);
            if (showBackgroundSettings)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(backgroundStyle);
                if (backgroundStyle.enumValueIndex == (int)BackgroundStyle.SolidColor)
                {
                    EditorGUILayout.PropertyField(backgroundColor);
                }
                else if (backgroundStyle.enumValueIndex == (int)BackgroundStyle.Gradient)
                {
                    EditorGUILayout.PropertyField(backgroundGradient);
                    EditorGUILayout.PropertyField(gradientAngle);
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            showTextSettings = EditorGUILayout.Foldout(showTextSettings, "Text Settings", true);
            if (showTextSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(fontColorStyle);
                if (fontColorStyle.enumValueIndex == (int)FontColorStyle.Custom)
                {
                    EditorGUILayout.PropertyField(textColor);
                }
                EditorGUILayout.PropertyField(font);
                EditorGUILayout.PropertyField(fontSize);
                EditorGUILayout.PropertyField(fontStyle);
                EditorGUILayout.PropertyField(alignment);
                EditorGUILayout.PropertyField(textDropShadow);
                EditorGUILayout.PropertyField(enableTextOutline);
                if (enableTextOutline.boolValue)
                {
                    EditorGUILayout.PropertyField(textOutlineColor);
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            showDecorationSettings = EditorGUILayout.Foldout(showDecorationSettings, "Decoration Settings", true);
            if (showDecorationSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(containerLines);
                if (containerLines.enumValueIndex != (int)ContainerLines.None)
                {
                    EditorGUILayout.PropertyField(containerLinesColor);
                }
                EditorGUILayout.PropertyField(useCustomIcon);
                if (useCustomIcon.boolValue)
                {
                    EditorGUILayout.PropertyField(customIcon);
                }
                EditorGUILayout.PropertyField(enableRichText);
                EditorGUI.indentLevel--;
            }
        }

        private void DrawViewMode()
        {

        }
    }
}