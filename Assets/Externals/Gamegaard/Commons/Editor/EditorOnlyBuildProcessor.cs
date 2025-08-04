using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gamegaard.SerializableAttributes
{
    public class EditorOnlyBuildProcessor : IProcessSceneWithReport
    {
        public int callbackOrder => 0;

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            if (report != null)
            {
#if UNITY_2023_3_OR_NEWER
                EditorOnlyBehaviour[] editorOnlyComponents = Object.FindObjectsByType<EditorOnlyBehaviour>(FindObjectsSortMode.None);
#else
                EditorOnlyBehaviour[] editorOnlyComponents = Object.FindObjectsOfType<EditorOnlyBehaviour>();
#endif
                foreach (EditorOnlyBehaviour component in editorOnlyComponents)
                {
                    Object.DestroyImmediate(component);
                }
            }
        }
    }
}
