#if PICO_OPENXR_SDK
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Unity.XR.PXR;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;
#endif


namespace Unity.XR.OpenXR.Features.PICOSupport
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "PICO Scene Capture",
        Hidden = false,
        BuildTargetGroups = new[] { UnityEditor.BuildTargetGroup.Android },
        Company = "PICO",
        OpenxrExtensionStrings = extensionString,
        Version = "1.0.0",
        FeatureId = featureId)]
#endif
    public class PICOSceneCapture: OpenXRFeature
    {
        public const string featureId = "com.pico.openxr.feature.scenecapture";
        public const string extensionString = "XR_PICO_scene_capture XR_PICO_spatial_sensing XR_EXT_future";
        public static bool isEnable => OpenXRRuntime.IsExtensionEnabled("XR_PICO_scene_capture");
        protected override void OnSessionCreate(ulong xrSession)
        {
            base.OnSessionCreate(xrSession);
            PXR_Plugin.MixedReality.UPxr_CreateSceneCaptureSenseDataProvider();
        }

        protected override void OnSessionExiting(ulong xrSession)
        {
            PXR_MixedReality.GetSenseDataProviderState(PxrSenseDataProviderType.SceneCapture, out var providerState);
            if (providerState == PxrSenseDataProviderState.Running)
            {
                PXR_MixedReality.StopSenseDataProvider(PxrSenseDataProviderType.SceneCapture);
            }

            PXR_Plugin.MixedReality.UPxr_DestroySenseDataProvider(
                PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SceneCapture));

            base.OnSessionExiting(xrSession);
        }
    }
}
#endif