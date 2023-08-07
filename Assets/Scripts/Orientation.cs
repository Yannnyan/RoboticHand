using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loader;
using RoboModel;
using System.IO;

using TMPro;
using Unity.Barracuda;
using System.Text;
using System;

namespace Orientation
{
    public class Orientation : MonoBehaviour
    {

        public Dictionary<string, List<GameObject>> hand_dict;
        public string hand_name = "-";


        Dictionary<string, List<Vector3>> joint_to_orientation;
        HandLoader loader;

        // Start is called before the first frame update
        void Start()
        {
            if(hand_name != "-")
                hand_dict = GetHand(hand_name);
            else
                hand_dict = GetHand();
        }

        private int loop_amount(string name)
        {
            switch (name)
            {
                case "wrist":
                    return 1;
                case "thumb":
                    return 4;
                case "pinky":
                    return 4;
                default:
                    return 3;
            }
        }

        private string walk_backwards(string name, int j)
        {
            if (name.EndsWith("wrist"))
            {
                return "/" + name;
            }

            StringBuilder new_name = new StringBuilder("/b_r_wrist");
            int l_amount = loop_amount(name[4..]);
            int min_val = l_amount == 4 ? 0 : 1;
            int max_val = l_amount == 3 ? j + 1 : j;
            int index = min_val;

            while (index <= max_val)
            {
                new_name.Append("/" + name + index.ToString());
                index++;
            }
            return new_name.ToString();

        }
        public void LoadAndTransform(float[] rotations)
        {
            Loader.HandLoader handLoader = new HandLoader();
            handLoader.rotations = rotations;
            this.joint_to_orientation = handLoader.get_hand_joints_rotations_vectors();
            transform_hand_model();
        }
        private void transform_hand_model()
        {
            string root_path = "/Hands/RightHand/RightHandVisual/OculusHand_R";
            string[] names = { "wrist", "thumb", "index", "middle", "ring", "pinky" };
            string prefix = "b_r_";
            //GameObject wrist = GameObject.Find("/Hands/RightHand/RightHandVisual/OculusHand_R/");

            for (int i = 0; i < names.Length; i++)
            {
                int l_amount = loop_amount(names[i]);
                for (int j = 0; j < l_amount; j++)
                {
                    string name = prefix + names[i];
                    //Debug.Log(root_path + walk_backwards(name, j));
                    string path = root_path + walk_backwards(name, j);
                    GameObject game = GameObject.Find(path);
                    game.transform.rotation = Quaternion.Euler(this.joint_to_orientation[names[i]][j]);
                }
            }

            //wrist.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        }
        public void LoadAndTransformSecondary(float[] rotations)
        {
            Loader.HandLoader handLoader = new HandLoader();
            handLoader.rotations = rotations;
            this.joint_to_orientation = handLoader.get_hand_joints_rotations_vectors();

            string[] names = { "wrist", "thumb", "index", "middle", "ring", "pinky" };
            foreach (string name in names)
            {
                for (int i=0; i< hand_dict[name].Count; i++)
                {
                    if (i >= this.joint_to_orientation[name].Count) // discard the tip
                        continue;
                    try
                    {
                        hand_dict[name][i].transform.rotation = Quaternion.Euler(this.joint_to_orientation[name][i]);

                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                }
            }
        }

        public void test_image()
        {
            var texture = new Texture2D(224, 224);
            byte[] b = File.ReadAllBytes(@"C:\Users\Yan\Desktop\Tiktik\Testonnx\Testonnx\Input1.png");
            texture.LoadImage(b);
            GameObject floor = GameObject.Find("Cube");
            
            floor.GetComponent<Renderer>().material.SetTexture(1, texture);
        }

        public float[] record_hand()
        {
            string root_path = "/Hands/RightHand/RightHandVisual/OculusHand_R";
            string[] names = { "wrist", "thumb", "index", "middle", "ring", "pinky" };
            string prefix = "b_r_";
            //GameObject wrist = GameObject.Find("/Hands/RightHand/RightHandVisual/OculusHand_R/");
            float[] record = new float[54];
            int ri = 0;
            for (int i = 0; i < names.Length; i++)
            {
                int l_amount = loop_amount(names[i]);
                for (int j = 0; j < l_amount; j++)
                {
                    string name = prefix + names[i];
                    // Debug.Log(root_path + walk_backwards(name, j));
                    GameObject game = GameObject.Find(root_path + walk_backwards(name, j));
                    Vector3 angles = game.transform.rotation.eulerAngles;
                    record[ri++] = angles.x;
                    record[ri++] = angles.y;
                    record[ri++] = angles.z;
                }
            }
            return record;
            
        }

        public Dictionary<string, List<GameObject>> GetHandByTag()
        {
            string[] names = { "wrist", "thumb", "index", "middle", "ring", "pinky" };
            Dictionary<string, List<GameObject>> hand_model = new Dictionary<string, List<GameObject>>();
            foreach (string name in names)
            {
                int l_amount = loop_amount(name);
                List<GameObject> finger_objects = new List<GameObject>();
                if(name == "wrist")
                {
                    finger_objects.Add(GameObject.FindWithTag(name));
                }
                else
                {
                    for (int i = 0; i < l_amount; i++)
                    {
                        string tag;
                        if (name != "thumb" && name != "pinky")
                            tag = name + (i + 1).ToString();
                        else
                            tag = name + i.ToString();
                        finger_objects.Add(GameObject.FindGameObjectWithTag(tag));
                    }
                    
                }
                hand_model.Add(name, finger_objects);

            }
            return hand_model;
        }


        
        public Dictionary<string, List<GameObject>> GetHand(string hand_name="Hands")
        {
            string root_path = "/" + hand_name + "/RightHand/RightHandVisual/OculusHand_R";
            string[] names = { "wrist", "thumb", "index", "middle", "ring", "pinky" };
            string prefix = "b_r_";
            Dictionary<string, List<GameObject>> hand = new Dictionary<string, List<GameObject>>();
            
            for (int i = 0; i < names.Length; i++)
            {
                hand[names[i]] = new List<GameObject>();
                int l_amount = loop_amount(names[i]);
                for (int j = 0; j < l_amount; j++)
                {
                    string name = prefix + names[i];
                    // Debug.Log(root_path + walk_backwards(name, j));
                    string path = root_path + walk_backwards(name, j);
                    GameObject game;
                    try
                    {
                        
                        game = GameObject.Find(path);

                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                        game = null;
                    }

                    hand[names[i]].Add(game);
                    // get also the tip
                    if (j == l_amount - 1)
                    {
                        GameObject game_tip = GameObject.Find(root_path + walk_backwards(name, j) + "/" + "r_" + names[i] + "_finger_tip_marker");
                        hand[names[i]].Add(game_tip);
                    }
                }
               
            }
            return hand;
        }
        // Update is called once per frame
        void Update()
        {

        }
    }

}
