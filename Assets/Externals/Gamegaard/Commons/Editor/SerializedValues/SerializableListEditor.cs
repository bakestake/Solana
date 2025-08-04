using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using Gamegaard.Commons.Editor;

namespace Gamegaard.CustomValues.Editor
{
    [CustomPropertyDrawer(typeof(SerializableList<>), true)]
    public class SerializableListDrawer : PropertyDrawer
    {
        private ReorderableList reorderableList;
        private Type elementType;
        private bool isExpanded = true;

        private const int XOffset = 10;
        private const int FouldoutWidth = 15;
        private const int SizeFieldWidth = 48;
        private const int LabelPadding = 60;
        private readonly static float singleLineHeight = EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty itemsProperty = property.FindPropertyRelative("items");

            EnsureReorderableListInitialized(property, itemsProperty);

            EditorGUI.BeginProperty(position, label, property);
            DrawHeader(position, property, itemsProperty);

            if (isExpanded)
            {
                Rect listRect = new Rect(position.x + XOffset, position.y + singleLineHeight, position.width - XOffset, position.height - singleLineHeight);
                reorderableList.DoList(listRect);
                HandleDragAndDrop(position, property, itemsProperty);
            }

            EditorGUI.EndProperty();
        }

        private void DrawHeader(Rect position, SerializedProperty property, SerializedProperty itemsProperty)
        {
            Rect foldoutRect = new Rect(position.x + XOffset, position.y, FouldoutWidth, singleLineHeight);
            isExpanded = EditorGUI.Foldout(foldoutRect, isExpanded, GUIContent.none, true);

            Rect labelRect = new Rect(position.x + XOffset, position.y, position.width - LabelPadding, singleLineHeight);
            EditorGUI.LabelField(labelRect, property.displayName);

            Rect sizeFieldRect = new Rect(position.x + position.width - SizeFieldWidth, position.y, SizeFieldWidth, EditorGUIUtility.singleLineHeight);
            int newSize = EditorGUI.DelayedIntField(sizeFieldRect, itemsProperty.arraySize);
            
            if (newSize != itemsProperty.arraySize && newSize >= 0)
            {
                itemsProperty.arraySize = newSize;
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        private void EnsureReorderableListInitialized(SerializedProperty property, SerializedProperty itemsProperty)
        {
            if (reorderableList == null || reorderableList.serializedProperty.serializedObject != property.serializedObject)
            {
                CreateReorderableList(property, itemsProperty);
            }
        }

        private void CreateReorderableList(SerializedProperty property, SerializedProperty itemsProperty)
        {
            elementType = fieldInfo.FieldType.GetGenericArguments()[0];

            reorderableList = new ReorderableList(property.serializedObject, itemsProperty, true, false, true, true)
            {
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    DrawElement(rect, itemsProperty.GetArrayElementAtIndex(index));
                },

                elementHeightCallback = index => GetElementHeight(itemsProperty.GetArrayElementAtIndex(index)),

                onAddDropdownCallback = (rect, list) =>
                {
                    ShowTypeSelectionMenu(rect, list, property);
                }
            };
        }

        private void DrawElement(Rect rect, SerializedProperty element)
        {
            SerializedProperty unityReference = element.FindPropertyRelative("unityReference");
            SerializedProperty serializedReference = element.FindPropertyRelative("serializedReference");
            SerializedProperty currentType = element.FindPropertyRelative("currentType");

            Type storedType = !string.IsNullOrEmpty(currentType.stringValue) ? Type.GetType(currentType.stringValue) : null;
            if (storedType == null) return;

            rect.height = singleLineHeight;

            if (typeof(UnityEngine.Object).IsAssignableFrom(storedType))
            {
                unityReference.objectReferenceValue = EditorGUI.ObjectField(
                    rect,
                    new GUIContent($"{storedType.Name}"),
                    unityReference.objectReferenceValue,
                    storedType,
                    !EditorUtility.IsPersistent(unityReference.objectReferenceValue)
                );
            }
            else if (serializedReference.managedReferenceValue != null)
            {
                rect.x += XOffset;
                rect.width -= XOffset;
                EditorGUI.PropertyField(rect, serializedReference, new GUIContent($"{storedType.Name}"), true);
            }
        }

        private float GetElementHeight(SerializedProperty element)
        {
            SerializedProperty unityReference = element.FindPropertyRelative("unityReference");
            SerializedProperty serializedReference = element.FindPropertyRelative("serializedReference");
            SerializedProperty currentType = element.FindPropertyRelative("currentType");

            Type storedType = !string.IsNullOrEmpty(currentType.stringValue) ? Type.GetType(currentType.stringValue) : null;

            if (storedType != null)
            {
                if (typeof(UnityEngine.Object).IsAssignableFrom(storedType))
                {
                    return EditorGUI.GetPropertyHeight(unityReference, true);
                }
                else if (serializedReference.managedReferenceValue != null)
                {
                    return EditorGUI.GetPropertyHeight(serializedReference, true);
                }
            }

            return singleLineHeight + 4;
        }

        private void HandleDragAndDrop(Rect position, SerializedProperty property, SerializedProperty itemsProperty)
        {
            Event evt = Event.current;
            if ((evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform) && position.Contains(evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    ProcessDraggedObjects(itemsProperty, property);
                    property.serializedObject.ApplyModifiedProperties();
                    evt.Use();
                }
            }
        }

        private void ProcessDraggedObjects(SerializedProperty itemsProperty, SerializedProperty property)
        {
            foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
            {
                object instance = GetInstanceFromDraggedObject(draggedObject);
                if (instance == null) continue;

                itemsProperty.arraySize++;
                SerializedProperty newElement = itemsProperty.GetArrayElementAtIndex(itemsProperty.arraySize - 1);
                SerializedProperty unityReference = newElement.FindPropertyRelative("unityReference");
                SerializedProperty serializedReference = newElement.FindPropertyRelative("serializedReference");
                SerializedProperty currentType = newElement.FindPropertyRelative("currentType");

                currentType.stringValue = instance.GetType().AssemblyQualifiedName;

                if (instance is UnityEngine.Object unityObj)
                {
                    unityReference.objectReferenceValue = unityObj;
                }
                else
                {
                    serializedReference.managedReferenceValue = Activator.CreateInstance(instance.GetType());
                }

                property.serializedObject.ApplyModifiedProperties();
            }
        }

        private object GetInstanceFromDraggedObject(UnityEngine.Object draggedObject)
        {
            if (draggedObject is GameObject go)
            {
                return go.GetComponents<Component>().FirstOrDefault(component => elementType.IsAssignableFrom(component.GetType()));
            }

            if (elementType.IsAssignableFrom(draggedObject.GetType()))
            {
                return draggedObject;
            }

            if (draggedObject is MonoScript script)
            {
                Type scriptType = script.GetClass();
                if (scriptType != null && elementType.IsAssignableFrom(scriptType) && !typeof(UnityEngine.Object).IsAssignableFrom(scriptType))
                {
                    return Activator.CreateInstance(scriptType);
                }
            }

            return null;
        }

        private void ShowTypeSelectionMenu(Rect buttonRect, ReorderableList list, SerializedProperty property)
        {
            List<Type> validTypes = TypeFinder.GetDerivedTypesWithValidConstructors(elementType);
            if (validTypes.Count > 0)
            {
                TypeSearchDropdown dropdown = new TypeSearchDropdown(
                    new AdvancedDropdownState(),
                    validTypes,
                    selectedType => AddNewElementOfType(list, selectedType, property),
                    false, false
                );

                dropdown.Show(new Rect(Event.current.mousePosition, Vector2.zero));
            }
        }

        private void AddNewElementOfType(ReorderableList list, Type selectedType, SerializedProperty property)
        {
            if (selectedType == null)
            {
                Debug.LogError("Selected type is null.");
                return;
            }

            list.serializedProperty.arraySize++;
            property.serializedObject.ApplyModifiedProperties();

            int index = list.serializedProperty.arraySize - 1;
            SerializedProperty newElement = list.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty unityReference = newElement.FindPropertyRelative("unityReference");
            SerializedProperty serializedReference = newElement.FindPropertyRelative("serializedReference");
            SerializedProperty currentType = newElement.FindPropertyRelative("currentType");

            currentType.stringValue = selectedType.AssemblyQualifiedName;

            if (typeof(UnityEngine.Object).IsAssignableFrom(selectedType))
            {
                unityReference.objectReferenceValue = null;
            }
            else
            {
                serializedReference.managedReferenceValue = Activator.CreateInstance(selectedType);
                unityReference.objectReferenceValue = null;
            }

            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty itemsProperty = property.FindPropertyRelative("items");
            EnsureReorderableListInitialized(property, itemsProperty);
            return isExpanded ? reorderableList.GetHeight() + singleLineHeight : singleLineHeight;
        }
    }
}