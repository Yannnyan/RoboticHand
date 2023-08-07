using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoboticHand;
using System.IO;

namespace Voxelization
{
    public class Triangulize : MonoBehaviour
    {
        private List<MeshFilter> meshFilters = new List<MeshFilter>();

        private List<List<Vector3>> mesh_vecs;

        public CreateInputFile createInputFile;

        public string WriteTrianglesPath = @"C:\Users\Yan\Desktop\triangles.csv";

        public string ReadAugmentsPath = @"C:\Users\Yan\Desktop\augment_input.csv";

        // Start is called before the first frame update
        void Start()
        {
            (List<int> record_num, List<List<Vector3>> wrist_vecs, List<List<Vector3>> mesh_vecs) = createInputFile.read_inputFile(ReadAugmentsPath);

            this.mesh_vecs = mesh_vecs;

            Debug.Log("Amount Lines Read: " + mesh_vecs.Count);

            write_Triangles_to_csv(WriteTrianglesPath, mesh_vecs, get_triangles());
        }

        // Update is called once per frame
        void Update()
        {

        }

        public List<List<Vector3>> getMeshVectors()
        {
            (List<int> record_num, List<List<Vector3>> wrist_vecs, List<List<Vector3>> mesh_vecs) = createInputFile.read_inputFile(ReadAugmentsPath);

            this.mesh_vecs = mesh_vecs;

            return this.mesh_vecs;
        }

        /**
         * 
         * perform reverse engineering to set the triangles to the mesh
         */
        private void write_Triangles_to_csv(string write_path, List<List<Vector3>> mesh_vecs, int[][] triangles)
        {
            Debug.Log("Writing lines into: " + write_path);

            // 
            using StreamWriter writer = new(write_path, false);

            int triangle_index;

            Vector3[] vecarr = { new Vector3(20, 0, 0), new Vector3(0, 20, 0), new Vector3(0, 0, 20) };

            for (int i = 0; i < mesh_vecs.Count;)
            {
                Debug.Log("Completed: " + i.ToString() + "iterations");
                for (int j = 0; j < vecarr.Length; j++)
                {

                    Vector3 rotateAngles = vecarr[j];

                    for (Vector3 rvec = Augment.modVec(rotateAngles, rotateAngles, 360);
                        rvec != rotateAngles;
                        rvec = Augment.modVec(rvec, rotateAngles, 360))
                    {
                        triangle_index = (i % triangles.Length);

                        i++; // 

                        writer.Write("" + triangle_index.ToString() + ",");

                        writer.WriteLine(string.Join(",", triangles[triangle_index]));
                    }
                }
            }
            writer.Close();

        }

        public List<List<int>> read_triangles_indices(string path)
        {
            List<List<int>> triangles = new List<List<int>>();
            
            using (var reader = new StreamReader(path))
            {
                while( !reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    string[] line_cells = line.Split(",");

                    List<int> lst = new();

                    bool first = true;

                    foreach (string cell in line_cells)
                    {
                        if(first)
                        {
                            first = false;
                            continue;
                        }
                        if (cell != "" && cell != " ")
                            lst.Add(int.Parse(cell));
                    }

                    triangles.Add(lst);
                }
                reader.Close();
            }

            return triangles;
        }

        private int[][] get_triangles()
        {
            GameObject objectHolder = GameObject.Find("ObjectHolder");

            List<string> children = new List<string>();

            int[][] triangles = new int[objectHolder.transform.childCount][];

            int tri_ind = 0;

            foreach (Transform child_t in objectHolder.transform)
            {
                string child = child_t.name;

                MeshFilter meshFilter = child_t.GetComponent<MeshFilter>();

                meshFilters.Add(meshFilter);

                children.Add(child);

                triangles[tri_ind++] = meshFilter.mesh.triangles;
            }

            children.ForEach((child) => Debug.Log(child));

            return triangles;
        }

    }

}
