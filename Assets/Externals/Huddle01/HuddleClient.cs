using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Huddle01.Core;
using Mediasoup;
using Huddle01.Core.Settings;
using Huddle01.Core.Services;
using Huddle01.Core.EventBroadcast;
using System;
using static Huddle01.Core.Services.WebSocketService;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.Android;
using Huddle01.Services;
using Huddle01;
using System.Text;

public class HuddleClient : Singleton<HuddleClient>
{
    //Delegates
    public delegate void PeerAddedEventHandler(string peerId);
    public delegate void PeerLeftEventHandler(string peerId);
    public delegate void PeerMutedEventHandler(string peerId);
    public delegate void PeerUnMutedEventHandler(string peerId);
    public delegate void RoomClosedEventHandler();
    public delegate void PeerMetadataUpdatedEventHandler(string peerId);
    public delegate void JoinRoomEventHandler();
    public delegate void ResumePeerVideoEventHandler(string peerId);
    public delegate void StopPeerVideoEventHandler(string peerId);
    public delegate void MessageReceivedEventHandler(string data);
    public delegate void LeaveRoomEventHandler();
    public delegate void AudioDevicesEventHandler(List<MediaDevice> devices);
    public delegate void VideoDevicesEventHandler(List<MediaDevice> devices);

    //Events
    public static event PeerAddedEventHandler PeerAdded;
    public static event PeerLeftEventHandler PeerLeft;
    public static event PeerMutedEventHandler PeerMuted;
    public static event PeerUnMutedEventHandler PeerUnMuted;
    public static event RoomClosedEventHandler RoomClosed;
    public static event PeerMetadataUpdatedEventHandler PeerMetadata;
    public static event JoinRoomEventHandler OnJoinRoom;
    public static event ResumePeerVideoEventHandler OnResumePeerVideo;
    public static event StopPeerVideoEventHandler OnStopPeerVideo;
    public static event MessageReceivedEventHandler OnMessageReceived;
    public static event LeaveRoomEventHandler OnLeaveRoom;
    public static event AudioDevicesEventHandler OnAudioDeviceReceived;
    public static event VideoDevicesEventHandler OnVideoDeviceReceived;

    //Token
    public string HuddleToken => _huddleToken;
    public string _huddleToken;

#if !UNITY_WEBGL
    //Room
    public Room RoomObj=>_room;
    public Room _room;

    //LocalPeer
    public LocalPeer LocalPeerObj=>_localPeer;
    public LocalPeer _localPeer;

    private DeviceHandler DeviceHandlerObj => _deviceHandler;
    private DeviceHandler _deviceHandler;

#endif


    //Permissions
    private Huddle01.Core.Permissions PermissionsObj => _permissions;
    private Huddle01.Core.Permissions _permissions;

    //Auto consume
    public bool IsAutoConsume => _isAutoConsume;
    private bool _isAutoConsume;

    //Region
    private const string Region = "ASI";
    private const string Country = "IN";
    private const string Version = "unity@1.0.1";

    //Server Settings
    public ServerSettings ServerSettingsObj => _serverSettings;
    private ServerSettings _serverSettings;

    //WebSocketEventsProcessor
    public WebSocketEventsProcessor WebSocketEventsProcessorObj => _webSocketEventsProcessor;
    private WebSocketEventsProcessor _webSocketEventsProcessor;

    //EventsBroadcaster
    public EventsBroadcaster EventsBroadcasterObj => _eventsBroadcaster;
    private EventsBroadcaster _eventsBroadcaster;

    //WebSocketService
    public WebSocketService WebSocketServiceObj => _webSocketService;
    private WebSocketService _webSocketService;

    //RequestProcessor
    public RequestProcessor RequestProcessorObj => _requestProcessor;
    private RequestProcessor _requestProcessor;

    private string RoomId => _roomId;
    private string _roomId;

    public string ProjectId { get; set; }

    private string _currentServerUrl;

#if UNITY_WEBGL
    private List<string> _allPeers = new List<string>();
#endif

