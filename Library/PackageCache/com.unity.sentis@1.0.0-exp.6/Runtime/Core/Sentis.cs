using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; // CustomYieldInstruction
using UnityEngine.Assertions;
using UnityEngine.Rendering;

namespace Unity.Sentis {

/// <summary>
/// Types of devices that Sentis uses to execute a neural network.
/// </summary>
public enum DeviceType
{
    /// <summary>
    /// Executes using the GPU.
    /// </summary>
    GPU = 1 << 8,

    /// <summary>
    /// Executes using the CPU.
    /// </summary>
    CPU = 1 << 9,
}

/// <summary>
/// Types of backend that Sentis uses to execute a neural network.
/// </summary>
public enum BackendType
{
    /// <summary>
    /// Executes using compute shaders on the GPU.
    /// </summary>
    GPUCompute = 0 | DeviceType.GPU,

    /// <summary>
    /// CommandBuffer implementation
    /// </summary>
    GPUCommandBuffer = 1 | DeviceType.GPU,

    /// <summary>
    /// Executes using pixel shaders on the GPU.
    /// </summary>
    GPUPixel = 2 | DeviceType.GPU,

    /// <summary>
    /// Executes using Burst on the CPU.
    /// </summary>
    CPU = 0 | DeviceType.CPU,
}

/// <summary>
/// An interface that allows you to execute neural networks (models).
///
/// `IWorker` abstracts implementation details on different hardware devices such as the CPU and the GPU. `IWorker` lets you do the following:
///
/// - Specify inputs.
/// - Schedule the work.
/// - Get outputs.
///
/// Internally, `IWorker` translates the neural network from a <see cref="Model"/> into a set of operations, then sends the operations to the hardware device for asynchronous execution.
///
/// Use `WorkerFactory.CreateWorker` or `Model.CreateWorker` to create a new instance of a worker.
/// </summary>
public interface IWorker : IDisposable
{
    /// <summary>
    /// Prepares the worker to execute the model using inputs of given shapes.
    /// </summary>
    /// <param name="inputShapes">A dictionary mapping input names to tensor shapes.</param>
    void PrepareForInput(IDictionary<string, TensorShape> inputShapes);

    /// <summary>
    /// Sets a tensor as the default input of the model. For models with more than one input this sets the first input.
    /// </summary>
    /// <param name="inputTensor">The tensor to set to the default input of the model.</param>
    void SetInput(Tensor inputTensor);

    /// <summary>
    /// Sets a tensor as a named input of the model.
    /// </summary>
    /// <param name="name">The name of the input to set.</param>
    /// <param name="inputTensor">The tensor to set as the input.</param>
    void SetInput(string name, Tensor inputTensor);

    /// <summary>
    /// Schedules the execution of the model on the worker. This is non-blocking.
    /// </summary>
    IWorker Execute();

    /// <summary>
    /// Sets a tensor as the default input of the model and schedules the execution of the model on the worker. This is non-blocking. For models with more than one input this sets the first input.
    /// </summary>
    /// <param name="inputTensor">The tensor to set to the default input of the model.</param>
    IWorker Execute(Tensor inputTensor);

    /// <summary>
    /// Sets multiple tensors as the inputs of the model and schedules execution of the model. This is non-blocking.
    /// </summary>
    /// <param name="inputTensors">The tensors to use as the inputs of the model as a dictionary mapping input names to tensors.</param>
    IWorker Execute(IDictionary<string, Tensor> inputTensors);

    /// <summary>
    /// Schedules the execution of the model one layer at a time. This is non-blocking.
    ///
    /// To schedule the execution of the next layer of the model, call `MoveNext` on the `IEnumerator` object this method returns.
    /// </summary>
    IEnumerator StartManualSchedule();

    /// <summary>
    /// Sets a tensor as the default input of the model and schedules execution of the model one layer at a time. This is non-blocking. For models with more than one input this sets the first input.
    ///
    /// To schedule execution of the next layer of the model, call `MoveNext` on the `IEnumerator` object this method returns.
    /// </summary>
    /// <param name="inputTensor">The tensor to set to the default input of the model.</param>
    IEnumerator StartManualSchedule(Tensor inputTensor);

    /// <summary>
    /// Sets multiple tensors as the inputs of the model and schedules execution of the model one layer at a time. This is non-blocking.
    ///
    /// To schedule execution of the next layer of the model, call `MoveNext` on the `IEnumerator` object this method returns.
    /// </summary>
    /// <param name="inputTensors">The tensors to use as the inputs of the model as a dictionary mapping input names to tensors.</param>
    IEnumerator StartManualSchedule(IDictionary<string, Tensor> inputTensors);

    /// <summary>
    /// Schedules the execution of the part of the model that hasn't been scheduled yet. This is non-blocking.
    /// </summary>
    /// <param name="blocking">When the value is `true`, the method blocks further code until the model finishes executing.</param>
    void FlushSchedule(bool blocking = false);

    /// <summary>
    /// Returns the proportion of the model scheduled for execution since the last call to `StartManualSchedule`.
    ///
    /// Returns 0.0 after you call `StartManualSchedule`. Returns 1.0 when the model is fully scheduled.
    ///
    /// The value increases each time you iterate on the `IEnumerator` that `StartManualSchedule` returns.
    /// </summary>
    float scheduleProgress { get; }

