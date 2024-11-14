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
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using System.Linq;
using System.Runtime.InteropServices;

namespace Unity.XR.PXR
{
    public class PXR_Manager : MonoBehaviour
    {
        private const string TAG = "[PXR_Manager]";
        private static PXR_Manager instance = null;
        public static PXR_Manager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PXR_Manager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("[PXR_Manager]");
                        DontDestroyOnLoad(go);
                        instance = go.AddComponent<PXR_Manager>();
                        Debug.LogError("PXRLog instance is not initialized!");
                    }
                }
                return instance;
            }
        }

        private Camera[] eyeCamera;
        private bool appSpaceWarp;
        private Transform m_AppSpaceTransform;
        private DepthTextureMode m_CachedDepthTextureMode;

        [HideInInspector]
        public bool screenFade;
        [HideInInspector]
        public bool eyeTracking;
        [HideInInspector]
        public FaceTrackingMode trackingMode = FaceTrackingMode.PXR_FTM_NONE;
        [HideInInspector]
        public SharpeningMode sharpeningMode = SharpeningMode.None;
        [HideInInspector]
        public SharpeningEnhance sharpeningEnhance = SharpeningEnhance.None;
        [HideInInspector]
        public bool faceTracking;
        [HideInInspector]
        public bool lipsyncTracking;
        [HideInInspector]
        public bool lateLatching;
        [HideInInspector]
        public bool latelatchingDebug;
        [HideInInspector]
        public bool bodyTracking;
        [HideInInspector]
        public FoveationLevel foveationLevel = FoveationLevel.None;
        [HideInInspector]
        public bool adaptiveResolution;
        [HideInInspector]
        public FoveationLevel eyeFoveationLevel = FoveationLevel.None;
        [HideInInspector]
        public FoveatedRenderingMode foveatedRenderingMode = FoveatedRenderingMode.FixedFoveatedRendering;

        //MRC
        #region MRCData
        [HideInInspector]
        public bool openMRC = true;
        [HideInInspector]
        public LayerMask foregroundLayerMask = -1;
        [HideInInspector]
        public LayerMask backgroundLayerMask = -1;
        private static bool mrcXmlCamData = false;
        private static bool initMRCSucceed = false;

        private Texture[] swapChain = new Texture[2];
        private struct LayerTexture
        {
            public Texture[] swapChain;
        };
        private LayerTexture[] layerTexturesInfo;
        private bool createMRCOverlaySucceed = false;
        private int imageIndex;
        private UInt32 imageCounts = 0;

        private static ExternalCameraInfo cameraInfo;
        private bool mrcCamObjActived = false;
        private float[] cameraAttribute;
        private PxrLayerParam layerParam = new PxrLayerParam();
        [HideInInspector]
        public GameObject backgroundCamObj = null;
        [HideInInspector]
        public GameObject foregroundCamObj = null;
        [HideInInspector]
        public RenderTexture mrcBackgroundRT = null;
        [HideInInspector]
        public RenderTexture mrcForegroundRT = null;
        private Color foregroundColor = new Color(0, 1, 0, 1);
        private static float height;
        [SerializeField]
        [HideInInspector]
        public AdaptiveResolutionPowerSetting adaptiveResolutionPowerSetting = AdaptiveResolutionPowerSetting.BALANCED;

        [SerializeField]
        [HideInInspector]
        public float minEyeTextureScale = 0.7f;

        [SerializeField]
        [HideInInspector]
        public float maxEyeTextureScale = 1.26f;

        private IntPtr layerSubmitPtr = IntPtr.Zero;

        #endregion

        private bool isNeedResume = false;

        //Super Resolution
        [HideInInspector]
        public bool enableSuperResolution;

        [HideInInspector]
        public bool useRecommendedAntiAliasingLevel = true;

        private List<PxrEventDataBuffer> eventList = new List<PxrEventDataBuffer>();

        #region 2.0 API Deprecate
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static event Action<PxrEventAnchorEntityCreated> AnchorEntityCreated;
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static event Action<PxrEventSpatialTrackingStateUpdate> SpatialTrackingStateUpdate;
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static event Action<PxrEventAnchorEntityPersisted> AnchorEntityPersisted;
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static event Action<PxrEventAnchorEntityUnPersisted> AnchorEntityUnPersisted;
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static event Action<PxrEventAnchorEntityCleared> AnchorEntityCleared;
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static event Action<PxrEventAnchorEntityLoaded> AnchorEntityLoaded;
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static event Action<PxrEventSpatialSceneCaptured> SpatialSceneCaptured;
        #endregion

        public static event Action<PxrSpatialMapSizeLimitedReason> SpatialMapSizeLimited;
        public static event Action<PxrEventAutoRoomCaptureUpdated> AutoRoomCaptureUpdated;
        public static event Action<PxrEventSenseDataProviderStateChanged> SenseDataProviderStateChanged;
        public static event Action<PxrVstStatus> VstDisplayStatusChanged;
        public static event Action<ulong> SenseDataUpdated;
        public static event Action SpatialAnchorDataUpdated;
        public static event Action<List<PxrSpatialMeshInfo>> SpatialMeshDataUpdated;
        public static event Action SceneAnchorDataUpdated;
        public static event Action SemiAutoCaptureDataUpdated;
        public static event Action<bool> EnableVideoSeeThroughAction;

        private static bool _enableVideoSeeThrough;
        [HideInInspector]
        public static bool EnableVideoSeeThrough
        {
            get => _enableVideoSeeThrough;
            set
            {
                if (_enableVideoSeeThrough != value)
                {
                    _enableVideoSeeThrough = value;
                    PXR_Plugin.Boundary.UPxr_SetSeeThroughBackground(value);

                    if (EnableVideoSeeThroughAction != null)
                    {
                        EnableVideoSeeThroughAction(value);
                    }
                }
            }
        }



        void Awake()
        {
            //version log
            Debug.Log("PXRLog XR Platform----SDK Version:" + PXR_Plugin.System.UPxr_GetSDKVersion());

            //log level
            int logLevel = PXR_Plugin.System.UPxr_GetConfigInt(ConfigType.UnityLogLevel);
            Debug.Log("PXRLog XR Platform----SDK logLevel:" + logLevel);
            PLog.logLevel = (PLog.LogLevel)logLevel;
            eyeCamera = new Camera[3];
            Camera[] cam = gameObject.GetComponentsInChildren<Camera>();
            for (int i = 0; i < cam.Length; i++)
            {
                if (cam[i].stereoTargetEye == StereoTargetEyeMask.Both && cam[i] == Camera.main)
                {
                    eyeCamera[0] = cam[i];
                }
                else if (cam[i].stereoTargetEye == StereoTargetEyeMask.Left)
                {
                    eyeCamera[1] = cam[i];
                }
                else if (cam[i].stereoTargetEye == StereoTargetEyeMask.Right)
                {
                    eyeCamera[2] = cam[i];
                }
            }

            PXR_Plugin.System.UPxr_EnableEyeTracking(eyeTracking);
            PXR_Plugin.System.UPxr_EnableFaceTracking(faceTracking);
            PXR_Plugin.System.UPxr_EnableLipSync(lipsyncTracking);

            StartCoroutine("SetFoveationLevel");

            if (GraphicsSettings.renderPipelineAsset == null || QualitySettings.renderPipeline == null)
            {
                int recommendedAntiAliasingLevel = PXR_Plugin.System.UPxr_GetConfigInt(ConfigType.AntiAliasingLevelRecommended);
                if (useRecommendedAntiAliasingLevel && QualitySettings.antiAliasing != recommendedAntiAliasingLevel)
                {
                    QualitySettings.antiAliasing = recommendedAntiAliasingLevel;
                    List<XRDisplaySubsystem> displaySubsystems = new List<XRDisplaySubsystem>();
                    SubsystemManager.GetInstances(displaySubsystems);

                    if (displaySubsystems.Count > 0)
                    {
                        displaySubsystems[0].SetMSAALevel(recommendedAntiAliasingLevel);
                    }
                }
                if (eyeTracking)
                {
                    PXR_Plugin.MotionTracking.UPxr_WantEyeTrackingService();
                }
                if (faceTracking || lipsyncTracking)
                {
                    PXR_Plugin.MotionTracking.UPxr_WantFaceTrackingService();
                }
                if (bodyTracking)
                {
                    PXR_Plugin.MotionTracking.UPxr_WantBodyTrackingService();
                }

                Debug.LogFormat(TAG_MRC + "Awake openMRC = {0} ,MRCInitSucceed = {1}.", openMRC, initMRCSucceed);
                PXR_Plugin.System.UPxr_LogSdkApi("pico_msaa|" + QualitySettings.antiAliasing.ToString());
            }
        }

        IEnumerator SetFoveationLevel()
        {
            int num = 3;
            bool result;
            do
            {
                if (FoveatedRenderingMode.EyeTrackedFoveatedRendering == foveatedRenderingMode)
                {
                    result = PXR_FoveationRendering.SetFoveationLevel(eyeFoveationLevel, true);
                }
                else
                {
                    result = PXR_FoveationRendering.SetFoveationLevel(foveationLevel, false);
                }
                PLog.i(TAG, "num = " + num + ", result = " + result);

                yield return new WaitForSeconds(1);
            } while (!result && num-- > 0);
        }

        void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                PXR_Plugin.Boundary.UPxr_SetSeeThroughBackground(EnableVideoSeeThrough);
                if (isNeedResume)
                {
                    StartCoroutine("StartXR");
                    isNeedResume = false;
                }
            }
        }

        private void OnApplicationQuit()
        {
            Debug.LogFormat(TAG_MRC + "OnApplicationQuit openMRC = {0} ,MRCInitSucceed = {1}.", openMRC, initMRCSucceed);
            if (openMRC && initMRCSucceed)
            {
                PXR_Plugin.Render.UPxr_DestroyLayer(LAYER_MRC);
            }
        }

        public IEnumerator StartXR()
        {
            yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

            if (XRGeneralSettings.Instance.Manager.activeLoader == null)
            {
                Debug.LogError("PXRLog Initializing XR Failed. Check log for details.");
            }
            else
            {
                XRGeneralSettings.Instance.Manager.StartSubsystems();
            }
        }

        void StopXR()
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }

        void Start()
        {
#if UNITY_EDITOR
            Application.targetFrameRate = 72;
#endif
            PXR_Plugin.Controller.UPxr_SetControllerDelay();

            if (adaptiveResolution)
            {
                XRSettings.eyeTextureResolutionScale = maxEyeTextureScale;
            }
        }

        void Update()
        {
            //recenter callback
            if (PXR_Plugin.System.UPxr_GetHomeKey())
            {
                if (PXR_Plugin.System.RecenterSuccess != null)
                {
                    PXR_Plugin.System.RecenterSuccess();
                }
                PXR_Plugin.System.UPxr_InitHomeKey();
            }

            if (openMRC && initMRCSucceed)
            {
                UpdateMRCCam();
            }

            //Adaptive Resolution
            if (adaptiveResolution)
            {
                UpdateAdaptiveResolution();
            }

            //pollEvent
            PollEvent();
        }

        void UpdateAdaptiveResolution()
        {
            float lastRenderScale = XRSettings.renderViewportScale;
            int newWidth = (int)((float)XRSettings.eyeTextureWidth * lastRenderScale);
            int success = PXR_Plugin.System.UPxr_UpdateAdaptiveResolution(ref newWidth, adaptiveResolutionPowerSetting);

            if (success == -1)
                return;

            float currRenderScale = (float)newWidth / (float)XRSettings.eyeTextureWidth;
            float minScale = minEyeTextureScale / maxEyeTextureScale;
            float newRenderScale = Mathf.Min(1.0f, Mathf.Max(currRenderScale, minScale));

            UnityEngine.XR.XRSettings.renderViewportScale = newRenderScale;
        }

        void OnEnable()
        {
            if (PXR_OverLay.Instances.Count > 0)
            {
                if (Camera.main.gameObject.GetComponent<PXR_OverlayManager>() == null)
                {
                    Camera.main.gameObject.AddComponent<PXR_OverlayManager>();
                }

                foreach (var layer in PXR_OverLay.Instances)
                {
                    if (eyeCamera[0] != null && eyeCamera[0].enabled)
                    {
                        layer.RefreshCamera(eyeCamera[0], eyeCamera[0]);
                    }
                    else if (eyeCamera[1] != null && eyeCamera[1].enabled)
                    {
                        layer.RefreshCamera(eyeCamera[1], eyeCamera[2]);
                    }
                }
            }
            if (openMRC)
            {
                if (GraphicsDeviceType.Vulkan == SystemInfo.graphicsDeviceType)
                {
                    PXR_Plugin.Sensor.UPxr_HMDUpdateSwitch(false);
                }

                if (GraphicsSettings.renderPipelineAsset != null)
                {
                    RenderPipelineManager.endFrameRendering += EndRendering;
                }
                else
                {
                    Camera.onPostRender += OnPostRenderCallBack;
                }
            }
        }

        private void LateUpdate()
        {
            if (appSpaceWarp && m_AppSpaceTransform != null)
            {
                PXR_Plugin.Render.UPxr_SetAppSpacePosition(m_AppSpaceTransform.position.x, m_AppSpaceTransform.position.y, m_AppSpaceTransform.position.z);
                PXR_Plugin.Render.UPxr_SetAppSpaceRotation(m_AppSpaceTransform.rotation.x, m_AppSpaceTransform.rotation.y, m_AppSpaceTransform.rotation.z, m_AppSpaceTransform.rotation.w);
            }
        }

        private void PollEvent()
        {
            eventList.Clear();
            bool ret = PXR_Plugin.MixedReality.UPxr_PollEventQueue(ref eventList);
            if (ret)
            {
                for (int i = 0; i < eventList.Count; i++)
                {
                    PLog.d("PXRLog", "PollEvent" + eventList[i].type);
                    switch (eventList[i].type)
                    {
                        case PxrStructureType.AnchorEntityCreated:
                            {
                                if (AnchorEntityCreated != null)
                                {
                                    PxrEventAnchorEntityCreated info = new PxrEventAnchorEntityCreated()
                                    {
                                        taskId = BitConverter.ToUInt64(eventList[i].data, 0),
                                        result = (PxrResult)BitConverter.ToInt32(eventList[i].data, 8),
                                        anchorHandle = BitConverter.ToUInt64(eventList[i].data, 16),
                                    };

                                    byte[] byteArray = new byte[16];
                                    var value0 = BitConverter.ToUInt64(eventList[i].data, 24);
                                    var value1 = BitConverter.ToUInt64(eventList[i].data, 32);
                                    BitConverter.GetBytes(value0).CopyTo(byteArray, 0);
                                    BitConverter.GetBytes(value1).CopyTo(byteArray, 8);
                                    info.uuid = new Guid(byteArray);
                                    AnchorEntityCreated(info);
                                }
                            }
                            break;
                        case PxrStructureType.AnchorEntityPersisted:
                            {
                                if (AnchorEntityPersisted != null)
                                {
                                    PxrEventAnchorEntityPersisted info = new PxrEventAnchorEntityPersisted()
                                    {
                                        taskId = BitConverter.ToUInt64(eventList[i].data, 0),
                                        result = (PxrResult)BitConverter.ToInt32(eventList[i].data, 8),
                                        location = (PxrPersistLocation)BitConverter.ToInt32(eventList[i].data, 12)
                                    };
                                    AnchorEntityPersisted(info);
                                }
                            }
                            break;
                        case PxrStructureType.AnchorEntityUnPersisted:
                            {
                                if (AnchorEntityUnPersisted != null)
                                {
                                    PxrEventAnchorEntityUnPersisted info = new PxrEventAnchorEntityUnPersisted()
                                    {
                                        taskId = BitConverter.ToUInt64(eventList[i].data, 0),
                                        result = (PxrResult)BitConverter.ToInt32(eventList[i].data, 8),
                                        location = (PxrPersistLocation)BitConverter.ToInt32(eventList[i].data, 12)
                                    };
                                    AnchorEntityUnPersisted(info);
                                }
                            }
                            break;
                        case PxrStructureType.AnchorEntityCleared:
                            {
                                if (AnchorEntityCleared != null)
                                {
                                    PxrEventAnchorEntityCleared info = new PxrEventAnchorEntityCleared()
                                    {
                                        taskId = BitConverter.ToUInt64(eventList[i].data, 0),
                                        result = (PxrResult)BitConverter.ToInt32(eventList[i].data, 8),
                                        location = (PxrPersistLocation)BitConverter.ToInt32(eventList[i].data, 12)
                                    };
                                    AnchorEntityCleared(info);
                                }
                            }
                            break;
                        case PxrStructureType.AnchorEntityLoaded:
                            {
                                if (AnchorEntityLoaded != null)
                                {
                                    PxrEventAnchorEntityLoaded info = new PxrEventAnchorEntityLoaded()
                                    {
                                        taskId = BitConverter.ToUInt64(eventList[i].data, 0),
                                        result = (PxrResult)BitConverter.ToInt32(eventList[i].data, 8),
                                        count = BitConverter.ToUInt32(eventList[i].data, 12),
                                        location = (PxrPersistLocation)BitConverter.ToInt32(eventList[i].data, 16)
                                    };
                                    AnchorEntityLoaded(info);
                                }
                            }
                            break;
                        case PxrStructureType.SpatialTrackingStateUpdate:
                            {
                                if (SpatialTrackingStateUpdate != null)
                                {
                                    PxrEventSpatialTrackingStateUpdate info = new PxrEventSpatialTrackingStateUpdate()
                                    {
                                        state = (PxrSpatialTrackingState)BitConverter.ToInt32(eventList[i].data, 0),
                                        message = (PxrSpatialTrackingStateMessage)BitConverter.ToInt32(eventList[i].data, 4),
                                    };
                                    SpatialTrackingStateUpdate(info);
                                }
                            }
                            break;
                        case PxrStructureType.SpatialSceneCaptured:
                            {
                                if (SpatialSceneCaptured != null)
                                {
                                    PxrEventSpatialSceneCaptured info = new PxrEventSpatialSceneCaptured()
                                    {
                                        taskId = BitConverter.ToUInt64(eventList[i].data, 0),
                                        result = (PxrResult)BitConverter.ToInt32(eventList[i].data, 8),
                                        status = (PxrSpatialSceneCaptureStatus)BitConverter.ToUInt32(eventList[i].data, 12),
                                    };
                                    SpatialSceneCaptured(info);
                                }
                            }
                            break;
                        case PxrStructureType.AutoRoomCaptureUpdated:
                            {
                                if (AutoRoomCaptureUpdated != null)
                                {
                                    PxrEventAutoRoomCaptureUpdated info = new PxrEventAutoRoomCaptureUpdated()
                                    {
                                        state = (PxrSpatialSceneCaptureStatus)BitConverter.ToUInt32(eventList[i].data, 0),
                                        msg = BitConverter.ToUInt32(eventList[i].data, 4),
                                    };
                                    AutoRoomCaptureUpdated(info);
                                }
                            }
                            break;
                        case PxrStructureType.SpatialMapSizeLimited:
                            {
                                if (SpatialMapSizeLimited != null)
                                {
                                    var reason = (PxrSpatialMapSizeLimitedReason)BitConverter.ToInt32(eventList[i].data, 0);
                                    SpatialMapSizeLimited(reason);
                                }
                            }
                            break;
                        case PxrStructureType.SenseDataUpdated:
                            {
                                ulong providerHandle = BitConverter.ToUInt64(eventList[i].data, 0);
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
                                    StartCoroutine(QuerySpatialMeshAnchor());
                                }

                                if (providerHandle == PXR_Plugin.MixedReality.SemiAutoSceneCaptureProviderHandle)
                                {
                                    if (SemiAutoCaptureDataUpdated != null)
                                    {
                                        SemiAutoCaptureDataUpdated();
                                    }
                                }

                            }
                            break;
                        case PxrStructureType.SenseDataProviderStateChanged:
                            {
                                if (SenseDataProviderStateChanged != null)
                                {
                                    PxrEventSenseDataProviderStateChanged data = new PxrEventSenseDataProviderStateChanged()
                                    {
                                        providerHandle = BitConverter.ToUInt64(eventList[i].data, 0),
                                        newState = (PxrSenseDataProviderState)BitConverter.ToInt32(eventList[i].data, 8),
                                    };
                                    SenseDataProviderStateChanged(data);
                                }
                            }
                            break;
                        case PxrStructureType.VSTDisplayStatusChanged:
                            {
                                if (VstDisplayStatusChanged != null)
                                {
                                    var status = (PxrVstStatus)BitConverter.ToUInt32(eventList[i].data, 0);
                                    VstDisplayStatusChanged(status);
                                }
                            }
                            break;
                    }
                }
            }
        }

        private IEnumerator QuerySpatialMeshAnchor()
        {
            var task = PXR_MixedReality.QueryMeshAnchorAsync();
            yield return new WaitUntil(() => task.IsCompleted);

            var (result, meshInfos) = task.Result;

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

        public void SetSpaceWarp(bool enabled)
        {
            for (int i = 0; i < 3; i++)
            {
                if (eyeCamera[i] != null && eyeCamera[i].enabled)
                {
                    if (enabled)
                    {
                        m_CachedDepthTextureMode = eyeCamera[i].depthTextureMode;
                        eyeCamera[i].depthTextureMode |= (DepthTextureMode.MotionVectors | DepthTextureMode.Depth);

                        if (eyeCamera[i].transform.parent == null)
                        {
                            m_AppSpaceTransform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                        }
                        else
                        {
                            m_AppSpaceTransform = eyeCamera[i].transform.parent;
                        }
                    }
                    else
                    {
                        eyeCamera[i].depthTextureMode = m_CachedDepthTextureMode;
                        m_AppSpaceTransform = null;
                    }
                }
            }
            PXR_Plugin.Render.UPxr_SetSpaceWarp(enabled);
            appSpaceWarp = enabled;
        }


        void OnDisable()
        {
            StopAllCoroutines();
            if (openMRC)
            {
                if (GraphicsDeviceType.Vulkan == SystemInfo.graphicsDeviceType)
                {
                    PXR_Plugin.Sensor.UPxr_HMDUpdateSwitch(true);
                }

                if (GraphicsSettings.renderPipelineAsset != null)
                {
                    RenderPipelineManager.endFrameRendering -= EndRendering;
                }
                else
                {
                    Camera.onPostRender -= OnPostRenderCallBack;
                }
            }
        }

        public Camera[] GetEyeCamera()
        {
            return eyeCamera;
        }


        #region MRC FUNC
        private const string TAG_MRC = "PXR MRC ";
        private const int LAYER_MRC = 99999;
        private GameObject mrcCube;

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

        public void OnPostRenderCallBack(Camera cam)
        {
            if (cam == null || cam != Camera.main || cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right) return;

            if (openMRC && PXR_Plugin.System.UPxr_GetMRCEnable() && PXR_Plugin.Boundary.seeThroughState != 2)
            {
                CreateMRCOverlay();
                CopyAndSubmitMRCLayer();
            }
        }


        private void CreateMRCOverlay()
        {
            PLog.d(TAG_MRC, $"CreateMRCOverlay. mrcXmlCamData={mrcXmlCamData}, initMRCSucceed={initMRCSucceed}, createMRCOverlaySucceed={createMRCOverlaySucceed}");
            if (createMRCOverlaySucceed) return;

            if (!mrcXmlCamData)
            {
                PXR_Plugin.System.UPxr_GetExternalCameraInfo(out cameraInfo);
                mrcCamObjActived = false;

                if (cameraInfo.width <= 0 || cameraInfo.height <= 0 || cameraInfo.fov <= 0)
                {
                    mrcXmlCamData = false;
                    PLog.e(TAG_MRC, "Abnormal calibration data, so MRC init failed!  mrcXmlCamData : false.");
                    return;
                }
                mrcXmlCamData = true;

                PLog.i(TAG_MRC, " mrcXmlCamData : true.");
            }

            if (!initMRCSucceed)
            {
                layerParam.layerId = LAYER_MRC;
                layerParam.layerShape = PXR_OverLay.OverlayShape.Quad;
                layerParam.layerType = PXR_OverLay.OverlayType.Overlay;
                layerParam.layerLayout = PXR_OverLay.LayerLayout.Stereo;
                layerParam.format = (UInt64)RenderTextureFormat.Default;
                layerParam.width = (uint)cameraInfo.width;
                layerParam.height = (uint)cameraInfo.height;
                layerParam.sampleCount = 1;
                layerParam.faceCount = 1;
                layerParam.arraySize = 1;
                layerParam.mipmapCount = 0;
                layerParam.layerFlags = 0;
                PXR_Plugin.Render.UPxr_CreateLayerParam(layerParam);

                initMRCSucceed = true;
                PLog.i(TAG_MRC, "Init Succeed. initMRCSucceed : true.");
            }

            if (null == layerTexturesInfo)
            {
                layerTexturesInfo = new LayerTexture[2];
            }

            for (int i = 0; i < 2; i++)
            {
                int ret = PXR_Plugin.Render.UPxr_GetLayerImageCount(LAYER_MRC, (EyeType)i, ref imageCounts);
                if (ret != 0 || imageCounts < 1)
                {
                    PLog.e(TAG_MRC, "UPxr_GetLayerImageCount failed, i:" + i);
                    continue;
                }
                if (layerTexturesInfo[i].swapChain == null)
                {
                    layerTexturesInfo[i].swapChain = new Texture[imageCounts];
                }
                for (int j = 0; j < imageCounts; j++)
                {
                    IntPtr ptr = IntPtr.Zero;
                    PXR_Plugin.Render.UPxr_GetLayerImagePtr(LAYER_MRC, (EyeType)i, j, ref ptr);

                    if (IntPtr.Zero == ptr)
                    {
                        PLog.e(TAG_MRC, "UPxr_GetLayerImagePtr is Zero, i:" + i);
                        continue;
                    }

                    Texture texture = Texture2D.CreateExternalTexture((int)cameraInfo.width, (int)cameraInfo.height, TextureFormat.RGBA32, false, true, ptr);

                    if (null == texture)
                    {
                        PLog.e(TAG_MRC, "CreateExternalTexture texture null, i:" + i);
                        continue;
                    }

                    layerTexturesInfo[i].swapChain[j] = texture;
                }

                createMRCOverlaySucceed = true;
                PLog.i(TAG_MRC, " UPxr_GetLayerImagePtr createMRCOverlaySucceed : true. i:" + i);
            }
        }

        public void CopyAndSubmitMRCLayer()
        {
            PLog.d(TAG_MRC, $"CopyAndSubmitMRCLayer. initMRCSucceed={initMRCSucceed}, createMRCOverlaySucceed={createMRCOverlaySucceed}");
            if (!initMRCSucceed || !createMRCOverlaySucceed) return;

            if (GraphicsDeviceType.Vulkan == SystemInfo.graphicsDeviceType)
            {
                PXR_Plugin.Render.UPxr_GetLayerNextImageIndex(LAYER_MRC, ref imageIndex);
            }
            else
            {

                PXR_Plugin.Render.UPxr_GetLayerNextImageIndexByRender(LAYER_MRC, ref imageIndex);
            }

            for (int eyeId = 0; eyeId < 2; ++eyeId)
            {
                Texture dstT = layerTexturesInfo[eyeId].swapChain[imageIndex];

                if (dstT == null)
                {
                    PLog.e(TAG_MRC, "dstT is null, eyeId:" + eyeId);
                    continue;
                }

                RenderTexture rt = (0 == eyeId) ? mrcBackgroundRT : mrcForegroundRT;

                RenderTexture tempRT = null;

                if (!(QualitySettings.activeColorSpace == ColorSpace.Gamma && rt != null && rt.format == RenderTextureFormat.ARGB32))
                {
                    RenderTextureDescriptor descriptor = new RenderTextureDescriptor((int)cameraInfo.width, (int)cameraInfo.height, RenderTextureFormat.ARGB32, 0); descriptor.msaaSamples = 1;
                    descriptor.useMipMap = false;
                    descriptor.autoGenerateMips = false;
                    descriptor.sRGB = false;
                    tempRT = RenderTexture.GetTemporary(descriptor);

                    if (!tempRT.IsCreated())
                    {
                        tempRT.Create();
                    }
                    rt.DiscardContents();
                    tempRT.DiscardContents();

                    Graphics.Blit(rt, tempRT);
                    Graphics.CopyTexture(tempRT, 0, 0, dstT, 0, 0);
                }
                else
                {
                    Graphics.CopyTexture(rt, 0, 0, dstT, 0, 0);
                }

                if (tempRT != null)
                {
                    RenderTexture.ReleaseTemporary(tempRT);
                }
            }

            PxrLayerQuad layerSubmit = new PxrLayerQuad();
            layerSubmit.header.layerId = LAYER_MRC;
            layerSubmit.header.layerFlags = (UInt32)PxrLayerSubmitFlags.PxrLayerFlagMRCComposition;
            layerSubmit.width = 1.0f;
            layerSubmit.height = 1.0f;
            layerSubmit.header.colorScaleX = 1.0f;
            layerSubmit.header.colorScaleY = 1.0f;
            layerSubmit.header.colorScaleZ = 1.0f;
            layerSubmit.header.colorScaleW = 1.0f;
            layerSubmit.pose.orientation.w = 1.0f;
            layerSubmit.header.headPose.orientation.x = 0;
            layerSubmit.header.headPose.orientation.y = 0;
            layerSubmit.header.headPose.orientation.z = 0;
            layerSubmit.header.headPose.orientation.w = 1;

            if (layerSubmitPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(layerSubmitPtr);
                layerSubmitPtr = IntPtr.Zero;
            }
            layerSubmitPtr = Marshal.AllocHGlobal(Marshal.SizeOf(layerSubmit));
            Marshal.StructureToPtr(layerSubmit, layerSubmitPtr, false);
            PXR_Plugin.Render.UPxr_SubmitLayerQuadByRender(layerSubmitPtr);
        }

        private void UpdateMRCCam()
        {
            PLog.d(TAG_MRC, $"UpdateMRCCam. openMRC={openMRC}, initMRCSucceed={initMRCSucceed}");

            if (!PXR_Plugin.System.UPxr_GetMRCEnable())
            {
                if (mrcCamObjActived)
                {
                    mrcCamObjActived = false;
                    backgroundCamObj.SetActive(false);
                    foregroundCamObj.SetActive(false);
                    PXR_Plugin.Boundary.SeethroughStateChangedAction -= SeethroughStateChangedMethod;
                }
                return;
            }

            if (null != Camera.main.transform && (null == backgroundCamObj || !mrcCamObjActived))
            {
                CreateMRCCam();
            }

            if (PLog.LogLevel.LogVerbose < PLog.logLevel && null != backgroundCamObj)
            {
                if (mrcCube == null)
                {
                    mrcCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    mrcCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.2f);
                    mrcCube.transform.parent = backgroundCamObj.transform;
                    mrcCube.transform.localPosition = Vector3.zero;
                    mrcCube.transform.localEulerAngles = Vector3.zero;
                    PLog.d(TAG_MRC, "create background camera object cube.");

                    if (GraphicsSettings.renderPipelineAsset != null)
                    {
                        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                        Renderer renderer = mrcCube.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            renderer.sharedMaterial = material;
                            PLog.d(TAG_MRC, "set urp material for cube.");
                        }
                    }
                }
            }
            else
            {
                if (mrcCube != null)
                {
                    Destroy(mrcCube);
                    PLog.d(TAG_MRC, "Destroy background camera object cube.");
                }
            }

            if (null != foregroundCamObj)
            {
                Vector3 cameraLookAt = Camera.main.transform.position - foregroundCamObj.transform.position;
                float distance = Vector3.Dot(cameraLookAt, foregroundCamObj.transform.forward);
                foregroundCamObj.GetComponent<Camera>().farClipPlane = Mathf.Max(foregroundCamObj.GetComponent<Camera>().nearClipPlane + 0.001f, distance);
            }

            CalibrationMRCCam();
        }

        public void CreateMRCCam()
        {
            if (backgroundCamObj == null)
            {
                backgroundCamObj = new GameObject("myBackgroundCamera");
                backgroundCamObj.transform.parent = Camera.main.transform.parent;
                backgroundCamObj.AddComponent<Camera>();
                PLog.i(TAG_MRC, "create background camera object.");
            }
            InitMRCCam(backgroundCamObj.GetComponent<Camera>(), false);
            backgroundCamObj.SetActive(true);

            if (foregroundCamObj == null)
            {
                foregroundCamObj = new GameObject("myForegroundCamera");
                foregroundCamObj.transform.parent = Camera.main.transform.parent;
                foregroundCamObj.AddComponent<Camera>();
                PLog.i(TAG_MRC, "create foreground camera object.");
            }
            InitMRCCam(foregroundCamObj.GetComponent<Camera>(), true);
            foregroundCamObj.SetActive(true);

            mrcCamObjActived = true;
            PXR_Plugin.Boundary.SeethroughStateChangedAction += SeethroughStateChangedMethod;

            PLog.i(TAG_MRC, "Camera Obj Actived. mrcCamObjActived : true.");
        }

        private void SeethroughStateChangedMethod(int status)
        {
            PLog.i(TAG_MRC, $"SeethroughStateChangedMethod status = {status}, backgroundCamObj = {backgroundCamObj != null}");
            if (backgroundCamObj != null)
            {
                Camera camera = backgroundCamObj.GetComponent<Camera>();
                if (3 == status) // MR
                {
                    camera.clearFlags = CameraClearFlags.SolidColor;
                    camera.backgroundColor = foregroundColor;
                }
                else if (0 == status) // VR
                {
                    camera.clearFlags = Camera.main.clearFlags;
                    camera.backgroundColor = Camera.main.backgroundColor;
                }
            }
        }

        private void InitMRCCam(Camera camera, bool isForeground)
        {
            camera.stereoTargetEye = StereoTargetEyeMask.None;
            camera.transform.localScale = Vector3.one;
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localEulerAngles = Vector3.zero;
            camera.gameObject.layer = 0;
            camera.orthographic = false;
            camera.fieldOfView = cameraInfo.fov;
            camera.aspect = (float)cameraInfo.width / cameraInfo.height;
            camera.allowMSAA = true;
            if (isForeground)
            {
                camera.depth = 10000;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.cullingMask = foregroundLayerMask;
                camera.backgroundColor = foregroundColor;
                if (mrcForegroundRT == null)
                {
                    mrcForegroundRT = new RenderTexture((int)cameraInfo.width, (int)cameraInfo.height, 24, RenderTextureFormat.ARGB32);
                }
                mrcForegroundRT.name = "foregroundMrcRenderTexture";
                camera.targetTexture = mrcForegroundRT;
                PLog.i(TAG_MRC, "init foreground camera.");
            }
            else
            {
                camera.depth = 9999;
                camera.clearFlags = Camera.main.clearFlags;
                camera.cullingMask = backgroundLayerMask;
                camera.backgroundColor = Camera.main.backgroundColor;
                if (camera.clearFlags == CameraClearFlags.SolidColor && camera.backgroundColor == new Color(0, 0, 0, 0)) // MR
                {
                    camera.backgroundColor = foregroundColor;
                }
                if (mrcBackgroundRT == null)
                {
                    mrcBackgroundRT = new RenderTexture((int)cameraInfo.width, (int)cameraInfo.height, 24, RenderTextureFormat.ARGB32);
                }
                mrcBackgroundRT.name = "backgroundMrcRenderTexture";
                camera.targetTexture = mrcBackgroundRT;
                PLog.i(TAG_MRC, "init background camera.");
            }
        }

        public void CalibrationMRCCam()
        {
            if (!PXR_Plugin.System.UPxr_GetMRCEnable() || null == backgroundCamObj || null == foregroundCamObj) return;

            PxrPosef pose = new PxrPosef();
            PxrTrackingOrigin mode = new PxrTrackingOrigin();
            PXR_Plugin.System.UPxr_GetTrackingOrigin(ref mode);
            PXR_Plugin.System.UPxr_GetExternalCameraPose(mode, ref pose);

            backgroundCamObj.transform.localPosition = new Vector3(pose.position.x, pose.position.y, (-pose.position.z) * 1f);
            foregroundCamObj.transform.localPosition = new Vector3(pose.position.x, pose.position.y, (-pose.position.z) * 1f);

            Vector3 rototion = new Quaternion(pose.orientation.x, pose.orientation.y, pose.orientation.z, pose.orientation.w).eulerAngles;
            backgroundCamObj.transform.localEulerAngles = new Vector3(-rototion.x, -rototion.y, -rototion.z);
            foregroundCamObj.transform.localEulerAngles = new Vector3(-rototion.x, -rototion.y, -rototion.z);

            PLog.d(TAG_MRC, $"CalibrationMRCCam backgroundCamObj.transform.localPosition={ backgroundCamObj.transform.localPosition}");
        }
        #endregion
    }
}