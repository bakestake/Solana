using UnityEngine;
using UnityEditor;
using TMPro;

public class TMPLoremIpsumWindow : EditorWindow
{
    private int charCount = 100;
    private TMP_Text textMeshPro;
    private bool isRandom = false;

    public static void ShowWindow(TMP_Text tmp)
    {
        TMPLoremIpsumWindow window = GetWindow<TMPLoremIpsumWindow>("Generate Lorem Ipsum");
        window.textMeshPro = tmp;
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate Lorem Ipsum", EditorStyles.boldLabel);

        charCount = EditorGUILayout.IntField("Number of Characters", charCount);
        isRandom = EditorGUILayout.Toggle("Random Start", isRandom);

        if (GUILayout.Button("Generate"))
        {
            if (textMeshPro != null)
            {
                LoremIpsumGenerator generator = new LoremIpsumGenerator();
                textMeshPro.text = generator.GenerateLoremIpsum(charCount, isRandom);
                Close();
            }
        }
    }
}