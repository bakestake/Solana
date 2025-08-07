#if !UNITY_WEBGL
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.WebRTC;
using Huddle01.Core;
using Mediasoup;
using System;
using UnityEngine.UI;

public class RemotePeerSection : MonoBehaviour
{
    public AudioSource AudioSourceRef=> _audioSource;

    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private RemotePeer _remotePeerData;

    public AudioSpectrumView _audioSpectrum;

    List<Tuple<Action<Consumer<Mediasoup.Types.AppData>, string>, Consumer<Mediasoup.Types.AppData>, string>> ConsumeTasks =
                            new List<Tuple<Action<Consumer<Mediasoup.Types.AppData>, string>, Consumer<Mediasoup.Types.AppData>, string>>();
    List<Tuple<Action<string, string>, string, string>> CloseConsumerTasks =
                            new List<Tuple<Action<string, string>, string, string>>();

    public RawImage _videoTexture;

    private VideoStreamTrack _videotrack;

    private GameObject _audioRef;

    [SerializeField]
    private Image _micStatus;

    [SerializeField]
    private Texture2D _defaultTextureForVideo;


    void Start()
    {
        StartCoroutine(WebRTC.Update());
    }

    
    void Update()
    {
        while (ConsumeTasks.Count > 0)
        {
            ConsumeTasks[0].Item1.Invoke(ConsumeTasks[0].Item2, ConsumeTasks[0].Item3);
            ConsumeTasks.RemoveAt(0);
        }

        while (CloseConsumerTasks.Count > 0)
        {
            CloseConsumerTasks[0].Item1.Invoke(CloseConsumerTasks[0].Item2, CloseConsumerTasks[0].Item3);
            CloseConsumerTasks.RemoveAt(0);
        }

        if (_videotrack != null && _videotrack.ReadyState == TrackState.Live && _videotrack.Enabled) 
        {
            Debug.Log($"tex format {_videotrack.Texture == null}");

            _videoTexture.texture = _videotrack.Texture;
        }
    }

    public void Init(RemotePeer remotePeer) 
    {
        _remotePeerData = remotePeer;

       /* LocalPeer.Instance.On("stream-playable", async (arg) =>
        {
            Consumer<Mediasoup.Types.AppData> consumer = arg[0] as Consumer<Mediasoup.Types.AppData>;
            string label = arg[1] as string;
            SetAudioSource(consumer.track);
        });*/

        remotePeer.On("stream-playable", async (arg) =>
        {
            Debug.Log("Play stream");
            try 
            {
                Consumer<Mediasoup.Types.AppData> consumer = arg[0] as Consumer<Mediasoup.Types.AppData>;
                string label = arg[1] as string;

                Debug.Log(consumer.track.ReadyState);

                if (label.Equals("audio"))
                {
                    var temp = new Tuple<Action<Consumer<Mediasoup.Types.AppData>, string>, Consumer<Mediasoup.Types.AppData>, string>(PlayAudioStream, consumer, label);
                    ConsumeTasks.Add(temp);
                } else if (label.Equals("video")) 
                {
                    var temp = new Tuple<Action<Consumer<Mediasoup.Types.AppData>, string>, Consumer<Mediasoup.Types.AppData>, string>(PlayVideoStream, consumer, label);
                    ConsumeTasks.Add(temp);
                }

            } catch (Exception ex) 
            {
                Debug.LogError(ex);
            }
        });

        remotePeer.On("stream-closed", async (arg) =>
        {
            Debug.Log("Close stream");
            var temp = new Tuple<Action<string, string>,string,string>(CloseAudioTrack,arg[0] as string, arg[1] as string);
            CloseConsumerTasks.Add(temp);
        });

        Room.Instance.On("peer-left", async (arg) => DestroyThisObject());

    }

    private void DestroyThisObject() 
    {
        Destroy(this.gameObject);
    }


    private void CloseAudioTrack(string label,string peerId) 
    {
        //_audioRef.GetComponent<AudioSource>().Stop();
        //_audioRef.GetComponent<AudioSource>().loop = false;

        if (label.Equals("audio")) 
        {
            _audioSpectrum._audioTrack = null;
            _audioSpectrum.StartGraph = false;

            Destroy(_audioRef);
            _audioRef = null;

            _micStatus.color = Color.red;
        }

        if (label.Equals("video"))
        {
            _videotrack.OnVideoReceived -= (tex) => { _videoTexture.texture = tex; };
            _videotrack.Enabled = false;

            _videoTexture.texture = _defaultTextureForVideo;
        }
        
    }

    private void PlayAudioStream(Consumer<Mediasoup.Types.AppData> consumer,string label)
    {
        SetAudioSource(consumer.track);
        _audioSpectrum._audioTrack = consumer.track as AudioStreamTrack;
        _audioSpectrum.StartGraph = true;
        _micStatus.color = Color.green;
    }

    private void PlayVideoStream(Consumer<Mediasoup.Types.AppData> consumer, string label)
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
        } catch (Exception ex) 
        {
            Debug.LogError($"Texture format issue {ex}");
        }
        

      /*  _videotrack.OnVideoReceived += tex =>
        {
            _videoTexture.texture = tex;
        };*/
    }

    public void SetAudioSource(MediaStreamTrack track) 
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

    private void OnDestroy()
    {
        _audioSource.Stop();
    }

}
#endif

