using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Gamegaard.HierarchyMaster.Editor
{
    public class HierarchyFolderRenderer : HierarchyRendererBase, IHierarchyIconProvider
    {
        private static readonly GUIContent folderClosedIcon = EditorGUIUtility.IconContent("Folder Icon");
        private static readonly GUIContent folderOpenedIcon = EditorGUIUtility.IconContent("FolderOpened Icon");

        private static readonly Type SceneHierarchyWindowType = Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor");
        private static readonly Type SceneHierarchyType = Type.GetType("UnityEditor.SceneHierarchy,UnityEditor");
        private static readonly Type TreeViewControllerType = Type.GetType("UnityEditor.IMGUI.Controls.TreeViewController,UnityEditor");
        private static readonly Type TreeViewDataSourceType = Type.GetType("UnityEditor.IMGUI.Controls.ITreeViewDataSource,UnityEditor");

        private static readonly MethodInfo GetAllHierarchyWindowsMethod = SceneHierarchyWindowType?.GetMethod("GetAllSceneHierarchyWindows", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        private static readonly PropertyInfo SceneHierarchyProperty = SceneHierarchyWindowType?.GetProperty("sceneHierarchy", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        private static readonly PropertyInfo TreeViewProperty = SceneHierarchyType?.GetProperty("treeView", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        private static readonly PropertyInfo DataProperty = TreeViewControllerType?.GetProperty("data", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        private static readonly MethodInfo GetRowsMethod = TreeViewDataSourceType?.GetMethod("GetRows", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        private static readonly MethodInfo IsExpandedMethod = TreeViewDataSourceType?.GetMethod("IsExpanded", new[] { typeof(TreeViewItem) });

        public int Priority => 99;
        public bool UseBackground => true;

        public override void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {

        }

        public GUIContent GetHierarchyIcon(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out HierarchyFolder folder))
            {
                bool isExpanded = IsExpanded(gameObject);

                if (folder.UseCustomIcons)
                {
                    Texture2D icon = isExpanded ? folder.CustomOpenFolderIcon : folder.CustomClosedFolderIcon;

                    if (icon == null)
                    {
                        icon = folder.CustomOpenFolderIcon ?? folder.CustomClosedFolderIcon;
                    }

                    if (icon != null)
                    {
                        return new GUIContent(icon);
                    }
                }

                return isExpanded ? folderOpenedIcon : folderClosedIcon;
            }
            return null;
        }

        private static bool IsExpanded(GameObject gameObject)
        {
            if (gameObject == null || gameObject.transform.childCount == 0)
                return false;

            if (GetAllHierarchyWindowsMethod == null || SceneHierarchyProperty == null || TreeViewProperty == null || DataProperty == null || GetRowsMethod == null || IsExpandedMethod == null)
                return false;

            IList windows = (IList)GetAllHierarchyWindowsMethod.Invoke(null, null);
            foreach (object window in windows)
            {
                object sceneHierarchy = SceneHierarchyProperty.GetValue(window);
                object treeView = TreeViewProperty.GetValue(sceneHierarchy);
                object data = DataProperty.GetValue(treeView);

                IList<TreeViewItem> rows = (IList<TreeViewItem>)GetRowsMethod.Invoke(data, null);
                foreach (TreeViewItem item in rows)
                {
                    if (item.id == gameObject.GetInstanceID())
                    {
                        return (bool)IsExpandedMethod.Invoke(data, new object[] { item });
                    }
                }
            }

            return false;
        }
    }
}