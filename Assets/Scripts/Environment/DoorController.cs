using UnityEngine;

public class DoorController : MonoBehaviour
{
    public bool IsLocked { get; private set; } = true;

    public void Unlock()
    {
        IsLocked = false;
        // Add any door unlocking animation/logic here
        gameObject.SetActive(false); // Simple implementation - just hide the door
    }

    public void Lock()
    {
        IsLocked = true;
        gameObject.SetActive(true);
    }
}