#if PICO_OPENXR_SDK
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Unity.XR.CoreUtils;
using Unity.XR.PXR;
using UnityEditor;
using UnityEngine;

#if AR_FOUNDATION_5||AR_FOUNDATION_6
using UnityEngine.XR.ARSubsystems;
#endif
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.NativeTypes;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;
#endif

namespace Unity.XR.OpenXR.Features.PICOSupport
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "OpenXR Passthrough",
        Hidden = false,
        BuildTargetGroups = new[] { UnityEditor.BuildTargetGroup.Android },
        Company = "PICO",
        OpenxrExtensionStrings = extensionString,
        Version = PXR_Constants.SDKVersion,
        FeatureId = featureId)]
#endif
    public class PassthroughFeature : OpenXRFeatureBase
    {
        public const string featureId = "com.pico.openxr.feature.passthrough";
        public const string extensionString = "XR_FB_passthrough";
        public const int XR_PASSTHROUGH_COLOR_MAP_MONO_SIZE_FB = 256;
        private static byte[] colorData;
        private static uint Size = 0;
        private static bool isInit = false;
        private static bool isPause = false;
        private static int _enableVideoSeeThrough=-1;
        public static event Action<bool> EnableVideoSeeThroughAction;
        private static bool isRefreshRecenterSpace = false;
        public static bool isExtensionEnable => OpenXRRuntime.IsExtensionEnabled(extensionString);
        [HideInInspector]
        public static bool EnableVideoSeeThrough
        {
            get => _enableVideoSeeThrough==1;
            set
            {
                if (value)
                {
                    if (_enableVideoSeeThrough != 1)
                    {
                        _enableVideoSeeThrough = 1;
                        EnableSeeThroughManual(value);

                        if (EnableVideoSeeThroughAction != null)
                        {
                            EnableVideoSeeThroughAction(value);
                        }
                    }
                }
                else
                {
                    if (_enableVideoSeeThrough == 1)
                    {
                        _enableVideoSeeThrough = 0;
                        EnableSeeThroughManual(value);

                        if (EnableVideoSeeThroughAction != null)
                        {
                            EnableVideoSeeThroughAction(value);
                        }
                    }
                }
            }
        }
        protected override void OnSessionBegin(ulong xrSessionId)
        {
            if (!isRefreshRecenterSpace)
            {
                OpenXRSettings.RefreshRecenterSpace(); 
                isRefreshRecenterSpace = true;
            }
        }

        public override string GetExtensionString()
        {
            return extensionString;
        }

        public static void PassthroughStart()
        {
            passthroughStart();
            isPause = false;
        }

        public static void PassthroughPause()
        {
            passthroughPause();
            isPause = true;
        }
        
        //This interface has been changed to a private interface.
        //Please use the EnableVideoSeeThrough .
        private static bool EnableSeeThroughManual(bool value)
        {
            if (!isExtensionEnable)
            {
                return false;
            }

            if (!isInit)
            {
                isInit = initializePassthrough();
            }

            if (value)
            {
                createFullScreenLayer();
                if (!isPause)
                {
                    passthroughStart();
                }
            }
            else
            {
                passthroughPause();
            }

            return true;
        }

        public static void Destroy()
        {
            if (!isExtensionEnable)
            {
                return;
            }

            Passthrough_Destroy();
        }

        private void OnDestroy()
        {
            Destroy();
        }

        private static void AllocateColorMapData(uint size)
        {
            if (colorData != null && size != colorData.Length)
            {
                Clear();
            }

            if (colorData == null)
            {
                colorData = new byte[size];
            }
        }

        private static void Clear()
        {
            if (colorData != null)
            {
                colorData = null;
            }
        }

        private static void WriteVector3ToColorMap(int colorIndex, ref Vector3 color)
        {
            for (int c = 0; c < 3; c++)
            {
                byte[] bytes = BitConverter.GetBytes(color[c]);
                Buffer.BlockCopy(bytes, 0, colorData, colorIndex * 12 + c * 4, 4);
            }
        }

        private static void WriteFloatToColorMap(int index, float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, colorData, index * sizeof(float), sizeof(float));
        }

        private static void WriteColorToColorMap(int colorIndex, ref Color color)
        {
            for (int c = 0; c < 4; c++)
            {
                byte[] bytes = BitConverter.GetBytes(color[c]);
                Buffer.BlockCopy(bytes, 0, colorData, colorIndex * 16 + c * 4, 4);
            }
        }


        public static unsafe void SetBrightnessContrastSaturation(ref PassthroughStyle style, float brightness = 0.0f,
            float contrast = 0.0f, float saturation = 0.0f)
        {
            style.enableColorMap = true;
            style.TextureColorMapType = PassthroughColorMapType.BrightnessContrastSaturation;
            Size = 3 * sizeof(float);
            AllocateColorMapData(Size);
            WriteFloatToColorMap(0, brightness);

            WriteFloatToColorMap(1, contrast);

            WriteFloatToColorMap(2, saturation);
            fixed (byte* p = colorData)
            {
                style.TextureColorMapData = (IntPtr)p;
            }

            style.TextureColorMapDataSize = Size;
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < Size; i++)
            {
                str.Append(colorData[i]);
            }

            Debug.Log("SetPassthroughStyle SetBrightnessContrastSaturation colorData：" + str);
        }

        public static unsafe void SetColorMapbyMonoToMono(ref PassthroughStyle style, int[] values)
        {
            if (values.Length != XR_PASSTHROUGH_COLOR_MAP_MONO_SIZE_FB)
                throw new ArgumentException("Must provide exactly 256 values");
            style.enableColorMap = true;
            style.TextureColorMapType = PassthroughColorMapType.MonoToMono;
            Size = XR_PASSTHROUGH_COLOR_MAP_MONO_SIZE_FB * 4;
            AllocateColorMapData(Size);
            Buffer.BlockCopy(values, 0, colorData, 0, (int)Size);

            fixed (byte* p = colorData)
            {
                style.TextureColorMapData = (IntPtr)p;
            }

            style.TextureColorMapDataSize = Size;
        }

        public static unsafe void SetColorMapbyMonoToRgba(ref PassthroughStyle style, Color[] values)
        {
            if (values.Length != XR_PASSTHROUGH_COLOR_MAP_MONO_SIZE_FB)
                throw new ArgumentException("Must provide exactly 256 colors");

            style.TextureColorMapType = PassthroughColorMapType.MonoToRgba;
            style.enableColorMap = true;
            Size = XR_PASSTHROUGH_COLOR_MAP_MONO_SIZE_FB * 4 * 4;

            AllocateColorMapData(Size);

            for (int i = 0; i < XR_PASSTHROUGH_COLOR_MAP_MONO_SIZE_FB; i++)
            {
                WriteColorToColorMap(i, ref values[i]);
            }

            fixed (byte* p = colorData)
            {
                style.TextureColorMapData = (IntPtr)p;
            }

            style.TextureColorMapDataSize = Size;
        }

        public static _PassthroughStyle ToPassthroughStyle(PassthroughStyle c)
        {
            _PassthroughStyle mPassthroughStyle = new _PassthroughStyle();
            mPassthroughStyle.enableEdgeColor = (uint)(c.enableEdgeColor ? 1 : 0);
            mPassthroughStyle.enableColorMap = (uint)(c.enableColorMap ? 1 : 0);
            mPassthroughStyle.TextureOpacityFactor = c.TextureOpacityFactor;
            mPassthroughStyle.TextureColorMapType = c.TextureColorMapType;
            mPassthroughStyle.TextureColorMapDataSize = c.TextureColorMapDataSize;
            mPassthroughStyle.TextureColorMapData = c.TextureColorMapData;
            mPassthroughStyle.EdgeColor = new Colorf()
                { r = c.EdgeColor.r, g = c.EdgeColor.g, b = c.EdgeColor.b, a = c.EdgeColor.a };
            return mPassthroughStyle;
        }

        public static void SetPassthroughStyle(PassthroughStyle style)
        {
            setPassthroughStyle(ToPassthroughStyle(style));
        }

        public static bool IsPassthroughSupported()
        {
            return isPassthroughSupported();
        }


        public static unsafe bool CreateTriangleMesh(int id, Vector3[] vertices, int[] triangles,
            GeometryInstanceTransform transform)
        {
            if (vertices == null || triangles == null || vertices.Length == 0 || triangles.Length == 0)
            {
                return false;
            }

            if (!isInit)
            {
                isInit = initializePassthrough();
            }

            int vertexCount = vertices.Length;
            int triangleCount = triangles.Length;

            Size = (uint)vertexCount * 3 * 4;

            AllocateColorMapData(Size);

            for (int i = 0; i < vertexCount; i++)
            {
                WriteVector3ToColorMap(i, ref vertices[i]);
            }

            IntPtr vertexDataPtr = IntPtr.Zero;

            fixed (byte* p = colorData)
            {
                vertexDataPtr = (IntPtr)p;
            }

            StringBuilder str = new StringBuilder();
            for (int i = 0; i < 3 * 4; i++)
            {
                str.Append(colorData[i]);
            }

            Debug.Log("CreateTriangleMesh vertexDataPtr colorData：" + str);
            str.Clear();

            Size = (uint)triangleCount * 4;
            AllocateColorMapData(Size);
            Buffer.BlockCopy(triangles, 0, colorData, 0, (int)Size);
            IntPtr triangleDataPtr = IntPtr.Zero;
            fixed (byte* p = colorData)
            {
                triangleDataPtr = (IntPtr)p;
            }

            for (int i = 0; i < colorData.Length; i++)
            {
                str.Append(colorData[i]);
            }

            // Debug.Log("CreateTriangleMesh triangleDataPtr colorData：" + str);
            //
            // Debug.Log("CreateTriangleMesh  vertexDataPtr=" + vertexDataPtr + "  vertexCount=" + vertexCount);
            // Debug.Log("CreateTriangleMesh triangleDataPtr=" + triangleDataPtr + "  triangleCount=" + triangleCount);

            XrResult result =
                createTriangleMesh(id, vertexDataPtr, vertexCount, triangleDataPtr, triangleCount, transform);
            Clear();
            if (result == XrResult.Success)
            {
                return true;
            }

            return false;
        }

        public static void UpdateMeshTransform(int id, GeometryInstanceTransform transform)
        {
            updatePassthroughMeshTransform(id, transform);
        }
        
        
