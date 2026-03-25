#if !PICO_OPENXR_SDK
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

using Unity.XR.PXR.SecureMR;
using Point = Unity.XR.PXR.SecureMR.Point;

namespace Unity.XR.PXR
{
    #region SecureMR

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMRPipelineExecuteParameter
    {
        public XrStructureType type;
        public IntPtr next;
        public ulong pipelineRunToBeWaited;
        public ulong conditionTensor;
        public uint pairCount;
        public IntPtr pipelineIOPair; //SecureMrPipelineIOPair[]
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMRPipelineIOPair
    {
        public XrStructureType type;
        public IntPtr next;
        public ulong localPlaceHolderTensor;
        public ulong globalTensor;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMRTensorBuffer
    {
        public XrStructureType type;
        public IntPtr next;
        public uint bufferSize;
        public IntPtr buffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMRTensorCreateInfoShape
    {
        public XrStructureType type;
        public IntPtr next;
        public bool placeHolder;
        public uint dimensionsCount;
        public IntPtr dimensions;
        public IntPtr format; // XrSecureMrTensorFormat
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMRTensorCreateInfoGltf
    {
        public XrStructureType type;
        public IntPtr next;
        public bool placeHolder;
        public uint bufferSize;
        public IntPtr buffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMRTensorFormat
    {
        public SecureMRTensorDataType dataType;
        public sbyte channel;
        public SecureMRTensorUsage tensorUsage;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorArithmeticCompose
    {
        public XrStructureType type;
        public IntPtr next;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2048)]
        public byte[] configText;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorComparison
    {
        public XrStructureType type;
        public IntPtr next;
        public SecureMRComparison comparison;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorNonMaximumSuppression
    {
        public XrStructureType type;
        public IntPtr next;
        public float threshold;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorNormalize
    {
        public XrStructureType type;
        public IntPtr next;
        public SecureMRNormalizeType normalizeType;
    }

    /// <summary>
    /// convert:https://docs.opencv.org/3.4/d8/d01/group__imgproc__color__conversions.html
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorColorConvert
    {
        public XrStructureType type;
        public IntPtr next;
        public int convert;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorSortMatrix
    {
        public XrStructureType type;
        public IntPtr next;
        public SecureMRMatrixSortType sortType;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorUpdateGltf
    {
        public XrStructureType type;
        public IntPtr next;
        public SecureMRGltfOperatorAttribute attribute;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorRenderText
    {
        public XrStructureType type;
        public IntPtr next;
        public SecureMRFontTypeface typeFace;
        public string languageAndLocale;
        public int width;
        public int height;
    }
    public enum ResultMsg
    {
        Unknown = int.MaxValue,
        SUCCESS = 0,
        PENDING = 1,
        FAILED = -1,
        TIMEOUT = -2,
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorModel
    {
        public XrStructureType type;
        public IntPtr next;
        public uint modelInputCount;
        public IntPtr modelInputs;//SecureMrOperatorIOMap[]
        public uint modelOutputCount;
        public IntPtr modelOutputs;//SecureMrOperatorIOMap[]
        public uint bufferSize;
        public IntPtr buffer;
        public SecureMRModelType modelType;
        public string modelName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorIOMap
    {
        public XrStructureType type;
        public IntPtr next;
        public SecureMRModelEncoding encodingType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] nodeName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] operatorIOName;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorJavascript
    {
        public XrStructureType type;
        public IntPtr next;
        public string configText;
        public int configTextLength;
    }

    public struct SecureMROperatorModelConfig
    {
        public SecureMRModelEncoding encodingType;
        public string nodeName;
        public string operatorIOName;
    }

    public enum SecureMRModelEncoding
    {
        Float32 = 1,
        UInt8 = 2,
        Int8 = 3,
        UInt16 = 4,
        Int32 = 5,
    }

    public enum SecureMRModelType
    {
        QnnContextBinary = 1,
    }

    public enum SecureMRFontTypeface
    {
        Default = 1,
        SansSerif = 2,
        Serif = 3,
        Monospace = 4,
        Bold = 5,
        Italic = 6,
    }

    public enum SecureMRGltfOperatorAttribute
    {
        Texture = 1,
        Animation = 2,
        WorldPose = 3,
        LocalTransform = 4,
        MaterialMetallicFactor = 5,
        MaterialRoughnessFactor = 6,
        MaterialOcclusionMapTexture = 7,
        MaterialBaseColorFactor = 8,
        MaterialEmissiveFactor = 9,
        MaterialEmissiveStrength = 10,
        MaterialEmissiveTexture = 11,
        MaterialBaseColorTexture = 12,
        MaterialNormalMapTexture = 13,
        MaterialMetallicRoughnessTexture = 14,
    }

    public enum SecureMRMatrixSortType
    {
        Column = 1,
        Row = 2,
    }

    public enum SecureMRNormalizeType
    {
        L1 = 1,
        L2 = 2,
        Inf = 3,
        MinMax = 4,
    }

    public enum SecureMRTensorDataType
    {
        Unknown = -1,
        Byte = 1,
        Sbyte,
        Ushort,
        Short,
        Int,
        Float,
        Double,
        DynamicTextureByte,
        DynamicTextureFloat
        
    }

    public enum SecureMRTensorUsage
    {
        Unknown = -1,
        Point = 1,
        Scalar,
        Slice,
        Color,
        TimeStamp,
        Matrix,
        DynamicTexture = 8
    }

    public enum SecureMRComparison
    {
        Unknown = 0,
        LargerThan = 1,
        SmallerThan = 2,
        SmallerOrEqual = 3,
        LargerOrEqual = 4,
        EqualTo = 5,
        NotEqual = 6,
    }

    public enum SecureMROperatorType
    {
        Unknown = 0,
        ArithmeticCompose = 1,
        ElementwiseMin = 4,
        ElementwiseMax = 5,
        ElementwiseMultiply = 6,
        CustomizedCompare = 7,
        ElementwiseOr = 8,
        ElementwiseAnd = 9,
        All = 10,
        Any = 11,
        Nms = 12,
        SolvePnP = 13,
        GetAffine = 14,
        ApplyAffine = 15,
        ApplyAffinePoint = 16,
        UvTo3DInCameraSpace = 17,
        Assignment = 18,
        RunModelInference = 19,
        Normalize = 21,
        CameraSpaceToWorld = 22,
        RectifiedVstAccess = 23,
        Argmax = 24,
        ConvertColor = 25,
        SortVector = 26,
        Inversion = 27,
        GetTransformMatrix = 28,
        SortMatrix = 29,
        SwitchGltfRenderStatus = 30,
        UpdateGltf = 31,
        RenderText = 32,
        LoadTexture = 33,
        Svd = 34,
        Norm = 35,
        SwapHwcChw = 36,
        Javascript = 39,
    }

    #endregion

    public static class PXR_SecureMRPlugin
    {
        public const string TAG = "SecureMR";

        #region SecureMR

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMRFramework(int width, int height, out ulong providerHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_DestroySecureMRFramework(ulong providerHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMRPipeline(ulong providerHandle, out ulong pipelineHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_DestroySecureMRPipeline(ulong pipelineHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_ExecuteSecureMRPipeline(ulong pipelineHandle,
            ref SecureMRPipelineExecuteParameter parameter, out ulong pipelineRunHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetSecureMROperatorOperandByName(ulong pipelineHandle, ulong operatorHandle,
            ulong tensorHandle, string name);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetSecureMROperatorResultByName(ulong pipelineHandle, ulong operatorHandle,
            ulong tensorHandle, string name);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMRTensorByShape(ulong frameworkHandle,
            SecureMRTensorCreateInfoShape createInfo, out ulong tensorHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMRTensorByGltf(ulong frameworkHandle,
            SecureMRTensorCreateInfoGltf createInfo, out ulong tensorHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMRPipelineTensorByShape(ulong pipelineHandle,
            SecureMRTensorCreateInfoShape createInfo, out ulong tensorHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMRPipelineTensorByGltf(ulong pipelineHandle,
            SecureMRTensorCreateInfoGltf createInfo, out ulong tensorHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_ResetSecureMRTensor(ulong tensorHandle, ref SecureMRTensorBuffer buffer);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_ResetSecureMRPipelineTensor(ulong pipelineHandle, ulong tensorHandle,
            ref SecureMRTensorBuffer buffer);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_DestroySecureMRTensor(ulong tensorHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperator(ulong pipelineHandle, SecureMROperatorType operatorType,
            out ulong operatorHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorArithmeticCompose(ulong pipelineHandle,
            ref SecureMROperatorArithmeticCompose config, out ulong operatorHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorComparison(ulong pipelineHandle,
            ref SecureMROperatorComparison config, out ulong operatorHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorNonMaximumSuppression(ulong pipelineHandle,
            ref SecureMROperatorNonMaximumSuppression config, out ulong operatorHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorUVTo3D(ulong pipelineHandle, out ulong operatorHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorNormalize(ulong pipelineHandle,
            ref SecureMROperatorNormalize config, out ulong operatorHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorColorConvert(ulong pipelineHandle,
            ref SecureMROperatorColorConvert config, out ulong operatorHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorSortMatrix(ulong pipelineHandle,
            ref SecureMROperatorSortMatrix config, out ulong operatorHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorUpdateGltf(ulong pipelineHandle,
            ref SecureMROperatorUpdateGltf config, out ulong operatorHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorRenderText(ulong pipelineHandle,
            ref SecureMROperatorRenderText config, out ulong operatorHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorModel(ulong pipelineHandle, ref SecureMROperatorModel model,
            out ulong operatorHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorJavascript(ulong pipelineHandle,
            ref SecureMROperatorJavascript model, out ulong operatorHandle);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateBufferFromGlobalTensorAsync(ulong tensor, out ulong future);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateBufferFromGlobalTensorComplete(ulong tensor, ulong future,
            ref XrCreateBufferFromGlobalTensorCompletion completion);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateTextureFromGlobalTensorAsync(ulong tensor, out ulong future);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateTextureFromGlobalTensorComplete(ulong tensor, ulong future,
            ref XrCreateTextureFromGlobalTensorCompletion completion);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetReadbackTextureImage(ulong readbackTexture, IntPtr img);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_ReleaseReadbackTexture(ulong readbackTexture);

        #endregion

        public static readonly Dictionary<Type, SecureMRTensorDataType> TensorDataTypeToEnum =
            new Dictionary<Type, SecureMRTensorDataType>
            {
                { typeof(char), SecureMRTensorDataType.Byte },
                { typeof(byte), SecureMRTensorDataType.Byte },
                { typeof(sbyte), SecureMRTensorDataType.Sbyte },
                { typeof(ushort), SecureMRTensorDataType.Ushort },
                { typeof(short), SecureMRTensorDataType.Short },
                { typeof(int), SecureMRTensorDataType.Int },
                { typeof(float), SecureMRTensorDataType.Float },
                { typeof(double), SecureMRTensorDataType.Double },
            };

        public static readonly Dictionary<Type, SecureMRTensorUsage> TensorClassToEnum =
            new Dictionary<Type, SecureMRTensorUsage>
            {
                { typeof(Scalar), SecureMRTensorUsage.Scalar },
                { typeof(Point), SecureMRTensorUsage.Point },
                { typeof(Slice), SecureMRTensorUsage.Slice },
                { typeof(SecureMR.Color), SecureMRTensorUsage.Color },
                { typeof(TimeStamp), SecureMRTensorUsage.TimeStamp },
                { typeof(Matrix), SecureMRTensorUsage.Matrix },
                { typeof(DynamicTexture), SecureMRTensorUsage.DynamicTexture}
            };

        public static readonly Dictionary<SecureMRTensorUsage, Type> TensorEnumToClass =
            new Dictionary<SecureMRTensorUsage, Type>
            {
                { SecureMRTensorUsage.Scalar, typeof(Scalar) },
                { SecureMRTensorUsage.Point, typeof(Point) },
                { SecureMRTensorUsage.Slice, typeof(Slice) },
                { SecureMRTensorUsage.Color, typeof(SecureMR.Color) },
                { SecureMRTensorUsage.TimeStamp, typeof(TimeStamp) },
                { SecureMRTensorUsage.Matrix, typeof(Matrix) },
                { SecureMRTensorUsage.DynamicTexture,typeof(DynamicTexture)}
            };

        public static readonly Dictionary<Type, SecureMROperatorType> OperatorClassToEnum =
            new Dictionary<Type, SecureMROperatorType>
            {
                { typeof(ArithmeticComposeOperator), SecureMROperatorType.ArithmeticCompose },
                { typeof(ElementwiseMinOperator), SecureMROperatorType.ElementwiseMin },
                { typeof(ElementwiseMaxOperator), SecureMROperatorType.ElementwiseMax },
                { typeof(ElementwiseMultiplyOperator), SecureMROperatorType.ElementwiseMultiply },
                { typeof(CustomizedCompareOperator), SecureMROperatorType.CustomizedCompare },
                { typeof(ElementwiseOrOperator), SecureMROperatorType.ElementwiseOr },
                { typeof(ElementwiseAndOperator), SecureMROperatorType.ElementwiseAnd },
                { typeof(AllOperator), SecureMROperatorType.All },
                { typeof(AnyOperator), SecureMROperatorType.Any },
                { typeof(NmsOperator), SecureMROperatorType.Nms },
                { typeof(SolvePnPOperator), SecureMROperatorType.SolvePnP },
                { typeof(GetAffineOperator), SecureMROperatorType.GetAffine },
                { typeof(ApplyAffineOperator), SecureMROperatorType.ApplyAffine },
                { typeof(ApplyAffinePointOperator), SecureMROperatorType.ApplyAffinePoint },
                { typeof(UvTo3DInCameraSpaceOperator), SecureMROperatorType.UvTo3DInCameraSpace },
                { typeof(AssignmentOperator), SecureMROperatorType.Assignment },
                { typeof(RunModelInferenceOperator), SecureMROperatorType.RunModelInference },
                { typeof(NormalizeOperator), SecureMROperatorType.Normalize },
                { typeof(CameraSpaceToWorldOperator), SecureMROperatorType.CameraSpaceToWorld },
                { typeof(RectifiedVstAccessOperator), SecureMROperatorType.RectifiedVstAccess },
                { typeof(ArgmaxOperator), SecureMROperatorType.Argmax },
                { typeof(ConvertColorOperator), SecureMROperatorType.ConvertColor },
                { typeof(SortVectorOperator), SecureMROperatorType.SortVector },
                { typeof(InversionOperator), SecureMROperatorType.Inversion },
                { typeof(GetTransformMatrixOperator), SecureMROperatorType.GetTransformMatrix },
                { typeof(SortMatrixOperator), SecureMROperatorType.SortMatrix },
                { typeof(SwitchGltfRenderStatusOperator), SecureMROperatorType.SwitchGltfRenderStatus },
                { typeof(UpdateGltfOperator), SecureMROperatorType.UpdateGltf },
                { typeof(RenderTextOperator), SecureMROperatorType.RenderText },
                { typeof(LoadTextureOperator), SecureMROperatorType.LoadTexture },
                { typeof(SvdOperator), SecureMROperatorType.Svd },
                { typeof(NormOperator), SecureMROperatorType.Norm },
                { typeof(SwapHwcChwOperator), SecureMROperatorType.SwapHwcChw },
                { typeof(JavascriptOperator), SecureMROperatorType.Javascript }
            };

        public static PxrResult UPxr_CreateSecureMRProvider(int width, int height, out ulong providerHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                var result = Pxr_CreateSecureMRFramework(width, height, out providerHandle);

                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            providerHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_DestroySecureMRProvider(ulong providerHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                var result = Pxr_DestroySecureMRFramework(providerHandle);

                return  PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);

#else
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_CreateSecureMRPipeline(ulong providerHandle, out ulong pipelineHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                var result = Pxr_CreateSecureMRPipeline(providerHandle, out pipelineHandle);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            pipelineHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_DestroySecureMRPipeline(ulong pipelineHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                var result = Pxr_DestroySecureMRPipeline(pipelineHandle);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_SetSecureMROperatorOperandByName(ulong pipelineHandle, ulong operatorHandle,
            ulong tensorHandle, string name)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                var result = Pxr_SetSecureMROperatorOperandByName(pipelineHandle, operatorHandle, tensorHandle, name);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_SetSecureMROperatorResultByName(ulong pipelineHandle, ulong operatorHandle,
            ulong tensorHandle, string name)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                var result = Pxr_SetSecureMROperatorResultByName(pipelineHandle, operatorHandle, tensorHandle, name);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_ExecuteSecureMRPipeline(ulong pipelineHandle,
            Dictionary<ulong, ulong> tensorMappings, out ulong pipelineRunHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMRPipelineExecuteParameter parameter = new SecureMRPipelineExecuteParameter
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_PIPELINE_EXECUTE_PARAMETER_PICO,
                    next = IntPtr.Zero,
                    pipelineRunToBeWaited = 0,
                    conditionTensor = 0,
                    pairCount = 0,
                    pipelineIOPair = IntPtr.Zero
                };

                if (tensorMappings != null && tensorMappings.Count > 0)
                {
                    parameter.pairCount = (uint)tensorMappings.Count;
                    List<SecureMRPipelineIOPair> pairs = new List<SecureMRPipelineIOPair>();
                    foreach (var tensorMapping in tensorMappings)
                    {
                        SecureMRPipelineIOPair pair = new SecureMRPipelineIOPair
                        {
                            type = XrStructureType.XR_TYPE_SECURE_MR_PIPELINE_IO_PAIR_PICO,
                            next = IntPtr.Zero,
                            localPlaceHolderTensor = tensorMapping.Key,
                            globalTensor = tensorMapping.Value,
                        };
                        pairs.Add(pair);
                    }

                    int structSize = Marshal.SizeOf(typeof(SecureMRPipelineIOPair));
                    parameter.pipelineIOPair = Marshal.AllocHGlobal(structSize * tensorMappings.Count);
                    for (int i = 0; i < tensorMappings.Count; i++)
                    {
                        IntPtr temp = parameter.pipelineIOPair + i * structSize;
                        Marshal.StructureToPtr(pairs[i], temp, false);
                    }
                }

                var result = Pxr_ExecuteSecureMRPipeline(pipelineHandle, ref parameter, out pipelineRunHandle);

                Marshal.FreeHGlobal(parameter.pipelineIOPair);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            pipelineRunHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_ExecuteSecureMRPipelineAfter(ulong pipelineHandle, ulong lastPipelineRunHandle,
            Dictionary<ulong, ulong> tensorMappings, out ulong pipelineRunHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMRPipelineExecuteParameter parameter = new SecureMRPipelineExecuteParameter
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_PIPELINE_EXECUTE_PARAMETER_PICO,
                    next = IntPtr.Zero,
                    pipelineRunToBeWaited = lastPipelineRunHandle,
                    conditionTensor = 0,
                };
                parameter.pairCount = (uint)tensorMappings.Count;

                List<SecureMRPipelineIOPair> pairs = new List<SecureMRPipelineIOPair>();
                foreach (var tensorMapping in tensorMappings)
                {
                    SecureMRPipelineIOPair pair = new SecureMRPipelineIOPair
                    {
                        type = XrStructureType.XR_TYPE_SECURE_MR_PIPELINE_IO_PAIR_PICO,
                        next = IntPtr.Zero,
                        localPlaceHolderTensor = tensorMapping.Key,
                        globalTensor = tensorMapping.Value,
                    };
                    pairs.Add(pair);
                }

                int structSize = Marshal.SizeOf(typeof(SecureMRPipelineIOPair));
                parameter.pipelineIOPair = Marshal.AllocHGlobal(structSize * tensorMappings.Count);
                for (int i = 0; i < tensorMappings.Count; i++)
                {
                    IntPtr temp = parameter.pipelineIOPair + i * structSize;
                    Marshal.StructureToPtr(pairs[i], temp, false);
                }

                var result = Pxr_ExecuteSecureMRPipeline(pipelineHandle, ref parameter, out pipelineRunHandle);

                Marshal.FreeHGlobal(parameter.pipelineIOPair);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            pipelineRunHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_ExecuteSecureMRPipelineConditional(ulong pipelineHandle, ulong conditionTensor,
            Dictionary<ulong, ulong> tensorMappings, out ulong pipelineRunHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMRPipelineExecuteParameter parameter = new SecureMRPipelineExecuteParameter
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_PIPELINE_EXECUTE_PARAMETER_PICO,
                    next = IntPtr.Zero,
                    pipelineRunToBeWaited = 0,
                    conditionTensor = conditionTensor,
                };
                parameter.pairCount = (uint)tensorMappings.Count;

                List<SecureMRPipelineIOPair> pairs = new List<SecureMRPipelineIOPair>();
                foreach (var tensorMapping in tensorMappings)
                {
                    SecureMRPipelineIOPair pair = new SecureMRPipelineIOPair
                    {
                        type = XrStructureType.XR_TYPE_SECURE_MR_PIPELINE_IO_PAIR_PICO,
                        next = IntPtr.Zero,
                        localPlaceHolderTensor = tensorMapping.Key,
                        globalTensor = tensorMapping.Value,
                    };
                    pairs.Add(pair);
                }

                int structSize = Marshal.SizeOf(typeof(SecureMRPipelineIOPair));
                parameter.pipelineIOPair = Marshal.AllocHGlobal(structSize * tensorMappings.Count);
                for (int i = 0; i < tensorMappings.Count; i++)
                {
                    IntPtr temp = parameter.pipelineIOPair + i * structSize;
                    Marshal.StructureToPtr(pairs[i], temp, false);
                }

                var result = Pxr_ExecuteSecureMRPipeline(pipelineHandle, ref parameter, out pipelineRunHandle);

                Marshal.FreeHGlobal(parameter.pipelineIOPair);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            pipelineRunHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_CreateSecureMRTensorByShape(ulong frameworkHandle, SecureMRTensorDataType dataType,
            int[] dimensions, sbyte channel, SecureMRTensorUsage usage, out ulong tensorHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMRTensorCreateInfoShape createInfo = new SecureMRTensorCreateInfoShape
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_TENSOR_CREATE_INFO_SHAPE_PICO,
                    next = IntPtr.Zero,
                    placeHolder = false,
                    dimensionsCount = (uint)dimensions.Length,
                    dimensions = IntPtr.Zero,
                    format = IntPtr.Zero
                };
                
                SecureMRTensorFormat tensorFormat = new SecureMRTensorFormat
                {
                    dataType = dataType,
                    channel = channel,
                    tensorUsage = usage,
                };

                if (dimensions.Length > 0)
                {
                    int size = Marshal.SizeOf(typeof(int)) * dimensions.Length;
                    createInfo.dimensions = Marshal.AllocHGlobal(size);
                    Marshal.Copy(dimensions, 0, createInfo.dimensions, size);
                }

                int size1 = Marshal.SizeOf(typeof(SecureMRTensorFormat));
                createInfo.format = Marshal.AllocHGlobal(size1);
                Marshal.StructureToPtr(tensorFormat, createInfo.format, false);

                var result = Pxr_CreateSecureMRTensorByShape(frameworkHandle, createInfo, out tensorHandle);
                if (dimensions.Length > 0)
                {
                    Marshal.FreeHGlobal(createInfo.dimensions);
                }
                
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            tensorHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_CreateSecureMRPipelineTensorByShape(ulong pipelineHandle, bool placeHolder,
            SecureMRTensorDataType dataType, int[] dimensions, sbyte channel, SecureMRTensorUsage usage,
            out ulong tensorHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMRTensorCreateInfoShape createInfo = new SecureMRTensorCreateInfoShape
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_TENSOR_CREATE_INFO_SHAPE_PICO,
                    next = IntPtr.Zero,
                    placeHolder = placeHolder,
                    dimensionsCount = 0,
                    dimensions = IntPtr.Zero,
                    format = IntPtr.Zero
                };

                SecureMRTensorFormat tensorFormat = new SecureMRTensorFormat
                {
                    dataType = dataType,
                    channel = channel,
                    tensorUsage = usage,
                };

                if (dimensions.Length > 0)
                {
                    createInfo.dimensionsCount = (uint)dimensions.Length;
                    int size = Marshal.SizeOf(typeof(int)) * dimensions.Length;
                    createInfo.dimensions = Marshal.AllocHGlobal(size);
                    Marshal.Copy(dimensions, 0, createInfo.dimensions, size);
                }

                int size1 = Marshal.SizeOf(typeof(SecureMRTensorFormat));
                createInfo.format = Marshal.AllocHGlobal(size1);
                Marshal.StructureToPtr(tensorFormat, createInfo.format, false);
                var result = Pxr_CreateSecureMRPipelineTensorByShape(pipelineHandle, createInfo, out tensorHandle);
                if (dimensions.Length > 0)
                {
                    Marshal.FreeHGlobal(createInfo.dimensions);
                }
                
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            tensorHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_CreateSecureMRTensorByGltf(ulong frameworkHandle, byte[] gltfData,
            out ulong tensorHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMRTensorCreateInfoGltf createInfo = new SecureMRTensorCreateInfoGltf
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_TENSOR_CREATE_INFO_GLTF_PICO,
                    next = IntPtr.Zero,
                    placeHolder = false,
                };
                int size = Marshal.SizeOf(typeof(byte)) * gltfData.Length;

                createInfo.bufferSize = (uint)size;
                createInfo.buffer = Marshal.AllocHGlobal(size);
                Marshal.Copy(gltfData, 0, createInfo.buffer, size);

                var result = Pxr_CreateSecureMRTensorByGltf(frameworkHandle, createInfo, out tensorHandle);
                Marshal.FreeHGlobal(createInfo.buffer);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            tensorHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_CreateSecureMRPipelineTensorByGltf(ulong pipelineHandle, bool placeHolder,
            byte[] gltfData, out ulong tensorHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMRTensorCreateInfoGltf createInfo = new SecureMRTensorCreateInfoGltf
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_TENSOR_CREATE_INFO_GLTF_PICO,
                    next = IntPtr.Zero,
                    placeHolder = placeHolder,
                    bufferSize = 0,
                    buffer = IntPtr.Zero
                };

                if (gltfData!=null && gltfData.Length > 0)
                {
                    int size = Marshal.SizeOf(typeof(byte)) * gltfData.Length;
                    createInfo.bufferSize = (uint)size;
                    createInfo.buffer = Marshal.AllocHGlobal(size);
                    Marshal.Copy(gltfData, 0, createInfo.buffer, size);
                }
                var result = Pxr_CreateSecureMRPipelineTensorByGltf(pipelineHandle, createInfo, out tensorHandle);
                if (gltfData!=null && gltfData.Length > 0)
                {
                    Marshal.FreeHGlobal(createInfo.buffer);
                }
                
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            tensorHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_ResetSecureMRTensor<T>(ulong tensorHandle, T[] tensorData)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMRTensorBuffer buffer = new SecureMRTensorBuffer
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_TENSOR_BUFFER_PICO,
                    next = IntPtr.Zero,
                    bufferSize = 0
                };

                int size = Marshal.SizeOf(typeof(T)) * tensorData.Length;
                buffer.bufferSize = (uint)size;
                buffer.buffer = Marshal.AllocHGlobal(size);
                if (typeof(T) == typeof(byte))
                {
                    Marshal.Copy(tensorData as byte[], 0, buffer.buffer, tensorData.Length);
                }
                else if (typeof(T) == typeof(sbyte))
                {
                    sbyte[] sbyteArray = tensorData as sbyte[];
                    byte[] byteArray = new byte[size];
                    Buffer.BlockCopy(sbyteArray, 0, byteArray, 0, size);
                    Marshal.Copy(byteArray, 0, buffer.buffer, size);
                }
                else if (typeof(T) == typeof(short))
                {
                    Marshal.Copy(tensorData as short[], 0, buffer.buffer, tensorData.Length);
                }
                else if (typeof(T) == typeof(ushort))
                {
                    ushort[] ushortArray = tensorData as ushort[];
                    byte[] byteArray = new byte[size];
                    Buffer.BlockCopy(ushortArray, 0, byteArray, 0, size);
                    Marshal.Copy(byteArray, 0, buffer.buffer, size);
                }
                else if (typeof(T) == typeof(int))
                {
                    Marshal.Copy(tensorData as int[], 0, buffer.buffer, tensorData.Length);
                }
                else if (typeof(T) == typeof(float))
                {
                    Marshal.Copy(tensorData as float[], 0, buffer.buffer, tensorData.Length);
                }
                else if (typeof(T) == typeof(double))
                {
                    double[] doubleArray = tensorData as double[];
                    byte[] byteArray = new byte[size];
                    Buffer.BlockCopy(doubleArray, 0, byteArray, 0, size);
                    Marshal.Copy(byteArray, 0, buffer.buffer, size);
                }

                var result = Pxr_ResetSecureMRTensor(tensorHandle, ref buffer);

                Marshal.FreeHGlobal(buffer.buffer);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_ResetSecureMRPipelineTensor<T>(ulong pipelineHandle, ulong tensorHandle,
            T[] tensorData)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMRTensorBuffer buffer = new SecureMRTensorBuffer
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_TENSOR_BUFFER_PICO,
                    next = IntPtr.Zero,
                };

                int size = Marshal.SizeOf(typeof(T)) * tensorData.Length;
                buffer.bufferSize = (uint)size;
                buffer.buffer = Marshal.AllocHGlobal(size);
                if (typeof(T) == typeof(byte))
                {
                    Marshal.Copy(tensorData as byte[], 0, buffer.buffer, tensorData.Length);
                }
                else if (typeof(T) == typeof(sbyte))
                {
                    sbyte[] sbyteArray = tensorData as sbyte[];
                    byte[] byteArray = new byte[size];
                    Buffer.BlockCopy(sbyteArray, 0, byteArray, 0, size);
                    Marshal.Copy(byteArray, 0, buffer.buffer, size);
                }
                else if (typeof(T) == typeof(short))
                {
                    Marshal.Copy(tensorData as short[], 0, buffer.buffer, tensorData.Length);
                }
                else if (typeof(T) == typeof(ushort))
                {
                    ushort[] ushortArray = tensorData as ushort[];
                    byte[] byteArray = new byte[size];
                    Buffer.BlockCopy(ushortArray, 0, byteArray, 0, size);
                    Marshal.Copy(byteArray, 0, buffer.buffer, size);
                }
                else if (typeof(T) == typeof(int))
                {
                    Marshal.Copy(tensorData as int[], 0, buffer.buffer, tensorData.Length);
                }
                else if (typeof(T) == typeof(float))
                {
                    Marshal.Copy(tensorData as float[], 0, buffer.buffer, tensorData.Length);
                }
                else if (typeof(T) == typeof(double))
                {
                    double[] doubleArray = tensorData as double[];
                    byte[] byteArray = new byte[size];
                    Buffer.BlockCopy(doubleArray, 0, byteArray, 0, size);
                    Marshal.Copy(byteArray, 0, buffer.buffer, size);
                }

                var result = Pxr_ResetSecureMRPipelineTensor(pipelineHandle, tensorHandle, ref buffer);

                Marshal.FreeHGlobal(buffer.buffer);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_DestroySecureMRTensor(ulong tensorHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                var result = Pxr_DestroySecureMRTensor(tensorHandle);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_CreateSecureMROperator(ulong pipelineHandle, SecureMROperatorType operatorType,
            out ulong operatorHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                var result = Pxr_CreateSecureMROperator(pipelineHandle, operatorType, out operatorHandle);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            operatorHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_CreateSecureMROperatorArithmeticCompose(ulong pipelineHandle, string configText,
            out ulong operatorHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMROperatorArithmeticCompose config = new SecureMROperatorArithmeticCompose
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_ARITHMETIC_COMPOSE_PICO,
                    next = IntPtr.Zero,
                };

                config.configText = new byte[2048];
                byte[] stringBytes = Encoding.UTF8.GetBytes(configText);
                int copyLength = Math.Min(stringBytes.Length, 2048);
                Array.Copy(stringBytes,config.configText,copyLength);

                var result =
 Pxr_CreateSecureMROperatorArithmeticCompose(pipelineHandle, ref config, out operatorHandle);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            operatorHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_CreateSecureMROperatorComparison(ulong pipelineHandle,
            SecureMRComparison comparison, out ulong operatorHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMROperatorComparison config = new SecureMROperatorComparison
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_COMPARISON_PICO,
                    next = IntPtr.Zero,
                    comparison = comparison
                };
                var result = Pxr_CreateSecureMROperatorComparison(pipelineHandle, ref config, out operatorHandle);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            operatorHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_CreateSecureMROperatorNonMaximumSuppression(ulong pipelineHandle, float threshold,
            out ulong operatorHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMROperatorNonMaximumSuppression config = new SecureMROperatorNonMaximumSuppression
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_NON_MAXIMUM_SUPPRESSION_PICO,
                    next = IntPtr.Zero,
                    threshold = threshold,
                };
                var result =
 Pxr_CreateSecureMROperatorNonMaximumSuppression(pipelineHandle, ref config, out operatorHandle);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            operatorHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_CreateSecureMROperatorUVTo3D(ulong pipelineHandle, out ulong operatorHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                var result = Pxr_CreateSecureMROperatorUVTo3D(pipelineHandle, out operatorHandle);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            operatorHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_CreateSecureMROperatorNormalize(ulong pipelineHandle,
            SecureMRNormalizeType normalizeType, out ulong operatorHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMROperatorNormalize config = new SecureMROperatorNormalize
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_NORMALIZE_PICO,
                    next = IntPtr.Zero,
                    normalizeType = normalizeType
                };
                var result = Pxr_CreateSecureMROperatorNormalize(pipelineHandle, ref config, out operatorHandle);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            operatorHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_CreateSecureMROperatorColorConvert(ulong pipelineHandle, int convert,
            out ulong operatorHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMROperatorColorConvert config = new SecureMROperatorColorConvert
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_COLOR_CONVERT_PICO,
                    next = IntPtr.Zero,
                    convert = convert,
                };
                var result = Pxr_CreateSecureMROperatorColorConvert(pipelineHandle, ref config, out operatorHandle);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            operatorHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_CreateSecureMROperatorSortMatrix(ulong pipelineHandle,
            SecureMRMatrixSortType sortType, out ulong operatorHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMROperatorSortMatrix config = new SecureMROperatorSortMatrix
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_SORT_MATRIX_PICO,
                    next = IntPtr.Zero,
                    sortType = sortType,
                };
                var result = Pxr_CreateSecureMROperatorSortMatrix(pipelineHandle, ref config, out operatorHandle);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            operatorHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_CreateSecureMROperatorUpdateGltf(ulong pipelineHandle,
            SecureMRGltfOperatorAttribute attribute, out ulong operatorHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMROperatorUpdateGltf config = new SecureMROperatorUpdateGltf
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_UPDATE_GLTF_PICO,
                    next = IntPtr.Zero,
                    attribute = attribute
                };
                var result = Pxr_CreateSecureMROperatorUpdateGltf(pipelineHandle, ref config, out operatorHandle);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            operatorHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_CreateSecureMROperatorRenderText(ulong pipelineHandle,
            SecureMRFontTypeface typeface, string languageAndLocale, int width, int height, out ulong operatorHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMROperatorRenderText config = new SecureMROperatorRenderText
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_RENDER_TEXT_PICO,
                    next = IntPtr.Zero,
                    typeFace = typeface,
                    languageAndLocale = languageAndLocale,
                    width = width,
                    height = height
                };
                var result = Pxr_CreateSecureMROperatorRenderText(pipelineHandle, ref config, out operatorHandle);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            operatorHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_CreateSecureMrOperatorModel(ulong pipelineHandle,
            List<SecureMROperatorModelConfig> inputConfigs, List<SecureMROperatorModelConfig> outputConfigs,
            byte[] modelData, SecureMRModelType modelType, string modelName, out ulong operatorHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMROperatorModel model = new SecureMROperatorModel
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_MODEL_PICO,
                    next = IntPtr.Zero,
                    modelInputCount = (uint)inputConfigs.Count,
                    modelOutputCount = (uint)outputConfigs.Count,
                    bufferSize = (uint)modelData.Length,
                    modelType = modelType,
                    modelName = modelName,
                };

                //input config
                List<SecureMROperatorIOMap> inputPairs = new List<SecureMROperatorIOMap>();
                foreach (var inputConfig in inputConfigs)
                {
                    SecureMROperatorIOMap inputPair = new SecureMROperatorIOMap
                    {
                        type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_IO_MAP_PICO,
                        next = IntPtr.Zero,
                        encodingType = inputConfig.encodingType
                    };

                    inputPair.nodeName = new byte[512];
                    byte[] nodeNameBytes = Encoding.UTF8.GetBytes(inputConfig.nodeName);
                    int copyLength = Math.Min(nodeNameBytes.Length, 512);
                    Array.Copy(nodeNameBytes, inputPair.nodeName, copyLength);

                    inputPair.operatorIOName = new byte[512];
                    byte[] ioNameBytes = Encoding.UTF8.GetBytes(inputConfig.nodeName);
                    copyLength = Math.Min(ioNameBytes.Length, 512);
                    Array.Copy(ioNameBytes, inputPair.operatorIOName, copyLength);

                    inputPairs.Add(inputPair);
                }
                int structSize = Marshal.SizeOf(typeof(SecureMROperatorIOMap));
                model.modelInputs = Marshal.AllocHGlobal(structSize * inputConfigs.Count);
                for (int i = 0; i < inputConfigs.Count; i++)
                {
                    IntPtr temp = model.modelInputs + i * structSize;
                    Marshal.StructureToPtr(inputPairs[i], temp, false);
                }

                //output config
                List<SecureMROperatorIOMap> outputPairs = new List<SecureMROperatorIOMap>();
                foreach (var outputConfig in outputConfigs)
                {
                    SecureMROperatorIOMap outputPair = new SecureMROperatorIOMap
                    {
                        type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_IO_MAP_PICO,
                        next = IntPtr.Zero,
                        encodingType = outputConfig.encodingType
                    };

                    outputPair.nodeName = new byte[512];
                    byte[] nodeNameBytes = Encoding.UTF8.GetBytes(outputConfig.nodeName);
                    int copyLength = Math.Min(nodeNameBytes.Length, 512);
                    Array.Copy(nodeNameBytes, outputPair.nodeName, copyLength);

                    outputPair.operatorIOName = new byte[512];
                    byte[] ioNameBytes = Encoding.UTF8.GetBytes(outputConfig.nodeName);
                    copyLength = Math.Min(ioNameBytes.Length, 512);
                    Array.Copy(ioNameBytes, outputPair.operatorIOName, copyLength);

                    outputPairs.Add(outputPair);
                }
                model.modelOutputs = Marshal.AllocHGlobal(structSize * outputConfigs.Count);
                for (int i = 0; i < outputConfigs.Count; i++)
                {
                    IntPtr temp = model.modelOutputs + i * structSize;
                    Marshal.StructureToPtr(outputPairs[i], temp, false);
                }

                //modelData
                int size = Marshal.SizeOf(typeof(byte)) * modelData.Length;
                model.buffer = Marshal.AllocHGlobal(size);
                Marshal.Copy(modelData, 0, model.buffer, size);

                var result = Pxr_CreateSecureMROperatorModel(pipelineHandle, ref model, out operatorHandle);
                Marshal.FreeHGlobal(model.buffer);
                Marshal.FreeHGlobal(model.modelInputs);
                Marshal.FreeHGlobal(model.modelOutputs);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            operatorHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }
        
        public static PxrResult UPxr_CreateSecureMROperatorJavascript(ulong pipelineHandle, string configText, out ulong operatorHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMROperatorJavascript config = new SecureMROperatorJavascript
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_JAVASCRIPT_PICO,
                    next = IntPtr.Zero,
                    configText = configText,
                    configTextLength = configText.Length,
                };
                var result = Pxr_CreateSecureMROperatorJavascript(pipelineHandle, ref config, out operatorHandle);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
            operatorHandle = 0;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_CreateBufferFromGlobalTensorAsync(ulong tensor, out ulong future)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var ret = Pxr_CreateBufferFromGlobalTensorAsync(tensor, out future);
            return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(ret);
#else
            future = 0UL;
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_CreateBufferFromGlobalTensorComplete(ulong tensor, ulong future, out byte[] data)
        {
            data = Array.Empty<byte>();
#if UNITY_ANDROID && !UNITY_EDITOR
            XrCreateBufferFromGlobalTensorCompletion completion = new XrCreateBufferFromGlobalTensorCompletion()
            {
                type = XrStructureType.XR_TYPE_CREATE_BUFFER_FROM_GLOBAL_TENSOR_COMPLETION_PICO,
                next = IntPtr.Zero,
            };
            XrReadbackTensorBuffer tensorBuffer = new XrReadbackTensorBuffer()
            {
                type = XrStructureType.XR_TYPE_READBACK_TENSOR_BUFFER_PICO,
            };
            IntPtr tbPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(XrReadbackTensorBuffer)));
            Marshal.StructureToPtr(tensorBuffer, tbPtr, false);
            completion.tensorBuffer = tbPtr;
            
            var ret = Pxr_CreateBufferFromGlobalTensorComplete(tensor, future, ref completion);
            
            if (ret != 0)
            {
                Marshal.FreeHGlobal(tbPtr);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(ret);
            }
            
            tensorBuffer = Marshal.PtrToStructure<XrReadbackTensorBuffer>(tbPtr);
            tensorBuffer.bufferCapacityInput = tensorBuffer.bufferCountOutput;
            tensorBuffer.buffer = Marshal.AllocHGlobal((int)tensorBuffer.bufferCapacityInput);
            Marshal.StructureToPtr(tensorBuffer, tbPtr, true);
            ret = Pxr_CreateBufferFromGlobalTensorComplete(tensor, future, ref completion);
            
            if (ret != 0)
            {
                Marshal.FreeHGlobal(tensorBuffer.buffer);
                Marshal.FreeHGlobal(tbPtr);
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(ret);
            }
            
            tensorBuffer = Marshal.PtrToStructure<XrReadbackTensorBuffer>(tbPtr);
            data= new byte[tensorBuffer.bufferCountOutput];
            if (tensorBuffer.bufferCountOutput > 0)
            {
                Marshal.Copy(tensorBuffer.buffer, data, 0, (int)tensorBuffer.bufferCountOutput);
            }

            Marshal.FreeHGlobal(tensorBuffer.buffer);
            Marshal.FreeHGlobal(tbPtr);
            return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(ret);
#else
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_CreateTextureFromGlobalTensorAsync(ulong tensor, out ulong future)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var ret = Pxr_CreateTextureFromGlobalTensorAsync(tensor, out future);
            return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(ret);
#else
            future = 0UL;
            return PxrResult.SUCCESS;
#endif
        }
        
        public static PxrResult UPxr_CreateTextureFromGlobalTensorComplete(ulong tensor, ulong future, out ulong textureHandle)
        {
            textureHandle = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
            XrCreateTextureFromGlobalTensorCompletion completion = new XrCreateTextureFromGlobalTensorCompletion()
            {
                type = XrStructureType.XR_TYPE_CREATE_TEXTURE_FROM_GLOBAL_TENSOR_COMPLETION_PICO
            };
			var ret = Pxr_CreateTextureFromGlobalTensorComplete(tensor, future, ref completion);
            textureHandle = completion.texture;
			return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(ret);
#else
            return PxrResult.SUCCESS;
#endif
        }
        
        public static PxrResult UPxr_GetReadbackTextureImageVulkan(ulong textureHandle, out IntPtr vkImage)
        {
            vkImage = IntPtr.Zero;
#if UNITY_ANDROID && !UNITY_EDITOR
			var size = Marshal.SizeOf(typeof(XrReadbackTextureImageVulkan));
			IntPtr ptr = Marshal.AllocHGlobal(size);
			try
			{
				var hdr = new XrReadbackTextureImageVulkan
				{
					type = XrStructureType.XR_TYPE_READBACK_TEXTURE_IMAGE_VULKAN_PICO,
					next = IntPtr.Zero,
					image = IntPtr.Zero
				};
				Marshal.StructureToPtr(hdr, ptr, false);
				var ret = Pxr_GetReadbackTextureImage(textureHandle, ptr);
				if (ret == 0)
				{
					hdr = Marshal.PtrToStructure<XrReadbackTextureImageVulkan>(ptr);
					vkImage = hdr.image;
				}
				return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(ret);
			}
			finally
			{
				Marshal.FreeHGlobal(ptr);
			}
#else
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_GetReadbackTextureImageOpenGL(ulong textureHandle, out uint glTexId)
        {
            glTexId = 0U;
#if UNITY_ANDROID && !UNITY_EDITOR
			var size = Marshal.SizeOf(typeof(XrReadbackTextureImageOpenGL));
			IntPtr ptr = Marshal.AllocHGlobal(size);
			try
			{
				var hdr = new XrReadbackTextureImageOpenGL
				{
					type = XrStructureType.XR_TYPE_READBACK_TEXTURE_IMAGE_OPENGL_PICO,
					next = IntPtr.Zero,
					texId = 0U
				};
				Marshal.StructureToPtr(hdr, ptr, false);
				var ret = Pxr_GetReadbackTextureImage(textureHandle, ptr);
				if (ret == 0)
				{
					hdr = Marshal.PtrToStructure<XrReadbackTextureImageOpenGL>(ptr);
					glTexId = hdr.texId;
				}
                return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(ret);
			}
			finally
			{
				Marshal.FreeHGlobal(ptr);
			}
#else
            return PxrResult.SUCCESS;
#endif
        }

        public static PxrResult UPxr_ReleaseReadbackTexture(ulong textureHandle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
			var ret = Pxr_ReleaseReadbackTexture(textureHandle);
			return PXR_Plugin.MixedReality.UPxr_ConvertIntToPxrResult(ret);
#else
            return PxrResult.SUCCESS;
#endif
        }
        
    }
}
#endif
