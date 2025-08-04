using Gamegaard.Commons.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace Gamegaard.DynamicValues.Editor
{
    [CustomPropertyDrawer(typeof(MultipleModifierApplier<>), true)]
    public class MultipleModifierApplierDrawer : PropertyDrawer
    {
        private ReorderableList _reorderableList;
        private SerializedProperty _modifiersProperty;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_reorderableList == null)
            {
                InitializeReorderableList(property);
            }

            EditorGUI.BeginProperty(position, label, property);
            _reorderableList.DoList(position);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_reorderableList == null)
            {
                InitializeReorderableList(property);
            }

            return _reorderableList.GetHeight();
        }

        private void InitializeReorderableList(SerializedProperty property)
        {
            _modifiersProperty = property.FindPropertyRelative("_modifiers");

            _reorderableList = new ReorderableList(property.serializedObject, _modifiersProperty, true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Modifiers");
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    SerializedProperty element = _modifiersProperty.GetArrayElementAtIndex(index);
                    SerializedProperty nameProperty = element.FindPropertyRelative("EditorName");

                    rect.y += 2;

                    Rect nameLabelRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

                    if (Event.current.type == EventType.MouseDown && Event.current.clickCount == 2 && nameLabelRect.Contains(Event.current.mousePosition))
                    {
                        InputWindow.ShowWindow(
                            "Rename Modifier",
                            "Enter a new name for the modifier:",
                            "Modifier Name",
                            "Rename",
                            "Cancel",
                            newName =>
                            {
                                nameProperty.stringValue = newName;
                                _modifiersProperty.serializedObject.ApplyModifiedProperties();
                            }
                        );
                        Event.current.Use();
                    }

                    EditorGUI.PropertyField(
                        new Rect(rect.x + 15, rect.y, rect.width - 15, EditorGUI.GetPropertyHeight(element, true)),
                        element,
                        true
                    );
                },

                elementHeightCallback = index =>
                {
                    SerializedProperty element = _modifiersProperty.GetArrayElementAtIndex(index);
                    return EditorGUI.GetPropertyHeight(element) + 4;
                },

                onAddCallback = list =>
                {
                    ShowAddMenu();
                },

                onRemoveCallback = list =>
                {
                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                    property.serializedObject.ApplyModifiedProperties();
                }
            };
        }

        private void ShowAddMenu()
        {
            Type multipleModifierType = fieldInfo.FieldType.GetGenericArguments()[0];
            Type baseType = typeof(ModifierApplierBaseGeneric<>).MakeGenericType(multipleModifierType);

            List<Type> validTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsGenericType)
                .ToList();

            TypeSearchDropdown dropdown = new TypeSearchDropdown(new AdvancedDropdownState(), validTypes, selectedType =>
            {
                _modifiersProperty.arraySize++;
                SerializedProperty newElement = _modifiersProperty.GetArrayElementAtIndex(_modifiersProperty.arraySize - 1);

                object instance = Activator.CreateInstance(selectedType);
                newElement.managedReferenceValue = instance;

                SerializedProperty nameProperty = newElement.FindPropertyRelative("EditorName");
                if (nameProperty != null)
                {
                    string name = selectedType.Name.Replace("Modifier", "");
                    nameProperty.stringValue = name;
                }

                _modifiersProperty.serializedObject.ApplyModifiedProperties();
            }, false, false);

            dropdown.Show(new Rect(Event.current.mousePosition, Vector2.zero));
        }
    }
}
