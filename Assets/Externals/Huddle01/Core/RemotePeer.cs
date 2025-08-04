#if !UNITY_WEBGL

using System;
using System.Collections.Generic;
using System.Linq;
using Mediasoup.Internal;
using UnityEngine;
using Mediasoup;
using System.Threading.Tasks;

namespace Huddle01.Core
{   
    [Serializable]
    public class RemotePeer : EventEmitter
    {
        public string PeerId { get; private set; }
        public string Metadata=>_metadata;
        private string _metadata = "{}";
        private string _role;
        private readonly Permissions permissions = new Permissions();
        private readonly Dictionary<string, Dictionary<string, string>> _labelsToProducerId = new Dictionary<string, Dictionary<string, string>>();

        public RemotePeer(string peerId, string metadata = null, string role = null)
        {
            PeerId = peerId;
            if (!string.IsNullOrEmpty(metadata))
            {
                _metadata = metadata;
            }
            if (!string.IsNullOrEmpty(role))
            {
                _role = role;
            }
        }
        // Labels are the unique identifier for the media stream that the remote peer is producing
        public List<string> Labels => new List<string>(_labelsToProducerId.Keys);
        public List<string> ProducerIds => new List<string>(_labelsToProducerId.Values.Select(labelData => labelData.GetValueOrDefault("producerId")));
        // Role of the Peer.
        public string Role
        {
            get => _role;
            set
            {
                _role = value;
                if (value != null)
                {
                    _ = Emit("role-updated", value);
                }
            }
        }
        // Checks if the remote peer is producing the label
        public bool HasLabel(string label)
        {
            return _labelsToProducerId.ContainsKey(label);
        }
        // Returns the data associated with the label, this is the producerId
        public Dictionary<string, string> GetLabelData(string label)
        {
            _labelsToProducerId.TryGetValue(label, out var labelData);
            return labelData;
        }

        // Removes all the states of the remote peer and clears memory
        public void Close(string remotePeer)
        {
            Debug.Log("Closing Remote Peer");
            RemoveAllListeners(remotePeer);
        }

        // Adds label data and manages consumption based on auto-consume setting
        public async Task AddLabelData(string label, string producerId)
        {
            _labelsToProducerId[label] = new Dictionary<string, string> { { "producerId", producerId } };
            try
            {
                bool autoConsume = Room.Instance.AutoConsume;
                LocalPeer localPeer = LocalPeer.Instance;
                if (autoConsume)
                {
                    Debug.Log("AUTO CONSUME IS ENABLED, CONSUMING THE PRODUCER'S STREAM");

                    Dictionary<string, object> tempDic = new Dictionary<string, object> 
                    {
                        {"peerId", PeerId },
                        {"label", label},
                        {"appData",new AppData()}
                    };

                    await localPeer.Consume(tempDic);
                }
                else
                {
                    _ = Emit("stream-available", new Dictionary<string, object>
                    {
                        { "label", label },
                        { "labelData", new Dictionary<string, string> { { "producerId", producerId } } }
                    });
                }
            }
            catch (Exception error)
            {
                Debug.Log($"Error While Consuming | Error: {error}");
                _ = Emit("stream-available", new Dictionary<string, object>
                {
                    { "label", label },
                    { "labelData", new Dictionary<string, string> { { "producerId", producerId } } }
                });
            }
        }

        // Remove a Label from the Remote Peer and emit a `stream-closed` event
        public void RemoveLabelData(string label, (int code, string tag, string message)? reason = null)
        {
            _labelsToProducerId.Remove(label);
            _ = Emit("stream-closed", label, reason);
        }

        public Consumer<Mediasoup.Types.AppData> GetConsumer(string label) 
        {   
            LocalPeer localPeer = LocalPeer.Instance;
            return localPeer.GetConsumer(label, PeerId);
        }

        public void SetMetadata(string metadata) 
        {
            _metadata = metadata;
        }

    }
}

#endif

