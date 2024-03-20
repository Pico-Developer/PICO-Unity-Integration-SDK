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

namespace Unity.XR.PXR
{
    /// <summary>
    /// The codes that indicates the state of motion tracking features.
    /// </summary>
    public enum TrackingStateCode
    {
        /// <summary>
        /// Request succeeded.
        /// </summary>
        PXR_MT_SUCCESS = 0,
        /// <summary>
        /// Request failed.
        /// </summary>
        PXR_MT_FAILURE = -1,
        /// <summary>
        /// Invalid mode.
        /// </summary>
        PXR_MT_MODE_NONE = -2,
        /// <summary>
        /// The current device does not support this feature.
        /// </summary>
        PXR_MT_DEVICE_NOT_SUPPORT = -3,
        /// <summary>
        /// This feature is not started.
        /// </summary>
        PXR_MT_SERVICE_NEED_START = -4,
        /// <summary>
        /// Eye tracking permission denied.
        /// </summary>
        PXR_MT_ET_PERMISSION_DENIED = -5,
        /// <summary>
        /// Face tracking permission denied.
        /// </summary>
        PXR_MT_FT_PERMISSION_DENIED = -6,
        /// <summary>
        /// Microphone permission denied.
        /// </summary>
        PXR_MT_MIC_PERMISSION_DENIED = -7,
        /// <summary>
        /// (Reserved)
        /// </summary>
        PXR_MT_SYSTEM_DENIED = -8,
        /// <summary>
        /// Unknown error.
        /// </summary>
        PXR_MT_UNKNOW_ERROR = -9
    }

    #region Eye Tracking
    /// <summary>
    /// Eye tracking modes.
    /// </summary>
    public enum EyeTrackingMode
    {
        /// <summary>
        /// To disable eye tracking. 
        /// </summary>
        PXR_ETM_NONE = -1,
        /// <summary>
        /// To enable eye tracking.
        /// </summary>
        PXR_ETM_BOTH = 0,
        /// <summary>
        /// (Reserved)
        /// </summary>
        PXR_ETM_COUNT = 1
    }

    public enum PerEyeUsage
    {
        LeftEye = 0,
        RightEye = 1,
        Combined = 2,
        EyeCount = 3
    }

    /// <summary>
    /// Eye tracking data flags.
    /// </summary>
    public enum EyeTrackingDataGetFlags : long
    {
        /// <summary>
        /// Do not return any data.
        /// </summary>
        PXR_EYE_DEFAULT = 0,
        /// <summary>
        /// To return the positions of both eyes.
        /// </summary>
        PXR_EYE_POSITION = 1 << 0,
        /// <summary>
        /// To return the orientations of both eyes.
        /// </summary>
        PXR_EYE_ORIENTATION = 1 << 1
    }

    /// <summary>
    /// The information to pass for starting eye tracking.
    /// </summary>
    public struct EyeTrackingStartInfo
    {
        private int apiVersion;
        /// <summary>
        /// Whether the app needs eye tracking calibration.
        /// * `0`: needs
        /// * `1`: does not need
        /// </summary>
        public byte needCalibration;
        /// <summary>
        /// Select an eye tracking mode for the app. Refer to the `EyeTrackingMode` enum for details.
        /// </summary>
        public EyeTrackingMode mode;
        public void SetVersion(int version)
        {
            apiVersion = version;
        }
        public override string ToString()
        {
            return string.Format("apiVersion :{0}, needCalibration:{1}, mode:{2}", apiVersion, needCalibration, mode);
        }

    }

    public struct EyeTrackingStopInfo
    {
        private int apiVersion;
        public void SetVersion(int version)
        {
            apiVersion = version;
        }
        public override string ToString()
        {
            return string.Format("apiVersion :{0}", apiVersion);
        }
    }

    /// <summary>
    /// Information about the state of eye tracking.
    /// </summary>
    public struct EyeTrackingState
    {
        private int apiVersion;
        /// <summary>
        /// Eye tracking mode. Refer to the `EyeTrackingMode` enum for details.
        /// </summary>
        public EyeTrackingMode currentTrackingMode;
        /// <summary>
        /// The state code of eye tracking. Refer to the `TrackingStateCode` enum for details.
        /// </summary>
        public TrackingStateCode code;
        public void SetVersion(int version)
        {
            apiVersion = version;
        }
        public override string ToString()
        {
            return string.Format("apiVersion :{0}, currentTrackingMode:{1}, code:{2}", apiVersion, currentTrackingMode, code);
        }
    }

