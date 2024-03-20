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

namespace Pico.Platform
{
    public class CloudStorageService
    {
        /// <summary>
        /// Starts cloud data backup whenever needed.
        /// </summary>
        /// <returns>Returns nothing for a success, otherwise returns error information.</returns>
        public static Task StartNewBackup()
        {
            return new Task(CLIB.ppf_CloudStorage_StartNewBackup());
        }
    }
}