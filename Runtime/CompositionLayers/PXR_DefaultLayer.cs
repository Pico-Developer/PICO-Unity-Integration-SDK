#if XR_COMPOSITION_LAYERS

using Unity.XR.CompositionLayers;
using Unity.XR.CompositionLayers.Layers;
using Unity.XR.CompositionLayers.Services;

namespace Unity.XR.PXR
{
    internal class PXR_DefaultLayer : PXR_LayerProvider.ILayerHandler
    {
        unsafe void SetDefaultLayerAttributes(CompositionLayerManager.LayerInfo layerInfo)
        {
            var extensions = PXR_LayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Layer);
            PXR_LayerUtility.SetDefaultSceneLayerExtensions(extensions);

            var flags = layerInfo.Layer.LayerData.BlendType == BlendType.Premultiply ? XrCompositionLayerFlags.SourceAlpha : XrCompositionLayerFlags.SourceAlpha | XrCompositionLayerFlags.UnPremultipliedAlpha;
            PXR_LayerUtility.SetDefaultLayerFlags(flags);
        }

        public void CreateLayer(CompositionLayerManager.LayerInfo layerInfo) => SetDefaultLayerAttributes(layerInfo);

        public void ModifyLayer(CompositionLayerManager.LayerInfo layerInfo) => SetDefaultLayerAttributes(layerInfo);

        public void OnUpdate()
        {
            return;
        }

        public void RemoveLayer(int id)
        {
            return;
        }

        public void SetActiveLayer(CompositionLayerManager.LayerInfo layerInfo)
        {
            return;
        }
    }
}
#endif
