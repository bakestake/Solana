using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class TextLoremIpsumWindow : EditorWindow
{
    private int charCount = 100;
    private bool isRandom = false;
    private Text text;

    public static void ShowWindow(Text tmp)
    {
        TextLoremIpsumWindow window = GetWindow<TextLoremIpsumWindow>("Generate Lorem Ipsum");
        window.text = tmp;
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate Lorem Ipsum", EditorStyles.boldLabel);

        charCount = EditorGUILayout.IntField("Number of Characters", charCount);
        isRandom = EditorGUILayout.Toggle("Random Start", isRandom);

        if (GUILayout.Button("Generate"))
        {
            if (text != null)
            {
                LoremIpsumGenerator generator = new LoremIpsumGenerator();
                text.text = generator.GenerateLoremIpsum(charCount, isRandom);
                Close();
            }
        }
    }
}
