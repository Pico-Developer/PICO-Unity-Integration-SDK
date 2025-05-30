using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR.SecureMR
{
    public abstract class PXR_SecureMRTensorData : MonoBehaviour
    {
        public virtual byte[] ToByteArray()
        {
            return null;
        }
        
        public virtual float[] ToFloatArray()
        {
            return null;
        }
        
        public virtual int[] ToIntArray()
        {
            return null;
        }

        public virtual ushort[] ToUShortArray()
        {
            return null;
        }
    }
}

