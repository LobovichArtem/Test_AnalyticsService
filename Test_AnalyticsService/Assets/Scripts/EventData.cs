using System;

[Serializable]
public struct EventData
{
    public string Type;
    public string Data;

    public EventData(string type, string data)
    {
        Type = type;
        Data = data;
    }
}
