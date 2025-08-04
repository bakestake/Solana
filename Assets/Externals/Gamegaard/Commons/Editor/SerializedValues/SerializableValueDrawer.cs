using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using Gamegaard.Commons.Editor;

namespace Gamegaard.CustomValues.Editor
{
    [CustomPropertyDrawer(typeof(SerializableValue<>), true)]
    public class SerializableValueDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 25f;
        private const float Padding = 4f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty unityRef = property.FindPropertyRelative("unityReference");
            SerializedProperty serializedRef = property.FindPropertyRelative("serializedReference");
            SerializedProperty currentType = property.FindPropertyRelative("currentType");
            Type interfaceType = fieldInfo.FieldType.GetGenericArguments()[0];

            float propertyHeight = EditorGUIUtility.singleLineHeight;
            Rect fieldRect = new Rect(position.x, position.y, position.width - ButtonWidth - Padding, propertyHeight);
            Rect buttonRect = new Rect(position.x + position.width - ButtonWidth, position.y, ButtonWidth, propertyHeight);

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            if (serializedRef.managedReferenceValue == null)
            {
                UnityEngine.Object newUnityObj = EditorGUI.ObjectField(
                    fieldRect,
                    label.text,
                    unityRef.objectReferenceValue,
                    Type.GetType(currentType.stringValue) ?? typeof(UnityEngine.Object),
                    true);

                if (EditorGUI.EndChangeCheck())
                {
                    unityRef.objectReferenceValue = newUnityObj;
                    serializedRef.managedReferenceValue = null;
                    property.serializedObject.ApplyModifiedProperties();
                    GUI.changed = true;
                }
            }
            else
            {
                string className = serializedRef.managedReferenceValue.GetType().Name;
                EditorGUI.LabelField(fieldRect, $"{label.text}: {className}", EditorStyles.boldLabel);
            }

            ShowTypeSearchButton(buttonRect, property, interfaceType);

            if (serializedRef.managedReferenceValue != null)
            {
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(
                    new Rect(position.x, position.y + propertyHeight, position.width, EditorGUI.GetPropertyHeight(serializedRef, true)),
                    serializedRef,
                    GUIContent.none,
                    true);
                EditorGUI.LabelField(
                       new Rect(position.x + 5, position.y + propertyHeight, position.width, EditorGUI.GetPropertyHeight(serializedRef, false)),
                       "Value");
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty serializedRef = property.FindPropertyRelative("serializedReference");

            if (serializedRef.managedReferenceValue == null)
            {
                return EditorGUIUtility.singleLineHeight + Padding;
            }

            return EditorGUIUtility.singleLineHeight + EditorGUI.GetPropertyHeight(serializedRef, true) + Padding;
        }

        private void ShowTypeSearchButton(Rect buttonRect, SerializedProperty property, Type interfaceType)
        {
            if (GUI.Button(buttonRect, "🔍"))
            {
                List<Type> validTypes = TypeFinder.GetDerivedTypesWithValidConstructors(interfaceType);
                if (validTypes.Count > 0)
                {
                    TypeSearchDropdown dropdown = new TypeSearchDropdown(
                        new AdvancedDropdownState(),
                        validTypes,
                        selectedType => AssignTypeToProperty(property, selectedType),
                        false, false);

                    dropdown.Show(new Rect(Event.current.mousePosition, Vector2.zero));
                }
                else
                {
                    Debug.LogWarning($"No valid types found for {interfaceType}. Ensure subclasses exist and have parameterless constructors.");
                }
            }
        }

        private void AssignTypeToProperty(SerializedProperty property, Type type)
        {
            property.serializedObject.Update();

            SerializedProperty unityRef = property.FindPropertyRelative("unityReference");
            SerializedProperty serializedRef = property.FindPropertyRelative("serializedReference");
            SerializedProperty currentType = property.FindPropertyRelative("currentType");

            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                unityRef.objectReferenceValue = null;
                serializedRef.managedReferenceValue = null;
            }
            else
            {
                object instance = Activator.CreateInstance(type);
                serializedRef.managedReferenceValue = instance;
                unityRef.objectReferenceValue = null;
            }

            currentType.stringValue = type.AssemblyQualifiedName;

            property.serializedObject.ApplyModifiedProperties();
            GUI.changed = true;
        }
    }
}