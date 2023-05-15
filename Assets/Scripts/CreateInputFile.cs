using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RoboticHand
{
    public class CreateInputFile : MonoBehaviour
    {
        private string fname = "input_mesh_wrist.csv";
        public string F_To_write;
        int max_index;
        
        public void Awake()
        {
            max_index = 100000;
            //F_To_write = @"C:\Users\Yan\Object_creation\input_mesh_wrist.csv";
            F_To_write = Path.Combine(Application.persistentDataPath, fname);
            F_To_write = @"C:\Users\Yan\Desktop\augment_input.csv"; // change me when recording!!!
            if (!File.Exists(F_To_write))
                write_inputFileHeaders(false);
        }

        public void setPath(string pathToCsv)
        {
            F_To_write = pathToCsv;
        }
        public List<Vector3> getMeshPoints(GameObject current_object)
        {
            Debug.Log("getMeshPoints: " + current_object.name);
            Transform transoform = current_object.transform;
            Mesh mesh = current_object.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            List<Vector3> lines_holder = new List<Vector3>();

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertexWorldPosition = transoform.TransformPoint(vertices[i]);
                //Debug.Log($"{vertexWorldPosition.x}, {vertexWorldPosition.y}, {vertexWorldPosition.z}");
                lines_holder.Add(vertexWorldPosition);
            }
            return lines_holder;
        }

        public void write_inputFileHeaders(bool append)
        {

            using (var w = new StreamWriter(F_To_write, append))
            {
                w.Write(string.Format("{0},", "Output"));
                var line = string.Format("{0},{1},{2},", "w_p_x", "w_p_y", "w_p_z");
                w.Write(line);
                line = string.Format("{0},{1},{2},", "w_r_x", "w_r_y", "w_r_z");
                w.Write(line);
                for (int i = 0; i < max_index; i++)
                {
                    line = string.Format("{0},{1},{2},", $"point{i}_x", $"point{i}_y", $"point{i}_z");
                    w.Write(line);
                }
                w.WriteLine();
            }
        }

        public void write_inputFile(List<Vector3> mesh_points, Vector3 wrist_position, Vector3 wrist_rotation, int write_index)
        {
            using (var w = new StreamWriter(F_To_write, true))
            {
                Debug.Log("writing a line");
                w.Write(string.Format("{0},", write_index));
                var line = string.Format("{0},{1},{2},", wrist_position.x, wrist_position.y, wrist_position.z);
                w.Write(line);
                line = string.Format("{0},{1},{2},", wrist_rotation.x, wrist_rotation.y, wrist_rotation.z);
                w.Write(line);
                for (int i = 0; i < max_index; i++)
                {
                    /**
                     * if we have less points than 1000 then we put 0 values
                     * 
                     */
                    line = i < mesh_points.Count ?
                        string.Format("{0},{1},{2},", mesh_points[i].x, mesh_points[i].y, mesh_points[i].z) :
                        string.Format("{0},{1},{2},", "null", "null", "null");
                    if (i >= mesh_points.Count)
                        break;
                    w.Write(line);
                }
                w.WriteLine();
            }
        }

        public (List<int>,List<List<Vector3>>,List<List<Vector3>>) read_inputFile(string F_TO_READ)
        {
            List<List<Vector3>> mesh_vects= new List<List<Vector3>>();
            List<List<Vector3>> wrist_vects= new List<List<Vector3>>();
            List<int> record_nums = new List<int>();
            string[] lines = File.ReadAllLines(F_TO_READ);
            Debug.Log(lines[0]);
            for(int j=1; j< lines.Length; j++)
            {
                string line = lines[j];
                //Debug.Log(line);
                string[] cols = line.Split(",");
                List<Vector3> v = new List<Vector3>();
                List<Vector3> vw = new List<Vector3>();
                // add the record numbers for each line
                record_nums.Add(int.Parse(cols[0]));
                // add the wrist vectors
                for(int i=1; i<7-2; i+=3)
                {
                    if (cols[i] == "null" || cols[i] == " ")
                    {
                        break;
                    }
                    vw.Add(new Vector3(float.Parse(cols[i]), float.Parse(cols[i + 1]), float.Parse(cols[i + 2])));
                }
                // add the mesh vectors
                for(int i=7; i<cols.Length-2; i+=3)
                {
                    
                    if (cols[i] == "null" || cols[i] == " ")
                    {
                        break;
                    }
                   
                    v.Add(new Vector3(float.Parse(cols[i]), float.Parse(cols[i+1]), float.Parse(cols[i+2])));
                }
                mesh_vects.Add(v);
                wrist_vects.Add(vw);
            }
            return (record_nums, wrist_vects ,mesh_vects);
        }

    }

}
