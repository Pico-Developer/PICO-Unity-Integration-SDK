#if PICO_OPENXR_SDK
using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.NativeTypes;

#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;
#endif

namespace Unity.XR.OpenXR.Features.PICOSupport
{
    public enum SystemDisplayFrequency
    {
        Default,
        RefreshRate72 = 72,
        RefreshRate90 = 90,
        RefreshRate120 = 120,
    }
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "OpenXR Display Refresh Rate",
        Hidden = false,
        BuildTargetGroups = new[] { UnityEditor.BuildTargetGroup.Android },
        Company = "PICO",
        OpenxrExtensionStrings = extensionString,
        Version = PXR_Constants.SDKVersion,
        FeatureId = featureId)]
#endif
    public class DisplayRefreshRateFeature : OpenXRFeatureBase
    {
        public const string featureId = "com.pico.openxr.feature.refreshrate";
        public const string extensionString = "XR_FB_display_refresh_rate";
        public static bool isExtensionEnable => OpenXRRuntime.IsExtensionEnabled(extensionString);

        public override string GetExtensionString()
        {
            return extensionString;
        }
        
        public override void SessionCreate(ulong xrSessionId)
        {
            PXR_OpenXRProjectSetting projectConfig = PXR_OpenXRProjectSetting.GetProjectConfig();
            if (projectConfig.displayFrequency != SystemDisplayFrequency.Default)
            {
                SetDisplayRefreshRate(projectConfig.displayFrequency);
            }
        }
        public static bool SetDisplayRefreshRate(SystemDisplayFrequency DisplayFrequency)
        {
            PLog.e(extensionString,$"SetDisplayRefreshRate:{DisplayFrequency}");
            float rate = 0;
            switch (DisplayFrequency)
            {
                case SystemDisplayFrequency.Default:
                    return true;
                case SystemDisplayFrequency.RefreshRate72:
                    rate = 72;
                    break;
                case SystemDisplayFrequency.RefreshRate90:
                    rate = 90;
                    break;
                case SystemDisplayFrequency.RefreshRate120:
                    rate = 120;
                    break;
            }

            return SetDisplayRefreshRate(rate);
        }

        public static bool GetDisplayRefreshRate(ref float displayRefreshRate)
        {
            if (!isExtensionEnable)
            {
                return false;
            }
            return Pxr_GetDisplayRefreshRate(ref displayRefreshRate) == (int)XrResult.Success;
        }

        public static bool SetDisplayRefreshRate(float displayRefreshRate)
        {
            if (!isExtensionEnable)
            {
                return false;
            }
            
            return Pxr_SetDisplayRefreshRate(displayRefreshRate) == (int)XrResult.Success;
        }
        [Obsolete("Please use GetDisplayFrequenciesAvailable")]
        public static int GetDisplayRefreshRateCount()
        {
            return 0;
        }
        [Obsolete("Please use GetDisplayFrequenciesAvailable")]
        public static bool TryGetSupportedDisplayRefreshRates(
            Allocator allocator, out NativeArray<float> refreshRates)
        {
            refreshRates = default;
            return false;
        }

        public static float[] GetDisplayFrequenciesAvailable()
        {
            if (!isExtensionEnable)
            {
                return null;
            }

            float[] configArray = { 0 };
            int configCount = 0;
            IntPtr configHandle = IntPtr.Zero;
            bool ret = false;
            ret = Pxr_GetDisplayRefreshRatesAvailable(ref configCount, ref configHandle);
            if (ret)
            {
                configArray = new float[configCount];
                Marshal.Copy(configHandle, configArray, 0, configCount);
            }

            return configArray;
        }

        [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetDisplayRefreshRate(ref float displayRefreshRate);
        [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetDisplayRefreshRate(float refreshRate);
        [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Pxr_GetDisplayRefreshRatesAvailable(ref int configCount, ref IntPtr configArray);
    }
}
#endif