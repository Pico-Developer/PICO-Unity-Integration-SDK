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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using LitJson;
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
        
        public static bool UPxr_InitEnterpriseService(bool isCamera=false)
        {
#if PICO_PLATFORM
                tobHelperClass = new AndroidJavaClass("com.picoxr.tobservice.ToBServiceUtils");
                tobHelper = tobHelperClass.CallStatic<AndroidJavaObject>("getInstance");
                unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                BAuthLib = new AndroidJavaClass("com.pvr.tobauthlib.AuthCheckServer");
#endif
             return !isCamera || UPxr_GetToken();
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
        public static int UPxr_OpenTimingStartup(int hour, int minute, int repeat)
        {
            int value = -1;
#if PICO_PLATFORM
            value =IToBService.Call<int>("openTimingStartup", hour, minute,repeat);
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
        public static int UPxr_OpenTimingShutdown(int hour, int minute, int repeat)
        {
            int value = -1;
#if PICO_PLATFORM
            value =IToBService.Call<int>("openTimingShutdown",  hour, minute,repeat);
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
        public static int UPxr_SetSystemDate(int year, int month, int day)
        {
            int value = -1;
#if PICO_PLATFORM
            value =IToBService.Call<int>("setSystemDate", year, month, day);
#endif
            return value;
        }
        public static int UPxr_SetSystemTime(int hourOfDay, int minute, int second)
        {
            int value = -1;
#if PICO_PLATFORM
            value =IToBService.Call<int>("setSystemTime", hourOfDay, minute, second);
#endif
            return value;
        }
        public static int UPxr_KeepAliveBackground(int keepAlivePid, int flags, int level)
        {
            int value = -1;
#if PICO_PLATFORM
            value =IToBService.Call<int>("keepAliveBackground", keepAlivePid, flags, level);
#endif
            return value;
        }
        public static int UPxr_OpenIPDDetectionPage()
        {
            int value = -1;
#if PICO_PLATFORM
            value =IToBService.Call<int>("openIPDDetectionPage");
#endif
            return value;
        }
        public static int UPxr_SetFloorHeight(float height)
        {
            int value = -1;
#if PICO_PLATFORM
            value =IToBService.Call<int>("setFloorHeight",height);
#endif
            return value;
        }
        public static float UPxr_GetFloorHeight()
        {
            float value = -1;
#if PICO_PLATFORM
            value =IToBService.Call<float>("getFloorHeight");
#endif
            return value;
        }
        public static String UPxr_GetTimingStartupStatusTwo(int ext)
        {
            String value = "";
#if PICO_PLATFORM
            value =IToBService.Call<String>("pbsGetTimingStartupStatusTwo",ext);
#endif
            return value;
        }
        public static String UPxr_GetTimingShutDownStatusTwo(int ext)
        {
            String value = "";
#if PICO_PLATFORM
            value =IToBService.Call<String>("pbsGetTimingShutDownStatusTwo",ext);
#endif
            return value;
        }
        public static String[] UPxr_GetRunningAppProcesses()
        {
            String[] value = null;
#if PICO_PLATFORM
            value=tobHelper.Call<String[]>("pbsGetRunningAppProcesses");
#endif
            return value;
        }
        
        public static String UPxr_GetFocusedApp()
        {
            String value = "";
#if PICO_PLATFORM
            value = tobHelper.Call<String>("pbsGetFocusedApp");
#endif
            return value;
        }

        

        public static String UPxr_StartService(AndroidJavaObject intent)
        {
            String value = "";
#if PICO_PLATFORM
            value = tobHelper.Call<String>("pbsStartService", intent);
#endif
            return value;
        }
        

        public static String UPxr_StartForegroundService(AndroidJavaObject intent)
        {
            String value = "";
#if PICO_PLATFORM
            value = tobHelper.Call<String>("pbsStartForegroundService", intent);
#endif
            return value;
        }
        
        public static int UPxr_SendBroadcast(AndroidJavaObject intent)
        {
            int value = -1;
#if PICO_PLATFORM
            value = IToBService.Call<int>("sendBroadcast", intent);
#endif
            return value;
        }
        

        public static int UPxr_SendOrderedBroadcast(AndroidJavaObject intent, String receiverPermission)
        {
            int value = -1;
#if PICO_PLATFORM
            value = IToBService.Call<int>("sendOrderedBroadcast", intent,string.IsNullOrEmpty(receiverPermission)?null:receiverPermission);
#endif
            return value;
        }
        public static int UPxr_SetVirtualEnvironment(String envPath)
        {
            int value = -1;
#if PICO_PLATFORM
            value = IToBService.Call<int>("setVirtualEnvironment",string.IsNullOrEmpty(envPath)?null:envPath);
#endif
            return value;
        }
        public static string UPxr_GetVirtualEnvironment()
        {
            string value = "";
#if PICO_PLATFORM
            value = IToBService.Call<string>("getVirtualEnvironment");
#endif
            return value;
        }



        private static IntPtr? _VirtualDisplayPlugin;
        private static IntPtr createVirtualDisplayMethodId;
        private static IntPtr startAppMethodId;
        private static jvalue[] setUnityActivityParams;
        private static IntPtr? _Activity;

        private static IntPtr Activity
        {
            get
            {
                if (!_Activity.HasValue)
                {
                    try
                    {
                        IntPtr unityPlayerClass = AndroidJNI.FindClass("com/unity3d/player/UnityPlayer");
                        IntPtr currentActivityField = AndroidJNI.GetStaticFieldID(unityPlayerClass, "currentActivity",
                            "Landroid/app/Activity;");
                        IntPtr activity = AndroidJNI.GetStaticObjectField(unityPlayerClass, currentActivityField);

                        _Activity = AndroidJNI.NewGlobalRef(activity);

                        AndroidJNI.DeleteLocalRef(activity);
                        AndroidJNI.DeleteLocalRef(unityPlayerClass);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                        _Activity = IntPtr.Zero;
                    }
                }

                return _Activity.GetValueOrDefault();
            }
        }

        private static IntPtr VirtualDisplayPlugin
        {
            get
            {
                if (!_VirtualDisplayPlugin.HasValue)
                {
                    try
                    {
                        IntPtr myClass =
                            AndroidJNI.FindClass("com/picoxr/tobservice/VirtualDisplay/VirtualDisplayPlugin");

                        if (myClass != IntPtr.Zero)
                        {
                            _VirtualDisplayPlugin = AndroidJNI.NewGlobalRef(myClass);

                            AndroidJNI.DeleteLocalRef(myClass);
                        }
                        else
                        {
                            Debug.LogError("Failed to find VirtualDisplayPlugin class");
                            _VirtualDisplayPlugin = IntPtr.Zero;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Failed to find VirtualDisplayPlugin class");
                        Debug.LogException(ex);
                        _VirtualDisplayPlugin = IntPtr.Zero;
                    }
                }

                return _VirtualDisplayPlugin.GetValueOrDefault();
            }
        }

        private static IntPtr setUnityActivityMethodId;

        public static void SetUnityActivity()
        {
            if (setUnityActivityMethodId == System.IntPtr.Zero)
            {
                setUnityActivityMethodId = AndroidJNI.GetStaticMethodID(VirtualDisplayPlugin, "setUnityActivity",
                    "(Landroid/content/Context;)V");
                setUnityActivityParams = new jvalue[1];
            }

            setUnityActivityParams[0].l = Activity;
            AndroidJNI.CallStaticVoidMethod(VirtualDisplayPlugin, setUnityActivityMethodId, setUnityActivityParams);
        }
        private static jvalue[] CVDParams;
        public static int UPxr_CreateVirtualDisplay(string displayName, IntPtr surfaceObj, int width, int height,
            int densityDpi, int flags)
        {
            int value = -1;
#if PICO_PLATFORM
            // SetUnityActivity();
            if (createVirtualDisplayMethodId == System.IntPtr.Zero)
            {
                createVirtualDisplayMethodId = AndroidJNI.GetStaticMethodID(VirtualDisplayPlugin,
                    "CreateVirtualDisplay", "(Ljava/lang/String;Landroid/view/Surface;IIII)I");
                CVDParams = new jvalue[6];
            }

            IntPtr displayNameString = AndroidJNI.NewStringUTF(displayName);

            CVDParams[0].l = displayNameString;
            CVDParams[1].l = surfaceObj;
            CVDParams[2].i = width;
            CVDParams[3].i = height;
            CVDParams[4].i = densityDpi;
            CVDParams[5].i = flags;
            value = AndroidJNI.CallStaticIntMethod(VirtualDisplayPlugin, createVirtualDisplayMethodId, CVDParams);

            AndroidJNI.DeleteLocalRef(displayNameString);
#endif
            return value;
        }
        private static jvalue[] SAParams;
        public static int UPxr_StartApp(int displayId, AndroidJavaObject intent)
        {
            int value = -1;
#if PICO_PLATFORM
            if (startAppMethodId == IntPtr.Zero)
            {
                startAppMethodId =
                    AndroidJNI.GetStaticMethodID(VirtualDisplayPlugin, "StartApp", "(ILandroid/content/Intent;)I");
                SAParams = new jvalue[2];
            }

            SAParams[0].i = displayId;
            SAParams[1].l = intent.GetRawObject();
            value = AndroidJNI.CallStaticIntMethod(VirtualDisplayPlugin, startAppMethodId, SAParams);

#endif
            return value;
        }
        private static IntPtr releaseVirtualDisplayMethodId;
        private static jvalue[] RVDParams;
        public static int UPxr_ReleaseVirtualDisplay(int displayId)
        {
            int value = -1;
#if PICO_PLATFORM
            if (releaseVirtualDisplayMethodId == IntPtr.Zero)
            {
                releaseVirtualDisplayMethodId =
                    AndroidJNI.GetStaticMethodID(VirtualDisplayPlugin, "ReleaseVirtualDisplay", "(I)I");
                RVDParams = new jvalue[1];
            }

            RVDParams[0].i = displayId;
            value = AndroidJNI.CallStaticIntMethod(VirtualDisplayPlugin, releaseVirtualDisplayMethodId, RVDParams);

#endif
            return value;
        }
        private static IntPtr setVirtualDisplaySurfaceMethodId;
        private static jvalue[] SFParams;
        public static int UPxr_SetVirtualDisplaySurface(int displayId, IntPtr surfaceObj)
        {
            int value = -1;
#if PICO_PLATFORM
            if (setVirtualDisplaySurfaceMethodId == IntPtr.Zero)
            {
                setVirtualDisplaySurfaceMethodId = AndroidJNI.GetStaticMethodID(VirtualDisplayPlugin,
                    "SetVirtualDisplaySurface", "(ILandroid/view/Surface;)I");
                SFParams = new jvalue[2];
            }
            
            SFParams[0].i = displayId;
            SFParams[1].l = surfaceObj;
           
            value = AndroidJNI.CallStaticIntMethod(VirtualDisplayPlugin, setVirtualDisplaySurfaceMethodId, SFParams);
#endif
            return value;
        }
        private static IntPtr injectEventMMethodId;
        private static jvalue[] JEMParams;
        public static int UPxr_InjectEvent(int displayId, int action, int source, float x, float y)
        {
            int value = -1;
#if PICO_PLATFORM
            if (injectEventMMethodId == IntPtr.Zero)
            {
                injectEventMMethodId =
                    AndroidJNI.GetStaticMethodID(VirtualDisplayPlugin, "InjectEvent", "(IIIFF)I");
                JEMParams = new jvalue[5];
            }
           
            JEMParams[0].i = displayId;
            JEMParams[1].i = action;
            JEMParams[2].i = source;
            JEMParams[3].f = x;
            JEMParams[4].f = y;
           
            value = AndroidJNI.CallStaticIntMethod(VirtualDisplayPlugin, injectEventMMethodId, JEMParams);

#endif
            return value;
        }
        private static IntPtr injectEventKMethodId;
        private static jvalue[] JEParams;
        public static int UPxr_InjectEvent(int displayId, int action, int source, int keycode)
        {
            int value = -1;
#if PICO_PLATFORM
            if (injectEventKMethodId == IntPtr.Zero)
            {
                injectEventKMethodId =
                    AndroidJNI.GetStaticMethodID(VirtualDisplayPlugin, "InjectEvent", "(IIII)I");
                JEParams = new jvalue[4];
            }

            JEParams[0].i = displayId;
            JEParams[1].i = action;
            JEParams[2].i = source;
            JEParams[3].i = keycode;
           
            value = AndroidJNI.CallStaticIntMethod(VirtualDisplayPlugin, injectEventKMethodId, JEParams);

#endif
            return value;
        }
        private static IntPtr resizeVirtualDisplayMethodId;
        private static jvalue[] RVParams;
        public static int UPxr_ResizeVirtualDisplay(int displayId, int width, int height, int densityDpi)
        {
            int value = -1;
#if PICO_PLATFORM
            if (resizeVirtualDisplayMethodId == IntPtr.Zero)
            {
                resizeVirtualDisplayMethodId =
                    AndroidJNI.GetStaticMethodID(VirtualDisplayPlugin, "ResizeVirtualDisplay", "(IIII)I");
                RVParams = new jvalue[4];
            }

            RVParams[0].i = displayId;
            RVParams[1].i = width;
            RVParams[2].i = height;
            RVParams[3].i = densityDpi;
           
            value = AndroidJNI.CallStaticIntMethod(VirtualDisplayPlugin, resizeVirtualDisplayMethodId, RVParams);

#endif
            return value;
        }

        public static int UPxr_ShowGlobalMessageDialog(Texture2D icon, String title, String body, long time, int gap,
            int position)
        {
            int value = 1;
#if PICO_PLATFORM
            if (icon == null)
            {
                value = tobHelper.Call<int>("pbsShowGlobalMessageDialog", null, 0, 0, title, body, time, gap, position);
            }
            else
            {
                Color[] colors = icon.GetPixels();
                // Color[] colors=  new Color[icon.width * icon.height];
                int[] colorint = new int[icon.width * icon.height * 4];
                // for (int i = 0; i < colors.Length; i++)
                // {
                //     colors[i]=Color.red;
                // }
                for (int i = 0; i < colors.Length; i++)
                {
                    colorint[4 * i] = (int)(colors[i].a * 255);
                    colorint[4 * i + 1] = (int)(colors[i].r * 255);
                    colorint[4 * i + 2] = (int)(colors[i].g * 255);
                    colorint[4 * i + 3] = (int)(colors[i].b * 255);
                }

                value = tobHelper.Call<int>("pbsShowGlobalMessageDialog", colorint, icon.width, icon.height, title,
                    body, time, gap, position);
            }
#endif
            return value;
        }

        public static Point3D[] UPxr_GetLargeSpaceBoundsInfo()
        {
            String[] value = null;
            List<Point3D> ModelList = new List<Point3D>();
#if PICO_PLATFORM
            value = tobHelper.Call<String[]>("pbsGetLargeSpaceBoundsInfo");
            // Point3D[] value1 = IToBService.Call<Point3D[]>("getLargeSpaceBoundsInfo");
            foreach (var json in value)
            {
                JsonData jsonData = JsonMapper.ToObject(json);
                Point3D model = new Point3D();
                model.x = double.Parse(jsonData["x"].ToString());
                model.y = double.Parse(jsonData["y"].ToString());
                model.z = double.Parse(jsonData["z"].ToString());
                ModelList.Add(model);
           
            }
#endif
            return ModelList.ToArray();
        }
        public static void UPxr_OpenLargeSpaceQuickMode(int length, int width, int originType, bool openVst, float distance, int timeout, Action<int> callback)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsOpenLargeSpaceQuickMode",length,width,originType,openVst,distance,timeout,new IntCallback(callback));
#endif
        }
        public static void UPxr_CloseLargeSpaceQuickMode()
        {
#if PICO_PLATFORM
            IToBService.Call("closeLargeSpaceQuickMode");
#endif
        }
        public static void UPxr_SetOriginOfLargeSpaceQuickMode(int originType, bool openVst, float distance, int timeout, Action<int> callback)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsSetOriginOfLargeSpaceQuickMode",originType,openVst,distance,timeout,new IntCallback(callback));
#endif
        }
        public static void UPxr_SetBoundaryOfLargeSpaceQuickMode(int length, int width,Action<int> callback)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsSetBoundaryOfLargeSpaceQuickMode",length,width,new IntCallback(callback));
#endif
        }
        public static LargeSpaceQuickModeInfo UPxr_GetLargeSpaceQuickModeInfo()
        {
            String value = "";
            LargeSpaceQuickModeInfo model = new LargeSpaceQuickModeInfo();
#if PICO_PLATFORM
            value = tobHelper.Call<String>("pbsGetLargeSpaceQuickModeInfo");
            JsonData jsonData = JsonMapper.ToObject(value);
            model.length= int.Parse(jsonData["length"].ToString());
            model.width = int.Parse(jsonData["width"].ToString());
            model.originType = int.Parse(jsonData["originType"].ToString());
            model.status = bool.Parse(jsonData["status"].ToString());
#endif
            return model;
        }
        
        public static int UPxr_StartLeftControllerPair()
        {
            int value = 1;
#if PICO_PLATFORM
            value=IToBService.Call<int>("startLeftControllerPair");
#endif
            return value;
        }
        public static int UPxr_MakeLeftControllerUnPair()
        {
            int value = 1;
#if PICO_PLATFORM
            value=IToBService.Call<int>("makeLeftControllerUnPair");
#endif
            return value;
        }
        public static int UPxr_StartRightControllerPair()
        {
            int value = 1;
#if PICO_PLATFORM
            value=IToBService.Call<int>("startRightControllerPair");
#endif
            return value;
        }
        public static int UPxr_MakeRightControllerUnPair()
        {
            int value = 1;
#if PICO_PLATFORM
            value=IToBService.Call<int>("makeRightControllerUnPair");
#endif
            return value;
        }
        
        public static int UPxr_StopControllerPair()
        {
            int value = 1;
#if PICO_PLATFORM
            value=IToBService.Call<int>("stopControllerPair");
#endif
            return value;
        }
        public static int UPxr_SetControllerPreferHand(bool isLeft)
        {
            int value = 1;
#if PICO_PLATFORM
            value=IToBService.Call<int>("setControllerPreferHand",isLeft);
#endif
            return value;
        }
        public static int UPxr_SetControllerVibrateAmplitude(int value)
        {
            int value1 = 1;
#if PICO_PLATFORM
            value1=IToBService.Call<int>("setControllerVibrateAmplitude",value);
#endif
            return value1;
        }
        public static int UPxr_SetPowerManageMode(int value)
        {
            int value1 = 1;
#if PICO_PLATFORM
            value1=IToBService.Call<int>("setPowerManageMode",value);
#endif
            return value1;
        }
        public static int UPxr_StartRoomMark()
        {
            int value1 = 1;
#if PICO_PLATFORM
            value1=IToBService.Call<int>("startRoomMark");
#endif
            return value1;
        }
        public static int UPxr_ClearRoomMark()
        {
            int value1 = 1;
#if PICO_PLATFORM
            value1=IToBService.Call<int>("clearRoomMark");
#endif
            return value1;
        }
        public static int UPxr_ClearEyeTrackData()
        {
            int value1 = 1;
#if PICO_PLATFORM
            value1=IToBService.Call<int>("clearEyeTrackData");
#endif
            return value1;
        }
        public static int UPxr_SetEyeTrackRate(int value)
        {
            int value1 = 1;
#if PICO_PLATFORM
            value1=IToBService.Call<int>("setEyeTrackRate",value);
#endif
            return value1;
        }
        public static int UPxr_SetTrackFrequency(int value)
        {
            int value1 = 1;
#if PICO_PLATFORM
            value1=IToBService.Call<int>("setTrackFrequency",value);
#endif
            return value1;
        }
        public static int UPxr_StartSetSecureBorder()
        {
            int value1 = 1;
#if PICO_PLATFORM
            value1=IToBService.Call<int>("startSetSecureBorder");
#endif
            return value1;
        }
        public static int UPxr_SetDistanceSensitivity(int value)
        {
            int value1 = 1;
#if PICO_PLATFORM
            value1=IToBService.Call<int>("setDistanceSensitivity",value);
#endif
            return value1;
        }
        public static int UPxr_SetSpeedSensitivity(int value)
        {
            int value1 = 1;
#if PICO_PLATFORM
            value1=IToBService.Call<int>("setSpeedSensitivity",value);
#endif
            return value1;
        }
        public static int UPxr_SetMotionTrackerPredictionCoefficient(float value)
        {
            int value1 = 1;
#if PICO_PLATFORM
            value1=IToBService.Call<int>("setMotionTrackerPredictionCoefficient",value);
#endif
            return value1;
        }
        public static float UPxr_GetMotionTrackerPredictionCoefficient()
        {
            float value1 = -1;
#if PICO_PLATFORM
            value1=IToBService.Call<float>("getMotionTrackerPredictionCoefficient");
#endif
            return value1;
        }
        public static int UPxr_StartMotionTrackerApp(int failMode, int avatarMode)
        {
            int value1 = 1;
#if PICO_PLATFORM
            value1=IToBService.Call<int>("startMotionTrackerApp",failMode,avatarMode);
#endif
            return value1;
        }
        public static int UPxr_SetSingleEyeSource(bool isLeft)
        {
            int value1 = 1;
#if PICO_PLATFORM
            value1=IToBService.Call<int>("setSingleEyeSource",isLeft);
#endif
            return value1;
        }
        public static int UPxr_SetViewVisual(int value)
        {
            int value1 = 1;
#if PICO_PLATFORM
            value1=IToBService.Call<int>("setViewVisual",value);
#endif
            return value1;
        }
        public static int UPxr_SetAcceptCastMode(int value)
        {
            int value1 = 1;
#if PICO_PLATFORM
            value1=IToBService.Call<int>("setAcceptCastMode",value);
#endif
            return value1;
        }
        public static int UPxr_SetScreenCastMode(int value)
        {
            int value1 = 1;
#if PICO_PLATFORM
            value1=IToBService.Call<int>("setScreenCastMode",value);
#endif
            return value1;
        }
        public static int UPxr_SetScreenRecordShotRatio(int value)
        {
            int value1 = 1;
#if PICO_PLATFORM
            value1=IToBService.Call<int>("setScreenRecordShotRatio",value);
#endif
            return value1;
        }
        public static int UPxr_SetScreenResolution(int width, int height)
        {
            int value1 = 1;
#if PICO_PLATFORM
            value1=IToBService.Call<int>("setScreenResolution",width,height);
#endif
            return value1;
        }
        public static int UPxr_SetScreenRecordFrameRate(int value)
        {
            int value1 = 1;
#if PICO_PLATFORM
            value1=IToBService.Call<int>("setScreenRecordFrameRate",value);
#endif
            return value1;
        }
        public static void UPxr_HideGlobalMessageDialog()
        {
#if PICO_PLATFORM
            IToBService.Call("hideGlobalMessageDialog");
#endif
        }
        public static int UPxr_ShowGlobalTipsDialog(Texture2D icon, String title, long time, int position, int bgColor)
        {
            int value = 1;
#if PICO_PLATFORM
            if (icon == null)
            {
                value = tobHelper.Call<int>("pbsShowGlobalTipsDialog", null, 0, 0, title, time, position, bgColor);
            }
            else
            {
                Color[] colors = icon.GetPixels();
                // Color[] colors=  new Color[icon.width * icon.height];
                int[] colorint = new int[icon.width * icon.height * 4];
                // for (int i = 0; i < colors.Length; i++)
                // {
                //     colors[i]=Color.red;
                // }
                for (int i = 0; i < colors.Length; i++)
                {
                    colorint[4 * i] = (int)(colors[i].a * 255);
                    colorint[4 * i + 1] = (int)(colors[i].r * 255);
                    colorint[4 * i + 2] = (int)(colors[i].g * 255);
                    colorint[4 * i + 3] = (int)(colors[i].b * 255);
                }

                value = tobHelper.Call<int>("pbsShowGlobalTipsDialog", colorint, icon.width, icon.height,  title, time, position, bgColor);
            }
#endif
            return value;
        }
        public static void UPxr_HideGlobalTipsDialog()
        {
#if PICO_PLATFORM
            IToBService.Call("hideGlobalTipsDialog");
#endif
        }
        public static int UPxr_ShowGlobalBigStatusDialog(Texture2D icon,String title, String body, long time, int gap, int position)
        {
            int value = 1;
#if PICO_PLATFORM
            if (icon == null)
            {
                value = tobHelper.Call<int>("pbsShowGlobalBigStatusDialog", null, 0, 0, title,body, time,gap, position);
            }
            else
            {
                Color[] colors = icon.GetPixels();
                // Color[] colors=  new Color[icon.width * icon.height];
                int[] colorint = new int[icon.width * icon.height * 4];
                // for (int i = 0; i < colors.Length; i++)
                // {
                //     colors[i]=Color.red;
                // }
                for (int i = 0; i < colors.Length; i++)
                {
                    colorint[4 * i] = (int)(colors[i].a * 255);
                    colorint[4 * i + 1] = (int)(colors[i].r * 255);
                    colorint[4 * i + 2] = (int)(colors[i].g * 255);
                    colorint[4 * i + 3] = (int)(colors[i].b * 255);
                }

                value = tobHelper.Call<int>("pbsShowGlobalBigStatusDialog", colorint, icon.width, icon.height,  title,body, time,gap, position);
            }
#endif
            return value;
        }
        public static void UPxr_HideGlobalBigStatusDialog()
        {
#if PICO_PLATFORM
            IToBService.Call("hideGlobalBigStatusDialog");
#endif
        }
        public static int UPxr_ShowGlobalSmallStatusDialog(Texture2D icon,String title,  long time, int gap, int position)
        {
            int value = 1;
#if PICO_PLATFORM
            if (icon == null)
            {
                value = tobHelper.Call<int>("pbsShowGlobalSmallStatusDialog", null, 0, 0, title, time,gap, position);
            }
            else
            {
                Color[] colors = icon.GetPixels();
                // Color[] colors=  new Color[icon.width * icon.height];
                int[] colorint = new int[icon.width * icon.height * 4];
                // for (int i = 0; i < colors.Length; i++)
                // {
                //     colors[i]=Color.red;
                // }
                for (int i = 0; i < colors.Length; i++)
                {
                    colorint[4 * i] = (int)(colors[i].a * 255);
                    colorint[4 * i + 1] = (int)(colors[i].r * 255);
                    colorint[4 * i + 2] = (int)(colors[i].g * 255);
                    colorint[4 * i + 3] = (int)(colors[i].b * 255);
                }

                value = tobHelper.Call<int>("pbsShowGlobalSmallStatusDialog", colorint, icon.width, icon.height,  title,time,gap, position);
            }
#endif
            return value;
        }
        public static void UPxr_HideGlobalSmallStatusDialog()
        {
#if PICO_PLATFORM
            IToBService.Call("hideGlobalSmallStatusDialog");
#endif
        }
        
        public static int UPxr_ShowGlobalDialogByType(String type,Texture2D icon,String title, String body, long time, int gap, int position, int bgColor)
        {
            int value = 1;
#if PICO_PLATFORM
            if (icon == null)
            {
                value = tobHelper.Call<int>("pbsShowGlobalDialogByType", type,null, 0, 0, title,body, time,gap, position,bgColor);
            }
            else
            {
                Color[] colors = icon.GetPixels();
                // Color[] colors=  new Color[icon.width * icon.height];
                int[] colorint = new int[icon.width * icon.height * 4];
                // for (int i = 0; i < colors.Length; i++)
                // {
                //     colors[i]=Color.red;
                // }
                for (int i = 0; i < colors.Length; i++)
                {
                    colorint[4 * i] = (int)(colors[i].a * 255);
                    colorint[4 * i + 1] = (int)(colors[i].r * 255);
                    colorint[4 * i + 2] = (int)(colors[i].g * 255);
                    colorint[4 * i + 3] = (int)(colors[i].b * 255);
                }

                value = tobHelper.Call<int>("pbsShowGlobalDialogByType", type,colorint, icon.width, icon.height,  title,body, time,gap, position,bgColor);
            }
#endif
            return value;
        }
        public static void UPxr_HideGlobalDialogByType(String type)
        {
#if PICO_PLATFORM
            IToBService.Call("hideGlobalDialogByType",type);
#endif
        }

        public static int UPxr_Recenter()
        {
            int value = 1;
#if PICO_PLATFORM
            value= IToBService.Call<int>("recenter");
#endif
            return value;
        }

        public static void UPxr_ScanQRCode(Action<string> callback)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsScanQRCode", new StringCallback(callback));
#endif
        }

        public static int UPxr_OnlineSystemUpdate(SystemUpdateCallback callback)
        {
            int value = -1;
#if PICO_PLATFORM
            value = tobHelper.Call<int>("pbsSonlineSystemUpdate", callback);
#endif
            return value;
        }
        public static int UPxr_OfflineSystemUpdate(OffLineSystemUpdateConfig systemUpdateConfig,SystemUpdateCallback callback)
        {
            int value = -1;
#if PICO_PLATFORM
            if (systemUpdateConfig != null)
            {
                if (string.IsNullOrEmpty(systemUpdateConfig.otaFilePath))
                {
                   Debug.LogError("systemUpdateConfig.otaFilePath is null");
                }
                else
                {
                    value = tobHelper.Call<int>("pbsOfflineSystemUpdate", systemUpdateConfig.otaFilePath, systemUpdateConfig.autoReboot, systemUpdateConfig.showProgress, callback);
                }
               
            }
            else
            {
                Debug.LogError("systemUpdateConfig is null");
            }
#endif
            return value;
        }
        public static int UPxr_GetControllerVibrateAmplitude()
        {
            int value = 1;
#if PICO_PLATFORM
            value= IToBService.Call<int>("getControllerVibrateAmplitude");
#endif
            return value;
        }
        public static int UPxr_SetHMDVolumeKeyFunc(int func)
        {
            int value = 1;
#if PICO_PLATFORM
            value= IToBService.Call<int>("setHMDVolumeKeyFunc",func);
#endif
            return value;
        }
        public static int UPxr_GetHMDVolumeKeyFunc()
        {
            int value = 1;
#if PICO_PLATFORM
            value= IToBService.Call<int>("getHMDVolumeKeyFunc");
#endif
            return value;
        }
        public static int UPxr_GetPowerManageMode()
        {
            int value = 1;
#if PICO_PLATFORM
            value= IToBService.Call<int>("getPowerManageMode");
#endif
            return value;
        }
        public static int UPxr_GetEyeTrackRate()
        {
            int value = 1;
#if PICO_PLATFORM
            value= IToBService.Call<int>("getEyeTrackRate");
#endif
            return value;
        }
        public static int UPxr_GetTrackFrequency()
        {
            int value = 1;
#if PICO_PLATFORM
            value= IToBService.Call<int>("getTrackFrequency");
#endif
            return value;
        }
        public static int UPxr_GetDistanceSensitivity()
        {
            int value = 1;
#if PICO_PLATFORM
            value= IToBService.Call<int>("getDistanceSensitivity");
#endif
            return value;
        }
        public static int UPxr_GetSpeedSensitivity()
        {
            int value = 1;
#if PICO_PLATFORM
            value= IToBService.Call<int>("getSpeedSensitivity");
#endif
            return value;
        }
        public static int UPxr_SetMRCollisionAlertSensitivity(float v)
        {
            int value = 1;
#if PICO_PLATFORM
            value= IToBService.Call<int>("setMRCollisionAlertSensitivity",v);
#endif
            return value;
        }
        public static float UPxr_GetMRCollisionAlertSensitivity()
        {
            float value = 1;
#if PICO_PLATFORM
            value= IToBService.Call<float>("getMRCollisionAlertSensitivity");
#endif
            return value;
        }
        public static void UPxr_ConnectWifi(WifiConfiguration configuration,Action<int> callback)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsConnectWifi", configuration.ssid,configuration.password,configuration.isClient,new IntCallback(callback));
