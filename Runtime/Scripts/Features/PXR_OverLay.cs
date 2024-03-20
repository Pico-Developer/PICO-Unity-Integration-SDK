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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace Unity.XR.PXR
{
    public class PXR_OverLay : MonoBehaviour, IComparable<PXR_OverLay>
    {
        private const string TAG = "[PXR_CompositeLayers]";
        public static List<PXR_OverLay> Instances = new List<PXR_OverLay>();

        private static int overlayID = 0;
        [NonSerialized]
        public int overlayIndex;
        public int layerDepth;
        public int imageIndex = 0;
        public OverlayType overlayType = OverlayType.Overlay;
        public OverlayShape overlayShape = OverlayShape.Quad;
        public TextureType textureType = TextureType.ExternalSurface;
        public Transform overlayTransform;
        public Camera xrRig;

        public Texture[] layerTextures = new Texture[2] { null, null };

        public bool isPremultipliedAlpha = false;
        public bool isDynamic = false;
        public int[] overlayTextureIds = new int[2];
        public Matrix4x4[] mvMatrixs = new Matrix4x4[2];
        public Vector3[] modelScales = new Vector3[2];
        public Quaternion[] modelRotations = new Quaternion[2];
        public Vector3[] modelTranslations = new Vector3[2];
        public Quaternion[] cameraRotations = new Quaternion[2];
        public Vector3[] cameraTranslations = new Vector3[2];
        public Camera[] overlayEyeCamera = new Camera[2];

        public bool overrideColorScaleAndOffset = false;
        public Vector4 colorScale = Vector4.one;
        public Vector4 colorOffset = Vector4.zero;

        // Eac
        public Vector3 offsetPosLeft = Vector3.zero;
        public Vector3 offsetPosRight = Vector3.zero;
        public Vector4 offsetRotLeft = new Vector4(0, 0, 0, 1);
        public Vector4 offsetRotRight = new Vector4(0, 0, 0, 1);
        public EACModelType eacModelType = EACModelType.Eac360;
        public float overlapFactor = 1.0f;
        public ulong timestamp = 0;

        private Vector4 overlayLayerColorScaleDefault = Vector4.one;
        private Vector4 overlayLayerColorOffsetDefault = Vector4.zero;

        public bool isExternalAndroidSurface = false;
        public bool isExternalAndroidSurfaceDRM = false;
        public Surface3DType externalAndroidSurface3DType = Surface3DType.Single;
        public IntPtr externalAndroidSurfaceObject = IntPtr.Zero;
        public delegate void ExternalAndroidSurfaceObjectCreated();
        public ExternalAndroidSurfaceObjectCreated externalAndroidSurfaceObjectCreated = null;

        // 360 
        public float radius = 0; // >0

        // ImageRect
        public bool useImageRect = false;
        public TextureRect textureRect = TextureRect.StereoScopic;
        public DestinationRect destinationRect = DestinationRect.Default;
        public Rect srcRectLeft = new Rect(0, 0, 1, 1);
        public Rect srcRectRight = new Rect(0, 0, 1, 1);
        public Rect dstRectLeft = new Rect(0, 0, 1, 1);
        public Rect dstRectRight = new Rect(0, 0, 1, 1);

        public PxrRecti imageRectLeft;
        public PxrRecti imageRectRight;


        // LayerBlend
        public bool useLayerBlend = false;
        public PxrBlendFactor srcColor = PxrBlendFactor.PxrBlendFactorOne;
        public PxrBlendFactor dstColor = PxrBlendFactor.PxrBlendFactorOne;
        public PxrBlendFactor srcAlpha = PxrBlendFactor.PxrBlendFactorOne;
        public PxrBlendFactor dstAlpha = PxrBlendFactor.PxrBlendFactorOne;
        public float[] colorMatrix = new float[18] {
            1,0,0, // left
            0,1,0,
            0,0,1,
            1,0,0, // right
            0,1,0,
            0,0,1,
        };

        public bool isClones = false;
        public bool isClonesToNew = false;
        public PXR_OverLay originalOverLay;
        public IntPtr layerSubmitPtr = IntPtr.Zero;
        public APIExecutionStatus Quad2Status = APIExecutionStatus.None;
        public APIExecutionStatus Cylinder2Status = APIExecutionStatus.None;
        public APIExecutionStatus Equirect2Status = APIExecutionStatus.None;

        private bool toCreateSwapChain = false;
        private bool toCopyRT = false;
        private bool copiedRT = false;
        private int eyeCount = 2;
        private UInt32 imageCounts = 0;
        private PxrLayerParam overlayParam = new PxrLayerParam();
        private struct NativeTexture
        {
            public Texture[] textures;
        };
        private NativeTexture[] nativeTextures;
        private static Material cubeM;
        private IntPtr leftPtr = IntPtr.Zero;
        private IntPtr rightPtr = IntPtr.Zero;
        private static Material textureM;

        public HDRFlags hdr = HDRFlags.None;

        public int CompareTo(PXR_OverLay other)
        {
            return layerDepth.CompareTo(other.layerDepth);
        }

        protected void Awake()
        {
            xrRig = Camera.main;
            Instances.Add(this);
            if (null == xrRig.gameObject.GetComponent<PXR_OverlayManager>())
            {
                xrRig.gameObject.AddComponent<PXR_OverlayManager>();
            }

            overlayEyeCamera[0] = xrRig;
            overlayEyeCamera[1] = xrRig;

            overlayTransform = GetComponent<Transform>();
#if UNITY_ANDROID && !UNITY_EDITOR
            if (overlayTransform != null)
            {
                MeshRenderer render = overlayTransform.GetComponent<MeshRenderer>();
                if (render != null)
                {
                    render.enabled = false;
                }
            }
#endif

            if (!isClones)
            {
                InitializeBuffer();
            }
        }

        private void Start()
        {
            if (isClones)
            {
                InitializeBuffer();
            }

            if (PXR_Manager.Instance == null)
            {
                return;
            }

            Camera[] cam = PXR_Manager.Instance.GetEyeCamera();
            if (cam[0] != null && cam[0].enabled)
            {
                RefreshCamera(cam[0], cam[0]);
            }
            else if (cam[1] != null && cam[2] != null)
            {
                RefreshCamera(cam[1], cam[2]);
            }
        }

        public void RefreshCamera(Camera leftCamera, Camera rightCamera)
        {
            overlayEyeCamera[0] = leftCamera;
            overlayEyeCamera[1] = rightCamera;
        }

        private void InitializeBuffer()
        {
            if (!isExternalAndroidSurface)
            {
                if (null == layerTextures[0] && null == layerTextures[1])
                {
                    PLog.e(TAG, "  The left and right images are all empty!");
                    return;
                }
                else if (null == layerTextures[0] && null != layerTextures[1])
                {
                    layerTextures[0] = layerTextures[1];
                }
                else if (null != layerTextures[0] && null == layerTextures[1])
                {
                    layerTextures[1] = layerTextures[0];
                }
                overlayParam.width = (uint)layerTextures[1].width;
                overlayParam.height = (uint)layerTextures[1].height;
            }
            else
            {
                overlayParam.width = 1024;
                overlayParam.height = 1024;
            }

            overlayID++;
            overlayIndex = overlayID;
            overlayParam.layerId = overlayIndex;
            overlayParam.layerShape = overlayShape == 0 ? OverlayShape.Quad : overlayShape;
            overlayParam.layerType = overlayType;
            overlayParam.arraySize = 1;
            overlayParam.mipmapCount = 1;
            overlayParam.sampleCount = 1;
            overlayParam.layerFlags = 0;

            if (GraphicsDeviceType.Vulkan == SystemInfo.graphicsDeviceType)
            {
                overlayParam.format = QualitySettings.activeColorSpace == ColorSpace.Linear ? (UInt64)ColorForamt.VK_FORMAT_R8G8B8A8_SRGB : (UInt64)RenderTextureFormat.Default;
            }
            else
            {
                overlayParam.format = QualitySettings.activeColorSpace == ColorSpace.Linear ? (UInt64)ColorForamt.GL_SRGB8_ALPHA8 : (UInt64)RenderTextureFormat.Default;
            }

            if (OverlayShape.Cubemap == overlayShape)
            {
                overlayParam.faceCount = 6;
                if (cubeM == null)
                    cubeM = new Material(Shader.Find("PXR_SDK/PXR_CubemapBlit"));
            }
            else
            {
                overlayParam.faceCount = 1;
                if (textureM == null)
                    textureM = new Material(Shader.Find("PXR_SDK/PXR_Texture2DBlit"));
            }

            if (isClones)
            {
                if (null != originalOverLay)
                {
                    overlayParam.layerFlags |= (UInt32)PxrLayerCreateFlags.PxrLayerFlagSharedImagesBetweenLayers;
                    leftPtr = Marshal.AllocHGlobal(Marshal.SizeOf(originalOverLay.overlayIndex));
                    rightPtr = Marshal.AllocHGlobal(Marshal.SizeOf(originalOverLay.overlayIndex));
                    Marshal.WriteInt64(leftPtr, originalOverLay.overlayIndex);
                    Marshal.WriteInt64(rightPtr, originalOverLay.overlayIndex);
                    overlayParam.leftExternalImages = leftPtr;
                    overlayParam.rightExternalImages = rightPtr;
                    isExternalAndroidSurface = originalOverLay.isExternalAndroidSurface;
                    isDynamic = originalOverLay.isDynamic;
                    overlayParam.width = (UInt32)Mathf.Min(overlayParam.width, originalOverLay.overlayParam.width);
                    overlayParam.height = (UInt32)Mathf.Min(overlayParam.height, originalOverLay.overlayParam.height);
                }
                else
                {
                    PLog.e(TAG, "In clone state, originalOverLay cannot be empty!");
                }
            }

            if (isExternalAndroidSurface)
            {
                if (isExternalAndroidSurfaceDRM)
                {
                    overlayParam.layerFlags |= (UInt32)(PxrLayerCreateFlags.PxrLayerFlagAndroidSurface | PxrLayerCreateFlags.PxrLayerFlagProtectedContent);
                }
                else
                {
                    overlayParam.layerFlags |= (UInt32)PxrLayerCreateFlags.PxrLayerFlagAndroidSurface;
                }

                if (Surface3DType.LeftRight == externalAndroidSurface3DType)
                {
                    overlayParam.layerFlags |= (UInt32)PxrLayerCreateFlags.PxrLayerFlag3DLeftRightSurface;
                }
                else if (Surface3DType.TopBottom == externalAndroidSurface3DType)
                {
                    overlayParam.layerFlags |= (UInt32)PxrLayerCreateFlags.PxrLayerFlag3DTopBottomSurface;
                }

                overlayParam.layerLayout = LayerLayout.Mono;

                PLog.i(TAG, $"UPxr_CreateLayer() overlayParam.layerId={overlayParam.layerId}, layerShape={overlayParam.layerShape}, layerType={overlayParam.layerType}, width={overlayParam.width}, height={overlayParam.height}, layerFlags={overlayParam.layerFlags}, format={overlayParam.format}, layerLayout={overlayParam.layerLayout}.");
                IntPtr layerParamPtr = Marshal.AllocHGlobal(Marshal.SizeOf(overlayParam));
                Marshal.StructureToPtr(overlayParam, layerParamPtr, false);
                PXR_Plugin.Render.UPxr_CreateLayer(layerParamPtr);
                Marshal.FreeHGlobal(layerParamPtr);
            }
            else
            {
                if (!isDynamic)
                {
                    overlayParam.layerFlags |= (UInt32)PxrLayerCreateFlags.PxrLayerFlagStaticImage;
                }

                if ((layerTextures[0] != null && layerTextures[1] != null && layerTextures[0] == layerTextures[1]) || null == layerTextures[1])
                {
                    eyeCount = 1;
                    overlayParam.layerLayout = LayerLayout.Mono;
                }
                else
                {
                    eyeCount = 2;
                    overlayParam.layerLayout = LayerLayout.Stereo;
                }

                PXR_Plugin.Render.UPxr_CreateLayerParam(overlayParam);
                toCreateSwapChain = true;
                CreateTexture();
            }
        }

        public void CreateExternalSurface(PXR_OverLay overlayInstance)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (IntPtr.Zero != overlayInstance.externalAndroidSurfaceObject)
            {
                return;
            }

            PXR_Plugin.Render.UPxr_GetLayerAndroidSurface(overlayInstance.overlayIndex, 0, ref overlayInstance.externalAndroidSurfaceObject);
            PLog.i(TAG, string.Format("CreateExternalSurface: Overlay Type:{0}, LayerDepth:{1}, SurfaceObject:{2}", overlayInstance.overlayType, overlayInstance.overlayIndex, overlayInstance.externalAndroidSurfaceObject));
            
            if (IntPtr.Zero == overlayInstance.externalAndroidSurfaceObject || null == overlayInstance.externalAndroidSurfaceObjectCreated)
            {
                return;
            }
            
            overlayInstance.externalAndroidSurfaceObjectCreated();
#endif
        }

        public void UpdateCoords()
        {
            if (null == overlayTransform || !overlayTransform.gameObject.activeSelf || null == overlayEyeCamera[0] || null == overlayEyeCamera[1])
            {
                return;
            }

            for (int i = 0; i < mvMatrixs.Length; i++)
            {
                mvMatrixs[i] = overlayEyeCamera[i].worldToCameraMatrix * overlayTransform.localToWorldMatrix;
                if (overlayTransform is RectTransform uiTransform)
                {
                    var rect = uiTransform.rect;
                    var lossyScale = overlayTransform.lossyScale;
                    modelScales[i] = new Vector3(rect.width * lossyScale.x,
                        rect.height * lossyScale.y, 1);
                    modelTranslations[i] = uiTransform.TransformPoint(rect.center);
                }
                else
                {
                    modelScales[i] = overlayTransform.lossyScale;
                    modelTranslations[i] = overlayTransform.position;
                }
                modelRotations[i] = overlayTransform.rotation;
                cameraRotations[i] = overlayEyeCamera[i].transform.rotation;
                cameraTranslations[i] = overlayEyeCamera[i].transform.position;
            }
        }

        public bool CreateTexture()
        {
            if (!toCreateSwapChain)
            {
                return false;
            }

            if (null == nativeTextures)
                nativeTextures = new NativeTexture[eyeCount];

            for (int i = 0; i < eyeCount; i++)
            {
                int ret = PXR_Plugin.Render.UPxr_GetLayerImageCount(overlayIndex, (EyeType)i, ref imageCounts);
                if (ret != 0 || imageCounts < 1)
                {
                    return false;
                }

                if (null == nativeTextures[i].textures)
                {
                    nativeTextures[i].textures = new Texture[imageCounts];
                }

                for (int j = 0; j < imageCounts; j++)
                {
                    IntPtr ptr = IntPtr.Zero;
                    PXR_Plugin.Render.UPxr_GetLayerImagePtr(overlayIndex, (EyeType)i, j, ref ptr);

                    if (IntPtr.Zero == ptr)
                    {
                        return false;
                    }

                    Texture texture;
                    if (OverlayShape.Cubemap == overlayShape)
                    {
                        texture = Cubemap.CreateExternalTexture((int)overlayParam.width, TextureFormat.RGBA32, false, ptr);
                    }
                    else
                    {
                        texture = Texture2D.CreateExternalTexture((int)overlayParam.width, (int)overlayParam.height, TextureFormat.RGBA32, false, true, ptr);
                    }

                    if (null == texture)
                    {
                        return false;
                    }

                    nativeTextures[i].textures[j] = texture;
                }
            }

            toCreateSwapChain = false;
            toCopyRT = true;
            copiedRT = false;

            FreePtr();

            return true;
        }

        public bool CopyRT()
        {
            if (isClones)
            {
                return true;
            }

            if (!toCopyRT)
            {
                return copiedRT;
            }

            if (!isDynamic && copiedRT)
            {
                return copiedRT;
            }

            if (null == nativeTextures)
            {
                return false;
            }
            if (GraphicsDeviceType.Vulkan != SystemInfo.graphicsDeviceType)
            {
                PXR_Plugin.Render.UPxr_GetLayerNextImageIndexByRender(overlayIndex, ref imageIndex);
            }
            for (int i = 0; i < eyeCount; i++)
            {
                Texture nativeTexture = nativeTextures[i].textures[imageIndex];

                if (null == nativeTexture || null == layerTextures[i])
                    continue;

                RenderTexture texture = layerTextures[i] as RenderTexture;

                if (OverlayShape.Cubemap == overlayShape && null == layerTextures[i] as Cubemap)
                {
                    return false;
                }

                for (int f = 0; f < (int)overlayParam.faceCount; f++)
                {
                    if (QualitySettings.activeColorSpace == ColorSpace.Gamma && texture != null && texture.format == RenderTextureFormat.ARGB32)
                    {
                        Graphics.CopyTexture(layerTextures[i], f, 0, nativeTexture, f, 0);
                    }
                    else
                    {
                        RenderTextureDescriptor rtDes = new RenderTextureDescriptor((int)overlayParam.width, (int)overlayParam.height, RenderTextureFormat.ARGB32, 0);
                        rtDes.msaaSamples = (int)overlayParam.sampleCount;
                        rtDes.useMipMap = true;
                        rtDes.autoGenerateMips = false;
                        rtDes.sRGB = true;

                        RenderTexture renderTexture = RenderTexture.GetTemporary(rtDes);

                        if (!renderTexture.IsCreated())
                        {
                            renderTexture.Create();
                        }
                        renderTexture.DiscardContents();

                        if (OverlayShape.Cubemap == overlayShape)
                        {
                            cubeM.SetInt("_d", f);
                            Graphics.Blit(layerTextures[i], renderTexture, cubeM);
                        }
                        else
                        {
                            textureM.mainTexture = texture;
                            textureM.SetPass(0);
                            textureM.SetInt("_premultiply", isPremultipliedAlpha ? 1 : 0);
                            Graphics.Blit(layerTextures[i], renderTexture, textureM);
                        }
                        Graphics.CopyTexture(renderTexture, 0, 0, nativeTexture, f, 0);
                        RenderTexture.ReleaseTemporary(renderTexture);
                    }
                }
                copiedRT = true;
            }

            return copiedRT;
        }

        public void SetTexture(Texture texture, bool dynamic)
        {
            if (isExternalAndroidSurface)
            {
                PLog.w(TAG, "Not support setTexture !");
                return;
            }

            if (isClones)
            {
                return;
            }
            else
            {
                foreach (PXR_OverLay overlay in PXR_OverLay.Instances)
                {
                    if (overlay.isClones && null != overlay.originalOverLay && overlay.originalOverLay.overlayIndex == overlayIndex)
                    {
                        overlay.DestroyLayer();
                        overlay.isClonesToNew = true;
                    }
                }
            }

            toCopyRT = false;
            PXR_Plugin.Render.UPxr_DestroyLayerByRender(overlayIndex);
            ClearTexture();
            for (int i = 0; i < layerTextures.Length; i++)
            {
                layerTextures[i] = texture;
            }

            isDynamic = dynamic;
            InitializeBuffer();

            if (!isClones)
            {
                foreach (PXR_OverLay overlay in PXR_OverLay.Instances)
                {
                    if (overlay.isClones && overlay.isClonesToNew)
                    {
                        overlay.originalOverLay = this;
                        overlay.InitializeBuffer();
                        overlay.isClonesToNew = false;
                    }
                }
            }
        }

        private void FreePtr()
        {
            if (leftPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(leftPtr);
                leftPtr = IntPtr.Zero;
            }

            if (rightPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(rightPtr);
                rightPtr = IntPtr.Zero;
            }

            if (layerSubmitPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(layerSubmitPtr);
                layerSubmitPtr = IntPtr.Zero;
            }
        }

        public void OnDestroy()
        {
            DestroyLayer();
            Instances.Remove(this);
        }

        public void DestroyLayer()
        {
            if (!isClones)
            {
                List<PXR_OverLay> toDestroyClones = new List<PXR_OverLay>();
                foreach (PXR_OverLay overlay in Instances)
                {
                    if (overlay.isClones && null != overlay.originalOverLay && overlay.originalOverLay.overlayIndex == overlayIndex)
                    {
                        toDestroyClones.Add(overlay);
                    }
                }

                foreach (PXR_OverLay overLay in toDestroyClones)
                {
                    PXR_Plugin.Render.UPxr_DestroyLayerByRender(overLay.overlayIndex);
                    ClearTexture();
                }

                PXR_Plugin.Render.UPxr_DestroyLayerByRender(overlayIndex);
            }
            else
            {
                if (null != originalOverLay && Instances.Contains(originalOverLay))
                {
                    PXR_Plugin.Render.UPxr_DestroyLayerByRender(overlayIndex);
                }
            }
            ClearTexture();

            if (isExternalAndroidSurface)
            {
                externalAndroidSurfaceObject = IntPtr.Zero;
            }
        }

        private void ClearTexture()
        {
            FreePtr();

            if (isExternalAndroidSurface || null == nativeTextures || isClones)
            {
                return;
            }

            for (int i = 0; i < eyeCount; i++)
            {
                if (null == nativeTextures[i].textures)
                {
                    continue;
                }

                for (int j = 0; j < imageCounts; j++)
                    DestroyImmediate(nativeTextures[i].textures[j]);
            }

            nativeTextures = null;
        }

        public void SetLayerColorScaleAndOffset(Vector4 scale, Vector4 offset)
        {
            colorScale = scale;
            colorOffset = offset;
        }

        public void SetEACOffsetPosAndRot(Vector3 leftPos, Vector3 rightPos, Vector4 leftRot, Vector4 rightRot)
        {
            offsetPosLeft = leftPos;
            offsetPosRight = rightPos;
            offsetRotLeft = leftRot;
            offsetRotRight = rightRot;
        }

        public void SetEACFactor(float factor)
        {
            overlapFactor = factor;
        }

        public Vector4 GetLayerColorScale()
        {
            if (!overrideColorScaleAndOffset)
            {
                return overlayLayerColorScaleDefault;
            }
            return colorScale;
        }

        public Vector4 GetLayerColorOffset()
        {
            if (!overrideColorScaleAndOffset)
            {
                return overlayLayerColorOffsetDefault;
            }
            return colorOffset;
        }

        public PxrRecti getPxrRectiLeft(bool left)
        {
            if (left)
            {
                imageRectLeft.x = (int)(overlayParam.width * srcRectLeft.x);
                imageRectLeft.y = (int)(overlayParam.height * srcRectLeft.y);
                imageRectLeft.width = (int)(overlayParam.width * Mathf.Min(srcRectLeft.width, 1 - srcRectLeft.x));
                imageRectLeft.height = (int)(overlayParam.height * Mathf.Min(srcRectLeft.height, 1 - srcRectLeft.y));
                return imageRectLeft;
            }
            else
            {
                imageRectRight.x = (int)(overlayParam.width * srcRectRight.x);
                imageRectRight.y = (int)(overlayParam.height * srcRectRight.y);
                imageRectRight.width = (int)(overlayParam.width * Mathf.Min(srcRectRight.width, 1 - srcRectRight.x));
                imageRectRight.height = (int)(overlayParam.height * Mathf.Min(srcRectRight.height, 1 - srcRectRight.y));
                return imageRectRight;
            }
        }

        public UInt32 getHDRFlags()
        {
            UInt32 hdrFlags = 0;
            if (!isExternalAndroidSurface)
            {
                return hdrFlags;
            }
            switch (hdr)
            {
                case HDRFlags.HdrPQ:
                    hdrFlags |= (UInt32)PxrLayerSubmitFlags.PxrLayerFlagColorSpaceHdrPQ;
                    break;
                case HDRFlags.HdrHLG:
                    hdrFlags |= (UInt32)PxrLayerSubmitFlags.PxrLayerFlagColorSpaceHdrHLG;
                    break;
                default:
                    break;
            }
            return hdrFlags;
        }

        public enum HDRFlags
        {
            None,
            HdrPQ,
            HdrHLG,
        }

        public enum OverlayShape
        {
            Quad = 1,
            Cylinder = 2,
            Equirect = 3,
            Cubemap = 5,
            Eac = 6
        }

        public enum OverlayType
        {
            Overlay = 0,
            Underlay = 1
        }

        public enum TextureType
        {
            ExternalSurface,
            DynamicTexture,
            StaticTexture
        }

        public enum LayerLayout
        {
            Stereo = 0,
            DoubleWide = 1,
            Array = 2,
            Mono = 3
        }

        public enum Surface3DType
        {
            Single = 0,
            LeftRight,
            TopBottom
        }

        public enum TextureRect
        {
            MonoScopic,
            StereoScopic,
            Custom
        }

        public enum DestinationRect
        {
            Default,
            Custom
        }

        public enum EACModelType
        {
            Eac360 = 0,
            Eac360ViewPort = 1,
            Eac180 = 4,
            Eac180ViewPort = 5,
        }

        public enum ColorForamt
        {
            VK_FORMAT_R8G8B8A8_UNORM = 37,
            VK_FORMAT_R8G8B8A8_SRGB = 43,
            GL_SRGB8_ALPHA8 = 0x8c43,
            GL_RGBA8 = 0x8058
        }

        public enum APIExecutionStatus
        {
            None,
            True,
            False
        }
    }
}