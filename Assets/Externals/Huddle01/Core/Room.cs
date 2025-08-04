#if !UNITY_WEBGL

using Huddle01.Core.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using Google.Protobuf.Collections;
using System;
using Mediasoup.Internal;
using Huddle01.Settings;

namespace Huddle01.Core
{
    public class Room : EventEmitter
    {
        private static readonly Lazy<Room> _instance = new Lazy<Room>(() => new Room());
        public static Room Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public delegate void LobbyPeerEventHandler(List<string> lobbyPeerIds);
        public delegate void MetadataUpdatedEventHandler(string metadata);
        public delegate void RoomControlsUpdatedEventHandler();
        public delegate void RoomConnectingEventHandler();
        public delegate void RoomClosedEventHandler(string reason);

        public event LobbyPeerEventHandler LobbyPeerUpdated;
        public event MetadataUpdatedEventHandler MetadataUpdated;
        public event RoomControlsUpdatedEventHandler RoomControlsUpdated;
        public event RoomConnectingEventHandler RoomConnecting;
        public event RoomClosedEventHandler RoomClosed;

        private WebSocketService _webSocketService;
        private Permissions _permissions;

        private string _roomId;

        public string RoomId
        {
            get => _roomId;
            set
            {
                if (_roomId != null)
                {
                    return;
                }
                _roomId = value;
            }
        }


        public string SessionId => _sessionId;
        private string _sessionId;

        public Dictionary<string, object> LobbyPeers => _lobbyPeers;
        private Dictionary<string, object> _lobbyPeers = new Dictionary<string, object>();

        private Dictionary<string, object> _config = new Dictionary<string, object>
        {
            { "roomLocked", false },
            { "allowProduce", true },
            { "allowProduceSources", new Dictionary<string, bool>
                {
                    { "cam", true },
                    { "mic", true },
                    { "screen", true }
                }
            },
            { "allowConsume", true },
            { "allowSendData", true }
        };

        /// Auto consume flag, if set to true, Peers Joining the Room will automatically consume the media streams of the remote peers
        public bool AutoConsume => _autoConsume;
        private bool _autoConsume = true;

        private Dictionary<string, int> _activeSpeakers = new Dictionary<string, int> { { "size", 8 } };

        public RoomState State => _state;
        private RoomState _state = RoomState.idle;

        public Dictionary<string, string> Stats => _stats;
        private Dictionary<string, string> _stats = new Dictionary<string, string> { { "startTime", "0" } };

        public Dictionary<string, RemotePeer> RemotePeers => _remotePeers;
        private Dictionary<string, RemotePeer> _remotePeers = new Dictionary<string, RemotePeer>();

        public string Metadata => _metadata;
        private string _metadata;


        private Room()
        {

        }

        public Room Init(WebSocketService webSocketService, Permissions permissions, bool autoConsume)
        {
            _webSocketService = webSocketService;
            _permissions = permissions;
            _autoConsume = autoConsume;

            return Instance;
        }


        private async void RemoveLobbyPeer(string peerId)
        {
            if (_lobbyPeers.ContainsKey(peerId))
            {
                _lobbyPeers.Remove(peerId);
                LobbyPeerUpdated?.Invoke(_lobbyPeers.Keys.ToList());
                await Emit(EventConstantStrings.LobbyPeerUpdatedEventString, _lobbyPeers.Keys.ToList());
            }
        }

        public void SetRoomSate(RoomState roomState)
        {
            if (_state != roomState)
            {
                _state = roomState;
            }
        }

        public List<string> GetLobbyPeerIds()
        {
            return _lobbyPeers.Keys.ToList();
        }

        public async void SetLobbyPeersMap(Dictionary<string, object> lobbyPeers)
        {
            _lobbyPeers = lobbyPeers;
            LobbyPeerUpdated?.Invoke(_lobbyPeers.Keys.ToList());
            await Emit(EventConstantStrings.LobbyPeerUpdatedEventString, _lobbyPeers.Keys.ToList());
        }

        public void SetRoomStats(Dictionary<string, string> stats)
        {
            _stats = stats;
        }

