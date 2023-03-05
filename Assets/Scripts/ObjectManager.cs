using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HandTracking.Parser;
using System.IO;
/**
 * This class should implement swapping and tracking the game objects that we want the user
 * to interact with
 */

public class ObjectManager : MonoBehaviour
{

    #region ObjectsToTrack
    public enum ObjectsToTrack
    {
        Cup,
        Cone,
        Cylinder,
        Cube,
        Ball,
        Coin,
        Arc,
        Pot,
        Lid
    }
    [SerializeField]
    private GameObject Cup;
    public Quaternion RotationCup;
    public Vector3 ScaleCup;
    [SerializeField]
    private GameObject Cone;
    public Quaternion RotationCone;
    public Vector3 ScaleCone;
    [SerializeField]
    private GameObject Cylinder;
    public Quaternion RotationCylinder;
    public Vector3 ScaleCylinder;
    [SerializeField]
    private GameObject Cube;
    public Quaternion RotationCube;
    public Vector3 ScaleCube;
    [SerializeField]
    private GameObject Ball;
    public Quaternion RotationBall;
    public Vector3 ScaleBall;
    [SerializeField]
    private GameObject Coin;
    public Quaternion RotationCoin;
    public Vector3 ScaleCoin;
    [SerializeField]
    private GameObject Arc;
    public Quaternion RotationArc;
    public Vector3 ScaleArc;
    [SerializeField]
    private GameObject Pot;
    public Quaternion RotationPot;
    public Vector3 ScalePot;
    [SerializeField]
    private GameObject Lid;
    public Quaternion RotationLid;
    public Vector3 ScaleLid;

    #endregion
    public Vector3 Position = new Vector3((float)0.089, (float)0.751, (float)0.59);
    
    public Vector3 Scale = new Vector3((float)0.05266825, (float)0.04649679, (float)0.03134177);
    public double ScaleRange = 0.01;
    public int RotateDegreeRange = 30;
    // these are the shapes we intend to display to the user
    private GameObject[] objects_to_track;
    // original scales
    private Vector3[] original_objects_scales;
    // original angles
    private Quaternion[] original_objects_angles;
    // this is the current shape
    private GameObject current_object;
    private int index;
    private System.Random random_rotate;
    private System.Random random_scale;
    private int record_num = 0;
    

    // Start is called before the first frame update
    void Awake()
    {
        random_rotate = new System.Random(7);
        random_scale = new System.Random(29);
        
        // initialize objects_to_track
        GameObject cup = Instantiate(Cup, Position, RotationCup);
        GameObject cone = Instantiate(Cone, Position, RotationCone);
        GameObject cylinder = Instantiate(Cylinder, Position, RotationCylinder);
        GameObject cube = Instantiate(Cube, Position, RotationCube);
        GameObject ball = Instantiate(Ball, Position, RotationBall);
        GameObject coin = Instantiate(Coin, Position, RotationCoin);
        GameObject arc = Instantiate(Arc, Position, RotationArc);
        GameObject pot = Instantiate(Pot, Position, RotationPot);
        GameObject lid = Instantiate(Lid, Position, RotationLid);
        objects_to_track = new GameObject[] { cup, cone, cylinder, cube, ball, coin, arc, pot, lid};
        original_objects_scales = new Vector3[objects_to_track.Length];
        original_objects_angles = new Quaternion[objects_to_track.Length];
        Vector3[] scales = new Vector3[] {ScaleCup, ScaleCone, ScaleCylinder, ScaleCube, ScaleBall, ScaleCoin, ScaleArc,
        ScalePot, ScaleLid};
        for (int i = 0; i < objects_to_track.Length; i++)
        {
            Vector3 currentScale = scales[i];
            if (scales[i].x == 0 && scales[i].y == 0 && scales[i].z == 0) 
                currentScale = Scale; // set default if user didn't change
            objects_to_track[i].transform.localScale = currentScale;
            objects_to_track[i].SetActive(false);
            original_objects_scales[i] = currentScale;
            original_objects_angles[i] = objects_to_track[i].transform.rotation;
        }
        // initialize first object, and index
        index = -1;
        current_object = objects_to_track[0];
        string fnameRight = "PositionBonesRight" + ".csv";
        string FILE_NAME_R = Path.Combine(Application.persistentDataPath, fnameRight);
        if(File.Exists(FILE_NAME_R))
            record_num = CSVParser.get_record_num(FILE_NAME_R);

    }
    /**
     * Sets the next object in circular order
     * 0,1,2,3,4,5,6,0,1, ...
     */
    private void SetNextObject()
    {
        if (index + 1 >= objects_to_track.Length)
        {
            index = -1;
        }
        current_object = objects_to_track[++index];
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
        Debug.Log("[ObjectManager] Random Scale Button was pressed.");
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
        Debug.Log("[ObjectManager] Random Rotation Button was pressed.");
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
        record_num++;
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


}
