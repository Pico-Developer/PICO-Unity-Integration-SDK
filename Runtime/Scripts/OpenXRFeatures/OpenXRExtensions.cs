#if PICO_OPENXR_SDK
using System.Collections.Generic;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using Object = System.Object;
using UnityEngine.XR.OpenXR.Features.Interactions;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;
using Unity.XR.PXR;
using UnityEngine.XR;


#if UNITY_EDITOR
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

#if AR_FOUNDATION_5||AR_FOUNDATION_6
using UnityEngine.XR.ARSubsystems;
#endif
namespace Unity.XR.OpenXR.Features.PICOSupport
{
#if UNITY_EDITOR
    public class ExtensionsConfig
    {
        public const string OpenXrExtensionList = "XR_FB_composition_layer_alpha_blend " +
                                                  "XR_FB_triangle_mesh " +
                                                  "XR_KHR_composition_layer_color_scale_bias " +
                                                  "XR_KHR_composition_layer_cylinder " +
                                                  "XR_KHR_composition_layer_equirect2 " +
                                                  "XR_KHR_composition_layer_cube " +
                                                  "XR_BD_composition_layer_eac " +
                                                  "XR_BD_composition_layer_fisheye " +
                                                  "XR_BD_composition_layer_blurred_quad " +
                                                  "XR_KHR_android_surface_swapchain " +
                                                  "XR_BD_composition_layer_color_matrix " +
                                                  "XR_BD_composition_layer_settings " +
                                                  "XR_KHR_composition_layer_depth ";
    }

    [OpenXRFeature(UiName = "PICO OpenXR Features",
        Desc = "PICO XR Features for OpenXR.",
        Company = "PICO",
        Priority = 100,
        Version = PXR_Constants.SDKVersion,
        BuildTargetGroups = new[] { BuildTargetGroup.Android },
        OpenxrExtensionStrings = ExtensionsConfig.OpenXrExtensionList,
        FeatureId = featureId
    )]
#endif
    public class OpenXRExtensions : OpenXRFeature
    {
        public const string featureId = "com.unity.openxr.pico.features";
        public const string PXR_PLATFORM_DLL = "PxrPlatform";
        private static ulong xrInstance = 0ul;
        private static ulong xrSession = 0ul;
        public static event Action<ulong> SenseDataUpdated;
        public static event Action SpatialAnchorDataUpdated;
        public static event Action SceneAnchorDataUpdated;
        
        public static event Action<PxrEventSenseDataProviderStateChanged> SenseDataProviderStateChanged;
        public static event Action<List<PxrSpatialMeshInfo>> SpatialMeshDataUpdated;
        
        static bool isCoroutineRunning = false;

        protected override bool OnInstanceCreate(ulong instance)
        {
            Debug.Log($"[PICOOpenXRExtensions] OnInstanceCreate: {instance}");
            xrInstance = instance;
            xrSession = 0ul;
            PICO_OnInstanceCreate(instance);
            return true;
        }

        protected override void OnSessionCreate(ulong xrSessionId)
        {
            Debug.Log($"[PICOOpenXRExtensions] OnSessionCreate: {xrSessionId}");
            xrSession = xrSessionId;
            PICO_OnSessionCreate(xrSessionId);
            PXR_Plugin.System.UPxr_SetXrEventDataBufferCallBack(XrEventDataBufferFunction);
        }

        public static int GetReferenceSpaceBoundsRect(XrReferenceSpaceType referenceSpace, ref XrExtent2Df extent2D)
        {
            return PICO_xrGetReferenceSpaceBoundsRect(
                xrSession, referenceSpace, ref extent2D);
        }

