using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gamegaard.HierarchyMaster.Editor
{
    public static class FolderCustomOptions
    {
        private const int priority = 0;
        private static bool folderCreationScheduled;

        [MenuItem("GameObject/Create Folder", false, priority)]
        private static void CreateNewFolder()
        {
            GameObject[] selectedObjects = Selection.gameObjects;

            if (selectedObjects == null || selectedObjects.Length == 0)
            {
                GameObject newFolder = new GameObject("New Folder");
                Undo.RegisterCreatedObjectUndo(newFolder, "Create Folder");
                Undo.AddComponent<HierarchyFolder>(newFolder);
                Selection.activeGameObject = newFolder;
                return;
            }

            if (folderCreationScheduled)
                return;

            folderCreationScheduled = true;

            EditorApplication.delayCall += () =>
            {
                GameObject lastCreated = null;

                foreach (GameObject selected in selectedObjects)
                {
                    GameObject newFolder = new GameObject("New Folder");
                    Undo.RegisterCreatedObjectUndo(newFolder, "Create Folder");

                    Undo.SetTransformParent(newFolder.transform, selected.transform, "Parent Folder");
                    newFolder.transform.localPosition = Vector3.zero;

                    Undo.AddComponent<HierarchyFolder>(newFolder);
                    lastCreated = newFolder;
                }

                if (lastCreated != null)
                {
                    Selection.activeGameObject = lastCreated;
                }
                folderCreationScheduled = false;
            };
        }

        [MenuItem("GameObject/Create Parent Folder", false, priority)]
        private static void CreateNewFolderParent()
        {
            if (folderCreationScheduled)
                return;

            folderCreationScheduled = true;

            EditorApplication.delayCall += () =>
            {
                GameObject[] selectedObjects = Selection.gameObjects;
                if (selectedObjects.Length == 0) return;

                List<HierarchyFolder> modifiedFolders = new List<HierarchyFolder>();

                foreach (GameObject obj in selectedObjects)
                {
                    SetReparentingEnabledRecursively(obj.transform, false, modifiedFolders);
                }

                Transform parent = selectedObjects[0].transform.parent;
                int siblingIndex = selectedObjects[0].transform.GetSiblingIndex();

                GameObject folderObject = new GameObject("New Folder");
                Undo.RegisterCreatedObjectUndo(folderObject, "Create Folder");
                Undo.SetTransformParent(folderObject.transform, parent, "Set Folder Parent");
                folderObject.transform.SetSiblingIndex(siblingIndex);

                foreach (GameObject obj in selectedObjects)
                {
                    Undo.SetTransformParent(obj.transform, folderObject.transform, "Reparent to Folder");
                }

                Selection.activeGameObject = folderObject;
                Undo.AddComponent<HierarchyFolder>(folderObject);
                folderCreationScheduled = false;

                foreach (HierarchyFolder folder in modifiedFolders)
                {
                    folder.IsReparentingEnabled = true;
                }
            };
        }

        private static void SetReparentingEnabledRecursively(Transform root, bool enabled, List<HierarchyFolder> modified)
        {
            if (root.TryGetComponent(out HierarchyFolder folder) && folder.IsReparentingEnabled != enabled)
            {
                folder.IsReparentingEnabled = enabled;
                if (!modified.Contains(folder))
                    modified.Add(folder);
            }

            for (int i = 0; i < root.childCount; i++)
            {
                SetReparentingEnabledRecursively(root.GetChild(i), enabled, modified);
            }
        }

        [MenuItem("GameObject/Create Parent Folder", true)]
        private static bool ValidateCreateNewFolderParent()
        {
            GameObject[] selected = Selection.gameObjects;
            if (selected.Length == 0)
                return false;

            Transform commonParent = selected[0].transform.parent;

            for (int i = 1; i < selected.Length; i++)
            {
                if (selected[i].transform.parent != commonParent)
                    return false;
            }

            return true;
        }

        [MenuItem("CONTEXT/HierarchyFolder/Remove Component", false)]
        private static void RemoveComponent(MenuCommand command)
        {
            Debug.LogWarning("A folder component cannot be removed!");
        }

        [MenuItem("CONTEXT/HierarchyFolder/Remove Component", true)]
        private static bool RemoveComponent()
        {
            return false;
        }
    }
}