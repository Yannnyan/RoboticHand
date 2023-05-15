using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpwanObjects : MonoBehaviour
{
    [SerializeField] GameObject[] objects;
    [SerializeField] KeyCode key;
    private int index = 0;
    private GameObject lastObject;
    private Client client;
    // Start is called before the first frame update
    void Start()
    {
        client = GetComponent<Client>();
        lastObject = spawnObject(objects[index]);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(key))
        {
            Destroy(lastObject);
            index = (index + 1) % objects.Length;
            lastObject = spawnObject(objects[index]);
            var inputData = lastObject.GetComponent<WirteTheObject>().GetInputObject();
            client.SedMesh(inputData);
        }
    }

    protected virtual GameObject spawnObject(GameObject prefabToSpawn)
    {
        //Debug.Log("Spawning a new object");

        // Step 1: spawn the new object.
        GameObject newObject = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        return newObject;
    }
}
