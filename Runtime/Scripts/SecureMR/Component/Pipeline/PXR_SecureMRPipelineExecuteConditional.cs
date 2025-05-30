using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR.SecureMR
{
    public class PXR_SecureMrPipelineExecuteConditional : PXR_SecureMRPipelineExecute
    {
        public PXR_SecureMRGlobalTensor globalTensor;
        
        public override void Execute()
        {
            if (TensorMappings != null && TensorMappings.Length > 0)
            {
                TensorMapping mappings = new TensorMapping();
                foreach (var tensorPair in TensorMappings)
                {
                    mappings.Set(tensorPair.localTensorReference.tensor, tensorPair.globalTensor.tensor);
                }

                Pipeline.pipeline.ExecuteConditional(globalTensor.tensor.TensorHandle, mappings);
            }
            else
            {
                Pipeline.pipeline.ExecuteConditional(globalTensor.tensor.TensorHandle);
            }
        }
    }
}

