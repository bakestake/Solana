#if !UNITY_WEBGL


using System.Collections.Generic;
using UnityEngine;
using Huddle01.Core.Services;
using Huddle01.Core.EventBroadcast;
using System;
using Mediasoup;
using Mediasoup.Internal;
using Mediasoup.RtpParameter;
using Huddle01.Helper;
using Mediasoup.Transports;
using Google.Protobuf.Collections;
using System.Linq;
using Unity.WebRTC;
using System.Threading.Tasks;
using System.Text;
using EnumExtensions = Huddle01.Helper.EnumExtensions;
using Mediasoup.SctpParameter;
using Huddle01.Settings;
using System.Collections;

namespace Huddle01.Core
{
    public class LocalPeer : EventEmitter
    {
        private static LocalPeer _instance = null;

        public static LocalPeer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocalPeer();
                }
                return _instance;
            }
        }


        private const string SendString = "send";
        private const string RecvString = "recv";

        public string RoomId => _roomId;
        private string _roomId;

        private EventsBroadcaster _eventsBroadcaster;

        private Device _device;
        private RtpCapabilities _routerRtpCapabilitiesObj;

        public WebSocketService _webSocketService;

        public string PeerId => _peerId;
        private string _peerId;

        private Transport<Mediasoup.Types.AppData> _sendTransport;
        private Transport<Mediasoup.Types.AppData> _recvTransport;

        public Dictionary<string, RemotePeer> RemotePeers;
        private Dictionary<string, RemotePeer> _remotePeers = new Dictionary<string, RemotePeer>();
       
        private List<Func<Task>> _waitingToConsume = new List<Func<Task>>();
        private Dictionary<string, Func<Task>> _waitingToProduce = new Dictionary<string, Func<Task>>();
        private Dictionary<string, TaskCompletionSource<string>> _pendingProducerTasks = new Dictionary<string, TaskCompletionSource<string>>();

        private Dictionary<string, TaskCompletionSource<Consumer<Mediasoup.Types.AppData>>> _pendingConsumerTasks = new Dictionary<string, TaskCompletionSource<Consumer<Mediasoup.Types.AppData>>>();

        public EnhancedMap<Consumer<Mediasoup.Types.AppData>> Consumers => _consumers;
        private EnhancedMap<Consumer<Mediasoup.Types.AppData>> _consumers = new EnhancedMap<Consumer<Mediasoup.Types.AppData>>();

        private Permissions _permissions;
        private string _metadata;

        private int maxDataMessageSize = 1024;

        private Dictionary<string, MediaStream> _activeStreams = new Dictionary<string, MediaStream>();

        private bool _joined = false;

        private Dictionary<string, Task<MediaStream>> _pendingFetchingStream = new Dictionary<string, Task<MediaStream>>();

        private Dictionary<string, string> _labelToProducerId = new Dictionary<string, string>();

        private Dictionary<string, Producer<Mediasoup.Types.AppData>> _producers = new Dictionary<string, Producer<Mediasoup.Types.AppData>>();

        public string SelectedVideoStreamDevice = "0";
        public string SelectedAudioStreamDevice = "0";

        private Dictionary<TransportType, Task<Transport<Mediasoup.Types.AppData>>> _pendingTransportTasks =
                                    new Dictionary<TransportType, Task<Transport<Mediasoup.Types.AppData>>>();

        private DeviceHandler _deviceHandler;

        private Dictionary<string, TaskCompletionSource<string>> _getProducerIdTcs = new Dictionary<string, TaskCompletionSource<string>>();

        private TaskCompletionSource<bool> _createTransportTcs;

        private LocalPeer()
        {

        }


        public LocalPeer Init(WebSocketService webSocketService, EventsBroadcaster eventsBroadcaster, DeviceHandler deviceHandler, Permissions permission)
        {
            _webSocketService = webSocketService;
            _eventsBroadcaster = eventsBroadcaster;
            _deviceHandler = deviceHandler;
            _permissions = permission;

            _eventsBroadcaster.OnEventRaised += HandleWebsocketEvents;
            _webSocketService.Reconnected += OnSocketReconnected;

            return _instance;
        }

        ~LocalPeer()
        {
            _eventsBroadcaster.OnEventRaised -= HandleWebsocketEvents;
            _webSocketService.Reconnected -= OnSocketReconnected;
        }

        private void OnSocketReconnected()
        {
            Request request = _webSocketService.RequestPublisher.SyncMeetingStateRequest();
            _webSocketService.SendRequest(request);
        }

        public void SetMicDevice(string micDevice) 
        {
            SelectedAudioStreamDevice = micDevice;
        }

        public void SetCameraDevice(string cameraDevice)
        {
            SelectedVideoStreamDevice = cameraDevice;
        }

        private void HandleWebsocketEvents(Response.ResponseOneofCase responseCase, Response response)
        {
            switch (responseCase)
            {
                case Response.ResponseOneofCase.Hello:
                    HelloReceived(response.Hello);
                    break;

                case Response.ResponseOneofCase.ConnectRoomResponse:
                    ConnectRoomResponseReceived(response.ConnectRoomResponse);
                    break;

                case Response.ResponseOneofCase.CreateTransportOnClient:
                    CreateTransportOnClientReceived(response.CreateTransportOnClient);
                    break;

                case Response.ResponseOneofCase.SyncMeetingStateResponse:
                    SyncMeetingStateResponseReceived(response.SyncMeetingStateResponse);
                    break;

                case Response.ResponseOneofCase.ProduceResponse:
                    ProduceResponseReceived(response.ProduceResponse);
                    break;

                case Response.ResponseOneofCase.ConsumeResponse:
                    ConsumeResponseReceived(response.ConsumeResponse);
                    break;

                case Response.ResponseOneofCase.CloseProducerSuccess:
                    CloseProducerSuccessReceived(response.CloseProducerSuccess);
                    break;

                case Response.ResponseOneofCase.CloseConsumerSuccess:
                    CloseConsumerSuccessReceived(response.CloseConsumerSuccess);
                    break;

                case Response.ResponseOneofCase.ConnectTransportResponse:
                    ConnectTransportResponseReceived(response.ConnectTransportResponse);
                    break;

                case Response.ResponseOneofCase.RestartTransportIceResponse:
                    RestartTransportIceResponseReceived(response.RestartTransportIceResponse);
                    break;

                case Response.ResponseOneofCase.NewPeerJoined:
                    NewPeerJoinedReceived(response.NewPeerJoined);
                    break;

                case Response.ResponseOneofCase.NewLobbyPeer:
                    NewLobbyPeerReceived(response.NewLobbyPeer);
                    break;

                case Response.ResponseOneofCase.NewPermissions:
                    NewPermissionsReceived(response.NewPermissions);
                    break;

                case Response.ResponseOneofCase.NewRoomControls:
                    NewRoomControlsReceived(response.NewRoomControls);
                    break;

                case Response.ResponseOneofCase.NewPeerRole:
                    NewPeerRoleReceived(response.NewPeerRole);
                    break;

                case Response.ResponseOneofCase.ReceiveData:
                    ReceiveDataReceived(response.ReceiveData);
                    break;

                case Response.ResponseOneofCase.PeerMetadataUpdated:
                    PeerMetadataUpdatedReceived(response.PeerMetadataUpdated);
                    break;

                case Response.ResponseOneofCase.RoomMetadataUpdated:
                    RoomMetadataUpdatedReceived(response.RoomMetadataUpdated);
                    break;

                case Response.ResponseOneofCase.RoomClosedProducers:
                    RoomClosedProducersReceived(response.RoomClosedProducers);
                    break;

                case Response.ResponseOneofCase.PeerLeft:
                    PeerLeftReceived(response.PeerLeft);
                    break;

                case Response.ResponseOneofCase.LobbyPeerLeft:
                    LobbyPeerLeftReceived(response.LobbyPeerLeft);
                    break;

                case Response.ResponseOneofCase.WaitingRoom:
                    WaitingRoomReceived(response.WaitingRoom);
                    break;

                case Response.ResponseOneofCase.Error:
                    ErrorReceived(response.Error);
                    break;

                default:
                    Debug.LogError("Unhandled response case: " + responseCase);
                    break;
            }
        }

        private async void ConnectRoomResponseReceived(ConnectRoomResponse connectRoomResponse)
        {
            Debug.Log(connectRoomResponse.RouterRTPCapabilities);
            _device = new Device();
            _routerRtpCapabilitiesObj = new RtpCapabilities();

            if (connectRoomResponse?.RouterRTPCapabilities != null)
            {
                if (connectRoomResponse.RouterRTPCapabilities.Codecs != null)
                {
                    foreach (var codec in connectRoomResponse.RouterRTPCapabilities.Codecs)
                    {
                        var rtpCodecCapability = new RtpCodecCapability
                        {
                            Kind = codec.Kind == "audio" ? MediaKind.AUDIO : MediaKind.VIDEO,
                            MimeType = codec.MimeType,
                            PreferredPayloadType = (byte?)codec.PreferredPayloadType,
                            ClockRate = (uint)codec.ClockRate,
                            Channels =  codec.Channels==0 ? null: (byte?)codec.Channels,
                            RtcpFeedback = new List<RtcpFeedback>()
                        };

                        if (codec.Parameters != null)
                        {
                            var parametersDictionary = new Dictionary<string, object>();
                            foreach (var parameter in codec.Parameters)
                            {
                                parametersDictionary[parameter.Key] = parameter.Value;
                            }
                            rtpCodecCapability.Parameters = parametersDictionary;
                        }

                        _routerRtpCapabilitiesObj.Codecs.Add(rtpCodecCapability);

                        if (codec.RtcpFeedback != null)
                        {
                            foreach (var feedback in codec.RtcpFeedback)
                            {
                                rtpCodecCapability.RtcpFeedback.Add(new RtcpFeedback
                                {
                                    Type = feedback.Type,
                                    Parameter = feedback.Parameter
                                });
                            }
                        }
                    }
                }

                if (connectRoomResponse.RouterRTPCapabilities.HeaderExtensions != null)
                {
                    foreach (var extension in connectRoomResponse.RouterRTPCapabilities.HeaderExtensions)
                    {
                        var rtpHeaderExtension = new RtpHeaderExtension
                        {
                            Kind = ProtoEnumHelper.GetMediaKind(extension.Kind),
                            Uri = ProtoEnumHelper.GetRtpHeaderExtensionUri(extension.Uri) ?? default(RtpHeaderExtensionUri),
                            PreferredId = (byte)extension.PreferredId,
                            Direction = ProtoEnumHelper.GetRtpHeaderExtensionDirection(extension.Direction),
                        };

                        _routerRtpCapabilitiesObj.HeaderExtensions.Add(rtpHeaderExtension);
                    }
                }
            }
            try
            {
                await _device.Load(_routerRtpCapabilitiesObj);
                if (_device.IsLoaded())
                {
                    Debug.Log("Device loaded successfully");
                }

                _ = Emit("device-created",_device);

                SetRemotePeers(connectRoomResponse);

                foreach (LobbyPeers item in connectRoomResponse.RoomInfo.LobbyPeers)
                {
                     Room.Instance.LobbyPeers.Add(item.PeerId, new RemotePeer(item.PeerId, item.Metadata));
                }

                Room.Instance.SetRoomSate(RoomState.connected);
                _joined = true;
                _ = Room.Instance.Emit("room-joined");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading the device: {ex.Message}");
            }
        }

        private void HelloReceived(Hello hello)
        {
            Debug.Log($"Hellow received : {hello.RoomId}");
            _roomId = hello.RoomId;
            _peerId = hello.PeerId;
            //ConnectRoomRequest();
        }

        private void CreateTransportOnClientReceived(CreateTransportOnClient createTransportOnClient)
        {
            var transportType = createTransportOnClient.TransportType;
            try
            {
                if (string.IsNullOrEmpty(_peerId))
                {
                    throw new Exception("Cannot Create Transport, No PeerId Found for the user.");
                }

                Debug.Log($"CreateTransportOnClientReceived {Newtonsoft.Json.JsonConvert.SerializeObject(createTransportOnClient)}");

                CreateDeviceTransport(createTransportOnClient, transportType);

                if (transportType.Equals(SendString))
                {
                    Debug.Log($"Emit new-send-transport");
                    Emit("new-send-transport", _sendTransport);
                }

                if (transportType.Equals(RecvString))
                {
                    Emit("new-recv-transport", _recvTransport);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Creating MediasoupTransport On Client Transportype => {transportType}");
            }
        }

        private void SyncMeetingStateResponseReceived(SyncMeetingStateResponse syncMeetingStateResponse)
        {
            List<PeersInfo> latestPeers = new List<PeersInfo>();

            HashSet<string> latestPeersSet = new HashSet<string>();

            foreach (var peer in syncMeetingStateResponse.RoomInfo.Peers)
            {
                latestPeers.Add(peer);
            }

            if (latestPeers != null && latestPeers.Count > 0)
            {
                latestPeers = latestPeers.Select(p => new PeersInfo
                {
                    PeerId = p.PeerId
                }).ToList();

                latestPeersSet = new HashSet<string>(latestPeers.Select(p => p.PeerId));
            }

            foreach (var entry in _remotePeers)
            {
                string peerId = entry.Key;
                RemotePeer peer = entry.Value;

                if (!latestPeersSet.Contains(peerId))
                {
                    foreach (var label in peer.Labels)
                    {
                        CloseRemotePeerConsumer(peerId, label);
                    }

                    peer.Close(peerId);
                    _remotePeers.Remove(peerId);

                    _ = Room.Instance.Emit("peer-left", peerId);

                    continue;
                }

                var latestPeerInfo = latestPeers.FirstOrDefault(p => p.PeerId == peerId);

                if (latestPeerInfo == null)
                {
                    continue;
                }

                HashSet<string> newProducerSet = new HashSet<string>(
                    latestPeerInfo.Producers.Select(p => p.Label)
                );

                foreach (var label in peer.Labels)
                {
                    if (!newProducerSet.Contains(label))
                    {
                        CloseRemotePeerConsumer(peerId, label);
                    }
                }

                HashSet<string> currentProducerSet = new HashSet<string>(peer.ProducerIds);

                foreach (var producer in latestPeerInfo.Producers)
                {
                    if (!currentProducerSet.Contains(producer.Id))
                    {
                        // Check transport connection state
                        if (_recvTransport == null ||
                            _recvTransport.connectionState == RTCIceConnectionState.New ||
                            _recvTransport.connectionState == RTCIceConnectionState.Checking)
                        {
                            // Add new producer data to peer
                            _ = peer.AddLabelData(producer.Label, producer.Id);
                        }
                        else
                        {
                            _waitingToConsume.Add(() =>
                            {
                                return peer.AddLabelData(producer.Label, producer.Id);
                            });
                        }
                    }
                    else
                    {
                        Consumer<Mediasoup.Types.AppData> consumer = peer.GetConsumer(producer.Label);
                        if (producer.Paused)
                        {
                            consumer.Pause();
                            _ = peer.Emit("stream-paused", new
                            {
                                label = producer.Label,
                                peerId = peerId,
                                producerId = producer.Id
                            });
                        }
                        else
                        {
                            consumer.Resume();
                            _ = peer.Emit("stream-playable", consumer, producer.Label); 
                        }
                    }
                }
            }


            var filteredPeers = latestPeers
                                .Where(latestPeer => !_remotePeers.ContainsKey(latestPeer.PeerId) && latestPeer.PeerId != _peerId)
                                .ToList();

            foreach (var latestPeer in filteredPeers)
            {
                var remotePeer = new RemotePeer(latestPeer.PeerId, latestPeer.Metadata, latestPeer.Role);

                // remotePeer.PeerId = latestPeer.PeerId;
                remotePeer.Role = latestPeer.Role;
                //remotePeer.MetaData = latestPeer.Metadata;

                _remotePeers[latestPeer.PeerId] = remotePeer;

                var remoteProducers = latestPeer.Producers;

                foreach (var p in remoteProducers)
                {
                    // Check the transport connection state
                    if (_recvTransport == null ||
                        _recvTransport.connectionState == RTCIceConnectionState.New ||
                        _recvTransport.connectionState == RTCIceConnectionState.Connected)
                    {
                        _ = remotePeer.AddLabelData(p.Id, p.Label);
                    }
                    else
                    {
                         _waitingToConsume.Add(()=> 
                        {
                            return remotePeer.AddLabelData(p.Id, p.Label);
                        });
                    }
                }

                _ = Room.Instance.Emit("new-peer-joined", new { peer = remotePeer });

            }
        }

        private void CreateDeviceTransport(CreateTransportOnClient createTransportOnClient, string transportType)
        {
            try
            {
                Debug.Log("CreateDeviceTransport");
                Transport<Mediasoup.Types.AppData> transport = null;

                string id = createTransportOnClient.TransportSDPInfo.Id;
                Debug.Log("Get IceParam");

                IceParameters iceParam = new IceParameters
                {
                    iceLite = createTransportOnClient.TransportSDPInfo.IceParameters.IceLite,
                    password = createTransportOnClient.TransportSDPInfo.IceParameters.Password,
                    usernameFragment = createTransportOnClient.TransportSDPInfo.IceParameters.UsernameFragment
                };

                List<IceCandidate> iceCandidates = new List<IceCandidate>();

                foreach (ProtoIceCandidates candi in createTransportOnClient.TransportSDPInfo.IceCandidates)
                {
                    IceCandidate candidate = new IceCandidate
                    {
                        address = candi.Address,
                        foundation = candi.Foundation,
                        ip = candi.Ip,
                        port = candi.Port,
                        priority = candi.Priority,
                        protocol = candi.Protocol,
                        tcpType = candi.TcpType,
                        type = candi.Type
                    };

                    iceCandidates.Add(candidate);
                }

                DtlsParameters dtlsParam = new DtlsParameters();
                List<DtlsFingerprint> dtlsFingerprints = new List<DtlsFingerprint>();
                foreach (ProtoDtlsFingerPrints fingerPrint in createTransportOnClient.TransportSDPInfo.DtlsParameters.Fingerprints)
                {
                    DtlsFingerprint fp = new DtlsFingerprint
                    {
                        algorithm = EnumExtensions.GetEnumByStringValue<FingerPrintAlgorithm>(fingerPrint.Algorithm),
                        value = fingerPrint.Value
                    };

                    dtlsFingerprints.Add(fp);
                }
                dtlsParam.fingerprints = dtlsFingerprints;

                //Fix this after wards
                if (createTransportOnClient.TransportSDPInfo.DtlsParameters.Role.Equals("auto"))
                {
                    dtlsParam.role = DtlsRole.auto;
                }
                else if (createTransportOnClient.TransportSDPInfo.DtlsParameters.Role.Equals("client"))
                {
                    dtlsParam.role = DtlsRole.client;
                }
                else if (createTransportOnClient.TransportSDPInfo.DtlsParameters.Role.Equals("server"))
                {
                    dtlsParam.role = DtlsRole.server;
                }

                SctpParameters sctpParam = new SctpParameters
                {
                    maxMessageSize = createTransportOnClient.TransportSDPInfo.SctpParameters.MaxMessageSize,
                    mis = createTransportOnClient.TransportSDPInfo.SctpParameters.MIS,
                    os = createTransportOnClient.TransportSDPInfo.SctpParameters.OS,
                    port = createTransportOnClient.TransportSDPInfo.SctpParameters.Port
                };


                if (transportType.Equals(SendString))
                {
                    _sendTransport = _device.CreateSendTransport(id, iceParam, iceCandidates, dtlsParam, sctpParam,
                                                                    null, null, null, new Dictionary<string, object>(), new Mediasoup.Types.AppData());

                    

                    transport = _sendTransport;

                }

                if (transportType.Equals(RecvString))
                {
                    _recvTransport = _device.CreateRecvTransport(id, iceParam, iceCandidates, dtlsParam, sctpParam,
                                                                    null, RTCIceTransportPolicy.All, null, new Dictionary<string, object>(), new Mediasoup.Types.AppData());

                    transport = _recvTransport;
                }

                Debug.Log("Transport has been created");

                transport.On("connectionstatechange", async (args) =>
                {
                    string state = (string)args[0];

                    HandleConnectionStateChange(transport, state, transportType);
                });

                transport.On("connect", async (args) =>
                {
                    DtlsParameters dtlsParameters = (DtlsParameters)args[0];
                    Action callBack = (Action)args[1];
                    Action<Exception> errBack = (Action<Exception>)args[2];

                    ProtoDtlsParameters protoDtlsParameters = GetDtlsParam(dtlsParameters);
                    /*try
                    {
                        Once("connectTransportResponse", async (args) => callBack.Invoke());

                        Request request = new Request
                        {
                            ConnectTransport = new ConnectTransport
                            {
                                DtlsParameters = protoDtlsParameters,
                                TransportType = transportType
                            }
                        };

                        _webSocketService.SendRequest(request);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log($"Error Transport Connect Event | error: {ex}");

                    }*/
                });

                transport.On("produce", async (args) => 
                {
                     
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Create Device Transport | error: {ex}");
            }
        }

        private void HandleConnectionStateChange(Transport<Mediasoup.Types.AppData> transport, string state, string transportType)
        {
            try
            {
                Debug.Log($"{transportType} Transport Connection State Changed, state: {state}");

                Dictionary<string, Action> handler = new Dictionary<string, Action>
                {
                    { "connected", () => HandleConnected(transport, transportType) },
                    { "disconnected", () => HandleDisconnected(transport, transportType) },
                    { "failed", () => Debug.LogError($"{transportType} Transport Failed") },
                    { "connecting", () => Debug.Log($"{transportType} Transport Connecting") },
                    { "closed", () => Debug.Log($"{transportType} Transport Closed") },
                    { "new", () => Debug.Log($"{transportType} Transport New") }
                };

                if (handler.TryGetValue(state, out Action? action))
                {
                    action.Invoke();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine($"❌ Error in connectionStateChangeHandler | error: {err}");
            }
        }

        private void HandleConnected(Transport<Mediasoup.Types.AppData> transport, string transportType)
        {
            Console.WriteLine($"🔔 {transportType} Transport Connected");
            if (transportType == "send")
            {
                HandleWaitingToProduce();
            }
            else if (transportType == "recv")
            {
                HandleWaitingToConsume();
            }
        }

        private bool _iceRestartDebounce = false;

        private void HandleDisconnected(Transport<Mediasoup.Types.AppData> transport, string transportType)
        {
            if (_iceRestartDebounce)
            {
                return;
            }

            _iceRestartDebounce = true;

            Request request = new Request
            {
                RestartTransportIce = new RestartTransportIce
                {
                    TransportId = transport.id,
                    TransportType = transportType
                }
            };

            Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ => _iceRestartDebounce = false);
            Debug.Log($"{transportType} Transport Disconnected");
        }

        
        private void ProduceResponseReceived(ProduceResponse produceResponse)
        {
            Debug.Log($"ProduceResponseReceived");

            try
            {
                string peerId = produceResponse.PeerId;
                string producerId = produceResponse.ProducerId;
                string label = produceResponse.Label;

                if (peerId == _peerId)
                {
                    if (_pendingProducerTasks.ContainsKey(label))
                    {
                        Debug.Log($"Got producer id from server");
                        _pendingProducerTasks[label].SetResult(producerId);
                        _pendingProducerTasks.Remove(label);
                    }
                }
                else
                {
                    var remotePeer = Room.Instance.GetRemotePeerById(peerId);

                    if (_recvTransport == null ||
                        _recvTransport.connectionState == RTCIceConnectionState.New ||
                        _recvTransport.connectionState == RTCIceConnectionState.Connected)
                    {
                      _ =  remotePeer.AddLabelData(label, producerId);
                    }
                    else
                    {
                        _waitingToConsume.Add(() =>
                        {
                            return remotePeer.AddLabelData(label, producerId);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error Produce Response | error: {ex.Message}");
            }
        }

        private void ConsumeResponseReceived(ConsumeResponse consumeResponse)
        {
            RemotePeer remotePeer = Room.Instance.GetRemotePeerById(consumeResponse.ProducerPeerId);
            if (!remotePeer.HasLabel(consumeResponse.Label))
            {
                Debug.LogError("Remote Peer is not producing this label");
            }

            Mediasoup.Types.AppData appdata = new Mediasoup.Types.AppData();
            appdata.Add("source", consumeResponse.Label);
            appdata.Add("producerPeerId", consumeResponse.ProducerId);
            appdata.Add("label", consumeResponse.Label);
            appdata.Add("consumeResponse", consumeResponse);

            Debug.Log($"proto rtp {Newtonsoft.Json.JsonConvert.SerializeObject(consumeResponse)}");

            ConsumerOptions consumerOptions = new ConsumerOptions
            {
                appData = appdata,
                producerId = consumeResponse.ProducerId,
                kind = consumeResponse.Kind,
                rtpParameters = GetRtpParametersFromConsumerResponse(consumeResponse),
                id = consumeResponse.ConsumerId

            };

            _recvTransport.ConsumeAsync<Mediasoup.Types.AppData>(consumerOptions, ConnectTransportToServerSuccess, PostConsume);
        }

        private void PostConsume(Consumer<Mediasoup.Types.AppData> consumer)
        {
            Debug.Log("PostConsume");
            try
            {
                var appDataDic = consumer.appData as Mediasoup.Types.AppData;

                Request resumeConsumerRequest = new Request
                {
                    ResumeConsumer = new ResumeConsumer
                    {
                        ConsumerId = consumer.id,
                        ProducerPeerId = appDataDic["producerPeerId"].ToString()
                    }
                };

                Debug.Log($"PostConsume : ");
                _webSocketService.SendRequestWithoutWait(resumeConsumerRequest);

                string label = appDataDic["label"] as string;
                string producerId = appDataDic["producerPeerId"] as string;
                ConsumeResponse consumeResponse = appDataDic["consumeResponse"] as ConsumeResponse;
                Debug.Log($"PostConsume : {Newtonsoft.Json.JsonConvert.SerializeObject(consumeResponse)}");

                if (!consumeResponse.ProducerPaused)
                {
                    consumer.Resume();
                    _ = Room.Instance.Emit("stream-added", label, consumeResponse.ProducerPeerId);

                    RemotePeer remotePeer = Room.Instance.GetRemotePeerById(consumeResponse.ProducerPeerId);
                    _ = remotePeer.Emit("stream-playable", consumer, label);
                }
                else 
                {
                    consumer.Pause();
                    RemotePeer remotePeer = Room.Instance.GetRemotePeerById(consumeResponse.ProducerPeerId);
                    _ = remotePeer.Emit("stream-available", label, consumeResponse.ProducerId);
                }

                Consumer<Mediasoup.Types.AppData> tempconsumer = _consumers.Get(label, producerId);

                if (tempconsumer != null)
                {
                    _consumers.Set(label, producerId, tempconsumer);
                    tempconsumer.Resume();
                    Emit("consume",label, producerId);
                }

                tempconsumer?.On("transportclose", async (args) =>
                {
                    CloseConsumer(label, producerId);
                });

                tempconsumer?.On("@close", async (args) =>
                {
                    CloseConsumer(label, producerId);
                });

            }
            catch (Exception ex)
            {
                Debug.LogError($"{ex.Message}");
                throw new Exception($"Error Consume Response | error: {ex}");
            }
        }

        private void CloseProducerSuccessReceived(CloseProducerSuccess closeProducerSuccess)
        {
            if (_peerId.Equals(closeProducerSuccess.PeerId))
            {
                return;
            }

            string peerId = closeProducerSuccess.PeerId;
            string label = closeProducerSuccess.Label;

            try
            {
                CloseRemotePeerConsumer(peerId, label);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Closing Producer | error: {ex}");
            }
        }

        private void CloseRemotePeerConsumer(string peerId, string label)
        {
            try
            {
                RemotePeer remotePeer = Room.Instance.GetRemotePeerById(peerId);
                remotePeer.RemoveLabelData(label);
                Consumer<Mediasoup.Types.AppData> consumer = GetConsumer(label, peerId);

                if (consumer != null)
                {
                    CloseConsumer(label, peerId);
                    _consumers.Delete(label, peerId);
                    if (label.Equals("video"))
                    {
                        CustomValueNotifierHolder.PeersNotifier.Value = new Dictionary<string, object>(CustomValueNotifierHolder.PeersNotifier.Value);
                        CustomValueNotifierHolder.PeersNotifier.Value[peerId] = null;
                    }
                }

               _= Room.Instance.Emit("stream-closed", label, peerId);
               _= remotePeer.Emit("stream-closed", label, peerId);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error Closing Remote Peer's Consumer | error: {ex}");
            }
        }

        private void CloseConsumerSuccessReceived(CloseConsumerSuccess closeConsumerSuccess)
        {
            Debug.Log($"Consumer closed {closeConsumerSuccess.PeerId}");
        }

        private void ConnectTransportResponseReceived(ConnectTransportResponse connectTransportResponse)
        {
            Debug.Log($"Connect {connectTransportResponse.TransportType} trnasport on server response");
            try
            {
                string transportType = connectTransportResponse.TransportType;
                Transport<Mediasoup.Types.AppData> transport;
                if (transportType.Equals("send"))
                {
                    transport = _sendTransport;
                }
                else
                {
                    transport = _recvTransport;
                }

                if (transport == null)
                {
                    Debug.LogError($"{transportType} Transport Not Initialized");
                    throw new Exception($"{transportType} Transport Not Initialized");
                }

                transport.Emit("connectTransportResponse");
                _createTransportTcs.SetResult(true);
                _createTransportTcs = null;

            }
            catch (Exception ex)
            {
                Debug.LogError($"Error Connecting Transport On Server Response | error: {ex}");
            }
        }

        private void RestartTransportIceResponseReceived(RestartTransportIceResponse restartTransportIceResponse)
        {
            string transportType = restartTransportIceResponse.TransportType;
            IceParameters iceParameters = new IceParameters();// restartTransportIceResponse.IceParameters;
            iceParameters.usernameFragment = restartTransportIceResponse.IceParameters.UsernameFragment;
            iceParameters.password = restartTransportIceResponse.IceParameters.Password;
            iceParameters.iceLite = restartTransportIceResponse.IceParameters.IceLite;

            Transport<Mediasoup.Types.AppData> transport = (transportType == SendString) ? _sendTransport : _recvTransport;
            if (transport == null)
            {
                Debug.LogError("Transport Not Found");
            }

            transport.RestartIceAsync(iceParameters);


        }

        void NewPeerJoinedReceived(NewPeerJoined newPeerJoined)
        {
            if (_peerId?.Equals(newPeerJoined.PeerId) == true) return;

            try
            {
                string peerId = newPeerJoined.PeerId ?? string.Empty;
                string role = newPeerJoined.Role ?? string.Empty;
                string metadata = newPeerJoined.Metadata ?? string.Empty;

                RemotePeer remotePeer = new RemotePeer(peerId, metadata, role);

                _remotePeers ??= new Dictionary<string, RemotePeer>();
                _remotePeers[peerId] = remotePeer;

                CustomValueNotifierHolder.PeersNotifier.Value ??= new Dictionary<string, object>();
                CustomValueNotifierHolder.PeersNotifier.Value[peerId] = null;
                Room.Instance.AddNewJoinedPeer(peerId,remotePeer);
                _ = Room.Instance.Emit("new-peer-joined", remotePeer);
                
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error New Peer Joined | error: {ex}");
            }
        }

        void NewLobbyPeerReceived(NewLobbyPeer newLobbyPeer)
        {
            try
            {
                Debug.Log($"New lobby peer {newLobbyPeer.PeerId}");

                Dictionary<string, object> metadata = new Dictionary<string, object>();

                Room.Instance.LobbyPeers.Add(newLobbyPeer.PeerId, newLobbyPeer.Metadata);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error New Lobb Peer | Error: {ex}");
            }
        }

        void NewPermissionsReceived(NewPermissions newPermissions)
        {
            try
            {
                _permissions.UpdatePermissions(newPermissions.Acl);
                _ = Emit("permissions-updated", _permissions);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error Updating Permissions | Error: {ex}");
            }
        }

        void NewRoomControlsReceived(NewRoomControls newRoomControls)
        {
            Debug.Log("Received New Room Controls");
            try
            {
                var newRoomControlsData = new Dictionary<string, object>
                {
                    {
                        "allowConsume", new Dictionary<string, object>
                        {
                            { "type", "allowProduce" },
                            { "value", newRoomControls.Controls.AllowConsume }
                        }
                    },
                    {
                        "allowProduce", new Dictionary<string, object>
                        {
                            { "type", "allowProduce" },
                            { "value", newRoomControls.Controls.AllowProduce }
                        }
                    },
                    {
                        "allowProduceSources", new Dictionary<string, object>
                        {
                            { "type", "allowProduceSources" },
                            { "value", new Dictionary<string, object>
                                {
                                    { "cam", newRoomControls.Controls.AllowProduceSources.Cam },
                                    { "mic", newRoomControls.Controls.AllowProduceSources.Mic },
                                    { "screen", newRoomControls.Controls.AllowProduceSources.Screen }
                                }
                            }
                        }
                    },
                    {
                        "allowSendData", new Dictionary<string, object>
                        {
                            { "type", "allowSendData" },
                            { "value", newRoomControls.Controls.AllowSendData }
                        }
                    },
                    {
                        "roomLocked", new Dictionary<string, object>
                        {
                            { "type", "roomLocked" },
                            { "value", newRoomControls.Controls.RoomLocked }
                        }
                    }
                };
                Room.Instance.UpdateRoomControl(newRoomControlsData);
                _ = Emit("room-controls-updated", newRoomControlsData);
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error Updating Room Controls | error: ${ex.Message}");
            }
        }

        void NewPeerRoleReceived(NewPeerRole newPeerRole)
        {
            Debug.Log($"Received New Peer's Role | data {newPeerRole.PeerId}");
            try
            {
                string peerId = newPeerRole.PeerId;
                string role = newPeerRole.Role;

                if (_peerId.Equals(peerId))
                {
                    Debug.Log($"Updating Local Peer's Role");
                    _permissions.Role = role;
                    Emit("role-updated", role);

                    return;
                }

                RemotePeer remotePeer = Room.Instance.GetRemotePeerById(peerId);
                string prevRole = remotePeer.Role;
                remotePeer.Role = role;

                Room.Instance.Emit("room-role-updated", peerId, role, prevRole);


            }
            catch (Exception ex)
            {
                Debug.LogError($"Error Updating Peer's Role | error : {ex}");
            }
        }

        void ReceiveDataReceived(ReceiveData receiveData)
        {
            Debug.Log($"Received data | data {receiveData.Payload}");
            _ = Emit("receive-data", Newtonsoft.Json.JsonConvert.SerializeObject(receiveData));
        }

        void PeerMetadataUpdatedReceived(PeerMetadataUpdated peerMetadataUpdated)
        {
            Debug.Log($"Peer  metadata updated | data {peerMetadataUpdated.Metadata}");
            try
            {
                string peerId = peerMetadataUpdated.PeerId;
                string metadata = peerMetadataUpdated.Metadata;
                if (_peerId.Equals(peerId))
                {
                    //updateMetadata self
                    return;
                }

                RemotePeer remotePeer = Room.Instance.GetRemotePeerById(peerId);
                remotePeer.SetMetadata(metadata);
                _ = Room.Instance.Emit("PeerMetadataUpdated", remotePeer.PeerId, remotePeer.Metadata);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error Updating Metadata | Error: {ex}");
            }
        }

        void RoomMetadataUpdatedReceived(RoomMetadataUpdated roomMetadataUpdated)
        {
            try
            {
                Room.Instance.SetMetadata(roomMetadataUpdated.Metadata);
            }
            catch (Exception ex)
            {
                Debug.Log($"Error Updating Room Metadata | Error: {ex}");
            }
        }

        private void RoomClosedProducersReceived(RoomClosedProducers roomClosedProducers)
        {
            Debug.Log($"Received Room's Closed Producers");
            try
            {
                RoomClosedProducers.Types.CloseProducerReason reason = roomClosedProducers.Reason;
                foreach (var item in roomClosedProducers.Producers)
                {
                    string label = item.Label;
                    string peerId = item.PeerId;

                    if (peerId.Equals(_peerId))
                    {
                        StopProducing(label);
                        continue;
                    }

                    try
                    {
                        RemotePeer remotePeer = Room.Instance.GetRemotePeerById(peerId);
                        Consumer<Mediasoup.Types.AppData> consumer = GetConsumer(label, peerId);

                        if (consumer != null)
                        {
                            CloseConsumer(label, peerId);
                            remotePeer.RemoveLabelData(label);
                            Room.Instance.Emit("stream-closed", label, peerId);
                            remotePeer.Emit("stream-closed", label, peerId);
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error Closing Producer | error: {ex}");
                    }

                }

                 _ = Room.Instance.Emit("room-notification", new Dictionary<string, object>
                {
                    { "code", reason.Code!=0 ? reason.Code : "4004" },
                    { "message", !string.IsNullOrEmpty(reason.Message)? reason.Message : "Room Closed" },
                    { "tag", !string.IsNullOrEmpty(reason.Tag) ? reason.Tag : "ROOM_CLOSED" }
                });

            }
            catch (Exception ex)
            {
                Debug.LogError($"Error Updating Room's Closed Producers | error: {ex}");
            }
        }

        void PeerLeftReceived(PeerLeft peerLeft)
        {
            try
            {
                string peerId = peerLeft.PeerId;
                RemotePeer remotePeer = Room.Instance.GetRemotePeerById(peerId);
                List<string> labels = remotePeer.Labels;

                foreach (var label in labels)
                {
                    CloseRemotePeerConsumer(peerId, label);
                }

                remotePeer.Close(remotePeer.PeerId);
                _remotePeers.Remove(peerId);

                CustomValueNotifierHolder.PeersNotifier.Value = new Dictionary<string, object>(CustomValueNotifierHolder.PeersNotifier.Value);
                CustomValueNotifierHolder.PeersNotifier.Value.Remove(peerId);
                _ = Room.Instance.Emit("peer-left", peerId);

            }
            catch (Exception ex)
            {
                Debug.Log($"Peer left | Error: {ex}");
            }
        }

        void LobbyPeerLeftReceived(LobbyPeerLeft lobbyPeerLeft)
        {
            try
            {
                string peerId = lobbyPeerLeft.PeerId;
                Dictionary<string, object> lobbyPeers = Room.Instance.LobbyPeers;
                if (lobbyPeers.ContainsKey(peerId))
                {
                    lobbyPeers.Remove(peerId);
                }

                Room.Instance.SetLobbyPeersMap(lobbyPeers);

            }
            catch (Exception ex)
            {
                Debug.LogError($"Error Lobby Peer's Left | error: {ex}");
            }
        }

        void WaitingRoomReceived(WaitingRoom waitingRoom)
        {
            Debug.Log($"Waiting room");
            Emit("room-waiting", waitingRoom);
        }

        void ErrorReceived(Error error)
        {

        }

        public void ConnectRoomRequest(string roomId)
        {
            _roomId = roomId;
            _webSocketService.SendRequest(_webSocketService.RequestPublisher.ConnectRoomRequest(_roomId));
        }

        public void SetRemotePeers(ConnectRoomResponse connectRoomResponse) 
        {
            RoomInfo roomInfo = connectRoomResponse.RoomInfo;

            foreach (var peer in roomInfo.Peers)
            {
                if (peer.PeerId.Equals(_peerId))
                    continue;

                RemotePeer remotePeer = new RemotePeer(peer.PeerId,peer.Metadata,peer.Role);
                _remotePeers[peer.PeerId] = remotePeer;

                Room.Instance.AddNewJoinedPeer(peer.PeerId, remotePeer);
                _ = Room.Instance.Emit("new-peer-joined", remotePeer);

                Dictionary<string, string> producers = new Dictionary<string, string>();
                foreach (var item in peer.Producers)
                {
                    producers[item.Label] = item.Id;
                }

                CustomValueNotifierHolder.PeersNotifier.Value = new Dictionary<string, object>(CustomValueNotifierHolder.PeersNotifier.Value);
                CustomValueNotifierHolder.PeersNotifier.Value[peer.PeerId] = null;

                if (producers.Count > 0) 
                {
                    foreach (var prod in producers) 
                    {
                        // Check transport connection state
                        if (_recvTransport == null ||
                            _recvTransport.connectionState == RTCIceConnectionState.New ||
                            _recvTransport.connectionState == RTCIceConnectionState.Checking)
                        {
                            // Add new producer data to peer
                            _ = remotePeer.AddLabelData(prod.Key, prod.Value);
                        }
                        else
                        {
                            _waitingToConsume.Add(() =>
                            {
                                return remotePeer.AddLabelData(prod.Key, prod.Value);
                            });
                        }
                    }
                }
            }
        }

        public RtpParameters GetRtpParametersFromConsumerResponse(ConsumeResponse consumeResponse)
        {
            RtpParameters tempRtpParams = new RtpParameters
            {
                Mid = consumeResponse.RtpParameters.Mid,
            };

            List<RtpCodecParameters> rtpCodecParam = new List<RtpCodecParameters>();

            foreach (var codec in consumeResponse.RtpParameters.Codecs)
            {
                RtpCodecParameters cod = new RtpCodecParameters
                {
                    Channels = codec.Channels != 0 ? (byte)codec.Channels : (byte)1,
                    MimeType = codec.MimeType,
                    PayloadType = (byte)codec.PayloadType,
                    ClockRate = (uint)(codec.ClockRate),
                };

                List<RtcpFeedback> rtcpfb = new List<RtcpFeedback>();

                if (codec.RtcpFeedback != null && codec.RtcpFeedback.Count > 0)
                {
                    foreach (var fb in codec.RtcpFeedback)
                    {
                        rtcpfb.Add(new RtcpFeedback
                        {
                            Type = fb.Type,
                            Parameter = fb.Parameter
                            
                        });
                    }
                }

                cod.RtcpFeedback = rtcpfb;
                rtpCodecParam.Add(cod);
            }

            tempRtpParams.Codecs = rtpCodecParam;

            List<RtpEncodingParameters> rtpep = new List<RtpEncodingParameters>();

            if (consumeResponse.RtpParameters.Encodings != null && consumeResponse.RtpParameters.Encodings.Count > 0)
            {
                foreach (var en in consumeResponse.RtpParameters.Encodings)
                {
                    rtpep.Add(new RtpEncodingParameters
                    {
                        Active = en.Active,
                        CodecPayloadType = (byte)en.CodecPayloadType,
                        Dtx = en.Dtx,
                        Rid = en.Rid,
                        Rtx = new Rtx { Ssrc = (uint)en.Rtx.Ssrc },
                        Ssrc = (uint)en.Ssrc,
                        MaxBitrate = (uint)en.MaxBitrate,
                        ScalabilityMode = en.ScalabilityMode,
                        ScaleResolutionDownBy = en.ScaleResolutionDownBy,
                        MaxFramerate = (uint)en.MaxFramerate,
                    });
                }
            }
            tempRtpParams.Encodings = rtpep;

            List<RtpHeaderExtensionParameters> headerExtensions = new List<RtpHeaderExtensionParameters>();
            if (consumeResponse.RtpParameters.HeaderExtensions != null && consumeResponse.RtpParameters.HeaderExtensions.Count > 0)
            {
                foreach (var he in consumeResponse.RtpParameters.HeaderExtensions)
                {
                    RtpHeaderExtensionParameters heParam = new RtpHeaderExtensionParameters
                    {
                        Encrypt = he.Encrypt,
                        Id = (byte)he.Id,
                    };

                    Dictionary<string, object> tempParamDic = new Dictionary<string, object>();

                    foreach (var item in he.Parameters)
                    {
                        tempParamDic.Add(item.Key, item.Value);
                    }

                    heParam.Parameters = tempParamDic;
                    heParam.Uri = GetHeaderExtensionUri(he.Uri);

                    headerExtensions.Add(heParam);

                }
            }

            tempRtpParams.HeaderExtensions = headerExtensions;


            Mediasoup.RtpParameter.RtcpParameters rtcpParam = new Mediasoup.RtpParameter.RtcpParameters
            {
                CNAME = consumeResponse.RtpParameters.Rtcp.Cname,
                ReducedSize = consumeResponse.RtpParameters.Rtcp.ReducedSize,
            };

            tempRtpParams.Rtcp = rtcpParam;

            return tempRtpParams;
        }

        //Later add this to mediasoup
        public RtpHeaderExtensionUri GetHeaderExtensionUri(string uri)
        {
            switch (uri)
            {
                case "urn:ietf:params:rtp-hdrext:sdes:mid":
                    return RtpHeaderExtensionUri.Mid;
                case "urn:ietf:params:rtp-hdrext:sdes:rtp-stream-id":
                    return RtpHeaderExtensionUri.RtpStreamId;
                case "urn:ietf:params:rtp-hdrext:sdes:repaired-rtp-stream-id":
                    return RtpHeaderExtensionUri.RepairRtpStreamId;
                case "http://tools.ietf.org/html/draft-ietf-avtext-framemarking-07":
                    return RtpHeaderExtensionUri.FrameMarkingDraft07;
                case "urn:ietf:params:rtp-hdrext:framemarking":
                    return RtpHeaderExtensionUri.FrameMarking;
                case "urn:ietf:params:rtp-hdrext:ssrc-audio-level":
                    return RtpHeaderExtensionUri.AudioLevel;
                case "urn:3gpp:video-orientation":
                    return RtpHeaderExtensionUri.VideoOrientation;
                case "urn:ietf:params:rtp-hdrext:toffset":
                    return RtpHeaderExtensionUri.TimeOffset;
                case "http://www.ietf.org/id/draft-holmer-rmcat-transport-wide-cc-extensions-01":
                    return RtpHeaderExtensionUri.TransportWideCcDraft01;
                case "http://www.webrtc.org/experiments/rtp-hdrext/abs-send-time":
                    return RtpHeaderExtensionUri.AbsSendTime;
                case "http://www.webrtc.org/experiments/rtp-hdrext/abs-capture-time":
                    return RtpHeaderExtensionUri.AbsCaptureTime;
            }

            return RtpHeaderExtensionUri.Mid;
        }

        public void CloseConsumer(string label, string peerId)
        {
            try
            {
                var consumer = GetConsumer(label, peerId);
                consumer?.Close();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error closing consumer | error: {ex.Message}");
            }
        }

        public void SetLobbyPeers(Dictionary<string, object> roomInfo)
        {
            Dictionary<string, object> lobbyPeers;

            if (roomInfo.ContainsKey("lobbyPeers"))
            {
                lobbyPeers = roomInfo["lobbyPeers"] as Dictionary<string, object>;
                Room.Instance.SetLobbyPeersMap(lobbyPeers);
            }
        }

        public void UpdateMetadata(string metadata)
        {
            _metadata = metadata;
            _webSocketService.SendRequest(_webSocketService._requestPublisher.UpdatePeerMetadataRequest(_peerId,metadata));


            _ = Emit("metadata-updated", _metadata);
        }

        public Consumer<Mediasoup.Types.AppData> GetConsumer(string label, string peerId)
        {
            Consumer<Mediasoup.Types.AppData> consumer = _consumers.Get(label, peerId);
            if (consumer != null) return consumer;
            return null;
        }

        public void SendData(string sendDataTo, string payload, string label)
        {
            try
            {
                if (!_permissions.CheckPermission(PermissionType.canSendData, null)) return;

                if (EstimateSize(payload) > maxDataMessageSize) return;

                var parsedTo = sendDataTo == "*" ? new List<string> { "*" } : new List<string> { sendDataTo };

                Request request = new Request
                {
                    SendData = new SendData
                    {
                        Payload = payload,
                        Label = label
                    }
                };

                //Todo Set to here, iterate sendDataTo
                foreach (var item in parsedTo)
                {
                    request.SendData.To.Add(item);
                }

                _webSocketService.SendRequest(request);

            }
            catch (Exception ex)
            {
                Debug.LogError($"Error Sending Data | error: {ex}");
            }
        }

        private int EstimateSize(string payload)
        {
            if (string.IsNullOrEmpty(payload))
            {
                return 0;
            }

            return Encoding.UTF8.GetByteCount(payload);
        }

        public void HandleWaitingToProduce()
        {
            try
            {
                if (_permissions.CheckPermission(PermissionType.canProduce, null))
                {
                    _waitingToProduce.Keys.ToList().ForEach(label => CloseStream(label));
                    _waitingToProduce.Clear();
                    return;
                }

                foreach (var item in _waitingToProduce)
                {
                    if (item.Key.Equals("video") && _permissions.CheckPermission(null, ProduceSources.cam))
                    {
                        CloseStream(item.Key);
                        return;
                    }

                    if (item.Key.Equals("audio") && _permissions.CheckPermission(null, ProduceSources.mic))
                    {
                        CloseStream(item.Key);
                        return;
                    }

                    try
                    {
                        item.Value.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error Producing Stream which was waiting to be produced with label:{item.Key} | error {ex}");
                    }
                }
                _waitingToProduce.Clear();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error Handling Waiting To Produce | error: {ex}");
            }
        }

        public void HandleWaitingToConsume()
        {
            foreach (var item in _waitingToConsume)
            {
                try { item(); } catch (Exception ex) { Debug.LogError($"Unable to Consume after ice restart error-> {ex}"); }

            }
            _waitingToConsume.Clear();
        }

        private void CloseStream(string label)
        {
            var stream = _activeStreams[label];
            if (stream != null)
            {
                //deviceHandler.stopStream(stream); //Todo
                _activeStreams.Remove(label);

                _ = Emit("stream-closed", new Dictionary<string, object?>
                {
                    { "label", label },
                    { "reason", new Dictionary<string, object?>
                        {
                            { "code", 4444 },
                            { "tag", "User's Permissions Denied" },
                            { "message", "CLOSED_BY_ADMIN" }
                        }
                    }
                });

                _waitingToProduce.Remove(label);
            }
        }

        public async Task ConsumerCallback()
        {

        }

        public void Close()
        {
            _device = null;
            _pendingProducerTasks.Clear();

            _waitingToProduce.Clear();
            _waitingToConsume.Clear();

            _permissions.Reset();
        }

        public async Task Produce(string label, MediaStream stream, bool stopTrackOnClose = false, Mediasoup.Types.AppData appdata = null)
        {
            Debug.Log("Produce");
            try
            {
                if (!_permissions.CheckPermission(PermissionType.canProduce, null)) return;

                string kind = stream != null ? GetMediaStreamKind(stream) : null;
                MediaStreamTrack track = null;

                if (label.Equals("audio"))
                {
                    track = stream.GetAudioTracks().FirstOrDefault();
                }
                else
                {
                    track = stream?.GetVideoTracks().FirstOrDefault();
                }

                if (track != null) 
                {
                //    track.onEnded = () {stopProducing(label: label);
                }

                if (!_joined ||
                    (_sendTransport.connectionState != RTCIceConnectionState.Connected &&
                    _sendTransport.connectionState != RTCIceConnectionState.New))
                {
                    _waitingToProduce[label] = async () => await Produce(label, stream, stopTrackOnClose,appdata);
                    _pendingProducerTasks.Remove(label);
                    return;
                    /*TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

                    async Task Fn()
                    {
                        try
                        {
                            await Produce(label, stream);
                            tcs.SetResult(true);
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                        finally
                        {
                            _pendingProducerTasks.Remove(label);
                        }
                    }

                    _waitingToProduce[label] = Fn;
                    await tcs.Task;
                    return;*/
                }


                if (_sendTransport == null)
                {
                    throw new InvalidOperationException("Send Transport Not Initialized, Internal Error");
                }

                Debug.Log($"Produce Called for kind: {kind}, label: {label}");

                if (stream != null)
                {
                    if (label == "audio")
                    {
                        Mediasoup.Types.AppData audioAppData = new Mediasoup.Types.AppData();
                        audioAppData.Add("producerPeerId", _peerId);
                        audioAppData.Add("label", label);

                        ProducerOptions<Mediasoup.Types.AppData> producerOptions = RtpConstantParam.ProducerOptionsObjForAudio;
                        producerOptions.appData = audioAppData;
                        producerOptions.disableTrackOnPause = stopTrackOnClose;
                        producerOptions.track = track;
                        producerOptions.stopTracks = true;
                        producerOptions.zeroRtpOnPause = true;

                        producerOptions.codecOptions = new ProducerCodecOptions
                        {
                            opusStereo = true,
                            opusDtx = true
                        };

                        if (appdata != null) producerOptions.appData = appdata;

                        await _sendTransport.ProduceAsync(GetProducerId,ConnectTransportToServerSuccess, ProducerCallback, producerOptions);
                    }


                    if (label == "video")
                    {
                        Mediasoup.Types.AppData videoAppData = new Mediasoup.Types.AppData();
                        videoAppData.Add("producerPeerId", _peerId);
                        videoAppData.Add("label", label);

                        List<RtpCodecCapability> codecs = _device.GetRtpCapabilities().Codecs;
                        RtpCodecCapability codec = codecs.FirstOrDefault
                            (codec => codec.MimeType.ToLower().Equals("video/vp9"));

                        Debug.Log($"videocodec => {Newtonsoft.Json.JsonConvert.SerializeObject(codec)}");

                        ProducerOptions<Mediasoup.Types.AppData> producerOptions = RtpConstantParam.ProducerOptionsObjForVideo;
                        producerOptions.appData = videoAppData;
                        producerOptions.disableTrackOnPause = stopTrackOnClose;
                        producerOptions.track = track;
                        producerOptions.stopTracks = true;
                        producerOptions.zeroRtpOnPause = true;
                        producerOptions.codec = codec;

                        producerOptions.codecOptions = new ProducerCodecOptions
                        {
                            videoGoogleStartBitrate = 1000
                        };

                        if (appdata != null) producerOptions.appData = appdata;


                        await _sendTransport.ProduceAsync(GetProducerId, ConnectTransportToServerSuccess, ProducerCallback, producerOptions);
                    }

                    Debug.Log($"Produce Data");

                    _sendTransport.On("transportclose", async (args) => StopProducing(label));
                    Emit("stream-playable", label);
                }

            }
            catch (Exception ex)
            {
                Debug.LogError($"Error Create Producer Failed | error: {ex}");
                throw new Exception("Error Create Producer Failed");
            }
        }

        private async Task<bool> ConnectTransportToServerSuccess(DtlsParameters dtls,string transportType)
        {
            _createTransportTcs = new TaskCompletionSource<bool>();

            DtlsParameters dtlsParameters = dtls;
            
            ProtoDtlsParameters protoDtlsParameters = GetDtlsParam(dtlsParameters);

            //Once("connectTransportResponse", async (args) => callBack.Invoke());


            Debug.Log($"Send transport connect request");

            Request request = new Request
            {
                ConnectTransport = new ConnectTransport
                {
                    DtlsParameters = protoDtlsParameters,
                    TransportType = transportType
                }
            };

            _webSocketService.SendRequest(request);
            

            return await _createTransportTcs.Task;
        }

        private void ProducerCallback(Producer<Mediasoup.Types.AppData> obj)
        {
            Debug.Log($"Producer ID ->{obj.id}");
            string label = obj.track.Kind == TrackKind.Audio ? "audio" : "video";

            _producers[obj.id] = obj;
            _labelToProducerId[label] = obj.id;


            Debug.Log($"{_labelToProducerId[label]} for label {label}");
        }

        private async Task<string> GetProducerId(TrackKind arg1, RtpParameters arg2, Mediasoup.Types.AppData arg3)
        {
            TaskCompletionSource<string> _tcs = new TaskCompletionSource<string>();
            _pendingProducerTasks.Add(arg1 == TrackKind.Audio ? "audio" : "video", _tcs);

            string label = (string)arg3["label"];
            string producerId = (string)arg3["producerPeerId"];
            AppData appData = new AppData();
            appData.AppData_.Add("label", new Value {StringValue=label});
            appData.AppData_.Add("producerPeerId", new Value {StringValue=producerId });


            Request request = new Request
            {
                Produce = new Produce
                {
                    Kind = arg1 == TrackKind.Audio ? "audio" : "video",
                    RtpParameters = ProtoEnumHelper.GetProtoRtpParameters(arg2, arg1 == TrackKind.Audio ? "audio" : "video"),
                    Label = label,
                    Paused = false,
                    AppData = appData,
                }
            };

            _webSocketService.SendRequest(request);
            return await _pendingProducerTasks[arg1 == TrackKind.Audio ? "audio" : "video"].Task;
        }

        private string GetMediaStreamKind(MediaStream stream)
        {
            string kind = null;

            var tracks = stream.GetTracks();

            foreach (var track in tracks)
            {
                if (track.Kind == TrackKind.Audio)
                {
                    kind = "audio";
                    break;
                }

                if (track.Kind == TrackKind.Video)
                {
                    kind = "video";
                    break;
                }
            }

            if (string.IsNullOrEmpty(kind)) { throw new Exception("Stream kind not found"); }

            return kind;
        }

        public async Task<MediaStream> EnableVideo(MediaStream customVideoStream = null)
        {
            if (!_permissions.CheckPermission(null, ProduceSources.cam)) return null;
            if (_activeStreams.ContainsKey("video") && _activeStreams["video"] != null)
            {
                Debug.Log("Cam stream already enables");
                return null;
            }

            if (_sendTransport == null)
            {
                await CreateTransportOnServer(TransportType.send);
                Debug.Log("CreateTransportOnServer Done");
            }

            MediaStream stream;
            if (customVideoStream != null)
            {
                stream = customVideoStream;
            }
            else
            {
                Task<MediaStream> ongoingStream = null;

                if (_pendingFetchingStream.ContainsKey("cam")) 
                {
                    ongoingStream = _pendingFetchingStream["cam"];
                }

                if (ongoingStream != null)
                {
                    await ongoingStream;
                }
                else
                {
                    Debug.Log("Fetch camera");
                    Task<MediaStream> streamFuture =  _deviceHandler.FetchStream(SelectedVideoStreamDevice, "video");
                    Debug.Log("Fetch camera is done");
                    _pendingFetchingStream["cam"] = streamFuture;
                    
                }

                if (!_pendingFetchingStream.ContainsKey("cam") && _pendingFetchingStream["cam"] == null)
                {
                    Debug.Log("_pendingFetchingStream doesnt have camera");
                    return null;
                }

                Debug.Log("Fetching video stream");
                var fetchedStream = await _pendingFetchingStream["cam"];
                Debug.Log($"Video stream is {fetchedStream.GetVideoTracks().ToList().FirstOrDefault().ReadyState}");

                if (fetchedStream == null)
                {
                    Debug.LogError("Stream Not Found, cannot do enableVideo");
                    throw new Exception("Stream Not Found");
                }

                stream = fetchedStream;

                if (stream != null)
                {
                    Debug.Log("Stream is not null");
                    _activeStreams["video"] = stream;
                    Emit("stream-fetched", "cam", stream, "video");
                }
                else 
                {
                    Debug.LogError($"Video stream is null");
                }

                Mediasoup.Types.AppData appdata = new Mediasoup.Types.AppData();
                appdata.Add("producerPeerId", PeerId);
                appdata.Add("label", "video");
                Debug.Log("start Produce");
                await Produce("video", stream, true, appdata);

                _pendingFetchingStream.Remove("cam");
                return stream;
            }
            return null;
        }

        public async Task<MediaStream> EnableAudio(MediaStream customAudioStream = null)
        {
            try
            {
                Debug.Log($"enableAudio called");
                if (!_permissions.CheckPermission(null, ProduceSources.mic))
                {
                    Debug.Log($"Cannot Enable Audio, Permission Denied");
                    throw new Exception($"Cannot Enable Audio, Permission Denied");
                }

                if (_activeStreams.ContainsKey("audio") && _activeStreams["audio"] != null)
                {
                    Debug.Log("Mic stream already enables");
                    return null;
                }

                Debug.Log("Checked Permission");

                MediaStream existingStream = null;

                if (_activeStreams.ContainsKey("audio"))
                {
                    existingStream = _activeStreams["audio"];
                }


                if (existingStream != null)
                {
                    Debug.Log($"Mic stream already enabled");
                    return null;
                }

                if (_sendTransport == null)
                {
                    await CreateTransportOnServer(TransportType.send);
                    Debug.Log("CreateTransportOnServer Done");
                }

                Debug.Log("Create audio stream now");

                MediaStream stream;
                if (customAudioStream != null)
                {
                    stream = customAudioStream;
                }
                else
                {
                    Task<MediaStream> ongoingStreamFuture = null;

                    if (_pendingFetchingStream.ContainsKey("mic"))
                    {
                        ongoingStreamFuture = _pendingFetchingStream["mic"];
                    }

                    if (ongoingStreamFuture != null)
                    {
                        await ongoingStreamFuture;
                    }
                    else
                    {
                        Debug.Log("Fetch stream from device handler");
                        Task<MediaStream> streamFuture = _deviceHandler.FetchStream(SelectedAudioStreamDevice, "audio");
                        _pendingFetchingStream.Add("mic", streamFuture);
                        Debug.Log("Fetch stream from device handler done");
                    }

                    Debug.Log("pendingFuture");
                    Task<MediaStream> pendingFuture = null;

                    Debug.Log("_pendingFetchingStream");

                    if (_pendingFetchingStream.ContainsKey("mic"))
                    {
                        pendingFuture = _pendingFetchingStream["mic"];
                        Debug.Log("_pendingFetchingStream 01");
                    }

                    if (pendingFuture == null)
                    {
                        Debug.Log("Pending Mic Future Not Found");
                        return null;
                    }

                    var fetchedStream = await pendingFuture;

                    if (fetchedStream == null)
                    {
                        Debug.Log($"Stream Not Found, cannot do enableAudio");
                        throw new Exception($"Stream Not Found");
                    }

                    stream = fetchedStream;

                    if (stream != null)
                    {
                        _activeStreams["audio"] = stream;
                        _ = Emit("stream-fetched", "mic", stream, "audio");
                    }

                    
                    Mediasoup.Types.AppData appdata = new Mediasoup.Types.AppData();
                    appdata.Add("producerPeerId", PeerId);
                    appdata.Add("label", "audio");
                    await Produce("audio", stream, true, appdata);

                    _pendingFetchingStream.Remove("mic");
                    Debug.Log("Produce Done");
                    return stream;
                }

                Debug.Log("Return null");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Cant enable Mic error-> {ex}");
                throw new Exception($"Cant enable Mic error-> {ex}");
            }

        }

        public async Task StopProducing(string label)
        {
            _waitingToProduce.Remove(label);
            _pendingProducerTasks.Remove(label);

            bool closedStream = false;

            var producer = GetProducerWithLabel(label);

            if (producer != null)
            {
                producer.Close();

                producer.On("trackended", async (args) => Debug.LogError("Track Ended For the Producer"));

                _producers.Remove(producer.id);

                closedStream = true;

                Request request = new Request
                {
                    CloseProducer = new CloseProducer
                    {
                        ProducerId = producer.id
                    }
                };

                _webSocketService.SendRequest(request);

                var closedStreamLabel = label;
                var stream = _activeStreams[closedStreamLabel];

                if (stream != null)
                {
                    _deviceHandler.StopStream(stream, label);
                    _activeStreams.Remove(closedStreamLabel);
                    closedStream = true;
                }

                if (closedStream)
                {
                    Emit("stream-closed", label, "1200", "STREAM_CLOSED", "Stopped Streaming");
                }
            }
        }

        private Producer<Mediasoup.Types.AppData> GetProducerWithLabel(string label)
        {
            try
            {
                if (_labelToProducerId.TryGetValue(label , out string val))
                {
                    var producer = _producers[val];
                    return producer;
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Cannot Find Producer With Identifier: $label | error: {ex}");
                return null;
            }
        }

        public async Task<Transport<Mediasoup.Types.AppData>> CreateTransportOnServer(TransportType transportType)
        {
            // Check if a task is already pending for this transport type
            if (_pendingTransportTasks.TryGetValue(transportType, out var pendingFuture))
            {
                Debug.Log($"Transport Task Already Pending for this transportType {transportType}");
                return await pendingFuture;
            }

            var transportTaskSource = new TaskCompletionSource<Transport<Mediasoup.Types.AppData>>();

            // Define event handler to handle new transport event
            async Task HandleNewTransport(Transport<Mediasoup.Types.AppData> transportRef)
            {
                Debug.Log($"HandleNewTransport");

                if (transportRef!=null)
                {
                    Debug.Log("Run task now");
                    _pendingTransportTasks.Remove(transportType);
                    transportTaskSource.SetResult(transportRef);
                }
            }

            // Set the event listener based on transport type
            if (transportType == TransportType.send)
            {
                Once("new-send-transport", async (args) => HandleNewTransport(args[0] as Transport<Mediasoup.Types.AppData> ));
            }
            else if (transportType == TransportType.recv)
            {
                Once("new-recv-transport", async (args) => HandleNewTransport(args[0] as Transport<Mediasoup.Types.AppData>));
            }

            Device device = _device;
            Transport<Mediasoup.Types.AppData> transport = transportType == TransportType.send ? _sendTransport : _recvTransport;

            if (transport != null)
            {
                transportTaskSource.SetException(new InvalidOperationException($"❌ Transport Already Initialized, type: {transportType}"));
            }

            if (_sendTransport == null && transportType == TransportType.send)
            {
                Debug.Log("_createTransportOnServer | request for createTransport");
                var protoSctpCapabilities = GetSctpFromDevice(device);
                Request request = new Request
                {
                    CreateTransport = new CreateTransport
                    {
                        SctpCapabilities = protoSctpCapabilities,
                        TransportType = SendString
                    }
                };

                _webSocketService.SendRequest(request);
            }
            if (_recvTransport == null && transportType == TransportType.recv)
            {
                Debug.Log("_createTransportOnServer | request for createTransport");
                var protoSctpCapabilities = GetSctpFromDevice(device);

                Request request = new Request
                {
                    CreateTransport = new CreateTransport
                    {
                        SctpCapabilities = protoSctpCapabilities,
                        TransportType = RecvString
                    }
                };

                _webSocketService.SendRequest(request);
            }

            Debug.Log("Return transport");

            _pendingTransportTasks[transportType] = transportTaskSource.Task;
            return await transportTaskSource.Task;
        }

        public async Task DisableVideo()
        {
            await StopProducing("video");
        }

        public async Task DisableAudio()
        {
            await StopProducing("audio");
        }

        public async Task Consume(Dictionary<string, object> data)
        {
            Debug.Log("Start Consuming");

            if (!_permissions.CheckPermission(PermissionType.canConsume, null))
            {
                return;
            }

            Debug.Log("Check recv Transport");

            if (_recvTransport == null)
            {
                Debug.Log($"Recv Transport Not Initialized, Creating RecvTransport");
                await CreateTransportOnServer(TransportType.recv);
            }

            try
            {
                string peerId = data["peerId"] as string;
                string label = data["label"] as string;

                AppData appdata = data["appData"] as AppData;
                AppData parseAppData = new AppData();
                foreach (var item in appdata.AppData_)
                {
                    parseAppData.AppData_.Add(item.Key, item.Value);
                }

                var remotePeer = _remotePeers[peerId];
                if (remotePeer == null)
                {
                    throw new Exception($"Remote Peer Not Found with PeerId {peerId}");
                }

                var labelData = remotePeer.GetLabelData(label);

                if (labelData == null)
                {
                    throw new Exception($"Remote Peer is not producing with Label {labelData}");
                }

                Debug.Log($"Consuming Stream with label {label}");

                var consumerTaskSource = new TaskCompletionSource<Consumer<Mediasoup.Types.AppData>>();

                void HandleStreamPlayable(Dictionary<string, object> streamData)
                {
                    if (streamData["label"].ToString() == label)
                    {
                        remotePeer.RemoveListener("stream-playable", async (args) =>
                        {
                            var streamData = (Dictionary<string, object>)args[0];
                            HandleStreamPlayable(streamData);
                        });
                        consumerTaskSource.SetResult((Consumer<Mediasoup.Types.AppData>)streamData["consumer"]);
                    }
                }

                // Register the event handler
                remotePeer.On("stream-playable", async (args) =>
                {
                    var streamData = (Dictionary<string, object>)args[0];
                    HandleStreamPlayable(streamData);
                });

                Request request = new Request
                {
                    Consume = new Consume
                    {
                        AppData = parseAppData,
                        ProducerId = labelData["producerId"],
                        ProducerPeerId = peerId
                    }
                };

                _webSocketService.SendRequest(request);

                _pendingConsumerTasks[labelData["producerId"]] = consumerTaskSource;

            }
            catch (Exception ex)
            {
                Debug.LogError($"Error Consuming Stream | error: {ex}");
            }
        }

        public void StopConsuming(string peerId,string label) 
        {
            if (!Room.Instance.RemotePeers.ContainsKey(peerId)) 
            {
                Debug.LogError($"Remote Peer doesnt exist");
            }

            RemotePeer remotePeer = Room.Instance.GetRemotePeerById(peerId);

            if (!remotePeer.HasLabel(label)) 
            {
                Debug.LogError($"Remote Peer is not producing anything with label: {label}");
            }

            Consumer<Mediasoup.Types.AppData> consumer = GetConsumer(peerId,label);

            if (consumer==null) 
            {
                Debug.LogError($"Consumer not found");
            }

            Request request = new Request
            {
                CloseConsumer = new CloseConsumer
                {
                    ConsumerId = consumer.id,
                }
            };
            _webSocketService.SendRequest(request);

            _ = remotePeer.Emit("stream-closed", label);

            CloseConsumer(label,peerId);

        }

        private ProtoSctpCapabilities GetSctpFromDevice(Device device)
        {
            ProtoSctpCapabilities sctp = new ProtoSctpCapabilities();
            sctp.NumStreams = new ProtoNumSctpStreams
            {
                MIS = device.GetSctpCapabilities().numStreams.MIS,
                OS = device.GetSctpCapabilities().numStreams.OS
            };

            return sctp;
        }


        private ProtoDtlsParameters GetDtlsParam(DtlsParameters dtlsParam)
        {
            ProtoDtlsParameters protoDtls = new ProtoDtlsParameters();

            foreach (var item in dtlsParam.fingerprints)
            {
                ProtoDtlsFingerPrints protoDtlsFp = new ProtoDtlsFingerPrints
                {
                    Algorithm = EnumExtensions.GetStringValue<FingerPrintAlgorithm>(item.algorithm),
                    Value = item.value
                };

                protoDtls.Fingerprints.Add(protoDtlsFp);
            }

            protoDtls.Role = EnumExtensions.GetStringValue<DtlsRole>(dtlsParam.role);

            return protoDtls;
        }

        public async Task ChangeAudioTrack(string deviceName) 
        {
            var stream = _activeStreams["audio"];
            _activeStreams["audio"] = await _deviceHandler.ChangeMicTrack(stream, deviceName);
        }

        public async Task ChangeVideoTrack(string deviceName)
        {
            var stream = _activeStreams["video"];
            _activeStreams["video"] = await _deviceHandler.ChangeCamTrack(stream, deviceName);
        }
    }
}

public enum TransportType
{
    send,
    recv,
}

#endif
