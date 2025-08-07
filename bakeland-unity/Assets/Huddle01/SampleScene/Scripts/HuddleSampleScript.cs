#if !UNITY_WEBGL

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
using UnityEngine.UI;
using System.Linq;

public class HuddleSampleScript : MonoBehaviour
{
    [SerializeField]
    private DeviceHandler _deviceHandler;

    public LocalPeer LocalPeerObj;
    public Room RoomObj;

    private ServerSettings _serverSettings;
    private WebSocketEventsProcessor _webSocketEventsProcessor;
    private EventsBroadcaster _eventsBroadcaster;

    private WebSocketService _webSocketService;
    private RequestProcessor _requestProcessor;

    private Huddle01.Core.Permissions _permissions;

    private bool _autoConsume;

    private bool _isWebSocketConnected;

    private string _serverUrl = "wss://eu-central-1-blue-sushi.huddle01.media/ws?";//https://us-east-1-blue-sushi.huddle01.media

    private string _currentServerUrl;

    [SerializeField]
    private string token = "eyJhbGciOiJSUzI1NiJ9.eyJyb29tSWQiOiJrZ2ctaXlyZS1ia3MiLCJtdXRlT25FbnRyeSI6ZmFsc2UsInZpZGVvT25FbnRyeSI6ZmFsc2UsInJvb21UeXBlIjoiVklERU8iLCJyb2xlIjoiaG9zdCIsInBlcm1pc3Npb25zIjp7ImFkbWluIjp0cnVlLCJjYW5Db25zdW1lIjp0cnVlLCJjYW5Qcm9kdWNlIjp0cnVlLCJjYW5Qcm9kdWNlU291cmNlcyI6eyJjYW0iOnRydWUsIm1pYyI6dHJ1ZSwic2NyZWVuIjp0cnVlfSwiY2FuU2VuZERhdGEiOnRydWUsImNhblJlY3ZEYXRhIjp0cnVlLCJjYW5VcGRhdGVNZXRhZGF0YSI6dHJ1ZX0sInBlZXJJZCI6InBlZXJJZC1TWEZCLWF3azBFVWh1cndRM2U1SmIiLCJwdXJwb3NlIjoiU0RLIiwicm9vbUluZm8iOnsicm9vbUxvY2tlZCI6ZmFsc2UsIm11dGVPbkVudHJ5IjpmYWxzZSwicm9vbVR5cGUiOiJWSURFTyIsInZpZGVvT25FbnRyeSI6ZmFsc2V9LCJpYXQiOjE3Mjc4MDA4MjYsImlzcyI6Ikh1ZGRsZTAxIiwiZXhwIjoxNzI3ODExNjI2fQ.Ki1Zqoym1RuJ749V6f8SsSgLk-XAfm5N5wfyJNGRf3wGIdblJNoQSrCHOuSxl9S1ol3E1hlwSBj89aAf0paPxCJxhbx4b6Y0xW4kVtukiQSUPhXjtrmdx_dATx41Bbb23rVa5_QLgj2-iawKsG4ea1NfESJi_JwLc77-2OrcAWqw_kS6tEonGkcq7j3b9FbsCxJ90Yzf4EeLluroV0U-J_BfJ6wZO0GCXMWVeNJsKY9bLlrRXthGjEUVkABugOZQmmpKqSyU5oo1MbSJDjyaPHH-icqNoTrz8qx7rxHz4DnzAoqyjODgFUp8qlvPhRRONo2fAcZQUJY_kC4HO1tIBFCgK_uN-Da7du3eTCviNIaP2Gf1_qyvUQUJKfLJD5u4FV1dAoURNU4hkZLetEvhGyJ60GVoZhdjijjZV91umXZ1c3tR0DxB9RUsqB_cN_4bIdXpLMN4ohBbsfy9-2ecQqryIeofcWuFju5WdC0vZu1ZWl-Y-SFx3pDYKRE-GSbCNFAtqGQ6gwuYuClE4XYxA7QRmiT_WsN8SREj0Wf7cQTkrjMoZ1XOtVvBXaOPMjWxZLbNHRfJntzzLiKXl0jPHKMI5Zqi9KiowUKGpYAeTqpmomhp3PIcM5HMDSx21d3Sxroh6hhl3UtQp_uDGuYmAI8uelE-0S9yntpBMgFBybY";
    private const string region = "ASI";
    private const string coutry = "IN";
    private const string version = "2";

