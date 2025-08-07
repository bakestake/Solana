#if !UNITY_WEBGL

using Mediasoup;
using Mediasoup.RtpParameter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Huddle01.Settings 
{
    public class RtpConstantParam
    {

        public static ProducerOptions<Mediasoup.Types.AppData> ProducerOptionsObjForVideo = new ProducerOptions<Mediasoup.Types.AppData>
        {
            encodings = {   new RtpEncodingParameters
                            {
                                Rid = "r0",
                                MaxBitrate = 300000,
                                ScalabilityMode = "S1T3",
                                ScaleResolutionDownBy = 4,
                                MaxFramerate = 15,
                            },
                            new RtpEncodingParameters
                            {
                                Rid = "r1",
                                MaxBitrate = 600000,
                                ScalabilityMode = "S1T3",
                                ScaleResolutionDownBy = 2,
                                MaxFramerate = 30,
                            },
                            new RtpEncodingParameters
                            {
                                Rid = "r2",
                                MaxBitrate = 9000000,
                                ScalabilityMode = "S1T3",
                                ScaleResolutionDownBy = 4,
                                MaxFramerate = 30,
                            }
                        },

            codecOptions = { videoGoogleStartBitrate = 1000 },
        };

        public static ProducerOptions<Mediasoup.Types.AppData> ProducerOptionsObjForAudio  = new ProducerOptions<Mediasoup.Types.AppData>
        {
            encodings = {   
                            new RtpEncodingParameters
                            {
                                MaxBitrate = 128000,
                            },
                        }

        };
    }
}

#endif






