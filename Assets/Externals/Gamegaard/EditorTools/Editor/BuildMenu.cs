using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using Unity.EditorCoroutines.Editor;
using System.IO;
using UnityEngine.SceneManagement;

public class BuildTools : EditorWindow
{
    private string customBuildPath = Path.Combine(Application.dataPath, "../Builds");
    private string buildVersion = "1.0.0";
    private string selectedPreset = "Custom";
    private const string presetsKey = "BuildToolPresets";
    private const string lastSelectedPresetKey = "BuildTool_LastSelectedPreset";
    private List<string> presetNames;
    private List<EditorBuildSettingsScene> scenesToBuild;
    private Dictionary<string, string> presets = new Dictionary<string, string>();
    private readonly Dictionary<BuildTarget, bool> targetsToBuild = new Dictionary<BuildTarget, bool>();
    private readonly List<BuildTarget> availableTargets = new List<BuildTarget>();

    [MenuItem("Tools/Build/Fast Build", true)]
    public static bool ShowBuildToolsValidate()
    {
        return !EditorApplication.isPlaying;
    }

    [MenuItem("Tools/Build/Fast Build")]
    public static void ShowBuildTools()
    {
        GetWindow<BuildTools>("Build Tools");
    }

    private void OnEnable()
    {
        InitializeAvailableTargets();
        SyncTargetsToBuild();
        LoadPresets();
        LoadScenesToBuild();

        if (EditorPrefs.HasKey(lastSelectedPresetKey))
        {
            selectedPreset = EditorPrefs.GetString(lastSelectedPresetKey);
            ApplyPreset(selectedPreset);
        }
        else
        {
            selectedPreset = "None";
            ApplyPreset(selectedPreset);
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Build Settings", EditorStyles.boldLabel);
        DrawPresetDropdown();
        DrawBuildVersion();
        DrawBuildPathButton();
        DrawBuildManagementButtons();
        GUILayout.Space(10);
        int numEnabled = DrawBuildTargets();
        GUILayout.Space(10);
        DrawSceneDisplayer();
        DrawBuildButton(numEnabled);
    }

    private void DrawPresetDropdown()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Preset", GUILayout.Width(50));
        int selectedIndex = presetNames.IndexOf(selectedPreset);
        int newIndex = EditorGUILayout.Popup(selectedIndex, presetNames.ToArray(), GUILayout.Width(150));

        if (newIndex != selectedIndex)
        {
            selectedPreset = presetNames[newIndex];
            EditorPrefs.SetString(lastSelectedPresetKey, selectedPreset);
            ApplyPreset(selectedPreset);
        }

        if (GUILayout.Button("New", GUILayout.Width(50)))
        {
            InputWindow.ShowWindow("New Preset Name", "Enter a name for the new preset:", "Preset Name", onInputSubmitted: (newPresetName) =>
            {
                if (!string.IsNullOrEmpty(newPresetName) && !presets.ContainsKey(newPresetName))
                {
                    SavePreset(newPresetName);
                    presetNames.Add(newPresetName);
                    selectedPreset = newPresetName;
                    EditorPrefs.SetString(lastSelectedPresetKey, selectedPreset);
                    Repaint();
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Name", "The preset name is either empty or already exists.", "OK");
                }
            });
        }

        if (selectedPreset != "PC" && selectedPreset != "Mobile" && selectedPreset != "All" && selectedPreset != "None")
        {
            if (GUILayout.Button("Update", GUILayout.Width(60)))
            {
                SavePreset(selectedPreset, false);
                EditorUtility.DisplayDialog("Preset Updated", $"Preset '{selectedPreset}' has been updated.", "OK");
            }

            if (GUILayout.Button("Delete", GUILayout.Width(60)))
            {
                DeletePreset(selectedPreset);
            }
        }

        GUILayout.EndHorizontal();
    }

    private void DrawBuildVersion()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Version", GUILayout.Width(50));
        string newVersion = EditorGUILayout.TextField(buildVersion, GUILayout.Width(70));

