using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR.SecureMR
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-99)]
    public class PXR_SecureMRPipeline : MonoBehaviour
    {
        public PXR_SecureMRPipelineTensor[] tensors;
        public PXR_SecureMROperator[] operators;
        internal Pipeline pipeline;

        public void Initialize(PXR_SecureMRProvider provider)
        {
            pipeline = provider.provider.CreatePipeline();

            foreach (var tensor in tensors)
            {
                tensor.Initialize(this);
            }
            
            foreach (var secureMrOperator in operators)
            {
                secureMrOperator.InitializeOperator(this);
            }
        }

        private void Start()
        {
            foreach (var secureMrOperator in operators)
            {
                secureMrOperator.InitializeParameters();
            }
        }
    }
}

