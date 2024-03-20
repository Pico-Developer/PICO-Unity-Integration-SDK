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

using System.ComponentModel;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Unity.XR.PXR
{
    //MR
    #region MR

    #region new mr
    public struct PxrAnchorEntityCreateInfo
    {
        public PxrTrackingOrigin origin;
        public PxrPosef pose;
        public double time;
    }

    public struct PxrAnchorEntityDestroyInfo
    {
        public ulong anchorHandle;
    }

    public struct PxrUuid
    {
        public ulong value0;
        public ulong value1;
    }

    public struct PxrAnchorComponentSceneLabelInfo
    {
        public PxrStructureType type;
        public PxrSceneLabel label;
    }

    public struct PxrAnchorComponentPlaneInfo
    {
        public PxrStructureType type;
        public PxrVector3f center;
        public PxrExtent2Df extent;
        public uint polygonSize;
        public IntPtr polygonVertices; //=>PxrVector3f[]
    }

    public struct PxrAnchorComponentVolumeInfo
    {
        public PxrStructureType type;
        public PxrVector3f center;
        public PxrVector3f extent;
    }

    public struct PxrExtent2Df
    {
        public float width;
        public float height;
    }

    public struct PxrAnchorPlaneBoundaryInfo
    {
        public PxrVector3f center;
        public PxrExtent2Df extent;
    }

    public struct PxrAnchorPlanePolygonInfo
    {
        public uint inputCount;
        public uint outputCount;
        public IntPtr vertices;
    }

    public struct PxrAnchorVolumeInfo
    {
        public PxrVector3f center;
        public PxrVector3f extent;
    }

    public struct PxrAnchorEntityList
    {
        public uint count;
        public IntPtr anchorHandles;//=>ulong[]
    }

    public struct PxrAnchorEntityPersistInfo
    {
        public PxrAnchorEntityList anchorList;
        public PxrPersistLocation location;
    }

    /// <summary>
    /// Information about the event of creating an anchor entity.
    /// </summary>
    public struct PxrEventAnchorEntityCreated
    {
        /// <summary>
        /// Task ID.
        /// </summary>
        public ulong taskId;
        /// <summary>
        /// Task result, which indicates whether the anchor entity is successfully created.
        /// </summary>
        public PxrResult result;
        /// <summary>
        /// The handle of the anchor entity.
        /// </summary>
        public ulong anchorHandle;
        /// <summary>
        /// The UUID of the anchor entity.
        /// </summary>
        public Guid uuid;
    }

    /// <summary>
    /// Information about the event of persisting an anchor entity.
    /// </summary>
    public struct PxrEventAnchorEntityPersisted
    {
        /// <summary>
        /// Task ID.
        /// </summary>
        public ulong taskId;
        /// <summary>
        /// Task result, which indicates whether the anchor entity is successfully persisted.
        /// </summary>
        public PxrResult result;
        /// <summary>
        /// The location where the anchor entity is saved. Currently, the anchor entity can only be saved to the device's local storage.
        /// </summary>
        public PxrPersistLocation location;
    }

    public struct PxrAnchorEntityUnPersistInfo
    {
        public PxrAnchorEntityList anchorList;
        public PxrPersistLocation location;
    }

    /// <summary>
    /// Information about the event of unpersisting an anchor entity.
    /// </summary>
    public struct PxrEventAnchorEntityUnPersisted
    {
        /// <summary>
        /// Task ID.
        /// </summary>
        public ulong taskId;
        /// <summary>
        /// Task result, which indicates whether the anchor entity is successfully unpersisted.
        /// </summary>
        public PxrResult result;
        /// <summary>
        /// The location from which the anchor entity is unpersisted. Currently, the anchor entity can only be unpersisted from the device's local storage.
        /// </summary>
        public PxrPersistLocation location;
    }

    public struct PxrAnchorEntityClearInfo
    {
        public PxrPersistLocation location;
    }

    /// <summary>
    /// Information about the event of clearing all anchor entities.
    /// </summary>
    public struct PxrEventAnchorEntityCleared
    {
        /// <summary>
        /// Task ID.
        /// </summary>
        public ulong taskId;
        /// <summary>
        /// Task result, which indicates whether the anchor entities are successfully cleared.
        /// </summary>
        public PxrResult result;
        /// <summary>
        /// The location of the anchor entities cleared.
        /// </summary>
        public PxrPersistLocation location;
    }

    public struct PxrAnchorEntityLoadInfo
    {
        public uint maxResult;
        public ulong timeout;
        public PxrPersistLocation location;
        public IntPtr include; //=>PxrAnchorEntityLoadFilterBaseHeader
        public IntPtr exclude; //=>PxrAnchorEntityLoadFilterBaseHeader
    }

    public struct PxrAnchorEntityLoadUuidFilter
    {
        public PxrStructureType type;
        public uint uuidCount;
        public IntPtr uuidList; //=>PxrUuid[]
    }

    public struct PxrAnchorEntityLoadComponentFilter
    {
        public PxrStructureType type;
        public ulong typeFlags;
    }
    public struct PxrAnchorEntityLoadSpatialSceneFilter
    {
        public PxrStructureType type;
        public ulong typeFlags;
    }

    public struct PxrAnchorEntityLoadResult
    {
        public ulong anchor;
        public PxrUuid uuid;
    }

    public struct PxrAnchorEntityLoadResults
    {
        public uint inputCount;
        public uint outputCount;
        public IntPtr loadResults; //=>PxrAnchorEntityLoadResult[]
    }

    /// <summary>
    /// Information about the event of loading anchor entities.
    /// </summary>
    public struct PxrEventAnchorEntityLoaded
    {
        /// <summary>
        /// Task ID.
        /// </summary>
        public ulong taskId;
        /// <summary>
        /// Task result, which indicates whether the anchor entities are successfully loaded.
        /// </summary>
        public PxrResult result;
        /// <summary>
        /// The number of anchor entities loaded.
        /// </summary>
        public uint count;
        /// <summary>
        /// The location from which the anchor entities are loaded.
        /// </summary>
        public PxrPersistLocation location;
    }

    /// <summary>
    /// Information about the event of room calibration.
    /// </summary>
    public struct PxrEventSpatialSceneCaptured
    {
        /// <summary>
        /// Task ID.
        /// </summary>
        public ulong taskId;
        /// <summary>
        /// Task result, which indicate whether the room is successfully calibrated.
        /// </summary>
        public PxrResult result;
        /// <summary>
        /// (not defined)
        /// </summary>
        public PxrSpatialSceneCaptureStatus status;
    }

    public struct PxrEventSpatialTrackingStateUpdate
    {
        public PxrSpatialTrackingState state;
        public PxrSpatialTrackingStateMessage message;
    }

    public enum PxrSpatialSceneCaptureStatus
    {
        NotDefined = 0,
    }

    /// <summary>
    /// The flags of components.
    /// </summary>
    public enum PxrAnchorComponentTypeFlags
    {
        Pose = 0x00000001,
        Persistence = 0x00000002,
        SceneLabel = 0x00000004,
        Plane = 0x00000008,
        Volume = 0x00000010
    }

    public enum PxrSpatialSceneDataTypeFlags
    {
        Unknown = 0x00000001,
        Floor = 0x00000002,
        Ceiling = 0x00000004,
        Wall = 0x00000008,
        Door = 0x00000010,
        Window = 0x00000020,
        Opening = 0x00000040,
        Object = 0x00000080
    }

    public enum PxrTrackingOrigin
    {
        Eye = 0,
        Floor = 1,
        Stage = 2
    }

    public enum PxrAnchorComponentType
    {
        Pose = 0,
        Persistence = 1,
        SceneLabel = 2,
        Plane = 3,
        Volume = 4,
    }

    public enum PxrSceneLabel
    {
        UnKnown = 0,
        Floor,
        Ceiling,
        Wall,
        Door,
        Window,
        Opening,
        Table,
        Sofa,
    }

    /// <summary>
    /// The location that an anchor entity is saved to.
    /// </summary>
    public enum PxrPersistLocation
    {
        /// <summary>
        /// The device's local storage.
        /// </summary>
        Local = 1,
        /// <summary>
        /// (Not supported yet)
        /// </summary>
        Remote = 2,
    }

    public enum PxrLayerEffect
    {
        Contrast = 0,
        Saturation = 1,
        Brightness = 2,
        Colortemp = 3,
    }

    #endregion

    //deprecate
    // Spatial Instance uuid 128 bit
    public struct PxrSpatialInstanceUuid
    {
        public UInt64 value0;
        public UInt64 value1;
    }

    // Create info struct used when creating a spatial anchor
    public struct PxrSpatialAnchorCreateInfo
    {
        public PxrReferenceType referenceType; // Global or Local
        public PxrPosef pose;                  // Position and Rotation
        public double time;                  // timestamp in milliseconds
    }

    // Save info struct used when saving a spatial anchor
    public struct PxrSpatialAnchorSaveInfo
    {
        public UInt64 anchorHandle;
        public PxrSpatialPersistenceLocation location;
        public PxrSpatialPersistenceMode persistenceMode;
    }

    // Delete info struct used when deleting a spatial anchor
    public struct PxrSpatialAnchorDeleteInfo
    {
        public UInt64 anchorHandle;
        public PxrSpatialPersistenceLocation persistenceLocation;
    }

    // Used to load all spatial instances that match the uuids provided
    // in the filter info. If numIds is set to 0, all Ids will be loaded.
    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSpatialInstanceIdFilter
    {
        public UInt32 numIds;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public PxrSpatialInstanceUuid[] uuids; 
    }

    // Load info strut used when loading spatial instances
    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSpatialInstanceLoadByIdInfo
    {
        public UInt32 maxNum;                                   // Max num of handles can be returned by this query
        public double timeout;                                  // milliseconds
        public PxrSpatialPersistenceLocation persistenceLocation; // Where to load
        public PxrSpatialInstanceIdFilter idFilter;        // What to load  
    }

    public enum PxrSpatialTrackingState
    {
        Invalid = 0,
        Valid = 1,
        Limited = 2,
    }

    public enum PxrSpatialTrackingStateMessage
    {
        Unknown = 0,
        Error = 1,

        Locating = 100,
        Located = 101,
        LocatingFailed = 102,
        LocatingFailedInvalidMap = 103,
        LocatingFailedNoMap = 104,
        LocateStopping = 105,
        LocateStopFailed = 106,
        LocateStopped = 107,
        MapCreating = 108,
        MapCreateFailed = 109,
        MapCreated = 110,
        MapSaving = 111,
        MapSaveFailed = 112,
        MapSaveFailedLowQuality = 113,
        MapSaveFailedInsufficentDiskSpace = 114,
        MapSaved = 115,
        MrEngineStarted = 116,
        MrEngineStopped = 117,
        MrEngineDestroyed = 118,
        MrMapLoss = 119,
    }

    public struct PxrSpatialTrackingStateInfo
    {
        public PxrSpatialTrackingState state;
        public PxrSpatialTrackingStateMessage message;
    }

    public struct PxrEventSpatialTrackingStateInfo
    {
        public PxrStructureType type;
        public PxrEventLevel eventLevel;
        public PxrSpatialTrackingStateInfo stateInfo;
    }

    public struct PxrEventSpatialAnchorSaveResult
    {
        public PxrStructureType type;
        public PxrEventLevel eventLevel;
        public PxrSpatialPersistenceResult result;
        public UInt64 asyncRequestId;
        public PxrSpatialInstanceUuid uuid;
        public UInt64 handle;
        public PxrSpatialPersistenceLocation location;
    }

    public struct PxrEventSpatialAnchorDeleteResult
    {
        public PxrStructureType type;
        public PxrEventLevel eventLevel;
        public PxrSpatialPersistenceResult result;
        public UInt64 asyncRequestId;
        public PxrSpatialInstanceUuid uuid;
        public PxrSpatialPersistenceLocation location;
    }

    public struct PxrSpatialAnchorLoadResult
    {
        public UInt64 anchorHandle;
        public PxrSpatialInstanceUuid uuid;
    }

    public struct PxrEventSpatialAnchorLoadResults
    {
        public PxrStructureType type;
        public PxrEventLevel eventLevel;
        public PxrSpatialPersistenceResult result;
        public bool hasNext;
        public UInt64 asyncRequestId;
        public UInt32 numResults;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public PxrSpatialAnchorLoadResult[] loadResults;
    }

    public struct PxrSpatialAnchorLoadResults
    {
        public Int32 resultCapacityInput;
        public Int32 resultCountOutput;
        public IntPtr results;  //PxrSpatialAnchorLoadResult
    }

    public struct PxrEventSpatialAnchorLoadResultsAvailable
    {
        public PxrStructureType type;
        public PxrEventLevel eventLevel;
        public UInt64 asyncRequestId;
    }

    public struct PxrEventSpatialAnchorLoadResultsComplete
    {
        public PxrStructureType type;
        public PxrEventLevel eventLevel;
        public UInt64 asyncRequestId;
        public PxrSpatialPersistenceResult result;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrEventDataBuffer
    {
        public PxrStructureType type;
        public PxrEventLevel eventLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 500)]
        public byte[] data;
    };

    public struct PxrRoomSceneDataSaveInfo
    {
        public ulong roomSceneDataHandle;
        public PxrSpatialPersistenceLocation location;
    }

    public struct PxrRoomSceneDataDeleteInfo
    {
        public ulong roomSceneDataHandle;
        public PxrSpatialPersistenceLocation location;
    }

    public struct PxrEventRoomSceneDataSaveResult
    {
        public PxrStructureType type;
        public PxrEventLevel eventLevel;
        public PxrSpatialPersistenceResult result;
        public ulong asyncRequestId;
        public ulong handle;
        public PxrSpatialPersistenceLocation location;
    }

    public struct PxrEventRoomSceneDataDeleteResult
    {
        public PxrStructureType type;
        public PxrEventLevel eventLevel;
        public PxrSpatialPersistenceResult result;
        public ulong asyncRequestId;
        public ulong handle;
        public PxrSpatialPersistenceLocation location;
    }

    public struct PxrRoomSceneLoadInfo
    {
        public double timeoutMs;                           // time in milliseconds
        public PxrSpatialPersistenceLocation location;
    }

    public struct PxrRoomSceneLoadResult
    {
        public ulong anchorHandle;
        public PxrSpatialInstanceUuid anchorUuid;
        public ulong roomSceneDataHandle;
        public int dataLen;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public byte[] data;
    }

    public struct PxrEventRoomSceneLoadResultsComplete
    {
        public PxrStructureType type;
        public PxrEventLevel eventLevel;
        public ulong asyncRequestId;
        public PxrSpatialPersistenceResult result;
    }

    public struct PxrRoomSceneLoadResults
    {
        public int resultCapacityInput;
        public int resultCountOutput;
        public IntPtr results; //PxrRoomSceneLoadResult
    }

    public struct PxrEventRoomSceneDataUpdateResult
    {
        public PxrStructureType type;
        public PxrEventLevel level;
        public PxrSpatialInstanceUuid anchorUuid;
        public ulong roomSceneDataHandle;
        public uint result;
        public uint dataLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] roomSceneData;
    }

    public struct PxrEventSemiAutoCandidatesUpdate
    {
        public uint state;
        public uint count;
    }

    public struct PxrSemiAutoRoomCaptureCandidatesUpdate
    {
        public PxrStructureType type;
        public PxrEventLevel eventLevel;
        public PxrSpatialPersistenceResult result;
    }

    public struct PxrPoint3D
    {
        public float x;
        public float y;
        public float z;
    }

    public struct PxrEventDataSeethroughStateChanged
    {
        public PxrStructureType type;
        public PxrEventLevel eventLevel;
        public int state;
    };



    public enum PxrStructureType
    {
        Unknown = 0,
        PXR_TYPE_EVENT_DATA_SEETHROUGH_STATE_CHANGED = 9,

        SpatialAnchorSaveResult = 20,
        SpatialAnchorDeleteResult = 21,
        SpatialAnchorLoadResults = 22,
        NewSpaceReady = 28,
        SpatialAnchorLoadResultsAvailable = 29,
        SpatialAnchorLoadResultsComplete = 30,
        RoomSceneDataSaveResult = 31,
        RoomSceneDataDeleteResult = 32,
        RoomSceneLoadResultsComplete = 33,
        SemiAutoRoomCaptureCandidatesUpdate = 36,
        RoomSceneDataUpdateResult = 37,
        TrackingStateChanged = 40,
        //mrsdk2.0
        MRMin = 100,
        SpatialTrackingStateUpdate = 101,
        AnchorEntityProperties = 102,
        AnchorEntityCreateInfo = 103,
        AnchorEntityDestroyInfo = 104,
        AnchorSpaceCreateInfo = 105,
        AnchorComponentSceneLabelInfo = 106,
        AnchorComponentPlaneInfo = 107,
        AnchorComponentVolumeInfo = 108,
        AnchorComponentAddInfo = 109,
        AnchorComponentRemoveInfo = 110,
        AnchorPlaneBoundaryInfo = 111,
        AnchorPlanePolygonInfo = 112,
        AnchorVolumeInfo = 113,
        AnchorEntityPersistInfo = 114,
        AnchorEntityUnPersistInfo = 115,
        AnchorEntityList = 116,
        AnchorEntityClearInfo = 117,
        AnchorEntityPersisted = 118,
        AnchorEntityUnPersisted = 119,
        AnchorEntityCleared = 120,
        AnchorEntityLoadInfo = 121,
        AnchorEntityLoadUuidFilter = 122,
        AnchorEntityLoadComponentFilter = 123,
        AnchorEntityLoaded = 124,
        AnchorEntityLoadResult = 125,
        SpatialSceneCaptureStartInfo = 126,
        SpatialSceneCaptured = 127,
        AnchorEntityLoadSpatialSceneFilter = 128,
        SemiAutoCandidatesUpdate = 129,
        AnchorEntityCreated = 130,
        MRMax = 200
    }

    
    /// <summary>
    /// The result of mixed reality-realted events.
    /// </summary>
    public enum PxrResult
    {
        SUCCESS = 0,
        TIMEOUT_EXPIRED = 1,
        SESSION_LOSS_PENDING = 3,
        EVENT_UNAVAILABLE = 4,
        SPACE_BOUNDS_UNAVAILABLE = 7,
        SESSION_NOT_FOCUSED = 8,
        FRAME_DISCARDED = 9,
        ERROR_VALIDATION_FAILURE = -1,
        ERROR_RUNTIME_FAILURE = -2,
        ERROR_OUT_OF_MEMORY = -3,
        ERROR_API_VERSION_UNSUPPORTED = -4,
        ERROR_INITIALIZATION_FAILED = -6,
        ERROR_FUNCTION_UNSUPPORTED = -7,
        ERROR_FEATURE_UNSUPPORTED = -8,
        ERROR_EXTENSION_NOT_PRESENT = -9,
        ERROR_LIMIT_REACHED = -10,
        ERROR_SIZE_INSUFFICIENT = -11,
        ERROR_HANDLE_INVALID = -12,

        ERROR_SPATIAL_LOCALIZATION_RUNNING = -1000,
        ERROR_SPATIAL_LOCALIZATION_NOT_RUNNING = -1001,
        ERROR_SPATIAL_MAP_CREATED = -1002,
        ERROR_SPATIAL_MAP_NOT_CREATED = -1003,
        ERROR_COMPONENT_NOT_SUPPORTED = -501,
        ERROR_COMPONENT_CONFLICT = -502,
        ERROR_COMPONENT_NOT_ADDED = -503,
        ERROR_COMPONENT_ADDED = -504,
        ERROR_ANCHOR_ENTITY_NOT_FOUND = -505
    }

    public enum PxrEventLevel
    {
        Low = 0,
        Mid,
        High
    }
    
    // The reference frame in which the pose is calculated,
    // Currently Local and Global are supported.
    public enum PxrReferenceType
    {
        NotDefined = 0,
        Local = 1,
        Global = 2
    }

    // Storage location to be used to store, load, erase, and query spatial instances from
    public enum PxrSpatialPersistenceLocation
    {
        NotDefined = 0,
        Local = 1, // local device storage
        Remote = 2, // remote storage
    }

    // Persistence mode, only one mode is supported and may be more mode in future.
    public enum PxrSpatialPersistenceMode
    {
        NotDefined = 0,
        Default = 1, // only this mode is supported now.
    }

    public enum PxrSpatialPersistenceResult
    {
        ErrorRuntimeFailure = -2,
        ErrorValidationFailure = -1,
        Success = 0,
        TimeoutExpired = 1,
    }

    #endregion

    [StructLayout(LayoutKind.Sequential)]
    public struct UserDefinedSettings
    {
        public ushort stereoRenderingMode;
        public ushort colorSpace;
        public ushort systemDisplayFrequency;
        public ushort useContentProtect;
        public ushort optimizeBufferDiscards;
        public ushort enableAppSpaceWarp;
        public ushort enableSubsampled;
        public ushort lateLatchingDebug;
        public ushort enableStageMode;
        public ushort enableSuperResolution;
        public ushort normalSharpening;
        public ushort qualitySharpening;
        public ushort fixedFoveatedSharpening;
        public ushort selfAdaptiveSharpening;
    }

    public enum RenderEvent
    {
        CreateTexture,
        DeleteTexture,
        UpdateTexture
    }

    public enum ResUtilsType
    {
        TypeTextSize,
        TypeColor,
        TypeText,
        TypeFont,
        TypeValue,
        TypeDrawable,
        TypeObject,
        TypeObjectArray,
    }

    public enum GraphicsAPI
    {
        OpenGLES,
        Vulkan
    };

    public enum EyeType
    {
        EyeLeft,
        EyeRight,
        EyeBoth
    };

    public enum ConfigType
    {
        RenderTextureWidth,
        RenderTextureHeight,
        ShowFps,
        RuntimeLogLevel,
        PluginLogLevel,
        UnityLogLevel,
        UnrealLogLevel,
        NativeLogLevel,
        TargetFrameRate,
        NeckModelX,
        NeckModelY,
        NeckModelZ,
        DisplayRefreshRate,
        Ability6Dof,
        DeviceModel,
        PhysicalIPD,
        ToDelaSensorY,
        SystemDisplayRate,
        FoveationSubsampledEnabled,
        TrackingOriginHeight,
        EngineVersion,
        UnrealOpenglNoError,
        EnableCPT,
        MRCTextureID,
        RenderFPS,
        AntiAliasingLevelRecommended,
        MRCTextureID2,
        PxrSetSurfaceView,
        PxrAPIVersion,
        PxrMrcPosiyionYOffset,
        PxrMrcTextureWidth,
        PxrMrcTextureHeight,
        PxrAndroidLayerDimensions = 34,
        PxrANDROID_SN,
        PxrSetDesiredFPS,
        PxrGetSeethroughState,
        PxrSetLayerBlend,
        PxrLeftEyeFOV,
        PxrRightEyeFOV,
        PxrBothEyeFOV,
        SupportQuickSeethrough,
        SetFilterType,
        SetSubmitLayerEXTItemColorMatrix,
    };

    public enum FoveatedRenderingMode
    {
        FixedFoveatedRendering = 0,
        EyeTrackedFoveatedRendering = 1
    }

    public enum FoveationLevel
    {
        None = -1,
        Low,
        Med,
        High,
        TopHigh
    }

    public enum BoundaryType
    {
        OuterBoundary,
        PlayArea
    }

    public enum BoundaryTrackingNode
    {
        HandLeft,
        HandRight,
        Head
    }

    public enum PxrTrackingState
    {
        LostNoReason,
        LostCamera,
        LostHighLight,
        LostLowLight,
        LostLowFeatureCount,
        LostReLocation,
        LostInitialization,
        LostNoCamera,
        LostNoIMU,
        LostIMUJitter,
        LostUnknown,
    }

    public enum ResetSensorOption
    {
        ResetPosition,
        ResetRotation,
        ResetRotationYOnly,
        ResetAll
    };

    public enum PxrLayerCreateFlags
    {
        PxrLayerFlagAndroidSurface = 1 << 0,
        PxrLayerFlagProtectedContent = 1 << 1,
        PxrLayerFlagStaticImage = 1 << 2,
        PxrLayerFlagUseExternalImages = 1 << 4,
        PxrLayerFlag3DLeftRightSurface = 1 << 5,
        PxrLayerFlag3DTopBottomSurface = 1 << 6,
        PxrLayerFlagEnableFrameExtrapolation = 1 << 7,
        PxrLayerFlagEnableSubsampled = 1 << 8,
        PxrLayerFlagEnableFrameExtrapolationPTW = 1 << 9,
        PxrLayerFlagSharedImagesBetweenLayers = 1 << 10,
    }

    public enum PxrLayerSubmitFlagsEXT
    {
        PxrLayerFlagMRCComposition = 1 << 30,
    }

    public enum PxrLayerSubmitFlags
    {
        PxrLayerFlagNoCompositionDepthTesting = 1 << 3,
        PxrLayerFlagUseExternalHeadPose = 1 << 5,
        PxrLayerFlagLayerPoseNotInTrackingSpace = 1 << 6,
        PxrLayerFlagHeadLocked = 1 << 7,
        PxrLayerFlagUseExternalImageIndex = 1 << 8,
        PxrLayerFlagPresentationProtection = 1 << 9,
        PxrLayerFlagSourceAlpha_1_0 = 1 << 10,
        PxrLayerFlagUseFrameExtrapolation = 1 << 11,
        PxrLayerFlagQuickSeethrough = 1 << 12,
        PxrLayerFlagEnableNormalSuperSampling = 1 << 13,
        PxrLayerFlagEnableQualitySuperSampling = 1 << 14,
        PxrLayerFlagEnableNormalSharpening = 1 << 15,
        PxrLayerFlagEnableQualitySharpening = 1 << 16,
        PxrLayerFlagEnableFixedFoveatedSuperSampling = 1 << 17,
        PxrLayerFlagEnableFixedFoveatedSharpening = 1 << 18,
        PxrLayerFlagEnableSelfAdaptiveSharpening = 1 << 19,
        PxrLayerFlagPremultipliedAlpha = 1 << 20,
        PxrLayerFlagColorSpaceHdrPQ = 1 << 22,
        PxrLayerFlagColorSpaceHdrHLG = 1 << 23
    }

    public enum PxrControllerKeyMap
    {
        PXR_CONTROLLER_KEY_HOME = 0,
        PXR_CONTROLLER_KEY_AX = 1,
        PXR_CONTROLLER_KEY_BY = 2,
        PXR_CONTROLLER_KEY_BACK = 3,
        PXR_CONTROLLER_KEY_TRIGGER = 4,
        PXR_CONTROLLER_KEY_VOL_UP = 5,
        PXR_CONTROLLER_KEY_VOL_DOWN = 6,
        PXR_CONTROLLER_KEY_ROCKER = 7,
        PXR_CONTROLLER_KEY_GRIP = 8,
        PXR_CONTROLLER_KEY_TOUCHPAD = 9,
        PXR_CONTROLLER_KEY_LASTONE = 127,

        PXR_CONTROLLER_TOUCH_AX = 128,
        PXR_CONTROLLER_TOUCH_BY = 129,
        PXR_CONTROLLER_TOUCH_ROCKER = 130,
        PXR_CONTROLLER_TOUCH_TRIGGER = 131,
        PXR_CONTROLLER_TOUCH_THUMB = 132,
        PXR_CONTROLLER_TOUCH_LASTONE = 255
    }

    public enum GetDataType
    {
        PXR_GET_FACE_DATA_DEFAULT = 0,
        PXR_GET_FACE_DATA = 3,
        PXR_GET_LIP_DATA = 4,
        PXR_GET_FACELIP_DATA = 5,
    }

    /// <summary>
    /// Body joint enumerations.
    /// * For leg tracking mode, joints numbered from 0 to 15 return data.
    /// * For full body tracking mode, all joints return data.
    /// </summary>
    public enum BodyTrackerRole
    {
        Pelvis = 0,
        LEFT_HIP = 1,
        RIGHT_HIP = 2,
        SPINE1 = 3,
        LEFT_KNEE = 4,
        RIGHT_KNEE = 5,
        SPINE2 = 6,
        LEFT_ANKLE = 7,
        RIGHT_ANKLE = 8,
        SPINE3 = 9,
        LEFT_FOOT = 10,
        RIGHT_FOOT = 11,
        NECK = 12,
        LEFT_COLLAR = 13,
        RIGHT_COLLAR = 14,
        HEAD = 15,
        LEFT_SHOULDER = 16,
        RIGHT_SHOULDER = 17,
        LEFT_ELBOW = 18,
        RIGHT_ELBOW = 19,
        LEFT_WRIST = 20,
        RIGHT_WRIST = 21,
        LEFT_HAND = 22,
        RIGHT_HAND = 23,
        NONE_ROLE = 24,                // unvalid
        MIN_ROLE = 0,                 // min value
        MAX_ROLE = 23,                // max value
        ROLE_NUM = 24,
    }

    public enum BodyActionList
    {
        PxrNoneAction = 0x00000000,
        PxrTouchGround = 0x00000001,
        PxrKeepStatic = 0x00000002,
    }

    /// <summary>
    /// Contains data about the position and rotation of a body joint.
    /// </summary>
    public struct BodyTrackerTransPose
    {
        /// <summary>
        /// IMU timestamp.
        /// </summary>
        public Int64 TimeStamp;
        /// <summary>
        /// The joint's position on the X axis.
        /// </summary>
        public double PosX;
        /// <summary>
        /// The joint's position on the Y axis.
        /// </summary>
        public double PosY;
        /// <summary>
        /// The joint's position on the Z axis.
        /// </summary>
        public double PosZ;
        /// <summary>
        /// The joint's rotation on the X component of the Quaternion.
        /// </summary>
        public double RotQx;
        /// <summary>
        /// The joint's rotation on the Y component of the Quaternion.
        /// </summary>
        public double RotQy;
        /// <summary>
        /// The joint's rotation on the Z component of the Quaternion.
        /// </summary>
        public double RotQz;
        /// <summary>
        /// The joint's rotation on the W component of the Quaternion.
        /// </summary>
        public double RotQw;
    }


    /// <summary>
    /// Contains data about the position, velocity, acceleration, and action of a body joint.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BodyTrackerTransform
    {
        /// <summary>
        /// Body joint name. If the value is `NONE_ROLE`, the joint's data will not be calculated.
        /// </summary>
        public BodyTrackerRole bone;
        /// <summary>
        /// The joint's position in the scene. Use `localpose` for your app.
        /// </summary>
        public BodyTrackerTransPose localpose;
        /// <summary>
        /// (do not use `globalpose`)
        /// </summary>
        public BodyTrackerTransPose globalpose;
        /// <summary>
        /// The joint's velocity on the X, Y, and Z axes.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] velo;
        /// <summary>
        /// The joint's acceleration on the X, Y, and Z axes.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] acce;
        /// <summary>
        /// The joint's angular velocity on the X, Y, and Z axes.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] wvelo;
        /// <summary>
        /// The joint's angular acceleration on the X, Y, and Z axes.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] wacce;
        /// <summary>
        /// Multiple actions can be supported at the same time by means of OR
        /// </summary>
        public UInt32 Action;
    }

    /// <summary>
    /// Contains data about the position, velocity, acceleration, and action of each body joint.
    /// </summary>
    public struct BodyTrackerResult
    {
        /// <summary>
        /// A fixed-length array, each position transmits the data of one body joint.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public BodyTrackerTransform[] trackingdata;
    }


    /// <summary>
    /// Information about PICO Motion Tracker's connection state.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct PxrFitnessBandConnectState
    {
        /// <summary>
        /// 
        /// </summary>
        public Byte num;
        /// <summary>
        /// 
        /// </summary>
        public fixed Byte trackerID[12];
    }

    public enum BodyTrackingAlgParamType
    {
        HUMAN_HEIGHT = 0,
        SWIFT_MODE = 1,
        BONE_PARAM = 2
    }
    public struct BodyTrackingAlgParam
    {
        public int BodyJointSet;
        public BodyTrackingBoneLength BoneLength;
    }

    /// <summary>
    /// The struct that defines the lengths (in centimeters) of different body parts of the avatar.
    /// </summary>
    public struct BodyTrackingBoneLength
    {
        /// <summary>
        /// The length of the head, which is from the top of the head to the upper area of the neck.
        /// </summary>
        public float headLen;
        /// <summary>
        /// The length of the neck, which is from the upper area of the neck to the lower area of the neck.
        /// </summary>
        public float neckLen;
        /// <summary>
        /// The length of the torso, which is from the lower area of the neck to the navel.
        /// </summary>
        public float torsoLen;
        /// <summary>
        /// The length of the hip, which is from the navel to the center of the upper area of the upper leg.
        /// </summary>
        public float hipLen;
        /// <summary>
        /// The length of the upper leg, which from the hip to the knee-joint.
        /// </summary>
        public float upperLegLen;
        /// <summary>
        /// The length of the lower leg, which is from the knee-joint to the ankle.
        /// </summary>
        public float lowerLegLen;
        /// <summary>
        /// The length of the foot, which is from the ankle to the tiptoe.
        /// </summary>
        public float footLen;
        /// <summary>
        /// The length of the shoulder, which is between the left and right shoulder joints.
        /// </summary>
        public float shoulderLen;
        /// <summary>
        /// The length of the upper arm, which is from the sholder joint to the elbow joint.
        /// </summary>
        public float upperArmLen;
        /// <summary>
        /// The length of the lower arm, which is from the elbow joint to the wrist.
        /// </summary>
        public float lowerArmLen;
        /// <summary>
        /// The length of the hand, which is from the wrist to the finger tip.
        /// </summary>
        public float handLen;
    }

    public enum AdaptiveResolutionPowerSetting
    {
        HIGH_QUALITY, // performance factor = 0.9
        BALANCED, // performance factor = 0.8
        BATTERY_SAVING // performance factor = 0.7
    }

    public struct FoveationParams
    {
        public float foveationGainX;
        public float foveationGainY;
        public float foveationArea;
        public float foveationMinimum;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EyeTrackingGazeRay
    {
        public Vector3 direction;
        public bool isValid;
        public Vector3 origin;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSensorState
    {
        public int status;
        public PxrPosef pose;
        public PxrVector3f angularVelocity;
        public PxrVector3f linearVelocity;
        public PxrVector3f angularAcceleration;
        public PxrVector3f linearAcceleration;
        public UInt64 poseTimeStampNs;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSensorState2
    {
        public int status;
        public PxrPosef pose;
        public PxrPosef globalPose;
        public PxrVector3f angularVelocity;
        public PxrVector3f linearVelocity;
        public PxrVector3f angularAcceleration;
        public PxrVector3f linearAcceleration;
        public UInt64 poseTimeStampNs;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrControllerTracking
    {
        public PxrSensorState localControllerPose;
        public PxrSensorState globalControllerPose;
    }

    public enum PxrControllerType
    {
        PxrInputG2 = 3,
        PxrInputNeo2 = 4,
        PxrInputNeo3 = 5,
        PxrInputPICO_4 = 6
    }

    public enum PxrControllerDof
    {
        PxrController3Dof,
        PxrController6Dof
    }

    public enum PxrControllerBond
    {
        PxrControllerIsBond,
        PxrControllerUnBond
    }

    public enum PxrBlendFactor
    {
        PxrBlendFactorZero = 0,
        PxrBlendFactorOne = 1,
        PxrBlendFactorSrcAlpha = 2,
        PxrBlendFactorOneMinusSrcAlpha = 3,
        PxrBlendFactorDstAlpha = 4,
        PxrBlendFactorOneMinusDstAlpha = 5
    };

    public enum PxrDeviceAbilities
    {
        PxrTrackingModeRotationBit,
        PxrTrackingModePositionBit,
        PxrTrackingModeEyeBit,
        PxrTrackingModeFaceBit,
        PxrTrackingModeBroadBandMontorBit,
        PxrTrackingModeHandBit
    }

    public enum FaceTrackingMode {
        None,
        Hybrid,
        FaceOnly,
        LipsyncOnly
    }

    public enum SkipInitSettingFlag {
        SkipHandleConnectionTeaching = 1,
        SkipTriggerKeyTeaching       = 1 << 1,
        SkipLanguage                 = 1 << 2,
        SkipCountry                  = 1 << 3,
        SkipWIFI                     = 1 << 4,
        SkipQuickSetting             = 1 << 5
    }
    
    public enum PxrPerfSettings {
        CPU = 1,
        GPU = 2,
    }
    
    public enum PxrSettingsLevel {
        POWER_SAVINGS = 0,
        SUSTAINED_LOW = 1,
        SUSTAINED_HIGH = 3,
        BOOST = 5,
    }

     public enum PxrFtLipsyncValue
    {
        STOP_FT,
        STOP_LIPSYNC,
        START_FT,
        START_LIPSYNC,
    }

     public enum PxrGazeType
    {
        Never,
        DuringMotion,
        Always
    }

    public enum PxrArmModelType
    {
        Controller,
        Wrist,
        Elbow,
        Shoulder
    }
    public enum SharpeningMode
    {
        None,
        Normal,
        Quality
    }
    public enum SharpeningEnhance
    {
        None,
        FixedFoveated,
        SelfAdaptive,
        Both
    }
 

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrControllerCapability
    {
        public PxrControllerType type;
        public PxrControllerDof inputDof;
        public PxrControllerBond inputBond;
        public UInt64 Abilities;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrLayerParam
    {
        public int layerId;
        public PXR_OverLay.OverlayShape layerShape;
        public PXR_OverLay.OverlayType layerType;
        public PXR_OverLay.LayerLayout layerLayout;
        public UInt64 format;
        public UInt32 width;
        public UInt32 height;
        public UInt32 sampleCount;
        public UInt32 faceCount;
        public UInt32 arraySize;
        public UInt32 mipmapCount;
        public UInt32 layerFlags;
        public UInt32 externalImageCount;
        public IntPtr leftExternalImages;
        public IntPtr rightExternalImages;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrVector4f
    {
        public float x;
        public float y;
        public float z;
        public float w;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrVector3f
    {
        public float x;
        public float y;
        public float z;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrVector2f
    {
        public float x;
        public float y;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrBoundaryTriggerInfo
    {
        public bool isTriggering;
        public float closestDistance;
        public PxrVector3f closestPoint;
        public PxrVector3f closestPointNormal;
        public bool valid;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrPosef
    {
        public PxrVector4f orientation;
        public PxrVector3f position;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrRecti
    {
        public int x;
        public int y;
        public int width;
        public int height;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrLayerBlend
    {
        public PxrBlendFactor srcColor;
        public PxrBlendFactor dstColor;
        public PxrBlendFactor srcAlpha;
        public PxrBlendFactor dstAlpha;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrLayerHeader
    {
        public int layerId;
        public UInt32 layerFlags;
        public float colorScaleX;
        public float colorScaleY;
        public float colorScaleZ;
        public float colorScaleW;
        public float colorBiasX;
        public float colorBiasY;
        public float colorBiasZ;
        public float colorBiasW;
        public int compositionDepth;
        public int sensorFrameIndex;
        public int imageIndex;
        public PxrPosef headPose;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrLayerHeader2
    {
        public int layerId;
        public UInt32 layerFlags;
        public float colorScaleX;
        public float colorScaleY;
        public float colorScaleZ;
        public float colorScaleW;
        public float colorBiasX;
        public float colorBiasY;
        public float colorBiasZ;
        public float colorBiasW;
        public int compositionDepth;
        public int sensorFrameIndex;
        public int imageIndex;
        public PxrPosef headPose;
        public PXR_OverLay.OverlayShape layerShape;
        public UInt32 useLayerBlend;
        public PxrLayerBlend layerBlend;
        public UInt32 useImageRect;
        public PxrRecti imageRectLeft;
        public PxrRecti imageRectRight;
        public UInt64 reserved0;
        public UInt64 reserved1;
        public UInt64 reserved2;
        public UInt64 reserved3;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrLayerQuad
    {
        public PxrLayerHeader header;
        public PxrPosef pose;
        public float width;
        public float height;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrLayerQuad2
    {
        public PxrLayerHeader2 header;
        public PxrPosef poseLeft;
        public PxrPosef poseRight;
        public PxrVector2f sizeLeft;
        public PxrVector2f sizeRight;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrLayerCylinder
    {
        public PxrLayerHeader header;
        public PxrPosef pose;
        public float radius;
        public float centralAngle;
        public float height;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrLayerCylinder2
    {
        public PxrLayerHeader2 header;
        public PxrPosef poseLeft;
        public PxrPosef poseRight;
        public float radiusLeft;
        public float radiusRight;
        public float centralAngleLeft;
        public float centralAngleRight;
        public float heightLeft;
        public float heightRight;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrLayerEquirect
    {
        public PxrLayerHeader2 header;
        public PxrPosef poseLeft;
        public PxrPosef poseRight;
        public float radiusLeft;
        public float radiusRight;
        public float scaleXLeft;
        public float scaleXRight;
        public float scaleYLeft;
        public float scaleYRight;
        public float biasXLeft;
        public float biasXRight;
        public float biasYLeft;
        public float biasYRight;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrLayerEquirect2
    {
        public PxrLayerHeader2 header;
        public PxrPosef poseLeft;
        public PxrPosef poseRight;
        public float radiusLeft;
        public float radiusRight;
        public float centralHorizontalAngleLeft;
        public float centralHorizontalAngleRight;
        public float upperVerticalAngleLeft;
        public float upperVerticalAngleRight;
        public float lowerVerticalAngleLeft;
        public float lowerVerticalAngleRight;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrLayerCube2
    {
        public PxrLayerHeader2 header;
        public PxrPosef poseLeft;
        public PxrPosef poseRight;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrLayerEac2
    {
        public PxrLayerHeader2 header;
        public PxrPosef poseLeft;
        public PxrPosef poseRight;
        public PxrVector3f offsetPosLeft;
        public PxrVector3f offsetPosRight;
        public PxrVector4f offsetRotLeft;
        public PxrVector4f offsetRotRight;
        public UInt32 degreeType;
        public float overlapFactor;
        public UInt64 timestamp;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct AudioClipData
    {
        public int slot;//手柄
        public UInt64 buffersize;//数据大小
        public int sampleRate;// 采样率
        public int channelCounts;//通道数
        public int bitrate;//bit率
        public int reversal;//反转
        public int isCache;//是否缓存
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct VibrateInfo {
        public uint slot;
        public uint reversal;
        public float amp;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrPhfParams {
        public UInt64 frameseq;
        public UInt16 play;
        public UInt16 frequency;
        public UInt16 loop;
        public float gain;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrPhfFile
    {
        public string phfVersion;
        public int frameDuration;
        public PxrPhfParams[] patternData_L;
        public PxrPhfParams[] patternData_R;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrPhfParamsNum {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public PxrPhfParams[] phfParams;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct PxrFaceTrackingInfo
    {
        public Int64 timestamp;                         // us
        public fixed float blendShapeWeight[72];                //72（52+20）Expression component weight
        public fixed float videoInputValid[10];                 // Input validity of upper and lower face
        public float laughingProb;                      // Coefficient of laughter
        public fixed float emotionProb[10];                     // Emotional factor
        public fixed float reserved[128];
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrExtent2Di
    {
        public int width;
        public int height;
    };

    public static class PXR_Plugin
    {
        private const string PXR_SDK_Version = "2.5.3";
        private const string PXR_PLATFORM_DLL = "PxrPlatform";
        public const string PXR_API_DLL = "pxr_api";
        private static int PXR_API_Version = 0;

        #region DLLImports
        //MR
        //new
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_EnablePassthroughStyle(bool value);
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetPassthroughStyle(PxrLayerEffect type, float value, float duration);
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetPassthroughLUT(ref byte[] data, int width, int height, int row, int col);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_CreateAnchorEntity(ref PxrAnchorEntityCreateInfo info, out ulong anchorHandle);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_DestroyAnchorEntity(ref PxrAnchorEntityDestroyInfo info);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_GetAnchorPose(ulong anchorHandle, PxrTrackingOrigin origin, out PxrPosef pose);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_GetAnchorEntityUuid(ulong anchorHandle, out PxrUuid uuid);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_GetAnchorComponentFlags(ulong anchorHandle,
            out ulong flag);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_GetAnchorSceneLabel(ulong anchorHandle, out PxrSceneLabel label);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_GetAnchorPlaneBoundaryInfo(ulong anchorHandle,
            ref PxrAnchorPlaneBoundaryInfo info);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_GetAnchorPlanePolygonInfo(ulong anchorHandle,
            ref PxrAnchorPlanePolygonInfo info);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_GetAnchorBoxInfo(ulong anchorHandle, ref PxrAnchorVolumeInfo info);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_PersistAnchorEntity(ref PxrAnchorEntityPersistInfo info,
            out ulong taskId);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_UnpersistAnchorEntity(ref PxrAnchorEntityUnPersistInfo info,
            out ulong taskId);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_ClearPersistedAnchorEntity(ref PxrAnchorEntityClearInfo info,
            out ulong taskId);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_LoadAnchorEntity(ref PxrAnchorEntityLoadInfo info, out ulong taskId);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_GetAnchorEntityLoadResults(ulong taskId, ref PxrAnchorEntityLoadResults results);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_StartSpatialSceneCapture(out ulong taskId);

        //old
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StartHumanOcclusion();
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_AcquireMeshingInfo(IntPtr maskBuffer,IntPtr verticesBuffer, out uint verticesSize, IntPtr facetsBuffer, out uint facetSize, out bool haveRead);
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StopHumanOcclusion();
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSpatialAnchor(ref PxrSpatialAnchorCreateInfo info, ref UInt64 handle);
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_DestroySpatialAnchor(UInt64 handle);
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SaveSpatialAnchor(ref PxrSpatialAnchorSaveInfo info, ref UInt64 requestId);
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_DeleteSpatialAnchor(ref PxrSpatialAnchorDeleteInfo info, ref UInt64 requestId);
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_LoadSpatialAnchorById(ref PxrSpatialInstanceLoadByIdInfo info, ref UInt64 requestId);
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetSpatialAnchorLoadResults(UInt64 requestId,ref PxrSpatialAnchorLoadResults loadResults);
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetSpatialAnchorPose(UInt64 handle, double predictDisplayTime, PxrReferenceType type, ref PxrPosef pose);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetSpatialAnchorUuid(UInt64 handle, ref PxrSpatialInstanceUuid uuid);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Pxr_PollEventFromXRPlugin(ref int eventNum, IntPtr[] eventData);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateRoomSceneData(PxrSpatialInstanceUuid anchorUuid, IntPtr roomSceneData, int dataLen, ref ulong roomSceneDataHandle);
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_DestroyRoomSceneData(ulong roomSceneDataHandle);
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SaveRoomSceneData(ref PxrRoomSceneDataSaveInfo saveInfo, ref ulong requestId);
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_DeleteRoomSceneData(ref PxrRoomSceneDataDeleteInfo deleteInfo, ref ulong requestId);
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_LoadRoomScene(ref PxrRoomSceneLoadInfo loadInfo, ref ulong requestId);
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetRoomSceneLoadResults(ulong requestId, ref PxrRoomSceneLoadResults results);
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StartRoomCapture();
        
        //PassThrough
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_CameraStart();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_CameraStop();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_CameraDestroy();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Pxr_CameraGetRenderEventFunc();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_CameraSetRenderEventPending();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_CameraWaitForRenderEvent();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_CameraUpdateFrame(int eye);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_CameraCreateTexturesMainThread();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_CameraDeleteTexturesMainThread();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_CameraUpdateTexturesMainThread();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pxr_SetFoveationLevelEnable(int enable);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pxr_SetEyeFoveationLevelEnable(int enable);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_SetFFRSubsampled(bool enable);

        //System
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pxr_LoadPlugin();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_UnloadPlugin();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pxr_GetHomeKey();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_InitHomeKey();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pxr_GetMRCEnable();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_SetUserDefinedSettings(UserDefinedSettings settings);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_Construct(PXR_Loader.ConvertRotationWith2VectorDelegate fromToRotation);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern float Pxr_RefreshRateChanged();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pxr_GetFocusState();
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pxr_IsSensorReady();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetSensorStatus();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_GetLayerImagePtr(int layerId, EyeType eye, int imageIndex, ref IntPtr image);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_CreateLayerParam(PxrLayerParam layerParam);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_DestroyLayerByRender(int layerId);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_EnableEyeTracking(bool enable);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_EnableFaceTracking(bool enable);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_EnableLipsync(bool enable);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_SetInputDeviceChangedCallBack(InputDeviceChangedCallBack callback);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_SetSeethroughStateChangedCallBack(SeethroughStateChangedCallBack callback);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_SetFitnessBandNumberOfConnectionsCallBack(FitnessBandNumberOfConnectionsCallBack callback);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_SetFitnessBandElectricQuantityCallBack(FitnessBandElectricQuantityCallBack callback);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_SetFitnessBandAbnormalCalibrationDataCallBack(FitnessBandAbnormalCalibrationDataCallBack callback);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_SetLoglevelChangedCallBack(LoglevelChangedCallBack callback);
        
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_EnablePremultipliedAlpha(bool enable);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_SetGraphicOption(GraphicsAPI option);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateLayer(IntPtr layerParam);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetLayerNextImageIndex(int layerId, ref int imageIndex);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetLayerImageCount(int layerId, EyeType eye, ref UInt32 imageCount);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetLayerImage(int layerId, EyeType eye, int imageIndex, ref UInt64 image);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetConfigIntArray(ConfigType configIndex, int[] configSetData, int dataCount);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetConfigFloatArray(ConfigType configIndex, float[] configSetData, int dataCount);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetLayerAndroidSurface(int layerId, EyeType eye, ref IntPtr androidSurface);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_DestroyLayer(int layerId);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayer(IntPtr layer);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerQuad(PxrLayerQuad layer);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerQuad2(PxrLayerQuad2 layer);


        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetLayerNextImageIndexByRender(int layerId, ref int imageIndex);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerQuadByRender(IntPtr ptr);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerQuad2ByRender(IntPtr ptr);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerCylinderByRender(IntPtr ptr);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerCylinder2ByRender(IntPtr ptr);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerEquirectByRender(IntPtr ptr);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerEquirect2ByRender(IntPtr ptr);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerCube2ByRender(IntPtr ptr);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerEac2ByRender(IntPtr ptr);


        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerCylinder(PxrLayerCylinder layer);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerCylinder2(PxrLayerCylinder2 layer);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerEquirect(PxrLayerEquirect layer);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerEquirect2(PxrLayerEquirect2 layer);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerCube2(PxrLayerCube2 layer);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerEac2(PxrLayerEac2 layer);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_SetLayerBlend(bool enable, PxrLayerBlend layerBlend);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern FoveationLevel Pxr_GetFoveationLevel();

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetFoveationParams(FoveationParams foveationParams);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetFrustum(EyeType eye, float fovLeft, float fovRight, float fovUp, float fovDown, float near, float far);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetFrustum(EyeType eye, ref float fovLeft, ref float fovRight, ref float fovUp, ref float fovDown, ref float near, ref float far);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetConfigFloat(ConfigType configIndex, ref float value);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetConfigInt(ConfigType configIndex, ref int value);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetConfigInt(ConfigType configSetIndex, int configSetData);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetConfigString(ConfigType configSetIndex, string configSetData);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetConfigUint64(ConfigType configSetIndex, UInt64 configSetData);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_ResetSensor(ResetSensorOption option);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetSensorLostCustomMode(bool value);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetSensorLostCMST(bool value);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pxr_GetDisplayRefreshRatesAvailable(ref int configCount, ref IntPtr configArray);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetDisplayRefreshRate(float refreshRate);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetPredictedDisplayTime(ref double predictedDisplayTime);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pxr_SetExtraLatencyMode(int mode);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetTrackingMode(ref UInt64 trackingModeFlags);
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        
        public static extern int Pxr_SetTrackingOrigin(PxrTrackingOrigin mode);
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetTrackingOrigin(ref PxrTrackingOrigin mode);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_UpdateContentProtectState(int state);

        //Tracking Sensor
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetPredictedMainSensorState2(double predictTimeMs, ref PxrSensorState2 sensorState, ref int sensorFrameIndex);

        //Controller
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_SetControllerOriginOffset(int controllerID, Vector3 offset);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetControllerTrackingState(UInt32 deviceID, double predictTime, float[] headSensorData, ref PxrControllerTracking tracking);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetControllerMainInputHandle(UInt32 deviceID);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetControllerMainInputHandle(ref int deviceID);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetControllerVibration(UInt32 deviceID, float strength, int time);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetControllerEnableKey(bool isEnable, PxrControllerKeyMap Key);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_ResetController(UInt32 deviceID);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetArmModelParameters(PxrGazeType gazetype, PxrArmModelType armmodeltype, float elbowHeight, float elbowDepth, float pointerTiltAngle);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetControllerHandness(ref int handness);

        //Vibration

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetControllerVibrationEvent(UInt32 deviceID, int frequency, float strength, int time);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetControllerCapabilities(UInt32 deviceID, ref PxrControllerCapability capability);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StopControllerVCMotor(int clientId);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StartControllerVCMotor(string file, int slot);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetControllerAmp(float mode);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetControllerDelay(int delay);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern string Pxr_GetVibrateDelayTime(ref int length);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StartVibrateBySharemF(float[] data, ref AudioClipData parameter, ref int source_id);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StartVibrateByCache(int clicpid);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_ClearVibrateByCache(int clicpid);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StartVibrateByPHF(string data, int buffersize, ref int sourceID, ref VibrateInfo vibrateInfo);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_PauseVibrate(int sourceID);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_ResumeVibrate(int sourceID);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_UpdateVibrateParams(int clicp_id, ref VibrateInfo vibrateInfo);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateHapticStream(string phfVersion, UInt32 frameDurationMs, ref VibrateInfo hapticInfo, float speed, ref int id);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_WriteHapticStream(int id, ref PxrPhfParamsNum frames, UInt32 numFrames);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetPHFHapticSpeed(int id, float speed);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetPHFHapticSpeed(int id, ref float speed);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetCurrentFrameSequence(int id, ref UInt64 frameSequence);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StartPHFHaptic(int source_id);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StopPHFHaptic(int source_id);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_RemovePHFHaptic(int source_id);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_SetLogInfoActive(bool value);



        //Boundary
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetSeeThroughState();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_SetVideoSeethroughState(bool value);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_TestNodeIsInBoundary(BoundaryTrackingNode node, bool isPlayArea, ref PxrBoundaryTriggerInfo info);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_TestPointIsInBoundary(ref PxrVector3f point, bool isPlayArea, ref PxrBoundaryTriggerInfo info);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetBoundaryGeometry(bool isPlayArea, UInt32 pointsCountInput, ref UInt32 pointsCountOutput, PxrVector3f[] outPoints);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetBoundaryDimensions(bool isPlayArea, out PxrVector3f dimension);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pxr_GetBoundaryConfigured();

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pxr_GetBoundaryEnabled();

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetBoundaryVisible(bool value);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetSeeThroughBackground(bool value);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pxr_GetBoundaryVisible();
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_ResetSensorHard();
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetTrackingState();

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetGuardianSystemDisable(bool disable);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_ResumeGuardianSystemForSTS();

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_PauseGuardianSystemForSTS();

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_ShutdownSdkGuardianSystem();

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetRoomModeState();

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_DisableBoundary();

        //MRC
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pxr_GetMrcStatus();

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetMrcPose(ref PxrPosef pose);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetMrcPose(ref PxrPosef pose);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetIsSupportMovingMrc(bool support);

        //Face tracking
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetTrackingMode(double trackingMode);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetFaceTrackingData(Int64 ts, int flags, ref PxrFaceTrackingInfo faceTrackingInfo);
        
        //Application SpaceWarp
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_SetSpaceWarp(int value);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_SetAppSpacePosition(float x, float y, float z);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_SetAppSpaceRotation(float x, float y, float z, float w);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetTrackingStatus(String key, String value);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetPerformanceLevels(PxrPerfSettings which, PxrSettingsLevel level);
        
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetPerformanceLevels(PxrPerfSettings which, ref PxrSettingsLevel level);
      
        //Body tracking
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetBodyTrackingPose(double predictTime, ref BodyTrackerResult bodyTrackerResult);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetBodyTrackingMode(int mode);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetFitnessBandConnectState(ref PxrFitnessBandConnectState state);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetFitnessBandBattery(int trackerId, ref int battery);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetFitnessBandCalibState(ref int calibrated);
              
        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_LogSdkApi(string sdkInfo);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetBodyTrackingAlgParam(BodyTrackingAlgParamType AlgParamType, ref BodyTrackingAlgParam Param);

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_UpdateAdaptiveResolution(ref PxrExtent2Di dimensions, AdaptiveResolutionPowerSetting powerSetting);

        #endregion

        public static class System
        {
            public static Action RecenterSuccess;
            public static Action FocusStateAcquired;
            public static Action FocusStateLost;
            public static Action SensorReady;
            public static Action<int> InputDeviceChanged;
            public static Action<int> LoglevelChangedChanged;
            public static Action<int> SeethroughStateChangedChanged;
            public static Action<int, int> FitnessBandNumberOfConnections;
            public static Action<int, int> FitnessBandElectricQuantity;
            public static Action<int, int> FitnessBandAbnormalCalibrationData;

            public static void UPxr_SetTrackingOrigin(PxrTrackingOrigin mode)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_SetTrackingOrigin(mode);
#endif
            }

            public static void UPxr_GetTrackingOrigin(ref PxrTrackingOrigin mode)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_GetTrackingOrigin(ref mode);
#endif
            }

            public static bool UPxr_LoadPICOPlugin()
            {
                PLog.d(TAG, "UPxr_Load PICO Plugin");
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_LoadPlugin();
#else  
                return false;
#endif
            }

            public static void UPxr_UnloadPICOPlugin()
            {
                PLog.d(TAG, "UPxr_Unload PICO Plugin");
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_UnloadPlugin();
#endif
            }

            public static bool UPxr_QueryDeviceAbilities(PxrDeviceAbilities abilities)
            {
                UInt64 flags = UInt64.MinValue;
#if UNITY_ANDROID && !UNITY_EDITOR
                if (UPxr_GetAPIVersion() >= 0x2000304)
                {
                    Pxr_GetTrackingMode(ref flags);
                }
#endif
                switch (abilities)
                {
                    case PxrDeviceAbilities.PxrTrackingModeRotationBit:
                        {
                            return Convert.ToBoolean(flags & 0x00000001);
                        }
                    case PxrDeviceAbilities.PxrTrackingModePositionBit:
                        {
                            return Convert.ToBoolean(flags & 0x00000002);
                        }
                    case PxrDeviceAbilities.PxrTrackingModeEyeBit:
                        {
                            return Convert.ToBoolean(flags & 0x00000004);
                        }
                    case PxrDeviceAbilities.PxrTrackingModeFaceBit:
                        {
                            return Convert.ToBoolean(flags & 0x00000008);
                        }
                    case PxrDeviceAbilities.PxrTrackingModeBroadBandMontorBit:
                        {
                            return Convert.ToBoolean(flags & 0x00000010);
                        }
                    case PxrDeviceAbilities.PxrTrackingModeHandBit:
                        {
                            return Convert.ToBoolean(flags & 0x00000020);
                        }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(abilities), abilities, null);
                }
            }

            public static void UPxr_InitializeFocusCallback()
            {
                Application.onBeforeRender += UPxr_FocusUpdate;
                Application.onBeforeRender += UPxr_SensorReadyStateUpdate;
            }

            public static void UPxr_DeinitializeFocusCallback()
            {
                Application.onBeforeRender -= UPxr_FocusUpdate;
                Application.onBeforeRender -= UPxr_SensorReadyStateUpdate;
            }

            public static void UPxr_SetInputDeviceChangedCallBack(InputDeviceChangedCallBack callback)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                Pxr_SetInputDeviceChangedCallBack(callback);
#endif
            }

            public static void UPxr_SetSeethroughStateChangedCallBack(SeethroughStateChangedCallBack callback)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                Pxr_SetSeethroughStateChangedCallBack(callback);
#endif
            }

            public static void UPxr_SetFitnessBandNumberOfConnectionsCallBack(FitnessBandNumberOfConnectionsCallBack callback) {
#if !UNITY_EDITOR && UNITY_ANDROID
                Pxr_SetFitnessBandNumberOfConnectionsCallBack(callback);
#endif
            }

            public static void UPxr_SetFitnessBandAbnormalCalibrationDataCallBack(FitnessBandAbnormalCalibrationDataCallBack callback)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                Pxr_SetFitnessBandAbnormalCalibrationDataCallBack(callback);
#endif
            }

            public static void UPxr_SetFitnessBandElectricQuantityCallBack(FitnessBandElectricQuantityCallBack callback)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                Pxr_SetFitnessBandElectricQuantityCallBack(callback);
#endif
            }

            public static void UPxr_SetLoglevelChangedCallBack(LoglevelChangedCallBack callback)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                Pxr_SetLoglevelChangedCallBack(callback);
#endif
            }

            public static bool UPxr_GetFocusState()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetFocusState();
#else
                return false;
#endif
            }

            public static bool UPxr_IsSensorReady()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_IsSensorReady();
#else
                return false;
#endif
            }

            private static bool lastAppFocusState = false;
            private static void UPxr_FocusUpdate()
            {
                bool appfocus = UPxr_GetFocusState();
                if (appfocus && !lastAppFocusState)
                {
                    if (FocusStateAcquired != null)
                    {
                        FocusStateAcquired();
                    }
                }

                if (!appfocus && lastAppFocusState)
                {
                    if (FocusStateLost != null)
                    {
                        FocusStateLost();
                    }
                }

                lastAppFocusState = appfocus;
            }

            private static bool lastSensorReadyState = false;
            private static void UPxr_SensorReadyStateUpdate()
            {
                bool sensorReady = UPxr_IsSensorReady();
                if (sensorReady && !lastSensorReadyState)
                {
                    if (SensorReady != null)
                    {
                        SensorReady();
                    }
                }

                lastSensorReadyState = sensorReady;
            }

            public static string UPxr_GetSDKVersion()
            {
                return PXR_SDK_Version;
            }

            public static int UPxr_LogSdkApi(string sdkInfo)
            {
                PLog.d(TAG, "UPxr_LogSdkApi() sdkInfo:" + sdkInfo);
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_LogSdkApi(sdkInfo);
#endif
                PLog.d(TAG, "UPxr_LogSdkApi() result:" + result);
                return result;
            }

            public static float UPxr_GetSystemDisplayFrequency()
            {
                return UPxr_GetConfigFloat(ConfigType.SystemDisplayRate);
            }

            public static float UPxr_GetMrcY()
            {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000300)
                {
                    return UPxr_GetConfigFloat(ConfigType.PxrMrcPosiyionYOffset);
                }
                else
                {
                    return 0;
                }
            }

            public static double UPxr_GetPredictedDisplayTime()
            {
                PLog.d(TAG, "UPxr_GetPredictedDisplayTime()");
                double predictedDisplayTime = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_GetPredictedDisplayTime(ref predictedDisplayTime);
#endif
                PLog.d(TAG, "UPxr_GetPredictedDisplayTime() predictedDisplayTime：" + predictedDisplayTime);
                return predictedDisplayTime;
            }

            public static bool UPxr_SetExtraLatencyMode(int mode)
            {
                PLog.d(TAG, "UPxr_SetExtraLatencyMode() mode:" + mode);
                bool result = false;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SetExtraLatencyMode(mode);
#endif
                PLog.d(TAG, "UPxr_SetExtraLatencyMode() result:" + result);
                return result;
            }

            public static int UPxr_UpdateAdaptiveResolution(ref int width, AdaptiveResolutionPowerSetting powerSetting)
            {
                int success = 1;
                PxrExtent2Di dim;

                dim.width = width;
                dim.height = width;
#if !UNITY_EDITOR && UNITY_ANDROID

                success = Pxr_UpdateAdaptiveResolution(ref dim, powerSetting);
                width = dim.width;
                PLog.i(TAG, "UPxr_UpdateAdaptiveResolution ：" + width);
#endif
                return success;
            }

            public static void UPxr_SetUserDefinedSettings(UserDefinedSettings settings)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_SetUserDefinedSettings(settings);
#endif
            }

            public static void UPxr_Construct(PXR_Loader.ConvertRotationWith2VectorDelegate fromToRotation)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_Construct(fromToRotation);
#endif
            }

            public static bool UPxr_GetHomeKey()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetHomeKey();
#endif
                return false;
            }

            public static void UPxr_InitHomeKey()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_InitHomeKey();
#endif
            }

            public static bool UPxr_GetMRCEnable()
            {
                bool result = false;
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000300)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    result = Pxr_GetMRCEnable();
#endif
                }
                PLog.d(TAG, "UPxr_GetMRCEnable() result:" + result);
                return result;
            }

            public static int UPxr_SetMRCTextureID(UInt64 IDData)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Debug.Log("ConfigType.MRCTextureID:"+IDData);
                return Pxr_SetConfigUint64(ConfigType.MRCTextureID, IDData);
#else
                return 0;
#endif
            }

            public static int UPxr_SetMRCTextureID2(UInt64 IDData)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Debug.Log("ConfigType.MRCTextureID2:"+IDData);
                return Pxr_SetConfigUint64(ConfigType.MRCTextureID2, IDData);
#else
                return 0;
#endif
            }

            public static int UPxr_SetMrcTextutrWidth(UInt64 width)
            {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000300)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_SetConfigUint64(ConfigType.PxrMrcTextureWidth, width);
#endif
                }
                return 0;
            }

            public static int UPxr_SetMrcTextutrHeight(UInt64 height)
            {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000300)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    return Pxr_SetConfigUint64(ConfigType.PxrMrcTextureHeight, height);
#endif
                }
                return 0;
            }

            public static bool UPxr_GetMrcPose(ref PxrPosef pose)
            {
                int result = 0;
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000300)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    result = Pxr_GetMrcPose(ref pose);
#endif
                }

                PLog.d(TAG, "UPxr_GetMrcPose() result:" + result + " pos.x:" + pose.position.x + " pos.y:" + pose.position.y + " pos.z:" + pose.position.z
                    + " ori.x:" + pose.orientation.x + " ori.y:" + pose.orientation.y + " ori.z:" + pose.orientation.z + " ori.w:" + pose.orientation.w);
                return result == 0;
            }

            public static bool UPxr_SetMrcPose(ref PxrPosef pose)
            {
                int result = 0;
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000300)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    result = Pxr_SetMrcPose(ref pose);
#endif
                }

                PLog.d(TAG, "UPxr_SetMrcPose() result:" + result + " pos.x:" + pose.position.x + " pos.y:" + pose.position.y + " pos.z:" + pose.position.z
                    + " ori.x:" + pose.orientation.x + " ori.y:" + pose.orientation.y + " ori.z:" + pose.orientation.z + " ori.w:" + pose.orientation.w);
                return result == 0;
            }

            public static void UPxr_SetIsSupportMovingMrc(bool support)
            {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000306)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_SetIsSupportMovingMrc(support);
#endif
                }
            }

            public static bool UPxr_GetMrcStatus()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
            if(PXR_Plugin.System.UPxr_GetAPIVersion()>= 0x2000300) {
                return Pxr_GetMrcStatus();
            }else{
                return false;
            }
#else
                return false;
#endif
            }

            public static void UPxr_EnableEyeTracking(bool enable)
            {
                Debug.Log(TAG + "UPxr_EnableEyeTracking() enable:" + enable);
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_EnableEyeTracking(enable);
#endif
            }

            public static void UPxr_EnableFaceTracking(bool enable)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_EnableFaceTracking(enable);
#endif
            }

            public static void UPxr_EnableLipSync(bool enable){
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_EnableLipsync(enable);
#endif
            }

            public static int UPxr_GetFaceTrackingData(Int64 ts, int flags, ref PxrFaceTrackingInfo faceTrackingInfo)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if(PXR_Plugin.System.UPxr_GetAPIVersion()>= 0x2000309) {
                    Pxr_GetFaceTrackingData(ts, flags, ref faceTrackingInfo );
                }
