#if PICO_OPENXR_SDK
using UnityEditor;
using UnityEngine.XR.OpenXR.Features;
using System.Runtime.InteropServices;
using System;
using Unity.XR.OpenXR.Features.PICOSupport;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR.OpenXR;

#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;

[OpenXRFeature(UiName = "OpenXR Foveation",
    BuildTargetGroups = new[] { BuildTargetGroup.Android },
    OpenxrExtensionStrings = extensionList,
    Company = "PICO",
    Version = PXR_Constants.SDKVersion,
    FeatureId = featureId)]
#endif


public class FoveationFeature : OpenXRFeatureBase
{
    public const string extensionList = "XR_FB_foveation " +
                                        "XR_FB_foveation_configuration " +
                                        "XR_FB_foveation_vulkan " +
                                        "XR_META_foveation_eye_tracked " +
                                        "XR_META_vulkan_swapchain_create_info " +
                                        "XR_FB_swapchain_update_state ";

    public const string featureId = "com.pico.openxr.feature.foveation";
    private static string TAG = "FoveationFeature";
    public enum FoveatedRenderingLevel
    {
        Off = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }
    public enum FoveatedRenderingMode
    {
        FixedFoveatedRendering = 0,
        EyeTrackedFoveatedRendering = 1
    }

    private static UInt32 _foveatedRenderingLevel = 0;
    private static UInt32 _useDynamicFoveation = 0;
    public static bool isExtensionEnable => OpenXRRuntime.IsExtensionEnabled("XR_FB_foveation");
    public override string GetExtensionString()
    {
        return extensionList;
    }

    public override void SessionCreate(ulong xrSessionId)
    {
        if (!isExtensionEnable)
        {
            return ;
        }
        PXR_OpenXRProjectSetting projectConfig = PXR_OpenXRProjectSetting.GetProjectConfig();
        if (projectConfig.foveationEnable)
        {
            PICO_setFoveationEyeTracked(projectConfig.foveatedRenderingMode ==
                                        FoveatedRenderingMode.EyeTrackedFoveatedRendering);
            foveatedRenderingLevel = projectConfig.foveatedRenderingLevel;
        }
    }
    public static FoveatedRenderingLevel foveatedRenderingLevel
    {
        get
        {
            if (!isExtensionEnable)
            {
                return FoveatedRenderingLevel.Off;
            }
            UInt32 level;
            FBGetFoveationLevel(out level);
            PLog.i(TAG,$"  foveatedRenderingLevel get if level= {level}");
            return (FoveatedRenderingLevel)level;
        }
        set
        {
            if (!isExtensionEnable)
            {
                return;
            }
            PLog.e(TAG,$"  foveatedRenderingLevel set if value= {value}");
            _foveatedRenderingLevel = (UInt32)value;
            FBSetFoveationLevel(xrSession, _foveatedRenderingLevel, 0.0f, _useDynamicFoveation);
        }
    }

    public static bool useDynamicFoveatedRendering
    {
        get
        {
            if (!isExtensionEnable)
            {
                return false;
            }
            UInt32 dynamic;
            FBGetFoveationLevel(out dynamic);
            return dynamic != 0;
        }
        set
        {
            if (!isExtensionEnable)
            {
                return ;
            }
            if (value)
                _useDynamicFoveation = 1;
            else
                _useDynamicFoveation = 0;
            FBSetFoveationLevel(xrSession, _foveatedRenderingLevel, 0.0f, _useDynamicFoveation);
        }
    }

    public static bool supportsFoveationEyeTracked
    {
        get
        {
            if (!isExtensionEnable)
            {
                return false;
            }
            bool supported=false;
            Pxr_GetEyeTrackingFoveationRenderingSupported(ref supported);
            return supported;
        }
    }
    


    #region OpenXR Plugin DLL Imports

    [DllImport("UnityOpenXR", EntryPoint = "FBSetFoveationLevel")]
    private static extern void FBSetFoveationLevel(UInt64 session, UInt32 level, float verticalOffset, UInt32 dynamic);

    [DllImport("UnityOpenXR", EntryPoint = "FBGetFoveationLevel")]
    private static extern void FBGetFoveationLevel(out UInt32 level);

    [DllImport("UnityOpenXR", EntryPoint = "FBGetFoveationDynamic")]
    private static extern void FBGetFoveationDynamic(out UInt32 dynamic);

    #endregion


    [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool Pxr_GetEyeTrackingFoveationRenderingSupported(ref bool supported);

    [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern void PICO_setFoveationEyeTracked(bool value);
}
#endif