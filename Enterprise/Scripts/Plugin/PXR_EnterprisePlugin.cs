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
#if (UNITY_ANDROID && !UNITY_EDITOR)
#define PICO_PLATFORM
#endif
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#if PICO_XR
using Unity.XR.PXR;
#else
using Unity.XR.OpenXR.Features.PICOSupport;
#endif
using UnityEngine;
using UnityEngine.XR;

namespace Unity.XR.PICO.TOBSupport
{
    public partial class PXR_EnterprisePlugin
    {
        private const string TAG = "[PXR_EnterprisePlugin]";
        public const int MAX_SIZE = 12208032;

        public static string token;
        private static int curSize = 0;
        private static bool camOpenned = false;

        private static FrameItemExt antiDistortionFrameItemExt;
        private static FrameItemExt distortionFrameItemExt;
        private static bool initDistortionFrame;

        [DllImport("libpxr_xrsdk_native", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getHeadTrackingConfidence();

        [DllImport("libpxr_xrsdk_native", CallingConvention = CallingConvention.Cdecl)]
        public static extern int openVSTCamera();

        [DllImport("libpxr_xrsdk_native", CallingConvention = CallingConvention.Cdecl)]
        public static extern int closeVSTCamera();

        [DllImport("libpxr_xrsdk_native", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getHeadTrackingData(Int64 predictTime, ref SixDof data, int type);

        [DllImport("libpxr_xrsdk_native", CallingConvention = CallingConvention.Cdecl)]
        public static extern int acquireVSTCameraFrame(ref FrameItemExt frame);

        [DllImport("libpxr_xrsdk_native", CallingConvention = CallingConvention.Cdecl)]
        public static extern int acquireVSTCameraFrameAntiDistortion(string token, Int32 width, Int32 height, ref FrameItemExt frame);

        [DllImport("libpxr_xrsdk_native", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getCameraParameters(string token, out RGBCameraParams rgb_Camera_Params);

#if PICO_XR
        [DllImport("pxr_api", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("openxr_pico", EntryPoint = "PICO_GetPredictedDisplayTime",
            CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Pxr_GetPredictedDisplayTime(ref double predictedDisplayTime);
        
#if PICO_XR
         [DllImport("pxr_api", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("openxr_pico", EntryPoint = "PICO_GetPredictedMainSensorState2",
            CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Pxr_GetPredictedMainSensorState2(double predictTimeMs, ref PxrSensorState2 sensorState, ref int sensorFrameIndex);

#if PICO_PLATFORM
            private static AndroidJavaClass unityPlayer;
            private static AndroidJavaObject currentActivity;
            private static AndroidJavaObject tobHelper;
            private static AndroidJavaClass tobHelperClass;
            private static AndroidJavaObject IToBService;
            private static AndroidJavaClass BAuthLib;
#endif

        public static Action<bool> BoolCallback;
        public static Action<int> IntCallback;
        public static Action<long> LongCallback;
        public static Action<string> StringCallback;
        
        private static AndroidJavaObject GetEnumType(Enum enumType)
        {
            AndroidJavaClass enumjs =
                new AndroidJavaClass("com.pvr.tobservice.enums" + enumType.GetType().ToString().Replace("Unity.XR.PICO.TOBSupport.", ".PBS_"));
            AndroidJavaObject enumjo = enumjs.GetStatic<AndroidJavaObject>(enumType.ToString());
            return enumjo;
        }
        
        public static bool UPxr_InitEnterpriseService()
        {
#if PICO_PLATFORM
                tobHelperClass = new AndroidJavaClass("com.picoxr.tobservice.ToBServiceUtils");
                tobHelper = tobHelperClass.CallStatic<AndroidJavaObject>("getInstance");
                unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                BAuthLib = new AndroidJavaClass("com.pvr.tobauthlib.AuthCheckServer");
#endif
            return UPxr_GetToken();
        }

        public static void UPxr_SetBindCallBack(BindCallback mBoolCallback)
        {
#if PICO_PLATFORM
            tobHelper.Call("setBindCallBack", mBoolCallback);
#endif
        }

        public static void UPxr_BindEnterpriseService(Action<bool> callback = null)
        {
#if PICO_PLATFORM

            UPxr_SetBindCallBack(new BindCallback(callback));
            tobHelper.Call("bindTobService", currentActivity);
#endif
        }

        public static void UPxr_UnBindEnterpriseService()
        {
#if PICO_PLATFORM
                tobHelper.Call("unBindTobService");
#endif
        }

        public static void GetServiceBinder()
        {
#if PICO_PLATFORM
            IToBService = tobHelper.Call<AndroidJavaObject>("getServiceBinder");
#endif
        }

        public static string UPxr_StateGetDeviceInfo(SystemInfoEnum type,int ext)
        {
            string result = "";
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return result;
            }
            result = IToBService.Call<string>("pbsStateGetDeviceInfo", GetEnumType(type), ext);
#endif
            return result;
        }

        public static void UPxr_ControlSetDeviceAction(DeviceControlEnum deviceControl, Action<int> callback)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsControlSetDeviceAction", GetEnumType(deviceControl), new IntCallback(callback));
#endif
        }

        public static void UPxr_ControlAPPManager(PackageControlEnum packageControl, string path, Action<int> callback,int ext)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsControlAPPManger", GetEnumType(packageControl), path, ext,  new IntCallback(callback));
#endif
        }

        public static void UPxr_ControlSetAutoConnectWIFI(string ssid, string pwd, Action<bool> callback,int ext)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsControlSetAutoConnectWIFI", ssid, pwd, ext, new BoolCallback(callback));
#endif
        }

        public static void UPxr_ControlClearAutoConnectWIFI(Action<bool> callback)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsControlClearAutoConnectWIFI", new BoolCallback(callback));
#endif
        }

        public static void UPxr_PropertySetHomeKey(HomeEventEnum eventEnum, HomeFunctionEnum function, Action<bool> callback)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsPropertySetHomeKey", GetEnumType(eventEnum), GetEnumType(function),  new BoolCallback(callback));
#endif
        }

        public static void UPxr_PropertySetHomeKeyAll(HomeEventEnum eventEnum, HomeFunctionEnum function, int timesetup, string pkg, string className, Action<bool> callback)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsPropertySetHomeKeyAll", GetEnumType(eventEnum), GetEnumType(function), timesetup, pkg,
                className, new BoolCallback(callback));
#endif
        }

        public static void UPxr_PropertyDisablePowerKey(bool isSingleTap, bool enable, Action<int> callback)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsPropertyDisablePowerKey", isSingleTap, enable,  new IntCallback(callback));
#endif
        }

        public static void UPxr_PropertySetScreenOffDelay(ScreenOffDelayTimeEnum timeEnum, Action<int> callback)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsPropertySetScreenOffDelay", GetEnumType(timeEnum), new IntCallback(callback));
#endif
        }

        public static void UPxr_PropertySetSleepDelay(SleepDelayTimeEnum timeEnum)
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }

            IToBService.Call("pbsPropertySetSleepDelay", GetEnumType(timeEnum));
#endif
        }