#endif
                return 0;
            }

            public static int UPxr_SetFaceTrackingStatus(PxrFtLipsyncValue value) {
                int num = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                if(PXR_Plugin.System.UPxr_GetAPIVersion()>= 0x200030A) {
                    
                    num = Pxr_SetTrackingStatus("ft_lipsync_ctl", ((int)value).ToString());
                }
#endif
                return num;
            }


            private const string TAG = "[PXR_Plugin/System]";
#if UNITY_ANDROID && !UNITY_EDITOR
            private static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            private static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            private static AndroidJavaClass sysActivity = new AndroidJavaClass("com.psmart.aosoperation.SysActivity");
            private static AndroidJavaClass batteryReceiver = new AndroidJavaClass("com.psmart.aosoperation.BatteryReceiver");
            private static AndroidJavaClass audioReceiver = new AndroidJavaClass("com.psmart.aosoperation.AudioReceiver");
#endif

            public static bool UPxr_StopBatteryReceiver()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    batteryReceiver.CallStatic("pxr_StopReceiver", currentActivity);
                    return true;
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "UPxr_StopBatteryReceiver Error :" + e);
                    return false;
                }
#else
                return true;
#endif
            }

            public static bool UPxr_StartBatteryReceiver(string objName)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    batteryReceiver.CallStatic("pxr_StartReceiver", currentActivity, objName);
                    return true;
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "UPxr_StartBatteryReceiver Error :" + e);
                    return false;
                }