    /// <summary>
    /// Returns a reference to the default output tensor. This is non-blocking.
    ///
    /// For models with more than one output this returns a reference to the first output tensor.
    ///
    /// The reference is valid only until you call `Execute()` or `Dispose()` on the worker.
    ///
    /// If you want to dispose of the worker but keep the tensor, use `CopyOutput()` instead, or use `TakeOwnership()` on the output tensor.
    /// </summary>
    /// <param name="prepareCacheForAccess">Schedules a non-blocking download from the GPU to the CPU.</param>
    Tensor PeekOutput(bool prepareCacheForAccess = false);

    /// <summary>
    /// Returns a reference to an output tensor with a given `name`. This is non-blocking.
    ///
    /// The reference is valid only until you call `Execute()` or `Dispose()` on the worker.
    ///
    /// If you want to dispose of the worker but keep the tensor, use `CopyOutput()` instead, or use `TakeOwnership()` on the output tensor.
    /// </summary>
    /// <param name="name">The name of the output tensor to peek.</param>
    /// <param name="prepareCacheForAccess">Whether to schedule a non-blocking download to the CPU.</param>
    Tensor PeekOutput(string name, bool prepareCacheForAccess = false);

    /// <summary>
    /// Returns a summary of the execution of the model.
    /// </summary>
    string Summary();

    IOps GetOps();
}

/// <summary>
/// Provides extension methods for the `IWorker` interface.
/// </summary>
public static class WorkerExtensions
{
    // @TODO: add optional targetDevice argument of type Device
    /// <summary>
    /// Returns a CPU copy of the first output tensor. This is a blocking method, so the rest of your code waits until the model fully executes.
    /// </summary>
    public static Tensor CopyOutput(this IWorker worker)
    {
        // @TODO: implement as PeekOutput()+DeepCopy() instead of Unpin()+TakeOwnership()
        var output = worker.PeekOutput();
        output.DetachFromDevice(); // detach will readback to CPU and
                                   // give allocator a chance to reuse allocated buffer
        output.TakeOwnership();
        return output;
    }

    // @TODO: add optional targetDevice argument of type Device
    /// <summary>
    /// Returns a CPU copy of a given output tensor `name`. This is a blocking method, so the rest of your code waits until the model fully executes.
    /// </summary>
    public static Tensor CopyOutput(this IWorker worker, string name)
    {
        // @TODO: implement as PeekOutput()+DeepCopy() instead of Unpin()+TakeOwnership()
        var output = worker.PeekOutput(name);
        output.DetachFromDevice(); // detach will readback to CPU and
                                   // give allocator a chance to reuse allocated buffer
        output.TakeOwnership();
        return output;
    }

    /// <summary>
    /// Non-blocking API that schedules network execution on CommandBuffer in one go.
    /// </summary>
    /// <param name="cb">CommandBuffer</param>
    /// <param name="worker">IWorker</param>
    /// <param name="inputs">input Tensors</param>
    public static void ExecuteWorker(this CommandBuffer cb, IWorker worker, Dictionary<string, Tensor> inputs)
    {
        var ops = worker.GetOps();
        Assert.IsTrue(ops is GPUCommandBufferOps);
        (ops as GPUCommandBufferOps).cb = cb;
        worker.Execute(inputs);
    }

    /// <summary>
    /// Non-blocking API that schedules network execution on CommandBuffer in one go.
    /// </summary>
    /// <param name="cb">CommandBuffer</param>
    /// <param name="worker">IWorker</param>
    /// <param name="input">Input Tensor</param>
    public static void ExecuteWorker(this CommandBuffer cb, IWorker worker, Tensor input)
    {
        var ops = worker.GetOps();
        Assert.IsTrue(ops is GPUCommandBufferOps);
        (ops as GPUCommandBufferOps).cb = cb;
        worker.Execute(input);
    }
}

/// <summary>
/// Provides methods for instantiating workers and ops on given back ends.
/// </summary>
public class WorkerFactory
{
    /// <summary>
    /// Represents the configuration for a `WorkerFactory`.
    /// </summary>
    public struct WorkerConfiguration
    {
        /// <summary>
        /// Whether to log debug information about model execution to the Console window. The default is `false`.
        /// </summary>
        public bool verbose;

        /// <summary>
        /// If true the worker is allowed to take ownership of the weights memory from the model
        /// this is useful so worker to limit memory pressure when the worker need to copy those
        /// weight to a different device.
        /// </summary>
        public bool takeoverWeights;

        /// <summary>
        /// Initializes and returns an instance of `WorkerConfiguration`.
        /// </summary>
        public WorkerConfiguration(bool verbose = false, bool takeoverWeights = false)
        {
            this.verbose = verbose;
            this.takeoverWeights = takeoverWeights;
        }
    }

    /// <summary>
    /// Initializes and returns an instance of `IOps` on a given back end.
    /// </summary>
    public static IOps CreateOps(BackendType backendType, ITensorAllocator allocator)
    {
        return BackendFactory.CreateOps(backendType, allocator, false);
    }

