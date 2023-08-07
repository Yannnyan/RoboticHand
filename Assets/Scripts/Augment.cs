using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoboticHand;
using HandTracking.Parser;
using System.IO;


namespace RoboticHand
{
    public class Augment : MonoBehaviour
    {
        [SerializeField]
        private MeshPoints meshpoints;
        [SerializeField]
        private CreateInputFile createInputFile;

        private string csvpath = @"C:\Users\Yan\Desktop\augment_input.csv"; // write
        int record_num = 0;

        void Awake()
        {
            createInputFile.setPath(csvpath);

            if (!File.Exists(createInputFile.F_To_write))
                record_num = 0;
            else
                record_num = CSVParser.get_record_num(csvpath);

        }
        // Start is called before the first frame update
        void Start()
        {
            createInputFile.setPath(csvpath);
            if (!File.Exists(createInputFile.F_To_write))
                createInputFile.write_inputFileHeaders(false);
            string readPath = @"C:\Users\Yan\Desktop\input_mesh_wrist.csv"; // read
            (List<int> record_nums, List<List<Vector3>> wrist_vects, List<List<Vector3>> meshes) = createInputFile.read_inputFile(readPath);
            Vector3[] vecarr = { new Vector3(20, 0, 0), new Vector3(0, 20, 0), new Vector3(0, 0, 20) };
            for (int i = 0; i < meshes.Count; i++)
            {
                for (int j = 0; j < vecarr.Length; j++)
                {
                    augment_hand(wrist_vects[i], meshes[i], vecarr[j], record_nums[i]);
                }
            }


        }
        public static Vector3 modVec(Vector3 vec, Vector3 add, int maxAngle)
        {
            return new Vector3(
                (vec.x + add.x) % maxAngle,
                (vec.y + add.y) % maxAngle,
                (vec.z + add.z) % maxAngle);
        }
        /**
         * <param name="mesh"> includes all the mesh vectors of the object, </param>
         * <param name="rotateAngles"> specifies how to rotate the object in x, y, z, </param>
         * <param name="wristInitVectors"> defines how the wrist initially places </param>
         * <param name="recording_num"> defines what is the corresponding origin of the augmentation</param>
         */
        void augment_hand(List<Vector3> wristInitVectors, List<Vector3> mesh, Vector3 rotateAngles, int recording_num)
        {
            Vector3 center_vec = new Vector3(0, 0, 0);
            foreach (Vector3 vec in mesh)
            {
                center_vec += vec;
            }
            center_vec /= mesh.Count;
            for (int i = 0; i < mesh.Count; i++)
            {
                mesh[i] -= center_vec;
            }
            // rotate

            // draw the mesh
            //meshpoints.spawnLineGenerator(test_mesh, Color.red);

            GameObject Hands = GameObject.Find("Hands");
            Hands.transform.position -= center_vec;


            GameObject hand = GameObject.Find("Hands/RightHand/RightHandVisual/OculusHand_R/b_r_wrist");
            hand.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0)); // make the rotation 0
            hand.transform.Rotate(wristInitVectors[1], Space.Self); // rotates the wrist to the given wristInitVectors
            for (Vector3 rvec = modVec(rotateAngles, rotateAngles, 360); rvec != rotateAngles; rvec = modVec(rvec, rotateAngles, 360))
            {
                hand.transform.Rotate(rotateAngles.x, rotateAngles.y, rotateAngles.z, Space.Self);
                mesh = MeshPoints.RotateMesh(mesh, rotateAngles.x, rotateAngles.y, rotateAngles.z);
                Debug.Log(rvec.ToString());
                createInputFile.write_inputFile(mesh, hand.transform.position, hand.transform.rotation.eulerAngles, recording_num);
            }

        }


        // Update is called once per frame
        void Update()
        {

        }
    }

}
