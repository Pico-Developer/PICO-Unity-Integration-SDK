﻿#if AR_FOUNDATION_5 || AR_FOUNDATION_6
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public class PXR_CameraSubsystem : XRCameraSubsystem
{
    internal const string k_SubsystemId = "PXR_CameraSubsystem";

    internal static PXR_CameraSubsystem instance { get; private set; }

    /// <summary>
    /// Do not call this directly. Call create on a valid <see cref="XRCameraSubsystemDescriptor"/> instead.
    /// </summary>
    public PXR_CameraSubsystem()
    {
        instance = this;
    }

    class CameraProvider : Provider
    {
        /// <summary>
        /// Start the camera functionality.
        /// </summary>
        public override void Start()
        {
            Debug.Log($"{k_SubsystemId} Start().");
            PXR_Plugin.Boundary.UPxr_SetSeeThroughBackground(true);
            PXR_Plugin.System.SessionStateChanged += EnableVST;
        }

        /// <summary>
        /// Stop the camera functionality.
        /// </summary>
        public override void Stop()
        {
            Debug.Log($"{k_SubsystemId} Stop().");
            PXR_Plugin.Boundary.UPxr_SetSeeThroughBackground(false);
            PXR_Plugin.System.SessionStateChanged -= EnableVST;
        }

        /// <summary>
        /// Destroy any resources required for the camera functionality.
        /// </summary>
        public override void Destroy() { }

        public void EnableVST(XrSessionState state)
        {
            if (state == XrSessionState.Ready)
            {
                PXR_Plugin.Boundary.UPxr_SetSeeThroughBackground(true);
            }
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void RegisterDescriptor()
    {
#if AR_FOUNDATION_5
        var cameraSubsystemCinfo = new XRCameraSubsystemCinfo
#endif

#if AR_FOUNDATION_6
        var cameraSubsystemCinfo = new XRCameraSubsystemDescriptor.Cinfo
#endif
        {
            id = k_SubsystemId,
            providerType = typeof(CameraProvider),
            subsystemTypeOverride = typeof(PXR_CameraSubsystem),
            supportsAverageBrightness = false,
            supportsAverageColorTemperature = false,
            supportsColorCorrection = false,
            supportsDisplayMatrix = false,
            supportsProjectionMatrix = false,
            supportsTimestamp = false,
            supportsCameraConfigurations = false,
            supportsCameraImage = false,
            supportsAverageIntensityInLumens = false,
            supportsFocusModes = false,
            supportsFaceTrackingAmbientIntensityLightEstimation = false,
            supportsFaceTrackingHDRLightEstimation = false,
            supportsWorldTrackingAmbientIntensityLightEstimation = false,
            supportsWorldTrackingHDRLightEstimation = false,
            supportsCameraGrain = false,
        };

#if AR_FOUNDATION_5
        if (!XRCameraSubsystem.Register(cameraSubsystemCinfo))
        {
            PLog.e(k_SubsystemId, $"Failed to register the {k_SubsystemId} subsystem.");
        }
#endif

#if AR_FOUNDATION_6
        XRCameraSubsystemDescriptor.Register(cameraSubsystemCinfo);
#endif
    }
}
#endif