using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HandTracking.Parser;
using HandTracking.TrackHand;
using System.IO;
using RoboticHand;
/**
 * This class should implement swapping and tracking the game objects that we want the user
 * to interact with
 */

public class ObjectManager : MonoBehaviour
{

    #region ObjectsToTrack
    [SerializeField]
    private TrackHandRight trackHandRight;
    [SerializeField]
    private CreateInputFile createInputFile;
    bool done_recording = true;


    #endregion
    public Vector3 Position = new Vector3((float)0.089, (float)0.35, (float)0.59);
    // Vector3(0.0526682511,0.0464967899,0.0313417688)
    private Vector3 Scale = new Vector3((float)0.0526682511 * 4, (float)0.0464967899 * 4, (float)0.0313417688 * 4);
    public double ScaleRange = 0.01;
    public int RotateDegreeRange = 360; // was 30
    // these are the shapes we intend to display to the user
    private List<GameObject> objects_to_track;
    // original scales
    private List<Vector3> original_objects_scales;
    // original angles
    private List<Quaternion> original_objects_angles;
    // this is the current shape
    private GameObject current_object;
    private int index = -1;
    private System.Random random_rotate;
    private System.Random random_scale;
    private int record_num = 0;
    public float period = 0.1f;

    // Start is called before the first frame update
    void Awake()
    {
        // init seed
        random_rotate = new System.Random();
        random_scale = new System.Random();

        // initialize objects_to_track

        objects_to_track = new List<GameObject>();
        original_objects_angles = new List<Quaternion>();
        original_objects_scales = new List<Vector3>();

        // get objects from object holder
        GameObject holder = GameObject.Find("ObjectHolder");
        holder.transform.gameObject.transform.localScale = Scale;

        foreach (Transform t in holder.transform)
        {
            // include objects in the gameobject list
            t.gameObject.transform.position = Position;
            objects_to_track.Add(t.gameObject);
            original_objects_scales.Add(t.localScale);
            original_objects_angles.Add(t.localRotation);
            t.gameObject.SetActive(false);
        }
        // initialize first object, and index
        current_object = objects_to_track[0];
        string fnameRight = "PositionBonesRight" + ".csv";
        string FILE_NAME_R = Path.Combine(Application.persistentDataPath, fnameRight);
        if(File.Exists(FILE_NAME_R))
            record_num = CSVParser.get_record_num(FILE_NAME_R);
        this.LoadNextObject();
    }

    void Update()
    {
        // trackHandRight.printHand();
        //if (Time.time > nextActionTime)
        //{
        //    nextActionTime += period;
        //    // execute block of code here
        //    trackHandRight.printHand();
        //}
    }
    /**
     * Sets the next object in circular order
     * 0,1,2,3,4,5,6,0,1, ...
     */
    private void SetNextObject()
    {
        if (index + 1 >= objects_to_track.Count)
        {
            index = -1;
        }
        current_object = objects_to_track[++index];
        // change rotation and scale
        RandomRotationOnClick(); 
        RandomScaleOnClick();
        //Debug.Log(current_object.name);
    }
    private void DisplayNextObject()
    {
        current_object.SetActive(true);
    }
    private void DisableCurrentObject()
    {
        current_object.SetActive(false);
    }
    /**
     * This is what we call outside to swap objects
     */
    public void LoadNextObject()
    {
        Debug.Log("[ObjectManager] Swap Objects Button was Pressed!");
        DisableCurrentObject();
        SetNextObject();
        DisplayNextObject();
    }

    /**
     * Generates random scale and changes the current object's scale
     */
    public void RandomScaleOnClick()
    {
        // Debug.Log("[ObjectManager] Random Scale Button was pressed.");
        float num_gen_x = (float)(random_scale.NextDouble() * ScaleRange);
        float num_gen_y = (float)(random_scale.NextDouble() * ScaleRange);
        float num_gen_z = (float)(random_scale.NextDouble() * ScaleRange);
        int is_neg_x = random_scale.Next(2);
        int is_neg_y = random_scale.Next(2);
        int is_neg_z = random_scale.Next(2);
        if (is_neg_x == 1)
            num_gen_x *= -1;
        if (is_neg_y == 1)
            num_gen_y *= -1;
        if (is_neg_z == 1)
            num_gen_z *= -1;
        Vector3 tweak = new(num_gen_x, num_gen_y, num_gen_z);
        ChangeObjectScale(tweak);

    }
    public void RandomRotationOnClick()
    {
        // Debug.Log("[ObjectManager] Random Rotation Button was pressed.");
        // get floating number for x, y , z
        float num_gen_x = (float)(random_rotate.NextDouble() * RotateDegreeRange);
        float num_gen_y = (float)(random_rotate.NextDouble() * RotateDegreeRange);
        float num_gen_z = (float)(random_rotate.NextDouble() * RotateDegreeRange);
        // change sign
        int is_neg_x = random_rotate.Next(2);
        int is_neg_y = random_rotate.Next(2);
        int is_neg_z = random_rotate.Next(2);
        if (is_neg_x == 1)
            num_gen_x *= -1;
        if (is_neg_y == 1)
            num_gen_y *= -1;
        if (is_neg_z == 1)
            num_gen_z *= -1;
        // set rotation
        Vector3 tweak = new(num_gen_x, num_gen_y, num_gen_z);
        ChangeObjectRotation(tweak);
    }

    public void TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot($"Input{record_num}.png");
        Debug.Log("[ObjectManager] Took Screenshot");
    }
    public void MakeInvisible()
    {
        GameObject console = GameObject.Find("DebugArea");
        console.GetComponent<Canvas>().enabled = !console.GetComponent<Canvas>().enabled;
        Debug.Log("[ObjectManager] Debugger Made Invisible");
    }
    /**
     * Recieves a Scale tweak that should act as a scaler to increase or decrease the scale 
     * of the original object scale
     */
    private void ChangeObjectScale(Vector3 newScaleTweak)
    {
        objects_to_track[index].transform.localScale = original_objects_scales[index] + newScaleTweak;
    }

    /**
     * Recieves a Vector that defines angles tweaks and changes the rotation by these angles with euler angles
     * 
     */
    private void ChangeObjectRotation(Vector3 newEulerAnglesTweak)
    {
        Vector3 newEulerAngles = original_objects_angles[index].eulerAngles + newEulerAnglesTweak;
        objects_to_track[index].transform.rotation = Quaternion.Euler(newEulerAngles);
    }
    public void RecordButtonPress()
    {
        if (current_object != null)
        {
            record_num++;
            write_input_file();
            write_target_file();
        }
        else
        {
            Debug.Log("[ObjectManager] Current Object Is Null");
        }
        
    }

    private void write_target_file()
    {
        trackHandRight.writeToFile();
    }
    private void write_input_file()
    {
        Debug.Log("Getting mesh of object: " + current_object.name);
        createInputFile.write_inputFile(createInputFile.getMeshPoints(current_object),
            trackHandRight.handRootBoneObj.transform.position,
            trackHandRight.handRootBoneObj.transform.rotation.eulerAngles,
            record_num)
            ;
    }
    public void onTouch()
    {
        //if(current_object.activeSelf)
        if(done_recording)
        {
            done_recording = false;
            TakeScreenshot();
            StartCoroutine(onTouchRoutine());
        }
            

    }
    public IEnumerator onTouchRoutine()
    {
        RecordButtonPress();
        //yield on a new YieldInstruction that waits for i seconds.
        yield return new WaitForSeconds(2);

        //After we have waited i seconds print the time again.
        LoadNextObject();
        done_recording =true;
    }
}
