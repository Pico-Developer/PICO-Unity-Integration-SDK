using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR.SecureMR
{
    [DefaultExecutionOrder(-100)]
    public class PXR_SecureMRProvider : MonoBehaviour
    {
        internal Provider provider;
        public int vstImageWidth = 1024;
        public int vstImageHeight = 1024;

        public PXR_SecureMRGlobalTensor[] globalTensors;
        public PXR_SecureMRPipeline[] pipelines;
        
        void Awake()
        {
            provider = new Provider(vstImageWidth, vstImageHeight);

            foreach (var globalTensor in globalTensors)
            {
                globalTensor.Initialize(this.provider);
            }

            foreach (var pxrSecureMrPipeline in pipelines)
            {
                pxrSecureMrPipeline.Initialize(this);
            }
        }
    }
}

