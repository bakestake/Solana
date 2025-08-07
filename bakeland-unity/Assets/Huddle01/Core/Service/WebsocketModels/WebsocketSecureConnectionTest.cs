#if !UNITY_WEBGL

using UnityEngine;
using Huddle01.Core.Services;
using NativeWebSocket;
using Huddle01.Core.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Huddle01.Core.EventBroadcast;
using Google.Protobuf;
using Huddle01.Core;

public class WebsocketSecureConnectionTest : MonoBehaviour
{
    private const string AuthenticationKey = "Authorization";

    private const string AuthToken = "";

    private bool _isConnected = false;

    private ServerSettings _serverSettings;
    private WebSocketEventsProcessor _webSocketEventsProcessor;
    private EventsBroadcaster _eventsBroadcaster;

    private WebSocketService _webSocketService;
    private RequestProcessor _requestProcessor;

    private DeviceHandler _deviceHandler;

    private Huddle01.Core.Permissions _permissions;

    private LocalPeer _localPeer;
    private Room _room;

    private string token = "eyJhbGciOiJSUzI1NiJ9.eyJyb29tSWQiOiJkemUta3Njbi1sbm4iLCJtdXRlT25FbnRyeSI6ZmFsc2UsInZpZGVvT25FbnRyeSI6ZmFsc2UsInJvb21UeXBlIjoiVklERU8iLCJyb2xlIjoiaG9zdCIsInBlcm1pc3Npb25zIjp7ImFkbWluIjp0cnVlLCJjYW5Db25zdW1lIjp0cnVlLCJjYW5Qcm9kdWNlIjp0cnVlLCJjYW5Qcm9kdWNlU291cmNlcyI6eyJjYW0iOnRydWUsIm1pYyI6dHJ1ZSwic2NyZWVuIjp0cnVlfSwiY2FuU2VuZERhdGEiOnRydWUsImNhblJlY3ZEYXRhIjp0cnVlLCJjYW5VcGRhdGVNZXRhZGF0YSI6dHJ1ZX0sInBlZXJJZCI6InBlZXJJZC1pcUFyVlc4a1JITFhPM0t4ZThBMjciLCJwdXJwb3NlIjoiU0RLIiwicm9vbUluZm8iOnsicm9vbUxvY2tlZCI6ZmFsc2UsIm11dGVPbkVudHJ5IjpmYWxzZSwicm9vbVR5cGUiOiJWSURFTyIsInZpZGVvT25FbnRyeSI6ZmFsc2V9LCJpYXQiOjE3MjY1NjUzMTMsImlzcyI6Ikh1ZGRsZTAxIiwiZXhwIjoxNzI2NTc2MTEzfQ.LGLGeCnXIY6lpo34rMkexuORC4iuH_wU4Mu35X5aGY0b-9QFk3q1MIQjCVKdmKS-Koa5dW3UWy5WqgGPgYFbiE1m-Xq-NxyJddGP8nl6ZB8YN4FzE1VrE9Qaq7aneVhWkzDaXw_j0FLiRwWgxAsAEL0R_bbL2KYIHD3QWGRq3dMmgsCt4kRqGrhcr8xP1Y0eSSX4ykdKvTKnWj67MmBD1IHRMzANZ8qCqC4ZMiwWVcnsS7wMJN9YkRuBFg2Boj3Iuo-Oi_Ww1z9McqyhO8_LeyQYPSy1LwDJiwGZM-yaiGEWg72EQ1qz6MySMNON6Xoyus3toBHLmfAYDuYifUNDKO5cLsbbVoBQtvhwFJtxrENw1LOTK9FaStpjNO5meKH0h_TI8cu6x4MCFWsWKjzjmdEslO2-1fDOB_HbYI6efuClgA61Vrot4I3KTNxlPuXCGLRSQSW95eKV-sT55myElJgjUXaMwg9-Tfi0aNwUZfzOzBRKZkRQhFBYRmWfvJJ2YhFOF93zJN3_312W9oh4ZxVvJ1y7QH1nLkiv_GJTACbB_GEDFXPcQQi7WAUUBniHoMgn5nZAzYOP2vhxSJn3wHGH5UOPeDXwKHubDV-rXcQ1kPmWTxsmzXnmU-TUiDgiBXmfWgTVv28R5Rb-EhgLQ4G85wmFo5WGuZtWSt-THhQ";
    private string region = "ASI";
    private string coutry = "IN";
    private string version = "2";

