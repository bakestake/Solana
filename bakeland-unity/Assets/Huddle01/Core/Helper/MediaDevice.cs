using Newtonsoft.Json;

[System.Serializable]
public class MediaDevice
{
    [JsonProperty("deviceId")]
    public string DeviceId;

    [JsonProperty("kind")]
    public string Kind;

    [JsonProperty("label")]
    public string Label;

    [JsonProperty("groupId")]
    public string GroupId;
}
