#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Gamegaard.HierarchyMaster.Editor
{
    public class FolderProcessor : IProcessSceneWithReport
    {
        public int callbackOrder => 0;

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            ProcessMode processMode = report == null ? ProcessMode.PlayMode : ProcessMode.Build;
            StrippingMode strippingMode = processMode == ProcessMode.PlayMode ? HierarchyMasterSettings.Instance.EditorStrippingMode : HierarchyMasterSettings.Instance.BuildStrippingMode;

#if UNITY_2023_3_OR_NEWER
            HierarchyFolder[] folderObjects = Object.FindObjectsByType<HierarchyFolder>(FindObjectsSortMode.None);
#else
            HierarchyFolder[] folderObjects = Object.FindObjectsOfType<HierarchyFolder>();
#endif
            List<HierarchyFolder> foldersCopy = new List<HierarchyFolder>(folderObjects);

            foreach (HierarchyFolder folder in foldersCopy)
            {
                if (folder != null && folder.gameObject != null)
                {
                    BuildProcessingUtils.StripFolder(folder, strippingMode);
                }
            }

            DDOLConsoleInterceptor.ApplyPending();
        }
    }

    [InitializeOnLoad]
    public static class DDOLConsoleInterceptor
    {
        private const string TargetMessage = "DontDestroyOnLoad only works for root GameObjects or components on root GameObjects.";
        private static readonly List<GameObject> intercepted = new();

        static DDOLConsoleInterceptor()
        {
            Application.logMessageReceived += OnLogMessage;
        }

        private static void OnLogMessage(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Warning || !condition.StartsWith(TargetMessage))
                return;

            GameObject go = TryFindGameObjectFromStack(stackTrace);
            if (go == null) return;

            if (IsDirectChildOfHierarchyFolder(go.transform))
            {
                if (!intercepted.Contains(go))
                    intercepted.Add(go);
            }
        }

        private static GameObject TryFindGameObjectFromStack(string stackTrace)
        {
            Match match = Regex.Match(stackTrace, @"(?<method>[\w\.:]+)\s*\(\)\s*\(at\s(?<file>.+):(?<line>\d+)\)");
            if (!match.Success) return null;

            string method = match.Groups["method"].Value;

#if UNITY_2023_3_OR_NEWER
            GameObject[] objects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
#else
            GameObject[] objects = Object.FindObjectsOfType<GameObject>();
#endif

            foreach (GameObject go in objects)
            {
                Component[] components = go.GetComponents<Component>();
                foreach (Component comp in components)
                {
                    if (comp == null) continue;
                    if (method.StartsWith(comp.GetType().FullName))
                        return go;
                }
            }

            return null;
        }

        private static bool IsDirectChildOfHierarchyFolder(Transform t)
        {
            return t.parent != null && t.parent.GetComponent<HierarchyFolder>() != null;
        }

        public static void ApplyPending()
        {
            for (int i = intercepted.Count - 1; i >= 0; i--)
            {
                GameObject go = intercepted[i];
                if (go == null) continue;

                if (go.transform.parent == null)
                {
                    Object.DontDestroyOnLoad(go);
                    intercepted.RemoveAt(i);
                }
            }

            intercepted.Clear();
        }
    }
}
#endif