#if UNITY_EDITOR
        /// <summary>
        /// Validation Rules for ARCameraFeature.
        /// </summary>
        protected override void GetValidationChecks(List<ValidationRule> rules, BuildTargetGroup targetGroup)
        {
            var AdditionalRules = new ValidationRule[]
            {
                new ValidationRule(this)
                {
                    message = "Passthrough requires Camera clear flags set to solid color with alpha value zero.",
                    checkPredicate = () =>
                    {
                        
                        var xrOrigin = FindObjectsOfType<XROrigin>();
                    
                        if (xrOrigin != null && xrOrigin.Length > 0)
                        {
                            if (!xrOrigin[0].enabled) return true;
                        }
                        else
                        {
                            return true;
                        }

                        var camera = xrOrigin[0].Camera;
                        if (camera == null) return true;

                        return camera.clearFlags == CameraClearFlags.SolidColor && Mathf.Approximately(camera.backgroundColor.a, 0);
                    },
                    fixItAutomatic = true,
                    fixItMessage = "Set your XR Origin camera's Clear Flags to solid color with alpha value zero.",
                    fixIt = () =>
                    {
                        var xrOrigin = FindObjectsOfType<XROrigin>();
                        if (xrOrigin!=null&&xrOrigin.Length>0)
                        {
                            if (xrOrigin[0].enabled)
                            {
                                var camera = xrOrigin[0].Camera;
                                if (camera != null )
                                {
                                    camera.clearFlags = CameraClearFlags.SolidColor;
                                    Color clearColor = camera.backgroundColor;
                                    clearColor.a = 0;
                                    camera.backgroundColor = clearColor;
                                }
                            }
                        }
                        
                    },
                    error = false
                }
            };

            rules.AddRange(AdditionalRules);
        }
