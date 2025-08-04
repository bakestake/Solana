using UnityEditor;
using UnityEngine;

namespace Gamegaard.DynamicValues.Editor
{
    [CustomPropertyDrawer(typeof(DynamicInt))]
    public class IntDynamicPropertyDrawer : DynamicPropertyDrawer<int>
    {
        protected override bool DrawMinMaxCustomValueField(Rect position, GUIContent content, SerializedProperty property, float min, float max)
        {
            float labelWidth = 50f;
            float spacing = 5f;

            Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            Rect fieldRect = new Rect(position.x + labelWidth + spacing, position.y, position.width - labelWidth - spacing, position.height);

            EditorGUI.LabelField(labelRect, content);

            int finalMin = (min == float.MinValue) ? int.MinValue : (int)min;
            int finalMax = (max == float.MaxValue) ? int.MaxValue : (int)max;

            int temp = property.intValue;
            property.intValue = Mathf.Clamp(EditorGUI.DelayedIntField(fieldRect, property.intValue), finalMin, finalMax);
            return property.intValue != temp;
        }
    }
}