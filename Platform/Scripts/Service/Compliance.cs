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
    /**
     * \ingroup Platform
     * Application or games need comply with the laws where they distributes. So developers
     * should take compliance into consideration. This module provides some useful methods
     * to implement compliance.
     */
    public static class ComplianceService
    {
        /// <summary>
        /// Detects sensitive words in texts.
        /// </summary>
        /// <param name="scene">Indicates where the text appears. For example, the text can appear in a username, room name, in-room chat, etc.</param>
        /// <param name="content">The text to check, which can be a username, room-chat message, etc.</param>
        /// <returns>
        /// Whether the text contains sensitive words. If it contains, the app should not allow
        /// the user to publish the text and can take the strategy proposed by the
        /// result.
        /// </returns>
        public static Task<DetectSensitiveResult> DetectSensitive(DetectSensitiveScene scene, string content)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<DetectSensitiveResult>(CLIB.ppf_Compliance_DetectSensitive(scene, content));
        }
    }
}