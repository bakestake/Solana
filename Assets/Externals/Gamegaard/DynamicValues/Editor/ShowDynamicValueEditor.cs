using UnityEditor;
using UnityEngine;

namespace Gamegaard.DynamicValues.Editor
{
    [CustomPropertyDrawer(typeof(ShowDynamicValue))]
    public class ShowDynamicValueEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            GUI.enabled = false;

            var dynamicValueProperty = fieldInfo.GetValue(property.serializedObject.targetObject);
            var finalValueProperty = dynamicValueProperty.GetType().GetProperty("FinalValue");

            if (finalValueProperty != null)
            {
                object finalValue = finalValueProperty.GetValue(dynamicValueProperty, null);

                GUIContent content = new GUIContent(label.text, "Final value calculated through all modifiers");
                if (finalValue is int @int)
                {
                    EditorGUI.IntField(position, content, @int);
                }
                else if (finalValue is float @float)
                {
                    EditorGUI.FloatField(position, content, @float);
                }
                else
                {
                    EditorGUI.LabelField(position, label, "Unsupported Type");
                }
            }
            else
            {
                EditorGUI.LabelField(position, label, "FinalValue not found");
            }
            GUI.enabled = true;
            EditorGUI.EndProperty();
        }
    }
}