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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.XR.PXR.SecureMR;
using UnityEngine;
using UnityEngine.XR;
using Point = Unity.XR.PXR.SecureMR.Point;
using Random = System.Random;

namespace Unity.XR.PXR
{
    //MR
    #region MR

    #region SecureMR

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMRPipelineExecuteParameter
    {
        public XrStructureType type;
        public IntPtr next;
        public ulong pipelineRunToBeWaited;
        public ulong conditionTensor;
        public uint pairCount;
        public IntPtr pipelineIOPair; //SecureMrPipelineIOPair[]
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMRPipelineIOPair
    {
        public XrStructureType type;
        public IntPtr next;
        public ulong localPlaceHolderTensor;
        public ulong globalTensor;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMRTensorBuffer
    {
        public XrStructureType type;
        public IntPtr next;
        public uint bufferSize;
        public IntPtr buffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMRTensorCreateInfoShape
    {
        public XrStructureType type;
        public IntPtr next;
        public bool placeHolder;
        public uint dimensionsCount;
        public IntPtr dimensions;
        public IntPtr format; // XrSecureMrTensorFormat
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMRTensorCreateInfoGltf
    {
        public XrStructureType type;
        public IntPtr next;
        public bool placeHolder;
        public uint bufferSize;
        public IntPtr buffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMRTensorFormat
    {
        public SecureMRTensorDataType dataType;
        public sbyte channel;
        public SecureMRTensorUsage tensorUsage;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorArithmeticCompose
    {
        public XrStructureType type;
        public IntPtr next;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2048)]
        public byte[] configText;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorComparison
    {
        public XrStructureType type;
        public IntPtr next;
        public SecureMRComparison comparison;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorNonMaximumSuppression
    {
        public XrStructureType type;
        public IntPtr next;
        public float threshold;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorNormalize
    {
        public XrStructureType type;
        public IntPtr next;
        public SecureMRNormalizeType normalizeType;
    }

    /// <summary>
    /// convert:https://docs.opencv.org/3.4/d8/d01/group__imgproc__color__conversions.html
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorColorConvert
    {
        public XrStructureType type;
        public IntPtr next;
        public int convert;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorSortMatrix
    {
        public XrStructureType type;
        public IntPtr next;
        public SecureMRMatrixSortType sortType;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorUpdateGltf
    {
        public XrStructureType type;
        public IntPtr next;
        public SecureMRGltfOperatorAttribute attribute;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorRenderText
    {
        public XrStructureType type;
        public IntPtr next;
        public SecureMRFontTypeface typeFace;
        public string languageAndLocale;
        public int width;
        public int height;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorModel
    {
        public XrStructureType type;
        public IntPtr next;
        public uint modelInputCount;
        public IntPtr modelInputs;//SecureMrOperatorIOMap[]
        public uint modelOutputCount;
        public IntPtr modelOutputs;//SecureMrOperatorIOMap[]
        public uint bufferSize;
        public IntPtr buffer;
        public SecureMRModelType modelType;
        public string modelName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecureMROperatorIOMap
    {
        public XrStructureType type;
        public IntPtr next;
        public SecureMRModelEncoding encodingType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] nodeName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] operatorIOName;
    }

    public struct SecureMROperatorModelConfig
    {
        public SecureMRModelEncoding encodingType;
        public string nodeName;
        public string operatorIOName;
    }

    public enum SecureMRModelEncoding
    {
        Float32 = 1,
        UInt8 = 2,
        Int8 = 3,
        UInt16 = 4,
        Int32 = 5,
    }

    public enum SecureMRModelType
    {
        QnnContextBinary = 1,
    }

    public enum SecureMRFontTypeface
    {
        Default = 1,
        SansSerif = 2,
        Serif = 3,
        Monospace = 4,
        Bold = 5,
        Italic = 6,
    }

    public enum SecureMRGltfOperatorAttribute
    {
        Texture = 1,
        Animation = 2,
        WorldPose = 3,
        LocalTransform = 4,
        MaterialMetallicFactor = 5,
        MaterialRoughnessFactor = 6,
        MaterialOcclusionMapTexture = 7,
        MaterialBaseColorFactor = 8,
        MaterialEmissiveFactor = 9,
        MaterialEmissiveStrength = 10,
        MaterialEmissiveTexture = 11,
        MaterialBaseColorTexture = 12,
        MaterialNormalMapTexture = 13,
        MaterialMetallicRoughnessTexture = 14,
    }

    public enum SecureMRMatrixSortType
    {
        Column = 1,
        Row = 2,
    }

    public enum SecureMRNormalizeType
    {
        L1 = 1,
        L2 = 2,
        Inf = 3,
        MinMax = 4,
    }

    public enum SecureMRTensorDataType
    {
        Unknown = -1,
        Byte = 1,
        Sbyte,
        Ushort,
        Short,
        Int,
        Float,
        Double,
    }

    public enum SecureMRTensorUsage
    {
        Unknown = -1,
        Point = 1,
        Scalar,
        Slice,
        Color,
        TimeStamp,
        Matrix
    }

    public enum SecureMRComparison
    {
        Unknown = 0,
        LargerThan = 1,
        SmallerThan = 2,
        SmallerOrEqual = 3,
        LargerOrEqual = 4,
        EqualTo = 5,
        NotEqual = 6,
    }

    public enum SecureMROperatorType
    {
        Unknown = 0,
        ArithmeticCompose = 1,
        ElementwiseMin = 4,
        ElementwiseMax = 5,
        ElementwiseMultiply = 6,
        CustomizedCompare = 7,
        ElementwiseOr = 8,
        ElementwiseAnd = 9,
        All = 10,
        Any = 11,
        Nms = 12,
        SolvePnP = 13,
        GetAffine = 14,
        ApplyAffine = 15,
        ApplyAffinePoint = 16,
        UvTo3DInCameraSpace = 17,
        Assignment = 18,
        RunModelInference = 19,
        Normalize = 21,
        CameraSpaceToWorld = 22,
        RectifiedVstAccess = 23,
        Argmax = 24,
        ConvertColor = 25,
        SortVector = 26,
        Inversion = 27,
        GetTransformMatrix = 28,
        SortMatrix = 29,
        SwitchGltfRenderStatus = 30,
        UpdateGltf = 31,
        RenderText = 32,
        LoadTexture = 33,
    }

    #endregion

    #region MR3.0
    
    public enum PxrVstStatus
    {
        Disabled = 0,
        Enabling,
        Enabled,
        Disabling,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrEventSenseDataProviderStateChanged
    {
        public ulong providerHandle;
        public PxrSenseDataProviderState newState;
    }

    public struct PxrEventAutoRoomCaptureUpdated
    {
        public PxrSpatialSceneCaptureStatus state; 
        public uint msg;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrFuturePollInfo
    {
        public PxrStructureType type;
        public ulong future;
    }

    public enum PxrFutureState
    {
        Pending = 1,
        Ready = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrFuturePollResult
    {
        public PxrStructureType type;
        public PxrFutureState state;
    }

    public enum PxrSpatialMapSizeLimitedReason
    {
        MapSizeLimitedUnknown = 0,
        MapQuantitySizeLimited,
        SingleMapSizeLimited,
        TotalMapSizeLimited,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSenseDataProviderStartCompletion
    {
        public PxrStructureType type;
        public PxrResult futureResult;
    }

    /// <summary>
    /// The state of sense data provider.
    /// </summary>
    public enum PxrSenseDataProviderState
    {
        /// <summary>
        /// The data provider has been initialized.
        /// </summary>
        Initialized,
        /// <summary>
        /// The data provider is running normally.
        /// </summary>
        Running,
        /// <summary>
        /// The data provider has been stopped.
        /// </summary>
        Stopped
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSenseDataQueryCompletion
    {
        public PxrStructureType type;
        public PxrResult futureResult;
        public ulong snapshotHandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSenseDataQueryInfo
    {
        public PxrStructureType type;
        public IntPtr filter; //PxrSenseDataQueryFilterBaseHeader
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrQueriedSenseData
    {
        public PxrStructureType type;
        public uint queriedSpatialEntityCapacityInput;
        public uint queriedSpatialEntityCountOutput;
        public IntPtr queriedSpatialEntities;//PxrQueriedSpatialEntityInfo[]
    }
    

    /// <summary>
    /// The component types of scene anchors.
    /// </summary>
    public enum PxrSceneComponentType
    {
        Location = 0,
        /// <summary>
        /// Semantic label.
        /// </summary>
        Semantic,
        /// <summary>
        /// The scene anchor is associated with a 2D box object.
        /// </summary>
        Box2D,
        /// <summary>
        /// The scene anchor is associated with a polygon object.
        /// </summary>
        Polygon,
        /// <summary>
        /// The scene anchor is associated with a 3D box object.
        /// </summary>
        Box3D,
        TriangleMesh = 5,
    }
    
    
    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSpatialEntityLocationGetInfo
    {
        public PxrStructureType type;
        public ulong entity;
        public PxrSceneComponentType componentType;
        public ulong baseSpace;
        public ulong time;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSpatialEntityLocationInfo
    {
        public PxrStructureType type;
        public ulong locationFlags;
        public PxrPosef pose;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSpatialEntitySemanticInfo
    {
        public PxrStructureType type;
        public uint semanticCapacityInput;
        public uint semanticCountOutput;
        public IntPtr semanticLabels;//PxrSemanticLabel[]
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSceneBox3DInfo
    {
        public PxrStructureType type;
        public PxrBoxf box3D;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSceneBox2DInfo
    {
        public PxrStructureType type;
        public PxrSceneBox2D box2D;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrScenePolygonInfo
    {
        public PxrStructureType type;
        public uint polygonCapacityInput;
        public uint polygonCountOutput;
        public IntPtr vertices; //PxrVector2f[]
    }

    public struct PxrSpatialEntityAnchorRetrieveInfo
    {
        public PxrStructureType type;
        public ulong spatialEntity;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrAnchorLocateInfo
    {
        public PxrStructureType type;
        public PxrTrackingOrigin baseSpace;
        public ulong time;
        public ulong anchorHandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSpaceLocation
    {
        public PxrStructureType type;
        public ulong locationFlags; //PxrSpaceLocationFlags
        public PxrPosef pose;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSpatialAnchorCreateInfo
    {
        public PxrStructureType type;
        public PxrPosef pose;
        public PxrTrackingOrigin baseSpace;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSpatialAnchorCreateCompletion
    {
        public PxrStructureType type;
        public PxrResult futureResult;
        public ulong anchorHandle;
        public PxrUuid uuid;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSpatialAnchorPersistInfo
    {
        public PxrStructureType type;
        public PxrPersistenceLocation location;
        public ulong anchorHandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSpatialAnchorPersistCompletion
    {
        public PxrStructureType type;
        public PxrResult futureResult;
        public ulong anchorHandle;
        public PxrUuid uuid;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSpatialAnchorUnpersistInfo
    {
        public PxrStructureType type;
        public PxrPersistenceLocation location;
        public ulong anchorHandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSpatialAnchorUnpersistCompletion
    {
        public PxrStructureType type;
        public PxrResult futureResult;
        public ulong anchorHandle;
        public PxrUuid uuid;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSceneCaptureStartCompletion
    {
        public PxrStructureType type;
        public PxrResult futureResult;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSpatialAnchorShareInfo
    {
        public PxrStructureType type;
        public ulong anchorHandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSpatialAnchorShareCompletion
    {
        public PxrStructureType type;
        public PxrResult futureResult;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSharedSpatialAnchorDownloadInfo
    {
        public PxrStructureType type;
        public PxrUuid uuid;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSpatialEntityComponentInfoBaseHeader
    {
        public PxrStructureType type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSharedSpatialAnchorDownloadCompletion
    {
        public PxrStructureType type;
        public PxrResult futureResult;
    }
    

    /// <summary>
    /// The types of sense data provider.
    /// </summary>
    public enum PxrSenseDataProviderType
    {
        /// <summary>
        /// Spatial anchor data provider.
        /// </summary>
        SpatialAnchor,
        /// <summary>
        /// Scene capture data provider.
        /// </summary>
        SceneCapture,
    }


    /// <summary>
    /// The semantic labels of scene anchors.
    /// </summary>
    public enum PxrSemanticLabel
    {
        Unknown = 0,
        /// <summary>
        /// A floor.
        /// </summary>
        Floor,
        /// <summary>
        /// A ceiling.
        /// </summary>
        Ceiling,
        /// <summary>
        /// A wall in the real-world scene. Doors and windows must exist within walls.
        /// </summary>
        Wall,
        /// <summary>
        /// A door, which must exist within a wall.
        /// </summary>
        Door,
        /// <summary>
        /// A window, which must exist within a wall.
        /// </summary>
        Window,
        Opening,
        /// <summary>
        /// A table.
        /// </summary>
        Table,
        /// <summary>
        /// A sofa.
        /// </summary>
        Sofa,
        /// <summary>
        /// A chair.
        /// </summary>
        Chair,
        Human = 10,
        Curtain = 13,
        Cabinet,
        Bed,
        Plant,
        Screen,
        /// <summary>
        /// Virtual walls are generated when scene capture is automatically closed. They are not associated with real-world walls, and you can not draw doors or windows on them.
        /// </summary>
        VirtualWall = 18,
        Refrigerator,
        WashingMachine,
        AirConditioner,
        Lamp,
        WallArt = 23,
        Stairway
    }

    public enum PxrMeshLod
    {
        Low,
        Medium,
        High
    }
    public enum HandTrackingSupport
    {
        ControllersAndHands,
        HandsOnly
    }
    [System.Flags]
    public enum PxrMeshConfigFlags : ulong
    {
        Semantic = 0x00000001,
        SemanticAlignWithVertex= 0x00000002
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct PxrQuerySenseDataUuidFilter
    {
        public PxrStructureType type;
        public uint uuidCount;
        public IntPtr uuidList; //=>PxrUuid[]
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrQuerySenseDataSemanticFilter
    {
        public PxrStructureType type;
        public uint semanticCount;
        public IntPtr semantics; //=>PxrSemanticLabel[]
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrQueriedSenseDataGetInfo
    {
        public PxrStructureType type;
        public ulong snapshotHandle;
    }

    /// <summary>
    /// Information about the 3D box oject.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSceneBox3D
    {
        /// <summary>
        /// The position of the object.
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// The rotation of the object.
        /// </summary>
        public Quaternion rotation;
        /// <summary>
        /// The length, width, and height of the object.
        /// </summary>
        public Vector3 extent;
    }

    /// <summary>
    /// Informatiom about the 2D box object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSceneBox2D
    {
        /// <summary>
        /// The offset of the 2D box object relative to the center of the scene anchor.
        /// </summary>
        public Vector2 offset;
        /// <summary>
        /// The length and width of the 2D box object.
        /// </summary>
        public XrExtent2Df extent;
    }


    /// <summary>
    /// Information about the polygon object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PxrScenePolygon
    {
        /// <summary>
        /// The array of vertices of the polygon object.
        /// </summary>
        public Vector2[] vertices;
    }

    public struct PxrSceneComponentData
    {
        public Guid uuid;
        public Vector3 position;
        public Quaternion rotation;
        public PxrSemanticLabel label;
        public PxrSceneComponentType[] types;
        public PxrSceneBox3D box3D;
        public PxrSceneBox2D box2D;
        public PxrScenePolygon polygon;
    }

    public struct PxrUuid
    {
        public ulong value0;
        public ulong value1;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct XrSpatialAnchorCompletion
    {
        public XrStructureType type;
        public IntPtr next;
        public PxrResult futureResult;
        public ulong anchorHandle;
        public PxrUuid uuid;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrSpatialMeshInfo
    {
        public Guid uuid;
        public MeshChangeState state;
        public Vector3 position;
        public Quaternion rotation;
        public ushort[] indices;
        public Vector3[] vertices;
        public PxrSemanticLabel[] labels;
    }

    #endregion

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
        NewCaptureResult = 1,
        OutOfCaptureZone = 2, 
        ErrorMessage = 3,
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


    
    [StructLayout(LayoutKind.Sequential)]
    public struct PxrEventDataBuffer
    {
        public PxrStructureType type;
        public PxrEventLevel eventLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 500)]
        public byte[] data;
    };
 
    [StructLayout(LayoutKind.Sequential)]
    public struct XrEventDataBuffer
    {
        public XrStructureType type;
        public IntPtr next;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4000)]
        public byte[] data;
    };
    public struct PxrEventSemiAutoCandidatesUpdate
    {
        public uint state;
        public uint count;
    }

    public struct PxrPoint3D
    {
        public float x;
        public float y;
        public float z;
    }

    public enum PxrStructureType
    {
        Unknown = 0,
        SessionStateChanged = 2,
        Controller = 6,
        SeethroughStateChanged = 9,
        RefreshRateChanged = 17,
        SDKLoglevelChanged = 35,
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
        AutoRoomCaptureUpdated = 131,
        SpatialMapSizeLimited = 142,
        MotionTrackerKeyEvent = 201,
        EXTDevConnectStateEvent = 202,              
        EXTDevBatteryStateEvent = 203,
        MotionTrackingModeChangedEvent = 204,
        EXTDevPassDataEvent = 205,
		//mr sdk 3.0

        SpatialMeshProviderCreateInfo = 221,
        SpatialAnchorProviderCreateInfo = 222,
        SceneCaptureProviderCreateInfo = 224,
        SenseDataProviderStartCompletion = 225,
        SpatialEntitySemanticFilter = 226,
        AnchorEntityUuidFilters = 227,
        SpatialEntityLocationGetInfo = 228,
        SpatialEntitySemanticGetInfo = 229,
        SpatialEntityBoundingBox2DGetInfo = 230,
        SpatialEntityBoundingBox3DGetInfo = 231,
        SpatialEntityPolygonGetInfo = 232,
        SpatialEntityTriangleMeshGetInfo = 233,
        SpatialEntityLocationInfo = 234,
        SpatialEntitySemanticInfo = 235,
        SpatialEntityBoundingBox2DInfo = 236,
        SpatialEntityBoundingBox3DInfo = 237,
        SpatialEntityPolygonInfo = 238,
        SpatialEntityTriangleMeshInfo = 239,
        SpatialAnchorShareInfo = 240,
        SpatialAnchorShareCompletion = 241,
        SpatialAnchorDownloadInfo = 242,
        SpatialAnchorDownloadCompletion = 243,
        SpatialEntityAnchorRetrieveInfo = 244,
        AnchorLocationInfo = 245,
        SpatialAnchorCreateInfo = 246,
        SpatialAnchorCreateCompletion = 247,
        SpatialAnchorPersistInfo = 248,
        SpatialAnchorPersistCompletion = 249,
        SpatialAnchorUnPersistInfo = 250,
        SpatialAnchorUnPersistCompletion = 251,
        SenseDataQueryInfo = 254,
        SenseDataQueryCompletion = 255,
        QueriedSenseData = 256,
        QueriedSenseDataGetInfo = 257,
        SenseDataProviderStartInfo = 258,
        SceneCaptureStartSceneCaptureCompletion = 262,
        AutoSceneCaptureResultInfo = 263,
        AutoSceneCaptureResultGetInfo = 264,
        SemiAutoSceneCaptureResultInfo = 265,
        SemiAutoSceneCaptureResultGetInfo = 266,
        SpatialEntityComponentSetInfo = 267,   //以下都是set相关
        SpatialEntitySemanticComponentInfo = 268,
        SpatialEntityBoundingBox2DComponentInfo = 269,
        SpatialEntityPolygonComponentInfo = 270,
        SpatialEntityBoundingBox3DComponentInfo = 271,
        UnPersistAnchorByUuidCompletion = 272,
        DataSpatialMapMemLimited = 273,
        FuturePollInfo = 300,
        FuturePollResult = 301,
        SenseDataUpdated = 400,
        SenseDataProviderStateChanged = 401,

        SpaceLocation = 1000,
        SpaceVelocity = 1001,
    }

    public enum PxrDeviceEventType
    {
        CONNECTCHANGED = 0,
        MAIN_CHANGED = 1,
        VERSION = 2,
        SN = 3,
        BIND_STATUS = 4,
        PXR_STATION_STATUS = 5,
        IOBUSY = 6,
        OTASTAUS = 7,
        ID = 8,
        OTASATAION_PROGRESS = 9,
        OTASATAION_CODE = 10,
        OTACONTROLLER_PROGRESS = 11,
        OTACONTROLLER_CODE = 12,
        OTA_SUCCESS = 13,
        BLEMAC = 14,
        HANDNESS_CHANGED = 15,
        CHANNEL = 16,
        LOSSRATE = 17,
        THREAD_STARTED = 18,
        MENUPRESSED_STATE = 19,
        HANDTRACKING_SETTING = 20,
        INPUTDEVICE_CHANGED = 21,
        SYSTEMGESTURE_STATE = 22,
        MOTION_TRACKER_STATE = 23,
        MOTION_TRACKER_BATTERY = 24,
        BODYTRACKING_STATE_ERROR_CODE = 25,
        BODYTRACKING_ACTION = 26
    };
    
    /// <summary>
    /// The result of mixed reality-realted events.
    /// </summary>
    public enum PxrResult
    {
        Unknown = int.MaxValue, 
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
        ERROR_INSTANCE_LOST = -13,
        ERROR_SESSION_RUNNING = -14,
        ERROR_SESSION_NOT_RUNNING = -16,
        ERROR_SESSION_LOST = -17,
        ERROR_SYSTEM_INVALID = -18,
        ERROR_PATH_INVALID = -19,
        ERROR_PATH_COUNT_EXCEEDED = -20,
        ERROR_PATH_FORMAT_INVALID = -21,
        ERROR_PATH_UNSUPPORTED = -22,
        ERROR_LAYER_INVALID = -23,
        ERROR_LAYER_LIMIT_EXCEEDED = -24,
        ERROR_SWAPCHAIN_RECT_INVALID = -25,
        ERROR_SWAPCHAIN_FORMAT_UNSUPPORTED = -26,
        ERROR_ACTION_TYPE_MISMATCH = -27,
        ERROR_SESSION_NOT_READY = -28,
        ERROR_SESSION_NOT_STOPPING = -29,
        ERROR_TIME_INVALID = -30,
        ERROR_REFERENCE_SPACE_UNSUPPORTED = -31,
        ERROR_FILE_ACCESS_ERROR = -32,
        ERROR_FILE_CONTENTS_INVALID = -33,
        ERROR_FORM_FACTOR_UNSUPPORTED = -34,
        ERROR_FORM_FACTOR_UNAVAILABLE = -35,
        ERROR_API_LAYER_NOT_PRESENT = -36,
        ERROR_CALL_ORDER_INVALID = -37,
        ERROR_GRAPHICS_DEVICE_INVALID = -38,
        ERROR_POSE_INVALID = -39,
        ERROR_INDEX_OUT_OF_RANGE = -40,
        ERROR_VIEW_CONFIGURATION_TYPE_UNSUPPORTED = -41,
        ERROR_ENVIRONMENT_BLEND_MODE_UNSUPPORTED = -42,
        ERROR_NAME_DUPLICATED = -44,
        ERROR_NAME_INVALID = -45,
        ERROR_ACTIONSET_NOT_ATTACHED = -46,
        ERROR_ACTIONSETS_ALREADY_ATTACHED = -47,
        ERROR_LOCALIZED_NAME_DUPLICATED = -48,
        ERROR_LOCALIZED_NAME_INVALID = -49,
        ERROR_GRAPHICS_REQUIREMENTS_CALL_MISSING = -50,
        ERROR_RUNTIME_UNAVAILABLE = -51,

        ERROR_PERMISSION_INSUFFICIENT = -1000710000,
        ERROR_EXTENSION_DEPENDENCY_NOT_ENABLED = -1000710001,
        ERROR_ANDROID_THREAD_SETTINGS_ID_INVALID_KHR = -1000003000,
        ERROR_ANDROID_THREAD_SETTINGS_FAILURE_KHR = -1000003001,
        ERROR_CREATE_SPATIAL_ANCHOR_FAILED_MSFT = -1000039001,
        ERROR_SECONDARY_VIEW_CONFIGURATION_TYPE_NOT_ENABLED_MSFT = -1000053000,
        ERROR_CONTROLLER_MODEL_KEY_INVALID_MSFT = -1000055000,
        ERROR_REPROJECTION_MODE_UNSUPPORTED_MSFT = -1000066000,
        ERROR_COMPUTE_NEW_SCENE_NOT_COMPLETED_MSFT = -1000097000,
        ERROR_SCENE_COMPONENT_ID_INVALID_MSFT = -1000097001,
        ERROR_SCENE_COMPONENT_TYPE_MISMATCH_MSFT = -1000097002,
        ERROR_SCENE_MESH_BUFFER_ID_INVALID_MSFT = -1000097003,
        ERROR_SCENE_COMPUTE_FEATURE_INCOMPATIBLE_MSFT = -1000097004,
        ERROR_SCENE_COMPUTE_CONSISTENCY_MISMATCH_MSFT = -1000097005,
        ERROR_DISPLAY_REFRESH_RATE_UNSUPPORTED_FB = -1000101000,
        ERROR_COLOR_SPACE_UNSUPPORTED_FB = -1000108000,
        ERROR_SPACE_COMPONENT_NOT_SUPPORTED_FB = -1000113000,
        ERROR_SPACE_COMPONENT_NOT_ENABLED_FB = -1000113001,
        ERROR_SPACE_COMPONENT_STATUS_PENDING_FB = -1000113002,
        ERROR_SPACE_COMPONENT_STATUS_ALREADY_SET_FB = -1000113003,
        ERROR_UNEXPECTED_STATE_PASSTHROUGH_FB = -1000118000,
        ERROR_FEATURE_ALREADY_CREATED_PASSTHROUGH_FB = -1000118001,
        ERROR_FEATURE_REQUIRED_PASSTHROUGH_FB = -1000118002,
        ERROR_NOT_PERMITTED_PASSTHROUGH_FB = -1000118003,
        ERROR_INSUFFICIENT_RESOURCES_PASSTHROUGH_FB = -1000118004,
        ERROR_UNKNOWN_PASSTHROUGH_FB = -1000118050,
        ERROR_RENDER_MODEL_KEY_INVALID_FB = -1000119000,
        RENDER_MODEL_UNAVAILABLE_FB = 1000119020,
        ERROR_MARKER_NOT_TRACKED_VARJO = -1000124000,
        ERROR_MARKER_ID_INVALID_VARJO = -1000124001,
        ERROR_MARKER_DETECTOR_PERMISSION_DENIED_ML = -1000138000,
        ERROR_MARKER_DETECTOR_LOCATE_FAILED_ML = -1000138001,
        ERROR_MARKER_DETECTOR_INVALID_DATA_QUERY_ML = -1000138002,
        ERROR_MARKER_DETECTOR_INVALID_CREATE_INFO_ML = -1000138003,
        ERROR_MARKER_INVALID_ML = -1000138004,
        ERROR_LOCALIZATION_MAP_INCOMPATIBLE_ML = -1000139000,
        ERROR_LOCALIZATION_MAP_UNAVAILABLE_ML = -1000139001,
        ERROR_LOCALIZATION_MAP_FAIL_ML = -1000139002,
        ERROR_LOCALIZATION_MAP_IMPORT_EXPORT_PERMISSION_DENIED_ML = -1000139003,
        ERROR_LOCALIZATION_MAP_PERMISSION_DENIED_ML = -1000139004,
        ERROR_LOCALIZATION_MAP_ALREADY_EXISTS_ML = -1000139005,
        ERROR_LOCALIZATION_MAP_CANNOT_EXPORT_CLOUD_MAP_ML = -1000139006,
        ERROR_SPATIAL_ANCHORS_PERMISSION_DENIED_ML = -1000140000,
        ERROR_SPATIAL_ANCHORS_NOT_LOCALIZED_ML = -1000140001,
        ERROR_SPATIAL_ANCHORS_OUT_OF_MAP_BOUNDS_ML = -1000140002,
        ERROR_SPATIAL_ANCHORS_SPACE_NOT_LOCATABLE_ML = -1000140003,
        ERROR_SPATIAL_ANCHORS_ANCHOR_NOT_FOUND_ML = -1000141000,
        ERROR_SPATIAL_ANCHOR_NAME_NOT_FOUND_MSFT = -1000142001,
        ERROR_SPATIAL_ANCHOR_NAME_INVALID_MSFT = -1000142002,
        SCENE_MARKER_DATA_NOT_STRING_MSFT = 1000147000,
        ERROR_SPACE_MAPPING_INSUFFICIENT_FB = -1000169000,
        ERROR_SPACE_LOCALIZATION_FAILED_FB = -1000169001,
        ERROR_SPACE_NETWORK_TIMEOUT_FB = -1000169002,
        ERROR_SPACE_NETWORK_REQUEST_FAILED_FB = -1000169003,
        ERROR_SPACE_CLOUD_STORAGE_DISABLED_FB = -1000169004,
        ERROR_PASSTHROUGH_COLOR_LUT_BUFFER_SIZE_MISMATCH_META = -1000266000,
        ENVIRONMENT_DEPTH_NOT_AVAILABLE_META = 1000291000,
        ERROR_HINT_ALREADY_SET_QCOM = -1000306000,
        ERROR_NOT_AN_ANCHOR_HTC = -1000319000,
        ERROR_SPACE_NOT_LOCATABLE_EXT = -1000429000,
        ERROR_PLANE_DETECTION_PERMISSION_DENIED_EXT = -1000429001,
        ERROR_FUTURE_PENDING_EXT = -1000469001,
        ERROR_FUTURE_INVALID_EXT = -1000469002,
        ERROR_SYSTEM_NOTIFICATION_PERMISSION_DENIED_ML = -1000473000,
        ERROR_SYSTEM_NOTIFICATION_INCOMPATIBLE_SKU_ML = -1000473001,
        ERROR_WORLD_MESH_DETECTOR_PERMISSION_DENIED_ML = -1000474000,
        ERROR_WORLD_MESH_DETECTOR_SPACE_NOT_LOCATABLE_ML = -1000474001,
        ERROR_FACIAL_EXPRESSION_PERMISSION_DENIED_ML = 1000482000,
        ERROR_COLOCATION_DISCOVERY_NETWORK_FAILED_META = -1000571001,
        ERROR_COLOCATION_DISCOVERY_NO_DISCOVERY_METHOD_META = -1000571002,
        COLOCATION_DISCOVERY_ALREADY_ADVERTISING_META = 1000571003,
        COLOCATION_DISCOVERY_ALREADY_DISCOVERING_META = 1000571004,
        ERROR_SPACE_GROUP_NOT_FOUND_META = -1000572002,
        ERROR_VIRTUAL_BOUNDARY_TRIGGER_NODE_TYPE_UNSUPPORTED_PICO = -1010001000,
        ERROR_MOTION_TRACKER_TYPE_MISMATCH_PICO = -1010002000,
        ERROR_MOTION_TRACKER_COUNT_EXCEEDED_PICO = -1010002001,
        ERROR_MOTION_TRACKING_MODE_MISMATCH_PICO = -1010002002,
        ERROR_MOTION_TRACKER_ID_INVALID_PICO = -1010002003,
        ERROR_SECURE_MR_UNREGISTERED_HANDLE_PICO = -1010007000,
        ERROR_SECURE_MR_INVALID_PARAM_PICO = -1010007001,
        ERROR_SECURE_MR_PIPELINE_LOCKED_PICO = -1010007002,
        ERROR_SECURE_MR_USAGE_BEFORE_INIT_PICO = -1010007003,
        ERROR_SECURE_MR_OTHER_INTERNAL_ERROR_PICO = -1010007004,
        ERROR_EXPAND_TRACKER_ID_INVALID_PICO = -1010008000,

        ERROR_SPACE_LOCATING = -1200390101,
        ERROR_SPATIAL_SENSING_SERVICE_UNAVAILABLE_PICO = -1200389027,

        ERROR_ANCHOR_SHARING_NETWORK_TIMEOUT = -1200391101,
        ERROR_ANCHOR_SHARING_AUTHENTICATION_FAILURE = -1200391102,
        ERROR_ANCHOR_SHARING_NETWORK_FAILURE = -1200391103,
        ERROR_ANCHOR_SHARING_LOCALIZATION_FAIL = -1200391104,
        ERROR_ANCHOR_SHARING_MAP_INSUFFICIENT = -1200391105,

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
        public ushort enableETFR;
        public ushort foveationLevel;
		public ushort spatialMeshLod;
        public ushort enableEyeTracking;
        public ushort dynamicFoveation;
    }
    
    public enum SuperResolutionOrSharpeningType
    {
        None,
        SuperResolution,
        NormalSharpening,
        NormalSharpeningAndFixedFoveated,
        NormalSharpeningAndSelfAdaptive,
        NormalSharpeningAndFixedFoveatedAndSelfAdaptive,
        QualitySharpening,
        QualitySharpeningAndFixedFoveated,
        QualitySharpeningAndSelfAdaptive,
        QualitySharpeningAndFixedFoveatedAndSelfAdaptive,
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
        PxrLayerFlagEnableNormalSupersampling = 1 << 13,
        PxrLayerFlagEnableQualitySupersampling = 1 << 14,
        PxrLayerFlagEnableNormalSharpening = 1 << 15,
        PxrLayerFlagEnableQualitySharpening = 1 << 16,
        PxrLayerFlagEnableFixedFoveatedSupersampling = 1 << 17,
        PxrLayerFlagEnableFixedFoveatedSharpening = 1 << 18,
        PxrLayerFlagEnableSelfAdaptiveSharpening = 1 << 19,
        PxrLayerFlagPremultipliedAlpha = 1 << 20,
        PxrLayerFlagEnableSuperResolution = 1 << 21,
        PxrLayerFlagColorSpaceHdrPQ = 1 << 22,
        PxrLayerFlagColorSpaceHdrHLG = 1 << 23,
        PxrLayerFlagFixLayer = 1 << 25,
        PxrLayerFlagBlurredQuadModeSmallWindow = 1 << 26,
        PxrLayerFlagBlurredQuadModeImmersion = 1 << 27,
        PxrLayerFlagMRCComposition = 1 << 30,
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

    public enum BodyActionList:ulong
    {
        PxrNoneAction  = 0,
        PxrTouchGround = 1,
        PxrKeepStatic = 2
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
        public override string ToString()
        {
            return string.Format("TimeStamp :{0}, PosX:{1}, PosY:{2}, PosZ:{3}, RotQx:{4}, RotQy:{5}, RotQz:{6}, RotQw:{7}\n", TimeStamp, PosX, PosY, PosZ, RotQx, RotQy, RotQz, RotQw);
        }
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
    public unsafe struct PxrMotionTracker1ConnectState
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
        MOTION_TRACKER_MODE = 1,
        BONE_PARAM = 2
    }
    public struct BodyTrackingAlgParam
    {
        public BodyTrackingMode BodyJointSet;
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
        private PxrPosef globalPose;
        public PxrVector3f angularVelocity;
        public PxrVector3f linearVelocity;
        public PxrVector3f angularAcceleration;
        public PxrVector3f linearAcceleration;
        public UInt64 poseTimeStampNs;
        private int viewNumber;
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
        PxrInputPICO_4 = 6,
        PxrInputG3 = 7,
        PxrInputPICO_4U = 8
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
        PxrTrackingModeBroadBandMotorBit,
        PxrTrackingModeHandBit
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

    public enum SuperSamplingMode
    {
        None,
        Normal,
        Quality
    }

    public enum SuperSamplingEnhance
    {
        None,
        FixedFoveated
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
        public Quaternion ToQuat()
        {
            return new Quaternion() { x = x, y = y, z = z, w = w };
        }

        public Quaternion ToQuatFlippedZ()
        {
            return new Quaternion() { x = x, y = y, z = -z, w = -w };
        }

    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrVector3f
    {
        public float x;
        public float y;
        public float z;
        public Vector3 ToVector3()
        {
            return new Vector3() { x = x, y = y, z = z };
        }
        public Vector3 ToVector3FlippedZ()
        {
            return new Vector3() { x = x, y = y, z = -z };
        }
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
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
        public float[] colorMatrix;
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
        public float blurredQuadScale;
        public float blurredQuadShift;
        public float blurredQuadFOV;
        public float blurredQuadIPD;
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
    public struct PxrLayerFisheye
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

    [StructLayout(LayoutKind.Sequential)]
    public struct ExternalCameraInfo
    {
        public UInt32 width;
        public UInt32 height;
        public float fov;
    };

    public static class PXR_Plugin
    {
        private const string PXR_SDK_Version = "3.2.4";
        public const string PXR_PLATFORM_DLL = "PxrPlatform";
        private static int PXR_API_Version = 0;

        #region DLLImports
        //MR

        #region SecureMR
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMRFramework(int width, int height, out ulong providerHandle);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_DestroySecureMRFramework(ulong providerHandle);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMRPipeline(ulong providerHandle,out ulong pipelineHandle);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_DestroySecureMRPipeline(ulong pipelineHandle);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_ExecuteSecureMRPipeline(ulong pipelineHandle, ref SecureMRPipelineExecuteParameter parameter, out ulong pipelineRunHandle);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetSecureMROperatorOperandByName(ulong pipelineHandle, ulong operatorHandle,ulong tensorHandle, string name);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetSecureMROperatorResultByName(ulong pipelineHandle, ulong operatorHandle, ulong tensorHandle, string name);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMRTensorByShape(ulong frameworkHandle, SecureMRTensorCreateInfoShape createInfo ,out ulong tensorHandle);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMRTensorByGltf(ulong frameworkHandle, SecureMRTensorCreateInfoGltf createInfo, out ulong tensorHandle);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMRPipelineTensorByShape(ulong pipelineHandle, SecureMRTensorCreateInfoShape createInfo, out ulong tensorHandle);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMRPipelineTensorByGltf(ulong pipelineHandle, SecureMRTensorCreateInfoGltf createInfo, out ulong tensorHandle);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_ResetSecureMRTensor(ulong tensorHandle,ref SecureMRTensorBuffer buffer);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_ResetSecureMRPipelineTensor(ulong pipelineHandle,ulong tensorHandle, ref SecureMRTensorBuffer buffer);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_DestroySecureMRTensor(ulong tensorHandle);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperator(ulong pipelineHandle, SecureMROperatorType operatorType, out ulong operatorHandle);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorArithmeticCompose(ulong pipelineHandle, ref SecureMROperatorArithmeticCompose config, out ulong operatorHandle);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorComparison(ulong pipelineHandle, ref SecureMROperatorComparison config, out ulong operatorHandle);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorNonMaximumSuppression(ulong pipelineHandle, ref SecureMROperatorNonMaximumSuppression config, out ulong operatorHandle);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorUVTo3D(ulong pipelineHandle, out ulong operatorHandle);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorNormalize(ulong pipelineHandle, ref SecureMROperatorNormalize config,out ulong operatorHandle);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorColorConvert(ulong pipelineHandle, ref SecureMROperatorColorConvert config, out ulong operatorHandle);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorSortMatrix(ulong pipelineHandle, ref SecureMROperatorSortMatrix config, out ulong operatorHandle);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorUpdateGltf(ulong pipelineHandle, ref SecureMROperatorUpdateGltf config, out ulong operatorHandle);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorRenderText(ulong pipelineHandle, ref SecureMROperatorRenderText config, out ulong operatorHandle);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSecureMROperatorModel(ulong pipelineHandle, ref SecureMROperatorModel model, out ulong operatorHandle);

        #endregion

        #region 3.0 api Refactoring

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetMeshLOD(ushort spatialMeshLod);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StartSenseDataProviderAsync(ulong providerHandle, out ulong future);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_PollFutureEXT(ref XrFuturePollInfoEXT pollInfo, ref XrFuturePollResultEXT pollResult);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_PollFutureWithProgress(ref XrFuturePollInfoEXT pollInfo, ref XrFuturePollResultProgress pollResultProgress);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSenseDataProvider(ref XrSenseDataProviderCreateInfoBaseHeader createInfo, out ulong providerHandle);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StartSenseDataProviderComplete(ulong future, ref XrSenseDataProviderStartCompletion completion);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetSenseDataProviderState(ulong providerHandle, ref PxrSenseDataProviderState state);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StopSenseDataProvider(ulong providerHandle);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_DestroySenseDataProvider(ulong providerHandle);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSpatialAnchorAsync(ulong providerHandle, ref PxrPosef info, out ulong future);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateSpatialAnchorComplete(ulong providerHandle, ulong future, ref XrSpatialAnchorCompletion completion);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_PersistSpatialAnchorAsync(ulong providerHandle, ref XrSpatialAnchorPersistInfo info, out ulong future);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_PersistSpatialAnchorComplete(ulong providerHandle, ulong future, ref XrSpatialAnchorCompletion completion);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_DestroyAnchor(ulong anchorHandle);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_QuerySenseDataAsync(ulong providerHandle, ref XrSenseDataQueryInfo info, out ulong future);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_QuerySenseDataComplete(ulong providerHandle, ulong future, ref XrSenseDataQueryCompletion completion);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetQueriedSenseData(ulong providerHandle, ref XrQueriedSenseDataGetInfo info, ref XrQueriedSenseData senseData);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_RetrieveSpatialEntityAnchor(ulong snapshotHandle, ref XrSpatialEntityAnchorRetrieveInfo info, out ulong anchorHandle);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_DestroySenseDataQueryResult(ulong snapshotHandle);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_LocateAnchor(ulong anchorHandle, ref XrSpaceLocation location);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetAnchorUuid(ulong anchorHandle, out PxrUuid uuid);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_UnpersistSpatialAnchorAsync(ulong providerHandle, ref XrSpatialAnchorUnpersistInfo info, out ulong future);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_UnpersistSpatialAnchorComplete(ulong providerHandle, ulong future, ref XrSpatialAnchorCompletion completion);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StartSceneCaptureAsync(ulong providerHandle, out ulong future);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StartSceneCaptureComplete(ulong providerHandle, ulong future, ref XrSceneCaptureStartCompletion completion);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_EnumerateSpatialEntityComponentTypes(ulong snapshotHandle, ulong spatialEntity, uint inputCount, out uint outputCount,
            IntPtr types);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong Pxr_GetSpatialMeshProviderHandle();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetSpatialEntityComponentInfo(ulong snapshotHandle, ref XrSpatialEntityComponentGetInfoBaseHeader componentGetInfo,
            ref XrSpatialEntityComponentDataBaseHeader componentInfo);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetSpatialEntitySemanticInfo(ulong snapshotHandle, ref XrSpatialEntityGetInfo componentGetInfo,
            ref XrSpatialEntitySemanticData componentInfo);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetSpatialEntityLocationInfo(ulong snapshotHandle, ref XrSpatialEntityLocationGetInfo componentGetInfo,
            ref XrSpatialEntityLocationData componentInfo);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetSpatialEntityBox3DInfo(ulong snapshotHandle, ref XrSpatialEntityGetInfo componentGetInfo,
            ref XrSpatialEntityBoundingBox3DData componentInfo);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetSpatialEntityBox2DInfo(ulong snapshotHandle, ref XrSpatialEntityGetInfo componentGetInfo,
            ref XrSpatialEntityBoundingBox2DData componentInfo);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetSpatialEntityPolygonInfo(ulong snapshotHandle, ref XrSpatialEntityGetInfo componentGetInfo,
            ref XrSpatialEntityPolygonData componentInfo);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetSpatialMeshVerticesAndIndices(ulong snapshotHandle, ref XrSpatialEntityGetInfo componentGetInfo,
            ref PxrTriangleMeshInfo componentInfo);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe void Pxr_AddOrUpdateMesh(ulong id1, ulong id2, int numVertices, void* vertices, int numTriangles, void* indices,
            Vector3 position, Quaternion rotation);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_RemoveMesh(ulong id1, ulong id2);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_ClearMeshes();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_ShareSpatialAnchorAsync(ulong providerHandle, ref XrSpatialAnchorShareInfo info,out ulong future);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_ShareSpatialAnchorComplete(ulong providerHandle, ulong future,ref XrSpatialAnchorShareCompletion completion);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_DownloadSharedSpatialAnchorAsync(ulong providerHandle, ref XrSharedSpatialAnchorDownloadInfo info,out ulong future);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_DownloadSharedSpatialAnchorComplete(ulong providerHandle, ulong future,ref XrSpatialAnchorShareCompletion completion);

        #endregion
        

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_EnablePassthroughStyle(bool value);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetPassthroughStyle(PxrLayerEffect type, float value, float duration);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetPassthroughLUT(ref byte[] data, int width, int height, int row, int col);

        #region 2.0 api
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_CreateAnchorEntity(ref PxrAnchorEntityCreateInfo info, out ulong anchorHandle);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_DestroyAnchorEntity(ref PxrAnchorEntityDestroyInfo info);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_GetAnchorPose(ulong anchorHandle, PxrTrackingOrigin origin, out PxrPosef pose);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_GetAnchorEntityUuid(ulong anchorHandle, out PxrUuid uuid);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_GetAnchorComponentFlags(ulong anchorHandle,
            out ulong flag);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_GetAnchorSceneLabel(ulong anchorHandle, out PxrSceneLabel label);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_GetAnchorPlaneBoundaryInfo(ulong anchorHandle,
            ref PxrAnchorPlaneBoundaryInfo info);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_GetAnchorPlanePolygonInfo(ulong anchorHandle,
            ref PxrAnchorPlanePolygonInfo info);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_GetAnchorBoxInfo(ulong anchorHandle, ref PxrAnchorVolumeInfo info);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_PersistAnchorEntity(ref PxrAnchorEntityPersistInfo info,
            out ulong taskId);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_UnpersistAnchorEntity(ref PxrAnchorEntityUnPersistInfo info,
            out ulong taskId);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_ClearPersistedAnchorEntity(ref PxrAnchorEntityClearInfo info,
            out ulong taskId);



        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_StartSpatialSceneCapture(out ulong taskId);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]

        #endregion
        private static extern bool Pxr_PollEventFromXRPlugin(ref int eventNum, IntPtr[] eventData);
        
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
        private static extern void Pxr_SetEventDataBufferCallBack(XrEventDataBufferCallBack callback);
      
		
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_EnablePremultipliedAlpha(bool enable);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_SetSuperResolutionOrSharpening(SuperResolutionOrSharpeningType type);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_SetGraphicOption(GraphicsAPI option);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetLayerNextImageIndex(int layerId, ref int imageIndex);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetLayerImageCount(int layerId, EyeType eye, ref UInt32 imageCount);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetConfigIntArray(ConfigType configIndex, int[] configSetData, int dataCount);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetConfigFloatArray(ConfigType configIndex, float[] configSetData, int dataCount);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetLayerAndroidSurface(int layerId, EyeType eye, ref IntPtr androidSurface);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerQuad(IntPtr ptr);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerCylinder(IntPtr ptr);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerEquirect(IntPtr ptr);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerCube(IntPtr ptr);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerEac(IntPtr ptr);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SubmitLayerFisheye(IntPtr ptr);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_SetLayerBlend(bool enable, PxrLayerBlend layerBlend);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_SetLayerColorScale(float x, float y, float z, float w);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_SetLayerColorBias(float x, float y, float z, float w);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern FoveationLevel Pxr_GetFoveationLevel();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetFoveationParams(FoveationParams foveationParams);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetFrustum(EyeType eye, float fovLeft, float fovRight, float fovUp, float fovDown, float near, float far);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetFrustum(EyeType eye, ref float fovLeft, ref float fovRight, ref float fovUp, ref float fovDown, ref float near, ref float far);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetConfigFloat(ConfigType configIndex, ref float value);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetConfigInt(ConfigType configIndex, ref int value);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetConfigInt(ConfigType configSetIndex, int configSetData);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetConfigString(ConfigType configSetIndex, string configSetData);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetConfigUint64(ConfigType configSetIndex, UInt64 configSetData);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_ResetSensor(ResetSensorOption option);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetSensorLostCustomMode(bool value);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetSensorLostCMST(bool value);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pxr_GetDisplayRefreshRatesAvailable(ref int configCount, ref IntPtr configArray);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetDisplayRefreshRate(float refreshRate);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetDisplayRefreshRate(ref float displayRefreshRate);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetPredictedDisplayTime(ref double predictedDisplayTime);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pxr_SetExtraLatencyMode(int mode);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetTrackingMode(ref UInt64 trackingModeFlags);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        
        public static extern int Pxr_SetTrackingOrigin(PxrTrackingOrigin mode);
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetTrackingOrigin(ref PxrTrackingOrigin mode);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_UpdateContentProtectState(int state);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_UpdateDisplayRefreshRate(int rate);

        //Tracking Sensor
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetPredictedMainSensorState2(double predictTimeMs, ref PxrSensorState2 sensorState, ref int sensorFrameIndex);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_HMDUpdateSwitch(bool enable);        

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_SubmitLayerSwitch(bool enable);

        //Controller
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_SetControllerOriginOffset(int controllerID, Vector3 offset);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetControllerTrackingState(UInt32 deviceID, double predictTime, ref PxrControllerTracking tracking);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetControllerMainInputHandle(UInt32 deviceID);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetControllerMainInputHandle(ref int deviceID);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetControllerVibration(UInt32 deviceID, float strength, int time);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetControllerEnableKey(bool isEnable, PxrControllerKeyMap Key);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_ResetController(UInt32 deviceID);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetArmModelParameters(PxrGazeType gazetype, PxrArmModelType armmodeltype, float elbowHeight, float elbowDepth, float pointerTiltAngle);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetControllerHandness(ref int handness);

        //Vibration

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetControllerVibrationEvent(UInt32 deviceID, int frequency, float strength, int time);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetControllerCapabilities(UInt32 deviceID, ref PxrControllerCapability capability);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StopControllerVCMotor(int clientId);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StartControllerVCMotor(string file, int slot);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetControllerAmp(float mode);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetControllerDelay(int delay);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern string Pxr_GetVibrateDelayTime(ref int length);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StartVibrateBySharemF(float[] data, ref AudioClipData parameter, ref int source_id);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StartVibrateByCache(int clicpid);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_ClearVibrateByCache(int clicpid);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StartVibrateByPHF(string data, int buffersize, ref int sourceID, ref VibrateInfo vibrateInfo);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_PauseVibrate(int sourceID);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_ResumeVibrate(int sourceID);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_UpdateVibrateParams(int clicp_id, ref VibrateInfo vibrateInfo);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_CreateHapticStream(string phfVersion, UInt32 frameDurationMs, ref VibrateInfo hapticInfo, float speed, ref int id);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_WriteHapticStream(int id, ref PxrPhfParamsNum frames, UInt32 numFrames);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetPHFHapticSpeed(int id, float speed);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetPHFHapticSpeed(int id, ref float speed);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetCurrentFrameSequence(int id, ref UInt64 frameSequence);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StartPHFHaptic(int source_id);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StopPHFHaptic(int source_id);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_RemovePHFHaptic(int source_id);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_SetLogInfoActive(bool value);


        //Boundary

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_SetVideoSeethroughState(bool value);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_TestNodeIsInBoundary(BoundaryTrackingNode node, bool isPlayArea, ref PxrBoundaryTriggerInfo info);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_TestPointIsInBoundary(ref PxrVector3f point, bool isPlayArea, ref PxrBoundaryTriggerInfo info);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetBoundaryGeometry(bool isPlayArea, UInt32 pointsCountInput, ref UInt32 pointsCountOutput, PxrVector3f[] outPoints);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetBoundaryDimensions(bool isPlayArea, out PxrVector3f dimension);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pxr_GetBoundaryConfigured();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pxr_GetBoundaryEnabled();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetBoundaryVisible(bool value);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetSeeThroughBackground(bool value);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pxr_GetBoundaryVisible();
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_ResetSensorHard();
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetTrackingState();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetGuardianSystemDisable(bool disable);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_ResumeGuardianSystemForSTS();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_PauseGuardianSystemForSTS();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_ShutdownSdkGuardianSystem();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetRoomModeState();

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_DisableBoundary();

        //Face tracking
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetTrackingMode(double trackingMode);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetFaceTrackingData(Int64 ts, int flags, ref PxrFaceTrackingInfo faceTrackingInfo);
        
        //Application SpaceWarp
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_SetSpaceWarp(int value);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_SetAppSpacePosition(float x, float y, float z);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pxr_SetAppSpaceRotation(float x, float y, float z, float w);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetTrackingStatus(String key, String value);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetPerformanceLevels(PxrPerfSettings which, PxrSettingsLevel level);
        
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetPerformanceLevels(PxrPerfSettings which, ref PxrSettingsLevel level);
      
        //Body tracking
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetBodyTrackingPose(double predictTime, ref BodyTrackerResult bodyTrackerResult);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_SetBodyTrackingMode(int mode);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetFitnessBandConnectState(ref PxrMotionTracker1ConnectState state);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetFitnessBandBattery(int trackerId, ref int battery);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetFitnessBandCalibState(ref int calibrated);
              
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_LogSdkApi(string sdkInfo);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_SetBodyTrackingAlgParam(BodyTrackingAlgParamType AlgParamType, ref BodyTrackingAlgParam Param);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_UpdateAdaptiveResolution(ref PxrExtent2Di dimensions, AdaptiveResolutionPowerSetting powerSetting);

        //MRC
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetExternalCameraInfo(ref ExternalCameraInfo cameraInfo);

        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetExternalCameraPose(ref PxrPosef outPose);
        
        [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern float Pxr_VstModelOffset();

 		[DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetPSensorState(ref bool isUserPresent);
        #endregion

        public static class System
        {
            public static Action RecenterSuccess;
            public static Action FocusStateAcquired;
            public static Action FocusStateLost;
            public static Action SensorReady;
            public static Action<XrSessionState> SessionStateChanged;
            public static Action<int> InputDeviceChanged;
            public static Action<float> DisplayRefreshRateChangedAction;
   			public static Action<bool> UserPresenceChangedAction;
   			public static Action<bool> MRCStateChangedAction;
 			public static string ProductName;
 			public static bool enableMRC = false;
            public static float UPxr_VstModelOffset()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
               return Pxr_VstModelOffset();
#endif
                return 0;
            }
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

            public static bool UPxr_QueryDeviceAbilities(PxrDeviceAbilities abilities)
            {
                UInt64 flags = UInt64.MinValue;
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_GetTrackingMode(ref flags);
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
                    case PxrDeviceAbilities.PxrTrackingModeBroadBandMotorBit:
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
                                                
            public static void UPxr_SetXrEventDataBufferCallBack(XrEventDataBufferCallBack callback)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                Pxr_SetEventDataBufferCallBack(callback);
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
                int result = 0;

                return result;
            }
            public static float[] UPxr_GetDisplayFrequenciesAvailable()
            {

                float[] configArray = { 0 };
                int configCount = 0;
                IntPtr configHandle = IntPtr.Zero;
                bool ret = false;
#if UNITY_ANDROID && !UNITY_EDITOR
                ret = Pxr_GetDisplayRefreshRatesAvailable(ref configCount, ref configHandle);
#endif
                if (ret)
                {
                    configArray = new float[configCount];
                    Marshal.Copy(configHandle, configArray, 0, configCount);
                }

                return configArray;
            }
            public static float UPxr_GetSystemDisplayFrequency()
            {
                float displayRefreshRate = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                 Pxr_GetDisplayRefreshRate(ref displayRefreshRate);
#endif
                return displayRefreshRate;
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

            public static int UPxr_GetExternalCameraInfo(out ExternalCameraInfo cameraInfo)
            {
                int result = 0;
                cameraInfo = new ExternalCameraInfo();
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_GetExternalCameraInfo(ref cameraInfo);
#endif
                PLog.i(TAG, $"UPxr_GetExternalCameraInfo() result = {result}, cameraInfo.width = {cameraInfo.width}, cameraInfo.height = {cameraInfo.height}, cameraInfo.fov = {cameraInfo.fov}");
                return result;
            }

            public static int UPxr_GetExternalCameraPose(PxrTrackingOrigin pxrTrackingOrigin, out PxrPosef outPose)
            {
                int result = 0;
                outPose = new PxrPosef();
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_GetExternalCameraPose(ref outPose);
#endif
                PLog.d(TAG, $"UPxr_GetExternalCameraPose() result = {result}, pxrTrackingOrigin = {pxrTrackingOrigin}, outPose.orientation = {outPose.orientation.ToQuat()}, outPose.position = {outPose.position.ToVector3()}");
                return result;
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
                Pxr_GetFaceTrackingData(ts, flags, ref faceTrackingInfo );
#endif
                return 0;
            }

            public static int UPxr_SetFaceTrackingStatus(PxrFtLipsyncValue value) {
                int num = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                num = Pxr_SetTrackingStatus("ft_lipsync_ctl", ((int)value).ToString());
#endif
                return num;
            }

            public static bool UPxr_GetMRCEnable()
            {
                PLog.d(TAG, "UPxr_GetMRCEnable() enableMRC:" + enableMRC);
                return enableMRC;
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
                return true;
            }

            public static int UPxr_GetCurrentBrightness()
            {
                return 0;
            }

            public static int[] UPxr_GetScreenBrightnessLevel()
            {
                int[] currentlight = { 0 };

                return currentlight;
            }

            public static void UPxr_SetScreenBrightnessLevel(int vrBrightness, int level)
            {

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
                    if (!UPxr_InitAudioDevice()) return false;
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
                     if (!UPxr_InitAudioDevice()) return maxvolm;
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
                     if (!UPxr_InitAudioDevice()) return currentvolm;
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
                     if (!UPxr_InitAudioDevice()) return false;
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
                     if (!UPxr_InitAudioDevice()) return false;
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
                     if (!UPxr_InitAudioDevice()) return false;
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

            public static string UPxr_GetProductName()
            {
                string product = "";
#if UNITY_ANDROID && !UNITY_EDITOR
                AndroidJavaClass buildClass = new AndroidJavaClass("android.os.Build");
                product = buildClass.GetStatic<string>("PRODUCT");
#endif
                return product;
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
                result = Pxr_SetPerformanceLevels(which, level);
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
                result = Pxr_GetPerformanceLevels(which, ref level);
#endif
                PLog.d(TAG, "UPxr_GetPerformanceLevels() result:" + result + ", level:" + level);
                return level;
            }

            [Obsolete("UPxr_GetDeviceSN has been deprecated")]
            public static string UPxr_GetDeviceSN()
            {
                string serialNum = "UNKONWN";
                return serialNum;
            }

            [Obsolete("UPxr_Sleep has been deprecated")]
            public static void UPxr_Sleep()
            {

            }

            [Obsolete("UPxr_SetSecure has been deprecated")]
            public static void UPxr_SetSecure(bool isOpen)
            {

            }

            [Obsolete("UPxr_GetColorRes has been deprecated")]
            public static int UPxr_GetColorRes(string name)
            {
                int value = -1;

                return value;
            }

            [Obsolete("UPxr_GetConfigInt has been deprecated")]
            public static int UPxr_GetConfigInt(string name)
            {
                int value = -1;

                return value;
            }

            [Obsolete("UPxr_GetConfigString has been deprecated")]
            public static string UPxr_GetConfigString(string name)
            {
                string value = "";

                return value;
            }

            [Obsolete("UPxr_GetDrawableLocation has been deprecated")]
            public static string UPxr_GetDrawableLocation(string name)
            {
                string value = "";

                return value;
            }

            [Obsolete("UPxr_GetTextSize has been deprecated")]
            public static int UPxr_GetTextSize(string name)
            {
                int value = -1;

                return value;
            }

            [Obsolete("UPxr_GetLangString has been deprecated")]
            public static string UPxr_GetLangString(string name)
            {
                string value = "";

                return value;
            }

            [Obsolete("UPxr_GetStringValue has been deprecated")]
            public static string UPxr_GetStringValue(string id, int type)
            {
                string value = "";

                return value;
            }

            [Obsolete("UPxr_GetIntValue has been deprecated")]
            public static int UPxr_GetIntValue(string id, int type)
            {
                int value = -1;

                return value;
            }

            [Obsolete("UPxr_GetFloatValue has been deprecated")]
            public static float UPxr_GetFloatValue(string id)
            {
                float value = -1;

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

            [Obsolete("UPxr_GetCharSpace has been deprecated")]
            public static int UPxr_GetCharSpace(string id)
            {
                int value = -1;

                return value;
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
               double predictTime = UPxr_GetPredictedDisplayTime();
                return Pxr_GetPredictedMainSensorState2(predictTime, ref sensorState, ref sensorFrameIndex);
#else
                return 0;
#endif
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


        }

        public static class Boundary
        {
            private const string TAG = "[PXR_Plugin/Boundary]";
            public static int seeThroughState = 0;

            /// <summary>
            /// A callback function that notifies the change of seethrough state.
            /// </summary>
            public static Action<int> SeethroughStateChangedAction;


            public static PxrBoundaryTriggerInfo UPxr_TestNodeIsInBoundary(BoundaryTrackingNode node,
                BoundaryType boundaryType)
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

            public static PxrBoundaryTriggerInfo UPxr_TestPointIsInBoundary(PxrVector3f point,
                BoundaryType boundaryType)
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

            public static void UPxr_SetSeeThroughState(bool value)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_SetVideoSeethroughState(value);
#endif
            }

            [Obsolete("Deprecated", true)]
            public static void UPxr_ResetSeeThroughSensor() { }


            public static PxrTrackingState UPxr_GetSeeThroughTrackingState()
            {
                int state = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                state = Pxr_GetTrackingState();
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
  			public static void UPxr_SetFFRSubsampled(bool enable)
            {
                PLog.d(TAG, "UPxr_SetFFRSubsampled() level:" + enable);
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_SetFFRSubsampled(enable);
#endif
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
                result = Pxr_SetConfigFloatArray(type, fovData, 4);
#endif
                PLog.d(TAG, string.Format("UPxr_SetEyeFOV Pxr_SetConfigFloatArray type = {0}, fovData[0] = {1},  fovData[1] = {2},  fovData[2] = {3},  fovData[3] = {4}, result = {5}", type, fovData[0], fovData[1], fovData[2], fovData[3], result));
                return result;
            }

            public static void UPxr_CreateLayerParam(PxrLayerParam layerParam)
            {
                if (layerParam.layerId < 1)
                {
                    PLog.e(TAG, "UPxr_CreateLayerParam() layerId:" + layerParam.layerId + " must be greater than 1.");
                    return;
                }
                PLog.i(TAG, $"Pxr_CreateLayerParam() layerParam.layerId={layerParam.layerId}, layerShape={layerParam.layerShape}, layerType={layerParam.layerType}, width={layerParam.width}, height={layerParam.height}, layerFlags={layerParam.layerFlags}, format={layerParam.format}, layerLayout={layerParam.layerLayout}.");
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_CreateLayerParam(layerParam);
#endif
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

            public static void UPxr_GetLayerImagePtr(int layerId, EyeType eye, int imageIndex, ref IntPtr image)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_GetLayerImagePtr(layerId, eye, imageIndex, ref image);
#endif
                PLog.d(TAG, "UPxr_GetLayerImagePtr() layerId:" + layerId + " eye:" + eye + " imageIndex:" + imageIndex + " image:" + image);
            }
            

            #region Compositor Layer
            public static int UPxr_GetLayerAndroidSurface(int layerId, EyeType eye, ref IntPtr androidSurface)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_GetLayerAndroidSurface(layerId, eye, ref androidSurface);
#endif
                PLog.d(TAG, "UPxr_GetLayerAndroidSurface() layerId:" + layerId + " eye:" + eye + " androidSurface:" + androidSurface + " result:" + result);
                return result;
            }

            public static void UPxr_DestroyLayerByRender(int layerId)
            {
                if (layerId < 1)
                {
                    PLog.e(TAG, "UPxr_DestroyLayerByRender() layerId:" + layerId + " must be greater than 1.");
                    return;
                }
                PLog.i(TAG, "UPxr_DestroyLayerByRender() layerId:" + layerId);
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_DestroyLayerByRender(layerId);
#endif
            }

            public static bool UPxr_GetLayerNextImageIndexByRender(int layerId, ref int imageIndex)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_GetLayerNextImageIndex(layerId, ref imageIndex);
#endif
                PLog.d(TAG, "UPxr_GetLayerNextImageIndexByRender() layerId:" + layerId + " imageIndex:" + imageIndex);
                return result == -8;
            }

            public static bool UPxr_SubmitLayerQuad2ByRender(IntPtr ptr)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerQuad(ptr);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerQuad() ptr:" + ptr + " result:" + result);
                return result == -8;
            }

            public static bool UPxr_SubmitLayerCylinder2ByRender(IntPtr ptr)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerCylinder(ptr);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerCylinder() ptr:" + ptr + " result:" + result);
                return result == -8;
            }

            public static bool UPxr_SubmitLayerEquirect2ByRender(IntPtr ptr)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerEquirect(ptr);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerEquirect() ptr:" + ptr + " result:" + result);
                return result == -8;
            }

            public static bool UPxr_SubmitLayerCube2ByRender(IntPtr ptr)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerCube(ptr);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerCube() ptr:" + ptr + " result:" + result);
                return result == -8;
            }

            public static bool UPxr_SubmitLayerEac2ByRender(IntPtr ptr)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerEac(ptr);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerEac() ptr:" + ptr + " result:" + result);
                return result == -8;
            }
            public static bool UPxr_SubmitLayerFisheyeByRender(IntPtr ptr)
            {
                int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = Pxr_SubmitLayerFisheye(ptr);
#endif
                PLog.d(TAG, "UPxr_SubmitLayerFisheye() ptr:" + ptr + " result:" + result);
                return result == -8;
            }
            #endregion
            public static void UPxr_SetLayerBlend(bool enable, PxrLayerBlend layerBlend)
            {
                PLog.d(TAG, "UPxr_SetLayerBlend() enable:" + enable + " layerBlend.srcColor:" + layerBlend.srcColor + " dstColor:" + layerBlend.dstColor + " srcAlpha:" + layerBlend.srcAlpha + " dstAlpha:" + layerBlend.dstAlpha);
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_SetLayerBlend(enable, layerBlend);
#endif
            }
            
            public static void UPxr_SetLayerColorScale(float x, float y, float z, float w)
            {
                PLog.d(TAG, $"UPxr_SetLayerColorScale() x:{x},y:{y},z:{z},w:{w}");
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_SetLayerColorScale(x, y, z, w);
#endif
            }

            public static void UPxr_SetLayerColorBias(float x, float y, float z, float w)
            {
                PLog.d(TAG, $"UPxr_SetLayerColorBias() x:{x},y:{y},z:{z},w:{w}");
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_SetLayerColorBias(x, y, z, w);
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
            public static bool UPxr_SubmitLayerQuadByRender(IntPtr ptr)
            {
                int result = 0;
                return result == -8;
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

            public static void UPxr_SetSuperResolutionOrSharpening(SuperResolutionOrSharpeningType type)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_SetSuperResolutionOrSharpening(type);
#endif
                PLog.i(TAG, "UPxr_SetSuperResolutionOrSharpening " + type);
            }
        }

        public static class Sensor
        {
            private const string TAG = "[PXR_Plugin/Sensor]";

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


            public static bool UPxr_GetPSensorState()
            {
                PLog.d(TAG, "UPxr_GetPSensorState()");
                bool isUserPresent = false;
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_GetPSensorState(ref isUserPresent);
#endif
                PLog.d(TAG, "UPxr_GetPSensorState() psensor:" + isUserPresent);
                return isUserPresent;
            }
            public static int UPxr_HMDUpdateSwitch(bool value)
            {
                PLog.d(TAG, $"UPxr_HMDUpdateSwitch() value={value}");
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_HMDUpdateSwitch(value);
#else
                return 0;
#endif
            }
            
            public static void UPxr_SubmitLayerSwitch(bool enable)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_SubmitLayerSwitch(enable);
#endif
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

            public static int UPxr_GetMotionTrackerConnectStateWithID(ref PxrMotionTracker1ConnectState state)
            {
                return 0;
            }
            
            public static int UPxr_GetMotionTrackerCalibState(ref int calibrated) {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetFitnessBandCalibState(ref calibrated);
#endif
                return 0;
            }

            public static int UPxr_SetBodyTrackingMode(BodyTrackingMode mode)
            {
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    BodyTrackingAlgParam pxrBodyTrackingAlgParam = new BodyTrackingAlgParam();
                    pxrBodyTrackingAlgParam.BodyJointSet = mode;
                    return Pxr_SetBodyTrackingAlgParam(BodyTrackingAlgParamType.MOTION_TRACKER_MODE, ref pxrBodyTrackingAlgParam);
#endif
                }
                return 0;
            }

            public static int UPxr_SetBodyTrackingBoneLength(BodyTrackingBoneLength boneLength)
            {
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    BodyTrackingAlgParam pxrBodyTrackingAlgParam = new BodyTrackingAlgParam();
                    pxrBodyTrackingAlgParam.BodyJointSet = BodyTrackingMode.BTM_FULL_BODY_HIGH;
                    pxrBodyTrackingAlgParam.BoneLength = boneLength;
                    return Pxr_SetBodyTrackingAlgParam(BodyTrackingAlgParamType.BONE_PARAM, ref pxrBodyTrackingAlgParam);
#endif
                }
                return 0;
            }

            public static int UPxr_SetControllerVibrationEvent(UInt32 hand, int frequency, float strength, int time)
            {
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


            public static int UPxr_StopControllerVCMotor(int id)
            {
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
                
                return 0;
            }

            public static int UPxr_SetControllerAmp(float mode)
            {
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_SetControllerAmp(mode);
#endif
                }
                return 0;
            }

            public static int UPxr_SetControllerDelay()
            {
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
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetVibrateDelayTime(ref x);
#endif
                }
                return " ";
            }

            public static int UPxr_StartVibrateBySharem(float[] data, int slot, int buffersize, int sampleRate, int channelMask, int bitrate ,int channelFlip, ref int sourceId)
            {
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
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] UPxr_ClearVibrateByCache " + clicpid.ToString());
                    return Pxr_ClearVibrateByCache(clicpid);
#endif
                }
                return 0;
            }

            public static int UPxr_StartVibrateByPHF(string data, int buffersize, ref int sourceId, int slot, int reversal, float amp) 
            {
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

            public static int UPxr_PauseVibrate(int sourceID) 
            {
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] Pxr_PauseVibrate " + sourceID.ToString());
                    return Pxr_PauseVibrate(sourceID);
#endif
                }
                return 0;
            }

            public static int UPxr_ResumeVibrate(int sourceID) 
            {
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] Pxr_ResumeVibrate " + sourceID.ToString());
                    return Pxr_ResumeVibrate(sourceID);
#endif
                }
                return 0;
            }

            public static int UPxr_UpdateVibrateParams(int clicp_id,int slot, int reversal, float amp)
            {
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

            public static int UPxr_CreateHapticStream(string phfVersion, UInt32 frameDurationMs, ref VibrateInfo hapticInfo, float speed, ref int id)
            {
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] UPxr_CreateHapticStream ");
                    return Pxr_CreateHapticStream(phfVersion, frameDurationMs, ref hapticInfo, speed, ref id);    
#endif
                }
                return 0;
            }

            public static int UPxr_WriteHapticStream(int id, ref PxrPhfParamsNum frames, UInt32 numFrames)
            {
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] UPxr_WriteHapticStream ");
                    return Pxr_WriteHapticStream( id, ref  frames,  numFrames); 
#endif
                }
                return 0;
            }

            public static int UPxr_SetPHFHapticSpeed(int id, float speed) 
            {
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] UPxr_SetPHFHapticSpeed ");
                    return Pxr_SetPHFHapticSpeed( id,  speed);
#endif
                }
                return 0;
            }

            public static int UPxr_GetPHFHapticSpeed(int id, ref float speed)
            {
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] UPxr_GetPHFHapticSpeed ");
                    return Pxr_GetPHFHapticSpeed( id, ref speed);
#endif
                }
                return 0;
            }

            public static int UPxr_GetCurrentFrameSequence(int id, ref UInt64 frameSequence) 
            {
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] UPxr_GetCurrentFrameSequence ");
                    return Pxr_GetCurrentFrameSequence( id, ref  frameSequence);
#endif
                }
                return 0;
            }

            public static int UPxr_StartPHFHaptic(int source_id)
            {
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] UPxr_StartPHFHaptic ");
                    return Pxr_StartPHFHaptic(source_id);
#endif
                }
                return 0;
            }

            public static int UPxr_StopPHFHaptic(int source_id)
            {
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Debug.Log("[VCMotor_SDK] UPxr_StopPHFHaptic ");
                    return Pxr_StopPHFHaptic(source_id);
#endif
                }
                return 0;
            }

            public static int UPxr_RemovePHFHaptic(int source_id) 
            {
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

            public static int UPxr_GetControllerTrackingState(UInt32 deviceID, double predictTime, ref PxrControllerTracking tracking)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetControllerTrackingState(deviceID,predictTime, ref tracking);
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
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Pxr_ResetController(0);
#endif
                }
            }

            public static void UPxr_SetArmModelParameters(PxrGazeType gazetype, PxrArmModelType armmodeltype, float elbowHeight, float elbowDepth, float pointerTiltAngle)
            {
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Pxr_SetArmModelParameters(gazetype, armmodeltype, elbowHeight, elbowDepth, pointerTiltAngle);
#endif
                }
            }

            public static void UPxr_GetControllerHandness(ref int deviceID)
            {
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    Pxr_GetControllerHandness(ref deviceID);
#endif
                }
            }
        }

        public static class HandTracking
        {
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetHandTrackerSettingState(ref bool settingState);

            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetHandTrackerActiveInputType(ref ActiveInputDevice activeInputDevice);

            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetHandTrackerAimState(HandType hand, ref HandAimState aimState);

            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetHandTrackerJointLocations(HandType hand, ref HandJointLocations jointLocations);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetHandTrackerHandScale(int hand,ref float  scale);

            public static bool UPxr_GetHandTrackerSettingState()
            {
                bool val = false;
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
                val = Pxr_GetHandTrackerAimState(hand,ref aimState) == 0;
                return val;
            }

            public static bool UPxr_GetHandTrackerJointLocations(HandType hand, ref HandJointLocations jointLocations)
            {
                bool val = false;
                val = Pxr_GetHandTrackerJointLocations(hand, ref jointLocations) == 0;
                return val;
            }
            public static bool UPxr_GetHandScale(int hand,ref float scale)
            {
                bool val = false;
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_GetHandTrackerHandScale(hand, ref scale) == 0;
#endif
                }
                return val;
            }
        }

        public static class MotionTracking
        {
            const string TAG = "MotionTracking"; 
            #region Eye Tracking

            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_WantEyeTrackingService();
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static unsafe extern int Pxr_GetEyeTrackingSupported(ref bool supported, ref int supportedModesCount, EyeTrackingMode* supportedModes);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_StartEyeTracking1(ref EyeTrackingStartInfo startInfo);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_StopEyeTracking1(ref EyeTrackingStopInfo stopInfo);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetEyeTrackingState(ref bool isTracking, ref EyeTrackingState state);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
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
#if !UNITY_EDITOR && UNITY_ANDROID
                val = Pxr_WantEyeTrackingService();
#endif
                return val;
            }

            public static unsafe int UPxr_GetEyeTrackingSupported(ref bool supported, ref int supportedModesCount, ref EyeTrackingMode[] supportedModes)
            {
                int val = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                supportedModes = new EyeTrackingMode[Enum.GetNames(typeof(EyeTrackingMode)).Length];
                fixed (EyeTrackingMode* pointer = supportedModes)
                {
                    val = Pxr_GetEyeTrackingSupported(ref supported, ref supportedModesCount, pointer);
                }
#endif
                return val;
            }

            public static int UPxr_StartEyeTracking1(ref EyeTrackingStartInfo startInfo)
            {
                int val = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                 val = Pxr_StartEyeTracking1(ref startInfo);
#endif
                return val;
            }

            public static int UPxr_StopEyeTracking1(ref EyeTrackingStopInfo stopInfo)
            {
                int val = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                val = Pxr_StopEyeTracking1(ref stopInfo);
#endif
                return val;
            }

            public static int UPxr_GetEyeTrackingState(ref bool isTracking, ref EyeTrackingState state)
            {
                int val = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                val = Pxr_GetEyeTrackingState(ref isTracking, ref state);
#endif
                return val;
            }

            public static int UPxr_GetEyeTrackingData1(ref EyeTrackingDataGetInfo getInfo, ref EyeTrackingData data)
            {
                int val = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                val = Pxr_GetEyeTrackingData1(ref getInfo, ref data);
#endif
                return val;
            }

            public static int UPxr_GetEyeOpenness(ref float leftEyeOpenness, ref float rightEyeOpenness)
            {
                int val = 0;
#if !UNITY_EDITOR && UNITY_ANDROID
                val = Pxr_GetEyeOpenness(ref leftEyeOpenness, ref rightEyeOpenness);
#endif
                return val;
            }

            public static int UPxr_GetEyePupilInfo(ref EyePupilInfo eyePupilPosition)
            {
                int val = 0;
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
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_GetEyeBlink(ref timestamp, ref isLeftBlink, ref isRightBlink);
#endif
                }
                return val;
            }

            #endregion

            #region Face Tracking

            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_WantFaceTrackingService();
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static unsafe extern int Pxr_GetFaceTrackingSupported(ref bool supported, ref int supportedModesCount, FaceTrackingMode* supportedModes);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_StartFaceTracking(ref FaceTrackingStartInfo startInfo);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_StopFaceTracking(ref FaceTrackingStopInfo stopInfo);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetFaceTrackingState(ref bool isTracking, ref FaceTrackingState state);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetFaceTrackingData1(ref FaceTrackingDataGetInfo getInfo, ref FaceTrackingData data);

            public static int UPxr_WantFaceTrackingService()
            {
                int val = -1;
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_WantFaceTrackingService();
#endif
                }
                return val;
            }

            public static unsafe int UPxr_GetFaceTrackingSupported(ref bool supported, ref int supportedModesCount, ref FaceTrackingMode[] supportedModes)
            {
                int val = -1;
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    supportedModes = new FaceTrackingMode[Enum.GetNames(typeof(FaceTrackingMode)).Length];
                    fixed (FaceTrackingMode* pointer = supportedModes)
                    {
                        val = Pxr_GetFaceTrackingSupported(ref supported, ref supportedModesCount, pointer);
                    }
#endif
                }
                return val;
            }

            public static int UPxr_StartFaceTracking(ref FaceTrackingStartInfo startInfo)
            {
                int val = -1;
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
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_GetFaceTrackingData1(ref getInfo, ref data);
#endif
                }
                return val;
            }

            #endregion

            #region BodyTracking
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_StartBodyTrackingCalibApp();
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_WantBodyTrackingService();
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetBodyTrackingSupported(ref bool supported);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_StartBodyTracking(ref BodyTrackingStartInfo startInfo);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_StopBodyTracking();
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetBodyTrackingState(ref bool isTracking, ref BodyTrackingStatus state);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetBodyTrackingData(ref BodyTrackingGetDataInfo getInfo, ref BodyTrackingData data);

            public static int UPxr_StartMotionTrackerCalibApp()
            {
                int val = -1;
               
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                val = Pxr_StartBodyTrackingCalibApp();
#endif
                }
                return val;
            }
            public static int UPxr_WantBodyTrackingService()
            {
                int val = -1;
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_WantBodyTrackingService();
#endif
                }
                return val;
            }
            public static unsafe int UPxr_GetBodyTrackingSupported(ref bool supported)
            {
                int val = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                val = Pxr_GetBodyTrackingSupported(ref supported);
#endif
                return val;
            }
            public static int UPxr_StartBodyTracking(BodyJointSet JointSet,BodyTrackingBoneLength boneLength)
            {
                BodyTrackingStartInfo startInfo = new BodyTrackingStartInfo();
                startInfo.jointSet = JointSet;
                startInfo.BoneLength = boneLength;
                int val = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_StartBodyTracking(ref startInfo);
#endif
                return val;
            }
            public static int UPxr_StopBodyTracking()
            {
                int val = -1;
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_StopBodyTracking();
#endif
                }
                return val;
            }
            public static int UPxr_GetBodyTrackingState(ref bool isTracking, ref BodyTrackingStatus state)
            {
                int val = -1;
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_GetBodyTrackingState(ref isTracking, ref state);
#endif
                }
                return val;
            }
            public unsafe static int UPxr_GetBodyTrackingData(ref BodyTrackingGetDataInfo getInfo, ref BodyTrackingData data)
            {
                int val = -1;
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_GetBodyTrackingData(ref getInfo, ref data);
                    for (int i = 0; i < (int)BodyTrackerRole.ROLE_NUM; i++)
                    {
                        data.roleDatas[i].localPose.PosZ = -data.roleDatas[i].localPose.PosZ;
                        data.roleDatas[i].localPose.RotQz = -data.roleDatas[i].localPose.RotQz;
                        data.roleDatas[i].localPose.RotQw = -data.roleDatas[i].localPose.RotQw;
                        data.roleDatas[i].velo[3] = -data.roleDatas[i].velo[3];
                        data.roleDatas[i].acce[3] = -data.roleDatas[i].acce[3];
                        data.roleDatas[i].wvelo[3] = -data.roleDatas[i].wvelo[3];
                        data.roleDatas[i].wacce[3] = -data.roleDatas[i].wacce[3];
                    }
#endif
                }
                return val;
            }
            #endregion

            #region MotionTracker
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_SetExpandDeviceVibrate(long deviceid, ExpandDeviceVibrate motorVibrate);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_SetExpandDeviceCustomDataCapability( bool state);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetExpandDeviceBattery(long deviceid, ref float out_battery, ref int out_charger);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int Pxr_GetExpandDevice(ref int configCount, ref IntPtr deviceArray);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_RequestMotionTrackerDevice(UInt32 deviceCount);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetLocateMotionTracker(long trackerid,ref MotionTrackerLocation location,ref bool isValidPose);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            private static extern int Pxr_GetMotionTrackerBatteryState(long trackerid,ref float batteryLevel,ref int state);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int Pxr_GetExpandDeviceCustomData(ref Int32 customDataCountOutput);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int Pxr_GetExpandDeviceCustomDatabyID(int num,ref Int64 deviceId, ref IntPtr deviceArray,ref int datasize);
            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int Pxr_SetExpandDeviceCustomData( int dataSize);

            [DllImport(PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int Pxr_SetExpandDeviceCustomDatabyID(int num, long deviceId, int dataSize, byte[] data);
            public static int UPxr_CheckMotionTrackerNumber(int number)
            {
                int val = -1;

#if !UNITY_EDITOR && UNITY_ANDROID
                val = Pxr_RequestMotionTrackerDevice((UInt32)number);
#endif

                PLog.d(TAG, $"UPxr_CheckMotionTrackerNumber() number={number}, val={val}");
                return val;
            }
            public static int UPxr_GetMotionTrackerLocation(long trackerid, ref MotionTrackerLocation location, ref bool isValidPose)
            {
                int val = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                  val = Pxr_GetLocateMotionTracker(trackerid,ref location,ref isValidPose);
#endif
                return val;
            }
            public static  int UPxr_GetExpandDevice(out long[] deviceArray)
            {
                int ret = -1;
                deviceArray = null;
                int Count = 0;
                IntPtr configHandle = IntPtr.Zero;
#if !UNITY_EDITOR && UNITY_ANDROID
                ret = Pxr_GetExpandDevice(ref Count, ref configHandle);
                if (ret == 0)
                {
                    deviceArray = new long[Count];
                    Marshal.Copy(configHandle, deviceArray, 0, Count);
                }
#endif
                return ret;
            }
            public static int UPxr_SetExpandDeviceVibrate(long deviceid, ExpandDeviceVibrate motorVibrate)
            {
                int val = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                val = Pxr_SetExpandDeviceVibrate(deviceid, motorVibrate);
#endif
                PLog.d(TAG, $"UPxr_SetExpandDeviceVibrate() level={motorVibrate.amplitude}, frequency={motorVibrate.frequency}, duration={motorVibrate.duration}, trackersSN={deviceid}, val={val}");
                return val;
            }
            public static int UPxr_SetExpandDeviceCustomDataCapability(bool state)
            {
                int val = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                    val = Pxr_SetExpandDeviceCustomDataCapability(state);
#endif
                PLog.d(TAG, $"UPxr_SetExpandDeviceCustomDataCapability() state={state}, val={val}");
                return val;
            }

            public static int UPxr_SetExpandDeviceCustomData(ref ExpandDevicesCustomData[] deviceArray)
            {
                int val = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                for (int i = 0; i < deviceArray.Length; i++)
                {
                    Pxr_SetExpandDeviceCustomDatabyID(i, deviceArray[i].deviceId, deviceArray.Length, deviceArray[i].data);
                }

                val = Pxr_SetExpandDeviceCustomData(deviceArray.Length);
#endif
                return val;
            }

            public static int UPxr_GetExpandDeviceCustomData(out List<ExpandDevicesCustomData>  _dataArray)
            {
                Int32 Count = 0;
                _dataArray=new List<ExpandDevicesCustomData>();
                int ret = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                 ret = Pxr_GetExpandDeviceCustomData(ref Count);
                if (ret==(int)PxrResult.SUCCESS)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        ExpandDevicesCustomData _data=new ExpandDevicesCustomData();
                        int datasize = 0;
                        IntPtr Handle = IntPtr.Zero;
                        ret = Pxr_GetExpandDeviceCustomDatabyID(i,ref _data.deviceId,ref Handle,ref datasize);
                        if (ret == (int)PxrResult.SUCCESS)
                        {
                            _data.data = new byte[datasize];
                            Marshal.Copy(Handle, _data.data, 0, datasize);
                        }
                        _dataArray.Add(_data);
                    }
                }
#endif
                return ret;
            }

            public static int UPxr_GetExpandDeviceBattery(long deviceid, ref float battery, ref XrBatteryChargingState charger)
            {
                int val = -1;
                int _charger = 0;
#if !UNITY_EDITOR && UNITY_ANDROID
               val = Pxr_GetExpandDeviceBattery(deviceid, ref battery, ref _charger);
#endif
                if (val == 0)
                {
                    charger = (XrBatteryChargingState)_charger;
                }
                PLog.d(TAG, $"UPxr_GetExpandDeviceBattery() battery={battery}, charger={charger}, deviceid={deviceid}, val={val}");
                return val;
            }
            public static int UPxr_GetMotionTrackerBatteryState(long trackerid,ref float batteryLevel,ref XrBatteryChargingState state)
            {
                int val = -1;
                int _charger = 0;
#if !UNITY_EDITOR && UNITY_ANDROID
                val = Pxr_GetMotionTrackerBatteryState(trackerid, ref batteryLevel, ref _charger);
#endif
                if (val == 0)
                {
                    state = (XrBatteryChargingState)_charger;
                }
                return val;
            }
            

            #endregion
        }

        public static class MixedReality
        {
            private const string TAG = "[PXR_Plugin/MixedReality]";
            #region MR 3.0

            public static ulong SpatialAnchorProviderHandle { get; set; }
            public static ulong SceneCaptureProviderHandle { get; set; }
            public static ulong SemiAutoSceneCaptureProviderHandle { get; set; }
            public static ulong AutoSceneCaptureProviderHandle { get; set; }
            public static Dictionary<ulong, PxrSceneComponentData> SceneAnchorData = new Dictionary<ulong, PxrSceneComponentData>();
            public static Dictionary<Guid, PxrSpatialMeshInfo> SpatialMeshData = new Dictionary<Guid, PxrSpatialMeshInfo>();
            public static Dictionary<Guid, ulong> meshAnchorLastData = new Dictionary<Guid, ulong>();
            private static readonly Dictionary<Guid, List<IDisposable>> nativeMeshArrays = new Dictionary<Guid, List<IDisposable>>();

            public static PxrSpatialSceneDataTypeFlags UPxr_ConvertSemanticToSceneFlag(PxrSemanticLabel label)
            {
                switch (label)
                {
                    case PxrSemanticLabel.Unknown:
                        return PxrSpatialSceneDataTypeFlags.Unknown;
                    case PxrSemanticLabel.Floor:
                        return PxrSpatialSceneDataTypeFlags.Floor;
                    case PxrSemanticLabel.Ceiling:
                        return PxrSpatialSceneDataTypeFlags.Ceiling;
                    case PxrSemanticLabel.Wall:
                        return PxrSpatialSceneDataTypeFlags.Wall;
                    case PxrSemanticLabel.Door:
                        return PxrSpatialSceneDataTypeFlags.Door;
                    case PxrSemanticLabel.Window:
                        return PxrSpatialSceneDataTypeFlags.Window;
                    case PxrSemanticLabel.Opening:
                        return PxrSpatialSceneDataTypeFlags.Opening;
                    case PxrSemanticLabel.Table:
                        return PxrSpatialSceneDataTypeFlags.Object;
                    case PxrSemanticLabel.Sofa:
                        return PxrSpatialSceneDataTypeFlags.Object;
                    case PxrSemanticLabel.Chair:
                        return PxrSpatialSceneDataTypeFlags.Object;
                    case PxrSemanticLabel.Human:
                        return PxrSpatialSceneDataTypeFlags.Object;
                    case PxrSemanticLabel.VirtualWall:
                        return PxrSpatialSceneDataTypeFlags.Wall;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(label), label, null);
                }
            }

            public static PxrResult UPxr_ConvertIntToPxrResult(int value)
            {
                if (Enum.IsDefined(typeof(PxrResult), value))
                {
                    return (PxrResult)value;
                }
                else
                {
                    return PxrResult.Unknown;
                }
            }
            public static PxrResult UPxr_CreateSenseDataProvider(ref XrSenseDataProviderCreateInfoBaseHeader info,out ulong providerHandle )
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                var pxrResult = Pxr_CreateSenseDataProvider(ref info, out providerHandle);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                providerHandle = ulong.MinValue;
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_CreateSpatialAnchorSenseDataProvider()
            {
                
#if UNITY_ANDROID && !UNITY_EDITOR
               XrSenseDataProviderCreateInfoBaseHeader header = new XrSenseDataProviderCreateInfoBaseHeader()
                {
                    type =XrStructureType.XR_TYPE_SENSE_DATA_PROVIDER_CREATE_INFO_SPATIAL_ANCHOR
                };
                
                var pxrResult = UPxr_CreateSenseDataProvider(ref header, out var providerHandle);
                SpatialAnchorProviderHandle = providerHandle;
                return pxrResult;
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_CreateSceneCaptureSenseDataProvider()
            {
                
#if UNITY_ANDROID && !UNITY_EDITOR
               XrSenseDataProviderCreateInfoBaseHeader header = new XrSenseDataProviderCreateInfoBaseHeader()
                {
                    type = XrStructureType.XR_TYPE_SENSE_DATA_PROVIDER_CREATE_INFO_SCENE_CAPTURE,
                };

                var pxrResult = UPxr_CreateSenseDataProvider(ref header, out var providerHandle);
                SceneCaptureProviderHandle = providerHandle;
                return pxrResult;
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }
            public static ulong UPxr_GetSpatialMeshProviderHandle()
            {
#if UNITY_ANDROID && !UNITY_EDITOR

                return Pxr_GetSpatialMeshProviderHandle();
#else
                return ulong.MinValue;
#endif
            }

            public static void UPxr_AddOrUpdateMesh(PxrSpatialMeshInfo meshInfo)
            {
                byte[] temp = meshInfo.uuid.ToByteArray();
                var id1 = BitConverter.ToUInt64(temp, 0);
                var id2 = BitConverter.ToUInt64(temp, 8);
                var vertices = new NativeArray<Vector3>(meshInfo.vertices, Allocator.Persistent);
                var indices = new NativeArray<ushort>(meshInfo.indices, Allocator.Persistent);
                
                unsafe
                {
                    Pxr_AddOrUpdateMesh(id1, id2, meshInfo.vertices.Length, NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(vertices), meshInfo.indices.Length, NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(indices), meshInfo.position, meshInfo.rotation);
                }

                if (nativeMeshArrays.TryGetValue(meshInfo.uuid, out var nativeArrays))
                    nativeArrays.ForEach(x => x.Dispose());
                nativeMeshArrays[meshInfo.uuid] = new List<IDisposable> { vertices, indices};
            }

            public static void UPxr_RemoveMesh(Guid uuid)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                byte[] temp = uuid.ToByteArray();
                var id1 = BitConverter.ToUInt64(temp, 0);
                var id2 = BitConverter.ToUInt64(temp, 8);
                Pxr_RemoveMesh(id1, id2);
#endif
                if (nativeMeshArrays.TryGetValue(uuid, out var nativeArrays))
                {
                    nativeArrays.ForEach(x => x.Dispose());
                    nativeMeshArrays.Remove(uuid);
                }
            }

            public static void UPxr_DisposeMesh()
            {
                foreach (var nativeArrays in nativeMeshArrays.Values)
                {
                    nativeArrays.ForEach(x => x.Dispose());
                }

                nativeMeshArrays.Clear();
                UPxr_ClearMeshes();
            }

            public static void UPxr_ClearMeshes()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Pxr_ClearMeshes();
#endif
            }

            public static ulong UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType type)
            {
                switch (type)
                {
                    case PxrSenseDataProviderType.SpatialAnchor:
                        return SpatialAnchorProviderHandle;
                    case PxrSenseDataProviderType.SceneCapture:
                        return SceneCaptureProviderHandle;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }

            public static PxrResult UPxr_StartSenseDataProviderAsync(ulong providerHandle, out ulong future)
            {
                future = UInt64.MinValue;
#if UNITY_ANDROID && !UNITY_EDITOR
                var pxrResult = Pxr_StartSenseDataProviderAsync(providerHandle, out future);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_StartSenseDataProviderComplete(ulong future,out XrSenseDataProviderStartCompletion completion)
            {
                completion = new XrSenseDataProviderStartCompletion()
                {
                    type = XrStructureType.XR_TYPE_SENSE_DATA_PROVIDER_START_COMPLETION,
                };
#if UNITY_ANDROID && !UNITY_EDITOR
                var pxrResult = Pxr_StartSenseDataProviderComplete(future, ref completion);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_GetSenseDataProviderState(ulong providerHandle, out PxrSenseDataProviderState state)
            {
                state = PxrSenseDataProviderState.Stopped;
#if UNITY_ANDROID && !UNITY_EDITOR
                var pxrResult = Pxr_GetSenseDataProviderState(providerHandle,ref state);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#endif
                return PxrResult.ERROR_RUNTIME_FAILURE;
            }

            public static PxrResult UPxr_StopSenseDataProvider(ulong providerHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                var pxrResult = Pxr_StopSenseDataProvider(providerHandle);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_DestroySenseDataProvider(ulong providerHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                var pxrResult = Pxr_DestroySenseDataProvider(providerHandle);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }


            public static PxrResult UPxr_QuerySenseDataAsync(ulong providerHandle,ref XrSenseDataQueryInfo info, out ulong future)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                var pxrResult = Pxr_QuerySenseDataAsync(providerHandle, ref info, out future);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                future = ulong.MinValue;
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_DestroySenseDataQueryResult(ulong queryResultHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                var pxrResult = Pxr_DestroySenseDataQueryResult(queryResultHandle);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_QuerySenseDataByUuidAsync(Guid[] uuids, out ulong future)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                XrSenseDataQueryInfo info = new XrSenseDataQueryInfo()
                {
                    type = XrStructureType.XR_TYPE_SENSE_DATA_QUERY_INFO,
                };
                XrSenseDataFilterUuid uuidFilter = new XrSenseDataFilterUuid()
                {
                    type = XrStructureType.XR_TYPE_SENSE_DATA_FILTER_UUID
                };

                if (uuids.Length > 0)
                {
                    uuidFilter.uuidCount = (uint)uuids.Length;
                    uuidFilter.uuidList = Marshal.AllocHGlobal(uuids.Length * Marshal.SizeOf(typeof(Guid)));
                    byte[] bytes = uuids.SelectMany(g => g.ToByteArray()).ToArray();
                    Marshal.Copy(bytes, 0, uuidFilter.uuidList, uuids.Length * Marshal.SizeOf(typeof(Guid)));
                    int size = Marshal.SizeOf<XrSenseDataFilterUuid>();
                    info.filter = Marshal.AllocHGlobal(size);
                    Marshal.StructureToPtr(uuidFilter, info.filter, false);
                }
                else
                {
                    info.filter = IntPtr.Zero;
                }

                var pxrResult = UPxr_QuerySenseDataAsync(UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SpatialAnchor), ref info, out future);
                Marshal.FreeHGlobal(uuidFilter.uuidList);
                Marshal.FreeHGlobal(info.filter);
                return pxrResult;
#else
                future = ulong.MinValue;
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_QuerySenseDataBySemanticAsync(PxrSemanticLabel[] labels, out ulong future)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                XrSenseDataQueryInfo info = new XrSenseDataQueryInfo()
                {
                    type = XrStructureType.XR_TYPE_SENSE_DATA_QUERY_INFO,
                };
                
                XrSenseDataFilterSemantic semanticFilter = new XrSenseDataFilterSemantic()
                {
                    type = XrStructureType.XR_TYPE_SENSE_DATA_FILTER_SEMANTIC
                };

                if (labels.Length > 0)
                {
                    semanticFilter.semanticCount = (uint)labels.Length;
                    int[] labelsAsInts = labels.Select(x => (int)x).ToArray();
                    semanticFilter.semantics = Marshal.AllocHGlobal(labels.Length * Marshal.SizeOf(typeof(int)));
                    Marshal.Copy(labelsAsInts, 0, semanticFilter.semantics, labelsAsInts.Length);
                    int size = Marshal.SizeOf<XrSenseDataFilterSemantic>();
                    info.filter = Marshal.AllocHGlobal(size);
                    Marshal.StructureToPtr(semanticFilter, info.filter, false);
                }
                else
                {
                    info.filter = IntPtr.Zero;
                }

                var pxrResult = UPxr_QuerySenseDataAsync(UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SceneCapture), ref info, out future);
                Marshal.FreeHGlobal(semanticFilter.semantics);
                Marshal.FreeHGlobal(info.filter);
                return pxrResult;
#else
                future = ulong.MinValue;
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_QuerySenseDataComplete(ulong providerHandle,ulong future,out XrSenseDataQueryCompletion completion)
            {
                completion = new XrSenseDataQueryCompletion()
                {
                    type = XrStructureType.XR_TYPE_SENSE_DATA_QUERY_COMPLETION,
                };
#if UNITY_ANDROID && !UNITY_EDITOR
                var pxrResult = Pxr_QuerySenseDataComplete(providerHandle, future,ref completion);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }


            public static PxrResult UPxr_GetQueriedSenseData(ulong providerHandle, ulong snapshotHandle,out List<PxrQueriedSpatialEntityInfo> entityinfos)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
               XrQueriedSenseDataGetInfo info = new XrQueriedSenseDataGetInfo()
                {
                    type = XrStructureType.XR_TYPE_QUERIED_SENSE_DATA_GET_INFO,
                    snapshotHandle = snapshotHandle
                };

                XrQueriedSenseData senseDataFirst = new XrQueriedSenseData()
                {
                    type = XrStructureType.XR_TYPE_QUERIED_SENSE_DATA_GET_INFO,
                    queriedSpatialEntityCapacityInput = 0,
                    queriedSpatialEntityCountOutput = 0,
                };

                var getResultFirst = Pxr_GetQueriedSenseData(providerHandle, ref info, ref senseDataFirst);
                if (UPxr_ConvertIntToPxrResult(getResultFirst) == PxrResult.SUCCESS)
                {
                    XrQueriedSenseData senseDataSecond = new XrQueriedSenseData()
                    {
                        type = XrStructureType.XR_TYPE_QUERIED_SENSE_DATA_GET_INFO,
                        queriedSpatialEntityCapacityInput = senseDataFirst.queriedSpatialEntityCountOutput,
                        queriedSpatialEntityCountOutput = senseDataFirst.queriedSpatialEntityCountOutput,
                    };
                    int resultSize = Marshal.SizeOf<PxrQueriedSpatialEntityInfo>();
                    int bytesSize = (int)senseDataFirst.queriedSpatialEntityCountOutput * resultSize;
                    senseDataSecond.queriedSpatialEntities = Marshal.AllocHGlobal(bytesSize);
                    var getResultSecond = Pxr_GetQueriedSenseData(providerHandle, ref info, ref senseDataSecond);
                    entityinfos = new List<PxrQueriedSpatialEntityInfo>();
                    if (UPxr_ConvertIntToPxrResult(getResultSecond) == PxrResult.SUCCESS)
                    {
                        for (int i = 0; i < senseDataFirst.queriedSpatialEntityCountOutput; i++)
                        {
                            PxrQueriedSpatialEntityInfo t =
                                (PxrQueriedSpatialEntityInfo)Marshal.PtrToStructure(senseDataSecond.queriedSpatialEntities + i * resultSize,
                                    typeof(PxrQueriedSpatialEntityInfo));
                            entityinfos.Add(t);
                        }
                    }
                    Marshal.FreeHGlobal(senseDataSecond.queriedSpatialEntities);
                    return UPxr_ConvertIntToPxrResult(getResultSecond);
                }
                else
                {
                    entityinfos = new List<PxrQueriedSpatialEntityInfo>();
                    return UPxr_ConvertIntToPxrResult(getResultFirst);
                }
#else
                entityinfos = new List<PxrQueriedSpatialEntityInfo>();
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_EnumerateSpatialEntityComponentTypes(ulong snapshotHandle, ulong spatialEntityHandle, out PxrSceneComponentType[] types)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                var componentTypes = IntPtr.Zero;
                types = Array.Empty<PxrSceneComponentType>();
                var firstResult = Pxr_EnumerateSpatialEntityComponentTypes(snapshotHandle, spatialEntityHandle, 0, out var firstOutputCount, componentTypes);
                if (UPxr_ConvertIntToPxrResult(firstResult) == PxrResult.SUCCESS)
                {
                    int size = (int)firstOutputCount * Marshal.SizeOf(typeof(int));
                    componentTypes = Marshal.AllocHGlobal(size);
                    var secondResult = Pxr_EnumerateSpatialEntityComponentTypes(snapshotHandle, spatialEntityHandle, firstOutputCount,
                        out var outputCount, componentTypes);
                    if (UPxr_ConvertIntToPxrResult(secondResult) == PxrResult.SUCCESS)
                    {
                        types = new PxrSceneComponentType[outputCount];
                        int[] typesInts = new int[outputCount];
                        Marshal.Copy(componentTypes, typesInts, 0, (int)firstOutputCount);
                        for (int i = 0; i < outputCount; i++)
                        {
                            types[i] = (PxrSceneComponentType)typesInts[i];
                        }

                        Marshal.FreeHGlobal(componentTypes);
                        return PxrResult.SUCCESS;
                    }
                    else
                    {
                        types = Array.Empty<PxrSceneComponentType>();
                        Marshal.FreeHGlobal(componentTypes);
                        return UPxr_ConvertIntToPxrResult(secondResult);
                    }
                }
                else
                {
                    types = Array.Empty<PxrSceneComponentType>();
                    return UPxr_ConvertIntToPxrResult(firstResult);
                }
#else
                types = Array.Empty<PxrSceneComponentType>();
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_GetSpatialEntityLocationInfo(ulong snapshotHandle, ulong spatialEntityHandle, out Vector3 position, out Quaternion rotation)
            {
                position = Vector3.zero;
                rotation = Quaternion.identity;
#if UNITY_ANDROID && !UNITY_EDITOR
                var getInfo = new XrSpatialEntityLocationGetInfo
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ENTITY_LOCATION_GET_INFO,
                    entity = spatialEntityHandle,
                    componentType = PxrSceneComponentType.Location,
                    baseSpace = 0,
                    time = 0,
                };
                XrSpatialEntityLocationData locationInfo = new XrSpatialEntityLocationData()
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ENTITY_LOCATION_DATA
                };
                var result = Pxr_GetSpatialEntityLocationInfo(snapshotHandle, ref getInfo, ref locationInfo);
                if (UPxr_ConvertIntToPxrResult(result) == PxrResult.SUCCESS)
                {
                    foreach (PxrSpaceLocationFlags value in Enum.GetValues(typeof(PxrSpaceLocationFlags)))
                    {
                        if ((locationInfo.location.locationFlags & (ulong)value) != (ulong)value)
                        {
                            position = Vector3.zero;
                            rotation = Quaternion.identity;
                            return PxrResult.ERROR_POSE_INVALID;
                        }
                    }
                    rotation = new Quaternion(locationInfo.location.pose.orientation.x, locationInfo.location.pose.orientation.y, -locationInfo.location.pose.orientation.z,
                        -locationInfo.location.pose.orientation.w);
                    position = new Vector3(locationInfo.location.pose.position.x, locationInfo.location.pose.position.y, -locationInfo.location.pose.position.z);
                }
                return UPxr_ConvertIntToPxrResult(result);
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_GetSpatialMesh(ulong snapshotHandle, ulong entityHandle, ref PxrSpatialMeshInfo meshInfo)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                var result = UPxr_GetSpatialMeshVerticesAndIndices(snapshotHandle, entityHandle, out var indices, out var vertices);
                if (result == PxrResult.SUCCESS)
                {
                    meshInfo.indices = indices;
                    meshInfo.vertices = vertices;
                    result = UPxr_GetSpatialSemantics(snapshotHandle, entityHandle, out var labels);
                    if (result == PxrResult.SUCCESS)
                    {
                        meshInfo.labels = labels;
                        result = UPxr_GetSpatialEntityLocationInfo(snapshotHandle, entityHandle, out var position, out var rotation);
                        if (result == PxrResult.SUCCESS)
                        {
                            meshInfo.position = position;
                            meshInfo.rotation = rotation;

                            return PxrResult.SUCCESS;
                        }
                        else
                        {
                            return result;
                        }
                    }
                    else
                    {
                        return result;
                    }
                }
                else
                {
                    return result;
                }
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_GetSpatialMeshVerticesAndIndices(ulong snapshotHandle, ulong entityHandle, out ushort[] indices, out Vector3[] vertices)
            {
                indices = Array.Empty<ushort>();
                vertices = Array.Empty<Vector3>();
#if UNITY_ANDROID && !UNITY_EDITOR
                var getInfo = new XrSpatialEntityGetInfo
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ENTITY_TRIANGLE_MESH_GET_INFO,
                    entity = entityHandle,
                    componentType = PxrSceneComponentType.TriangleMesh
                };

                PxrTriangleMeshInfo meshInfo = new PxrTriangleMeshInfo()
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ENTITY_TRIANGLE_MESH_DATA,
                    vertexCapacityInput = 0,
                    vertexCountOutput = 0,
                    vertices = IntPtr.Zero,
                    indexCapacityInput = 0,
                    indexCountOutput = 0,
                    indices = IntPtr.Zero
                };
    
                var result = Pxr_GetSpatialMeshVerticesAndIndices(snapshotHandle, ref getInfo, ref meshInfo);
                if (UPxr_ConvertIntToPxrResult(result) == PxrResult.SUCCESS)
                {
                 
                    meshInfo.indexCapacityInput = meshInfo.indexCountOutput;
                    meshInfo.indices = Marshal.AllocHGlobal((int)meshInfo.indexCountOutput * Marshal.SizeOf(typeof(ushort)));
                    meshInfo.vertexCapacityInput = meshInfo.vertexCountOutput;
                    meshInfo.vertices = Marshal.AllocHGlobal((int)meshInfo.vertexCountOutput * Marshal.SizeOf(typeof(PxrVector3f)));
                   
                    result = Pxr_GetSpatialMeshVerticesAndIndices(snapshotHandle, ref getInfo, ref meshInfo);
                    if (UPxr_ConvertIntToPxrResult(result) == PxrResult.SUCCESS)
                    {
                       
                        indices = new ushort[meshInfo.indexCountOutput];
                        if (meshInfo.indexCountOutput > 0)
                        {
                            var indicesTmp = new short[meshInfo.indexCountOutput];
                            Marshal.Copy(meshInfo.indices, indicesTmp, 0, (int)meshInfo.indexCountOutput);
                            indices = indicesTmp.Select(l => (ushort)l).ToArray();

                            for (int i = 0; i < indices.Length; i += 3)
                            {
                                (indices[i + 1], indices[i + 2]) = (indices[i + 2], indices[i + 1]);
                            }
                        }
                        vertices = new Vector3[meshInfo.vertexCountOutput];
                        if (meshInfo.vertexCountOutput > 0)
                        {
                            IntPtr tempPtr = meshInfo.vertices;
                            for (int i = 0; i < meshInfo.vertexCountOutput; i++)
                            {
                                vertices[i] = Marshal.PtrToStructure<Vector3>(tempPtr);
                                tempPtr += Marshal.SizeOf(typeof(Vector3));
                            }

                            vertices = vertices.Select(v => new Vector3(v.x, v.y, -v.z)).ToArray();
                        }
                    }
                    
                    Marshal.FreeHGlobal(meshInfo.indices);
                    Marshal.FreeHGlobal(meshInfo.vertices);
                    return UPxr_ConvertIntToPxrResult(result);
                }
               
                return UPxr_ConvertIntToPxrResult(result);
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif

            }

            public static PxrResult UPxr_GetSpatialSemantics(ulong snapshotHandle, ulong spatialEntityHandle, out PxrSemanticLabel[] labels)
            {
                labels = Array.Empty<PxrSemanticLabel>();
#if UNITY_ANDROID && !UNITY_EDITOR
                XrSpatialEntityGetInfo getInfo = new XrSpatialEntityGetInfo
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ENTITY_SEMANTIC_GET_INFO,
                    entity = spatialEntityHandle,
                    componentType = PxrSceneComponentType.Semantic
                };
                XrSpatialEntitySemanticData semanticInfo = new XrSpatialEntitySemanticData()
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ENTITY_SEMANTIC_DATA,
                    semanticCapacityInput = 0,
                    semanticCountOutput = 0,
                    semanticLabels = IntPtr.Zero
                };

                var result = Pxr_GetSpatialEntitySemanticInfo(snapshotHandle, ref getInfo, ref semanticInfo);
                if (UPxr_ConvertIntToPxrResult(result) == PxrResult.SUCCESS)
                {
                    if (semanticInfo.semanticCountOutput > 0)
                    {
                        semanticInfo.semanticCapacityInput = semanticInfo.semanticCountOutput;

                        semanticInfo.semanticLabels = Marshal.AllocHGlobal((int)semanticInfo.semanticCapacityInput * Marshal.SizeOf(typeof(int)));

                        result = Pxr_GetSpatialEntitySemanticInfo(snapshotHandle, ref getInfo, ref semanticInfo);
                        if (UPxr_ConvertIntToPxrResult(result) == PxrResult.SUCCESS)
                        {
                            labels = new PxrSemanticLabel[semanticInfo.semanticCountOutput];
                            var sTmp = new int[semanticInfo.semanticCountOutput];
                            Marshal.Copy(semanticInfo.semanticLabels, sTmp, 0, (int)semanticInfo.semanticCountOutput);
                            labels = sTmp.Select(l => (PxrSemanticLabel)l).ToArray();
                        }

                        Marshal.FreeHGlobal(semanticInfo.semanticLabels);
                        return UPxr_ConvertIntToPxrResult(result);
                    }

                    return UPxr_ConvertIntToPxrResult(result);
                }

                return UPxr_ConvertIntToPxrResult(result);
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_GetSpatialEntitySemanticInfo(ulong snapshotHandle, ulong spatialEntityHandle, out PxrSemanticLabel label)
            {
                label = PxrSemanticLabel.Unknown;
#if UNITY_ANDROID && !UNITY_EDITOR
                PxrResult result = UPxr_GetSpatialSemantics(snapshotHandle, spatialEntityHandle, out var labels);
                if (result == PxrResult.SUCCESS && labels.Length > 0)
                {
                    label = labels[0];
                }

                return result;
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_GetSpatialEntityBox3DInfo(ulong snapshotHandle,ulong spatialEntityHandle,out Vector3 position, out Quaternion rotation, out Vector3 extent)
            {
                position = Vector3.zero;
                rotation = Quaternion.identity;
                extent = Vector3.zero;
#if UNITY_ANDROID && !UNITY_EDITOR
                var getInfo = new XrSpatialEntityGetInfo
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ENTITY_BOUNDING_BOX_3D_GET_INFO,
                    next = IntPtr.Zero,
                    entity = spatialEntityHandle,
                    componentType = PxrSceneComponentType.Box3D
                };
          
                
                XrSpatialEntityBoundingBox3DData box3DInfo = new XrSpatialEntityBoundingBox3DData()
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ENTITY_BOUNDING_BOX_3D_DATA,
                    next = IntPtr.Zero
                };

                var result =  Pxr_GetSpatialEntityBox3DInfo(snapshotHandle, ref getInfo, ref box3DInfo);
                if (UPxr_ConvertIntToPxrResult(result) == PxrResult.SUCCESS)
                {
                    position = new Vector3(box3DInfo.box3D.center.position.x, box3DInfo.box3D.center.position.y, box3DInfo.box3D.center.position.z);
                    rotation = new Quaternion(box3DInfo.box3D.center.orientation.x, box3DInfo.box3D.center.orientation.y, box3DInfo.box3D.center.orientation.z,
                        box3DInfo.box3D.center.orientation.w);
                    extent = new Vector3(box3DInfo.box3D.extents.width, box3DInfo.box3D.extents.height, box3DInfo.box3D.extents.depth);
            
                }
                return UPxr_ConvertIntToPxrResult(result);
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_GetSpatialEntityBox2DInfo(ulong snapshotHandle, ulong spatialEntityHandle, out Vector2 offset, out Vector2 extent)
            {
                offset = Vector2.zero;
                extent = Vector2.zero;
#if UNITY_ANDROID && !UNITY_EDITOR
                var getInfo = new XrSpatialEntityGetInfo
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ENTITY_BOUNDING_BOX_2D_GET_INFO,
                    entity = spatialEntityHandle,
                    componentType = PxrSceneComponentType.Box2D
                };
                
                XrSpatialEntityBoundingBox2DData box2DInfo = new XrSpatialEntityBoundingBox2DData()
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ENTITY_BOUNDING_BOX_2D_DATA,
                };
                
                var result =  Pxr_GetSpatialEntityBox2DInfo(snapshotHandle, ref getInfo, ref box2DInfo);
                if (UPxr_ConvertIntToPxrResult(result) == PxrResult.SUCCESS)
                {
                    offset = box2DInfo.box2D.offset;
                    extent =box2DInfo.box2D.extent.ToVector2();
                }
                return UPxr_ConvertIntToPxrResult(result);
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_GetSpatialEntityPolygonInfo(ulong snapshotHandle, ulong spatialEntityHandle, out Vector2[] vertices)
            {
                vertices = Array.Empty<Vector2>();
#if UNITY_ANDROID && !UNITY_EDITOR
                var getInfo = new XrSpatialEntityGetInfo
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ENTITY_POLYGON_GET_INFO,
                    entity = spatialEntityHandle,
                    componentType = PxrSceneComponentType.Polygon
                };

                XrSpatialEntityPolygonData polygonInfo = new XrSpatialEntityPolygonData()
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ENTITY_POLYGON_DATA,
                    polygonCapacityInput = 0,
                    polygonCountOutput = 0,
                    vertices = IntPtr.Zero
                };

                var result = Pxr_GetSpatialEntityPolygonInfo(snapshotHandle, ref getInfo, ref polygonInfo);

                if (UPxr_ConvertIntToPxrResult(result) == PxrResult.SUCCESS)
                {
                    if (polygonInfo.polygonCountOutput > 0)
                    {
                        polygonInfo.polygonCapacityInput = polygonInfo.polygonCountOutput;
                        polygonInfo.vertices = Marshal.AllocHGlobal((int)polygonInfo.polygonCountOutput * Marshal.SizeOf(typeof(PxrVector2f)));

                        result = Pxr_GetSpatialEntityPolygonInfo(snapshotHandle, ref getInfo, ref polygonInfo);
                        if (UPxr_ConvertIntToPxrResult(result) == PxrResult.SUCCESS)
                        {
                            vertices = new Vector2[polygonInfo.polygonCountOutput];
                            var vector2fs = new PxrVector2f[polygonInfo.polygonCountOutput];
                            for (int i = 0; i < polygonInfo.polygonCountOutput; i++)
                            {
                                vector2fs[i] = Marshal.PtrToStructure<PxrVector2f>(polygonInfo.vertices + i * Marshal.SizeOf(typeof(PxrVector2f)));
                                vertices[i].x = vector2fs[i].x;
                                vertices[i].y = vector2fs[i].y;
                            }
                        }

                        Marshal.FreeHGlobal(polygonInfo.vertices);
                    }
                }

                return UPxr_ConvertIntToPxrResult(result);
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_RetrieveSpatialEntityAnchor(ulong snapshotHandle, ulong spatialEntityHandle,out ulong anchorHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                XrSpatialEntityAnchorRetrieveInfo info = new XrSpatialEntityAnchorRetrieveInfo()
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ENTITY_ANCHOR_RETRIEVE_INFO,
                    spatialEntity = spatialEntityHandle,
                };
                var pxrResult = Pxr_RetrieveSpatialEntityAnchor(snapshotHandle, ref info,out anchorHandle);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                anchorHandle = ulong.MinValue;
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_PollFuture(ulong future,out PxrFutureState futureState)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                XrFuturePollInfoEXT pollInfo = new XrFuturePollInfoEXT()
                {
                    type = XrStructureType.XR_TYPE_FUTURE_POLL_INFO_EXT,
                    future = future,
                };
                XrFuturePollResultEXT pollResult = new XrFuturePollResultEXT()
                {
                    type = XrStructureType.XR_TYPE_FUTURE_POLL_RESULT_EXT,
                };
                var pxrResult =  Pxr_PollFutureEXT(ref pollInfo, ref pollResult);
                futureState = pollResult.state;

                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                futureState = PxrFutureState.Ready;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_PollFutureWithProgress(ulong future, out PxrFutureState futureState,out int progress)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                XrFuturePollInfoEXT pollInfo = new XrFuturePollInfoEXT()
                {
                    type = XrStructureType.XR_TYPE_FUTURE_POLL_INFO_EXT,
                    future = future,
                };

                XrFuturePollResultProgress pollResultProgress = new XrFuturePollResultProgress();

                var pxrResult = Pxr_PollFutureWithProgress(ref pollInfo, ref pollResultProgress);
                futureState = pollResultProgress.state;
                progress = pollResultProgress.progress;

                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                progress = 100;
                futureState = PxrFutureState.Ready;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_CreateSpatialAnchorAsync(ulong providerHandle,Vector3 position, Quaternion rotation, out ulong future)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                PxrPosef pose = new PxrPosef()
                {
                    orientation = new PxrVector4f()
                    {
                        x = rotation.x,
                        y = rotation.y,
                        z = -rotation.z,
                        w = -rotation.w
                    },
                    position = new PxrVector3f()
                    {
                        x = position.x,
                        y = position.y,
                        z = -position.z
                    }
                };
                var pxrResult = Pxr_CreateSpatialAnchorAsync(providerHandle, ref pose, out future);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                future = ulong.MinValue;
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_CreateSpatialAnchorComplete(ulong providerHandle, ulong future,out XrSpatialAnchorCompletion completion)
            {
                completion = new XrSpatialAnchorCompletion()
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ANCHOR_CREATE_COMPLETION
                };
#if UNITY_ANDROID && !UNITY_EDITOR
                var pxrResult = Pxr_CreateSpatialAnchorComplete(providerHandle, future, ref completion);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_DestroyAnchor(ulong anchorHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                var pxrResult = Pxr_DestroyAnchor(anchorHandle);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_GetAnchorUuid(ulong anchorHandle, out Guid uuid)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                var pxrResult = Pxr_GetAnchorUuid(anchorHandle,out var pUuid);
                byte[] byteArray = new byte[16];
                BitConverter.GetBytes(pUuid.value0).CopyTo(byteArray, 0);
                BitConverter.GetBytes(pUuid.value1).CopyTo(byteArray, 8);
                uuid = new Guid(byteArray);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                uuid = Guid.Empty;
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_LocateAnchor(ulong anchorHandle, out Vector3 position, out Quaternion rotation)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                XrSpaceLocation location = new XrSpaceLocation()
                {
                    type = XrStructureType.XR_TYPE_SPACE_LOCATION,
                };
                var pxrResult = Pxr_LocateAnchor(anchorHandle, ref location);
                if (UPxr_ConvertIntToPxrResult(pxrResult) == PxrResult.SUCCESS)
                {
                    foreach (PxrSpaceLocationFlags value in Enum.GetValues(typeof(PxrSpaceLocationFlags)))
                    {
                        if ((location.locationFlags & (ulong)value) != (ulong)value)
                        {
                            position = Vector3.zero;
                            rotation = Quaternion.identity;
                            return PxrResult.ERROR_POSE_INVALID;
                        }
                    }
                    rotation = new Quaternion(location.pose.orientation.x, location.pose.orientation.y, -location.pose.orientation.z, -location.pose.orientation.w);
                    position = new Vector3(location.pose.position.x, location.pose.position.y, -location.pose.position.z);
                    return UPxr_ConvertIntToPxrResult(pxrResult);
                }
                else
                {
                    position = Vector3.zero;
                    rotation = Quaternion.identity;
                    return UPxr_ConvertIntToPxrResult(pxrResult);
                }
#else
                position = Vector3.zero;
                rotation = Quaternion.identity;
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_PersistSpatialAnchorAsync(ulong providerHandle, ulong anchorHandle,out ulong future)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                XrSpatialAnchorPersistInfo persistInfo = new XrSpatialAnchorPersistInfo()
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ANCHOR_PERSIST_INFO,
                    location = PxrPersistenceLocation.Local,
                    anchorHandle = anchorHandle
                };
                var pxrResult = Pxr_PersistSpatialAnchorAsync(providerHandle, ref persistInfo, out future);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                future = ulong.MinValue;
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_PersistSpatialAnchorComplete(ulong providerHandle,ulong future,out XrSpatialAnchorCompletion completion)
            {
                completion = new XrSpatialAnchorCompletion()
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ANCHOR_PERSIST_COMPLETION,
                };
#if UNITY_ANDROID && !UNITY_EDITOR
                var pxrResult = Pxr_PersistSpatialAnchorComplete(providerHandle, future, ref completion);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_UnPersistSpatialAnchorAsync(ulong providerHandle, ulong anchorHandle, out ulong future)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                XrSpatialAnchorUnpersistInfo unPersistInfo = new XrSpatialAnchorUnpersistInfo()
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ANCHOR_UNPERSIST_INFO,
                    location = PxrPersistenceLocation.Local,
                    anchorHandle = anchorHandle
                };
                var pxrResult = Pxr_UnpersistSpatialAnchorAsync(providerHandle, ref unPersistInfo, out future);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                future = ulong.MinValue;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_UnPersistSpatialAnchorComplete(ulong providerHandle,ulong future, out XrSpatialAnchorCompletion completion)
            {
                completion = new XrSpatialAnchorCompletion()
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ANCHOR_UNPERSIST_COMPLETION,
                };
#if UNITY_ANDROID && !UNITY_EDITOR
                var pxrResult = Pxr_UnpersistSpatialAnchorComplete(providerHandle, future, ref completion);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_StartSceneCaptureAsync(out ulong future)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                var pxrResult = Pxr_StartSceneCaptureAsync(UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SceneCapture), out future);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                future = ulong.MinValue;
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_StartSceneCaptureComplete(ulong future,out XrSceneCaptureStartCompletion completion)
            {
                completion = new XrSceneCaptureStartCompletion()
                {
                    type = XrStructureType.XR_TYPE_SCENE_CAPTURE_START_COMPLETION
                };
#if UNITY_ANDROID && !UNITY_EDITOR
                var pxrResult = Pxr_StartSceneCaptureComplete(UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SceneCapture), future,ref completion);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_ShareSpatialAnchorAsync(ulong anchorHandle, out ulong future)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                XrSpatialAnchorShareInfo info = new XrSpatialAnchorShareInfo()
                {
                    type =XrStructureType.XR_TYPE_SPATIAL_ANCHOR_SHARE_INFO,
                    anchorHandle = anchorHandle,
                };
                var pxrResult = Pxr_ShareSpatialAnchorAsync(UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SpatialAnchor), ref info, out future);

                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                 future = ulong.MinValue;
                 return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_ShareSpatialAnchorComplete(ulong future, out XrSpatialAnchorShareCompletion completion)
            {
                completion = new XrSpatialAnchorShareCompletion()
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ANCHOR_SHARE_COMPLETION,
                };
