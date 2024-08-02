using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text;

public class SaveLoadSystem
{
    private const string FilePath = "pendingEvents.json";
    private static string _filePath;

    public SaveLoadSystem()
    {
        _filePath = Path.Combine(Application.persistentDataPath, FilePath);
    }

    public SaveLoadSystem(string filePath)
    {
        _filePath = Path.Combine(Application.persistentDataPath, filePath);
    }


    public void SavePendingEvent(EventData pendingEvent)
    {
        using (StreamWriter writer = File.AppendText(_filePath))
        {
            string json = JsonUtility.ToJson(pendingEvent);
            writer.WriteLine(json);
        }
    }


    public List<EventData> LoadLastPendingEvents()
    {
        if (!File.Exists(_filePath))        
            return null;
        
        List<EventData> data = new List<EventData>();
        using (StreamReader reader = new StreamReader(_filePath, Encoding.UTF8))
        {
            string line;
            while ((line = reader.ReadLine()) != null)            
                data.Add(JsonUtility.FromJson<EventData>(line));
        }
        return data;
    }

    public void ClearLastPendingEvents()
    {
        if (!File.Exists(_filePath))
            return;        

        using (StreamWriter writer = new StreamWriter(_filePath))
        {
            writer.Write(string.Empty);
        }
    }
}
