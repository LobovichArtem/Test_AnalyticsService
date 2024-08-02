using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class EventService : MonoBehaviour
{
    private const string SERVER_URL = "http://localhost:8080/";
    private float _cooldownBeforeSend = 2.0f;

    private Coroutine _cooldownCoroutine;
    private SaveLoadSystem _saveLoadSystem;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        _saveLoadSystem = new SaveLoadSystem();

        if (_cooldownCoroutine == null)        
            _cooldownCoroutine = StartCoroutine(CooldownCoroutine());
    }

    

    public void TrackEvent(string type, string data)
    {
        SavePendingEvents(new EventData(type, data));
    }


    private IEnumerator CooldownCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_cooldownBeforeSend);
            StartCoroutine(SendQueuedEventsAsync());
        }
    }

    private IEnumerator SendQueuedEventsAsync()
    {
        //List<EventData> data = new List<EventData>();
        var data = _saveLoadSystem.LoadLastPendingEvents();
        if (data == null || data.Count == 0)
            yield break;

        EventData[] list = new EventData[data.Count];
        data.CopyTo(list);

        var eventsJson = JsonConvert.SerializeObject(data);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(SERVER_URL, eventsJson))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error sending events to analytics server: {request.error}");
            }
            else
            {
                if (request.responseCode == 200)
                {
                    _saveLoadSystem.ClearLastPendingEvents();
                    Debug.Log("Events sent successfully.");
                }
                else
                {
                    Debug.Log($"WTF: {request.responseCode}");
                }
            }
        }


    }

    private void SavePendingEvents(EventData eventData) => _saveLoadSystem.SavePendingEvent(eventData);

}
