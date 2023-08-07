using Unity.Barracuda;
using UnityEngine;
using Genetic;


public class ConvextModel : MonoBehaviour
{
    [SerializeField]
    private NNModel model;
    [SerializeField]
    private Model runtimeModel;
    [SerializeField]
    Orientation.Orientation orientation;
    private string outputLayerName;
    private IWorker worker;


    // Start is called before the first frame update
    void Start()
    {
        runtimeModel = ModelLoader.Load(model);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst,runtimeModel);
        Debug.Log(string.Format("OutputLayer Shape: {0}", runtimeModel.outputs.Count));
        for (int i = 0; i < runtimeModel.layers.Count; i++)
            Debug.Log(runtimeModel.layers[i].name);
        outputLayerName = runtimeModel.outputs[runtimeModel.outputs.Count - 1];
        Debug.Log("Last Layer name: " + runtimeModel.outputs[runtimeModel.outputs.Count - 1]);
        
    }

    public void Predict(float[] inputs)
    {
        // debug
        for (int i = 0; i < runtimeModel.layers.Count; i++)
            Debug.Log(runtimeModel.layers[i].name);

        outputLayerName = runtimeModel.outputs[runtimeModel.outputs.Count - 1];

        int[] inputShape = new int[] {1,6006};

        Tensor inputTensor = new Tensor(inputShape[0], inputShape[1]);

        inputTensor.data.Upload(inputs, new TensorShape(inputShape[0], inputShape[1]));

        Tensor outputTensor = worker.ExecuteAndWaitForCompletion(inputTensor);
        // Retrieve the data from outputTensor
        float[] outputData = outputTensor.ToReadOnlyArray(); 
        // handle memory leaks
        inputTensor.Dispose(); 

        outputTensor.Dispose();

        Debug.Log(string.Format("Output shape: {0},", outputData.Length));
        
        outputData = Genetic.Manager.normalize_output(outputData);
        
       
        Debug.Log(string.Format("[{0}]", string.Join(", ", outputData)));
        
        // init to 0
        // transform
        orientation.LoadAndTransformSecondary(outputData);
        
        //Debug.Log(outputTensor.DataToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
