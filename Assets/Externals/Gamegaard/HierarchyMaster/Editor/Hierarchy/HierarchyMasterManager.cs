using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gamegaard.HierarchyMaster.Editor
{
    [InitializeOnLoad]
    public static class HierarchyMasterManager
    {
        private static readonly List<HierarchyRendererBase> renderers = new List<HierarchyRendererBase>();
        private static readonly List<IHierarchyIconProvider> iconProviders = new List<IHierarchyIconProvider>();

        static HierarchyMasterManager()
        {
            ApplySettings();
            EditorApplication.projectChanged += ApplySettings;
        }

        public static void ApplySettings()
        {
            HierarchyMasterSettings settings = HierarchyMasterSettings.Instance;
            List<HierarchyRendererBase> newRenderers = new List<HierarchyRendererBase>();

            if (settings.EnableHierarchyStyleRenderer)
                newRenderers.Add(new HierarchyStyleRenderer());

            if (settings.EnableHierarchyComponentIcons)
                newRenderers.Add(new HierarchyComponentIcons());

            if (settings.EnableHierarchyFolderRenderer)
                newRenderers.Add(new HierarchyFolderRenderer());

            renderers.Clear();
            renderers.AddRange(newRenderers);

            iconProviders.Clear();
            foreach (var renderer in renderers.OfType<IHierarchyIconProvider>())
            {
                iconProviders.Add(renderer);
            }

            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;

            EditorApplication.RepaintHierarchyWindow();
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject == null) return;

            HierarchyItem item = new HierarchyItem(instanceID, selectionRect, null);

            bool backgroundAlreadyDrawn = false;

            foreach (HierarchyRendererBase renderer in renderers)
            {
                if (renderer is IHierarchyBackgroundProvider backgroundProvider && backgroundProvider.ProvidesBackground(gameObject))
                {
                    backgroundAlreadyDrawn = true;
                }

                renderer.OnHierarchyGUI(instanceID, selectionRect);
            }

            DrawCorrectIcon(item, backgroundAlreadyDrawn);
        }

        private static void DrawCorrectIcon(HierarchyItem item, bool backgroundAlreadyDrawn)
        {
            GUIContent icon = GetIconForObject(item.GameObject, out bool useBackground);
            if (icon != null && icon.image != null)
            {
                Rect iconRect = item.PrefabIconRect;

                if (useBackground && !backgroundAlreadyDrawn)
                {
                    Color backgroundColor = GetHierarchyBackgroundColor(item);
                    EditorGUI.DrawRect(iconRect, backgroundColor);
                }

                GUI.DrawTexture(iconRect, icon.image, ScaleMode.ScaleToFit, true);
            }
        }

        private static Color GetHierarchyBackgroundColor(HierarchyItem item)
        {
            bool isWindowFocused = UnityThemeUtils.IsHierarchyFocused;

            return UnityThemeUtils.GetDefaultBackgroundColor(isWindowFocused, item.IsSelectedAndValid, item.IsHovered);
        }

        private static GUIContent GetIconForObject(GameObject gameObject, out bool useBackground)
        {
            GUIContent selectedIcon = null;
            int highestPriority = int.MinValue;
            useBackground = false;

            foreach (var provider in iconProviders)
            {
                GUIContent icon = provider.GetHierarchyIcon(gameObject);
                if (icon != null && provider.Priority > highestPriority)
                {
                    selectedIcon = icon;
                    highestPriority = provider.Priority;
                    useBackground = provider.UseBackground;
                }
            }

            return selectedIcon;
        }
    }
}