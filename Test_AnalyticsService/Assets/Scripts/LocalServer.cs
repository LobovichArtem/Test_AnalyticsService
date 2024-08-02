using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

public class LocalServer : MonoBehaviour
{
    private HttpListener listener;
    private bool isRunning;

    private void Start()
    {
        StartServer();
    }

    private void StartServer()
    {
        listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();
        isRunning = true;

        Debug.Log("Сервер запущен на http://localhost:8080/");
        ListenForRequests();
    }

    private async void ListenForRequests()
    {
        while (isRunning)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            ProcessRequest(context);
        }
    }

    private void ProcessRequest(HttpListenerContext context)
    {        
        string requestBody;
        using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
        {
            requestBody = reader.ReadToEnd();
        }

        string decodedJson = WebUtility.UrlDecode(requestBody);

        var eventData = JsonConvert.DeserializeObject<List<EventData>>(decodedJson);

        foreach(EventData data in eventData)
            Debug.Log($"Received Event: Type = {data.Type}, Data = {data.Data}");

        context.Response.Close();
    }

    private void OnApplicationQuit()
    {
        isRunning = false;
        listener.Stop();
        listener.Close();
        Debug.Log("Сервер остановлен.");
    }
}
