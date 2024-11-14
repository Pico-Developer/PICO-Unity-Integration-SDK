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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Unity.XR.PICO.TOBSupport
{
    /**
     * Enterprise APIs are only supported by enterprise devices, including PICO Neo2, Neo2 Eye, Neo3 Pro、Neo3 Pro Eye, G2 4K/4K E/4K Plus (system version 4.0.3 or later), and PICO 4 Enterprise.
     * Do not use them on consumer devices.
     */
    public class PXR_Enterprise
    {
        /// <summary>
        /// Initializes the enterprise service for a specified object. Must be called before calling other enterprise APIs.
        /// </summary>
        /// <param name="isCamera">Whether to enable video seethrough:
        /// * `true`: enable
        /// * `false`: disable
        /// `false` is the default value if you do not specify any.
        /// </param>
        /// <returns>Whether the enterprise service has been initialized:
        /// * `true`: success
        /// * `false`: failure
        public static bool InitEnterpriseService(bool isCamera=false)
        {
            PXR_EnterpriseTools.Instance.StartUp();
            bool result = PXR_EnterprisePlugin.UPxr_InitEnterpriseService(isCamera);
            return result;
        }

        /// <summary>
        /// Binds the enterprise service. Must be called before calling other system related functions.
        /// </summary>
        /// <param name="callback">
        /// Service-binding result callback that allows for bool values:
        /// * `true`: success
        /// * `false`: failure
        /// If no callback is specified, the parameter will default to null.
        /// </param>
        public static void BindEnterpriseService(Action<bool> callback=null)
        {
            PXR_EnterprisePlugin.UPxr_BindEnterpriseService(callback);
        }

        /// <summary>
        /// Unbinds the enterprise service.
        /// </summary>
        public static void UnBindEnterpriseService()
        {
            PXR_EnterprisePlugin.UPxr_UnBindEnterpriseService();
        }
        
        /// <summary>
        /// Gets the specified type of device information.
        /// </summary>
        /// <param name="type">The target information type. Enumerations:
        /// * `ELECTRIC_QUANTITY`: battery
        /// * `PUI_VERSION`: system version
        /// * `EQUIPMENT_MODEL`: device model
        /// * `EQUIPMENT_SN`: device SN code
        /// * `CUSTOMER_SN`: customer SN code
        /// * `INTERNAL_STORAGE_SPACE_OF_THE_DEVICE`: device storage
        /// * `DEVICE_BLUETOOTH_STATUS`: bluetooth status
        /// * `BLUETOOTH_NAME_CONNECTED`: bluetooth name
        /// * `BLUETOOTH_MAC_ADDRESS`: bluetooth MAC address
        /// * `DEVICE_WIFI_STATUS`: Wi-Fi connection status
        /// * `WIFI_NAME_CONNECTED`: connected Wi-Fi name
        /// * `WLAN_MAC_ADDRESS`: WLAN MAC address
        /// * `DEVICE_IP`: device IP address
        /// * `CHARGING_STATUS`: device charging status
        /// * `BLUETOOTH_INFO_DEVICE`: information about the device's original bluetooth
        /// * `BLUETOOTH_INFO_CONNECTED`: information about the bluetooth connected
        /// * `CAMERA_TEMPERATURE_CELSIUS`: camera's temperature in Celsius
        /// * `CAMERA_TEMPERATURE_FAHRENHEIT`: camera's temperature in Fahrenheit
        /// * `LARGESPACE_MAP_INFO`: large space map information
        /// </param>
        /// <param name="ext">Reserved parameter. Default to `0`.</param>
        /// <returns>The specified type of device information. For `CHARGING_STATUS`, an int value will be returned: `2`-charging; `3`-not charging.</returns>
        public static string StateGetDeviceInfo(SystemInfoEnum type, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_StateGetDeviceInfo(type,ext);
        }

        /// <summary>
        /// Shuts down or reboots the device.
        /// @note This is a protected API. You need to add `<meta-data android:name="pico_advance_interface" android:value="0"/>`
        /// to the app's AndroidManifest.xml file for calling this API, after which the app is unable to be published on the PICO Store.
        /// </summary>
        /// <param name="deviceControl">Device action. Enumerations:
        /// * `DEVICE_CONTROL_REBOOT`
        /// * `DEVICE_CONTROL_SHUTDOWN`
        /// </param>
        /// <param name="callback">Callback:
        /// * `1`: failed to shut down or reboot the device
        /// * `2`: no permission to perform this operation
        /// </param>
        public static void ControlSetDeviceAction(DeviceControlEnum deviceControl, Action<int> callback)
        {
            PXR_EnterprisePlugin.UPxr_ControlSetDeviceAction(deviceControl, callback);
        }

        /// <summary>
        /// Installs or uninstalls app silently.
        /// @note This is a protected API. You need to add `<meta-data android:name="pico_advance_interface" android:value="0"/>`
        /// to the app's AndroidManifest.xml file for calling this API, after which the app is unable to be published on the PICO Store.
        /// </summary>
        /// <param name="packageControl">The action. Enumerations:
        /// * `PACKAGE_SILENCE_INSTALL`: silent installation
        /// * `PACKAGE_SILENCE_UNINSTALL`: silent uninstallation
        /// </param>
        /// <param name="path">The path to the app package for silent installation or the name of the app package for silent uninstallation.</param>
        /// <param name="callback">Callback:
        /// * `0`: success
        /// * `1`: failure
        /// * `2`: no permission to perform this operation
        /// </param>
        /// <param name="ext">Reserved parameter. Default to `0`.</param>
        public static void ControlAPPManager(PackageControlEnum packageControl, string path, Action<int> callback, int ext=0)
        {
            PXR_EnterprisePlugin.UPxr_ControlAPPManager(packageControl, path, callback,ext);
        }

        /// <summary>
        /// Sets a Wi-Fi that the device is automatically connected to.
        /// </summary>
        /// <param name="ssid">Wi-Fi name.</param>
        /// <param name="pwd">Wi-Fi password.</param>
        /// <param name="callback">Callback:
        /// * `true`: connected
        /// * `false`: failed to connect
        /// </param>
        /// <param name="ext">Reserved parameter. Default to `0`.</param>
        public static void ControlSetAutoConnectWIFI(string ssid, string pwd, Action<bool> callback, int ext=0)
        {
            PXR_EnterprisePlugin.UPxr_ControlSetAutoConnectWIFI(ssid, pwd, callback,ext);
        }

        /// <summary>
        /// Removes the Wi-Fi that the device is automatically connected to.
        /// </summary>
        /// <param name="callback">Callback:
        /// * `true`: removed
        /// * `false`: failed to remove
        /// </param>
        public static void ControlClearAutoConnectWIFI(Action<bool> callback)
        {
            PXR_EnterprisePlugin.UPxr_ControlClearAutoConnectWIFI(callback);
        }

        /// <summary>
        /// Sets the Home button event.
        /// </summary>
        /// <param name="eventEnum">Target event. Enumerations:
        /// * `SINGLE_CLICK`: single-click
        /// * `DOUBLE_CLICK`: double-click
        /// * `LONG_PRESS`: long press
        /// * `SINGLE_CLICK_RIGHT_CTL`: single-click on the right controller's Home button
        /// * `DOUBLE_CLICK_RIGHT_CTL`: double-click on the right controller's Home button
        /// * `LONG_PRESS_RIGHT_CTL`: long press on the right controller's Home button
        /// * `SINGLE_CLICK_LEFT_CTL`: single-click on the left controller's Home button
        /// * `DOUBLE_CLICK_LEFT_CTL`: double-click on the left controller's Home button
        /// * `LONG_PRESS_LEFT_CTL`: long press on the left controller's Home button
        /// * `SINGLE_CLICK_HMD`: single-click on the HMD's Home button
        /// * `DOUBLE_CLICK_HMD`: double-click on the HMD's Home button
        /// * `LONG_PRESS_HMD`: long press on the HMD's Home button
        /// </param>
        /// <param name="function">The function of the event. Enumerations:
        /// * `VALUE_HOME_GO_TO_SETTING`: go to Settings
        /// * `VALUE_HOME_BACK`: back (only supported by PICO G2 4K)
        /// * `VALUE_HOME_RECENTER`: recenter the screen
        /// * `VALUE_HOME_OPEN_APP`: open a specified app
        /// * `VALUE_HOME_DISABLE`: disable the Home button
        /// * `VALUE_HOME_GO_TO_HOME`: open the launcher
        /// * `VALUE_HOME_SEND_BROADCAST`: send Home-button-click broadcast
        /// * `VALUE_HOME_CLEAN_MEMORY`: clear background apps
        /// * `VALUE_HOME_QUICK_SETTING`: enable quick settings
        /// * `VALUE_HOME_SCREEN_CAP`: enable screen capture
        /// * `VALUE_HOME_SCREEN_RECORD`: enable screen recording
        /// </param>
        /// <param name="callback">Callback:
        /// * `true`: success
        /// * `false`: failure
        /// </param>
        public static void PropertySetHomeKey(HomeEventEnum eventEnum, HomeFunctionEnum function, Action<bool> callback)
        {
            PXR_EnterprisePlugin.UPxr_PropertySetHomeKey(eventEnum, function, callback);
        }

        /// <summary>
        /// Sets extended settings for the Home button.
        /// </summary>
        /// <param name="eventEnum">Target event. Enumerations:
        /// * `SINGLE_CLICK_RIGHT_CTL`: single-click on the right controller's Home button
        /// * `DOUBLE_CLICK_RIGHT_CTL`: double-click on the right controller's Home button
        /// * `LONG_PRESS_RIGHT_CTL`: long press on the right controller's Home button
        /// * `SINGLE_CLICK_LEFT_CTL`: single-click on the left controller's Home button
        /// * `DOUBLE_CLICK_LEFT_CTL`: double-click on the left controller's Home button
        /// * `LONG_PRESS_LEFT_CTL`: long press on the left controller's Home button
        /// * `SINGLE_CLICK_HMD`: single-click on the HMD's Home button
        /// * `DOUBLE_CLICK_HMD`: double-click on the HMD's Home button
        /// * `LONG_PRESS_HMD`: long press on the HMD's Home button
        /// </param>
        /// <param name="function">The function of the event. Enumerations:
        /// * `VALUE_HOME_GO_TO_SETTING`: go to Settings
        /// * `VALUE_HOME_BACK`: back (only supported by PICO G2 4K)
        /// * `VALUE_HOME_RECENTER`: recenter the screen
        /// * `VALUE_HOME_OPEN_APP`: open a specified app
        /// * `VALUE_HOME_DISABLE`: disable the Home button
        /// * `VALUE_HOME_GO_TO_HOME`: open the launcher
        /// * `VALUE_HOME_SEND_BROADCAST`: send Home-key-click broadcast
        /// * `VALUE_HOME_CLEAN_MEMORY`: clear background apps
        /// * `VALUE_HOME_QUICK_SETTING`: enable quick settings
        /// * `VALUE_HOME_SCREEN_CAP`: enable screen capture
        /// * `VALUE_HOME_SCREEN_RECORD`: enable screen recording
        /// </param>
        /// <param name="timesetup">The interval of key pressing is set only if there is the double click event or long pressing event. When shortly pressing the Home button, pass `0`.</param>
        /// <param name="pkg">Pass `null`.</param>
        /// <param name="className">Pass `null`.</param>
        /// <param name="callback">Callback:
        /// * `true`: set
        /// * `false`: failed to set
        /// </param>
        public static void PropertySetHomeKeyAll(HomeEventEnum eventEnum, HomeFunctionEnum function, int timesetup, string pkg, string className, Action<bool> callback)
        {
            PXR_EnterprisePlugin.UPxr_PropertySetHomeKeyAll(eventEnum, function, timesetup, pkg, className, callback);
        }

        /// <summary>
        /// Sets the Power button's event.
        /// </summary>
        /// <param name="isSingleTap">Whether it is a single click event:
        /// * `true`: single-click event
        /// * `false`: long-press event
        /// </param>
        /// <param name="enable">Enable or disable the Power button:
        /// * `true`: enable
        /// * `false`: disable
        /// </param>
        /// <param name="callback">Callback:
        /// * `0`: set
        /// * `1`: failed to set
        /// </param>
        public static void PropertyDisablePowerKey(bool isSingleTap, bool enable, Action<int> callback)
        {
            PXR_EnterprisePlugin.UPxr_PropertyDisablePowerKey(isSingleTap, enable, callback);
        }

        /// <summary>
        /// Sets the time the screen turns off when the device is not in use.
        /// </summary>
        /// <param name="timeEnum">Screen off timeout. Enumerations:
        /// * `Never`: never off
        /// * `THREE`: 3s (only supported by PICO G2 4K)
        /// * `TEN`: 10s (only supported by PICO G2 4K)
        /// * `THIRTY`: 30s
        /// * `SIXTY`: 60s
        /// * `THREE_HUNDRED`: 5 mins
        /// * `SIX_HUNDRED`: 10 mins
        /// </param>
        /// <param name="callback">Callback:
        /// * `0`: set
        /// * `1`: failed to set
        /// * `10`: the screen off timeout should not be longer than the system sleep timeout
        /// </param>
        public static void PropertySetScreenOffDelay(ScreenOffDelayTimeEnum timeEnum, Action<int> callback)
        {
            PXR_EnterprisePlugin.UPxr_PropertySetScreenOffDelay(timeEnum, callback);
        }

        /// <summary>
        /// Sets the time the system sleeps when the device is not in use.
        /// </summary>
        /// <param name="timeEnum">System sleep timeout. Enumerations:
        /// * `Never`: never sleep
        /// * `FIFTEEN`: 15s (only supported by PICO G2 4K)
        /// * `THIRTY`: 30s (only supported by PICO G2 4K)
        /// * `SIXTY`: 60s (only supported by PICO G2 4K)
        /// * `THREE_HUNDRED`: 5 mins
        /// * `SIX_HUNDRED`: 10 mins
        /// * `ONE_THOUSAND_AND_EIGHT_HUNDRED`: 30 mins
        /// </param>
        public static void PropertySetSleepDelay(SleepDelayTimeEnum timeEnum)
        {
            PXR_EnterprisePlugin.UPxr_PropertySetSleepDelay(timeEnum);
        }

        /// <summary>
        /// Switches specified system function on/off.
        /// </summary>
        /// <param name="systemFunction">Function name. Enumerations:
        /// * `SFS_USB`: USB debugging
        /// * `SFS_AUTOSLEEP`: auto sleep
        /// * `SFS_SCREENON_CHARGING`: screen-on charging
        /// * `SFS_OTG_CHARGING`: OTG charging (supported by G2 devices)
        /// * `SFS_RETURN_MENU_IN_2DMODE`: display the Return icon on the 2D screen
        /// * `SFS_COMBINATION_KEY`: combination key
        /// * `SFS_CALIBRATION_WITH_POWER_ON`: calibration with power on
        /// * `SFS_SYSTEM_UPDATE`: system update (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_CAST_SERVICE`: phone casting service
        /// * `SFS_EYE_PROTECTION`: eye-protection mode
        /// * `SFS_SECURITY_ZONE_PERMANENTLY`: permanently disable the 6DoF play area (supported by PICO Neo2 devices)
        /// * `SFS_GLOBAL_CALIBRATION`: global calibration (supported by PICO G2 devices)
        /// * `SFS_Auto_Calibration`: auto calibration
        /// * `SFS_USB_BOOT`: USB plug-in boot
        /// * `SFS_VOLUME_UI`: global volume UI (need to restart the device to make the setting take effect)
        /// * `SFS_CONTROLLER_UI`: global controller connected UI
        /// * `SFS_NAVGATION_SWITCH`: navigation bar
        /// * `SFS_SHORTCUT_SHOW_RECORD_UI`: screen recording button UI
        /// * `SFS_SHORTCUT_SHOW_FIT_UI`: PICO fit UI
        /// * `SFS_SHORTCUT_SHOW_CAST_UI`: screencast button UI
        /// * `SFS_SHORTCUT_SHOW_CAPTURE_UI`: screenshot button UI
        /// * `SFS_USB_FORCE_HOST`: set the Neo3 Pro/Pro Eye device as the host device
        /// * `SFS_SET_DEFAULT_SAFETY_ZONE`: set a default play area for PICO Neo3 and PICO 4 series devices
        /// * `SFS_ALLOW_RESET_BOUNDARY`: allow to reset customized boundary for PICO Neo3 series devices
        /// * `SFS_BOUNDARY_CONFIRMATION_SCREEN`: whether to display the boundary confirmation screen for PICO Neo3 and PICO 4 series devices
        /// * `SFS_LONG_PRESS_HOME_TO_RECENTER`: long press the Home button to recenter for PICO Neo3 and PICO 4 series devices
        /// * `SFS_POWER_CTRL_WIFI_ENABLE`: stay connected to the network when the device sleeps/turns off (supported by PICO Neo3 Pro with system version 4.8.0 or later and PICO 4 Enterprise with system version OTA-5.2.8 or later)
        /// * `SFS_WIFI_DISABLE`: disable Wi-Fi (supported by PICO Neo3 Pro with system version 4.8.0 or later and PICO 4 Enterprise with system version OTA-5.2.8 or later)
        /// * `SFS_SIX_DOF_SWITCH`: 6DoF position tracking for PICO Neo3 and PICO 4 series devices
        /// * `SFS_INVERSE_DISPERSION`: anti-dispersion (supported by PICO Neo3 Pro with system version 4.8.0 or later and PICO 4 Enterprise with system version OTA0-5.2.8 or later)
        /// * `SFS_LOGCAT`: system log switch (/data/logs) (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_PSENSOR`: PSensor switch (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SYSTEM_UPDATE_OTA`: OTA upgrade (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SYSTEM_UPDATE_APP`: app upgrade and update (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_WLAN_UI`: quickly set whether to show the WLAN button (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_BOUNDARY_UI`: quickly set whether to show the boundary button (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_BLUETOOTH_UI`: quickly set whether to show the bluetooth button (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_CLEAN_TASK_UI`: quickly set whether to show the one-click clear button (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_IPD_ADJUSTMENT_UI`: quickly set whether to show the IPD adjustment button (supported by PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_POWER_UI`: quickly set whether to show the power button (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_EDIT_UI`: quickly set whether to show the edit button (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_BASIC_SETTING_APP_LIBRARY_UI`: the button for customizing the app library (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_BASIC_SETTING_SHORTCUT_UI`: the button for customizing quick settings (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_LED_FLASHING_WHEN_SCREEN_OFF`: whether to keep the LED indicator light on when the device's screen is off and the battery is below 20% (supported by PICO G3 devices)
        /// * `SFS_BASIC_SETTING_CUSTOMIZE_SETTING_UI`: customize settings item to show or hide in basic settings
        /// * `SFS_BASIC_SETTING_SHOW_APP_QUIT_CONFIRM_DIALOG`: whether to show the app-quit dialog box when switching to a new app
        /// * `SFS_BASIC_SETTING_KILL_BACKGROUND_VR_APP`: whether to kill background VR apps (`1`: kill, and this is the default setting; `2`: do not kill)
        /// * `SFS_BASIC_SETTING_SHOW_CAST_NOTIFICATION`: whether to show a blue icon when casting the screen. The icon is displayed by default, and you can set the value to `0` to hide it.
        /// * `SFS_AUTOMATIC_IPD`: auto IPD switch (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.7.0 or later)
        /// * `SFS_QUICK_SEETHROUGH_MODE`: quick seethrough mode switch (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.7.0 or later)
        /// * `SFS_HIGN_REFERSH_MODE`: high refresh mode switch (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.7.0 or later)
        /// * `SFS_SEETHROUGH_APP_KEEP_RUNNING`: set whether to keep the app running under the seethrough mode (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.8.0 or later)
        /// * `SFS_OUTDOOR_TRACKING_ENHANCEMENT`: enhance outdoor position tracking (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.8.0 or later)
        /// * `SFS_AUTOIPD_AUTO_COMFIRM`: quick auto-IPD (supported by PICO 4 Enterprise with system version 5.8.0 or later)
        /// * `SFS_LAUNCH_AUTOIPD_IF_GLASSES_WEARED`: set whether to launch auto-IPD after wearing the headset (supported by PICO 4 Enterprise with system version 5.8.0 or later)
        /// * `SFS_GESTURE_RECOGNITION_HOME_ENABLE`: Home gesture switch (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.8.0 or later) 
        /// * `SFS_GESTURE_RECOGNITION_RESET_ENABLE`: enable/disable the Reset gesture (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.8.0 or later) 
        /// * `SFS_AUTO_COPY_FILES_FROM_USB_DEVICE`: automatically import OTG resources (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.8.0 or later) 
        /// * `SFS_WIFI_P2P_AUTO_CONNECT`: WiFi P2P auto connection. All devices support silent connection, and no need to add a pop-up window
        /// * `SFS_LOCK_SCREEN_FILE_COPY_ENABLE`: Enable/disable file copy when the screen is locked
        /// * `SFS_TRACKING_ENABLE_DYNAMIC_MARKER`: Enable/disable dynamic marker tracking
        /// * `SFS_ENABLE_3DOF_CONTROLLER_TRACKING`: Switch between 3DoF and 6DoF modes for controllers
        /// * `SFS_SYSTEM_AUTO_UPDATE`: automatic update of the system
        /// * `SFS_USB_TETHERING`: USB tethering
        /// * `SFS_REAL_TIME_RESPONSE_HMD_BACK_KEY_IN_VR_APP`: Respond to the headset's Back button in real-time for VR apps. Switch on: When the headset's Back button is pressed, a `DOWN` event is sent, and when released, an `UP` event is sent. Switch off: When the headset's Back button is pressed, no `DOWN` event is sent, but when the button is released, both a `DOWN` and an `UP` event are sent simultaneously
        /// * `SFS_RETRIEVE_MAP_BY_MARKER_FIRST`: Prioritize using the marker point to relocate on the map
        /// </param>
        /// <param name="switchEnum">Whether to switch the function on/off:
        /// * `S_ON`: switch on
        /// * `S_OFF`: switch off
        /// </param>
        /// <param name="ext">Reserved parameter. Default to `0`.</param>
        public static void SwitchSystemFunction(SystemFunctionSwitchEnum systemFunction, SwitchEnum switchEnum, int ext=0)
        {
            PXR_EnterprisePlugin.UPxr_SwitchSystemFunction(systemFunction, switchEnum,ext);
        }

        /// <summary>
        /// Sets the USB configuration mode.
        /// </summary>
        /// <param name="uSBConfigModeEnum">USB configuration mode. Enumerations:
        /// * `MTP`: MTP mode
        /// * `CHARGE`: charging mode
        /// </param>
        /// <param name="ext">Reserved parameter. Default to `0`.</param>
        public static void SwitchSetUsbConfigurationOption(USBConfigModeEnum uSBConfigModeEnum, int ext=0)
        {
            PXR_EnterprisePlugin.UPxr_SwitchSetUsbConfigurationOption(uSBConfigModeEnum,ext);
        }

        /// <summary>
        /// Sets the duration after which the controllers enter the pairing mode.
        /// @note Supported by PICO Neo3 Pro (system version 5.4.0 or later) and PICO 4 Enterprise (system version 5.2.8 or later)
        /// </summary>
        /// <param name="timeEnum">Duration enumerations:
        /// * `SIX`: 6 seconds
        /// * `FIFTEEN`: 15 seconds
        /// * `SIXTY`: 60 seconds
        /// * `ONE_HUNDRED_AND_TWENTY`: 120 seconds (2 minutes)
        /// * `SIX_HUNDRED`: 600 seconds (5 minutes)
        /// * `NEVER`: never enter the pairing mode
        /// </param>
        /// <param name="callback">Returns the result:
        /// * `0`: failure
        /// * `1`: success
        /// </param>
        /// <param name="ext">Reserved parameter. Default to `0`.</param>
        public static void SetControllerPairTime(ControllerPairTimeEnum timeEnum, Action<int> callback, int ext=0)
        {
            PXR_EnterprisePlugin.UPxr_SetControllerPairTime(timeEnum, callback,ext);
        }

        /// <summary>
        /// Gets the duration after which the controllers enter the pairing mode.
        /// @note Supported by PICO Neo3 Pro (system version 5.4.0 or later) and PICO 4 Enterprise (system version 5.2.8 or later)
        /// </summary>
        /// <param name="callback">Returns a duration enumeration from the following:
        /// * `SIX`: 6 seconds
        /// * `FIFTEEN`: 15 seconds
        /// * `SIXTY`: 60 seconds
        /// * `ONE_HUNDRED_AND_TWENTY`: 120 seconds (2 minutes)
        /// * `SIX_HUNDRED`: 600 seconds (5 minutes)
        /// * `NEVER`: never enter the pairing mode
        /// </param>
        /// <param name="ext">Reserved parameter. Default to `0`.</param>
        public static void GetControllerPairTime(Action<int> callback, int ext=0)
        {
            PXR_EnterprisePlugin.UPxr_GetControllerPairTime(callback,ext);
        }

        /// <summary>
        /// Turns the screen on.
        /// @note This is a protected API. You need to add `<meta-data android:name="pico_advance_interface" android:value="0"/>`
        /// to the app's AndroidManifest.xml file for calling this API, after which the app is unable to be published on the PICO Store.
        /// </summary>
        public static void ScreenOn()
        {
            PXR_EnterprisePlugin.UPxr_ScreenOn();
        }

        /// <summary>
        /// Turns the screen off.
        /// @note This is a protected API. You need to add `<meta-data android:name="pico_advance_interface" android:value="0"/>`
        /// to the app's AndroidManifest.xml file for calling this API, after which the app is unable to be published on the PICO Store.
        /// </summary>
        public static void ScreenOff()
        {
            PXR_EnterprisePlugin.UPxr_ScreenOff();
        }

        /// <summary>
        /// Acquires the wake lock.
        /// </summary>
        public static void AcquireWakeLock()
        {
            PXR_EnterprisePlugin.UPxr_AcquireWakeLock();
        }

        /// <summary>
        /// Releases the wake lock.
        /// </summary>
        public static void ReleaseWakeLock()
        {
            PXR_EnterprisePlugin.UPxr_ReleaseWakeLock();
        }

        /// <summary>
        /// Enables the Confirm button.
        /// </summary>
        public static void EnableEnterKey()
        {
            PXR_EnterprisePlugin.UPxr_EnableEnterKey();
        }

        /// <summary>
        /// Disables the Confirm button.
        /// </summary>
        public static void DisableEnterKey()
        {
            PXR_EnterprisePlugin.UPxr_DisableEnterKey();
        }

        /// <summary>
        /// Enables the Volume button.
        /// </summary>
        public static void EnableVolumeKey()
        {
            PXR_EnterprisePlugin.UPxr_EnableVolumeKey();
        }

        /// <summary>
        /// Disables the Volume button.
        /// </summary>
        public static void DisableVolumeKey()
        {
            PXR_EnterprisePlugin.UPxr_DisableVolumeKey();
        }

        /// <summary>
        /// Enables the Back button.
        /// </summary>
        public static void EnableBackKey()
        {
            PXR_EnterprisePlugin.UPxr_EnableBackKey();
        }

        /// <summary>
        /// Disables the Back button.
        /// </summary>
        public static void DisableBackKey()
        {
            PXR_EnterprisePlugin.UPxr_DisableBackKey();
        }


        /// <summary>
        /// Resets all buttons to default configuration.
        /// </summary>
        /// <param name="callback">Whether all keys have been successfully reset to default configuration:
        /// * `true`: reset
        /// * `false`: failed to reset
        /// </param>
        public static void ResetAllKeyToDefault(Action<bool> callback)
        {
            PXR_EnterprisePlugin.UPxr_ResetAllKeyToDefault(callback);
        }

        /// <summary>
        /// Sets an app as the launcher app. Need to restart the device to make the setting work.
        /// </summary>
        /// <param name="switchEnum">(deprecated)</param>
        /// <param name="packageName">The app's package name.</param>
        public static void SetAPPAsHome(SwitchEnum switchEnum, string packageName)
        {
            PXR_EnterprisePlugin.UPxr_SetAPPAsHome(switchEnum, packageName);
        }

        /// <summary>
        /// Force quits app(s) by passing app PID or package name.
        /// @note This is a protected API. You need to add `<meta-data android:name="pico_advance_interface" android:value="0"/>`
        /// to the app's AndroidManifest.xml file for calling this API, after which the app is unable to be published on the PICO Store.
        /// </summary>
        /// <param name="pids">An array of app PID(s).</param>
        /// <param name="packageNames">An array of package name(s).</param>
        /// <param name="ext">Reserved parameter. Default to `0`.</param>
        public static void KillAppsByPidOrPackageName(int[] pids, string[] packageNames, int ext=0)
        {
            PXR_EnterprisePlugin.UPxr_KillAppsByPidOrPackageName(pids, packageNames,ext);
        }

        /// <summary>
        /// Force quits background app(s) expect those in the allowlist.
        /// @note This is a protected API. You need to add `<meta-data android:name="pico_advance_interface" android:value="0"/>`
        /// to the app's AndroidManifest.xml file for calling this API, after which the app is unable to be published on the PICO Store.
        /// </summary>
        /// <param name="packageNames">An array of package name(s) to be added to the allowlist. The corresponding app(s) in the allowlist will not be force quit.</param>
        /// <param name="ext">Reserved parameter. Default to `0`.</param>
        public static void KillBackgroundAppsWithWhiteList(string[] packageNames, int ext=0)
        {
            PXR_EnterprisePlugin.UPxr_KillBackgroundAppsWithWhiteList(packageNames,ext);
        }

        /// <summary>
        /// Freezes the screen to the front. The screen will turn around with the HMD.
        /// @note Supported by G2 4K and Neo2 devices.
        /// </summary>
        /// <param name="freeze">Whether to freeze the screen:
        /// * `true`: freeze
        /// * `false`: stop freezing
        /// </param>
        public static void FreezeScreen(bool freeze)
        {
            PXR_EnterprisePlugin.UPxr_FreezeScreen(freeze);
        }

        /// <summary>
        /// Turns on the screencast function.
        /// </summary>
        public static void OpenMiracast()
        {
            PXR_EnterprisePlugin.UPxr_OpenMiracast();
        }

        /// <summary>
        /// Gets the status of the screencast function.
        /// </summary>
        /// <returns>The status of the screencast function:
        /// * `true`: on
        /// * `false`: off
        /// </returns>
        public static bool IsMiracastOn()
        {
            return PXR_EnterprisePlugin.UPxr_IsMiracastOn();
        }

        /// <summary>
        /// Turns off the screencast function.
        /// </summary>
        public static void CloseMiracast()
        {
            PXR_EnterprisePlugin.UPxr_CloseMiracast();
        }

        /// <summary>
        /// Starts looking for devices that can be used for screen casting.
        /// </summary>
        public static void StartScan()
        {
            PXR_EnterprisePlugin.UPxr_StartScan();
        }

        /// <summary>
        /// Stops looking for devices that can be used for screen casting.
        /// </summary>
        public static void StopScan()
        {
            PXR_EnterprisePlugin.UPxr_StopScan();
        }

        /// <summary>
        /// Casts the screen to the specified device.
        /// </summary>
        /// <param name="modelJson">A modelJson structure containing the following fields:
        /// * `deviceAddress`
        /// * `deviceName`
        /// * `isAvailable` (`true`-device available; `false`-device not available)
        /// </param>
        public static void ConnectWifiDisplay(string modelJson)
        {
            PXR_EnterprisePlugin.UPxr_ConnectWifiDisplay(modelJson);
        }

        /// <summary>
        /// Stops casting the screen to the current device.
        /// </summary>
        public static void DisConnectWifiDisplay()
        {
            PXR_EnterprisePlugin.UPxr_DisConnectWifiDisplay();
        }

        /// <summary>
        /// Forgets the device that have been connected for screencast.
        /// </summary>
        /// <param name="address">Device address.</param>
        public static void ForgetWifiDisplay(string address)
        {
            PXR_EnterprisePlugin.UPxr_ForgetWifiDisplay(address);
        }

        /// <summary>
        /// Renames the device connected for screencast. The name is only for local storage.
        /// </summary>
        /// <param name="address">The MAC address of the device.</param>
        /// <param name="newName">The new device name.</param>
        public static void RenameWifiDisplay(string address, string newName)
        {
            PXR_EnterprisePlugin.UPxr_RenameWifiDisplay(address, newName);
        }

        /// <summary>
        /// Sets the callback for the scanning result, which returns `List<PBS_WifiDisplayModel>` that contains the devices previously connected for screencast and the devices currently found for screencast.
        /// </summary>
        /// <param name="models">
        /// Returns `List<WifiDisplayModel>` that contains the currently scanned device.
        /// </param>
        public static void SetWDModelsCallback(Action<List<WifiDisplayModel>> models)
        {
            PXR_EnterprisePlugin.UPxr_SetWDModelsCallback(models);
        }

        /// <summary>
        /// Sets the callback for the scanning result, which returns the JSON string that contains the devices previously connected for screencast and the devices currently found for screencast.
        /// </summary>
        /// <param name="callback">
        /// Returns a JSON string that contains the currently scanned device.
        /// </param>
        public static void SetWDJsonCallback(Action<string> callback)
        {
            PXR_EnterprisePlugin.UPxr_SetWDJsonCallback(callback);
        }

        /// <summary>
        /// Manually updates the list of devices for screencast.
        /// </summary>
        public static void UpdateWifiDisplays()
        {
            PXR_EnterprisePlugin.UPxr_UpdateWifiDisplays();
        }

        /// <summary>
        /// Gets the information of the currently connected device.
        /// </summary>
        /// <returns>The information of the currently connected device.</returns>
        public static string GetConnectedWD()
        {
            return PXR_EnterprisePlugin.UPxr_GetConnectedWD();
        }

        /// <summary>
        /// Switches the large space scene on.
        /// @note Supported by 6Dof devices.
        /// </summary>
        /// <param name="open">Whether to switch the large space scene on:
        /// * `true`: switch on
        /// * `false`: not to switch on
        /// </param>
        /// <param name="callback">Callback:
        /// * `true`: success
        /// * `false`: failure
        /// </param>
        /// <param name="ext">Reserved parameter. Default to `0`.</param>
        public static void SwitchLargeSpaceScene(bool open, Action<bool> callback, int ext=0)
        {
            PXR_EnterprisePlugin.UPxr_SwitchLargeSpaceScene(open, callback,ext);
        }

        /// <summary>
        /// Gets the status of the large space scene.
        /// @note Supported by 6Dof devices.
        /// </summary>
        /// <param name="callback">Returns the status of large space:
        /// * `0`: switched off
        /// * `1`: switched on
        /// </param>
        /// <param name="ext">Reserved parameter. Default to `0`.</param>
        public static void GetSwitchLargeSpaceStatus(Action<string> callback, int ext=0)
        {
            PXR_EnterprisePlugin.UPxr_GetSwitchLargeSpaceStatus(callback,ext);
        }

        /// <summary>
        /// Saves the large space map.
        /// @note Supported by 6Dof devices.
        /// </summary>
        /// <param name="ext">Reserved parameter. Default to `0`.</param>
        /// <returns>Whether the large space map has been saved:
        /// * `true`: saved
        /// * `false`: failed to save
        /// </returns>
        public static bool SaveLargeSpaceMaps(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_SaveLargeSpaceMaps(ext);
        }

        /// <summary>
        /// Exports maps. The exported maps are stored in the /maps/export file.
        /// @note Supported by 6Dof devices.
        /// </summary>
        /// <param name="callback">Returns the result:
        /// * `true`: exported
        /// * `false`: failed to export
        /// </param>
        /// <param name="ext">Reserved parameter. Default to `0`.</param>
        public static void ExportMaps(Action<bool> callback, int ext=0)
        {
            PXR_EnterprisePlugin.UPxr_ExportMaps(callback,ext);
        }

        /// <summary>
        /// Imports maps. Need to copy maps to the /maps folder.
        /// @note Supported by 6Dof devices.
        /// </summary>
        /// <param name="callback">Returns the result:
        /// * `true`: imported
        /// * `false`: failed to import
        /// </param>
        /// <param name="ext">Reserved parameter. Default to `0`.</param>
        public static void ImportMaps(Action<bool> callback, int ext=0)
        {
            PXR_EnterprisePlugin.UPxr_ImportMaps(callback,ext);
        }

        /// <summary>
        /// Gets each CPU's utilization for the current device.
        /// </summary>
        /// <returns>An array of CPU utilization info.</returns>
        public static float[] GetCpuUsages()
        {
            return PXR_EnterprisePlugin.UPxr_GetCpuUsages();
        }

        /// <summary>
        /// Gets device temperature in Celsius.
        /// </summary>
        /// <param name="type">The requested type of device temperature:
        /// * `DEVICE_TEMPERATURE_CPU`: CPU temperature
        /// * `DEVICE_TEMPERATURE_GPU`: GPU temperature
        /// * `DEVICE_TEMPERATURE_BATTERY`: battery temperature
        /// * `DEVICE_TEMPERATURE_SKIN`: surface temperature
        /// </param>
        /// <param name="source">The requested source of device temperature:
        /// * `TEMPERATURE_CURRENT`: current temperature
        /// * `TEMPERATURE_THROTTLING`: temperature threshold for throttling
        /// * `TEMPERATURE_SHUTDOWN`: temperature threshold for shutdown
        /// * `TEMPERATURE_THROTTLING_BELOW_VR_MIN`: temperature threshold for throttling. If the actual temperature is higher than the threshold, the lowest clock frequency for VR mode will not be met
        /// </param>
        /// <returns>An array of requested float device temperatures in Celsius.</returns>
        public static float[] GetDeviceTemperatures(int type, int source)
        {
            return PXR_EnterprisePlugin.UPxr_GetDeviceTemperatures(type, source);
        }

        /// <summary>
        /// Captures the current screen.
        /// @note Not supported by G2 4K devices.
        /// </summary>
        public static void Capture()
        {
            PXR_EnterprisePlugin.UPxr_Capture();
        }

        /// <summary>
        /// Records the screen. Call this function again to stop recording.
        /// @note Not supported by G2 4K devices.
        /// </summary>
        public static void Record()
        {
            PXR_EnterprisePlugin.UPxr_Record();
        }

        /// <summary>
        /// Connects the device to a specified Wi-Fi.  
        /// </summary>
        /// <param name="ssid">Wi-Fi name.</param>
        /// <param name="pwd">Wi-Fi password.</param>
        /// <param name="ext">Reserved parameter, pass `0` by default.</param>
        /// <param name="callback">The callback for indicating whether the Wi-Fi connection is successful:
        /// * `0`: connected
        /// * `1`: password error
        /// * `2`: unknown error
        /// </param>
        public static void ControlSetAutoConnectWIFIWithErrorCodeCallback(String ssid, String pwd, int ext, Action<int> callback)
        {
            PXR_EnterprisePlugin.UPxr_ControlSetAutoConnectWIFIWithErrorCodeCallback(ssid, pwd, ext, callback);
        }

        /// <summary>
        /// Keeps an app active. In other words, improves the priority of an app, thereby making the system not to force quit the app.
        /// @note Supported by PICO Neo3 Pro (system version 4.8.0 or later) and PICO 4 Enterprise (system version OTA-5.2.8 or later).
        /// </summary>
        /// <param name="appPackageName">App package name.</param>
        /// <param name="keepAlive">Whether to keep the app active (i.e., whether to enhance the priority of the app):
        /// * `true`: keep
        /// * `false`: not keep
        /// </param>
        /// <param name="ext">Reserved parameter, pass `0`.</param>
        public static void AppKeepAlive(String appPackageName, bool keepAlive, int ext)
        {
            PXR_EnterprisePlugin.UPxr_AppKeepAlive(appPackageName, keepAlive, ext);
        }

        /// <summary>
        /// Schedules auto startup for the device.
        /// @note Supported by PICO Neo3 Pro (system version 4.8.0 or later) and PICO 4 Enterprise (system version OTA-5.2.8 or later).
        /// </summary>
        /// <param name="year">Year, for example, `2022`.</param>
        /// <param name="month">Month, for example, `2`.</param>
        /// <param name="day">Day, for example, `22`.</param>
        /// <param name="hour">Hour, for example, `22`.</param>
        /// <param name="minute">Minute, for example, `22`.</param>
        /// <param name="open">Whether to enable scheduled auto startup for the device:
        /// * `true`: enable
        /// * `false`: disable
        /// </param>
        public static void TimingStartup(int year, int month, int day, int hour, int minute, bool open)
        {
            PXR_EnterprisePlugin.UPxr_TimingStartup(year, month, day, hour, minute, open);
        }

        /// <summary>
        /// Schedules auto shutdown for the device.
        /// @note Supported by PICO Neo3 Pro (system version 4.8.0 or later) and PICO 4 Enterprise (system version 5.4.0 or later).
        /// </summary>
        /// <param name="year">Year, for example, `2022`.</param>
        /// <param name="month">Month, for example, `2`.</param>
        /// <param name="day">Day, for example, `22`.</param>
        /// <param name="hour">Hour, for example, `22`.</param>
        /// <param name="minute">Minute, for example, `22`.</param>
        /// <param name="open">Whether to enable scheduled auto shutdown for the device:
        /// * `true`: enable
        /// * `false`: disable
        /// </param>
        public static void TimingShutdown(int year, int month, int day, int hour, int minute, bool open)
        {
            PXR_EnterprisePlugin.UPxr_TimingShutdown(year, month, day, hour, minute, open);
        }

        /// <summary>
        /// Displays a specified settings screen.
        /// @note Supported by PICO Neo3 Pro (system version 4.8.0 or later) and PICO 4 Enterprise (system version OTA-5.2.8 or later).
        /// </summary>
        /// <param name="settingsEnum">The enumerations of settings screen:
        /// * `START_VR_SETTINGS_ITEM_WIFI`: the Wi-Fi settings screen;
        /// * `START_VR_SETTINGS_ITEM_BLUETOOTH`: the bluetooth settings screen;
        /// * `START_VR_SETTINGS_ITEM_CONTROLLER`: the controller settings screen;
        /// * `START_VR_SETTINGS_ITEM_LAB`: the lab settings screen;
        /// * `START_VR_SETTINGS_ITEM_BRIGHTNESS`: the brightness settings screen;
        /// * `START_VR_SETTINGS_ITEM_GENERAL)`: the general settings screen;
        /// * `START_VR_SETTINGS_ITEM_NOTIFICATION`: the notification settings screen.
        /// </param>
        /// <param name="hideOtherItem">Whether to display the selected settings screen:
        /// * `true`: display
        /// * `false`: hide
        /// </param>
        /// <param name="ext">Reserved parameter, pass `0`.</param>
        public static void StartVrSettingsItem(StartVRSettingsEnum settingsEnum, bool hideOtherItem, int ext)
        {
            PXR_EnterprisePlugin.UPxr_StartVrSettingsItem(settingsEnum, hideOtherItem, ext);
        }

        /// <summary>
        /// Changes the Volume button's function to that of the Home and Enter button's, or restores the volume adjustment function to the Volume button.
        /// @note Supported by PICO 4 Enterprise with system version OTA-5.2.8 or later.
        /// </summary>
        /// <param name="switchEnum">Whether to change the Volume button's function:
        /// * `S_ON`: change
        /// * `S_OFF`: do not change
        /// </param>
        /// <param name="ext">Reserved parameter, pass `0`.</param>
        public static void SwitchVolumeToHomeAndEnter(SwitchEnum switchEnum, int ext)
        {
            PXR_EnterprisePlugin.UPxr_SwitchVolumeToHomeAndEnter(switchEnum, ext);
        }

        /// <summary>
        /// Gets whether the Volume button's function has been changed to that of the Home and Enter button's.
        /// @note Supported by PICO 4 Enterprise with system version OTA-5.2.8 or later.
        /// </summary>
        /// <returns>
        /// * `S_ON`: changed
        /// * `S_OFF`: not changed
        /// </returns>
        public static SwitchEnum IsVolumeChangeToHomeAndEnter()
        {
            return PXR_EnterprisePlugin.UPxr_IsVolumeChangeToHomeAndEnter();
        }

        /// <summary>
        /// Upgrades the OTA.
        /// @note Supported by PICO Neo3 Pro (system version 4.8.0 or later) and PICO 4 Enterprise (system version OTA0-5.2.8 or later).
        /// </summary>
        /// <param name="otaPackagePath">The location of the OTA package.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// * `21`: OTA package version too low
        /// </returns>
        public static int InstallOTAPackage(String otaPackagePath, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_InstallOTAPackage(otaPackagePath,ext);
        }

        /// <summary>
        /// Gets the configuration of the Wi-Fi network that the device automatically connects to.
        /// </summary>
        /// <returns>The SSID and password of the Wi-Fi network.</returns>
        public static string GetAutoConnectWiFiConfig(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetAutoConnectWiFiConfig(ext);
        }

        /// <summary>
        /// Gets the scheduled auto startup settings for the device.
        /// @note Supported by PICO Neo3 Pro (system version 4.8.0 or later) and PICO 4 Enterprise (system version OTA-5.2.8 or later).
        /// </summary>
        /// <returns>
        /// * `open`: the status of scheduled auto startup:
        ///   * `true`: enabled
        ///   * `false`: disabled
        /// * `time`: the time when the device auto starts up, for example, `1658980380000`. Returned when `open` is `true`.
        /// </returns>
        public static string GetTimingStartupStatus(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetTimingStartupStatus(ext);
        }

        /// <summary>
        /// Gets the scheduled auto shutdown settings for the device.
        /// @note Supported by PICO Neo3 Pro (system version 4.8.0 or later) and PICO 4 Enterprise (system version OTA-5.2.8 or later).
        /// </summary>
        /// <returns>
        /// * `open`: the status of scheduled auto shutdown:
        ///   * `true`: enabled
        ///   * `false`: disabled
        /// * `time`: the time when the device auto shuts down, for example, `1658980380000`. Returned when `open` is `true`.
        /// </returns>
        public static string GetTimingShutdownStatus(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetTimingShutdownStatus(ext);
        }

        /// <summary>
        /// Gets the status of a specified controller button.
        /// @note Supported by PICO Neo3 Pro (system version 4.8.0 or later) and PICO 4 Enterprise (system version OTA0-5.2.8 or later).
        /// </summary>
        /// <param name="pxrControllerKey">The enumerations of controller button:
        /// * `CONTROLLER_KEY_JOYSTICK` 
        /// * `CONTROLLER_KEY_MENU`
        /// * `CONTROLLER_KEY_TRIGGER`
        /// * `CONTROLLER_KEY_RIGHT_A`
        /// * `CONTROLLER_KEY_RIGHT_B`
        /// * `CONTROLLER_KEY_LEFT_X`
        /// * `CONTROLLER_KEY_LEFT_Y`
        /// * `CONTROLLER_KEY_LEFT_GRIP`
        /// * `CONTROLLER_KEY_RIGHT_GRIP`
        /// </param>
        /// <returns>The button's status:
        /// * `0`: disabled
        /// * `1`: enabled
        /// </returns>
        public static int GetControllerKeyState(ControllerKeyEnum pxrControllerKey, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetControllerKeyState(pxrControllerKey,ext);
        }

        /// <summary>
        /// Enables or disables a specified controller button.
        /// @note Supported by PICO Neo3 Pro (system version 4.8.0 or later) and PICO 4 Enterprise (system version OTA-5.2.8 or later).
        /// </summary>
        /// <param name="pxrControllerKey">The enumerations of controller button:
        /// * `CONTROLLER_KEY_JOYSTICK` 
        /// * `CONTROLLER_KEY_MENU`
        /// * `CONTROLLER_KEY_TRIGGER`
        /// * `CONTROLLER_KEY_RIGHT_A`
        /// * `CONTROLLER_KEY_RIGHT_B`
        /// * `CONTROLLER_KEY_LEFT_X`
        /// * `CONTROLLER_KEY_LEFT_Y`
        /// * `CONTROLLER_KEY_LEFT_GRIP`
        /// * `CONTROLLER_KEY_RIGHT_GRIP`
        /// </param>
        /// <param name="status">Whether to enable or disable the button:
        /// * `S_ON`: enable
        /// * `S_OFF`: disable
        /// </param>
        /// <returns>
        /// `0` indicates success, other values indicate failure.
        /// </returns>
        public static int SetControllerKeyState(ControllerKeyEnum pxrControllerKey, SwitchEnum status, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_SetControllerKeyState(pxrControllerKey, status,ext);
        }

        /// <summary>
        /// Gets the status of the switch which is for powering off the USB cable when the device is shut down.
        /// </summary>
        /// <returns>The switch's status:
        /// * `S_ON`: on
        /// * `S_OFF`: off
        /// </returns>
        public static SwitchEnum GetPowerOffWithUSBCable(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_ControlGetPowerOffWithUSBCable(ext);
        }

        /// <summary>
        /// Gets the screen timeout setting for the device.
        /// </summary>
        /// <returns>`PBS_ScreenOffDelayTimeEnum`: the enumerations of screen timeout. </returns>
        public static ScreenOffDelayTimeEnum GetScreenOffDelay(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_PropertyGetScreenOffDelay(ext);
        }

        /// <summary>
        /// Gets the sleep timeout settings for the device.
        /// </summary>
        /// <returns>`PBS_SleepDelayTimeEnum`: the enumeration of sleep timeout.</returns>
        public static SleepDelayTimeEnum GetSleepDelay(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_PropertyGetSleepDelay(ext);
        }

        /// <summary>
        /// Gets the current settings for the Power button.
        /// </summary>
        /// <returns>
        /// * `null`: not set
        /// * `singleTap`: whether a single-tap event has been set
        /// * `longTap`: whether a long-press event has been set
        /// * `longPressTime`: the time after which the long-press event takes place. Returned when `longTap` is `true`.
        /// </returns>
        public static string GetPowerKeyStatus(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_PropertyGetPowerKeyStatus(ext);
        }

        /// <summary>
        /// Get the Enter button's status.
        /// </summary>
        /// <returns>
        /// * `0`: disabled
        /// * `1`: enabled
        /// </returns>
        public static int GetEnterKeyStatus(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetEnterKeyStatus(ext);
        }

        /// <summary>
        /// Get the Volume button's status.
        /// </summary>
        /// <returns>
        /// * `0`: disabled
        /// * `1`: enabled
        /// </returns>
        public static int GetVolumeKeyStatus(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetVolumeKeyStatus(ext);
        }

        /// <summary>
        /// Get the Back button's status.
        /// </summary>
        /// <returns>
        /// * `0`: disabled
        /// * `1`: enabled
        /// </returns>
        public static int GetBackKeyStatus(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetBackKeyStatus(ext);
        }

        /// <summary>
        /// Gets the event settings for the Home button.
        /// </summary>
        /// <param name="homeEvent">The enumerations of event type:
        /// * `SINGLE_CLICK`: single-click event
        /// * `DOUBLE_CLICK`: double-click event
        /// * `LONG_PRESS`: long-press event
        /// </param>
        /// <returns>
        /// * For `SINGLE_CLICK` and `DOUBLE_CLICK`, the event(s) you set will be returned.
        /// * For `LONG_PRESS`, the time and event you set will be returned. If you have not set a time for a long-press event, time will be `null`.
        /// 
        /// * If you have not set any event for the event type you pass in the request, the response will return `null`.
        /// * For event enumerations, see `PropertySetHomeKey` or `PropertySetHomeKeyAll`.
        /// </returns>
        public static string GetHomeKeyStatus(HomeEventEnum homeEvent, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_PropertyGetHomeKeyStatus(homeEvent,ext);
        }

        /// <summary>
        /// Gets the status of a specified system function switch.
        /// </summary>
        /// <param name="systemFunction">The enumerations of system function switch:
        /// * `SFS_USB`: USB debugging
        /// * `SFS_AUTOSLEEP`: auto sleep
        /// * `SFS_SCREENON_CHARGING`: screen-on charging
        /// * `SFS_OTG_CHARGING`: OTG charging (supported by G2 devices)
        /// * `SFS_RETURN_MENU_IN_2DMODE`: display the Return icon on the 2D screen
        /// * `SFS_COMBINATION_KEY`: combination key
        /// * `SFS_CALIBRATION_WITH_POWER_ON`: calibration with power on
        /// * `SFS_SYSTEM_UPDATE`: system update (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_CAST_SERVICE`: phone casting service
        /// * `SFS_EYE_PROTECTION`: eye-protection mode
        /// * `SFS_SECURITY_ZONE_PERMANENTLY`: permanently disable the 6DoF play area (supported by PICO Neo2 devices)
        /// * `SFS_GLOBAL_CALIBRATION`: global calibration (supported by PICO G2 devices)
        /// * `SFS_Auto_Calibration`: auto calibration
        /// * `SFS_USB_BOOT`: USB plug-in boot
        /// * `SFS_VOLUME_UI`: global volume UI (need to restart the device to make the setting take effect)
        /// * `SFS_CONTROLLER_UI`: global controller connected UI
        /// * `SFS_NAVGATION_SWITCH`: navigation bar
        /// * `SFS_SHORTCUT_SHOW_RECORD_UI`: screen recording button UI
        /// * `SFS_SHORTCUT_SHOW_FIT_UI`: PICO fit UI
        /// * `SFS_SHORTCUT_SHOW_CAST_UI`: screencast button UI
        /// * `SFS_SHORTCUT_SHOW_CAPTURE_UI`: screenshot button UI
        /// * `SFS_USB_FORCE_HOST`: set the Neo3 Pro/Pro Eye device as the host device
        /// * `SFS_SET_DEFAULT_SAFETY_ZONE`: set a default play area for PICO Neo3 and PICO 4 series devices
        /// * `SFS_ALLOW_RESET_BOUNDARY`: allow to reset customized boundary for PICO Neo3 series devices
        /// * `SFS_BOUNDARY_CONFIRMATION_SCREEN`: whether to display the boundary confirmation screen for PICO Neo3 and PICO 4 series devices
        /// * `SFS_LONG_PRESS_HOME_TO_RECENTER`: long press the Home button to recenter for PICO Neo3 and PICO 4 series devices
        /// * `SFS_POWER_CTRL_WIFI_ENABLE`: stay connected to the network when the device sleeps/turns off (supported by PICO Neo3 Pro with system version 4.8.0 or later and PICO 4 Enterprise with system version OTA-5.2.8 or later)
        /// * `SFS_WIFI_DISABLE`: disable Wi-Fi (supported by PICO Neo3 Pro with system version 4.8.0 or later and PICO 4 Enterprise with system version OTA-5.2.8 or later)
        /// * `SFS_SIX_DOF_SWITCH`: 6DoF position tracking for PICO Neo3 and PICO 4 series devices
        /// * `SFS_INVERSE_DISPERSION`: anti-dispersion (supported by PICO Neo3 Pro with system version 4.8.0 or later and PICO 4 Enterprise with system version OTA0-5.2.8 or later)
        /// * `SFS_LOGCAT`: system log switch (/data/logs) (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_PSENSOR`: PSensor switch (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SYSTEM_UPDATE_OTA`: OTA upgrade (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SYSTEM_UPDATE_APP`: app upgrade and update (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_WLAN_UI`: quickly set whether to show the WLAN button (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_BOUNDARY_UI`: quickly set whether to show the boundary button (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_BLUETOOTH_UI`: quickly set whether to show the bluetooth button (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_CLEAN_TASK_UI`: quickly set whether to show the one-click clear button (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_IPD_ADJUSTMENT_UI`: quickly set whether to show the IPD adjustment button (supported by PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_POWER_UI`: quickly set whether to show the power button (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_EDIT_UI`: quickly set whether to show the edit button (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_BASIC_SETTING_APP_LIBRARY_UI`: the button for customizing the app library (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_BASIC_SETTING_SHORTCUT_UI`: the button for customizing quick settings (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_LED_FLASHING_WHEN_SCREEN_OFF`: whether to keep the LED indicator light on when the device's screen is off and the battery is below 20% (supported by PICO G3 devices)
        /// * `SFS_BASIC_SETTING_CUSTOMIZE_SETTING_UI`: customize settings item to show or hide in basic settings
        /// * `SFS_BASIC_SETTING_SHOW_APP_QUIT_CONFIRM_DIALOG`: whether to show the app-quit dialog box when switching to a new app
        /// * `SFS_BASIC_SETTING_KILL_BACKGROUND_VR_APP`: whether to kill background VR apps (`1`: kill, and this is the default setting; `2`: do not kill)
        /// * `SFS_BASIC_SETTING_SHOW_CAST_NOTIFICATION`: whether to show a blue icon when casting the screen. The icon is displayed by default, and you can set the value to `0` to hide it.
        /// * `SFS_AUTOMATIC_IPD`: auto IPD switch (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.7.0 or later)
        /// * `SFS_QUICK_SEETHROUGH_MODE`: quick seethrough mode switch (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.7.0 or later)
        /// * `SFS_HIGN_REFERSH_MODE`: high refresh mode switch (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.7.0 or later)
        /// * `SFS_SEETHROUGH_APP_KEEP_RUNNING`: set whether to keep the app running under the seethrough mode (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.8.0 or later)
        /// * `SFS_OUTDOOR_TRACKING_ENHANCEMENT`: enhance outdoor position tracking (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.8.0 or later)
        /// * `SFS_AUTOIPD_AUTO_COMFIRM`: quick auto-IPD (supported by PICO 4 Enterprise with system version 5.8.0 or later)
        /// * `SFS_LAUNCH_AUTOIPD_IF_GLASSES_WEARED`: set whether to launch auto-IPD after wearing the headset (supported by PICO 4 Enterprise with system version 5.8.0 or later)
        /// * `SFS_GESTURE_RECOGNITION_HOME_ENABLE`: Home gesture switch (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.8.0 or later) 
        /// * `SFS_GESTURE_RECOGNITION_RESET_ENABLE`: enable/disable the Reset gesture (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.8.0 or later) 
        /// * `SFS_AUTO_COPY_FILES_FROM_USB_DEVICE`: automatically import OTG resources (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.8.0 or later) 
        /// * `SFS_WIFI_P2P_AUTO_CONNECT`: WiFi P2P auto connection. All devices support silent connection, and no need to add a pop-up window
        /// * `SFS_LOCK_SCREEN_FILE_COPY_ENABLE`: Enable/disable file copy when the screen is locked
        /// * `SFS_TRACKING_ENABLE_DYNAMIC_MARKER`: Enable/disable dynamic marker tracking
        /// * `SFS_ENABLE_3DOF_CONTROLLER_TRACKING`: Switch between 3DoF and 6DoF modes for controllers
        /// * `SFS_SYSTEM_VIBRATION_ENABLED`: haptic feedback (supported by OS 5.6.0 or later)
        /// * `SFS_BLUE_TOOTH`: bluetooth switch
        /// * `SFS_ENHANCED_VIDEO_QUALITY`: enhance video quality (supported by OS 5.8.0 or later)
        /// * `SFS_GESTURE_RECOGNITION`: hand tracking (supported by OS 5.6.0 or later)
        /// * `SFS_BRIGHTNESS_AUTO_ADJUST`: self-adaptive brightness (supported by OS 5.6.0 or later)
        /// * `SFS_HIGH_CURRENT_OTG_MODE`:high-current OTG mode (supported by OS 5.8.0 or later)
        /// * `SFS_BACKGROUND_APP_PLAY_AUDIO`: forbid background apps from playing audio (supported by OS 5.6.0 or later)
        /// * `SFS_NO_DISTURB_MODE`: Do Not Disturb mode (supported by OS 5.6.0 or later)
        /// * `SFS_MONOCULAR_SCREENCAST`: monocular screencast (supported by OS 5.7.0 or later)
        /// * `SFS_MONOCULAR_SCREEN_CAPTURE`: monocular screen recording or screen capturing (supported by OS 5.7.0 or later)
        /// * `SFS_STABILIZATION_FOR_RECORDING`: to reduce screen shaking in screen recording (supported by OS 5.7.0 or later)
        /// * `SFS_HIDE_2D_APP_WHEN_GO_TO_HOME`: When the primary screen app is a VR app, return to the launcher to minimize 2D apps
        /// * `SFS_CONTROLLER_VIBRATE`: the switch to enable/disable controller vibration
        /// * `SFS_REFRESH_MODE`: the switch to enable/disable refresh mode
        /// * `SFS_SMART_AUDIO`: the switch to enable/disable smart audio
        /// * `SFS_EYE_TRACK`: the switch to enable/disable eye tracking
        /// * `SFS_FACE_SIMULATE`: the switch to enable/disable face tracking
        /// * `SFS_ENABLE_MIC_WHEN_RECORD`: the switch to enable/disable microphone during screen recording
        /// * `SFS_KEEP_RECORD_WHEN_SCREEN_OFF`: whether to keep recording the screen when the screen is off
        /// * `SFS_CONTROLLER_TIP_VIBRATE`: within the boundary, the switch to enable/disable controller vibration alerts
        /// * `SFS_CONTROLLER_SEE_THROUGH`: within the boundary, the switch to enable/disable the trigger of video seethrough by controller
        /// * `SFS_LOW_BORDER_HEIGHT`: within the boundary, the switch to lower the height of the boundary
        /// * `SFS_FAST_MOVE_TIP`: within the boundary, the switch to enable/disable quick movement safety alerts
        /// * `SFS_WIRELESS_USB_ADB`: the switch to enable/disable wireless USB debugging 
        /// * `SFS_SYSTEM_AUTO_UPDATE`: automatic update of the system
        /// * `SFS_USB_TETHERING`: USB tethering
        /// * `SFS_REAL_TIME_RESPONSE_HMD_BACK_KEY_IN_VR_APP`: Respond to the headset's Back button in real-time for VR apps. Switch on: When the headset's Back button is pressed, a `DOWN` event is sent, and when released, an `UP` event is sent. Switch off: When the headset's Back button is pressed, no `DOWN` event is sent, but when the button is released, both a `DOWN` and an `UP` event are sent simultaneously
        /// * `SFS_RETRIEVE_MAP_BY_MARKER_FIRST`: Prioritize using the marker point to relocate on the map
        /// </param>
        /// <param name="callback">The callback that returns the switch's status:
        /// * `0`: off
        /// * `1`: on
        /// * `2`: not supported by device
        /// For `SFS_SYSTEM_UPDATE`, the returns are as follows:
        /// * `0`: off
        /// * `1`: OTA upgrade on
        /// * `2`: app upgrade on
        /// * `3`: OTA and app upgrade on
        /// </param>
        public static void GetSwitchSystemFunctionStatus(SystemFunctionSwitchEnum systemFunction, Action<int> callback, int ext=0)
        {
            PXR_EnterprisePlugin.UPxr_GetSwitchSystemFunctionStatus(systemFunction, callback,ext);
        }

        /// <summary>
        /// Gets the configured USB mode.
        /// </summary>
        /// <returns>
        /// * `MTP`: MTP mode
        /// * `CHARGE`: charging mode
        /// </returns>
        public static string GetUsbConfigurationOption(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_SwitchGetUsbConfigurationOption(ext);
        }

        /// <summary>
        /// Gets the current launcher.
        /// @note Supported by PICO Neo3 Pro (system version 4.8.0 or later) and PICO 4 Enterprise (system version OTA0-5.2.8 or later).
        /// </summary>
        /// <returns>The package name or class name of the launcher.</returns>
        public static string GetCurrentLauncher(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetCurrentLauncher(ext);
        }

        /// <summary>
        /// Initializes the screencast service.
        /// @note Supported by PICO Neo3 Pro (system version 4.8.0 or later) and PICO 4 Enterprise (system version OTA0-5.2.8 or later).
        /// </summary>
        /// <param name="callback">The callback:
        /// * `0`: disconnect
        /// * `1`: connect
        /// * `2`: no microphone permission
        /// </param>
        /// <returns>
        /// * `0`: failure
        /// * `1`: success
        /// Returns `0` when there is no microphone permission.
        /// </returns>
        public static int PICOCastInit(Action<int> callback, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_PICOCastInit(callback,ext);
        }

        /// <summary>
        /// Sets whether to show the screencast authorization window.
        /// @note Supported by PICO Neo3 Pro (system version 4.8.0 or later) and PICO 4 Enterprise (system version OTA0-5.2.8 or later).
        /// </summary>
        /// <param name="authZ">
        /// * `0`: ask every time (default)
        /// * `1`: always allow
        /// * `2`: not accepted
        /// </param>
        /// <returns>
        /// * `0`: failure
        /// * `1`: success
        /// </returns>
        public static int PICOCastSetShowAuthorization(int authZ, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_PICOCastSetShowAuthorization(authZ,ext);
        }

        /// <summary>
        /// Gets the setting of whether to show the screencast authorization window.
        /// @note Supported by PICO Neo3 Pro (system version 4.8.0 or later) and PICO 4 Enterprise (system version OTA0-5.2.8 or later).
        /// </summary>
        /// <returns>
        /// * `0`: ask every time (default)
        /// * `1`: always allow
        /// * `2`: not accepted
        /// </returns>
        public static int PICOCastGetShowAuthorization(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_PICOCastGetShowAuthorization(ext);
        }

        /// <summary>
        /// Gets the URL for screencast.
        /// @note Supported by PICO Neo3 Pro (system version 4.8.0 or later) and PICO 4 Enterprise (system version OTA0-5.2.8 or later).
        /// </summary>
        /// <param name="urlType">The enumerations of URL type:
        /// * `NormalURL`: Normal URL. The screencast authorization window will show if it is not set.
        /// * `NoConfirmURL`: Non-confirm URL. The screencast authorization window will not show in the browser. Screencast will start once you enter the URL.
        /// * `RtmpURL`: Returns the RTMP live streaming URL. The screencast authorization window will not appear on the VR headset's screen.
        /// </param>
        /// <returns>The URL for screencast.</returns>
        public static string PICOCastGetUrl(PICOCastUrlTypeEnum urlType, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_PICOCastGetUrl(urlType,ext);
        }

        /// <summary>
        /// Stops screencast.
        /// @note Supported by PICO Neo3 Pro (system version 4.8.0 or later) and PICO 4 Enterprise (system version OTA0-5.2.8 or later).
        /// </summary>
        /// <returns>
        /// * `0`: failure
        /// * `1`: success
        /// </returns>
        public static int PICOCastStopCast(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_PICOCastStopCast(ext);
        }

        /// <summary>
        /// sets screencast options.
        /// @note Supported by PICO Neo3 Pro (system version 4.8.0 or later) and PICO 4 Enterprise (system version OTA0-5.2.8 or later).
        /// </summary>
        /// <param name="castOptionOrStatus">The enumerations of the property to set:
        /// * `OPTION_RESOLUTION_LEVEL`: resolution level
        /// * `OPTION_BITRATE_LEVEL`: bitrate level
        /// * `OPTION_AUDIO_ENABLE`: whether to enable the audio
        /// </param>
        /// <param name="castOptionValue">The values that can be set for each property:
        /// * For `OPTION_RESOLUTION_LEVEL`:
        ///   * `OPTION_VALUE_RESOLUTION_HIGH`
        ///   * `OPTION_VALUE_RESOLUTION_MIDDLE`
        ///   * `OPTION_VALUE_RESOLUTION_AUTO`
        ///   * `OPTION_VALUE_RESOLUTION_HIGH_2K`
        ///   * `OPTION_VALUE_RESOLUTION_HIGH_4K`
        /// * For `OPTION_BITRATE_LEVEL`:
        ///   * `OPTION_VALUE_BITRATE_HIGH`
        ///   * `OPTION_VALUE_BITRATE_MIDDLE`
        ///   * `OPTION_VALUE_BITRATE_LOW`
        /// * For `OPTION_AUDIO_ENABLE`:
        ///   * `OPTION_VALUE_AUDIO_ON`
        ///   * `OPTION_VALUE_AUDIO_OFF`
        /// </param>
        /// <returns>
        /// * `0`: failure
        /// * `1`: success
        /// </returns>
        public static int PICOCastSetOption(PICOCastOptionOrStatusEnum castOptionOrStatus, PICOCastOptionValueEnum castOptionValue, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_PICOCastSetOption(castOptionOrStatus, castOptionValue,ext);
        }

        /// <summary>
        /// Gets the screencast settings for the current device.
        /// @note Supported by PICO Neo3 Pro (system version 4.8.0 or later) and PICO 4 Enterprise (system version OTA0-5.2.8 or later).
        /// </summary>
        /// <param name="castOptionOrStatus">The enumerations of the screencast property to get setting for:
        /// * `OPTION_RESOLUTION_LEVEL`: resolution level
        /// * `OPTION_BITRATE_LEVEL`: bitrate level
        /// * `OPTION_AUDIO_ENABLE`: whether the audio is enabled
        /// * `PICOCAST_STATUS`: returns the current screencast status
        /// </param>
        /// <returns>The setting of the selected property:
        /// * For `OPTION_RESOLUTION_LEVEL`:
        ///   * `OPTION_VALUE_RESOLUTION_HIGH`
        ///   * `OPTION_VALUE_RESOLUTION_MIDDLE`
        ///   * `OPTION_VALUE_RESOLUTION_AUTO`
        ///   * `OPTION_VALUE_RESOLUTION_HIGH_2K`
        ///   * `OPTION_VALUE_RESOLUTION_HIGH_4K`
        /// * For `OPTION_BITRATE_LEVEL`:
        ///   * `OPTION_VALUE_BITRATE_HIGH`
        ///   * `OPTION_VALUE_BITRATE_MIDDLE`
        ///   * `OPTION_VALUE_BITRATE_LOW`
        /// * For `OPTION_AUDIO_ENABLE`:
        ///   * `OPTION_VALUE_AUDIO_ON`
        ///   * `OPTION_VALUE_AUDIO_OFF`
        /// * `PICOCAST_STATUS` :
        ///   * `STATUS_VALUE_STATE_STARTED`
        ///   * `STATUS_VALUE_STATE_STOPPED`
        ///   * `STATUS_VALUE_ERROR`
        /// </returns>
        public static PICOCastOptionValueEnum PICOCastGetOptionOrStatus(PICOCastOptionOrStatusEnum castOptionOrStatus, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_PICOCastGetOptionOrStatus(castOptionOrStatus,ext);
        }

        /// <summary>Sets the system language for the device. 
        /// For a language that is spoken in different countries/regions, the system language is then co-set by the language code and the device's country/region code. 
        /// For example, if the language code is set to `en` and the device's country/region code is `US`, the system language will be set to English (United States).</summary>
        /// @note Supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later.
        ///
        /// <param name="language">Supported language codes:
        /// * `cs`: Czech
        /// * `da`: Danish
        /// * `de`: German
        /// * `el`: Greek
        /// * `en`: English (United States / United Kingdom)
        /// * `es`: Spanish
        /// * `fi`: Finnish
        /// * `fr`: French
        /// * `it`: Italian
        /// * `ja`: Japanese
        /// * `ko`: Korean
        /// * `ms`: Malay
        /// * `nb`: Norwegian
        /// * `nl`: Dutch
        /// * `pl`: Polish
        /// * `pt`: Portuguese (Brazil / Portugal)
        /// * `ro`: Romanian
        /// * `ru`: Russian
        /// * `sv`: Swedish
        /// * `th`: Thai
        /// * `tr`: Turkish
        /// * `zh`: Chinese (Simplified) / Chinese (Hong Kong SAR of China) / Chinese (Traditional)
        /// For devices in Mainland China / Taiwan, China / Hong Kong SAR of China / Macao SAR of China, the country/region code has been defined in factory settings.
        /// </param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// * `22`: invalid language
        /// </returns>
        public static int SetSystemLanguage(String language, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_SetSystemLanguage(language,ext);
        }

        /// <summary>Gets the device's system language.</summary>
        /// @note Supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later.
        ///
        /// <returns>The system language set for the device. For details, refer to the 
        /// parameter description for `SetSystemLanguage`.</returns>
        public static String GetSystemLanguage(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetSystemLanguage(ext);
        }

        /// <summary>Sets a default Wi-Fi network for the device. Once set, the device will automatically connect to the Wi-Fi network if accessible.</summary>
        /// @note Supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later.
        /// 
        /// <param name="ssid">The SSID (name) of the Wi-Fi network.</param>
        /// <param name="pwd">The password of the Wi-Fi network.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int ConfigWifi(String ssid, String pwd, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_ConfigWifi(ssid, pwd,ext);
        }

        /// <summary>Gets the device's default Wi-Fi network.</summary>
        /// @note Supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later.
        /// 
        /// <returns>The SSID (name) of the Wi-Fi network.</returns>
        public static String[] GetConfiguredWifi(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetConfiguredWifi(ext);
        }

        /// <summary>Sets a country/region for the device.</summary>
        /// @note Supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later.
        /// 
        /// <param name="countryCode">The country/region code co-determines the device's system language with the language code you set via `SetSystemLanguage`.
        /// Below are supported country/region codes:
        /// * `AD`: Andorra
        /// * `AT`: Austria
        /// * `AU`: Australia
        /// * `BE`: Belgium
        /// * `BG`: Bulgaria
        /// * `CA`: Canada
        /// * `CH`: Switzerland
        /// * `CZ`: Czech Republic
        /// * `DE`: Germany
        /// * `DK`: Denmark
        /// * `EE`: Estonia
        /// * `ES`: Spain
        /// * `FI`: Finland
        /// * `FR`: France
        /// * `GB`: the Great Britain
        /// * `GR`: Greece
        /// * `HR`: Croatia
        /// * `HU`: Hungary
        /// * `IE`: Ireland
        /// * `IL`: Israel
        /// * `IS`: Iceland
        /// * `IT`: Italy
        /// * `JP`: Japan
        /// * `KR`: Korea
        /// * `LI`: Liechtenstein
        /// * `LT`: Lithuania
        /// * `LU`: Luxembourg
        /// * `LV`: Latvia
        /// * `MC`: Monaco
        /// * `MT`: Malta
        /// * `MY`: Malaysia
        /// * `NL`: Netherlands
        /// * `NO`: Norway
        /// * `NZ`: New Zealand
        /// * `PL`: Poland
        /// * `PT`: Portugal
        /// * `RO`: Romania
        /// * `SE`: Sweden
        /// * `SG`: Singapore
        /// * `SI`: Slovenia
        /// * `SK`: Slovakia
        /// * `SM`: San Marino
        /// * `TR`: Turkey
        /// * `US`: the United States
        /// * `VA`: Vatican
        /// </param>
        /// <param name="callback">Set the callback to get the result:
        /// * `0`: success
        /// * `1`: failure
        /// </param>
        public static int SetSystemCountryCode(String countryCode, Action<int> callback, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_SetSystemCountryCode(countryCode, callback,ext);
        }

        /// <summary>Gets the device's country/region code.</summary>
        /// @note Supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later.
        ///
        /// <returns>A string value that indicates the device's current country/region code. 
        /// For supported country/region codes, see the parameter description in `SetSystemCountryCode`.</returns>
        public static string GetSystemCountryCode(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetSystemCountryCode(ext);
        }

        /// <summary>Sets the page to skip in initialization settings.</summary>
        /// @note Supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later.
        ///
        /// <param name="flag">Set the flag.
        /// The first 6 bits are valid, the 7th to 32rd bits are reserved. For each bit, `0` indicates showing and `1` indicates hiding.
        /// * `Constants#INIT_SETTING_HANDLE_CONNECTION_TEACHING`: the controller connection tutorial page
        /// * `Constants#INIT_SETTING_TRIGGER_KEY_TEACHING`: the Trigger button tutorial page
        /// * `Constants#INIT_SETTING_SELECT_LANGUAGE`: the language selection page
        /// * `Constants#INIT_SETTING_SELECT_COUNTRY`: the country/region selection page. Only available for devices in non-Mainland China countries/regions.
        /// * `Constants#INIT_SETTING_WIFI_SETTING`: the Wi-Fi settings page
        /// * `Constants#INIT_SETTING_QUICK_SETTING`: the quick settings page
        /// </param>
        /// Below is an example implementation:
        /// ```csharp
        /// int flag = Constants.INIT_SETTING_HANDLE_CONNECTION_TEACHING | Constants.INIT_SETTING_TRIGGER_KEY_TEACHING;
        /// int result = serviceBinder.pbsSetSkipInitSettingPage(flag,0);
        /// ```
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int SetSkipInitSettingPage(int flag, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_SetSkipInitSettingPage(flag,ext);
        }

        /// <summary>Gets the page to skip in initialization settings.</summary>
        /// @note Supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later.
        ///
        /// <returns>Returns the flag set in `SetSkipInitSettingPage`.</returns>
        public static int GetSkipInitSettingPage(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetSkipInitSettingPage(ext);
        }

        /// <summary>Gets whether the initialization settings have been complete.</summary>
        /// @note Supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later.
        ///
        /// <returns> 
        /// * `0`: not complete
        /// * `1`: complete
        /// </returns>
        public static int IsInitSettingComplete(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_IsInitSettingComplete(ext);
        }

        /// <summary>Starts an activity in another app.</summary>
        /// @note Supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later.
        /// <param name="packageName">(Optional) The app's package name.</param>
        /// <param name="className">(Optional) The app's class name.</param>
        /// <param name="action">(Optional) The action to be performed.</param>
        /// <param name="extra">The basic types of standard fields that can be used as extra data.</param> 
        /// <param name="categories">Standard categories that can be used to further clarify an Intent. Add a new category to the intent.</param>
        /// <param name="flags">Add additional flags to the intent.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int StartActivity(String packageName, String className, String action, String extra, String[] categories, int[] flags, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_StartActivity(packageName, className, action, extra, categories, flags,ext);
        }

        /// <summary>Shows/hides specified app(s) in the library.
        /// @note Supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later.
        /// </summary>
        /// <param name="packageNames">Package name(s). If there are multiple names, use commas (,) to separate them.</param>
        /// <param name="switchEnum">Specifies to show/hide the app(s), enums:
        /// * `S_ON`: show
        /// * `S_OFF`: hide
        /// </param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int CustomizeAppLibrary(String[] packageNames, SwitchEnum switchEnum, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_CustomizeAppLibrary(packageNames, switchEnum,ext);
        }

        /// <summary>
        /// Gets the controller's battery level.
        /// @note Supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later.
        /// </summary>
        /// <returns>Returns the following information: 
        /// * array[0]: the left controller's battery level
        /// * array[1]: the right controller's battery level
        /// * an integer from 1 to 5, which indicates the battery level, the bigger the integer, the higher the battery level
        /// </returns>
        public static int[] GetControllerBattery(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetControllerBattery(ext);
        }

        /// <summary>
        /// Gets the controller's connection status.
        /// @note Supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later.
        /// </summary>
        /// <returns>
        /// * `0`: both controllers are disconnected
        /// * `1`: the left controller is connected
        /// * `2`: the right controller is connected
        /// * `3`: both controllers are connected
        /// </returns>
        public static int GetControllerConnectState(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetControllerConnectState(ext);
        }

        /// <summary>
        /// Gets the apps that are hidden in the library.
        /// @note Supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later.
        /// </summary>
        /// <returns>The packages names of hidden apps. Multiple names are separated by commas (,).</returns>
        public static string GetAppLibraryHideList(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetAppLibraryHideList(ext);
        }

        /// <summary>
        /// Sets the device that outputs audio during screen casting. 
        /// @note
        /// - Supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.5.0 or later.
        /// - This API is only for miracast.
        /// </summary>
        /// <param name="screencastAudioOutput">Specifies the device that outputs audio. Enumerations:
        /// * `AUDIO_SINK`: the HMD
        /// * `AUDIO_TARGET`: the receiver
        /// * `AUDIO_SINK_TARGET`: both the HMD and the receiver
        /// </param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int SetScreenCastAudioOutput(ScreencastAudioOutputEnum screencastAudioOutput, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_SetScreenCastAudioOutput(screencastAudioOutput,ext);
        }

        /// <summary>
        /// Gets the device that outputs audio during screen casting.
        /// @note
        /// - Supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.5.0 or later.
        /// - This API is only for miracast.
        /// </summary>
        /// <returns>
        /// Enumerations:
        /// * `AUDIO_SINK`: the HMD
        /// * `AUDIO_TARGET`: the receiver
        /// * `AUDIO_SINK_TARGET`: both the HMD and the receiver
        /// </returns>
        public static ScreencastAudioOutputEnum GetScreenCastAudioOutput(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetScreenCastAudioOutput(ext);
        }

        /// <summary>
        /// Displays or hides the specified tab or option on the Settings pane.
        /// @note Supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.5.0 or later.
        /// </summary>
        /// <param name="customizeSettingsTabEnum">Specifies the tab or option to display or hide. Enumerations:
        /// * `CUSTOMIZE_SETTINGS_TAB_WLAN`: the "WLAN" tab
        /// * `CUSTOMIZE_SETTINGS_TAB_CONTROLLER`: the "Controller" tab
        /// * `CUSTOMIZE_SETTINGS_TAB_BLUETOOTH`: the "Bluetooth" tab
        /// * `CUSTOMIZE_SETTINGS_TAB_DISPLAY`: the "Display" tab
        /// * `CUSTOMIZE_SETTINGS_TAB_LAB`: the "LAB" tab
        /// * `CUSTOMIZE_SETTINGS_TAB_GENERAL_FACTORY_RESET`: the "Factory Reset" option on the "General" tab
        /// * `CUSTOMIZE_SETTINGS_TAB_GENERAL_LOCKSCREEN`: the "Lock Screen" option on the "General" tab
        /// </param>
        /// <param name="switchEnum">Sets to display or hide the specified tab or option:
        /// * `S_ON`: display
        /// * `S_OFF`: hide
        /// </param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int UPxr_CustomizeSettingsTabStatus(CustomizeSettingsTabEnum customizeSettingsTabEnum, SwitchEnum switchEnum, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_CustomizeSettingsTabStatus(customizeSettingsTabEnum, switchEnum,ext);
        }

        /// <summary>
        /// Gets the status set for the specified tab or option on the Settings pane.
        /// @note Supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.5.0 or later.
        /// </summary>
        /// <param name="customizeSettingsTabEnum">Specifies the tab or option to get status for. Enumerations:
        /// * `CUSTOMIZE_SETTINGS_TAB_WLAN`: the "WLAN" tab
        /// * `CUSTOMIZE_SETTINGS_TAB_CONTROLLER`: the "Controller" tab
        /// * `CUSTOMIZE_SETTINGS_TAB_BLUETOOTH`: the "Bluetooth" tab
        /// * `CUSTOMIZE_SETTINGS_TAB_DISPLAY`: the "Display" tab
        /// * `CUSTOMIZE_SETTINGS_TAB_LAB`: the "LAB" tab
        /// * `CUSTOMIZE_SETTINGS_TAB_GENERAL_FACTORY_RESET`: the "Factory Reset" option on the "General" tab
        /// * `CUSTOMIZE_SETTINGS_TAB_GENERAL_LOCKSCREEN`: the "Lock Screen" option on the "General" tab
        /// </param>
        /// <returns>
        /// The status of the specified tab or option:
        /// * `S_ON`: displayed
        /// * `S_OFF`: hidden
        /// </returns>
        public static SwitchEnum UPxr_GetCustomizeSettingsTabStatus(CustomizeSettingsTabEnum customizeSettingsTabEnum, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetCustomizeSettingsTabStatus(customizeSettingsTabEnum,ext);
        }
        
        /// <summary>
        /// Shuts down the PICO device when the USB plug is unplugged or the plug runs out of power.
        /// </summary>
        /// <param name="switchEnum">Determines whether to enable/disable this function:
        /// * `S_ON`: enable
        /// * `S_OFF`: disable
        /// </param>
        public static void SetPowerOffWithUSBCable(SwitchEnum switchEnum, int ext=0)
        {
             PXR_EnterprisePlugin.UPxr_SetPowerOffWithUSBCable(switchEnum,ext);
        }
        /// <summary>
        /// Removes a specific Home key event setting, which restores the event to its default setting.
        /// </summary>
        /// <param name="switchEnum">Specify a Home key event from the following:
        /// `PBS_HomeEventEnum. SINGLE_CLICK`: single-click event
        /// `PBS_HomeEventEnum. DOUBLE_CLICK`: double-click event
        /// `PBS_HomeEventEnum. LONG_PRESS`: long press event
        /// `PBS_HomeEventEnum. SINGLE_CLICK_RIGHT_CTL`: single-click on the right controller's Home button
        /// `PBS_HomeEventEnum. DOUBLE_CLICK_RIGHT_CTL`: double-click on the right controller's Home button
        /// `PBS_HomeEventEnum. LONG_PRESS_RIGHT_CTL`: long press on the right controller's Home button
        /// `PBS_HomeEventEnum. SINGLE_CLICK_LEFT_CTL`: single-click on the left controller's Home button
        /// `PBS_HomeEventEnum. DOUBLE_CLICK_LEFT_CTL`: double-click on the left controller's Home button
        /// `PBS_HomeEventEnum. LONG_PRESS_LEFT_CTL`: long press on the left controller's Home button
        /// `PBS_HomeEventEnum. SINGLE_CLICK_HMD`: single-click on the HMD's Home button
        /// `PBS_HomeEventEnum. DOUBLE_CLICK_HMD`: double-click on the HMD's Home button
        /// `PBS_HomeEventEnum. LONG_PRESS_HMD`: long press on the HMD's Home button
        /// </param>
        public static void RemoveControllerHomeKey(HomeEventEnum EventEnum)
        {
            PXR_EnterprisePlugin.UPxr_RemoveControllerHomeKey(EventEnum);
        }
        
        /// <summary>
        /// Sets the power on logo or the power on/off animation.
        /// </summary>
        /// <param name="powerOnOffLogoEnum">Specify a setting from the following:
        /// * `PBS_PowerOnOffLogoEnum. PLPowerOnLogo`: sets a logo for the first frame after powering on the device
        /// * `PBS_PowerOnOffLogoEnum. PLPowerOnAnimation`: sets the power on animation
        /// * `PBS_PowerOnOffLogoEnum. PLPowerOffAnimation`: sets the power off animation
        /// </param>
        /// <param name="path">
        /// * For setting a logo for the first frame after powering on the device, pass the path where the .img file is stored, for example, `/sdcard/bootlogo.img`.
        /// * For setting the power on/off animation, pass the folder where the pictures composing the animation is stored.
        /// </param>
        /// <param name="callback">Result callback:
        /// * `true`: success
        /// * `false`: failure
        /// </param>
        public static void SetPowerOnOffLogo(PowerOnOffLogoEnum powerOnOffLogoEnum, String path, Action<bool> callback, int ext=0)
        {
            PXR_EnterprisePlugin.UPxr_SetPowerOnOffLogo(powerOnOffLogoEnum,path,callback,ext);
        }
        /// <summary>
        /// Sets an interpupillary distance (IPD).
        /// @note Supported by PICO 4 Enterprise with system version 5.7.0 or later.
        /// </summary>
        /// <param name="ipd">
        /// The IPD to set. Valid value range: [62,72]. Unit: millimeters.
        /// </param>
        /// <param name="callback">Result callback:
        /// * `0`: success
        /// * `1`: failure
        /// * `23`: the `ipd` value is out of the valid range
        /// </param>
        public static void SetIPD(float ipd, Action<int> callback)
        {
            PXR_EnterprisePlugin.UPxr_SetIPD(ipd,callback);
        }
        
        /// <summary>
        /// Gets the device configured for miracast.
        /// </summary>
        /// <returns>
        /// The name of the device.
        /// </returns>
        public static string GetAutoMiracastConfig(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetAutoMiracastConfig(ext);
        }
        
        /// <summary>
        /// Sets screencast-related parameters.
        /// @note Supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.7.0 or later.
        /// </summary>
        /// <param name="mediaFormat">
        /// The mediaFormat object to set. Currently, only support settings the bitrate.
        /// </param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int SetPicoCastMediaFormat(PicoCastMediaFormat mediaFormat, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_SetPicoCastMediaFormat(mediaFormat,ext);
        }
        
        /// <summary>
        /// Gets the pose and ID of the marker.
        /// @note Supported by 6Dof devices.
        /// </summary>
        /// <param name="trackingMode">Specify a tracking origin mode from the following:
        /// * `TrackingOriginModeFlags.Device`: Device mode. The system sets the device's initial position as the origin. The device's height from the floor is not calculated.
        /// * `TrackingOriginModeFlags.Floor`: Floor mode. The system sets an origin based on the device's original position and the device's height from the floor. 
        /// </param>
        /// <param name="cameraYOffset">
        /// Set the offset added to the camera's Y direction, which is for simulating a user's height and is only applicable if you select the 'Device' mode.
        /// </param>
        /// <param name="markerInfos">
        /// The callback function for returning marker information.
        /// </param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int SetMarkerInfoCallback(TrackingOriginModeFlags trackingMode,float cameraYOffset,Action<List<MarkerInfo>> markerInfos)
        {
            return PXR_EnterprisePlugin.UPxr_setMarkerInfoCallback(trackingMode,cameraYOffset,markerInfos);
        }

        /// <summary>
        /// Opens the RGB camera.
        /// </summary>
        /// <returns>Whether the RGB camera has been opened:
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool OpenVSTCamera()
        {
            return PXR_EnterprisePlugin.UPxr_OpenVSTCamera();
        }

        /// <summary>
        /// Closes the RGB camera.
        /// </summary>
        /// <returns>Whether the RGB camera has been closed:
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool CloseVSTCamera()
        {
            return PXR_EnterprisePlugin.UPxr_CloseVSTCamera();
        }

        /// <summary>
        /// Gets camera parameters (including intrinsics & extrinsics).
        /// </summary>
        /// <returns> RGBCameraParams including intrinsics and extrinsics.
        /// </returns>
        public static RGBCameraParams GetCameraParameters()
        {
            return PXR_EnterprisePlugin.UPxr_GetCameraParameters();
        }

        /// <summary>
        /// Gets the current head tracking confidence.
        /// </summary>
        /// <returns>
        /// * `0`: bad
        /// * `1`: good
        /// </returns>
        public static int GetHeadTrackingConfidence()
        {
            return PXR_EnterprisePlugin.UPxr_GetHeadTrackingConfidence();
        }

        /// <summary>
        /// Acquires RGB camera frame (the original image before anti-distortion).
        /// </summary>
        /// <param name="frame">Frame info.</param>
        /// <returns>
        /// Returns '0' for success and other values for failure.
        /// </returns>
        public static int AcquireVSTCameraFrame(out Frame frame)
        {
            return PXR_EnterprisePlugin.UPxr_AcquireVSTCameraFrame(out frame);
        }

        /// <summary>
        /// Acquires RGB camera frame (the image after anti-distortion).
        /// </summary>
        /// <param name="width">Desired frame width, should not exceed 2328.</param>
        /// <param name="height">Desired frame height, should not exceed 1748.</param>
        /// <param name="frame">Frame info.</param>
        /// <returns>
        /// Returns '0' for success and other values for failure.
        /// </returns>
        public static int AcquireVSTCameraFrameAntiDistortion(int width, int height, out Frame frame)
        {
            return PXR_EnterprisePlugin.UPxr_AcquireVSTCameraFrameAntiDistortion(width, height, out frame);
        }

        /// <summary>
        /// Gets the predicted time when the VST image is to be displayed.
        /// <returns>The predicted display time.</returns>
        public static double GetPredictedDisplayTime()
        {
            return PXR_EnterprisePlugin.UPxr_GetPredictedDisplayTime();
        }

        /// <summary>
        /// Gets the predicted pose and status of the main sensor when the VST image is being displayed.
        /// </summary>
        /// <param name="predictTime">Predict time.</param>
        /// <returns>The predicted status of the sensor.</returns>
        public static SensorState GetPredictedMainSensorState(double predictTime)
        {
            return PXR_EnterprisePlugin.UPxr_GetPredictedMainSensorState(predictTime);
        }

        /// <summary>
        /// Directs the user to the floor-height-adjustment app to adjust the floor's height.
        /// @note Supported by PICO Neo3 Pro, general PICO Neo3 devices activated as enterprise devices, and PICO 4 Enterprise.
        /// </summary>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int GotoSeeThroughFloorSetting(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_gotoSeeThroughFloorSetting(ext);
        }

        /// <summary>
        /// Copies a file or a folder from the source path to the destination path.
        /// @note Supported by PICO Neo3 Pro, general PICO Neo3 devices activated as enterprise devices, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="srcPath">
        /// The source path of the file or folder.
        /// * For mobile storage devices, the prefix of the path is 'udisk://'. For example, the path of the Movie folder under the root directory should be passed as 'udisk://Movie'.
        /// * For internal storage paths, directly specify the path under the root directory. For example, the path of the Picture folder under the root directory should be passed as 'Picture'.
        /// </param>
        /// <param name="dstPath">
        /// The destination path that the file or folder is copied to.
        /// * For mobile storage devices, the prefix of the path is 'udisk://'. For example, the path of the Movie folder under the root directory should be passed as 'udisk://Movie'.
        /// * For internal storage paths, directly write the path under the root directory. For example, the path of the Picture folder under the root directory should be passed as 'Picture'.
        /// </param>
        /// <param name="callback">The result callback:
        /// * `onCopyStart`: copy start callback
        /// * `onCopyProgress(double process)`: copy progress callback, value range:[0.00, 1.00]
        /// * `onCopyFinish(int errorCode)`: `0` (copy succeeded); `101` (USB flash disk is not connected); `103` (insufficient storage space in the target device); `104` (copy failed)
        /// </param>
        /// <returns>
        /// * `0`: API call succeeded, wait for copy to start
        /// * `101`: USB flash drive is not connected
        /// * `102`: source file/folder does not exist
        /// * `106`: null parameter
        /// </returns>
        public static int FileCopy(String srcPath, String dstPath, FileCopyCallback callback)
        {
            return PXR_EnterprisePlugin.UPxr_fileCopy(srcPath, dstPath, callback);
        }

        /// <summary>
        /// Checks whether a map is being used.
        /// @note Supported by PICO Neo3 Pro, general PICO Neo3 devices activated as enterprise devices, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="path">The path of the map's zip file.</param>
        /// <param name="callback">The result callback:
        /// * `0`: success
        /// * `1`: failure
        /// * `101`: file does not exist
        /// * `102`: failed to unzip the file
        /// * `103`: file corruption
        /// * `104`: position tracking is disabled
        /// * `106`: failed to get the current map's information
        /// * `107`: `path` parameter is null
        /// </param>
        public static void IsMapInEffect(String path, Action<int> callback, int ext=0)
        {
            PXR_EnterprisePlugin.UPxr_IsMapInEffect(path, callback,ext);
        }

        /// <summary>
        /// Imports a map.
        /// @note Supported by PICO Neo3 Pro, general PICO Neo3 devices activated as enterprise devices, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="path">The path of the map's zip file.</param>
        /// <param name="callback">The result callback:
        /// * `0`: success
        /// * `1`: failure
        /// * `101`: file does not exist
        /// * `102`: failed to unzip the file
        /// * `103`: file corruption
        /// * `104`: position tracking is disabled
        /// * `107`: `path` parameter is null
        /// </param>
        public static void ImportMapByPath(String path, Action<int> callback, int ext=0)
        {
            PXR_EnterprisePlugin.UPxr_ImportMapByPath(path, callback,ext);
        }
        
        /// <summary>Sets a name for the WiFi P2P device.</summary>
        /// <param name="devicename">Device name. The maximum length is 30.</param>
        /// <param name="callback">Result callback:
        /// * `0`: success
        /// * `1`: failure
        /// * `101`: `deviceName` param is null
        /// * `102`: the length of `deviceName` param exceeds the maximum length allowed
        /// </param>
        /// <param name="ext">Extension. Pass `0`.</param>
        public static void  SetWifiP2PDeviceName(String deviceName, Action<int> callback, int ext=0)
        {
            PXR_EnterprisePlugin.UPxr_SetWifiP2PDeviceName(deviceName, callback,ext);
        }
        
        /// <summary>Gets the WiFi P2P device's name.</summary>
        /// <param name="ext">Extension. Pass `0`.</param>
        /// <returns>The device's name.</param>
        public static string GetWifiP2PDeviceName(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetWifiP2PDeviceName(ext);
        }

        /// <summary>Set screen brightness.</summary>
        /// <param name="brightness">Specify the brightness of the screen. Value range:[0,255].</param>
        /// <param name="ext">Extension. Pass `0`.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// * `101`: the specified brightness is out of the allowed value range
        /// </returns>
        public static int SetScreenBrightness(int brightness, int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_SetScreenBrightness(brightness,ext);
        }

        /// <summary>Switches specified system function on/off.</summary>
        /// <param name="systemFunction">Function name. Enumerations:
        /// * `SFS_USB`: USB debugging
        /// * `SFS_AUTOSLEEP`: auto sleep
        /// * `SFS_SCREENON_CHARGING`: screen-on charging
        /// * `SFS_OTG_CHARGING`: OTG charging (supported by G2 devices)
        /// * `SFS_RETURN_MENU_IN_2DMODE`: display the Return icon on the 2D screen
        /// * `SFS_COMBINATION_KEY`: combination key
        /// * `SFS_CALIBRATION_WITH_POWER_ON`: calibration with power on
        /// * `SFS_SYSTEM_UPDATE`: system update (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_CAST_SERVICE`: phone casting service
        /// * `SFS_EYE_PROTECTION`: eye-protection mode
        /// * `SFS_SECURITY_ZONE_PERMANENTLY`: permanently disable the 6DoF play area (supported by PICO Neo2 devices)
        /// * `SFS_GLOBAL_CALIBRATION`: global calibration (supported by PICO G2 devices)
        /// * `SFS_Auto_Calibration`: auto calibration
        /// * `SFS_USB_BOOT`: USB plug-in boot
        /// * `SFS_VOLUME_UI`: global volume UI (need to restart the device to make the setting take effect)
        /// * `SFS_CONTROLLER_UI`: global controller connected UI
        /// * `SFS_NAVGATION_SWITCH`: navigation bar
        /// * `SFS_SHORTCUT_SHOW_RECORD_UI`: screen recording button UI
        /// * `SFS_SHORTCUT_SHOW_FIT_UI`: PICO fit UI
        /// * `SFS_SHORTCUT_SHOW_CAST_UI`: screencast button UI
        /// * `SFS_SHORTCUT_SHOW_CAPTURE_UI`: screenshot button UI
        /// * `SFS_USB_FORCE_HOST`: set the Neo3 Pro/Pro Eye device as the host device
        /// * `SFS_SET_DEFAULT_SAFETY_ZONE`: set a default play area for PICO Neo3 and PICO 4 series devices
        /// * `SFS_ALLOW_RESET_BOUNDARY`: allow to reset customized boundary for PICO Neo3 series devices
        /// * `SFS_BOUNDARY_CONFIRMATION_SCREEN`: whether to display the boundary confirmation screen for PICO Neo3 and PICO 4 series devices
        /// * `SFS_LONG_PRESS_HOME_TO_RECENTER`: long press the Home button to recenter for PICO Neo3 and PICO 4 series devices
        /// * `SFS_POWER_CTRL_WIFI_ENABLE`: stay connected to the network when the device sleeps/turns off (supported by PICO Neo3 Pro with system version 4.8.0 or later and PICO 4 Enterprise with system version OTA-5.2.8 or later)
        /// * `SFS_WIFI_DISABLE`: disable Wi-Fi (supported by PICO Neo3 Pro with system version 4.8.0 or later and PICO 4 Enterprise with system version OTA-5.2.8 or later)
        /// * `SFS_SIX_DOF_SWITCH`: 6DoF position tracking for PICO Neo3 and PICO 4 series devices
        /// * `SFS_INVERSE_DISPERSION`: anti-dispersion (supported by PICO Neo3 Pro with system version 4.8.0 or later and PICO 4 Enterprise with system version OTA0-5.2.8 or later)
        /// * `SFS_LOGCAT`: system log switch (/data/logs) (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_PSENSOR`: PSensor switch (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SYSTEM_UPDATE_OTA`: OTA upgrade (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SYSTEM_UPDATE_APP`: app upgrade and update (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_WLAN_UI`: quickly set whether to show the WLAN button (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_BOUNDARY_UI`: quickly set whether to show the boundary button (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_BLUETOOTH_UI`: quickly set whether to show the bluetooth button (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_CLEAN_TASK_UI`: quickly set whether to show the one-click clear button (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_IPD_ADJUSTMENT_UI`: quickly set whether to show the IPD adjustment button (supported by PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_POWER_UI`: quickly set whether to show the power button (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_SHORTCUT_SHOW_EDIT_UI`: quickly set whether to show the edit button (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_BASIC_SETTING_APP_LIBRARY_UI`: the button for customizing the app library (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_BASIC_SETTING_SHORTCUT_UI`: the button for customizing quick settings (supported by PICO Neo3 Pro and PICO 4 Enterprise with system version 5.4.0 or later)
        /// * `SFS_LED_FLASHING_WHEN_SCREEN_OFF`: whether to keep the LED indicator light on when the device's screen is off and the battery is below 20% (supported by PICO G3 devices)
        /// * `SFS_BASIC_SETTING_CUSTOMIZE_SETTING_UI`: customize settings item to show or hide in basic settings
        /// * `SFS_BASIC_SETTING_SHOW_APP_QUIT_CONFIRM_DIALOG`: whether to show the app-quit dialog box when switching to a new app
        /// * `SFS_BASIC_SETTING_KILL_BACKGROUND_VR_APP`: whether to kill background VR apps (`1`: kill, and this is the default setting; `2`: do not kill)
        /// * `SFS_BASIC_SETTING_SHOW_CAST_NOTIFICATION`: whether to show a blue icon when casting the screen. The icon is displayed by default, and you can set the value to `0` to hide it.
        /// * `SFS_AUTOMATIC_IPD`: auto IPD switch (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.7.0 or later)
        /// * `SFS_QUICK_SEETHROUGH_MODE`: quick seethrough mode switch (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.7.0 or later)
        /// * `SFS_HIGN_REFERSH_MODE`: high refresh mode switch (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.7.0 or later)
        /// * `SFS_SEETHROUGH_APP_KEEP_RUNNING`: set whether to keep the app running under the seethrough mode (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.8.0 or later)
        /// * `SFS_OUTDOOR_TRACKING_ENHANCEMENT`: enhance outdoor position tracking (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.8.0 or later)
        /// * `SFS_AUTOIPD_AUTO_COMFIRM`: quick auto-IPD (supported by PICO 4 Enterprise with system version 5.8.0 or later)
        /// * `SFS_LAUNCH_AUTOIPD_IF_GLASSES_WEARED`: set whether to launch auto-IPD after wearing the headset (supported by PICO 4 Enterprise with system version 5.8.0 or later)
        /// * `SFS_GESTURE_RECOGNITION_HOME_ENABLE`: Home gesture switch (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.8.0 or later) 
        /// * `SFS_GESTURE_RECOGNITION_RESET_ENABLE`: enable/disable the Reset gesture (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.8.0 or later) 
        /// * `SFS_AUTO_COPY_FILES_FROM_USB_DEVICE`: automatically import OTG resources (supported by PICO Neo3 series and PICO 4 Enterprise with system version 5.8.0 or later) 
        /// * `SFS_WIFI_P2P_AUTO_CONNECT`: WiFi P2P auto connection. All devices support silent connection, and no need to add a pop-up window
        /// * `SFS_LOCK_SCREEN_FILE_COPY_ENABLE`: Enable/disable file copy when the screen is locked
        /// * `SFS_TRACKING_ENABLE_DYNAMIC_MARKER`: Enable/disable dynamic marker tracking
        /// * `SFS_ENABLE_3DOF_CONTROLLER_TRACKING`: Switch between 3DoF and 6DoF modes for controllers
        /// * `SFS_SYSTEM_VIBRATION_ENABLED`: haptic feedback (supported by OS 5.6.0 or later)
        /// * `SFS_BLUE_TOOTH`: bluetooth switch
        /// * `SFS_ENHANCED_VIDEO_QUALITY`: enhance video quality (supported by OS 5.8.0 or later)
        /// * `SFS_GESTURE_RECOGNITION`: hand tracking (supported by OS 5.6.0 or later)
        /// * `SFS_BRIGHTNESS_AUTO_ADJUST`: self-adaptive brightness (supported by OS 5.6.0 or later)
        /// * `SFS_HIGH_CURRENT_OTG_MODE`:high-current OTG mode (supported by OS 5.8.0 or later)
        /// * `SFS_BACKGROUND_APP_PLAY_AUDIO`: forbid background apps from playing audio (supported by OS 5.6.0 or later)
        /// * `SFS_NO_DISTURB_MODE`: Do Not Disturb mode (supported by OS 5.6.0 or later)
        /// * `SFS_MONOCULAR_SCREENCAST`: monocular screencast (supported by OS 5.7.0 or later)
        /// * `SFS_MONOCULAR_SCREEN_CAPTURE`: monocular screen recording or screen capturing (supported by OS 5.7.0 or later)
        /// * `SFS_STABILIZATION_FOR_RECORDING`: to reduce screen shaking in screen recording (supported by OS 5.7.0 or later)
        /// * `SFS_HIDE_2D_APP_WHEN_GO_TO_HOME`: When the primary screen app is a VR app, return to the launcher to minimize 2D apps
        /// * `SFS_CONTROLLER_VIBRATE`: the switch to enable/disable controller vibration
        /// * `SFS_REFRESH_MODE`: the switch to enable/disable refresh mode
        /// * `SFS_SMART_AUDIO`: the switch to enable/disable smart audio
        /// * `SFS_EYE_TRACK`: the switch to enable/disable eye tracking
        /// * `SFS_FACE_SIMULATE`: the switch to enable/disable face tracking
        /// * `SFS_ENABLE_MIC_WHEN_RECORD`: the switch to enable/disable microphone during screen recording
        /// * `SFS_KEEP_RECORD_WHEN_SCREEN_OFF`: whether to keep recording the screen when the screen is off
        /// * `SFS_CONTROLLER_TIP_VIBRATE`: within the boundary, the switch to enable/disable controller vibration alerts
        /// * `SFS_CONTROLLER_SEE_THROUGH`: within the boundary, the switch to enable/disable the trigger of video seethrough by controller
        /// * `SFS_LOW_BORDER_HEIGHT`: within the boundary, the switch to lower the height of the boundary
        /// * `SFS_FAST_MOVE_TIP`: within the boundary, the switch to enable/disable quick movement safety alerts
        /// * `SFS_WIRELESS_USB_ADB`: the switch to enable/disable wireless USB debugging 
        /// </param>
        /// <param name="switchEnum">Specify whether to switch the function on/off:
        /// * `S_ON`: switch on
        /// * `S_OFF`: switch off
        /// </param>
        /// <param name="callback">
        /// * `0`: success
        /// * `1`: failure
        /// * `2`: the device is not supported
        /// </param>
        /// <param name="ext">Extension. Pass `0`.</param>
        public static void SwitchSystemFunction(int systemFunction, int switchEnum, Action<int> callback,int ext=0)
        {
            PXR_EnterprisePlugin.UPxr_SwitchSystemFunction(systemFunction, switchEnum,callback,ext);
        }

        /// <summary>Sets the usability of a specified system key.</summary>
        /// <param name="key">Specify the system key. Enumerations:
        /// * `ENTER_KEY`: the Enter key
        /// * `BACK_KEY`: the Back key
        /// * `VOLUME_KEY`: the Volume key
        /// </param>
        /// <param name="usability">Specify the usability of the key:
        /// * `S_ON`: enable the key
        /// * `S_OFF`: disable the key
        /// </param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int SetSystemKeyUsability(int key, int usability)
        {
            return PXR_EnterprisePlugin.UPxr_SetSystemKeyUsability(key,usability);
        }

        /// <summary>Sets a third-party app as the launcher.</summary>
        /// <param name="packageName">Specify the package name of the app.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int SetLauncher(String packageName)
        {
            return PXR_EnterprisePlugin.UPxr_SetLauncher(packageName);
        }
 
        /// <summary>Sets a time after which the device automatically enters the sleep mode.</summary>
        /// <param name="delayTimeEnum">Specify the system sleep timeout. Enumerations:
        /// * `FIFTEEN`: 15s (only supported by PICO G2 4K)
        /// * `THIRTY`: 30s (only supported by PICO G2 4K)
        /// * `SIXTY`: 60s (only supported by PICO G2 4K)
        /// * `THREE_HUNDRED`: 5 mins
        /// * `SIX_HUNDRED`: 10 mins
        /// * `ONE_THOUSAND_AND_EIGHT_HUNDRED`: 30 mins
        /// * `Never`: never sleep
        /// </param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int SetSystemAutoSleepTime(SleepDelayTimeEnum delayTimeEnum)
        {
            return PXR_EnterprisePlugin.UPxr_SetSystemAutoSleepTime(delayTimeEnum);
        }

        /// <summary>Schedules auto startup for the device.
        /// @note Only supported by PICO Neo3 series, PICO 4 Enterprise, and PICO G3.
        /// </summary>弃用
        /// <param name="year">Specify the year, for example, `2022`.</param>
        /// <param name="month">Specify the month for example, 2.</param>
        /// <param name="day">Specify the day, for example, `22`.</param>
        /// <param name="hour">Specify the hour, for example, `22`.</param>
        /// <param name="minute">Specify the minute, for example, `22`.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns> 
        public static int OpenTimingStartup(int year, int month, int day, int hour, int minute)
        {
            return PXR_EnterprisePlugin.UPxr_OpenTimingStartup(year, month, day, hour, minute);
        }

        /// <summary>Disables scheduled auto startup for the device.
        /// @note Only supported by PICO Neo3 series, PICO 4 Enterprise, and PICO G3.
        /// </summary>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int CloseTimingStartup()
        {
            return PXR_EnterprisePlugin.UPxr_CloseTimingStartup();
        }

        /// <summary>Schedules auto shutdown for the device.
        /// @note Only supported by PICO Neo3 series, PICO 4 Enterprise, and PICO G3.
        /// </summary>
        /// <param name="year">Specify the year, for example, `2022`.</param>
        /// <param name="month">Specify the month for example, 2.</param>
        /// <param name="day">Specify the day, for example, `22`.</param>
        /// <param name="hour">Specify the hour, for example, `22`.</param>
        /// <param name="minute">Specify the minute, for example, `22`.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int OpenTimingShutdown(int year, int month, int day, int hour, int minute)
        {
            return PXR_EnterprisePlugin.UPxr_OpenTimingShutdown(year, month, day, hour, minute);
        }

        /// <summary>Disables scheduled auto shutdown for the device.
        /// @note Only supported by PICO Neo3 series, PICO 4 Enterprise, and PICO G3.
        /// </summary>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int CloseTimingShutdown()
        {
            return PXR_EnterprisePlugin.UPxr_CloseTimingShutdown();
        }

        /// <summary>Sets a time zone.</summary>
        /// <param name="timeZone">Specify the time zone. You can get the time zones supported by the current device through `TimeZone.getAvailableIDs()`.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// * `101`: the `timeZone` param is null
        /// * `102`: the specified time zone is not supported
        /// </returns>
        public static int SetTimeZone(String timeZone)
        {
            return PXR_EnterprisePlugin.UPxr_SetTimeZone(timeZone);
        }

        /// <summary>Checks whether the user has the entitlement to use the app.
        /// @note Only supported by PICO 4 Enterprise with system version 5.9.0 or later.
        /// </summary>
        /// <param name="packageName">Specify the package name of the app.</param>
        /// <param name="callback">
        /// Below is the result callback:
        /// * `1`: failed to call the API
        /// One of the following is returned when the user has the entitlement to use the app:
        /// * `100`: the queried app is not in the entitlement check list
        /// * `101`: no internet connection and no cached data of the app
        /// * `102`: the user has the entitlement to use the app
        /// * `103`: internet exception, the local cache has found that the user has the entitlement to use the app
        /// One of the following is returned when the user doesn't have the entitlement to use the app:
        /// * `102`: the user doesn't have the entitlement to use the app
        /// * `103`: internet exception, and the user doesn't have the entitlement to use the app according to the local cached data
        /// * `104`: the app's signature doesn't match the signature returned by the server
        /// * `105`: internet exception, the local cache has found a mismatch between the app signature and the one returned by the server
        /// </param>
        public static void AppCopyrightVerify(String packageName, Action<int> callback)
        {
            PXR_EnterprisePlugin.UPxr_AppCopyrightVerify(packageName,callback);
        }

        /// <summary>Goes to the environment texture check page.
        /// @note Only supported by 6DoF devices including PICO Neo3 with the enterprise mode enabled, PICO Neo3 Pro, and PICO 4 Enterprise.
        /// </summary>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// * `-1`: not supported by the device
        /// </returns>
        public static int GotoEnvironmentTextureCheck()
        {
            return PXR_EnterprisePlugin.UPxr_GotoEnvironmentTextureCheck();
        }
        
        /// <summary>Sets a system date.</summary>
        /// <param name="year">Specifies the year.</param>
        /// <param name="month">Specifies the month.</param>
        /// <param name="day">Specifies the day</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// * `101`: failure, the button to automatically get the date is switched on
        /// </returns>
        public static int SetSystemDate(int year, int month, int day)
        {
            return PXR_EnterprisePlugin.UPxr_SetSystemDate(year, month, day);
        }

        /// <summary>Sets a system time.</summary>
        /// <param name="hourOfDay">Specifies the hour of the day.</param>
        /// <param name="minute">Specifies the minute.</param>
        /// <param name="second">Specifies the second.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// * `101`: failure, the button to automatically get the date is switched on
        /// </returns>
        public static int SetSystemTime(int hourOfDay, int minute, int second)
        {
            return PXR_EnterprisePlugin.UPxr_SetSystemTime(hourOfDay, minute, second);
        }
        
        /// <summary>Gets the app(s) that are running.</summary>
        /// <returns>
        /// `ActivityManager.RunningAppProcessInfo[]`: Information about the running app(s).
        /// </returns>
        public static string[] GetRunningAppProcesses()
        {
            return PXR_EnterprisePlugin.UPxr_GetRunningAppProcesses();
        }

        /// <summary>Gets the foreground app.</summary>
        /// <returns>
        /// `ComponentName`: Information about the foreground app.
        /// </returns>    
        public static string GetFocusedApp()
        {
            return PXR_EnterprisePlugin.UPxr_GetFocusedApp();
        }

        /// <summary>Keeps a process alive by raising its priority level.</summary>
        /// <param name="keepAlivePid">Specifies the PID of the process to keep alive.</param>
        /// <param name="flags">Specifies the flag. The API will perform relevant operation according to the flag value. Below are available values and corresponding operations:
        /// * `2`: raise priority level for the current process.
        /// * `1`: raise priority level for all the processes under the package of the current process.
        /// * `0`: cancelling the high priority level of flag `1` or `2`.
        /// </param>
        /// <param name="level">Specifies the priority level that the process is raised to. `1` indicates a high priority level in which adj is raised to 149.
        /// </param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>   
        public static int KeepAliveBackground(int keepAlivePid, int flags, int level)
        {
            return PXR_EnterprisePlugin.UPxr_KeepAliveBackground(keepAlivePid, flags, level);
        }

        /// <summary>Opens the IPD detection page.</summary>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int OpenIPDDetectionPage()
        {
            return PXR_EnterprisePlugin.UPxr_OpenIPDDetectionPage();
        }

        /// <summary>Sets the height of the floor.
        /// @note Only available for 6DoF devices.
        /// </summary>
        /// <param name="height">Specifies the height of the floor in meters.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// * `-2`: not supported by the device
        /// </returns>
        public static int SetFloorHeight(float height)
        {
            return PXR_EnterprisePlugin.UPxr_SetFloorHeight(height);
        }

        /// <summary>Gets the height of the floor.
        /// @note Only available for 6DoF devices.
        /// </summary>
        /// <returns>The height of the floor in meters.</returns>
        public static float  GetFloorHeight()
        {
            return PXR_EnterprisePlugin.UPxr_GetFloorHeight();
        }

        /// <summary>Sets up timing shutdown for the device.
        /// @note Only available for PICO 4 Enterprise, PICO G3, and PICO Neo3.
        /// </summary>
        /// <param name="hour">Specifies the hour.</param>
        /// <param name="minute">Specifies the minute.</param>
        /// <param name="repeat">Specifies the repeat mode：
        /// * `0`: only once
        /// * The first seven bits: represent which day of the week (Monday to Sunday) is selected for repeated shutdown. For example, 0x03 indicates executing repeated shutdown on Monday and Tuesday.
        /// </param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int OpenTimingShutdown(int hour, int minute, int repeat)
        {
            return PXR_EnterprisePlugin.UPxr_OpenTimingShutdown(hour,minute,repeat);
        }

        /// <summary>Sets up timing startup for the device.
        /// @note Only available for PICO 4 Enterprise, PICO G3, and PICO Neo3.
        /// </summary>
        /// <param name="hour">Specifies the hour.</param>
        /// <param name="minute">Specifies the minute.</param>
        /// <param name="repeat">Specifies the repeat mode：
        /// * `0`: only once
        /// * The first seven bits: represent which day of the week (Monday to Sunday) is selected for repeated startup. For example, 0x03 indicates executing repeated startup on Monday and Tuesday. 
        /// </param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>
        public static int OpenTimingStartup(int hour, int minute, int repeat)
        {
            return PXR_EnterprisePlugin.UPxr_OpenTimingStartup( hour, minute,repeat);
        }

        /// <summary>Gets timing startup settings for the device.
        /// @note Only available for PICO 4 Enterprise, PICO Neo3 Enterprise, PICO G3 with OS version 5.4.0 or later, and PICO Neo3 Pro with OS version 4.8.0/4.8.1 or later.
        /// </summary>
        /// <param name="ext">This parameter is only reserved for future use.</param>
        /// <returns>
        /// * `open`: Whether timing startup is open: `true` (opened); `false` (not opened).
        /// * `curTime`: The time for the next startup.
        /// * `time`: The time scheduled for startup.
        /// * `repeatMode`: The repeat mode.
        /// </returns>
        public static String GetTimingStartupStatusTwo(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetTimingStartupStatusTwo(ext);
        }

        /// <summary>Gets timing shutdown settings for the device.
        /// @note Only available for PICO 4 Enterprise, PICO Neo3 Enterprise, PICO G3 with OS version 5.4.0 or later, and PICO Neo3 Pro with OS version 4.8.0/4.8.1 or later.
        /// </summary>
        /// <param name="ext">This parameter is only reserved for future use.</param>
        /// <returns>
        /// * `open`: Whether timing shutdown is open: `true` (opened); `false` (not opened).
        /// * `curTime`: The time for next shutdown.
        /// * `time`: The time scheduled for shutdown.
        /// * `repeatMode`: The repeat mode.
        /// </returns>
        public static String GetTimingShutDownStatusTwo(int ext=0)
        {
            return PXR_EnterprisePlugin.UPxr_GetTimingShutDownStatusTwo(ext);
        }

        /// <summary>Starts a service.</summary>
        /// <param name="intent">Specifies the service to start. The intent type is provied by PICO SDK.</param>
        /// <returns>
        /// If the service is starting or already running, it returns the `ComponentName` of the actual service that has been started. Otherwise, if the service does not exist, it returns `null`.
        /// </returns>
        public static String StartService(Intent intent)
        {
            return PXR_EnterprisePlugin.UPxr_StartService(intent.getIntent());
        }

        /// <summary>Starts a service.</summary>
        /// <param name="intent">Specifies the service to start. The intent type is provided by Unity.</param>
        /// <returns>
        /// If the service is starting or already running, it returns the `ComponentName` of the actual service that has been started. Otherwise, if the service does not exist, it returns `null`.
        /// </returns> 
        public static String StartService(AndroidJavaObject intent)
        {
            return PXR_EnterprisePlugin.UPxr_StartService(intent);
        }

        /// <summary>Starts a foreground service.</summary>
        /// <param name="intent">Specifies the service to start. The intent type is provied by PICO SDK.</param>
        /// <returns>
        /// If the service is starting or already running, it returns the `ComponentName` of the actual service that has been started. Otherwise, if the service does not exist, it returns `null`.
        /// </returns>
        public static String StartForegroundService(Intent intent)
        {
            return PXR_EnterprisePlugin.UPxr_StartForegroundService(intent.getIntent());
        }

        /// <summary>Starts a foreground service.</summary>
        /// <param name="intent">Specifies the service to start. The intent type is provided by Unity.</param>
        /// <returns>
        /// If the service is starting or already running, it returns the `ComponentName` of the actual service that has been started. Otherwise, if the service does not exist, it returns `null`.
        /// </returns>
        public static String StartForegroundService(AndroidJavaObject intent)
        {
            return PXR_EnterprisePlugin.UPxr_StartForegroundService(intent);
        }

        /// <summary>Sends broadcast.</summary>
        /// <param name="intent">Specifies the broadcast to send. The intent type is provied by PICO SDK.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>               
        public static int SendBroadcast(Intent intent)
        {
            return PXR_EnterprisePlugin.UPxr_SendBroadcast(intent.getIntent());
        }

        /// <summary>Sends broadcast.</summary>
        /// <param name="intent">Specifies the broadcast to send. The intent type is provied by Unity.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>  
        public static int SendBroadcast(AndroidJavaObject intent)
        {
            return PXR_EnterprisePlugin.UPxr_SendBroadcast(intent);
        }

        /// <summary>Sends ordered broadcast.</summary>
        /// <param name="intent">Specifies the broadcast to send. The intent type is provied by PICO SDK.</param>
        /// <param name="receiverPermission">(Optional) The broadcast receiver must hold the specified permissions in order to receive your broadcast. If it is null, no permissions are required.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>   
        public static int SendOrderedBroadcast(Intent intent, String receiverPermission="")
        {
            return PXR_EnterprisePlugin.UPxr_SendOrderedBroadcast(intent.getIntent(),receiverPermission);
        }

        /// <summary>Sends ordered broadcast.</summary>
        /// <param name="intent">Specifies the broadcast to send. The intent type is provied by Unity.</param>
        /// <param name="receiverPermission">(Optional) The broadcast receiver must hold the specified permissions in order to receive your broadcast. If it is null, no permissions are required.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>   
        public static int SendOrderedBroadcast(AndroidJavaObject intent, String receiverPermission="")
        {
            return PXR_EnterprisePlugin.UPxr_SendOrderedBroadcast(intent,receiverPermission);
        }

        /// <summary>Sets a virtual environment.</summary>
        /// <param name="envPath">Specifies the path of the virtual environment file. If you pass `null`, the system's built-in virtual environment will be restored.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>   
        public static int SetVirtualEnvironment(String envPath)
        {
            return PXR_EnterprisePlugin.UPxr_SetVirtualEnvironment(envPath);
        }

        /// <summary>Gets the current virtual environment.</summary>
        /// <returns>
        /// The current virtual environment, and `null` indicates the system's built-in virtual environment.
        /// </returns>   
        public static string GetVirtualEnvironment()
        {
            return PXR_EnterprisePlugin.UPxr_GetVirtualEnvironment();
        }

        /// <summary>Creates a virtual display.</summary>
        /// <param name="displayName">Specifies the name of the virtual display.</param>
        /// <param name="surfaceObj">Specifies the surface on which the virtual content is displayed.</param>
        /// <param name="densityDpi">Specifies the density of the virtual display in dpi. This value must be greater than `0`.</param>
        /// <param name="flags">A combination of virtual display flags:
        /// * `VIRTUAL_DISPLAY_FLAG_PUBLIC`: When this flag is set, the virtual display is public.
        /// * `VIRTUAL_DISPLAY_FLAG_PRESENTATION`: When this flag is set, the virtual display is registered as a presentation display.
        /// * `VIRTUAL_DISPLAY_FLAG_SECURE`: When this flag is set, the virtual display is considered secure.
        /// * `VIRTUAL_DISPLAY_FLAG_OWN_CONTENT_ONLY`: Only show this display's own content; do not mirror the content of another display.
        /// * `VIRTUAL_DISPLAY_FLAG_AUTO_MIRROR`: Allows content to be mirrored on private displays when no content is being shown.
        /// </param>
        /// <returns>
        /// Returns the `displayID` for success, and `-101` for failure.
        /// </returns>
        public static int CreateVirtualDisplay(string displayName, IntPtr surfaceObj, int densityDpi, int flags)
        {
            return PXR_EnterprisePlugin.UPxr_CreateVirtualDisplay(displayName, surfaceObj, 1024, 1024, densityDpi,
                flags);
        }

        /// <summary>Starts an app on the virtual display.</summary>
        /// <param name="displayId">Specifies the ID of the virtual display.</param>
        /// <param name="intent">Specifies the intent of `startActivity`. The intent type is provied by PICO SDK.</param>        
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// * `101`: the specified virtual display does not exist
        /// </returns>  
        public static int StartApp(int displayId, Intent intent)
        {
            return PXR_EnterprisePlugin.UPxr_StartApp(displayId, intent.getIntent());
        }

        /// <summary>Starts an app on the virtual display.</summary>
        /// <param name="displayId">Specifies the ID of the virtual display.</param>
        /// <param name="intent">Specifies the intent of `startActivity`. The intent type is provied by Unity.</param>        
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// * `101`: the specified virtual display does not exist
        /// </returns>  
        public static int StartApp(int displayId, AndroidJavaObject intent)
        {
            return PXR_EnterprisePlugin.UPxr_StartApp(displayId, intent);
        }

        /// <summary>Destroys a virtual display.</summary>
        /// <param name="displayId">Specifies the ID of the virtual display.</param>     
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// * `101`: the specified virtual display does not exist
        /// </returns>  
        public static int ReleaseVirtualDisplay(int displayId)
        {
            return PXR_EnterprisePlugin.UPxr_ReleaseVirtualDisplay(displayId);
        }

        /// <summary>Sets a surface for the virtual display.</summary>
        /// <param name="displayId">Specifies the ID of the virtual display.</param>
        /// <param name="surfaceObj">Specifies the surface to display virtual content.</param>          
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// * `101`: the specified virtual display does not exist
        /// </returns>  
        public static int SetVirtualDisplaySurface(int displayId, IntPtr surfaceObj)
        {
            return PXR_EnterprisePlugin.UPxr_SetVirtualDisplaySurface(displayId, surfaceObj);
        }

        /// <summary>Injects the input event.</summary>
        /// <param name="displayId">Specifies the ID of the virtual display.</param>
        /// <param name="action">Specifies the kind of action being performed, such as `ACTION_DOWN`.</param>      
        /// <param name="source">Specifies the state of any meta / modifier keys that were in effect when the event was generated.</param>
        /// <param name="x">Specifies the X coordinate of this event.</param>
        /// <param name="y">Specifies the Y coordinate of this event.</param>    
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// * `101`: the specified virtual display does not exist
        /// </returns>  
        public static int InjectEvent(int displayId, int action, int source, float x, float y)
        {
            return PXR_EnterprisePlugin.UPxr_InjectEvent(displayId, action, source, 1024*x, 1024*y);
        }

        /// <summary>Injects the input event.</summary>
        /// <param name="displayId">Specifies the ID of the virtual display.</param>
        /// <param name="action">Action code: either `ACTION_DOWN`, `ACTION_UP`, or `ACTION_MULTIPLE`.</param>      
        /// <param name="source">The source of the event.</param>
        /// <param name="keycode">The key code.</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// * `101`: the specified virtual display does not exist
        /// </returns>  
        public static int InjectEvent(int displayId, int action, int source, int keycode)
        {
            return PXR_EnterprisePlugin.UPxr_InjectEvent(displayId, action, source, keycode);
        }

        /// <summary>Shows the global message dialog box.</summary>
        /// <param name="icon">Specifies the icon of the dialog box. You can pass `null` to use the default icon.</param>
        /// <param name="title">Specifies the title of the dialog box, with no length limit, truncated at the end if too long.</param>    
        /// <param name="body">Specifies the content of the dialog box, with no length limit, truncated at the end if too long.</param>
        /// <param name="time">The display duration is  (1-100)*1000, unit: milliseconds. 
        /// * `-1`: continuously display
        /// * `0`: collapse
        /// </param>
        /// <param name="gap">The spacing between the icon and title, in pixels. The default spacing is `0` if not specified. Value range: [0, 200].</param>
        /// <param name="position">Display position adjustment: Relative to the default position, move up or down. Down is positive, up is negative, in pixels. The default position is `0`. Value range: [-800, 800].</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        /// </returns>  
        public static int ShowGlobalMessageDialog(Texture2D icon, String title, String body, long time, int gap, int position)
        {
            return PXR_EnterprisePlugin.UPxr_ShowGlobalMessageDialog(icon, title, body, time,gap,position);
        }

        /// <summary>Gets the information about the bounds of the large space.
        /// @note Only supported by 6 DoF devices, including PICO Neo3 Pro, PICO Neo3 Enterprise, and PICO 4 Enterprise.
        /// </summary>
        /// <returns>`Point3D[]`: Information about the bounds of the large space.</returns>  
        public static Point3D[] GetLargeSpaceBoundsInfo()
        {
            return PXR_EnterprisePlugin.UPxr_GetLargeSpaceBoundsInfo();
        }

        /// <summary>Enables the large space quick mode to quickly set a large space with specified settings.
        /// @note Only supported by 6 DoF devices, including PICO Neo3 Pro, PICO Neo3 Enterprise, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="length">Specifies the length of the large space in meters. Value range: [3,10].</param>
        /// <param name="width">Specifies the width of the large space in meters. Value range: [3,10].</param>
        /// <param name="originType">Specifies how to set the origin:
        /// * `0`: auto set
        /// * `1`: set by scanning the marker
        /// </param>
        /// <param name="openVst">Specifies whether to open the video seethrough mode for setting the origin by scanning the marker.</param>   
        /// <param name="distance">Specifies the distance between the origin and the marker after scanning the marker. The minimum distance is 0.5 meters.</param>   
        /// <param name="timeout">Specifies the timeout duration for scanning the marker in a non-video-seethrough mode, in milliseconds. The default value is `10000`.</param>   
        /// <param name="callback">The callback result:
        /// * `0`: success
        /// * `1`: failure
        /// * `-3`: parameter exceeds the valid value range
        /// * `104`: position tracking disabled
        /// * `201`: quick mode enabled
        /// * `203`: setting origin in this way is not supported
        /// * `204`: scanning marker timeout
        /// </param>                  
        public static void OpenLargeSpaceQuickMode(int length, int width, int originType, bool openVst,
            float distance, int timeout, Action<int> callback)
        {
             PXR_EnterprisePlugin.UPxr_OpenLargeSpaceQuickMode(length,width,originType,openVst,distance,timeout,callback);
        }

        /// <summary>Disables the large space quick mode.
        /// @note Only supported by 6 DoF devices, including PICO Neo3 Pro, PICO Neo3 Enterprise, and PICO 4 Enterprise.
        /// </summary>
        public static void CloseLargeSpaceQuickMode()
        {
            PXR_EnterprisePlugin.UPxr_CloseLargeSpaceQuickMode();
        }

        /// <summary>Sets the origin and positive orientation of the large space quick mode.
        /// @note Only supported by 6 DoF devices, including PICO Neo3 Pro, PICO Neo3 Enterprise, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="originType">Specifies how to set the origin:
        /// * `0`: auto set
        /// * `1`: set by scanning the marker
        /// </param>
        /// <param name="openVst">Specifies whether to open the video seethrough mode for setting the origin by scanning the marker.</param>   
        /// <param name="distance">Specifies the distance between the origin and the marker after scanning the marker. The minimum distance is 0.5 meters.</param>   
        /// <param name="timeout">Specifies the timeout duration for scanning the marker in a non-video-seethrough mode, in milliseconds. The default value is `10000`.</param>   
        /// <param name="callback">The callback result:
        /// * `0`: success
        /// * `1`: failure
        /// * `-3`: parameter exceeds the valid value range
        /// * `104`: position tracking disabled
        /// * `202`: quick mode disabled
        /// * `203`: setting origin in this way is not supported
        /// * `204`: scanning marker timeout
        /// </param> 
        public static void SetOriginOfLargeSpaceQuickMode(int originType, bool openVst, float distance, int timeout,
            Action<int> callback)
        {
            PXR_EnterprisePlugin.UPxr_SetOriginOfLargeSpaceQuickMode(originType,openVst,distance,timeout,callback);
        }

        /// <summary>Sets ths size of the boundary for large space quick mode.
        /// @note Only supported by 6 DoF devices, including PICO Neo3 Pro, PICO Neo3 Enterprise, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="length">Specifies the length of the boundary in meters. Value range: [3,10].</param>
        /// <param name="width">Specifies the width of the boundary in meters. Value range: [3,10].</param>
        /// <param name="callback">The callback result:
        /// * `0`: success
        /// * `1`: failure
        /// * `-3`: parameter exceeds the valid value range
        /// * `104`: position tracking disabled
        /// * `202`: quick mode disabled
        /// </param>                  
        public static void SetBoundaryOfLargeSpaceQuickMode(int length, int width, Action<int> callback)
        {
            PXR_EnterprisePlugin.UPxr_SetBoundaryOfLargeSpaceQuickMode(length,width,callback);
        }

        /// <summary>Gets the information about the large space quick mode.
        /// @note Only supported by 6 DoF devices, including PICO Neo3 Pro, PICO Neo3 Enterprise, and PICO 4 Enterprise.
        /// </summary>
        /// <returns>
        /// Information about the large space quick mode (`LargeSpaceQuickModeInfo`), including the following:
        /// * `status`: Whether the quick mode is enabled or disabled
        /// * `length`: The length of the boundary
        /// * `Width`: The width of the boundary
        /// * `originType`: The way to set the origin: `-1` (not set); `0` (default); `1` (by scanning the marker)
        /// </returns>  
        public static LargeSpaceQuickModeInfo GetLargeSpaceQuickModeInfo()
        {
            return PXR_EnterprisePlugin.UPxr_GetLargeSpaceQuickModeInfo();
        }

        /// <summary>Pairs the left controller.</summary>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns>  
        public static int StartLeftControllerPair()
        {
            return PXR_EnterprisePlugin.UPxr_StartLeftControllerPair();
        }

        /// <summary>Unpairs the left controller.</summary>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns>  
        public static int MakeLeftControllerUnPair()
        {
            return PXR_EnterprisePlugin.UPxr_MakeLeftControllerUnPair();
        }

        /// <summary>Pairs the right controller.</summary>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns>  
        public static int StartRightControllerPair()
        {
            return PXR_EnterprisePlugin.UPxr_StartRightControllerPair();
        }

        /// <summary>Unpairs the right controller.</summary>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns>  
        public static int MakeRightControllerUnPair()
        {
            return PXR_EnterprisePlugin.UPxr_MakeRightControllerUnPair();
        }

        /// <summary>Stops pairing controllers.</summary>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns>  
        public static int StopControllerPair()
        {
            return PXR_EnterprisePlugin.UPxr_StopControllerPair();
        }

        /// <summary>Sets the preferred controller according to hand preferences.</summary>
        /// <param name="isLeft">Specifies the preferred controller:
        /// * `true`: left controller
        /// * `false`: right controller                
        /// </param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns>  
        public static int SetControllerPreferHand(bool isLeft)
        {
            return PXR_EnterprisePlugin.UPxr_SetControllerPreferHand(isLeft);
        }

        /// <summary>Sets a vibration amplitude for controllers.</summary>
        /// <param name="value">Specifies the amplitude. Value range: [0.6].</param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns>  
        public static int SetControllerVibrateAmplitude(int value)
        {
            return PXR_EnterprisePlugin.UPxr_SetControllerVibrateAmplitude(value);
        }

        /// <summary>Sets the power mode.</summary>
        /// <param name="value">Specifies the power mode:
        /// * `0`: power-saving mode
        /// * `1`: standard mode
        /// * `2`: performance mode
        /// </param>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns>  
        public static int SetPowerManageMode(int value)
        {
            return PXR_EnterprisePlugin.UPxr_SetPowerManageMode(value);
        }

        /// <summary>Starts the Room Capture app.
        /// @note Only supported by 6DoF devices, including PICO Neo3 Pro, PICO Neo3 Enterprise, and PICO 4 Enterprise.
        /// </summary>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns>          
        public static int StartRoomMark()
        {
            return PXR_EnterprisePlugin.UPxr_StartRoomMark();
        }

        /// <summary>Clears room capture data.
        /// @note Only supported by 6DoF devices, including PICO Neo3 Pro, PICO Neo3 Enterprise, and PICO 4 Enterprise.
        /// </summary>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns> 
        public static int ClearRoomMark()
        {
            return PXR_EnterprisePlugin.UPxr_ClearRoomMark();
        }

        /// <summary>Clears eye tracking data.
        /// @note Only supported by devices with the eye tracking capability.
        /// </summary>
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns> 
        public static int ClearEyeTrackData()
        {
            return PXR_EnterprisePlugin.UPxr_ClearEyeTrackData();
        }

        /// <summary>Sets a frame rate for eye tracking.
        /// @note Only supported by devices with the eye tracking capability.
        /// </summary>
        /// <param name="value">Specifies the frame rate: 
        /// * `60`
        /// * `90`
        /// </param>     
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns> 
        public static int SetEyeTrackRate(int value)
        {
            return PXR_EnterprisePlugin.UPxr_SetEyeTrackRate(value);
        }

        /// <summary>Sets the tracking frequency.
        /// @note Only supported by6 DoF devices, including PICO Neo3 Pro, PICO Neo3 Enterprise, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="value">Specifies the tracking frequency in Hz:
        /// * `0`: auto
        /// * `50`
        /// * `60`
        /// </param>   
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns> 
        public static int SetTrackFrequency(int value)
        {
            return PXR_EnterprisePlugin.UPxr_SetTrackFrequency(value);
        }

        /// <summary>Starts setting the boundary.
        /// @note Only supported by 6DoF devices, including PICO Neo3 Pro, PICO Neo3 Enterprise, and PICO 4 Enterprise.
        /// </summary> 
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns> 
        public static int StartSetSecureBorder()
        {
            return PXR_EnterprisePlugin.UPxr_StartSetSecureBorder();
        }

        /// <summary>Sets distance sensitivity for the boundary.
        /// @note Only supported by 6 DoF devices, including PICO Neo3 Pro, PICO Neo3 Enterprise, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="value">Specifies the sensitivity value. Value range: [150, 800].</param>   
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns>
        public static int SetDistanceSensitivity(int value)
        {
            return PXR_EnterprisePlugin.UPxr_SetDistanceSensitivity(value);
        }

        /// <summary>Sets speed sensitivity for the boundary.
        /// @note Only supported by 6DoF devices, including PICO Neo3 Pro, PICO Neo3 Enterprise, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="value">Specifies the speed sensitivity. Value range: [0,100].</param>   
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns>
        public static int SetSpeedSensitivity(int value)
        {
            return PXR_EnterprisePlugin.UPxr_SetSpeedSensitivity(value);
        }

        /// <summary>Sets the prediction coefficient for PICO Motion Tracker.
        /// @note Only supported by 6DoF devices, including PICO Neo3 Pro, PICO Neo3 Enterprise, and PICO 4 Enterprise.
        /// </summary>
        /// <param name="value">Specifies the prediction coefficient. Value range: [0.0, 1.0].</param>   
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns>
        public static int SetMotionTrackerPredictionCoefficient(float value)
        {
            return PXR_EnterprisePlugin.UPxr_SetMotionTrackerPredictionCoefficient(value);
        }

        /// <summary>Gets the prediction coefficient of PICO Motion Tracker.
        /// @note Only supported by 6DoF devices, including PICO Neo3 Pro, PICO Neo3 Enterprise, and PICO 4 Enterprise.
        /// </summary> 
        /// <returns>The prediction coefficient.</returns>
        public static float GetMotionTrackerPredictionCoefficient()
        {
            return PXR_EnterprisePlugin.UPxr_GetMotionTrackerPredictionCoefficient();
        }

        /// <summary>Starts the PICO Motion Tracker app to perform calibration.
        /// @note Only supported by 6DoF devices, including PICO Neo3 Pro, PICO Neo3 Enterprise, and PICO 4 Enterprise.
        /// </summary> 
        /// <param name="failMode">Specifies the operation to execute when calibration fails:
        /// * `0`: default operation (neither auto restart nor auto close the app)
        /// * `1`: auto restart the app
        /// * `2`: auto close the app
        /// </param>
        /// <param name="avatarMode">Specifies the display effect of the calibration avatar pop-up after a successful calibration:
        /// * `0`: default
        /// * `-1`: do not display the pop-up
        /// * [1, 60]: the display duration of the pop-up, in seconds. It will automatically hide when exceeding the set duration.
        /// </param>    
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns>
        public static int StartMotionTrackerApp(int failMode, int avatarMode)
        {
            return PXR_EnterprisePlugin.UPxr_StartMotionTrackerApp(failMode, avatarMode);
        }

        /// <summary>Sets the source of the single-eye image.</summary> 
        /// <param name="isLeft">Specifies an eye as the single-eye image source:
        /// * `true`: left eye
        /// * `false`: right eye
        /// </param>    
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns>
        public static int SetSingleEyeSource(bool isLeft)
        {
            return PXR_EnterprisePlugin.UPxr_SetSingleEyeSource(isLeft);
        }

        /// <summary>Sets the visual effect of the view.</summary> 
        /// <param name="value">Specifies the view mode:
        /// * `0`: wide-angle
        /// * `1`: standard
        /// </param>    
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns>
        public static int SetViewVisual(int value)
        {
            return PXR_EnterprisePlugin.UPxr_SetViewVisual(value);
        }

        /// <summary>Sets whether to accept external screen casting.</summary> 
        /// <param name="value">Specifies the mode:
        /// * `0`: ask every time
        /// * `1`: allow
        /// * `2`: reject
        /// </param>    
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns>
        public static int SetAcceptCastMode(int value)
        {
            return PXR_EnterprisePlugin.UPxr_SetAcceptCastMode(value);
        }

        /// <summary>Shares the screen to the external device.</summary> 
        /// <param name="value">Specifies the mode:
        /// * `0`: ask every time
        /// * `1`: allow
        /// * `2`: reject
        /// </param>    
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns>
        public static int SetScreenCastMode(int value)
        {
            return PXR_EnterprisePlugin.UPxr_SetScreenCastMode(value);
        }

        /// <summary>Set the aspect ratio for screen recording and screen capture.</summary> 
        /// <param name="value">Specifies the ratio:
        /// * `0`:  1:1
        /// * `1`: 16:9
        /// * `2`: 9:16
        /// </param>    
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns>
        public static int SetScreenRecordShotRatio(int value)
        {
            return PXR_EnterprisePlugin.UPxr_SetScreenRecordShotRatio(value);
        }

        /// <summary>Set the resolution for screen recording and screen capture.</summary> 
        /// <param name="width">Specifies the width.</param> 
        /// <param name="height">Specifies the height.</param> 
        /// The supported width and height values depend on the device's current aspect ratio.
        /// | Aspect Ratio | Supported Width & Height |
        /// |---|---|
        /// | 1:1 | 1920*1920 |
        /// | 16:9 | 1920*1080, 1280*720 |
        /// | 9:16 | 1080*1920, 720*1280 |
        /// <returns>
        /// * `0`: success
        /// * `1`: failure
        public static int SetScreenResolution(int width, int height)
        {
            return PXR_EnterprisePlugin.UPxr_SetScreenResolution(width,height);
        }

        /// <summary>Sets the frame rate for screen recording.</summary> 
        /// <param name="value">Specifies the frame rate. Valid values are: `24`, `30`, `36`.</param> 
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns>   
        public static int SetScreenRecordFrameRate(int value)
        {
            return PXR_EnterprisePlugin.UPxr_SetScreenRecordFrameRate(value);
        }

        /// <summary>Shows the global dialog box for status notification. The style of the dialog box is different from that of the big and small dialog boxes in `showGlobalBigStatusDialog` and `showGlobalSmallStatusDialog`.
        /// </summary> 
        /// <param name="icon">Specifies the icon of the dialog box. You can pass `null` to use the default icon.</param>
        /// <param name="title">Specifies the title of the dialog box, with no length limit, truncated at the end if too long.</param> 
        /// <param name="time">The display duration is  (1-100)*1000, unit: milliseconds. 
        /// * `-1`: continuously display
        /// * `0`: collapse
        /// </param> 
        /// <param name="position">Display position adjustment: Relative to the default position, move up or down. Down is positive, up is negative, in pixels. The default position is `0`. Value range: [0, 1600].</param> 
        /// <param name="bgColor">Specifies the background color of the dialog box. For example, Color.parseColor("#887766"). Pass `0` to use the default color.</param> 
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns>  
        public static int ShowGlobalTipsDialog(Texture2D icon, String title, long time,  int position,int bgColor)
        {
            return PXR_EnterprisePlugin.UPxr_ShowGlobalTipsDialog(icon, title, time,position,bgColor);
        }

        /// <summary>Hides the global message dialog box.</summary> 
        public static void HideGlobalMessageDialog()
        {
            PXR_EnterprisePlugin.UPxr_HideGlobalMessageDialog();
        }

        /// <summary>Hides the global tips dialog box.</summary> 
        public static void HideGlobalTipsDialog()
        {
            PXR_EnterprisePlugin.UPxr_HideGlobalTipsDialog();
        }

        /// <summary>Shows the big global dialog box for status notification. You can fill in content to be the body of the big dialog box.</summary> 
        /// <param name="icon">Specifies the icon of the dialog box. You can pass `null` to use the default icon.</param>
        /// <param name="title">Specifies the title of the dialog box, with no length limit, truncated at the end if too long.</param> 
        /// <param name="body">Specifies the content of the dialog box, with no length limit, truncated at the end if too long.</param>
        /// <param name="time">The display duration is  (1-100)*1000, unit: milliseconds. 
        /// * `-1`: continuously display
        /// * `0`: collapse
        /// </param> 
        /// <param name="gap">The spacing between the icon and title, in pixels. The default spacing is `0` if not specified. Value range: [0, 200].</param>
        /// <param name="position">Display position adjustment: Relative to the default position, move up or down. Down is positive, up is negative, in pixels. The default position is `0`. Value range: [-800, 800].
        /// </param> 
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns> 
        public static int ShowGlobalBigStatusDialog(Texture2D icon, String title, String body, long time, int gap,
            int position)
        {
            return PXR_EnterprisePlugin.UPxr_ShowGlobalBigStatusDialog(icon, title, body, time, gap, position);
        }

        /// <summary>Hides the big global dialog box for status notification.</summary> 
        public static void HideGlobalBigStatusDialog()
        {
            PXR_EnterprisePlugin.UPxr_HideGlobalBigStatusDialog();
        }

        /// <summary>Shows the small global dialog box for status notification. You cannot fill in content in the small box.</summary> 
        /// <param name="icon">Specifies the icon of the dialog box. You can pass `null` to use the default icon.</param>
        /// <param name="title">Specifies the title of the dialog box. If the title is too long, it will scroll for playback.</param> 
        /// <param name="time">The display duration is  (1-100)*1000, unit: milliseconds. 
        /// * `-1`: continuously display
        /// * `0`: collapse
        /// </param>
        /// <param name="gap">The spacing between the icon and title, in pixels. The default spacing is `0` if not specified. Value range: [0, 200].</param>
        /// <param name="position">Display position adjustment: Relative to the default position, move up or down. Down is positive, up is negative, in pixels. The default position is `0`. Value range: [-800, 800].</param> 
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns> 
        public static int ShowGlobalSmallStatusDialog(Texture2D icon,String title,  long time, int gap, int position)
        {
            return PXR_EnterprisePlugin.UPxr_ShowGlobalSmallStatusDialog(icon, title, time, gap, position);
        }

        /// <summary>Hides the small global dialog box for status notification.</summary> 
        public static void HideGlobalSmallStatusDialog()
        {
            PXR_EnterprisePlugin.UPxr_HideGlobalSmallStatusDialog();
        }
        
        /// <summary>Shows a specified type of global dialog box.</summary> 
        /// <param name="type">Specifies the type of dialog box to display:
        /// * `MESSAGE_DIALOG`: message notification
        /// * `STATUS_TIPS`: tips display
        /// * `STATUS_BIG_DIALOG`:  big dialog box for status notification
        /// * `STATUS_SMALL_DIALOG`: small dialog box for status notification
        /// </param>
        /// <param name="icon">Specifies the icon of the dialog box. You can pass `null` to use the default icon.</param>
        /// <param name="title">Specifies the title of the dialog box, with no length limit, truncated at the end if too long.</param> 
        /// <param name="body">Specifies the content of the dialog box, with no length limit, truncated at the end if too long.</param>
        /// <param name="time">The display duration is  (1-100)*1000, unit: milliseconds. 
        /// * `-1`: continuously display
        /// * `0`: collapse
        /// </param>        
        /// <param name="gap">The spacing between the icon and title, in pixels. The default spacing is `0` if not specified. Value range: [0, 200].</param>
        /// <param name="position">Display position adjustment: Relative to the default position, move up or down. Down is positive, up is negative, in pixels. The default position is `0`.
        /// * Value range for `STATUS_TIPS`: [0, 1600]
        /// * Value range for the rest: [-800, 800]
        /// </param>
        /// <param name="bgColor">Specifies the background color of the dialog box. For example, Color.parseColor("#887766"). Pass `0` to use the default color.</param>  
        /// <returns>
        /// * `0`: success
        /// * `1`: failure       
        /// </returns> 
        public static int ShowGlobalDialogByType(String type,Texture2D icon,String title, String body, long time, int gap, int position, int bgColor)
        {
            return PXR_EnterprisePlugin.UPxr_ShowGlobalDialogByType(type,icon, title, body,time, gap, position,bgColor);
        }

        /// <summary>Hides a specified type of global dialog box.</summary> 
        /// <param name="type">Specifies the type of dialog box to hide:
        /// * `MESSAGE_DIALOG`: message notification
        /// * `STATUS_TIPS`: tips display
        /// * `STATUS_BIG_DIALOG`:  big dialog box for status notification
        /// * `STATUS_SMALL_DIALOG`: small dialog box for status notification
        /// </param>
        public static void HideGlobalDialogByType(String type)
        {
            PXR_EnterprisePlugin.UPxr_HideGlobalDialogByType(type);
        }

        /// <summary>Recenters the forward direction of the headset's origin. This API has the same functionality as a long press of the Home button.</summary>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        public static int Recenter()
        {
           return PXR_EnterprisePlugin.UPxr_Recenter();
        }

        /// <summary>Scans the QR code.
        /// @note Only supported by PICO 4 Enterprise and PICO 4 Ultra Enterprise.</summary>
        /// <param name="callback">Returns the callback for the scan result:
        /// - `-2`: not supported by the device
        /// - null: scanning the QR code failed
        /// - others: the information about the QR code scanned
        /// </param>
        public static void ScanQRCode(Action<string> callback)
        {
            PXR_EnterprisePlugin.UPxr_ScanQRCode(callback);
        }

        /// <summary>Updates the device's system online.
        /// @note Only supported by PICO 4 Ultra Enterprise.</summary>
        /// <param name="callback">The callback of update status, progress, and result.</param>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// - `2`: permission verification failed
        /// - `-1`: the API is deprecated
        /// - `-2`: not supported by the device
        /// </returns>
        public static int OnlineSystemUpdate(SystemUpdateCallback callback)
        {
            return PXR_EnterprisePlugin.UPxr_OnlineSystemUpdate(callback);
        }
        
        /// <summary>Updates the device's system offline.
        /// @note Only supported by PICO 4 Ultra Enterprise.</summary>
        /// <param name="systemUpdateConfig">Offline update-related parameter settings.</param>
        /// <param name="callback">The callback of update status, progress, and result.</param>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// - `2`: permission verification failed
        /// - `-1`: the API is deprecated
        /// - `-2`: not supported by the device
        /// </returns>
        public static int OfflineSystemUpdate(OffLineSystemUpdateConfig systemUpdateConfig, SystemUpdateCallback callback)
        {
            return PXR_EnterprisePlugin.UPxr_OfflineSystemUpdate(systemUpdateConfig,callback);
        }
        
        /// <summary>
        /// Gets the vibration amplitude of controllers.
        /// @note Only supported by PICO Neo3 Pro/Pro Eye/Enterprise, PICO 4 Enterprise, PICO G3, and PICO 4 Ultra.
        /// </summary>
        /// <returns>
        /// The vibration amplitude that ranges from `0` to `6`. The greater the value, the stronger the amplitude. Returning `-2` indicates that the device does not support this API.
        /// </returns>
        public static int GetControllerVibrateAmplitude()
        {
            return PXR_EnterprisePlugin.UPxr_GetControllerVibrateAmplitude();
        }
        
        /// <summary>
        /// Sets a functionality for the volume button of the HMD.
        /// @note Only supported by PICO 4 Enterprise and PICO 4 Ultra.
        /// </summary>
        /// <param name="func">Specifies the functionality:
        /// - `0`: default (i.e., use the functionality set in system settings)
        /// - `1`: for adjusting the volume
        /// - `2`: for adjusting the IPD
        /// </param>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// - `-2`: the device does not support this API
        /// - `-3`: the value specified for the parameter is invalid
        /// </returns>
        public static int SetHMDVolumeKeyFunc(int func)
        {
            return PXR_EnterprisePlugin.UPxr_SetHMDVolumeKeyFunc(func);
        }
        
        /// <summary>
        /// Gets the functionality of the volume button of the HMD.
        /// @note Only supported by PICO 4 Enterprise and PICO 4 Ultra.
        /// </summary>
        /// <returns>
        /// - `0`: default (i.e., use the functionality set in system settings)
        /// - `1`: for adjusting the volume
        /// - `2`: for adjusting the IPD
        /// - `-2`: the device does not support this API
        /// </returns>
        public static int GetHMDVolumeKeyFunc()
        {
            return PXR_EnterprisePlugin.UPxr_GetHMDVolumeKeyFunc();
        }
        
        /// <summary>
        /// Gets the device's power management mode.
        /// @note Only supported by PICO Neo3 Pro/Pro Eye/Enterprise, PICO 4 Enterprise, PICO G3, and PICO 4 Ultra.
        /// </summary>
        /// <returns>
        /// - For PICO 4 Ultra: `0` (power-saving mode); `1` (ensure the display quality first); `3` (ensure that multiple windows can work normally first)
        /// - For other device models: `0` (power-saving mode); `1` (standard mode); `2` (performance mode)
        /// - `-2`: the device does not support this API
        /// </returns>
        public static int GetPowerManageMode()
        {
            return PXR_EnterprisePlugin.UPxr_GetPowerManageMode();
        }
        
        /// <summary>
        /// Gets the frame rate of eye tracking.
        /// @note Only supported by PICO Neo3 Pro Eye, PICO 4 Pro, and PICO 4 Enterprise.
        /// </summary>
        /// <returns>
        /// - `60`: 60Hz
        /// - `90`: 90Hz
        /// - `-2`: the device does not support this API
        /// </returns>
        public static int GetEyeTrackRate()
        {
            return PXR_EnterprisePlugin.UPxr_GetEyeTrackRate();
        }
        
        /// <summary>
        /// Gets the tracking frequency for camera and seethrough.
        /// @note Only supported by PICO Neo3 Pro/Pro Eye/Enterprise, PICO 4 Enterprise, and PICO 4 Ultra.
        /// </summary>
        /// <returns>
        /// - `0`: auto-adjusted frequency
        /// - `50`: 50Hz
        /// - `60`: 60Hz
        /// - `-2`: the device does not support this API
        /// </returns>
        public static int GetTrackFrequency()
        {
            return PXR_EnterprisePlugin.UPxr_GetTrackFrequency();
        }
   
        /// <summary>
        /// Gets the device's distance sensing sensitivity.
        /// @note Only supported by PICO Neo3 Pro/Pro Eye/Enterprise, PICO 4 Enterprise, and PICO 4 Ultra.
        /// </summary>
        /// <returns>
        /// A sensitivity value that ranges from `150` to `800`. The greater value, the higher the sensitivity. Returning `-2` indicates that the device does not support this API.
        /// </returns>
        public static int GetDistanceSensitivity()
        {
            return PXR_EnterprisePlugin.UPxr_GetDistanceSensitivity();
        }
        
        /// <summary>
        /// Gets the device's speed sensing sensitivity.
        /// @note Only supported by PICO Neo3 Pro/Pro Eye/Enterprise, PICO 4 Enterprise, and PICO 4 Ultra.
        /// </summary>
        /// - A sensitivity value that ranges from `0` to `100`. The greater the value, the higher the sensitivity.
        /// - `-1`: the speed sensing switch is toggled off
        /// - `-2`: the device does not support this API
        /// <returns></returns>
        public static int GetSpeedSensitivity()
        {
            return PXR_EnterprisePlugin.UPxr_GetSpeedSensitivity();
        }
        
        /// <summary>
        /// Set the device's collision alert sensitivity.
        /// @note Only supported by PICO 4 Ultra.
        /// </summary>
        /// <param name="value">Specifies the sensitivity value. Value range: [0.0, 1.0]. The greater the value, the higher the sensitivity.</param>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// - `-2`: the device does not support this API
        /// - `-3`: the specified value is out of the valid range
        /// </returns>
        public static int SetMRCollisionAlertSensitivity(float value)
        {
            return PXR_EnterprisePlugin.UPxr_SetMRCollisionAlertSensitivity(value);
        }
        
        /// <summary>
        /// Gest the device's collision alert sensitivity.
        /// @note Only supported by PICO 4 Ultra.
        /// </summary>
        /// <returns>The sensitivity value that ranges from `0.0` to `1.0`. Returning `-2` indicates that the device does not support this API.</returns>
        public static float  GetMRCollisionAlertSensitivity()
        {
            return PXR_EnterprisePlugin.UPxr_GetMRCollisionAlertSensitivity();
        }
        
        /// <summary>
        /// Sets up the WiFi that the device connects to.
        /// </summary>
        /// <param name="configuration">Specifies the WiFi configuration. See [here](https://developer.android.com/reference/android/net/wifi/WifiConfiguration) for details.</param>
        /// <param name="callback">The result callback:
        /// - `0`: connected to the network
        /// - `4`: connection timeout
        /// - `102`: this network is disabled due to multiple association rejections
        /// - `103`: this network is disabled due to multiple authentication failures
        /// - `104`: this network is disabled due to multiple DHCP failures
        /// - `105`: this network is disabled due to a secure network, but no credentials are provided
        /// - `106`: this network is temporarily disabled because it cannot access the Internet
        /// - `107`: this network is disabled because WPS is started
        /// - `108`: this network is disabled due to an EAP-TLS failure
        /// - `109`: this network is disabled due to a lack of user credentials
        /// - `110`: this network is permanently disabled because it cannot access the Internet and the user does not want to stay connected
        /// - `111`: this network is disabled because the WifiManager disables it explicitly
        /// - `112`: this network is disabled due to user switching
        /// - `113`: this network is disabled due to the wrong password
        /// - `114`: this network is disabled due to a lack of subscription
        /// </param>
        public static void ConnectWifi(WifiConfiguration configuration,Action<int> callback)
        {
             PXR_EnterprisePlugin.UPxr_ConnectWifi(configuration,callback);
        }
        
        /// <summary>
        /// Sets up WifiConfiguration and connects the device to this WiFi.
        /// </summary>
        /// <param name="configuration">Specifies the WiFi configuration. See [here](https://developer.android.com/reference/android/net/wifi/WifiConfiguration) for details.</param>
        /// <param name="staticIP">Specifies the static IP address.</param>
        /// <param name="subnet_mask">Specifies the subnet mask.</param>
        /// <param name="gateway">Specifies the gateway.</param>
        /// <param name="DNS">Specifies the domain name system.</param>
        /// <param name="callback">The result callback:
        /// - `0`: connected to the network
        /// - `4`: connection timeout
        /// - `102`: this network is disabled due to multiple association rejections
        /// - `103`: this network is disabled due to multiple authentication failures
        /// - `104`: this network is disabled due to multiple DHCP failures
        /// - `105`: this network is disabled due to a secure network, but no credentials are provided
        /// - `106`: this network is temporarily disabled because it cannot access the Internet
        /// - `107`: this network is disabled because WPS is started
        /// - `108`: this network is disabled due to an EAP-TLS failure
        /// - `109`: this network is disabled due to a lack of user credentials
        /// - `110`: this network is permanently disabled because it cannot access the Internet and the user does not want to stay connected
        /// - `111`: this network is disabled because the WifiManager disables it explicitly
        /// - `112`: this network is disabled due to user switching
        /// - `113`: this network is disabled due to the wrong password
        /// - `114`: this network is disabled due to a lack of subscription
        /// </param>
        /// <returns>The WifiConfiguration that includes fields like the static ID address and more.</returns>
        public static void SetStaticIpConfigurationtoConnectWifi(WifiConfiguration configuration,string staticIP,string subnet_mask,string gateway,string[] DNS,Action<int> callback)
        {
            PXR_EnterprisePlugin.UPxr_SetStaticIpConfigurationtoConnectWifi(configuration,staticIP,subnet_mask,gateway,DNS,callback);
        }
        
        /// <summary>
        /// Gets the eye that serves as the source of the monocular image.
        /// @note Only supported by PICO Neo3 Pro/Pro Eye/Enterprise, PICO 4 Enterprise, PICO G3, and PICO 4 Ultra.
        /// </summary>
        /// <returns>
        /// - `0`: the left eye
        /// - `1`: the right eye
        /// - `-2`: the device does not support this API
        /// </returns>
        public static int GetSingleEyeSource()
        {
            return PXR_EnterprisePlugin.UPxr_GetSingleEyeSource();
        }
        
        /// <summary>
        /// Gets the device's view mode.
        /// @note Only supported by PICO Neo3 Pro/Pro Eye/Enterprise, PICO 4 Enterprise, PICO G3, and PICO 4 Ultra.
        /// </summary>
        /// <returns>
        /// - `0`: wide-angle mode
        /// - `1`: standard mode
        /// - `-2`: the device does not support this API
        /// </returns>
        public static int GetViewVisual()
        {
            return PXR_EnterprisePlugin.UPxr_GetViewVisual();
        }
        
        /// <summary>
        /// Gets whether the device accepts screen sharing from the external device. 
        /// @note Only supported by PICO Neo3 Pro/Pro Eye/Enterprise, PICO 4 Enterprise, PICO G3, and PICO 4 Ultra.
        /// </summary>
        /// <returns>
        /// - `0`: ask every time
        /// - `1`: always allow
        /// - `2`: always reject
        /// - `-2`: the device does not support this API
        /// </returns>
        public static int GetAcceptCastMode()
        {
            return PXR_EnterprisePlugin.UPxr_GetAcceptCastMode();
        }
        
        /// <summary>
        /// Gets whether the device allows the sharing of its screen to the external device.
        /// @note Only supported by PICO Neo3 Pro/Pro Eye/Enterprise, PICO 4 Enterprise, PICO G3, and PICO 4 Ultra.
        /// </summary>
        /// <returns>
        /// - `0`: ask every time
        /// - `1`: always allow
        /// - `2`: always reject
        /// - `-2`: the device does not support this API
        /// </returns>
        public static int GetScreenCastMode()
        {
            return PXR_EnterprisePlugin.UPxr_GetScreenCastMode();
        }
        
        /// <summary>
        /// Gets the aspect ratio for screen recording and screenshots.
        /// @note Only supported by PICO Neo3 Pro/Pro Eye/Enterprise, PICO 4 Enterprise, PICO G3, and PICO 4 Ultra.
        /// </summary>
        /// <returns>The aspect ratio:
        /// - `0` (1:1)
        /// - `1` (16:9) 
        /// - `2` (9:16)
        /// - `-2` (the device does not support this API)
        /// </returns>
        public static int GetScreenRecordShotRatio()
        {
            return PXR_EnterprisePlugin.UPxr_GetScreenRecordShotRatio();
        }
        
        /// <summary>
        /// Gets the resolution for screen recording and screenshots.
        /// @note Only supported by PICO Neo3 Pro/Pro Eye/Enterprise, PICO 4 Enterprise, PICO G3, and PICO 4 Ultra.
        /// </summary>
        /// <returns>
        /// The resolution. The format is [width, height]. Returning [-2, -2] indicates that the device does not support this API.
        /// </returns>
        public static int[] GetScreenResolution()
        {
            return PXR_EnterprisePlugin.UPxr_GetScreenResolution();
        }
        
        /// <summary>
        /// Gets the frame rate for screen recording.
        /// @note Only supported by PICO Neo3 Pro/Pro Eye/Enterprise, PICO 4 Enterprise, and PICO G3.
        /// </summary>
        /// <returns>The frame rate:
        /// - `24`: 24fps
        /// - `30`: 30fps
        /// - `36`: 36fps
        /// - `-2`: the device does not support this API
        /// </returns>
        public static int GetScreenRecordFrameRate()
        {
            return PXR_EnterprisePlugin.UPxr_GetScreenRecordFrameRate();
        }
    }
}