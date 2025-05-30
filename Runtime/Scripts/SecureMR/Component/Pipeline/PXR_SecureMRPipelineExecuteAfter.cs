using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR.SecureMR
{
    public class PXR_SecureMrPipelineExecuteAfter : PXR_SecureMRPipelineExecute
    {
        public PXR_SecureMRPipelineExecute afterPipeline;
        
        public override void Execute()
        {
            if (TensorMappings != null && TensorMappings.Length > 0)
            {
                TensorMapping mappings = new TensorMapping();
                foreach (var tensorPair in TensorMappings)
                {
                    mappings.Set(tensorPair.localTensorReference.tensor, tensorPair.globalTensor.tensor);
                }

                Pipeline.pipeline.ExecuteAfter(afterPipeline.Pipeline.pipeline.pipelineHandle, mappings);
            }
            else
            {
                Pipeline.pipeline.ExecuteAfter(afterPipeline.Pipeline.pipeline.pipelineHandle);
            }
        }
    }
}

