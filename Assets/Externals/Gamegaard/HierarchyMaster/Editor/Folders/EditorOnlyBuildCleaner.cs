using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gamegaard.HierarchyMaster.Editor
{
    [InitializeOnLoad]
    public static class EditorOnlyBuildCleaner
    {
        private static readonly Dictionary<GameObject, List<ComponentData>> prefabBackup = new();
        private static bool alreadyBuilding = false;

        static EditorOnlyBuildCleaner()
        {
            BuildPlayerWindow.RegisterBuildPlayerHandler(OnBuildPlayer);
        }

        private static void OnBuildPlayer(BuildPlayerOptions options)
        {
            if (alreadyBuilding)
                return;

            alreadyBuilding = true;

            try
            {
                FolderConstraintsContext.SetBuildSuppression(true);
                BackupAndCleanPrefabs();
                BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
            }
            finally
            {
                RestorePrefabs();
                FolderConstraintsContext.SetBuildSuppression(false);
            }
        }

        private static void BackupAndCleanPrefabs()
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab == null || prefabBackup.ContainsKey(prefab))
                    continue;

                Component[] components = prefab.GetComponentsInChildren<Component>(true);
                List<ComponentData> backupList = new();

                foreach (Component comp in components)
                {
                    if (comp == null)
                        continue;

                    Type type = comp.GetType();
                    if (type.IsSubclassOf(typeof(EditorOnlyBehaviour)))
                    {
                        backupList.Add(new ComponentData
                        {
                            gameObject = comp.gameObject,
                            type = type,
                            serialized = JsonUtility.ToJson(comp)
                        });

                        UnityEngine.Object.DestroyImmediate(comp, true);
                    }
                }

                if (backupList.Count > 0)
                {
                    prefabBackup[prefab] = backupList;
                    EditorUtility.SetDirty(prefab);
                }
            }

            AssetDatabase.SaveAssets();
        }

        private static void RestorePrefabs()
        {
            foreach (var kvp in prefabBackup)
            {
                GameObject prefab = kvp.Key;
                List<ComponentData> components = kvp.Value;

                foreach (ComponentData data in components)
                {
                    Component restored = data.gameObject.AddComponent(data.type);
                    JsonUtility.FromJsonOverwrite(data.serialized, restored);
                }

                EditorUtility.SetDirty(prefab);
            }

            AssetDatabase.SaveAssets();
            prefabBackup.Clear();
        }

        [Serializable]
        private class ComponentData
        {
            public GameObject gameObject;
            public Type type;
            public string serialized;
        }
    }
}