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

using Pico.Platform.Models;
using UnityEngine;

namespace Pico.Platform
{
    public class HighlightService
    {
        /// <summary>
        /// Starts a new session. Before using screen recording and capturing-related functions, make sure you are in a session.
        /// </summary>
        /// <returns>The session ID, which is a string.</returns>
        public static Task<string> StartSession()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<string>(CLIB.ppf_Highlight_StartSession());
        }

        /// <summary>
        /// Captures the screen.
        /// </summary>
        /// <returns>The information about this capture, including image path and job ID.</returns>
        public static Task<CaptureInfo> CaptureScreen()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<CaptureInfo>(CLIB.ppf_Highlight_CaptureScreen());
        }

        /// <summary>
        /// Starts recording the screen.
        /// </summary>
        public static Task StartRecord()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task(CLIB.ppf_Highlight_StartRecord());
        }

        /// <summary>
        /// Stops recording the screen.
        /// </summary>
        /// <returns>The infomraiton about this recording, including video path, video duration, video size, and job ID.</returns>
        public static Task<RecordInfo> StopRecord()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<RecordInfo>(CLIB.ppf_Highlight_StopRecord());
        }
        
        /// <summary>
        /// Lists all the media resources for a session.
        /// </summary>
        /// <param name="sessionId">Passes the ID of the session which is returned by `StartSession`.</param>
        /// <returns>The information about the images captured and videos recorded during this session.</returns>
        public static Task<SessionMedia> ListMedia(string sessionId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<SessionMedia>(CLIB.ppf_Highlight_ListMedia(sessionId));
        }

        /// <summary>
        /// Saves an image or a video to the device's local storage.
        /// </summary>
        /// <param name="jobId">Passes the ID of the screen-capturing or screen-recording task where the image or video is created.</param>
        /// <param name="sessionId">Passes the ID of the session where the task takes place.</param>
        /// <returns>The job ID and session ID of the image or video saved.</returns>
        public static Task SaveMedia(string jobId, string sessionId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task(CLIB.ppf_Highlight_SaveMedia(jobId, sessionId));
        }

        /// <summary>
        /// Shares an image or a video to the social media on the mobile phone.
        /// </summary>
        /// <param name="jobId">Passes the ID of the screen-capturing or screen-recording task where the image or video is created.</param>
        /// <param name="sessionId">Passes the ID of the session where the task takes place.</param>
        /// <returns>The job ID and session ID of the image or video shared.</returns>
        public static Task ShareMedia(string jobId, string sessionId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task(CLIB.ppf_Highlight_ShareMedia(jobId, sessionId));
        }

        /// <summary>
        /// The maiximum duration for a video is 15 minutes.
        /// After the `StartRecord` function is called, if the `StopRecord` function is not called in time or if the recording is ended due to other causes, the system will automatically stop recording and return the recording information.
        /// </summary>
        /// <param name="handler">Returns the recording information.</param>
        public static void SetOnRecordStopHandler(Message<RecordInfo>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Highlight_OnRecordStop, handler);
        }
    }
}