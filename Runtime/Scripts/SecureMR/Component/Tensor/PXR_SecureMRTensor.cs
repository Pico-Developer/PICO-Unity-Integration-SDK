using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR.SecureMR
{
    public abstract class PXR_SecureMRTensor : MonoBehaviour
    {
        internal Tensor tensor;
        public PXR_SecureMRMetadata metadata;
    }
}