using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR.SecureMR
{
    public abstract class OperatorConfiguration
    {

    }

    public class ArithmeticComposeOperatorConfiguration : OperatorConfiguration
    {
        public string configText { get; set; }

        public ArithmeticComposeOperatorConfiguration(string configText)
        {
            this.configText = configText;
        }
    }

    public class ComparisonOperatorConfiguration : OperatorConfiguration
    {
        public SecureMRComparison comparison { get; set; }

        public ComparisonOperatorConfiguration(SecureMRComparison comparison)
        {
            this.comparison = comparison;
        }
    }

    public class NmsOperatorConfiguration : OperatorConfiguration
    {
        public float threshold { get; set; }

        public NmsOperatorConfiguration(float threshold)
        {
            this.threshold = threshold;
        }
    }

    public class NormalizeOperatorConfiguration : OperatorConfiguration
    {
        public SecureMRNormalizeType normalizeType { get; set; }

        public NormalizeOperatorConfiguration(SecureMRNormalizeType normalizeType)
        {
            this.normalizeType = normalizeType;
        }
    }

    public class ColorConvertOperatorConfiguration : OperatorConfiguration
    {
        public int convert { get; set; }
        public ColorConvertOperatorConfiguration(int convert)
        {
            this.convert = convert;
        }
    }

    public class SortMatrixOperatorConfiguration : OperatorConfiguration
    {
        public SecureMRMatrixSortType sortType { get; set; }
        public SortMatrixOperatorConfiguration(SecureMRMatrixSortType sortType)
        {
            this.sortType = sortType;
        }
    }

    public class UpdateGltfOperatorConfiguration : OperatorConfiguration
    {
        public SecureMRGltfOperatorAttribute attribute { get; set; }
        public UpdateGltfOperatorConfiguration(SecureMRGltfOperatorAttribute attribute)
        {
            this.attribute = attribute;
        }
    }

    public class RenderTextOperatorConfiguration : OperatorConfiguration
    {
        public SecureMRFontTypeface typeface { get; set; }
        public string languageAndLocale { get; set; }
        public int width { get; set; }
        public int height { get; set; }

        public RenderTextOperatorConfiguration(SecureMRFontTypeface typeface, string languageAndLocale, int width, int height)
        {
            this.typeface = typeface;
            this.languageAndLocale = languageAndLocale;
            this.width = width;
            this.height = height;
        }
    }

    public class ModelOperatorConfiguration : OperatorConfiguration
    {
        public List<SecureMROperatorModelConfig> inputConfigs { get; set; }
        public List<SecureMROperatorModelConfig> outputConfigs { get; set; }
        public byte[] modelData { get; set; }
        public SecureMRModelType modelType { get; set; }
        public string modelName { get; set; }

        public ModelOperatorConfiguration(List<SecureMROperatorModelConfig> inputConfigs, List<SecureMROperatorModelConfig> outputConfigs, byte[] modelData, SecureMRModelType modelType, string modelName)
        {
            this.inputConfigs = inputConfigs;
            this.outputConfigs = outputConfigs;
            this.modelData = modelData;
            this.modelType = modelType;
            this.modelName = modelName;
        }
        
        public ModelOperatorConfiguration(byte[] modelData, SecureMRModelType modelType, string modelName)
        {
            this.inputConfigs = new List<SecureMROperatorModelConfig>();
            this.outputConfigs = new List<SecureMROperatorModelConfig>();
            this.modelData = modelData;
            this.modelType = modelType;
            this.modelName = modelName;
        }

        public void AddInputMapping(string nodeName, string operatorIOName, SecureMRModelEncoding encodingType)
        {
            var config = new SecureMROperatorModelConfig
                { encodingType = encodingType, nodeName = nodeName, operatorIOName = operatorIOName };
            inputConfigs.Add(config);
            
        }

        public void AddOutputMapping(string nodeName, string operatorIOName, SecureMRModelEncoding encodingType)
        {
            var config = new SecureMROperatorModelConfig
                { encodingType = encodingType, nodeName = nodeName, operatorIOName = operatorIOName };
            outputConfigs.Add(config);
        }
    }

    public abstract class Operator
    {
        public SecureMROperatorType OperatorType { get; private set; }
        public ulong OperatorHandle { get; internal set; }
        public ulong PipelineHandle { get; private set; }

        public PxrResult SetOperand(string name, Tensor tensor)
        {
            return PXR_Plugin.SecureMR.UPxr_SetSecureMROperatorOperandByName(PipelineHandle, OperatorHandle, tensor.TensorHandle, name);
        }

        public PxrResult SetResult(string name, Tensor tensor)
        {
            return PXR_Plugin.SecureMR.UPxr_SetSecureMROperatorResultByName(PipelineHandle, OperatorHandle, tensor.TensorHandle, name);
        }

        public Operator(ulong pipelineHandle, SecureMROperatorType operatorType)
        {
            PipelineHandle = pipelineHandle;
            OperatorType = operatorType;
        }
    }

    public class ArithmeticComposeOperator : Operator
    {
        public ArithmeticComposeOperator(ulong pipelineHandle, SecureMROperatorType operatorType, ArithmeticComposeOperatorConfiguration config) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperatorArithmeticCompose(base.PipelineHandle, config.configText, out var operatorHandle);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class ElementwiseMinOperator : Operator
    {
        public ElementwiseMinOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperator(pipelineHandle, operatorType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class ElementwiseMaxOperator : Operator
    {
        public ElementwiseMaxOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperator(pipelineHandle, operatorType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class ElementwiseMultiplyOperator : Operator
    {
        public ElementwiseMultiplyOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperator(pipelineHandle, operatorType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class CustomizedCompareOperator : Operator
    {
        public CustomizedCompareOperator(ulong pipelineHandle, SecureMROperatorType operatorType, ComparisonOperatorConfiguration configuration) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperatorComparison(base.PipelineHandle, configuration.comparison, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class ElementwiseOrOperator : Operator
    {
        public ElementwiseOrOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperator(pipelineHandle, operatorType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class ElementwiseAndOperator : Operator
    {
        public ElementwiseAndOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperator(pipelineHandle, operatorType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class AllOperator : Operator
    {
        public AllOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperator(pipelineHandle, operatorType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class AnyOperator : Operator
    {
        public AnyOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperator(pipelineHandle, operatorType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class NmsOperator : Operator
    {
        public NmsOperator(ulong pipelineHandle, SecureMROperatorType operatorType, NmsOperatorConfiguration configuration) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperatorNonMaximumSuppression(base.PipelineHandle, configuration.threshold, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class SolvePnPOperator : Operator
    {
        public SolvePnPOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperator(pipelineHandle, operatorType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class GetAffineOperator : Operator
    {
        public GetAffineOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperator(pipelineHandle, operatorType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class ApplyAffineOperator : Operator
    {
        public ApplyAffineOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperator(pipelineHandle, operatorType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class ApplyAffinePointOperator : Operator
    {
        public ApplyAffinePointOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperator(pipelineHandle, operatorType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class UvTo3DInCameraSpaceOperator : Operator
    {
        public UvTo3DInCameraSpaceOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperatorUVTo3D(pipelineHandle, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class AssignmentOperator : Operator
    {
        public AssignmentOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperator(pipelineHandle, operatorType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class RunModelInferenceOperator : Operator
    {
        public RunModelInferenceOperator(ulong pipelineHandle, SecureMROperatorType operatorType, ModelOperatorConfiguration configuration) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMrOperatorModel(pipelineHandle, configuration.inputConfigs, configuration.outputConfigs, configuration.modelData, configuration.modelType, configuration.modelName, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class NormalizeOperator : Operator
    {
        public NormalizeOperator(ulong pipelineHandle, SecureMROperatorType operatorType, NormalizeOperatorConfiguration configuration) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperatorNormalize(pipelineHandle, configuration.normalizeType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class CameraSpaceToWorldOperator : Operator
    {
        public CameraSpaceToWorldOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperator(pipelineHandle, operatorType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class RectifiedVstAccessOperator : Operator
    {
        public RectifiedVstAccessOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperator(pipelineHandle, operatorType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class ArgmaxOperator : Operator
    {
        public ArgmaxOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperator(pipelineHandle, operatorType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class ConvertColorOperator : Operator
    {
        public ConvertColorOperator(ulong pipelineHandle, SecureMROperatorType operatorType, ColorConvertOperatorConfiguration convertConfiguration) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperatorColorConvert(pipelineHandle, convertConfiguration.convert, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class SortVectorOperator : Operator
    {
        public SortVectorOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperator(pipelineHandle, operatorType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class InversionOperator : Operator
    {
        public InversionOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperator(pipelineHandle, operatorType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class GetTransformMatrixOperator : Operator
    {
        public GetTransformMatrixOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperator(pipelineHandle, operatorType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class SortMatrixOperator : Operator
    {
        public SortMatrixOperator(ulong pipelineHandle, SecureMROperatorType operatorType, SortMatrixOperatorConfiguration configuration) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperatorSortMatrix(pipelineHandle, configuration.sortType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class SwitchGltfRenderStatusOperator : Operator
    {
        public SwitchGltfRenderStatusOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperator(pipelineHandle, operatorType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class UpdateGltfOperator : Operator
    {
        public UpdateGltfOperator(ulong pipelineHandle, SecureMROperatorType operatorType, UpdateGltfOperatorConfiguration configuration) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperatorUpdateGltf(pipelineHandle, configuration.attribute, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class RenderTextOperator : Operator
    {
        public RenderTextOperator(ulong pipelineHandle, SecureMROperatorType operatorType, RenderTextOperatorConfiguration configuration) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperatorRenderText(pipelineHandle, configuration.typeface, configuration.languageAndLocale, configuration.width, configuration.height, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }

    public class LoadTextureOperator : Operator
    {
        public LoadTextureOperator(ulong pipelineHandle, SecureMROperatorType operatorType) : base(pipelineHandle, operatorType)
        {
            var result = PXR_Plugin.SecureMR.UPxr_CreateSecureMROperator(pipelineHandle, operatorType, out var operatorHandle);
            PLog.i(PXR_Plugin.SecureMR.TAG, $"Create {operatorType} operator" + result, false);
            if (result == PxrResult.SUCCESS)
            {
                base.OperatorHandle = operatorHandle;
            }
        }
    }
}

