using System.IO;
using UnityEngine;
using UnityEditor;

public class ExportSubSprites : Editor
{
    [MenuItem("Assets/Export Sub‑Sprites")]
    static void Export()
    {
        string folder = EditorUtility.OpenFolderPanel("Export subsprites to folder", "", "");
        if (string.IsNullOrEmpty(folder)) return;

        foreach (var obj in Selection.objects)
        {
            if (obj is Sprite sprite)
            {
                // Extract the sprite's texture region
                var tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                var pixels = sprite.texture.GetPixels(
                    (int)sprite.rect.x, (int)sprite.rect.y,
                    (int)sprite.rect.width, (int)sprite.rect.height
                );
                tex.SetPixels(pixels);
                tex.Apply();

                // Save as PNG
                var path = Path.Combine(folder, sprite.name + ".png");
                File.WriteAllBytes(path, tex.EncodeToPNG());
            }
        }

        EditorUtility.RevealInFinder(folder);
    }

    [MenuItem("Assets/Export Sub‑Sprites", true)]
    static bool CanExport()
    {
        return Selection.objects.Length > 0 && Selection.objects[0] is Sprite;
    }
}
