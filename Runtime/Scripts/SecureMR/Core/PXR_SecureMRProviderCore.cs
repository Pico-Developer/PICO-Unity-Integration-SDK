using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR.SecureMR
{
    public class Provider
    {
        private ulong providerHandle;

        public Provider(int width,int height)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMRProvider(width, height, out providerHandle);
            if (result != PxrResult.SUCCESS)
            {
                throw new InvalidOperationException("Failed to create SecureMRProvider" + result);
            }
            else
            {
                PLog.i(PXR_Plugin.SecureMR.TAG,"Create SecureMR provider success",false);
            }
        }

        /// <summary>
        /// Instantiates a new pipeline object associated with the current provider context.
        /// </summary>
        /// <returns>A new pipeline instance bound to the current provider.</returns>
        public Pipeline CreatePipeline()
        {
            return new Pipeline(providerHandle);
        }

        /// <summary>
        /// Creates a global tensor.
        /// </summary>
        /// <param name="channels">Specifies the number of channels for the global tensor.</param>
        /// <param name="shape">Specifies the shape of the global tensor.</param>
        /// <param name="data">(Optional) Specifies the data array used to initialize the global tensor, which can be null.</param>
        /// <typeparam name="T">Specifies the data type of the global tensor, which must be value type (struct).</typeparam>
        /// <typeparam name="TType">Specifies the type of the global tensor, which must inherit from `TensorBase` and has parameter-free constructor.</typeparam>
        /// <returns>A newly created global tensor instance.</returns>
        /// <exception cref="InvalidOperationException">
        /// `InvalidOperationException`: thrown when failing to create the global tensor.
        /// </exception>
        public Tensor CreateTensor<T, TType>(int channels, TensorShape shape, T[] data = null)
            where T : struct
            where TType : TensorBase, new()
        {
            PXR_Plugin.SecureMR.TensorDataTypeToEnum.TryGetValue(typeof(T), out var dataType);
            PXR_Plugin.SecureMR.TensorClassToEnum.TryGetValue(typeof(TType), out var enumValue);
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMRTensorByShape(providerHandle, dataType, shape.Dimensions, (sbyte)channels, enumValue, out var tensorHandle);
            if (result == PxrResult.SUCCESS)
            {
                if (data != null)
                {
                    result = PXR_Plugin.SecureMR.UPxr_ResetSecureMRTensor(tensorHandle, data);
                    if (result != PxrResult.SUCCESS)
                    {
                        throw new InvalidOperationException("Failed to set tensor data" + result);
                    }
                }
                return new Tensor(tensorHandle, 0, false, true);
            }
            else
            {
                throw new InvalidOperationException("Failed to create global tensor" + result);
            }
            
        }
        
        /// <summary>
        /// Creates a global glTF tensor. This is especially for processing 3D model or scene data.
        /// </summary>
        /// <param name="data">Specifies the glTF data that the global tensor is created from.</param>
        /// <typeparam name="TType">Must inherit from base class `Gltf` and has parameter-free constructor.</typeparam>
        /// <returns>A newly created global glTF tensor instance.</returns>
        /// <exception cref="InvalidOperationException">
        /// `InvalidOperationException`: thrown when failing to create a global glTF tensor.
        /// </exception>
        public Tensor CreateTensor<TType>(byte[] data)
            where TType : Gltf, new()
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMRTensorByGltf(providerHandle, data, out var tensorHandle);
            if (result == PxrResult.SUCCESS)
            {
                return new Tensor(tensorHandle, 0, false, true);
            }
            else
            {
                throw new InvalidOperationException("Failed to create global gltf tensor" + result);
            }
        }
        
        /// <summary>
        /// Destroys a SecureMR provider instance to release resources and reset the handle.
        /// </summary>
        public void Destroy()
        {
            var result = PXR_Plugin.SecureMR.UPxr_DestroySecureMRProvider(providerHandle);
            if (result == PxrResult.SUCCESS)
            {
                providerHandle = 0;
            }
            else
            {
                PLog.i(PXR_Plugin.SecureMR.TAG, "Destroy SecureMR provider failed" + result, false);
            }
        }

        ~Provider()
        {
            Destroy();
        }
    }

}

