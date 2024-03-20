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

namespace Pico.Platform
{
    public class TimeUtil
    {
        public static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static int GetUtcSeconds()
        {
            return DateTimeToSeconds(DateTime.Now);
        }

        public static long GetUtcMilliSeconds()
        {
            return DateTimeToMilliSeconds(DateTime.Now);
        }

        public static int DateTimeToSeconds(DateTime t)
        {
            return (int) (t.ToUniversalTime() - UnixEpoch).TotalSeconds;
        }

        public static long DateTimeToMilliSeconds(DateTime t)
        {
            return (long) (t.ToUniversalTime() - UnixEpoch).TotalMilliseconds;
        }

        public static DateTime MilliSecondsToDateTime(long milliSeconds)
        {
            return UnixEpoch.AddMilliseconds(milliSeconds).ToLocalTime();
        }

        public static DateTime SecondsToDateTime(long seconds)
        {
            return UnixEpoch.AddSeconds(seconds).ToLocalTime();
        }
    }
}