    /// <summary>
    /// The information to pass for getting eye tracking data.
    /// </summary>
    public struct EyeTrackingDataGetInfo
    {
        private int apiVersion;
        /// <summary>
        /// Reserved. Pass `0`.
        /// </summary>
        public long displayTime;
        /// <summary>
        /// Specifies what eye tracking data to return. Refer to the `EyeTrackingDataGetFlags` enum for details.
        /// </summary>
        public EyeTrackingDataGetFlags flags;
        public void SetVersion(int version)
        {
            apiVersion = version;
        }
        public override string ToString()
        {
            return string.Format("apiVersion :{0}, displayTime:{1}, flags:{2}", apiVersion, displayTime, flags);
        }
    }

    public struct PxrPose
    {
        public PxrVector3f position;
        public PxrVector4f orientation;
        public override string ToString()
        {
            return string.Format("orientation :({0},{1},{2},{3}) position:({4},{5},{6})",
                orientation.x.ToString("F6"), orientation.y.ToString("F6"), orientation.z.ToString("F6"), orientation.w.ToString("F6"),
                position.x.ToString("F6"), position.y.ToString("F6"), position.z.ToString("F6"));
        }
    };

    public struct PerEyeData
    {
        private int apiVersion;
        public PxrPose pose;
        public byte isPoseValid;
        public float openness;
        public byte isOpennessValid;
        public void SetVersion(int version)
        {
            apiVersion = version;
        }
        public override string ToString()
        {
            return string.Format("apiVersion :{0}, pose:{1}, isPoseValid:{2}, openness:{3}, isOpennessValid:{4}", apiVersion, pose, isPoseValid, openness, isOpennessValid);
        }
    }

