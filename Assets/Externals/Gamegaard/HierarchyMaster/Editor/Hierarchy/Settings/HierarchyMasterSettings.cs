using UnityEditor;
using UnityEngine;

namespace Gamegaard.HierarchyMaster.Editor
{
    public class HierarchyMasterSettings : ScriptableObject
    {
        [Header("Hierarchy Style")]
        [SerializeField] private bool enableHierarchyStyleRenderer = true;

        [Header("Component Icons")]
        [SerializeField] private bool enableHierarchyComponentIcons;

        [Header("Folders")]
        [SerializeField] private bool enableHierarchyFolderRenderer = true;
        [SerializeField] private bool ignoreAllFolderPrefabAlerts;
        [SerializeField] private bool lockTools;
        [SerializeField] private bool showTransformComponent;
        [SerializeField] private bool forceZeroTransform = true;

        [Header("Folder Stripping")]
        [SerializeField] private StrippingMode editorStrippingMode = StrippingMode.Delete;
        [SerializeField] private StrippingMode buildStrippingMode = StrippingMode.Delete;

        public bool EnableHierarchyStyleRenderer => enableHierarchyStyleRenderer;
        public bool EnableHierarchyComponentIcons => enableHierarchyComponentIcons;
        public bool EnableHierarchyFolderRenderer => enableHierarchyFolderRenderer;
        public bool IgnoreAllFolderPrefabAlerts => ignoreAllFolderPrefabAlerts;
        public bool LockTools => lockTools;
        public bool ShowTransformComponent => showTransformComponent;
        public bool ForceZeroTransform => forceZeroTransform;

        public StrippingMode EditorStrippingMode => editorStrippingMode;
        public StrippingMode BuildStrippingMode => buildStrippingMode;

        private static HierarchyMasterSettings instance;

        public static HierarchyMasterSettings Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                string[] guids = AssetDatabase.FindAssets("t:HierarchyMasterSettings");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    instance = AssetDatabase.LoadAssetAtPath<HierarchyMasterSettings>(path);
                }

                if (instance == null)
                {
                    instance = CreateInstance<HierarchyMasterSettings>();
                    const string defaultPath = "Assets/Settings/HierarchyMasterSettings.asset";
                    System.IO.Directory.CreateDirectory("Assets/Settings");
                    AssetDatabase.CreateAsset(instance, defaultPath);
                    AssetDatabase.SaveAssets();
                }

                return instance;
            }
        }
    }
}
