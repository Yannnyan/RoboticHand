using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoboticHand;
using System.IO;


public class Extractor : MonoBehaviour
{
    public CreateInputFile inputfile;


    // Start is called before the first frame update
    void Start()
    {
        GameObject holder = GameObject.Find("ObjectHolder");

        List<(string, List<Vector3>)> to_record = new List<(string, List<Vector3>)>();

        foreach (Transform t in holder.transform)
        {
            // include objects in the gameobject list
            string name = t.name;
            List<Vector3> mesh_p = inputfile.getMeshPoints(t.gameObject);
            to_record.Add((name, mesh_p));
        }
        string path = @"C:\Users\Yan\Desktop\record.csv";
        using (var w = new StreamWriter(path,true))
        {
            for (int i = 0; i < to_record.Count; i++)
            {
                w.Write(string.Format("{0},", to_record[i].Item1));
                w.Write(string.Format("{0},",0.ToString()));
                for(int j=0; j< to_record[i].Item2.Count; j++)
                {
                    w.Write(string.Format("{0},{1},{2},", to_record[i].Item2[j].x, to_record[i].Item2[j].y, to_record[i].Item2[j].z));
                }
                w.WriteLine();
            }    
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }



}
