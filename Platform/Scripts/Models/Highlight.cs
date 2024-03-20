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
    /// Information about screen capturing.
    /// </summary>
    public class CaptureInfo
    {
        /// <summary>
        /// The path where the image is located.
        /// </summary>
        public readonly string ImagePath;
        /// <summary>
        /// The ID of the screen-capturing task.
        /// </summary>
        public readonly string JobId;

        public CaptureInfo(IntPtr o)
        {
            ImagePath = CLIB.ppf_CaptureInfo_GetImagePath(o);
            JobId = CLIB.ppf_CaptureInfo_GetJobId(o);
        }
    }


    /// <summary>
    /// Information about screen recording.
    /// </summary>
    public class RecordInfo
    {
        /// <summary>
        /// The path where the video is located.
        /// </summary>
        public readonly string VideoPath;
        /// <summary>
        /// The duration of the video. Unit: milliseconds.
        /// </summary>
        public readonly int DurationInMilliSeconds;
        /// <summary>
        /// The width of the video.
        /// </summary>
        public readonly int Width;
        /// <summary>
        /// The height of the video.
        /// </summary>
        public readonly int Height;
        /// <summary>
        /// The ID of the screen-recording task.
        /// </summary>
        public readonly string JobId;

        public RecordInfo(IntPtr o)
        {
            VideoPath = CLIB.ppf_RecordInfo_GetVideoPath(o);
            DurationInMilliSeconds = CLIB.ppf_RecordInfo_GetDurationInMilliSeconds(o);
            Width = CLIB.ppf_RecordInfo_GetWidth(o);
            Height = CLIB.ppf_RecordInfo_GetHeight(o);
            JobId = CLIB.ppf_RecordInfo_GetJobId(o);
        }
    }


    /// <summary>
    /// Information about the images captured and videos recorded in a session.
    /// </summary>
    public class SessionMedia
    {
        /// <summary>
        /// Image information, including image paths and job IDs.
        /// </summary>
        public readonly CaptureInfo[] Images;
        /// <summary>
        /// Video information, including video paths, video durations, video sizes, and job IDs.
        /// </summary>
        public readonly RecordInfo[] Videos;

        public SessionMedia(IntPtr o)
        {
            {
                int sz = (int) CLIB.ppf_SessionMedia_GetImagesSize(o);
                Images = new CaptureInfo[sz];
                for (int i = 0; i < sz; i++)
                {
                    Images[i] = new CaptureInfo(CLIB.ppf_SessionMedia_GetImages(o, (UIntPtr) i));
                }
            }
            {
                int sz = (int) CLIB.ppf_SessionMedia_GetVideosSize(o);
                Videos = new RecordInfo[sz];
                for (int i = 0; i < sz; i++)
                {
                    Videos[i] = new RecordInfo(CLIB.ppf_SessionMedia_GetVideos(o, (UIntPtr) i));
                }
            }
        }
    }
}