#else
                return true;
#endif
            }

            private static bool isInitAudio = false;

            public static bool UPxr_InitAudioDevice()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    if (isInitAudio) return true;
                    if (sysActivity == null) return false;
                    sysActivity.CallStatic("pxr_InitAudioDevice", currentActivity);
                    isInitAudio = true;
                    return true;
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "UPxr_InitAudioDevice Error :" + e);
                    return false;
                }
#else
                return true;
#endif
            }

            public static bool UPxr_SetBrightness(int brightness)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    sysActivity.CallStatic("pxr_SetScreen_Brightness", brightness, currentActivity);
                    return true;
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "UPxr_SetBrightness Error :" + e);
                    return false;
                }
#else
                return true;
#endif
            }

            public static int UPxr_GetCurrentBrightness()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                int currentlight = 0;
                try
                {
                    currentlight = sysActivity.CallStatic<int>("pxr_GetScreen_Brightness", currentActivity);
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "UPxr_GetCurrentBrightness Error :" + e);
                }

                return currentlight;
#else
                return 0;
#endif
            }

            public static int[] UPxr_GetScreenBrightnessLevel()
            {
                int[] currentlight = { 0 };
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    currentlight = sysActivity.CallStatic<int[]>("getScreenBrightnessLevel");
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "UPxr_GetScreenBrightnessLevel Error :" + e);
                }
