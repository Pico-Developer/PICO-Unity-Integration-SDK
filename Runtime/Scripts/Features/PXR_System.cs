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

namespace Unity.XR.PXR
{
    /// <summary>
    /// A callback function that notifies the change of input device.
    /// </summary>
    public delegate void InputDeviceChangedCallBack(int value);
    /// <summary>
    /// A callback function that notifies the change of seethrough state.
    /// </summary>
    public delegate void SeethroughStateChangedCallBack(int value);
    /// <summary>
    /// A callback function that notifies the current connection status of PICO Motion Tracker and the number of motion trackers connected.
    /// For connection status, `0` indicates "disconnected" and `1` indicates "connected".
    /// </summary>
    public delegate void FitnessBandNumberOfConnectionsCallBack(int state, int value);
    /// <summary>
    /// A callback function that notifies calibration exceptions.
    /// The user then needs to recalibrate with PICO Motion Tracker.
    /// </summary>
    public delegate void FitnessBandAbnormalCalibrationDataCallBack(int state, int value);
    /// <summary>
    /// A callback function that notifies the battery of PICO Motion Traker.
    /// Value range: [0,5]. `0` indicates a low battery, which can affect the tracking accuracy.
    /// </summary>
    public delegate void FitnessBandElectricQuantityCallBack(int trackerID, int battery);
    /// <summary>
    /// A callback function that notifies the change of loglevel state.
    /// </summary>
    public delegate void LoglevelChangedCallBack(int value);
    
    public class PXR_System
    {
        /// <summary>
        /// Gets the SDK version.
        /// </summary>
        /// <returns>The SDK version.</returns>
        public static string GetSDKVersion()
        {
            return PXR_Plugin.System.UPxr_GetSDKVersion();
        }

        /// <summary>
        /// Gets the predicted time a frame will be displayed after being rendered.
        /// </summary>
        /// <returns>The predicted time (in miliseconds).</returns>
        public static double GetPredictedDisplayTime()
        {
            return PXR_Plugin.System.UPxr_GetPredictedDisplayTime();
        }

        /// <summary>
        /// Sets the extra latency mode. Note: Call this function once only.
        /// </summary>
        /// <param name="mode">The latency mode:
        /// * `0`: ExtraLatencyModeOff (Disable ExtraLatencyMode mode. This option will display the latest rendered frame for display)
        /// * `1`: ExtraLatencyModeOn (Enable ExtraLatencyMode mode. This option will display one frame prior to the latest rendered frame)
        /// * `2`: ExtraLatencyModeDynamic (Use system default setup)
        /// </param>
        /// <returns>Whether the extra latency mode has been set:
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool SetExtraLatencyMode(int mode)
        {
            return PXR_Plugin.System.UPxr_SetExtraLatencyMode(mode);
        }

        /// <summary>
        /// Gets the sensor's status.
        /// </summary>
        /// <returns>The sensor's status:
        /// * `0`: null
        /// * `1`: 3DoF
        /// * `3`: 6DoF
        /// </returns>
        public static int GetSensorStatus()
        {
            return PXR_Plugin.System.UPxr_GetSensorStatus();
        }

        /// <summary>
        /// Sets the system display frequency rate.
        /// </summary>
        /// <param name="rate">The frequency rate: `72`; `90`; `120`. Other values are invalid.</param>
        public static void SetSystemDisplayFrequency(float rate)
        {
            PXR_Plugin.System.UPxr_SetSystemDisplayFrequency(rate);
        }

        /// <summary>
        /// Gets the system display frequency rate.
        /// </summary>
        /// <returns>The system display frequency rate.</returns>
        public static float GetSystemDisplayFrequency()
        {
            return PXR_Plugin.System.UPxr_GetSystemDisplayFrequency();
        }

