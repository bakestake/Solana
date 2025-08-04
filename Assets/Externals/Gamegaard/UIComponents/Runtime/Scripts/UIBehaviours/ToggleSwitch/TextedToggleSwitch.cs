using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Slider))]
public class TextedToggleSwitch : ToggleSwitch
{
    [SerializeField] private TextMeshProUGUI currentStateText;
    [SerializeField] protected string enabledStateText;
    [SerializeField] protected string disabledStateText;

    protected override void SetStateAndStartAnimation(bool isEnabled)
    {
        base.SetStateAndStartAnimation(isEnabled);
        currentStateText.text = isEnabled ? enabledStateText : disabledStateText;
    }
}
