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

using UnityEngine;

namespace Unity.XR.PXR
{
    public class PXR_FoveationRendering
    {
        private static PXR_FoveationRendering instance = null;
        public static PXR_FoveationRendering Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PXR_FoveationRendering();
                }

                return instance;
            }
        }

        /// <summary>
        /// Sets a foveated rendering level.
        /// </summary>
        /// <param name="level">Select a foveated rendering level:
        /// * `None`: disable foveated rendering
        /// * `Low`
        /// * `Med`
        /// * `High`
        /// * `TopHigh`
        /// </param>
        /// <param name="isETFR">
        /// Describe if the foveated rendering mode is eye tracked foveated rendering (ETFR):
        /// * `true`: ETFR
        /// * `false`: not ETFR
        /// </param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool SetFoveationLevel(FoveationLevel level, bool isETFR)
        {
            if (isETFR)
            {
                return PXR_Plugin.Render.UPxr_SetEyeFoveationLevel(level);
            }
            else
            {
                return PXR_Plugin.Render.UPxr_SetFoveationLevel(level);
            }
        }

        /// <summary>
        /// Gets the current foveated rendering level.
        /// </summary>
        /// <returns>The current foveated rendering level:
        /// * `None` (`-1`): foveated rendering disabled
        /// * `Low`
        /// * `Med`
        /// * `High`
        /// * `TopHigh`
        /// </returns>
        public static FoveationLevel GetFoveationLevel()
        {
            return PXR_Plugin.Render.UPxr_GetFoveationLevel();
        }

        /// <summary>
        /// Sets foveated rendering parameters.
        /// </summary>
        /// <param name="foveationGainX">Set the reduction rate of peripheral pixels in the X-axis direction. Value range: [1.0, 10.0], the greater the value, the higher the reduction rate.</param>
        /// <param name="foveationGainY">Set the reduction rate of peripheral pixels in the Y-axis direction. Value range: [1.0, 10.0], the greater the value, the higher the reduction rate.</param>
        /// <param name="foveationArea">Set the range of foveated area whose resolution is not to be reduced. Value range: [0.0, 4.0], the higher the value, the bigger the high-quality central area.</param>
        /// <param name="foveationMinimum">Set the minimum pixel density. Recommended values: 1/32, 1/16, 1/8, 1/4, 1/2. The actual pixel density will be greater than or equal to the value set here.</param>
        public static void SetFoveationParameters(float foveationGainX, float foveationGainY, float foveationArea, float foveationMinimum)
        {
            PXR_Plugin.Render.UPxr_SetFoveationParameters(foveationGainX, foveationGainY, foveationArea, foveationMinimum);
        }
    }
}