        public Dictionary<string, object> GetLobbyPeerMetadata(string peerId)
        {

            if (_lobbyPeers.TryGetValue(peerId, out var lobbyPeerObj) && lobbyPeerObj is Dictionary<string, object> lobbyPeer)
            {
                var metadata = new Dictionary<string, object>();

                if (lobbyPeer.TryGetValue("metadata", out var metadataValue) && metadataValue is string lobbyMetadata)
                {
                    metadata = JsonConvert.DeserializeObject<Dictionary<string, object>>(lobbyMetadata) ?? new Dictionary<string, object>();
                }

                return new Dictionary<string, object>
                {
                    { "peerId", peerId },
                    { "metadata", metadata }
                };
            }

            return new Dictionary<string, object>
            {
                { "peerId", peerId },
                { "metadata", new Dictionary<string, object>() }
            };
        }

        public async void SetNewLobbyPeers(List<object> peers)
        {
            foreach (var peerObj in peers)
            {
                if (peerObj is Dictionary<string, object> peer)
                {
                    if (peer.TryGetValue("peerId", out var peerIdValue) && peerIdValue is string peerId)
                    {
                        _lobbyPeers[peerId] = peer;
                    }
                }
            }

            LobbyPeerUpdated?.Invoke(_lobbyPeers.Keys.ToList());
            await Emit(EventConstantStrings.LobbyPeerUpdatedEventString, _lobbyPeers.Keys.ToList());
        }

        public async void SetMetadata(string metadata)
        {
            _metadata = metadata;
            MetadataUpdated?.Invoke(metadata);
            await Emit(EventConstantStrings.MetadataUpdatedEventString, _metadata);

        }

        public T GetMetadata<T>()
        {
            var data = JsonConvert.DeserializeObject<T>(_metadata ?? "{}");
            return data;
        }

        public void UpdateMetadata<T>(T data)
        {
            if (_state == RoomState.closed || _state == RoomState.failed || _state == RoomState.left)
            {
                Debug.Log($"Cannot Update Metadata, You have not joined the room yet");
                return;
            }

            _metadata = JsonConvert.SerializeObject(data);
            _webSocketService.SendRequest(_webSocketService.RequestPublisher.UpdateRoomMetadataRequest(_metadata));
        }

        public void SetRoomId(string roomId)
        {
            if (!string.IsNullOrEmpty(_roomId))
            {
                Debug.LogError("RoomId is already set, Ignoring the new roomId, end this room and create a new room");
                return;
            }

            _roomId = roomId;
        }

        public void SetSessionId(string sessionId)
        {
            if (!string.IsNullOrEmpty(_sessionId))
            {
                Debug.LogError("SessionId is already set, Ignoring the new sessionId, end this room and create a new room");
                return;
            }

            _sessionId = sessionId;
        }

        public List<string> GetPeerIds()
        {
            return _remotePeers.Keys.ToList();
        }

        public async void UpdateRoomControl(Dictionary<string, object> data)
        {
            if (!_permissions.CheckPermission(PermissionType.admin, null))
            {
                return;
            }

            if (_config.ContainsKey(data["type"].ToString()))
            {
                _config[data["type"].ToString()] = data["value"];
            }
            else
            {
                _config.Add(data["type"].ToString(), data["value"]);
            }

            RoomControlsUpdated?.Invoke();
            await Emit(EventConstantStrings.RoomControlsUpdatedEventString);

            if (data["type"].ToString() == "allowProduceSources")
            {
                Dictionary<string, bool> allowProduceSources = _config["allowProduceSources"] as Dictionary<string, bool>;
                Request request = _webSocketService.RequestPublisher.UpdateRoomControlsRequest(new UpdateRoomControls
                {
                    ProduceSourcesControl = new ProduceSourcesControl
                    {
                        Type = "produceSourcesControl",
                        Value = new global::ProduceSources
                        {
                            Cam = allowProduceSources["cam"],
                            Mic = allowProduceSources["mic"],
                            Screen = allowProduceSources["screen"],
                        }
                    }
                });

                _webSocketService.SendRequest(request);

            }
            else
            {
                Request request = _webSocketService.RequestPublisher.UpdateRoomControlsRequest(new UpdateRoomControls
                {
                    RoomControl = new RoomControlType
                    {
                        Type = data["type"].ToString(),
                        Value = (bool)data["value"]
                    }
                });

                _webSocketService.SendRequest(request);
            }

        }

