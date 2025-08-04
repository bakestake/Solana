using Gamegaard.Commons.Editor;
using Gamegaard.UI.PopupText;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace Gamegaard.Editor
{
    [CustomEditor(typeof(PopupTextBase), true)]
    public class PopupTextBaseEditor : UnityEditor.Editor
    {
        private ReorderableList _effectsList;
        private SerializedProperty _effectsProperty;

        private void OnEnable()
        {
            _effectsProperty = serializedObject.FindProperty("effects");
            InitializeEffectsList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "effects");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Popup Effects", EditorStyles.boldLabel);
            _effectsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        private void InitializeEffectsList()
        {
            _effectsList = new ReorderableList(serializedObject, _effectsProperty, true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Modifiers");
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    SerializedProperty element = _effectsProperty.GetArrayElementAtIndex(index);
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
                                _effectsProperty.serializedObject.ApplyModifiedProperties();
                            }
                        );
                        Event.current.Use();
                    }

                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, rect.width, EditorGUI.GetPropertyHeight(element, true)),
                        element,
                        true
                    );
                },

                elementHeightCallback = index =>
                {
                    SerializedProperty element = _effectsProperty.GetArrayElementAtIndex(index);
                    return EditorGUI.GetPropertyHeight(element) + 4;
                },

                onAddCallback = list =>
                {
                    ShowAddMenu();
                },

                onRemoveCallback = list =>
                {
                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                    serializedObject.ApplyModifiedProperties();
                }
            };
        }

        private void ShowAddMenu()
        {
            TypeSearchDropdown dropdown = new TypeSearchDropdown(new AdvancedDropdownState(), typeof(PopupTextEffectBase), OnTypeSelected, GetAlreadyAddedTypes(), false, false, false, false);

            dropdown.Show(new Rect(Event.current.mousePosition, Vector2.zero));

            void OnTypeSelected(Type selectedType)
            {
                _effectsProperty.arraySize++;
                SerializedProperty newElement = _effectsProperty.GetArrayElementAtIndex(_effectsProperty.arraySize - 1);

                object instance = Activator.CreateInstance(selectedType);
                newElement.managedReferenceValue = instance;

                SerializedProperty nameProperty = newElement.FindPropertyRelative("EditorName");
                if (nameProperty != null)
                {
                    string name = selectedType.Name.Replace("Effect", "");
                    nameProperty.stringValue = name;
                }

                _effectsProperty.serializedObject.ApplyModifiedProperties();
            }

            List<Type> GetAlreadyAddedTypes()
            {
                List<Type> addedTypes = new List<Type>() { typeof(PopupTextEffectBase) };

                for (int i = 0; i < _effectsProperty.arraySize; i++)
                {
                    SerializedProperty element = _effectsProperty.GetArrayElementAtIndex(i);
                    if (element.managedReferenceValue != null)
                    {
                        addedTypes.Add(element.managedReferenceValue.GetType());
                    }
                }
                return addedTypes;
            }
        }
    }
}
