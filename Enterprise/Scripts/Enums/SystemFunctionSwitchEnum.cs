using System;

namespace Unity.XR.PICO.TOBSupport
{
    public enum SystemFunctionSwitchEnum
    {
        /// <summary>
        /// USB debugging switch.
        /// </summary>
        SFS_USB = 0,

        /// <summary>
        /// Automatic sleep switch.
        /// </summary>
        SFS_AUTOSLEEP = 1,

        /// <summary>
        /// Screen-on charging switch.
        /// </summary>
        [Obsolete]
        SFS_SCREENON_CHARGING = 2,

        /// <summary>
        /// OTG charging switch (only available on G2 series).
        /// </summary>
        SFS_OTG_CHARGING = 3,

        /// <summary>
        /// Switch for displaying the return icon in 2D mode (only available on G2 series).
        /// </summary>
        SFS_RETURN_MENU_IN_2DMODE = 4,

        /// <summary>
        /// Combination key switch.
        /// </summary>
        SFS_COMBINATION_KEY = 5,

        /// <summary>
        /// Power-on calibration switch (only available on G2 series).
        /// </summary>
        SFS_CALIBRATION_WITH_POWER_ON = 6,

        /// <summary>
        /// System upgrade and update switch.
        /// </summary>
        SFS_SYSTEM_UPDATE = 7,

        /// <summary>
        /// Mobile screen mirroring switch, only supports PUI4.X.
        /// </summary>
        SFS_CAST_SERVICE = 8,

        /// <summary>
        /// Eye protection mode switch.
        /// </summary>
        SFS_EYE_PROTECTION = 9,

        /// <summary>
        /// Permanent disable switch for 6Dof safety zone.
        /// </summary>
        SFS_SECURITY_ZONE_PERMANENTLY = 10,

        /// <summary>
        /// Global calibration switch (only supported in 3dof mode).
        /// </summary>
        SFS_GLOBAL_CALIBRATION = 11,

        /// <summary>
        /// Automatic calibration (Obsolete).
        /// </summary>
        [Obsolete] SFS_Auto_Calibration = 12,

        /// <summary>
        /// USB boot switch.
        /// </summary>
        SFS_USB_BOOT = 13,

        /// <summary>
        /// Global UI prompt switch for volume adjustment.
        /// </summary>
        SFS_VOLUME_UI = 14,

        /// <summary>
        /// Global UI prompt switch for controller connection.
        /// </summary>
        SFS_CONTROLLER_UI = 15,

        /// <summary>
        /// Interface for turning on/off the navigation bar.
        /// </summary>
        SFS_NAVGATION_SWITCH = 16,

        /// <summary>
        /// Switch for displaying the screen recording button.
        /// </summary>
        SFS_SHORTCUT_SHOW_RECORD_UI = 17,

        /// <summary>
        /// Switch for turning on/off the fitness UI, supported on Neo3 Pro PUI4.x.
        /// </summary>
        SFS_SHORTCUT_SHOW_FIT_UI = 18,

        /// <summary>
        /// Switch for displaying the screen mirroring button.
        /// </summary>
        SFS_SHORTCUT_SHOW_CAST_UI = 19,

        /// <summary>
        /// Switch for displaying the screen capture button.
        /// </summary>
        SFS_SHORTCUT_SHOW_CAPTURE_UI = 20,

        /// <summary>
        /// Switch for killing 2D apps in the background (Obsolete).
        /// </summary>
        [Obsolete] SFS_STOP_MEM_INFO_SERVICE = 21,

        /// <summary>
        /// Switch for restricting app auto-startup (Obsolete).
        /// </summary>
        [Obsolete] SFS_START_APP_BOOT_COMPLETED = 22,

        /// <summary>
        /// Set the device as a host device.
        /// </summary>
        SFS_USB_FORCE_HOST = 23,

        /// <summary>
        /// Set the default safety zone for Neo3 and PICO4 series devices.
        /// </summary>
        SFS_SET_DEFAULT_SAFETY_ZONE = 24,

        /// <summary>
        /// Allow resetting the boundary for Neo3 and PICO4 series devices.
        /// </summary>
        SFS_ALLOW_RESET_BOUNDARY = 25,

        /// <summary>
        /// Switch for displaying the confirmation screen for the safety boundary on Neo3 and PICO4 series devices.
        /// </summary>
        SFS_BOUNDARY_CONFIRMATION_SCREEN = 26,

