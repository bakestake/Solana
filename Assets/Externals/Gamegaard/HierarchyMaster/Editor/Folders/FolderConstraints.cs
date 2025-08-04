using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Gamegaard.HierarchyMaster.Editor
{
    [InitializeOnLoad]
    public static class FolderConstraints
    {
        static FolderConstraints()
        {
            PrefabUtility.prefabInstanceUpdated += CheckPrefabConversion;
            ObjectFactory.componentWasAdded += OnComponentAdded;
        }

        private static void CheckPrefabConversion(GameObject instance)
        {
            bool proceed;
            if (!FolderConstraintsContext.SuppressPrefabAlerts)
            {
                if (!instance.TryGetComponent(out HierarchyFolder _))
                    return;

                if (!PrefabUtility.IsPartOfPrefabInstance(instance))
                    return;

                if (PrefabUtility.IsPartOfPrefabAsset(instance))
                    return;

                if (PrefabStageUtility.GetCurrentPrefabStage() != null)
                    return;

                if (!instance.scene.IsValid())
                    return;

                proceed = EditorUtility.DisplayDialog(
                   "Convert Folder to Prefab?",
                   "This object is a HierarchyFolder and is intended only as an organizational container in the scene.\n\n" +
                   "Using it as a prefab is not recommended, since it will behave as a normal GameObject in runtime builds.\n\n" +
                   "Do you want to keep it as a prefab anyway?",
                   "Yes, keep as prefab",
                   "No, unpack and delete prefab"
               );
            }
            else
            {
                proceed = true;
            }

            if (!proceed)
            {
                GameObject parentPrefab = PrefabUtility.GetCorrespondingObjectFromSource(instance);

                if (parentPrefab != null)
                {
                    string prefabPath = AssetDatabase.GetAssetPath(parentPrefab);
                    AssetDatabase.DeleteAsset(prefabPath);
                }

                PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
                EditorSceneManager.MarkSceneDirty(instance.scene);
            }
        }

        private static void OnComponentAdded(Component component)
        {
            if (!component.TryGetComponent(out HierarchyFolder folder))
                return;

            if (component is not EditorOnlyBehaviour)
            {
                Object.DestroyImmediate(component, true);

                if (FolderConstraintsContext.SuppressPrefabAlerts)
                    return;

                Debug.LogWarning("Only Editor-only components are allowed on Hierarchy Folders. This component was removed.");
                return;
            }

            if (PrefabUtility.IsPartOfPrefabAsset(component.gameObject))
            {
                if (!FolderConstraintsContext.SuppressPrefabAlerts)
                {
                    bool proceed = EditorUtility.DisplayDialog(
                        "Add HierarchyFolder to Prefab?",
                        "You're adding a HierarchyFolder to a prefab asset.\n\n" +
                        "This component is intended only as a container in the editor and will not be present at runtime.\n\n" +
                        "Do you still want to keep it?",
                        "Yes, keep",
                        "No, remove"
                    );

                    if (!proceed)
                    {
                        Object.DestroyImmediate(component,true);
                        return;
                    }
                }
                else
                {
                    Object.DestroyImmediate(component, true);
                    return;
                }
            }
        }
    }
}