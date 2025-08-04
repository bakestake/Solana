using UnityEngine;

[System.Serializable]
public class ToolbarTriggerValue
{
    [SerializeField] private string optionName;
    [SerializeField] private ToolbarButtonValueInspector[] options;

    public string OptionName => optionName;
    public ToolbarButtonValueInspector[] Options => options;
}