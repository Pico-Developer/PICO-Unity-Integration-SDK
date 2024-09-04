using Unity.XR.PXR;
#if AR_FOUNDATION
using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public class PXR_FaceSubsystem : XRFaceSubsystem
{
    internal const string k_SubsystemId = "PXR_FaceSubsystem";

    internal const int FACE_COUNT = 52;
    internal const int LIPSYNC_COUNT = 20;

    public override TrackableChanges<XRFace> GetChanges(Allocator allocator)
    {
        return base.GetChanges(allocator);
    }

    public unsafe static int GetBlendShapeCoefficients(ref PxrFaceTrackingInfo ftInfo)
    {
        PXR_System.GetFaceTrackingData(0, GetDataType.PXR_GET_FACE_DATA_DEFAULT, ref ftInfo);
        return 0;
    }

    class FaceProvider : Provider
    {
        bool isFaceTrackingSupported = false;
        int inited;

        int supportedModesCount;
        FaceTrackingMode[] supportedModes;
        public override int supportedFaceCount => base.supportedFaceCount;

        public override int requestedMaximumFaceCount { get => base.requestedMaximumFaceCount; set => base.requestedMaximumFaceCount = value; }

        public override int currentMaximumFaceCount => base.currentMaximumFaceCount;

        public override void Destroy()
        {
            PLog.i(k_SubsystemId, "Destroy");
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public unsafe override TrackableChanges<XRFace> GetChanges(XRFace defaultFace, Allocator allocator)
        {
            return new TrackableChanges<XRFace>();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void Start()
        {
            PXR_MotionTracking.GetFaceTrackingSupported(ref isFaceTrackingSupported, ref supportedModesCount, ref supportedModes);
            if (isFaceTrackingSupported)
            {
                FaceTrackingStartInfo startInfo = new FaceTrackingStartInfo();
                startInfo.mode = FaceTrackingMode.PXR_FTM_FACE_LIPS_BS;
                inited = PXR_MotionTracking.StartFaceTracking(ref startInfo);
            }
            Debug.Log($"{k_SubsystemId} Start(). isFaceTrackingSupported:{isFaceTrackingSupported}, init:{inited}");
        }

        public override void Stop()
        {
            if (isFaceTrackingSupported)
            {
                FaceTrackingStopInfo stopInfo = new FaceTrackingStopInfo();
                stopInfo.pause = 0;
                inited = PXR_MotionTracking.StopFaceTracking(ref stopInfo);
            }
            Debug.Log($"{k_SubsystemId} Stop(). isFaceTrackingSupported:{isFaceTrackingSupported}, init:{inited}");
        }

        public override string ToString()
        {
            return base.ToString();
        }

        protected override bool TryInitialize()
        {
            PXR_Plugin.System.UPxr_EnableFaceTracking(true);
            PXR_Plugin.MotionTracking.UPxr_WantFaceTrackingService();
            return base.TryInitialize();
        }
    }

    // this method is run on startup of the app to register this provider with XR Subsystem Manager
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void RegisterDescriptor()
    {
        PLog.i(k_SubsystemId, "RegisterDescriptor");
        var descriptorParams = new FaceSubsystemParams
        {
            supportsFacePose = false,
            supportsFaceMeshVerticesAndIndices = true,
            supportsFaceMeshUVs = true,
            supportsFaceMeshNormals = true,
            id = k_SubsystemId,
            providerType = typeof(FaceProvider),
            subsystemTypeOverride = typeof(PXR_FaceSubsystem)
        };

        XRFaceSubsystemDescriptor.Create(descriptorParams);
    }
}
#endif