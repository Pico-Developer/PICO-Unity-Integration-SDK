#if AR_FOUNDATION
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public class PXR_SessionSubsystem : XRSessionSubsystem
{
    internal const string k_SubsystemId = "PXR_SessionSubsystem";

    internal static PXR_SessionSubsystem instance { get; private set; }

    /// <summary>
    /// Do not call this directly. Call create on a valid <see cref="XRSessionSubsystemDescriptor"/> instead.
    /// </summary>
    public PXR_SessionSubsystem()
    {
        instance = this;
    }

    internal void OnSessionStateChange(int newState)
           => ((SessionProvider)provider).OnSessionStateChange(newState);

    class SessionProvider : Provider
    {
        XrSessionState m_SessionState;

        /// <inheritdoc/>
        public override TrackingState trackingState
        {
            get
            {
                switch (m_SessionState)
                {
                    case XrSessionState.Idle:
                    case XrSessionState.Ready:
                    case XrSessionState.Synchronized:
                        return TrackingState.Limited;

                    case XrSessionState.Visible:
                    case XrSessionState.Focused:
                        return TrackingState.Tracking;

                    case XrSessionState.Unknown:
                    case XrSessionState.Stopping:
                    case XrSessionState.LossPending:
                    case XrSessionState.Exiting:
                    default:
                        return TrackingState.None;
                }
            }
        }

        /// <inheritdoc/>
        public override NotTrackingReason notTrackingReason
        {
            get
            {
                switch (m_SessionState)
                {
                    case XrSessionState.Idle:
                    case XrSessionState.Ready:
                    case XrSessionState.Synchronized:
                        return NotTrackingReason.Initializing;

                    case XrSessionState.Visible:
                    case XrSessionState.Focused:
                        return NotTrackingReason.None;

                    case XrSessionState.Unknown:
                    case XrSessionState.Stopping:
                    case XrSessionState.LossPending:
                    case XrSessionState.Exiting:
                    default:
                        return NotTrackingReason.Unsupported;
                }
            }
        }

        public void OnSessionStateChange(int newState)
        {
            m_SessionState = (XrSessionState)newState;
            PLog.i(k_SubsystemId, $" OnSessionStateChange m_SessionState:{m_SessionState}");
        }
    }

    enum XrSessionState
    {
        Unknown = 0,
        Idle = 1,
        Ready = 2,
        Synchronized = 3,
        Visible = 4,
        Focused = 5,
        Stopping = 6,
        LossPending = 7,
        Exiting = 8,
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void RegisterDescriptor()
    {
        XRSessionSubsystemDescriptor.RegisterDescriptor(new XRSessionSubsystemDescriptor.Cinfo
        {
            id = k_SubsystemId,
            providerType = typeof(SessionProvider),
            subsystemTypeOverride = typeof(PXR_SessionSubsystem),
            supportsInstall = false,
            supportsMatchFrameRate = false
        });
    }

}
#endif