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

using System.Runtime.InteropServices;
using UnityEngine;

#if PICO_LIVE_PREVIEW && UNITY_EDITOR
using Unity.XR.PICO.LivePreview;
#endif

namespace Unity.XR.PXR
{
    public enum HandType
    {
        HandLeft = 0,
        HandRight = 1,
    }

    /// <summary>
    /// The current active input device.
    /// </summary>
    public enum ActiveInputDevice
    {
        /// <summary>
        /// HMD
        /// </summary>
        HeadActive = 0,
        /// <summary>
        /// Controllers
        /// </summary>
        ControllerActive = 1,
        /// <summary>
        /// Hands
        /// </summary>
        HandTrackingActive = 2,
    }

    public struct Vector3f
    {
        public float x;
        public float y;
        public float z;

        public Vector3 ToVector3()
        {
            return new Vector3() { x = x, y = y, z = -z };
        }
    }

    public struct Quatf
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Quaternion ToQuat()
        {
            return new Quaternion() { x = x, y = y, z = -z, w = -w };
        }
    }
    
    /// <summary>
    /// The location of hand joint.
    /// </summary>
    public struct Posef
    {
        /// <summary>
        /// The orientation of hand joint.
        /// </summary>
        public Quatf Orientation;
        /// <summary>
        /// The position of hand joint.
        /// </summary>
        public Vector3f Position;
        public override string ToString()
        {
            return string.Format("Orientation :{0}, {1}, {2}, {3}  Position: {4}, {5}, {6}",
                Orientation.x, Orientation.y, Orientation.z, Orientation.w,
                Position.x, Position.y, Position.z);
        }
    }
    
    /// <summary>
    /// The status of ray and fingers.
    /// </summary>
    public enum HandAimStatus : ulong
    {
        /// <summary>
        /// Whether the data is valid.
        /// </summary>
        AimComputed = 0x00000001,
        /// <summary>
        /// Whether the ray appears.
        /// </summary>
        AimRayValid = 0x00000002,
        /// <summary>
        /// Whether the index finger pinches.
        /// </summary>
        AimIndexPinching = 0x00000004,
        /// <summary>
        /// Whether the middle finger pinches.
        /// </summary>
        AimMiddlePinching = 0x00000008,
        /// <summary>
        /// Whether the ring finger pinches.
        /// </summary>
        AimRingPinching = 0x00000010,
        /// <summary>
        /// Whether the little finger pinches.
        /// </summary>
        AimLittlePinching = 0x00000020,
        /// <summary>
        /// Whether the ray touches.
        /// </summary>
        AimRayTouched = 0x00000200
    }

    /// <summary>
    /// The data about the poses of ray and fingers.
    /// </summary>
    public struct HandAimState
    {
        /// <summary>
        /// The status of hand tracking. If it is not `tracked`, confidence will be `0`.
        /// </summary>
        public HandAimStatus aimStatus;
        /// <summary>
        /// The pose of the ray.
        /// </summary>
        public Posef aimRayPose;
        /// <summary>
        /// The strength of index finger's pinch.
        /// </summary>
        private float pinchStrengthIndex;
        /// <summary>
        /// The strength of middle finger's pinch.
        /// </summary>
        private float pinchStrengthMiddle;
        /// <summary>
        /// The strength of ring finger's pinch.
        /// </summary>
        private float pinchStrengthRing;
        /// <summary>
        /// The strength of little finger's pinch.
        /// </summary>
        private float pinchStrengthLittle;
        /// <summary>
        /// The strength of ray's touch.
        /// </summary>
        public float touchStrengthRay;
    }

    /// <summary>
    /// The data about the status of hand joint location.
    /// </summary>
    public enum HandLocationStatus : ulong
    {
        /// <summary>
        /// Whether the joint's orientation is valid.
        /// </summary>
        OrientationValid = 0x00000001,
        /// <summary>
        /// Whether the joint's position is valid.
        /// </summary>
        PositionValid = 0x00000002,
        /// <summary>
        /// Whether the joint's orientation is being tracked.
        /// </summary>
        OrientationTracked = 0x00000004,
        /// <summary>
        /// Whether the joint's position is being tracked.
        /// </summary>
        PositionTracked = 0x00000008
    }

    public enum HandJoint
    {
        JointPalm = 0,
        JointWrist = 1,

        JointThumbMetacarpal = 2,
        JointThumbProximal = 3,
        JointThumbDistal = 4,
        JointThumbTip = 5,

        JointIndexMetacarpal = 6,
        JointIndexProximal = 7,
        JointIndexIntermediate = 8,
        JointIndexDistal = 9,
        JointIndexTip = 10,

        JointMiddleMetacarpal = 11,
        JointMiddleProximal = 12,
        JointMiddleIntermediate = 13,
        JointMiddleDistal = 14,
        JointMiddleTip = 15,

        JointRingMetacarpal = 16,
        JointRingProximal = 17,
        JointRingIntermediate = 18,
        JointRingDistal = 19,
        JointRingTip = 20,

        JointLittleMetacarpal = 21,
        JointLittleProximal = 22,
        JointLittleIntermediate = 23,
        JointLittleDistal = 24,
        JointLittleTip = 25,

        JointMax = 26
    }

    /// <summary>
    /// The data about the location of hand joint.
    /// </summary>
    public struct HandJointLocation
    {
        /// <summary>
        /// The status of hand joint location.
        /// </summary>
        public HandLocationStatus locationStatus;
        /// <summary>
        /// The orientation and position of hand joint.
        /// </summary>
        public Posef pose;
        /// <summary>
        /// The radius of hand joint.
        /// </summary>
        public float radius;
    }

    /// <summary>
    /// The data about hand tracking.
    /// </summary>
    public struct HandJointLocations
    {
        /// <summary>
        /// The quality level of hand tracking:
        /// `0`: low
        /// `1`: high
        /// </summary>
        public uint isActive;
        /// <summary>
        /// The number of hand joints that the SDK supports. Currenty returns `26`.
        /// </summary>
        public uint jointCount;
        /// <summary>
        /// The scale of the hand.
        /// </summary>
        public float handScale;

        /// <summary>
        /// The locations (orientation and position) of hand joints.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)HandJoint.JointMax)]
        public HandJointLocation[] jointLocations;
    }

    public enum HandFinger
    {
        Thumb = 0,
        Index = 1,
        Middle = 2,
        Ring = 3,
        Pinky = 4
    }

    public static class PXR_HandTracking
    {
        /// <summary>Gets whether hand tracking is enabled or disabled.</summary>
        /// <returns>
        /// * `true`: enabled
        /// * `false`: disabled
        /// </returns>
        public static bool GetSettingState()
        {
#if PICO_LIVE_PREVIEW && UNITY_EDITOR
            return PXR_PTApi.UPxr_GetSettingState();
#endif
            return PXR_Plugin.HandTracking.UPxr_GetHandTrackerSettingState();
            
        }

        /// <summary>Gets the current active input device.</summary>
        /// <returns>The current active input device:
        /// * `HeadActive`: HMD 
        /// * `ControllerActive`: controllers
        /// * `HandTrackingActive`: hands
        /// </returns>
        public static ActiveInputDevice GetActiveInputDevice()
        {
#if PICO_LIVE_PREVIEW && UNITY_EDITOR
            return PXR_PTApi.UPxr_GetGetHandTrackerActiveState() ? ActiveInputDevice.HandTrackingActive : ActiveInputDevice.ControllerActive;
#endif
            return PXR_Plugin.HandTracking.UPxr_GetHandTrackerActiveInputType();
        }

        /// <summary>Gets the data about the pose of a specified hand, including the status of the ray and fingers, the strength of finger pinch and ray touch.</summary>
        /// <param name="hand">The hand to get data for:
        /// * `HandLeft`: left hand
        /// * `HandRight`: right hand
        /// </param>
        /// <param name="aimState">`HandAimState` contains the data about the poses of ray and fingers.
        /// If you use PICO hand prefabs without changing any of their default settings, you will get the following data:
        /// ```csharp
        /// public class PXR_Hand
        /// {
        ///     // Whether the data is valid.
        ///     public bool Computed { get; private set; }
        ///
        ///     // The ray pose.
        ///     public Posef RayPose { get; private set; }
        ///     // Whether the ray was displayed.
        ///     public bool RayValid { get; private set; }
        ///     // Whether the ray touched.
        ///     public bool RayTouched { get; private set; }
        ///     // The strength of ray touch.
        ///     public float TouchStrengthRay { get; private set; }
        /// ```
        /// </param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool GetAimState(HandType hand, ref HandAimState aimState)
        {
            if (!PXR_ProjectSetting.GetProjectConfig().handTracking) 
                return false;

#if PICO_LIVE_PREVIEW && UNITY_EDITOR
            PICO.LivePreview.HandAimState lPHandAimState = new PICO.LivePreview.HandAimState();
            PXR_PTApi.UPxr_GetHandTrackerAimState((int)hand, ref lPHandAimState);
            aimState.aimStatus = (HandAimStatus)lPHandAimState.aimStatus;
            aimState.touchStrengthRay = lPHandAimState.touchStrengthRay;
            aimState.aimRayPose.Position.x = lPHandAimState.aimRayPose.Position.x;
            aimState.aimRayPose.Position.y = lPHandAimState.aimRayPose.Position.y;
            aimState.aimRayPose.Position.z = lPHandAimState.aimRayPose.Position.z;
            aimState.aimRayPose.Orientation.x = lPHandAimState.aimRayPose.Orientation.x;
            aimState.aimRayPose.Orientation.y = lPHandAimState.aimRayPose.Orientation.y;
            aimState.aimRayPose.Orientation.z = lPHandAimState.aimRayPose.Orientation.z;
            aimState.aimRayPose.Orientation.w = lPHandAimState.aimRayPose.Orientation.w;
            return true;
#endif
            return PXR_Plugin.HandTracking.UPxr_GetHandTrackerAimState(hand, ref aimState);
        }

        /// <summary>Gets the locations of joints for a specified hand.</summary>
        /// <param name="hand">The hand to get joint locations for:
        /// * `HandLeft`: left hand
        /// * `HandRight`: right hand
        /// </param>
        /// <param name="jointLocations">Contains data about the locations of the joints in the specified hand.</param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool GetJointLocations(HandType hand, ref HandJointLocations jointLocations)
        {
            if (!PXR_ProjectSetting.GetProjectConfig().handTracking) 
                return false;
#if PICO_LIVE_PREVIEW && UNITY_EDITOR
            PICO.LivePreview.HandJointLocations lPHandJointLocations = new PICO.LivePreview.HandJointLocations();
            PXR_PTApi.UPxr_GetHandTrackerJointLocations((int)hand,ref lPHandJointLocations);
            jointLocations.handScale = lPHandJointLocations.handScale;
            jointLocations.isActive = lPHandJointLocations.isActive;
            jointLocations.jointCount = lPHandJointLocations.jointCount;
            jointLocations.jointLocations = new HandJointLocation[lPHandJointLocations.jointCount];
            for (int i = 0; i < lPHandJointLocations.jointCount; i++)
            {
                jointLocations.jointLocations[i].locationStatus = (HandLocationStatus)lPHandJointLocations.jointLocations[i].locationStatus;
                jointLocations.jointLocations[i].radius = lPHandJointLocations.jointLocations[i].radius;
                jointLocations.jointLocations[i].pose.Position.x = lPHandJointLocations.jointLocations[i].pose.Position.x;
                jointLocations.jointLocations[i].pose.Position.y = lPHandJointLocations.jointLocations[i].pose.Position.y;
                jointLocations.jointLocations[i].pose.Position.z = lPHandJointLocations.jointLocations[i].pose.Position.z;
                jointLocations.jointLocations[i].pose.Orientation.x = lPHandJointLocations.jointLocations[i].pose.Orientation.x;
                jointLocations.jointLocations[i].pose.Orientation.y = lPHandJointLocations.jointLocations[i].pose.Orientation.y;
                jointLocations.jointLocations[i].pose.Orientation.z = lPHandJointLocations.jointLocations[i].pose.Orientation.z;
                jointLocations.jointLocations[i].pose.Orientation.w = lPHandJointLocations.jointLocations[i].pose.Orientation.w;

            }
            return true;
#endif
            return PXR_Plugin.HandTracking.UPxr_GetHandTrackerJointLocations(hand, ref jointLocations);
        }

    }
}

