#if XR_COMPOSITION_LAYERS
using Unity.XR.CompositionLayers;
using Unity.XR.CompositionLayers.Extensions;
using Unity.XR.CompositionLayers.Layers;
using Unity.XR.CompositionLayers.Services;
using Unity.XR.PXR;
using UnityEngine;

namespace Unity.XR.PXR
{
    //Default PXR_ Composition Layer - Quad Layer support
    internal class PXR_QuadLayer : PXR_CustomLayerHandler<XrCompositionLayerQuad>
    {
        protected override bool CreateSwapchain(CompositionLayerManager.LayerInfo layer, out SwapchainCreateInfo swapchainCreateInfo)
        {
            unsafe
            {
                var texturesExtension = layer.Layer.GetComponent<TexturesExtension>();
                if (texturesExtension == null || texturesExtension.enabled == false)
                {
                    swapchainCreateInfo = default;
                    return false;
                }

                switch (texturesExtension.sourceTexture)
                {
                    case TexturesExtension.SourceTextureEnum.LocalTexture:
                    {
                        if (texturesExtension.LeftTexture == null)
                            goto default;

                        var xrCreateInfo = new XrSwapchainCreateInfo()
                        {
                            Type = (uint)XrStructureType.XR_TYPE_SWAPCHAIN_CREATE_INFO,
                            Next = PXR_LayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Swapchain),
                            CreateFlags = 0,
                            UsageFlags = (ulong)(XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_SAMPLED_BIT | XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_COLOR_ATTACHMENT_BIT),
                            Format = PXR_LayerUtility.GetDefaultColorFormat(),
                            SampleCount = 1,
                            Width = (uint)texturesExtension.LeftTexture.width,
                            Height = (uint)texturesExtension.LeftTexture.height,
                            FaceCount = 1,
                            ArraySize = 1,
                            MipCount = (uint)texturesExtension.LeftTexture.mipmapCount,
                        };

                        swapchainCreateInfo = new SwapchainCreateInfo(xrCreateInfo, isExternalSurface: false);
                        return true;
                    }

                    case TexturesExtension.SourceTextureEnum.AndroidSurface:
                    {
#if UNITY_ANDROID
                        var xrCreateInfo = new XrSwapchainCreateInfo()
                        {
                            Type = (uint)XrStructureType.XR_TYPE_SWAPCHAIN_CREATE_INFO,
                            Next = PXR_LayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Swapchain),
                            CreateFlags = 0,
                            UsageFlags = (ulong)(XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_SAMPLED_BIT | XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_COLOR_ATTACHMENT_BIT),
                            Format = 0,
                            SampleCount = 0,
                            Width = (uint)texturesExtension.Resolution.x,
                            Height = (uint)texturesExtension.Resolution.y,
                            FaceCount = 0,
                            ArraySize = 0,
                            MipCount = 0,
                        };

                        swapchainCreateInfo = new SwapchainCreateInfo(xrCreateInfo, isExternalSurface: true);
                        return true;
#else
                        goto default;
#endif
                    }

                    default:
                        swapchainCreateInfo = default;
                        return false;
                }
            }
        }

        protected override bool CreateNativeLayer(CompositionLayerManager.LayerInfo layer, SwapchainCreatedOutput swapchainOutput, out XrCompositionLayerQuad nativeLayer)
        {
            unsafe
            {
                var texturesExtension = layer.Layer.GetComponent<TexturesExtension>();
                if (texturesExtension == null || texturesExtension.enabled == false)
                {
                    nativeLayer = default;
                    return false;
                }

                var data = layer.Layer.LayerData as QuadLayerData;
                var transform = layer.Layer.GetComponent<Transform>();
                int subImageWidth = 0;
                int subImageHeight = 0;

                switch (texturesExtension.sourceTexture)
                {
                    case TexturesExtension.SourceTextureEnum.LocalTexture:
                    {
                        if (texturesExtension.LeftTexture != null)
                        {
                            subImageWidth = texturesExtension.LeftTexture.width;
                            subImageHeight = texturesExtension.LeftTexture.height;
                        }
                        break;
                    }

                    case TexturesExtension.SourceTextureEnum.AndroidSurface:
                    {
                        subImageWidth = (int)texturesExtension.Resolution.x;
                        subImageHeight = (int)texturesExtension.Resolution.y;
                        break;
                    }
                }

                var correctedSize = texturesExtension.CropToAspect ?
                    FixAspectRatio(data, transform, subImageWidth, subImageHeight) :
                    data.GetScaledSize(transform.lossyScale);

                nativeLayer = new XrCompositionLayerQuad()
                {
                    Type = (uint)XrStructureType.XR_TYPE_COMPOSITION_LAYER_QUAD,
                    Next = PXR_LayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Layer),
                    LayerFlags = data.BlendType == BlendType.Premultiply ? XrCompositionLayerFlags.SourceAlpha : XrCompositionLayerFlags.SourceAlpha | XrCompositionLayerFlags.UnPremultipliedAlpha,
                    Space = PXR_LayerUtility.GetCurrentAppSpace(),
                    EyeVisibility = 0,
                    SubImage = new XrSwapchainSubImage()
                    {
                        Swapchain = swapchainOutput.handle,
                        ImageRect = new XrRect2Di()
                        {
                            Offset = new XrOffset2Di() { X = 0, Y = 0 },
                            Extent = new XrExtent2Di()
                            {
                                Width = subImageWidth,
                                Height = subImageHeight
                            }
                        },
                        ImageArrayIndex = 0
                    },
                    Pose = new XrPosef(PXR_Utility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).position, PXR_Utility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).rotation),
                    Size = new XrExtent2Df()
                    {
                        width = correctedSize.x,
                        height = correctedSize.y
                    }
                };
                return true;
            }
        }

        protected override bool ModifyNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref XrCompositionLayerQuad nativeLayer)
        {
            var texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false)
                return false;

            var data = layerInfo.Layer.LayerData as QuadLayerData;
            var transform = layerInfo.Layer.GetComponent<Transform>();
            nativeLayer.Pose = new XrPosef(PXR_Utility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).position, PXR_Utility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).rotation);

            int subImageWidth = 0;
            int subImageHeight = 0;

            switch (texturesExtension.sourceTexture)
            {
                case TexturesExtension.SourceTextureEnum.LocalTexture:
                {
                    if (texturesExtension.LeftTexture != null)
                    {
                        subImageWidth = texturesExtension.LeftTexture.width;
                        subImageHeight = texturesExtension.LeftTexture.height;
                    }
                    break;
                }

                case TexturesExtension.SourceTextureEnum.AndroidSurface:
                {
                    subImageWidth = (int)texturesExtension.Resolution.x;
                    subImageHeight = (int)texturesExtension.Resolution.y;
                    break;
                }
            }

            nativeLayer.SubImage.ImageRect.Extent = new XrExtent2Di()
            {
                Width = subImageWidth,
                Height = subImageHeight
            };

            var correctedSize = texturesExtension.CropToAspect ?
                FixAspectRatio(data, transform, subImageWidth, subImageHeight) :
                data.GetScaledSize(transform.lossyScale);
            nativeLayer.Size = new XrExtent2Df()
            {
                width = correctedSize.x,
                height = correctedSize.y
            };

            unsafe
            {
                nativeLayer.Next = PXR_LayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Layer);
            }

            return true;
        }

        protected override bool ActiveNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref XrCompositionLayerQuad nativeLayer)
        {
            var texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false)
                return false;

            var data = layerInfo.Layer.LayerData as QuadLayerData;
            var transform = layerInfo.Layer.GetComponent<Transform>();
            nativeLayer.Pose = new XrPosef(PXR_Utility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).position, PXR_Utility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).rotation);
            nativeLayer.Space = PXR_LayerUtility.GetCurrentAppSpace();

            if (texturesExtension.CustomRects)
            {
                int subImageWidth = 0;
                int subImageHeight = 0;
                switch (texturesExtension.sourceTexture)
                {
                    case TexturesExtension.SourceTextureEnum.LocalTexture:
                    {
                        if (texturesExtension.LeftTexture != null)
                        {
                            subImageWidth = texturesExtension.LeftTexture.width;
                            subImageHeight = texturesExtension.LeftTexture.height;
                        }
                        break;
                    }

                    case TexturesExtension.SourceTextureEnum.AndroidSurface:
                    {
                        subImageWidth = (int)texturesExtension.Resolution.x;
                        subImageHeight = (int)texturesExtension.Resolution.y;
                        break;
                    }
                }

                nativeLayer.SubImage.ImageRect = new XrRect2Di()
                {
                    Offset = new XrOffset2Di()
                    {
                        X = (int)(subImageWidth * texturesExtension.LeftEyeSourceRect.x),
                        Y = (int)(subImageHeight * texturesExtension.LeftEyeSourceRect.y)
                    },

                    Extent = new XrExtent2Di()
                    {
                        Width = (int)(subImageWidth * texturesExtension.LeftEyeSourceRect.width),
                        Height = (int)(subImageHeight * texturesExtension.LeftEyeSourceRect.height)
                    }
                };

                var currentPosition = PXR_Utility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).position;
                var correctedSize = texturesExtension.CropToAspect ?
                    FixAspectRatio(data, transform, subImageWidth, subImageHeight) :
                    data.GetScaledSize(transform.lossyScale);

                float transformedX = currentPosition.x + (((texturesExtension.LeftEyeDestinationRect.x + (0.5f * texturesExtension.LeftEyeDestinationRect.width) - 0.5f)) * correctedSize.x);
                float transformedY = currentPosition.y + (((texturesExtension.LeftEyeDestinationRect.y + (0.5f * texturesExtension.LeftEyeDestinationRect.height) - 0.5f)) * (-1.0f * correctedSize.y));
                nativeLayer.Pose = new XrPosef(new Vector3(transformedX, transformedY, currentPosition.z), PXR_Utility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).rotation);
                nativeLayer.Size = new XrExtent2Df()
                {
                    width = correctedSize.x * texturesExtension.LeftEyeDestinationRect.width,
                    height = correctedSize.y * texturesExtension.LeftEyeDestinationRect.height
                };
            }

            return base.ActiveNativeLayer(layerInfo, ref nativeLayer);
        }

        static Vector2 FixAspectRatio(QuadLayerData data, Transform transform, int texWidth, int texHeight)
        {
            var requestedSize = data.GetScaledSize(transform.lossyScale);
            float reqSizeRatio = (float)requestedSize.x / (float)requestedSize.y;
            float texRatio = (float)texWidth / (float)texHeight;
            if (reqSizeRatio > texRatio)
            {
                // too wide
                requestedSize.x = requestedSize.y * texRatio;
            }
            else if (reqSizeRatio < texRatio)
            {
                // too narrow
                requestedSize.y = requestedSize.x / texRatio;
            }
            return requestedSize;
        }
    }
}


#endif
