using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace OfflineValidation
{
    public class TextChanger : MonoBehaviour
    {
        public TextMeshPro curTextMSE;
        public TextMeshPro curTextR2;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void setR2(string txt)
        {
            curTextR2.text = "R2: " + txt;
        }
        public void setMSE(string txt)
        {
            curTextMSE.text = "MSE: " + txt;
        }

    }

}