        /// <summary>
        /// Recenter by long-pressing the Home key on Neo3 and PICO4 series devices.
        /// </summary>
        SFS_LONG_PRESS_HOME_TO_RECENTER = 27,

        /// <summary>
        /// Keep the device connected to the network after the screen is off. Supported on PICO4 E [PUI5.4.0 and above], PICO G3 [PUI5.4.0 and above], and Neo3 Pro [PUI4.8.0 & 4.8.1 and above].
        /// </summary>
        SFS_POWER_CTRL_WIFI_ENABLE = 28,

        /// <summary>
        /// Disable Wi-Fi. Supported on PICO4 E [PUI5.4.0 and above], PICO G3 [PUI5.4.0 and above], and Neo3 Pro [PUI4.8.0 & 4.8.1 and above].
        /// </summary>
        SFS_WIFI_DISABLE = 29,

        /// <summary>
        /// 6Dof switch for Neo3 and PICO4 series devices.
        /// </summary>
        SFS_SIX_DOF_SWITCH = 30,

        /// <summary>
        /// Inverse dispersion switch, available on PICO Neo3 and G3.
        /// </summary>
        SFS_INVERSE_DISPERSION = 31,

        /// <summary>
        /// Switch for logcat, path: data/logs.
        /// </summary>
        SFS_LOGCAT = 32,

        /// <summary>
        /// Switch for the proximity sensor.
        /// </summary>
        SFS_PSENSOR = 33,

        /// <summary>
        /// OTA upgrade switch, available on [PUI5.4.0 and above].
        /// </summary>
        SFS_SYSTEM_UPDATE_OTA = 34,

        /// <summary>
        /// App upgrade and update switch, available on [PUI5.4.0 and above].
        /// </summary>
        SFS_SYSTEM_UPDATE_APP = 35,

        /// <summary>
        /// Switch for displaying the WLAN button in the quick settings, available on [PUI5.4.0 and above].
        /// </summary>
        SFS_SHORTCUT_SHOW_WLAN_UI = 36,

        /// <summary>
        /// Switch for displaying the safety boundary button in the quick settings, available on PICO4E & Neo3Pro [PUI5.4.0].
        /// </summary>
        SFS_SHORTCUT_SHOW_BOUNDARY_UI = 37,

        /// <summary>
        /// Switch for displaying the Bluetooth button in the quick settings, available on [PUI5.4.0 and above].
        /// </summary>
        SFS_SHORTCUT_SHOW_BLUETOOTH_UI = 38,

        /// <summary>
        /// Switch for displaying the one-click cleanup button in the quick settings, available on [PUI5.4.0].
        /// </summary>
        SFS_SHORTCUT_SHOW_CLEAN_TASK_UI = 39,

        /// <summary>
        /// Switch for displaying the IPD adjustment button in the quick settings, available on PICO4E [PUI5.4.0].
        /// </summary>
        SFS_SHORTCUT_SHOW_IPD_ADJUSTMENT_UI = 40,

        /// <summary>
        /// Switch for displaying the power (shutdown/restart) button in the quick settings, available on [PUI5.4.0 and above].
        /// </summary>
        SFS_SHORTCUT_SHOW_POWER_UI = 41,

        /// <summary>
        /// Switch for displaying the edit button in the quick settings, available on [PUI5.4.0 and above].
        /// </summary>
        SFS_SHORTCUT_SHOW_EDIT_UI = 42,

        /// <summary>
        /// Custom resource button in the industry settings - basic settings, available on [PUI5.4.0 and above].
        /// </summary>
        SFS_BASIC_SETTING_APP_LIBRARY_UI = 43,

        /// <summary>
        /// Custom quick settings button in the industry settings - basic settings, available on [PUI5.4.0 and above].
        /// </summary>
        SFS_BASIC_SETTING_SHORTCUT_UI = 44,

        /// <summary>
        /// Whether the LED indicator lights up when the screen is off and the battery level is less than 20%, available on PICO G3.
        /// </summary>
        SFS_LED_FLASHING_WHEN_SCREEN_OFF = 45,

        /// <summary>
        /// Show/hide the custom settings items in the basic settings.
        /// </summary>
        SFS_BASIC_SETTING_CUSTOMIZE_SETTING_UI = 46,
        
        /// <summary>
        /// Switch for displaying the app quit confirmation dialog when switching apps.
        /// </summary>
        SFS_BASIC_SETTING_SHOW_APP_QUIT_CONFIRM_DIALOG = 47,

