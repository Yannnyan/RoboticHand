using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Unity.Barracuda;
using System.Text;
using System.IO;

namespace RoboModel
{
    public class RoboticHandModel
    {

        private string inputImagePath;

        private Model runtimeModel;

        private IWorker worker;

        private Texture2D texture;

        // thisi s the shape of the tensor: ( 1, 224, 224, 3 )
        public RoboticHandModel(NNModel onnxModel, Texture2D texture)
        {
            runtimeModel = ModelLoader.Load(onnxModel);
            worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, runtimeModel);
            this.texture = texture;

            //var input = new Tensor(1, 224, 224, 3);

            //Debug.Log(outstring.ToString());
        }

        public float[] Predict(string inputImagePath)
        {
            //var input = new Tensor(1,224,224,3);

            //var url1 = new WWW(@"C:\Users\Yan\Desktop\Tiktik\Testonnx\Testonnx\Input1.png");
            //var _texture = new Texture2D(224, 224, TextureFormat.DXT5, false);
            //try
            //{
            //    url1.LoadImageIntoTexture(_texture);
            //    _texture.wrapMode = TextureWrapMode.Clamp;
            //}
            //catch 
            //{
            //    Debug.Log("hi");
            //}
            //var texture = new Texture2D(224, 224, TextureFormat.ARGB32, false);

            byte[] b = File.ReadAllBytes(@"C:\Users\Yan\Desktop\Tiktik\Testonnx\Testonnx\Input1.png");
            Texture2D texture = new Texture2D(224, 224, TextureFormat.RGBA32, false);
            
            bool worked = ImageConversion.LoadImage(texture, b, false);
            Debug.Log("worked? : " + worked.ToString());
            texture.Reinitialize(224, 224, TextureFormat.RGBA32, false);
            Color[] colors = texture.GetPixels();
            
            //float[] result = new float[224 * 224 * 3];
            Tensor input = new Tensor(texture, 3);
            
            for (int i = 0; i < 224*224 * 3; i++)
            {
                //input[i] = ((float)b[i] / 256);
                //input[i] = 2;
                Debug.Log(colors[i]);
                //Debug.Log(input[i]);
            }
            //texture.LoadImage(b);
            //texture.Reinitialize(224, 224);
            //for(int i=0; i< 224; i++)
            //{
            //    for (int j = 0; j < 224; j++)
            //        Debug.Log(", " + texture.GetPixel(i,j));
            //}
            
            var channel_count = 3;
            //TextAsset image = Resources.Load("Input1") as TextAsset;
            //Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            //texture.LoadImage(b);
            //texture.Reinitialize(224, 224);
            //Tensor input = new Tensor(texture, channel_count);
            
            
            
            var shape = input.shape;
            Debug.Log(shape + " or " + shape.batch + shape.height + shape.width + shape.channels);
            var output = worker.Execute(input).PeekOutput();
            float[] arr = output.ToReadOnlyArray();
            input.Dispose();
            output.Dispose();
            StringBuilder outstring = new StringBuilder("");
            for (int i = 0; i < arr.Length; i++)
            {
                outstring.Append($"{i}: {arr[i]}, ");
            }
            return arr;
        }

    }

}
