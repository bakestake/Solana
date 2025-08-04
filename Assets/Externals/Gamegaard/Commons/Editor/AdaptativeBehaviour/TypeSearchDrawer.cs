using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using Gamegaard.Commons.Editor;

namespace Gamegaard.AdaptativeBehavior.Editor
{
    [CustomPropertyDrawer(typeof(TypeSearchAttribute))]
    public class TypeSearchDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 25f;
        private const float Padding = 4f;
        private const string SelectedTypeKey = "SelectedType_";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TypeSearchAttribute searchAttribute = (TypeSearchAttribute)attribute;
            Type baseType = searchAttribute.BaseType;

            float propertyHeight = EditorGUI.GetPropertyHeight(property, true);
            Rect fieldRect = new Rect(position.x, position.y, position.width - ButtonWidth - Padding, propertyHeight);
            Rect buttonRect = new Rect(position.x + position.width - ButtonWidth, position.y, ButtonWidth, EditorGUIUtility.singleLineHeight);

            string propertyKey = SelectedTypeKey + property.propertyPath;
            string selectedTypeName = EditorPrefs.GetString(propertyKey, string.Empty);
            Type selectedType = !string.IsNullOrEmpty(selectedTypeName) ? Type.GetType(selectedTypeName) : null;

            object instance = property.managedReferenceValue;
            bool isNull = instance == null;
            string displayText = isNull ? "None" : instance.GetType().Name;

            if (typeof(UnityEngine.Object).IsAssignableFrom(baseType))
            {
                property.objectReferenceValue = EditorGUI.ObjectField(fieldRect, label, property.objectReferenceValue, baseType, true);
            }
            else if (instance != null && instance.GetType().IsSerializable)
            {
                EditorGUI.PropertyField(fieldRect, property, label, true);
            }
            else
            {
                EditorGUI.LabelField(fieldRect, label, new GUIContent(displayText));
            }

            if (GUI.Button(buttonRect, "🔍"))
            {
                ShowTypeSearchMenu(property, baseType);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true) + Padding;
        }

        private void ShowTypeSearchMenu(SerializedProperty property, Type baseType)
        {
            List<Type> validTypes = GetDerivedTypesWithValidConstructors(baseType);

            if (validTypes.Count > 0)
            {
                TypeSearchDropdown dropdown = new TypeSearchDropdown(
                    new AdvancedDropdownState(),
                    validTypes,
                    selectedType =>
                    {
                        AssignTypeToProperty(property, selectedType);
                    }, false, false);

                dropdown.Show(new Rect(Event.current.mousePosition, Vector2.zero));
            }
            else
            {
                Debug.LogWarning($"No valid types found for {baseType}. Ensure subclasses exist and have parameterless constructors.");
            }
        }

        private void AssignTypeToProperty(SerializedProperty property, Type type)
        {
            property.serializedObject.Update();

            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                ScriptableObject instance = ScriptableObject.CreateInstance(type);

                string assetPath = "Assets/EditorGenerated";
                if (!AssetDatabase.IsValidFolder(assetPath))
                {
                    AssetDatabase.CreateFolder("Assets", "EditorGenerated");
                }

                string uniquePath = AssetDatabase.GenerateUniqueAssetPath($"{assetPath}/{type.Name}.asset");
                AssetDatabase.CreateAsset(instance, uniquePath);
                AssetDatabase.SaveAssets();

                property.objectReferenceValue = instance;
            }
            else
            {
                object instance = Activator.CreateInstance(type);
                property.managedReferenceValue = instance;
            }

            property.serializedObject.ApplyModifiedProperties();
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