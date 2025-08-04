using Gamegaard.Commons.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace Gamegaard.CustomValues.Editor
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
    public class SerializableDictionaryEditor : PropertyDrawer
    {
        private bool foldout = true;
        private ReorderableList list;
        private static readonly float singleLineHeight = EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect foldoutRect = new Rect(position.x, position.y, position.width - 50, singleLineHeight);
            foldout = EditorGUI.Foldout(foldoutRect, foldout, label, true);

            if (foldout)
            {
                if (list == null)
                {
                    InitializeReorderableList(property);
                }

                EditorGUI.indentLevel++;

                Rect listPosition = new Rect(position.x, position.y + singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, list.GetHeight());
                list.DoList(listPosition);

                EditorGUI.indentLevel--;
            }

            property.serializedObject.ApplyModifiedProperties();
        }

        private void InitializeReorderableList(SerializedProperty property)
        {
            SerializedProperty keyValuePairsProp = property.FindPropertyRelative("keyValuePairs");

            list = new ReorderableList(property.serializedObject, keyValuePairsProp);

            list.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Dictionary Elements");
            };

            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                SerializedProperty element = keyValuePairsProp.GetArrayElementAtIndex(index);
                if (element == null) return;

                SerializedProperty keyProp = element.FindPropertyRelative("key");
                SerializedProperty valueProp = element.FindPropertyRelative("value");

                SerializedProperty unityReferenceProp = valueProp.FindPropertyRelative("unityReference");
                SerializedProperty serializedReferenceProp = valueProp.FindPropertyRelative("serializedReference");
                SerializedProperty currentTypeProp = valueProp.FindPropertyRelative("currentType");

                Type valueType = null;
                if (currentTypeProp != null && !string.IsNullOrEmpty(currentTypeProp.stringValue))
                {
                    valueType = Type.GetType(currentTypeProp.stringValue);
                }

                Rect keyRect = new Rect(rect.x, rect.y, rect.width / 2 - 5, singleLineHeight);
                Rect valueRect = new Rect(rect.x + rect.width / 2 + 5, rect.y, rect.width / 2 - 5, singleLineHeight);

                EditorGUI.PropertyField(keyRect, keyProp, GUIContent.none);
                if (valueType != null)
                {
                    if (typeof(UnityEngine.Object).IsAssignableFrom(valueType))
                    {
                        unityReferenceProp.objectReferenceValue = EditorGUI.ObjectField(valueRect, unityReferenceProp.objectReferenceValue, valueType, true);
                    }
                    else if (valueType == typeof(int))
                    {
                        serializedReferenceProp.intValue = EditorGUI.IntField(valueRect, serializedReferenceProp.intValue);
                    }
                    else if (valueType == typeof(float))
                    {
                        serializedReferenceProp.floatValue = EditorGUI.FloatField(valueRect, serializedReferenceProp.floatValue);
                    }
                    else if (valueType == typeof(bool))
                    {
                        serializedReferenceProp.boolValue = EditorGUI.Toggle(valueRect, serializedReferenceProp.boolValue);
                    }
                    else if (valueType == typeof(string))
                    {
                        serializedReferenceProp.stringValue = EditorGUI.TextField(valueRect, serializedReferenceProp.stringValue);
                    }
                    else
                    {
                        Rect labelRect = new Rect(rect.x, rect.y - singleLineHeight, rect.width, rect.height);
                        Rect newRect = new Rect(rect.x, rect.y + singleLineHeight + EditorGUIUtility.standardVerticalSpacing, rect.width, EditorGUI.GetPropertyHeight(serializedReferenceProp, true));
                        EditorGUI.PropertyField(newRect, serializedReferenceProp, new GUIContent("Value"), true);
                    }
                }
                else
                {
                    EditorGUI.PropertyField(valueRect, serializedReferenceProp, new GUIContent("Value"), true);
                }
            };

            list.elementHeightCallback = (index) =>
            {
                SerializedProperty element = keyValuePairsProp.GetArrayElementAtIndex(index);
                SerializedProperty valueProp = element.FindPropertyRelative("value");
                SerializedProperty serializedReferenceProp = valueProp.FindPropertyRelative("serializedReference");

                SerializedProperty currentTypeProp = valueProp.FindPropertyRelative("currentType");
                Type valueType = !string.IsNullOrEmpty(currentTypeProp.stringValue) ? Type.GetType(currentTypeProp.stringValue) : null;

                if (valueType != null && (!valueType.IsPrimitive && !typeof(UnityEngine.Object).IsAssignableFrom(valueType)))
                {
                    return singleLineHeight + EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(serializedReferenceProp, true);
                }

                return singleLineHeight;
            };


            list.onAddDropdownCallback = (Rect rect, ReorderableList l) =>
            {
                ShowAddMenuForComplexTypes(keyValuePairsProp, property);
            };

            list.onRemoveCallback = (ReorderableList l) =>
            {
                if (l.index >= 0 && l.index < keyValuePairsProp.arraySize)
                {
                    keyValuePairsProp.DeleteArrayElementAtIndex(l.index);
                    property.serializedObject.ApplyModifiedProperties();
                }
            };
        }

        private void ShowAddMenuForComplexTypes(SerializedProperty keyValuePairsProp, SerializedProperty property)
        {
            if (TryResolveDictionaryTypes(fieldInfo?.FieldType, out Type keyType, out Type valueType))
            {
                List<Type> validTypes = TypeFinder.GetDerivedTypesWithValidConstructors(valueType);

                if (validTypes.Count > 0)
                {
                    TypeSearchDropdown dropdown = new TypeSearchDropdown(
                        new AdvancedDropdownState(),
                        validTypes,
                        selectedType =>
                        {
                            AddNewElementWithType(keyValuePairsProp, property, selectedType);
                        },
                        false,
                        false
                    );

                    dropdown.Show(new Rect(Event.current.mousePosition, Vector2.zero));
                }
                else
                {
                    Debug.LogWarning($"Nenhum tipo válido encontrado para {valueType}.");
                }
            }
        }

        private void AddNewElementWithType(SerializedProperty keyValuePairsProp, SerializedProperty property, Type selectedType)
        {
            int newIndex = keyValuePairsProp.arraySize;
            keyValuePairsProp.InsertArrayElementAtIndex(newIndex);

            SerializedProperty newElement = keyValuePairsProp.GetArrayElementAtIndex(newIndex);
            SerializedProperty newKeyProp = newElement.FindPropertyRelative("key");
            SerializedProperty newValueProp = newElement.FindPropertyRelative("value");

            SerializedProperty unityReferenceProp = newValueProp.FindPropertyRelative("unityReference");
            SerializedProperty serializedReferenceProp = newValueProp.FindPropertyRelative("serializedReference");
            SerializedProperty currentTypeProp = newValueProp.FindPropertyRelative("currentType");

            currentTypeProp.stringValue = selectedType.AssemblyQualifiedName;

            if (typeof(UnityEngine.Object).IsAssignableFrom(selectedType))
            {
                unityReferenceProp.objectReferenceValue = null;
            }
            else
            {
                serializedReferenceProp.managedReferenceValue = Activator.CreateInstance(selectedType);
            }

            SetUniqueKey(newKeyProp, keyValuePairsProp);

            property.serializedObject.ApplyModifiedProperties();
        }

        private void SetUniqueKey(SerializedProperty keyProp, SerializedProperty keyValuePairsProp)
        {
            if (TryResolveDictionaryTypes(fieldInfo?.FieldType, out Type keyType, out Type _))
            {
                object uniqueKey = GenericEditorUtils.GenerateUniqueKey(keyType, keyValuePairsProp);
                GenericEditorUtils.SetGenericPropertyValue(keyProp, uniqueKey);
            }
        }

        private bool TryResolveDictionaryTypes(Type fieldType, out Type keyType, out Type valueType)
        {
            while (fieldType != null && fieldType != typeof(object))
            {
                if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(SerializableDictionary<,>))
                {
                    Type[] args = fieldType.GetGenericArguments();
                    if (args.Length == 2)
                    {
                        keyType = args[0];
                        valueType = args[1];
                        return true;
                    }
                }

                fieldType = fieldType.BaseType;
            }

            keyType = null;
            valueType = null;
            return false;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (foldout && list != null)
            {
                return singleLineHeight + EditorGUIUtility.standardVerticalSpacing + list.GetHeight();
            }

            return singleLineHeight;
        }
    }
}