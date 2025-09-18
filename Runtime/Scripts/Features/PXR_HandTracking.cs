#if !PICO_OPENXR_SDK
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
using System.Runtime.InteropServices;
using UnityEngine;

#if PICO_LIVE_PREVIEW && UNITY_EDITOR
using Unity.XR.PICO.LivePreview;
#endif

namespace Unity.XR.PXR
{
    public static class PXR_HandTracking
    {
        /// <summary>Gets whether hand tracking is enabled or disabled.</summary>
        /// <returns>
        /// * `true`: enabled
        /// * `false`: disabled
        /// </returns>
        ///interface has been deprecated
        [Obsolete("interface has been deprecated", true)]
        public static bool GetSettingState()
        {
            return false;
            
        }

        /// <summary>Gets the current active input device.</summary>
        /// <returns>The current active input device:
        /// * `HeadActive`: HMD 
        /// * `ControllerActive`: controllers
        /// * `HandTrackingActive`: hands
        /// </returns>
        public static ActiveInputDevice GetActiveInputDevice()
        {
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
        ///     // The ray pose.
        ///     public Posef RayPose { get; private set; }
        ///     // Whether the ray was displayed.
        ///     public bool RayValid { get; private set; }
        ///     // Whether the ray pinched.
        ///     public bool Pinch { get; private set; }
        ///     // The strength of ray pinch.
        ///     public float PinchStrength { get; private set; }
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
            return PXR_Plugin.HandTracking.UPxr_GetHandTrackerJointLocations(hand, ref jointLocations);
        }

        /// <summary>
        /// Gets the scaling ratio of the hand model.
        /// </summary>
        /// <param name="hand">Specifies the hand to get scaling ratio for:
        /// * `HandLeft`: left hand
        /// * `HandRight`: right hand
        /// </param>
        /// <param name="scale">Returns the scaling ratio for the specified hand.</param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool GetHandScale(HandType hand,ref float scale)
        {
            return PXR_Plugin.HandTracking.UPxr_GetHandScale((int)hand, ref scale);
        }

    }
}
#endif