    public void InitForWebgl(string projectId, string roomId)
    {
        ProjectId = projectId;
        _roomId = roomId;
#if UNITY_WEBGL
        //Setting AutoConsume to false. So that consume functionality works in Huddle
        HuddleInitForWebgl(ProjectId, false);
#endif
    }

#if !UNITY_WEBGL

    public void InitForNative(string projectId,string roomId, DeviceHandler deviceHandler)
    {
        ProjectId = projectId;
        _roomId = roomId;
        _deviceHandler = deviceHandler;
        HuddleInitForNative(deviceHandler);
    }
#endif



    public void SetRegion()
    {

    }

    public void JoinRoom(string roomId, string token = null)
    {
        _roomId = roomId;
#if UNITY_WEBGL
        Huddle01JSNative.JoinRoom(_roomId, _huddleToken);
#else
        _localPeer.ConnectRoomRequest(roomId);
#endif
    }

    public void LeaveRoom()
    {

#if UNITY_WEBGL
        Huddle01JSNative.LeaveRoom();
#else
        _room.LeaveRoom();
#endif

    }


    #region Unity Callbacks

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (_webSocketService != null && _webSocketService.IsConnected)
        {
            _webSocketService._webSocket.DispatchMessageQueue();
        }
#endif
    }

    #endregion

    #region Init

#if !UNITY_WEBGL
    private void HuddleInitForNative(DeviceHandler deviceHandler)
    {
        _deviceHandler = deviceHandler;
        _eventsBroadcaster = new EventsBroadcaster();
        _requestProcessor = new RequestProcessor();
        _permissions = new Huddle01.Core.Permissions();

        _serverSettings = new ServerSettings();

        _webSocketEventsProcessor = new WebSocketEventsProcessor(_eventsBroadcaster);

        _webSocketService = new WebSocketService(_serverSettings, _webSocketEventsProcessor, _requestProcessor);

        _localPeer = LocalPeer.Instance.Init(_webSocketService, _eventsBroadcaster, _deviceHandler, _permissions);
        _room = Room.Instance.Init(_webSocketService, _permissions, true);

        StartCoroutine(GetServerUrl(Region, Country, Version));

        if (!_permissions.CheckPermission(PermissionType.canProduce, null)) return;

        SubscribeLocalPeerEvents();
        SubscribeRoomEvent();
}
#endif

    private void HuddleInitForWebgl(string projectId, bool autoConsume)
    {
#if UNITY_WEBGL
        Huddle01JSNative.InitHuddle01WebSdk(projectId, autoConsume);
        StartCoroutine(GetHuddleTokenForWebgl(projectId, _roomId));

#endif

    }

    public void ConnectToHuddleServer(string url)
    {
        _serverSettings.WebSocketUrl = url;
        _webSocketService.Connect();
    }

    #endregion

    #region Subscribe events

    private void SubscribeRoomEvent()
    {
#if !UNITY_WEBGL
        _room.On("room-joined", async (args) => OnRoomJoined());
        _room.On("new-peer-joined", async (args) =>
        {
            RemotePeer remotePeer = args[0] as RemotePeer;
            OnPeerAdded(remotePeer.PeerId);
        });
        _room.On("peer-left" , async(args) => 
        {
            string peerId = args[0] as string;
            OnPeerLeft(peerId);
        });
        _room.On("PeerMetadataUpdated",async(args) => 
        {
            string peerId = args[0] as string;
            OnPeerMetadataUpdated(peerId);
        });

#endif
    }

    private void SubscribeLocalPeerEvents()
    {
#if !UNITY_WEBGL
        _localPeer.On("consume", async (args) => OnConsumePeer(args));
        _localPeer.On("receive-data", async (args) =>
        {
            string data = args[0] as string;
            MessageReceived(data);
        });
#endif
    }

    private void OnConsumePeer(object[] args)
    {

    }

    #endregion

    #region MediaStream


    public void EnableVideo()
    {
#if UNITY_WEBGL
        Huddle01JSNative.EnableVideo();
#else
        _ = _localPeer.EnableVideo();
#endif
    }

    public void DisableVideo()
    {
#if UNITY_WEBGL
        Huddle01JSNative.DisableVideo();
#else
        _ = _localPeer.DisableVideo();
#endif
    }

    public void EnableAudio()
    {
#if UNITY_WEBGL
        Huddle01JSNative.EnableAudio();
#else
        _ = _localPeer.EnableAudio();
#endif
    }

    public void DisableAudio()
    {
#if UNITY_WEBGL
        Huddle01JSNative.DisableAudio();
#else
        _ = _localPeer.DisableAudio();
#endif
    }

    public void SendTextMessage(string sendDataTo, string payload, string label)
    {
#if UNITY_WEBGL
        Huddle01JSNative.SendTextMessage(payload);
#else
        LocalPeerObj.SendData(sendDataTo, payload, label);
#endif
    }

    public void ChangeAudioDevice(string deviceName)
    {
#if !UNITY_WEBGL
        Microphone.GetDeviceCaps(deviceName, out int minFreq, out int maxFreq);
        DeviceHandler.SelectedAudioDevice = deviceName;
        LocalPeerObj.SelectedAudioStreamDevice = deviceName;
        _ = LocalPeerObj.ChangeAudioTrack(deviceName);
#endif

    }


    public void ChangeVideoDevice(string deviceName)
    {
#if !UNITY_WEBGL
        DeviceHandler.SelectedCameraDevice = deviceName;
        LocalPeerObj.SelectedVideoStreamDevice = deviceName;
#endif
    }


    #endregion

    #region Consume Peer


    public void ConsumeRemotePeer(string peerId, string label)
    {
        Dictionary<string, object> consumeData = new Dictionary<string, object>();
        consumeData.Add("peerId", peerId);
        consumeData.Add("label", label);
#if !UNITY_WEBGL
        //_ = _localPeer.Consume(consumeData,label);
#endif
    }

    public void StopConsumingPeer(string peerId, string label)
    {
#if !UNITY_WEBGL
        _localPeer.StopConsuming(peerId,label);

#else
        Huddle01JSNative.StopConsumingPeer(peerId, label);
#endif
    }

    #endregion

    #region Metadata

    public void UpdateMetadata(string metadataJson)
    {
#if !UNITY_WEBGL
       _localPeer.UpdateMetadata(metadataJson);

#else
        Huddle01JSNative.UpdatePeerMetaData(metadataJson);

#endif
    }

    #endregion

    #region Helpers

