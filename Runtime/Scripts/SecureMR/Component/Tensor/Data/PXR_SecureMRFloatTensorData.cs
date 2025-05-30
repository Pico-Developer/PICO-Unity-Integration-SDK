using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR.SecureMR
{
    public class PXR_SecureMRFloatTensorData : PXR_SecureMRTensorData
    {
        public float[] data;

        public override float[] ToFloatArray()
        {
            return data;
        }
    }
}

