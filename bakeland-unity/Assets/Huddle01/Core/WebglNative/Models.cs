using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class HuddleUserInfo
{
    public string PeerId;
    public string Metadata;
    public bool IsRemotePeer;
    public string Role;
}

[System.Serializable]
public class JoinRoomResponse
{
    [JsonProperty("token")]
    public string Token;
    [JsonProperty("hostUrl")]
    public string HostUrl;
    [JsonProperty("redirectUrl")]
    public string RedirectUrl;

    public string ConvertToJSON()
    {
        return JsonUtility.ToJson(this);
    }

}

[System.Serializable]
public class MessageReceivedResponse 
{
    [JsonProperty("from")]
    public string From;

    [JsonProperty("label")]
    public string Label;

    [JsonProperty("payload")]
    public string Payload;
}


