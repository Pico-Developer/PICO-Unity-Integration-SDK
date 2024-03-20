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

        private float refreshRate = -1.0f;
        private Camera[] eyeCamera;
        private bool appSpaceWarp;
        private Transform m_AppSpaceTransform;
        private DepthTextureMode m_CachedDepthTextureMode;

        [HideInInspector]
        public bool screenFade;
        [HideInInspector]
        public bool eyeTracking;
        [HideInInspector]
        public FaceTrackingMode trackingMode = FaceTrackingMode.None;
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
        public LayerMask backLayerMask = -1;
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

        private static CameraData xmlCamData;

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

        public Action<float> DisplayRefreshRateChanged;

        [HideInInspector]
        public bool useRecommendedAntiAliasingLevel = true;

        private List<PxrEventDataBuffer> eventList = new List<PxrEventDataBuffer>();


        //New
        public static event Action<PxrEventAnchorEntityCreated> AnchorEntityCreated;
        public static event Action<PxrEventSpatialTrackingStateUpdate> SpatialTrackingStateUpdate;
        public static event Action<PxrEventAnchorEntityPersisted> AnchorEntityPersisted;
        public static event Action<PxrEventAnchorEntityUnPersisted> AnchorEntityUnPersisted;
        public static event Action<PxrEventAnchorEntityCleared> AnchorEntityCleared;
        public static event Action<PxrEventAnchorEntityLoaded> AnchorEntityLoaded;
        public static event Action<PxrEventSpatialSceneCaptured> SpatialSceneCaptured;
        public static event Action<PxrEventSemiAutoCandidatesUpdate> SemiAutoCandidatesUpdate;

        //Deprecate
        public static event Action<PxrEventSpatialAnchorSaveResult> SpatialAnchorSaveResult;
        public static event Action<PxrEventSpatialAnchorDeleteResult> SpatialAnchorDeleteResult;
        public static event Action<PxrEventSpatialAnchorLoadResults> SpatialAnchorLoadResults;
        public static event Action<PxrEventSpatialAnchorLoadResultsAvailable> SpatialAnchorLoadResultsAvailable;
        public static event Action<PxrEventSpatialAnchorLoadResultsComplete> SpatialAnchorLoadResultsComplete;
        public static event Action<PxrEventRoomSceneLoadResultsComplete> RoomSceneLoadResultsComplete;
        public static event Action<PxrEventRoomSceneDataSaveResult> RoomSceneDataSaveResult;
        public static event Action<PxrEventRoomSceneDataDeleteResult> RoomSceneDataDeleteResult;
        public static event Action<PxrEventRoomSceneDataUpdateResult> RoomSceneDataUpdateResult;
        public static event Action<PxrSemiAutoRoomCaptureCandidatesUpdate> SemiAutoRoomCaptureCandidatesUpdate;
        public static event Action<PxrEventSpatialTrackingStateInfo> SpatialTrackingStateInfo;

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
            }

            if (bodyTracking)
            {
                PXR_Plugin.Controller.UPxr_SetBodyTrackingMode(1);
            }

            Debug.LogFormat(TAG_MRC + "Awake openMRC = {0} ,MRCInitSucceed = {1}.", openMRC, initMRCSucceed);
            //if (openMRC && initMRCSucceed == false)
            //{
            //    MRCInitialize();
            //}
            PXR_Plugin.System.UPxr_LogSdkApi("pico_msaa|" + QualitySettings.antiAliasing.ToString());
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
            if (Math.Abs(refreshRate - PXR_Plugin.System.UPxr_RefreshRateChanged()) > 0.1f)
            {
                refreshRate = PXR_Plugin.System.UPxr_RefreshRateChanged();
                if (DisplayRefreshRateChanged != null)
                    DisplayRefreshRateChanged(refreshRate);
            }
            //recenter callback
            if (PXR_Plugin.System.UPxr_GetHomeKey())
            {
                if (PXR_Plugin.System.RecenterSuccess != null)
                {
                    PXR_Plugin.System.RecenterSuccess();
                }
                PXR_Plugin.System.UPxr_InitHomeKey();
            }

            UpdateMRCCam();
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
            //Debug.Log(" RenderViewportScale: " + newRenderScale);

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
                PXR_Plugin.System.RecenterSuccess += CalibrationMRCCamCallback;
            }

            PXR_Plugin.System.LoglevelChangedChanged += LoglevelChangedCallback;
        }

        private void LateUpdate()
        {
            if (openMRC && PXR_Plugin.System.UPxr_GetMRCEnable())
            {
                MRCInitialize();
                CreateMRCOverlay();
                CopyAndSubmitMRCLayer();
            }

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
                    Debug.Log("PXRLog PollEventOfMixedReality" + eventList[i].type);
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
                        case PxrStructureType.PXR_TYPE_EVENT_DATA_SEETHROUGH_STATE_CHANGED:
                            {
                                PXR_Plugin.System.SeethroughStateChangedChanged((int)BitConverter.ToInt32(eventList[i].data, 0));
                            }
                            break;
                        case PxrStructureType.SemiAutoCandidatesUpdate:
                            {
                                if (SemiAutoCandidatesUpdate != null)
                                {
                                    PxrEventSemiAutoCandidatesUpdate data = new PxrEventSemiAutoCandidatesUpdate()
                                    {
                                        state = BitConverter.ToUInt32(eventList[i].data, 0),
                                        count = BitConverter.ToUInt32(eventList[i].data, 4),
                                    };
                                    SemiAutoCandidatesUpdate(data);
                                }
                            }
                            break;
                        case PxrStructureType.SpatialAnchorSaveResult:
                            {
                                if (SpatialAnchorSaveResult != null)
                                {
                                    PxrEventSpatialAnchorSaveResult result = new PxrEventSpatialAnchorSaveResult()
                                    {
                                        type = PxrStructureType.SpatialAnchorSaveResult,
                                        eventLevel = eventList[i].eventLevel,
                                        result = (PxrSpatialPersistenceResult)BitConverter.ToInt32(eventList[i].data, 0),
                                        asyncRequestId = BitConverter.ToUInt64(eventList[i].data, 8),
                                        uuid = new PxrSpatialInstanceUuid()
                                        {
                                            value0 = BitConverter.ToUInt64(eventList[i].data, 16),
                                            value1 = BitConverter.ToUInt64(eventList[i].data, 24),
                                        },
                                        handle = BitConverter.ToUInt64(eventList[i].data, 32),
                                        location = (PxrSpatialPersistenceLocation)BitConverter.ToInt32(eventList[i].data, 40)
                                    };
                                    SpatialAnchorSaveResult(result);
                                }
                            }
                            break;
                        case PxrStructureType.SpatialAnchorDeleteResult:
                            {
                                if (SpatialAnchorDeleteResult != null)
                                {
                                    PxrEventSpatialAnchorDeleteResult result = new PxrEventSpatialAnchorDeleteResult()
                                    {
                                        type = PxrStructureType.SpatialAnchorDeleteResult,
                                        eventLevel = eventList[i].eventLevel,
                                        result = (PxrSpatialPersistenceResult)BitConverter.ToInt32(eventList[i].data, 0),
                                        asyncRequestId = BitConverter.ToUInt64(eventList[i].data, 8),
                                        uuid = new PxrSpatialInstanceUuid()
                                        {
                                            value0 = BitConverter.ToUInt64(eventList[i].data, 16),
                                            value1 = BitConverter.ToUInt64(eventList[i].data, 24),
                                        },
                                        location = (PxrSpatialPersistenceLocation)BitConverter.ToInt32(eventList[i].data, 32)
                                    };
                                    SpatialAnchorDeleteResult(result);
                                }
                            }
                            break;
                        case PxrStructureType.SpatialAnchorLoadResults:
                            {
                                if (SpatialAnchorLoadResults != null)
                                {
                                    PxrEventSpatialAnchorLoadResults results = new PxrEventSpatialAnchorLoadResults()
                                    {
                                        type = PxrStructureType.SpatialAnchorLoadResults,
                                        eventLevel = eventList[i].eventLevel,
                                        result = (PxrSpatialPersistenceResult)BitConverter.ToInt32(eventList[i].data, 0),
                                        hasNext = BitConverter.ToBoolean(eventList[i].data, 4),
                                        asyncRequestId = BitConverter.ToUInt64(eventList[i].data, 8),
                                        numResults = BitConverter.ToUInt32(eventList[i].data, 16),
                                        loadResults = new PxrSpatialAnchorLoadResult[BitConverter.ToUInt32(eventList[i].data, 16)],
                                    };
                                    int offset = 24;
                                    for (int j = 0; j < results.numResults; j++)
                                    {
                                        results.loadResults[j] = new PxrSpatialAnchorLoadResult();
                                        results.loadResults[j].anchorHandle = BitConverter.ToUInt64(eventList[i].data, offset);
                                        offset += 8;
                                        results.loadResults[j].uuid = new PxrSpatialInstanceUuid();
                                        results.loadResults[j].uuid.value0 = BitConverter.ToUInt64(eventList[i].data, offset);
                                        offset += 8;
                                        results.loadResults[j].uuid.value1 = BitConverter.ToUInt64(eventList[i].data, offset);
                                        offset += 8;
                                    }
                                    SpatialAnchorLoadResults(results);
                                }
                            }
                            break;
                        case PxrStructureType.SpatialAnchorLoadResultsAvailable:
                            {
                                if (SpatialAnchorLoadResultsAvailable != null)
                                {
                                    PxrEventSpatialAnchorLoadResultsAvailable data = new PxrEventSpatialAnchorLoadResultsAvailable()
                                    {
                                        type = PxrStructureType.SpatialAnchorLoadResultsAvailable,
                                        eventLevel = eventList[i].eventLevel,
                                        asyncRequestId = BitConverter.ToUInt64(eventList[i].data, 0),
                                    };
                                    SpatialAnchorLoadResultsAvailable(data);
                                }
                            }
                            break;
                        case PxrStructureType.SpatialAnchorLoadResultsComplete:
                            {
                                if (SpatialAnchorLoadResultsComplete != null)
                                {
                                    PxrEventSpatialAnchorLoadResultsComplete data = new PxrEventSpatialAnchorLoadResultsComplete()
                                    {
                                        type = PxrStructureType.SpatialAnchorLoadResultsComplete,
                                        eventLevel = eventList[i].eventLevel,
                                        asyncRequestId = BitConverter.ToUInt64(eventList[i].data, 0),
                                        result = (PxrSpatialPersistenceResult)BitConverter.ToInt32(eventList[i].data, 8),
                                    };
                                    SpatialAnchorLoadResultsComplete(data);
                                }
                            }
                            break;
                        case PxrStructureType.RoomSceneDataSaveResult:
                            {
                                if (RoomSceneDataSaveResult != null)
                                {
                                    PxrEventRoomSceneDataSaveResult data = new PxrEventRoomSceneDataSaveResult()
                                    {
                                        type = PxrStructureType.RoomSceneDataSaveResult,
                                        eventLevel = eventList[i].eventLevel,
                                        result = (PxrSpatialPersistenceResult)BitConverter.ToInt32(eventList[i].data, 0),
                                        asyncRequestId = BitConverter.ToUInt64(eventList[i].data, 8),
                                        handle = BitConverter.ToUInt64(eventList[i].data, 16),
                                        location = (PxrSpatialPersistenceLocation)BitConverter.ToInt32(eventList[i].data, 24),
                                    };
                                    RoomSceneDataSaveResult(data);
                                }
                            }
                            break;
                        case PxrStructureType.RoomSceneDataDeleteResult:
                            {
                                if (RoomSceneDataDeleteResult != null)
                                {
                                    PxrEventRoomSceneDataDeleteResult data = new PxrEventRoomSceneDataDeleteResult()
                                    {
                                        type = PxrStructureType.RoomSceneDataDeleteResult,
                                        eventLevel = eventList[i].eventLevel,
                                        result = (PxrSpatialPersistenceResult)BitConverter.ToInt32(eventList[i].data, 0),
                                        asyncRequestId = BitConverter.ToUInt64(eventList[i].data, 8),
                                        handle = BitConverter.ToUInt64(eventList[i].data, 16),
                                        location = (PxrSpatialPersistenceLocation)BitConverter.ToInt32(eventList[i].data, 24),
                                    };
                                    RoomSceneDataDeleteResult(data);
                                }
                            }
                            break;
                        case PxrStructureType.RoomSceneLoadResultsComplete:
                            {
                                if (RoomSceneLoadResultsComplete != null)
                                {
                                    PxrEventRoomSceneLoadResultsComplete data = new PxrEventRoomSceneLoadResultsComplete()
                                    {
                                        type = PxrStructureType.RoomSceneLoadResultsComplete,
                                        eventLevel = eventList[i].eventLevel,
                                        asyncRequestId = BitConverter.ToUInt64(eventList[i].data, 0),
                                        result = (PxrSpatialPersistenceResult)BitConverter.ToInt32(eventList[i].data, 8),
                                    };
                                    RoomSceneLoadResultsComplete(data);
                                }
                            }
                            break;
                        case PxrStructureType.RoomSceneDataUpdateResult:
                            {
                                if (RoomSceneDataUpdateResult != null)
                                {
                                    PxrEventRoomSceneDataUpdateResult data = new PxrEventRoomSceneDataUpdateResult()
                                    {
                                        type = PxrStructureType.RoomSceneDataUpdateResult,
                                        level = eventList[i].eventLevel,
                                        anchorUuid = new PxrSpatialInstanceUuid()
                                        {
                                            value0 = BitConverter.ToUInt64(eventList[i].data, 0),
                                            value1 = BitConverter.ToUInt64(eventList[i].data, 8),
                                        },
                                        roomSceneDataHandle = BitConverter.ToUInt64(eventList[i].data, 16),
                                        result = BitConverter.ToUInt32(eventList[i].data, 24),
                                        dataLength = BitConverter.ToUInt32(eventList[i].data, 28),
                                    };
                                    data.roomSceneData = new byte[data.dataLength];
                                    Buffer.BlockCopy(eventList[i].data, 32, data.roomSceneData, 0, (int)data.dataLength);
                                    RoomSceneDataUpdateResult(data);
                                }
                            }
                            break;
                        case PxrStructureType.SemiAutoRoomCaptureCandidatesUpdate:
                            {
                                if (SemiAutoRoomCaptureCandidatesUpdate != null)
                                {
                                    PxrSemiAutoRoomCaptureCandidatesUpdate data = new PxrSemiAutoRoomCaptureCandidatesUpdate()
                                    {
                                        type = PxrStructureType.SemiAutoRoomCaptureCandidatesUpdate,
                                        eventLevel = eventList[i].eventLevel,
                                        result = (PxrSpatialPersistenceResult)BitConverter.ToInt32(eventList[i].data, 0),
                                    };
                                    SemiAutoRoomCaptureCandidatesUpdate(data);
                                }
                            }
                            break;
                        case PxrStructureType.TrackingStateChanged:
                            {
                                if (SpatialTrackingStateInfo != null)
                                {
                                    PxrEventSpatialTrackingStateInfo info = new PxrEventSpatialTrackingStateInfo()
                                    {
                                        type = PxrStructureType.TrackingStateChanged,
                                        eventLevel = eventList[i].eventLevel,
                                        stateInfo = new PxrSpatialTrackingStateInfo()
                                        {
                                            state = (PxrSpatialTrackingState)BitConverter.ToInt32(eventList[i].data, 0),
                                            message = (PxrSpatialTrackingStateMessage)BitConverter.ToInt32(eventList[i].data, 4),
                                        },
                                    };
                                    SpatialTrackingStateInfo(info);
                                }
                            }
                            break;
                    }
                }
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
                PXR_Plugin.System.RecenterSuccess -= CalibrationMRCCamCallback;
            }

            PXR_Plugin.System.LoglevelChangedChanged -= LoglevelChangedCallback;
        }

        void LoglevelChangedCallback(int value)
        {
            PLog.logLevel = (PLog.LogLevel)value;
            PLog.i(TAG, "LoglevelChangedCallback value " + value);
        }

        public Camera[] GetEyeCamera()
        {
            return eyeCamera;
        }


        #region MRC FUNC
        private const string TAG_MRC = "PXR MRC ";
        private const int LAYER_MRC = 99999;
        private GameObject mrcCube;

        public void MRCInitialize()
        {
            if (mrcXmlCamData)
            {
                return;
            }
            xmlCamData = new CameraData();
            string path = Application.persistentDataPath + "/mrc.xml";
            cameraAttribute = PXR_Plugin.PlatformSetting.UPxr_MRCCalibration(path);
            PLog.i(TAG_MRC, "cameraDataLength: " + cameraAttribute.Length);
            for (int i = 0; i < cameraAttribute.Length; i++)
            {
                PLog.i(TAG_MRC, "cameraData: " + i.ToString() + ": " + cameraAttribute[i].ToString());
            }
            xmlCamData.imageW = cameraAttribute[0];
            xmlCamData.imageH = cameraAttribute[1];
            xmlCamData.yFov = cameraAttribute[2];
            xmlCamData.position.x = cameraAttribute[3];
            xmlCamData.position.y = cameraAttribute[4];
            xmlCamData.position.z = cameraAttribute[5];
            xmlCamData.orientation.x = cameraAttribute[6];
            xmlCamData.orientation.y = cameraAttribute[7];
            xmlCamData.orientation.z = cameraAttribute[8];
            xmlCamData.orientation.w = cameraAttribute[9];

            Invoke("Pxr_GetHeight", 0.5f);
            PXR_Plugin.System.UPxr_SetIsSupportMovingMrc(true);
            PxrPosef pose = new PxrPosef();
            pose.position = xmlCamData.position;
            pose.orientation = xmlCamData.orientation;
            PXR_Plugin.System.UPxr_SetMrcPose(ref pose);
            PXR_Plugin.System.UPxr_GetMrcPose(ref pose);
            xmlCamData.position = pose.position;
            xmlCamData.orientation = pose.orientation;
            mrcCamObjActived = false;

            PXR_Plugin.System.UPxr_SetMrcTextutrWidth((ulong)xmlCamData.imageW);
            PXR_Plugin.System.UPxr_SetMrcTextutrHeight((ulong)xmlCamData.imageH);

            if (xmlCamData.imageW <= 0 || xmlCamData.imageH <= 0)
            {
                mrcXmlCamData = false;
                PLog.e(TAG_MRC, "Abnormal calibration data, so MRC init failed!  mrcXmlCamData : false.");
                return;
            }
            mrcXmlCamData = true;

            PLog.i(TAG_MRC, " mrcXmlCamData : true.");
        }

        private void CreateMRCOverlay()
        {
            PLog.d(TAG_MRC, $"CreateMRCOverlay. mrcXmlCamData={mrcXmlCamData}, initMRCSucceed={initMRCSucceed}, createMRCOverlaySucceed={createMRCOverlaySucceed}");
            if (!mrcXmlCamData)
            {
                return;
            }

            if (!initMRCSucceed)
            {
                layerParam.layerId = LAYER_MRC;
                layerParam.layerShape = PXR_OverLay.OverlayShape.Quad;
                layerParam.layerType = PXR_OverLay.OverlayType.Overlay;
                layerParam.layerLayout = PXR_OverLay.LayerLayout.Stereo;
                layerParam.format = (UInt64)RenderTextureFormat.Default;
                layerParam.width = (uint)xmlCamData.imageW;
                layerParam.height = (uint)xmlCamData.imageH;
                layerParam.sampleCount = 1;
                layerParam.faceCount = 1;
                layerParam.arraySize = 1;
                layerParam.mipmapCount = 0;
                layerParam.layerFlags = 0;
                PXR_Plugin.Render.UPxr_CreateLayerParam(layerParam);

                initMRCSucceed = true;
                PLog.i(TAG_MRC, "Init Succeed. initMRCSucceed : true.");
            }

            if (!initMRCSucceed || createMRCOverlaySucceed) return;

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

                    Texture texture = Texture2D.CreateExternalTexture((int)xmlCamData.imageW, (int)xmlCamData.imageH, TextureFormat.RGBA32, false, true, ptr);

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
                    RenderTextureDescriptor descriptor = new RenderTextureDescriptor((int)xmlCamData.imageW, (int)xmlCamData.imageH, RenderTextureFormat.ARGB32, 0);
                    descriptor.msaaSamples = 1;
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
            layerSubmit.header.layerFlags = (UInt32)PxrLayerSubmitFlagsEXT.PxrLayerFlagMRCComposition;
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
            if (!openMRC || !initMRCSucceed) return;

            if (!PXR_Plugin.System.UPxr_GetMRCEnable())
            {
                if (mrcCamObjActived)
                {
                    mrcCamObjActived = false;
                    backgroundCamObj.SetActive(false);
                    foregroundCamObj.SetActive(false);
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
            InitBackgroungCam(backgroundCamObj.GetComponent<Camera>());
            backgroundCamObj.SetActive(true);

            if (foregroundCamObj == null)
            {
                foregroundCamObj = new GameObject("myForegroundCamera");
                foregroundCamObj.transform.parent = Camera.main.transform.parent;
                foregroundCamObj.AddComponent<Camera>();
                PLog.i(TAG_MRC, "create foreground camera object.");
            }
            InitForegroundCam(foregroundCamObj.GetComponent<Camera>());
            foregroundCamObj.SetActive(true);

            mrcCamObjActived = true;

            PLog.i(TAG_MRC, "Camera Obj Actived. mrcCamObjActived : true.");
        }

        private void InitBackgroungCam(Camera camera)
        {
            camera.clearFlags = Camera.main.clearFlags;
            camera.backgroundColor = Camera.main.backgroundColor;
            if (camera.clearFlags == CameraClearFlags.SolidColor && camera.backgroundColor == new Color(0, 0, 0, 0)) // MR
            {
                camera.backgroundColor = foregroundColor;
            }
            camera.stereoTargetEye = StereoTargetEyeMask.None;
            camera.transform.localScale = Vector3.one;
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localEulerAngles = Vector3.zero;
            camera.depth = 9999;
            camera.gameObject.layer = 0;
            camera.orthographic = false;
            camera.fieldOfView = xmlCamData.yFov;
            camera.aspect = xmlCamData.imageW / xmlCamData.imageH;
            camera.allowMSAA = true;
            camera.cullingMask = backLayerMask;
            if (mrcBackgroundRT == null)
            {
                mrcBackgroundRT = new RenderTexture((int)xmlCamData.imageW, (int)xmlCamData.imageH, 24, RenderTextureFormat.ARGB32);
            }
            mrcBackgroundRT.name = "backgroundMrcRenderTexture";
            camera.targetTexture = mrcBackgroundRT;
            PLog.i(TAG_MRC, "init background camera.");
        }

        void InitForegroundCam(Camera camera)
        {
            camera.backgroundColor = foregroundColor;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.stereoTargetEye = StereoTargetEyeMask.None;
            camera.transform.localScale = Vector3.one;
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localEulerAngles = Vector3.zero;
            camera.depth = 10000;
            camera.gameObject.layer = 0;
            camera.orthographic = false;
            camera.fieldOfView = xmlCamData.yFov;
            camera.aspect = xmlCamData.imageW / xmlCamData.imageH;
            camera.allowMSAA = true;
            camera.cullingMask = foregroundLayerMask;
            if (mrcForegroundRT == null)
            {
                mrcForegroundRT = new RenderTexture((int)xmlCamData.imageW, (int)xmlCamData.imageH, 24, RenderTextureFormat.ARGB32);
            }
            mrcForegroundRT.name = "foregroundMrcRenderTexture";
            camera.targetTexture = mrcForegroundRT;
            PLog.i(TAG_MRC, "init foreground camera.");
        }

        public void CalibrationMRCCamCallback()
        {
            Invoke("Pxr_GetHeight", 0.05f);
            CalibrationMRCCam();
        }

        public void CalibrationMRCCam()
        {
            if (!PXR_Plugin.System.UPxr_GetMRCEnable() || null == backgroundCamObj || null == foregroundCamObj) return;

            PxrPosef pose = new PxrPosef();
            pose.orientation.x = 0;
            pose.orientation.y = 0;
            pose.orientation.z = 0;
            pose.orientation.w = 0;
            pose.position.x = 0;
            pose.position.y = 0;
            pose.position.z = 0;
            PXR_Plugin.System.UPxr_GetMrcPose(ref pose);
            backgroundCamObj.transform.localPosition = new Vector3(pose.position.x, pose.position.y + height, (-pose.position.z) * 1f);
            foregroundCamObj.transform.localPosition = new Vector3(pose.position.x, pose.position.y + height, (-pose.position.z) * 1f);
            Vector3 rototion = new Quaternion(pose.orientation.x, pose.orientation.y, pose.orientation.z, pose.orientation.w).eulerAngles;
            backgroundCamObj.transform.localEulerAngles = new Vector3(-rototion.x, -rototion.y, -rototion.z);
            foregroundCamObj.transform.localEulerAngles = new Vector3(-rototion.x, -rototion.y, -rototion.z);

            PLog.d(TAG_MRC, $"CalibrationMRCCam Camera.main.transform.localPosition.y={ Camera.main.transform.localPosition.y}");
        }

        public Vector3 UPxr_ToVector3(float[] translation)
        {
            Debug.Log("translation:" + new Vector3(translation[0], translation[1], -translation[2]).ToString());
            return new Vector3(translation[0], translation[1] + height, (-translation[2]) * 1f);
        }

        public Vector3 UPxr_ToRotation(float[] rotation)
        {
            Quaternion quaternion = new Quaternion(rotation[0], rotation[1], rotation[2], rotation[3]);
            Vector3 vector3 = quaternion.eulerAngles;
            Debug.Log("rotation:" + vector3.ToString());
            return new Vector3(-vector3.x, -vector3.y, -vector3.z);
        }

        public void Pxr_GetHeight()
        {
            float mrcY = PXR_Plugin.System.UPxr_GetMrcY();
            height = Camera.main.transform.localPosition.y - mrcY;
            PLog.i(TAG_MRC, $"Pxr_GetMrcY : {mrcY} , Camera.main.transform.localPosition.y={ Camera.main.transform.localPosition.y}");
        }

        #endregion
    }
    public struct CameraData
    {
        public string id;
        public string name;
        public float imageW;
        public float imageH;
        public float yFov;
        public PxrVector3f position;
        public PxrVector4f orientation;
    }
}