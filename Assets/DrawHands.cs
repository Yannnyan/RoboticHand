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

        // awake is called once when the script is loaded
        // if we have a path to read, read it and then display the hands in the update per 60 frames.
        void Awake()
        {
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
            if (frame % 60 == 0)
            {
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
                        spawnLineGenerator(leftLine);
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
                        spawnLineGenerator(rightLine);
                    }
                    indexRight++;
                }
            }
            // continue to count frames
            frame++;
          
        }
    
       
        private void spawnLineGenerator(List<Vector3> fingerLine)
        {
            GameObject newLineGenerator = Instantiate(lineGeneratorPrefab);
            LineRenderer lineRenderer = newLineGenerator.GetComponent<LineRenderer>();

            for(int i=0; i< fingerLine.Count; i++)
            {
                lineRenderer.SetPosition(i, fingerLine[i]);
            }

        }
    }

}