        /// <summary>
        /// Gets the predicted status of the sensor.
        /// </summary>
        /// <param name="sensorState">Sensor's coordinate:
        /// * `pose`: in-app coordinate
        /// * `globalPose`: global coordinate
        /// </param>
        /// <param name="sensorFrameIndex">Sensor frame index.</param>
        /// <returns>The predicted status of the sensor.</returns>
        public static int GetPredictedMainSensorStateNew(ref PxrSensorState2 sensorState, ref int sensorFrameIndex) {
            return PXR_Plugin.System.UPxr_GetPredictedMainSensorStateNew(ref sensorState, ref sensorFrameIndex);
        }
        
        /// <summary>
        /// Enables/disables content protection.
        /// </summary>
        /// <param name="data">Specifies whether to enable/disable content protection:
        /// * `0`: disable
        /// * `1`: enable
        /// </param>
        /// <returns>Whether content protection is successfully enabled/disabled:
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int ContentProtect(int data) {
            return PXR_Plugin.System.UPxr_ContentProtect(data);
        }

        /// <summary>
        /// Enables/disables face tracking.
        /// @note Only supported by PICO 4 Pro and PICO 4 Enterprise.
        /// </summary>
        /// <param name="enable">Whether to enable/disable face tracking:
        /// * `true`: enable
        /// * `false`: disable
        /// </param>
        public static void EnableFaceTracking(bool enable) {
            PXR_Plugin.System.UPxr_EnableFaceTracking(enable);
        }

        /// <summary>
        /// Enables/disables lipsync.
        /// @note Only supported by PICO 4 Pro and PICO 4 Enterprise.
        /// </summary>
        /// <param name="enable">Whether to enable/disable lipsync:
        /// * `true`: enable
        /// * `false`: disable
        /// </param>
        public static void EnableLipSync(bool enable){
            PXR_Plugin.System.UPxr_EnableLipSync(enable);
        }

        /// <summary>
        /// Gets face tracking data.
        /// @note Only supported by PICO 4 Pro and PICO 4 Enterprise.
        /// </summary>
        /// <param name="ts">(Optional) A reserved parameter, pass `0`.</param>
        /// <param name="flags">The face tracking mode to retrieve data for. Enumertions:
        /// * `PXR_GET_FACE_DATA_DEFAULT` (invalid, only for making it compatible with older SDK version)
        /// * `PXR_GET_FACE_DATA`: face only
        /// * `PXR_GET_LIP_DATA`: lipsync only
        /// * `PXR_GET_FACELIP_DATA`: hybrid (both face and lipsync)
        /// </param>
        /// <param name="faceTrackingInfo">Returns the `PxrFaceTrackingInfo` struct that contains the following face tracking data:
        /// * `timestamp`: Int64, reserved field
        /// * `blendShapeWeight`: float[], pass `0`.
        /// * `videoInputValid`: float[], the input validity of the upper and lower parts of the face.
        /// * `laughingProb`: float[], the coefficient of laughter.
        /// * `emotionProb`: float[], the emotion factor.
        /// * `reserved`: float[], reserved field.
        /// </param>
        public static void GetFaceTrackingData(Int64 ts, GetDataType flags, ref PxrFaceTrackingInfo faceTrackingInfo) {
            PXR_Plugin.System.UPxr_GetFaceTrackingData( ts,  (int)flags, ref  faceTrackingInfo);
        }

        /// <summary>Sets a GPU or CPU level for the device.</summary>
        /// <param name="which">Choose to set a GPU or CPU level:
        /// * `CPU`
        /// * `GPU`
        /// </param>
        /// <param name="level">Select a level from the following:
        /// * `POWER_SAVINGS`: power-saving level
        /// * `SUSTAINED_LOW`: low level
        /// * `SUSTAINED_HIGH`: high level
        /// * `BOOST`: top-high level, be careful to use this level
        /// </param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int SetPerformanceLevels(PxrPerfSettings which, PxrSettingsLevel level)
        {
            return PXR_Plugin.System.UPxr_SetPerformanceLevels(which, level);
        }

