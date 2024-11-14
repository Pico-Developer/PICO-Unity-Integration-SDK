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

using LitJson;
using System;
using UnityEngine;
using UnityEngine.XR;

namespace Unity.XR.PXR
{
    public static class PXR_Input
    {
        /// <summary>Device models.</summary>
        public enum ControllerDevice
        {
            /// <summary>PICO G2.</summary>
            G2 = 3,
            /// <summary>PICO Neo2.</summary>
            Neo2,
            /// <summary>PICO Neo3.</summary>
            Neo3,
            /// <summary>PICO 4.</summary>
            PICO_4,
            /// <summary>PICO G3.</summary>
            G3,
            /// <summary>PICO 4 Ultra.</summary>
            PICO_4U,
            /// <summary>A new device model.</summary>
            NewController = 10
        }

        /// <summmary>The controller types.</summary>
        public enum Controller
        {
            /// <summary>Left controller.</summary>
            LeftController,
            /// <summary>Right controller.</summary>
            RightController,
        }

        /// <summary>For specifying which controller to vibrate.</summary>
        public enum VibrateController
        {
            /// <summary>None.</summary>
            No = 0,
            /// <summary>The left controller.</summary>
            Left = 1,
            /// <summary>The right controller.</summary>
            Right = 2,
            /// <summary>Both the left and right controllers.</summary>
            LeftAndRight = 3,
        }

        /// <summary>For specifying the controller(s) to send the haptic data to.</summary>
        public enum VibrateType
        {
            /// <summary>Both controllers.</summary>
            None = 0,
            /// <summary>The left controller.</summary>
            LeftController = 1,
            /// <summary>The right controller.</summary>
            RightController = 2,
            /// <summary>Both controllers.</summary>
            BothController = 3,
            /// <summary>The left controller of PICO 4 Ultra device.</summary>
            LeftPICO4U = 4,
            /// <summary>The right controller of PICO 4 Ultra device.</summary>
            RightPICO4U = 8,
            /// <summary>Both controllers of PICO 4 Ultra device.</summary>
            BothPICO4U = 12,
        }

        /// <summary>Whether to keep the controller vibrating while caching haptic data.</summary>
        public enum CacheType
        {
            /// <summary>Don't cache.</summary>
            DontCache = 0,
            /// <summary>Cache haptic data and keep vibrating.</summary>
            CacheAndVibrate = 1,
            /// <summary>Cache haptic data and stop vibrating.</summary>
            CacheNoVibrate = 2,
        }

        /// <summary>Whether to enable audio channel inversion. Once audio channel inversion is enabled, the left controller vibrates with the audio data from the right channel, and vice versa.</summary>
        public enum ChannelFlip
        {
            /// <summary>Disable audio channel inversion.</summary>
            No,
            /// <summary>Enable audio channel inversion.</summary>
            Yes,
        }

        /// <summary>Whether to keep the controller vibrating while caching audio-based vibration data.</summary>
        public enum CacheConfig
        {
            /// <summary>Cache audio-based vibration data and keep vibrating.</summary>
            CacheAndVibrate = 1,
            /// <summary>Cache audio-based vibration data and stop vibrating.</summary>
            CacheNoVibrate = 2,
        }

        /// <summary>The status of controllers.</summary>
        public enum ControllerStatus
        {
            /// <summary>The controller is static.</summary>
            Static = 0,
            /// <summary>The controller is in 6DoF tracking mode.</summary>
            SixDof,
            /// <summary>The controller is in 3DoF tracking mode.</summary>
            ThreeDof,
            /// <summary>The controller remains static for a long time and is now in sleep mode.</summary>
            Sleep,
            /// <summary>The controller collided with something else during 3DoF tracking.</summary>
            CollidedIn3Dof,
            /// <summary>The controller collided with something else during 6DoF tracking.</summary>
            CollidedIn6Dof,
        }

        /// <summary>Gets the status of the specified controller.</summary>
        /// <param name="controller">Specifies the controller to get status for: `LeftController` or `RightController`.</param>
        /// <returns>The status of the specified controller:
        /// - `static`: the controller is static
        /// - `SixDof`: the controller is in 6DoF tracking mode
        /// - `ThreeDof`: the controller is in 3DoF tracking mode
        /// - `Sleep`: the controller remains static for a long time and is now in sleep mode
        /// - `CollidedIn3Dof`: the controller collided with something else during 3DoF tracking
        /// - `CollidedIn6Dof`: the controller collided with something else during 6DoF tracking
        /// </returns>
        public static ControllerStatus GetControllerStatus(Controller controller)
        {
            PxrControllerTracking pxrControllerTracking = new PxrControllerTracking();
            float[] headData = new float[7] { 0, 0, 0, 0, 0, 0, 0 };

            PXR_Plugin.Controller.UPxr_GetControllerTrackingState((uint)controller, PXR_Plugin.System.UPxr_GetPredictedDisplayTime(), headData, ref pxrControllerTracking);

            return (ControllerStatus)pxrControllerTracking.localControllerPose.status;
        }

        /// <summary>A callback that indicates the input source (hand poses/controllers) has changed.</summary>
        public static Action<ActiveInputDevice> InputDeviceChanged;

        /// <summary>
        /// Gets the current dominant controller.
        /// </summary>
        /// <returns>The current dominant controller: `LeftController`; `RightController`.</returns>
        public static Controller GetDominantHand()
        {
            return (Controller)PXR_Plugin.Controller.UPxr_GetControllerMainInputHandle();
        }

        /// <summary>
        /// Sets a controller as the dominant controller.
        /// </summary>
        /// <param name="controller">The controller to be set as the dominant controller: `0`-left controller; `1`-right controller.</param>
        public static void SetDominantHand(Controller controller)
        {
            PXR_Plugin.Controller.UPxr_SetControllerMainInputHandle((UInt32)controller);
        }

