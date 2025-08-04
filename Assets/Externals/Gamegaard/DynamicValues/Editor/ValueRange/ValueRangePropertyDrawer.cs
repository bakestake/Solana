using UnityEditor;
using UnityEngine;

namespace Gamegaard.DynamicValues.Editor
{
    [CustomPropertyDrawer(typeof(ValueRange<>), true)]
    public class ValueRangePropertyDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 20f;
        private const float HorizontalSpacing = 15f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty currentValue = property.FindPropertyRelative("_currentValue");
            SerializedProperty maxValue = property.FindPropertyRelative("_maxValue");

            position.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height), label);

            float labelWidth = EditorGUIUtility.labelWidth;
            float fieldWidth = (position.width - labelWidth - ButtonWidth - HorizontalSpacing) / 2f;

            Rect currentRect = new Rect(position.x + labelWidth, position.y, fieldWidth, EditorGUIUtility.singleLineHeight);
            Rect finalPosition = new Rect(position.x + labelWidth + fieldWidth + HorizontalSpacing, position.y, fieldWidth - 5, EditorGUIUtility.singleLineHeight);

            DrawCustomValueField(currentRect, new GUIContent("Current:", "The current value."), currentValue);
            DrawCustomValueField(finalPosition, new GUIContent("Max:", "The max allowed value."), maxValue);

            float buttonXOffset = position.x + position.width - ButtonWidth - 3;
            Rect buttonRect = new Rect(buttonXOffset, position.y, ButtonWidth, EditorGUIUtility.singleLineHeight);
            DrawExpandCollapseButton(buttonRect, property);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                SerializedProperty minValue = property.FindPropertyRelative("_minValue");

                if (minValue != null)
                {
                    DrawMinValueField(minValue);
                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                SerializedProperty valueCurve = property.FindPropertyRelative("valueCurve");
                if (valueCurve != null)
                {
                    EditorGUILayout.PropertyField(valueCurve, new GUIContent("Value Curve:", "Curve defining the value adjustment."));
                }

                SerializedProperty changeTime = property.FindPropertyRelative("valueShiftTimer.timerDuration");
                SerializedProperty valueShift = property.FindPropertyRelative("valueShift");
                if (changeTime != null)
                {
                    EditorGUILayout.PropertyField(changeTime, new GUIContent("Change Time:", "Interval for applying value shifts."));
                }

                if (valueShift != null)
                {
                    EditorGUILayout.PropertyField(valueShift, new GUIContent("Value Shift:", "The magnitude of value changes over time."));
                }
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        protected void DrawCustomValueField(Rect position, GUIContent content, SerializedProperty property)
        {
            float spacing = 5f;
            float labelWidth = 50f;

            Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            Rect fieldRect = new Rect(position.x + labelWidth + spacing, position.y, position.width - labelWidth - spacing, position.height);

            if (property.propertyType == SerializedPropertyType.Float)
            {
                EditorGUI.LabelField(labelRect, content);
                property.floatValue = EditorGUI.DelayedFloatField(fieldRect, property.floatValue);
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                EditorGUI.LabelField(labelRect, content);
                property.intValue = EditorGUI.DelayedIntField(fieldRect, property.intValue);
            }
        }

        private void DrawMinValueField(SerializedProperty minValue)
        {
            EditorGUILayout.PropertyField(minValue, new GUIContent("Min Value:", "Defines the minimum allowable value."));
        }

        private void DrawExpandCollapseButton(Rect buttonRect, SerializedProperty property)
        {

            string symbol = property.isExpanded ? "-" : "+";
            if (GUI.Button(buttonRect, symbol))
            {
                property.isExpanded = !property.isExpanded;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}