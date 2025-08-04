using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Bakeland.Editor
{
    public static class ItemUIDExporter
    {
        [MenuItem("Tools/Export/Export Item UIDs")]
        public static void ExportItemUIDsToTextFile()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(Item)}");

            if (guids.Length == 0)
            {
                Debug.LogWarning("No items found to export.");
                return;
            }

            List<Item> items = new List<Item>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Item item = AssetDatabase.LoadAssetAtPath<Item>(path);
                if (item != null)
                {
                    items.Add(item);
                }
            }

            var mintableItems = items
                .Where(i => i.IsMintable)
                .OrderBy(i => i.UniqueID)
                .ToList();

            var nonMintableItems = items
                .Where(i => !i.IsMintable)
                .OrderBy(i => i.UniqueID)
                .ToList();

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("--- Mintable Items ---");
            foreach (var item in mintableItems)
            {
                builder.AppendLine($"{item.ItemName} - UID: {item.UniqueID}");
            }

            builder.AppendLine();
            builder.AppendLine("--- Non-Mintable Items ---");
            foreach (var item in nonMintableItems)
            {
                builder.AppendLine($"{item.ItemName} - UID: {item.UniqueID}");
            }

            string directory = EditorUtility.SaveFolderPanel("Select Output Folder", Application.dataPath, "");
            if (string.IsNullOrEmpty(directory)) return;

            string filePath = Path.Combine(directory, "ItemUIDs.txt");
            File.WriteAllText(filePath, builder.ToString());

            Debug.Log($"Item UID list exported to:\n{filePath}");
        }
    }
}
