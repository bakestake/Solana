using UnityEngine;
using Huddle01.Core.EventBroadcast;
using Google.Protobuf;

namespace Huddle01.Core.Services
{
    public class WebSocketEventsProcessor
    {
        private EventsBroadcaster _eventBroadcaster;

        public WebSocketEventsProcessor(EventsBroadcaster eventBroadcaster)
        {
            _eventBroadcaster = eventBroadcaster;
        }

        public Response ParseMessage(byte[] message)
        {
            Response parsedMessage = Response.Parser.ParseFrom(message);
            return parsedMessage;
        }


        //Process incoming msg from web socket
        public void ProcessWebSocketEvents(byte[] message)
        {
            Response response = ParseMessage(message);
            Response.ResponseOneofCase eventType = response.ResponseCase;
            Debug.Log(response.ResponseCase);
            _eventBroadcaster.RaiseEvent(eventType, response);
            
           /* switch (eventType)
            {
                case Response.ResponseOneofCase.Hello:
                    _eventBroadcaster.RaiseHelloEvent(response.Hello);
                    break;
                case Response.ResponseOneofCase.ConnectRoomResponse:
                    _eventBroadcaster.RaiseConnectRoomResponseEvent(response.ConnectRoomResponse);
                    break;
                case Response.ResponseOneofCase.CreateTransportOnClient:

                    _eventBroadcaster.RaiseCreateTransportOnClientEvent(response.CreateTransportOnClient);
                    break;
                case Response.ResponseOneofCase.ProduceResponse:
                    _eventBroadcaster.RaiseProduceResponseEvent(response.ProduceResponse);
                    break;
                case Response.ResponseOneofCase.ConsumeDataResponse:
                    _eventBroadcaster.RaiseConsumeDataResponseEvent(response.ConsumeDataResponse);
                    break;
                case Response.ResponseOneofCase.ProduceDataResponse:
                    _eventBroadcaster.RaiseProduceDataResponseEvent(response.ProduceDataResponse);
                    break;
                case Response.ResponseOneofCase.SyncMeetingStateResponse:
                    _eventBroadcaster.RaiseSyncMeetingStateResponseEvent(response.SyncMeetingStateResponse);
                    break;
                case Response.ResponseOneofCase.ConsumeResponse:
                    _eventBroadcaster.RaiseConsumeResponseEvent(response.ConsumeResponse);
                    break;
                case Response.ResponseOneofCase.CloseProducerSuccess:
                    _eventBroadcaster.RaiseCloseProducerSuccessEvent(response.CloseProducerSuccess);
                    break;
                case Response.ResponseOneofCase.PauseProducerSuccess:
                    _eventBroadcaster.RaisePauseProducerSuccessEvent(response.PauseProducerSuccess);
                    break;
                case Response.ResponseOneofCase.ResumeProducerSuccess:
                    _eventBroadcaster.RaiseResumeProducerSuccessEvent(response.ResumeProducerSuccess);
                    break;
                case Response.ResponseOneofCase.CloseConsumerSuccess:
                    _eventBroadcaster.RaiseCloseConsumerSuccessEvent(response.CloseConsumerSuccess);
                    break;
                case Response.ResponseOneofCase.ConnectTransportResponse:
                    _eventBroadcaster.RaiseConnectTransportResponseEvent(response.ConnectTransportResponse);
                    break;
                case Response.ResponseOneofCase.RestartTransportIceResponse:
                    _eventBroadcaster.RaiseRestartTransportIceResponseEvent(response.RestartTransportIceResponse);
                    break;
                case Response.ResponseOneofCase.NewPeerJoined:
                    _eventBroadcaster.RaiseNewPeerJoinedEvent(response.NewPeerJoined);
                    break;
                case Response.ResponseOneofCase.NewLobbyPeer:
                    _eventBroadcaster.RaiseNewLobbyPeerEvent(response.NewLobbyPeer);
                    break;
                case Response.ResponseOneofCase.NewPermissions:
                    _eventBroadcaster.RaiseNewPermissionsEvent(response.NewPermissions);
                    break;
                case Response.ResponseOneofCase.NewRoomControls:
                    _eventBroadcaster.RaiseNewRoomControlsEvent(response.NewRoomControls);
                    break;
                case Response.ResponseOneofCase.NewPeerRole:
                    _eventBroadcaster.RaiseNewPeerRoleEvent(response.NewPeerRole);
                    break;
                case Response.ResponseOneofCase.ReceiveData:
                    _eventBroadcaster.RaiseReceiveDataEvent(response.ReceiveData);
                    break;
                case Response.ResponseOneofCase.PeerMetadataUpdated:
                    _eventBroadcaster.RaisePeerMetadataUpdatedEvent(response.PeerMetadataUpdated);
                    break;
                case Response.ResponseOneofCase.RoomMetadataUpdated:
                    _eventBroadcaster.RaiseRoomMetadataUpdatedEvent(response.RoomMetadataUpdated);
                    break;
                case Response.ResponseOneofCase.RoomClosedProducers:
                    _eventBroadcaster.RaiseRoomClosedProducersEvent(response.RoomClosedProducers);
                    break;
                case Response.ResponseOneofCase.PeerLeft:
                    _eventBroadcaster.RaisePeerLeftEvent(response.PeerLeft);
                    break;
                case Response.ResponseOneofCase.LobbyPeerLeft:
                    _eventBroadcaster.RaiseLobbyPeerLeftEvent(response.LobbyPeerLeft);
                    break;
                case Response.ResponseOneofCase.WaitingRoom:
                    _eventBroadcaster.RaiseWaitingRoomEvent(response.WaitingRoom);
                    break;
                case Response.ResponseOneofCase.Error:
                    _eventBroadcaster.RaiseErrorEvent(response.Error);
                    break;
                default:
                    Debug.Log("Unknown Response.ResponseOneofCase");
                    break;
            }*/
        }

        public void CheckReq()
        {
            ConnectRoom connectRoom = new ConnectRoom();
            connectRoom.RoomId = "hmq-hhzh-kkj";

            byte[] messageArray = connectRoom.ToByteArray();
        }


    }
}