    [Space(10)]
    [SerializeField]
    private GameObject _remotePeerUIPrefab;
    [SerializeField]
    private Transform _remotePeerContentHolder;

    [SerializeField]
    private TMP_InputField _tokenValueField;

    [SerializeField]
    private GameObject _mainMenu2;

    [SerializeField]
    private GameObject _mainMenu1;

    public string HuddleToken;

    [Space(10)]
    [Header("DropDowns")]
    [SerializeField]
    private Dropdown _micDropDown;
    [SerializeField]
    private Dropdown _camDropDown;

    private string m_deviceName = null;
    private string cameraDeviceName = null;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Huddle01 SDK Initialized");
        _eventsBroadcaster = new EventsBroadcaster();
        _requestProcessor = new RequestProcessor();
        _permissions = new Huddle01.Core.Permissions();

        _serverSettings = new ServerSettings();

        _webSocketEventsProcessor = new WebSocketEventsProcessor(_eventsBroadcaster);

        _webSocketService = new WebSocketService(_serverSettings, _webSocketEventsProcessor, _requestProcessor);

        LocalPeerObj = LocalPeer.Instance.Init(_webSocketService, _eventsBroadcaster, _deviceHandler, _permissions);
        RoomObj = Room.Instance.Init(_webSocketService, _permissions, true);

        LocalPeerObj.On("consume", async (args)=> ConsumePeer(args));

        if (!_permissions.CheckPermission(PermissionType.canProduce, null)) return;

        RoomObj.On("room-joined",async(args)=> SetupRoomPeers());

        //StartCoroutine(GetAndSetHuddleToken(Constants.HuddleRoomId, Constants.HuddleApiKey));
        StartCoroutine(GetServerUrl(_tokenValueField.text, region, coutry, version));
        if(!Permission.HasUserAuthorizedPermission(Permission.Camera))
        Permission.RequestUserPermission(Permission.Camera);

        _micDropDown.options = Microphone.devices.Select(name => new Dropdown.OptionData(name)).ToList();
        _micDropDown.onValueChanged.AddListener(OnDeviceChanged);

