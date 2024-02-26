using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Orientation;
using System.IO;

public class RotateHand : MonoBehaviour
{
    private float[] rotations;
    public Orientation.Orientation orientation;
    private bool toStop = false;
    private int joint_num = 0;
    private string capturePath = @"C:\Users\Yan\Desktop\Simulator Snaps";
    private int capture_id = 0;
    private int finger_pressed = 0;

    // Start is called before the first frame update
    void Start()
    {
        toStop = true;
        StartCoroutine(GetNextInput());
    }



    // Update is called once per frame
    void Update()
    {
        //for (int i = 0; i < 18; i++)
        
    }


    float[] getNextRotation()
    {
        int num_floats = 54;
        float[] result = new float[num_floats];
        for (int i = 0; i < num_floats; i++)
        {
            result[i] = Random.Range((float)0, 1) * 360;
        }
        return result;
    }
    void rotateHand()
    {
        if (!toStop)
        {
            if (finger_pressed == 0)
            {
                for(int i=0; i< 5; i++)
                {
                    runNextRotation(2 + joint_num + 3 * i);
                }
            }
            else if(finger_pressed == 1)
            {
                runNextRotation(1 + joint_num);
            }
            else if(finger_pressed == 2)
            {
                runNextRotation(5 + joint_num);
            }
            else if (finger_pressed == 3)
            {
                runNextRotation(8 + joint_num);
            }
            else if (finger_pressed == 4)
            {
                runNextRotation(11 + joint_num);
            }
            else if (finger_pressed == 5)
            {
                runNextRotation(14 + joint_num);
            }
        }
    }
    void runNextRotation(int iToChange)
    {
        //rotations[iToChange * 3]+=10;
        //rotations[iToChange * 3 + 1] += 10;
        rotations = orientation.record_hand();
        rotations[iToChange * 3 + 2] += 1;
        orientation.LoadAndTransform(rotations);
    }
    
    private void writeTheArr(float[] arr)
    {
        string F_To_write = @"C:\Users\Yan\Desktop\Simulator Snaps\handRecordings.csv";
        using (var w = new StreamWriter(F_To_write, true)) 
        {
            for (int i = 0; i < arr.Length; i++)
            {
                w.Write(arr[i]);
                w.Write(",");
            }
            w.WriteLine();
        }
    }
    private IEnumerator GetNextInput()
    {
        // do stuff here, show win screen, etc.

        // just a simple time delay as an example
        // yield return new WaitForSeconds(2.5f);

        // wait for player to press space
        yield return waitForKeyPress(); // wait for this function to return

        // do other stuff after key press
    }

    private IEnumerator waitForKeyPress()
    {
        string chars = "012345";
        while (true) // essentially a "while true", but with a bool to break out naturally
        {
            string input = "";
            if (Input.GetKeyDown("w"))
            {
                toStop = !toStop;
            }
            else if (Input.GetKeyDown("q"))
            {
                joint_num = finger_pressed == 1 || finger_pressed == 5 ? (joint_num + 1) % 4 : (joint_num + 1) % 3;
            }
            else if (Input.GetKeyDown("p"))
            {
                writeTheArr(rotations);
                ScreenCapture.CaptureScreenshot(capturePath + "/Capture" + capture_id.ToString() + ".png");
                capture_id++;
            }
            foreach (char c in chars)
            {
                if (Input.GetKey("" + c))
                {
                    finger_pressed = c - '0';
                    break;
                }
            }
            
            rotateHand();



            yield return null; // wait until next frame, then continue execution from here (loop continues)
        }

        // now this function returns
    }
}
