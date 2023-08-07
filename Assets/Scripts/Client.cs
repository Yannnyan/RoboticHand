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

    private string formatImages(byte[][][] images)
    {
        Dictionary<string, Dictionary<string, byte[]>> jsonDictionary = new Dictionary<string, Dictionary<string, byte[]>>();

        for (int i = 0; i < images.Length; i++)
        {
            string objectKey = "object_" + i.ToString();
            Dictionary<string, byte[]> perspectiveDictionary = new Dictionary<string, byte[]>();

            for (int j = 0; j < images[i].Length; j++)
            {
                string perspectiveKey = "perspective_" + j.ToString();
                perspectiveDictionary[perspectiveKey] = images[i][j];
            }

            jsonDictionary[objectKey] = perspectiveDictionary;
        }

        string json = JsonConvert.SerializeObject(jsonDictionary);
        return json;
    }

    public static string formatPaths(string[][] paths)
    {
        Dictionary<string, Dictionary<string, string>> jsondict = new Dictionary<string, Dictionary<string, string>>();

        for (int i = 0; i < paths.Length; i++)
        {
            string objectKey = "object_" + i.ToString();
            Dictionary<string, string> perspectiveDictionary = new Dictionary<string, string>();
            for (int j = 0; j < paths[i].Length; j++)
            {
                string perspectiveKey = "perspective_" + j.ToString();
                perspectiveDictionary[perspectiveKey] = paths[i][j];
            }
            jsondict[objectKey] = perspectiveDictionary;
        }

        string json = JsonConvert.SerializeObject(jsondict);
        return json;
    }

    public void SendImagesPaths(string[][] paths)
    {
        string json = formatPaths(paths);
        StartCoroutine(SendDataToPygadServer(json));
        
    }
    public void SendImages(byte[][][] images)
    {
        string json = formatImages(images);
        StartCoroutine(SendDataToPygadServer(json));
    }

    private IEnumerator SendDataToPygadServer(string json)
    /**
     * Recieves the data formatted json and sends it to the server
     * 
     */
    {
        UnityWebRequest request = UnityWebRequest.Post(URL, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        // Send the request and wait for the response
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Request was successful!");
        }

    }

    public delegate void onServerMessage(float[] rotations);

    public void SedMesh(string meshMessege, Vector3[] handPosRot, onServerMessage onmsg)
    {
        StartCoroutine(SendDataToServer(meshMessege, handPosRot, onmsg));
    }

    private IEnumerator SendDataToServer(string data, Vector3[] handPosRot, onServerMessage onmsg)
    {
        Debug.Log(data);

        string wr_data = string.Format("{0},{1},{2},{3},{4},{5}", handPosRot[0].x, handPosRot[0].y, handPosRot[0].z,
                                                                  handPosRot[1].x, handPosRot[1].y, handPosRot[1].z);

        string json = "{\"message\":\"" + data + "\", \"wrist\":\"" + wr_data + "\"}";

        UnityWebRequest request = UnityWebRequest.Post(URL, "POST");

        request.SetRequestHeader("Content-Type", "application/json");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Request was successful!");
            // get data from server response
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.downloadHandler.text);

            Debug.Log(values["message"]);

            string[] arr = values["message"].Trim('[', ']').Split(" ");

            float[] rotations_output = new float[54];

            if (arr.Length != rotations_output.Length)
                Debug.Log("rotations not in length 54!");

            for(int i = 0; i < arr.Length; i++)
            {
                rotations_output[i] = float.Parse(arr[i]);
            }
            onmsg(rotations_output);



        }
        else
        {
            Debug.Log("Request failed with error: " + request.error);
        }
        request.Dispose();
    }
}