        /// <summary>
        /// Sets controller vibration, including vibration amplitude and duration.
        /// @note The `SendHapticImpulse` method offered by UnityXR is also supported. Click [here](https://docs.unity3d.com/ScriptReference/XR.InputDevice.SendHapticImpulse.html) for more information.
        /// </summary>
        /// <param name="strength">Vibration amplitude. The valid value ranges from `0` to `1`. The greater the value, the stronger the vibration amplitude. To stop controller vibration, call this function again and set this parameter to `0`.</param>
        /// <param name="time">Vibration duration. The valid value ranges from `0` to `65535` ms.</param>
        /// <param name="controller">The controller to set vibration for:
        /// * `0`: left controller
        /// * `1`: right controller
        /// </param>
        [Obsolete("Please use SendHapticImpulse instead")]
        public static void SetControllerVibration(float strength, int time, Controller controller)
        {
            PXR_Plugin.Controller.UPxr_SetControllerVibration((UInt32)controller, strength, time);
        }

        /// <summary>
        /// Gets the device model.
        /// </summary>
        /// <returns>The device model. Enumerations: `G2`, `Neo2`, `Neo3`, `NewController`, `PICO_4`.</returns> 
        public static ControllerDevice GetControllerDeviceType()
        {
            return (ControllerDevice)PXR_Plugin.Controller.UPxr_GetControllerType();
        }

        /// <summary>
        /// Gets the connection status for a specified controller.
        /// </summary>
        /// <param name="controller">The controller to get connection status for:
        /// * `0`: left controller
        /// * `1`: right controller
        /// </param>
        /// <returns>The connection status of the specified controller:
        /// * `true`: connected
        /// * `false`: not connected
        /// </returns>
        public static bool IsControllerConnected(Controller controller)
        {
            var state = false;
            switch (controller)
            {
                case Controller.LeftController:
                    InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(PXR_Usages.controllerStatus, out state);
                    return state;
                case Controller.RightController:
                    InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(PXR_Usages.controllerStatus, out state);
                    return state;
            }
            return state;
        }

        /// <summary>
        /// Sets the offset of the controller's display position to its real position.
        /// </summary>
        /// <param name="hand">The controller to set an offset for:
        /// * `0`: left controller
        /// * `1`: right controller
        /// </param>
        /// <param name="offset">The offset (in meters).</param>
        public static void SetControllerOriginOffset(Controller controller, Vector3 offset)
        {
            PXR_Plugin.Controller.UPxr_SetControllerOriginOffset((int)controller, offset);
        }

        /// <summary>
        /// Gets the predicted orientation of a specified controller after a specified time.
        /// </summary>
        /// <param name="hand">The controller to get the predicted rotation for:
        /// * `0`: left controller
        /// * `1`: right controller
        /// </param>
        /// <param name="predictTime">The time for prediction (in milliseconds).</param>
        /// <returns>The predicted orientation.</returns>
        public static Quaternion GetControllerPredictRotation(Controller controller, double predictTime)
        {
            PxrControllerTracking pxrControllerTracking = new PxrControllerTracking();
            float[] headData = new float[7] { 0, 0, 0, 0, 0, 0, 0 };

            PXR_Plugin.Controller.UPxr_GetControllerTrackingState((uint)controller, predictTime, headData, ref pxrControllerTracking);

            return new Quaternion(
                pxrControllerTracking.localControllerPose.pose.orientation.x,
                pxrControllerTracking.localControllerPose.pose.orientation.y,
                pxrControllerTracking.localControllerPose.pose.orientation.z,
                pxrControllerTracking.localControllerPose.pose.orientation.w);
        }

        /// <summary>
        /// Gets the predicted position of a specified controller after a specified time.
        /// </summary>
        /// <param name="hand">The controller to get the predicted position for:
        /// * `0`: left controller
        /// * `1`: right controller
        /// </param>
        /// <param name="predictTime">The time for prediction (in milliseconds).</param>
        /// <returns>The predicted position.</returns>
        public static Vector3 GetControllerPredictPosition(Controller controller, double predictTime)
        {
            PxrControllerTracking pxrControllerTracking = new PxrControllerTracking();
            float[] headData = new float[7] { 0, 0, 0, 0, 0, 0, 0 };

            PXR_Plugin.Controller.UPxr_GetControllerTrackingState((uint)controller, predictTime, headData, ref pxrControllerTracking);

            return new Vector3(
                pxrControllerTracking.localControllerPose.pose.position.x,
                pxrControllerTracking.localControllerPose.pose.position.y,
                pxrControllerTracking.localControllerPose.pose.position.z);
        }

        /// @deprecated Use \ref SendHapticImpulse instead.
        /// <summary>
        /// Sets event-triggered vibration for a specified controller.
        /// </summary>
        /// <param name="hand">The controller to enable vibration for:
        /// * `0`: left controller
        /// * `1`: right controller
        /// </param>
        /// <param name="frequency">Vibration frequency, which ranges from `50` to `500` Hz.</param>
        /// <param name="strength">Vibration amplitude. Its valid value ranges from `0` to `1`. The higher the value, the stronger the vibration amplitude.</param>
        /// <param name="time">Vibration duration, which ranges from `0` to `65535` ms.</param>
        [Obsolete("Please use SendHapticImpulse instead")]
        public static int SetControllerVibrationEvent(UInt32 hand, int frequency, float strength, int time)
        {
            return PXR_Plugin.Controller.UPxr_SetControllerVibrationEvent(hand, frequency, strength, time);
        }

        /// @deprecated Use \ref StopHapticBuffer(int sourceId, bool clearCache) instead.
        /// <summary>
        /// Stops audio-triggered vibration.
        /// </summary>
        /// <param name="id">A reserved parameter, set it to the source ID returned by `StartVibrateBySharem` or `SaveVibrateByCache` to stop the corresponding vibration,
        /// or set it to `0` to stop all vibrations.</param>
        [Obsolete("Please use StopHapticBuffer instead")]
        public static int StopControllerVCMotor(int sourceId)
        {
            return PXR_Plugin.Controller.UPxr_StopControllerVCMotor(sourceId);
        }