        /// <summary>
        /// Switch for killing background VR apps. 1 means kill, 2 means don't kill, default is kill.
        /// </summary>
        SFS_BASIC_SETTING_KILL_BACKGROUND_VR_APP = 48,

        /// <summary>
        /// Switch for displaying a blue icon during screen mirroring. Default is to display, set to 0 to hide.
        /// </summary>
        SFS_BASIC_SETTING_SHOW_CAST_NOTIFICATION = 49,

        /// <summary>
        /// Automatic IPD switch, available on PICO 4E.
        /// </summary>
        SFS_AUTOMATIC_IPD = 50,

        /// <summary>
        /// Quick see-through mode switch, available on PICO Neo3 Pro, PICO 4E, and Neo3 Enterprise Edition [PUI 5.7.0].
        /// </summary>
        SFS_QUICK_SEETHROUGH_MODE = 51,

        /// <summary>
        /// High refresh rate mode switch, available on PICO Neo3 Pro, PICO 4E, and Neo3 Enterprise Edition [PUI 5.7.0].
        /// </summary>
        SFS_HIGN_REFERSH_MODE = 52,

        /// <summary>
        /// Switch for keeping apps running in see-through mode, available on PICO Neo3 Pro, PICO 4E, Neo3 Enterprise Edition, and G3 [PUI 5.8.0].
        /// </summary>
        SFS_SEETHROUGH_APP_KEEP_RUNNING = 53,

        /// <summary>
        /// Outdoor tracking enhancement, available on PICO Neo3 Pro, PICO 4E, and Neo3 Enterprise Edition [PUI 5.8.0].
        /// </summary>
        SFS_OUTDOOR_TRACKING_ENHANCEMENT = 54,

        /// <summary>
        /// Quick IPD confirmation, available on PICO 4E [PUI 5.8.0].
        /// </summary>
        SFS_AUTOIPD_AUTO_COMFIRM = 55,

        /// <summary>
        /// Launch automatic IPD when the headset is worn, available on PICO 4E [PUI 5.8.0].
        /// </summary>
        SFS_LAUNCH_AUTOIPD_IF_GLASSES_WEARED = 56,

        /// <summary>
        /// Enable home gesture recognition, available on PICO Neo3 Pro, PICO 4E, and Neo3 Enterprise Edition [PUI 5.8.0].
        /// </summary>
        SFS_GESTURE_RECOGNITION_HOME_ENABLE = 57,

        /// <summary>
        /// Enable reset gesture recognition, available on PICO Neo3 Pro, PICO 4E, and Neo3 Enterprise Edition [PUI 5.8.0].
        /// </summary>
        SFS_GESTURE_RECOGNITION_RESET_ENABLE = 58,

        /// <summary>
        /// Automatic file copying from USB device (OTG), available on PICO Neo3 Pro, PICO 4E, Neo3 Enterprise Edition, and G3 [PUI 5.8.0].
        /// </summary>
        SFS_AUTO_COPY_FILES_FROM_USB_DEVICE = 59,

        /// <summary>
        /// Wi-Fi P2P auto-connect, allowing silent connection without pop-up window.
        /// </summary>
        SFS_WIFI_P2P_AUTO_CONNECT = 60,

        /// <summary>
        /// Switch for enabling file copying when the screen is locked.
        /// </summary>
        SFS_LOCK_SCREEN_FILE_COPY_ENABLE = 61,

        /// <summary>
        /// Switch for enabling dynamic marker tracking.
        /// </summary>
        SFS_TRACKING_ENABLE_DYNAMIC_MARKER = 62,

        /// <summary>
        /// Switch for toggling between 3DOF and 6DOF modes for the controller.
        /// </summary>
        SFS_ENABLE_3DOF_CONTROLLER_TRACKING = 63,

        /// <summary>
        /// Enable controller vibration feedback, supported from PUI560.
        /// </summary>
        SFS_SYSTEM_VIBRATION_ENABLED = 64,

        /// <summary>
        /// Bluetooth switch.
        /// </summary>
        SFS_BLUE_TOOTH = 65,

        /// <summary>
        /// Enhanced video quality, supported from PUI580.
        /// </summary>
        SFS_ENHANCED_VIDEO_QUALITY = 66,

        /// <summary>
        /// Gesture recognition (tracking), supported from PUI560.
        /// </summary>
        SFS_GESTURE_RECOGNITION = 67,

        /// <summary>
        /// Automatic brightness adjustment, supported from PUI560.
        /// </summary>
        SFS_BRIGHTNESS_AUTO_ADJUST = 68,

