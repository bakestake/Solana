

using Mediasoup.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Huddle01.Core.Models;
using System;
#if !UNITY_WEBGL
using Unity.WebRTC;
#endif

namespace Huddle01.Core 
{
    public class DeviceHandler : MonoBehaviour
    {
        [SerializeField] private AudioSource inputAudioSource;

        [SerializeField]
        private UnityEngine.UI.RawImage _rawImage;

        [SerializeField]
        private Texture2D _defaultVideoTexture;

#if !UNITY_WEBGL

        public static string SelectedAudioDevice = "0";
        public static string SelectedCameraDevice = "0";

        private string _currentAudioStreamingDevice;
        private string _currentVideoStreamingDevice;

        private AudioClip _m_clipInput;
        private int m_samplingFrequency = 48000;
        private int m_lengthSeconds = 1;
        private string _selectedAudioDevice;

        private WebCamTexture _webCamTexture;

        VideoStreamTrack videoStreamTrack;

        RenderTexture renderTexture;

        private void Start()
        {
            StartCoroutine(WebRTC.Update());
        }

        public void Stop(MediaStream stream)
        {
            if (stream == null)
            {
                return;
            }

            foreach (MediaStreamTrack track in stream.GetTracks()) 
            {
                track.Stop();
            }
        }


        private void Update()
        {
            if (_webCamTexture!=null && _webCamTexture.isPlaying) 
            {
                Debug.Log("updating render texture");
                Graphics.Blit(_webCamTexture, renderTexture);

                if (videoStreamTrack != null && videoStreamTrack.Enabled) 
                {
                    //_rawImage.texture = videoStreamTrack.Texture;
                }

            }
        }

        public async Task<MediaStream> FetchStream(string deviceId,string label) 
        {
            if (label == "video") 
            {

                if (deviceId.Equals("0"))
                {
                    deviceId = WebCamTexture.devices[0].name;
                }

                _currentVideoStreamingDevice = deviceId;
                var tcs = new TaskCompletionSource<bool>();

                StartCoroutine(CaptureWebCamAndSetTcs(deviceId, tcs));
                await tcs.Task;
                Debug.Log("Creating web camera from web camera");
                try 
                {
                    videoStreamTrack = new VideoStreamTrack(renderTexture);
                    videoStreamTrack.Enabled = true;
                    MediaStream mediaStream = new MediaStream();
                    mediaStream.AddTrack(videoStreamTrack);
                    return mediaStream;
                } 
                catch (System.Exception ex) 
                {
                    Debug.LogError($"Cant fetch stream {ex}");
                    throw new System.Exception(ex.Message);
                }
                
            }

            if (label == "audio") 
            {
                if (deviceId.Equals("0"))
                {
                    deviceId = Microphone.devices[0];
                }

                _m_clipInput = Microphone.Start(deviceId,true,m_lengthSeconds,m_samplingFrequency);
                _currentAudioStreamingDevice = deviceId;
                inputAudioSource.loop = true;
                inputAudioSource.clip = _m_clipInput;
                inputAudioSource.Play();

                while (!(Microphone.GetPosition(deviceId) > 0)) { }

                MediaStream mediaStream = new MediaStream();
                AudioStreamTrack audioStreamTrack = new AudioStreamTrack(inputAudioSource);
                audioStreamTrack.Enabled = true;
                mediaStream.AddTrack(audioStreamTrack);

                return mediaStream;
            }

            return null;
            
        }

        private IEnumerator CaptureWebCamAndSetTcs(string deviceId, TaskCompletionSource<bool> tcs)
        {
            yield return CaptureWebCamVideo(deviceId);

            yield return new WaitForSeconds(0.1f);

            tcs.SetResult(true);
        }


        public void StopStream(MediaStream stream,string label) 
        {
            Debug.Log($"Stop all streamsssss");
            if (stream == null) return;

            foreach (var item in stream.GetTracks())
            {
                item.Stop();
                item.Dispose();
            }

            if (label == "audio") 
            {
                if (Microphone.IsRecording(Microphone.devices[0]))
                {
                    Microphone.End(Microphone.devices[0]);
                }
            }

            if (label == "video")
            {
                if (_webCamTexture.isPlaying)
                {
                    _webCamTexture.Stop();
                    _rawImage.texture = _defaultVideoTexture;
                }
            }

        }

