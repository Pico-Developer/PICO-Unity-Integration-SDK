#if XR_COMPOSITION_LAYERS
using Unity.XR.CompositionLayers;
using Unity.XR.CompositionLayers.Extensions;
using Unity.XR.CompositionLayers.Layers;
using Unity.XR.CompositionLayers.Services;
using UnityEngine;

namespace Unity.XR.PXR
{
    internal class PXR_CylinderLayer : PXR_CustomLayerHandler<XrCompositionLayerCylinderKHR>
    {
        float savedDelta;
        bool layerDataChanged = false;

        struct CylinderLayerSize
        {
            public float radius;
            public float centralAngle;
            public float aspectRatio;

            public static implicit operator CylinderLayerSize(Vector3 v) => new CylinderLayerSize
            {
                radius = v.x,
                centralAngle = v.y,
                aspectRatio = v.z
            };
        }

        protected override bool CreateSwapchain(CompositionLayerManager.LayerInfo layer, out SwapchainCreateInfo swapchainCreateInfo)
        {
            if (layer.Layer == null)
            {
                swapchainCreateInfo = default;
                return false;
            }

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

        protected override bool CreateNativeLayer(CompositionLayerManager.LayerInfo layer, SwapchainCreatedOutput swapchainOutput, out XrCompositionLayerCylinderKHR nativeLayer)
        {
            unsafe
            {
                var data = layer.Layer.LayerData as CylinderLayerData;
                var transform = layer.Layer.GetComponent<Transform>();
                var texturesExtension = layer.Layer.GetComponent<TexturesExtension>();
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

                CylinderLayerSize scaledSize = data.GetScaledSize(transform.lossyScale);
                if (texturesExtension.CropToAspect)
                {
                    scaledSize = FixAspectRatio(data, scaledSize, subImageWidth, subImageHeight);
                }

                nativeLayer = new XrCompositionLayerCylinderKHR()
                {
                    Type = (uint)XrStructureType.XR_TYPE_COMPOSITION_LAYER_CYLINDER_KHR,
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
                    Radius = data.ApplyTransformScale ? scaledSize.radius : data.Radius,
                    CentralAngle = data.ApplyTransformScale ? scaledSize.centralAngle : data.CentralAngle,
                    AspectRatio = data.ApplyTransformScale ? scaledSize.aspectRatio : data.AspectRatio,
                };
                layerDataChanged = true;
                return true;
            }
        }

        protected override bool ModifyNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref XrCompositionLayerCylinderKHR nativeLayer)
        {
            var texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false)
                return false;

            var data = layerInfo.Layer.LayerData as CylinderLayerData;
            GetSubImageDimensions(out int subImageWidth, out int subImageHeight, texturesExtension);
            nativeLayer.SubImage.ImageRect.Extent = new XrExtent2Di()
            {
                Width = subImageWidth,
                Height = subImageHeight
            };

            var transform = layerInfo.Layer.GetComponent<Transform>();
            CylinderLayerSize scaledSize = data.GetScaledSize(transform.lossyScale);
            if (texturesExtension.CropToAspect)
            {
                scaledSize = FixAspectRatio(data, scaledSize, subImageWidth, subImageHeight);
            }
            nativeLayer.Radius = data.ApplyTransformScale ? scaledSize.radius : data.Radius;
            nativeLayer.CentralAngle = data.ApplyTransformScale ? scaledSize.centralAngle : data.CentralAngle;
            nativeLayer.AspectRatio = data.ApplyTransformScale ? scaledSize.aspectRatio : data.AspectRatio;

            unsafe
            {
                nativeLayer.Next = PXR_LayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Layer);
            }
            layerDataChanged = true;
            return true;
        }

        protected override bool ActiveNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref XrCompositionLayerCylinderKHR nativeLayer)
        {
            var texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false)
                return false;

            var transform = layerInfo.Layer.GetComponent<Transform>();

            // Special treatment for cylinder type based on destination rects.
            if (texturesExtension != null && texturesExtension.CustomRects)
            {
                var cylinderLayer = layerInfo.Layer.LayerData as CylinderLayerData;
                float rotationDelta = (texturesExtension.LeftEyeDestinationRect.x + (0.5f * texturesExtension.LeftEyeDestinationRect.width) - 0.5f) * cylinderLayer.CentralAngle / (float)System.Math.PI * 180.0f;

                if (rotationDelta != savedDelta)
                {
                    Quaternion savedDeltaQuaternion = Quaternion.AngleAxis(savedDelta, Vector3.up);
                    Quaternion deltaQuaternion = Quaternion.AngleAxis(rotationDelta, Vector3.up);
                    Quaternion difference = deltaQuaternion * Quaternion.Inverse(savedDeltaQuaternion);

                    savedDelta = rotationDelta;
                    transform.rotation *= difference;
                }
            }

            nativeLayer.Pose = new XrPosef(PXR_Utility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).position, PXR_Utility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).rotation);
            nativeLayer.Space = PXR_LayerUtility.GetCurrentAppSpace();

            if (texturesExtension.CustomRects && layerDataChanged)
            {
                GetSubImageDimensions(out int subImageWidth, out int subImageHeight, texturesExtension);

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
                float cylinderHeight = nativeLayer.Radius * nativeLayer.CentralAngle / nativeLayer.AspectRatio;
                float transformedY = currentPosition.y + (((texturesExtension.LeftEyeDestinationRect.y + (0.5f * texturesExtension.LeftEyeDestinationRect.height) - 0.5f)) * (-1.0f * cylinderHeight));
                nativeLayer.Pose = new XrPosef(new Vector3(currentPosition.x, transformedY, currentPosition.z), PXR_Utility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).rotation);

                nativeLayer.CentralAngle = nativeLayer.CentralAngle * texturesExtension.LeftEyeDestinationRect.width;
                nativeLayer.AspectRatio = nativeLayer.AspectRatio * texturesExtension.LeftEyeDestinationRect.width / texturesExtension.LeftEyeDestinationRect.height;
                layerDataChanged = false;
            }

            return base.ActiveNativeLayer(layerInfo, ref nativeLayer);
        }

        static CylinderLayerSize FixAspectRatio(CylinderLayerData data, CylinderLayerSize scaledSize, int texWidth, int texHeight)
        {
            // because we're cropping and trying to maintain the same other parameters, we don't
            // need to consider data.MaintainAspectRatio here. That's mostly an editor concern, anyway.
            float texRatio = (float)texWidth / (float)texHeight;
            if (scaledSize.aspectRatio > texRatio)
            {
                // too wide
                float width = scaledSize.radius * scaledSize.centralAngle;
                float height = width / scaledSize.aspectRatio;
                scaledSize.centralAngle = height * texRatio / scaledSize.radius;
                scaledSize.aspectRatio = texRatio;
            }
            else if (scaledSize.aspectRatio < texRatio)
            {
                // too narrow
                scaledSize.aspectRatio = texRatio;
            }

            return scaledSize;
        }

        static void GetSubImageDimensions(out int width, out int height, TexturesExtension texturesExtension)
        {
            width = 0;
            height = 0;

            switch (texturesExtension.sourceTexture)
            {
                case TexturesExtension.SourceTextureEnum.LocalTexture:
                {
                    if (texturesExtension.LeftTexture != null)
                    {
                        width = texturesExtension.LeftTexture.width;
                        height = texturesExtension.LeftTexture.height;
                    }
                    break;
                }

                case TexturesExtension.SourceTextureEnum.AndroidSurface:
                {
                    width = (int)texturesExtension.Resolution.x;
                    height = (int)texturesExtension.Resolution.y;
                    break;
                }
            }
        }
    }

}


#endif
