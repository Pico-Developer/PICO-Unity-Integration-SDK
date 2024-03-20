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
    public class DetectSensitiveResult
    {
        /// The filtered text is a string which replace sensitive words with `*`. 
        public readonly string FilteredText;

        /// The proposed strategy to handle user operation. 
        public readonly SensitiveProposal Proposal;

        public DetectSensitiveResult(IntPtr o)
        {
            FilteredText = CLIB.ppf_DetectSensitiveResult_GetFilteredText(o);
            Proposal = CLIB.ppf_DetectSensitiveResult_GetProposal(o);
        }
    }
}