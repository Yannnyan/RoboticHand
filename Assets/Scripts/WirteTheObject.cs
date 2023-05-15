using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WirteTheObject : MonoBehaviour
{
    [SerializeField] int max_index = 100000;

    public List<Vector3> GetMeshPoints()
    {
        Debug.Log("getMeshPoints: " + this.name);
        Transform transoform = transform;
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        List<Vector3> lines_holder = new List<Vector3>();

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertexWorldPosition = transoform.TransformPoint(vertices[i]);
            lines_holder.Add(vertexWorldPosition);
        }
        return lines_holder;
    }

    /*public void write_inputFileHeaders(bool append)
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
    }*/

    public string GetInputObject()
    {
        List<Vector3> mesh_points = GetMeshPoints();

        string data = "[";
        int j = 0; for (; j < mesh_points.Count; j++)
        {
            if(j == 0)
                data += mesh_points[j].x.ToString();
            else
                data += "," +mesh_points[j].x.ToString();
            data += "," + mesh_points[j].y.ToString();
            data += "," +mesh_points[j].z.ToString();
            

        }
        data += "]";
        Debug.Log(j);
        return data;
    }

    /*public (List<int>,List<List<Vector3>>,List<List<Vector3>>) read_inputFile(string F_TO_READ)
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

}*/
}
