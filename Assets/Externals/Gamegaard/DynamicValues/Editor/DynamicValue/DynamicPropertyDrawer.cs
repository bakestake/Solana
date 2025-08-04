using Gamegaard.CustomValues.Editor;
using System;
using UnityEditor;
using UnityEngine;

namespace Gamegaard.DynamicValues.Editor
{
    public abstract class DynamicPropertyDrawer<TValue> : PropertyDrawer
    {
        private const float ButtonWidth = 20f;
        private const float HorizontalSpacing = 15f;

        private bool isDirty = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            bool isPartOfArray = property.propertyPath.Contains("Array");

            if (isPartOfArray)
            {
                property.isExpanded = EditorGUI.Foldout(
                    new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                    property.isExpanded,
                    label
                );

                if (property.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    RenderPropertyContent(position, property, label, false);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                RenderPropertyContent(position, property, label, true);
            }

            EditorGUI.EndProperty();
        }

        private void RenderPropertyContent(Rect position, SerializedProperty property, GUIContent label, bool showLabel)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            object container = fieldInfo.GetValue(property.serializedObject.targetObject);

            DynamicValue<TValue> value = property.GetValueFromSerializedProperty<DynamicValue<TValue>>(fieldInfo);
            SerializedProperty baseValueProperty = property.FindPropertyRelative("_baseValue");

            if (showLabel)
            {
                DrawLabel(position, label, labelWidth);
            }

            if (IsDynamicRangeType())
            {
                float fieldWidth = (position.width - labelWidth - ButtonWidth - HorizontalSpacing) / 2f;
                DrawBaseValueField(position, labelWidth, fieldWidth, property, baseValueProperty);
                DrawRangeFields(position, labelWidth, fieldWidth, property);
            }
            else
            {
                DrawFullWidthBaseField(position, labelWidth, baseValueProperty);
            }

            DrawExpandCollapseButton(position, property, value);

            if (value != null && value.EditorIsExpanded)
            {
                EditorGUI.indentLevel++;
                DrawFinalValueField(property);
                DrawAdditionalFields(property, value);
                EditorGUI.indentLevel--;
            }

            if (isDirty)
            {
                ScheduleRecalculation(value);
                isDirty = false;
            }
        }

        private void ScheduleRecalculation(DynamicValue<TValue> value)
        {
            void Recalculate()
            {
                EditorApplication.update -= Recalculate;
                value.Recalculate();
            }

            EditorApplication.update += Recalculate;
        }

        protected abstract bool DrawMinMaxCustomValueField(Rect position, GUIContent content, SerializedProperty property, float min, float max);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (property.isExpanded)
            {
                SerializedProperty iterator = property.Copy();
                SerializedProperty endProperty = iterator.GetEndProperty();

                while (iterator.NextVisible(true) && !SerializedProperty.EqualContents(iterator, endProperty))
                {
                    height += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
                }
            }

