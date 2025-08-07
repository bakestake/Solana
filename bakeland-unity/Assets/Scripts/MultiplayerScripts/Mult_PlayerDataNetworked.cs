using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using BakelandWalletInteraction;
using Huddle01.Core;

public class Mult_PlayerDataNetworked : NetworkBehaviour
{
    [System.Serializable]
    public class TokenResponse
    {
        public string access_token;
    }

    // Local Runtime references
    private PVPQueue _overviewPanel = null;
    private HConsumePanel hConsumePanel = null;
    private ChangeDetector _changeDetector;
    //For Huddle
    private CreateRoomHuddle createHuddleRoom = null;
    private UaserWalletInteractions userWallet;
    private DeviceHandler _deviceHandler;
    private HuddleClient _huddleClientInstance;
    private string HuddleProjectId = "pi_cKDXutw7NAfxVLud";


    // Game Session SPECIFIC Settings are used in the UI.
    // The method passed to the OnChanged attribute is called everytime the [Networked] parameter is changed.
    [HideInInspector]
    [Networked]
    public NetworkString<_16> NickName { get; private set; }

    [HideInInspector]
    [Networked]
    public NetworkString<_16> localPeerID { get; private set; }


    public override void Spawned()
    {
        // --- Client
        // Find the local non-networked PlayerData to read the data and communicate it to the Host via a single RPC 
        if (Object.HasInputAuthority)
        {
            var nickName = FindObjectOfType<Mult_playerData>().GetNickName();
            RpcSetNickName(nickName);

            //Huddle Token
            createHuddleRoom = FindObjectOfType<CreateRoomHuddle>();
            if (createHuddleRoom != null)
            {
                userWallet = FindObjectOfType<UaserWalletInteractions>();
                if (userWallet != null)
                {
                    string clientAddress = userWallet.GetConnectedAddress();
                    string roomID = ServerGameManager.Instance.RoomId;
                    if (!string.Equals(roomID, "null") && !string.Equals(clientAddress, ""))
                    {
                        CreateToken(clientAddress, roomID);
                        // Initialize the Huddle Client
                        _deviceHandler = createHuddleRoom.GetComponent<DeviceHandler>();
                        HuddleClient.Instance.InitForWebgl(HuddleProjectId, roomID);
                        _huddleClientInstance = HuddleClient.Instance;
                        //Join Huddle Room
                        _huddleClientInstance.JoinRoom(roomID);
                        SubscribeRoomJoinEvent();
                    }
                    else
                    {
                        DebugUtils.LogError("Either the Room ID: " + roomID + " or the client Address: " + clientAddress + " is null");
                    }
                }
                else
                {
                    DebugUtils.LogError("The User Wallet Class is not valid");
                }
            }
            else
            {
                DebugUtils.LogError("The Create Huddle Room is not valid");
            }
        }

        // --- Host & Client
        // Set the local runtime references.
        _overviewPanel = FindObjectOfType<PVPQueue>();
        // Add an entry to the local Overview panel with the information of this spaceship
        _overviewPanel.AddEntry(Object.InputAuthority, this);
        // Refresh panel visuals in Spawned to set to initial values.
        _overviewPanel.UpdateNickName(Object.InputAuthority, NickName.ToString());

        hConsumePanel = FindObjectOfType<HConsumePanel>();

        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        // --- Host
        // Initialized game specific settings
        if (Object.HasStateAuthority == false) return;
        ServerGameManager.Instance.SpawnHuddleAudioManager();
    }

    public override void Render()
    {
        if (_changeDetector == null)
        {
            Debug.Log("Change Detector is null");
        }
        foreach (var change in _changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(NickName):
                    _overviewPanel.UpdateNickName(Object.InputAuthority, NickName.ToString());
                    break;
            }
        }
    }

    // Remove the entry in the local Overview panel for this Player
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        _overviewPanel.RemoveEntry(Object.InputAuthority);
        //Leave Room
        OnLeaveRoom();
    }

    // RPC used to send player information to the Host
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetNickName(string nickName)
    {
        if (string.IsNullOrEmpty(nickName)) return;
        NickName = nickName;
    }

    // RPC used to send player information to the Host
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetLocalPeerID(string peerID)
    {
        if (string.IsNullOrEmpty(peerID)) return;
        localPeerID = peerID;
    }

    private void SubscribeRoomJoinEvent()
    {
        HuddleClient.OnJoinRoom += OnRoomJoined;
    }

    private void OnRoomJoined()
    {
        //Client
        if (Object.HasInputAuthority)
        {
            var PeerId = _huddleClientInstance.GetLocalPeerId();
            RpcSetLocalPeerID(PeerId);
        }
    }

    private void OnLeaveRoom()
    {
        //Client
        if (Object.HasInputAuthority)
        {
            _huddleClientInstance.LeaveRoom();
        }
    }

    public void AddToConsumePanel(PlayerRef playerRef, string nickName, string remotePeerID)
    {
        hConsumePanel = hConsumePanel == null ? FindObjectOfType<HConsumePanel>() : hConsumePanel;
        // Add an entry to the local Overview panel with the information of this spaceship
        hConsumePanel.AddEntry(playerRef);
        // Refresh panel visuals in Spawned to set to initial values.
        hConsumePanel.UpdateNickName(playerRef, nickName, remotePeerID, this);
    }

    public void RemoveFromConsume(PlayerRef playerRef)
    {
        hConsumePanel.RemoveEntry(playerRef);
    }

    public void StartConsumingPeer(string remotePeerID)
    {
        if (Object.HasInputAuthority)
        {
            _huddleClientInstance.ConsumeRemotePeer(remotePeerID, remotePeerID);
        }
    }

    public void StopConsumingPeer(string remotePeerID)
    {
        if (Object.HasInputAuthority)
        {
            _huddleClientInstance.StopConsumingPeer(remotePeerID, remotePeerID);
        }
    }

    //Huddle Token
    public async void CreateToken(string clientAddress, string roomID)
    {
        string ReadApi_BaseURL = $"https://t8kmnpwzy5.execute-api.eu-west-3.amazonaws.com/dev";
        try
        {
            DebugUtils.Log("The api URL is: " + $"{ReadApi_BaseURL}/getHuddleToken/{clientAddress}/{roomID}");
            await CreateHuddleTokenAsync($"{ReadApi_BaseURL}/getHuddleToken/{clientAddress}/{roomID}", (string res) =>
            {
                TokenResponse response = JsonUtility.FromJson<TokenResponse>(res);
                DebugUtils.Log("The unparsed json value is which is the Huddle Token:" + res);
                DebugUtils.Log("The parsed JSON response, which is the Huddle Token:" + response.access_token);
                ServerGameManager.Instance.AccessToken = response.access_token;
            });
        }
        catch (Exception e)
        {
            DebugUtils.LogError("There is an error with reason" + e);
        }
    }

    private async Task CreateHuddleTokenAsync(string apiUrl, Action<string> callback)
    {
        try
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
            {
                webRequest.SetRequestHeader("Origin", "*");
                await webRequest.SendWebRequest();
                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    DebugUtils.Log(apiUrl);
                    callback(null);
                }
                else
                {
                    string jsonResponse = webRequest.downloadHandler.text;
                    callback(jsonResponse);
                }
            }
        }
        catch (Exception e)
        {
            DebugUtils.LogError("Api error occurred: " + e.Message);
        }
    }
}
