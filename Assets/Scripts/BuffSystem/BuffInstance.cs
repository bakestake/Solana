using UnityEngine;

public class BuffInstance
{
    public Buff buff;
    private GameObject target;
    private float remainingDuration;
    public GameObject uiElement;  // UI element for this buff

    public BuffInstance(Buff buff)
    {
        this.buff = buff;
        this.remainingDuration = buff.defaultDuration;
    }

    // Apply the buff's effect
    public void Apply()
    {
        buff.ApplyEffect(target);
    }

    // Update the buff's remaining duration
    public void UpdateDuration(float deltaTime)
    {
        remainingDuration -= deltaTime;

        // Update the UI with the remaining time
        if (uiElement != null)
        {
            BuffUI buffUIScript = uiElement.GetComponent<BuffUI>();
            buffUIScript.SetDuration(remainingDuration);
        }
    }

    // Check if the buff has expired
    public bool IsExpired()
    {
        return remainingDuration <= 0;
    }

    // Remove the buff's effect
    public void Expire()
    {
        buff.RemoveEffect(target);
    }

    // Extend the buff's duration by its default duration
    public void ExtendDuration()
    {
        remainingDuration += buff.defaultDuration;
    }

    // Get the remaining time (for UI purposes)
    public float GetRemainingTime()
    {
        return remainingDuration;
    }
}