#endif
                return currentlight;
            }

            public static void UPxr_SetScreenBrightnessLevel(int vrBrightness, int level)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    sysActivity.CallStatic("setScreenBrightnessLevel", vrBrightness, level);
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "UPxr_SetScreenBrightnessLevel Error :" + e);
                }
#endif
            }

            public static bool UPxr_StartAudioReceiver(string startreceivre)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    if (!UPxr_InitAudioDevice()) return false;
                    audioReceiver.CallStatic("pxr_StartReceiver", currentActivity, startreceivre);
                    return true;
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "UPxr_StartAudioReceiver Error :" + e);
                    return false;
                }
#else
                return true;
#endif
            }

            public static bool UPxr_StopAudioReceiver()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    audioReceiver.CallStatic("pxr_StopReceiver", currentActivity);
                    return true;
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "UPxr_StopAudioReceiver Error :" + e);
                    return false;
                }

#else
                return true;
#endif
            }

            public static int UPxr_GetMaxVolumeNumber()
            {
                int maxvolm = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    maxvolm = sysActivity.CallStatic<int>("pxr_GetMaxAudionumber");
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "UPxr_GetMaxVolumeNumber Error :" + e);
                }
#endif
                return maxvolm;
            }

            public static int UPxr_GetCurrentVolumeNumber()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                int currentvolm = 0;
                try
                {
                    currentvolm = sysActivity.CallStatic<int>("pxr_GetAudionumber");
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "UPxr_GetCurrentVolumeNumber Error :" + e);
                }

                return currentvolm;