        /// <summary>
        /// High-current OTG mode, supported from PUI580.
        /// </summary>
        SFS_HIGH_CURRENT_OTG_MODE = 69,

        /// <summary>
        /// Disable background app audio playback, supported from PUI560.
        /// </summary>
        SFS_BACKGROUND_APP_PLAY_AUDIO = 70,

        /// <summary>
        /// Do not disturb mode, supported from PUI560.
        /// </summary>
        SFS_NO_DISTURB_MODE = 71,

        /// <summary>
        /// Monocular screen mirroring, supported from PUI570.
        /// </summary>
        SFS_MONOCULAR_SCREENCAST = 72,

        /// <summary>
        /// Monocular screen capture and recording, supported from PUI570.
        /// </summary>
        SFS_MONOCULAR_SCREEN_CAPTURE = 73,

        /// <summary>
        /// Stabilize the recording screen to reduce jitter, supported from PUI570.
        /// </summary>
        SFS_STABILIZATION_FOR_RECORDING = 74,

        /// <summary>
        /// Hide 2D apps when returning to the home screen if the main app is a VR app.
        /// </summary>
        SFS_HIDE_2D_APP_WHEN_GO_TO_HOME = 75,

        /// <summary>
        /// Controller vibration switch.
        /// </summary>
        SFS_CONTROLLER_VIBRATE = 76,

        /// <summary>
        /// Refresh mode switch.
        /// </summary>
        SFS_REFRESH_MODE = 77,

        /// <summary>
        /// Smart audio switch.
        /// </summary>
        SFS_SMART_AUDIO = 78,

        /// <summary>
        /// Eye tracking switch.
        /// </summary>
        SFS_EYE_TRACK = 79,

        /// <summary>
        /// Facial expression simulation switch.
        /// </summary>
        SFS_FACE_SIMULATE = 80,

        /// <summary>
        /// Enable microphone during screen recording.
        /// </summary>
        SFS_ENABLE_MIC_WHEN_RECORD = 81,

        /// <summary>
        /// Continue recording when the screen is off.
        /// </summary>
        SFS_KEEP_RECORD_WHEN_SCREEN_OFF = 82,

        /// <summary>
        /// Controller vibration tip in the safety boundary.
        /// </summary>
        SFS_CONTROLLER_TIP_VIBRATE = 83,

        /// <summary>
        /// Controller-triggered see-through in the safety boundary.
        /// </summary>
        SFS_CONTROLLER_SEE_THROUGH = 84,

        /// <summary>
        /// Lower the height of the safety boundary in place.
        /// </summary>
        SFS_LOW_BORDER_HEIGHT = 85,

        /// <summary>
        /// Safety tip for fast movement in the safety boundary.
        /// </summary>
        SFS_FAST_MOVE_TIP = 86,

        /// <summary>
        /// Enable wireless USB debugging.
        /// </summary>
        SFS_WIRELESS_USB_ADB = 87,

        /// <summary>
        /// Automatic system update.
        /// </summary>
        SFS_SYSTEM_AUTO_UPDATE = 88,

        /// <summary>
        /// USB tethering switch.
        /// </summary>
        SFS_USB_TETHERING = 89,

        /// <summary>
        /// Respond to the HMD back key in real-time in VR apps.
        /// When the switch is on: Pressing the HMD back key sends a DOWN event, and releasing it sends an UP event.
        /// When the switch is off: Pressing the HMD back key does not send a DOWN event, and releasing it sends both DOWN/UP events.
        /// </summary>
        SFS_REAL_TIME_RESPONSE_HMD_BACK_KEY_IN_VR_APP = 90,

        /// <summary>
        /// Prioritize using markers to retrieve the map.
        /// </summary>
        SFS_RETRIEVE_MAP_BY_MARKER_FIRST = 91,

        /// <summary>
        /// Detect if the controller is in a still state.
        /// </summary>
        SFS_CONTROLLER_STILL = 92,

        /// <summary>
        /// Switch for displaying the performance button in the quick settings, available on Sparrow_PUI513.
        /// </summary>
        SFS_SHORTCUT_SHOW_PERFORMANCE_UI = 93,

        /// <summary>
        /// Battery status display.
        /// 0 - Do not display.
        /// 1 - Display on the HUD.
        /// 2 - Always display.
        /// </summary>
        SFS_BATTERY_STATUS_DISPLAY = 94,

        /// <summary>
        /// Quick relocation.
        /// </summary>
        SFS_QUICK_RELOCATION = 95
    }
}