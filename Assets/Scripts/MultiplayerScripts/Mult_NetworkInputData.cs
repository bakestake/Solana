using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct Mult_NetworkInputData : INetworkInput
{
    public NetworkButtons Buttons;
    public float HorizontalInput;
    public float VerticalInput;
}

//----------Main Multiplayer Game Inputs------//
enum MultiplayerButtons
{
    SpeedWalking = 0,
}
//----------Main Multiplayer Game Inputs End--//

// -------- PVP Game Inputs ----------//

enum PVPButtons
{
    Attack = 0,
    Block = 1,
    Jump = 2,
    Ultimate = 3,
}

//--------- PVP Game Input End -------//