        /// <summary>Gets the device's GPU or CPU level.</summary>
        /// <param name="which">Choose to get GPU or CPU level:
        /// * `CPU`
        /// * `GPU`
        /// </param>
        /// <returns>
        /// Returns one of the following levels:
        /// * `POWER_SAVINGS`: power-saving level
        /// * `SUSTAINED_LOW`: low level
        /// * `SUSTAINED_HIGH`: high level
        /// * `BOOST`: top-high level, be careful to use this level
        /// </returns>
        public static PxrSettingsLevel GetPerformanceLevels(PxrPerfSettings which)
        {
            return PXR_Plugin.System.UPxr_GetPerformanceLevels(which);
        }

        /// <summary>Sets FOV in four directions (left, right, up, and down) for specified eye(s).</summary>
        /// <param name="eye">The eye to set FOV for:
        /// * `LeftEye`
        /// * `RightEye`
        /// * `BothEye`
        /// </param>
        /// <param name="fovLeft">The horizontal FOV (in degrees) for the left part of the eye, for example, `47.5`.</param>
        /// <param name="fovRight">The horizontal FOV (in degrees) for the right part of the eye..</param>
        /// <param name="fovUp">The vertical FOV (in degrees) for the upper part of the eye.</param>
        /// <param name="fovDown">The vertical FOV (in degrees) for the lower part of the eye.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int SetEyeFOV(EyeType eye, float fovLeft, float fovRight, float fovUp, float fovDown)
        {
            return PXR_Plugin.Render.UPxr_SetEyeFOV(eye, fovLeft, fovRight, fovUp, fovDown);
        }

        /// <summary>
        /// Switches the face tracking mode.
        /// @note Only supported by PICO 4 Pro and PICO 4 Enterprise.
        /// </summary>
        /// <param name="value">
        /// `STOP_FT`: to stop the "Face Only" mode.
        /// `STOP_LIPSYNC`: to stop the "Lipsync Only" mode.
        /// `START_FT`: to start the "Face Only" mode.
        /// `START_LIPSYNC`: to start the "Lipsync Only" mode.
        /// </param>
        /// <returns>
        /// `0`: success
        /// `1`: failure
        /// </returns>
        public static int SetFaceTrackingStatus(PxrFtLipsyncValue value) {
            return PXR_Plugin.System.UPxr_SetFaceTrackingStatus(value);
        }

        /// <summary>
        /// Sets a tracking origin mode for the app.
        /// When the user moves in the virtual scene, the system tracks and calculates the user's positional changes based on the origin.
        /// </summary>
        /// <param name="originMode">Selects a tracking origin mode from the following:
        /// * `TrackingOriginModeFlags.Device`: Device mode. The system sets the device's initial position as the origin. The device's height from the floor is not calculated.
        /// * `TrackingOriginModeFlags.Floor`: Floor mode. The system sets an origin based on the device's original position and the device's height from the floor. 
        /// </param>
        public static void SetTrackingOrigin(PxrTrackingOrigin originMode)
        {
            PXR_Plugin.System.UPxr_SetTrackingOrigin(originMode);
        }

        /// <summary>
        /// Gets the tracking origin mode of the app.
        /// </summary>
        /// <param name="originMode">Returns the app's tracking origin mode:
        /// * `TrackingOriginModeFlags.Device`: Device mode
        /// * `TrackingOriginModeFlags.Floor`: Floor mode
        /// For the description of each mode, refer to `SetTrackingOrigin`.
        /// </param>
        public static void GetTrackingOrigin(out PxrTrackingOrigin originMode)
        {
            originMode = PxrTrackingOrigin.Eye;
            PXR_Plugin.System.UPxr_GetTrackingOrigin(ref originMode);
        }

        /// <summary>
        /// Turns on the power service for a specified object.
        /// </summary>
        /// <param name="objName">The name of the object to turn on the power service for.</param>
        /// <returns>Whether the power service has been turned on:
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool StartBatteryReceiver(string objName)
        {
            return PXR_Plugin.System.UPxr_StartBatteryReceiver(objName);
        }

