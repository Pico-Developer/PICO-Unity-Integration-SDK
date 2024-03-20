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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Unity.XR.PXR
{
    public class PXR_EyeTracking
    {
        /// <summary>
        /// Gets the PosMatrix of the head. 
        /// @note Only supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="matrix">A Matrix4x4 value returned by the result.</param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool GetHeadPosMatrix(out Matrix4x4 matrix)
        {
            matrix = Matrix4x4.identity;

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            if (!GetEyeTrackingDevice(out InputDevice device))
                return false;

            Vector3 headPos = Vector3.zero;
            if (!device.TryGetFeatureValue(CommonUsages.devicePosition, out headPos))
            {
                Debug.LogError("PXRLog Failed at GetHeadPosMatrix Pos");
                return false;
            }

            Quaternion headRot = Quaternion.identity;
            if (!device.TryGetFeatureValue(CommonUsages.deviceRotation, out headRot))
            {
                Debug.LogError("PXRLog Failed at GetHeadPosMatrix Rot");
                return false;
            }

            matrix = Matrix4x4.TRS(headPos, headRot, Vector3.one);
            return true;
        }

        static InputDevice curDevice;

        /// <summary>
        /// Gets the input device for eye tracking data. 
        /// @note Only supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="device">The input device returned by the result.</param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        static bool GetEyeTrackingDevice(out InputDevice device)
        {
            if (curDevice!= null&& curDevice.isValid)
            {
                device = curDevice;
                return true;
            }

            device = default;

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.EyeTracking | InputDeviceCharacteristics.HeadMounted, devices);
            if (devices.Count == 0)
            {
                Debug.LogError("PXRLog Failed at GetEyeTrackingDevice devices.Count");
                return false;
            }
            device = devices[0];
            curDevice = device;

            if (!device.isValid)
            {
                Debug.LogError("PXRLog Failed at GetEyeTrackingDevice device.isValid");
            }
            return device.isValid;
        }

        /// <summary>
        /// Gets the position of the combined gaze point.
        /// @note Only supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="point">A vector3 value returned by the result.</param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool GetCombineEyeGazePoint(out Vector3 point)
        {
            point = Vector3.zero;

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            if (!GetEyeTrackingDevice(out InputDevice device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.combineEyePoint, out point))
            {
                Debug.Log("PXRLog Failed at GetCombineEyeGazePoint point");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the direction of the combined gaze point.
        /// @note Only supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="vector">A vector3 value returned by the result.</param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool GetCombineEyeGazeVector(out Vector3 vector)
        {
            vector = Vector3.zero;

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            if (!GetEyeTrackingDevice(out InputDevice device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.combineEyeVector, out vector))
            {
                Debug.LogError("PXRLog Failed at GetCombineEyeGazeVector vector");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the openness/closeness of the left eye. 
        /// @note Only supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="openness">A float value returned by the result. The value ranges from `0.0` to `1.0`. `0.0` incicates completely closed, `1.0` indicates completely open.</param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool GetLeftEyeGazeOpenness(out float openness)
        {
            openness = 0;

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            if (!GetEyeTrackingDevice(out InputDevice device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.leftEyeOpenness, out openness))
            {
                Debug.LogError("PXRLog Failed at GetLeftEyeGazeOpenness openness");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the openness/closeness of the right eye. 
        /// @note Only supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="openness">A float value returned by the result. The value ranges from `0.0` to `1.0`. `0.0` indicates completely closed, `1.0` indicates completely open.</param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool GetRightEyeGazeOpenness(out float openness)
        {
            openness = 0;

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            if (!GetEyeTrackingDevice(out InputDevice device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.rightEyeOpenness, out openness))
            {
                Debug.LogError("PXRLog Failed at GetRightEyeGazeOpenness openness");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets whether the data of the current left eye is available. 
        /// @note Only supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="status">An int value returned by the result: 
        /// * `0`: not available
        /// * `1`: available
        /// </param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool GetLeftEyePoseStatus(out uint status)
        {
            status = 0;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            if (!GetEyeTrackingDevice(out InputDevice device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.leftEyePoseStatus, out status))
            {
                Debug.LogError("PXRLog Failed at GetLeftEyePoseStatus status");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets whether the data of the current right eye is available.
        /// @note Only supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="status">An int value returned by the result: 
        /// * `0`: not available
        /// * `1`: available
        /// </param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool GetRightEyePoseStatus(out uint status)
        {
            status = 0;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            if (!GetEyeTrackingDevice(out InputDevice device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.rightEyePoseStatus, out status))
            {
                Debug.LogError("PXRLog Failed at GetRightEyePoseStatus status");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets whether the data of the combined eye is available.
        /// @note Only supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="status">An int value returned by the result: 
        /// `0`: not available
        /// `1`: available
        /// </param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool GetCombinedEyePoseStatus(out uint status)
        {
            status = 0;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            if (!GetEyeTrackingDevice(out InputDevice device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.combinedEyePoseStatus, out status))
            {
                Debug.LogError("PXRLog Failed at GetCombinedEyePoseStatus status");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the position of the left eye in a coordinate system. The upper-right point of the sensor is taken as the origin (0, 0) and the lower-left point is taken as (1, 1). 
        /// @note Only supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="position">A vector3 value returned by the result.</param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool GetLeftEyePositionGuide(out Vector3 position)
        {
            position = Vector3.zero;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            if (!GetEyeTrackingDevice(out InputDevice device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.leftEyePositionGuide, out position))
            {
                Debug.LogError("PXRLog Failed at GetLeftEyePositionGuide pos");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the position of the right eye in a coordinate system. The upper-right point of the sensor is taken as the origin (0, 0) and the lower-left point is taken as (1, 1). 
        /// @note Only supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="position">A vector3 value returned by the result.</param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool GetRightEyePositionGuide(out Vector3 position)
        {
            position = Vector3.zero;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            if (!GetEyeTrackingDevice(out InputDevice device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.rightEyePositionGuide, out position))
            {
                Debug.LogError("PXRLog Failed at GetRightEyePositionGuide pos");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the foveated gaze direction (i.e., the central point of fixed foveated rendering). 
        /// @note Only supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="direction">A vector3 value returned by the result.</param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool GetFoveatedGazeDirection(out Vector3 direction)
        {
            direction = Vector3.zero;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            if (!GetEyeTrackingDevice(out InputDevice device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.foveatedGazeDirection, out direction))
            {
                Debug.LogError("PXRLog Failed at GetFoveatedGazeDirection direction");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets whether the current foveated gaze tracking data is available.
        /// @note Only supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="status">An int value returned by the result: 
        /// * `0`: not available 
        /// * `1`: available
        /// </param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool GetFoveatedGazeTrackingState(out uint state)
        {
            state = 0;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            if (!GetEyeTrackingDevice(out InputDevice device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.foveatedGazeTrackingState, out state))
            {
                Debug.LogError("PXRLog Failed at GetFoveatedGazeTrackingState state");
                return false;
            }
            return true;
        }

    }
}