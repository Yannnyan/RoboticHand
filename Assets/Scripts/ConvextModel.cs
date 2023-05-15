using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Unity.Barracuda;
using System.Text;
using System.IO;
using Loader;
using Orientation;

public class ConvextModel : MonoBehaviour
{
    [SerializeField]
    private NNModel model;
    [SerializeField]

    private Model runtimeModel;
    private string outputLayerName;
    private IWorker worker;


    // Start is called before the first frame update
    void Start()
    {
        runtimeModel = ModelLoader.Load(model);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst,runtimeModel);
        outputLayerName = runtimeModel.outputs[runtimeModel.outputs.Count - 1];
        Predict();
    }

    public void Predict()
    {
        int[] inputShape = new int[] {1,6006};
        float[] randoms = new float[inputShape[1]];
        Debug.Log(inputShape[0] + ", " + inputShape[1]);
        for(int i =0; i< inputShape[1]; i++)
        {
            float rand = Random.Range(0f, 1f);
            randoms[i] = rand;
        }

        Tensor inputTensor = new Tensor(inputShape[0], inputShape[1]);
        inputTensor.data.Upload(randoms, new TensorShape(inputShape[0], inputShape[1]));
        worker.Execute(inputTensor);
        Tensor outputTensor = worker.PeekOutput(outputLayerName);
        Orientation.Orientation orientation = new Orientation.Orientation();
        Debug.Log(outputTensor.DataToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
