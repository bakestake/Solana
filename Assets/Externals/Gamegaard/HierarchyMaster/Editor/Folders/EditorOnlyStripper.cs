using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gamegaard.HierarchyMaster.Editor
{
    public class EditorOnlyStripper : IProcessSceneWithReport
    {
        public int callbackOrder => 0;

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            if (!scene.isLoaded)
                return;

            List<Component> toRemove = new List<Component>();
            foreach (GameObject rootObj in scene.GetRootGameObjects())
            {
                BuildProcessingUtils.CollectEditorOnlyBehaviours(rootObj.transform, toRemove);
            }

            foreach (Component c in toRemove)
            {
                if (c != null)
                    UnityEngine.Object.DestroyImmediate(c, true);
            }

            if (toRemove.Count > 0)
                Debug.Log($"[EditorOnlyStripper] Removed {toRemove.Count} editor-only components from scene '{scene.name}'.");
        }
    }
}