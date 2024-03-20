/*******************************************************************************
Copyright © 2015-2022 PICO Technology Co., Ltd.All rights reserved.  

NOTICE：All information contained herein is, and remains the property of 
PICO Technology Co., Ltd. The intellectual and technical concepts 
contained herein are proprietary to PICO Technology Co., Ltd. and may be 
covered by patents, patents in process, and are protected by trade secret or 
copyright law. Dissemination of this information or reproduction of this 
material is strictly forbidden unless prior written permission is obtained from
PICO Technology Co., Ltd. 
*******************************************************************************/

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unity.XR.PXR
{
    public class PXR_OverlayManager : MonoBehaviour
    {
        private void OnEnable()
        {
#if UNITY_2019_1_OR_NEWER
            if (GraphicsSettings.renderPipelineAsset != null)
            {
                RenderPipelineManager.beginFrameRendering += BeginRendering;
                RenderPipelineManager.endFrameRendering += EndRendering;
            }
            else
            {
                Camera.onPreRender += OnPreRenderCallBack;
                Camera.onPostRender += OnPostRenderCallBack;
            }
#endif
        }

        private void OnDisable()
        {
#if UNITY_2019_1_OR_NEWER
            if (GraphicsSettings.renderPipelineAsset != null)
            {
                RenderPipelineManager.beginFrameRendering -= BeginRendering;
                RenderPipelineManager.endFrameRendering -= EndRendering;
            }
            else
            {
                Camera.onPreRender -= OnPreRenderCallBack;
                Camera.onPostRender -= OnPostRenderCallBack;
            }
#endif
        }

        private void Start()
        {
            // external surface
            if (PXR_OverLay.Instances.Count > 0)
            {
                foreach (var overlay in PXR_OverLay.Instances)
                {
                    if (overlay.isExternalAndroidSurface)
                    {
                        overlay.CreateExternalSurface(overlay);
                    }
                }
            }
        }

        private void BeginRendering(ScriptableRenderContext arg1, Camera[] arg2)
        {
            foreach (Camera cam in arg2)
            {
                if (cam != null && Camera.main == cam)
                {
                    OnPreRenderCallBack(cam);
                }
            }
        }

        private void EndRendering(ScriptableRenderContext arg1, Camera[] arg2)
        {
            foreach (Camera cam in arg2)
            {
                if (cam != null && Camera.main == cam)
                {
                    OnPostRenderCallBack(cam);
                }
            }
        }

        private void OnPreRenderCallBack(Camera cam)
        {
            // There is only one XR main camera in the scene.
            if (null == Camera.main) return;
            if (cam == null || cam != Camera.main || cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right) return;

            //CompositeLayers
            int boundaryState = PXR_Plugin.Boundary.UPxr_GetSeeThroughState();
            if (null == PXR_OverLay.Instances) return;
            if (PXR_OverLay.Instances.Count > 0 && boundaryState != 2)
            {
                foreach (var overlay in PXR_OverLay.Instances)
                {
                    if (!overlay.isActiveAndEnabled) continue;
                    if (null == overlay.layerTextures) continue;
                    if (!overlay.isClones && overlay.layerTextures[0] == null && overlay.layerTextures[1] == null && !overlay.isExternalAndroidSurface) continue;
                    if (overlay.overlayTransform != null && !overlay.overlayTransform.gameObject.activeSelf) continue;
                    overlay.CreateTexture();
                    if (GraphicsDeviceType.Vulkan == SystemInfo.graphicsDeviceType)
                    {
                        PXR_Plugin.Render.UPxr_GetLayerNextImageIndex(overlay.overlayIndex, ref overlay.imageIndex);
                    }
                }
            }
        }

        private void OnPostRenderCallBack(Camera cam)
        {
            // There is only one XR main camera in the scene.
            if (null == Camera.main) return;
            if (cam == null || cam != Camera.main || cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right) return;

            int boundaryState = PXR_Plugin.Boundary.UPxr_GetSeeThroughState();
            if (null == PXR_OverLay.Instances) return;
            if (PXR_OverLay.Instances.Count > 0 && boundaryState != 2)
            {
                PXR_OverLay.Instances.Sort();
                foreach (var compositeLayer in PXR_OverLay.Instances)
                {
                    if (null == compositeLayer) continue;
                    compositeLayer.UpdateCoords();
                    if (!compositeLayer.isActiveAndEnabled) continue;
                    if (null == compositeLayer.layerTextures) continue;
                    if (!compositeLayer.isClones && compositeLayer.layerTextures[0] == null && compositeLayer.layerTextures[1] == null && !compositeLayer.isExternalAndroidSurface) continue;
                    if (compositeLayer.overlayTransform != null && null == compositeLayer.overlayTransform.gameObject) continue;
                    if (compositeLayer.overlayTransform != null && !compositeLayer.overlayTransform.gameObject.activeSelf) continue;

                    Vector4 colorScale = compositeLayer.GetLayerColorScale();
                    Vector4 colorBias = compositeLayer.GetLayerColorOffset();
                    bool isHeadLocked = false;
                    if (compositeLayer.overlayTransform != null && compositeLayer.overlayTransform.parent == transform)
                    {
                        isHeadLocked = true;
                    }

                    if (!compositeLayer.isExternalAndroidSurface && !compositeLayer.CopyRT()) continue;
                    if (null == compositeLayer.cameraRotations || null == compositeLayer.modelScales || null == compositeLayer.modelTranslations) continue;

                    // set color matrics
                    PXR_Plugin.Render.UPxr_SetConfigFloatArray(ConfigType.SetSubmitLayerEXTItemColorMatrix, compositeLayer.colorMatrix, 18);

                    PxrLayerHeader2 header = new PxrLayerHeader2();
                    PxrPosef poseLeft = new PxrPosef();
                    PxrPosef poseRight = new PxrPosef();

                    header.layerId = compositeLayer.overlayIndex;
                    header.colorScaleX = colorScale.x;
                    header.colorScaleY = colorScale.y;
                    header.colorScaleZ = colorScale.z;
                    header.colorScaleW = colorScale.w;
                    header.colorBiasX = colorBias.x;
                    header.colorBiasY = colorBias.y;
                    header.colorBiasZ = colorBias.z;
                    header.colorBiasW = colorBias.w;
                    header.compositionDepth = compositeLayer.layerDepth;
                    header.headPose.orientation.x = compositeLayer.cameraRotations[0].x;
                    header.headPose.orientation.y = compositeLayer.cameraRotations[0].y;
                    header.headPose.orientation.z = -compositeLayer.cameraRotations[0].z;
                    header.headPose.orientation.w = -compositeLayer.cameraRotations[0].w;
                    header.headPose.position.x = (compositeLayer.cameraTranslations[0].x + compositeLayer.cameraTranslations[1].x) / 2;
                    header.headPose.position.y = (compositeLayer.cameraTranslations[0].y + compositeLayer.cameraTranslations[1].y) / 2;
                    header.headPose.position.z = -(compositeLayer.cameraTranslations[0].z + compositeLayer.cameraTranslations[1].z) / 2;
                    header.layerShape = compositeLayer.overlayShape;
                    header.useLayerBlend = (UInt32)(compositeLayer.useLayerBlend ? 1 : 0);
                    header.layerBlend.srcColor = compositeLayer.srcColor;
                    header.layerBlend.dstColor = compositeLayer.dstColor;
                    header.layerBlend.srcAlpha = compositeLayer.srcAlpha;
                    header.layerBlend.dstAlpha = compositeLayer.dstAlpha;
                    header.useImageRect = (UInt32)(compositeLayer.useImageRect ? 1 : 0);
                    header.imageRectLeft = compositeLayer.getPxrRectiLeft(true);
                    header.imageRectRight = compositeLayer.getPxrRectiLeft(false);

                    if (isHeadLocked)
                    {
                        poseLeft.orientation.x = compositeLayer.overlayTransform.localRotation.x;
                        poseLeft.orientation.y = compositeLayer.overlayTransform.localRotation.y;
                        poseLeft.orientation.z = -compositeLayer.overlayTransform.localRotation.z;
                        poseLeft.orientation.w = -compositeLayer.overlayTransform.localRotation.w;
                        poseLeft.position.x = compositeLayer.overlayTransform.localPosition.x;
                        poseLeft.position.y = compositeLayer.overlayTransform.localPosition.y;
                        poseLeft.position.z = -compositeLayer.overlayTransform.localPosition.z;

                        poseRight.orientation.x = compositeLayer.overlayTransform.localRotation.x;
                        poseRight.orientation.y = compositeLayer.overlayTransform.localRotation.y;
                        poseRight.orientation.z = -compositeLayer.overlayTransform.localRotation.z;
                        poseRight.orientation.w = -compositeLayer.overlayTransform.localRotation.w;
                        poseRight.position.x = compositeLayer.overlayTransform.localPosition.x;
                        poseRight.position.y = compositeLayer.overlayTransform.localPosition.y;
                        poseRight.position.z = -compositeLayer.overlayTransform.localPosition.z;

                        header.layerFlags = (UInt32)(
                            PxrLayerSubmitFlags.PxrLayerFlagLayerPoseNotInTrackingSpace |
                            PxrLayerSubmitFlags.PxrLayerFlagHeadLocked);
                    }
                    else
                    {
                        poseLeft.orientation.x = compositeLayer.modelRotations[0].x;
                        poseLeft.orientation.y = compositeLayer.modelRotations[0].y;
                        poseLeft.orientation.z = -compositeLayer.modelRotations[0].z;
                        poseLeft.orientation.w = -compositeLayer.modelRotations[0].w;
                        poseLeft.position.x = compositeLayer.modelTranslations[0].x;
                        poseLeft.position.y = compositeLayer.modelTranslations[0].y;
                        poseLeft.position.z = -compositeLayer.modelTranslations[0].z;
                        poseRight.orientation.x = compositeLayer.modelRotations[0].x;
                        poseRight.orientation.y = compositeLayer.modelRotations[0].y;
                        poseRight.orientation.z = -compositeLayer.modelRotations[0].z;
                        poseRight.orientation.w = -compositeLayer.modelRotations[0].w;
                        poseRight.position.x = compositeLayer.modelTranslations[0].x;
                        poseRight.position.y = compositeLayer.modelTranslations[0].y;
                        poseRight.position.z = -compositeLayer.modelTranslations[0].z;

                        header.layerFlags = (UInt32)(
                            PxrLayerSubmitFlags.PxrLayerFlagUseExternalHeadPose |
                            PxrLayerSubmitFlags.PxrLayerFlagLayerPoseNotInTrackingSpace);
                    }

                    header.layerFlags |= compositeLayer.getHDRFlags();
                    if (compositeLayer.isPremultipliedAlpha)
                    {
                        header.layerFlags |= (UInt32)PxrLayerSubmitFlags.PxrLayerFlagPremultipliedAlpha;
                    }

                    if (compositeLayer.overlayShape == PXR_OverLay.OverlayShape.Quad)
                    {
                        if (PXR_OverLay.APIExecutionStatus.None == compositeLayer.Quad2Status)
                        {
                            PxrLayerQuad2 layer = new PxrLayerQuad2();
                            layer.header.layerId = -1;
                            if (PXR_Plugin.Render.UPxr_SubmitLayerQuad2(layer))
                            {
                                compositeLayer.Quad2Status = PXR_OverLay.APIExecutionStatus.False;
                                PLog.i("OverlayManager", "Quad2Status:UPxr_SubmitLayerQuad ");
                            }
                            else
                            {
                                compositeLayer.Quad2Status = PXR_OverLay.APIExecutionStatus.True;
                                PLog.i("OverlayManager", "Quad2Status:UPxr_SubmitLayerQuad2 ");
                            }
                        }

                        PxrLayerQuad2 layerSubmit2 = new PxrLayerQuad2();
                        layerSubmit2.header = header;
                        layerSubmit2.poseLeft = poseLeft;
                        layerSubmit2.poseRight = poseRight;

                        layerSubmit2.sizeLeft.x = compositeLayer.modelScales[0].x;
                        layerSubmit2.sizeLeft.y = compositeLayer.modelScales[0].y;
                        layerSubmit2.sizeRight.x = compositeLayer.modelScales[0].x;
                        layerSubmit2.sizeRight.y = compositeLayer.modelScales[0].y;

                        if (compositeLayer.useImageRect)
                        {
                            Vector3 lPos = new Vector3();
                            Vector3 rPos = new Vector3();
                            Quaternion quaternion = new Quaternion(compositeLayer.modelRotations[0].x, compositeLayer.modelRotations[0].y, -compositeLayer.modelRotations[0].z, -compositeLayer.modelRotations[0].w);

                            lPos.x = compositeLayer.modelScales[0].x * (-0.5f + compositeLayer.dstRectLeft.x + 0.5f * Mathf.Min(compositeLayer.dstRectLeft.width, 1 - compositeLayer.dstRectLeft.x));
                            lPos.y = compositeLayer.modelScales[0].y * (-0.5f + compositeLayer.dstRectLeft.y + 0.5f * Mathf.Min(compositeLayer.dstRectLeft.height, 1 - compositeLayer.dstRectLeft.y));
                            lPos.z = 0;
                            lPos = quaternion * lPos;
                            layerSubmit2.poseLeft.position.x += lPos.x;
                            layerSubmit2.poseLeft.position.y += lPos.y;
                            layerSubmit2.poseLeft.position.z += lPos.z;

                            rPos.x = compositeLayer.modelScales[0].x * (-0.5f + compositeLayer.dstRectRight.x + 0.5f * Mathf.Min(compositeLayer.dstRectRight.width, 1 - compositeLayer.dstRectRight.x));
                            rPos.y = compositeLayer.modelScales[0].y * (-0.5f + compositeLayer.dstRectRight.y + 0.5f * Mathf.Min(compositeLayer.dstRectRight.height, 1 - compositeLayer.dstRectRight.y));
                            rPos.z = 0;
                            rPos = quaternion * rPos;
                            layerSubmit2.poseRight.position.x += rPos.x;
                            layerSubmit2.poseRight.position.y += rPos.y;
                            layerSubmit2.poseRight.position.z += rPos.z;

                            layerSubmit2.sizeLeft.x = compositeLayer.modelScales[0].x * Mathf.Min(compositeLayer.dstRectLeft.width, 1 - compositeLayer.dstRectLeft.x);
                            layerSubmit2.sizeLeft.y = compositeLayer.modelScales[0].y * Mathf.Min(compositeLayer.dstRectLeft.height, 1 - compositeLayer.dstRectLeft.y);
                            layerSubmit2.sizeRight.x = compositeLayer.modelScales[0].x * Mathf.Min(compositeLayer.dstRectRight.width, 1 - compositeLayer.dstRectRight.x);
                            layerSubmit2.sizeRight.y = compositeLayer.modelScales[0].y * Mathf.Min(compositeLayer.dstRectRight.height, 1 - compositeLayer.dstRectRight.y);
                        }
                        if (compositeLayer.layerSubmitPtr != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(compositeLayer.layerSubmitPtr);
                            compositeLayer.layerSubmitPtr = IntPtr.Zero;
                        }
                        compositeLayer.layerSubmitPtr = Marshal.AllocHGlobal(Marshal.SizeOf(layerSubmit2));
                        Marshal.StructureToPtr(layerSubmit2, compositeLayer.layerSubmitPtr, false);

                        if (PXR_OverLay.APIExecutionStatus.True == compositeLayer.Quad2Status)
                        {
                            PXR_Plugin.Render.UPxr_SubmitLayerQuad2ByRender(compositeLayer.layerSubmitPtr);
                        }
                        else
                        {
                            PxrLayerQuad layerSubmit = new PxrLayerQuad();
                            layerSubmit.header.layerId = compositeLayer.overlayIndex;
                            layerSubmit.header.colorScaleX = colorScale.x;
                            layerSubmit.header.colorScaleY = colorScale.y;
                            layerSubmit.header.colorScaleZ = colorScale.z;
                            layerSubmit.header.colorScaleW = colorScale.w;
                            layerSubmit.header.colorBiasX = colorBias.x;
                            layerSubmit.header.colorBiasY = colorBias.y;
                            layerSubmit.header.colorBiasZ = colorBias.z;
                            layerSubmit.header.colorBiasW = colorBias.w;
                            layerSubmit.header.compositionDepth = compositeLayer.layerDepth;
                            layerSubmit.header.headPose.orientation.x = compositeLayer.cameraRotations[0].x;
                            layerSubmit.header.headPose.orientation.y = compositeLayer.cameraRotations[0].y;
                            layerSubmit.header.headPose.orientation.z = -compositeLayer.cameraRotations[0].z;
                            layerSubmit.header.headPose.orientation.w = -compositeLayer.cameraRotations[0].w;
                            layerSubmit.header.headPose.position.x = (compositeLayer.cameraTranslations[0].x + compositeLayer.cameraTranslations[1].x) / 2;
                            layerSubmit.header.headPose.position.y = (compositeLayer.cameraTranslations[0].y + compositeLayer.cameraTranslations[1].y) / 2;
                            layerSubmit.header.headPose.position.z = -(compositeLayer.cameraTranslations[0].z + compositeLayer.cameraTranslations[1].z) / 2;

                            if (isHeadLocked)
                            {
                                layerSubmit.pose.orientation.x = compositeLayer.overlayTransform.localRotation.x;
                                layerSubmit.pose.orientation.y = compositeLayer.overlayTransform.localRotation.y;
                                layerSubmit.pose.orientation.z = -compositeLayer.overlayTransform.localRotation.z;
                                layerSubmit.pose.orientation.w = -compositeLayer.overlayTransform.localRotation.w;
                                layerSubmit.pose.position.x = compositeLayer.overlayTransform.localPosition.x;
                                layerSubmit.pose.position.y = compositeLayer.overlayTransform.localPosition.y;
                                layerSubmit.pose.position.z = -compositeLayer.overlayTransform.localPosition.z;

                                layerSubmit.header.layerFlags = (UInt32)(
                                    PxrLayerSubmitFlags.PxrLayerFlagLayerPoseNotInTrackingSpace |
                                    PxrLayerSubmitFlags.PxrLayerFlagHeadLocked);
                            }
                            else
                            {
                                layerSubmit.pose.orientation.x = compositeLayer.modelRotations[0].x;
                                layerSubmit.pose.orientation.y = compositeLayer.modelRotations[0].y;
                                layerSubmit.pose.orientation.z = -compositeLayer.modelRotations[0].z;
                                layerSubmit.pose.orientation.w = -compositeLayer.modelRotations[0].w;
                                layerSubmit.pose.position.x = compositeLayer.modelTranslations[0].x;
                                layerSubmit.pose.position.y = compositeLayer.modelTranslations[0].y;
                                layerSubmit.pose.position.z = -compositeLayer.modelTranslations[0].z;

                                layerSubmit.header.layerFlags = (UInt32)(
                                    PxrLayerSubmitFlags.PxrLayerFlagUseExternalHeadPose |
                                    PxrLayerSubmitFlags.PxrLayerFlagLayerPoseNotInTrackingSpace);
                            }

                            layerSubmit.width = compositeLayer.modelScales[0].x;
                            layerSubmit.height = compositeLayer.modelScales[0].y;

                            if (compositeLayer.layerSubmitPtr != IntPtr.Zero)
                            {
                                Marshal.FreeHGlobal(compositeLayer.layerSubmitPtr);
                                compositeLayer.layerSubmitPtr = IntPtr.Zero;
                            }
                            compositeLayer.layerSubmitPtr = Marshal.AllocHGlobal(Marshal.SizeOf(layerSubmit));
                            Marshal.StructureToPtr(layerSubmit, compositeLayer.layerSubmitPtr, false);
                            PXR_Plugin.Render.UPxr_SubmitLayerQuadByRender(compositeLayer.layerSubmitPtr);
                        }
                    }
                    else if (compositeLayer.overlayShape == PXR_OverLay.OverlayShape.Cylinder)
                    {
                        if (PXR_OverLay.APIExecutionStatus.None == compositeLayer.Cylinder2Status)
                        {
                            PxrLayerCylinder2 layer = new PxrLayerCylinder2();
                            layer.header.layerId = -1;
                            if (PXR_Plugin.Render.UPxr_SubmitLayerCylinder2(layer))
                            {
                                compositeLayer.Cylinder2Status = PXR_OverLay.APIExecutionStatus.False;
                                PLog.i("OverlayManager", "Quad2Status:UPxr_SubmitLayerCylinder ");
                            }
                            else
                            {
                                compositeLayer.Cylinder2Status = PXR_OverLay.APIExecutionStatus.True;
                                PLog.i("OverlayManager", "Quad2Status:UPxr_SubmitLayerCylinder2 ");
                            }
                        }

                        PxrLayerCylinder2 layerSubmit2 = new PxrLayerCylinder2();
                        layerSubmit2.header = header;
                        layerSubmit2.poseLeft = poseLeft;
                        layerSubmit2.poseRight = poseRight;

                        if (compositeLayer.modelScales[0].z != 0)
                        {
                            layerSubmit2.centralAngleLeft = compositeLayer.modelScales[0].x / compositeLayer.modelScales[0].z;
                            layerSubmit2.centralAngleRight = compositeLayer.modelScales[0].x / compositeLayer.modelScales[0].z;
                        }
                        else
                        {
                            Debug.LogError("PXRLog scale.z is 0");
                        }
                        layerSubmit2.heightLeft = compositeLayer.modelScales[0].y;
                        layerSubmit2.heightRight = compositeLayer.modelScales[0].y;
                        layerSubmit2.radiusLeft = compositeLayer.modelScales[0].z;
                        layerSubmit2.radiusRight = compositeLayer.modelScales[0].z;

                        if (compositeLayer.layerSubmitPtr != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(compositeLayer.layerSubmitPtr);
                            compositeLayer.layerSubmitPtr = IntPtr.Zero;
                        }
                        compositeLayer.layerSubmitPtr = Marshal.AllocHGlobal(Marshal.SizeOf(layerSubmit2));
                        Marshal.StructureToPtr(layerSubmit2, compositeLayer.layerSubmitPtr, false);

                        if (PXR_OverLay.APIExecutionStatus.True == compositeLayer.Cylinder2Status)
                        {
                            PXR_Plugin.Render.UPxr_SubmitLayerCylinder2ByRender(compositeLayer.layerSubmitPtr);
                        }
                        else
                        {
                            PxrLayerCylinder layerSubmit = new PxrLayerCylinder();
                            layerSubmit.header.layerId = compositeLayer.overlayIndex;
                            layerSubmit.header.colorScaleX = colorScale.x;
                            layerSubmit.header.colorScaleY = colorScale.y;
                            layerSubmit.header.colorScaleZ = colorScale.z;
                            layerSubmit.header.colorScaleW = colorScale.w;
                            layerSubmit.header.colorBiasX = colorBias.x;
                            layerSubmit.header.colorBiasY = colorBias.y;
                            layerSubmit.header.colorBiasZ = colorBias.z;
                            layerSubmit.header.colorBiasW = colorBias.w;
                            layerSubmit.header.compositionDepth = compositeLayer.layerDepth;
                            layerSubmit.header.headPose.orientation.x = compositeLayer.cameraRotations[0].x;
                            layerSubmit.header.headPose.orientation.y = compositeLayer.cameraRotations[0].y;
                            layerSubmit.header.headPose.orientation.z = -compositeLayer.cameraRotations[0].z;
                            layerSubmit.header.headPose.orientation.w = -compositeLayer.cameraRotations[0].w;
                            layerSubmit.header.headPose.position.x = (compositeLayer.cameraTranslations[0].x + compositeLayer.cameraTranslations[1].x) / 2;
                            layerSubmit.header.headPose.position.y = (compositeLayer.cameraTranslations[0].y + compositeLayer.cameraTranslations[1].y) / 2;
                            layerSubmit.header.headPose.position.z = -(compositeLayer.cameraTranslations[0].z + compositeLayer.cameraTranslations[1].z) / 2;

                            if (isHeadLocked)
                            {
                                layerSubmit.pose.orientation.x = compositeLayer.overlayTransform.localRotation.x;
                                layerSubmit.pose.orientation.y = compositeLayer.overlayTransform.localRotation.y;
                                layerSubmit.pose.orientation.z = -compositeLayer.overlayTransform.localRotation.z;
                                layerSubmit.pose.orientation.w = -compositeLayer.overlayTransform.localRotation.w;
                                layerSubmit.pose.position.x = compositeLayer.overlayTransform.localPosition.x;
                                layerSubmit.pose.position.y = compositeLayer.overlayTransform.localPosition.y;
                                layerSubmit.pose.position.z = -compositeLayer.overlayTransform.localPosition.z;

                                layerSubmit.header.layerFlags = (UInt32)(
                                    PxrLayerSubmitFlags.PxrLayerFlagLayerPoseNotInTrackingSpace |
                                    PxrLayerSubmitFlags.PxrLayerFlagHeadLocked);
                            }
                            else
                            {
                                layerSubmit.pose.orientation.x = compositeLayer.modelRotations[0].x;
                                layerSubmit.pose.orientation.y = compositeLayer.modelRotations[0].y;
                                layerSubmit.pose.orientation.z = -compositeLayer.modelRotations[0].z;
                                layerSubmit.pose.orientation.w = -compositeLayer.modelRotations[0].w;
                                layerSubmit.pose.position.x = compositeLayer.modelTranslations[0].x;
                                layerSubmit.pose.position.y = compositeLayer.modelTranslations[0].y;
                                layerSubmit.pose.position.z = -compositeLayer.modelTranslations[0].z;

                                layerSubmit.header.layerFlags = (UInt32)(
                                    PxrLayerSubmitFlags.PxrLayerFlagUseExternalHeadPose |
                                    PxrLayerSubmitFlags.PxrLayerFlagLayerPoseNotInTrackingSpace);
                            }

                            if (compositeLayer.modelScales[0].z != 0)
                            {
                                layerSubmit.centralAngle = compositeLayer.modelScales[0].x / compositeLayer.modelScales[0].z;
                            }
                            else
                            {
                                Debug.LogError("PXRLog scale.z is 0");
                            }
                            layerSubmit.height = compositeLayer.modelScales[0].y;
                            layerSubmit.radius = compositeLayer.modelScales[0].z;

                            if (compositeLayer.layerSubmitPtr != IntPtr.Zero)
                            {
                                Marshal.FreeHGlobal(compositeLayer.layerSubmitPtr);
                                compositeLayer.layerSubmitPtr = IntPtr.Zero;
                            }
                            compositeLayer.layerSubmitPtr = Marshal.AllocHGlobal(Marshal.SizeOf(layerSubmit));
                            Marshal.StructureToPtr(layerSubmit, compositeLayer.layerSubmitPtr, false);

                            PXR_Plugin.Render.UPxr_SubmitLayerCylinderByRender(compositeLayer.layerSubmitPtr);
                        }
                    }
                    else if (compositeLayer.overlayShape == PXR_OverLay.OverlayShape.Equirect)
                    {
                        if (PXR_OverLay.APIExecutionStatus.None == compositeLayer.Equirect2Status)
                        {
                            PxrLayerEquirect2 layer = new PxrLayerEquirect2();
                            layer.header.layerId = -1;
                            if (PXR_Plugin.Render.UPxr_SubmitLayerEquirect2(layer))
                            {
                                compositeLayer.Equirect2Status = PXR_OverLay.APIExecutionStatus.False;
                                PLog.i("OverlayManager", "Equirect2Status:UPxr_SubmitLayerEquirect ");
                            }
                            else
                            {
                                compositeLayer.Equirect2Status = PXR_OverLay.APIExecutionStatus.True;
                                PLog.i("OverlayManager", "Equirect2Status:UPxr_SubmitLayerEquirect2 ");
                            }
                        }

                        PxrLayerEquirect2 layerSubmit2 = new PxrLayerEquirect2();
                        layerSubmit2.header = header;
                        layerSubmit2.poseLeft = poseLeft;
                        layerSubmit2.poseRight = poseRight;
                        layerSubmit2.header.layerShape = (PXR_OverLay.OverlayShape)4;

                        layerSubmit2.radiusLeft = compositeLayer.radius;
                        layerSubmit2.radiusRight = compositeLayer.radius;
                        layerSubmit2.centralHorizontalAngleLeft = compositeLayer.dstRectLeft.width * 2 * Mathf.PI;
                        layerSubmit2.centralHorizontalAngleRight = compositeLayer.dstRectRight.width * 2 * Mathf.PI;
                        layerSubmit2.upperVerticalAngleLeft = (compositeLayer.dstRectLeft.height + compositeLayer.dstRectLeft.y - 0.5f) * Mathf.PI;
                        layerSubmit2.upperVerticalAngleRight = (compositeLayer.dstRectRight.height + compositeLayer.dstRectRight.y - 0.5f) * Mathf.PI;
                        layerSubmit2.lowerVerticalAngleLeft = (compositeLayer.dstRectLeft.y - 0.5f) * Mathf.PI;
                        layerSubmit2.lowerVerticalAngleRight = (compositeLayer.dstRectRight.y - 0.5f) * Mathf.PI;

                        if (compositeLayer.layerSubmitPtr != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(compositeLayer.layerSubmitPtr);
                            compositeLayer.layerSubmitPtr = IntPtr.Zero;
                        }
                        compositeLayer.layerSubmitPtr = Marshal.AllocHGlobal(Marshal.SizeOf(layerSubmit2));
                        Marshal.StructureToPtr(layerSubmit2, compositeLayer.layerSubmitPtr, false);

                        if (PXR_OverLay.APIExecutionStatus.True == compositeLayer.Equirect2Status)
                        {
                            PXR_Plugin.Render.UPxr_SubmitLayerEquirect2ByRender(compositeLayer.layerSubmitPtr);
                        }
                        else
                        {
                            PxrLayerEquirect layerSubmit = new PxrLayerEquirect();
                            layerSubmit.header = header;
                            layerSubmit.poseLeft = poseLeft;
                            layerSubmit.poseRight = poseRight;
                            layerSubmit.header.layerShape = PXR_OverLay.OverlayShape.Equirect;

                            layerSubmit.radiusLeft = compositeLayer.radius;
                            layerSubmit.radiusRight = compositeLayer.radius;
                            layerSubmit.scaleXLeft = 1 / compositeLayer.dstRectLeft.width;
                            layerSubmit.scaleXRight = 1 / compositeLayer.dstRectRight.width;
                            layerSubmit.scaleYLeft = 1 / compositeLayer.dstRectLeft.height;
                            layerSubmit.scaleYRight = 1 / compositeLayer.dstRectRight.height;
                            layerSubmit.biasXLeft = -compositeLayer.dstRectLeft.x / compositeLayer.dstRectLeft.width;
                            layerSubmit.biasXRight = -compositeLayer.dstRectRight.x / compositeLayer.dstRectRight.width;
                            layerSubmit.biasYLeft = 1 + (compositeLayer.dstRectLeft.y - 1) / compositeLayer.dstRectLeft.height;
                            layerSubmit.biasYRight = 1 + (compositeLayer.dstRectRight.y - 1) / compositeLayer.dstRectRight.height;

                            if (compositeLayer.layerSubmitPtr != IntPtr.Zero)
                            {
                                Marshal.FreeHGlobal(compositeLayer.layerSubmitPtr);
                                compositeLayer.layerSubmitPtr = IntPtr.Zero;
                            }
                            compositeLayer.layerSubmitPtr = Marshal.AllocHGlobal(Marshal.SizeOf(layerSubmit));
                            Marshal.StructureToPtr(layerSubmit, compositeLayer.layerSubmitPtr, false);
                            PXR_Plugin.Render.UPxr_SubmitLayerEquirectByRender(compositeLayer.layerSubmitPtr);
                        }
                    }
                    else if (compositeLayer.overlayShape == PXR_OverLay.OverlayShape.Cubemap)
                    {
                        PxrLayerCube2 layerSubmit2 = new PxrLayerCube2();
                        layerSubmit2.header = header;
                        layerSubmit2.poseLeft = poseLeft;
                        layerSubmit2.poseRight = poseRight;

                        if (compositeLayer.layerSubmitPtr != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(compositeLayer.layerSubmitPtr);
                            compositeLayer.layerSubmitPtr = IntPtr.Zero;
                        }
                        compositeLayer.layerSubmitPtr = Marshal.AllocHGlobal(Marshal.SizeOf(layerSubmit2));
                        Marshal.StructureToPtr(layerSubmit2, compositeLayer.layerSubmitPtr, false);
                        PXR_Plugin.Render.UPxr_SubmitLayerCube2ByRender(compositeLayer.layerSubmitPtr);
                    }
                    else if (compositeLayer.overlayShape == PXR_OverLay.OverlayShape.Eac)
                    {
                        PxrLayerEac2 layerSubmit2 = new PxrLayerEac2();
                        layerSubmit2.header = header;
                        layerSubmit2.poseLeft = poseLeft;
                        layerSubmit2.poseRight = poseRight;

                        layerSubmit2.offsetPosLeft.x = compositeLayer.offsetPosLeft.x;
                        layerSubmit2.offsetPosLeft.y = compositeLayer.offsetPosLeft.y;
                        layerSubmit2.offsetPosLeft.z = compositeLayer.offsetPosLeft.z;
                        layerSubmit2.offsetPosRight.x = compositeLayer.offsetPosRight.x;
                        layerSubmit2.offsetPosRight.y = compositeLayer.offsetPosRight.y;
                        layerSubmit2.offsetPosRight.z = compositeLayer.offsetPosRight.z;
                        layerSubmit2.offsetRotLeft.x = compositeLayer.offsetRotLeft.x;
                        layerSubmit2.offsetRotLeft.y = compositeLayer.offsetRotLeft.y;
                        layerSubmit2.offsetRotLeft.z = compositeLayer.offsetRotLeft.z;
                        layerSubmit2.offsetRotLeft.w = compositeLayer.offsetRotLeft.w;
                        layerSubmit2.offsetRotRight.x = compositeLayer.offsetRotRight.x;
                        layerSubmit2.offsetRotRight.y = compositeLayer.offsetRotRight.y;
                        layerSubmit2.offsetRotRight.z = compositeLayer.offsetRotRight.z;
                        layerSubmit2.offsetRotRight.w = compositeLayer.offsetRotRight.w;
                        layerSubmit2.degreeType = (uint)compositeLayer.eacModelType;
                        layerSubmit2.overlapFactor = compositeLayer.overlapFactor;
                        layerSubmit2.timestamp = compositeLayer.timestamp;

                        if (compositeLayer.layerSubmitPtr != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(compositeLayer.layerSubmitPtr);
                            compositeLayer.layerSubmitPtr = IntPtr.Zero;
                        }
                        compositeLayer.layerSubmitPtr = Marshal.AllocHGlobal(Marshal.SizeOf(layerSubmit2));
                        Marshal.StructureToPtr(layerSubmit2, compositeLayer.layerSubmitPtr, false);
                        PXR_Plugin.Render.UPxr_SubmitLayerEac2ByRender(compositeLayer.layerSubmitPtr);
                    }
                }
            }
        }
    }
}