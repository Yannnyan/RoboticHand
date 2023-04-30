using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoboticHand;
using System.Linq;

namespace RoboticHand

{
    public class MeshPoints : MonoBehaviour
    {
        [SerializeField]
        private GameObject lineGeneratorPrefab;
        GameObject cone;
        private List<GameObject> linesGenerated;
        [SerializeField]
        private CreateInputFile createInputFile;

        /**
         * Drawing the cone mesh points 
         */
        // Start is called before the first frame update
        void Start()
        {
            if(linesGenerated == null)
                linesGenerated = new List<GameObject>();
            //cone = GameObject.Find("Pot");
            //Transform cone_transoform = cone.transform;
            //Mesh coneMesh = cone.GetComponent<MeshFilter>().mesh;
            //Vector3[] vertices = coneMesh.vertices;
            //List<Vector3> lines_holder = new List<Vector3>();

            //for(int i = 0; i < vertices.Length; i++)
            //{
            //    Vector3 vertexWorldPosition = cone_transoform.TransformPoint(vertices[i]);
            //    Debug.Log($"{vertexWorldPosition.x}, {vertexWorldPosition.y}, {vertexWorldPosition.z}");
            //    lines_holder.Add(vertexWorldPosition);
            //}
            //spawnLineGenerator(lines_holder, Color.red);
            //List<List<Vector3>> meshVectors = createInputFile.read_inputFile();
            //Debug.Log(meshVectors.Count);

            //createMesh(meshVectors[0]);
            //spawnLineGenerator(meshVectors[0], Color.red);
            //while(i < 32)
            //{
            //    StartCoroutine(waiter(i,meshVectors));
            //    //destroyPreviousDrawing();
            //    i++;
            //}
        }

        public static List<Vector3> RotateMesh(List<Vector3> originalVerts, float angleX, float angleY, float angleZ)
        {

            List<Vector3> rotatedVerts = new List<Vector3>();
            var qAngle = Quaternion.Euler(angleX,angleY, angleZ);
            for (int vert = 0; vert < originalVerts.Count; vert++)
            {
                rotatedVerts.Add(qAngle * originalVerts[vert]);
            }

            return rotatedVerts;
        }
        public void createMesh(List<Vector3> vertices)
        {
            //vertices = RotateMesh(vertices, 270,0,0);
            Mesh mesh = new Mesh();
            int[] triangles = Enumerable.Range(0, vertices.Count-1).ToArray();
            
            mesh.SetVertices(vertices);
            mesh.triangles = triangles;
            Vector3 middle = mesh.bounds.center;
            Mesh moved_mesh = new Mesh();
            List<Vector3> moved_vertices = new List<Vector3>();
            foreach (Vector3 vec in vertices)
            {
                moved_vertices.Add(vec - middle);
            }
            MeshFilter mf = gameObject.AddComponent<MeshFilter>();
            mf.mesh = moved_mesh;


            //this.gameObject.transform.Rotate(new Vector3(90, 0, 0), Space.World);
            //this.gameObject.transform.Translate(new Vector3(90, 0, 0), Space.World);
            //this.gameObject.transform.rotation = Quaternion.Euler(90,0,0);
        }
        IEnumerator waiter(int i, List<List<Vector3>> meshVectors)
        {
            spawnLineGenerator(meshVectors[i], Color.blue);

            //Wait for 2 seconds
            yield return new WaitForSeconds(2);


        }
        public void spawnLineGenerator(List<Vector3> fingerLine, Color color)
        {
            if(linesGenerated == null)
            {
                linesGenerated = new List<GameObject>();
            }
            GameObject newLineGenerator = Instantiate(lineGeneratorPrefab);

            LineRenderer lineRenderer = newLineGenerator.GetComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            lineRenderer.positionCount = fingerLine.Count;

            for (int i = 0; i < fingerLine.Count; i++)
            {
                lineRenderer.SetPosition(i, fingerLine[i]);
            }
            GradientColorKey[] tempColorKeys = new GradientColorKey[1];
            for (int i = 0; i < tempColorKeys.Length; i++)
                tempColorKeys[i] = new GradientColorKey(color, i);
            Gradient tempGradient = new Gradient();
            tempGradient.colorKeys = tempColorKeys;
            lineRenderer.colorGradient = tempGradient;

            linesGenerated.Add(newLineGenerator);

        }
        private void destroyPreviousDrawing()
        {
            List<GameObject> linesTemp = linesGenerated.GetRange(0, linesGenerated.Count);
            foreach (GameObject line in linesTemp)
            {
                linesGenerated.Remove(line);
                Destroy(line);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
