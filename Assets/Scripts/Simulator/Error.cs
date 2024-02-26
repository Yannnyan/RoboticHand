using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OfflineValidation;
using TMPro;
using Genetic;


namespace Simulator
{

    public class Error : MonoBehaviour
    {

        float mseerror = Mathf.Infinity;
        public Orientation.Orientation targetOrientation;
        public Orientation.Orientation currentOrientation;
        public string csvPath = "";

        public TextMeshPro curTextMSE;

        // Start is called before the first frame update
        void Start()
        {
            // load some orientation
            Dictionary<int, float[]> rotations = Inputter.readAllPredictions(csvPath);
            currentOrientation.LoadAndTransform(Manager.normalize_output_360(rotations[5]));
            targetOrientation.LoadAndTransform(Manager.normalize_output_360(get_relevant_joints(rotations[5])));

        }

        public static float[] get_relevant_joints(float[] rotations)
        {
            float[] new_rotations = new float[rotations.Length];
            int[] indices = { 0, 1, 2, 3, 4, 5, 7, 8, 11, 14, 16, 17, 20, 23, 25, 26, 29, 32, 34, 35, 38, 41, 42, 43, 44, 46, 47, 49, 50, 52, 53 };

            for (int i = 0; i < rotations.Length; i++)
            {
                if (indices.Contains(i))
                {
                    new_rotations[i] = rotations[i];
                }
                else if (i == 15 || i == 24 || i == 33)
                {
                    new_rotations[i] = new_rotations[i % 3];
                }
                else
                {
                    new_rotations[i] = new_rotations[i - 3];
                }
            }

            return new_rotations;
        }
        public static float[] get_some_joints(float[] rotations)
        {
            float[] new_rotations = new float[rotations.Length];
            for (int i = 0; i < new_rotations.Length; i++)
            {
                new_rotations[i] = rotations[i];
            }
            for (int j = 0; j < 3; j++)
            {
                new_rotations[j] = 0;
            }
            return new_rotations;
        }

        public float calcMseError(Orientation.Orientation orientation1, Orientation.Orientation orientation2)
        {
            float[] rotations1 = (orientation1.record_hand());
            float[] rotations2 = (orientation2.record_hand());


            float error = 0;
            for (int i = 0; i < rotations1.Length; i++)
            {
                float diff = (rotations2[i] - rotations1[i]);
                diff = diff >= 180 ? 360 - diff : diff;
                diff = diff <= -180 ? 360 + diff : diff; 
                error +=  diff * diff / (360 * 360);
            }
            return error;
        }
        // Update is called once per frame
        void Update()
        {
            // calc mse
            this.mseerror = calcMseError(this.currentOrientation, this.targetOrientation);
            this.curTextMSE.text = "MSE: " + this.mseerror.ToString();
        }
    }
}