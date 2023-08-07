using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxelization;

namespace Voxelization
{
    public class MeshGenerator : MonoBehaviour
    {
        public Triangulize triangulize;

        public Mesh mesh;

        private MeshFilter meshFilter;

        private List<List<int>> mesh_triangles;

        private List<List<Vector3>> mesh_vertices;

        public string triangles_indices_path = @"C:\Users\Yan\Desktop\triangles.csv";

        private int current_index = 0;

        // Start is called before the first frame update
        void Start()
        {
            mesh = new Mesh();

            meshFilter = GetComponent<MeshFilter>();

            Debug.Log("Reading Mesh Triangles");

            mesh_triangles = triangulize.read_triangles_indices(triangles_indices_path);

            Debug.Log("Reading Mesh Vectors");

            mesh_vertices = triangulize.getMeshVectors();

            Debug.Log("Read " + mesh_triangles.Count.ToString() + " Triangles");

            Debug.Log("Read " + mesh_vertices.Count.ToString() + " Vectices");
            
        }

        // Update is called once per frame
        void Update()
        {
            if( Input.GetKey(KeyCode.Space) )
            {

                current_index++;

                int mesh_index = current_index % mesh_vertices.Count;

                Debug.Log("Displaying mesh_index: " + mesh_index.ToString());

                mesh.Clear();

                mesh.vertices = mesh_vertices[mesh_index].ToArray();

                mesh.triangles = mesh_triangles[mesh_index].ToArray();

                meshFilter.mesh = mesh;
            }    
        }
    }
}

