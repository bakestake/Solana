using UnityEngine;

public class DoorController : MonoBehaviour
{
    private bool isLocked = true;

    public void Unlock()
    {
        isLocked = false;
        // Add any door unlocking animation/logic here
        gameObject.SetActive(false); // Simple implementation - just hide the door
    }

    public void Lock()
    {
        isLocked = true;
        gameObject.SetActive(true);
    }
}