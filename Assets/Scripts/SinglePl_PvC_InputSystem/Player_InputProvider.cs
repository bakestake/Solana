using UnityEngine;

public class Player_InputProvider : MonoBehaviour, IPvCInputs
{
    public float GetHorizontalInput()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public bool IsAttacking()
    {
        return Input.GetKeyDown(KeyCode.F) && PlayerController.canInteract; // Example: Space bar for attack
    }

    public bool IsBlocking()
    {
        return Input.GetKeyDown(KeyCode.B) && PlayerController.canInteract; // Example: Shift for block
    }

    public bool IsJumping()
    {
        return Input.GetKeyDown(KeyCode.Space) && PlayerController.canInteract; // Example: Up arrow for jump
    }

    public bool UseUltimate()
    {
        return Input.GetKeyDown(KeyCode.U) && PlayerController.canInteract; // Example: U key for ultimate move
    }
}