        public static void UPxr_SwitchSystemFunction(SystemFunctionSwitchEnum systemFunction, SwitchEnum switchEnum,int ext)
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsSwitchSystemFunction", GetEnumType(systemFunction), GetEnumType(switchEnum), ext);
#endif
        }

        public static void UPxr_SwitchSetUsbConfigurationOption(USBConfigModeEnum uSBConfigModeEnum,int ext)
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsSwitchSetUsbConfigurationOption", GetEnumType(uSBConfigModeEnum), ext);
#endif
        }

        public static void UPxr_SetControllerPairTime(ControllerPairTimeEnum timeEnum, Action<int> callback,int ext)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsSetControllerPairTime", GetEnumType(timeEnum),new IntCallback(callback), ext);
#endif
        }

        public static void UPxr_GetControllerPairTime(Action<int> callback,int ext)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsGetControllerPairTime",new IntCallback(callback), ext);
#endif
        }

        public static void UPxr_ScreenOn()
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsScreenOn");
#endif
        }

        public static void UPxr_ScreenOff()
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsScreenOff");
#endif
        }

        public static void UPxr_AcquireWakeLock()
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsAcquireWakeLock");
#endif
        }

        public static void UPxr_ReleaseWakeLock()
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsReleaseWakeLock");
#endif
        }

        public static void UPxr_EnableEnterKey()
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsEnableEnterKey");
#endif
        }

        public static void UPxr_DisableEnterKey()
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsDisableEnterKey");
#endif
        }

        public static void UPxr_EnableVolumeKey()
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsEnableVolumeKey");
#endif
        }

        public static void UPxr_DisableVolumeKey()
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsDisableVolumeKey");
#endif
        }

        public static void UPxr_EnableBackKey()
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsEnableBackKey");
#endif
        }

        public static void UPxr_DisableBackKey()
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsDisableBackKey");
#endif
        }


        public static void UPxr_ResetAllKeyToDefault(Action<bool> callback)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsResetAllKeyToDefault", new BoolCallback(callback));
#endif
        }

        public static void UPxr_SetAPPAsHome(SwitchEnum switchEnum, string packageName)
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsAppSetAPPAsHomeTwo", GetEnumType(switchEnum), packageName);
#endif
        }

        public static void UPxr_KillAppsByPidOrPackageName(int[] pids, string[] packageNames,int ext)
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsKillAppsByPidOrPackageName", pids, packageNames, ext);
#endif
        }

        public static void UPxr_KillBackgroundAppsWithWhiteList(string[] packageNames,int ext)
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsKillBackgroundAppsWithWhiteList",packageNames, ext);
#endif
        }

        public static void UPxr_FreezeScreen(bool freeze)
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsFreezeScreen", freeze);
#endif
        }

        public static void UPxr_OpenMiracast()
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsOpenMiracast");
#endif
        }

        public static bool UPxr_IsMiracastOn()
        {
            bool value = false;
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }

            value = IToBService.Call<bool>("pbsIsMiracastOn");
#endif
            return value;
        }

        public static void UPxr_CloseMiracast()
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsCloseMiracast");
#endif
        }

        public static void UPxr_StartScan()
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsStartScan");
#endif
        }

        public static void UPxr_StopScan()
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsStopScan");
#endif
        }

        public static void UPxr_ConnectWifiDisplay(string modelJson)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsConnectWifiDisplay", modelJson);
#endif
        }

        public static void UPxr_DisConnectWifiDisplay()
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsDisConnectWifiDisplay");
#endif
        }

        public static void UPxr_ForgetWifiDisplay(string address)
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsForgetWifiDisplay", address);
#endif
        }

        public static void UPxr_RenameWifiDisplay(string address, string newName)
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsRenameWifiDisplay", address, newName);
#endif
        }

        public static void UPxr_SetWDModelsCallback(Action<List<WifiDisplayModel>> callback)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsSetWDModelsCallback", new WifiDisplayModelCallback(callback));
#endif
        }

        public static void UPxr_SetWDJsonCallback(Action<string> callback)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsSetWDJsonCallback", new StringCallback(callback));
#endif
        }

        public static void UPxr_UpdateWifiDisplays()
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return;
            }
            IToBService.Call("pbsUpdateWifiDisplays");
#endif
        }

        public static string UPxr_GetConnectedWD()
        {
            string result = "";
#if PICO_PLATFORM
            result = tobHelper.Call<string>("pbsGetConnectedWD");
#endif
            return result;
        }

        public static void UPxr_SwitchLargeSpaceScene(bool open, Action<bool> callback,int ext)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsSwitchLargeSpaceScene", new BoolCallback(callback), open, ext);
#endif
        }

        public static void UPxr_GetSwitchLargeSpaceStatus(Action<string> callback,int ext)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsGetSwitchLargeSpaceStatus",new StringCallback(callback), ext);
#endif
        }

        public static bool UPxr_SaveLargeSpaceMaps(int ext)
        {
            bool value = false;
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }
            value = IToBService.Call<bool>("pbsSaveLargeSpaceMaps", ext);
#endif
            return value;
        }

        public static void UPxr_ExportMaps(Action<bool> callback,int ext)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsExportMaps", new BoolCallback(callback), ext);
#endif
        }

        public static void UPxr_ImportMaps(Action<bool> callback,int ext)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsImportMaps", new BoolCallback(callback), ext);
