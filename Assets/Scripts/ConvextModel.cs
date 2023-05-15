using Unity.Barracuda;
using UnityEngine;


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
    }

    public void Predict(float[] inputs)
    {
        int[] inputShape = new int[] {1,6006};
        /*
        float[] randoms = new float[inputShape[1]];
        //Debug.Log(inputShape[0] + ", " + inputShape[1]);
        for(int i =0; i< inputShape[1]; i++)
        {
            float rand = Random.Range(0f, 1f);
            randoms[i] = rand;
        }
        */
        Tensor inputTensor = new Tensor(inputShape[0], inputShape[1]);
        inputTensor.data.Upload(inputs, new TensorShape(inputShape[0], inputShape[1]));
        worker.Execute(inputTensor);
        Tensor outputTensor = worker.PeekOutput(outputLayerName);
        float[] outputData = outputTensor.ToReadOnlyArray(); // Retrieve the data from outputTensor
        float[] zeros = new float[outputData.Length];
        for (int i = 0; i < outputData.Length; i++)
        {
            outputData[i] *= 360; // Multiply each element by 360
            zeros[i] = 0;
        }
        Debug.Log(outputData);
        Orientation.Orientation orientation = new Orientation.Orientation();
        // init to 0
        orientation.LoadAndTransform(zeros);
        // transform
        orientation.LoadAndTransform(outputData);
        inputTensor.Dispose(); // handle memory leaks
        outputTensor.Dispose();
        //Debug.Log(outputTensor.DataToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
