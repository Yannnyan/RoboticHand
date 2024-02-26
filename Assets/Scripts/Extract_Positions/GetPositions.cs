using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Orientation;
using OfflineValidation;
using System.IO;
using System;
using Genetic;


public class GetPositions : MonoBehaviour
{
    public Orientation.Orientation HandOrient;
    public Dictionary<int, float[]> row_to_rotations;
    public string csvPath;
    public string outcsvPath;


    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string[] names = { "wrist", "thumb", "index", "middle", "ring", "pinky" };
            
            row_to_rotations = OfflineValidation.Inputter.readAllPredictions(csvPath);

            using (StreamWriter f = new StreamWriter(outcsvPath))
            {
                for (int i = 1; i <= row_to_rotations.Count; i++)
                {

                    float[] rotations = new float[54];

                    Debug.Log(row_to_rotations[i].Length);

                    Array.Copy(row_to_rotations[i], 54, rotations, 0, 54);

                    rotations = Genetic.Manager.normalize_output_360(rotations);

                    HandOrient.LoadAndTransformSecondary(rotations);
                    
                    var wrist = HandOrient.hand_dict["wrist"][0].transform.position;

                    for (int j = 0; j < names.Length; j++)
                    {
                        for (int k = 0; k < HandOrient.hand_dict[names[j]].Count; k++)
                        {
                            try
                            {
                                if (HandOrient.hand_dict[names[j]][k] == null)
                                    continue;
                                var t = HandOrient.hand_dict[names[j]][k].transform.position;

                                string joints_of_finger = string.Format("{0},{1},{2},", t.x - wrist.x, t.y - wrist.y, t.z - wrist.z);

                                f.Write(joints_of_finger);
                            }
                            catch(Exception e)
                            {
                                Debug.Log(e);
                                Debug.Log("Iterations number i: " + i.ToString() + " j: " + j.ToString() + " k: " + k.ToString());
                            }
                            

                        }

                    }
                    f.WriteLine();

                }
                f.Close();
            }
        }
        
    }
}
