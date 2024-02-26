using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Orientation;
using Genetic;
using OfflineValidation;
using Simulator;


namespace OfflineValidation
{
    public class Inputter : MonoBehaviour
    {
        public string actual_path;
        public string prediction_path;
        public string stats_path;
        public string convex_path;
        public string voxels_dir_path;
        public Orientation.Orientation orientation_preds;
        public Orientation.Orientation orientation_actuals;
        public ConvexDrawer convexhullDrawer;
        public VoxelsDrawer voxelsDrawer;
        public TextChanger textChanger;
        public bool isVoxels = false;
        public GameObject PointCloud;

        // hand pos
        private Vector3 handPredPos;
        private Vector3 handActualPos;
        // rotations and convex and voxels
        private Dictionary<int, float[]> row_to_rotations_actuals;
        private Dictionary<int, float[]> row_to_rotations_preds;
        private Dictionary<int, Vector3[]> row_to_convex;
        private string[] voxels_files_inorder;
        // scores
        private float[][] mse_r2_scores;

        // index of current row
        private int ind = 0;



        // Start is called before the first frame update
        void Start()
        {
            // define names
            string preds_hand_name = "Hands_Preds";
            string actuals_hand_name = "Hands_Actuals";

            // get hand positions
            row_to_convex = new Dictionary<int, Vector3[]>();
            handPredPos = orientation_preds.GetHand(preds_hand_name)["wrist"][0].transform.position;
            handActualPos = orientation_actuals.GetHand(actuals_hand_name)["wrist"][0].transform.position;

            // get predictions and actuals
            row_to_rotations_actuals = readAllPredictions(actual_path);
            row_to_rotations_preds = readAllPredictions(prediction_path);


            if (!isVoxels) {
                // read Convext
                readAllConvexWrist(convex_path);
            }

            else
            {
                readVoxelsFilesInOrder(voxels_dir_path);
            }

            // read mse/r2 scores
            mse_r2_scores = readAllScores(stats_path);
        }


        public static float[] get54RepFrom31(float[] dof31)
        {
            float[] result = new float[54];

            string joints_order = "[(0, 1, 2),(3, 4, 5), (3, 6, 7), (3, 6, 8), (3, 6, 9), (0, 10, 11), (0, 10, 12), (0, 10, 13), (0, 14, 15), (0, 14, 16),(0, 14, 17), (0, 18, 19), (0, 18, 20), (0, 18, 21), (22, 23, 24), (22, 25, 26), (22, 27, 28), (22, 29, 30)]";

            joints_order = joints_order.Replace("[", "").Replace("]","").Replace("(", "").Replace(")", "").Replace(" ", "");

            string[] numbers_arr = joints_order.Split(",");

            int j = 0;
            for(int i=0; i < numbers_arr.Length; i++)
            {
                int n = int.Parse(numbers_arr[i]);

                result[j++] = dof31[n];
            }

            return result;
        }
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // transform the hand model with the rotations at ind
                float[] cur_row_actuals = get54RepFrom31(row_to_rotations_actuals[ind]);
                float[] cur_row_preds = get54RepFrom31(row_to_rotations_preds[ind]);

                cur_row_preds = Genetic.Manager.normalize_output_360(cur_row_preds);
                cur_row_actuals = Genetic.Manager.normalize_output_360(cur_row_actuals);

                orientation_preds.LoadAndTransform(cur_row_preds);
                orientation_actuals.LoadAndTransform(cur_row_actuals);

                if(!isVoxels)
                {
                    //convexhullDrawer.renderLineConvex(row_to_convex[ind]);
                    Vector3 subject = new Vector3(-7.1500001f, 0f, -0.209999993f);

                    //convexhullDrawer.renderCircleConvex(row_to_convex[ind], handPredPos);
                    convexhullDrawer.renderCircleConvex(row_to_convex[ind], subject);
                }
                else
                {
                    voxelsDrawer.drawVoxels(voxels_files_inorder[ind]);
                }
                
                textChanger.setMSE("" + mse_r2_scores[0][ind]);

