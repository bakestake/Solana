using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    private WanderingMovementHandler movementHandler;

    private void Awake()
    {
        movementHandler = GetComponent<WanderingMovementHandler>();
    }

    public void Talking()
    {
        if (movementHandler == null) return;
        movementHandler.StopWandering();
    }

    public void EndTalking()
    {
        if (movementHandler == null) return;
        movementHandler.StartWandering();
    }

    public void ResetToWanderingState()
    {
        if (movementHandler == null) return;
        movementHandler.StartWandering();
    }

    public void PlayFootstepSound()
    {
    }
}