#else
                return 0;
#endif
            }

            public static bool UPxr_VolumeUp()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    sysActivity.CallStatic("pxr_UpAudio");
                    return true;
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "UPxr_VolumeUp Error :" + e.ToString());
                    return false;
                }
#else
                return true;
#endif
            }

            public static bool UPxr_VolumeDown()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    sysActivity.CallStatic("pxr_DownAudio");
                    return true;
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "UPxr_VolumeDown Error :" + e.ToString());
                    return false;
                }
#else
                return true;
#endif
            }

            public static bool UPxr_SetVolumeNum(int volume)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    sysActivity.CallStatic("pxr_ChangeAudio", volume);
                    return true;
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "UPxr_SetVolumeNum Error :" + e.ToString());
                    return false;
                }
#else
                return true;
#endif
            }

           
            public static string UPxr_GetDeviceMode()
            {
                string devicemode = "";
#if UNITY_ANDROID && !UNITY_EDITOR
            devicemode = SystemInfo.deviceModel;
#endif
                return devicemode;
            }

            public static float UPxr_GetConfigFloat(ConfigType type)
            {
                PLog.d(TAG, "UPxr_GetConfigFloat() type:" + type);
                float value = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_GetConfigFloat(type, ref value);
#endif
                PLog.d(TAG, "UPxr_GetConfigFloat() value:" + value);
                return value;
            }

            public static int UPxr_GetConfigInt(ConfigType type)
            {
                PLog.d(TAG, "UPxr_GetConfigInt() type:" + type);
                int value = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_GetConfigInt(type, ref value);
#endif
                PLog.d(TAG, "UPxr_GetConfigInt() value:" + value);
                return value;
            }

            public static int UPxr_SetConfigInt(ConfigType configSetIndex, int configSetData)
            {
                PLog.d(TAG, "UPxr_SetConfigInt() configSetIndex:" + configSetIndex + " configSetData:" + configSetData);
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SetConfigInt(configSetIndex, configSetData);
#endif
                PLog.d(TAG, "UPxr_SetConfigInt() result:" + result);
                return result;
            }

            public static int UPxr_ContentProtect(int data)
            {
                int num = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                num = Pxr_SetConfigInt(ConfigType.EnableCPT, data);
                Pxr_UpdateContentProtectState(data);
#endif
                return num;
            }

            public static int UPxr_SetConfigString(ConfigType configSetIndex, string configSetData)
            {
                PLog.d(TAG, "UPxr_SetConfigString() configSetIndex:" + configSetIndex + " configSetData:" + configSetData);
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SetConfigString(configSetIndex, configSetData);
#endif
                PLog.d(TAG, "UPxr_SetConfigString() result:" + result);
                return result;
            }

            public static int UPxr_SetSystemDisplayFrequency(float rate)
            {
                PLog.d(TAG, "UPxr_SetDisplayRefreshRate() rate:" + rate);
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SetDisplayRefreshRate(rate);
#endif
                PLog.d(TAG, "UPxr_SetDisplayRefreshRate() result:" + result);
                return result;
            }

            public static int UPxr_SetPerformanceLevels(PxrPerfSettings which, PxrSettingsLevel level)
            {
                PLog.d(TAG, "UPxr_SetPerformanceLevels() which:" + which + ", level:" + level);
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x200030A)
                {
                    result = Pxr_SetPerformanceLevels(which, level);
                }
#endif
                PLog.d(TAG, "UPxr_SetPerformanceLevels() result:" + result);
                return result;
            }
            
            public static PxrSettingsLevel UPxr_GetPerformanceLevels(PxrPerfSettings which)
            {
                PLog.d(TAG, "UPxr_GetPerformanceLevels() which:" + which);
                int result = 0;
                PxrSettingsLevel level = PxrSettingsLevel.POWER_SAVINGS;
#if UNITY_ANDROID && !UNITY_EDITOR
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x200030A)
                {
                    result = Pxr_GetPerformanceLevels(which, ref level);
                }
#endif
                PLog.d(TAG, "UPxr_GetPerformanceLevels() result:" + result + ", level:" + level);
                return level;
            }
            

            public static void UPxr_Sleep()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                sysActivity.CallStatic("pxr_Sleep");
#endif
            }

            public static void UPxr_SetSecure(bool isOpen)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                sysActivity.CallStatic("SetSecure",currentActivity,isOpen);
#endif
            }

            public static int UPxr_GetColorRes(string name)
            {
                int value = -1;
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    value = sysActivity.CallStatic<int>("getColorRes", currentActivity, name);
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "GetColorResError :" + e.ToString());
                }
#endif
                return value;
            }

            public static int UPxr_GetConfigInt(string name)
            {
                int value = -1;
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    value = sysActivity.CallStatic<int>("getConfigInt", currentActivity, name);
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "GetConfigIntError :" + e.ToString());
                }
#endif
                return value;
            }

            public static string UPxr_GetConfigString(string name)
            {
                string value = "";
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    value = sysActivity.CallStatic<string>("getConfigString", currentActivity, name);
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "GetConfigStringError :" + e.ToString());
                }
#endif
                return value;
            }

            public static string UPxr_GetDrawableLocation(string name)
            {
                string value = "";
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    value = sysActivity.CallStatic<string>("getDrawableLocation", currentActivity, name);
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "GetDrawableLocationError :" + e.ToString());
                }
#endif
                return value;
            }

            public static int UPxr_GetTextSize(string name)
            {
                int value = -1;
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    value = sysActivity.CallStatic<int>("getTextSize", currentActivity, name);
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "GetTextSizeError :" + e.ToString());
                }
#endif
                return value;
            }

            public static string UPxr_GetLangString(string name)
            {
                string value = "";
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    value = sysActivity.CallStatic<string>("getLangString", currentActivity, name);
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "GetLangStringError :" + e.ToString());
                }
#endif
                return value;
            }

            public static string UPxr_GetStringValue(string id, int type)
            {
                string value = "";
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    value = sysActivity.CallStatic<string>("getStringValue", currentActivity, id, type);
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "GetStringValueError :" + e.ToString());
                }
#endif
                return value;
            }

            public static int UPxr_GetIntValue(string id, int type)
            {
                int value = -1;
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    value = sysActivity.CallStatic<int>("getIntValue", currentActivity, id, type);
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "GetIntValueError :" + e.ToString());
                }
#endif
                return value;
            }

            public static float UPxr_GetFloatValue(string id)
            {
                float value = -1;
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    value = sysActivity.CallStatic<float>("getFloatValue", currentActivity, id);
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "GetFloatValueError :" + e.ToString());
                }
#endif
                return value;
            }

            public static string UPxr_GetObjectOrArray(string id, int type)
            {
                string value = "";
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    value = sysActivity.CallStatic<string>("getObjectOrArray", currentActivity, id, type);
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "GetObjectOrArrayError :" + e.ToString());
                }
#endif
                return value;
            }

            public static int UPxr_GetCharSpace(string id)
            {
                int value = -1;
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    value = sysActivity.CallStatic<int>("getCharSpace", currentActivity, id);
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "GetCharSpaceError :" + e.ToString());
                }
#endif
                return value;
            }

            public static float UPxr_RefreshRateChanged()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_RefreshRateChanged();
#else
                return -1.0f;
#endif
            }

            public static float[] UPxr_GetDisplayFrequenciesAvailable()
            {
                float[] configArray = null;
#if UNITY_ANDROID && !UNITY_EDITOR
                int configCount = 0;
                IntPtr configHandle = IntPtr.Zero;
                Pxr_GetDisplayRefreshRatesAvailable(ref configCount, ref configHandle);
                configArray = new float[configCount];
                Marshal.Copy(configHandle, configArray, 0, configCount);
                for (int i = 0; i < configCount; i++) {
                    Debug.Log("LLRR: UPxr_GetDisplayFrequenciesAvailable " + configArray[i]);
                }
#endif
                return configArray;
            }

            public static int UPxr_GetSensorStatus()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetSensorStatus();
#else
                return 0;
#endif
            }

            public static int UPxr_GetPredictedMainSensorStateNew(ref PxrSensorState2 sensorState, ref int sensorFrameIndex)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if(UPxr_GetAPIVersion() >= 0x2000201){
                    double predictTime = UPxr_GetPredictedDisplayTime();
                    return Pxr_GetPredictedMainSensorState2(predictTime, ref sensorState, ref sensorFrameIndex);
                }else
                {
                    return 0;
                }
#else
                return 0;
#endif
            }

            public static int UPxr_GetAPIVersion()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (PXR_API_Version < 0x0000001)
                {
                    PXR_API_Version = UPxr_GetConfigInt(ConfigType.PxrAPIVersion);
                    PLog.i(TAG, "API xrVersion :0x" + PXR_API_Version.ToString("X2"));
                }
                return PXR_API_Version;
#else
                return 0;
#endif
            }

            public static void UPxr_SetLogInfoActive(bool value)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_SetLogInfoActive(value);
#endif
            }

            public static void UPxr_OpenFitnessBandCalibrationAPP()
            {
                UPxr_OpenFitnessBandCalibrationPackage("com.pvr.swift");
            }

            public static void UPxr_OpenPackage(string pkgName)
            {
                AndroidJavaObject activity;
                AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                activity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                using (AndroidJavaObject joPackageManager = activity.Call<AndroidJavaObject>("getPackageManager"))
                {
                    using (AndroidJavaObject joIntent = joPackageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", pkgName))
                    {
                        if (null != joIntent)
                        {
                            activity.Call("startActivity", joIntent);
                        }
                        else
                        {
                            Debug.Log("This software is not installed: " + pkgName);
                        }
                    }
                }
            }

            public static void UPxr_OpenFitnessBandCalibrationPackage(string pkgName)
            {
                using (AndroidJavaClass jcPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (AndroidJavaObject joActivity = jcPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        using (AndroidJavaObject joPackageManager = joActivity.Call<AndroidJavaObject>("getPackageManager"))
                        {
                            using (AndroidJavaObject joIntent = joPackageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", pkgName))
                            {
                                if (null != joIntent)
                                {
                                    //AndroidJavaObject joNIntent = joIntent.Call<AndroidJavaObject>("addFlags", joIntent.GetStatic<int>("FLAG_ACTIVITY_REORDER_TO_FRONT"));
                                    AndroidJavaObject joNIntent = joIntent.Call<AndroidJavaObject>("putExtra", "enter_flag", Application.identifier);
                                    joActivity.Call("startActivity", joNIntent);
                                    joIntent.Dispose();
                                }
                                else
                                {
                                    string msg = "Package <" + pkgName + "> not exsits on device.";
                                    Debug.Log(msg);

                                    using (AndroidJavaClass jT = new AndroidJavaClass("android.widget.Toast"))
                                    {
                                        using (AndroidJavaObject jMsg = new AndroidJavaObject("java.lang.String", msg))
                                        {
                                            using (AndroidJavaObject jC = joActivity.Call<AndroidJavaObject>("getApplicationContext"))
                                            {
                                                int length = jT.GetStatic<int>("LENGTH_SHORT");
                                                using (AndroidJavaObject toast = jT.CallStatic<AndroidJavaObject>("makeText", jC, jMsg, length))
                                                {
                                                    toast.Call("show");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


        }

        public static class Boundary
        {
            private const string TAG = "[PXR_Plugin/Boundary]";

            public static PxrBoundaryTriggerInfo UPxr_TestNodeIsInBoundary(BoundaryTrackingNode node, BoundaryType boundaryType)
            {
                PxrBoundaryTriggerInfo testResult = new PxrBoundaryTriggerInfo();
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_TestNodeIsInBoundary(node, boundaryType == BoundaryType.PlayArea, ref testResult);
                testResult.closestPoint.z = -testResult.closestPoint.z;
                testResult.closestPointNormal.z = -testResult.closestPointNormal.z;
                if (!testResult.valid)
                {
                    PLog.d(TAG, string.Format("Pxr_TestBoundaryNode({0}, {1}) API call failed!", node, boundaryType));
                }
#endif
                return testResult;
            }

            public static PxrBoundaryTriggerInfo UPxr_TestPointIsInBoundary(PxrVector3f point, BoundaryType boundaryType)
            {
                PxrBoundaryTriggerInfo testResult = new PxrBoundaryTriggerInfo();
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_TestPointIsInBoundary(ref point, boundaryType == BoundaryType.PlayArea, ref testResult);

                if (!testResult.valid)
                {
                    PLog.d(TAG, string.Format("Pxr_TestBoundaryPoint({0}, {1}) API call failed!", point, boundaryType));
                }

#endif
                return testResult;
            }

            public static Vector3[] UPxr_GetBoundaryGeometry(BoundaryType boundaryType)
            {
                Vector3[] points = new Vector3[1];
#if UNITY_ANDROID && !UNITY_EDITOR

                UInt32 pointsCountOutput = 0;
                PxrVector3f[] outPointsFirst = null;
                Pxr_GetBoundaryGeometry(boundaryType == BoundaryType.PlayArea, 0, ref pointsCountOutput, outPointsFirst);
                if (pointsCountOutput <= 0)
                {
                    PLog.d(TAG, "Boundary geometry point count = " + pointsCountOutput);
                    return null;
                }

                PxrVector3f[] outPoints = new PxrVector3f[pointsCountOutput];
                Pxr_GetBoundaryGeometry(boundaryType == BoundaryType.PlayArea, pointsCountOutput, ref pointsCountOutput, outPoints);

                points = new Vector3[pointsCountOutput];
                for (int i = 0; i < pointsCountOutput; i++)
                {
                    points[i] = new Vector3()
                    {
                        x = outPoints[i].x,
                        y = outPoints[i].y,
                        z = -outPoints[i].z,
                    };
                }
#endif
                return points;
            }

            public static Vector3 UPxr_GetBoundaryDimensions(BoundaryType boundaryType)
            {
                // float x = 0, y = 0, z = 0;
                PxrVector3f dimension = new PxrVector3f();
#if UNITY_ANDROID && !UNITY_EDITOR
                int ret = 0;
                Pxr_GetBoundaryDimensions( boundaryType == BoundaryType.PlayArea, out dimension);
#endif
                return new Vector3(dimension.x, dimension.y, dimension.z);
            }

            public static void UPxr_SetBoundaryVisiable(bool value)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_SetBoundaryVisible(value);
#endif
            }

            public static bool UPxr_GetBoundaryVisiable()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetBoundaryVisible();
#else
                return true;
#endif
            }

            public static bool UPxr_GetBoundaryConfigured()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetBoundaryConfigured();
#else
                return true;
#endif
            }

            public static bool UPxr_GetBoundaryEnabled()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetBoundaryEnabled();
#else
                return true;
#endif
            }

            public static int UPxr_SetSeeThroughBackground(bool value)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_SetSeeThroughBackground(value);
#else
                return 0;
#endif
            }

            public static int UPxr_GetSeeThroughState()
            {
                var state = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    state = Pxr_GetSeeThroughState();
                    PLog.d(TAG, "UPxr_GetSeeThroughState() state:" + state);
                }
                catch (Exception e)
                {
                    Debug.Log("PXRLog UPxr_GetSeeThroughState :" + e.ToString());
                }
#endif
                return state;
            }

            public static void UPxr_SetSeeThroughState(bool value)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_SetVideoSeethroughState(value);
#endif
            }

            public static void UPxr_ResetSeeThroughSensor()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000301)
                {
                    Pxr_ResetSensorHard();
                }
#endif
            }

            public static PxrTrackingState UPxr_GetSeeThroughTrackingState()
            {
                int state = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000301)
                {
                    state = Pxr_GetTrackingState();
                }
#endif
                return (PxrTrackingState)state;
            }

            public static int UPxr_SetGuardianSystemDisable(bool value)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_SetGuardianSystemDisable(value);
#else
                return 0;
#endif
            }

            public static int UPxr_ResumeGuardianSystemForSTS()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_ResumeGuardianSystemForSTS();
#else
                return 0;
#endif
            }

            public static int UPxr_PauseGuardianSystemForSTS()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_PauseGuardianSystemForSTS();
