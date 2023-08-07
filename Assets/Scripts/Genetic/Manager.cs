using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Genetic
{
    public class Manager : MonoBehaviour
    {
        [SerializeField] WSClient client;
        [SerializeField] GetImages getImages;
        public Orientation.Orientation orientation;
        [SerializeField] HandRays handRays;
        private float[][] best_distances;
        private byte[][][] best_imgs;
        private int generation_num = 0;

        private List<string> messages = new List<string>();
        string pathname = @"C:\Users\Yan\Desktop\Robotic hand\Scripts\Genetic Server\outputs_images";

        // Start is called before the first frame update
        void Start()
        {
            WSClient.GotMessage msg = messages.Add;
            client.createSocket(msg);
            string[][] paths = getImages.RecordImages();
            client.SendImagesPaths(paths);

        }

        void Update()
        {
            if (messages.Count > 0)
            {
                string message = messages[0];
                messages.RemoveAt(0);
                reactMessage(message);
            }
        }
        public static float[] normalize_output_360(float[] outputData)
        {
            for (int i = 0; i < outputData.Length; i++)
            {
                outputData[i] *= 360;
            }
            return outputData;
        }
        public static float[] normalize_output(float[] outputData)
        {
            float[] minimum_joint_rotations = new float[]
            {
            352.9924f,180.479f,4.538668f,39.00467f,124.7582f,347.6329f,63.9227f,95.51402f,331.97f, 63.42553f,117.3327f,337.7044f,60.99609f,91.60537f,285.4651f,357.09f, 183.9797f,10.572f, 354.9348f,186.2075f,352.0483f,353.0488f,187.7275f,338.1941f,352.7332f,187.7359f,15.482f, 351.5585f,188.4694f,2.3875f, 348.2335f,190.6716f,332.0064f,346.7533f,194.8037f,22.241f, 342.9317f,195.424f,9.0899f, 341.3087f,192.3232f,340.033f,328.6554f,197.2972f,356.6837f,339.0399f,195.8436f,20.09f,  335.2876f,201.6648f,5.0139f, 332.9378f,195.9685f,337.5134f
            };
            float[] maximum_joint_rotations = new float[]{
            352.9924f,180.479f,4.538668f,39.00467f,124.7582f,347.6329f,63.9227f,95.51402f,258.97f,63.42553f,117.3327f,193.7044f,60.99609f,91.60537f,84.4651f,357.09f,183.9797f,297.572f,354.9348f,186.2075f,208.0483f,353.0488f,187.7275f,137.1941f,352.7332f,187.7359f,302.482f,351.5585f,188.4694f,218.3875f,348.2335f,190.6716f,131.0064f,346.7533f,194.8037f,309.241f,342.9317f,195.424f,225.0899f,341.3087f,192.3232f,139.033f,328.6554f,197.2972f,356.6837f,339.0399f,195.8436f,307.09f,335.2876f,201.6648f,221.0139f,332.9378f,195.9685f,136.5134f
        };
            float[] diff_joint_rotations = new float[outputData.Length];

            for (int i = 0; i < outputData.Length; i++)
            {
                // fix this? wtf??
                if (i >= 54)
                    break;
                diff_joint_rotations[i] = Mathf.Abs(maximum_joint_rotations[i] - minimum_joint_rotations[i]);
            }
            // preprocessing
            for (int i = 0; i < outputData.Length; i++)
            {
                if (i >= 54)
                    break;
                outputData[i] *= diff_joint_rotations[i]; // Multiply each element by 360
                outputData[i] += maximum_joint_rotations[i];
            }
            return outputData;
        }
        private void isBest(float[] distances, int object_index)
        {
            if (distances.Sum() < best_distances[object_index].Sum())
            {
                best_distances[object_index] = distances;
                best_imgs[object_index] = this.getImages.GetImagesFromCameras();
            }
        }
        public void reactMessage(string data)
        {

            Debug.Log("Reacting to message");
            try
            {
                Dictionary<string, Dictionary<string, float[]>> values =
                JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, float[]>>>(data);
                Debug.Log("Parsed json");
                int n;
                int num_items = values.Values.First().Keys.Count;
                Debug.Log("Num objects: " + num_items);
                best_distances = new float[num_items][];
                for (int i = 0; i < best_distances.Length; i++)
                {
                    best_distances[i] = new float[5];
                    for (int j = 0; j < 5; j++)
                        best_distances[i][j] = Mathf.Infinity;
                }
                best_imgs = new byte[num_items][][];
                Dictionary<string, float[]> avg_distances_dicts = new Dictionary<string, float[]>();
                //getImages.RecordImages();
                // iterate over the genes
                foreach (var key in values.Keys)
                {
                    n = 0;
                    //Debug.Log("Key: " + key);
                    float[] avg_distances = new float[5];

                    // iterate over the outputs per object
                    foreach (var key2 in values[key].Keys)
                    {
                        Debug.Log("key: " + key + " key2 " + key2);
                        float[] hand = values[key][key2];

                        int dna_index = int.Parse(key.Split("_")[1]);

                        int object_index = int.Parse(key2.Split("_")[1]);

                        getImages.SpawnNextObject(object_index);

                        hand = normalize_output_360(hand);

                        Debug.Log("Load and Transform");

                        orientation.LoadAndTransformSecondary(hand);

                        Debug.Log("Ray distances");

                        float[] distances = handRays.getRayDistances(orientation.hand_dict, getImages.lastObject);

                        // getImages.saveImageOnDisk(pathname + @"\dna_" + dna_index.ToString() + "_" + key2, dna_index);

                        for (int i = 0; i < distances.Length; i++)
                            avg_distances[i] += distances[i];
                        n++;
                        //isBest(avg_distances, object_index);

                    }
                    // get average result and send back
                    for (int i = 0; i < avg_distances.Length; i++)
                        avg_distances[i] /= n;
                    avg_distances_dicts.Add(key, avg_distances);
                }


                Debug.Log("Sending msg back");
                client.SendRaysResponse(avg_distances_dicts);
                //getImages.saveGenerationInfo(best_imgs, pathname, generation_num);
                generation_num += 1;
            }
            catch (Exception e)
            {
                float[] err_flt = { 100, 100, 100, 100, 100 };
                Dictionary<string, float[]> err = new Dictionary<string, float[]>();
                err.Add("gene_0", err_flt);
                client.SendRaysResponse(err);
            }


        }
    }

}
