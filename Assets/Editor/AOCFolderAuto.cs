using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public static class AOCFolderAuto
{
    private const string MENU_PATH = "Assets/Animator Override Controller/Auto‑Assign from Folder";

    [MenuItem(MENU_PATH)]
    static void AutoAssignOverrides()
    {
        var selected = Selection.objects
            .OfType<AnimatorOverrideController>()
            .ToArray();

        if (selected.Length == 0)
        {
            Debug.LogError("Select at least one AnimatorOverrideController asset.");
            return;
        }

        int totalAssigned = 0;

        foreach (var aoc in selected)
        {
            string aocPath = AssetDatabase.GetAssetPath(aoc);
            string folder = Path.GetDirectoryName(aocPath);

            var anims = AssetDatabase.FindAssets("t:AnimationClip", new[] { folder })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<AnimationClip>)
                .ToArray();

            if (anims.Length == 0)
            {
                Debug.LogWarning($"No AnimationClips found in folder '{folder}' for '{aoc.name}'.");
                continue;
            }

            var baseController = aoc.runtimeAnimatorController as AnimatorController;
            if (baseController == null)
            {
                Debug.LogError($"'{aoc.name}' has no valid base AnimatorController.");
                continue;
            }

            var overridesList = new AnimationClipOverrides(aoc.overridesCount);
            aoc.GetOverrides(overridesList);

            int assignedForThis = 0;
            foreach (var clip in anims)
            {
                if (overridesList.ContainsClipName(clip.name))
                {
                    overridesList[clip.name] = clip;
                    assignedForThis++;
                }
            }

            if (assignedForThis > 0)
            {
                aoc.ApplyOverrides(overridesList);
                EditorUtility.SetDirty(aoc);
                totalAssigned += assignedForThis;
                Debug.Log($"[{aoc.name}] Assigned {assignedForThis} overrides from folder '{folder}'.");
            }
            else
            {
                Debug.Log($"[{aoc.name}] No matching clip names found in folder '{folder}'.");
            }
        }

        if (totalAssigned > 0)
            AssetDatabase.SaveAssets();

        Debug.Log($"Auto-Assign complete: total overrides assigned = {totalAssigned}.");
    }

    [MenuItem(MENU_PATH, true)]
    static bool ValidateAutoAssign()
    {
        return Selection.objects.OfType<AnimatorOverrideController>().Any();
    }

    // Enhanced name-based override list
    class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
    {
        public AnimationClipOverrides(int capacity) : base(capacity) { }

        public bool ContainsClipName(string name)
        {
            return Find(pair => pair.Key.name == name).Key != null;
        }

        public AnimationClip this[string name]
        {
            get
            {
                var pair = Find(p => p.Key.name == name);
                return pair.Equals(default(KeyValuePair<AnimationClip, AnimationClip>))
                    ? null
                    : pair.Value;
            }
            set
            {
                int i = FindIndex(p => p.Key.name == name);
                if (i != -1)
                    this[i] = new KeyValuePair<AnimationClip, AnimationClip>(this[i].Key, value);
            }
        }
    }
}
