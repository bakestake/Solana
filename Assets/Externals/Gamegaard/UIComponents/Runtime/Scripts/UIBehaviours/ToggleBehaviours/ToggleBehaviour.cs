using Gamegaard;
using UnityEngine;
using UnityEngine.UI;

public abstract class ToggleBehaviour : SingleComponentBehaviour<Toggle>
{
    [SerializeField] private bool useInitialToggleState;

    protected override void Awake()
    {
        base.Awake();
        targetComponent.onValueChanged.AddListener(OnValueChange);
        if (useInitialToggleState)
        {
            OnValueChange(targetComponent.isOn);
        }
    }

    public abstract void OnValueChange(bool isEnabled);
}