using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using HandTracking.Parser;
namespace HandTracking.Draw
{
    public class DrawHands : MonoBehaviour
    {
        [SerializeField]
        private GameObject lineGeneratorPrefab;
        [SerializeField]
        private string LEFT_CSV_PATH;
        [SerializeField]
        private string RIGHT_CSV_PATH;
        private List<List<List<Vector3>>> leftFingerLines;
        private List<List<List<Vector3>>> rightFingerLines;
        int frame = 0;
        int indexLeft = 0;
        int indexRight = 0;
        private List<GameObject> linesGenerated;
        private Dictionary<CSVParser.ParsingOrder, Color> colorDict = new Dictionary<CSVParser.ParsingOrder, Color>
        { {CSVParser.ParsingOrder.ThumbFinger, Color.blue },
            {CSVParser.ParsingOrder.IndexFinger, Color.yellow },
            {CSVParser.ParsingOrder.MiddleFinger, Color.red },
            {CSVParser.ParsingOrder.RingFinger, Color.green },
            {CSVParser.ParsingOrder.PinkyFinger, Color.magenta } };
        // awake is called once when the script is loaded
        // if we have a path to read, read it and then display the hands in the update per 60 frames.
        void Awake()
        {
            linesGenerated = new List<GameObject>();
            CSVParser parser = new CSVParser();
            if(LEFT_CSV_PATH != null && LEFT_CSV_PATH != "")
                leftFingerLines = parser.readCSV(LEFT_CSV_PATH);
            if(RIGHT_CSV_PATH != null && RIGHT_CSV_PATH != "")
                rightFingerLines = parser.readCSV(RIGHT_CSV_PATH);

        }

        // Update is called once per frame
        // each 60 frames, display both hands if in range
        // display left hand, and without the wrist root rotation, and the root position (we draw from the root the lines)
        // supposed to look like this: https://user-images.githubusercontent.com/6291046/72648786-c8e6be00-3930-11ea-914e-b1fc9e6df1b6.png
        void Update()
        {
            // each 60 frames display the hand
            if (frame % 300 == 0)
            {
                destroyPreviousDrawing();
                //var thumb_line = leftFingerLines[indexLeft][0];
                //spawnLineGenerator(thumb_line);
                // if we are in range to display the left hand
                if (indexLeft < leftFingerLines.Count)
                {
                    for (var i = CSVParser.ParsingOrder.ThumbFinger; i < CSVParser.ParsingOrder.WristRootPos; i++)
                    {
                        List<Vector3> leftLine = new List<Vector3>();
                        // add the root pos
                        leftLine.AddRange(leftFingerLines[indexLeft][(int)CSVParser.ParsingOrder.WristRootPos]);
                        // add the current finger line
                        leftLine.AddRange(leftFingerLines[indexLeft][(int)i]);
                        
                        spawnLineGenerator(leftLine, colorDict[i]);
                    }
                    indexLeft++;
                }
                // if we are in range to display the right hand
                if (indexRight < rightFingerLines.Count)
                {
                    for (var i = CSVParser.ParsingOrder.ThumbFinger; i < CSVParser.ParsingOrder.WristRootPos; i++)
                    {

                        List<Vector3> rightLine = new List<Vector3>();
                        // add the root pos
                        rightLine.AddRange(rightFingerLines[indexRight][(int)(CSVParser.ParsingOrder.WristRootPos)]);
                        // add the current finger line
                        rightLine.AddRange(rightFingerLines[indexRight][(int)i]);
                        spawnLineGenerator(rightLine, colorDict[i]);
                    }
                    indexRight++;
                }
            }
            // continue to count frames
            frame++;
          
        }
        
        private void destroyPreviousDrawing()
        {
            List<GameObject> linesTemp = linesGenerated.GetRange(0, linesGenerated.Count);
            foreach(GameObject line in linesTemp)
            {
                linesGenerated.Remove(line);
                Destroy(line);
            }
        }
       
        private void spawnLineGenerator(List<Vector3> fingerLine, Color color)
        {
            GameObject newLineGenerator = Instantiate(lineGeneratorPrefab);
            
            LineRenderer lineRenderer = newLineGenerator.GetComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            lineRenderer.positionCount = fingerLine.Count;
            
            for(int i=0; i< fingerLine.Count; i++)
            {
                lineRenderer.SetPosition(i, fingerLine[i]);
            }
            GradientColorKey[] tempColorKeys = new GradientColorKey[fingerLine.Count];
            for (int i = 0; i < tempColorKeys.Length; i++)
                tempColorKeys[i] = new GradientColorKey(color, i);
            Gradient tempGradient = new Gradient();
            tempGradient.colorKeys = tempColorKeys;
            lineRenderer.colorGradient = tempGradient;
            
            linesGenerated.Add(newLineGenerator);

        }
    }

}