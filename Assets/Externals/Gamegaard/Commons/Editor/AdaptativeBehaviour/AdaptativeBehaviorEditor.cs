using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using Gamegaard.Commons.Editor;

namespace Gamegaard.AdaptativeBehavior.Editor
{
    [CustomEditor(typeof(AdaptativeBehavior))]
    public class AdaptativeBehaviorEditor : UnityEditor.Editor
    {
        private SerializedProperty effectsProp;
        private SerializedProperty onConditionsMeetProp;

        private ReorderableList effectsList;

        private void OnEnable()
        {
            effectsProp = serializedObject.FindProperty("effects");
            onConditionsMeetProp = serializedObject.FindProperty("OnConditionsMeet");

            effectsList = CreateReorderableList(effectsProp, "Effects", typeof(IAdaptativeBehaviorEffect));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Adaptative Behavior", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("conditions"), new GUIContent("Conditions"), true);

            EditorGUILayout.Space(5);
            effectsList.DoLayoutList();
            EditorGUILayout.Space(5);

            EditorGUILayout.PropertyField(onConditionsMeetProp, new GUIContent("On Conditions Meet"));
            EditorGUILayout.Space(5);

            serializedObject.ApplyModifiedProperties();
        }

        private ReorderableList CreateReorderableList(SerializedProperty property, string label, Type interfaceType)
        {
            return new ReorderableList(serializedObject, property, true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, label, EditorStyles.boldLabel);
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = property.GetArrayElementAtIndex(index);

                    if (element == null || element.managedReferenceValue == null)
                        return;

                    string className = element.managedReferenceValue.GetType().Name;
                    float elementHeight = EditorGUI.GetPropertyHeight(element, true);

                    rect.y += 2;
                    rect.height = elementHeight;
                    rect.x += 15;
                    rect.width -= 30;

                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), className, EditorStyles.boldLabel);
                    rect.y += EditorGUIUtility.singleLineHeight + 2;

                    EditorGUI.PropertyField(rect, element, GUIContent.none, true);
                },

                elementHeightCallback = index =>
                {
                    var element = property.GetArrayElementAtIndex(index);
                    if (element == null || element.managedReferenceValue == null)
                        return EditorGUIUtility.singleLineHeight;

                    return EditorGUI.GetPropertyHeight(element, true);
                },

                onAddDropdownCallback = (buttonRect, list) =>
                {
                    ShowAddMenuForComplexTypes(property, interfaceType);
                },

                onRemoveCallback = list =>
                {
                    if (list.index >= 0 && list.index < property.arraySize)
                    {
                        property.DeleteArrayElementAtIndex(list.index);
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            };
        }

        private void ShowAddMenuForComplexTypes(SerializedProperty property, Type baseType)
        {
            List<Type> validTypes = GetDerivedTypesWithValidConstructors(baseType);

            if (validTypes.Count > 0)
            {
                TypeSearchDropdown dropdown = new TypeSearchDropdown(
                    new AdvancedDropdownState(),
                    validTypes,
                    selectedType =>
                    {
                        AddComplexElement(property, selectedType);
                    }, false, false);

                dropdown.Show(new Rect(Event.current.mousePosition, Vector2.zero));
            }
            else
            {
                Debug.LogWarning($"No valid types found for {baseType}. Ensure subclasses exist and have parameterless constructors.");
            }
        }

        private List<Type> GetDerivedTypesWithValidConstructors(Type baseType)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null)
                .ToList();
        }

        private void AddComplexElement(SerializedProperty property, Type type)
        {
            property.serializedObject.Update();

            object instance = Activator.CreateInstance(type);
            property.arraySize++;
            SerializedProperty element = property.GetArrayElementAtIndex(property.arraySize - 1);
            element.managedReferenceValue = instance;

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}