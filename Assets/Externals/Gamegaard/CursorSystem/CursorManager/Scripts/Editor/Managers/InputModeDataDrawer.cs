using UnityEditor;
using UnityEngine;

namespace Gamegaard.CursorSystem.Editor
{
    [CustomPropertyDrawer(typeof(InputModeData))]
    public class InputModeDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty inputModeProp = property.FindPropertyRelative("inputMode");
            SerializedProperty inputActionReferenceProp = property.FindPropertyRelative("inputActionReference");

            Rect inputModeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            float spacing = EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(inputModeRect, inputModeProp, new GUIContent("Input Mode"));

            if ((InputMode)inputModeProp.enumValueIndex == InputMode.NewInputSystem)
            {
                Rect inputActionReferenceRect = new Rect(
                    position.x,
                    inputModeRect.yMax + spacing,
                    position.width,
                    EditorGUI.GetPropertyHeight(inputActionReferenceProp)
                );

                EditorGUI.PropertyField(inputActionReferenceRect, inputActionReferenceProp, new GUIContent("Input Action Reference"));
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty inputModeProp = property.FindPropertyRelative("inputMode");
            SerializedProperty inputActionReferenceProp = property.FindPropertyRelative("inputActionReference");

            float height = EditorGUIUtility.singleLineHeight;
            if ((InputMode)inputModeProp.enumValueIndex == InputMode.NewInputSystem)
            {
                height += EditorGUI.GetPropertyHeight(inputActionReferenceProp) + EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }
    }
}