    /// <summary>
    /// Initializes and returns an instance of `IWorker` on a given back end with a `model` to execute and `workerConfiguration`.
    /// </summary>
    public static IWorker CreateWorker(BackendType backendType, Model model, WorkerConfiguration workerConfiguration)
    {
        return BackendFactory.CreateWorker(backendType, model, workerConfiguration);
    }

    /// <summary>
    /// Initializes and returns an instance of `IWorker` on a given back end with a `model` to execute.
    /// </summary>
    public static IWorker CreateWorker(BackendType backendType, Model model, bool verbose = false)
    {
        var workerConfiguration = new WorkerConfiguration(verbose);
        return CreateWorker(backendType, model, workerConfiguration);
    }

    /// <summary>
    /// Initializes and returns an instance of `IWorker` on a given device with a `model` to execute. Sentis selects the best backend type available for `deviceType`.
    /// </summary>
    public static IWorker CreateWorker(Model model, DeviceType deviceType, bool verbose = false)
    {
        var type = GetBestTypeForDevice(deviceType);
        var workerConfiguration = new WorkerConfiguration(verbose);
        return CreateWorker(type, model, workerConfiguration);
    }

    /// <summary>
    /// Checks if a backend is a given `backendType`. For example, `IsType(Type.ComputeShader, DeviceType.GPU)` returns `true`.
    /// </summary>
    public static bool IsType(BackendType backendType, DeviceType deviceType)
    {
        return ((int)backendType & (int)deviceType) == (int)deviceType;
    }

    /// <summary>
    /// Returns the best backend type for the given `deviceType`.
    /// </summary>
    public static BackendType GetBestTypeForDevice(DeviceType deviceType)
    {
        switch (deviceType)
        {
            case DeviceType.GPU:
                if (SystemInfo.supportsComputeShaders && ComputeInfo.supportsCompute)
                {
                    return BackendType.GPUCompute;
                }
                else
                {
                    return BackendType.GPUPixel;
                }
            default:
                return BackendType.CPU;
        }
    }
}

/// <summary>
/// Provides methods for suspending the execution of coroutines until the worker completes execution on a device,
/// and Sentis downloads the contents of the specified tensor to the main CPU memory.
///
/// You usually shouldn't use `WaitForCompletion`, unless you access tensor contents on the CPU.
///
/// You can only use `WaitForCompletion` with a `yield` statement in coroutines.
/// </summary>
public class WaitForCompletion : CustomYieldInstruction
{
    private Tensor m_Tensor;

    /// <summary>
    /// Checks if the results of model execution are ready. Returns `true` when the results are ready.
    /// </summary>
    public override bool keepWaiting
    {
        get
        {
            bool cpuCacheIsReady = m_Tensor.PrepareCacheForAccess(blocking:false);
            return !cpuCacheIsReady;
        }
    }

    /// <summary>
    /// Suspends the execution of coroutines until the worker completes execution on a device,
    /// and Sentis downloads the contents of the specified tensor to the main CPU memory.
    /// </summary>
    /// <param name="tensor">`Tensor` that will be downloaded once worker execution is finished</param>
    public WaitForCompletion(Tensor tensor)
    {
        m_Tensor = tensor;
    }
}

/// <summary>
/// Represents extensions for the `Model` class.
/// </summary>
public static class ModelExtensions
{
    /// <summary>
    /// Initializes and returns an instance of `IWorker` with a `model` to execute. Sentis selects the best backend type available for `deviceType`.
    ///
    /// This is a convenience method that internally calls `ModelLoader.Load` followed by `WorkerFactory.CreateWorker`.
    /// </summary>
    /// <param name="model">The model to execute.</param>
    /// <param name="deviceType">The preferred device for execution. For example `DeviceType.GPU` specifies the fast GPU path.</param>
    /// <param name="verbose">Whether to log scheduling of layers execution to the console.</param>
    public static IWorker CreateWorker(this Model model, DeviceType deviceType, bool verbose = false)
    {
        return WorkerFactory.CreateWorker(model, deviceType, verbose);
    }
}

/// <summary>
/// Represents extensions for the `ModelAsset` class.
/// </summary>
public static class ModelAssetExtensions
{
    /// <summary>
    /// Initializes and returns an instance of `IWorker` with a `modelAsset` to execute. Sentis selects the best backend type available for `device`.
    ///
    /// This is a convenience method that internally calls `ModelLoader.Load` followed by `WorkerFactory.CreateWorker`.
    /// </summary>
    /// <param name="modelAsset">The model asset to execute.</param>
    /// <param name="deviceType">The preferred device for execution. For example `DeviceType.GPU` specifies the fast GPU path</param>
    /// <param name="verbose">Whether to log scheduling of layers execution to the console.</param>
    /// <returns>Worker instance</returns>
    public static IWorker CreateWorker(this ModelAsset modelAsset, DeviceType deviceType, bool verbose = false)
    {
        var model = ModelLoader.Load(modelAsset);
        return model.CreateWorker(deviceType, verbose);
    }
}

} // namespace Unity.Sentis
