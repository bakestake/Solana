using System.IO;
using UnityEditor;
using UnityEngine;

namespace Gamegaard.SavingSystem.Editor
{
    public static class SaveSystemOptions
    {
        public static string SavePath => Path.Combine(Application.persistentDataPath, "SaveData");

        [MenuItem("Tools/SaveSystem/Open Save Folder")]
        public static void OpenSaveFolder()
        {
            EditorUtility.RevealInFinder(SavePath);
        }

        [MenuItem("Tools/SaveSystem/Delete All Saves")]
        public static void DeleteAllSaves()
        {
            if (Directory.Exists(Application.persistentDataPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath);
                directoryInfo.Delete(true);
                Debug.Log("All save files deleted!");
            }
            else
            {
                Debug.LogWarning("Save folder not found.");
            }
        }
    }
}