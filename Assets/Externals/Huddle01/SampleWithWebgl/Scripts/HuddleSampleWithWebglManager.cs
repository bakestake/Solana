using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Huddle01.Core;
using Huddle01.Core.Settings;
using Huddle01.Core.Services;
using Huddle01.Core.EventBroadcast;
using System;
using static Huddle01.Core.Services.WebSocketService;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using Huddle01.Services;
using Huddle01;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace Huddle01.Sample 
{
    public class HuddleSampleWithWebglManager : MonoBehaviour
    {
        public delegate void PeerMetadataUpdatedHandler(string peerId,MetadataInfo metadata);

        public static event PeerMetadataUpdatedHandler PeerMedataUpdate;

        private HuddleClient _huddleClientInstance;

        public string PlayerName => _playerName;
        private string _playerName;

        [SerializeField]
        private string _roomId;

        [SerializeField]
        private string _projectId;

#if !UNITY_WENGL
        [SerializeField]
        private DeviceHandler _deviceHandler;
#endif


        private bool _isAudioEnabled = false;
        private bool _isVideoEnabled = false;

        [Space(10)]
        [Header("Menus")]
        [SerializeField]
        private GameObject _mainMenu;
        [SerializeField]
        private GameObject _callSection;

        [Space(10)]
        [Header("Main Menu References")]
        [SerializeField]
        private TMP_InputField _nameInputField;

        [Space(10)]
        [Header("RemotePeer Holder")]
        [SerializeField]
        private Transform _remotePeerHolder;

        [Space(10)]
        [Header("Remote Prefab")]
        [SerializeField]
        private GameObject _remotePrefabForWebgl;
        [SerializeField]
        private GameObject _remotePrefabForNative;

        [Space(10)]
        [Header("Options Ref")]
        [SerializeField]
        private Button _micButton;
        [SerializeField]
        private Button _cameraButton;
        [SerializeField]
        private Image _micImage;
        [SerializeField]
        private Image _cameraImage;
        [SerializeField]
        private Sprite _micMutedSprite;
        [SerializeField]
        private Sprite _micUnMutedSprite;
        [SerializeField]
        private Sprite _cameraDisabledSprite;
        [SerializeField]
        private Sprite _cameraEnabledSprite;

        public Dictionary<string, GameObject> RemotePeers => _remotePeers;
        private Dictionary<string, GameObject> _remotePeers = new Dictionary<string, GameObject>();

        public string LocalPeerId=>_localPeerId;
        private string _localPeerId;

        [SerializeField]
        private LocalPeerSection _localPeerSection;

        private MetadataInfo _localPeerMetadata = new MetadataInfo();

#region Unity Callback

        void Start()
        {

#if UNITY_WEBGL
            HuddleClient.Instance.InitForWebgl(_projectId, _roomId);
#else
            HuddleClient.Instance.InitForNative(_projectId,_roomId, _deviceHandler);
#endif

            _huddleClientInstance = HuddleClient.Instance;
            SubscribeEvents();
        }

        
        void Update()
        {

        }

#endregion

#region Main Methods

        public void JoinRoom()
        {
            if (string.IsNullOrEmpty(_nameInputField.text))
            {
                _playerName = "Guest";
            }
            else 
            {
                _playerName = _nameInputField.text;
            }

            
            _localPeerMetadata.Name = _playerName;
            _huddleClientInstance.JoinRoom(_roomId);
        }

        public void LeaveRoom()
        {
            _huddleClientInstance.LeaveRoom();
        }

        public void OnAudioButtonClick()
        {
            if (_isAudioEnabled)
            {
                DisableAudio();
            }
            else
            {
                EnableAudio();
            }

            _isAudioEnabled = !_isAudioEnabled;
        }

        public void OnVideoButtonClick()
        {
            if (_isVideoEnabled)
            {
                DisableVideo();
            }
            else
            {
                EnableVideo();
            }

            _isVideoEnabled = !_isVideoEnabled;
        }

        private void DisableAudio()
        {
            _huddleClientInstance.DisableAudio();
            _micImage.sprite = _micMutedSprite;
            StartCoroutine(EnableButtonAfterDelay(_micButton, 2));
        }

        private void EnableAudio()
        {
            _huddleClientInstance.EnableAudio();
            _micImage.sprite = _micUnMutedSprite;
            StartCoroutine(EnableButtonAfterDelay(_micButton, 2));
        }

        private void DisableVideo()
        {
            _huddleClientInstance.DisableVideo();
            _cameraImage.sprite = _cameraDisabledSprite;
            StartCoroutine(EnableButtonAfterDelay(_cameraButton, 2));
        }

        private void EnableVideo()
        {
            _huddleClientInstance.EnableVideo();
            _cameraImage.sprite = _cameraEnabledSprite;
            StartCoroutine(EnableButtonAfterDelay(_cameraButton, 2));
        }

        private void UpdateMetadata(MetadataInfo metadataInfo)
        {
            _huddleClientInstance.UpdateMetadata(JsonConvert.SerializeObject(metadataInfo));
        }

#endregion

#region Event handlers

        private void OnPeerMetadataUpdated(string peerId)
        {
            if (peerId.Equals(_localPeerId))
            {
                return;
            }

            Debug.Log($"Get metadata for {peerId}");
            string metadata =  _huddleClientInstance.GetMetadataOfRemotePeers(peerId);
            PeerMedataUpdate?.Invoke(peerId,JsonConvert.DeserializeObject<MetadataInfo>(metadata));
        }

        private void OnRoomClosed()
        {
            _huddleClientInstance.LeaveRoom();
            Application.Quit();
        }

        private void OnPeerUnmuted(string peerId)
        {
            if (peerId.Equals(_localPeerId)) 
            {
                return;
            }

            //for remote peer
#if UNITY_WEBGL

            RemotePeerWebgl remotePeerNative = _remotePeers[peerId].GetComponent<RemotePeerWebgl>();
            remotePeerNative.EnableAudio();
#endif

        }

        private void OnPeerMuted(string peerId)
        {
            if (peerId.Equals(_localPeerId))
            {
                return;
            }

            //for remote peer
#if UNITY_WEBGL

            RemotePeerWebgl remotePeerNative = _remotePeers[peerId].GetComponent<RemotePeerWebgl>();
            remotePeerNative.DisableAudio();
#endif
        }

        private void OnResumePeerVideo(string peerId)
        {
            if (peerId.Equals(_localPeerId))
            {
                //handle for webgl
#if UNITY_WEBGL
                _localPeerSection.EnableVideo(peerId);
#endif
                return;
            }

            //for remote peer
            //only for webgl
#if UNITY_WEBGL

            RemotePeerWebgl remotePeerNative = _remotePeers[peerId].GetComponent<RemotePeerWebgl>();
            remotePeerNative.EnableVideo(peerId);
#endif

        }

        private void OnStopPeerVideo(string peerId)
        {
            if (peerId.Equals(_localPeerId))
            {
                //handle for webgl
#if UNITY_WEBGL
                _localPeerSection.DisableVideo();
#endif
                return;
            }

            //for remote peer
            //handle for webgl as native has already subscribed
#if UNITY_WEBGL

            RemotePeerWebgl remotePeerNative = _remotePeers[peerId].GetComponent<RemotePeerWebgl>();
            remotePeerNative.DisableVideo();
#endif

        }

        private void OnPeerLeft(string peerId)
        {
            if (peerId.Equals(_localPeerId)) 
            {
                return;
            }

            Destroy(_remotePeers[peerId]);
            _remotePeers.Remove(peerId);
        }

        private void OnPeerAdded(string peerId)
        {
            if (peerId.Equals(_localPeerId))
            {
                return;
            }

            //instantiate remote peer prefab for webgl and for native
            GameObject tempRemotePeer = null;
#if !UNITY_WEBGL
            tempRemotePeer = InstantiateRemotePeerForNative();
            RemotePeerNative remotePeerNative = tempRemotePeer.GetComponent<RemotePeerNative>();
            remotePeerNative.Init(peerId);
#else
            tempRemotePeer = InstantiateRemotePeerForWebgl();
            RemotePeerWebgl remotePeerNative = tempRemotePeer.GetComponent<RemotePeerWebgl>();
            remotePeerNative.Init(peerId);
#endif

            _remotePeers[peerId] = tempRemotePeer;
            Debug.Log("Peer Added");

        }

        private void MessageReceived(string data)
        {
            //handle for native
            //handle for webgl
        }

        private void OnLeaveRoom()
        {
            Application.Quit();
        }

        private void OnRoomJoined()
        {
            _localPeerId = _huddleClientInstance.GetLocalPeerId();
            Debug.Log($"Local Peer Id {_localPeerId}");
            _mainMenu.SetActive(false);
            _callSection.SetActive(true);

            UpdateMetadata(_localPeerMetadata);
            _localPeerSection.SetupNameText(_localPeerMetadata.Name);
        }

#endregion

        private void SubscribeEvents() 
        {
            HuddleClient.OnJoinRoom += OnRoomJoined;
            HuddleClient.OnLeaveRoom += OnLeaveRoom;

            HuddleClient.OnMessageReceived += MessageReceived;

            HuddleClient.PeerAdded += OnPeerAdded;
            HuddleClient.PeerLeft += OnPeerLeft;

            HuddleClient.OnStopPeerVideo += OnStopPeerVideo;
            HuddleClient.OnResumePeerVideo += OnResumePeerVideo;

            HuddleClient.PeerMuted += OnPeerMuted;
            HuddleClient.PeerUnMuted += OnPeerUnmuted;

            HuddleClient.RoomClosed += OnRoomClosed;

            HuddleClient.PeerMetadata += OnPeerMetadataUpdated;
        }

        private void UnSubscribeEvents()
        {
            HuddleClient.OnJoinRoom -= OnRoomJoined;
            HuddleClient.OnLeaveRoom -= OnLeaveRoom;

            HuddleClient.OnMessageReceived -= MessageReceived;

            HuddleClient.PeerAdded -= OnPeerAdded;
            HuddleClient.PeerLeft -= OnPeerLeft;

            HuddleClient.OnStopPeerVideo -= OnStopPeerVideo;
            HuddleClient.OnResumePeerVideo -= OnResumePeerVideo;

            HuddleClient.PeerMuted -= OnPeerMuted;
            HuddleClient.PeerUnMuted -= OnPeerUnmuted;

            HuddleClient.RoomClosed -= OnRoomClosed;

            HuddleClient.PeerMetadata -= OnPeerMetadataUpdated;
        }

#region Remote Peer Instantiate

        private GameObject InstantiateRemotePeerForWebgl() 
        {
            GameObject go = Instantiate(_remotePrefabForWebgl, _remotePeerHolder);
            return go;
        }

        private GameObject InstantiateRemotePeerForNative()
        {
            GameObject go = Instantiate(_remotePrefabForNative, _remotePeerHolder);
            return go;
        }

#endregion

        private void OnDestroy()
        {
            UnSubscribeEvents();
        }

        IEnumerator EnableButtonAfterDelay(Button btn, int delayInSec) 
        {
            btn.interactable = false;
            yield return new WaitForSeconds(delayInSec);
            btn.interactable = true;
        }

    }

    [Serializable]
    public class MetadataInfo 
    {
        [JsonProperty("name")]
        public string Name;
    }

}