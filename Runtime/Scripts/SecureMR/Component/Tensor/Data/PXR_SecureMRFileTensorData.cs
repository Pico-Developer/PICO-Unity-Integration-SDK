using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR.SecureMR
{
    public class PXR_SecureMRFileTensorData : PXR_SecureMRTensorData
    {
        public TextAsset fileAsset;

        public override byte[] ToByteArray()
        {
            if (fileAsset != null)
            {
                return fileAsset.bytes;
            }
            else
            {
                return null;
            }
        }
    }
}