#endif
        }

        public static float[] UPxr_GetCpuUsages()
        {
            float[] data = null;
#if PICO_PLATFORM
            data = tobHelper.Call<float[]>("pbsGetCpuUsages");
#endif
            return data;
        }

        public static float[] UPxr_GetDeviceTemperatures(int type, int source)
        {
            float[] data = null;
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return null;
            }

            data = IToBService.Call<float[]>("pbsGetDeviceTemperatures", type, source);
#endif

            return data;
        }

        public static void UPxr_Capture()
        {
#if PICO_PLATFORM
            IToBService.Call("pbsCapture");
#endif
        }

        public static void UPxr_Record()
        {
#if PICO_PLATFORM
            IToBService.Call("pbsRecord");
#endif
        }

        public static void UPxr_ControlSetAutoConnectWIFIWithErrorCodeCallback(String ssid, String pwd, int ext, Action<int> callback)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsControlSetAutoConnectWIFIWithErrorCodeCallback",ssid,pwd,ext,new IntCallback(callback));
#endif
        }

        public static void UPxr_AppKeepAlive(String appPackageName, bool keepAlive, int ext)
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return ;
            }
            IToBService.Call("pbsAppKeepAlive",appPackageName,keepAlive,ext);
#endif
        }

        public static void UPxr_TimingStartup(int year, int month, int day, int hour, int minute, bool open)
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return ;
            }
            IToBService.Call("pbsTimingStartup", year, month, day, hour, minute, open);
#endif
        }

        public static void UPxr_TimingShutdown(int year, int month, int day, int hour, int minute, bool open)
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return ;
            }
            IToBService.Call("pbsTimingShutdown", year, month, day, hour, minute, open);
#endif
        }

        public static void UPxr_StartVrSettingsItem(StartVRSettingsEnum settingsEnum, bool hideOtherItem, int ext)
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return ;
            }
            IToBService.Call("pbsStartVrSettingsItem", GetEnumType(settingsEnum), hideOtherItem, ext);
#endif
        }

        public static void UPxr_SwitchVolumeToHomeAndEnter(SwitchEnum switchEnum, int ext)
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return ;
            }
            IToBService.Call("pbsSwitchVolumeToHomeAndEnter", GetEnumType(switchEnum), ext);
#endif
        }

        public static SwitchEnum UPxr_IsVolumeChangeToHomeAndEnter()
        {
            SwitchEnum switchEnum = SwitchEnum.S_OFF;
#if PICO_PLATFORM
                int num = 0;
                num = tobHelper.Call<int>("pbsIsVolumeChangeToHomeAndEnter");
                if (num == 0)
                {
                    switchEnum = SwitchEnum.S_ON;
                }
                else if (num == 1) {
                    switchEnum = SwitchEnum.S_OFF;
                }
#endif
            return switchEnum;
        }

        public static int UPxr_InstallOTAPackage(String otaPackagePath,int ext)
        {
            int value = 0;
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }
            
            value = IToBService.Call<int>("pbsInstallOTAPackage",otaPackagePath, ext);
#endif
            return value;
        }

        public static string UPxr_GetAutoConnectWiFiConfig(int ext)
        {
            string value= "";
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }

            value = IToBService.Call<string>("pbsGetAutoConnectWiFiConfig", ext);
#endif
            return value;
        }

        public static string UPxr_GetTimingStartupStatus(int ext)
        {
            string value = "";
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }

            value = IToBService.Call<string>("pbsGetTimingStartupStatus", ext);
#endif
            return value;
        }

        public static string UPxr_GetTimingShutdownStatus(int ext)
        {
            string value = "";
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }

            value = IToBService.Call<string>("pbsGetTimingShutdownStatus", ext);
#endif
            return value;
        }

        public static int UPxr_GetControllerKeyState(ControllerKeyEnum pxrControllerKey,int ext)
        {
            int value = 1;
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }

            value = IToBService.Call<int>("pbsGetControllerKeyState", GetEnumType(pxrControllerKey),ext);
#endif
            return value;
        }

        public static int UPxr_SetControllerKeyState(ControllerKeyEnum controllerKeyEnum, SwitchEnum status,int ext)
        {
            int value = 1;
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }

            value = IToBService.Call<int>("pbsSetControllerKeyState", GetEnumType(controllerKeyEnum),GetEnumType(status),ext);
#endif
            return value;
        }

        public static SwitchEnum UPxr_ControlGetPowerOffWithUSBCable(int ext)
        {
            SwitchEnum switchEnum = SwitchEnum.S_OFF;
#if PICO_PLATFORM
                int num = 0;
                num = tobHelper.Call<int>("pbsControlGetPowerOffWithUSBCable",ext);
                if (num == 0)
                {
                    switchEnum = SwitchEnum.S_ON;
                }
                else if (num == 1) {
                    switchEnum = SwitchEnum.S_OFF;
                }
#endif
            return switchEnum;
        }

        public static ScreenOffDelayTimeEnum UPxr_PropertyGetScreenOffDelay(int ext)
        {
            ScreenOffDelayTimeEnum screenOffDelayTimeEnum = ScreenOffDelayTimeEnum.NEVER;
#if PICO_PLATFORM
                int num = 0;
                num = tobHelper.Call<int>("pbsPropertyGetScreenOffDelay", ext);
                switch (num) {
                    case 0:
                        screenOffDelayTimeEnum = ScreenOffDelayTimeEnum.THREE;
                        break;
                    case 1:
                        screenOffDelayTimeEnum = ScreenOffDelayTimeEnum.TEN;
                        break;
                    case 2:
                        screenOffDelayTimeEnum = ScreenOffDelayTimeEnum.THIRTY;
                        break;
                    case 3:
                        screenOffDelayTimeEnum = ScreenOffDelayTimeEnum.SIXTY;
                        break;
                    case 4:
                        screenOffDelayTimeEnum = ScreenOffDelayTimeEnum.THREE_HUNDRED;
                        break;
                    case 5:
                        screenOffDelayTimeEnum = ScreenOffDelayTimeEnum.SIX_HUNDRED;
                        break;
                    case 6:
                        screenOffDelayTimeEnum = ScreenOffDelayTimeEnum.NEVER;
                        break;
                }
#endif
            return screenOffDelayTimeEnum;
        }

        public static SleepDelayTimeEnum UPxr_PropertyGetSleepDelay(int ext)
        {
            SleepDelayTimeEnum sleepDelayTime = SleepDelayTimeEnum.NEVER;
#if PICO_PLATFORM
                int num = 0;
                num = tobHelper.Call<int>("pbsPropertyGetSleepDelay", ext);
                switch (num)
                {
                    case 0:
                        sleepDelayTime = SleepDelayTimeEnum.FIFTEEN;
                        break;
                    case 1:
                        sleepDelayTime = SleepDelayTimeEnum.THIRTY;
                        break;
                    case 2:
                        sleepDelayTime = SleepDelayTimeEnum.SIXTY;
                        break;
                    case 3:
                        sleepDelayTime = SleepDelayTimeEnum.THREE_HUNDRED;
                        break;
                    case 4:
                        sleepDelayTime = SleepDelayTimeEnum.SIX_HUNDRED;
                        break;
                    case 5:
                        sleepDelayTime = SleepDelayTimeEnum.ONE_THOUSAND_AND_EIGHT_HUNDRED;
                        break;
                    case 6:
                        sleepDelayTime = SleepDelayTimeEnum.NEVER;
                        break;
                }
#endif
            return sleepDelayTime;
        }

        public static string UPxr_PropertyGetPowerKeyStatus(int ext)
        {
            string value = "";
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }

            value = IToBService.Call<string>("pbsPropertyGetPowerKeyStatus", ext);
