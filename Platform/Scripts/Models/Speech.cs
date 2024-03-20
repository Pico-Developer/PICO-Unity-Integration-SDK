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

namespace Pico.Platform.Models
{
    /// <summary>
    /// The automatic speech recognition result.
    /// </summary>
    public class AsrResult
    {
        /// <summary>
        /// The text recognized.
        /// </summary>
        public readonly string Text;
        /// <summary>
        /// Whether this is the final result:
        /// * `true`: yes
        /// * `false`: no
        /// </summary>
        public readonly bool IsFinalResult;

        public AsrResult(IntPtr o)
        {
            Text = CLIB.ppf_AsrResult_GetText(o);
            IsFinalResult = CLIB.ppf_AsrResult_GetIsFinalResult(o);
        }
    }

    /// <summary>
    /// Information about the automatic speech recognition error.
    /// </summary>
    public class SpeechError
    {
        /// <summary>
        /// Error message.
        /// </summary>
        public readonly string Message;
        /// <summary>
        /// The ID of the session where the error occurred.
        /// </summary>
        public readonly string SessionId;
        /// <summary>
        /// Error code.
        /// </summary>
        public readonly int Code;

        public SpeechError(IntPtr o)
        {
            Message = CLIB.ppf_SpeechError_GetMessage(o);
            Code = CLIB.ppf_SpeechError_GetCode(o);
            SessionId = CLIB.ppf_SpeechError_GetSessionId(o);
        }
    }
}