#if !UNITY_WEBGL
    public Dictionary<string,RemotePeer> GetAllPeersInRoom() 
    {
        return _room.RemotePeers; 
    }

    public RemotePeer GetRemotePeerInRoomById(string remotePeerId) 
    {
        return _room.GetRemotePeerById(remotePeerId);
    }

#endif

    public string GetMetadataOfRemotePeers(string remotePeerId)
    {
#if !UNITY_WEBGL
        return _room.GetRemotePeerById(remotePeerId).Metadata;

#else
        return Huddle01JSNative.GetRemotePeerMetaData(remotePeerId);
#endif

    }

    public string GetLocalPeerId()
    {
#if !UNITY_WEBGL
        return _localPeer.PeerId;

#else
        return Huddle01JSNative.GetLocalPeerId();
#endif

    }

#if UNITY_WEBGL

    public void GetAudioDevicesForWebGL()
    {
        Huddle01JSNative.GetAudioMediaDevices();
    }

    public void GetVideoDevicesForWebGL()
    {
        Huddle01JSNative.GetVideoMediaDevices();
    }

#endif

    #endregion

    #region Callbacks

    public void OnRoomJoined()
    {
        Debug.Log("Room Joined");
        OnJoinRoom?.Invoke();
    }

    public void OnPeerAdded(string peerInfo)
    {
        Debug.Log($"OnPeerAdded {peerInfo}");
#if UNITY_WEBGL
        if (!_allPeers.Contains(peerInfo)) _allPeers.Add(peerInfo);
#endif

        PeerAdded?.Invoke(peerInfo);
    }

    public void OnPeerLeft(string peerInfo)
    {
        Debug.Log($"OnPeerLeft {peerInfo}");
#if UNITY_WEBGL
        if (!_allPeers.Contains(peerInfo)) _allPeers.Remove(peerInfo);
#endif
        PeerLeft?.Invoke(peerInfo);
    }

    public void OnPeerMute(string peerId)
    {
        Debug.Log($"OnPeerMute {peerId}");
        PeerMuted?.Invoke(peerId);
    }

    public void OnPeerUnMute(string peerId)
    {
        Debug.Log($"OnPeerMute {peerId}");
        PeerUnMuted?.Invoke(peerId);
    }

    public void OnRoomClosed()
    {
        Debug.Log($"OnRoomClosed");
        RoomClosed?.Invoke();
    }

    public void OnPeerMetadataUpdated(string remotePeerId)
    {
        PeerMetadata?.Invoke(remotePeerId);
    }

    public void ResumeVideo(string peerId)
    {
        OnResumePeerVideo?.Invoke(peerId);
    }

    public void StopVideo(string peerId)
    {
        OnStopPeerVideo?.Invoke(peerId);
    }

    public void MessageReceived(string data)
    {
        Debug.Log($"Message received : {data}");
        OnMessageReceived?.Invoke(data);
    }

    public void OnLeavingRoom()
    {
        Debug.Log($"Message received");
        OnLeaveRoom.Invoke();
    }

    public void OnAudioDevicesReceived(string devices)
    {
        List<MediaDevice> mediaDevices = JsonConvert.DeserializeObject<List<MediaDevice>>(devices);
        OnAudioDeviceReceived?.Invoke(mediaDevices);
    }

    public void OnVideoDevicesReceived(string devices)
    {
        List<MediaDevice> mediaDevices = JsonConvert.DeserializeObject<List<MediaDevice>>(devices);
        OnVideoDeviceReceived?.Invoke(mediaDevices);
    }


    #endregion Callbacks


    #region Http Request

    IEnumerator GetServerUrl(string region, string country, string version)
    {
        yield return StartCoroutine(GetAndSetHuddleToken(_roomId, Constants.HuddleApiKey));

        Debug.Log("Connect to server");

        string apiServerUrl = "https://apira.huddle01.media/api/v1/getSushiUrl";
        //string apiServerUrl = "https://apira-testnet.huddle01.media/api/v1/getSushiUrl";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiServerUrl))
        {
            webRequest.SetRequestHeader("authorization", "Bearer " + _huddleToken);
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();
            Debug.Log($"response data {webRequest.downloadHandler.text}");
            Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(webRequest.downloadHandler.text);

            string response = responseData["url"];

            response = response.Replace("https://", "wss://");
            response += "/ws";

            _currentServerUrl = GetServerUrlWithParam(response, _huddleToken, version, region, country);
            Debug.Log(_currentServerUrl);
            ConnectToHuddleServer(_currentServerUrl);

        }
    }


    IEnumerator GetAndSetHuddleToken(string roomId, string apiKey)
    {
        string apiUrl = Constants.HuddleGetTokenUrl + "apiKey=" + apiKey + "&role=guest&roomId=" + roomId;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                _huddleToken = webRequest.downloadHandler.text;
            }
        }
    }

    private string GetServerUrlWithParam(string url, string token, string version, string region, string country)
    {
        url += "?token=" + token;
        url += "&version=" + version;
        url += "&region=" + region;
        url += "&country=" + country;

        return url;
    }

    //This should be done from backend
    //This is only for testing
    IEnumerator GetHuddleTokenForWebgl(string apiKey, string roomId)
    {
        //string apiUrl = "https://huddle01-token-simulator.vercel.app/api?apiKey=" + apiKey + "&role=guest&roomId=" + roomId;
        //string apiUrl = Constants.HuddleGetTokenUrl + "apiKey=" + apiKey + "&role=guest&roomId=" + roomId;

        //string apiUrl= "https://infra-api.huddle01.workers.dev/api/v2/sdk/create-peer-token";
        string apiUrl = "http://localhost:8787/?apiKey=" + apiKey + "&roomId=" + roomId;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("role", "guest");
            webRequest.SetRequestHeader("roomId", roomId);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                _huddleToken = webRequest.downloadHandler.text;
            }
        }

    }



    #endregion

    private void OnApplicationQuit()
    {
#if !UNITY_WEBGL
        Room.Instance.LeaveRoom();
#endif
    }

}