        /// @deprecated Deprecated.
        /// <summary>
        /// Starts audio-triggered vibration for specified controller(s). The audio data come from an audio file.
        /// </summary>
        /// <param name="file">The path to the audio file.</param>
        /// <param name="vibrateController">The controller(s) to enable vibration for:
        /// * `0`: none
        /// * `1`: left controller
        /// * `2`: right controller
        /// * `3`: left and right controllers
        /// </param>
        [Obsolete("Deprecated")]
        public static int StartControllerVCMotor(string file, VibrateController vibrateController)
        {
            return PXR_Plugin.Controller.UPxr_StartControllerVCMotor(file, (int)vibrateController);
        }

        /// @deprecated Deprecated.
        /// <summary>
        /// Sets the amplitude for audio-triggered vibration. Support changing the vibration amplitude during audio playback.
        /// </summary>
        /// <param name="mode">Vibration amplitude level:
        /// * `0`: no vibration
        /// * `1`: standard amplitude
        /// * `2`: 2×standard amplitude
        /// * `3`: 3×standard amplitude
        /// * `4`: 4×standard amplitude
        /// @note "3×standard amplitude" and "4×standard amplitude" are NOT recommended as they will cause serious loss of vibration details.
        /// </param>
        [Obsolete("Deprecated")]
        public static int SetControllerAmp(float mode)
        {
            return PXR_Plugin.Controller.UPxr_SetControllerAmp(mode);
        }

        /// @deprecated Use \ref SendHapticBuffer(VibrateType vibrateType, AudioClip audioClip, ChannelFlip channelFlip, ref int sourceId, CacheType cacheType) instead.
        /// <summary>
        /// Starts audio-triggered vibration for specified controller(s). The audio data come from an audio clip passed to the Unity Engine.
        /// </summary>
        /// <param name="audioClip">The path to the audio clip.</param>
        /// <param name="vibrateController">The controller(s) to enable vibration for:
        /// * `0`: none
        /// * `1`: left controller
        /// * `2`: right controller
        /// * `3`: left and right controllers
        /// </param>
        /// <param nname="channelFlip">Whether to enable audio channel inversion:
        /// * `Yes`: enable
        /// * `No`: disable
        /// Once audio channel inversion is enabled, the left controller vibrates with the audio data from the right channel, and vice versa.
        /// </param>
        /// <param nname="sourceId">Returns the unique ID for controlling the corresponding vibration,
        /// which will be used in `StartVibrateByCache`, `ClearVibrateByCache` or `StopControllerVCMotor`.</param>
        [Obsolete("Please use SendHapticBuffer instead")]
        public static int StartVibrateBySharem(AudioClip audioClip, VibrateController vibrateController, ChannelFlip channelFlip, ref int sourceId)
        {
            if (audioClip == null)
            {
                return 0;
            }
            float[] data = new float[audioClip.samples * audioClip.channels];
            int buffersize = audioClip.samples * audioClip.channels;
            audioClip.GetData(data, 0);
            int sampleRate = audioClip.frequency;
            int channelMask = audioClip.channels;
            return PXR_Plugin.Controller.UPxr_StartVibrateBySharem(data, (int)vibrateController, buffersize, sampleRate, channelMask, 32, (int)channelFlip, ref sourceId);
        }

        /**
         * @deprecated Use \ref SendHapticBuffer(VibrateType vibrateType, float[] pcmData, int buffersize, int frequency, int channelMask, ChannelFlip channelFlip, ref int sourceId, CacheType cacheType) instead.
         */
        /// <summary>
        /// Starts audio-triggered vibration for specified controller(s). This function is the overloaded version.
        /// </summary>
        /// <param name="data">The PCM data.</param>
        /// <param name="vibrateController">The controller(s) to enable vibration for:
        /// * `0`: none
        /// * `1`: left controller
        /// * `2`: right controller
        /// * `3`: left and right controllers
        /// </param>
        /// <param name="buffersize">The length of PCM data. Formula: (audioClip.samples)×(audioClip.channels).</param>
        /// <param name="frequency">Audio sampling rate.</param>
        /// <param name="channelMask">The number of channels.</param>
        /// <param name="channelFlip">Whether to enable audio channel inversion:
        /// * `Yes`: enable
        /// * `No`: disable
        ///  Once audio channel inversion is enabled, the left controller vibrates with the audio data from the right channel, and vice versa.
        /// </param>
        /// <param name="sourceId">Returns the unique ID for controlling the corresponding vibration,
        /// which will be used in `StartVibrateByCache`, `ClearVibrateByCache` or `StopControllerVCMotor`.</param>
        [Obsolete("Please use SendHapticBuffer instead")]
        public static int StartVibrateBySharem(float[] data, VibrateController vibrateController, int buffersize, int frequency, int channelMask, ChannelFlip channelFlip, ref int sourceId)
        {
            return PXR_Plugin.Controller.UPxr_StartVibrateBySharem(data, (int)vibrateController, buffersize, frequency, channelMask, 32, (int)channelFlip, ref sourceId);
        }

