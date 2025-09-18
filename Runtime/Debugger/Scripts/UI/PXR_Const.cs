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
#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace Unity.XR.PXR.Debugger
{
    public class PXR_DebuggerConst{
        public static string sdkPackageName = "Packages/com.unity.xr.picoxr/";
        public static string sdkRootName = "com.unity.xr.picoxr/";
    }
    public enum LauncherButton
{
    PressA,
    PressB,
    PressX,
    PressY,
}
public enum StartPosiion
{
    Far,
    Near ,
    Medium,
}
}
#endif