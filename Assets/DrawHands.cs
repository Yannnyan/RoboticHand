using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace HandTracking.Draw
{
    public class DrawHands : MonoBehaviour
    {
        [SerializeField]
        private GameObject lineGeneratorPrefab;

       

        // Start is called before the first frame update
        void Awake()
        {
        
        }
    
        // Update is called once per frame
        void Update()
        {
        
        }
    
       
        private void spawnLineGenerator(Vector3[] pointsArray)
        {
            GameObject newLineGenerator = Instantiate(lineGeneratorPrefab);
            LineRenderer lineRenderer = newLineGenerator.GetComponent<LineRenderer>();

            for(int i=0; i< pointsArray.Length; i++)
            {
                lineRenderer.SetPosition(i, pointsArray[i]);
            }

        }
    }

}