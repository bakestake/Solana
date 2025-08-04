using Gamegaard;
using UnityEngine;
using UnityEngine.Events;

public class EnableByMinCondition : TwoStateObject
{
    [SerializeField] private int targetValue;

    [SerializeField] private UnityEvent OnActivate;
    [SerializeField] private UnityEvent OnDeactivate;

    private int currentValue;

    public void AddValue(int value)
    {
        currentValue += value;
        CheckValue();
    }

    public void RemoveValue(int value)
    {
        currentValue -= value;
        CheckValue();
    }

    private void CheckValue()
    {
        if (!enabled) return;
        if(currentValue >= targetValue)
        {
            if (IsActive) return;
            Activate();
        }
        else
        {
            if (!IsActive) return;
            Deactivate();
        }
    }

    protected override void OnBecameActive()
    {
        base.OnBecameActive();
        OnActivate?.Invoke();
    }

    protected override void OnBecameInactive()
    {
        base.OnBecameInactive();
        OnDeactivate?.Invoke();
    }
}