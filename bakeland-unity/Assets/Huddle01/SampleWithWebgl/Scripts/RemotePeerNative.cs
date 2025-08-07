using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !UNITY_WEBGL
using Unity.WebRTC;
#endif
using Huddle01.Core;
using Mediasoup;
using System;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

namespace Huddle01.Sample
{
    public class RemotePeerNative : RemotePeerBase
    {
        
#if !UNITY_WEBGL

        private VideoStreamTrack _videotrack;

        private GameObject _audioRef;

        public RemotePeer RemotePeer => _remotePeer;
        private RemotePeer _remotePeer;

        List<Tuple<Action<Consumer<Mediasoup.Types.AppData>, string>, Consumer<Mediasoup.Types.AppData>, string>> ConsumeTasks =
                            new List<Tuple<Action<Consumer<Mediasoup.Types.AppData>, string>, Consumer<Mediasoup.Types.AppData>, string>>();
        List<Tuple<Action<string, string>, string, string>> CloseConsumerTasks =
                                new List<Tuple<Action<string, string>, string, string>>();


        void Start()
        {

        }

        void Update()
        {
            if (_remotePeer != null) 
            {
                _remotePeer.On("stream-playable", async (arg) =>
                {
                    try
                    {
                        Consumer<Mediasoup.Types.AppData> consumer = arg[0] as Consumer<Mediasoup.Types.AppData>;
                        string label = arg[1] as string;

                        if (label.Equals("audio"))
                        {
                            var temp = new Tuple<Action<Consumer<Mediasoup.Types.AppData>, string>, Consumer<Mediasoup.Types.AppData>, string>(EnableAudio, consumer, label);
                            ConsumeTasks.Add(temp);
                        }
                        else if (label.Equals("video"))
                        {
                            var temp = new Tuple<Action<Consumer<Mediasoup.Types.AppData>, string>, Consumer<Mediasoup.Types.AppData>, string>(EnableVideo, consumer, label);
                            ConsumeTasks.Add(temp);
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                });

                _remotePeer.On("stream-closed", async (arg) =>
                {
                    var temp = new Tuple<Action<string, string>, string, string>(OnCloseTrack, arg[0] as string, arg[1] as string);
                    CloseConsumerTasks.Add(temp);
                });
            }

            Room.Instance.On("peer-left", async (arg) => DestroyThisObject());
        }

        public override void Init(string remotePeerId)
        {
            base.Init(remotePeerId);
            _remotePeer = Room.Instance.GetRemotePeerById(remotePeerId);
        }

        public override void SetMetadata(MetadataInfo metadata)
        {
            _nameText.text = metadata.Name;
            base.SetMetadata(metadata);
        }

        public void EnableVideo(Consumer<Mediasoup.Types.AppData> consumer, string label)
        {
            Debug.Log("Play video stream");
            try
            {
                _videotrack = consumer.track as VideoStreamTrack;
                _videotrack.OnVideoReceived += (tex) =>
                {
                    Debug.Log($"tex format {tex.graphicsFormat}");
                    _videoTexture.texture = tex;

                };
                _videotrack.Enabled = true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Texture format issue {ex}");
            }
        }

        public void EnableAudio(Consumer<Mediasoup.Types.AppData> consumer, string label)
        {
            SetAudioSource(consumer.track);
            //_micStatus.color = Color.green;
        }

        private void SetAudioSource(MediaStreamTrack track)
        {
            try
            {
                _audioRef = new GameObject("AudioSource");
                _audioRef.transform.SetParent(this.transform);
                AudioSource aud = _audioRef.AddComponent<AudioSource>();

                AudioStreamTrack audioTrack = track as AudioStreamTrack;

                aud.SetTrack(audioTrack);
                aud.loop = true;
                aud.Play();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Cant play audio {ex}");
            }
        }

        public void DisableVideo(string peerId)
        {
            _videotrack.OnVideoReceived -= (tex) => { _videoTexture.texture = tex; };
            _videotrack.Enabled = false;
            //_videoTexture.texture = _defaultTextureForVideo;
        }

        public void DisableAudio(string peerId)
        {
            Destroy(_audioRef);
            _audioRef = null;
            //_micStatus.color = Color.red;
        }

        public void OnCloseTrack(string label, string peerId)
        {
            if (label.Equals("audio"))
            {
                DisableAudio(peerId);
            }

            if (label.Equals("video"))
            {
                DisableVideo(peerId);
            }
        }

        protected override void PeerMetadataUpdated(string peerId, MetadataInfo metadata)
        {
            if (peerId.Equals(_remotePeer.PeerId)) 
            {
                return;
            }

            _nameText.text = metadata.Name;
            base.PeerMetadataUpdated(peerId, metadata);
        }

        public override void DestroyThisObject()
        {
            if (_audioRef!=null) 
            {
                AudioSource aud = _audioRef.AddComponent<AudioSource>();
                if (aud.isPlaying) 
                {
                    aud.Stop();
                }
            }

            base.DestroyThisObject();
        }

#endif

    }
}





