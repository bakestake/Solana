using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPvCInputs
{
    float GetHorizontalInput(); // Move left/right
    bool IsAttacking();         // Attack input
    bool IsBlocking();          // Block input
    bool IsJumping();           // Jump input
    bool UseUltimate();         // Ultimate move
}

public enum PVCButtons
{
    Attack = 0,
    Block = 1,
    Jump = 2,
    Ultimate = 3,
}
