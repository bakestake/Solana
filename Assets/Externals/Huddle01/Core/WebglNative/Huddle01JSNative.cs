using AOT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Huddle01 
{
    public class Huddle01JSNative
    {
#if UNITY_WEBGL
        public delegate void JSDelegate(string message);

        [DllImport("__Internal")]
        internal static extern int NewTexture();

        [DllImport("__Internal")]
        internal static extern void AttachVideo(string peerId, int m_TextureId);

        [DllImport("__Internal")]
        internal static extern void InitHuddle01WebSdk(string appIdjson,bool autoCosume);

        [DllImport("__Internal")]
        internal static extern void JoinRoom(string roomId, string token);

        [DllImport("__Internal")]
        internal static extern void LeaveRoom();

        [DllImport("__Internal")]
        internal static extern void EnableAudio();

        [DllImport("__Internal")]
        internal static extern void DisableAudio();

        [DllImport("__Internal")]
        internal static extern void EnableVideo();

        [DllImport("__Internal")]
        internal static extern void DisableVideo();

        [DllImport("__Internal")]
        internal static extern void SendTextMessage(string message);

        [DllImport("__Internal")]
        internal static extern void ConsumePeer(string peerId,string label);

        [DllImport("__Internal")]
        internal static extern void StopConsumingPeer(string peerId, string label);

        [DllImport("__Internal")]
        internal static extern void UpdatePeerMetaData(string metadataJson);

        [DllImport("__Internal")]
        internal static extern string GetLocalPeerId();

        [DllImport("__Internal")]
        internal static extern string GetRemotePeerMetaData(string peerId);

        [DllImport("__Internal")]
        internal static extern void GetAudioMediaDevices();

        [DllImport("__Internal")]
        internal static extern void GetVideoMediaDevices();

#endif



    }
}