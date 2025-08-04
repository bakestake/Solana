using Gamegaard;
using TMPro;

public abstract class TextFieldBehaviour : SingleComponentBehaviour<TMP_InputField>
{
    protected override void Awake()
    {
        base.Awake();
        targetComponent.onEndEdit.AddListener(ValidateValue);
    }

    protected abstract void ValidateValue(string text);
}