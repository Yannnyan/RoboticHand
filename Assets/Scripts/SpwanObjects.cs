using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Orientation;
using Genetic;

public class SpwanObjects : MonoBehaviour
{
    [SerializeField] GameObject[] objects;
    [SerializeField] KeyCode key;
    public Orientation.Orientation orientation;
    private float[] rotations;
    private int index = 0;
    private GameObject lastObject;
    private Client client;

    // Start is called before the first frame update
    void Start()
    {
        client = GetComponent<Client>();
        lastObject = spawnObject(objects[index]);
    }

    private void updateRotations(float[] rotations)
    {
        this.rotations = rotations;
    }

    // Update is called once per frame
    void Update()
    {
        if(rotations != null)
        {
            orientation.LoadAndTransformSecondary(Genetic.Manager.normalize_output_360(rotations));
            rotations = null;
        }
        if(Input.GetKeyDown(key))
        {
            Destroy(lastObject);
            
            index = (index + 1) % objects.Length;
            
            lastObject = spawnObject(objects[index]);
            
            var inputData = lastObject.GetComponent<WirteTheObject>().GetInputObject();

            GameObject wrist = orientation.GetHandByTag()["wrist"][0];

            Client.onServerMessage onmsg = new Client.onServerMessage(updateRotations);

            client.SedMesh(
                inputData,
                new Vector3[] {new Vector3(0,0,0), new Vector3(0,0,0)}, // wrist.transform.position, wrist.transform.rotation.eulerAngles
                onmsg);

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
