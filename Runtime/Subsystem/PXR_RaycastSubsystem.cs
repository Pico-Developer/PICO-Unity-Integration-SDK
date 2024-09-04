#if AR_FOUNDATION
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public class PXR_RaycastSubsystem : XRRaycastSubsystem
{
    internal const string k_SubsystemId = "PXR_RaycastSubsystem";

    internal static PXR_RaycastSubsystem instance { get; private set; }

    /// <summary>
    /// Do not call this directly. Call create on a valid <see cref="XRSessionSubsystemDescriptor"/> instead.
    /// </summary>
    public PXR_RaycastSubsystem()
    {
        instance = this;
    }

    class RaycastProvider : Provider
    {

    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void RegisterDescriptor()
    {
        XRRaycastSubsystemDescriptor.RegisterDescriptor(new XRRaycastSubsystemDescriptor.Cinfo
        {
            id = k_SubsystemId,
            providerType = typeof(RaycastProvider),
            subsystemTypeOverride = typeof(PXR_RaycastSubsystem),
            supportsViewportBasedRaycast = false,
            supportsWorldBasedRaycast = false,
            supportedTrackableTypes = TrackableType.PlaneWithinBounds,
            supportsTrackedRaycasts = false,
        });
    }

}
#endif