#endif
            return value;
        }

        public static int UPxr_GetEnterKeyStatus(int ext)
        {
            int value = 1;
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }

            value = IToBService.Call<int>("pbsGetEnterKeyStatus",ext);
#endif
            return value;
        }

        public static int UPxr_GetVolumeKeyStatus(int ext)
        {
            int value = 1;
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }

            value = IToBService.Call<int>("pbsGetVolumeKeyStatus",ext);
#endif
            return value;
        }

        public static int UPxr_GetBackKeyStatus(int ext)
        {
            int value = 1;
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }

            value = IToBService.Call<int>("pbsGetBackKeyStatus",ext);
#endif
            return value;
        }

        public static string UPxr_PropertyGetHomeKeyStatus(HomeEventEnum homeEvent,int ext)
        {
            string value = "";
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }

            value = IToBService.Call<string>("pbsPropertyGetHomKeyStatus", GetEnumType(homeEvent),ext);
#endif
            return value;
        }

        public static void UPxr_GetSwitchSystemFunctionStatus(SystemFunctionSwitchEnum systemFunction, Action<int> callback,int ext)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsGetSwitchSystemFunctionStatus", GetEnumType(systemFunction), new IntCallback(callback),
                ext);
#endif
        }

        public static string UPxr_SwitchGetUsbConfigurationOption(int ext)
        {
            string value = "";
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }

            value = IToBService.Call<string>("pbsSwitchGetUsbConfigurationOption", ext);
#endif
            return value;
        }

        public static string UPxr_GetCurrentLauncher(int ext)
        {
            string value = "";
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }

            value = IToBService.Call<string>("pbsGetCurrentLauncher", ext);
#endif
            return value;
        }

        public static int UPxr_PICOCastInit(Action<int> callback,int ext)
        {
            int value = 0;
#if PICO_PLATFORM
            value = tobHelper.Call<int>("pbsPicoCastInit", new IntCallback(callback), ext);
#endif
            return value;
        }

        public static int UPxr_PICOCastSetShowAuthorization(int authZ,int ext)
        {
            int value = 0;
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }

            value = IToBService.Call<int>("pbsPicoCastSetShowAuthorization",authZ,ext);
#endif
            return value;
        }

        public static int UPxr_PICOCastGetShowAuthorization(int ext)
        {
            int value = 0;
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }
            value = IToBService.Call<int>("pbsPicoCastGetShowAuthorization",ext);
#endif
            return value;
        }

        public static string UPxr_PICOCastGetUrl(PICOCastUrlTypeEnum urlType,int ext)
        {
            string value = "";
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }
            value = IToBService.Call<string>("pbsPicoCastGetUrl",GetEnumType(urlType), ext);
#endif
            return value;
        }

        public static int UPxr_PICOCastStopCast(int ext)
        {
            int value = 0;
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }
            value = IToBService.Call<int>("pbsPicoCastStopCast",ext);
#endif
            return value;
        }

        public static int UPxr_PICOCastSetOption(PICOCastOptionOrStatusEnum castOptionOrStatus, PICOCastOptionValueEnum castOptionValue,int ext)
        {
            int value = 0;
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }
            value = IToBService.Call<int>("pbsPicoCastSetOption",GetEnumType(castOptionOrStatus),GetEnumType(castOptionValue),ext);
