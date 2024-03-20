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
using UnityEngine;

namespace Unity.XR.PXR
{
    public class PXR_Boundary
    {
        /// <summary>
        /// Sets the boundary as visible or invisible. Note: The setting defined in this function can be overridden by system settings (e.g., proximity trigger) or user settings (e.g., disabling the boundary system).
        /// </summary>
        /// <param name="value">Whether to set the boundary as visible or invisble: `true`-visible; `false`-invisible.</param>
        public static void SetVisible(bool value)
        {
            PXR_Plugin.Boundary.UPxr_SetBoundaryVisiable(value);
        }

        /// <summary>
        /// Gets whether the boundary is visible.
        /// </summary>
        /// <returns>`true`-visible; `false`-invisible.</returns>
        public static bool GetVisible()
        {
            return PXR_Plugin.Boundary.UPxr_GetBoundaryVisiable();
        }

        /// <summary>
        /// Checks whether the boundary is configured. Boundary-related functions are available for use only if the boundary is configured.
        /// </summary>
        /// <returns>`true`-configured; `false`-not configured.</returns>
        public static bool GetConfigured()
        {
            return PXR_Plugin.Boundary.UPxr_GetBoundaryConfigured();
        }

        /// <summary>
        /// Checks whether the boundary is enabled.
        /// </summary>
        /// <returns>`true`-enabled; `false`-not enabled.</returns>
        public static bool GetEnabled()
        {
            return PXR_Plugin.Boundary.UPxr_GetBoundaryEnabled();
        }

        /// <summary>
        /// Checks whether a tracked node (Left hand, Right hand, Head) will trigger the boundary.
        /// </summary>
        /// <param name="node">The node to track: HandLeft-left controller; HandRight-right controller; Head-HMD.</param>
        /// <param name="boundaryType">The boundary type: `OuterBoundary`-boundary (custom boundary or in-site fast boundary); `PlayArea`-the maximum rectangle in the custom boundary (no such a rectangle in the in-site fast boundary).</param>
        /// <returns>
        /// A struct that contains the following details:
        /// `IsTriggering`: bool, whether the boundary is triggered;
        /// `ClosestDistance`: float, the minimum distance between the tracked node and the boundary;
        /// `ClosestPoint`: vector3, the closest point between the tracked node and the boundary;
        /// `ClosestPointNormal`: vector3, the normal line of the closest point;
        /// `valid`: bool, whether the result returned is valid.
        /// </returns>
        public static PxrBoundaryTriggerInfo TestNode(BoundaryTrackingNode node, BoundaryType boundaryType)
        {
            return PXR_Plugin.Boundary.UPxr_TestNodeIsInBoundary(node, boundaryType);
        }

        /// <summary>
        /// Checks whether a tracked point will trigger the boundary.
        /// </summary>
        /// <param name="point">The coordinate of the point.</param>
        /// <param name="boundaryType">The boundary type: `OuterBoundary`-boundary (custom boundary or in-site fast boundary); `PlayArea`-customize the maximum rectangle in the custom boundary (no such rectangle for in-site fast boundary).</param>
        /// <returns>
        /// A struct that contains the following details:
        /// `IsTriggering`: bool, whether the boundary is triggered;
        /// `ClosestDistance`: float, the minimum distance between the tracked node and the boundary;
        /// `ClosestPoint`: vector3, the closest point between the tracked node and the boundary;
        /// `ClosestPointNormal`: vector3, the normal line of the closest point;
        /// `valid`: bool, whether the result returned is valid.
        /// </returns>
        public static PxrBoundaryTriggerInfo TestPoint(PxrVector3f point, BoundaryType boundaryType)
        {
            return PXR_Plugin.Boundary.UPxr_TestPointIsInBoundary(point, boundaryType);
        }

        /// <summary>
        /// Gets the collection of boundary points.
        /// </summary>
        /// <param name="boundaryType">The boundary type: `OuterBoundary`-boundary (custom boundary or in-site fast boundary); `PlayArea`-customize the maximum rectangle in the custom boundary (no such rectangle for in-site fast boundary).</param>
        /// <returns>A collection of boundary points.</returns>
        public static Vector3[] GetGeometry(BoundaryType boundaryType)
        {
            return PXR_Plugin.Boundary.UPxr_GetBoundaryGeometry(boundaryType);
        }

        /// <summary>
        /// Gets the size of the play area for the custom boundary.
        /// </summary>
        /// <param name="boundaryType">The boundary type: `OuterBoundary`-boundary (custom boundary or in-site fast boundary); `PlayArea`-customize the maximum rectangle in the custom boundary (no such rectangle for in-site fast boundary).</param>
        /// <returns>A vector3 value, `(x, y, z)`: `x`-the longer side of the play area; `y`-always return 1; `z`-the shorter side of the play area. Note: As rectangle is not available for in-site fast boundary, `(0, 0, 0)` will be returned.</returns>
        public static Vector3 GetDimensions(BoundaryType boundaryType)
        {
            return PXR_Plugin.Boundary.UPxr_GetBoundaryDimensions(boundaryType);
        }

        /// <summary>
        /// Gets the camera image of the device and use it as the environmental background. Before calling this function, make sure you have set the clear flags of the camera to solid color and have set the background color of the camera to 0 for the alpha channel.
        /// @note If the app is paused, this function will cease. Therefore, you need to call this function again after the app has been resumed.
        /// </summary>
        /// <param name="value">Whether to enable SeeThrough: `true`-enable; `false`-do not enable.</param>
        public static void EnableSeeThroughManual(bool value)
        {
            PXR_Plugin.Boundary.UPxr_SetSeeThroughBackground(value);
        }

        /// <summary>
        /// Gets the current status of seethrough tracking.
        /// </summary>
        /// <returns>Returns `PxrTrackingState`. Below are the enumerations:
        /// * `LostNoReason`: no reason
        /// * `LostCamera`: camera calibration data error
        /// * `LostHighLight`: environment lighting too bright
        /// * `LostLowLight`: environment lighting too dark
        /// * `LostLowFeatureCount`: few environmental features
        /// * `LostReLocation`: relocation in progress
        /// * `LostInitialization`: initialization in progress
        /// * `LostNoCamera`: camera data error
        /// * `LostNoIMU`: IMU data error
        /// * `LostIMUJitter`: IMU data jitter
        /// * `LostUnknown`: unknown error
        /// </returns>
        public static PxrTrackingState GetSeeThroughTrackingState() {
            return PXR_Plugin.Boundary.UPxr_GetSeeThroughTrackingState();
        }
        
        /// <summary>
        /// disable or enable boundary
        /// </summary>
        /// <param name="value"></param>
        public static void SetGuardianSystemDisable(bool value)
        {
            PXR_Plugin.Boundary.UPxr_SetGuardianSystemDisable(value);
        }

        /// <summary>
        /// Uses the global pose.
        /// </summary>
        /// <param name="value">Specifies whether to use the global pose.
        /// * `true`: use
        /// * `false`: do not use
        /// </param>
        public static void UseGlobalPose(bool value)
        {
            PXR_Plugin.Boundary.UPxr_SetSeeThroughState(value);
        }
    }
}


