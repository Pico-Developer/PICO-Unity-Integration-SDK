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
using Pico.Platform.Models;

namespace Pico.Platform
{
    public class SpeechService
    {
        /// <summary>
        /// Initializes the automatic speech recognition engine.
        /// </summary>
        /// <returns>The initialization result.</returns>
        public static AsrEngineInitResult InitAsrEngine()
        {
            return CLIB.ppf_Speech_InitAsrEngine();
        }

        /// <summary>
        /// Starts automatic speech recognition (ASR).
        /// </summary>
        /// <param name="autoStop">Specifies whether to automatically stop ASR when the speaker stops speaking (i.e., when no voice is detected):
        /// * `true`: auto stop
        /// * `false`: do not auto stop
        /// </param>
        /// <param name="showPunctual">Specifies whether to show punctuations in the text:
        /// * `true`: show
        /// * `false`: do not show
        /// </param>
        /// <param name="vadMaxDurationInSeconds">Specifies the maximum duration allowed for speaking per time. Unit: milliseconds.</param>
        /// <returns>
        /// `0` indicates success and other values indicates failure.
        /// </returns>
        public static int StartAsr(bool autoStop, bool showPunctual, int vadMaxDurationInSeconds)
        {
            var option = new StartAsrOptions();
            option.SetAutoStop(autoStop);
            option.SetShowPunctual(showPunctual);
            option.SetVadMaxDurationInSeconds(vadMaxDurationInSeconds);
            return CLIB.ppf_Speech_StartAsr((IntPtr) option);
        }

        /// <summary>
        /// Stops automatic speech recognition.
        /// </summary>
        public static void StopAsr()
        {
            CLIB.ppf_Speech_StopAsr();
        }

        /// <summary>
        /// When automatic speech recognition is enabled, it constantly converts the transmitted speech into text and returns it through the callback.
        /// @note After reconnection following a network disconnection during the recognition process, only the text recognized from the speech after the reconnection will be returned,
        /// and the text recognized from the speech before the disconnection will not be returned.
        /// </summary>
        /// <param name="handler">Returns the recognition result.</param>
        public static void SetOnAsrResultCallback(Message<AsrResult>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Speech_OnAsrResult, handler);
        }

        /// <summary>
        /// If an error occurs during the speech recognition process, it will be returned through this callback.
        /// </summary>
        /// <param name="handler">Returns the information about the error.</param>
        public static void SetOnSpeechErrorCallback(Message<SpeechError>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Speech_OnSpeechError, handler);
        }
    }

    public class StartAsrOptions
    {
        public StartAsrOptions()
        {
            Handle = CLIB.ppf_StartAsrOptions_Create();
        }

        
        public void SetAutoStop(bool value)
        {
            CLIB.ppf_StartAsrOptions_SetAutoStop(Handle, value);
        }

        
        public void SetVadMaxDurationInSeconds(int value)
        {
            CLIB.ppf_StartAsrOptions_SetVadMaxDurationInSeconds(Handle, value);
        }

        
        public void SetShowPunctual(bool value)
        {
            CLIB.ppf_StartAsrOptions_SetShowPunctual(Handle, value);
        }

        /// For passing to native C
        public static explicit operator IntPtr(StartAsrOptions options)
        {
            return options != null ? options.Handle : IntPtr.Zero;
        }

        ~StartAsrOptions()
        {
            CLIB.ppf_StartAsrOptions_Destroy(Handle);
        }

        public IntPtr Handle;
    }
}