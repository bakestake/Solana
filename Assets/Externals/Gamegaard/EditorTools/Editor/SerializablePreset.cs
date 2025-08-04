using System.Collections.Generic;

[System.Serializable]
public class SerializablePreset
{
    public Dictionary<string, bool> platformData;

    public SerializablePreset(Dictionary<string, bool> data)
    {
        platformData = data;
    }
}
