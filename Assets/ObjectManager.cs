using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class should implement swapping and tracking the game objects that we want the user
 * to interact with
 */

public class ObjectManager : MonoBehaviour
{
    #region ObjectsToTrack
    [SerializeField]
    private GameObject Cup;
    public Quaternion RotationCup;
    [SerializeField]
    private GameObject Cone;
    public Quaternion RotationCone;
    [SerializeField]
    private GameObject Cylinder;
    public Quaternion RotationCylinder;
    [SerializeField]
    private GameObject Cube;
    public Quaternion RotationCube;
    [SerializeField]
    private GameObject Ball;
    public Quaternion RotationBall;
    [SerializeField]
    private GameObject Coin;
    public Quaternion RotationCoin;
    [SerializeField]
    private GameObject Arc;
    public Quaternion RotationArc;

    #endregion
    public Vector3 Position = new Vector3((float)0.089, (float)0.659, (float)0.59);
    
    public Vector3 Scale = new Vector3((float)0.05266825, (float)0.04649679, (float)0.03134177);
    // these are the shapes we intend to display to the user
    private GameObject[] objects_to_track;
    // this is the current shape
    private GameObject current_object;
    private int index;
    int frame = 0;
    

    // Start is called before the first frame update
    void Awake()
    {
        // initialize objects_to_track
        GameObject cup = Instantiate(Cup, Position, RotationCup);
        GameObject cone = Instantiate(Cone, Position, RotationCone);
        GameObject cylinder = Instantiate(Cylinder, Position, RotationCylinder);
        GameObject cube = Instantiate(Cube, Position, RotationCube);
        GameObject ball = Instantiate(Ball, Position, RotationBall);
        GameObject coin = Instantiate(Coin, Position, RotationCoin);
        GameObject arc = Instantiate(Arc, Position, RotationArc);
        objects_to_track = new GameObject[] { cup, cone, cylinder, cube, ball, coin, arc};
        for (int i = 0; i < objects_to_track.Length; i++)
        {
            objects_to_track[i].transform.localScale = Scale;
            objects_to_track[i].SetActive(false);
        }
        // initialize first object, and index
        index = 0;
        current_object = objects_to_track[index];
        
    }
    //void Update()
    //{
    //    frame++;
    //    if(frame % 300 == 0)
    //        LoadNextObject();    
    //}
    /**
     * Sets the next object in circular order
     * 0,1,2,3,4,5,6,0,1, ...
     */
    private void SetNextObject()
    {
        if (index + 1 >= objects_to_track.Length)
        {
            index = 0;
        }
        current_object = objects_to_track[index++];
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
        DisableCurrentObject();
        SetNextObject();
        DisplayNextObject();
        Debug.Log("Swap Objects Button was Pressed!");
    }
}
