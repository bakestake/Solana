using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class LoremIpsumContextMenu
{
    [MenuItem("CONTEXT/TMP_Text/Generate Lorem Ipsum")]
    private static void GenerateLoremIpsumTMPROUGUI(MenuCommand command)
    {
        TMP_Text textMeshPro = (TMP_Text)command.context;
        TMPLoremIpsumWindow.ShowWindow(textMeshPro);
    }

    [MenuItem("CONTEXT/Text/Generate Lorem Ipsum")]
    private static void GenerateLoremIpsumText(MenuCommand command)
    {
        Text textMeshPro = (Text)command.context;
        TextLoremIpsumWindow.ShowWindow(textMeshPro);
    }
}