        public async Task<MediaStream> FetchAudioStream(string deviceId)
        {
            if (deviceId.Equals("0"))
            {
                deviceId = Microphone.devices[0];
            }

            _m_clipInput = Microphone.Start(deviceId, true, m_lengthSeconds, m_samplingFrequency);

            inputAudioSource.loop = true;
            inputAudioSource.clip = _m_clipInput;
            inputAudioSource.Play();

            while (!(Microphone.GetPosition(deviceId) > 0)) { }

            MediaStream mediaStream = new MediaStream();
            AudioStreamTrack audioStreamTrack = new AudioStreamTrack(inputAudioSource);
            audioStreamTrack.Enabled = true;
            mediaStream.AddTrack(audioStreamTrack);

            return mediaStream;
        }

        private IEnumerator CaptureWebCamVideo(string deviceId)
        {

            if (_webCamTexture!=null && _webCamTexture.isPlaying)
            {
                _webCamTexture.Stop();
                _rawImage.texture = _defaultVideoTexture;
            }

            if (deviceId.Equals("0"))
            {
                deviceId = WebCamTexture.devices[0].name;
            }

            WebCamDevice userCameraDevice = GetVideoDeviceById(deviceId);
            _webCamTexture = new WebCamTexture(userCameraDevice.name, 1280, 720, 30);
            _webCamTexture.Play();

            yield return new WaitUntil(() => _webCamTexture.didUpdateThisFrame);

            // Create a RenderTexture with a compatible format
            if (renderTexture == null) 
            {
                renderTexture = new RenderTexture(1280, 720, 30, RenderTextureFormat.BGRA32);
                renderTexture.Create();
            }
            
            // Blit the WebCamTexture to the RenderTexture
            Graphics.Blit(_webCamTexture, renderTexture);

            _rawImage.texture = renderTexture;
        }

        private WebCamDevice GetVideoDeviceById(string deviceId) 
        {
            foreach (WebCamDevice device in WebCamTexture.devices)
            {
                if (device.name.Equals(deviceId)) 
                {
                    return device;
                }
            }

            return WebCamTexture.devices[0];
        }

        private string GetMicrophoneDevideById(string deviceId) 
        {
            foreach (string device in Microphone.devices)
            {
                if (device.Equals(deviceId))
                {
                    return device;
                }
            }

            return Microphone.devices[0];
        }

#region Change Track

        public async Task<MediaStream> ChangeMicTrack(MediaStream mediaStream, string newMicrophoneDeviceId)
        {
            try
            {
                MediaStream newAudioStream = await FetchAudioStream(newMicrophoneDeviceId);
                MediaStreamTrack newAudioTrack = newAudioStream.GetAudioTracks().FirstOrDefault()
                    ?? throw new Exception("Failed to fetch new audio track.");
                var currentAudioTrack = mediaStream.GetAudioTracks().FirstOrDefault();
                currentAudioTrack?.Stop();
                currentAudioTrack?.Dispose();
                mediaStream.RemoveTrack(currentAudioTrack);
                mediaStream.AddTrack(newAudioTrack);
                Debug.Log("mic track successfully updated");
                return mediaStream;
            }
            catch (Exception ex)
            {
                Debug.LogError($"error changing mic track: {ex.Message}");
                throw;
            }
        }

        // change cam track

        public async Task<MediaStream> ChangeCamTrack(MediaStream mediaStream, string newCameraDeviceId)
        {
            try
            {
                var tcs = new TaskCompletionSource<bool>();

                StartCoroutine(CaptureWebCamAndSetTcs(newCameraDeviceId, tcs));
                await tcs.Task;

              //  var currentVideoTrack = mediaStream.GetVideoTracks().FirstOrDefault();
             //   currentVideoTrack?.Stop();
               // mediaStream.RemoveTrack(currentVideoTrack);

              //  videoStreamTrack = new VideoStreamTrack(renderTexture);
              //  videoStreamTrack.Enabled = true;
               // mediaStream.AddTrack(videoStreamTrack);
                Debug.Log("Camera track successfully updated.");

                return mediaStream;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error updating camera track: {ex.Message}");
                throw;
            }
        }

#endregion

#endif

    }

}






