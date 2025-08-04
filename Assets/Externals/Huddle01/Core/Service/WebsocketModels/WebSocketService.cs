using Huddle01.Core.Settings;
using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using System.Threading.Tasks;
using System;
using WalletConnectUnity.NativeWebSocket;

namespace Huddle01.Core.Services
{
    public class WebSocketService
    {
        public WebSocket _webSocket { get; private set; }
        public WebSocketState CurrentWebSocketState => _webSocket.State;

        public string AuthToken { get; set; }

        public bool IsConnected => _webSocket != null && _webSocket.State == WebSocketState.Open;
        public bool IsConnecting => _webSocket != null && _webSocket.State == WebSocketState.Connecting;

        private ServerSettings _serverSettings;
        private WebSocketEventsProcessor _webSocketEventsProcessor;

        private int _retryCount = 0;
        private bool _isTryingToReconnect = false;

        public RequestProcessor RequestPublisher => _requestPublisher;
        public RequestProcessor _requestPublisher;

        public delegate void ConnectionCallback();
        public delegate void DisconnectCallback(WebSocketCloseCode closeCode);
        public delegate void ErrorCallback(string errorCode);
        public delegate void OnMessageReceived(byte[] message);
        public delegate void ReconnectedCallback();

        public event OnMessageReceived MessageReceived;
        public event ConnectionCallback Connected;
        public event DisconnectCallback Disconnected;
        public event ErrorCallback ErrorReceived;
        public event ReconnectedCallback Reconnected;

        public WebSocketService(ServerSettings serverSettings, WebSocketEventsProcessor webSocketEventsProcessor, RequestProcessor requestPublisher)
        {
            _serverSettings = serverSettings;
            _webSocketEventsProcessor = webSocketEventsProcessor;
            _requestPublisher = requestPublisher;
        }

        ~WebSocketService()
        {
            _webSocket?.Close();
        }

        public async void Connect()
        {
            if (_webSocket == null)
            {
                InitializeWebSocket();
            }

            Debug.Log("Connect");
            // waiting for messages
            await _webSocket.Connect();
            Debug.Log("Connected");
        }

        private void InitializeWebSocket()
        {
            Debug.Log("Init Socket");
            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("User-Agent", "Unity3D");

            _webSocket = new WebSocket(_serverSettings.WebSocketUrl, header);

            _webSocket.OnOpen += OnConnected;
            _webSocket.OnError += OnErrorReceive;
            _webSocket.OnClose += OnConnectionClose;
            _webSocket.OnMessage += OnMessageReceive;
        }

        private void OnErrorReceive(string errorCode)
        {
            Debug.LogError($"Error received : {errorCode}");
            ErrorReceived?.Invoke(errorCode);
        }

        private void OnConnected()
        {
            Connected?.Invoke();
            if (_isTryingToReconnect) 
            {
                Reconnected?.Invoke();
                _isTryingToReconnect = false;
            }
        }

        public async void SendMessage(string message)
        {
#if  UNITY_EDITOR
            _webSocket.DispatchMessageQueue();
#endif
            if (_webSocket.State == WebSocketState.Open)
            {
                await _webSocket.SendText(message);
            }
        }

        public async void SendMessage(byte[] message)
        {
#if  UNITY_EDITOR
            _webSocket.DispatchMessageQueue();
#endif
            if (_webSocket.State == WebSocketState.Open)
            {
                await _webSocket.Send(message);
            }
        }

        public void SendRequest(Request request)
        {
            byte[] messageArray = request.ToByteArray();
            SendMessage(messageArray);
        }

        public void SendRequestWithoutWait(Request request) 
        {
            byte[] messageArray = request.ToByteArray();
            SendMessageWithoutWait(messageArray);
        }

        public void SendMessageWithoutWait(byte[] message)
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                _webSocket.Send(message);
            }
        }


        private void OnMessageReceive(byte[] message)
        {
            Debug.Log($"Received msg : {System.Text.Encoding.UTF8.GetString(message)}");    
            _webSocketEventsProcessor.ProcessWebSocketEvents(message);
        }

        private void OnConnectionClose(WebSocketCloseCode closeCode)
        {
            Disconnected?.Invoke(closeCode);
            _webSocket.OnOpen -= OnConnected;
            _webSocket.OnError -= OnErrorReceive;
            _webSocket.OnClose -= OnConnectionClose;
            _webSocket.OnMessage -= OnMessageReceive;
            _ = CloseAsync((int)closeCode);
        }

        private async Task CloseWebSocketAsync()
        {
            if (_webSocket != null)
            {
                await _webSocket.Close();
                _webSocket = null;
                Debug.Log("WebSocket Connection closed");
            }
        }

        // @ambjn -> reconnection CloseAsync
        public async Task CloseAsync(int code, string reason = null, bool reconnect = false)
        {
            if ((code >= 3000 && code <= 4999) || code == 1000 || (code == 1001 && !reconnect))
            {
                Debug.Log($"Closing the connection, Code: {code}, Reason: {reason}");
                await CloseWebSocketAsync();
                return;
            }

            await CloseWebSocketAsync();

            if (code == (int)WebSocketCloseCode.Abnormal || reconnect)
            {
                Debug.Log($"Connection closed abnormally. Reconnecting... Code: {code}, Reason: {reason}");

                while (_retryCount < 7)
                {
                    _isTryingToReconnect = true;

                    int delay = (int)Mathf.Pow(2, _retryCount) * 1000;
                    Debug.Log($"Reconnection attempt {_retryCount + 1}, Delaying {delay}ms...");
                    await Task.Delay(delay);

                    try
                    {
                        Connect();
                        Debug.Log($"Reconnected successfully");
                        _retryCount = 0;
                        return;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log($"Reconnection attempt {_retryCount + 1} failed: {ex.Message}");
                        _retryCount++;
                    }
                }

                Debug.Log($"Reconnection failed after 7 attempts");
                await CloseAsync(4002, "Max reconnection attempts exceeded");
            }
            else
            {
                Debug.Log($"Connection closed. Code: {code}, Reason: {reason}");
            }
        }

    }
}


