using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Newtonsoft.Json;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Huddle01.Sample
{
    public class UserSectionBase : MonoBehaviour
    {
        private HuddleUserInfo _userInfo;

        public HuddleUserInfo UserInfo => _userInfo;

        [SerializeField]
        private TMP_Text _nameText;

        [SerializeField]
        private GameObject _mutedIcon;

        [SerializeField]
        private GameObject _unmutedIcon;

        [SerializeField]
        private RawImage _videoTexture;

        public Texture2D Texture { get; private set; }

        public bool isVideoPlaying=false;

        private int m_TextureId =1;

        private void Start()
        {
            GetNewTextureId();
        }


        private void Update()
        {
            if (isVideoPlaying) 
            {
                SetupTexture();
            }
        }

        public void Setup(HuddleUserInfo userInfo)
        {
            _userInfo = userInfo;
            _mutedIcon.SetActive(true);
        }

        public void MuteUser() 
        {
            SetMuteIcon(true);
        }

        public void MuteUser(bool isMuted) 
        {
            SetMuteIcon(isMuted);
        }

        public void UpdateMetadata(string peerMetaData)
        {
            _userInfo.Metadata = peerMetaData;
            _nameText.text = _userInfo.Metadata;
        }

        private void SetMuteIcon(bool muted)
        {
            _mutedIcon.SetActive(muted);
            _unmutedIcon.SetActive(!muted);
        }

        public void GetNewTextureId() 
        {
#if UNITY_WEBGL
            m_TextureId = Huddle01JSNative.NewTexture();
#endif
        }

        public void SetupTexture() 
        {
            if (Texture != null)
                Object.Destroy(Texture);
            Texture = Texture2D.CreateExternalTexture(1280, 720, TextureFormat.RGBA32, false, true, (IntPtr)m_TextureId);
            _videoTexture.texture = Texture;
        }

        public void AttachVideo() 
        {
#if UNITY_WEBGL
           Huddle01JSNative.AttachVideo(_userInfo.PeerId, m_TextureId);
#endif
        }

        public void StopVideo() 
        {
            isVideoPlaying = false;
            _videoTexture.gameObject.SetActive(false);
        }

        public void ResumeVideo()
        {
#if UNITY_WEBGL
           _videoTexture.gameObject.SetActive(true);
            Huddle01JSNative.AttachVideo(_userInfo.PeerId, m_TextureId);
            isVideoPlaying = true;
#endif

        }
    }
}

