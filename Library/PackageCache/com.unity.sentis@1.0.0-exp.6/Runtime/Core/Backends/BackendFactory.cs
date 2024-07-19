using System;
using UnityEngine;

namespace Unity.Sentis {

static class BackendFactory
{
    public static IOps CreateOps(BackendType backendType, ITensorAllocator allocator, bool verbose)
    {
        switch (backendType)
        {
            case BackendType.GPUCompute:
                return new GPUComputeOps(allocator);
            case BackendType.GPUCommandBuffer:
                return new GPUCommandBufferOps(allocator);
            case BackendType.GPUPixel:
                return new GPUPixelOps(allocator);
            default:
                return new CPUOps(allocator);
        }
    }

    public static IWorker CreateWorker(BackendType backendType, Model model, WorkerFactory.WorkerConfiguration workerConfiguration)
    {
        if (WorkerFactory.IsType(backendType, DeviceType.GPU) && !SystemInfo.supportsComputeShaders && !Application.isEditor)
        {
            backendType = BackendType.GPUPixel;
        }

        IVars vars;
        if (backendType == BackendType.GPUPixel)
        {
            //TODO PixelShader worker uses Blit/Textures, cannot re-use vars unless the dispatch mechanism allows rendering to sub part of the texture
            vars = new GenericVarsWithReuse(forceCachingByShape: true);
        }
        else
        {
            vars = new DefaultVars();
        }

        ITensorAllocator allocator = vars.GetAllocator();

        if (workerConfiguration.verbose)
            D.Log($"Storage type: {vars.GetType()}. Allocator type: {allocator.GetType()}.");

        IOps ops = CreateOps(backendType, allocator, workerConfiguration.verbose);

        return new GenericWorker(model, ops, vars, workerConfiguration.verbose, workerConfiguration.takeoverWeights);
    }
}
} // namespace Unity.Sentis
