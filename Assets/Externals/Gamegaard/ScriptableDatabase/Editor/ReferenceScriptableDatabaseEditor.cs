using Gamegaard.Commons.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace Gamegaard.CustomValues.Database.Editor
{
    [CustomEditor(typeof(ReferenceScriptableDatabase<>), true)]
    public class ReferenceScriptableDatabaseEditor : UnityEditor.Editor
    {
        private ReorderableList reorderableList;
        private SerializedProperty datasProperty;

        private void OnEnable()
        {
            datasProperty = serializedObject.FindProperty("datas");

            if (datasProperty != null)
            {
                InitializeReorderableList();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Scriptable Database", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (reorderableList != null)
            {
                reorderableList.DoLayoutList();
            }
            else
            {
                EditorGUILayout.HelpBox("Lista de dados não encontrada ou não inicializada.", MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void InitializeReorderableList()
        {
            reorderableList = new ReorderableList(serializedObject, datasProperty, true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Database Items");
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    if (index >= datasProperty.arraySize) return;

                    SerializedProperty element = datasProperty.GetArrayElementAtIndex(index);

                    if (element.managedReferenceValue != null)
                    {
                        string typeName = element.managedReferenceFullTypename.Split('.').Last();

                        GUIContent label = new GUIContent(typeName);

                        EditorGUI.PropertyField(rect, element, label, true);
                    }
                    else
                    {
                        EditorGUI.LabelField(rect, $"Elemento {index + 1} (Inválido ou Nulo)");
                    }
                },

                elementHeightCallback = index =>
                {
                    if (index >= datasProperty.arraySize) return EditorGUIUtility.singleLineHeight;

                    SerializedProperty element = datasProperty.GetArrayElementAtIndex(index);
                    return EditorGUI.GetPropertyHeight(element, true) + EditorGUIUtility.standardVerticalSpacing;
                },

                onAddCallback = list =>
                {
                    Type baseType = GetListElementType(datasProperty);
                    if (baseType == null)
                    {
                        Debug.LogError("Não foi possível determinar o tipo base dos elementos da lista.");
                        return;
                    }

                    List<Type> allDerivedTypes = TypeCache.GetTypesDerivedFrom(baseType)
                        .Where(t => !t.IsAbstract && t.IsClass)
                        .ToList();

                    HashSet<Type> addedTypes = new HashSet<Type>();
                    for (int i = 0; i < datasProperty.arraySize; i++)
                    {
                        SerializedProperty element = datasProperty.GetArrayElementAtIndex(i);
                        if (element.managedReferenceValue != null)
                        {
                            addedTypes.Add(element.managedReferenceValue.GetType());
                        }
                    }

                    List<Type> availableTypes = allDerivedTypes.Where(type => !addedTypes.Contains(type)).ToList();

                    if (availableTypes.Count == 0)
                    {
                        Debug.LogWarning("Nenhum tipo disponível para adicionar.");
                        return;
                    }

                    TypeSearchDropdown dropdown = new TypeSearchDropdown(
                        new AdvancedDropdownState(),
                        availableTypes,
                        selectedType =>
                        {
                            var newElement = Activator.CreateInstance(selectedType);
                            datasProperty.serializedObject.Update();
                            datasProperty.arraySize++;
                            SerializedProperty addedElement = datasProperty.GetArrayElementAtIndex(datasProperty.arraySize - 1);
                            addedElement.managedReferenceValue = newElement;
                            datasProperty.serializedObject.ApplyModifiedProperties();
                        }, false, false);

                    dropdown.Show(new Rect(Event.current.mousePosition, Vector2.zero));
                },

                onRemoveCallback = list =>
                {
                    if (list.index >= 0 && list.index < datasProperty.arraySize)
                    {
                        datasProperty.DeleteArrayElementAtIndex(list.index);

                        if (list.index < datasProperty.arraySize &&
                            datasProperty.GetArrayElementAtIndex(list.index).managedReferenceValue == null)
                        {
                            datasProperty.DeleteArrayElementAtIndex(list.index);
                        }

                        datasProperty.serializedObject.ApplyModifiedProperties();
                    }
                }
            };
        }

        private Type GetListElementType(SerializedProperty property)
        {
            var targetType = target.GetType();
            while (targetType != null)
            {
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(ReferenceScriptableDatabase<>))
                {
                    return targetType.GetGenericArguments()[0];
                }

                targetType = targetType.BaseType;
            }

            return null;
        }
    }
}