        if (newVersion != buildVersion)
        {
            buildVersion = newVersion;
            PlayerSettings.bundleVersion = buildVersion;
        }

        if (GUILayout.Button("+Major", GUILayout.Width(50))) IncrementVersion(0);
        if (GUILayout.Button("+Minor", GUILayout.Width(50))) IncrementVersion(1);
        if (GUILayout.Button("+Patch", GUILayout.Width(50))) IncrementVersion(2);
        GUILayout.EndHorizontal();
    }

    private void IncrementVersion(int part)
    {
        string[] versionParts = buildVersion.Split('.');
        if (versionParts.Length != 3) return;

        if (int.TryParse(versionParts[part], out int value))
        {
            value++;
            versionParts[part] = value.ToString();
            for (int i = part + 1; i < versionParts.Length; i++) versionParts[i] = "0";
            buildVersion = string.Join(".", versionParts);
            PlayerSettings.bundleVersion = buildVersion;
        }
    }

    private void DrawBuildPathButton()
    {
        GUILayout.BeginHorizontal();
        customBuildPath = EditorGUILayout.TextField("Build Path", customBuildPath);
        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", "", "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                customBuildPath = selectedPath;
            }
        }
        GUILayout.EndHorizontal();
    }

    private void DrawBuildManagementButtons()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Open Build Folder", GUILayout.Width(150)))
        {
            if (Directory.Exists(customBuildPath))
                EditorUtility.RevealInFinder(customBuildPath);
            else
                EditorUtility.DisplayDialog("Build Folder Not Found", "The specified build folder does not exist.", "OK");
        }
        if (GUILayout.Button("Delete Previous Builds", GUILayout.Width(150)))
        {
            if (Directory.Exists(customBuildPath))
            {
                Directory.Delete(customBuildPath, true);
                Directory.CreateDirectory(customBuildPath);
                EditorUtility.DisplayDialog("Success", "Previous builds deleted successfully.", "OK");
            }
            else
                EditorUtility.DisplayDialog("Build Folder Not Found", "The specified build folder does not exist.", "OK");
        }
        GUILayout.EndHorizontal();
    }

    private int DrawBuildTargets()
    {
        GUILayout.Label("Select Build Targets", EditorStyles.boldLabel);

        int numEnabled = 0;
        foreach (BuildTarget target in availableTargets)
        {
            string targetName = GetFriendlyTargetName(target);
            bool isSupported = BuildPipeline.IsBuildTargetSupported(GetTargetGroup(target), target);

            GUILayout.BeginHorizontal();
            GUI.enabled = isSupported;
            targetsToBuild[target] = EditorGUILayout.Toggle(targetName, targetsToBuild[target]);
            GUI.enabled = true;

            if (!isSupported)
            {
                GUILayout.Label("Module not installed", EditorStyles.miniLabel);
                if (GUILayout.Button("Open Unity Hub", EditorStyles.miniButton, GUILayout.Width(100)))
                {
                    OpenUnityHubForModuleInstallation(target);
                }
            }
            else if (targetsToBuild[target])
            {
                numEnabled++;
            }

            GUILayout.EndHorizontal();
        }

        return numEnabled;
    }

    private void DrawSceneDisplayer()
    {
        GUILayout.Label("Scenes in Build", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Drag scenes here to add them to the build", EditorStyles.centeredGreyMiniLabel);

        DisplayScenesInBuild();

        if (GUILayout.Button("Add Open Scenes"))
        {
            AddOpenScenes();
        }
        EditorGUILayout.EndVertical();

        if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
        {
            HandleDragAndDrop();
        }

        GUILayout.Space(10);
    }

    private void DrawBuildButton(int numEnabled)
    {
        GUI.enabled = numEnabled > 0 && !EditorApplication.isPlaying;
        string prompt = numEnabled == 1 ? "Build Selected Platform" : $"Build {numEnabled} Selected Platforms";
        if (GUILayout.Button(prompt))
        {
            List<BuildTarget> selectedTargets = GetSelectedTargets();
            EditorCoroutineUtility.StartCoroutine(BuildSelectedTargets(selectedTargets), this);
        }
        GUI.enabled = true;
    }

    private void DisplayScenesInBuild()
    {
        for (int i = 0; i < scenesToBuild.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            scenesToBuild[i].enabled = EditorGUILayout.Toggle(scenesToBuild[i].enabled, GUILayout.Width(20));
            GUILayout.Label(Path.GetFileNameWithoutExtension(scenesToBuild[i].path), GUILayout.Width(200));

            if (GUILayout.Button("▲", GUILayout.Width(25)) && i > 0)
            {
                SwapScenes(i, i - 1);
            }
            if (GUILayout.Button("▼", GUILayout.Width(25)) && i < scenesToBuild.Count - 1)
            {
                SwapScenes(i, i + 1);
            }

            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                RemoveSceneAt(i);
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void HandleDragAndDrop()
    {
        if (DragAndDrop.paths.Length > 0 && DragAndDrop.paths[0].EndsWith(".unity"))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (Event.current.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                foreach (string path in DragAndDrop.paths)
                {
                    AddSceneToBuild(path);
                }
            }
            Event.current.Use();
        }
    }

    private void AddSceneToBuild(string scenePath)
    {
        if (!scenesToBuild.Exists(s => s.path == scenePath))
        {
            scenesToBuild.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenesToBuild.ToArray();
        }
    }

    private void RemoveSceneAt(int index)
    {
        scenesToBuild.RemoveAt(index);
        EditorBuildSettings.scenes = scenesToBuild.ToArray();
    }

    private void AddOpenScenes()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.isLoaded && !string.IsNullOrEmpty(scene.path))
            {
                AddSceneToBuild(scene.path);
            }
        }
    }

    private void SwapScenes(int indexA, int indexB)
    {
        EditorBuildSettingsScene temp = scenesToBuild[indexA];
        scenesToBuild[indexA] = scenesToBuild[indexB];
        scenesToBuild[indexB] = temp;
        EditorBuildSettings.scenes = scenesToBuild.ToArray();
    }

    private void LoadPresets()
    {
        presetNames = new List<string> { "PC", "Mobile", "All", "None" };
        presets = new Dictionary<string, string>();

        if (!EditorPrefs.HasKey(presetsKey))
        {
            SetDefaultPreset("PC", new Dictionary<BuildTarget, bool>
            {
                { BuildTarget.StandaloneWindows, true },
                { BuildTarget.StandaloneOSX, true },
                { BuildTarget.StandaloneLinux64, true }
            });

            SetDefaultPreset("Mobile", new Dictionary<BuildTarget, bool>
            {
                { BuildTarget.Android, true },
                { BuildTarget.iOS, true }
            });

            Dictionary<BuildTarget, bool> allPlatforms = new Dictionary<BuildTarget, bool>();
            foreach (BuildTarget target in availableTargets)
            {
                allPlatforms[target] = true;
            }
            SetDefaultPreset("All", allPlatforms);

            SetDefaultPreset("None", new Dictionary<BuildTarget, bool>());

            EditorPrefs.SetString(presetsKey, string.Join(";", presetNames));
        }
        else
        {
            var savedPresetNames = EditorPrefs.GetString(presetsKey).Split(';');
            foreach (var presetName in savedPresetNames)
            {
                if (!string.IsNullOrEmpty(presetName) && !presetNames.Contains(presetName))
                {
                    presetNames.Add(presetName);
                }
            }
        }
    }

    private void SetDefaultPreset(string presetName, Dictionary<BuildTarget, bool> configurations)
    {
        foreach (BuildTarget target in availableTargets)
        {
            targetsToBuild[target] = configurations.ContainsKey(target) ? configurations[target] : false;
        }

        SavePreset(presetName, false);
    }

    private void SavePreset(string presetName, bool popupMessage = true)
    {
        List<string> presetData = new List<string>();
        foreach (var target in targetsToBuild)
        {
            presetData.Add($"{target.Key}:{target.Value}");
        }

        string presetString = string.Join(";", presetData);
        EditorPrefs.SetString(presetsKey + "_" + presetName, presetString);

        if (!presetNames.Contains(presetName))
        {
            presetNames.Add(presetName);
            EditorPrefs.SetString(presetsKey, string.Join(";", presetNames));
        }

        if (popupMessage)
        {
            EditorUtility.DisplayDialog("Preset Saved", $"Preset '{presetName}' has been saved successfully.", "OK");
        }
    }

    private void ApplyPreset(string presetName)
    {
        if (EditorPrefs.HasKey(presetsKey + "_" + presetName))
        {
            string presetString = EditorPrefs.GetString(presetsKey + "_" + presetName);
            if (string.IsNullOrEmpty(presetString))
            {
                Debug.LogWarning($"Preset '{presetName}' está vazio ou não contém dados válidos.");
                return;
            }

            foreach (var target in availableTargets)
            {
                targetsToBuild[target] = false;
            }

            string[] presetData = presetString.Split(';');
            foreach (string data in presetData)
            {
                string[] targetData = data.Split(':');
                if (targetData.Length == 2 && System.Enum.TryParse(targetData[0], out BuildTarget target))
                {
                    if (bool.TryParse(targetData[1], out bool isEnabled))
                    {
                        targetsToBuild[target] = isEnabled;
                    }
                }
            }

            selectedPreset = presetName;
            EditorPrefs.SetString(lastSelectedPresetKey, selectedPreset);

            Repaint();
        }
        else
        {
            Debug.LogWarning($"Preset '{presetName}' não foi encontrado.");
        }
    }

    private void DeletePreset(string presetName)
    {
        if (presetNames.Contains(presetName))
        {
            presetNames.Remove(presetName);
            EditorPrefs.DeleteKey(presetsKey + "_" + presetName);
            EditorPrefs.SetString(presetsKey, string.Join(";", presetNames));

            selectedPreset = "None";
            EditorPrefs.SetString(lastSelectedPresetKey, selectedPreset);
            ApplyPreset(selectedPreset);

            Repaint();
            EditorUtility.DisplayDialog("Preset Deleted", $"Preset '{presetName}' has been deleted.", "OK");
        }
    }

    private bool ExecuteBuildTarget(BuildTarget target)
    {
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            target = target,
            targetGroup = GetTargetGroup(target),
            locationPathName = GetBuildPath(target)
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build for {target} completed in {report.summary.totalTime.Seconds} seconds");
            return true;
        }

        Debug.LogError($"Build for {target} failed");
        return false;
    }

    private void InitializeAvailableTargets()
    {
        availableTargets.Clear();
        foreach (BuildTarget target in System.Enum.GetValues(typeof(BuildTarget)))
        {
            if (BuildPipeline.IsBuildTargetSupported(GetTargetGroup(target), target))
            {
                availableTargets.Add(target);
            }
        }
    }

    private void SyncTargetsToBuild()
    {
        foreach (BuildTarget target in availableTargets)
        {
            if (!targetsToBuild.ContainsKey(target))
                targetsToBuild[target] = false;
        }

        foreach (BuildTarget target in new List<BuildTarget>(targetsToBuild.Keys))
        {
            if (!availableTargets.Contains(target))
                targetsToBuild.Remove(target);
        }
    }

    private List<BuildTarget> GetSelectedTargets()
    {
        List<BuildTarget> selectedTargets = new List<BuildTarget>();
        foreach (BuildTarget target in availableTargets)
        {
            if (targetsToBuild[target]) selectedTargets.Add(target);
        }
        return selectedTargets;
    }

    private void LoadScenesToBuild()
    {
        scenesToBuild = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
    }

    private string[] GetEnabledScenes()
    {
        List<string> enabledScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in scenesToBuild)
        {
            if (scene.enabled)
                enabledScenes.Add(scene.path);
        }
        return enabledScenes.ToArray();
    }

    private string GetBuildPath(BuildTarget target)
    {
        string targetFolder = Path.Combine(customBuildPath, GetFriendlyTargetName(target));
        if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);

        return target switch
        {
            BuildTarget.Android => Path.Combine(targetFolder, $"{PlayerSettings.productName}.apk"),
            BuildTarget.StandaloneWindows64 => Path.Combine(targetFolder, $"{PlayerSettings.productName}.exe"),
            BuildTarget.StandaloneLinux64 => Path.Combine(targetFolder, $"{PlayerSettings.productName}.x86_64"),
            _ => Path.Combine(targetFolder, PlayerSettings.productName)
        };
    }

    private void ResetToOriginalTarget(BuildTarget originalTarget)
    {
        if (EditorUserBuildSettings.activeBuildTarget != originalTarget)
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(GetTargetGroup(originalTarget), originalTarget);
    }

    private BuildTargetGroup GetTargetGroup(BuildTarget target) => target switch
    {
        BuildTarget.StandaloneOSX => BuildTargetGroup.Standalone,
        BuildTarget.StandaloneWindows => BuildTargetGroup.Standalone,
        BuildTarget.iOS => BuildTargetGroup.iOS,
        BuildTarget.Android => BuildTargetGroup.Android,
        BuildTarget.StandaloneWindows64 => BuildTargetGroup.Standalone,
        BuildTarget.WebGL => BuildTargetGroup.WebGL,
        BuildTarget.StandaloneLinux64 => BuildTargetGroup.Standalone,
        _ => BuildTargetGroup.Unknown
    };

    private string GetFriendlyTargetName(BuildTarget target) => target switch
    {
        BuildTarget.StandaloneOSX => "Mac",
        BuildTarget.StandaloneWindows => "Windows",
        BuildTarget.iOS => "iOS",
        BuildTarget.Android => "Android",
        BuildTarget.StandaloneWindows64 => "Windows 64-bit",
        BuildTarget.WebGL => "WebGL",
        BuildTarget.StandaloneLinux64 => "Linux",
        _ => target.ToString()
    };

    private void OpenUnityHubForModuleInstallation(BuildTarget target)
    {
        EditorUtility.DisplayDialog("Module Required", $"Please install the {GetFriendlyTargetName(target)} module via Unity Hub.", "Open Unity Hub");
        Application.OpenURL("unityhub://");
    }

    private IEnumerator BuildSelectedTargets(List<BuildTarget> targetsToBuild)
    {
        int buildAllProgressID = Progress.Start("Build All", "Building all selected platforms", Progress.Options.Sticky);
        Progress.ShowDetails();
        yield return new EditorWaitForSeconds(1f);

        BuildTarget originalTarget = EditorUserBuildSettings.activeBuildTarget;

        for (int targetIndex = 0; targetIndex < targetsToBuild.Count; targetIndex++)
        {
            BuildTarget buildTarget = targetsToBuild[targetIndex];
            Progress.Report(buildAllProgressID, targetIndex + 1, targetsToBuild.Count);
            int buildTaskProgressID = Progress.Start($"Build {buildTarget}", null, Progress.Options.Sticky, buildAllProgressID);
            yield return new EditorWaitForSeconds(1f);

            if (!ExecuteBuildTarget(buildTarget))
            {
                Progress.Finish(buildTaskProgressID, Progress.Status.Failed);
                Progress.Finish(buildAllProgressID, Progress.Status.Failed);
                ResetToOriginalTarget(originalTarget);
                yield break;
            }

            Progress.Finish(buildTaskProgressID, Progress.Status.Succeeded);
            yield return new EditorWaitForSeconds(1f);
        }

        Progress.Finish(buildAllProgressID, Progress.Status.Succeeded);
        ResetToOriginalTarget(originalTarget);
        yield return null;
    }
}