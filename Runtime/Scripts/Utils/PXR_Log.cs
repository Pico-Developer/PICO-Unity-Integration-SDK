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

#if UNITY_ANDROID && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using UnityEngine;

namespace Unity.XR.PXR
{
    public class PLog
    {
        //   7--all print, 4--info to fatal, 3--warning to fatal,
        //   2--error to fatal, 1--only fatal print
        public static LogLevel logLevel = LogLevel.LogWarn;

        public enum LogLevel
        {
            LogFatal = 1,
            LogError = 2,
            LogWarn = 3,
            LogInfo = 4,
            LogDebug = 5,
            LogVerbose,
        }

        public static void v(string tag, string message, bool showFrameCount = true)
        {
            if (LogLevel.LogVerbose <= logLevel)
            {
                Debug.Log(showFrameCount
                    ? string.Format("{0} FrameID={1}>>>>>>{2}", tag, Time.frameCount, message)
                    : string.Format("{0}>>>>>>{1}", tag, message));
            }
        }

        public static void d(string tag, string message, bool showFrameCount = true)
        {
            if (LogLevel.LogDebug <= logLevel)
            {
                Debug.Log(showFrameCount
                    ? string.Format("{0} FrameID={1}>>>>>>{2}", tag, Time.frameCount, message)
                    : string.Format("{0}>>>>>>{1}", tag, message));
            }
        }

        public static void i(string tag, string message, bool showFrameCount = true)
        {
            if (LogLevel.LogInfo <= logLevel)
            {
                Debug.Log(showFrameCount
                    ? string.Format("{0} FrameID={1}>>>>>>{2}", tag, Time.frameCount, message)
                    : string.Format("{0}>>>>>>{1}", tag, message));
            }
        }

        public static void w(string tag, string message, bool showFrameCount = true)
        {
            if (LogLevel.LogWarn <= logLevel)
            {
                Debug.Log(showFrameCount
                    ? string.Format("{0} FrameID={1}>>>>>>{2}", tag, Time.frameCount, message)
                    : string.Format("{0}>>>>>>{1}", tag, message));
            }
        }

        public static void e(string tag, string message, bool showFrameCount = true)
        {
            if (LogLevel.LogError <= logLevel)
            {
                Debug.Log(showFrameCount
                    ? string.Format("{0} FrameID={1}>>>>>>{2}", tag, Time.frameCount, message)
                    : string.Format("{0}>>>>>>{1}", tag, message));
            }
        }

        public static void f(string tag, string message, bool showFrameCount = true)
        {
            if (LogLevel.LogFatal <= logLevel)
            {
                Debug.Log(showFrameCount
                    ? string.Format("{0} FrameID={1}>>>>>>{2}", tag, Time.frameCount, message)
                    : string.Format("{0}>>>>>>{1}", tag, message));
            }
        }
    }
}