#endif
        }
        public static void UPxr_SetStaticIpConfigurationtoConnectWifi(WifiConfiguration configuration,string staticIP,string subnet_mask,string gateway,string[] DNS,Action<int> callback)
        {
#if PICO_PLATFORM
            tobHelper.Call("pbsSetStaticIpConfigurationtoConnectWifi", configuration.ssid,configuration.password,staticIP,subnet_mask,gateway,DNS,new IntCallback(callback));
#endif
        }
        public static int UPxr_GetSingleEyeSource()
        {
            int value = 1;
#if PICO_PLATFORM
            value= IToBService.Call<int>("getSingleEyeSource");
#endif
            return value;
        }
        public static int UPxr_GetViewVisual()
        {
            int value = 1;
#if PICO_PLATFORM
            value= IToBService.Call<int>("getViewVisual");
#endif
            return value;
        }
        public static int UPxr_GetAcceptCastMode()
        {
            int value = 1;
#if PICO_PLATFORM
            value= IToBService.Call<int>("getAcceptCastMode");
#endif
            return value;
        }
        public static int UPxr_GetScreenCastMode()
        {
            int value = 1;
#if PICO_PLATFORM
            value= IToBService.Call<int>("getScreenCastMode");
#endif
            return value;
        }
      
        public static int UPxr_GetScreenRecordShotRatio()
        {
            int value = 1;
#if PICO_PLATFORM
            value= IToBService.Call<int>("getScreenRecordShotRatio");
#endif
            return value;
        }
        public static int[] UPxr_GetScreenResolution()
        {
            int[] value = new[] {-2, -2}; 
#if PICO_PLATFORM
            value= IToBService.Call<int[]>("getScreenResolution");
#endif
            return value;
        }
        public static int UPxr_GetScreenRecordFrameRate()
        {
            int value = 1;
#if PICO_PLATFORM
            value= IToBService.Call<int>("getScreenRecordFrameRate");
#endif
            return value;
        }
    }
   
}