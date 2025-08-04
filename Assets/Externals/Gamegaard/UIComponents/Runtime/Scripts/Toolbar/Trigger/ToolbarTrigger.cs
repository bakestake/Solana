using TMPro;
using UnityEngine;

public abstract class ToolbarTrigger : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI text;
    [SerializeField] protected Toolbar toolbar;
    [SerializeField] protected ToolbarButtonValueInspector[] options;

    public void Initialize(string optionText, Toolbar toolbar, ToolbarButtonValueInspector[] options)
    {
        text.SetText(optionText);
        this.toolbar = toolbar;
        this.options = options;
    }

    public abstract void TriggerToolbar(bool selectFirstElement);

    public void CloseToolbar()
    {
        toolbar.CloseToolbar();
    }
}