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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Management;
using UnityEngine.XR;
using AOT;

#if UNITY_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using Unity.XR.PXR.Input;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

#if XR_HANDS
using UnityEngine.XR.Hands;
#endif


namespace Unity.XR.PXR
{
#if UNITY_INPUT_SYSTEM
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    static class InputLayoutLoader
    {
        static InputLayoutLoader()
        {
            RegisterInputLayouts();
        }

        public static void RegisterInputLayouts()
        {
            InputSystem.RegisterLayout<PXR_HMD>(matches: new InputDeviceMatcher().WithInterface(XRUtilities.InterfaceMatchAnyVersion).WithProduct(@"^(PICO HMD)|^(PICO Neo)|^(PICO G)"));
            InputSystem.RegisterLayout<PXR_Controller>(matches: new InputDeviceMatcher().WithInterface(XRUtilities.InterfaceMatchAnyVersion).WithProduct(@"^(PICO Controller)"));
        }
    }
#endif

    public class PXR_Loader : XRLoaderHelper
#if UNITY_EDITOR
    , IXRLoaderPreInit
#endif
    {
        private static List<XRDisplaySubsystemDescriptor> displaySubsystemDescriptors = new List<XRDisplaySubsystemDescriptor>();
        private static List<XRInputSubsystemDescriptor> inputSubsystemDescriptors = new List<XRInputSubsystemDescriptor>();
#if XR_HANDS
        private static List<XRHandSubsystemDescriptor> handSubsystemDescriptors = new List<XRHandSubsystemDescriptor>();
#endif
        public delegate Quaternion ConvertRotationWith2VectorDelegate(Vector3 from, Vector3 to);

        public XRDisplaySubsystem displaySubsystem
        {
            get
            {
                return GetLoadedSubsystem<XRDisplaySubsystem>();
            }
        }

        public XRInputSubsystem inputSubsystem
        {
            get
            {
                return GetLoadedSubsystem<XRInputSubsystem>();
            }
        }

        public override bool Initialize()
        {
#if UNITY_INPUT_SYSTEM
            InputLayoutLoader.RegisterInputLayouts();
#endif
#if UNITY_ANDROID
            PXR_Settings settings = GetSettings();
            if (settings != null)
            {
                UserDefinedSettings userDefinedSettings = new UserDefinedSettings
                {
                    stereoRenderingMode = settings.GetStereoRenderingMode(),
                    colorSpace = (ushort)((QualitySettings.activeColorSpace == ColorSpace.Linear) ? 1 : 0),
                    useContentProtect = Convert.ToUInt16(PXR_ProjectSetting.GetProjectConfig().useContentProtect),
                    systemDisplayFrequency = settings.GetSystemDisplayFrequency(),
                    optimizeBufferDiscards = settings.GetOptimizeBufferDiscards(),
                    enableAppSpaceWarp = Convert.ToUInt16(settings.enableAppSpaceWarp),                    
                    enableSubsampled = Convert.ToUInt16(PXR_ProjectSetting.GetProjectConfig().enableSubsampled),
                    lateLatchingDebug = Convert.ToUInt16(PXR_ProjectSetting.GetProjectConfig().latelatchingDebug),
                    enableStageMode = Convert.ToUInt16(PXR_ProjectSetting.GetProjectConfig().stageMode),
                    enableSuperResolution = Convert.ToUInt16(PXR_ProjectSetting.GetProjectConfig().superResolution),
                    normalSharpening = Convert.ToUInt16(PXR_ProjectSetting.GetProjectConfig().normalSharpening),
                    qualitySharpening = Convert.ToUInt16(PXR_ProjectSetting.GetProjectConfig().qualitySharpening),
                    fixedFoveatedSharpening = Convert.ToUInt16(PXR_ProjectSetting.GetProjectConfig().fixedFoveatedSharpening),
                    selfAdaptiveSharpening = Convert.ToUInt16(PXR_ProjectSetting.GetProjectConfig().selfAdaptiveSharpening)
                };

                PXR_Plugin.System.UPxr_Construct(ConvertRotationWith2Vector);
                PXR_Plugin.System.UPxr_SetInputDeviceChangedCallBack(InputDeviceChangedFunction);
                PXR_Plugin.System.UPxr_SetSeethroughStateChangedCallBack(SeethroughStateChangedFunction);
                PXR_Plugin.System.UPxr_SetFitnessBandNumberOfConnectionsCallBack(FitnessBandNumberOfConnectionsFunction);
                PXR_Plugin.System.UPxr_SetFitnessBandElectricQuantityCallBack(FitnessBandElectricQuantityFunction);
                PXR_Plugin.System.UPxr_SetFitnessBandAbnormalCalibrationDataCallBack(FitnessBandAbnormalCalibrationDataFunction);
                PXR_Plugin.System.UPxr_SetLoglevelChangedCallBack(LoglevelChangedFunction);
                PXR_Plugin.System.UPxr_SetUserDefinedSettings(userDefinedSettings);

            }
#endif

            CreateSubsystem<XRDisplaySubsystemDescriptor, XRDisplaySubsystem>(displaySubsystemDescriptors, "PICO Display");
            CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(inputSubsystemDescriptors, "PICO Input");

#if XR_HANDS
            CreateSubsystem<XRHandSubsystemDescriptor, XRHandSubsystem>(handSubsystemDescriptors, "PICO Hands");
#endif

            if (displaySubsystem == null && inputSubsystem == null)
            {
                Debug.LogError("PXRLog Unable to start PICO Plugin.");
            }
            else if (displaySubsystem == null)
            {
                Debug.LogError("PXRLog Failed to load display subsystem.");
            }
            else if (inputSubsystem == null)
            {
                Debug.LogError("PXRLog Failed to load input subsystem.");
            }
            else
            {
                PXR_Plugin.System.UPxr_InitializeFocusCallback();
            }

#if XR_HANDS
            var handSubSystem = GetLoadedSubsystem<XRHandSubsystem>();
            if (handSubSystem == null)
            {
                Debug.LogError("PXRLog Failed to load XRHandSubsystem.");
            }
#endif

            return displaySubsystem != null;
        }

        public override bool Start()
        {
            StartSubsystem<XRDisplaySubsystem>();
            StartSubsystem<XRInputSubsystem>();

#if XR_HANDS
            StartSubsystem<XRHandSubsystem>();
#endif

            return true;
        }

        public override bool Stop()
        {
            StopSubsystem<XRDisplaySubsystem>();
            StopSubsystem<XRInputSubsystem>();

#if XR_HANDS
            StopSubsystem<XRHandSubsystem>();
#endif

            return true;
        }

        public override bool Deinitialize()
        {
            DestroySubsystem<XRDisplaySubsystem>();
            DestroySubsystem<XRInputSubsystem>();

#if XR_HANDS
            DestroySubsystem<XRHandSubsystem>();
#endif

            PXR_Plugin.System.UPxr_DeinitializeFocusCallback();
            return true;
        }

        [MonoPInvokeCallback(typeof(ConvertRotationWith2VectorDelegate))]
        static Quaternion ConvertRotationWith2Vector(Vector3 from, Vector3 to)
        {
            return Quaternion.FromToRotation(from, to);
        }

        [MonoPInvokeCallback(typeof(InputDeviceChangedCallBack))]
        static void InputDeviceChangedFunction(int value)
        {
            if (PXR_Plugin.System.InputDeviceChanged != null)
            {
                PXR_Plugin.System.InputDeviceChanged(value);
            }
        }

        [MonoPInvokeCallback(typeof(SeethroughStateChangedCallBack))]
        static void SeethroughStateChangedFunction(int value)
        {
            if (PXR_Plugin.System.SeethroughStateChangedChanged != null)
            {
                PXR_Plugin.System.SeethroughStateChangedChanged(value);
            }
        }

        [MonoPInvokeCallback(typeof(FitnessBandNumberOfConnectionsCallBack))]
        static void FitnessBandNumberOfConnectionsFunction(int state, int value)
        {
            if (PXR_Plugin.System.FitnessBandNumberOfConnections != null)
            {
                PXR_Plugin.System.FitnessBandNumberOfConnections(state, value);
            }
        }

        [MonoPInvokeCallback(typeof(FitnessBandElectricQuantityCallBack))]
        static void FitnessBandElectricQuantityFunction(int trackerID, int battery)
        {
            if (PXR_Plugin.System.FitnessBandElectricQuantity != null)
            {
                PXR_Plugin.System.FitnessBandElectricQuantity(trackerID, battery);
            }
        }

        [MonoPInvokeCallback(typeof(FitnessBandAbnormalCalibrationDataCallBack))]
        static void FitnessBandAbnormalCalibrationDataFunction(int state, int value)
        {
            if (PXR_Plugin.System.FitnessBandAbnormalCalibrationData != null)
            {
                PXR_Plugin.System.FitnessBandAbnormalCalibrationData(state, value);
            }
        }

        [MonoPInvokeCallback(typeof(LoglevelChangedCallBack))]
        static void LoglevelChangedFunction(int value)
        {
            if (PXR_Plugin.System.LoglevelChangedChanged != null)
            {
                PXR_Plugin.System.LoglevelChangedChanged(value);
            }
        }

        public PXR_Settings GetSettings()
        {
            PXR_Settings settings = null;
#if UNITY_EDITOR
            UnityEditor.EditorBuildSettings.TryGetConfigObject<PXR_Settings>("Unity.XR.PXR.Settings", out settings);
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
            settings = PXR_Settings.settings;
#endif
            return settings;
        }

#if UNITY_EDITOR
        public string GetPreInitLibraryName(BuildTarget buildTarget, BuildTargetGroup buildTargetGroup)
        {
            return "PxrPlatform";
        }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void RuntimeLoadPicoPlugin()
        {
            PXR_Plugin.System.UPxr_LoadPICOPlugin();
            string version = "UnityXR_" + PXR_Plugin.System.UPxr_GetSDKVersion() + "_" + Application.unityVersion;
            PXR_Plugin.System.UPxr_SetConfigString( ConfigType.EngineVersion, version );
        }
#endif
    }
}
