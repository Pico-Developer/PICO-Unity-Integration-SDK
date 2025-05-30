using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR.SecureMR;

namespace Unity.XR.PXR.SecureMR
{
    public class PXR_SecureMRModelOperatorConfiguration : PXR_SecureMROperatorConfig
    {
        [Header("Model Settings")]
        [Tooltip("The ML model asset to use")]
        public TextAsset modelAsset;

        [Tooltip("The type of model to use")]
        public SecureMRModelType modelType = SecureMRModelType.QnnContextBinary;

        [Tooltip("Name of the model")]
        public string modelName;

        [System.Serializable]
        public class ModelIOConfig
        {
            [Tooltip("Node name in the model")]
            public string nodeName;

            [Tooltip("Operator IO name")]
            public string operatorIOName;

            [Tooltip("Encoding type for the tensor")]
            public SecureMRModelEncoding encodingType = SecureMRModelEncoding.Float32;
        }

        [Header("Input Configuration")]
        [Tooltip("Input tensor configurations")]
        public List<ModelIOConfig> inputs = new List<ModelIOConfig>();

        [Header("Output Configuration")]
        [Tooltip("Output tensor configurations")]
        public List<ModelIOConfig> outputs = new List<ModelIOConfig>();

        /// <summary>
        /// Creates a ModelOperatorConfiguration from this ScriptableObject
        /// </summary>
        /// <returns>A ModelOperatorConfiguration instance ready to use with CreateOperator</returns>
        public ModelOperatorConfiguration CreateModelOperatorConfiguration()
        {
            if (modelAsset == null)
            {
                Debug.LogError("Model asset is not assigned in the configuration");
                return null;
            }

            // Create the base configuration
            ModelOperatorConfiguration modelConfig = new ModelOperatorConfiguration(
                modelAsset.bytes,
                modelType,
                string.IsNullOrEmpty(modelName) ? modelAsset.name : modelName
            );

            // Add input mappings
            foreach (var input in inputs)
            {
                modelConfig.AddInputMapping(input.nodeName, input.operatorIOName, input.encodingType);
            }

            // Add output mappings
            foreach (var output in outputs)
            {
                modelConfig.AddOutputMapping(output.nodeName, output.operatorIOName, output.encodingType);
            }

            return modelConfig;
        }

    }
}