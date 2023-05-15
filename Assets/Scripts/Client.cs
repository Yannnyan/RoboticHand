using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class Client : MonoBehaviour
{
    [SerializeField] public string URL = "http://localhost:8000/";
    [SerializeField]
    ConvextModel model;

    private void Start()
    {
        //SedMesh();
    }
    public void SedMesh(string meshMessege)
    {
        StartCoroutine(SendDataToServer(meshMessege));
    }

    private IEnumerator SendDataToServer(string data)
    {
        Debug.Log(data);
        //Debug.Log("Sending msg");
        string json = "{\"message\":\"" + data + "\"}";
        UnityWebRequest request = UnityWebRequest.Post(URL, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        Debug.Log(json);
        // Send the request and wait for the response
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Request was successful!");
            Debug.Log("Response: " + request.downloadHandler.text);
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.downloadHandler.text);
            Debug.Log(values["message"].Trim('[', ']'));
            string[] arr = values["message"].Trim('[', ']').Split(" ");
            float[] convex = new float[6000];
            for (int j=0; j< arr.Length; j++)
            {
                Debug.Log(arr[j]);
                if (string.IsNullOrEmpty(arr[j]))
                    continue;
                convex[j] = float.Parse(arr[j]);
            }
            float[] input = new float[6006];
            int i = 0; for (; i< 3; i++)
            {
                input[i] = 0;
            }
            for (i = 3; i < 6; i++)
            {
                input[i] = 244;
            }
            for(;i < input.Length; i++)
            {
                input[i] = convex[i-6];
            }
            model.Predict(input);
        }
        else
        {
            Debug.Log("Request failed with error: " + request.error);
        }
    }
}