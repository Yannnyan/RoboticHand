using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.Assertions;

namespace HandTracking.TrackHand
{
    public enum HandToTrack
    {
        Left,
        Right
    }
    public class TrackHand : MonoBehaviour
    {
        [SerializeField]
        private HandToTrack handToTrack;
        //// index 0 and root
        [SerializeField]
        private GameObject handRootBoneObj;
        [SerializeField]
        private GameObject thumbFingerBone0Obj;
        [SerializeField]
        private GameObject pinkyFingerBone0Obj;
        //// index 1
        [SerializeField]
        private GameObject thumbFingerBone1Obj;  
        [SerializeField]
        private GameObject indexFingerBone1Obj;  
        [SerializeField]
        private GameObject middleFingerBone1Obj; 
        [SerializeField]
        private GameObject ringFingerBone1Obj;   
        [SerializeField]
        private GameObject pinkyFingerBone1Obj;
        //// index 2    
        [SerializeField]
        private GameObject thumbFingerBone2Obj;  
        [SerializeField]
        private GameObject indexFingerBone2Obj;  
        [SerializeField]
        private GameObject middleFingerBone2Obj; 
        [SerializeField]
        private GameObject ringFingerBone2Obj;   
        [SerializeField]
        private GameObject pinkyFingerBone2Obj;  
        //// index 3                       
        [SerializeField]
        private GameObject thumbFingerBone3Obj;  
        [SerializeField]
        private GameObject indexFingerBone3Obj;  
        [SerializeField]
        private GameObject middleFingerBone3Obj; 
        [SerializeField]
        private GameObject ringFingerBone3Obj;   
        [SerializeField]
        private GameObject pinkyFingerBone3Obj;
        #region drawable
        protected LineRenderer line;
        #endregion

