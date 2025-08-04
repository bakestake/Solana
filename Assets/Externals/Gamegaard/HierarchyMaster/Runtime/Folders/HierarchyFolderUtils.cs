using UnityEngine;
using System.Collections.Generic;

namespace Gamegaard.HierarchyMaster
{
    public static class HierarchyFolderUtils
    {
        private struct ChildInfo
        {
            public Transform transform;
            public Vector3 worldPosition;
            public Quaternion worldRotation;
            public int originalSiblingIndex;
        }

        internal static void RemoveFolder(HierarchyFolder folder)
        {
            ReinsertChildrenPreservingOrderAndWorldPosition(folder);
            Object.DestroyImmediate(folder.gameObject);
        }

        private static void ReinsertChildrenPreservingOrderAndWorldPosition(HierarchyFolder folder)
        {
            Transform baseFolder = GetTopmostHierarchyFolder(folder.transform);
            Transform parent = GetValidParent(baseFolder);
            int insertionIndex = baseFolder.GetSiblingIndex();

            List<ChildInfo> collected = new();

            CollectAllChildrenRecursive(baseFolder, collected);

            foreach (ChildInfo info in collected)
            {
                info.transform.SetParent(null, true);
            }

            for (int i = 0; i < collected.Count; i++)
            {
                Transform child = collected[i].transform;
                child.SetParent(parent, true);
                child.SetSiblingIndex(insertionIndex + i);
                child.position = collected[i].worldPosition;
                child.rotation = collected[i].worldRotation;
            }
        }

        private static void CollectAllChildrenRecursive(Transform root, List<ChildInfo> collected)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);

                if (child.GetComponent<HierarchyFolder>() != null)
                {
                    CollectAllChildrenRecursive(child, collected);
                }
                else
                {
                    collected.Add(new ChildInfo
                    {
                        transform = child,
                        worldPosition = child.position,
                        worldRotation = child.rotation,
                        originalSiblingIndex = child.GetSiblingIndex()
                    });
                }
            }
        }

        private static Transform GetTopmostHierarchyFolder(Transform current)
        {
            Transform top = current;
            while (top.parent != null && top.parent.GetComponent<HierarchyFolder>() != null)
            {
                top = top.parent;
            }
            return top;
        }

        public static Transform GetValidParent(Transform folderTransform)
        {
            Transform currentParent = folderTransform.parent;
            while (currentParent != null && currentParent.GetComponent<HierarchyFolder>() != null)
            {
                currentParent = currentParent.parent;
            }
            return currentParent;
        }
    }
}
