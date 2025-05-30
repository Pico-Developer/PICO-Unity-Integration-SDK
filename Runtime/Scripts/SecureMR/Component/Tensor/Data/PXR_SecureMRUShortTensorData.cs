namespace Unity.XR.PXR.SecureMR
{
    public class PXR_SecureMRUShortTensorData : PXR_SecureMRTensorData
    {
        public ushort[] data;

        public override ushort[] ToUShortArray()
        {
            return data;
        }
    }
}