        #region Oculus types
        private OVRSkeleton ovrSkeleton;
        //private OVRBone[] handJoints;
        //// index 0 and root
        private OVRBone handRootBone;
        private OVRBone thumbFingerBone0;
        private OVRBone pinkyFingerBone0;
        // index 1
        private OVRBone thumbFingerBone1;
        private OVRBone indexFingerBone1;
        private OVRBone middleFingerBone1;
        private OVRBone ringFingerBone1;
        private OVRBone pinkyFingerBone1;
        // index 2
        private OVRBone thumbFingerBone2;
        private OVRBone indexFingerBone2;
        private OVRBone middleFingerBone2;
        private OVRBone ringFingerBone2;
        private OVRBone pinkyFingerBone2;
        // index 3
        private OVRBone thumbFingerBone3;
        private OVRBone indexFingerBone3;
        private OVRBone middleFingerBone3;
        private OVRBone ringFingerBone3;
        private OVRBone pinkyFingerBone3;
        #endregion
        #region file_writing
        private string fnameRight = "PositionBonesRight" + ".csv";
        private string fnameLeft = "PositionBonesLeft" + ".csv";
        private string FILE_NAME_L;
        private string FILE_NAME_R;
        private string F_To_write;
        #endregion
        #region static_names
        public static string[] finger_names = {"thumb0", "thumb1", "thumb2", "thumb3",
        "index1", "index2", "index3",
        "middle1", "middle2","middle3",
        "ring1", "ring2","ring3",
        "pinky0","pinky1","pinky2","pinky3"};
        public static int frame = 0;
        #endregion
        // Start is called before the first frame update
        void Awake()
        {
            FILE_NAME_L = Path.Combine(Application.persistentDataPath, fnameLeft);
            FILE_NAME_R = Path.Combine(Application.persistentDataPath, fnameRight);
            ovrSkeleton = handRootBoneObj.GetComponent<OVRSkeleton>();
            line = GetComponent<LineRenderer>();
            F_To_write = handToTrack == HandToTrack.Left ? FILE_NAME_L : FILE_NAME_R;
            using (var w = new StreamWriter(F_To_write, false)) // delete all the existing data and write the headers of the file
            {
                foreach (string finger_name in finger_names)
                {
                    var line = string.Format("{0},{1},{2},{3},{4}", finger_name, "X", "Y", "Z", " ");
                    w.Write(line);
                }
                var line1 = string.Format("{0},{1},{2},{3},{4}", "root_hand", "RotateX", "RotateY", "RotateZ", " ");
                w.Write(line1);
                w.WriteLine();
            }
            // get initial bone to track
            handRootBone = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_WristRoot)
                .SingleOrDefault();
            thumbFingerBone0 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Thumb0)
                .SingleOrDefault();
            pinkyFingerBone0 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Pinky0)
                .SingleOrDefault();
            thumbFingerBone1 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Thumb1)
                .SingleOrDefault();
            thumbFingerBone2 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Thumb2)
                .SingleOrDefault();
            thumbFingerBone3 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Thumb3)
                .SingleOrDefault();
            indexFingerBone1 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Index1)
                .SingleOrDefault();
            indexFingerBone2 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Index2)
                .SingleOrDefault();
            indexFingerBone3 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Index3)
                .SingleOrDefault();
            middleFingerBone1 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Middle1)
                .SingleOrDefault();
            middleFingerBone2 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Middle2)
                .SingleOrDefault();
            middleFingerBone3 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Middle3)
                .SingleOrDefault();
            ringFingerBone1 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Ring1)
                .SingleOrDefault();
            ringFingerBone2 = ovrSkeleton.Bones
               .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Ring2)
               .SingleOrDefault();
            ringFingerBone3 = ovrSkeleton.Bones
               .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Ring3)
               .SingleOrDefault();
            pinkyFingerBone1 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Pinky1)
                .SingleOrDefault();
            pinkyFingerBone2 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Pinky2)
                .SingleOrDefault();
            pinkyFingerBone3 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Pinky3)
                .SingleOrDefault();
        }

        // Update is called once per frame
        void Update()
        {
            frame++;
            if(handRootBone == null)
            {
                handRootBone = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_WristRoot)
                .SingleOrDefault();
                handRootBoneObj = handRootBone.Transform.gameObject;
            }
            if(thumbFingerBone0 == null)
            {
                thumbFingerBone0 = ovrSkeleton.Bones
               .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Thumb0)
               .SingleOrDefault();
                thumbFingerBone0Obj = thumbFingerBone0.Transform.gameObject;
            }
            if(pinkyFingerBone0 == null)
            {
                pinkyFingerBone0 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Pinky0)
                .SingleOrDefault();
                pinkyFingerBone0Obj = pinkyFingerBone0.Transform.gameObject;
            }
            if(thumbFingerBone1 == null)
            {
                thumbFingerBone1 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Thumb1)
                .SingleOrDefault();
                thumbFingerBone1Obj = thumbFingerBone1.Transform.gameObject;
            }
            if(thumbFingerBone2 == null)
            {
                thumbFingerBone2 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Thumb2)
                .SingleOrDefault();
                thumbFingerBone2Obj = thumbFingerBone2.Transform.gameObject;
            }
            if(thumbFingerBone3 == null)
            {
                thumbFingerBone3 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Thumb3)
                .SingleOrDefault();
                thumbFingerBone3Obj = thumbFingerBone3.Transform.gameObject;
            }
            if(indexFingerBone1 == null)
            {
                indexFingerBone1 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Index1)
                .SingleOrDefault();
                indexFingerBone1Obj = indexFingerBone1.Transform.gameObject;
            }
            if(indexFingerBone2 == null)
            {
                indexFingerBone2 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Index2)
                .SingleOrDefault();
                indexFingerBone2Obj = indexFingerBone2.Transform.gameObject;
            }
            if(indexFingerBone3 == null)
            {
                indexFingerBone3 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Index3)
                .SingleOrDefault();
                indexFingerBone3Obj = indexFingerBone3.Transform.gameObject;
            }
            if(middleFingerBone1 == null)
            {
                middleFingerBone1 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Middle1)
                .SingleOrDefault();
                middleFingerBone1Obj = middleFingerBone1.Transform.gameObject;
            }
            if(middleFingerBone2 == null)
            {
                middleFingerBone2 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Middle2)
                .SingleOrDefault();
                middleFingerBone2Obj = middleFingerBone2.Transform.gameObject;
            }
            if(middleFingerBone3 == null)
            {
                middleFingerBone3 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Middle3)
                .SingleOrDefault();
                middleFingerBone3Obj = middleFingerBone3.Transform.gameObject;
            }
            if(ringFingerBone1 == null)
            {
                ringFingerBone1 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Ring1)
                .SingleOrDefault();
                ringFingerBone1Obj = ringFingerBone1.Transform.gameObject;
            }
            if(ringFingerBone2 == null)
            {
                ringFingerBone2 = ovrSkeleton.Bones
               .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Ring2)
               .SingleOrDefault();
                ringFingerBone2Obj = ringFingerBone2.Transform.gameObject;
            }
            if(ringFingerBone3 == null)
            {
                ringFingerBone3 = ovrSkeleton.Bones
               .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Ring3)
               .SingleOrDefault();
                ringFingerBone3Obj = ringFingerBone3.Transform.gameObject;
            }
            if(pinkyFingerBone1 == null)
            {
                pinkyFingerBone1 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Pinky1)
                .SingleOrDefault();
                pinkyFingerBone1Obj = pinkyFingerBone1.Transform.gameObject;
            }
            if(pinkyFingerBone2 == null)
            {
                pinkyFingerBone2 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Pinky2)
                .SingleOrDefault();
                pinkyFingerBone2Obj = pinkyFingerBone2.Transform.gameObject;
            }
            if(pinkyFingerBone3 == null)
            {
                pinkyFingerBone3 = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Pinky3)
                .SingleOrDefault();
                pinkyFingerBone3Obj = pinkyFingerBone3.Transform.gameObject;
            }
            if (frame % 60 == 0)
            {
                writeToFile();
            }
        }

        void writeToFile()
        {
            Debug.Log("writeLog");
            GameObject[] fingers = {thumbFingerBone0Obj, thumbFingerBone1Obj, thumbFingerBone2Obj, thumbFingerBone3Obj,
                                        indexFingerBone1Obj, indexFingerBone2Obj, indexFingerBone3Obj,
                                        middleFingerBone1Obj, middleFingerBone2Obj, middleFingerBone3Obj,
                                        ringFingerBone1Obj, ringFingerBone2Obj, ringFingerBone3Obj,
                                        pinkyFingerBone0Obj, pinkyFingerBone1Obj,pinkyFingerBone2Obj, pinkyFingerBone3Obj};
            using (var w = new StreamWriter(F_To_write, true))
            {

                Debug.Log("writing a line");
                for (int i = 0; i < fingers.Length; i++)
                {
                    var z = fingers[i];
                    var t = z.transform.position;
                    var line = string.Format("{0},{1},{2},{3},{4}", finger_names[i], t.x, t.y, t.z, " ");
                    w.Write(line);
                }
                var t1 = handRootBoneObj.transform.rotation;
                var line1 = string.Format("{0},{1},{2},{3},{4}", "root_hand", t1.x, t1.y, t1.z, " ");
                w.Write(line1);
                w.WriteLine();
            }
               
            // draw hand
            //for (int i = 0; i < fingers.Length; i++)
            //    line.SetPosition(i, fingers[i].transform.position);


        }


    }
}