            return height;
        }

        private void DrawLabel(Rect position, GUIContent label, float labelWidth)
        {
            Rect labelRect = new Rect(position.x, position.y, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);
        }

        private void DrawBaseValueField(Rect position, float labelWidth, float fieldWidth, SerializedProperty property, SerializedProperty baseValueProperty)
        {
            Rect fieldPosition = new Rect(position.x + labelWidth + fieldWidth + HorizontalSpacing, position.y, fieldWidth - 5, EditorGUIUtility.singleLineHeight);
            SerializedProperty minValueProperty = property.FindPropertyRelative("_range._minValue");

            float minValue = minValueProperty != null ? GetPropertyNumberValue(minValueProperty) : float.MinValue;
            isDirty = DrawMinMaxCustomValueField(fieldPosition, new GUIContent("Base:"), baseValueProperty, minValue, float.MaxValue) || isDirty;
        }

        private void DrawFullWidthBaseField(Rect position, float labelWidth, SerializedProperty baseValueProperty)
        {
            Rect fieldPosition = new Rect(position.x + labelWidth + 2, position.y, position.width - labelWidth - ButtonWidth - HorizontalSpacing + 8, EditorGUIUtility.singleLineHeight);

            if (baseValueProperty.propertyType == SerializedPropertyType.Float)
            {
                float temp = baseValueProperty.floatValue;
                baseValueProperty.floatValue = EditorGUI.DelayedFloatField(fieldPosition, baseValueProperty.floatValue);
                if (temp != baseValueProperty.floatValue)
                {
                    isDirty = true;
                }
            }
            else if (baseValueProperty.propertyType == SerializedPropertyType.Integer)
            {
                int temp = baseValueProperty.intValue;
                baseValueProperty.intValue = EditorGUI.DelayedIntField(fieldPosition, baseValueProperty.intValue);
                if (temp != baseValueProperty.intValue)
                {
                    isDirty = true;
                }
            }
        }

        private void DrawRangeFields(Rect position, float labelWidth, float fieldWidth, SerializedProperty property)
        {
            SerializedProperty minValueProperty = property.FindPropertyRelative("_range._minValue");
            SerializedProperty maxValueProperty = property.FindPropertyRelative("_range._maxValue");
            SerializedProperty currentValueProperty = property.FindPropertyRelative("_range._currentValue");

            float minValue = GetPropertyNumberValue(minValueProperty);
            float maxValue = GetPropertyNumberValue(maxValueProperty);

            Rect currentFieldRect = new Rect(position.x + labelWidth, position.y, fieldWidth, EditorGUIUtility.singleLineHeight);
            DrawMinMaxCustomValueField(currentFieldRect, new GUIContent("Current:"), currentValueProperty, minValue, maxValue);
        }

        private void DrawExpandCollapseButton(Rect position, SerializedProperty property, DynamicValue<TValue> value)
        {
            float buttonXOffset = position.x + position.width - ButtonWidth - 3;
            Rect buttonRect = new Rect(buttonXOffset, position.y, ButtonWidth, EditorGUIUtility.singleLineHeight);

            if (value != null)
            {
                string symbol = value.EditorIsExpanded ? "-" : "+";

                if (GUI.Button(buttonRect, new GUIContent(symbol)))
                {
                    value.EditorIsExpanded = !value.EditorIsExpanded;
                }
            }
        }

        private void DrawFinalValueField(SerializedProperty property)
        {
            SerializedProperty finalValueProperty = property.FindPropertyRelative("_calculatedValue");

            Rect rect = EditorGUILayout.GetControlRect();
            float leftOffset = 13f;
            Rect labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth + leftOffset, rect.height);
            Rect fieldRect = new Rect(rect.x + EditorGUIUtility.labelWidth - leftOffset, rect.y, rect.width - EditorGUIUtility.labelWidth, rect.height);

            GUIContent guiContent = new GUIContent("Calculated (Max):", "Represents the final computed value after applying all modifiers, including static and temporary adjustments.");

            GUI.enabled = false;

            EditorGUI.LabelField(labelRect, guiContent);

            if (finalValueProperty.propertyType == SerializedPropertyType.Float)
            {
                EditorGUI.FloatField(fieldRect, finalValueProperty.floatValue);
            }
            else if (finalValueProperty.propertyType == SerializedPropertyType.Integer)
            {
                EditorGUI.IntField(fieldRect, finalValueProperty.intValue);
            }

            GUI.enabled = true;

            if (Event.current.type == EventType.ContextClick && labelRect.Contains(Event.current.mousePosition))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Force Recalculate"), false, () => ForceRecalculation(property));
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        private void ForceRecalculation(SerializedProperty property)
        {
            if (fieldInfo.GetValue(property.serializedObject.targetObject) is DynamicValue<TValue> value)
            {
                value.Recalculate();
                Debug.Log("Recalculation forced.");
            }
            else
            {
                Debug.LogWarning("DynamicValue is null. Cannot recalculate.");
            }
        }

        private void DrawAdditionalFields(SerializedProperty property, DynamicValue<TValue> value)
        {
            SerializedProperty valueShiftProperty = property.FindPropertyRelative("valueShift._baseValue");
            SerializedProperty changeTimeProperty = property.FindPropertyRelative("valueShiftTimer.timerDuration");
            SerializedProperty valueCurveProperty = property.FindPropertyRelative("valueCurve");
            SerializedProperty minValueProperty = property.FindPropertyRelative("_range._minValue");

            if (minValueProperty != null)
                EditorGUILayout.PropertyField(minValueProperty, new GUIContent("Min Value"));

            if (valueShiftProperty != null)
                EditorGUILayout.PropertyField(valueShiftProperty, new GUIContent("Value Shift"));

            if (changeTimeProperty != null)
                EditorGUILayout.PropertyField(changeTimeProperty, new GUIContent("Change Time"));

            if (valueCurveProperty != null)
                EditorGUILayout.PropertyField(valueCurveProperty, new GUIContent("Value Curve"));

            GUIStyle smallFontStyle = new GUIStyle(GUI.skin.label) { fontSize = 10 };
            if (value != null)
            {
                EditorGUILayout.LabelField($"Mods: Total [{value.AllModifiersCount}], Static [{value.StaticModifierCount}], Temp [{value.TempModifierCount}]", smallFontStyle);
            }
        }

        private bool IsDynamicRangeType()
        {
            Type fieldType = fieldInfo.FieldType;
            return typeof(DynamicIntRange).IsAssignableFrom(fieldType) || typeof(DynamicFloatRange).IsAssignableFrom(fieldType);
        }

        private float GetPropertyNumberValue(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Integer ? property.intValue : property.floatValue;
        }
    }
}