        /// @deprecated Use \ref SendHapticBuffer(VibrateType vibrateType, AudioClip audioClip, ChannelFlip channelFlip, ref int sourceId, CacheType cacheType) instead.
        /// <summary>
        /// Caches audio-triggered vibration data for specified controller(s).
        /// @note The cached data can be extracted from the cache directory and then transmitted, which reduces resource consumption and improves service performance.
        /// </summary>
        /// <param name="audioClip">The path to the audio clip.</param>
        /// <param name="vibrateController">The controller(s) to cache data for:
        /// * `0`: none
        /// * `1`: left controller
        /// * `2`: right controller
        /// * `3`: left and right controllers</param>
        /// <param name="channelFlip">Whether to enable audio channel inversion:
        /// * `Yes`: enable
        /// * `No`: disable
        /// Once audio channel inversion is enabled, the left controller vibrates with the audio data from the right channel, and vice versa.
        /// </param>
        /// <param name="cacheConfig">Whether to keep the controller vibrating while caching audio-based vibration data:
        /// * `CacheAndVibrate`: cache and keep vibrating
        /// * `CacheNoVibrate`: cache and stop vibrating
        /// </param>
        /// <param name="sourceId">Returns the unique ID for controlling the corresponding vibration,
        /// which will be used in `StartVibrateByCache`, `ClearVibrateByCache` or `StopControllerVCMotor`.</param>
        /// <returns>
        /// * `0`: success
        /// * `-1`: failure
        /// </returns>
        [Obsolete("Please use SendHapticBuffer instead")]
        public static int SaveVibrateByCache(AudioClip audioClip, VibrateController vibrateController, ChannelFlip channelFlip, CacheConfig cacheConfig, ref int sourceId)
        {
            if (audioClip == null)
            {
                return 0;
            }
            float[] data = new float[audioClip.samples * audioClip.channels];
            int buffersize = audioClip.samples * audioClip.channels;
            audioClip.GetData(data, 0);
            int sampleRate = audioClip.frequency;
            int channelMask = audioClip.channels;
            return PXR_Plugin.Controller.UPxr_SaveVibrateByCache(data, (int)vibrateController, buffersize, sampleRate, channelMask, 32, (int)channelFlip, (int)cacheConfig, ref sourceId);
        }

        /// @deprecated Use \ref SendHapticBuffer(VibrateType vibrateType, float[] pcmData, int buffersize, int frequency, int channelMask, ChannelFlip channelFlip, ref int sourceId, CacheType cacheType)
        /// <summary>
        /// Caches audio-triggered vibration data for specified controller(s). This function is the overloaded version.
        /// @note The cached data can be extracted from the cache directory and then transmitted, which reduces resource consumption and improves service performance.
        /// </summary>
        /// <param name="data">The PCM data.</param>
        /// <param name="vibrateController">The controller(s) to cache data for:
        /// * `0`: none
        /// * `1`: left controller
        /// * `2`: right controller
        /// * `3`: left and right controllers
        /// </param>
        /// <param name="buffersize">The length of PCM data. Formula: (audioClip.samples)×(audioClip.channels)</param>
        /// <param name="frequency">Audio sampling rate.</param>
        /// <param name="channelMask">The number of channels.</param>
        /// <param name="channelFlip">Whether to enable audio channel inversion:
        /// * `Yes`: enable
        /// * `No`: disable
        /// Once audio channel inversion is enabled, the left controller vibrates with the audio data from the right channel, and vice versa.
        /// </param>
        /// <param name="cacheConfig">Whether to keep the controller vibrating while caching audio-based vibration data:
        /// * `CacheAndVibrate`: cache and keep vibrating
        /// * `CacheNoVibrate`: cache and stop vibrating
        /// </param>
        /// <param name="sourceId">Returns the unique ID for controlling the corresponding vibration,
        /// which will be used in `StartVibrateByCache`, `ClearVibrateByCache` or `StopControllerVCMotor`.</param>
        /// <returns>
        /// * `0`: success
        /// * `-1`: failure
        /// </returns>
        [Obsolete("Please use SendHapticBuffer instead")]
        public static int SaveVibrateByCache(float[] data, VibrateController vibrateController, int buffersize, int frequency, int channelMask, ChannelFlip channelFlip, CacheConfig cacheConfig, ref int sourceId)
        {
            return PXR_Plugin.Controller.UPxr_SaveVibrateByCache(data, (int)vibrateController, buffersize, frequency, channelMask, 32, (int)channelFlip, (int)cacheConfig, ref sourceId);
        }

        /// @deprecated Use \ref StartHapticBuffer instead.
        /// <summary>
        /// Plays cached audio-triggered vibration data.
        /// </summary>
        /// <param name="sourceId">The source ID returned by `StartVibrateBySharem` or `SaveVibrateByCache`.</param>
        /// <returns>
        /// * `0`: success
        /// * `-1`: failure
        /// </returns>
        [Obsolete("Please use StartHapticBuffer instead")]
        public static int StartVibrateByCache(int sourceId)
        {
            return PXR_Plugin.Controller.UPxr_StartVibrateByCache(sourceId);
        }

        /// @deprecated Use \ref StopHapticBuffer(clearCache) instead.
        /// <summary>
        /// Clears cached audio-triggered vibration data.
        /// </summary>
        /// <param name="sourceId">The source ID returned by `StartVibrateBySharem` or `SaveVibrateByCache`.</param>
        /// <returns>
        /// * `0`: success
        /// * `-1`: failure
        /// </returns>
        [Obsolete("Please use StopHapticBuffer(clearCache) instead")]
        public static int ClearVibrateByCache(int sourceId)
        {
            return PXR_Plugin.Controller.UPxr_ClearVibrateByCache(sourceId);
        }

        public static int SetControllerEnableKey(bool isEnable, PxrControllerKeyMap Key)
        {
            return PXR_Plugin.Controller.UPxr_SetControllerEnableKey(isEnable, Key);
        }

        /// @deprecated Use \ref SendHapticBuffer(VibrateType vibrateType, TextAsset phfText, ChannelFlip channelFlip, float amplitudeScale, ref int sourceId) instead.
        /// <summary>
        /// Starts PHF-triggered vibration for specified controller(s). PHF stands for PICO haptic file.
        /// </summary>
        /// <param name="phfText">The path to the PHF file.</param>
        /// <param name="sourceId">The source ID returned by `StartVibrateBySharem` or `SaveVibrateByCache`.</param>
        /// <param name="vibrateController">The controller(s) to enable vibration for:
        /// * `0`: none
        /// * `1`: left controller
        /// * `2`: right controller
        /// * `3`: left and right controllers
        /// </param>
        /// <param name="channelFlip">Whether to enable audio channel inversion:
        /// * `Yes`: enable
        /// * `No`: disable
        /// Once audio channel inversion is enabled, the left controller vibrates with the audio data from the right channel, and vice versa.</param>
        /// <param name="amp">The vibration gain, the valid value range from `0` to `2`:
        /// * `0`: no vibration
        /// * `1`: standard amplitude
        /// * `2`: 2×standard amplitude</param>
        /// <returns>
        /// * `0`: success
        /// * `-1`: failure
        /// </returns>
        [Obsolete("Please use SendHapticBuffer instead")]
        public static int StartVibrateByPHF(TextAsset phfText, ref int sourceId, VibrateController vibrateController, ChannelFlip channelFlip, float amp)
        {
            return PXR_Plugin.Controller.UPxr_StartVibrateByPHF(phfText.text, phfText.text.Length, ref sourceId, (int)vibrateController, (int)channelFlip, amp);
        }

