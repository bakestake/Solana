using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffUI : MonoBehaviour
{
    public Image icon;        // The icon representing the buff
    public TextMeshProUGUI durationText; // The text showing the remaining duration
    public Buff buff;

    // Initialize the UI with the buff's icon and duration
    public void Initialize(BuffInstance buffInstance)
    {
        icon.sprite = buffInstance.buff.icon;
        buff = buffInstance.buff;
        SetDuration(buffInstance.GetRemainingTime());
    }

    // Update the remaining time in the UI
    public void SetDuration(float remainingDuration)
    {
        durationText.text = remainingDuration.ToString("F1");  // Display remaining time in seconds
    }
}
