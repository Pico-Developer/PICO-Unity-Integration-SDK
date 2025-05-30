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
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR;
using AOT;

#if UNITY_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using Unity.XR.PXR.Input;
using System.Linq;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

#if AR_FOUNDATION_5 || AR_FOUNDATION_6
using UnityEngine.XR.ARSubsystems;
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
        private const string TAG = "PXR_Loader";
        private static List<XRDisplaySubsystemDescriptor> displaySubsystemDescriptors = new List<XRDisplaySubsystemDescriptor>();
        private static List<XRInputSubsystemDescriptor> inputSubsystemDescriptors = new List<XRInputSubsystemDescriptor>();
        private static List<XRMeshSubsystemDescriptor> meshSubsystemDescriptors = new List<XRMeshSubsystemDescriptor>();
#if XR_HANDS
        private static List<XRHandSubsystemDescriptor> handSubsystemDescriptors = new List<XRHandSubsystemDescriptor>();
#endif
#if AR_FOUNDATION_5 || AR_FOUNDATION_6
        private static List<XRSessionSubsystemDescriptor> sessionSubsystemDescriptors = new List<XRSessionSubsystemDescriptor>();
        private static List<XRCameraSubsystemDescriptor> cameraSubsystemDescriptors = new List<XRCameraSubsystemDescriptor>();
        private static List<XRFaceSubsystemDescriptor> faceSubsystemDescriptors = new List<XRFaceSubsystemDescriptor>();
        private static List<XRHumanBodySubsystemDescriptor> humanBodySubsystemDescriptors = new List<XRHumanBodySubsystemDescriptor>();
        private static List<XRAnchorSubsystemDescriptor> anchorSubsystemDescriptors = new List<XRAnchorSubsystemDescriptor>();
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

        public XRMeshSubsystem meshSubsystem
        {
            get
            {
                return GetLoadedSubsystem<XRMeshSubsystem>();
            }
        }
        internal enum LoaderState
        {
            Uninitialized,
            InitializeAttempted,
            Initialized,
            StartAttempted,
            Started,
            StopAttempted,
            Stopped,
            DeinitializeAttempted
        }

        internal LoaderState currentLoaderState { get; private set; } = LoaderState.Uninitialized;

        List<LoaderState> validLoaderInitStates = new List<LoaderState> { LoaderState.Uninitialized, LoaderState.InitializeAttempted };
        List<LoaderState> validLoaderStartStates = new List<LoaderState> { LoaderState.Initialized, LoaderState.StartAttempted, LoaderState.Stopped };
        List<LoaderState> validLoaderStopStates = new List<LoaderState> { LoaderState.StartAttempted, LoaderState.Started, LoaderState.StopAttempted };
        List<LoaderState> validLoaderDeinitStates = new List<LoaderState> { LoaderState.InitializeAttempted, LoaderState.Initialized, LoaderState.Stopped, LoaderState.DeinitializeAttempted };

        List<LoaderState> runningStates = new List<LoaderState>()
        {
            LoaderState.Initialized,
            LoaderState.StartAttempted,
            LoaderState.Started
        };
        public override bool Initialize()
        {
            Debug.Log($"{TAG} Initialize() currentLoaderState={currentLoaderState}");
#if UNITY_INPUT_SYSTEM
            InputLayoutLoader.RegisterInputLayouts();
#endif
// #if UNITY_ANDROID 
       
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
                    selfAdaptiveSharpening = Convert.ToUInt16(PXR_ProjectSetting.GetProjectConfig().selfAdaptiveSharpening),
                    enableETFR = Convert.ToUInt16(PXR_ProjectSetting.GetProjectConfig().enableETFR),
                    foveationLevel = Convert.ToUInt16((int)PXR_ProjectSetting.GetProjectConfig().foveationLevel + 1),
                    spatialMeshLod = Convert.ToUInt16(PXR_ProjectSetting.GetProjectConfig().meshLod),
                    enableEyeTracking = Convert.ToUInt16(PXR_ProjectSetting.GetProjectConfig().eyeTracking),
                    dynamicFoveation =1,
                };
                
                PXR_Plugin.System.UPxr_SetUserDefinedSettings(userDefinedSettings);
            }
            PXR_Plugin.System.UPxr_SetXrEventDataBufferCallBack(XrEventDataBufferFunction);
