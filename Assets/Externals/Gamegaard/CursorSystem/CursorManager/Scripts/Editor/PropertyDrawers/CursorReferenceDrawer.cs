using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gamegaard.CursorSystem.Editor
{
    [CanEditMultipleObjects]
    [CustomPropertyDrawer(typeof(CursorReference))]
    public class CursorReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty cursorProperty = property.FindPropertyRelative("cursor");
            SerializedProperty stateNameProperty = property.FindPropertyRelative("stateName");

            position.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(position, cursorProperty, new GUIContent("Cursor"));
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (cursorProperty.objectReferenceValue is CursorData2D<Sprite> spriteCursor)
            {
                RenderStateDropdown(position, spriteCursor, stateNameProperty);
            }
            else if (cursorProperty.objectReferenceValue is CursorData2D<Texture2D> textureCursor)
            {
                RenderStateDropdown(position, textureCursor, stateNameProperty);
            }
            else if (cursorProperty.objectReferenceValue != null)
            {
                EditorGUI.HelpBox(position, "Assigned object is not a valid CursorData2D.", MessageType.Warning);
                stateNameProperty.stringValue = "Default";
            }
            else
            {
                EditorGUI.HelpBox(position, "Assign a Cursor to select a State.", MessageType.Warning);
                stateNameProperty.stringValue = "Default";
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty cursorProperty = property.FindPropertyRelative("cursor");
            return cursorProperty.objectReferenceValue != null
                ? (EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing)
                : (EditorGUIUtility.singleLineHeight * 2);
        }

        private void RenderStateDropdown<T>(Rect position, CursorData2D<T> cursor, SerializedProperty stateNameProperty)
        {
            string[] stateNames = GetStateNames(cursor);
            int currentIndex = Mathf.Max(0, System.Array.IndexOf(stateNames, stateNameProperty.stringValue));

            int selectedIndex = EditorGUI.Popup(
                position,
                new GUIContent("State Name"),
                currentIndex,
                stateNames.Select(name => new GUIContent(name)).ToArray()
            );

            if (selectedIndex != currentIndex)
            {
                stateNameProperty.stringValue = stateNames[selectedIndex];
            }
        }

        private string[] GetStateNames<T>(CursorData2D<T> cursor)
        {
            List<string> stateNames = new List<string> { cursor.DefaultState.Name };

            if (cursor.CustomStates != null)
            {
                foreach (var state in cursor.CustomStates)
                {
                    if (!string.IsNullOrEmpty(state.Name))
                    {
                        stateNames.Add(state.Name);
                    }
                }
            }

            return stateNames.ToArray();
        }
    }
}
