using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Gamegaard.SerializableAttributes.Editor
{
    [CustomPropertyDrawer(typeof(AttributesDictionary))]
    [CustomPropertyDrawer(typeof(AttributeDisplayerAttribute))]
    public class AttributeDisplayerDrawer : PropertyDrawer
    {
        private ReorderableList reorderableList;
        private bool initialized;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (!initialized)
            {
                Initialize(property, label);
            }

            reorderableList.DoList(position);

            EditorGUI.EndProperty();
        }

        private void Initialize(SerializedProperty property, GUIContent label)
        {
            initialized = true;
            reorderableList = SerializableEditorElements.CreateAttributesList(property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!initialized)
            {
                Initialize(property, label);
            }

            return reorderableList?.GetHeight() ?? EditorGUIUtility.singleLineHeight;
        }
    }
}
