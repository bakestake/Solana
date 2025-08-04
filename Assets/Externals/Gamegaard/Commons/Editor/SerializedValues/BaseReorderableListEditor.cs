using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Gamegaard.CustomValues.Editor
{
    public abstract class BaseReorderableListEditor<T> : PropertyDrawer
    {
        private bool foldout = true;
        private ReorderableList list;

        protected abstract string GetHeaderName();
        protected abstract SerializedProperty GetElementsProperty(SerializedProperty property);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect foldoutRect = new Rect(position.x, position.y, position.width - 50, EditorGUIUtility.singleLineHeight);
            foldout = EditorGUI.Foldout(foldoutRect, foldout, label, true);

            if (foldout)
            {
                if (list == null)
                {
                    InitializeReorderableList(property);
                }

                EditorGUI.indentLevel++;
                Rect listPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, list.GetHeight());
                list.DoList(listPosition);
                EditorGUI.indentLevel--;
            }

            property.serializedObject.ApplyModifiedProperties();
        }

        private void InitializeReorderableList(SerializedProperty property)
        {
            SerializedProperty elementsProp = GetElementsProperty(property);
            list = new ReorderableList(property.serializedObject, elementsProp, true, true, true, true);

            list.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, GetHeaderName());
            };

            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                SerializedProperty element = elementsProp.GetArrayElementAtIndex(index);
                if (element == null) return;
                DrawElement(rect, element);
            };

            list.elementHeightCallback = (index) =>
            {
                SerializedProperty element = elementsProp.GetArrayElementAtIndex(index);
                if (element == null) return EditorGUIUtility.singleLineHeight;
                return EditorGUI.GetPropertyHeight(element, true);
            };

            list.onAddCallback = (ReorderableList l) =>
            {
                AddNewElement(elementsProp, property);
            };

            list.onRemoveCallback = (ReorderableList l) =>
            {
                if (l.index >= 0 && l.index < elementsProp.arraySize)
                {
                    elementsProp.DeleteArrayElementAtIndex(l.index);
                    property.serializedObject.ApplyModifiedProperties();
                }
            };
        }

        protected virtual void DrawElement(Rect rect, SerializedProperty element)
        {
            EditorGUI.PropertyField(rect, element, GUIContent.none, true);
        }

        protected abstract void AddNewElement(SerializedProperty elementsProp, SerializedProperty property);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return foldout && list != null
                ? EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + list.GetHeight()
                : EditorGUIUtility.singleLineHeight;
        }
    }
}