        /// @deprecated Use \ref PauseHapticBuffer instead.
        /// <summary>
        /// Pauses PHF-triggered vibration.
        /// </summary>
        /// <param name="sourceId">The source ID returned by `StartVibrateBySharem` or `SaveVibrateByCache`.</param>
        /// <returns>
        /// * `0`: success
        /// * `-1`: failure
        /// </returns>
        [Obsolete("Please use PauseHapticBuffer instead")]
        public static int PauseVibrate(int sourceId)
        {
            return PXR_Plugin.Controller.UPxr_PauseVibrate(sourceId);
        }

        /// @deprecated Use \ref ResumeHapticBuffer instead.
        /// <summary>
        /// Resumes PHF-triggered vibration.
        /// </summary>
        /// <param name="sourceId">The source ID returned by `StartVibrateBySharem` or `SaveVibrateByCache`.</param>
        /// <returns>
        /// * `0`: success
        /// * `-1`: failure
        /// </returns>
        [Obsolete("Please use ResumeHapticBuffer instead")]
        public static int ResumeVibrate(int sourceId)
        {
            return PXR_Plugin.Controller.UPxr_ResumeVibrate(sourceId);
        }

        /// @deprecated Use \ref UpdateHapticBuffer instead.
        /// <summary>
        /// Dynamically updates PHF and AudioClip vibration data.
        /// </summary>
        /// <param name="sourceId">The source ID returned by `StartVibrateBySharem` or `SaveVibrateByCache`.</param>
        /// <param name="vibrateController">The controller(s) to update PHF and AudioClip vibration data for:
        /// * `0`: none
        /// * `1`: left controller
        /// * `2`: right controller
        /// * `3`: left and right controllers
        /// </param>
        /// <param name="channelFlip">Whether to enable audio channel inversion:
        /// * `Yes`: enable
        /// * `No`: disable
        /// Once audio channel inversion is enabled, the left controller vibrates with the audio data from the right channel, and vice versa.</param>
        /// <param name="amp">The vibration gain, the valid value range from `0` to `2`:
        /// * `0`: no vibration
        /// * `1`: standard amplitude
        /// * `2`: 2×standard amplitude</param>
        /// <returns>
        /// * `0`: success
        /// * `-1`: failure
        /// </returns>
        [Obsolete("Please use UpdateHapticBuffer instead")]
        public static int UpdateVibrateParams(int sourceId, VibrateController vibrateController, ChannelFlip channelFlip, float amp)
        {
            return PXR_Plugin.Controller.UPxr_UpdateVibrateParams(sourceId, (int)vibrateController, (int)channelFlip, amp);
        }

        /// <summary>
        /// Gets the data about the poses of body joints.
        /// </summary>
        /// <param name="predictTime">Reserved parameter, pass `0`.</param>
        /// <param name="bodyTrackerResult">Contains the data about the poses of body joints, including position, action, and more.</param>
        public static int GetBodyTrackingPose(double predictTime, ref BodyTrackerResult bodyTrackerResult)
        {
            return PXR_Plugin.Controller.UPxr_GetBodyTrackingPose(predictTime, ref bodyTrackerResult);
        }

        /// <summary>
        /// Gets the number of PICO Motion Trackers currently connected and their IDs.
        /// </summary>
        /// <param name="state">The number and IDs of connected PICO Motion Trackers.</param>
        public static int GetMotionTrackerConnectStateWithID(ref PxrMotionTracker1ConnectState state)
        {
            return PXR_Plugin.Controller.UPxr_GetMotionTrackerConnectStateWithID(ref state);
        }

        /// <summary>
        /// Gets the battery of a specified PICO Motion Traker.
        /// </summary>
        /// <param name="trackerId">The ID of the motion tracker to get battery for.</param>
        /// <param name="battery">The motion tracker's battery. Value range: [0,5]. The smaller the value, the lower the battery level.</param>
        public static int GetMotionTrackerBattery(int trackerId, ref int battery)
        {
            return PXR_Plugin.Controller.UPxr_GetMotionTrackerBattery(trackerId, ref battery);
        }

        /// <summary>
        /// Gets whether the PICO Motion Tracker has completed calibration.
        /// </summary>
        /// <param name="calibrated">Indicates the calibration status:
        /// `0`: calibration uncompleted
        /// `1`: calibration completed
        /// </param>
        public static int GetMotionTrackerCalibState(ref int calibrated)
        {
            return PXR_Plugin.Controller.UPxr_GetMotionTrackerCalibState(ref calibrated);
        }

        /// <summary>
        /// Sets a body tracking mode for PICO Motion Tracker. If this API is not called, the mode defaults to leg tracking.
        /// @note If you want to set the mode to full-body tracking, you must call this API before calling `OpenMotionTrackerCalibrationAPP`.
        /// </summary>
        /// <param name="mode">Selects a body tracking mode from the following:
        /// * Motion Tracker 1.0  `0`: leg tracking, nodes numbered 0 to 15 in `BodyTrackerRole` enum will return data.
        /// * Motion Tracker 1.0  `1`: full-body tracking, nodes numbered 0 to 23 in `BodyTrackerRole` enum will return data.
        /// * Motion Tracker 2.0  `0`: full-body tracking, nodes numbered 0 to 23 in `BodyTrackerRole` enum will return data. Low latency.
        /// * Motion Tracker 2.0  `1`: full-body tracking, nodes numbered 0 to 23 in `BodyTrackerRole` enum will return data. High latency.
        /// </param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int SetBodyTrackingMode(BodyTrackingMode mode)
        {
            return PXR_Plugin.Controller.UPxr_SetBodyTrackingMode(mode);
        }

