using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Make_Dissapear : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
