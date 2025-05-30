namespace Unity.XR.PXR.SecureMR
{
    public class PXR_SecureMRByteTensorData : PXR_SecureMRTensorData
    {
        public byte[] data;

        public override byte[] ToByteArray()
        {
            return data;
        }
    }
}