        public void CloseStreamOfLabel(string label, List<string> peerIds)
        {
            if (!_permissions.CheckPermission(PermissionType.admin, null))
            {
                return;
            }

            RepeatedField<string> tempPeerIds = new RepeatedField<string>();

            foreach (var peerId in peerIds)
            {
                tempPeerIds.Add(peerId);
            }

            _webSocketService.SendRequest(_webSocketService.RequestPublisher.CloseStreamOfLabelRequest(label, tempPeerIds));
        }

        public void MuteEveryone()
        {
            if (!_permissions.CheckPermission(PermissionType.admin, null))
            {
                return;
            }

            _webSocketService.SendRequest(_webSocketService.RequestPublisher.CloseStreamOfLabelRequest("audio"));
        }

        public RemotePeer RemotePeerExists(string peerId)
        {
            RemotePeer remotePeer;
            if (_remotePeers.TryGetValue(peerId, out remotePeer))
            {
                return remotePeer;
            }

            return null;
        }

        public RemotePeer GetRemotePeerById(string peerId)
        {
            RemotePeer remotePeer;
            if (_remotePeers.TryGetValue(peerId, out remotePeer))
            {
                return remotePeer;
            }
            if (remotePeer == null)
            {
                throw new Exception("Remote Peer Not Found, peerId: $peerId");
            }

            return null;
        }

        public void AddNewJoinedPeer(string peerId,RemotePeer remotePeer) 
        {
            _remotePeers[peerId] = remotePeer;
        }

        public Room Connect(string roomId)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                Debug.LogError($"Room Id is required to connect to the room");
                throw new Exception($"Room Id is required to connect to the room");
            }

            _roomId = roomId;

            if (!_webSocketService.IsConnected)
            {
                throw new Exception($"Socket is Not Connected");
            }

            if (!_webSocketService.IsConnecting)
            {
                throw new Exception($"Socket is Connecting, Wait for it to be connected");
            }

            _webSocketService.SendRequest(_webSocketService.RequestPublisher.ConnectRoomRequest(roomId));
            _state = RoomState.connecting;

            //emit('room-connecting');
            RoomConnecting?.Invoke();
           _ = Emit(EventConstantStrings.RoomConnectingEventString);

            return this;
        }

        public void AdmitPeer(string peerId)
        {
            if (!_permissions.CheckPermission(PermissionType.admin, null))
            {
                return;
            }

            RemoveLobbyPeer(peerId);
            _webSocketService.SendRequest(_webSocketService.RequestPublisher.AcceptLobbyPeerRequest(peerId));
        }

        public void DebyPeer(string peerId)
        {
            if (!_permissions.CheckPermission(PermissionType.admin, null))
            {
                return;
            }

            _webSocketService.SendRequest(_webSocketService.RequestPublisher.DenyLobbyPeerRequest(peerId));
        }

        public void KickPeer(string peerId)
        {
            if (!_permissions.CheckPermission(PermissionType.admin, null))
            {
                return;
            }

            _webSocketService.SendRequest(_webSocketService.RequestPublisher.KickPeerRequest(peerId));
        }

        public void SetRemotePeersDic(Dictionary<string, RemotePeer> remotePeers) 
        {
            _remotePeers = remotePeers;
        }

        public async void Close(string reason)
        {
            _roomId = null;
            _remotePeers.Clear();
            _lobbyPeers.Clear();
            _metadata = null;
            _state = RoomState.left;
            //Emit room-closed
            RoomClosed?.Invoke(reason ?? "LEFT");
            await Emit(EventConstantStrings.RoomClosedEventString, new Dictionary<string, string> { { "reason", reason ?? "LEFT" } });
        }

        public void LeaveRoom() 
        {
            _ = _webSocketService.CloseAsync(1000, "CloseRoom",false);
        }

    }

    public enum RoomState
    {
        idle,
        connecting,
        connected,
        failed,
        left,
        closed
    }
}

#endif