#else
                return 0;
#endif
            }

            public static int UPxr_ShutdownSdkGuardianSystem()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_ShutdownSdkGuardianSystem();
#else
                return 0;
#endif
            }

            public static int UPxr_GetRoomModeState()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetRoomModeState();
#else
                return 0;
#endif
            }

            public static int UPxr_DisableBoundary()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_DisableBoundary();
#else
                return 0;
#endif
            }

        }

        public static class Render
        {
            private const string TAG = "[PXR_Plugin/Render]";

            public static bool UPxr_SetFoveationLevel(FoveationLevel level)
            {
                bool result = true;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SetFoveationLevelEnable((int)level);
#endif
                PLog.i(TAG, "UPxr_SetFoveationLevel() level:" + level + " result:" + result);
                return result;
            }

            public static bool UPxr_SetEyeFoveationLevel(FoveationLevel level)
            {
                bool result = true;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SetEyeFoveationLevelEnable((int)level);
#endif
                PLog.i(TAG, "UPxr_SetEyeFoveationLevel() level:" + level + " result:" + result);
                return result;
            }

            public static void UPxr_SetFFRSubsampled(bool enable)
            {
                PLog.d(TAG, "UPxr_SetFFRSubsampled() level:" + enable);
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_SetFFRSubsampled(enable);
#endif
            }

            public static FoveationLevel UPxr_GetFoveationLevel()
            {
                FoveationLevel result = FoveationLevel.None;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_GetFoveationLevel();
#endif
                PLog.d(TAG, "UPxr_GetFoveationLevel() result:" + result);
                return result;
            }

            public static int UPxr_SetFoveationParameters(float foveationGainX, float foveationGainY, float foveationArea, float foveationMinimum)
            {
                PLog.d(TAG, "UPxr_SetFoveationParameters() foveationGainX:" + foveationGainX + " foveationGainY:" + foveationGainY + " foveationArea:" + foveationArea + " foveationMinimum:" + foveationMinimum);
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR

                FoveationParams foveationParams = new FoveationParams();
                foveationParams.foveationGainX = foveationGainX;
                foveationParams.foveationGainY = foveationGainY;
                foveationParams.foveationArea = foveationArea;
                foveationParams.foveationMinimum = foveationMinimum;

                result = Pxr_SetFoveationParams(foveationParams);
#endif
                PLog.d(TAG, "UPxr_SetFoveationParameters() result:" + result);
                return result;
            }

            public static int UPxr_GetFrustum(EyeType eye, ref float fovLeft, ref float fovRight, ref float fovUp, ref float fovDown, ref float near, ref float far)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_GetFrustum(eye, ref fovLeft, ref fovRight, ref fovUp, ref fovDown, ref near, ref far);
#endif
                PLog.d(TAG, "UPxr_GetFrustum() result:" + result + " eye:" + eye + " fovLeft:" + fovLeft + " fovRight:" + fovRight + " fovUp:" + fovUp + " fovDown:" + fovDown + " near:" + near + " far:" + far);
                return result;
            }

            public static int UPxr_SetFrustum(EyeType eye, float fovLeft, float fovRight, float fovUp, float fovDown, float near, float far)
            {
                int result = 1;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SetFrustum(eye, fovLeft, fovRight, fovUp, fovDown, near, far);
#endif
                PLog.d(TAG, "UPxr_SetFrustum() result:" + result + " eye:" + eye + " fovLeft:" + fovLeft + " fovRight:" + fovRight + " fovUp:" + fovUp + " fovDown:" + fovDown + " near:" + near + " far:" + far);
                return result;
            }
            
            public static int UPxr_SetEyeFOV(EyeType eye, float fovLeft, float fovRight, float fovUp, float fovDown)
            {
                int result = 0;
                ConfigType type;
                switch (eye)
                {
                    case EyeType.EyeLeft:
                        type = ConfigType.PxrLeftEyeFOV;
                        break;
                    case EyeType.EyeRight:
                        type = ConfigType.PxrRightEyeFOV;
                        break;
                    default:
                        type = ConfigType.PxrBothEyeFOV;
                        break;
                }

                float[] fovData = new float[4];
                fovData[0] = -Mathf.Deg2Rad * fovLeft;
                fovData[1] = Mathf.Deg2Rad * fovRight;
                fovData[2] = Mathf.Deg2Rad * fovUp;
                fovData[3] = -Mathf.Deg2Rad * fovDown;

#if UNITY_ANDROID && !UNITY_EDITOR
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000300)
                {
                    result = Pxr_SetConfigFloatArray(type, fovData, 4);
                }
#endif
                PLog.d(TAG, string.Format("UPxr_SetEyeFOV Pxr_SetConfigFloatArray type = {0}, fovData[0] = {1},  fovData[1] = {2},  fovData[2] = {3},  fovData[3] = {4}, result = {5}", type, fovData[0], fovData[1], fovData[2], fovData[3], result));
                return result;
            }

            public static void UPxr_CreateLayer(IntPtr layerParam)
            {
                PLog.d(TAG, "UPxr_CreateLayer() ");
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_CreateLayer(layerParam);
#endif
            }

            public static void UPxr_CreateLayerParam(PxrLayerParam layerParam)
            {

                PLog.i(TAG, $"Pxr_CreateLayerParam() layerParam.layerId={layerParam.layerId}, layerShape={layerParam.layerShape}, layerType={layerParam.layerType}, width={layerParam.width}, height={layerParam.height}, layerFlags={layerParam.layerFlags}, format={layerParam.format}, layerLayout={layerParam.layerLayout}.");
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_CreateLayerParam(layerParam);
#endif
            }

            public static int UPxr_GetLayerNextImageIndex(int layerId, ref int imageIndex)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_GetLayerNextImageIndex(layerId, ref imageIndex);
#endif
                PLog.d(TAG, "UPxr_GetLayerNextImageIndex() layerId:" + layerId + " imageIndex:" + imageIndex + " result:" + result);
                return result;
            }

            public static int UPxr_GetLayerImageCount(int layerId, EyeType eye, ref UInt32 imageCount)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_GetLayerImageCount(layerId, eye, ref imageCount);
#endif
                PLog.d(TAG, "UPxr_GetLayerImageCount() layerId:" + layerId + " eye:" + eye + " imageCount:" + imageCount + " result:" + result);
                return result;
            }

            public static int UPxr_GetLayerImage(int layerId, EyeType eye, int imageIndex, ref UInt64 image)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_GetLayerImage(layerId, eye, imageIndex, ref image);
#endif
                PLog.d(TAG, "UPxr_GetLayerImage() layerId:" + layerId + " eye:" + eye + " imageIndex:" + imageIndex + " image:" + image + " result:" + result);
                return result;
            }

            public static void UPxr_GetLayerImagePtr(int layerId, EyeType eye, int imageIndex, ref IntPtr image)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_GetLayerImagePtr(layerId, eye, imageIndex, ref image);
#endif
                PLog.d(TAG, "UPxr_GetLayerImagePtr() layerId:" + layerId + " eye:" + eye + " imageIndex:" + imageIndex + " image:" + image);
            }

            public static int UPxr_SetConfigIntArray(int[] configSetData)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000300)
                {
                    return Pxr_SetConfigIntArray(ConfigType.PxrAndroidLayerDimensions, configSetData, 3);
                }
#endif
                return 0;
            }

            public static int UPxr_SetConfigFloatArray(ConfigType configIndex, float[] configSetData, int dataCount)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000300)
                {
                    return Pxr_SetConfigFloatArray(configIndex, configSetData, dataCount);
                }
#endif
                return 0;
            }

            public static int UPxr_GetLayerAndroidSurface(int layerId, EyeType eye, ref IntPtr androidSurface)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_GetLayerAndroidSurface(layerId, eye, ref androidSurface);
#endif
                PLog.d(TAG, "UPxr_GetLayerAndroidSurface() layerId:" + layerId + " eye:" + eye + " androidSurface:" + androidSurface + " result:" + result);
                return result;
            }

            public static int UPxr_DestroyLayer(int layerId)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_DestroyLayer(layerId);
#endif
                PLog.d(TAG, "UPxr_DestroyLayer() layerId:" + layerId + " result:" + result);
                return result;
            }

            public static void UPxr_DestroyLayerByRender(int layerId)
            {
                PLog.d(TAG, "UPxr_DestroyLayerByRender() layerId:" + layerId);
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_DestroyLayerByRender(layerId);
#endif
            }

            public static int UPxr_SubmitLayer(IntPtr layer)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayer(layer);
#endif
                PLog.d(TAG, "UPxr_SubmitLayer() layer:" + layer + " result:" + result);
                return result;
            }

            public static int UPxr_SubmitLayerQuad(PxrLayerQuad layer)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerQuad(layer);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerQuad() layer:" + layer + " result:" + result);
                return result;
            }

            public static bool UPxr_SubmitLayerQuad2(PxrLayerQuad2 layer)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerQuad2(layer);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerQuad2() layer:" + layer.header.layerId + " result:" + result);
                return result == -8;
            }


            public static bool UPxr_GetLayerNextImageIndexByRender(int layerId, ref int imageIndex)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_GetLayerNextImageIndexByRender(layerId, ref imageIndex);
#endif
                PLog.d(TAG, "UPxr_GetLayerNextImageIndexByRender() layerId:" + layerId + " imageIndex:" + imageIndex);
                return result == -8;
            }


            public static bool UPxr_SubmitLayerQuadByRender(IntPtr ptr)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerQuadByRender(ptr);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerQuadByRender() ptr:" + ptr + " result:" + result);
                return result == -8;
            }
            
            public static bool UPxr_SubmitLayerQuad2ByRender(IntPtr ptr)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerQuad2ByRender(ptr);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerQuad2ByRender() ptr:" + ptr + " result:" + result);
                return result == -8;
            }

            public static bool UPxr_SubmitLayerCylinderByRender(IntPtr ptr)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerCylinderByRender(ptr);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerCylinderByRender() ptr:" + ptr + " result:" + result);
                return result == -8;
            }

            public static bool UPxr_SubmitLayerCylinder2ByRender(IntPtr ptr)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerCylinder2ByRender(ptr);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerCylinder2ByRender() ptr:" + ptr + " result:" + result);
                return result == -8;
            }

            public static bool UPxr_SubmitLayerEquirectByRender(IntPtr ptr)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerEquirectByRender(ptr);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerEquirectByRender() ptr:" + ptr + " result:" + result);
                return result == -8;
            }

            public static bool UPxr_SubmitLayerEquirect2ByRender(IntPtr ptr)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerEquirect2ByRender(ptr);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerEquirect2ByRender() ptr:" + ptr + " result:" + result);
                return result == -8;
            }

            public static bool UPxr_SubmitLayerCube2ByRender(IntPtr ptr)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerCube2ByRender(ptr);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerCube2ByRender() ptr:" + ptr + " result:" + result);
                return result == -8;
            }

            public static bool UPxr_SubmitLayerEac2ByRender(IntPtr ptr)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerEac2ByRender(ptr);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerEac2ByRender() ptr:" + ptr + " result:" + result);
                return result == -8;
            }

            public static int UPxr_SubmitLayerCylinder(PxrLayerCylinder layer)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerCylinder(layer);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerCylinder() layer:" + layer + " result:" + result);
                return result;
            }

            public static bool UPxr_SubmitLayerCylinder2(PxrLayerCylinder2 layer)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerCylinder2(layer);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerCylinder2() layer:" + layer.header.layerId + " result:" + result);
                return result == -8;
            }

            public static bool UPxr_SubmitLayerEquirect(PxrLayerEquirect layer) // shape 3
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerEquirect(layer);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerEquirect() layer:" + layer.header.layerId + " result:" + result);
                return result == -8;
            }

            public static bool UPxr_SubmitLayerEquirect2(PxrLayerEquirect2 layer) // shape 4
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerEquirect2(layer);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerEquirect2() layer:" + layer.header.layerId + " result:" + result);
                return result == -8;
            }

            public static int UPxr_SubmitLayerCube2(PxrLayerCube2 layer)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerCube2(layer);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerCube2() layer:" + layer.header.layerId + " result:" + result);
                return result;
            }

            public static int UPxr_SubmitLayerEac2(PxrLayerEac2 layer)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerEac2(layer);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerEac2() layer:" + layer.header.layerId + " result:" + result);
                return result;
            }

            public static void UPxr_SetLayerBlend(bool enable, PxrLayerBlend layerBlend)
            {
                PLog.d(TAG, "UPxr_SetLayerBlend() enable:" + enable + " layerBlend.srcColor:" + layerBlend.srcColor + " dstColor:" + layerBlend.dstColor + " srcAlpha:" + layerBlend.srcAlpha + " dstAlpha:" + layerBlend.dstAlpha);
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_SetLayerBlend(enable, layerBlend);
#endif
            }

            public static void UPxr_SetSpaceWarp(bool value)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_SetSpaceWarp(value?1:0);
#endif
                PLog.d(TAG, "UPxr_SetSpaceWarp " + value);
            }

            public static void UPxr_SetAppSpacePosition(float x, float y, float z)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_SetAppSpacePosition(x, y, z);
#endif
            }

            public static void UPxr_SetAppSpaceRotation(float x, float y, float z, float w)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_SetAppSpaceRotation(x, y, z, w);
#endif
            }

            public static void UPxr_EnablePremultipliedAlpha(bool enable)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_EnablePremultipliedAlpha(enable);
#endif
                PLog.i(TAG, "Pxr_EnablePremultipliedAlpha " + enable);
            }

        }

        public static class Sensor
        {
            private const string TAG = "[PXR_Plugin/Sensor]";

#if UNITY_ANDROID && !UNITY_EDITOR
            private static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            private static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            private static AndroidJavaClass sysActivity = new AndroidJavaClass("com.psmart.aosoperation.SysActivity");
#endif

            public static int UPxr_ResetSensor(ResetSensorOption resetSensorOption)
            {
                PLog.d(TAG, string.Format("UPxr_ResetSensor : {0}", resetSensorOption));
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_ResetSensor(resetSensorOption);
#endif
                PLog.d(TAG, string.Format("UPxr_ResetSensor result: {0}", result));
                return result;
            }

            public static int UPvr_Enable6DofModule(bool enable)
            {
                PLog.d(TAG, string.Format("UPvr_Enable6DofModule : {0}", enable));
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_SetConfigInt(ConfigType.Ability6Dof, enable?1:0);
#else
                return 0;
#endif
            }

            public static void UPxr_InitPsensor()
            {
                PLog.d(TAG, "UPxr_InitPsensor()");
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    sysActivity.CallStatic("initPsensor", currentActivity);
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "Error :" + e.ToString());
                }