    private string _serverUrl = "wss://eu-central-1-blue-sushi.huddle01.media/ws?";//https://us-east-1-blue-sushi.huddle01.media

    private string _currentServerUrl;
    private bool _autoConsume = true;

    private void Start()
    {
        StartCoroutine(GetServerUrl(token, region, coutry, version));
    }

    private async void ConnectToServer(string url)
    {
        //Initiatelize Websocket service

        _eventsBroadcaster = new EventsBroadcaster();
        _requestProcessor = new RequestProcessor();
        _permissions = new Huddle01.Core.Permissions();

        _serverSettings = new ServerSettings();
        _serverSettings.WebSocketUrl = url;

        _webSocketEventsProcessor = new WebSocketEventsProcessor(_eventsBroadcaster);

        _webSocketService = new WebSocketService(_serverSettings, _webSocketEventsProcessor, _requestProcessor);

        _localPeer = LocalPeer.Instance.Init(_webSocketService, _eventsBroadcaster, _deviceHandler,new Huddle01.Core.Permissions());
        _room =  Room.Instance.Init(_webSocketService, _permissions, _autoConsume);

        //Subscribe to events
        _webSocketService.Connected += OnConnectionOpen;
        _webSocketService.MessageReceived += OnMessageReceived;

        //connect
        _webSocketService.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (_webSocketService != null)
        {
            _webSocketService._webSocket.DispatchMessageQueue();
        }
#endif
    }


    private void OnApplicationQuit()
    {
        //_webSocketService.CloseSocketConnnection();
    }

    private void OnConnectionOpen()
    {
        _isConnected = true;

        Debug.Log($"Connected successfully :");

        _webSocketService.ErrorReceived += OnErrorReceived;
        _webSocketService.Disconnected += OnConnectionClose;
    }

    private void OnErrorReceived(string error)
    {
        Debug.Log($"Error message : {error}");
    }

    private void OnConnectionClose(WebSocketCloseCode closeCode)
    {
        _isConnected = false;
        _webSocketService.Connected -= OnConnectionOpen;
        _webSocketService.MessageReceived -= OnMessageReceived;
        _webSocketService.ErrorReceived -= OnErrorReceived;
        _webSocketService.Disconnected -= OnConnectionClose;
    }

    private void OnMessageReceived(byte[] message)
    {
        Debug.Log($"Received message : {message}");
    }

    private string GetServerUrlWithParam(string url, string token, string version, string region, string country)
    {
        url += "?token=" + token;
        url += "&version=" + version;
        url += "&region=" + region;
        url += "&country=" + country;

        return url;
    }


    IEnumerator GetServerUrl(string token, string region, string country, string version)
    {
        string apiServerUrl = "https://apira.huddle01.media/api/v1/getSushiUrl";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiServerUrl))
        {
            webRequest.SetRequestHeader("authorization", "Bearer " + token);
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(webRequest.downloadHandler.text);

            string response = responseData["url"];

            response = response.Replace("https://", "wss://");
            response += "/ws";

            _currentServerUrl = GetServerUrlWithParam(response, token, version, region, country);
            ConnectToServer(_currentServerUrl);
        }
    }

    public void CheckReq()
    {
        ConnectRoom connectRoom = new ConnectRoom();
        connectRoom.RoomId = "hmq-hhzh-kkj";

        byte[] messageArray = connectRoom.ToByteArray();
    }

}

#endif


