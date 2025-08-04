using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.HierarchyMaster.Editor
{
    public static class BuildProcessingUtils
    {
        public static void CollectEditorOnlyBehaviours(Transform root, List<Component> list)
        {
            foreach (Component c in root.GetComponents<Component>())
            {
                if (c == null)
                    continue;

                Type type = c.GetType();
                if (type.IsSubclassOf(typeof(EditorOnlyBehaviour)))
                {
                    list.Add(c);
                }
            }

            for (int i = 0; i < root.childCount; i++)
            {
                CollectEditorOnlyBehaviours(root.GetChild(i), list);
            }
        }

        internal static void StripFolder(HierarchyFolder folder, StrippingMode strippingMode)
        {
            switch (strippingMode)
            {
                case StrippingMode.Delete:
                    HierarchyFolderUtils.RemoveFolder(folder);
                    break;

                case StrippingMode.ReplaceWithSeparator:
                    string name = $"------{folder.gameObject.name.ToUpper()}------";
                    Transform newSeparator = new GameObject(name).transform;
                    newSeparator.parent = HierarchyFolderUtils.GetValidParent(folder.transform);
                    newSeparator.SetSiblingIndex(folder.transform.GetSiblingIndex());
                    HierarchyFolderUtils.RemoveFolder(folder);
                    break;
            }
        }
    }
}