using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Client : MonoBehaviour
{
    [SerializeField] public string URL = "localhost:8000/";

    private void Start()
    {
        //StartCoroutine(GetDataFromServer());
    }
    public void SedMesh(string[] meshArray)
    {
        string meshMessege = string.Join(",", meshArray);
        StartCoroutine(SendDataToServer(meshMessege));
    }

    private IEnumerator SendDataToServer(string data)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(URL, data))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                // Process the response data
                string responseData = webRequest.downloadHandler.text;
                Debug.Log("Response: " + responseData);
            }
        }
    }
}