using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR.SecureMR
{
    public class PXR_SecureMRPipelineExecute : MonoBehaviour
    {
        public PXR_SecureMRPipeline Pipeline;
        public double periodInSeconds;
        public PXR_SecureMRTensorMapping[] TensorMappings;

        private double lastExecuted = 0.0;
        public virtual void Execute()
        {
            var timeNow = Time.realtimeSinceStartupAsDouble;
            if (timeNow < lastExecuted + periodInSeconds)
                return;
            
            if (TensorMappings !=null && TensorMappings.Length>0)
            {
                TensorMapping mappings = new TensorMapping();
                foreach (var tensorPair in TensorMappings)
                {
                    mappings.Set(tensorPair.localTensorReference.tensor,tensorPair.globalTensor.tensor);
                }
                Pipeline.pipeline.Execute(mappings);
            }
            else
            {
                Pipeline.pipeline.Execute();
            }

            lastExecuted = timeNow;
        }
    }
}