        public static XrReferenceSpaceType[] EnumerateReferenceSpaces()
        {
            UInt32 Output = 0;
            XrReferenceSpaceType[] outSpaces = null;
            PICO_xrEnumerateReferenceSpaces(xrSession, 0, ref Output, outSpaces);
            if (Output <= 0)
            {
                return null;
            }

            outSpaces = new XrReferenceSpaceType[Output];
            PICO_xrEnumerateReferenceSpaces(xrSession, Output, ref Output, outSpaces);
            return outSpaces;
        }

        
        [MonoPInvokeCallback(typeof(XrEventDataBufferCallBack))]
        static void XrEventDataBufferFunction(ref XrEventDataBuffer eventDB)
        {
            int status, action;
            Debug.Log($"XrEventDataBufferFunction eventType={eventDB.type}");
            switch (eventDB.type)
            {
                case XrStructureType.XR_TYPE_EVENT_DATA_SENSE_DATA_PROVIDER_STATE_CHANGED:
                {
                    if (SenseDataProviderStateChanged != null)
                    {
                        PxrEventSenseDataProviderStateChanged data = new PxrEventSenseDataProviderStateChanged()
                        {
                            providerHandle = BitConverter.ToUInt64(eventDB.data, 0),
                            newState = (PxrSenseDataProviderState)BitConverter.ToInt32(eventDB.data, 8),
                        };
                        SenseDataProviderStateChanged(data);
                    }

                    break;
                }
                case XrStructureType.XR_TYPE_EVENT_KEY_EVENT:
                {
                    if (PXR_Plugin.System.RecenterSuccess != null)
                    {
                        PXR_Plugin.System.RecenterSuccess();
                    }
                    break;
                }
                case XrStructureType.XR_TYPE_EVENT_DATA_SENSE_DATA_UPDATED:
                {
                    ulong providerHandle = BitConverter.ToUInt64(eventDB.data, 0);
                    PLog.i("EventDataFunction",$"providerHandle ={providerHandle}");
                    if (SenseDataUpdated != null)
                    {
                        SenseDataUpdated(providerHandle);
                    }

                    if (providerHandle == PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SpatialAnchor))
                    {
                        if (SpatialAnchorDataUpdated != null)
                        {
                            SpatialAnchorDataUpdated();
                        }
                    }

                    if (providerHandle == PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SceneCapture))
                    {
                        if (SceneAnchorDataUpdated != null)
                        {
                            SceneAnchorDataUpdated();
                        }
                    }

                    if (providerHandle == PXR_Plugin.MixedReality.UPxr_GetSpatialMeshProviderHandle())
                    {
                        if (!isCoroutineRunning)
                        {
                            QuerySpatialMeshAnchor();
                        }
                    }

                    break;
                }
            }
        }


        static async void QuerySpatialMeshAnchor()
        {
            isCoroutineRunning = true;
            var task = await PXR_MixedReality.QueryMeshAnchorAsync();
            isCoroutineRunning = false;
            var (result, meshInfos) = task;
            for (int i = 0; i < meshInfos.Count; i++)
            {
                switch (meshInfos[i].state)
                {
                    case MeshChangeState.Added:
                    case MeshChangeState.Updated:
                    {
                        PXR_Plugin.MixedReality.UPxr_AddOrUpdateMesh(meshInfos[i]);
                    }
                        break;
                    case MeshChangeState.Removed:
                    {
                        PXR_Plugin.MixedReality.UPxr_RemoveMesh(meshInfos[i].uuid);
                    }
                        break;
                    case MeshChangeState.Unchanged:
                    {
                        break;
                    }
                }
            }

            if (result == PxrResult.SUCCESS)
            {
                SpatialMeshDataUpdated?.Invoke(meshInfos);
            }
        }
        protected override void OnInstanceDestroy(ulong xrInstance)
        {
            Debug.Log($"[PICOOpenXRExtensions] OnInstanceDestroy: {xrInstance}");
            base.OnInstanceDestroy(xrInstance);
            xrInstance = 0ul;
            PICO_OnInstanceDestroy(xrInstance);
        }


        protected override IntPtr HookGetInstanceProcAddr(IntPtr func)
        {
            Debug.Log($"[PICOOpenXRExtensions] HookGetInstanceProcAddr: {func}");
            return PICO_HookCreateInstance(func);
        }

        protected override void OnAppSpaceChange(ulong xrSpace)
        {
            Debug.Log($"[PICOOpenXRExtensions] OnAppSpaceChange: {xrSpace}");
            PICO_OnAppSpaceChange(xrSpace);
        }

        protected override void OnSystemChange(ulong xrSystem)
        {
            Debug.Log($"[PICOOpenXRExtensions] OnSystemChange: {xrSystem}");
            PICO_OnSystemChange(xrSystem);
        }

        protected override void OnSessionStateChange(int oldState, int newState)
        {
            Debug.Log($"[PICOOpenXRExtensions] OnSessionStateChange: {oldState} -> {newState}");
        }


        protected override void OnSessionBegin(ulong xrSessionId)
        {
            Debug.Log($"[PICOOpenXRExtensions] OnSessionBegin: {xrSessionId}");
        }


        protected override void OnSessionEnd(ulong xrSessionId)
        {
            Debug.Log($"[PICOOpenXRExtensions] OnSessionEnd: {xrSessionId}");
        }


        protected override void OnSessionExiting(ulong xrSessionId)
        {
            Debug.Log($"[PICOOpenXRExtensions] OnSessionExiting: {xrSessionId}");
        }


        protected override void OnSessionDestroy(ulong xrSessionId)
        {
            Debug.Log($"[PICOOpenXRExtensions] OnSessionDestroy: {xrSessionId}");
            xrSession = 0ul;
            PICO_OnSessionDestroy(xrSessionId);
        }
        public static float GetLocationHeight()
        {
            float height = 0;
            PICO_GetLocationHeight( ref height);
            return height;
        }
