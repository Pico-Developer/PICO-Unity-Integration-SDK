using UnityEditor;

namespace Unity.XR.PXR.SecureMR
{
    public abstract class PXR_SecureMRPipelineTensor : PXR_SecureMRTensor
    {
        public void Initialize(PXR_SecureMRPipeline pxrSecureMrPipeline)
        {
            Pipeline smrPipeline = pxrSecureMrPipeline.pipeline;
            Initialize(smrPipeline);

        }
        
        protected virtual void Initialize(Pipeline smrPipeline)
        {
            if (metadata !=null && metadata is PXR_SecureMRGltfMetadata)
            {
                tensor = smrPipeline.CreateTensor<Gltf>(null);
            }
            else if (metadata != null && metadata is PXR_SecureMRTensorMetadata tensorMetadata)
            {
                switch(tensorMetadata.usage)
                {
                    case SecureMRTensorUsage.Matrix:
                        CreateMatrixTensor(smrPipeline,tensorMetadata);
                        break;
                    case SecureMRTensorUsage.Point:
                        CreatePointTensor(smrPipeline,tensorMetadata);
                        break;
                    case SecureMRTensorUsage.Color:
                        CreateColorTensor(smrPipeline, tensorMetadata);
                        break;
                    case SecureMRTensorUsage.TimeStamp:
                        CreateTimestampTensor(smrPipeline, tensorMetadata);
                        break;
                    case SecureMRTensorUsage.Slice:
                        if(tensorMetadata.dataType == SecureMRTensorDataType.Int)
                        {
                            tensor = smrPipeline.CreateTensor<int,Slice>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape));
                        }
                        break;
                    case SecureMRTensorUsage.Scalar:
                        CreateScalarTensor(smrPipeline, tensorMetadata);
                        break;
                }            
            }
        }
        
        private void CreateScalarTensor(Pipeline smrPipeline, PXR_SecureMRTensorMetadata tensorMetadata)
        {
            switch(tensorMetadata.dataType)
            {
                case SecureMRTensorDataType.Float:
                    tensor = smrPipeline.CreateTensor<float,Scalar>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape));
                    break;
                case SecureMRTensorDataType.Int:
                case SecureMRTensorDataType.Short:
                case SecureMRTensorDataType.Ushort:
                    tensor = smrPipeline.CreateTensor<int,Scalar>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape));
                    break;
                case SecureMRTensorDataType.Byte:
                case SecureMRTensorDataType.Sbyte:
                    tensor = smrPipeline.CreateTensor<byte,Scalar>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape));
                    break;

            }
        }

        private void CreateTimestampTensor(Pipeline smrPipeline, PXR_SecureMRTensorMetadata tensorMetadata)
        {
            if(tensorMetadata.dataType == SecureMRTensorDataType.Int)
            {
                tensor = smrPipeline.CreateTensor<int,TimeStamp>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape));
            }
        }

        private void CreateColorTensor(Pipeline smrPipeline, PXR_SecureMRTensorMetadata tensorMetadata)
        {
            switch(tensorMetadata.dataType)
            {
                case SecureMRTensorDataType.Float:
                    tensor = smrPipeline.CreateTensor<float,Color>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape));
                    break;
                case SecureMRTensorDataType.Short:
                case SecureMRTensorDataType.Ushort:
                case SecureMRTensorDataType.Int:
                    tensor = smrPipeline.CreateTensor<int,Color>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape));
                    break;
                case SecureMRTensorDataType.Byte:
                    tensor = smrPipeline.CreateTensor<byte, Color>(tensorMetadata.channel,
                        new TensorShape(tensorMetadata.shape));
                    break;
            }
        }

        private void CreatePointTensor(Pipeline smrPipeline, PXR_SecureMRTensorMetadata tensorMetadata)
        {
            if(tensorMetadata.dataType == SecureMRTensorDataType.Float)
            {
                tensor = smrPipeline.CreateTensor<float,Point>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape));
            }
        }

        void CreateMatrixTensor(Pipeline smrPipeline, PXR_SecureMRTensorMetadata tensorMetadata)
        {
            switch(tensorMetadata.dataType)
            {
                case SecureMRTensorDataType.Float:
                    tensor = smrPipeline.CreateTensor<float,Matrix>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape));
                    break;
                case SecureMRTensorDataType.Int:
                case SecureMRTensorDataType.Short:
                case SecureMRTensorDataType.Ushort:
                    tensor = smrPipeline.CreateTensor<int,Matrix>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape));
                    break;
                case SecureMRTensorDataType.Byte:
                case SecureMRTensorDataType.Sbyte:
                    tensor = smrPipeline.CreateTensor<byte,Matrix>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape));
                    break;

            }

            
        }
    }
}