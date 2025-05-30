using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR.SecureMR
{
    public class PXR_SecureMRIntTensorData : PXR_SecureMRTensorData
    {
        public int[] data;

        public override int[] ToIntArray()
        {
            return data;
        }
    }
}