    public struct EyeTrackingData
    {
        private int apiVersion;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)PerEyeUsage.EyeCount)]
        public PerEyeData[] eyeDatas;
        public void SetVersion(int version)
        {
            apiVersion = version;
        }
        public override string ToString()
        {
            return string.Format("apiVersion :{0},\n eyeDatas[0]:{1},\n eyeDatas[1]:{2},\n eyeDatas[2]:{3}", apiVersion, eyeDatas[0], eyeDatas[1], eyeDatas[2]);
        }
    }

    /// <summary>
    /// The information about the pupils of both eyes.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct EyePupilInfo
    {
        /// <summary>
        /// The diameter (unit: millimeters) of the left eye's pupil.
        /// </summary>
        public float leftEyePupilDiameter;
        /// <summary>
        /// The diameter (unit: millimeters) of the right eye's pupil.
        /// </summary>
        public float rightEyePupilDiameter;
        /// <summary>
        /// The position of the left eye's pupil.
        /// </summary>
        public fixed float leftEyePupilPosition[2];
        /// <summary>
        /// The position of the right eye's pupil.
        /// </summary>
        public fixed float rightEyePupilPosition[2];
        public override string ToString()
        {
            string str = string.Format("leftEyePupilDiameter :{0}, rightEyePupilDiameter:{1}", leftEyePupilDiameter.ToString("F6"), rightEyePupilDiameter.ToString("F6"));
            for (int i = 0; i < 2; i++)
            {
                str += string.Format("\nleftEyePupilPosition[{0}] :{1}", i, leftEyePupilPosition[i].ToString("F6"));
                str += string.Format(" rightEyePupilPosition[{0}] :{1}", i, rightEyePupilPosition[i].ToString("F6"));
            }
            return str;
        }
    }
    #endregion

    #region Face Tracking
    /// <summary>
    /// Face tracking modes.
    /// </summary>
    public enum FaceTrackingSupportedMode
    {
        /// <summary>
        /// No face tracking.
        /// </summary>
        PXR_FTM_NONE = -1,
        /// <summary>
        /// Face tracking only (without lipsync).
        /// </summary>
        PXR_FTM_FACE = 0,
        /// <summary>
        /// Lipsync only.
        /// </summary>
        PXR_FTM_LIPS = 1,
        /// <summary>
        /// Hybrid mode. Enable both face tracking and lipsync. The lip data's output format is viseme.
        /// </summary>
        PXR_FTM_FACE_LIPS_VIS = 2,
        /// <summary>
        /// Hybrid mode. Enable both face tracking and lipsync. The lip data's output format is blendshape.
        /// </summary>
        PXR_FTM_FACE_LIPS_BS = 3,
        /// <summary>
        /// (Reserved)
        /// </summary>
        PXR_FTM_COUNT = 4
    }

    /// <summary>
    /// Specifies the face tracking data to return.
    /// </summary>
    public enum FaceTrackingDataGetFlags : long
    {
        /// <summary>
        /// To return all types of face tracking data.
        /// </summary>
        PXR_FACE_DEFAULT = 0,
    }

    /// <summary>
    /// The information to pass for starting face tracking.
    /// </summary>
    public struct FaceTrackingStartInfo
    {
        private int apiVersion;
        /// <summary>
        /// The face tracking mode to enable. Refer to the `FaceTrackingSupportedMode` enum for details.
        /// </summary>
        public FaceTrackingSupportedMode mode;
        public void SetVersion(int version)
        {
            apiVersion = version;
        }
        public override string ToString()
        {
            return string.Format("apiVersion :{0}, mode:{1}", apiVersion, mode);
        }
    }

    /// <summary>
    /// The information to pass for stopping face tracking.
    /// </summary>
    public struct FaceTrackingStopInfo
    {
        private int apiVersion;
        /// <summary>
        /// Determines whether to pause face tracking.
        /// * `0`: pause
        /// * `1`: do not pause
        /// </summary>
        public byte pause;
        public void SetVersion(int version)
        {
            apiVersion = version;
        }
        public override string ToString()
        {
            return string.Format("apiVersion :{0}, pause:{1}", apiVersion, pause);
        }
    }

    /// <summary>
    /// Information about the state of face tracking.
    /// </summary>
    public struct FaceTrackingState
    {
        private int apiVersion;
        /// <summary>
        /// The face tracking mode of the app. Refer to the `FaceTrackingSupportedMode` enum for details.
        /// </summary>
        public FaceTrackingSupportedMode currentTrackingMode;
        /// <summary>
        /// Face tracking state code. Refer to the `TrackingStateCode` enum for details.
        /// </summary>
        public TrackingStateCode code;
        public void SetVersion(int version)
        {
            apiVersion = version;
        }
        public override string ToString()
        {
            return string.Format("apiVersion :{0}, currentTrackingMode:{1}, code:{2}", apiVersion, currentTrackingMode, code);
        }
    }

    /// <summary>
    /// The information to pass for getting face tracking data.
    /// </summary>
    public struct FaceTrackingDataGetInfo
    {
        private int apiVersion;
        /// <summary>
        /// Reserved. Pass `0`.
        /// </summary>
        public long displayTime;
        public void SetVersion(int version)
        {
            apiVersion = version;
        }
        public FaceTrackingDataGetFlags flags;
        public override string ToString()
        {
            return string.Format("apiVersion :{0}, displayTime:{1}, flags:{2}", apiVersion, displayTime, flags);
        }
    }

    /// <summary>
    /// Face tracking data.
    /// </summary>
    public unsafe struct FaceTrackingData
    {
        private int apiVersion;
        /// <summary>
        /// A float* value, the length must be 72. Refer to `BlendShapeIndex` for the definition of each value.
        /// </summary>
        public float* blendShapeWeight;
        /// <summary>
        /// The timestamp for the current data.
        /// </summary>
        public long timestamp;
        /// <summary>
        /// The laughing prob is a float ranging from `0` to `1`.
        /// </summary>
        public float laughingProb;
        /// <summary>
        /// Whether the data of the eye area is valid.
        /// </summary>
        public byte eyeValid;
        /// <summary>
        /// Whether the data of the face area is valid.
        /// </summary>
        public byte faceValid;
        public void SetVersion(int version)
        {
            apiVersion = version;
        }
        public override string ToString()
        {
            string str = string.Format("apiVersion :{0}, timestamp:{1}, laughingProb:{2}, eyeValid:{3}, faceValid:{4}\n", apiVersion, timestamp, laughingProb, eyeValid, faceValid);
            for (int i = 0; i < 72; i++)
            {
                str += string.Format(" blendShapeWeight[{0}]:{1}", i, blendShapeWeight[i].ToString("F6"));
            }

            return str;
        }
    }
    #endregion

    public class PXR_MotionTracking
    {
        //Eye Tracking
        public const int PXR_EYE_TRACKING_API_VERSION = 1;

        /// <summary>
        /// Wants eye tracking service for the current app.
        /// </summary>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        public static int WantEyeTrackingService()
        {
            return PXR_Plugin.MotionTracking.UPxr_WantEyeTrackingService();
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
        public static int GetEyeTrackingSupported(ref bool supported, ref int supportedModesCount, ref EyeTrackingMode supportedModes)
        {
            return PXR_Plugin.MotionTracking.UPxr_GetEyeTrackingSupported(ref supported, ref supportedModesCount, ref supportedModes);
        }

        /// <summary>
        /// Starts eye tracking.
        /// @note Supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
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
        /// @note Supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
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
        /// @note Supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
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
        /// @note Supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
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
        /// @note Supported by PICO 4 Enterprise.
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
        /// @note Supported by PICO 4 Enterprise.
        /// </summary>
        /// <param name="eyePupilPosition">Returns the diameters and positions of both pupils.</param>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        public static int GetEyePupilInfo(ref EyePupilInfo eyePupilPosition)
        {
            return PXR_Plugin.MotionTracking.UPxr_GetEyePupilInfo(ref eyePupilPosition);
        }

        public static int GetPerEyePose(ref long timestamp, ref Posef leftEyePose, ref Posef rightPose)
        {
            return PXR_Plugin.MotionTracking.UPxr_GetPerEyePose(ref timestamp, ref leftEyePose, ref rightPose);
        }

        public static int GetEyeBlink(ref long timestamp, ref bool isLeftBlink, ref bool isRightBlink)
        {
            return PXR_Plugin.MotionTracking.UPxr_GetEyeBlink(ref timestamp, ref isLeftBlink, ref isRightBlink);
        }

        //Face Tracking
        public const int PXR_FACE_TRACKING_API_VERSION = 1;

        /// <summary>
        /// Wants face tracking service for the current app.
        /// </summary>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        public static int WantFaceTrackingService()
        {
            return PXR_Plugin.MotionTracking.UPxr_WantFaceTrackingService();
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
        public static int GetFaceTrackingSupported(ref bool supported, ref int supportedModesCount, ref FaceTrackingSupportedMode supportedModes)
        {
            return PXR_Plugin.MotionTracking.UPxr_GetFaceTrackingSupported(ref supported, ref supportedModesCount, ref supportedModes);
        }

        /// <summary>
        /// Starts face tracking.
        /// @note Supported by PICO 4 Pro and PICO 4 Enterprise.
        /// </summary>
        /// <param name="startInfo">Passes the information for starting face tracking.</param>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        public static int StartFaceTracking(ref FaceTrackingStartInfo startInfo)
        {
            startInfo.SetVersion(PXR_FACE_TRACKING_API_VERSION);
            return PXR_Plugin.MotionTracking.UPxr_StartFaceTracking(ref startInfo);
        }

        /// <summary>
        /// Stops face tracking.
        /// @note Supported by PICO 4 Pro and PICO 4 Enterprise.
        /// </summary>
        /// <param name="stopInfo">Passes the information for stopping face tracking.</param>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        public static int StopFaceTracking(ref FaceTrackingStopInfo stopInfo)
        {
            stopInfo.SetVersion(PXR_FACE_TRACKING_API_VERSION);
            return PXR_Plugin.MotionTracking.UPxr_StopFaceTracking(ref stopInfo);
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
        public static int GetFaceTrackingState(ref bool isTracking, ref FaceTrackingState state)
        {
            state.SetVersion(PXR_FACE_TRACKING_API_VERSION);
            return PXR_Plugin.MotionTracking.UPxr_GetFaceTrackingState(ref isTracking, ref state);
        }

        /// <summary>
        /// Gets face tracking data.
        /// @note Supported by PICO 4 Pro and PICO 4 Enterprise.
        /// </summary>
        /// <param name="getInfo">Specifies the face tracking data you want.</param>
        /// <param name="data">Returns the desired face tracking data.</param>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        public static int GetFaceTrackingData(ref FaceTrackingDataGetInfo getInfo, ref FaceTrackingData data)
        {
            getInfo.SetVersion(PXR_FACE_TRACKING_API_VERSION);
            data.SetVersion(PXR_FACE_TRACKING_API_VERSION);
            return PXR_Plugin.MotionTracking.UPxr_GetFaceTrackingData1(ref getInfo, ref data);
        }

    }
}