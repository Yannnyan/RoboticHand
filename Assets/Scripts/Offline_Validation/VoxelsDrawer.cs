using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace OfflineValidation
{
    public class VoxelsDrawer : MonoBehaviour
    {

        public GameObject Cube;
        public float vox_size = 0.014f;
        private GameObject[] Voxels;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private Vector3[] readVoxels(string path_to_voxels)
        {

            string file_content = File.ReadAllText(path_to_voxels);

            string[] lines = file_content.Split("\n");

            Vector3[] voxels = new Vector3[lines.Length];

            for(int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line == "")
                    continue;

                string[] indices_str = line.Split(" ");

                Debug.Log(line);

                voxels[i] = new Vector3((int)float.Parse(indices_str[0]), (int)float.Parse(indices_str[1]), (int)float.Parse(indices_str[2]));
            }

            return voxels;
        }
        public void drawVoxels(string path_to_voxels)
        {
            // clear last draw:

            if(Voxels != null)
            {
                for (int j = 0; j < Voxels.Length; j++)
                {
                    DestroyImmediate(Voxels[j]);
                }
            }

            Vector3[] voxels = readVoxels(path_to_voxels);

            Vector3 voxel_size = Vector3.one / (vox_size * voxels.Length);

            Vector3 cube_center = Vector3.zero;

            GameObject[] voxels_cubes = new GameObject[voxels.Length];

            int i = 0;

            foreach (Vector3 voxel in voxels)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                cube.transform.position = new Vector3(voxel.x * voxel_size.x, voxel.y * voxel_size.y, voxel.z * voxel_size.z);

                cube.transform.localScale =  new Vector3(1*voxel_size.x, 1*voxel_size.y, 1*voxel_size.z);

                cube.transform.SetParent(Cube.transform, false);

                voxels_cubes[i++] = cube;

                cube_center += cube.transform.position;
            }

            cube_center /= voxels.Length;

            foreach(GameObject cube in voxels_cubes)
            {
                cube.transform.position -= cube_center;
                cube.transform.position += Cube.transform.position;
            }

            Voxels = voxels_cubes;
            
        }


    }

}