#if UNITY_ANDROID && !UNITY_EDITOR
                var pxrResult = Pxr_ShareSpatialAnchorComplete(UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SpatialAnchor), future, ref completion);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_DownloadSharedSpatialAnchorsAsync(Guid uuid, out ulong future)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                byte[] bytes = uuid.ToByteArray();
                XrSharedSpatialAnchorDownloadInfo info = new XrSharedSpatialAnchorDownloadInfo()
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ANCHOR_DOWNLOAD_INFO,
                    uuid = new PxrUuid()
                    {
                        value0 = BitConverter.ToUInt64(bytes, 0),
                        value1 = BitConverter.ToUInt64(bytes, 8)
                    },
                };
                var pxrResult = Pxr_DownloadSharedSpatialAnchorAsync(UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SpatialAnchor), ref info, out future);

                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                future = ulong.MinValue;
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            public static PxrResult UPxr_DownloadSharedSpatialAnchorsComplete(ulong future, out XrSpatialAnchorShareCompletion completion)
            {
                completion = new XrSpatialAnchorShareCompletion()
                {
                    type = XrStructureType.XR_TYPE_SPATIAL_ANCHOR_DOWNLOAD_COMPLETION,
                };
#if UNITY_ANDROID && !UNITY_EDITOR
                var pxrResult = Pxr_DownloadSharedSpatialAnchorComplete(UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SpatialAnchor), future, ref completion);
                return UPxr_ConvertIntToPxrResult(pxrResult);
