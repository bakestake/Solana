using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gamegaard.HierarchyMaster.Editor
{
    static class HierarchyMasterSettingsProvider
    {
        private const string SETTINGS_PATH = "Project/Gamegaard/Hierarchy Master";

        private const string TITLE = "Hierarchy Master Settings";
        private const string STYLE_SECTION = "Hierarchy Style";
        private const string COMPONENT_ICONS_SECTION = "Component Icons";
        private const string FOLDER_SECTION = "Folder Renderer";

        private const string STYLE_DESCRIPTION = "Enables custom styles for objects in the Hierarchy.";
        private const string COMPONENT_ICONS_DESCRIPTION = "Displays component icons directly in the Hierarchy.";
        private const string FOLDER_DESCRIPTION = "Transforms marked objects into visual folders in the Hierarchy.";

        private const string STYLE_LABEL = "Enable Hierarchy Style Renderer";
        private const string COMPONENT_ICONS_LABEL = "Enable Hierarchy Component Icons";
        private const string FOLDER_LABEL = "Enable Hierarchy Folder Renderer";
        private const string FOLDERALERTS_LABEL = "Ignore Hierarchy Folder Warnings";
        private const string TOOLSLOCK_LABEL = "Enable Tools Lock";
        private const string SHOW_TRANSFORM_LABEL = "Show Transform Component";
        private const string SHOW_TRANSFORM_DESCRIPTION = "Displays the Transform component in the Inspector for Hierarchy Folders.";

        private const string FORCE_ZERO_LABEL = "Force Transform Reset to Zero";
        private const string FORCE_ZERO_DESCRIPTION = "Forces the folder's Transform to be reset to zero after interaction.";

        private const string EDITOR_STRIP_LABEL = "Editor Mode Folder Stripping";
        private const string BUILD_STRIP_LABEL = "Build Folder Stripping";

        private static readonly HashSet<string> keywords = new()
        {
            "Hierarchy", "Master", "Settings", "Icons", "Folders"
        };

        [SettingsProvider]
        public static SettingsProvider CreateHierarchyMasterSettingsProvider()
        {
            return new SettingsProvider(SETTINGS_PATH, SettingsScope.Project)
            {
                guiHandler = (searchContext) =>
                {
                    EditorGUI.BeginChangeCheck();
                    SerializedObject settings = new SerializedObject(HierarchyMasterSettings.Instance);

                    GUILayout.Space(10);
                    EditorGUILayout.LabelField(TITLE, EditorStyles.boldLabel);
                    GUILayout.Space(5);

                    DrawHierarchyStyleArea(settings);
                    GUILayout.Space(15);

                    DrawComponentIconsArea(settings);
                    GUILayout.Space(15);

                    DrawFolderArea(settings);
                    GUILayout.Space(20);

                    if (EditorGUI.EndChangeCheck())
                    {
                        ApplyChanges(settings);
                    }
                },

                keywords = keywords
            };
        }

        private static void DrawHierarchyStyleArea(SerializedObject settings)
        {
            EditorGUILayout.LabelField(STYLE_SECTION, EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(STYLE_DESCRIPTION, MessageType.Info);
            SerializedProperty enableStyleRenderer = settings.FindProperty("enableHierarchyStyleRenderer");
            EditorGUILayout.PropertyField(enableStyleRenderer, new GUIContent(STYLE_LABEL));
        }

        private static void DrawComponentIconsArea(SerializedObject settings)
        {
            EditorGUILayout.LabelField(COMPONENT_ICONS_SECTION, EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(COMPONENT_ICONS_DESCRIPTION, MessageType.Info);
            SerializedProperty enableComponentIcons = settings.FindProperty("enableHierarchyComponentIcons");
            EditorGUILayout.PropertyField(enableComponentIcons, new GUIContent(COMPONENT_ICONS_LABEL));
        }

        private static void DrawFolderArea(SerializedObject settings)
        {
            EditorGUILayout.LabelField(FOLDER_SECTION, EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(FOLDER_DESCRIPTION, MessageType.Info);

            SerializedProperty enableFolderRenderer = settings.FindProperty("enableHierarchyFolderRenderer");
            SerializedProperty ignoreAllFolderPrefabAlerts = settings.FindProperty("ignoreAllFolderPrefabAlerts");
            SerializedProperty enableToolsLock = settings.FindProperty("lockTools");
            SerializedProperty showTransformComponent = settings.FindProperty("showTransformComponent");
            SerializedProperty forceZeroTransform = settings.FindProperty("forceZeroTransform");
            SerializedProperty editorStrippingMode = settings.FindProperty("editorStrippingMode");
            SerializedProperty buildStrippingMode = settings.FindProperty("buildStrippingMode");

            EditorGUILayout.PropertyField(enableFolderRenderer, new GUIContent(FOLDER_LABEL));
            EditorGUILayout.PropertyField(ignoreAllFolderPrefabAlerts, new GUIContent(FOLDERALERTS_LABEL));
            EditorGUILayout.PropertyField(enableToolsLock, new GUIContent(TOOLSLOCK_LABEL));
            EditorGUILayout.PropertyField(showTransformComponent, new GUIContent(SHOW_TRANSFORM_LABEL, SHOW_TRANSFORM_DESCRIPTION));
            EditorGUILayout.PropertyField(forceZeroTransform, new GUIContent(FORCE_ZERO_LABEL, FORCE_ZERO_DESCRIPTION));

            GUILayout.Space(10);
            EditorGUILayout.PropertyField(editorStrippingMode, new GUIContent(EDITOR_STRIP_LABEL));
            EditorGUILayout.PropertyField(buildStrippingMode, new GUIContent(BUILD_STRIP_LABEL));
        }

        private static void ApplyChanges(SerializedObject settings)
        {
            settings.ApplyModifiedProperties();
            HierarchyMasterManager.ApplySettings();
            EditorApplication.RepaintHierarchyWindow();
        }
    }
}
