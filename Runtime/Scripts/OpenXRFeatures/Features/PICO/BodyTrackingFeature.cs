#if PICO_OPENXR_SDK
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.XR.PXR;
using UnityEngine;
#if AR_FOUNDATION_5||AR_FOUNDATION_6
using UnityEngine.XR.ARSubsystems;
#endif
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;
#endif

namespace Unity.XR.OpenXR.Features.PICOSupport
{
    public enum XrBodyJointSetBD
    {
        XR_BODY_JOINT_SET_DEFAULT_BD = 0, //default joint set XR_BODY_JOINT_SET_BODY_STAR_WITHOUT_ARM_BD
        XR_BODY_JOINT_SET_BODY_START_WITHOUT_ARM_BD = 1,
        XR_BODY_JOINT_SET_BODY_FULL_STAR_BD = 2
    }

    
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "PICO Body Tracking",
        Hidden = false,
        BuildTargetGroups = new[] { UnityEditor.BuildTargetGroup.Android },
        Company = "PICO",
        OpenxrExtensionStrings = extensionString,
        Version = PXR_Constants.SDKVersion,
        FeatureId = featureId)]
#endif
    
    public class BodyTrackingFeature : OpenXRFeatureBase
    {
        public const string featureId = "com.pico.openxr.feature.PICO_BodyTracking";
        public const string extensionString = "XR_BD_body_tracking XR_PICO_body_tracking2";
    
        public static bool isEnable => OpenXRRuntime.IsExtensionEnabled("XR_BD_body_tracking");

        public override string GetExtensionString()
        {
            return extensionString;
        }
      
        [Obsolete("Please use StartBodyTracking(BodyJointSet JointSet, BodyTrackingBoneLength boneLength)")]
        public static bool StartBodyTracking(XrBodyJointSetBD Mode)
        {
            if (!isEnable)
            {
                return false;
            }
            
            BodyTrackingBoneLength boneLength=new BodyTrackingBoneLength();

            return StartBodyTracking((BodyJointSet)Mode, boneLength)==0;
        }
        /// <summary>Starts body tracking.</summary>
        /// <param name="mode">Specifies the body tracking mode (default or high-accuracy).</param>
        /// <param name="boneLength">Specifies lengths (unit: cm) for the bones of the avatar, which is only available for the `BTM_FULL_BODY_HIGH` mode.
        /// Bones that are not set lengths for will use the default values.
        /// </param>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        public static int StartBodyTracking(BodyJointSet JointSet, BodyTrackingBoneLength boneLength)
        {
            if (!isEnable)
            {
                return 1;
            }
            BodyTrackingStartInfo startInfo = new BodyTrackingStartInfo();
            startInfo.jointSet = JointSet;
            startInfo.BoneLength = boneLength;

            return Pxr_StartBodyTracking(ref startInfo);
        }
        /// <summary>Launches the PICO Motion Tracker app to perform calibration.
        /// - For PICO Motion Tracker (Beta), the user needs to follow the instructions on the home of the PICO Motion Tracker app to complete calibration.
        /// - For PICO Motion Tracker (Official), "single-glance calibration" will be performed. When a user has a glance at the PICO Motion Tracker on their lower legs, calibration is completed.
        /// </summary>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        public static int StartMotionTrackerCalibApp()
        {
            if (!isEnable)
            {
                return 1;
            }
            return Pxr_StartBodyTrackingCalibApp();
        }

        public static bool IsBodyTrackingSupported()
        {
            if (!isEnable)
            {
                return false;
            }
            bool supported=false;
            Pxr_GetBodyTrackingSupported(ref supported);
            return supported;
        }

        /// <summary>
        /// Gets the data about the poses of body joints.
        /// </summary>
        /// <param name="predictTime">Reserved parameter, pass `0`.</param>
        /// <param name="bodyTrackerResult">Contains the data about the poses of body joints, including position, action, and more.</param>
        [Obsolete("Please use GetBodyTrackingData",true)]
        public static bool GetBodyTrackingPose(ref BodyTrackerResult bodyTrackerResult)
        {
            return false;
        }
        /// <summary>Stops body tracking.</summary>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        public static int StopBodyTracking()
        {
            return Pxr_StopBodyTracking();
        }
        [Obsolete("Please use StopBodyTracking")]
        private void OnDestroy()
        {
            if (!isEnable)
            {
                return;
            }

            StopBodyTracking();
        }
        
        [Obsolete("Please use StartMotionTrackerCalibApp")]
        public static void OpenFitnessBandCalibrationAPP()
        {
            StartMotionTrackerCalibApp();
        }

        /// <summary>Gets body tracking data.</summary>
        /// <param name="getInfo"> Specifies the display time and the data filtering flags.
        /// For the display time, for example, when it is set to 0.1 second, it means predicting the pose of the tracked node 0.1 seconds ahead.
        /// </param>
        /// <param name="data">Returns the array of data for all tracked nodes.</param>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        public unsafe static int GetBodyTrackingData(ref BodyTrackingGetDataInfo getInfo, ref BodyTrackingData data)
        {
            if (!isEnable)
            {
                return 1;
            }
            int val = -1;
            {
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
            }
            return val;
        }
        /// <summary>Gets the state of PICO Motion Tracker and, if any, the reason for an exception.</summary>
        /// <param name="isTracking">Indicates whether the PICO Motion Tracker is tracking normally:
        /// - `true`: is tracking
        /// - `false`: tracking lost
        /// </param>
        /// <param name="state">Returns the information about body tracking state.</param>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        public static int GetBodyTrackingState(ref bool isTracking, ref BodyTrackingStatus state)
        {
            int val = -1;
            {
                val = Pxr_GetBodyTrackingState(ref isTracking, ref state);
            }
            return val;
        }

#if AR_FOUNDATION_5||AR_FOUNDATION_6
        public  bool isBodyTracking=false;
        static List<XRHumanBodySubsystemDescriptor> s_HumanBodyDescriptors = new List<XRHumanBodySubsystemDescriptor>();
        protected override void OnSubsystemCreate()
        {
            base.OnSubsystemCreate();
            if (isBodyTracking)
            {
                CreateSubsystem<XRHumanBodySubsystemDescriptor, XRHumanBodySubsystem>(
                    s_HumanBodyDescriptors,
                    PXR_HumanBodySubsystem.k_SubsystemId);
           
            }

        }
        protected override void OnSubsystemStart()
        {
            if (isBodyTracking)
            {
                StartSubsystem<XRHumanBodySubsystem>();
            }
        }
        protected override void OnSubsystemStop()
        {
            if (isBodyTracking)
            {
                StopSubsystem<XRHumanBodySubsystem>();
            }
        }
        protected override void OnSubsystemDestroy()
        {
            if (isBodyTracking)
            {
                DestroySubsystem<XRHumanBodySubsystem>();
            }
        }
#endif
        
        
        
        [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StartBodyTrackingCalibApp();
        [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetBodyTrackingSupported(ref bool supported);
        [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StartBodyTracking(ref BodyTrackingStartInfo startInfo);
        [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StopBodyTracking();
        [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetBodyTrackingState(ref bool isTracking, ref BodyTrackingStatus state);
        [DllImport(OpenXRExtensions.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetBodyTrackingData(ref BodyTrackingGetDataInfo getInfo, ref BodyTrackingData data);
        
        
    }
}
#endif