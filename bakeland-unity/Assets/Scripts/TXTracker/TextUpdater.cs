using TMPro;
using UnityEngine;

public class TextUpdater : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textObject;

    public void SetText(string text)
    {
        textObject.text = text;
    }

    public void SetProgressPercent(float progress)
    {
        textObject.SetText($"{(progress * 100).ToString("F0")}%");
    }
}