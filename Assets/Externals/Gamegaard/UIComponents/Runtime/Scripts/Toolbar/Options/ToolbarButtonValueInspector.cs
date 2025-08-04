using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ToolbarButtonValueInspector : ToolbarButtonValue
{
    [SerializeField] private UnityEvent onClickEvent;
    public override UnityAction OnClick => base.OnClick + onClickEvent.Invoke;

    public ToolbarButtonValueInspector(string text, UnityAction onClick, bool isEnabled) : base(text, onClick, isEnabled)
    {
    }
}