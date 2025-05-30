using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR.SecureMR
{
    public class PXR_SecureMRLocalTensor : PXR_SecureMRPipelineTensor
    {
        public PXR_SecureMRTensorData tensorData;

        protected override void Initialize(Pipeline smrPipeline)
        {
            base.Initialize(smrPipeline);

            if (tensor == null || tensorData == null)
            {
                return;
            }
            
            if (metadata !=null && metadata is PXR_SecureMRGltfMetadata  )
            {
                tensor.Reset(tensorData.ToByteArray());
            }
            else if (metadata != null && metadata is PXR_SecureMRTensorMetadata tensorMetadata )
            {
                switch(tensorMetadata.dataType)
                {
                    case SecureMRTensorDataType.Float:
                        tensor.Reset(tensorData.ToFloatArray());
                        break;
                    case SecureMRTensorDataType.Int:
                        tensor.Reset(tensorData.ToIntArray());
                        break;
                    case SecureMRTensorDataType.Short:
                    case SecureMRTensorDataType.Ushort:
                        tensor.Reset(tensorData.ToUShortArray());
                        break;
                    case SecureMRTensorDataType.Byte:
                    case SecureMRTensorDataType.Sbyte:
                        tensor.Reset(tensorData.ToByteArray());
                        break;

                }
            }
        }
    }
}