#endif
        
#if AR_FOUNDATION_5||AR_FOUNDATION_6
        public bool isCameraSubsystem=false;
        static List<XRCameraSubsystemDescriptor> s_CameraDescriptors = new List<XRCameraSubsystemDescriptor>();
        protected override void OnSubsystemCreate()
        {
            base.OnSubsystemCreate();
            if (isCameraSubsystem)
            {
                CreateSubsystem<XRCameraSubsystemDescriptor, XRCameraSubsystem>(
                    s_CameraDescriptors,
                    PXR_CameraSubsystem.k_SubsystemId);
            }

        }
        protected override void OnSubsystemStart()
        {
            if (isCameraSubsystem)
            {
                StartSubsystem<XRCameraSubsystem>();
            }
        }
        protected override void OnSubsystemStop()
        {
            if (isCameraSubsystem)
            {
                StopSubsystem<XRCameraSubsystem>();
            }
        }
        protected override void OnSubsystemDestroy()
        {
            if (isCameraSubsystem)
            {
                DestroySubsystem<XRCameraSubsystem>();
            }
        }
      
#endif
        protected override void OnSessionStateChange(int oldState, int newState)
        {
            base.OnSessionStateChange(oldState, newState);
            if (newState == 1)
            {
#if AR_FOUNDATION_5||AR_FOUNDATION_6
                if (isCameraSubsystem)
                {
                    StopSubsystem<XRCameraSubsystem>();
                }else{
                    if (_enableVideoSeeThrough!=-1)
                    {
                        EnableSeeThroughManual(false);
                    }
                }
#else
                if (_enableVideoSeeThrough!=-1)
                {
                    EnableSeeThroughManual(false);
                }
#endif
            }
            else if (newState == 5)
            {
#if AR_FOUNDATION_5||AR_FOUNDATION_6
                if (isCameraSubsystem)
                {
                    StartSubsystem<XRCameraSubsystem>();
                }else{
                    if (_enableVideoSeeThrough!=-1)
                    {
                        EnableSeeThroughManual(EnableVideoSeeThrough);
                    }
                }
#else
                if (_enableVideoSeeThrough!=-1)
                {
                    EnableSeeThroughManual(EnableVideoSeeThrough);
                }
               
#endif
            }
        }
       
        

        [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, EntryPoint = "PICO_InitializePassthrough", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool initializePassthrough();

        [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, EntryPoint = "PICO_CreateFullScreenLayer", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool createFullScreenLayer();

        [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, EntryPoint = "PICO_PassthroughStart", CallingConvention = CallingConvention.Cdecl)]
        private static extern void passthroughStart();

        [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, EntryPoint = "PICO_PassthroughPause", CallingConvention = CallingConvention.Cdecl)]
        private static extern void passthroughPause();

        [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, EntryPoint = "PICO_SetPassthroughStyle", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setPassthroughStyle(_PassthroughStyle style);

        [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, EntryPoint = "PICO_IsPassthroughSupported", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool isPassthroughSupported();

        [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, EntryPoint = "PICO_Passthrough_Destroy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Passthrough_Destroy();

        [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, EntryPoint = "PICO_CreateTriangleMesh", CallingConvention = CallingConvention.Cdecl)]
        private static extern XrResult createTriangleMesh(int id, IntPtr vertices, int vertexCount, IntPtr triangles,
            int triangleCount, GeometryInstanceTransform transform);

        [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, EntryPoint = "PICO_UpdatePassthroughMeshTransform",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void updatePassthroughMeshTransform(int id, GeometryInstanceTransform transform);
    }
}
#endif