                textChanger.setR2("" + mse_r2_scores[1][ind]);
                ind++;
            }

        }


        private class stringComp : IComparer
        {
            public int Compare(object x, object y)
            {
                string x_st = (string)x;

                string y_st = (string)y;

                string[] x_splat = x_st.Split("_"), 
                    y_splat = y_st.Split("_");

                float x1 = float.Parse(x_splat[x_splat.Length - 1]);

                float y1 = float.Parse(y_splat[y_splat.Length - 1]);

                if (x1 < y1) return -1;

                else if (x1 > y1) return 1;

                else return 0;
            }
        }

        private void readVoxelsFilesInOrder(string voxelsDirPath)
        {
            string[] files = Directory.GetFiles(voxelsDirPath);
            
            Array.Sort(files, new stringComp());

            //foreach (string file in files) Debug.Log(file);

            voxels_files_inorder = files;
        }

        private void readAllConvexWrist(string path)
        {
            //string path = @"C:\Users\Yan\Downloads\mesh_wrist.csv";
            string txt = File.ReadAllText(
                path
                );


            // parsing the data
            string[] rows = txt.Split('\n');
            int convex_start_index = 7;
            for (int i = 1; i < rows.Length; i++)
            {
                string[] cells_of_row = rows[i].Split(",");
                try
                {
                    if (cells_of_row[0].Equals("") || cells_of_row[0].Equals(" "))
                        break;
                    int row_index = int.Parse(cells_of_row[0]);
                    //Debug.Log("row index: " + row_index);
                    // parsing array
                    float[] parsed_arr = new float[cells_of_row.Length - convex_start_index];
                    for (int j = convex_start_index; j < cells_of_row.Length; j++)
                    {
                        parsed_arr[j - convex_start_index] = float.Parse(cells_of_row[j]);
                    }
                    Vector3[] vecs = new Vector3[(int)((parsed_arr.Length) / 3)];
                    int point_ind= 0;
                    // assigning values to the dict
                    for (int j = 0; j < parsed_arr.Length; j += 3)
                    {
                        vecs[point_ind++] = new Vector3(parsed_arr[j], parsed_arr[j + 1], parsed_arr[j + 2]);
                        
                    }
                    row_to_convex[row_index] = vecs;
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    break;
                }

            }
        }

        public static Dictionary<int, float[]> readAllPredictions(string path)
        {
            Dictionary<int, float[]> row_to_input = new Dictionary<int, float[]>();
            // string path = @"C:\Users\Yan\Downloads\predictions.csv";
            string txt = File.ReadAllText(
                path
                );


            // parsing the data
            string[] rows = txt.Split('\n');

            for (int i = 1; i < rows.Length; i++)
            {
                string[] cells_of_row = rows[i].Split(",");
                try
                {
                    if (cells_of_row[0].Equals("") || cells_of_row[0].Equals(" "))
                    {
                        break;
                    }
                    int row_index = int.Parse(cells_of_row[0]);
                    //Debug.Log("row index: " + row_index);
                    // parsing array
                    float[] parsed_arr = new float[cells_of_row.Length - 1];
                    for (int j = 1; j < cells_of_row.Length; j++)
                    {
                        // `Debug.Log(cells_of_row[j]);
                        if (cells_of_row[j].Equals("") || cells_of_row[j].Equals(",") || cells_of_row[j].Equals(" "))
                            continue;
                        parsed_arr[j - 1] = float.Parse(cells_of_row[j]);
                    }

                    // assigning values to the dict
                    row_to_input.Add(row_index, parsed_arr);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    continue;
                }

            }
            return row_to_input;
        }


        /**
         * returns 2d array, first row is MSE scores, second row is R2 scores
         */
        private float[][] readAllScores(string path)
        {
            string txt = File.ReadAllText(
                path
                );
            string[] rows = txt.Split("\n");
            float[][] scores = new float[2][];
            scores[0] = new float[rows.Length]; // mse
            scores[1] = new float[rows.Length]; // r2

            int startind = 1;
            for (int i= startind; i< rows.Length; i++)
            {
                if (rows[i] == "")
                    break;
                string[] cells = rows[i].Split(",");
                // first column is mse
                float mse = float.Parse(cells[1]);
                scores[0][i - startind] = mse;

                // second column is R2
                float r2 = float.Parse(cells[2]);
                scores[1][i - startind] = r2;
            }

            return scores;
        }

    }

}
