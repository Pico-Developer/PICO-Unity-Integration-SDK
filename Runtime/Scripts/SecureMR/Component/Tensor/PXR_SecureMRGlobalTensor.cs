using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

namespace Unity.XR.PXR.SecureMR
{
    public class PXR_SecureMRGlobalTensor : PXR_SecureMRTensor
    {
        public PXR_SecureMRTensorData tensorData;
        
        public void Initialize(Provider secureMrProvider)
        {
            if (metadata !=null && metadata is PXR_SecureMRGltfMetadata && tensorData != null  && tensorData.ToByteArray() != null)
            {
                 tensor = secureMrProvider.CreateTensor<Gltf>(tensorData.ToByteArray());
            }
            else if (metadata != null && metadata is PXR_SecureMRTensorMetadata tensorMetadata)
            {
                switch(tensorMetadata.usage)
                {
                    case SecureMRTensorUsage.Matrix:
                            CreateMatrixTensor(secureMrProvider, tensorMetadata);
                        break;
                    case SecureMRTensorUsage.Point:
                            CreatePointTensor(secureMrProvider, tensorMetadata);
                         break;
                    case SecureMRTensorUsage.Color:
                            CreateColorTensor(secureMrProvider, tensorMetadata);
                        break;
                    case SecureMRTensorUsage.TimeStamp:
                            CreateTimestampTensor(secureMrProvider, tensorMetadata);
                        break;
                    case SecureMRTensorUsage.Slice:
                        if(tensorMetadata.dataType == SecureMRTensorDataType.Int)
                        {
                            tensor = secureMrProvider.CreateTensor<int,Slice>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape), tensorData.ToIntArray());
                        }
                        break;
                    case SecureMRTensorUsage.Scalar:
                            CreateScalarTensor(secureMrProvider, tensorMetadata);
                        break;
                }            
            }
        }

        private void CreateScalarTensor(Provider secureMrProvider, PXR_SecureMRTensorMetadata tensorMetadata)
        {
            switch(tensorMetadata.dataType)
            {
                case SecureMRTensorDataType.Float:
                    tensor = secureMrProvider.CreateTensor<float,Scalar>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape), tensorData.ToFloatArray());
                    break;
                case SecureMRTensorDataType.Int:
                    tensor = secureMrProvider.CreateTensor<int,Scalar>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape), tensorData.ToIntArray());
                    break;
                case SecureMRTensorDataType.Short:
                case SecureMRTensorDataType.Ushort:
                    tensor = secureMrProvider.CreateTensor<ushort,Scalar>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape), tensorData.ToUShortArray());
                    break;                case SecureMRTensorDataType.Byte:
                case SecureMRTensorDataType.Sbyte:
                    tensor = secureMrProvider.CreateTensor<byte,Scalar>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape), tensorData.ToByteArray());
                    break;

            }
        }

        private void CreateTimestampTensor(Provider secureMrProvider, PXR_SecureMRTensorMetadata tensorMetadata)
        {
            if(tensorMetadata.dataType == SecureMRTensorDataType.Int)
            {
                tensor = secureMrProvider.CreateTensor<int,TimeStamp>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape), tensorData.ToIntArray());
            }
        }

        private void CreateColorTensor(Provider secureMrProvider, PXR_SecureMRTensorMetadata tensorMetadata)
        {
            switch(tensorMetadata.dataType)
            {
                case SecureMRTensorDataType.Float:
                    tensor = secureMrProvider.CreateTensor<float,Color>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape), tensorData.ToFloatArray());
                    break;
                case SecureMRTensorDataType.Int:
                    tensor = secureMrProvider.CreateTensor<int,Color>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape), tensorData.ToIntArray());
                    break;
                case SecureMRTensorDataType.Short:
                case SecureMRTensorDataType.Ushort:
                    tensor = secureMrProvider.CreateTensor<ushort,Color>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape), tensorData.ToUShortArray());
                    break;
                case SecureMRTensorDataType.Byte:
                    tensor = secureMrProvider.CreateTensor<byte, Color>(tensorMetadata.channel,
                        new TensorShape(tensorMetadata.shape), tensorData.ToByteArray());
                    break;
            }
        }

        private void CreatePointTensor(Provider secureMrProvider, PXR_SecureMRTensorMetadata tensorMetadata)
        {
            if(tensorMetadata.dataType == SecureMRTensorDataType.Float)
            {
                tensor = secureMrProvider.CreateTensor<float,Point>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape), tensorData.ToFloatArray());
            }
        }

        void CreateMatrixTensor(Provider secureMrProvider, PXR_SecureMRTensorMetadata tensorMetadata)
        {
            switch(tensorMetadata.dataType)
            {
                case SecureMRTensorDataType.Float:
                    tensor = secureMrProvider.CreateTensor<float,Matrix>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape), tensorData.ToFloatArray());
                    break;
                case SecureMRTensorDataType.Int:
                    tensor = secureMrProvider.CreateTensor<int,Matrix>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape), tensorData.ToIntArray());
                    break;
                case SecureMRTensorDataType.Short:
                case SecureMRTensorDataType.Ushort:
                    tensor = secureMrProvider.CreateTensor<ushort,Matrix>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape), tensorData.ToUShortArray());
                    break;
                case SecureMRTensorDataType.Byte:
                case SecureMRTensorDataType.Sbyte:
                    tensor = secureMrProvider.CreateTensor<byte,Matrix>(tensorMetadata.channel, new TensorShape(tensorMetadata.shape), tensorData.ToByteArray());
                    break;

            }
        }
    }
}

