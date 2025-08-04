using UnityEngine;
using UnityEditor;

namespace Gamegaard.CustomValues.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxInt))]
    public class MinMaxIntDrawer : MinMaxDrawerBase<int>
    {
        protected override void DrawField(Rect rect, SerializedProperty property, string fieldName, string fieldLabel)
        {
            EditorGUI.LabelField(rect, new GUIContent(fieldLabel));
            EditorGUI.PropertyField(new Rect(rect.x + TextSize, rect.y, rect.width - LabelWidth, rect.height), property.FindPropertyRelative(fieldName), GUIContent.none);
        }
    }
}
