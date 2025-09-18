#if PICO_OPENXR_SDK
using System.Runtime.InteropServices;
using Unity.XR.PXR;
using UnityEngine.XR.OpenXR;


#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;
#endif

namespace Unity.XR.OpenXR.Features.PICOSupport
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "OpenXR Composition Layer Secure Content",
        Hidden = false,
        BuildTargetGroups = new[] { UnityEditor.BuildTargetGroup.Android },
        Company = "PICO",
        OpenxrExtensionStrings = extensionString,
        Version = PXR_Constants.SDKVersion,
        FeatureId = featureId)]
#endif
    public class LayerSecureContentFeature : OpenXRFeatureBase
    {
        public const string featureId = "com.pico.openxr.feature.LayerSecureContent";
        public const string extensionString = "XR_FB_composition_layer_secure_content";
        
        public static bool isExtensionEnable => OpenXRRuntime.IsExtensionEnabled(extensionString);

        public override string GetExtensionString()
        {
            return extensionString;
        }
        public override void SessionCreate(ulong xrSessionId)
        {
            PXR_OpenXRProjectSetting projectConfig = PXR_OpenXRProjectSetting.GetProjectConfig();
            if (projectConfig.useContentProtect)
            {
                SetSecureContentFlag(projectConfig.contentProtectFlags);
            }
        }

        public static void SetSecureContentFlag(SecureContentFlag flag)
        {
            if (!isExtensionEnable)
            {
                return;
            }
            SetSecureContentFlag((int)flag);
        }
        [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetSecureContentFlag(int state);
    }
}
#endif