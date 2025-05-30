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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Unity.XR.PXR
{
  

    public class PXR_MotionTracking
    {
        #region Eye Tracking
        //Eye Tracking
        public const int PXR_EYE_TRACKING_API_VERSION = 1;

        /// <summary>
        /// Wants eye tracking service for the current app.
        /// </summary>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        [Obsolete("WantEyeTrackingService is not supported..", true)]
        public static int WantEyeTrackingService()
        {
            return -1;
        }

        /// <summary>
        /// Gets whether the current device supports eye tracking.
        /// </summary>
        /// <param name="supported">
        /// Returns a bool indicating whether eye tracking is supported:
        /// * `true`: supported
        /// * `false`: not supported
        /// </param>
        /// <param name="supportedModesCount">
        /// Returns the number of eye tracking modes supported by the current device.
        /// </param>
        /// <param name="supportedModes">
        /// Returns the eye tracking modes supported by the current device.
        /// </param>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        public static int GetEyeTrackingSupported(ref bool supported, ref int supportedModesCount, ref EyeTrackingMode[] supportedModes)
        {
            return PXR_Plugin.MotionTracking.UPxr_GetEyeTrackingSupported(ref supported, ref supportedModesCount, ref supportedModes);
        }

        /// <summary>
        /// Starts eye tracking.
        /// @note Only supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="startInfo">Passes the information for starting eye tracking.
        /// </param>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        public static int StartEyeTracking(ref EyeTrackingStartInfo startInfo)
        {
            startInfo.SetVersion(PXR_EYE_TRACKING_API_VERSION);
            return PXR_Plugin.MotionTracking.UPxr_StartEyeTracking1(ref startInfo);
        }

        /// <summary>
        /// Stops eye tracking.
        /// @note Only supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="stopInfo">Passes the information for stopping eye tracking. Currently, you do not need to pass anything.</param>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        public static int StopEyeTracking(ref EyeTrackingStopInfo stopInfo)
        {
            stopInfo.SetVersion(PXR_EYE_TRACKING_API_VERSION);
            return PXR_Plugin.MotionTracking.UPxr_StopEyeTracking1(ref stopInfo);
        }

        /// <summary>
        /// Gets the state of eye tracking.
        /// @note Only supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="isTracking">Returns a bool that indicates whether eye tracking is working:
        /// * `true`: eye tracking is working
        /// * `false`: eye tracking has been stopped
        /// </param>
        /// <param name="state">Returns the eye tracking state information, including the eye tracking mode and eye tracking state code.</param>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        public static int GetEyeTrackingState(ref bool isTracking, ref EyeTrackingState state)
        {
            state.SetVersion(PXR_EYE_TRACKING_API_VERSION);
            return PXR_Plugin.MotionTracking.UPxr_GetEyeTrackingState(ref isTracking, ref state);
        }

        /// <summary>
        /// Gets eye tracking data.
        /// @note Only supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="getInfo">Specifies the eye tracking data you want.</param>
        /// <param name="data">Returns the desired eye tracking data.</param>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        public static int GetEyeTrackingData(ref EyeTrackingDataGetInfo getInfo, ref EyeTrackingData data)
        {
            getInfo.SetVersion(PXR_EYE_TRACKING_API_VERSION);
            data.SetVersion(PXR_EYE_TRACKING_API_VERSION);
            return PXR_Plugin.MotionTracking.UPxr_GetEyeTrackingData1(ref getInfo, ref data);
        }

        //PICO4E
        /// <summary>
        /// Gets the opennesses of the left and right eyes.
        /// @note
        /// - Only supported by PICO 4 Enterprise.
        /// - To use this API, you need to add `<meta-data android:name="pvr.app.et_tob_advance" android:value="true"/>` to the app's AndroidManifest.xml file.
        /// </summary>
        /// <param name="leftEyeOpenness">The openness of the left eye, which is a float value ranges from `0.0` to `1.0`. `0.0` indicates completely closed, `1.0` indicates completely open.</param>
        /// <param name="rightEyeOpenness">The openness of the right eye, which is a float value ranges from `0.0` to `1.0`. `0.0` indicates completely closed, `1.0` indicates completely open.</param>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        public static int GetEyeOpenness(ref float leftEyeOpenness, ref float rightEyeOpenness)
        {
            return PXR_Plugin.MotionTracking.UPxr_GetEyeOpenness(ref leftEyeOpenness, ref rightEyeOpenness);
        }

        /// <summary>
        /// Gets the information about the pupils of both eyes.
        /// @note
        /// - Only supported by PICO 4 Enterprise.
        /// - To use this API, you need to add `<meta-data android:name="pvr.app.et_tob_advance" android:value="true"/>` to the app's AndroidManifest.xml file.
        /// </summary>
        /// <param name="eyePupilPosition">Returns the diameters and positions of both pupils.</param>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        public static int GetEyePupilInfo(ref EyePupilInfo eyePupilPosition)
        {
            return PXR_Plugin.MotionTracking.UPxr_GetEyePupilInfo(ref eyePupilPosition);
        }

        /// <summary>
        /// Gets the pose of the left and right eyes.
        /// @note
        /// - Only supported by PICO 4 Enterprise.
        /// - To use this API, you need to add `<meta-data android:name="pvr.app.et_tob_advance" android:value="true"/>` to the app's AndroidManifest.xml file.
        /// </summary>
        /// <param name="timestamp">Returns the timestamp (unit: nanosecond) of the eye pose information.</param>
        /// <param name="leftEyePose">Returns the position and rotation of the left eye.</param>
        /// <param name="rightPose">Returns the position and rotation of the right eye.</param>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        public static int GetPerEyePose(ref long timestamp, ref Posef leftEyePose, ref Posef rightPose)
        {
            return PXR_Plugin.MotionTracking.UPxr_GetPerEyePose(ref timestamp, ref leftEyePose, ref rightPose);
        }

        /// <summary>
        /// Gets whether the left and right eyes blinked.
        /// @note
        /// - Only supported by PICO 4 Enterprise.
        /// - To use this API, you need to add `<meta-data android:name="pvr.app.et_tob_advance" android:value="true"/>` to the app's AndroidManifest.xml file.
        /// </summary>
        /// <param name="timestamp">Returns the timestamp (in nanoseconds) of the eye blink information.</param>
        /// <param name="isLeftBlink">Returns whether the left eye blinked:
        /// - `true`: blinked (the user's left eye is closed, which will usually open again immediately to generate a blink event)
        /// - `false`: didn't blink (the user's left eye is open)
        /// </param>
        /// <param name="isRightBlink">Returns whether the right eye blined:
        /// - `true`: blinked (the user's right eye is closed, which will usually open again immediately to generate a blink event)
        /// - `false`: didn't blink (the user's right eye is open)
        /// </param>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        public static int GetEyeBlink(ref long timestamp, ref bool isLeftBlink, ref bool isRightBlink)
        {
            return PXR_Plugin.MotionTracking.UPxr_GetEyeBlink(ref timestamp, ref isLeftBlink, ref isRightBlink);
        }

        #endregion

        #region Face Tracking
        //Face Tracking
        public const int PXR_FACE_TRACKING_API_VERSION = 1;

        /// <summary>
        /// Wants face tracking service for the current app.
        /// </summary>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        [Obsolete("WantFaceTrackingService is not supported..", true)]
        public static int WantFaceTrackingService()
        {
            return -1;
        }

        /// <summary>
        /// Gets whether the current device supports face tracking.
        /// </summary>
        /// <param name="supported">Indicates whether the device supports face tracking:
        /// * `true`: support
        /// * `false`: not support
        /// </param>
        /// <param name="supportedModesCount">Returns the total number of face tracking modes supported by the device.</param>
        /// <param name="supportedModes">Returns the specific face tracking modes supported by the device.</param>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        [Obsolete("GetFaceTrackingSupported is not supported..", true)]
        public static unsafe int GetFaceTrackingSupported(ref bool supported, ref int supportedModesCount, ref FaceTrackingMode[] supportedModes)
        {
            // return PXR_Plugin.MotionTracking.UPxr_GetFaceTrackingSupported(ref supported, ref supportedModesCount, ref supportedModes);
            return -1;
        }

        /// <summary>
        /// Starts face tracking.
        /// @note Supported by PICO 4 Pro and PICO 4 Enterprise.
        /// </summary>
        /// <param name="startInfo">Passes the information for starting face tracking.</param>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        [Obsolete("StartFaceTracking is not supported..", true)]
        public static int StartFaceTracking(ref FaceTrackingStartInfo startInfo)
        {
            // startInfo.SetVersion(PXR_FACE_TRACKING_API_VERSION);
            // return PXR_Plugin.MotionTracking.UPxr_StartFaceTracking(ref startInfo);
            return -1;
        }

        /// <summary>
        /// Stops face tracking.
        /// @note Supported by PICO 4 Pro and PICO 4 Enterprise.
        /// </summary>
        /// <param name="stopInfo">Passes the information for stopping face tracking.</param>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        [Obsolete("StopFaceTracking is not supported..", true)]
        public static int StopFaceTracking(ref FaceTrackingStopInfo stopInfo)
        {
            // stopInfo.SetVersion(PXR_FACE_TRACKING_API_VERSION);
            // return PXR_Plugin.MotionTracking.UPxr_StopFaceTracking(ref stopInfo);
            return -1;
        }

        /// <summary>
        /// Gets the state of face tracking.
        /// @note Supported by PICO 4 Pro and PICO 4 Enterprise.
        /// </summary>
        /// <param name="isTracking">Returns a bool indicating whether face tracking is working:
        /// * `true`: face tracking is working
        /// * `false`: face tracking has been stopped
        /// </param>
        /// <param name="state">Returns the state of face tracking, including the face tracking mode and face tracking state code.
        /// </param>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        [Obsolete("GetFaceTrackingState is not supported..", true)]
        public static int GetFaceTrackingState(ref bool isTracking, ref FaceTrackingState state)
        {
            // state.SetVersion(PXR_FACE_TRACKING_API_VERSION);
            // return PXR_Plugin.MotionTracking.UPxr_GetFaceTrackingState(ref isTracking, ref state);
            return -1;
        }

        /// <summary>
        /// Gets face tracking data.
        /// @note Supported by PICO 4 Pro and PICO 4 Enterprise.
        /// </summary>
        /// <param name="getInfo">Specifies the face tracking data you want.</param>
        /// <param name="data">Returns the desired face tracking data.</param>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        [Obsolete("GetFaceTrackingData is not supported..", true)]
        public static int GetFaceTrackingData(ref FaceTrackingDataGetInfo getInfo, ref FaceTrackingData data)
        {
            // getInfo.SetVersion(PXR_FACE_TRACKING_API_VERSION);
            // data.SetVersion(PXR_FACE_TRACKING_API_VERSION);
            // return PXR_Plugin.MotionTracking.UPxr_GetFaceTrackingData1(ref getInfo, ref data);
            return -1;
        }
        #endregion

        #region Body Tracking
        /// <summary>
        /// A callback function that notifies calibration exceptions.
        /// The user then needs to recalibrate with PICO Motion Tracker.
        /// </summary>
        [Obsolete("BodyTrackingAbnormalCalibrationData is not supported..", true)]
        public static Action<int, int> BodyTrackingAbnormalCalibrationData;

        /// <summary>You can use this callback function to receive the status code and error code for body tracking.</summary>
        /// <returns>
        /// - `BodyTrackingStatusCode`: The status code.
        /// - `BodyTrackingErrorCode`: The error code.
        /// </returns>
        [Obsolete("BodyTrackingStateError is not supported..", true)]
        public static Action<BodyTrackingStatusCode, BodyTrackingErrorCode> BodyTrackingStateError;

        /// <summary>You can use this callback function to get notified when the action status of a tracked bone node changes.</summary>
        /// <returns>
        /// - `int`: Returns the bone No., and only `7` (`LEFT_ANKLE`) and `8` (`RIGHT_ANKLE`) are available currently. You can use the change of the status of the left and right ankles to get the foot-down action of the left and right feet.
        /// - `BodyActionList`: Receiving the `PxrFootDownAction` event indicates that the left and/or right foot has stepped on the floor.
        /// </returns>
        [Obsolete("BodyTrackingAction is not supported..", true)]
        public static Action<int, BodyActionList> BodyTrackingAction;

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
            return PXR_Plugin.MotionTracking.UPxr_StartMotionTrackerCalibApp();
        }

        /// <summary>Gets whether the current device supports body tracking.</summary>
        /// <param name="supported">Returns whether the current device supports body tracking:
        /// - `true`: support
        /// - `false`: not support
        /// </param>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        public static int GetBodyTrackingSupported(ref bool supported)
        {
            return PXR_Plugin.MotionTracking.UPxr_GetBodyTrackingSupported(ref supported);
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
        [Obsolete("Please use StartBodyTracking(BodyJointSet JointSet, BodyTrackingBoneLength boneLength)")]
        public static int StartBodyTracking(BodyTrackingMode mode, BodyTrackingBoneLength boneLength)
        {
            return StartBodyTracking(BodyJointSet.BODY_JOINT_SET_BODY_FULL_START,boneLength);
        }
        
        public static int StartBodyTracking(BodyJointSet JointSet, BodyTrackingBoneLength boneLength)
        {
            return PXR_Plugin.MotionTracking.UPxr_StartBodyTracking(JointSet,boneLength);
        }

        /// <summary>Stops body tracking.</summary>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        public static int StopBodyTracking()
        {
            return PXR_Plugin.MotionTracking.UPxr_StopBodyTracking();
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
        [Obsolete("Please use GetBodyTrackingState(ref bool isTracking, ref BodyTrackingStatus state)")]
        public static int GetBodyTrackingState(ref bool isTracking, ref BodyTrackingState state)
        {
            BodyTrackingStatus bs2 = new BodyTrackingStatus();
            int ret = GetBodyTrackingState(ref isTracking, ref bs2);
            state.stateCode=bs2.stateCode;
            state.errorCode=(BodyTrackingErrorCode)bs2.message;
            return ret;
        }
        public static int GetBodyTrackingState(ref bool isTracking, ref BodyTrackingStatus state)
        {
            return PXR_Plugin.MotionTracking.UPxr_GetBodyTrackingState(ref isTracking, ref state);
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
        public static int GetBodyTrackingData(ref BodyTrackingGetDataInfo getInfo, ref BodyTrackingData data)
        {
            return PXR_Plugin.MotionTracking.UPxr_GetBodyTrackingData(ref getInfo, ref data);
        }
        #endregion

        #region Motion Tracker
        //Motion Tracker

        /// <summary>
        /// You can use this callback function to get notified when the connection state of PICO Motion Tracker changes.
        /// For connection status, `0` indicates "disconnected" and `1` indicates "connected".
        /// </summary>
        [Obsolete("Deprecated",true)]
        public static Action<int, int> MotionTrackerNumberOfConnections;

        /// <summary>
        /// You can use this callback function to get notified when the battery level of PICO Motion Tracker changes.
        /// </summary>
        /// <Returns>
        /// The ID and battery level of the PICO Motion Tracker.
        /// - For PICO Motion Tracker (Beta), the value range of battery level is [0,5].
        /// `0` indicates a low battery, which can affect the tracking accuracy.
        /// </returns>
        [Obsolete("Deprecated",true)]
        public static Action<int, int> MotionTrackerBatteryLevel;

        /// <summary>
        /// You can use this callback function to get the key actions of the motion tracker.
        /// </summary>
        [Obsolete("Deprecated",true)]
        public static Action<MotionTrackerEventData> MotionTrackerKeyAction;

        /// <summary>
        /// You can use this callback function to get notified if the tracking mode changes.
        /// - `0`: body tracking
        /// - `1`: object tracking
        /// </summary>
        [Obsolete("Deprecated",true)]
        public static Action<MotionTrackerMode> MotionTrackingModeChangedAction;
        
        
        public static Action<RequestMotionTrackerCompleteEventData> RequestMotionTrackerCompleteAction;
        //trackerId state
        public static Action<long, int> MotionTrackerConnectionAction;
        public static Action<long, bool> MotionTrackerPowerKeyAction;
        

        /// <summary>Gets the number of trackers currently connected and their serial numbers.</summary>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        [Obsolete("Deprecated.Please use MotionTrackerConnectionAction instead", true)]
        public static int GetMotionTrackerConnectStateWithSN(ref MotionTrackerConnectState connectState)
        {
            return -1;
        }

        /// <summary>Gets the type of the PICO Motion Tracker connected.</summary>
        /// <returns>The type of the motion tracker (beta or official).</summary>
        [Obsolete("Deprecated",true)]
        public static MotionTrackerType GetMotionTrackerDeviceType()
        {
            return MotionTrackerType.MT_2;
        }

        /// <summary>Checks whether the current tracking mode and the number of motion trackers connected are as wanted.
        /// If not, a panel will appear to let the user switch the tracking mode and perform calibration accordingly.</summary>
        /// <param name="mode">Specifies the wanted tracking mode.</summary>
        /// <param name="number">Specifies the expected number of motion trackers. Value range: [0,3]. 
        /// - If you set `mode` to `BodyTracking`, you do not need to set this parameter as it will not work even if you set it.
        /// - If you set `mode` to `MotionTracking`, the default value of this parameter will be 0, and you can select a value from range [0,3].</param>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        [Obsolete("Deprecated.Please use CheckMotionTrackerNumber instead")]
        public static int CheckMotionTrackerModeAndNumber(MotionTrackerMode mode, MotionTrackerNum number = MotionTrackerNum.ONE)
        {
            return CheckMotionTrackerNumber(number);
        }
        public static int CheckMotionTrackerNumber(MotionTrackerNum number)
        {
            return PXR_Plugin.MotionTracking.UPxr_CheckMotionTrackerNumber((int)number);
        }
        /// <summary>Gets the current tracking mode of the PICO Motion Tracker connected.</summary>
        /// <returns>The current tracking mode.</summary>
        [Obsolete("Deprecated")]
        public static MotionTrackerMode GetMotionTrackerMode()
        {
            return MotionTrackerMode.MotionTracking;
        }

        /// <summary>Gets the location of a PICO Motion Tracker which is set to the "motion tracking" mode.</summary>
        /// <param name="trackerSN">Specifies the serial number of the motion tracker to get position for. You can pass only one serial number in one request.</param>
        /// <param name="locations">Returns the location of the specified motion tracker.</param>
        /// <param name="confidence">Returns the confidence of the returned data.</param>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        [Obsolete("Deprecated.Please use GetMotionTrackerLocation instead",true)]
        public static int GetMotionTrackerLocations(TrackerSN trackerSN, ref MotionTrackerLocations locations, ref MotionTrackerConfidence confidence, double predictTime = 0)
        {
            return -1;
        }
        public static int GetMotionTrackerLocation(long trackerid,ref MotionTrackerLocation location,ref bool isValidPose)
        {
            return PXR_Plugin.MotionTracking.UPxr_GetMotionTrackerLocation(trackerid, ref location, ref isValidPose);
        }
        
        public static int GetMotionTrackerBattery(long trackerid,ref float battery, ref XrBatteryChargingState charger)
        {
            return PXR_Plugin.MotionTracking.UPxr_GetMotionTrackerBatteryState(trackerid, ref battery, ref charger);
        }


        #endregion

        #region Motion Tracker For External Device
        /// <summary>You can use this callback function to get notified when the connection state of the external device changes.</summary>
        /// <returns>The connection state of the external device.</returns>
        [Obsolete("Deprecated.Please use ExpandDeviceConnectionAction instead",true)]
        public static Action<ExtDevConnectEventData> ExtDevConnectAction;

        /// <summary>You can use this callback function to get notified when the battery level and charging status of the external device changes.</summary>
        /// <returns>The current better level and charging status of the external device.</returns>
        [Obsolete("Deprecated.Please use ExpandDeviceBatteryAction instead",true)]
        public static Action<ExtDevBatteryEventData> ExtDevBatteryAction;

        /// <summary>
        /// You need to listen for this event to call the `GetExtDevTrackerByPassData` API:
        /// - When receiving `1`, it is necessary to call the `PXR_GetExtDevTrackerByPassData` API to obtain the data passed through.
        /// - When receiving `0`, stop obtaining the data passed through.
        /// </summary>
        public static Action<int> ExtDevPassDataAction;
        
        //第一个参数是trackerid
        //第二个参数是连接状态0是断开，1是连接 
        public static Action<long, int> ExpandDeviceConnectionAction;
        
        public static Action<ExpandDeviceBatteryEventData> ExpandDeviceBatteryAction;

        /// <summary>Gets the connection state of the external device.</summary>
        /// <param name="connectState">Returns the connection state of the external device.</param>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        [Obsolete("Deprecated.Please use ExpandDeviceConnectionAction instead",true)]
        public static int GetExtDevTrackerConnectState(ref ExtDevTrackerConnectState connectState)
        {
            return -1;
        }

        /// <summary>Sets vibration for the external device.</summary>
        /// <param name="motorVibrate">Specifies vibration settings.</param>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        [Obsolete("Deprecated.Please use SetExpandDeviceVibrate instead",true)]
        public static int SetExtDevTrackerMotorVibrate(ref ExtDevTrackerMotorVibrate motorVibrate)
        {
            return -1;
        }
        
        /// <summary>Sets the state for data passthrough-related APIs.</summary>
        /// <param name="state">Specifies the state of data passthrough-related APIs according to actual needs:
        /// Before calling `SetExpandDeviceCustomData` and `GetExpandDeviceCustomData`, set `state` to `true` to enable these APIs, or set `state` to `false` to disable these APIs.
        /// </param>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        public static int SetExtDevTrackerPassDataState(bool state)
        {
            return PXR_Plugin.MotionTracking.UPxr_SetExpandDeviceCustomDataCapability(state);
        }
        /// <summary>Sets data passthrough for the external device. The protocol is defined by yourself according to your own hardware. 
        /// There is no correspondence between the `set` and `get`-related methods themselves.</summary>
        /// <param name="passData">When PICO SDK's APIs are unable to meet your needs, you can define custom protocols and place them in the `passData` parameter.</param>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        [Obsolete("Deprecated.Please use SetExpandDeviceCustomData instead",true)]
        public static int SetExtDevTrackerByPassData(ref ExtDevTrackerPassData passData)
        {
            return -1;
        }

        /// <summary>Gets the data passed through for an external device.</summary>
        /// <param name="passData">Returns the details of the data passed through.</param>
        /// <param name="realLength">Returns the number of `passData` arrays filled by the underlying layer.</param>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        [Obsolete("Deprecated.Please use GetExpandDeviceCustomData instead",true)]
        public static int GetExtDevTrackerByPassData(ref ExtDevTrackerPassDataArray passData, ref int realLength)
        {
            return -1;
        }

        /// <summary>Gets the battery level of the external device.</summary>
        /// <param name="trackerSN">Specifies the serial number of the external device the get battery level for.</param>
        /// <param name="battery">Returns the current battery level of the external device. Value range: [0,10].</param>
        /// <param name="charger">Returns whether the external device is charging:
        /// - `0`: not charging
        /// - `1`: charging
        /// </param>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        [Obsolete("Deprecated.Please use GetExpandDeviceBattery instead",true)]
        public static int GetExtDevTrackerBattery(ref TrackerSN trackerSN, ref int battery, ref int charger)
        {
            return -1;
        }
       
        /// <summary>Gets the key values of the external device.</summary>
        /// <param name="trackerSN">Specifies the serial number of the external device to get key values for.</param>
        /// <param name="keyData">Returns the key values of the specified external device.</param>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        [Obsolete("Deprecated",true)]
        public static int GetExtDevTrackerKeyData(ref TrackerSN trackerSN, ref ExtDevTrackerKeyData keyData)
        {
            return -1;
        }
        public static int SetExpandDeviceVibrate(long deviceid, ExpandDeviceVibrate motorVibrate)
        {
            return PXR_Plugin.MotionTracking.UPxr_SetExpandDeviceVibrate(deviceid, motorVibrate);
        }
        public static int GetExpandDevice(out long[] deviceArray)
        {
            return PXR_Plugin.MotionTracking.UPxr_GetExpandDevice(out deviceArray);
        }
        public static int GetExpandDeviceBattery(long  deviceid, ref float battery, ref XrBatteryChargingState charger)
        {
            return PXR_Plugin.MotionTracking.UPxr_GetExpandDeviceBattery(deviceid, ref battery, ref charger);
        }
        public static int GetExpandDeviceCustomData(out List<ExpandDevicesCustomData>  dataArray)
        {
            return PXR_Plugin.MotionTracking.UPxr_GetExpandDeviceCustomData(out dataArray);
        }
        public static int SetExpandDeviceCustomData(ref ExpandDevicesCustomData[]  dataArray)
        {
            return PXR_Plugin.MotionTracking.UPxr_SetExpandDeviceCustomData(ref dataArray);
        }
        #endregion
    }
}