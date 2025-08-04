using UnityEditor;
using UnityEngine;
using Unity.EditorCoroutines.Editor;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

public class ObjectsRenamerWindow : EditorWindow
{
    private string renamePattern = "Object_{x:00}";
    private int startNumber = 1;
    private int progressId;

    [MenuItem("Tools/Utils/Object Renamer")]
    public static void ShowWindow()
    {
        ObjectsRenamerWindow window = GetWindow<ObjectsRenamerWindow>("Object Renamer");
        window.minSize = new Vector2(500, 100);
        window.maxSize = new Vector2(500, 100);
    }

    private void OnGUI()
    {
        GUILayout.Label("Rename Selected Objects and Files", EditorStyles.boldLabel);

        renamePattern = EditorGUILayout.TextField("Rename Pattern", renamePattern);
        startNumber = EditorGUILayout.IntField("Start Number", startNumber);

        if (GUILayout.Button("Rename"))
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(RenameSelectedObjectsOrFiles());
        }
    }

    private IEnumerator RenameSelectedObjectsOrFiles()
    {
        Object[] selectedObjects = Selection.objects;
        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("No objects or files selected to rename.");
            yield break;
        }

        progressId = Progress.Start("Renaming Objects", "Renaming selected objects...", Progress.Options.Sticky);

        bool containsGameObject = false;
        bool containsAsset = false;

        foreach (Object obj in selectedObjects)
        {
            if (obj is GameObject) containsGameObject = true;
            else if (AssetDatabase.Contains(obj)) containsAsset = true;
        }

        if (containsGameObject && !containsAsset)
        {
            yield return RenameGameObjects(selectedObjects.Length);
        }
        else if (containsAsset && !containsGameObject)
        {
            yield return RenameAssets(selectedObjects.Length);
        }
        else
        {
            Debug.LogWarning("Please select only either scene objects or files, not both.");
        }

        Progress.Finish(progressId); // Finaliza o processo após a conclusão
    }

    private IEnumerator RenameGameObjects(int totalCount)
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        for (int i = 0; i < selectedObjects.Length; i++)
        {
            GameObject go = selectedObjects[i];
            string newName = ProcessPattern(renamePattern, startNumber + i);

            Undo.RecordObject(go, "Rename GameObject");
            go.name = newName;

            Progress.Report(progressId, (float)(i + 1) / totalCount, $"Renaming GameObject {i + 1} of {totalCount}");
            yield return null;
        }

        Debug.Log($"Renamed {selectedObjects.Length} GameObjects.");
    }

    private IEnumerator RenameAssets(int totalCount)
    {
        Object[] selectedObjects = Selection.objects;

        for (int i = 0; i < selectedObjects.Length; i++)
        {
            Object obj = selectedObjects[i];
            if (AssetDatabase.Contains(obj))
            {
                string path = AssetDatabase.GetAssetPath(obj);
                string newName = ProcessPattern(renamePattern, startNumber + i);
                RenameAsset(path, newName);
            }

            Progress.Report(progressId, (float)(i + 1) / totalCount, $"Renaming Asset {i + 1} of {totalCount}");
            yield return null;
        }

        Debug.Log($"Renamed {selectedObjects.Length} assets.");
    }

    private string ProcessPattern(string pattern, int number)
    {
        Match match = Regex.Match(pattern, @"\{x:([^\}]+)\}");
        if (match.Success)
        {
            string format = match.Groups[1].Value;
            string formattedNumber = number.ToString(format);
            return pattern.Replace(match.Value, formattedNumber);
        }

        return pattern.Replace("{x}", number.ToString());
    }

    private void RenameAsset(string path, string newName)
    {
        if (AssetDatabase.IsValidFolder(path) || File.Exists(path))
        {
            AssetDatabase.RenameAsset(path, newName);
        }

        AssetDatabase.SaveAssets();
    }
}
