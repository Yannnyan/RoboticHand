using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectTouch : MonoBehaviour
{
    [SerializeField]
    private UnityEvent whenCollide;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Occured!");
        whenCollide.Invoke();
        //this.gameObject.SetActive(false);
    }
}
