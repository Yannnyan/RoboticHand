using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Globalization;


public class Dissapear_Object : MonoBehaviour
{
    int seconds_start;
    int last_second;
    int seconds;
    int state;
    // Start is called before the first frame update
    void Start()
    {
        last_second = 0;
        seconds = 0;
        seconds_start = DateTime.Now.Second;
        state = 0;

    }
    private void make_dissapear()
    {
        //Thread.Sleep(2000);
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void make_appear()
    {
        //Thread.Sleep(3000);
        GetComponent<MeshRenderer>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        seconds = (DateTime.Now.Second - last_second) % 60;
        print(last_second);
        print(seconds);
        if (state == 0 && seconds > 5)
        {
            make_dissapear();
            last_second = DateTime.Now.Second;
            state = 1;
        }
        else if (state == 1 && seconds > 5)
        {
            make_appear();
            last_second = DateTime.Now.Second;
            state = 0;
        }
        
    }
    
}