// #endif

            PXR_Plugin.System.ProductName = PXR_Plugin.System.UPxr_GetProductName();
            if (currentLoaderState == LoaderState.Initialized)
                return true;

            if (!validLoaderInitStates.Contains(currentLoaderState))
                return false;

            if (displaySubsystem == null)
            {
                CreateSubsystem<XRDisplaySubsystemDescriptor, XRDisplaySubsystem>(displaySubsystemDescriptors, "PICO Display");
                if (displaySubsystem == null)
                    return false;
            }

            if (inputSubsystem == null)
            {
                CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(inputSubsystemDescriptors, "PICO Input");
                if (inputSubsystem == null)
                    return false;
            }
            if (PXR_ProjectSetting.GetProjectConfig().spatialMesh)
            {
                CreateSubsystem<XRMeshSubsystemDescriptor, XRMeshSubsystem>(meshSubsystemDescriptors, "PICO Mesh");
            }
#if XR_HANDS
            CreateSubsystem<XRHandSubsystemDescriptor, XRHandSubsystem>(handSubsystemDescriptors, "PICO Hands");
#endif

#if AR_FOUNDATION_5 || AR_FOUNDATION_6
            if (PXR_ProjectSetting.GetProjectConfig().arFoundation)
            {
                CreateSubsystem<XRSessionSubsystemDescriptor, XRSessionSubsystem>(sessionSubsystemDescriptors, PXR_SessionSubsystem.k_SubsystemId);
                CreateSubsystem<XRCameraSubsystemDescriptor, XRCameraSubsystem>(cameraSubsystemDescriptors, PXR_CameraSubsystem.k_SubsystemId);
                if (PXR_ProjectSetting.GetProjectConfig().faceTracking)
                {
                    CreateSubsystem<XRFaceSubsystemDescriptor, XRFaceSubsystem>(faceSubsystemDescriptors, PXR_FaceSubsystem.k_SubsystemId);
                }
                if (PXR_ProjectSetting.GetProjectConfig().bodyTracking)
                {
                    CreateSubsystem<XRHumanBodySubsystemDescriptor, XRHumanBodySubsystem>(humanBodySubsystemDescriptors, PXR_HumanBodySubsystem.k_SubsystemId);
                }
                if (PXR_ProjectSetting.GetProjectConfig().spatialAnchor)
                {
                    CreateSubsystem<XRAnchorSubsystemDescriptor, XRAnchorSubsystem>(anchorSubsystemDescriptors, PXR_AnchorSubsystem.k_SubsystemId);
                }
            }
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

            if (PXR_ProjectSetting.GetProjectConfig().spatialAnchor)
            {
                PXR_Plugin.MixedReality.UPxr_CreateSpatialAnchorSenseDataProvider();
            }
            if (PXR_ProjectSetting.GetProjectConfig().sceneCapture)
            {
                PXR_Plugin.MixedReality.UPxr_CreateSceneCaptureSenseDataProvider();
            }

            currentLoaderState = LoaderState.Initialized;
            return displaySubsystem != null;
        }

        public override bool Start()
        {
            Debug.Log($"{TAG} Start() currentLoaderState={currentLoaderState}");
            if (currentLoaderState == LoaderState.Started)
                return true;

            if (!validLoaderStartStates.Contains(currentLoaderState))
                return false;

            currentLoaderState = LoaderState.StartAttempted;

            StartSubsystem<XRDisplaySubsystem>();
            StartSubsystem<XRInputSubsystem>();
#if XR_HANDS
            StartSubsystem<XRHandSubsystem>();
#endif
#if AR_FOUNDATION_5 || AR_FOUNDATION_6
            if (PXR_ProjectSetting.GetProjectConfig().arFoundation)
            {
                StartSubsystem<XRCameraSubsystem>();
                if (PXR_ProjectSetting.GetProjectConfig().bodyTracking)
                {
                    StartSubsystem<XRHumanBodySubsystem>();
                }

                if (PXR_ProjectSetting.GetProjectConfig().faceTracking)
                {
                    StartSubsystem<XRFaceSubsystem>();
                }
            }

            if (PXR_ProjectSetting.GetProjectConfig().spatialAnchor)
            {
                StartSubsystem<XRAnchorSubsystem>();
            }
#endif

            if (!displaySubsystem?.running ?? false)
            {
                StartSubsystem<XRDisplaySubsystem>();
            }

            if (!inputSubsystem?.running ?? false)
            {
                StartSubsystem<XRInputSubsystem>();
            }
            currentLoaderState = LoaderState.Started;

            return true;
        }

        public override bool Stop()
        {
            Debug.Log($"{TAG} Stop() currentLoaderState={currentLoaderState}");
            if (currentLoaderState == LoaderState.Stopped)
                return true;

            if (!validLoaderStopStates.Contains(currentLoaderState))
                return false;

            currentLoaderState = LoaderState.StopAttempted;

            var inputRunning = inputSubsystem?.running ?? false;
            var displayRunning = displaySubsystem?.running ?? false;
            if (inputRunning)
            {
                StopSubsystem<XRInputSubsystem>();
            }

            if (displayRunning)
            {
                StopSubsystem<XRDisplaySubsystem>();
            }

#if XR_HANDS
            StopSubsystem<XRHandSubsystem>();
#endif
#if AR_FOUNDATION_5 || AR_FOUNDATION_6
            if (PXR_ProjectSetting.GetProjectConfig().arFoundation)
            {
                StopSubsystem<XRCameraSubsystem>();
                if (PXR_ProjectSetting.GetProjectConfig().bodyTracking)
                {
                    StopSubsystem<XRHumanBodySubsystem>();
                }

                if (PXR_ProjectSetting.GetProjectConfig().faceTracking)
                {
                    StopSubsystem<XRFaceSubsystem>();
                }
            }

            if (PXR_ProjectSetting.GetProjectConfig().spatialAnchor)
            {
                StopSubsystem<XRAnchorSubsystem>();
            }
#endif
            currentLoaderState = LoaderState.Stopped;
            return true;
        }

        public override bool Deinitialize()
        {
            Debug.Log($"{TAG} Deinitialize() currentLoaderState={currentLoaderState}");

            if (currentLoaderState == LoaderState.Uninitialized)
                return true;

            if (!validLoaderDeinitStates.Contains(currentLoaderState))
            {
                return false;
            }

            currentLoaderState = LoaderState.DeinitializeAttempted;

            DestroySubsystem<XRDisplaySubsystem>();
            DestroySubsystem<XRInputSubsystem>();
            if (PXR_ProjectSetting.GetProjectConfig().spatialMesh)
            {
                if (meshSubsystem.running)
                {
                    StopSubsystem<XRMeshSubsystem>();
                }
                PXR_Plugin.MixedReality.UPxr_DisposeMesh();
                DestroySubsystem<XRMeshSubsystem>();
            }
#if XR_HANDS
            DestroySubsystem<XRHandSubsystem>();
#endif
#if AR_FOUNDATION_5 || AR_FOUNDATION_6
            if (PXR_ProjectSetting.GetProjectConfig().arFoundation)
            {
                DestroySubsystem<XRCameraSubsystem>();
                if (PXR_ProjectSetting.GetProjectConfig().bodyTracking)
                {
                    DestroySubsystem<XRHumanBodySubsystem>();
                }

                if (PXR_ProjectSetting.GetProjectConfig().faceTracking)
                {
                    DestroySubsystem<XRFaceSubsystem>();
                }
            }
#endif

            PXR_Plugin.System.UPxr_DeinitializeFocusCallback();

            if (PXR_ProjectSetting.GetProjectConfig().spatialAnchor)
            {
                PXR_MixedReality.GetSenseDataProviderState(PxrSenseDataProviderType.SpatialAnchor, out var providerState);
                if (providerState == PxrSenseDataProviderState.Running)
                {
                    PXR_MixedReality.StopSenseDataProvider(PxrSenseDataProviderType.SpatialAnchor);
                }
                PXR_Plugin.MixedReality.UPxr_DestroySenseDataProvider(PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SpatialAnchor));
            }
            if (PXR_ProjectSetting.GetProjectConfig().sceneCapture)
            {
                PXR_MixedReality.GetSenseDataProviderState(PxrSenseDataProviderType.SceneCapture, out var providerState);
                if (providerState == PxrSenseDataProviderState.Running)
                {
                    PXR_MixedReality.StopSenseDataProvider(PxrSenseDataProviderType.SceneCapture);
                }
                PXR_Plugin.MixedReality.UPxr_DestroySenseDataProvider(PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SceneCapture));
            }
            currentLoaderState = LoaderState.Uninitialized;

            return true;
        }

        [MonoPInvokeCallback(typeof(ConvertRotationWith2VectorDelegate))]
        static Quaternion ConvertRotationWith2Vector(Vector3 from, Vector3 to)
        {
            return Quaternion.FromToRotation(from, to);
        }

        [MonoPInvokeCallback(typeof(XrEventDataBufferCallBack))]
        static void XrEventDataBufferFunction(ref XrEventDataBuffer eventDB)
        {
            int status, action;
            PLog.d("PxrUnity",$"XrEventDataBufferFunction eventType={eventDB.type}",false);
            switch (eventDB.type)
            {
                case XrStructureType.XR_TYPE_EVENT_DATA_SESSION_STATE_CHANGED:
                    
                    int sessionstate = BitConverter.ToInt32(eventDB.data, 8);
                    if (PXR_Plugin.System.SessionStateChanged != null)
                    {
                        PXR_Plugin.System.SessionStateChanged((XrSessionState)sessionstate);
                    }
#if AR_FOUNDATION_5 || AR_FOUNDATION_6
                    PXR_SessionSubsystem.instance?.OnSessionStateChange((XrSessionState)sessionstate);
#endif
                    break;
                case XrStructureType.XR_TYPE_EVENT_CONTROLLER_STATE_CHANGED_PICO:

                    XrDeviceEventType eventType = (XrDeviceEventType)eventDB.data[0];
                    status = eventDB.data[5];
                    action = eventDB.data[6];
                    PLog.i(TAG, $"Controller eventType={eventType}, status={status}, action={action}", false);
                    switch (eventType)
                    {
                        case XrDeviceEventType.XR_DEVICE_INPUTDEVICE_CHANGED:
                            if (PXR_Plugin.System.InputDeviceChanged != null)
                            {
                                PXR_Plugin.System.InputDeviceChanged(status);
                            }
                            break;
                    }

                    break;
                case XrStructureType.XR_TYPE_EVENT_DATA_DISPLAY_REFRESH_RATE_CHANGED_FB:
                    float drRate = BitConverter.ToSingle(eventDB.data, 4);
                    if (PXR_Plugin.System.DisplayRefreshRateChangedAction != null)
                    {
                        PXR_Plugin.System.DisplayRefreshRateChangedAction(drRate);
                    }

                    PLog.i(TAG, $"RefreshRateChanged value ={drRate}", false);
                    break;
                case XrStructureType.XR_TYPE_EVENT_SEETHROUGH_STATE_CHANGED:
                    status = BitConverter.ToInt32(eventDB.data, 0);
                    PXR_Plugin.Boundary.seeThroughState = status;
                    if (PXR_Plugin.Boundary.SeethroughStateChangedAction != null)
                    {
                        PXR_Plugin.Boundary.SeethroughStateChangedAction(status);
                    }

                    PLog.i(TAG, $"SeethroughStateChanged status ={status}", false);
                    break;
                case XrStructureType.XR_TYPE_EVENT_DATA_MRC_STATUS_CHANGED_PICO:
                    status = BitConverter.ToInt32(eventDB.data, 0);
                    PLog.i(TAG, $"XR_TYPE_EVENT_DATA_MRC_STATUS_CHANGED_PICO status ={status}", false);
                    PXR_Plugin.System.enableMRC = status == 1;
                    if (PXR_Plugin.System.MRCStateChangedAction != null)
                    {
                        PXR_Plugin.System.MRCStateChangedAction(status == 1);
                    }

                    break;
                case XrStructureType.XR_TYPE_EVENT_LOG_LEVEL_CHANGE:
                    status = BitConverter.ToInt32(eventDB.data, 4);
                    PLog.logLevel = (PLog.LogLevel)status;
                    PLog.i(TAG, $"SDKLoglevelChanged logLevel ={status}", false);

                    break;
                case XrStructureType.XR_TYPE_EVENT_DATA_USER_PRESENCE_CHANGED_EXT:
                    bool isUserPresent = BitConverter.ToBoolean(eventDB.data, 8);
                    if (PXR_Plugin.System.UserPresenceChangedAction != null)
                    {
                        PXR_Plugin.System.UserPresenceChangedAction(isUserPresent);
                    }

                    break;
                case XrStructureType.XR_TYPE_EVENT_KEY_EVENT:
                    if (PXR_Plugin.System.RecenterSuccess != null)
                    {
                        PXR_Plugin.System.RecenterSuccess();
                    }

                    break;
                
                case XrStructureType.XR_TYPE_EVENT_DATA_ENVIRONMENT_BLEND_MODE_CHANGED_EXT:
                {
                    if (PXR_Manager.VstDisplayStatusChanged != null)
                    {
                        int status_ = BitConverter.ToInt32(eventDB.data, 8);
                        PXR_Manager.VstDisplayStatusChanged(status_==1?PxrVstStatus.Disabled:PxrVstStatus.Enabled);
                    }
                }
                    break;
                case XrStructureType.XR_TYPE_EVENT_DATA_SENSE_DATA_PROVIDER_STATE_CHANGED:
                case XrStructureType.XR_TYPE_EVENT_DATA_SENSE_DATA_UPDATED:
                case XrStructureType.XR_TYPE_EVENT_DATA_AUTO_SCENE_CAPTURE_UPDATE_PICO:
                {
                    PXR_Manager.Instance.PollEvent(eventDB);
                    break;
                }
                case XrStructureType.XR_TYPE_EVENT_DATA_REQUEST_MOTION_TRACKER_COMPLETE:
                    if (PXR_MotionTracking.RequestMotionTrackerCompleteAction != null)
                    {
                        RequestMotionTrackerCompleteEventData requestMotionTrackerCompleteEventData = new RequestMotionTrackerCompleteEventData();
                        requestMotionTrackerCompleteEventData.trackerCount = BitConverter.ToUInt32(eventDB.data, 0);
                        requestMotionTrackerCompleteEventData.trackerIds = new long[requestMotionTrackerCompleteEventData.trackerCount];
                        for (int i = 0; i < requestMotionTrackerCompleteEventData.trackerCount; i++)
                        {
                            requestMotionTrackerCompleteEventData.trackerIds[i] = BitConverter.ToInt16(eventDB.data, 8+ 8 * i);
                        }

                        requestMotionTrackerCompleteEventData.result =
                            (PxrResult)BitConverter.ToInt32(eventDB.data, 4 + 8 * (int)requestMotionTrackerCompleteEventData.trackerCount);
                        PXR_MotionTracking.RequestMotionTrackerCompleteAction(requestMotionTrackerCompleteEventData);
                    }
                    break;
                case XrStructureType.XR_TYPE_EVENT_DATA_MOTION_TRACKER_CONNECTION_STATE_CHANGED:
                    if (PXR_MotionTracking.MotionTrackerConnectionAction != null)
                    {
                        Int64 trackerId = BitConverter.ToInt64(eventDB.data, 0);
                        int state =  BitConverter.ToInt32(eventDB.data, 8);
                        PXR_MotionTracking.MotionTrackerConnectionAction(trackerId, state);
                    }
                    break;
                case XrStructureType.XR_TYPE_EVENT_DATA_MOTION_TRACKER_POWER_KEY_EVENT:
                    if (PXR_MotionTracking.MotionTrackerPowerKeyAction != null)
                    {
                        Int64 trackerId = BitConverter.ToInt64(eventDB.data, 0);
                        bool state =  BitConverter.ToBoolean(eventDB.data, 8);
                        PXR_MotionTracking.MotionTrackerPowerKeyAction(trackerId, state);
                    }
                    break;
                case XrStructureType.XR_TYPE_EVENT_DATA_EXPAND_DEVICE_CONNECTION_STATE_CHANGED:
                    if (PXR_MotionTracking.ExpandDeviceConnectionAction != null)
                    {
                        UInt64 trackerId = BitConverter.ToUInt64(eventDB.data, 0);
                        int state =  BitConverter.ToInt32(eventDB.data, 8);
                        PXR_MotionTracking.ExpandDeviceConnectionAction((long)trackerId, state);
                    }
                    break;
                case XrStructureType.XR_TYPE_EVENT_DATA_EXPAND_DEVICE_BATTERY_STATE_CHANGED:
                    if (PXR_MotionTracking.ExpandDeviceBatteryAction != null)
                    {
                        
                        ExpandDeviceBatteryEventData expandDevice = new ExpandDeviceBatteryEventData();
                        expandDevice.deviceId = BitConverter.ToUInt64(eventDB.data, 0);
                        expandDevice.batteryLevel = BitConverter.ToSingle(eventDB.data, 8);
                        expandDevice.chargingState = (XrBatteryChargingState)BitConverter.ToInt32(eventDB.data, 12);
                      
                        PXR_MotionTracking.ExpandDeviceBatteryAction(expandDevice);
                    }
                    break;
                case XrStructureType.XR_TYPE_EVENT_DATA_EXPAND_DEVICE_CUSTOM_DATA_STATE_CHANGED:
                    if (PXR_MotionTracking.ExtDevPassDataAction != null)
                    {
                        status = BitConverter.ToInt32(eventDB.data, 0);
                        PXR_MotionTracking.ExtDevPassDataAction(status);
                    }

                    break;
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
            string version = "UnityXR_" + PXR_Plugin.System.UPxr_GetSDKVersion() + "_" + Application.unityVersion;
            // PXR_Plugin.System.UPxr_SetConfigString( ConfigType.EngineVersion, version );
        }
#endif
    }
}