#endif
            return value;
        }

        public static PICOCastOptionValueEnum UPxr_PICOCastGetOptionOrStatus(PICOCastOptionOrStatusEnum castOptionOrStatus,int ext)
        {
            PICOCastOptionValueEnum value = PICOCastOptionValueEnum.STATUS_VALUE_ERROR;
#if PICO_PLATFORM
                int num = 0;
                if (tobHelper == null)
                {
                    return value;
                }
                num = tobHelper.Call<int>("pbsPicoCastGetOptionOrStatus", GetEnumType(castOptionOrStatus), ext);
                switch (num)
                {
                    case 0:
                        value = PICOCastOptionValueEnum.OPTION_VALUE_RESOLUTION_HIGH;
                        break;
                    case 1:
                        value = PICOCastOptionValueEnum.OPTION_VALUE_RESOLUTION_MIDDLE;
                        break;
                    case 2:
                        value = PICOCastOptionValueEnum.OPTION_VALUE_RESOLUTION_AUTO;
                        break;
                    case 3:
                        value = PICOCastOptionValueEnum.OPTION_VALUE_RESOLUTION_HIGH_2K;
                        break;
                    case 4:
                        value = PICOCastOptionValueEnum.OPTION_VALUE_RESOLUTION_HIGH_4K;
                        break;
                    case 5:
                        value = PICOCastOptionValueEnum.OPTION_VALUE_BITRATE_HIGH;
                        break;
                    case 6:
                        value = PICOCastOptionValueEnum.OPTION_VALUE_BITRATE_MIDDLE;
                        break;
                    case 7:
                        value = PICOCastOptionValueEnum.OPTION_VALUE_BITRATE_LOW;
                        break;
                    case 8:
                        value = PICOCastOptionValueEnum.OPTION_VALUE_AUDIO_ON;
                        break;
                    case 9:
                        value = PICOCastOptionValueEnum.OPTION_VALUE_AUDIO_OFF;
                        break;
                    case 10:
                        value = PICOCastOptionValueEnum.STATUS_VALUE_STATE_STARTED;
                        break;
                    case 11:
                        value = PICOCastOptionValueEnum.STATUS_VALUE_STATE_STOPPED;
                        break;
                    case 12:
                        value = PICOCastOptionValueEnum.STATUS_VALUE_ERROR;
                        break;
                }
#endif
                return value;
        }

        public static int UPxr_SetSystemLanguage(String language,int ext)
        {
            int num = 0;
#if PICO_PLATFORM
                num = IToBService.Call<int>("pbsSetSystemLanguage", language, ext);
#endif
            return num;
        }

        public static String UPxr_GetSystemLanguage(int ext)
        {
            string value = "";
#if PICO_PLATFORM
            value = IToBService.Call<string>("pbsGetSystemLanguage", ext);
#endif
            return value;
        }

        public static int UPxr_ConfigWifi(String ssid, String pwd,int ext)
        {
            int num = 0;
#if PICO_PLATFORM
                num = IToBService.Call<int>("pbsConfigWifi",ssid,pwd,ext);
#endif
            return num;
        }

        public static String[] UPxr_GetConfiguredWifi(int ext)
        {
#if PICO_PLATFORM
                return IToBService.Call<string[]>("pbsGetConfiguredWifi",ext);
#endif
            return null;
        }

        public static int UPxr_SetSystemCountryCode(String countryCode, Action<int> callback,int ext)
        {
            int num = 0;
#if PICO_PLATFORM
            num = tobHelper.Call<int>("pbsSetSystemCountryCode",countryCode,new IntCallback(callback),ext);
#endif
            return num;
        }

        public static string UPxr_GetSystemCountryCode(int ext)
        {
            string value = "";
#if PICO_PLATFORM
            value = IToBService.Call<string>("pbsGetSystemCountryCode",ext);
#endif
            return value;
        }

        public static int UPxr_SetSkipInitSettingPage(int flag,int ext)
        {
            int num = 0;
#if PICO_PLATFORM
                num = IToBService.Call<int>("pbsSetSkipInitSettingPage",flag,ext);
#endif
            return num;
        }

        public static int UPxr_GetSkipInitSettingPage(int ext)
        {
            int num = 0;
#if PICO_PLATFORM
                num = IToBService.Call<int>("pbsGetSkipInitSettingPage",ext);
#endif
            return num;
        }

        public static int UPxr_IsInitSettingComplete(int ext)
        {
            int num = 0;
#if PICO_PLATFORM
                num = IToBService.Call<int>("pbsIsInitSettingComplete",ext);
#endif
            return num;
        }

        public static int UPxr_StartActivity(String packageName, String className, String action, String extra, String[] categories, int[] flags,int ext)
        {
            int num = 0;
#if PICO_PLATFORM
                num = IToBService.Call<int>("pbsStartActivity", packageName, className, action, extra, categories, flags,ext);
#endif

            return num;
        }

        public static int UPxr_CustomizeAppLibrary(String[] packageNames, SwitchEnum switchEnum,int ext)
        {
            int num = 0;
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return num;
            }
            num = IToBService.Call<int>("pbsCustomizeAppLibrary", packageNames,GetEnumType(switchEnum), ext);
#endif
            return num;
        }

        public static int[] UPxr_GetControllerBattery(int ext)
        {
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return null;
            }
            return IToBService.Call<int[]>("pbsGetControllerBattery", ext);
#endif
            return null;
        }

        public static int UPxr_GetControllerConnectState(int ext)
        {
            int num = 0;
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return num;
            }
            num = IToBService.Call<int>("pbsGetControllerConnectState",ext);
#endif
            return num;
        }

        public static string UPxr_GetAppLibraryHideList(int ext)
        {
            string value = " ";
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }

            value = IToBService.Call<string>("pbsGetAppLibraryHideList",ext);
#endif
            return value;
        }

        public static int UPxr_SetScreenCastAudioOutput(ScreencastAudioOutputEnum screencastAudioOutput,int ext)
        {
            int value = 0;
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }

            value = IToBService.Call<int>("pbsSetScreenCastAudioOutput",GetEnumType(screencastAudioOutput),ext);
#endif
            return value;
        }

        public static ScreencastAudioOutputEnum UPxr_GetScreenCastAudioOutput(int ext)
        {
            ScreencastAudioOutputEnum value = ScreencastAudioOutputEnum.AUDIO_ERROR;
#if PICO_PLATFORM
                int num = 0;               
                num = tobHelper.Call<int>("pbsGetScreenCastAudioOutput",ext);
                switch (num)
                {
                    case 0:
                        value = ScreencastAudioOutputEnum.AUDIO_SINK;
                        break;
                    case 1:
                        value = ScreencastAudioOutputEnum.AUDIO_TARGET;
                        break;
                    case 2:
                        value = ScreencastAudioOutputEnum.AUDIO_SINK_TARGET;
                        break;
                }
#endif
            return value;
        }

        public static int UPxr_CustomizeSettingsTabStatus(CustomizeSettingsTabEnum customizeSettingsTabEnum, SwitchEnum switchEnum,int ext)
        {
            int value = 0;
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }

            value = IToBService.Call<int>("pbsCustomizeSettingsTabStatus", GetEnumType(customizeSettingsTabEnum), GetEnumType(switchEnum), ext);