#else
                return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
            }

            #endregion

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

        public static class SecureMR
        {
            public const string TAG = "SecureMR";

            public static readonly Dictionary<Type, SecureMRTensorDataType> TensorDataTypeToEnum = new Dictionary<Type, SecureMRTensorDataType>
            {
                { typeof(char), SecureMRTensorDataType.Byte },
                { typeof(byte), SecureMRTensorDataType.Byte },
                { typeof(sbyte), SecureMRTensorDataType.Sbyte },
                { typeof(ushort), SecureMRTensorDataType.Ushort },
                { typeof(short), SecureMRTensorDataType.Short },
                { typeof(int), SecureMRTensorDataType.Int },
                { typeof(float), SecureMRTensorDataType.Float },
                { typeof(double), SecureMRTensorDataType.Double },
            };

            public static readonly Dictionary<Type, SecureMRTensorUsage> TensorClassToEnum = new Dictionary<Type, SecureMRTensorUsage>
            {
                { typeof(Scalar), SecureMRTensorUsage.Scalar },
                { typeof(Point), SecureMRTensorUsage.Point },
                { typeof(Slice), SecureMRTensorUsage.Slice },
                { typeof(PXR.SecureMR.Color), SecureMRTensorUsage.Color },
                { typeof(TimeStamp), SecureMRTensorUsage.TimeStamp },
                { typeof(Matrix), SecureMRTensorUsage.Matrix }
            };
            
            public static readonly Dictionary<SecureMRTensorUsage, Type> TensorEnumToClass = new Dictionary<SecureMRTensorUsage, Type>
            {
                { SecureMRTensorUsage.Scalar, typeof(Scalar) },
                {  SecureMRTensorUsage.Point,typeof(Point) },
                {  SecureMRTensorUsage.Slice,typeof(Slice) },
                {  SecureMRTensorUsage.Color,typeof(PXR.SecureMR.Color) },
                { SecureMRTensorUsage.TimeStamp,typeof(TimeStamp)},
                {  SecureMRTensorUsage.Matrix ,typeof(Matrix)}
            };

            public static readonly Dictionary<Type, SecureMROperatorType> OperatorClassToEnum = new Dictionary<Type, SecureMROperatorType>
            {
                { typeof(ArithmeticComposeOperator), SecureMROperatorType.ArithmeticCompose },
                { typeof(ElementwiseMinOperator), SecureMROperatorType.ElementwiseMin },
                { typeof(ElementwiseMaxOperator), SecureMROperatorType.ElementwiseMax },
                { typeof(ElementwiseMultiplyOperator), SecureMROperatorType.ElementwiseMultiply },
                { typeof(CustomizedCompareOperator), SecureMROperatorType.CustomizedCompare },
                { typeof(ElementwiseOrOperator), SecureMROperatorType.ElementwiseOr },
                { typeof(ElementwiseAndOperator), SecureMROperatorType.ElementwiseAnd },
                { typeof(AllOperator), SecureMROperatorType.All },
                { typeof(AnyOperator), SecureMROperatorType.Any },
                { typeof(NmsOperator), SecureMROperatorType.Nms },
                { typeof(SolvePnPOperator), SecureMROperatorType.SolvePnP },
                { typeof(GetAffineOperator), SecureMROperatorType.GetAffine },
                { typeof(ApplyAffineOperator), SecureMROperatorType.ApplyAffine },
                { typeof(ApplyAffinePointOperator), SecureMROperatorType.ApplyAffinePoint },
                { typeof(UvTo3DInCameraSpaceOperator), SecureMROperatorType.UvTo3DInCameraSpace },
                { typeof(AssignmentOperator), SecureMROperatorType.Assignment },
                { typeof(RunModelInferenceOperator), SecureMROperatorType.RunModelInference },
                { typeof(NormalizeOperator), SecureMROperatorType.Normalize },
                { typeof(CameraSpaceToWorldOperator), SecureMROperatorType.CameraSpaceToWorld },
                { typeof(RectifiedVstAccessOperator), SecureMROperatorType.RectifiedVstAccess },
                { typeof(ArgmaxOperator), SecureMROperatorType.Argmax },
                { typeof(ConvertColorOperator), SecureMROperatorType.ConvertColor },
                { typeof(SortVectorOperator), SecureMROperatorType.SortVector },
                { typeof(InversionOperator), SecureMROperatorType.Inversion },
                { typeof(GetTransformMatrixOperator), SecureMROperatorType.GetTransformMatrix },
                { typeof(SortMatrixOperator), SecureMROperatorType.SortMatrix },
                { typeof(SwitchGltfRenderStatusOperator), SecureMROperatorType.SwitchGltfRenderStatus },
                { typeof(UpdateGltfOperator), SecureMROperatorType.UpdateGltf },
                { typeof(RenderTextOperator), SecureMROperatorType.RenderText },
                { typeof(LoadTextureOperator), SecureMROperatorType.LoadTexture },
            };
            
            public static PxrResult UPxr_CreateSecureMRProvider(int width, int height, out ulong providerHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                var result = Pxr_CreateSecureMRFramework(width, height, out providerHandle);

                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                providerHandle = 0;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_DestroySecureMRProvider(ulong providerHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                var result = Pxr_DestroySecureMRFramework(providerHandle);

                return MixedReality.UPxr_ConvertIntToPxrResult(result);

#else
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_CreateSecureMRPipeline(ulong providerHandle, out ulong pipelineHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                var result = Pxr_CreateSecureMRPipeline(providerHandle, out pipelineHandle);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                pipelineHandle = 0;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_DestroySecureMRPipeline(ulong pipelineHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                var result = Pxr_DestroySecureMRPipeline(pipelineHandle);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_SetSecureMROperatorOperandByName(ulong pipelineHandle, ulong operatorHandle, ulong tensorHandle, string name)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                var result = Pxr_SetSecureMROperatorOperandByName(pipelineHandle, operatorHandle, tensorHandle, name);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_SetSecureMROperatorResultByName(ulong pipelineHandle, ulong operatorHandle, ulong tensorHandle, string name)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                var result = Pxr_SetSecureMROperatorResultByName(pipelineHandle, operatorHandle, tensorHandle, name);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_ExecuteSecureMRPipeline(ulong pipelineHandle, Dictionary<ulong, ulong> tensorMappings, out ulong pipelineRunHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMRPipelineExecuteParameter parameter = new SecureMRPipelineExecuteParameter
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_PIPELINE_EXECUTE_PARAMETER_PICO,
                    next = IntPtr.Zero,
                    pipelineRunToBeWaited = 0,
                    conditionTensor = 0,
                    pairCount = 0,
                    pipelineIOPair = IntPtr.Zero
                };

                if (tensorMappings != null && tensorMappings.Count > 0)
                {
                    parameter.pairCount = (uint)tensorMappings.Count;
                    List<SecureMRPipelineIOPair> pairs = new List<SecureMRPipelineIOPair>();
                    foreach (var tensorMapping in tensorMappings)
                    {
                        SecureMRPipelineIOPair pair = new SecureMRPipelineIOPair
                        {
                            type = XrStructureType.XR_TYPE_SECURE_MR_PIPELINE_IO_PAIR_PICO,
                            next = IntPtr.Zero,
                            localPlaceHolderTensor = tensorMapping.Key,
                            globalTensor = tensorMapping.Value,
                        };
                        pairs.Add(pair);
                    }

                    int structSize = Marshal.SizeOf(typeof(SecureMRPipelineIOPair));
                    parameter.pipelineIOPair = Marshal.AllocHGlobal(structSize * tensorMappings.Count);
                    for (int i = 0; i < tensorMappings.Count; i++)
                    {
                        IntPtr temp = parameter.pipelineIOPair + i * structSize;
                        Marshal.StructureToPtr(pairs[i], temp, false);
                    }
                }

                var result = Pxr_ExecuteSecureMRPipeline(pipelineHandle, ref parameter, out pipelineRunHandle);

                Marshal.FreeHGlobal(parameter.pipelineIOPair);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                pipelineRunHandle = 0;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_ExecuteSecureMRPipelineAfter(ulong pipelineHandle, ulong lastPipelineRunHandle, Dictionary<ulong, ulong> tensorMappings, out ulong pipelineRunHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMRPipelineExecuteParameter parameter = new SecureMRPipelineExecuteParameter
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_PIPELINE_EXECUTE_PARAMETER_PICO,
                    next = IntPtr.Zero,
                    pipelineRunToBeWaited = lastPipelineRunHandle,
                    conditionTensor = 0,
                };
                parameter.pairCount = (uint)tensorMappings.Count;

                List<SecureMRPipelineIOPair> pairs = new List<SecureMRPipelineIOPair>();
                foreach (var tensorMapping in tensorMappings)
                {
                    SecureMRPipelineIOPair pair = new SecureMRPipelineIOPair
                    {
                        type = XrStructureType.XR_TYPE_SECURE_MR_PIPELINE_IO_PAIR_PICO,
                        next = IntPtr.Zero,
                        localPlaceHolderTensor = tensorMapping.Key,
                        globalTensor = tensorMapping.Value,
                    };
                    pairs.Add(pair);
                }

                int structSize = Marshal.SizeOf(typeof(SecureMRPipelineIOPair));
                parameter.pipelineIOPair = Marshal.AllocHGlobal(structSize * tensorMappings.Count);
                for (int i = 0; i < tensorMappings.Count; i++)
                {
                    IntPtr temp = parameter.pipelineIOPair + i * structSize;
                    Marshal.StructureToPtr(pairs[i], temp, false);
                }

                var result = Pxr_ExecuteSecureMRPipeline(pipelineHandle, ref parameter, out pipelineRunHandle);

                Marshal.FreeHGlobal(parameter.pipelineIOPair);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                pipelineRunHandle = 0;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_ExecuteSecureMRPipelineConditional(ulong pipelineHandle, ulong conditionTensor, Dictionary<ulong, ulong> tensorMappings, out ulong pipelineRunHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMRPipelineExecuteParameter parameter = new SecureMRPipelineExecuteParameter
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_PIPELINE_EXECUTE_PARAMETER_PICO,
                    next = IntPtr.Zero,
                    pipelineRunToBeWaited = 0,
                    conditionTensor = conditionTensor,
                };
                parameter.pairCount = (uint)tensorMappings.Count;

                List<SecureMRPipelineIOPair> pairs = new List<SecureMRPipelineIOPair>();
                foreach (var tensorMapping in tensorMappings)
                {
                    SecureMRPipelineIOPair pair = new SecureMRPipelineIOPair
                    {
                        type = XrStructureType.XR_TYPE_SECURE_MR_PIPELINE_IO_PAIR_PICO,
                        next = IntPtr.Zero,
                        localPlaceHolderTensor = tensorMapping.Key,
                        globalTensor = tensorMapping.Value,
                    };
                    pairs.Add(pair);
                }

                int structSize = Marshal.SizeOf(typeof(SecureMRPipelineIOPair));
                parameter.pipelineIOPair = Marshal.AllocHGlobal(structSize * tensorMappings.Count);
                for (int i = 0; i < tensorMappings.Count; i++)
                {
                    IntPtr temp = parameter.pipelineIOPair + i * structSize;
                    Marshal.StructureToPtr(pairs[i], temp, false);
                }

                var result = Pxr_ExecuteSecureMRPipeline(pipelineHandle, ref parameter, out pipelineRunHandle);

                Marshal.FreeHGlobal(parameter.pipelineIOPair);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                pipelineRunHandle = 0;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_CreateSecureMRTensorByShape(ulong frameworkHandle, SecureMRTensorDataType dataType, int[] dimensions, sbyte channel, SecureMRTensorUsage usage, out ulong tensorHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMRTensorCreateInfoShape createInfo = new SecureMRTensorCreateInfoShape
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_TENSOR_CREATE_INFO_SHAPE_PICO,
                    next = IntPtr.Zero,
                    placeHolder = false,
                    dimensionsCount = (uint)dimensions.Length,
                    dimensions = IntPtr.Zero,
                    format = IntPtr.Zero
                };

                SecureMRTensorFormat tensorFormat = new SecureMRTensorFormat
                {
                    dataType = dataType,
                    channel = channel,
                    tensorUsage = usage,
                };

                if (dimensions.Length > 0)
                {
                    int size = Marshal.SizeOf(typeof(int)) * dimensions.Length;
                    createInfo.dimensions = Marshal.AllocHGlobal(size);
                    Marshal.Copy(dimensions, 0, createInfo.dimensions, size);
                }

                int size1 = Marshal.SizeOf(typeof(SecureMRTensorFormat));
                createInfo.format = Marshal.AllocHGlobal(size1);
                Marshal.StructureToPtr(tensorFormat, createInfo.format, false);

                var result = Pxr_CreateSecureMRTensorByShape(frameworkHandle, createInfo, out tensorHandle);
                if (dimensions.Length > 0)
                {
                    Marshal.FreeHGlobal(createInfo.dimensions);
                }
                
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                tensorHandle = 0;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_CreateSecureMRPipelineTensorByShape(ulong pipelineHandle, bool placeHolder, SecureMRTensorDataType dataType, int[] dimensions, sbyte channel, SecureMRTensorUsage usage, out ulong tensorHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMRTensorCreateInfoShape createInfo = new SecureMRTensorCreateInfoShape
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_TENSOR_CREATE_INFO_SHAPE_PICO,
                    next = IntPtr.Zero,
                    placeHolder = placeHolder,
                    dimensionsCount = 0,
                    dimensions = IntPtr.Zero,
                    format = IntPtr.Zero
                };

                SecureMRTensorFormat tensorFormat = new SecureMRTensorFormat
                {
                    dataType = dataType,
                    channel = channel,
                    tensorUsage = usage,
                };

                if (dimensions.Length > 0)
                {
                    createInfo.dimensionsCount = (uint)dimensions.Length;
                    int size = Marshal.SizeOf(typeof(int)) * dimensions.Length;
                    createInfo.dimensions = Marshal.AllocHGlobal(size);
                    Marshal.Copy(dimensions, 0, createInfo.dimensions, size);
                }

                int size1 = Marshal.SizeOf(typeof(SecureMRTensorFormat));
                createInfo.format = Marshal.AllocHGlobal(size1);
                Marshal.StructureToPtr(tensorFormat, createInfo.format, false);
                var result = Pxr_CreateSecureMRPipelineTensorByShape(pipelineHandle, createInfo, out tensorHandle);
                if (dimensions.Length > 0)
                {
                    Marshal.FreeHGlobal(createInfo.dimensions);
                }
                
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                tensorHandle = 0;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_CreateSecureMRTensorByGltf(ulong frameworkHandle, byte[] gltfData, out ulong tensorHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMRTensorCreateInfoGltf createInfo = new SecureMRTensorCreateInfoGltf
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_TENSOR_CREATE_INFO_GLTF_PICO,
                    next = IntPtr.Zero,
                    placeHolder = false,
                };
                int size = Marshal.SizeOf(typeof(byte)) * gltfData.Length;

                createInfo.bufferSize = (uint)size;
                createInfo.buffer = Marshal.AllocHGlobal(size);
                Marshal.Copy(gltfData, 0, createInfo.buffer, size);

                var result = Pxr_CreateSecureMRTensorByGltf(frameworkHandle, createInfo, out tensorHandle);
                Marshal.FreeHGlobal(createInfo.buffer);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                tensorHandle = 0;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_CreateSecureMRPipelineTensorByGltf(ulong pipelineHandle, bool placeHolder, byte[] gltfData, out ulong tensorHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMRTensorCreateInfoGltf createInfo = new SecureMRTensorCreateInfoGltf
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_TENSOR_CREATE_INFO_GLTF_PICO,
                    next = IntPtr.Zero,
                    placeHolder = placeHolder,
                    bufferSize = 0,
                    buffer = IntPtr.Zero
                };

                if (gltfData!=null && gltfData.Length > 0)
                {
                    int size = Marshal.SizeOf(typeof(byte)) * gltfData.Length;
                    createInfo.bufferSize = (uint)size;
                    createInfo.buffer = Marshal.AllocHGlobal(size);
                    Marshal.Copy(gltfData, 0, createInfo.buffer, size);
                }
                var result = Pxr_CreateSecureMRPipelineTensorByGltf(pipelineHandle, createInfo, out tensorHandle);
                if (gltfData!=null && gltfData.Length > 0)
                {
                    Marshal.FreeHGlobal(createInfo.buffer);
                }
                
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                tensorHandle = 0;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_ResetSecureMRTensor<T>(ulong tensorHandle, T[] tensorData)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMRTensorBuffer buffer = new SecureMRTensorBuffer
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_TENSOR_BUFFER_PICO,
                    next = IntPtr.Zero,
                    bufferSize = 0
                };

                int size = Marshal.SizeOf(typeof(T)) * tensorData.Length;
                buffer.bufferSize = (uint)size;
                buffer.buffer = Marshal.AllocHGlobal(size);
                if (typeof(T) == typeof(byte))
                {
                    Marshal.Copy(tensorData as byte[], 0, buffer.buffer, tensorData.Length);
                }
                else if (typeof(T) == typeof(sbyte))
                {
                    sbyte[] sbyteArray = tensorData as sbyte[];
                    byte[] byteArray = new byte[size];
                    Buffer.BlockCopy(sbyteArray, 0, byteArray, 0, size);
                    Marshal.Copy(byteArray, 0, buffer.buffer, size);
                }
                else if (typeof(T) == typeof(short))
                {
                    Marshal.Copy(tensorData as short[], 0, buffer.buffer, tensorData.Length);
                }
                else if (typeof(T) == typeof(ushort))
                {
                    ushort[] ushortArray = tensorData as ushort[];
                    byte[] byteArray = new byte[size];
                    Buffer.BlockCopy(ushortArray, 0, byteArray, 0, size);
                    Marshal.Copy(byteArray, 0, buffer.buffer, size);
                }
                else if (typeof(T) == typeof(int))
                {
                    Marshal.Copy(tensorData as int[], 0, buffer.buffer, tensorData.Length);
                }
                else if (typeof(T) == typeof(float))
                {
                    Marshal.Copy(tensorData as float[], 0, buffer.buffer, tensorData.Length);
                }
                else if (typeof(T) == typeof(double))
                {
                    double[] doubleArray = tensorData as double[];
                    byte[] byteArray = new byte[size];
                    Buffer.BlockCopy(doubleArray, 0, byteArray, 0, size);
                    Marshal.Copy(byteArray, 0, buffer.buffer, size);
                }

                var result = Pxr_ResetSecureMRTensor(tensorHandle, ref buffer);

                Marshal.FreeHGlobal(buffer.buffer);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_ResetSecureMRPipelineTensor<T>(ulong pipelineHandle, ulong tensorHandle, T[] tensorData)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMRTensorBuffer buffer = new SecureMRTensorBuffer
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_TENSOR_BUFFER_PICO,
                    next = IntPtr.Zero,
                };

                int size = Marshal.SizeOf(typeof(T)) * tensorData.Length;
                buffer.bufferSize = (uint)size;
                buffer.buffer = Marshal.AllocHGlobal(size);
                if (typeof(T) == typeof(byte))
                {
                    Marshal.Copy(tensorData as byte[], 0, buffer.buffer, tensorData.Length);
                }
                else if (typeof(T) == typeof(sbyte))
                {
                    sbyte[] sbyteArray = tensorData as sbyte[];
                    byte[] byteArray = new byte[size];
                    Buffer.BlockCopy(sbyteArray, 0, byteArray, 0, size);
                    Marshal.Copy(byteArray, 0, buffer.buffer, size);
                }
                else if (typeof(T) == typeof(short))
                {
                    Marshal.Copy(tensorData as short[], 0, buffer.buffer, tensorData.Length);
                }
                else if (typeof(T) == typeof(ushort))
                {
                    ushort[] ushortArray = tensorData as ushort[];
                    byte[] byteArray = new byte[size];
                    Buffer.BlockCopy(ushortArray, 0, byteArray, 0, size);
                    Marshal.Copy(byteArray, 0, buffer.buffer, size);
                }
                else if (typeof(T) == typeof(int))
                {
                    Marshal.Copy(tensorData as int[], 0, buffer.buffer, tensorData.Length);
                }
                else if (typeof(T) == typeof(float))
                {
                    Marshal.Copy(tensorData as float[], 0, buffer.buffer, tensorData.Length);
                }
                else if (typeof(T) == typeof(double))
                {
                    double[] doubleArray = tensorData as double[];
                    byte[] byteArray = new byte[size];
                    Buffer.BlockCopy(doubleArray, 0, byteArray, 0, size);
                    Marshal.Copy(byteArray, 0, buffer.buffer, size);
                }

                var result = Pxr_ResetSecureMRPipelineTensor(pipelineHandle, tensorHandle, ref buffer);

                Marshal.FreeHGlobal(buffer.buffer);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_DestroySecureMRTensor(ulong tensorHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                var result = Pxr_DestroySecureMRTensor(tensorHandle);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_CreateSecureMROperator(ulong pipelineHandle, SecureMROperatorType operatorType, out ulong operatorHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                var result = Pxr_CreateSecureMROperator(pipelineHandle, operatorType, out operatorHandle);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                operatorHandle = 0;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_CreateSecureMROperatorArithmeticCompose(ulong pipelineHandle, string configText, out ulong operatorHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMROperatorArithmeticCompose config = new SecureMROperatorArithmeticCompose
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_ARITHMETIC_COMPOSE_PICO,
                    next = IntPtr.Zero,
                };

                config.configText = new byte[2048];
                byte[] stringBytes = Encoding.UTF8.GetBytes(configText);
                int copyLength = Math.Min(stringBytes.Length, 2048);
                Array.Copy(stringBytes,config.configText,copyLength);

                var result = Pxr_CreateSecureMROperatorArithmeticCompose(pipelineHandle, ref config, out operatorHandle);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                operatorHandle = 0;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_CreateSecureMROperatorComparison(ulong pipelineHandle, SecureMRComparison comparison, out ulong operatorHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMROperatorComparison config = new SecureMROperatorComparison
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_COMPARISON_PICO,
                    next = IntPtr.Zero,
                    comparison = comparison
                };
                var result = Pxr_CreateSecureMROperatorComparison(pipelineHandle, ref config, out operatorHandle);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                operatorHandle = 0;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_CreateSecureMROperatorNonMaximumSuppression(ulong pipelineHandle, float threshold, out ulong operatorHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMROperatorNonMaximumSuppression config = new SecureMROperatorNonMaximumSuppression
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_NON_MAXIMUM_SUPPRESSION_PICO,
                    next = IntPtr.Zero,
                    threshold = threshold,
                };
                var result = Pxr_CreateSecureMROperatorNonMaximumSuppression(pipelineHandle, ref config, out operatorHandle);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                operatorHandle = 0;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_CreateSecureMROperatorUVTo3D(ulong pipelineHandle, out ulong operatorHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                var result = Pxr_CreateSecureMROperatorUVTo3D(pipelineHandle, out operatorHandle);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                operatorHandle = 0;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_CreateSecureMROperatorNormalize(ulong pipelineHandle, SecureMRNormalizeType normalizeType, out ulong operatorHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMROperatorNormalize config = new SecureMROperatorNormalize
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_NORMALIZE_PICO,
                    next = IntPtr.Zero,
                    normalizeType = normalizeType
                };
                var result = Pxr_CreateSecureMROperatorNormalize(pipelineHandle, ref config, out operatorHandle);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                operatorHandle = 0;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_CreateSecureMROperatorColorConvert(ulong pipelineHandle, int convert, out ulong operatorHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMROperatorColorConvert config = new SecureMROperatorColorConvert
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_COLOR_CONVERT_PICO,
                    next = IntPtr.Zero,
                    convert = convert,
                };
                var result = Pxr_CreateSecureMROperatorColorConvert(pipelineHandle, ref config, out operatorHandle);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                operatorHandle = 0;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_CreateSecureMROperatorSortMatrix(ulong pipelineHandle, SecureMRMatrixSortType sortType, out ulong operatorHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMROperatorSortMatrix config = new SecureMROperatorSortMatrix
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_SORT_MATRIX_PICO,
                    next = IntPtr.Zero,
                    sortType = sortType,
                };
                var result = Pxr_CreateSecureMROperatorSortMatrix(pipelineHandle, ref config, out operatorHandle);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                operatorHandle = 0;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_CreateSecureMROperatorUpdateGltf(ulong pipelineHandle, SecureMRGltfOperatorAttribute attribute, out ulong operatorHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMROperatorUpdateGltf config = new SecureMROperatorUpdateGltf
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_UPDATE_GLTF_PICO,
                    next = IntPtr.Zero,
                    attribute = attribute
                };
                var result = Pxr_CreateSecureMROperatorUpdateGltf(pipelineHandle, ref config, out operatorHandle);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                operatorHandle = 0;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_CreateSecureMROperatorRenderText(ulong pipelineHandle, SecureMRFontTypeface typeface, string languageAndLocale, int width, int height, out ulong operatorHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMROperatorRenderText config = new SecureMROperatorRenderText
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_RENDER_TEXT_PICO,
                    next = IntPtr.Zero,
                    typeFace = typeface,
                    languageAndLocale = languageAndLocale,
                    width = width,
                    height = height
                };
                var result = Pxr_CreateSecureMROperatorRenderText(pipelineHandle, ref config, out operatorHandle);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                operatorHandle = 0;
                return PxrResult.SUCCESS;
#endif
            }

            public static PxrResult UPxr_CreateSecureMrOperatorModel(ulong pipelineHandle, List<SecureMROperatorModelConfig> inputConfigs, List<SecureMROperatorModelConfig> outputConfigs, byte[] modelData, SecureMRModelType modelType, string modelName, out ulong operatorHandle)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                SecureMROperatorModel model = new SecureMROperatorModel
                {
                    type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_MODEL_PICO,
                    next = IntPtr.Zero,
                    modelInputCount = (uint)inputConfigs.Count,
                    modelOutputCount = (uint)outputConfigs.Count,
                    bufferSize = (uint)modelData.Length,
                    modelType = modelType,
                    modelName = modelName,
                };

                //input config
                List<SecureMROperatorIOMap> inputPairs = new List<SecureMROperatorIOMap>();
                foreach (var inputConfig in inputConfigs)
                {
                    SecureMROperatorIOMap inputPair = new SecureMROperatorIOMap
                    {
                        type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_IO_MAP_PICO,
                        next = IntPtr.Zero,
                        encodingType = inputConfig.encodingType
                    };

                    inputPair.nodeName = new byte[512];
                    byte[] nodeNameBytes = Encoding.UTF8.GetBytes(inputConfig.nodeName);
                    int copyLength = Math.Min(nodeNameBytes.Length, 512);
                    Array.Copy(nodeNameBytes, inputPair.nodeName, copyLength);

                    inputPair.operatorIOName = new byte[512];
                    byte[] ioNameBytes = Encoding.UTF8.GetBytes(inputConfig.nodeName);
                    copyLength = Math.Min(ioNameBytes.Length, 512);
                    Array.Copy(ioNameBytes, inputPair.operatorIOName, copyLength);

                    inputPairs.Add(inputPair);
                }
                int structSize = Marshal.SizeOf(typeof(SecureMROperatorIOMap));
                model.modelInputs = Marshal.AllocHGlobal(structSize * inputConfigs.Count);
                for (int i = 0; i < inputConfigs.Count; i++)
                {
                    IntPtr temp = model.modelInputs + i * structSize;
                    Marshal.StructureToPtr(inputPairs[i], temp, false);
                }

                //output config
                List<SecureMROperatorIOMap> outputPairs = new List<SecureMROperatorIOMap>();
                foreach (var outputConfig in outputConfigs)
                {
                    SecureMROperatorIOMap outputPair = new SecureMROperatorIOMap
                    {
                        type = XrStructureType.XR_TYPE_SECURE_MR_OPERATOR_IO_MAP_PICO,
                        next = IntPtr.Zero,
                        encodingType = outputConfig.encodingType
                    };

                    outputPair.nodeName = new byte[512];
                    byte[] nodeNameBytes = Encoding.UTF8.GetBytes(outputConfig.nodeName);
                    int copyLength = Math.Min(nodeNameBytes.Length, 512);
                    Array.Copy(nodeNameBytes, outputPair.nodeName, copyLength);

                    outputPair.operatorIOName = new byte[512];
                    byte[] ioNameBytes = Encoding.UTF8.GetBytes(outputConfig.nodeName);
                    copyLength = Math.Min(ioNameBytes.Length, 512);
                    Array.Copy(ioNameBytes, outputPair.operatorIOName, copyLength);

                    outputPairs.Add(outputPair);
                }
                model.modelOutputs = Marshal.AllocHGlobal(structSize * outputConfigs.Count);
                for (int i = 0; i < outputConfigs.Count; i++)
                {
                    IntPtr temp = model.modelOutputs + i * structSize;
                    Marshal.StructureToPtr(outputPairs[i], temp, false);
                }

                //modelData
                int size = Marshal.SizeOf(typeof(byte)) * modelData.Length;
                model.buffer = Marshal.AllocHGlobal(size);
                Marshal.Copy(modelData, 0, model.buffer, size);

                var result = Pxr_CreateSecureMROperatorModel(pipelineHandle, ref model, out operatorHandle);
                Marshal.FreeHGlobal(model.buffer);
                Marshal.FreeHGlobal(model.modelInputs);
                Marshal.FreeHGlobal(model.modelOutputs);
                return MixedReality.UPxr_ConvertIntToPxrResult(result);
#else
                operatorHandle = 0;
                return PxrResult.SUCCESS;
#endif
            }
        }
    }
}