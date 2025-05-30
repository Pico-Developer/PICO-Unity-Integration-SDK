using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR.SecureMR
{
    public class PXR_SecureMRTensorMetadata : PXR_SecureMRMetadata
    {
        public int[] shape;
        public int channel;
        public SecureMRTensorDataType dataType = SecureMRTensorDataType.Float;
        public SecureMRTensorUsage usage = SecureMRTensorUsage.Matrix;
    }
}

