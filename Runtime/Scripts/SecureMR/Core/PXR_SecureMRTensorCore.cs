using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR.SecureMR
{
    public class Tensor
    {
        public ulong TensorHandle { get; private set; }
        public ulong PipelineHandle { get; private set; }

        public bool PlaceHolder { get; private set; }
        public bool IsGlobalTensor { get; private set; }

        public Tensor(ulong tensorHandle,ulong pipelineHandle,bool placeHolder,bool isGlobalTensor)
        {
            this.TensorHandle = tensorHandle;
            this.PipelineHandle = pipelineHandle;
            this.PlaceHolder = placeHolder;
            this.IsGlobalTensor = isGlobalTensor;
        }

        /// <summary>
        /// Resets a tensor. You can use this method to reset both global and local tensor data.
        /// </summary>
        /// <param name="tensorData">Specifies the data array for resetting the tensor.</param>
        /// <typeparam name="T">Specifies the data type of the tensor, which must be value type (struct).</typeparam>
        public void Reset<T>(T[] tensorData)
        {
            if (IsGlobalTensor)
            {
                var result = PXR_Plugin.SecureMR.UPxr_ResetSecureMRTensor(TensorHandle, tensorData);
                PLog.i(PXR_Plugin.SecureMR.TAG, "Reset global tensor data" + result, false);
            }
            else
            {
                var result = PXR_Plugin.SecureMR.UPxr_ResetSecureMRPipelineTensor(PipelineHandle, TensorHandle, tensorData);
                PLog.i(PXR_Plugin.SecureMR.TAG, "Reset local tensor data" + result, false);
            }
        }

        /// <summary>
        /// Destroys a global tensor.
        /// </summary>
        public void Destroy()
        {
            if (IsGlobalTensor)
            {
                var result = PXR_Plugin.SecureMR.UPxr_DestroySecureMRTensor(TensorHandle);
                PLog.i(PXR_Plugin.SecureMR.TAG, "Destroy global tensor" + result, false);
            }
        }
    }

    public abstract class TensorBase{}
    public class Color : TensorBase{}

    public class Gltf { }
    public class Matrix : TensorBase{}
    public class Point : TensorBase{}
    public class Scalar : TensorBase{}
    public class Slice : TensorBase{}
    public class TimeStamp : TensorBase { }
    
    public class TensorShape
    {
        public int[] Dimensions { get; }

        public TensorShape(params int[] dimensions)
        {
            if (dimensions == null || dimensions.Length == 0)
            {
                throw new ArgumentException("Dimensions array cannot be null or empty.");
            }

            Dimensions = dimensions;
        }
    }

    public class TensorMapping
    {
        public Dictionary<ulong, ulong> TensorMappings { get; private set; }

        public TensorMapping()
        {
            TensorMappings = new Dictionary<ulong, ulong>();
        }

        /// <summary>
        /// Creates the mapping between local tensor references and global tensors. The mapping is saved to the TensorMapping dictionary.
        /// </summary>
        /// <param name="localTensorReference">Specifies the local tensor reference object.</param>
        /// <param name="globalTensor">Specifies the global tensor object.</param>
        public void Set(Tensor localTensorReference, Tensor globalTensor)
        {
            TensorMappings.TryAdd(localTensorReference.TensorHandle, globalTensor.TensorHandle);
        }
    }
}


