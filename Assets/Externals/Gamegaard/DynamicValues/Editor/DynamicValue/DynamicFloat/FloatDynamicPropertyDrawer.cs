using UnityEditor;
using UnityEngine;

namespace Gamegaard.DynamicValues.Editor
{
    [CustomPropertyDrawer(typeof(DynamicFloat))]
    public class FloatDynamicPropertyDrawer : DynamicPropertyDrawer<float>
    {
        protected override bool DrawMinMaxCustomValueField(Rect position, GUIContent content, SerializedProperty property, float min, float max)
        {
            float labelWidth = 50f;
            float spacing = 5f;

            Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            Rect fieldRect = new Rect(position.x + labelWidth + spacing, position.y, position.width - labelWidth - spacing, position.height);

            EditorGUI.LabelField(labelRect, content);

            float temp = property.floatValue;
            property.floatValue = Mathf.Clamp(EditorGUI.DelayedFloatField(fieldRect, property.floatValue), min, max);
            return property.floatValue != temp;
        }
    }
}
