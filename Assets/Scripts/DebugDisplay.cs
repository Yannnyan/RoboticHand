using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugDisplay : MonoBehaviour
{
    Dictionary<string, string> debugLogs = new Dictionary<string, string>();
    Dictionary<string, int> indexLogs = new Dictionary<string, int>();
    public Text display;

    private void Update()
    {
        Debug.Log("time:" + Time.time);
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
        
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (logString.StartsWith("[OVRManager]") || logString.StartsWith("OVRControllerHelp:")
            || logString.StartsWith("The current MSAA level is"))
            return;
        if (type == LogType.Log)
        {
            string[] splitString = logString.Split(char.Parse(":"));
            string debugKey = splitString[0];
            string debugValue = splitString.Length > 1 ? splitString[1] : "";

            if (debugLogs.ContainsKey(debugKey))
            {
                debugLogs[debugKey] = debugValue + $" [{indexLogs[debugKey]++}]";
                
            }
            else
            {
                debugLogs.Add(debugKey, debugValue);
                indexLogs[debugKey] = 1;
            }

        }

        string displayText = "";
        foreach (KeyValuePair<string, string> log in debugLogs)
        {
            if (log.Value == "")
                displayText += log.Key + "\n";
            else
                displayText += log.Key + ": " + log.Value + "\n";
        }

        display.text = displayText;
    }
}