#if !UNITY_WEBGL

using Google.Protobuf.Collections;
using Mediasoup.RtpParameter;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Huddle01.Helper 
{
    public class FrameworkExtensions
    {

    }

    public class ProtoEnumHelper
    {

        public static ProtoRtpParameters GetProtoRtpParameters(RtpParameters rtpParameters,string mediaType) 
        {
            ProtoRtpParameters protoRtp = new ProtoRtpParameters
            {
                Mid = rtpParameters.Mid,
                Rtcp = new RtcpParameters 
                {
                    Cname = rtpParameters.Rtcp.CNAME,
                    ReducedSize = rtpParameters.Rtcp.ReducedSize.Value
                } 
            };

            foreach (var codec in rtpParameters.Codecs) 
            {
                ProtoCodecParameters protoCodec = new ProtoCodecParameters
                {
                    Channels = codec.Channels.HasValue? codec.Channels.Value : 0 ,
                    ClockRate = (int)codec.ClockRate,
                    MimeType = codec.MimeType,
                    PayloadType = codec.PayloadType,
                };

                foreach (var rtcpFeedback in codec.RtcpFeedback)
                {
                    ProtoRtcpFeedback protoRtcpFeedback = new ProtoRtcpFeedback
                    {
                        Parameter = rtcpFeedback.Parameter,
                        Type = rtcpFeedback.Type
                    };
                    protoCodec.RtcpFeedback.Add(protoRtcpFeedback);
                }


                if (codec.Parameters != null) 
                {
                    foreach (var parameters in codec.Parameters)
                    {
                        if (parameters.Key != null && parameters.Value != null) 
                        {
                            string val = string.IsNullOrEmpty(parameters.Value.ToString()) ? "" : parameters.Value.ToString();
                            protoCodec.Parameters.Add(parameters.Key, val);
                        }
                    }
                }

                protoRtp.Codecs.Add(protoCodec);

            }

            
            foreach (var headerExtensionParameters in rtpParameters.HeaderExtensions)
            {
                ProtoHeaderExtensionParameters protoHeaderExtention = new ProtoHeaderExtensionParameters
                {
                    Encrypt = headerExtensionParameters.Encrypt,
                    Id = headerExtensionParameters.Id,
                    Uri = GetEnumMemberValue(headerExtensionParameters.Uri),
                };

                if (headerExtensionParameters.Parameters != null && headerExtensionParameters.Parameters.Count > 0) 
                {
                    foreach (var item in headerExtensionParameters.Parameters)
                    {
                        protoHeaderExtention.Parameters.Add(item.Key,item.Value as string);
                    }
                }

                protoRtp.HeaderExtensions.Add(protoHeaderExtention);
            }

            if (mediaType.Equals("audio"))
            {
                foreach (var encoding in rtpParameters.Encodings)
                {
                    ProtoEncodings protoEncoding = new ProtoEncodings
                    {
                        /*Active = encoding.Active,
                        CodecPayloadType = encoding.CodecPayloadType.HasValue ? (int)encoding.CodecPayloadType.Value : 0,
                        Dtx = encoding.Dtx,*/
                        MaxBitrate = encoding.MaxBitrate.HasValue ? (int)encoding.MaxBitrate.Value : 0,
                        /*  MaxFramerate = encoding.MaxFramerate.HasValue ? (int)encoding.MaxFramerate.Value : 0,
                          //Rid = encoding.Rid.h,
                          Rtx = new ProtoEncodings.Types.ProtoRTX
                          {
                              Ssrc = encoding.Ssrc.HasValue ? encoding.Ssrc.Value : 0
                          },
                          Ssrc = encoding.Ssrc.HasValue? encoding.Ssrc.Value : 0,
                          //ScalabilityMode = encoding.ScalabilityMode.?,
                          ScaleResolutionDownBy = encoding.ScaleResolutionDownBy.HasValue? encoding.ScaleResolutionDownBy.Value : 0*/
                    };

                    protoRtp.Encodings.Add(protoEncoding);
                }
            }
            else if (mediaType.Equals("vedio"))
            {
                foreach (var encoding in rtpParameters.Encodings)
                {
                    ProtoEncodings protoEncoding = new ProtoEncodings
                    {
                        Active = encoding.Active,
                        CodecPayloadType = encoding.CodecPayloadType.HasValue ? (int)encoding.CodecPayloadType.Value : 0,
                        Dtx = encoding.Dtx,
                        MaxBitrate = encoding.MaxBitrate.HasValue ? (int)encoding.MaxBitrate.Value : 0,
                        MaxFramerate = encoding.MaxFramerate.HasValue ? (int)encoding.MaxFramerate.Value : 0,
                        Rid = !string.IsNullOrEmpty(encoding.Rid)? encoding.Rid : null,
                        Rtx = new ProtoEncodings.Types.ProtoRTX
                        {
                            Ssrc = encoding.Ssrc.HasValue ? encoding.Ssrc.Value : 0
                        },
                        Ssrc = encoding.Ssrc.HasValue? encoding.Ssrc.Value : 0,
                        ScalabilityMode = !string.IsNullOrEmpty(encoding.ScalabilityMode) ? encoding.ScalabilityMode : null,
                        ScaleResolutionDownBy = encoding.ScaleResolutionDownBy.HasValue? encoding.ScaleResolutionDownBy.Value : 0
                    };

                    protoRtp.Encodings.Add(protoEncoding);
                }
            }

            return protoRtp;
        }

        public static string GetEnumMemberValue(RtpHeaderExtensionUri enumValue)
        {
            var memberInfo = typeof(RtpHeaderExtensionUri).GetMember(enumValue.ToString());

            var enumMemberAttribute = memberInfo[0]
                .GetCustomAttributes(typeof(EnumMemberAttribute), false)
                .Cast<EnumMemberAttribute>()
                .FirstOrDefault();

            return enumMemberAttribute != null ? enumMemberAttribute.Value : enumValue.ToString();
        }

        public static RtpHeaderExtensionDirection? GetRtpHeaderExtensionDirection(string value)
        {
            foreach (var field in typeof(RtpHeaderExtensionDirection).GetFields())
            {
                var attribute = field.GetCustomAttribute<EnumMemberAttribute>();
                if (attribute != null && attribute.Value.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    return (RtpHeaderExtensionDirection)field.GetValue(null);
                }
            }
            return null;
        }
        public static RtpHeaderExtensionUri? GetRtpHeaderExtensionUri(string value)
        {
            foreach (var field in typeof(RtpHeaderExtensionUri).GetFields())
            {
                var attribute = field.GetCustomAttribute<EnumMemberAttribute>();
                if (attribute != null && attribute.Value.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    return (RtpHeaderExtensionUri)field.GetValue(null);
                }
            }
            return null;
        }
        public static MediaKind? GetMediaKind(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            foreach (var field in typeof(MediaKind).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attribute = field.GetCustomAttribute<EnumMemberAttribute>();
                if (attribute != null && string.Equals(attribute.Value, value, StringComparison.OrdinalIgnoreCase))
                {
                    if (field.GetValue(null) is MediaKind kind)
                    {
                        return kind;
                    }
                }
            }
            return null;
        }
    }

    public static class EnumExtensions
    {
        public static T GetEnumByStringValue<T>(string stringValue) where T : Enum
        {
            // Get the type of the enum
            var type = typeof(T);
            // Iterate over each enum field
            foreach (var field in type.GetFields())
            {
                // Get the attribute from the enum field
                var attribute = field.GetCustomAttributes(typeof(StringValueAttribute), false)
                                     .FirstOrDefault() as StringValueAttribute;

                // If the attribute is found and its StringValue matches the input, return the corresponding enum value
                if (attribute != null && attribute.Value == stringValue)
                {
                    return (T)field.GetValue(null);
                }
            }

            // Optionally, handle the case where no match is found
            throw new ArgumentException($"No enum value found for string '{stringValue}'");
        }

        // Method to retrieve the string value from the StringValueAttribute
        public static string GetStringValue<T>(this T enumValue) where T : Enum
        {
            // Get the type of the enum
            var type = enumValue.GetType();

            // Get the field for the enum value
            var field = type.GetField(enumValue.ToString());

            // Get the StringValueAttribute associated with the enum value, if it exists
            var attribute = field.GetCustomAttributes(typeof(StringValueAttribute), false)
                                 .FirstOrDefault() as StringValueAttribute;

            // If the attribute exists, return the StringValue, otherwise return the enum's name
            return attribute != null ? attribute.Value : enumValue.ToString();
        }

    }


}

#endif





