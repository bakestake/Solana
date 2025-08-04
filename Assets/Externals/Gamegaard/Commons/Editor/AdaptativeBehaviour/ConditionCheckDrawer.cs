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
    [CustomPropertyDrawer(typeof(ConditionCheck))]
    public class ConditionCheckDrawer : PropertyDrawer
    {
        private const float Padding = 4f;
        private static readonly float LineHeight = EditorGUIUtility.singleLineHeight;
        private bool isExpanded = true;

        private ReorderableList conditionsList;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!isExpanded) return LineHeight + Padding;

            SerializedProperty conditionEvaluatorProp = property.FindPropertyRelative("conditionEvaluator");
            SerializedProperty conditionsProp = property.FindPropertyRelative("conditions");

            float evaluatorHeight = EditorGUI.GetPropertyHeight(conditionEvaluatorProp, true);
            float conditionsHeight = conditionsList != null ? conditionsList.GetHeight() : EditorGUI.GetPropertyHeight(conditionsProp, true);

            return LineHeight + evaluatorHeight + conditionsHeight + (Padding * 4);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty conditionEvaluatorProp = property.FindPropertyRelative("conditionEvaluator");
            SerializedProperty conditionsProp = property.FindPropertyRelative("conditions");

            if (conditionsList == null)
                SetupReorderableList(conditionsProp);

            Rect foldoutRect = new Rect(position.x, position.y, position.width, LineHeight);
            isExpanded = EditorGUI.Foldout(foldoutRect, isExpanded, label, true, EditorStyles.foldout);

            if (!isExpanded) return;

            EditorGUI.indentLevel++;

            float yOffset = foldoutRect.y + LineHeight + Padding;

            Rect evaluatorRect = new Rect(position.x, yOffset, position.width, EditorGUI.GetPropertyHeight(conditionEvaluatorProp, true));
            EditorGUI.PropertyField(evaluatorRect, conditionEvaluatorProp, new GUIContent("Condition Evaluator"), true);
            yOffset += evaluatorRect.height + Padding;

            Rect conditionsRect = new Rect(position.x, yOffset, position.width, conditionsList.GetHeight());
            conditionsList.DoList(conditionsRect);

            EditorGUI.indentLevel--;
        }

        private void SetupReorderableList(SerializedProperty conditionsProp)
        {
            conditionsList = new ReorderableList(conditionsProp.serializedObject, conditionsProp, true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Conditions", EditorStyles.boldLabel);
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = conditionsProp.GetArrayElementAtIndex(index);

                    if (element == null || element.managedReferenceValue == null)
                        return;

                    string className = element.managedReferenceValue.GetType().Name;
                    float elementHeight = EditorGUI.GetPropertyHeight(element, true);

                    rect.x += 15;
                    rect.width -= 30;

                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), className, EditorStyles.boldLabel);
                    EditorGUI.PropertyField(rect, element, GUIContent.none, true);
                },

                elementHeightCallback = index =>
                {
                    var element = conditionsProp.GetArrayElementAtIndex(index);
                    if (element == null || element.managedReferenceValue == null)
                        return EditorGUIUtility.singleLineHeight;

                    return EditorGUI.GetPropertyHeight(element, true);
                },

                onAddDropdownCallback = (buttonRect, list) =>
                {
                    ShowAddMenuForConditions(conditionsProp);
                },

                onRemoveCallback = list =>
                {
                    if (list.index >= 0 && list.index < conditionsProp.arraySize)
                    {
                        conditionsProp.DeleteArrayElementAtIndex(list.index);
                        conditionsProp.serializedObject.ApplyModifiedProperties();
                    }
                }
            };
        }

        private void ShowAddMenuForConditions(SerializedProperty conditionsProp)
        {
            List<Type> validTypes = GetDerivedTypesWithValidConstructors(typeof(IAdaptativeBehaviorCondition));

            if (validTypes.Count > 0)
            {
                TypeSearchDropdown dropdown = new TypeSearchDropdown(
                    new AdvancedDropdownState(),
                    validTypes,
                    selectedType =>
                    {
                        AddCondition(conditionsProp, selectedType);
                    }, false, false);

                dropdown.Show(new Rect(Event.current.mousePosition, Vector2.zero));
            }
            else
            {
                Debug.LogWarning("No valid conditions found. Ensure they have a parameterless constructor.");
            }
        }

        private void AddCondition(SerializedProperty conditionsProp, Type type)
        {
            conditionsProp.serializedObject.Update();

            object instance = Activator.CreateInstance(type);
            conditionsProp.arraySize++;
            SerializedProperty newElement = conditionsProp.GetArrayElementAtIndex(conditionsProp.arraySize - 1);
            newElement.managedReferenceValue = instance;

            conditionsProp.serializedObject.ApplyModifiedProperties();
        }

        private List<Type> GetDerivedTypesWithValidConstructors(Type baseType)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null)
                .ToList();
        }
    }
}