        /// <summary>
        /// Turns off the power service.
        /// </summary>
        /// <returns>Whether the power service has been turned off:
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool StopBatteryReceiver()
        {
            return PXR_Plugin.System.UPxr_StopBatteryReceiver();
        }

        /// <summary>
        /// Sets the brightness for the current HMD.
        /// </summary>
        /// <param name="brightness">Target brightness. Value range: [0,255].</param>
        /// <returns>Whether the brightness has been set successfully:
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool SetCommonBrightness(int brightness)
        {
            return PXR_Plugin.System.UPxr_SetBrightness(brightness);
        }

        /// <summary>
        /// Gets the brightness of the current HMD.
        /// </summary>
        /// <returns>An int value that indicates the brightness. Value range: [0,255].</returns>
        public static int GetCommonBrightness()
        {
            return PXR_Plugin.System.UPxr_GetCurrentBrightness();
        }

        /// <summary>
        /// Gets the brightness level of the current screen.
        /// </summary>
        /// <returns>An int array. The first bit is the total brightness level supported, the second bit is the current brightness level, and it is the interval value of the brightness level from the third bit to the end bit.</returns>
        public static int[] GetScreenBrightnessLevel()
        {
            return PXR_Plugin.System.UPxr_GetScreenBrightnessLevel();
        }

        /// <summary>
        /// Sets a brightness level for the current screen.
        /// </summary>
        /// <param name="brightness">Brightness mode:
        /// * `0`: system default brightness setting.
        /// * `1`: custom brightness setting, you can then set param `level`.
        /// </param>
        /// <param name="level">Brightness level. Value range: [1,255].</param>
        public static void SetScreenBrightnessLevel(int brightness, int level)
        {
            PXR_Plugin.System.UPxr_SetScreenBrightnessLevel(brightness, level);
        }

        /// <summary>
        /// Turns on the volume service for a specified object.
        /// </summary>
        /// <param name="objName">The name of the object to turn on the volume service for.</param>
        /// <returns>Whether the volume service has been turned on:
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool StartAudioReceiver(string objName)
        {
            return PXR_Plugin.System.UPxr_StartAudioReceiver(objName);
        }

        /// <summary>
        /// Turns off the volume service.
        /// </summary>
        /// <returns>Whether the volume service has been turned off:
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool StopAudioReceiver()
        {
            return PXR_Plugin.System.UPxr_StopAudioReceiver();
        }

        /// <summary>
        /// Gets the maximum volume. 
        /// </summary>
        /// <returns>An int value that indicates the maximum volume.</returns>
        public static int GetMaxVolumeNumber()
        {
            return PXR_Plugin.System.UPxr_GetMaxVolumeNumber();
        }

        /// <summary>
        /// Gets the current volume.
        /// </summary>
        /// <returns>An int value that indicates the current volume. Value range: [0,15].</returns>
        public static int GetCurrentVolumeNumber()
        {
            return PXR_Plugin.System.UPxr_GetCurrentVolumeNumber();
        }

        /// <summary>
        /// Increases the volume.
        /// </summary>
        /// <returns>Whether the volume has been increased:
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool VolumeUp()
        {
            return PXR_Plugin.System.UPxr_VolumeUp();
        }

        /// <summary>
        /// Decreases the volume.
        /// </summary>
        /// <returns>Whether the volume has been decreased:
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool VolumeDown()
        {
            return PXR_Plugin.System.UPxr_VolumeDown();
        }

        /// <summary>
        /// Sets a volume. 
        /// </summary>
        /// <param name="volume">The target volume. Value range: [0,15].</param>
        /// <returns>Whether the target volume has been set:
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool SetVolumeNum(int volume)
        {
            return PXR_Plugin.System.UPxr_SetVolumeNum(volume);
        }
    }
}

