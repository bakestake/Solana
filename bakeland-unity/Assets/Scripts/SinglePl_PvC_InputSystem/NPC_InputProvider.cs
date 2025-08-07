using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class NPC_InputProvider : MonoBehaviour, IPvCInputs
{
    private NPCController npcController; // Your NPC logic controller

    public NPC_InputProvider(NPCController controller)
    {
        npcController = controller;
    }

    public float GetHorizontalInput()
    {
        // AI logic for movement (e.g., follow the player)
        return npcController.GetMovementDirection();
    }

    public bool IsAttacking()
    {
        // AI logic for attacking (e.g., distance from player)
        return npcController.ShouldAttack();
    }

    public bool IsBlocking()
    {
        // AI logic for blocking
        return npcController.ShouldBlock();
    }

    public bool IsJumping()
    {
        // AI logic for jumping
        return npcController.ShouldJump();
    }

    public bool UseUltimate()
    {
        // AI logic for using ultimate
        return npcController.ShouldUseUltimate();
    }
}
