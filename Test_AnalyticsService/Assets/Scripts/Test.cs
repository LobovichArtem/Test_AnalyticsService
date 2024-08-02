using UnityEngine;

public class Test : MonoBehaviour
{
    [ContextMenu("SendEvent")]
    public void SendEvent()
    {
        FindObjectOfType<EventService>().TrackEvent("TEST", Time.time.ToString());
    }

    [ContextMenu("SendTenEvent")]
    public void SendTenEvent()
    {
        var servise = FindObjectOfType<EventService>();
        for (int i = 0; i < 10; i++)
            Invoke("SendEvent", Random.Range(0.1f, 1f));
    }
}
