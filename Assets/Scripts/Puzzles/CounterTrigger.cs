using UnityEngine;
using UnityEngine.Events;

public class CounterTrigger : MonoBehaviour
{
    [SerializeField] private int targetCount = 1;
    [SerializeField] private UnityEvent OnTargetReached;
    [SerializeField] private UnityEvent OnTargetLosed;
    [SerializeField] private UnityEvent OnReset;

    private int currentCount;
    private bool isCompleted;

    public void Increment()
    {
        currentCount++;
        if (currentCount >= targetCount && !isCompleted)
        {
            isCompleted = true;
            OnTargetReached?.Invoke();
        }
    }

    public void Decrement()
    {
        currentCount--;
        if (currentCount < targetCount && isCompleted)
        {
            isCompleted = false;
            OnTargetLosed?.Invoke();
        }
    }

    public void ResetCounter()
    {
        currentCount = 0;
        if (isCompleted)
        {
            isCompleted = false;
            OnTargetLosed?.Invoke();
        }
    }
}