#endif
            return value;
        }

        public static SwitchEnum UPxr_GetCustomizeSettingsTabStatus(CustomizeSettingsTabEnum customizeSettingsTabEnum,int ext)
        {
            SwitchEnum switchEnum = SwitchEnum.S_OFF;
#if PICO_PLATFORM
                int num = 0;
                num = tobHelper.Call<int>("pbsGetCustomizeSettingsTabStatus",GetEnumType(customizeSettingsTabEnum),ext);
                if (num == 0)
                {
                    switchEnum = SwitchEnum.S_ON;
                }
                else if (num == 1) {
                    switchEnum = SwitchEnum.S_OFF;
                }
#endif
            return switchEnum;
        }
        
        public static void UPxr_SetPowerOffWithUSBCable(SwitchEnum switchEnum,int ext)
        {
           
#if PICO_PLATFORM
             if (IToBService==null)
            {
                return;
            }
            IToBService.Call("pbsControlSetPowerOffwithUSBCable", GetEnumType(switchEnum),ext);
#endif
        }
        public static void UPxr_RemoveControllerHomeKey(HomeEventEnum EventEnum)
        {
#if PICO_PLATFORM
            if (IToBService==null)
            {
                return;
            }
            IToBService.Call("pbsRemoveControllerHomeKey", GetEnumType(EventEnum));
#endif
        }
        public static void UPxr_SetPowerOnOffLogo(PowerOnOffLogoEnum powerOnOffLogoEnum, String path, Action<bool> callback,int ext)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsPropertySetPowerOnOffLogo",GetEnumType(powerOnOffLogoEnum),path,ext, new BoolCallback(callback));
#endif
        }
        public static void UPxr_SetIPD(float ipd, Action<int> callback)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsSetIPD",ipd, new IntCallback(callback));