#if AR_FOUNDATION_5||AR_FOUNDATION_6
        public  bool isSessionSubsystem=false;
        private static List<XRSessionSubsystemDescriptor> sessionSubsystemDescriptors = new List<XRSessionSubsystemDescriptor>();
        protected override void OnSubsystemCreate()
        {
            base.OnSubsystemCreate();
            if (isSessionSubsystem)
            {
                CreateSubsystem<XRSessionSubsystemDescriptor, XRSessionSubsystem>(sessionSubsystemDescriptors, PXR_SessionSubsystem.k_SubsystemId);
            }
        }
        protected override void OnSubsystemStart()
        {
            if (isSessionSubsystem)
            {
                StartSubsystem<XRSessionSubsystem>();
            }
        }
        protected override void OnSubsystemStop()
        {
            if (isSessionSubsystem)
            {
                StopSubsystem<XRSessionSubsystem>();
            }
        }
        protected override void OnSubsystemDestroy()
        {
            if (isSessionSubsystem)
            {
                DestroySubsystem<XRSessionSubsystem>();
            }
        }
#endif
        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PICO_HookCreateInstance(IntPtr func);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PICO_OnInstanceCreate(UInt64 xrInstance);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PICO_OnInstanceDestroy(UInt64 xrInstance);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PICO_OnSessionCreate(UInt64 xrSession);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PICO_OnAppSpaceChange(UInt64 xrSpace);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PICO_OnSessionStateChange(int oldState, int newState);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PICO_OnSessionBegin(UInt64 xrSession);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PICO_OnSessionEnd(UInt64 xrSession);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PICO_OnSessionExiting(UInt64 xrSession);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PICO_OnSessionDestroy(UInt64 xrSession);
        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PICO_OnSystemChange(UInt64 xrSystemId);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL,  CallingConvention = CallingConvention.Cdecl)]
        private static extern int PICO_xrEnumerateReferenceSpaces(ulong xrSession, UInt32 CountInput, ref UInt32 CountOutput,
            XrReferenceSpaceType[] Spaces);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL,  CallingConvention = CallingConvention.Cdecl)]
        private static extern int PICO_xrGetReferenceSpaceBoundsRect(ulong xrSession, XrReferenceSpaceType referenceSpace,
            ref XrExtent2Df extent2D);
        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, EntryPoint = "PICO_SetMarkMode", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetMarkMode();
        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL,  CallingConvention = CallingConvention.Cdecl)]
        private static extern int PICO_GetLocationHeight(ref float delaY);
        
    }
}
#endif