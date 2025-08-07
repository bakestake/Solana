using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace Huddle01.Sample
{
    public class RemotePeerWebgl : RemotePeerBase
    {
        public bool IsVideoPlaying => _isVideoPlaying;
        private bool _isVideoPlaying = false;

        private int m_TextureId = -1;

        public Texture2D Texture { get; private set; }

        private bool isVideoPlaying = false;

#if UNITY_WEBGL

        void Start()
        {
            GetNewTextureId();
        }

        void Update()
        {
            if (isVideoPlaying)
            {
                SetupTexture();
            }
        }

        public override void Init(string remotePeerId)
        {
            base.Init(remotePeerId);
        }

        public override void SetMetadata(MetadataInfo metadata)
        {
            _nameText.text = metadata.Name;
            base.SetMetadata(metadata);
        }

        public void GetNewTextureId()
        {
            m_TextureId = Huddle01JSNative.NewTexture();
        }

        public void SetupTexture()
        {
            if (Texture != null)
                UnityEngine.Object.Destroy(Texture);
            Texture = Texture2D.CreateExternalTexture(1280, 720, TextureFormat.RGBA32, false, true, (IntPtr)m_TextureId);
            _videoTexture.texture = Texture;
            
        }

        public void EnableVideo(string peerId) 
        {
            isVideoPlaying = true;
            Huddle01JSNative.AttachVideo(peerId, m_TextureId);
        }

        public void DisableVideo()
        {
            isVideoPlaying = false;
            _videoTexture.texture = _defaultTexture; 
        }

        public override void EnableAudio()
        {
            base.EnableAudio();
        }

        public override void DisableAudio()
        {
            base.DisableAudio();
        }

#endif
    }


}