#endif
            }

            public static int UPxr_GetPSensorState()
            {
                PLog.d(TAG, "UPxr_GetPSensorState()");
                int psensor = -1;
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    psensor = sysActivity.CallStatic<int>("getPsensorState");
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "Error :" + e.ToString());
                }
#endif
                PLog.d(TAG, "UPxr_GetPSensorState() psensor:" + psensor);
                return psensor;
            }

            public static void UPxr_UnregisterPsensor()
            {
                PLog.d(TAG, "UPxr_UnregisterPsensor()");
#if UNITY_ANDROID && !UNITY_EDITOR
                try
                {
                    sysActivity.CallStatic("unregisterListener");
                }
                catch (Exception e)
                {
                    PLog.e(TAG, "Error :" + e.ToString());
                }
#endif
            }

            public static int UPxr_SetSensorLostCustomMode(bool value)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_SetSensorLostCustomMode(value);
#else
                return 0;
#endif
            }

            public static int UPxr_SetSensorLostCMST(bool value)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_SetSensorLostCMST(value);
#else
                return 0;
#endif
            }

        }

        public static class PlatformSetting
        {
            private const string TAG = "[PXR_Plugin/PlatformSetting]";

#if UNITY_ANDROID && !UNITY_EDITOR
            private static AndroidJavaClass MRCCalibration = new AndroidJavaClass("com.psmart.aosoperation.MRCCalibration");
            private static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            private static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
#endif

            public static float[] UPxr_MRCCalibration(string path)
            {
                float[] MRCdata = new float[10];
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000300)
                {
                    AndroidJavaObject MrcCalibration = new AndroidJavaObject("com.psmart.aosoperation.MRCCalibration");
#if UNITY_ANDROID && !UNITY_EDITOR
                MRCdata =  MrcCalibration.Call<float[]>("readCalibrationData",path);
#endif
                }
                return MRCdata;
            }
        }

        public static class Controller
        {
            private const string TAG = "[PXR_Plugin/Controller]";

            public static int UPxr_SetControllerVibration(UInt32 hand, float strength, int time)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_SetControllerVibration(hand,strength, time);
#else
                return 0;
#endif
            }

            public static int UPxr_SetControllerEnableKey(bool isEnable, PxrControllerKeyMap Key)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_SetControllerEnableKey(isEnable, Key);
#else
                return 0;
#endif
            }

            public static int UPxr_GetBodyTrackingPose(double predictTime, ref BodyTrackerResult bodyTrackerResult)
            {
                int state = 1;
#if UNITY_ANDROID && !UNITY_EDITOR
                state = Pxr_GetBodyTrackingPose(predictTime,ref bodyTrackerResult);
                for (int i = 0; i < 24; i++) {
                    bodyTrackerResult.trackingdata[i].localpose.PosZ = -bodyTrackerResult.trackingdata[i].localpose.PosZ;
                    bodyTrackerResult.trackingdata[i].localpose.RotQz = -bodyTrackerResult.trackingdata[i].localpose.RotQz;
                    bodyTrackerResult.trackingdata[i].localpose.RotQw = -bodyTrackerResult.trackingdata[i].localpose.RotQw;
                    bodyTrackerResult.trackingdata[i].velo[2] = -bodyTrackerResult.trackingdata[i].velo[2];
                    bodyTrackerResult.trackingdata[i].acce[2] = -bodyTrackerResult.trackingdata[i].acce[2];
                    bodyTrackerResult.trackingdata[i].wvelo[2] = -bodyTrackerResult.trackingdata[i].wvelo[2];
                    bodyTrackerResult.trackingdata[i].wacce[2] = -bodyTrackerResult.trackingdata[i].wacce[2];
                }
#endif
                return state;
            }

            public static int UPxr_SetBodyTrackingMode(int mode) {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_SetBodyTrackingMode(mode);
#endif
                return 0;
            }

            public static int UPxr_GetFitnessBandConnectState(ref PxrFitnessBandConnectState state)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetFitnessBandConnectState(ref state);
#endif
                return 0;
            }

            public static int UPxr_GetFitnessBandBattery(int trackerId, ref int battery) {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetFitnessBandBattery(trackerId, ref battery);
#endif
                return 0;
            }

            public static int UPxr_GetFitnessBandCalibState(ref int calibrated) {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetFitnessBandCalibState(ref calibrated);
#endif
                return 0;
            }

            public static int UPxr_SetSwiftMode(int mode)
            {
                if (System.UPxr_GetAPIVersion() >= 0x200030C)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    BodyTrackingAlgParam pxrBodyTrackingAlgParam = new BodyTrackingAlgParam();
                    pxrBodyTrackingAlgParam.BodyJointSet = mode;
                    return Pxr_SetBodyTrackingAlgParam(BodyTrackingAlgParamType.SWIFT_MODE, ref pxrBodyTrackingAlgParam);
#endif
                }
                return 0;
            }

            public static int UPxr_SetBodyTrackingBoneLength(BodyTrackingBoneLength boneLength)
            {
                if (System.UPxr_GetAPIVersion() >= 0x200030C)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    BodyTrackingAlgParam pxrBodyTrackingAlgParam = new BodyTrackingAlgParam();
                    pxrBodyTrackingAlgParam.BodyJointSet = 1;
                    pxrBodyTrackingAlgParam.BoneLength = boneLength;
                    return Pxr_SetBodyTrackingAlgParam(BodyTrackingAlgParamType.BONE_PARAM, ref pxrBodyTrackingAlgParam);
#endif
                }
                return 0;
            }

            public static int UPxr_SetControllerVibrationEvent(UInt32 hand, int frequency, float strength, int time)
            {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000305)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_SetControllerVibrationEvent(hand, frequency,strength, time);
#endif
                }
                return 0;
            }

            public static int UPxr_GetControllerType()
            {
                var type = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                PxrControllerCapability capability = new PxrControllerCapability();
                Pxr_GetControllerCapabilities(0,ref capability);
                type = (int)capability.type;
#endif
                PLog.d(TAG, "UPxr_GetControllerType()" + type);
                return type;
            }

            //Pxr_StopControllerVCMotor

            public static int UPxr_StopControllerVCMotor(int id)
            {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000308)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                Debug.Log("[VCMotor_SDK] StopControllerVCMotor :" + id.ToString());
                return Pxr_StopControllerVCMotor(id);
#endif
                }
                return 0;
            }

            public static int UPxr_StartControllerVCMotor(string file, int slot)
            {
                //0-Left And Right 1-Left 2-Right 3-Left And Right
                //0-Reversal 1-No Reversal
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000308)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                Debug.Log("[VCMotor_SDK] StartControllerVCMotor " + file + " slot: " + slot.ToString());
                return Pxr_StartControllerVCMotor(file,slot);
#endif
                }
                return 0;
            }

            public static int UPxr_SetControllerAmp(float mode)
            {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000305)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_SetControllerAmp(mode);
#endif
                }
                return 0;
            }

            public static int UPxr_SetControllerDelay()
            {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000305)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                int delay = 3;
                int Length;
                int num;
                AudioSettings.GetDSPBufferSize(out Length, out num);
                if (Length == 256)
                {
                    delay = 1;
                }
                else if (Length == 512) {
                    delay = 2;
                } else if (Length == 1024) {
                    delay = 3;
                }
                Debug.Log("[VCMotor_SDK] UPxr_SetControllerDelay " + delay.ToString());
                return Pxr_SetControllerDelay(delay);
#endif
                }
                return 0;

            }

            public static string UPxr_GetVibrateDelayTime(ref int x)
            {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000305)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetVibrateDelayTime(ref x);
#endif
                }
                return " ";
            }

            public static int UPxr_StartVibrateBySharem(float[] data, int slot, int buffersize, int sampleRate, int channelMask, int bitrate ,int channelFlip, ref int sourceId)
            {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000308)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                AudioClipData audioClipData = new AudioClipData();
                audioClipData.slot = slot;
                audioClipData.channelCounts = channelMask;
                audioClipData.buffersize = (UInt64)buffersize;
                audioClipData.sampleRate = sampleRate;
                audioClipData.reversal = channelFlip;
                audioClipData.bitrate = bitrate;
                audioClipData.isCache = 0;
                Debug.Log("[VCMotor_SDK] Pxr_StartVibrateBySharem " + " slot: " + audioClipData.slot.ToString() + " buffersize:" + audioClipData.buffersize.ToString() + " sampleRate" + audioClipData.sampleRate.ToString() + " channelCounts:" + audioClipData.channelCounts.ToString()+" bitrate:" + audioClipData.bitrate.ToString());
                return Pxr_StartVibrateBySharemF(data, ref audioClipData, ref sourceId);
#endif
                }
                return 0;
            }

            public static int UPxr_SaveVibrateByCache(float[] data, int slot, int buffersize, int sampleRate, int channelMask, int bitrate, int slotconfig, int enableV , ref int sourceId)
            {

                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000308)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    AudioClipData audioClipData = new AudioClipData();
                    audioClipData.slot = slot;
                    audioClipData.buffersize = (UInt64)buffersize;
                    audioClipData.sampleRate = sampleRate;
                    audioClipData.channelCounts = channelMask;
                    audioClipData.bitrate = bitrate;
                    audioClipData.reversal = slotconfig;
                    audioClipData.isCache = enableV;
                    Debug.Log("[VCMotor_SDK] UPxr_SaveVibrateByCache " + " slot: " + audioClipData.slot.ToString() + " buffersize:" + audioClipData.buffersize.ToString() + " sampleRate" + audioClipData.sampleRate.ToString() + " channelMask:" + audioClipData.channelCounts.ToString() + " bitrate:" + audioClipData.bitrate.ToString());
                    return Pxr_StartVibrateBySharemF(data, ref audioClipData, ref sourceId);
#endif
                }
                return 0;
            }

            public static int UPxr_StartVibrateByCache(int clicpid)
            {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000308)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] UPxr_StartVibrateByCache " + clicpid.ToString());
                    return Pxr_StartVibrateByCache(clicpid);
#endif
                }
                return 0;
            }

            public static int UPxr_ClearVibrateByCache(int clicpid)
            {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000308)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] UPxr_ClearVibrateByCache " + clicpid.ToString());
                    return Pxr_ClearVibrateByCache(clicpid);
#endif
                }
                return 0;
            }

            public static int UPxr_StartVibrateByPHF(string data, int buffersize, ref int sourceId, int slot, int reversal, float amp) {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000308)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    VibrateInfo vibrateInfo = new VibrateInfo();
                    vibrateInfo.slot = (uint)slot;
                    vibrateInfo.reversal = (uint)reversal;
                    vibrateInfo.amp = amp;
                    Debug.Log("[VCMotor_SDK] Pxr_StartVibrateByPHF " + buffersize.ToString());
                    return Pxr_StartVibrateByPHF(data, buffersize, ref sourceId, ref vibrateInfo);
#endif
                }
                return 0;
            }

            public static int UPxr_PauseVibrate(int sourceID) {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000308)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] Pxr_PauseVibrate " + sourceID.ToString());
                    return Pxr_PauseVibrate(sourceID);
#endif
                }
                return 0;
            }

            public static int UPxr_ResumeVibrate(int sourceID) {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000308)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] Pxr_ResumeVibrate " + sourceID.ToString());
                    return Pxr_ResumeVibrate(sourceID);
#endif
                }
                return 0;
            }

            public static int UPxr_UpdateVibrateParams(int clicp_id,int slot, int reversal, float amp) {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x2000308)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    VibrateInfo vibrateInfo = new VibrateInfo();
                    vibrateInfo.slot = (uint)slot;
                    vibrateInfo.reversal = (uint)reversal;
                    vibrateInfo.amp = amp;
                    Debug.Log("[VCMotor_SDK] UPxr_UpdateVibrateParams " + clicp_id.ToString() + " solt: " + slot.ToString() + " reversal:" + reversal.ToString() + " AMP:" + amp.ToString());
                    return Pxr_UpdateVibrateParams(clicp_id, ref vibrateInfo);         
#endif
                }
                return 0;
            }

            public static int UPxr_CreateHapticStream(string phfVersion, UInt32 frameDurationMs, ref VibrateInfo hapticInfo, float speed, ref int id) {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x200030A)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] UPxr_CreateHapticStream ");
                    return Pxr_CreateHapticStream(phfVersion, frameDurationMs, ref hapticInfo, speed, ref id);    
#endif
                }
                return 0;
            }

            public static int UPxr_WriteHapticStream(int id, ref PxrPhfParamsNum frames, UInt32 numFrames) {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x200030A)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] UPxr_WriteHapticStream ");
                    return Pxr_WriteHapticStream( id, ref  frames,  numFrames); 
#endif
                }
                return 0;
            }

            public static int UPxr_SetPHFHapticSpeed(int id, float speed) {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x200030A)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] UPxr_SetPHFHapticSpeed ");
                    return Pxr_SetPHFHapticSpeed( id,  speed);
#endif
                }
                return 0;
            }

            public static int UPxr_GetPHFHapticSpeed(int id, ref float speed) {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x200030A)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] UPxr_GetPHFHapticSpeed ");
                    return Pxr_GetPHFHapticSpeed( id, ref speed);
#endif
                }
                return 0;
            }

            public static int UPxr_GetCurrentFrameSequence(int id, ref UInt64 frameSequence) {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x200030A)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] UPxr_GetCurrentFrameSequence ");
                    return Pxr_GetCurrentFrameSequence( id, ref  frameSequence);
#endif
                }
                return 0;
            }

            public static int UPxr_StartPHFHaptic(int source_id) {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x200030A)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] UPxr_StartPHFHaptic ");
                    return Pxr_StartPHFHaptic(source_id);
#endif
                }
                return 0;
            }

            public static int UPxr_StopPHFHaptic(int source_id) {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x200030A)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] UPxr_StopPHFHaptic ");
                    return Pxr_StopPHFHaptic(source_id);
#endif
                }
                return 0;
            }

            public static int UPxr_RemovePHFHaptic(int source_id) {
                if (PXR_Plugin.System.UPxr_GetAPIVersion() >= 0x200030A)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] UPxr_RemovePHFHaptic ");
                    return Pxr_RemovePHFHaptic(source_id);
#endif
                }
                return 0;
            }

            public static int UPxr_SetControllerMainInputHandle(UInt32 hand)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_SetControllerMainInputHandle(hand);
#else
                return 0;
#endif
            }

            public static PXR_Input.Controller UPxr_GetControllerMainInputHandle()
            {
                var hand = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_GetControllerMainInputHandle(ref hand);
#endif
                PLog.d(TAG, "Pxr_GetControllerMainInputHandle()" + hand.ToString());
                return (PXR_Input.Controller)hand;
            }

            public static int UPxr_GetControllerTrackingState(UInt32 deviceID, double predictTime, float[] headSensorData, ref PxrControllerTracking tracking)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetControllerTrackingState(deviceID,predictTime,headSensorData, ref tracking);
#else
                return 0;
#endif
            }

            public static void UPxr_SetControllerOriginOffset(int controllerID, Vector3 offset)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_SetControllerOriginOffset(controllerID, offset);
