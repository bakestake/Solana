using Gamegaard;
using TMPro;

public abstract class DropdownBehaviour : SingleComponentBehaviour<TMP_Dropdown>
{
    protected override void Awake()
    {
        base.Awake();
        targetComponent.onValueChanged.AddListener(OnValueChange);
    }

    public abstract void OnValueChange(int optionIndex);
}
