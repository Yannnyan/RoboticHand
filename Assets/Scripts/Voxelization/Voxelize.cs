using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoboticHand;



public class Voxelize : MonoBehaviour
{
    public MeshPoints meshpoints;
    public CreateInputFile inputFile;


    // Start is called before the first frame update
    void Start()
    {
        string F_TO_READ = @"C:\Users\Yan\Desktop\input_mesh_wrist.csv";
        meshpoints.createMesh(inputFile.read_inputFile(F_TO_READ).Item3[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
