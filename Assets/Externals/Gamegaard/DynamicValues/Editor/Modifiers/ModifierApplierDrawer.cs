using Gamegaard.Commons.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Gamegaard.DynamicValues.Editor
{
    [CustomPropertyDrawer(typeof(ModifierApplier<,>), true)]
    public class ModifierApplierDrawer : PropertyDrawer
    {
        private static readonly float LineHeight = EditorGUIUtility.singleLineHeight;
        private static readonly float VerticalSpacing = EditorGUIUtility.standardVerticalSpacing;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect foldoutRect = new Rect(position.x, position.y, position.width, LineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                position.y += LineHeight + VerticalSpacing;

                SerializedProperty typeProperty = property.FindPropertyRelative("type");
                SerializedProperty calculationStrategyProperty = property.FindPropertyRelative("calculationStrategy");

                if (typeProperty != null)
                {
                    Rect typeRect = new Rect(position.x, position.y, position.width, LineHeight);
                    EditorGUI.PropertyField(typeRect, typeProperty, new GUIContent("Type"));
                    position.y += LineHeight;

                    string description = GetEnumDescription(typeProperty.enumValueIndex);
                    GUIStyle descriptionStyle = new GUIStyle(EditorStyles.label)
                    {
                        fontSize = 10,
                        normal = { textColor = Color.gray }
                    };
                    Rect descriptionRect = new Rect(position.x, position.y, position.width, LineHeight);
                    EditorGUI.LabelField(descriptionRect, description, descriptionStyle);
                    position.y += LineHeight + VerticalSpacing;
                }

                SerializedProperty currentProperty = typeProperty.Copy();
                SerializedProperty endProperty = property.GetEndProperty();

                while (currentProperty.NextVisible(true) && !SerializedProperty.EqualContents(currentProperty, endProperty))
                {
                    if (currentProperty.propertyPath == calculationStrategyProperty.propertyPath)
                        continue;

                    Rect fieldRect = new Rect(position.x, position.y, position.width, EditorGUI.GetPropertyHeight(currentProperty));
                    EditorGUI.PropertyField(fieldRect, currentProperty, true);
                    position.y += EditorGUI.GetPropertyHeight(currentProperty) + VerticalSpacing;
                }

                if (typeProperty != null && typeProperty.enumValueIndex == (int)ModifierCalculationType.Custom)
                {
                    Rect strategyLabelRect = new Rect(position.x, position.y, position.width - 60, LineHeight);
                    Rect strategyButtonRect = new Rect(position.x + position.width - 55, position.y, 55, LineHeight);

                    DrawStrategyField(strategyLabelRect, strategyButtonRect, calculationStrategyProperty);
                    position.y += LineHeight + VerticalSpacing;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = LineHeight;

            if (property.isExpanded)
            {
                SerializedProperty typeProperty = property.FindPropertyRelative("type");
                if (typeProperty != null)
                {
                    height += LineHeight;
                    if (typeProperty.enumValueIndex != (int)ModifierCalculationType.Custom)
                    {
                        height -= LineHeight;
                    }
                }

                SerializedProperty currentProperty = property.Copy();
                SerializedProperty endProperty = property.GetEndProperty();

                while (currentProperty.NextVisible(true) && !SerializedProperty.EqualContents(currentProperty, endProperty))
                {
                    if (currentProperty.propertyPath == "calculationStrategy")
                        continue;

                    height += EditorGUI.GetPropertyHeight(currentProperty) + VerticalSpacing;
                }
            }

            return height;
        }

        private void DrawStrategyField(Rect labelRect, Rect buttonRect, SerializedProperty property)
        {
            string typeName;

            if (property.managedReferenceValue == null)
            {
                typeName = "<color=red>None</color> (No calculation strategy assigned, modifier will have no effect)";
            }
            else
            {
                string fullTypeName = property.managedReferenceFullTypename;
                typeName = string.IsNullOrEmpty(fullTypeName)
                    ? "<color=red>None</color> (No calculation strategy assigned, modifier will have no effect)"
                    : fullTypeName.Split('.').Last();
            }

            GUIStyle labelStyle = new GUIStyle(EditorStyles.label)
            {
                richText = true
            };

            EditorGUI.LabelField(labelRect, $"Strategy: <b>{typeName}</b>", labelStyle);

            if (GUI.Button(buttonRect, property.managedReferenceValue == null ? "Add" : "Change"))
            {
                ShowAddMenu(property);
            }
        }

        private void ShowAddMenu(SerializedProperty property)
        {
            if (property == null)
            {
                Debug.LogError("Property is null in ShowAddMenu.");
                return;
            }

            Type baseType = typeof(ModifierStrategyBase);

            List<Type> validTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract && !t.ContainsGenericParameters && t != baseType)
                .ToList();

            if (validTypes.Count == 0)
            {
                Debug.LogWarning("No valid ModifierCalculation types found.");
                return;
            }

            TypeSearchDropdown dropdown = new TypeSearchDropdown(new AdvancedDropdownState(), validTypes, selectedType =>
            {
                object instance = Activator.CreateInstance(selectedType);
                property.managedReferenceValue = instance;
                property.serializedObject.ApplyModifiedProperties();
            }, false, false);

            dropdown.Show(new Rect(Event.current.mousePosition, Vector2.zero));
        }

        private string GetEnumDescription(int enumIndex)
        {
            return enumIndex switch
            {
                0 => "Flat: Adds a fixed value to the base.",
                1 => "Percentage: Adds as a percentage (e.g., 10 = 10%).",
                2 => "Overall: Adds a percentage based on all modifiers.",
                3 => "Flat %: Adds based on flat modifiers.",
                4 => "Custom: Apply a custom calculation to the value.",
                _ => "Unknown Modifier Type."
            };
        }
    }
}