        /// <summary>
        /// Sets bone lengths for different parts of the avatar. The data will be sent to PICO'S algorithm to make the avatar's poses more accurate. 
        /// </summary>
        /// <param name="boneLength">Sets the bone lengths for different parts of the avatar. See the `BodyTrackingBoneLength` for details.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int SetBodyTrackingBoneLength(BodyTrackingBoneLength boneLength)
        {
            return PXR_Plugin.Controller.UPxr_SetBodyTrackingBoneLength(boneLength);
        }

        /// <summary>
        /// Sends a haptic impulse to specified controller(s) to trigger vibration.
        /// @note To stop vibration, call this API again and set both `amplitude` and `duration` to `0`.
        /// </summary>
        /// <param name="vibrateType">The controller(s) to send the haptic impulse to:
        /// * `None`
        /// * `LeftController`
        /// * `RightController`
        /// * `BothController`
        /// </param>
        /// <param name="amplitude">Vibration amplitude, which ranges from `0` to `1`. The higher the value, the stronger the vibration amplitude.</param>
        /// <param name="duration">Vibration duration, which ranges from `0` to `65535` ms.</param>
        /// <param name="frequency">Vibration frequency, which ranges from `50` to `500` Hz.</param>
        public static void SendHapticImpulse(VibrateType vibrateType, float amplitude, int duration, int frequency = 150)
        {
            switch (vibrateType)
            {
                case VibrateType.None:
                    break;
                case VibrateType.LeftController:
                    PXR_Plugin.Controller.UPxr_SetControllerVibrationEvent(0, frequency, amplitude, duration);
                    break;
                case VibrateType.RightController:
                    PXR_Plugin.Controller.UPxr_SetControllerVibrationEvent(1, frequency, amplitude, duration);
                    break;
                case VibrateType.BothController:
                    PXR_Plugin.Controller.UPxr_SetControllerVibrationEvent(0, frequency, amplitude, duration);
                    PXR_Plugin.Controller.UPxr_SetControllerVibrationEvent(1, frequency, amplitude, duration);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sends a buffer of haptic data to specified controller(s) to trigger vibration.
        /// </summary>
        /// <param name="vibrateType">The controller(s) to send the haptic data to:
        /// * `None`
        /// * `LeftController`
        /// * `RightController`
        /// * `BothController`
        /// </param>
        /// <param name="audioClip">The audio data pulled from the audio file stored in the AudioClip component is used as the haptic data.</param>
        /// <param name="channelFlip">Determines whether to enable audio channel inversion. Once enabled, the left controller vibrates with the audio data from the right channel, and vice versa.
        /// * `Yes`: enable
        /// * `No`: disable
        /// </param>
        /// <param name="sourceId">Returns the unique ID for controlling the corresponding buffered haptic,
        /// which will be used in `PauseHapticBuffer`, `ResumeHapticBuffer`, `UpdateHapticBuffer`, or `StopHapticBuffer`.</param>
        /// <param name="cacheType">Whether to keep the controller vibrating while caching haptic data:
        /// * `DontCache`: don't cache.
        /// * `CacheAndVibrate`: cache and keep vibrating.
        /// * `CacheNoVibrate`: cache and stop vibrating. Call `StartHapticBuffer` to start haptic after caching the data.
        /// @note If not defined, `DontCache` will be passed by default.
        /// </param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        /**
         * \overload int SendHapticBuffer(VibrateType vibrateType, AudioClip audioClip, ChannelFlip channelFlip, ref int sourceId, CacheType cacheType)
         */
        public static int SendHapticBuffer(VibrateType vibrateType, AudioClip audioClip, ChannelFlip channelFlip, ref int sourceId, CacheType cacheType = CacheType.DontCache)
        {
            if (audioClip == null)
            {
                return 0;
            }
            float[] data = new float[audioClip.samples * audioClip.channels];
            int buffersize = audioClip.samples * audioClip.channels;
            audioClip.GetData(data, 0);
            int sampleRate = audioClip.frequency;
            int channelMask = audioClip.channels;
            if (cacheType == CacheType.DontCache)
            {
                return PXR_Plugin.Controller.UPxr_StartVibrateBySharem(data, (int)vibrateType, buffersize, sampleRate, channelMask, 32, (int)channelFlip, ref sourceId);
            }
            else
            {
                return PXR_Plugin.Controller.UPxr_SaveVibrateByCache(data, (int)vibrateType, buffersize, sampleRate, channelMask, 32, (int)channelFlip, (int)cacheType, ref sourceId);
            }
        }

        /// <summary>
        /// Sends a buffer of haptic data to specified controller(s) to trigger vibration.
        /// </summary>
        /// <param name="vibrateType">The controller(s) to send the haptic data to:
        /// * `None`
        /// * `LeftController`
        /// * `RightController`
        /// * `BothController`
        /// </param>
        /// <param name="pcmData">The PCM data is converted from the audio file stored in the AudioClip component in the Unity Engine.</param>
        /// <param name="buffersize">The length of PCM data. Calculation formula: (audioClip.samples)×(audioClip.channels). Sample refers to the data in each channel.</param>
        /// <param name="frequency">Sample rate. The higher the sample rate, the closer the recorded signal is to the original.</param>
        /// <param name="channelMask">The number of channels that play the haptic data.</param>
        /// <param name="channelFlip">Determines whether to enable audio channel inversion. Once enabled, the left controller vibrates with the audio data from the right channel, and vice versa.
        /// * `Yes`: enable
        /// * `No`: disable
        /// </param>
        /// <param name="sourceId">Returns the unique ID for controlling the corresponding buffered haptic,
        /// which will be used in `PauseHapticBuffer`, `ResumeHapticBuffer`, `UpdateHapticBuffer`, or `StopHapticBuffer`.</param>
        /// <param name="cacheType">Whether to keep the controller vibrating while caching haptic data:
        /// * `DontCache`: don't cache.
        /// * `CacheAndVibrate`: cache and keep vibrating.
        /// * `CacheNoVibrate`: cache and stop vibrating. Call `StartHapticBuffer` to start vibration after caching the data.
        /// @note If not defined, `DontCache` will be passed by default.
        /// </param>
        /// <returns>
        /// * `0`: success
        /// * `-1`: failure
        /// </returns>
        /**
         * \overload int SendHapticBuffer(VibrateType vibrateType, float[] pcmData, int buffersize, int frequency, int channelMask, ChannelFlip channelFlip, ref int sourceId, CacheType cacheType)
         */
        public static int SendHapticBuffer(VibrateType vibrateType, float[] pcmData, int buffersize, int frequency, int channelMask, ChannelFlip channelFlip, ref int sourceId, CacheType cacheType = CacheType.DontCache)
        {
            if (cacheType == CacheType.DontCache)
            {
                return PXR_Plugin.Controller.UPxr_StartVibrateBySharem(pcmData, (int)vibrateType, buffersize, frequency, channelMask, 32, (int)channelFlip, ref sourceId);
            }
            else
            {
                return PXR_Plugin.Controller.UPxr_SaveVibrateByCache(pcmData, (int)vibrateType, buffersize, frequency, channelMask, 32, (int)channelFlip, (int)cacheType, ref sourceId);
            }
        }

        /// <summary>
        /// Sends a buffer of haptic data to specified controller(s) to trigger vibration.
        /// </summary>
        /// <param name="vibrateType">The controller(s) to send the haptic data to:
        /// * `None`
        /// * `LeftController`
        /// * `RightController`
        /// * `BothController`
        /// </param>
        /// <param name="phfText">The PHF file (.json) that contains haptic data.</param>
        /// <param name="channelFlip">Determines whether to enable audio channel inversion. Once enabled, the left controller vibrates with the audio data from the right channel, and vice versa.
        /// * `Yes`: enable
        /// * `No`: disable
        /// <param name="amplitudeScale">Vibration amplitude, the higher the amplitude, the stronger the haptic effect. The valid value range from `0` to `2`:
        /// * `0`: no vibration
        /// * `1`: standard amplitude
        /// * `2`: 2×standard amplitude
        /// </param>
        /// <param name="sourceId">Returns the unique ID for controlling the corresponding buffered haptic,
        /// which will be used in `PauseHapticBuffer`, `ResumeHapticBuffer`, `UpdateHapticBuffer`, or `StopHapticBuffer`.</param>
        /// <returns>
        /// * `0`: success
        /// * `-1`: failure
        /// </returns>
        public static int SendHapticBuffer(VibrateType vibrateType, TextAsset phfText, ChannelFlip channelFlip, float amplitudeScale, ref int sourceId)
        {
            return PXR_Plugin.Controller.UPxr_StartVibrateByPHF(phfText.text, phfText.text.Length, ref sourceId, (int)vibrateType, (int)channelFlip, amplitudeScale);
        }

        /// <summary>
        /// Stops a specified buffered haptic.
        /// </summary>
        /// <param name="sourceId">The source ID returned by `SendHapticBuffer`. Set it to the target source ID to stop a specific buffered haptic,
        /// or set it to `0` to stop all buffered haptics. If not defined, `0` will be passed to stop all buffered haptics by default.</param>
        /// <param name="clearCache">Determines whether to clear the cached data of the specified haptic.
        /// If not defined, `false` will be passed to keep the cached data by default.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int StopHapticBuffer(int sourceId = 0, bool clearCache = false)
        {
            if (clearCache)
            {
                PXR_Plugin.Controller.UPxr_ClearVibrateByCache(sourceId);
            }
            return PXR_Plugin.Controller.UPxr_StopControllerVCMotor(sourceId);
        }

        /// <summary>
        /// Pauses a specified buffered haptic.
        /// </summary>
        /// <param name="sourceId">The source ID returned by `SendHapticBuffer`.
        /// Set it to the target source ID to stop a specific buffered haptic.</param>
        /// <returns>
        /// * `0`: success
        /// * `-1`: failure
        /// </returns>
        public static int PauseHapticBuffer(int sourceId)
        {
            return PXR_Plugin.Controller.UPxr_PauseVibrate(sourceId);
        }

        /// <summary>
        /// Resumes a paused buffered haptic.
        /// </summary>
        /// <param name="sourceId">The source ID returned by `SendHapticBuffer`.
        /// Set it to the target source ID to resume a specific buffered haptic.</param>
        /// <returns>
        /// * `0`: success
        /// * `-1`: failure
        /// </returns>
        public static int ResumeHapticBuffer(int sourceId)
        {
            return PXR_Plugin.Controller.UPxr_ResumeVibrate(sourceId);
        }

        /// <summary>
        /// Starts a specified buffered haptic.
        /// @note If you pass `CacheNoVibrate` in `SendHapticBuffer`, call this API if you want to start haptic after caching the data.
        /// </summary>
        /// <param name="sourceId">The source ID returned by `SendHapticBuffer` when there is cached data for the haptic.</param>
        /// <returns>
        /// * `0`: success
        /// * `-1`: failure
        /// </returns>
        public static int StartHapticBuffer(int sourceId)
        {
            return PXR_Plugin.Controller.UPxr_StartVibrateByCache(sourceId);
        }

        /// <summary>
        /// Updates the settings for a specified buffered haptic.
        /// </summary>
        /// <param name="sourceId">The source ID returned by `SendHapticBuffer`.
        /// Set it to the target source ID to update a specific buffered haptic.</param>
        /// <param name="vibrateType">The controller(s) that the vibration is applied to:
        /// * `None`
        /// * `LeftController`
        /// * `RightController`
        /// * `BothController`
        /// </param>
        /// <param name="channelFlip">Determines whether to enable audio channel inversion. Once enabled, the left controller vibrates with the audio data from the right channel, and vice versa.
        /// * `Yes`: enable
        /// * `No`: disable
        /// <param name="amplitudeScale">Vibration amplitude, the higher the amplitude, the stronger the haptic effect. The valid value range from `0` to `2`:
        /// * `0`: no vibration
        /// * `1`: standard amplitude
        /// * `2`: 2×standard amplitude
        /// </param>
        /// <returns>
        /// * `0`: success
        /// * `-1`: failure
        /// </returns>
        public static int UpdateHapticBuffer(int sourceId, VibrateType vibrateType, ChannelFlip channelFlip, float amplitudeScale)
        {
            return PXR_Plugin.Controller.UPxr_UpdateVibrateParams(sourceId, (int)vibrateType, (int)channelFlip, amplitudeScale);
        }

        /// <summary>Creates a haptic stream.</summary>
        /// <param name="phfVersion">The version of the PICO haptic file (PHF) that the stream uses.</param>
        /// <param name="frameDurationMs">Interframe space, which is the amount of time in milliseconds existing between the transmissions of frames.</param>
        /// <param name="hapticInfo">The information about this haptic stream you create.</param>
        /// <param name="speed">The streaming speed.</param>
        /// <param name="id">Returns the ID of the stream.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int CreateHapticStream(string phfVersion, UInt32 frameDurationMs, ref VibrateInfo hapticInfo, float speed, ref int id)
        {
            return PXR_Plugin.Controller.UPxr_CreateHapticStream(phfVersion, frameDurationMs, ref hapticInfo, speed, ref id);
        }

        /// <summary>
        /// Writes haptic data to a specified stream.
        /// </summary>
        /// <param name="id">The ID of the target stream.</param>
        /// <param name="frames">The data contained in the PICO haptic file (PHF).</param>
        /// <param name="numFrames">The number of frames.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int WriteHapticStream(int id, ref PxrPhfParamsNum frames, UInt32 numFrames)
        {
            return PXR_Plugin.Controller.UPxr_WriteHapticStream(id, ref frames, numFrames);
        }

        /// <summary>
        /// Sets a transmission speed for a specified haptic stream.
        /// </summary>
        /// <param name="id">The ID of the stream.</param>
        /// <param name="speed">The transmission speed to set for the stream.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int SetHapticStreamSpeed(int id, float speed)
        {
            return PXR_Plugin.Controller.UPxr_SetPHFHapticSpeed(id, speed);
        }

        /// <summary>
        /// Gets the transmission speed of a specified haptic stream.
        /// </summary>
        /// <param name="id">The ID of the stream.</param>
        /// <param name="speed">Returns the stream's transmission speed.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int GetHapticStreamSpeed(int id, ref float speed)
        {
            return PXR_Plugin.Controller.UPxr_GetPHFHapticSpeed(id, ref speed);
        }

        /// <summary>
        /// Gets the No. of the frame that the controller currently plays.
        /// </summary>
        /// <param name="id">The ID of the haptic stream that triggers the vibration.</param>
        /// <param name="frameSequence">Returns the current frame's sequence No.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int GetHapticStreamCurrentFrameSequence(int id, ref UInt64 frameSequence)
        {
            return PXR_Plugin.Controller.UPxr_GetCurrentFrameSequence(id, ref frameSequence);
        }

        /// <summary>
        /// Starts the transmission of a specified haptic stream.
        /// </summary>
        /// <param name="source_id">The ID of the haptic stream.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int StartHapticStream(int source_id)
        {
            return PXR_Plugin.Controller.UPxr_StartPHFHaptic(source_id);
        }

        /// <summary>
        /// Stops the transmission of a specified haptic stream.
        /// </summary>
        /// <param name="source_id">The ID of the haptic stream.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int StopHapticStream(int source_id)
        {
            return PXR_Plugin.Controller.UPxr_StopPHFHaptic(source_id);
        }

        /// <summary>
        /// Removes a specified haptic stream.
        /// </summary>
        /// <param name="source_id">The ID of the stream.</param>
        /// <returns>
        /// * `0`: successGetMotionTrackerCalibState
        /// * `1`: failure
        /// </returns>
        public static int RemoveHapticStream(int source_id)
        {
            return PXR_Plugin.Controller.UPxr_RemovePHFHaptic(source_id);
        }

        /// <summary>
        /// Parses the haptic data in a specified PICO haptic file (PHF).
        /// </summary>
        /// <param name="phfText">The PICO haptic file (.json) to parse.</param>
        public static PxrPhfFile AnalysisHapticStreamPHF(TextAsset phfText)
        {
            String str = phfText.text;
            return JsonMapper.ToObject<PxrPhfFile>(str);
        }

        /// <summary>
        /// Recenters the controller on PICO G3.
        /// </summary>
        public static void ResetController()
        {
            PXR_Plugin.Controller.UPxr_ResetController();
        }

        /// <summary>
        /// Sets arm model parameters on PICO G3.
        /// </summary>
        /// <param name="gazetype">Gaze type, which is used to define the way of getting the HMD data.</param>
        /// <param name="armmodeltype">Arm model type</param>
        /// <param name="elbowHeight">The elbow's height, which changes the arm's length.Value range: (0.0f, 0.2f). The default value is 0.0f.</param>
        /// <param name="elbowDepth">The elbow's depth, which changes the arm's position.Value range: (0.0f, 0.2f). The default value is 0.0f.</param>
        /// <param name="pointerTiltAngle">The ray's tilt angle. Value range: (0.0f, 30.0f). The default value is 0.0f.</param>
        public static void SetArmModelParameters(PxrGazeType gazetype, PxrArmModelType armmodeltype, float elbowHeight, float elbowDepth, float pointerTiltAngle)
        {
            PXR_Plugin.Controller.UPxr_SetArmModelParameters(gazetype, armmodeltype, elbowHeight, elbowDepth, pointerTiltAngle);
        }

        /// <summary>
        /// Gets the current user's dominant hand in the system on PICO G3.
        /// </summary>
        /// <param name="deviceID"></param>
        public static void GetControllerHandness(ref int deviceID)
        {
            PXR_Plugin.Controller.UPxr_GetControllerHandness(ref deviceID);
        }

    }
}