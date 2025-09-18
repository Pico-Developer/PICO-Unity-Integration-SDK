#if !PICO_OPENXR_SDK
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR.SecureMR
{
    public class Pipeline
    {
        public ulong pipelineHandle;

        internal Pipeline(ulong frameworkHandle)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMRPipeline(frameworkHandle, out pipelineHandle);

            if (result != PxrResult.SUCCESS)
            {
                throw new InvalidOperationException("Failed to create SecureMR pipeline" + result);
            }
            else
            {
                PLog.i(PXR_Plugin.SecureMR.TAG, "Create SecureMR pipeline success", false);
            }
        }

        public T CreateOperator<T>() where T : Operator
        {
            PXR_Plugin.SecureMR.OperatorClassToEnum.TryGetValue(typeof(T), out var enumValue);
            return (T)Activator.CreateInstance(typeof(T), pipelineHandle,enumValue);
        }


        public T CreateOperator<T>(OperatorConfiguration configuration) where T : Operator
        {
            PXR_Plugin.SecureMR.OperatorClassToEnum.TryGetValue(typeof(T), out var enumValue);
            return (T)Activator.CreateInstance(typeof(T), pipelineHandle, enumValue, configuration);
        }


        public Tensor CreateTensor<T, TType>(int channels, TensorShape shape, T[] data = null)
            where T : struct
            where TType : TensorBase, new()
        {
            PXR_Plugin.SecureMR.TensorDataTypeToEnum.TryGetValue(typeof(T), out var dataType);
            PXR_Plugin.SecureMR.TensorClassToEnum.TryGetValue(typeof(TType), out var enumValue);
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMRPipelineTensorByShape(pipelineHandle, false, dataType, shape.Dimensions, (sbyte)channels, enumValue, out var tensorHandle);
            if (result == PxrResult.SUCCESS)
            {
                if (data != null)
                {
                    result = PXR_Plugin.SecureMR.UPxr_ResetSecureMRPipelineTensor(pipelineHandle, tensorHandle, data);
                    if (result != PxrResult.SUCCESS)
                    {
                        throw new InvalidOperationException("Failed to set tensor data:" + result);
                    }
                }
                return new Tensor(tensorHandle, pipelineHandle, false, false);
            }
            else
            {
                throw new InvalidOperationException("Failed to create local tensor:" + result);
            }
        }
        
        public Tensor CreateTensor<TType>(byte[] data)
            where TType : Gltf, new()
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMRPipelineTensorByGltf(pipelineHandle, false, data, out var tensorHandle);
            if (result == PxrResult.SUCCESS)
            {
                if (data != null)
                {
                    result = PXR_Plugin.SecureMR.UPxr_ResetSecureMRPipelineTensor(pipelineHandle, tensorHandle, data);
                    if (result != PxrResult.SUCCESS)
                    {
                        throw new InvalidOperationException("Failed to set tensor data:" + result);
                    }
                }
                return new Tensor(tensorHandle, pipelineHandle, false, false);
            }
            else
            {
                throw new InvalidOperationException("Failed to create local gltf tensor:" + result);
            }
        }

        public Tensor CreateTensorReference<T, TType>(int channels, TensorShape shape)
            where T : struct
            where TType : TensorBase, new()
        {
            PXR_Plugin.SecureMR.TensorDataTypeToEnum.TryGetValue(typeof(T), out var dataType);
            PXR_Plugin.SecureMR.TensorClassToEnum.TryGetValue(typeof(TType), out var enumValue);
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMRPipelineTensorByShape(pipelineHandle, true, dataType, shape.Dimensions, (sbyte)channels, enumValue, out var tensorHandle);
            if (result == PxrResult.SUCCESS)
            {
                return new Tensor(tensorHandle, pipelineHandle, true, false);
            }
            else
            {
                throw new InvalidOperationException("Failed to create local tensor reference:" + result);
            }
        }
        
        public Tensor CreateTensorReference<TType>()
            where TType : Gltf, new()
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMRPipelineTensorByGltf(pipelineHandle, true, null, out var tensorHandle);
            if (result == PxrResult.SUCCESS)
            {
                return new Tensor(tensorHandle, pipelineHandle, true, false);
            }
            else
            {
                throw new InvalidOperationException("Failed to create local tensor reference:" + result);
            }
        }

        public TensorMapping CreateTensorMapping()
        {
            return new TensorMapping();
        }
        
        public void Destroy()
        {
            var result = PXR_Plugin.SecureMR.UPxr_DestroySecureMRPipeline(pipelineHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, "Destroy SecureMR pipeline:" + result, false);
        }

        public ulong Execute(TensorMapping tensorMappings = null)
        {
            PxrResult result;
            ulong pipelineRunHandle;
            if (tensorMappings != null)
            {
                result = PXR_Plugin.SecureMR.UPxr_ExecuteSecureMRPipeline(pipelineHandle, tensorMappings.TensorMappings, out pipelineRunHandle);
                
            }
            else
            {
                result = PXR_Plugin.SecureMR.UPxr_ExecuteSecureMRPipeline(pipelineHandle, null, out pipelineRunHandle);
            }
            if (result == PxrResult.SUCCESS)
            {
                return pipelineRunHandle;
            }
            else
            {
                throw new InvalidOperationException("Failed to execute pipeline:" + result);
            }
        }

        public ulong ExecuteAfter(ulong runId, TensorMapping tensorMappings = null)
        {
            PxrResult result;
            ulong pipelineRunHandle;

            if (tensorMappings != null)
            {
                result = PXR_Plugin.SecureMR.UPxr_ExecuteSecureMRPipelineAfter(pipelineHandle, runId, tensorMappings.TensorMappings, out pipelineRunHandle);
            }
            else
            {
                result = PXR_Plugin.SecureMR.UPxr_ExecuteSecureMRPipelineAfter(pipelineHandle, runId, null, out pipelineRunHandle);
            }
            if (result == PxrResult.SUCCESS)
            {
                return pipelineRunHandle;
            }
            else
            {
                throw new InvalidOperationException("Failed to execute after pipeline:" + result);
            }
        }

        public ulong ExecuteConditional(ulong conditionTensorHandle, TensorMapping tensorMappings = null)
        {
            PxrResult result;
            ulong pipelineRunHandle;

            if (tensorMappings != null)
            {
                result = PXR_Plugin.SecureMR.UPxr_ExecuteSecureMRPipelineConditional(pipelineHandle, conditionTensorHandle, tensorMappings.TensorMappings, out pipelineRunHandle);
            }
            else
            {
                result = PXR_Plugin.SecureMR.UPxr_ExecuteSecureMRPipelineConditional(pipelineHandle, conditionTensorHandle, null, out pipelineRunHandle);
            }
            if (result == PxrResult.SUCCESS)
            {
                return pipelineRunHandle;
            }
            else
            {
                throw new InvalidOperationException("Failed to execute conditional pipeline:" + result);
            }
        }
    }
}

#endif