#endif
            }

            public static void UPxr_ResetController()
            {
                if (System.UPxr_GetAPIVersion() >= 0x200030B)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Pxr_ResetController(0);
#endif
                }
            }

            public static void UPxr_SetArmModelParameters(PxrGazeType gazetype, PxrArmModelType armmodeltype, float elbowHeight, float elbowDepth, float pointerTiltAngle)
            {
                if (System.UPxr_GetAPIVersion() >= 0x200030B)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Pxr_SetArmModelParameters(gazetype, armmodeltype, elbowHeight, elbowDepth, pointerTiltAngle);
#endif
                }
            }

            public static void UPxr_GetControllerHandness(ref int deviceID)
            {
                if (System.UPxr_GetAPIVersion() >= 0x200030B)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Pxr_GetControllerHandness(ref deviceID);
#endif
                }
            }
        }

        public static class HandTracking
        {
            [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetHandTrackerSettingState(ref bool settingState);

            [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetHandTrackerActiveInputType(ref ActiveInputDevice activeInputDevice);

            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetHandTrackerAimState(HandType hand, ref HandAimState aimState);

            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetHandTrackerJointLocations(HandType hand, ref HandJointLocations jointLocations);


            public static bool UPxr_GetHandTrackerSettingState()
            {
                bool val = false;
                if (System.UPxr_GetAPIVersion() >= 0x2000306)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                Pxr_GetHandTrackerSettingState(ref val);
#endif
                }
                return val;
            }

            public static ActiveInputDevice UPxr_GetHandTrackerActiveInputType()
            {
                ActiveInputDevice val = ActiveInputDevice.HeadActive;
                if (System.UPxr_GetAPIVersion() >= 0x2000307)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    Pxr_GetHandTrackerActiveInputType(ref val);
#endif
                }
                return val;
            }

            public static bool UPxr_GetHandTrackerAimState(HandType hand, ref HandAimState aimState)
            {
                bool val = false;
                if (System.UPxr_GetAPIVersion() >= 0x2000306)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                val = Pxr_GetHandTrackerAimState(hand,ref aimState) == 0;
#endif
                }
                return val;
            }

            public static bool UPxr_GetHandTrackerJointLocations(HandType hand, ref HandJointLocations jointLocations)
            {
                bool val = false;
                if (System.UPxr_GetAPIVersion() >= 0x2000306)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_GetHandTrackerJointLocations(hand, ref jointLocations) == 0;
#endif
                }
                return val;
            }
        }

        public static class MotionTracking
        {
            #region Eye Tracking

            [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_WantEyeTrackingService();
            [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetEyeTrackingSupported(ref bool supported, ref int supportedModesCount, ref EyeTrackingMode supportedModes);
            [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_StartEyeTracking1(ref EyeTrackingStartInfo startInfo);
            [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_StopEyeTracking1(ref EyeTrackingStopInfo stopInfo);
            [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetEyeTrackingState(ref bool isTracking, ref EyeTrackingState state);
            [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetEyeTrackingData1(ref EyeTrackingDataGetInfo getInfo, ref EyeTrackingData data);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetEyeOpenness(ref float leftEyeOpenness, ref float rightEyeOpenness);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetEyePupilInfo(ref EyePupilInfo eyePupilPosition);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetPerEyePose(ref long timestamp, ref Posef leftEyePose, ref Posef rightPose);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetEyeBlink(ref long timestamp, ref bool isLeftBlink, ref bool isRightBlink);


            public static int UPxr_WantEyeTrackingService()
            {
                int val = 0;
                if (System.UPxr_GetAPIVersion() >= 0x200030C)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_WantEyeTrackingService();
#endif
                }
                return val;
            }

            public static int UPxr_GetEyeTrackingSupported(ref bool supported, ref int supportedModesCount, ref EyeTrackingMode supportedModes)
            {
                int val = -1;
                if (System.UPxr_GetAPIVersion() >= 0x200030C)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_GetEyeTrackingSupported(ref supported, ref supportedModesCount, ref supportedModes);
#endif
                }
                return val;
            }

            public static int UPxr_StartEyeTracking1(ref EyeTrackingStartInfo startInfo)
            {
                int val = -1;
                if (System.UPxr_GetAPIVersion() >= 0x200030C)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_StartEyeTracking1(ref startInfo);
#endif
                }
                return val;
            }

            public static int UPxr_StopEyeTracking1(ref EyeTrackingStopInfo stopInfo)
            {
                int val = -1;
                if (System.UPxr_GetAPIVersion() >= 0x200030C)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_StopEyeTracking1(ref stopInfo);
#endif
                }
                return val;
            }

            public static int UPxr_GetEyeTrackingState(ref bool isTracking, ref EyeTrackingState state)
            {
                int val = -1;
                if (System.UPxr_GetAPIVersion() >= 0x200030C)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_GetEyeTrackingState(ref isTracking, ref state);
#endif
                }
                return val;
            }

            public static int UPxr_GetEyeTrackingData1(ref EyeTrackingDataGetInfo getInfo, ref EyeTrackingData data)
            {
                int val = -1;
                if (System.UPxr_GetAPIVersion() >= 0x200030C)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_GetEyeTrackingData1(ref getInfo, ref data);
#endif
                }
                return val;
            }

            public static int UPxr_GetEyeOpenness(ref float leftEyeOpenness, ref float rightEyeOpenness)
            {
                int val = 0;
                if (System.UPxr_GetAPIVersion() >= 0x200030C)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_GetEyeOpenness(ref leftEyeOpenness, ref rightEyeOpenness);
#endif
                }
                return val;
            }

            public static int UPxr_GetEyePupilInfo(ref EyePupilInfo eyePupilPosition)
            {
                int val = 0;
                if (System.UPxr_GetAPIVersion() >= 0x200030C)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_GetEyePupilInfo(ref eyePupilPosition);
#endif
                }
                return val;
            }

            public static int UPxr_GetPerEyePose(ref long timestamp, ref Posef leftEyePose, ref Posef rightPose)
            {
                int val = 0;
                if (System.UPxr_GetAPIVersion() >= 0x200030C)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_GetPerEyePose(ref timestamp, ref leftEyePose, ref rightPose);
#endif
                }
                return val;
            }

            public static int UPxr_GetEyeBlink(ref long timestamp, ref bool isLeftBlink, ref bool isRightBlink)
            {
                int val = 0;
                if (System.UPxr_GetAPIVersion() >= 0x200030C)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_GetEyeBlink(ref timestamp, ref isLeftBlink, ref isRightBlink);
#endif
                }
                return val;
            }

            #endregion

            #region Face Tracking

            [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_WantFaceTrackingService();
            [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetFaceTrackingSupported(ref bool supported, ref int supportedModesCount, ref FaceTrackingSupportedMode supportedModes);
            [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_StartFaceTracking(ref FaceTrackingStartInfo startInfo);
            [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_StopFaceTracking(ref FaceTrackingStopInfo stopInfo);
            [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetFaceTrackingState(ref bool isTracking, ref FaceTrackingState state);
            [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetFaceTrackingData1(ref FaceTrackingDataGetInfo getInfo, ref FaceTrackingData data);

            public static int UPxr_WantFaceTrackingService()
            {
                int val = -1;
                if (System.UPxr_GetAPIVersion() >= 0x200030C)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_WantFaceTrackingService();
#endif
                }
                return val;
            }

            public static int UPxr_GetFaceTrackingSupported(ref bool supported, ref int supportedModesCount, ref FaceTrackingSupportedMode supportedModes)
            {
                int val = -1;
                if (System.UPxr_GetAPIVersion() >= 0x200030C)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_GetFaceTrackingSupported(ref supported, ref supportedModesCount, ref supportedModes);
#endif
        }
                return val;
            }

            public static int UPxr_StartFaceTracking(ref FaceTrackingStartInfo startInfo)
            {
                int val = -1;
                if (System.UPxr_GetAPIVersion() >= 0x200030C)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_StartFaceTracking(ref startInfo);
#endif
                }
                return val;
            }

            public static int UPxr_StopFaceTracking(ref FaceTrackingStopInfo stopInfo)
            {
                int val = -1;
                if (System.UPxr_GetAPIVersion() >= 0x200030C)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_StopFaceTracking(ref stopInfo);
#endif
                }
                return val;
            }

            public static int UPxr_GetFaceTrackingState(ref bool isTracking, ref FaceTrackingState state)
            {
                int val = -1;
                if (System.UPxr_GetAPIVersion() >= 0x200030C)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_GetFaceTrackingState(ref isTracking, ref state);
#endif
                }
                return val;
            }

            public static int UPxr_GetFaceTrackingData1(ref FaceTrackingDataGetInfo getInfo, ref FaceTrackingData data)
            {
                int val = -1;
                if (System.UPxr_GetAPIVersion() >= 0x200030C)
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_GetFaceTrackingData1(ref getInfo, ref data);
#endif
                }
                return val;
            }

            #endregion
        }

        public static class MixedReality
        {
            private const string TAG = "[PXR_Plugin/MixedReality]";

            public static int UPxr_EnableVideoSeeThroughEffect(bool value)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_EnablePassthroughStyle(value);
#else
                return -1;
#endif
            }

            public static int UPxr_SetVideoSeeThroughEffect(PxrLayerEffect type, float value, float duration)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_SetPassthroughStyle(type, value, duration);
#else
                return -1;
#endif
            }

            public static int UPxr_SetVideoSeeThroughLUT(ref byte[] data, int width, int height, int row, int col)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_SetPassthroughLUT(ref data, width, height, row, col);
#else
                return -1;
#endif
            }


            public static PxrResult UPxr_CreateAnchorEntity(ref PxrAnchorEntityCreateInfo info, out ulong anchorHandle)
            {
                anchorHandle = ulong.MinValue;
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_CreateAnchorEntity(ref info,out anchorHandle);
#else
                return PxrResult.TIMEOUT_EXPIRED;
#endif
            }

            public static PxrResult UPxr_DestroyAnchorEntity(ref PxrAnchorEntityDestroyInfo info)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_DestroyAnchorEntity(ref info);
#else
                return PxrResult.TIMEOUT_EXPIRED;
#endif
            }

            public static PxrResult UPxr_PersistAnchorEntity(ref PxrAnchorEntityPersistInfo info, out ulong taskId)
            {
                taskId = ulong.MinValue;
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_PersistAnchorEntity(ref info, out taskId);
#else
                return PxrResult.TIMEOUT_EXPIRED;
#endif
            }

            public static PxrResult UPxr_UnpersistAnchorEntity(ref PxrAnchorEntityUnPersistInfo info, out ulong taskId)
            {
                taskId = ulong.MinValue;
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_UnpersistAnchorEntity(ref info, out taskId);
#else
                return PxrResult.TIMEOUT_EXPIRED;
#endif
            }

            public static PxrResult UPxr_ClearPersistedAnchorEntity(ref PxrAnchorEntityClearInfo info, out ulong taskId)
            {
                taskId = ulong.MinValue;
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_ClearPersistedAnchorEntity(ref info, out taskId);
#else
                return PxrResult.TIMEOUT_EXPIRED;
#endif
            }

            public static PxrResult UPxr_GetAnchorPose(ulong anchorHandle, PxrTrackingOrigin origin, out PxrPosef pose)
            {
                pose = new PxrPosef();
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetAnchorPose(anchorHandle,origin, out pose);
#else
                return PxrResult.TIMEOUT_EXPIRED;
#endif
            }

            public static PxrResult UPxr_GetAnchorEntityUuid(ulong anchorHandle, out PxrUuid uuid)
            {
                uuid = new PxrUuid();
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetAnchorEntityUuid(anchorHandle, out uuid);
#else
                return PxrResult.TIMEOUT_EXPIRED;
#endif
            }

            public static PxrResult UPxr_GetAnchorComponentFlags(ulong anchorHandle, out ulong flag)
            {
                flag = UInt64.MinValue;
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetAnchorComponentFlags(anchorHandle, out flag);
#else
                return PxrResult.TIMEOUT_EXPIRED;
#endif
            }

            public static PxrResult UPxr_LoadAnchorEntity(ref PxrAnchorEntityLoadInfo info, out ulong taskId)
            {
                taskId = ulong.MinValue;
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_LoadAnchorEntity(ref info, out taskId);
#else
                return PxrResult.TIMEOUT_EXPIRED;
#endif
            }

            public static PxrResult UPxr_GetAnchorEntityLoadResults(ulong taskId, ref PxrAnchorEntityLoadResults result)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetAnchorEntityLoadResults(taskId, ref result);
#else
                return PxrResult.TIMEOUT_EXPIRED;
#endif
            }

            public static PxrResult UPxr_StartSpatialSceneCapture(out ulong taskId)
            {
                taskId = ulong.MinValue;
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_StartSpatialSceneCapture(out taskId);
#else
                return PxrResult.TIMEOUT_EXPIRED;
#endif
            }

            public static PxrResult UPxr_GetAnchorVolumeInfo(ulong anchorHandle, ref PxrAnchorVolumeInfo info)
            {

#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetAnchorBoxInfo(anchorHandle, ref info);
#else
                return PxrResult.TIMEOUT_EXPIRED;
#endif
            }

            public static PxrResult UPxr_GetAnchorPlanePolygonInfo(ulong anchorHandle, ref PxrAnchorPlanePolygonInfo info)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetAnchorPlanePolygonInfo(anchorHandle, ref info);
#else
                return PxrResult.TIMEOUT_EXPIRED;
#endif
            }

            public static PxrResult UPxr_GetAnchorPlaneBoundaryInfo(ulong anchorHandle, ref PxrAnchorPlaneBoundaryInfo info)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetAnchorPlaneBoundaryInfo(anchorHandle, ref info);
#else
                return PxrResult.TIMEOUT_EXPIRED;
#endif
            }

            public static PxrResult UPxr_GetAnchorSceneLabel(ulong anchorHandle, out PxrSceneLabel label)
            {
                label = PxrSceneLabel.UnKnown;
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetAnchorSceneLabel(anchorHandle, out label);
#else
                return PxrResult.TIMEOUT_EXPIRED;
#endif
            }

            #region Deprecate
            public static int UPxr_StartHumanOcclusion()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_StartHumanOcclusion();
#else
                return -1;
#endif
            }
            public static int UPxr_AcquireMeshingInfo(IntPtr maskBuffer, IntPtr verticesBuffer, out uint verticesSize, IntPtr facetsBuffer, out uint facetSize, out bool haveRead)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_AcquireMeshingInfo(maskBuffer,verticesBuffer, out verticesSize, facetsBuffer, out facetSize, out haveRead);
#else
                verticesSize = 0;
                facetSize = 0;
                haveRead = false;
                return -1;
#endif
            }
            public static int UPxr_StopHumanOcclusion()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_StopHumanOcclusion();
#else
                return -1;
#endif
            }

            public static int UPxr_CreateSpatialAnchor(ref PxrSpatialAnchorCreateInfo info, ref UInt64 handle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_CreateSpatialAnchor(ref info,ref handle);
#else
                return -1;
#endif
            }

            public static int UPxr_DestroySpatialAnchor(UInt64 handle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_DestroySpatialAnchor(handle);
#else
                return -1;
#endif
            }

            public static int UPxr_SaveSpatialAnchor(ref PxrSpatialAnchorSaveInfo info, ref UInt64 requestId)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_SaveSpatialAnchor(ref info,ref requestId);
#else
                return -1;
#endif
            }

            public static int UPxr_DeleteSpatialAnchor(ref PxrSpatialAnchorDeleteInfo info, ref UInt64 requestId)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_DeleteSpatialAnchor(ref info,ref requestId);
#else
                return -1;
#endif
            }

            public static int UPxr_LoadSpatialAnchorById(ref PxrSpatialInstanceLoadByIdInfo info, ref UInt64 requestId)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_LoadSpatialAnchorById(ref info,ref requestId);
#else
                return -1;
#endif
            }

            public static int UPxr_GetSpatialAnchorLoadResults(UInt64 requestId, ref PxrSpatialAnchorLoadResults results)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetSpatialAnchorLoadResults(requestId,ref results);
#else
                return -1;
#endif
            }

            public static int UPxr_GetSpatialAnchorPose(UInt64 handle, double predictDisplayTime, PxrReferenceType type, ref PxrPosef pose)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetSpatialAnchorPose(handle, predictDisplayTime, type, ref pose);
#else
                return -1;
#endif
            }

            public static int UPxr_GetSpatialAnchorUuid(UInt64 handle, ref PxrSpatialInstanceUuid uuid)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetSpatialAnchorUuid(handle, ref uuid);
#else
                return -1;
#endif
            }

            public static int UPxr_CreateRoomSceneData(PxrSpatialInstanceUuid anchorUuid, IntPtr roomSceneData, int dataLen, ref ulong roomSceneDataHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_CreateRoomSceneData(anchorUuid, roomSceneData, dataLen, ref roomSceneDataHandle);
#else
                return -1;
#endif
            }

            public static int UPxr_DestroyRoomSceneData(ulong roomSceneDataHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_DestroyRoomSceneData(roomSceneDataHandle);
#else
                return -1;
#endif
            }

            public static int UPxr_SaveRoomSceneData(ref PxrRoomSceneDataSaveInfo saveInfo, ref ulong requestId)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_SaveRoomSceneData(ref saveInfo, ref requestId);
#else
                return -1;
#endif
            }

            public static int UPxr_DeleteRoomSceneData(ref PxrRoomSceneDataDeleteInfo deleteInfo, ref ulong requestId)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_DeleteRoomSceneData(ref deleteInfo, ref requestId);
#else
                return -1;
#endif
            }

            public static int UPxr_LoadRoomScene(ref PxrRoomSceneLoadInfo loadInfo, ref ulong requestId)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_LoadRoomScene(ref loadInfo, ref requestId);
#else
                return -1;
#endif
            }

            public static int UPxr_GetRoomSceneLoadResults(ulong requestId, ref PxrRoomSceneLoadResults results)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetRoomSceneLoadResults(requestId, ref results);
#else
                return -1;
#endif
            }

            public static int UPxr_StartRoomCapture()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_StartRoomCapture();
#else
                return -1;
#endif
            }

            #endregion

            private const int MAX_EVENT = 20;
            private static IntPtr[] eventArrayHandle = new IntPtr[MAX_EVENT];
            public static bool UPxr_PollEventQueue(ref List<PxrEventDataBuffer> bufferList)
            {
                bool ret = false;
#if UNITY_ANDROID && !UNITY_EDITOR
                int eventNum = 0;
                ret = Pxr_PollEventFromXRPlugin(ref eventNum, eventArrayHandle);
                if (ret)
                {
                    for (int i = 0; i < eventNum; i++)
                    {
                        PxrEventDataBuffer buffer = (PxrEventDataBuffer)Marshal.PtrToStructure(eventArrayHandle[i], typeof(PxrEventDataBuffer));
                        bufferList.Add(buffer);
                    }
                }
#endif
                return ret;
            }

        }
    }
}