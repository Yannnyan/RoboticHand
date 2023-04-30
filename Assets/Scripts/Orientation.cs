using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loader;
using RoboModel;
using System.IO;

using TMPro;
using Unity.Barracuda;
using System.Text;
namespace Orientation
{
    public class Orientation : MonoBehaviour
    {
        [SerializeField]
        NNModel onnxModel;

        [SerializeField]
        Texture2D texture;

        Dictionary<string, List<Vector3>> joint_to_orientation;

        HandLoader loader;
        // Start is called before the first frame update
        void Start()
        {
            //loader = new HandLoader();
            //RoboticHandModel model = new RoboticHandModel(onnxModel, texture);

            //loader.load_array(model.Predict("bla bla"));

            //this.joint_to_orientation = loader.get_hand_joints_rotations_vectors();

            //this.transform_hand_model();

            //this.test_image();

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
                new_name.Append("/" + name + index);
                index++;
            }
            return new_name.ToString();

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
                    Debug.Log(root_path + walk_backwards(name, j));
                    GameObject game = GameObject.Find(root_path + walk_backwards(name, j));
                    game.transform.rotation = Quaternion.Euler(this.joint_to_orientation[names[i]][j]);
                }
            }

            //wrist.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
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
                    Debug.Log(root_path + walk_backwards(name, j));
                    GameObject game = GameObject.Find(root_path + walk_backwards(name, j));
                    Vector3 angles = game.transform.rotation.eulerAngles;
                    record[ri++] = angles.x;
                    record[ri++] = angles.y;
                    record[ri++] = angles.z;
                }
            }
            return record;
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
