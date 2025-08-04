using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Huddle01.Sample 
{
    public abstract class RemotePeerBase : MonoBehaviour
    {
        public string RemotePeerId => _remotePeerId;
        protected string _remotePeerId;

        public MetadataInfo Metadata => _metadata;
        protected MetadataInfo _metadata;

        [SerializeField]
        protected RawImage _videoTexture;

        [SerializeField]
        protected GameObject _muteIconImage;

        [SerializeField]
        protected GameObject _unmuteIconImage;

        [SerializeField]
        protected TMP_Text _nameText;

        [SerializeField]
        protected Texture2D _defaultTexture;


        public virtual void Init(string remotePeerId) 
        {
            _remotePeerId = remotePeerId;
            _muteIconImage.SetActive(true);
            _nameText.text = "";
            HuddleSampleWithWebglManager.PeerMedataUpdate += PeerMetadataUpdated;
        }


        public virtual void SetMetadata(MetadataInfo metadata) 
        {
            _metadata = metadata;
        }

        protected virtual void PeerMetadataUpdated(string peerId, MetadataInfo metadata) 
        {
            if (_remotePeerId.Equals(peerId)) 
            {
                _nameText.text = metadata.Name;
            }
        }

        public virtual void DestroyThisObject()
        {
            Destroy(this.gameObject);
        }

        public virtual void EnableAudio()
        {
            _muteIconImage.SetActive(false);
            _unmuteIconImage.SetActive(true);
        }

        public virtual void DisableAudio()
        {
            _muteIconImage.SetActive(true);
            _unmuteIconImage.SetActive(false);
        }

        private void OnDestroy()
        {
            HuddleSampleWithWebglManager.PeerMedataUpdate -= PeerMetadataUpdated;
        }


    }
}