#endif
        }
        
        public static string UPxr_GetAutoMiracastConfig(int ext)
        {
            string value = " ";
#if PICO_PLATFORM
            if (IToBService == null)
            {
                return value;
            }
            value = IToBService.Call<string>("pbsGetAutoMiracastConfig",ext);
#endif
            return value;
        }
        public static int UPxr_SetPicoCastMediaFormat(PicoCastMediaFormat mediaFormat, int ext)
        {
            int value = -1;
#if PICO_PLATFORM
            value = tobHelper.Call<int>("setPicoCastMediaFormat",mediaFormat.bitrate,ext);
#endif
            return value;
        }
        
        public static int UPxr_setMarkerInfoCallback(TrackingOriginModeFlags trackingMode,float cameraYOffset,Action<List<MarkerInfo>>  mediaFormat)
        {
            int value = -1;

#if PICO_PLATFORM
            value = tobHelper.Call<int>("setMarkerInfoCallback",new MarkerInfoCallback(trackingMode,cameraYOffset,mediaFormat));
#endif
            return value;
        }


        private static bool UPxr_GetToken()
        {
            PLog.i(TAG, "GetToken Start");
#if PICO_PLATFORM
            token = BAuthLib.CallStatic<string>("featureAuthByToken", currentActivity, "getCameraInfo");
#endif
            if (string.IsNullOrEmpty(token))
            {
                PLog.e(TAG, "Failed to obtain token, camera data cannot be obtained!");
                return false;
            }
            PLog.i(TAG, "GetToken End token :" + token);
            return true;
        }

        public static int UPxr_GetHeadTrackingConfidence()
        {
            PLog.d(TAG, "GetHeadTrackingConfidence Start");
            int result = -1;
#if PICO_PLATFORM
            result = getHeadTrackingConfidence();
#endif
            PLog.d(TAG, "GetToken End result :" + result);

            return result;
        }

        public static bool UPxr_OpenVSTCamera()
        {
            PLog.d(TAG, "OpenVSTCamera Start");
            if (camOpenned)
            {
                PLog.d(TAG, "Camera has Openned!");
                return true;
            }

            int result = -1;
#if PICO_PLATFORM
            result = openVSTCamera();
#endif
            camOpenned = result == 0;
            PLog.d(TAG, "OpenVSTCamera End result :" + result + ", camOpenned : " + camOpenned);
            return result == 0;
        }

        public static bool UPxr_CloseVSTCamera()
        {
            PLog.d(TAG, "CloseVSTCamera Start");
            if (!camOpenned)
            {
                PLog.d(TAG, "Camera has Closed!");
                return true;
            }

            int result = -1;
#if PICO_PLATFORM
            result = closeVSTCamera();
#endif
            camOpenned = !(result == 0);
            PLog.d(TAG, "CloseVSTCamera End result :" + result + ", camOpenned : " + camOpenned);
            return result == 0;
        }

        public static int UPxr_GetHeadTrackingData(Int64 predictTime, ref SixDof data, int type)
        {
            PLog.d(TAG, "GetHeadTrackingData Start");
            int result = -1;
#if PICO_PLATFORM
            result = getHeadTrackingData(predictTime, ref data, type);
#endif
            PLog.d(TAG, "GetHeadTrackingData End result :" + result);

            return result;
        }

        public static int UPxr_AcquireVSTCameraFrame(out Frame frame)
        {
            PLog.d(TAG, "AcquireVSTCameraFrame Start");
            frame = new Frame();
            if (string.IsNullOrEmpty(token))
            {
                PLog.e(TAG, "Failed to obtain token, camera data cannot be obtained!");
                return -1;
            }

            if (!camOpenned)
            {
                PLog.e(TAG, "Failed to obtain data due to camera not being turned on!");
                return -1;
            }

            InitDistortionFrame();
            int result = -1;
#if PICO_PLATFORM
            result = acquireVSTCameraFrame(ref distortionFrameItemExt);
#endif
            frame.width = distortionFrameItemExt.frame.width;
            frame.height = distortionFrameItemExt.frame.height;
            frame.timestamp = distortionFrameItemExt.frame.timestamp;
            frame.datasize = distortionFrameItemExt.frame.datasize;
            frame.data = distortionFrameItemExt.frame.data;

            if (frame.pose != null)
            {
                frame.pose.position.x = (float)distortionFrameItemExt.six_dof_pose.pose.x;
                frame.pose.position.y = (float)distortionFrameItemExt.six_dof_pose.pose.y;
                frame.pose.position.z = (float)distortionFrameItemExt.six_dof_pose.pose.z;
                frame.pose.rotation.w = (float)distortionFrameItemExt.six_dof_pose.pose.rw;
                frame.pose.rotation.x = (float)distortionFrameItemExt.six_dof_pose.pose.rx;
                frame.pose.rotation.y = (float)distortionFrameItemExt.six_dof_pose.pose.ry;
                frame.pose.rotation.z = (float)distortionFrameItemExt.six_dof_pose.pose.rz;
            }
            frame.status = distortionFrameItemExt.six_dof_pose.pose.confidence;

            PLog.d(TAG, "AcquireVSTCameraFrame End result :" + result);
            return result;
        }

        public static int UPxr_AcquireVSTCameraFrameAntiDistortion(int width, int height, out Frame frame)
        {
            PLog.d(TAG, "AcquireVSTCameraFrameAntiDistortion Start width:" + width + ", height:" + height);
            frame = new Frame();
            if (string.IsNullOrEmpty(token))
            {
                PLog.e(TAG, "Failed to obtain token, camera data cannot be obtained!");
                return -1;
            }

            if (!camOpenned)
            {
                PLog.e(TAG, "Failed to obtain data due to camera not being turned on!");
                return -1;
            }

            int size = width * height * 3;
            InitAntiDistortionFrame(size);
            int result = -1;
#if PICO_PLATFORM
            result = acquireVSTCameraFrameAntiDistortion(token, width, height, ref antiDistortionFrameItemExt);
#endif
            PLog.d(TAG, "AcquireVSTCameraFrameAntiDistortion End result :" + result +
                ", width : " + antiDistortionFrameItemExt.frame.width +
                ", height : " + antiDistortionFrameItemExt.frame.height +
                ", datasize : " + antiDistortionFrameItemExt.frame.datasize +
                ", data : " + antiDistortionFrameItemExt.frame.data +
                ", confidence : " + antiDistortionFrameItemExt.six_dof_pose.pose.confidence);

            frame.width = antiDistortionFrameItemExt.frame.width;
            frame.height = antiDistortionFrameItemExt.frame.height;
            frame.timestamp = antiDistortionFrameItemExt.frame.timestamp;
            frame.datasize = antiDistortionFrameItemExt.frame.datasize;
            frame.data = antiDistortionFrameItemExt.frame.data;

            if (frame.pose != null)
            {
                frame.pose.position.x = (float)antiDistortionFrameItemExt.six_dof_pose.pose.x;
                frame.pose.position.y = (float)antiDistortionFrameItemExt.six_dof_pose.pose.y;
                frame.pose.position.z = (float)antiDistortionFrameItemExt.six_dof_pose.pose.z;
                frame.pose.rotation.w = (float)antiDistortionFrameItemExt.six_dof_pose.pose.rw;
                frame.pose.rotation.x = (float)antiDistortionFrameItemExt.six_dof_pose.pose.rx;
                frame.pose.rotation.y = (float)antiDistortionFrameItemExt.six_dof_pose.pose.ry;
                frame.pose.rotation.z = (float)antiDistortionFrameItemExt.six_dof_pose.pose.rz;
            }
            frame.status = antiDistortionFrameItemExt.six_dof_pose.pose.confidence;
            return result;
        }

        public static UnityEngine.Pose ToUnityPose(UnityEngine.Pose poseR)
        {
            UnityEngine.Pose poseL;
            poseL.position.x = poseR.position.x;
            poseL.position.y = poseR.position.y;
            poseL.position.z = -poseR.position.z;
            poseL.rotation.x = poseR.rotation.x;
            poseL.rotation.y = poseR.rotation.y;
            poseL.rotation.z = -poseR.rotation.z;
            poseL.rotation.w = -poseR.rotation.w;
            return poseL;
        }

        // RGB Camera pose （Left-handed coordinate system: X right, Y up, Z out）
        public static UnityEngine.Pose ToRGBCameraPose(RGBCameraParams cameraParams, UnityEngine.Pose headPose)
        {
            Vector3 headToCameraPos = new Vector3((float)cameraParams.x, (float)cameraParams.y, (float)cameraParams.z);
            Quaternion headToCameraRot = new Quaternion((float)cameraParams.rx, (float)cameraParams.ry, (float)cameraParams.rz, (float)cameraParams.rw);

            Matrix4x4 headMx = Matrix4x4.TRS(headPose.position, headPose.rotation, Vector3.one);
            Matrix4x4 cameraMx = Matrix4x4.TRS(headToCameraPos, headToCameraRot, Vector3.one);
            Matrix4x4 rgbMx = headMx * cameraMx;
            Matrix4x4 rotX180 = Matrix4x4.Rotate(Quaternion.Euler(180f, 0f, 0f));
            rgbMx *= rotX180;
#if UNITY_2021_2_OR_NEWER
            UnityEngine.Pose rgbCameraPose = ToUnityPose(new UnityEngine.Pose(rgbMx.GetPosition(), rgbMx.rotation));
#else
            UnityEngine.Pose rgbCameraPose = ToUnityPose(new UnityEngine.Pose(new Vector3(rgbMx.m03, rgbMx.m13, rgbMx.m23), rgbMx.rotation));
#endif
            return rgbCameraPose;
        }

        private static void InitDistortionFrame()
        {
            if (initDistortionFrame)
            {
                return;
            }
            distortionFrameItemExt = new FrameItemExt();
            initDistortionFrame = true;
        }

        private static void InitAntiDistortionFrame(int size)
        {
            if (curSize == size)
            {
                return;
            }
            Debug.LogFormat("InitAntiDistortionFrame curSize={0}, size={1}", curSize, size);
            antiDistortionFrameItemExt = new FrameItemExt();
            if (antiDistortionFrameItemExt.frame.data != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(antiDistortionFrameItemExt.frame.data);
                antiDistortionFrameItemExt.frame.data = IntPtr.Zero;
            }
            antiDistortionFrameItemExt.frame.data = Marshal.AllocHGlobal(size);
            curSize = size;
        }

        public static RGBCameraParams UPxr_GetCameraParameters()
        {
            PLog.d(TAG, "GetCameraParameters Start");
            RGBCameraParams rgbCameraParams = new RGBCameraParams();
            if (string.IsNullOrEmpty(token))
            {
                PLog.e(TAG, "Failed to obtain token, camera data cannot be obtained!");
                return rgbCameraParams;
            }
            int result = getCameraParameters(token, out rgbCameraParams);
            PLog.d(TAG, "GetCameraParameters End result :" + result);

            return rgbCameraParams;
        }

        public static double UPxr_GetPredictedDisplayTime()
        {
            PLog.d(TAG, "UPxr_GetPredictedDisplayTime()");
            double predictedDisplayTime = 0;
#if PICO_PLATFORM
            Pxr_GetPredictedDisplayTime(ref predictedDisplayTime);
#endif
            PLog.d(TAG, "UPxr_GetPredictedDisplayTime() predictedDisplayTime：" + predictedDisplayTime);
            return predictedDisplayTime;
        }

        public static SensorState UPxr_GetPredictedMainSensorState(double predictTime)
        {
            SensorState sensorState = new SensorState();
            PxrSensorState2 sensorState2 = new PxrSensorState2();
            int sensorFrameIndex = 0;
#if PICO_PLATFORM
            Pxr_GetPredictedMainSensorState2(predictTime, ref sensorState2, ref sensorFrameIndex);
#endif
            sensorState.status = sensorState2.status == 3 ? 1 : 0;
            sensorState.pose.position.x = sensorState2.globalPose.position.x;
            sensorState.pose.position.y = sensorState2.globalPose.position.y;
            sensorState.pose.position.z = sensorState2.globalPose.position.z;
            sensorState.pose.rotation.x = sensorState2.globalPose.orientation.x;
            sensorState.pose.rotation.y = sensorState2.globalPose.orientation.y;
            sensorState.pose.rotation.z = sensorState2.globalPose.orientation.z;
            sensorState.pose.rotation.w = sensorState2.globalPose.orientation.w;
            return sensorState;
        }
        
        public static int UPxr_gotoSeeThroughFloorSetting(int ext)
        {
            int value = -1;

#if PICO_PLATFORM
            value = IToBService.Call<int>("gotoSeeThroughFloorSetting",ext);
#endif
            return value;
        }
        public static int UPxr_fileCopy(String srcPath, String dstPath, FileCopyCallback callback)
        {
            int value = -1;
#if PICO_PLATFORM
            value = tobHelper.Call<int>("FileCopy",srcPath,dstPath,callback);
#endif
            return value;
        }
        public static void UPxr_IsMapInEffect(String path, Action<int> callback, int ext)
        {
           
#if PICO_PLATFORM
            tobHelper.Call("isMapInEffect",path,new IntCallback(callback),ext);
#endif
        }
        public static void UPxr_ImportMapByPath(String path, Action<int> callback, int ext)
        {
#if PICO_PLATFORM
            tobHelper.Call("importMapByPath",path,new IntCallback(callback),ext);
#endif
        }
        public static void UPxr_SetWifiP2PDeviceName(String deviceName, Action<int> callback, int ext)
        {
#if PICO_PLATFORM
            tobHelper.Call("setWifiP2PDeviceName",deviceName,new IntCallback(callback),ext);
#endif
        }
        public static String UPxr_GetWifiP2PDeviceName(int ext)
        {
            String value = "";

#if PICO_PLATFORM
            value = IToBService.Call<String>("getWifiP2PDeviceName",ext);
#endif
            return value;
        }

        public static int UPxr_SetScreenBrightness(int brightness, int ext)
        {
            int value = -1;

#if PICO_PLATFORM
            value = IToBService.Call<int>("setScreenBrightness", brightness, ext);
#endif
            return value;
        }
        public static void UPxr_SwitchSystemFunction(int systemFunction, int switchEnum, Action<int> callback, int ext)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsSwitchSystemFunction",systemFunction,switchEnum,new IntCallback(callback),ext);
