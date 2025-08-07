using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class PVP_PlayerInput : MonoBehaviour, INetworkRunnerCallbacks
{
    public static PVP_PlayerInput Instance { get; private set; }

    [HideInInspector]
    private List<SessionInfo> PvPSessionInfoList = new List<SessionInfo>();

    public int _selectedCharacter = 0;

    //Inputs of the player
    private const string AXIS_HORIZONTAL = "Horizontal";
    private const string BUTTON_ATTACK = "Fire1";
    private const string BUTTON_BLOCK = "Fire2";
    private const string BUTTON_JUMP = "Jump";
    private const string BUTTON_ULTIMATE = "Fire3";

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SelectCharacter(int selection)
    {
        _selectedCharacter = selection;
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new Mult_NetworkInputData();

        //Moving Right and left
        data.HorizontalInput = Input.GetAxisRaw(AXIS_HORIZONTAL);
        //Fire
        data.Buttons.Set(PVPButtons.Attack, Input.GetButton(BUTTON_ATTACK));
        //Block
        data.Buttons.Set(PVPButtons.Block, Input.GetButton(BUTTON_BLOCK));
        //Jump
        data.Buttons.Set(PVPButtons.Jump, Input.GetButton(BUTTON_JUMP));
        //Ultimate
        data.Buttons.Set(PVPButtons.Ultimate, Input.GetButton(BUTTON_ULTIMATE));


        input.Set(data);
    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        PvPSessionInfoList.Clear();
        PvPSessionInfoList = sessionList;
        ServerStartMenu.Instance.FillSessionList();
    }

    #region INetworkRunnerCallbacks
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { if (runner.LocalPlayer == player) { Instance = this; } }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    #endregion

}
