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

        /// <summary>
        /// Creates an operator of a specified type.
        /// </summary>
        /// <typeparam name="T">Specifies the operator type. The type must inherit from the base class `Operator`.</typeparam>
        /// <returns>The newly created operator instance of type `T`.</returns>
        public T CreateOperator<T>() where T : Operator
        {
            PXR_Plugin.SecureMR.OperatorClassToEnum.TryGetValue(typeof(T), out var enumValue);
            return (T)Activator.CreateInstance(typeof(T), pipelineHandle,enumValue);
        }
        
        /// <summary>
        /// Creates an operator of a specified type. This method supports passing config parameters for the operator.
        /// </summary>
        /// <param name="configuration">The config object of the operator, which is passed to the constructor of the operator.</param>
        /// <typeparam name="T">Specifies the operator type. The type must inherit from the base class `Operator`.</typeparam>
        /// <returns>The newly created operator instance of type `T`.</returns>
        public T CreateOperator<T>(OperatorConfiguration configuration) where T : Operator
        {
            PXR_Plugin.SecureMR.OperatorClassToEnum.TryGetValue(typeof(T), out var enumValue);
            return (T)Activator.CreateInstance(typeof(T), pipelineHandle, enumValue, configuration);
        }
        
        /// <summary>
        /// Creates an initializes a tensor of a specified type and shape. 
        /// </summary>
        /// <param name="channels">Specifies the number of channels for the tensor.</param>
        /// <param name="shape">Defines the shape of the tensor.</param>
        /// <param name="data">(Optional) Used to initialize the data array of the tensor, which can be null.</param>
        /// <typeparam name="T">Specifies the data type of the tensor, which must be the value type (struct).</typeparam>
        /// <typeparam name="TType">Specifies the type of the tensor, which must inherit from `TensorBase` and has parameter-free constructor.</typeparam>
        /// <returns>A newly created tensor instance.</returns>
        /// <exception cref="InvalidOperationException">
        /// `InvalidOperationException`: thrown when creating local tensor fails.
        /// </exception>
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
        
        /// <summary>
        /// Creates and initializes a tensor from binary glTF data.
        /// </summary>
        /// <param name="data">Specifies the data array that contains gITF data.</param>
        /// <typeparam name="TType">Must inherit from base class `Gltf` and has parameter-free constructor.</typeparam>
        /// <returns>A newly created tensor instance.</returns>
        /// <exception cref="InvalidOperationException">
        /// `InvalidOperationException`: thrown when create local glTF tensor fails.
        /// </exception>
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

        /// <summary>
        /// Creates a tensor reference (instead of an individual tensor with data). This reference points to the tensor data of specified type and shape.
        /// </summary>
        /// <param name="channels">Specifies the number of channels for the tensor.</param>
        /// <param name="shape">Specifies the shape of the tensor.</param>
        /// <typeparam name="T">Specifies the data type of the tensor, which must be the value type (struct).</typeparam>
        /// <typeparam name="TType">Specifies the type of the tensor, which must inherit from `TensorBase` and has parameter-free constructor.</typeparam>
        /// <returns>A newly created tensor instance.</returns>
        /// <exception cref="InvalidOperationException">
        /// `InvalidOperationException`: thrown when creating local tensor reference fails.
        /// </exception>
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
        
        /// <summary>
        /// Creates an empty tensor reference of glTF format. This reference can be bound to existing glTF data.
        /// </summary>
        /// <typeparam name="TType">Must inherit from base class `Gltf` and has parameter-free constructor.</typeparam>
        /// <returns>A new empty tensor reference instance.</returns>
        /// <exception cref="InvalidOperationException">
        /// `InvalidOperationException`: thrown when creating local tensor reference fails.
        /// </exception>
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

        /// <summary>
        /// Creates a new empty TensorMapping object.
        /// </summary>
        /// <returns>A new empty `TensorMapping` instance.</returns>
        public TensorMapping CreateTensorMapping()
        {
            return new TensorMapping();
        }
        
        /// <summary>
        /// Destroys the current SecureMR pipeline instance and releases resources.
        /// </summary>
        public void Destroy()
        {
            var result = PXR_Plugin.SecureMR.UPxr_DestroySecureMRPipeline(pipelineHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, "Destroy SecureMR pipeline:" + result, false);
        }

        /// <summary>
        /// Executes a SecureMR pipeline.
        /// </summary>
        /// <param name="tensorMappings">(Optional) Input/output the tensor's mapping relations. Defaults to null.</param>
        /// <returns>A unique `pipelineRunHandle` that identifies this pipeline execution.</returns>
        /// <exception cref="InvalidOperationException">
        /// `InvalidOperationException`: thrown when pipeline execution fails.
        /// </exception>
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

        /// <summary>
        /// Executes a new pipeline task in order after the previous one is finished.
        /// </summary>
        /// <param name="runId">The run handle of the previous pipeline execution. The new pipeline execution begins after the previous one is finished.</param>
        /// <param name="tensorMappings">(Optional) Input/output the tensor's mapping relations. Default to null.</param>
        /// <returns>A unique `pipelineRunHandle` that identifies this pipeline execution.</returns>
        /// <exception cref="InvalidOperationException">
        /// `InvalidOperationException`: thrown when failing to execute the new pipeline.
        /// </exception>
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

        /// <summary>
        /// Executes the conditional SecureMR pipeline based on the state of the condition tensor.
        /// </summary>
        /// <param name="conditionTensorHandle">The handle of the conditional tensor, which determines whether to execute the pipeline.</param>
        /// <param name="tensorMappings">(Optional) Input/output the tensor's mapping relations. Default to null.</param>
        /// <returns>A unique `pipelineRunHandle` that identifies this pipeline execution.</returns>
        /// <exception cref="InvalidOperationException">
        /// `InvalidOperationException`: thrown when failing to execute the conditional pipeline.
        /// </exception>
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

