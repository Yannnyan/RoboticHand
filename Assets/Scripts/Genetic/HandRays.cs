using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Orientation;

public class HandRays : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Dictionary<string, List<GameObject>> hand = handOrient.GetHand();
        //    foreach (string key in hand.Keys)
        //    {
        //        if (key == "wrist")
        //            continue;
        //        int last = hand[key].Count;
        //        GameObject last_joint = hand[key][last - 1];
        //        GameObject beforeLast_joint = hand[key][last - 2];
        //        Vector3 direction = last_joint.transform.position - beforeLast_joint.transform.position;
        //        if (Physics.Raycast(last_joint.transform.position, direction, out RaycastHit hitinfo, 20f))
        //        {
        //            Debug.Log(key + " Hit Something in distance: " + hitinfo.distance);
        //            Debug.DrawRay(last_joint.transform.position, direction * hitinfo.distance, Color.red);
        //        }
        //        else
        //        {
        //            Debug.Log(key + " Hit Nothing");
        //            Debug.DrawRay(last_joint.transform.position, last_joint.transform.TransformDirection(direction) * hitinfo.distance, Color.black);
        //        }
                
        //    }
        //}

    }

    public float[] getRayDistances(Dictionary<string, List<GameObject>>hand, GameObject obj)
    {
        float[] distances = new float[5];
        int i = 0;
        foreach (string key in hand.Keys)
        {
            if (key == "wrist")
                continue;
            int last = hand[key].Count;
            GameObject last_joint = hand[key][last - 1];
            GameObject beforeLast_joint = hand[key][last - 2];
            Vector3 direction = last_joint.transform.position - beforeLast_joint.transform.position;
            Vector3 ideal_direction = (obj.transform.position - last_joint.transform.position);
            // float eps = 0.0001f;

            //float correlation = Vector3.Dot(direction, ideal_direction);
            float correlation = 0;
            correlation = correlation < 0 ? correlation * 2 : correlation; // give extra weight to oposite side correlation 

            if (Physics.Raycast(last_joint.transform.position, direction, out RaycastHit hitinfo, 5f))
            {
                Debug.Log(key + " Hit " + hitinfo.collider.gameObject.name + " in distance: " + hitinfo.distance);

                Debug.DrawRay(last_joint.transform.position, direction * hitinfo.distance, Color.red);

                // take distance with oposite relation to ray distance
                distances[i++] = hitinfo.distance - correlation;
            }
            else
            {
                Debug.Log(key + " Hit Nothing");

                Debug.DrawRay(last_joint.transform.position, last_joint.transform.TransformDirection(direction) * hitinfo.distance, Color.black);

                distances[i++] = 5 - correlation;
            }
        }
        Debug.Log("Scores: " + string.Join(", ", distances));
        return distances;
    }
}
