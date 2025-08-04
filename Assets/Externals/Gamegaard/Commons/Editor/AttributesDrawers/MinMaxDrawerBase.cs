using UnityEngine;
using UnityEditor;

namespace Gamegaard.CustomValues.Editor
{
    public abstract class MinMaxDrawerBase<T> : PropertyDrawer
    {
        protected const float LabelWidth = 100f;
        protected const float TextSize = 30f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float labelWidth = EditorGUIUtility.labelWidth;

            float fieldWidth = (position.width - labelWidth) / 2f;
            Rect labelRect = new Rect(position.x, position.y, labelWidth, lineHeight);
            Rect minRect = new Rect(position.x + labelWidth, position.y, fieldWidth, lineHeight);
            Rect maxRect = new Rect(position.x + labelWidth + fieldWidth - ((LabelWidth + TextSize) / 2), position.y, fieldWidth, lineHeight);

            EditorGUI.LabelField(labelRect, label);

            EditorGUI.BeginChangeCheck();
            DrawField(minRect, property, "min", "Min");
            DrawField(maxRect, property, "max", "Max");

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }

        protected abstract void DrawField(Rect rect, SerializedProperty property, string fieldName, string fieldLabel);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
