using UnityEngine;
using UnityEngine.Events;

public class ToolbarButtonValue
{
    [SerializeField] private string text;
    [SerializeField] private bool isEnabled = true;
    private UnityAction onClick;

    public string Text => text;
    public virtual UnityAction OnClick => onClick;
    public bool IsEnabled => isEnabled;

    public ToolbarButtonValue(string text, UnityAction onClick, bool isEnabled)
    {
        this.text = text;
        this.onClick = onClick;
        this.isEnabled = isEnabled;
    }
}