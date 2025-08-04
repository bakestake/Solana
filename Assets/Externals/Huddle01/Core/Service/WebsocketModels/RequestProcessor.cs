using Google.Protobuf;
using Google.Protobuf.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Huddle01.Core.Services 
{
    public class RequestProcessor
    {

        public RequestProcessor()
        {
            
        }

        public Request ConnectRoomRequest(string roomId) 
        {
            ConnectRoom connectRoom = new ConnectRoom { RoomId=roomId };
            Request request = new Request {ConnectRoom = connectRoom };
            return request;
        }

        public Request CreateTransportRequest(ProtoSctpCapabilities protoSctpCapabilities,string transportType)
        {
            CreateTransport createTransport = new CreateTransport { SctpCapabilities = protoSctpCapabilities,TransportType = transportType };
            Request request = new Request {CreateTransport = createTransport };
            return request;
        }

        public Request ConnectTransportRequest(string transportType, ProtoDtlsParameters protoDtlsParameters)
        {
            ConnectTransport connectTransport = new ConnectTransport {TransportType=transportType,DtlsParameters=protoDtlsParameters };
            Request request = new Request { ConnectTransport = connectTransport};
            return request;
        }

        public Request CreateDataConsumerRequest(string label)
        {
            // Implement logic for CreateDataConsumer
            CreateDataConsumer createDataConsumer = new CreateDataConsumer { Label = label};
            Request request = new Request { CreateDataConsumer = createDataConsumer };
            return request;
        }

        public Request ProduceRequest(string label,string kind,ProtoRtpParameters protoRtpParameters,bool paused,AppData appData)
        {
            // Implement logic for Produce
            Produce produce = new Produce { Label = label , Kind = kind, RtpParameters = protoRtpParameters , Paused = paused , AppData = appData};
            Request request = new Request { Produce = produce};
            return request;
        }

        public Request ProduceDataRequest()
        {
            return null;
        }

        public Request ConsumeRequest(string producerId, string producerPeerId, AppData appData)
        {
            // Implement logic for Consume
            Consume consume = new Consume {ProducerId = producerId,ProducerPeerId = producerPeerId,AppData=appData };
            Request request = new Request { Consume=consume};
            return request;
        }

        public Request CloseProducerRequest(string producerId)
        {
            // Implement logic for CloseProducer
            CloseProducer closeProducer = new CloseProducer { ProducerId = producerId};
            Request request = new Request { CloseProducer = closeProducer};
            return request;
        }

        public Request PauseProducerRequest()
        {
            // Implement logic for PauseProducer
            return null;
        }

        public Request ResumeProducerRequest()
        {
            // Implement logic for ResumeProducer
            return null;
        }

        public Request CloseConsumerRequest()
        {
            // Implement logic for CloseConsumer
            return null;
        }

        public Request ResumeConsumerRequest(string consumerId,string producerPeerId)
        {
            // Implement logic for ResumeConsumer
            ResumeConsumer resumeConsumer = new ResumeConsumer { ConsumerId = consumerId,ProducerPeerId=producerPeerId};
            Request request = new Request { ResumeConsumer = resumeConsumer };
            return request;
        }

        public Request SyncMeetingStateRequest()
        {
            // Implement logic for SyncMeetingState
            SyncMeetingState syncMeetingState = new SyncMeetingState();
            Request request = new Request { SyncMeetingState = syncMeetingState};
            return request;
        }

        public Request RestartTransportIceRequest(string transportType,string transportId)
        {
            // Implement logic for RestartTransportIce
            RestartTransportIce restartTransportIce = new RestartTransportIce {TransportType = transportType,TransportId = transportId };
            Request request = new Request {RestartTransportIce = restartTransportIce };
            return request;
        }

        public Request SendDataRequest(List<string> to,string payload,string label)
        {
            // Implement logic for SendData
            
            SendData sendData = new SendData {Label = label,Payload=payload };

            foreach (var peerId in to)
            {
                sendData.To.Add(peerId);
            }

            Request request = new Request {SendData = sendData };
            return request;
        }

        public Request UpdateRoomControlsRequest(UpdateRoomControls updateRoomControls )
        {
            Request request = new Request { UpdateRoomControls = updateRoomControls};
            return request;
        }

        public Request UpdatePeerPermissionRequest()
        {
            // Implement logic for UpdatePeerPermission
            return null;
        }

        public Request ActivateSpeakerNotificationRequest()
        {
            // Implement logic for ActivateSpeakerNotification
            return null;
        }

        public Request UpdatePeerRoleRequest(string peerId,string role)
        {
            // Implement logic for UpdatePeerRole
            UpdatePeerRole updatePeerRole = new UpdatePeerRole { PeerId=peerId,Role=role};
            Request request = new Request { UpdatePeerRole = updatePeerRole};
            return request;
        }

        public Request UpdatePeerMetadataRequest(string peerId,string metadata)
        {
            // Implement logic for UpdatePeerMetadata
            UpdatePeerMetadata updatePeerMetadata = new UpdatePeerMetadata { PeerId =peerId,Metadata=metadata};
            Request request = new Request { UpdatePeerMetadata = updatePeerMetadata };
            return request;
        }

        public Request UpdateRoomMetadataRequest(string metadata)
        {
            // Implement logic for UpdateRoomMetadata
            UpdateRoomMetadata updateRoomMetadata = new UpdateRoomMetadata {Metadata=metadata };
            Request request = new Request { UpdateRoomMetadata = updateRoomMetadata};
            return request;
        }

        public Request CloseStreamOfLabelRequest(string label, RepeatedField<string> peerIds = null)
        {
            // Implement logic for CloseStreamOfLabel
            CloseStreamOfLabel closeStreamOfLabel = new CloseStreamOfLabel { Label = label};
            foreach (var peerId in peerIds)
            {
                closeStreamOfLabel.PeerIds.Add(peerId);
            }

            Request request = new Request { CloseStreamOfLabel = closeStreamOfLabel};
            return request;
        }

        public Request AcceptLobbyPeerRequest(string peerId)
        {
            // Implement logic for AcceptLobbyPeer
            AcceptLobbyPeer acceptLobbyPeer = new AcceptLobbyPeer { PeerId = peerId};
            Request request = new Request {AcceptLobbyPeer = acceptLobbyPeer};
            return request;
        }

        public Request DenyLobbyPeerRequest(string peerId)
        {
            // Implement logic for DenyLobbyPeer
            DenyLobbyPeer denyLobbyPeer = new DenyLobbyPeer { PeerId = peerId};
            Request request = new Request {DenyLobbyPeer= denyLobbyPeer };
            return request;
        }

        public Request KickPeerRequest(string peerId)
        {
            // Implement logic for KickPeer
            KickPeer kickPeer = new KickPeer { PeerId = peerId };
            Request request = new Request {KickPeer=kickPeer };
            return request;
        }

        public Request CloseRoomRequest()
        {
            // Implement logic for CloseRoom
            CloseRoom closeRoom = new CloseRoom();
            Request request = new Request {CloseRoom = closeRoom };
            return request;
        }

    }
}


