using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using Fusion.Addons.Hathora;

public class PlayerInputBehaviour : Fusion.Behaviour, INetworkRunnerCallbacks
{
    public static PlayerInputBehaviour Instance { get; private set; }

    [HideInInspector]
    private List<SessionInfo> SessionInfoList = new List<SessionInfo>();

    //Inputs of the player
    private const string AXIS_HORIZONTAL = "Horizontal";
    private const string AXIS_VERTICAL = "Vertical";
    private const string BUTTON_SPEEDWALKING = "Fire3";

    /// <summary>
    /// Allows interact and start game to be treated more like button presses.
    /// </summary>

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new Mult_NetworkInputData();

        if (Input.GetAxisRaw(AXIS_HORIZONTAL) != 0)
        {
            data.HorizontalInput = Input.GetAxisRaw(AXIS_HORIZONTAL);

        }
        if (Input.GetAxisRaw(AXIS_VERTICAL) != 0)
        {
            data.VerticalInput = Input.GetAxisRaw(AXIS_VERTICAL);
        }
        //Speed Walking
        data.Buttons.Set(MultiplayerButtons.SpeedWalking, Input.GetButton(BUTTON_SPEEDWALKING));

        input.Set(data);
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        SessionInfoList.Clear();
        SessionInfoList = sessionList;
        //ServerStartMenu.Instance.FillSessionList();
        //Getting Hathora Client
        HathoraClient hathoraClient = FindObjectOfType<HathoraClient>();
        if (hathoraClient)
        {
            hathoraClient.SetHasValidSession();
        }

    }

    public List<SessionInfo> GetSessionInfo()
    {
        return SessionInfoList;
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
