using UnityEngine;
using Unity.Sentis;
//using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using Unity.Sentis.Layers;
using UnityEngine.UI;
using TMPro;

public class ClassifyHandwrittenDigit : MonoBehaviour
{

    //public Texture2D inputTexture;
    public ModelAsset modelAsset;
    Tensor inputTensor;
    Model runtimeModel;
    IWorker worker;
    public float[] results;
    public Texture2D m_InputTextureDigit;

    void Start()
    {
        // Create the runtime model
        runtimeModel = ModelLoader.Load(modelAsset);
    }

    public int ClassifyNumber(Texture2D inputTexture)
    {
        // Create input data as a tensor
        inputTensor = TextureConverter.ToTensor(inputTexture);
        //inputTensor = TextureConverter.ToTensor(m_InputTextureDigit);

        //Debug tensor inputs and epected inputs
        ShowTensorInputAndExpectedInput();

        // Create an engine
        worker = WorkerFactory.CreateWorker(BackendType.GPUCompute, runtimeModel, true);

        // Run the model with the input data
        worker.Execute(inputTensor);
        // Get the result
        TensorFloat outputTensor = worker.PeekOutput() as TensorFloat;
        results = outputTensor.ToReadOnlyArray();

        float probab = 0;
        int num = 0;
        for (int i = 0; i < results.Length; i++)
        {
            if (results[i] > probab)
            {
                probab = results[i];
                num = i;
            }
        }

        Debug.Log("Max:" + probab + "num is:" + num);
        return num;
    }

    public void ShowTensorInputAndExpectedInput()
    {
        Debug.Log("***Input tensor shape: " + inputTensor.shape);

        // Iterate through the layers in the model
        foreach (Layer layer in runtimeModel.layers)
        {
            // Access layer properties, such as name, inputs, and outputs
            string layerName = layer.name;
            string[] inputNames = layer.inputs;
            string[] outputNames = layer.outputs;

            if (outputNames == null)
                Debug.Log("***outputNames is NULL");
            else
                Debug.Log("***outputNames is NOT NULL");

            Debug.Log("***Layer:" + layerName + " has sizeInputNames:" + inputNames.Length);
/*
            foreach (string i in inputNames)
            {
                Debug.Log("***inputName:" + i);
            }*/
        }
    }

    void OnDisable()
    {
        // Tell the GPU we're finished with the memory the engine used 
        worker.Dispose();
    }
}
