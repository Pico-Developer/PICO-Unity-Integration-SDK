#if !PICO_OPENXR_SDK
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

        public Pipeline CreatePipeline()
        {
            return new Pipeline(providerHandle);
        }

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
#endif