        _camDropDown.options =
           WebCamTexture.devices.Select(device => new Dropdown.OptionData(device.name)).ToList();
        _camDropDown.onValueChanged.AddListener(OnCameraDeviceChanged);

    }

    private void OnDeviceChanged(int value)
    {
        if (_micDropDown.options.Count == 0)
            return;
        m_deviceName = _micDropDown.options[value].text;
        Microphone.GetDeviceCaps(m_deviceName, out int minFreq, out int maxFreq);
        DeviceHandler.SelectedAudioDevice = m_deviceName;
        LocalPeerObj.SelectedAudioStreamDevice = m_deviceName;
    }


    private void OnCameraDeviceChanged(int value)
    {
        if (_camDropDown.options.Count == 0)
            return;
        cameraDeviceName = _camDropDown.options[value].text;
        DeviceHandler.SelectedCameraDevice = cameraDeviceName;
        LocalPeerObj.SelectedVideoStreamDevice = cameraDeviceName;
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (_webSocketService != null && _webSocketService.IsConnected)
        {
            _webSocketService._webSocket.DispatchMessageQueue();
        }
#endif
    }

    public void GetToken() 
    {
        StartCoroutine(GetServerUrl(_tokenValueField.text, region, coutry, version));
    }

    public void JoinRoom() 
    {
        LocalPeerObj.ConnectRoomRequest(Constants.HuddleRoomId);
        SubscribeToEvents();
    }

    public void StartMic() 
    {
        _ = LocalPeerObj.EnableAudio();
    }

    public void StartCam() 
    {
        _ = LocalPeerObj.EnableVideo();
    }

    public void StopMic() 
    {
        _ = LocalPeerObj.StopProducing("audio");
    }

    public void StopCam() 
    {
       _ = LocalPeerObj.StopProducing("video");
    }

    public void GetPeersInRoom() 
    {
        Debug.Log(RoomObj.RemotePeers.Count);
        foreach (var peer in RoomObj.RemotePeers)
        {
            Debug.Log($"Peer key {peer.Key} : {peer.Value.PeerId}");
        }
    }

    public void GetMetadataOfAllPeers()
    {
        foreach (var peer in RoomObj.RemotePeers)
        {
            Debug.Log($"Peer key {peer.Key} : {peer.Value.Metadata}");
        }
    }

    private void SubscribeToEvents() 
    {
        RoomObj.On("new-peer-joined",async (args) => 
        {
            RemotePeer remotePeer = args[0] as RemotePeer;
            OnNewPeerJoined(remotePeer);
        });

    }

    private void OnNewPeerJoined(RemotePeer remotePeer)
    {
        GameObject reemotePeerSection = Instantiate(_remotePeerUIPrefab, _remotePeerContentHolder);
        reemotePeerSection.transform.localScale = Vector3.one;
        reemotePeerSection.GetComponent<RemotePeerSection>().Init(remotePeer);
    }

    private void SetupRoomPeers() 
    {
        PopulateAllJoinPeers(LocalPeerObj.RemotePeers);
    }

    private void PopulateAllJoinPeers(Dictionary<string,RemotePeer> remotePeers) 
    {
        foreach (var peer in remotePeers)
        {
            GameObject reemotePeerSection = Instantiate(_remotePeerUIPrefab, _remotePeerContentHolder);
            reemotePeerSection.transform.localScale = Vector3.one;
            reemotePeerSection.name = peer.Key;
            reemotePeerSection.GetComponent<RemotePeerSection>().Init(peer.Value);
        }
        
    }

    public async void ConsumePeer(object[] args) 
    {
        string label = args[0] as string;
        string producerPeerId = args[1] as string;

        if (label.Equals("audio")) 
        {
            //Instantiate an object and add audio source

        }

    }

    IEnumerator GetServerUrl(string token, string region, string country, string version)
    {
        yield return StartCoroutine(GetAndSetHuddleToken(Constants.HuddleRoomId,Constants.HuddleApiKey));

        Debug.Log("Connect to server");

        string apiServerUrl = "https://apira.huddle01.media/api/v1/getSushiUrl";
        //string apiServerUrl = "https://apira-testnet.huddle01.media/api/v1/getSushiUrl";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiServerUrl))
        {
            webRequest.SetRequestHeader("authorization", "Bearer " + HuddleToken);
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();
            Debug.Log($"response data {webRequest.downloadHandler.text}");
            Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(webRequest.downloadHandler.text);
            
            string response = responseData["url"];

            response = response.Replace("https://", "wss://");
            response += "/ws";

            _currentServerUrl = GetServerUrlWithParam(response, HuddleToken, version, region, country);
            Debug.Log(_currentServerUrl);
            ConnectToServer(_currentServerUrl);

            _mainMenu2.SetActive(false);
            _mainMenu1.SetActive(true);
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

    private async void ConnectToServer(string url)
    {
        _serverSettings.WebSocketUrl = url;
        _webSocketService.Connect();
    }

    private void OnApplicationQuit()
    {
        Room.Instance.LeaveRoom();
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
                HuddleToken = webRequest.downloadHandler.text;
            }
        }
    }

    public void ChangeVideoDevice() 
    {
        _ = LocalPeerObj.ChangeVideoTrack(DeviceHandler.SelectedCameraDevice);
    }

    public void ChangeMicDevice() 
    {
        _ = LocalPeerObj.ChangeVideoTrack(DeviceHandler.SelectedAudioDevice);
    }
}

#endif


