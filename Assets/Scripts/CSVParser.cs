using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using UnityEngine;

namespace HandTracking.Parser
{
    
    public class CSVParser
    {
        /**
         * describes the order in which the List<List<Vector3>> is ordered after 
         * getting it from readCSV()
         * ThumbFinger the position of the thumb finger
            IndexFinger, the position of the index finger
            MiddleFinger, the position of the middle finger
            RingFinger, the position of the ring finger
            PinkyFinger, the position of the pinky finger
            WristRootPos, the position of the wrist root position
            WristRootRot the position of the wrist root rotation
         */
        public enum ParsingOrder
        {
            ThumbFinger,
            IndexFinger,
            MiddleFinger,
            RingFinger,
            PinkyFinger,
            WristRootPos,
            WristRootRot
        }
       
        int START_THUMB = 0, END_THUMB = 3, START_INDEX = 4, END_INDEX = 6,
            START_MIDDLE = 7, END_MIDDLE = 9, START_RING = 10, END_RING = 12,
            START_PINKY = 13, END_PINKY = 16, ROOT_POS = 17, ROOT_ROT=18;


       

        public static int get_record_num(string csvPath)
        {
            string[] lines = File.ReadAllLines(csvPath);
            return lines.Length - 1;
        }
        /**
         * Parses the Row we got from the csv to a list of Vectors,
         * where each vector represents the coordinates of a joint in the hand.
         */
        private List<Vector3> parseRowToVectors(string row)
        {
            List<string> values = new();
            values.AddRange(row.Split(','));
            values.RemoveAt(0);
            List<Vector3> vectors = new List<Vector3>();
            Vector3 current = new Vector3();
            float x = 0, y = 0, z = 0;
            for(int i=0; i< values.Count; i++)
            {
                if (i % 3 == 0)
                {
                    Debug.Log(values[i]);
                    x = float.Parse(values[i]);
                }
                else if (i % 3 == 1)
                    y = float.Parse(values[i]);
                else
                {
                    z = float.Parse(values[i]);
                    current = new Vector3();
                    current.Set(x, y, z);
                    vectors.Add(current);
                }
            }
            return vectors;
        }
        /**
         * parsing a single row to the finger lines, in the order defined
         * in ParsingOrder
         */
        private List<List<Vector3>> parseRowToFingerLines(string row)
        {
            List<Vector3> allVectors = parseRowToVectors(row);
            List<List<Vector3>> fingerLines = new List<List<Vector3>>();
            fingerLines.Add(getThumbLine(allVectors));
            fingerLines.Add(getIndexLine(allVectors));
            fingerLines.Add(getMiddleLine(allVectors));
            fingerLines.Add(getRingLine(allVectors));
            fingerLines.Add(getPinkyLine(allVectors));
            fingerLines.Add(getRootPos(allVectors));
            fingerLines.Add(getRootRot(allVectors));
            return fingerLines;
        }
        /**
         * gets a list of all the vectors, and extracts only the finger line vectors
         */
        private List<Vector3> getFingerLine(List<Vector3> vectors, int start, int end)
        {
            List<Vector3> fingerLineVectors = new List<Vector3>();
            
            for (int i = start; i <= end; i++)
                fingerLineVectors.Add(vectors[i]);
            return fingerLineVectors;
        }
        /**
        * gets a list of all the vectors, and extracts only the thumb line vectors
        */
        private List<Vector3> getThumbLine(List<Vector3> vectors) => getFingerLine(vectors, START_THUMB, END_THUMB);

        /**
        * gets a list of all the vectors, and extracts only the index line vectors
        */
        private List<Vector3> getIndexLine(List<Vector3> vectors) => getFingerLine(vectors, START_INDEX, END_INDEX);

        /**
        * gets a list of all the vectors, and extracts only the middle line vectors
        */
        private List<Vector3> getMiddleLine(List<Vector3> vectors) => getFingerLine(vectors, START_MIDDLE, END_MIDDLE);
        /**
        * gets a list of all the vectors, and extracts only the ring line vectors
        */
        private List<Vector3> getRingLine(List<Vector3> vectors) => getFingerLine(vectors, START_RING, END_RING);
        /**
        * gets a list of all the vectors, and extracts only the pinky line vectors
        */
        private List<Vector3> getPinkyLine(List<Vector3> vectors) => getFingerLine(vectors, START_PINKY, END_PINKY);
        /**
         * using the function with a list of all vectors to extract the position of the wrist root of the hand
         */
        private List<Vector3> getRootPos(List<Vector3> vectors) => getFingerLine(vectors, ROOT_POS, ROOT_POS);
        /**
         * using the same function to get the rotation
         */
        private List<Vector3> getRootRot(List<Vector3> vectors) => getFingerLine(vectors, ROOT_ROT, ROOT_ROT);


        /**
         * The outer list is the list of rows of the csv
         * the second list is the list of lines for the fingers and root,
         * the order of the lines is described in the enum ParsingOrder, defined in this class
         * the third inner list is the finger line itself, in order.
         * The order of the return list is described in the
         */
        public List<List<List<Vector3>>> readCSV(string csvPath)
        {
            List<List<List<Vector3>>> allFingerLines = new List<List<List<Vector3>>>();
            if (File.Exists(csvPath))
            {
                StreamReader reader = new StreamReader(File.OpenRead(csvPath));
                bool isFirst = true;
                while (!reader.EndOfStream)
                {
                    if(isFirst)
                    {
                        isFirst = false;
                        reader.ReadLine();
                        continue;
                    }
                    string line = reader.ReadLine();
                    List<List<Vector3>> fingerLines = parseRowToFingerLines(line);
                    allFingerLines.Add(fingerLines);
                }
            }
            return allFingerLines;
        }
    }
}
