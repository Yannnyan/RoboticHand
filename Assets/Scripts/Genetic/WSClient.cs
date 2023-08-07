using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WSClient : MonoBehaviour
{
    WebSocket ws;

    public delegate void GotMessage(string message);

    private void Start()
    {
        
        
    }

    public void createSocket(GotMessage gotMessage)
    {
        ws = new WebSocket("ws://localhost:8000/ws");
        ws.Connect();
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message Received from " + ((WebSocket)sender).Url + ", Data : " + e.Data);
            gotMessage(e.Data);
        };
            
    }
    private void Update()
    {
        
    }
    public void SendImagesPaths(string[][] paths)
    {
        string json = Client.formatPaths(paths);
        ws.Send(json);
    }
    private string formatRays(float[] distances, string gene)
    {
        Dictionary<string, float[]> results = new Dictionary<string, float[]>();
        results[gene] = distances;
        string str = JsonConvert.SerializeObject(results);
        return str;
    }
    public void SendRaysResponse(Dictionary<string, float[]> distances_dict)
    {
        string json = JsonConvert.SerializeObject(distances_dict);
        ws.Send(json);
    }
}
