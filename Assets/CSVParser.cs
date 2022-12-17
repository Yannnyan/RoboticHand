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
        string PATH_TO_CSV1;
        string PATH_TO_CSV2;
        int SIZE_VECTORS = 18; // amount of joints we extract from the csv
        int AMOUNT_FINGERS = 5;
        
        /**
         * Parses the Row we got from the csv to a list of Vectors,
         * where each vector represents the coordinates of a joint in the hand.
         */
        private List<Vector3> parseRowToVectors(string row)
        {
            var values = row.Split(',');
            List<Vector3> vectors = new List<Vector3>();
            Vector3 current = new Vector3();
            float x = 0, y = 0, z = 0;
            for(int i=0; i< values.Length; i++)
            {
                if (i % 4 == 0)
                {
                    current = new Vector3();
                }
                else if (i % 4 == 1)
                    x = float.Parse(values[i]);
                else if (i % 4 == 2)
                    y = float.Parse(values[i]);
                else
                {
                    z = float.Parse(values[i]);
                    current.Set(x, y, z);
                    vectors.Add(current);
                }
            }
            return vectors;
        }
        private List<List<Vector3>> parseRowToFingerLines(string row)
        {
            List<Vector3> allVectors = parseRowToVectors(row);
            List<List<Vector3>> fingerLines = new List<List<Vector3>>();
            fingerLines.Add(getThumbLine(allVectors));
            fingerLines.Add(getIndexLine(allVectors));
            fingerLines.Add(getMiddleLine(allVectors));
            fingerLines.Add(getRingLine(allVectors));
            fingerLines.Add(getPinkyLine(allVectors));
            return fingerLines;
        }
        /**
         * gets a list of all the vectors, and extracts only the finger line vectors
         */
        private List<Vector3> getFingerLine(List<Vector3> vectors, int start, int end)
        {
            List<Vector3> fingerLineVectors = new List<Vector3>();
            
            for (int i = start; i < end; i++)
                fingerLineVectors.Add(vectors[i]);
            return fingerLineVectors;
        }
        /**
        * gets a list of all the vectors, and extracts only the thumb line vectors
        */
        private List<Vector3> getThumbLine(List<Vector3> vectors) => getFingerLine(vectors, 0, 3);

        /**
        * gets a list of all the vectors, and extracts only the index line vectors
        */
        private List<Vector3> getIndexLine(List<Vector3> vectors) => getFingerLine(vectors, 4, 6);

        /**
        * gets a list of all the vectors, and extracts only the middle line vectors
        */
        private List<Vector3> getMiddleLine(List<Vector3> vectors) => getFingerLine(vectors, 7, 9);
        /**
        * gets a list of all the vectors, and extracts only the middle line vectors
        */
        private List<Vector3> getRingLine(List<Vector3> vectors) => getFingerLine(vectors, 10, 12);
        /**
        * gets a list of all the vectors, and extracts only the middle line vectors
        */
        private List<Vector3> getPinkyLine(List<Vector3> vectors) => getFingerLine(vectors, 13, 16);

        private List<List<List<Vector3>>> readCSV(string csvPath)
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
