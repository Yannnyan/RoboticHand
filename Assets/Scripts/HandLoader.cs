using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Loader
{
    
    public class HandLoader
    {
        //enum joints
        //{
        //    thumb0_X, thumb0_Y, thumb0_Z, thumb1_X, thumb1_Y, thumb1_Z, thumb2_X, thumb2_Y, thumb2_Z, thumb3_X, thumb3_Y, thumb3_Z, index1_X, index1_Y, index1_Z, index2_X, index2_Y, index2_Z, index3_X, index3_Y, index3_Z, middle1_X, middle1_Y, middle1_Z, middle2_X, middle2_Y, middle2_Z, middle3_X, middle3_Y, middle3_Z, ring1_X, ring1_Y, ring1_Z, ring2_X, ring2_Y, ring2_Z, ring3_X, ring3_Y, ring3_Z, pinky0_X, pinky0_Y, pinky0_Z, pinky1_X, pinky1_Y, pinky1_Z, pinky2_X, pinky2_Y, pinky2_Z, pinky3_X, pinky3_Y, pinky3_Z, roothand_X, roothand_Y, roothand_Z,,
        //}
        List<string> joints = (
            "wrist1_X, wrist1_Y, wrist1_Z," +
            " thumb0_X, thumb0_Y, thumb0_Z," +
            " thumb1_X, thumb1_Y, thumb1_Z," +
            " thumb2_X, thumb2_Y, thumb2_Z," +
            " thumb3_X, thumb3_Y, thumb3_Z, " +
            "index1_X, index1_Y, index1_Z," +
            " index2_X, index2_Y, index2_Z," +
            " index3_X, index3_Y, index3_Z," +
            " middle1_X, middle1_Y, middle1_Z," +
            " middle2_X, middle2_Y, middle2_Z," +
            " middle3_X, middle3_Y, middle3_Z," +
            " ring1_X, ring1_Y, ring1_Z," +
            " ring2_X, ring2_Y, ring2_Z, " +
            "ring3_X, ring3_Y, ring3_Z, " +
            "pinky0_X, pinky0_Y, pinky0_Z, " +
            "pinky1_X, pinky1_Y, pinky1_Z, " +
            "pinky2_X, pinky2_Y, pinky2_Z, " +
            "pinky3_X, pinky3_Y, pinky3_Z").Split(", ").ToList<string>();
        float[] outputs;
        public float[] rotations;
        float[] positions;
        public void load_array(float[] outputs_array)
        {
            Debug.Log(outputs_array.Length);
            outputs = outputs_array;
            rotations = new float[outputs_array.Length/2];
            positions = new float[outputs_array.Length/2];
            for(int i=0; i< outputs_array.Length; i++)
            {
                if(i < outputs.Length / 2)
                {
                    positions[i] = outputs[i];
                }
                else
                {
                    rotations[i - outputs.Length/2] = outputs[i];
                }
            }
        }
        
        /**get retations
         * 
         */
        public Dictionary<string,List<Vector3>> get_hand_joints_rotations_vectors()
        {
            Dictionary<string, List<Vector3>> joint_vectors = new Dictionary<string, List<Vector3>>();

            for(int i=0; i< joints.Count; i+=3)
            {
                // bone name such as index or middle
                string substr = joints[i].Substring(0, joints[i].Length-3);
                List<Vector3> vects;
                Vector3 vec;
                if(joint_vectors.TryGetValue(substr, out vects))
                {
                   
                    vec = new Vector3(rotations[i], rotations[i + 1], rotations[i + 2]);
                    vects.Add(vec);
                }
                else
                {
                    vec = new Vector3(rotations[i], rotations[i + 1], rotations[i + 2]);
                    vects = new List<Vector3>();
                    vects.Add(vec);
                    joint_vectors[substr] = vects;
                }
            }
            return joint_vectors;
        }




    }
}
