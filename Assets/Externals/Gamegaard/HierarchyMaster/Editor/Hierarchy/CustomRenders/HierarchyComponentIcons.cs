using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gamegaard.HierarchyMaster.Editor
{
    public class HierarchyComponentIcons : HierarchyRendererBase
    {
        public override void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (obj == null) return;

            Component[] components = obj.GetComponents<Component>();
            if (components.Length <= 1) return;

            float iconSize = 16f;
            float padding = 2f;
            float xOffset = selectionRect.xMax - iconSize;

            List<Component> validComponents = new List<Component>();

            foreach (Component component in components)
            {
                if (component is not Transform) validComponents.Add(component);
            }

            Event e = Event.current;

            for (int i = validComponents.Count - 1; i >= 0; i--)
            {
                Component component = validComponents[i];
                Texture icon = EditorGUIUtility.ObjectContent(component, component.GetType()).image;
                if (icon == null) continue;

                Rect iconRect = new Rect(xOffset, selectionRect.y, iconSize, iconSize);
                GUI.DrawTexture(iconRect, icon);

                if (e.type == EventType.MouseDown && e.button == 0 && iconRect.Contains(e.mousePosition))
                {
                    EditorUtility.OpenPropertyEditor(component);
                    e.Use();
                }

                xOffset -= iconSize + padding;
            }
        }
    }
}