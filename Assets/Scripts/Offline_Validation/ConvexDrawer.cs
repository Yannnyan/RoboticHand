using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OfflineValidation{
    public class ConvexDrawer : MonoBehaviour
    {
        public LineRenderer lineRenderer;
        private GameObject[] point_cloud;
        public GameObject cloud;
        // Start is called before the first frame update
        void Start()
        {
            //lineRenderer.SetWidth(0.04f, 0.04f);

        }

        // Update is called once per frame
        void Update()
        {

        }


        public void renderLineConvex(Vector3[] convex)
        {
            
            lineRenderer.positionCount = convex.Length;
            for (int i = 0; i < convex.Length; i++)
            {
                Vector3 point = convex[i];
                lineRenderer.SetPosition(i, point);
            }
            
        }

        public void renderCircleConvex(Vector3[] convex, Vector3 subjectTo)
        {
            cloud.transform.localScale = Vector3.one;
            cloud.transform.localPosition = Vector3.zero;
            if(point_cloud != null)
            {
                for(int i=0; i< point_cloud.Length; i++)
                {
                    DestroyImmediate(point_cloud[i]);
                }
                point_cloud = null;
            }
            point_cloud = new GameObject[convex.Length];
            for (int i=0; i< convex.Length; i++)
            {
                
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //Debug.Log(conv);
                sphere.transform.position = convex[i];
                sphere.transform.localScale = Vector3.one * 0.01f;
                point_cloud[i] = sphere;
                sphere.transform.parent = cloud.transform;
            }
            cloud.transform.localScale = Vector3.one * 10;
            cloud.transform.position = subjectTo + new Vector3(-1.5f,0,0);
            




        }
    }

}