#endif
        }
        public static int UPxr_SetSystemKeyUsability(int key, int usability)
        {
            int value = -1;

#if PICO_PLATFORM
            value = IToBService.Call<int>("setSystemKeyUsability", key, usability);
#endif
            return value;
        }
        public static int UPxr_SetLauncher(String packageName)
        {
            int value = -1;

#if PICO_PLATFORM
            value = IToBService.Call<int>("setLauncher", packageName);
#endif
            return value;
        }
        public static int UPxr_SetSystemAutoSleepTime(SleepDelayTimeEnum delayTimeEnum)
        {
            int value = -1;

#if PICO_PLATFORM
             value = IToBService.Call<int>("setLauncher", GetEnumType(delayTimeEnum));
#endif
            return value;
        }

        public static int UPxr_OpenTimingStartup(int year, int month, int day, int hour, int minute)
        {
            int value = -1;
#if PICO_PLATFORM
            value =IToBService.Call<int>("openTimingStartup", year, month, day, hour, minute);
#endif
            return value;
        }
        public static int UPxr_CloseTimingStartup()
        {
            int value = -1;
#if PICO_PLATFORM
            value =IToBService.Call<int>("closeTimingStartup");
#endif
            return value;
        }
        public static int UPxr_OpenTimingShutdown(int year, int month, int day, int hour, int minute)
        {
            int value = -1;
#if PICO_PLATFORM
            value =IToBService.Call<int>("openTimingShutdown", year, month, day, hour, minute);
#endif
            return value;
        }
        public static int UPxr_CloseTimingShutdown()
        {
            int value = -1;
#if PICO_PLATFORM
            value =IToBService.Call<int>("closeTimingShutdown");
#endif
            return value;
        }
        public static int UPxr_SetTimeZone(String timeZone)
        {
            int value = -1;

#if PICO_PLATFORM
            value = IToBService.Call<int>("setTimeZone", timeZone);
#endif
            return value;
        }
        public static void UPxr_AppCopyrightVerify(string packageName, Action<int> callback)
        {
#if PICO_PLATFORM
            tobHelper.Call("appCopyrightVerify",packageName,new IntCallback(callback));
#endif
        }
        public static int UPxr_GotoEnvironmentTextureCheck()
        {
            int value = -1;

#if PICO_PLATFORM
            value = IToBService.Call<int>("gotoEnvironmentTextureCheck");
#endif
            return value;
        }
        private const string LibraryName = "PICO_TOBAPI";

        [DllImport(LibraryName,  CallingConvention = CallingConvention.Cdecl)]
        public static extern float oxr_get_trackingorigin_height();
        
    }
}