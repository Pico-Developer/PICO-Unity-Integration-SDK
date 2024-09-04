using System;

namespace Unity.XR.PICO.TOBSupport
{
    public enum SystemFunctionSwitchEnum
    {
        //USB调试开关
        SFS_USB = 0,

        //自动休眠开关
        SFS_AUTOSLEEP = 1,

        //亮屏充电开关
        SFS_SCREENON_CHARGING = 2,

        //OTG充电开关（仅G2系列有）
        SFS_OTG_CHARGING = 3,

        //2D界面下返回图标显示开关（仅G2系列有）
        SFS_RETURN_MENU_IN_2DMODE = 4,

        //组合键开关
        SFS_COMBINATION_KEY = 5,

        //开机校准开关（仅G2系列有）
        SFS_CALIBRATION_WITH_POWER_ON = 6,

        //升级更新开关
        SFS_SYSTEM_UPDATE = 7,

        //手机投屏开关
        //只支持PUI4.X
        SFS_CAST_SERVICE = 8,

        //护眼模式开关
        SFS_EYE_PROTECTION = 9,

        // 6Dof安全区永久关闭开关
        SFS_SECURITY_ZONE_PERMANENTLY = 10,

        //全局校准开关（仅3dof模式支持）
        SFS_GLOBAL_CALIBRATION = 11,

        //自动校准
        [Obsolete]
        SFS_Auto_Calibration = 12,

        //USB插入启动
        SFS_USB_BOOT = 13,

        //音量全局UI提示开关
        SFS_VOLUME_UI = 14,

        //手柄连接全局UI提示开关
        SFS_CONTROLLER_UI = 15,

        //打开/关闭导航栏的接口
        SFS_NAVGATION_SWITCH = 16,

        //是否显示录屏按钮
        SFS_SHORTCUT_SHOW_RECORD_UI = 17,

        //打开/关闭运动UI
        //Neo3 Pro PUI4.x支持
        SFS_SHORTCUT_SHOW_FIT_UI = 18,

        //是否显示投屏按钮
        SFS_SHORTCUT_SHOW_CAST_UI = 19,

        //是否显示截屏按钮
        SFS_SHORTCUT_SHOW_CAPTURE_UI = 20,

        //2D应用杀活开关
        [Obsolete]
        SFS_STOP_MEM_INFO_SERVICE = 21,

        //限制APP自启动开关
        [Obsolete]
        SFS_START_APP_BOOT_COMPLETED = 22,

        //设备设置为host设备
        SFS_USB_FORCE_HOST = 23,

        //Neo3、PICO4系列设备设置快速安全区
        SFS_SET_DEFAULT_SAFETY_ZONE = 24,

        //Neo3 PICO4系列设备可重新自定义安全区
        SFS_ALLOW_RESET_BOUNDARY = 25,

        //Neo3、PICO4系列设备安全区是否显示确认界面
        SFS_BOUNDARY_CONFIRMATION_SCREEN = 26,

        //Neo3、PICO4系列设备长按Home键校准
        SFS_LONG_PRESS_HOME_TO_RECENTER = 27,

        //灭屏后设备保持网络连接
        //PICO4 E【PUI5.4.0及以上】
        //PICO G3【PUI5.4.0及以上】
        //Neo3 Pro【PUI4.8.0&4.8.1及以上】
        SFS_POWER_CTRL_WIFI_ENABLE = 28,

        //禁用wifi
        //PICO4 E【PUI5.4.0及以上】
        //PICO G3【PUI5.4.0及以上】
        //Neo3 Pro【PUI4.8.0&4.8.1及以上】
        SFS_WIFI_DISABLE = 29,

        //Neo3、PICO4系列设备6Dof开关
        SFS_SIX_DOF_SWITCH = 30,

        //反色散开关
        //PICO Neo3、G3
        SFS_INVERSE_DISPERSION = 31,

        //switch logcat , path: data/logs
        SFS_LOGCAT = 32,

        //switch psensor
        SFS_PSENSOR = 33,

        //OTA升级开关
        //【PUI5.4.0 以上】
        SFS_SYSTEM_UPDATE_OTA = 34,

        //应用升级更新开关
        //【PUI5.4.0 以上】
        SFS_SYSTEM_UPDATE_APP = 35,

        //快捷设置是否显示WLAN按钮
        //【PUI5.4.0 以上】
        SFS_SHORTCUT_SHOW_WLAN_UI = 36,

        //快捷设置是否显示安全区按钮
        //PICO4E & Neo3Pro 【PUI5.4.0】
        SFS_SHORTCUT_SHOW_BOUNDARY_UI = 37,

        //快捷设置是否显示蓝牙按钮
        //【PUI5.4.0 以上】
        SFS_SHORTCUT_SHOW_BLUETOOTH_UI = 38,

        //快捷设置是否显示一键清理按钮
        //【PUI5.4.0】
        SFS_SHORTCUT_SHOW_CLEAN_TASK_UI = 39,

        //快捷设置是否显示瞳距调节按钮
        //PICO4E【PUI5.4.0】
        SFS_SHORTCUT_SHOW_IPD_ADJUSTMENT_UI = 40,

        //快捷设置是否显示关机/重启按钮
        //【PUI5.4.0 以上】
        SFS_SHORTCUT_SHOW_POWER_UI = 41,

        //快捷设置是否显示编辑按钮
        //【PUI5.4.0 以上】
        SFS_SHORTCUT_SHOW_EDIT_UI = 42,

        //行业设置-基础设置-自定义资源按钮
        //【PUI5.4.0 以上】
        SFS_BASIC_SETTING_APP_LIBRARY_UI = 43,

        //行业设置-基础设置-自定义快捷设置按钮
        //【PUI5.4.0 以上】
        SFS_BASIC_SETTING_SHORTCUT_UI = 44,

        //LED 指示灯灭屏并且电量小于20%时是否亮
        //PICO G3
        SFS_LED_FLASHING_WHEN_SCREEN_OFF = 45,

        //基础设置中自定义设置条目的显示隐藏
        SFS_BASIC_SETTING_CUSTOMIZE_SETTING_UI = 46,

        //--------------以下开关上线版本详见更新历史-----------------

        //app 切换是否显示退出对话框
        SFS_BASIC_SETTING_SHOW_APP_QUIT_CONFIRM_DIALOG = 47,

        //是否查杀后台VR的应用，1杀，当设置为2时，不杀，默认为杀
        SFS_BASIC_SETTING_KILL_BACKGROUND_VR_APP = 48,

        //是否显示投屏时的一个蓝色图标，默认显示，当设置为0时，隐藏蓝色图标
        SFS_BASIC_SETTING_SHOW_CAST_NOTIFICATION = 49,

        //自动IPD开关
        //PICO 4E
        SFS_AUTOMATIC_IPD = 50,

        //快速透视模式开关
        //PICO Neo3 Pro & PICO 4E & Neo3 企业版【PUI 5.7.0】
        SFS_QUICK_SEETHROUGH_MODE = 51,

        //高刷新率模式开关
        //PICO Neo3 Pro & PICO 4E & Neo3 企业版【PUI 5.7.0】
        SFS_HIGN_REFERSH_MODE = 52,
        //PICO Neo3 Pro & PICO 4E & Neo3 企业版 & G3【PUI 5.8.0】
        //透视模式下应用是否正常运行开关
        SFS_SEETHROUGH_APP_KEEP_RUNNING = 53,

        //PICO Neo3 Pro & PICO 4E & Neo3 企业版【PUI 5.8.0】
        //户外环境跟踪定位优化
        SFS_OUTDOOR_TRACKING_ENHANCEMENT = 54,

        //PICO 4E【PUI 5.8.0】
        //快捷IPD
        SFS_AUTOIPD_AUTO_COMFIRM = 55,

        //PICO 4E【PUI 5.8.0】
        //佩戴头戴是否启动自动IPD
        SFS_LAUNCH_AUTOIPD_IF_GLASSES_WEARED = 56,

        //PICO Neo3 Pro & PICO 4E & Neo3 企业版【PUI 5.8.0】
        //HOME手势开关
        SFS_GESTURE_RECOGNITION_HOME_ENABLE = 57,

        //PICO Neo3 Pro & PICO 4E & Neo3 企业版【PUI 5.8.0】
        //RESET手势开关
        SFS_GESTURE_RECOGNITION_RESET_ENABLE = 58,

        //PICO Neo3 Pro & PICO 4E & Neo3 企业版 & G3【PUI 5.8.0】
        //OTG 资源自动导入
        SFS_AUTO_COPY_FILES_FROM_USB_DEVICE = 59,

        //WIFI直连-任何设备都可静默连接，无需弹窗
        SFS_WIFI_P2P_AUTO_CONNECT = 60,

        //锁屏状态下文件拷贝开关
        SFS_LOCK_SCREEN_FILE_COPY_ENABLE = 61,

        //动态marker检测开关
        SFS_TRACKING_ENABLE_DYNAMIC_MARKER = 62,

        //手柄3/6 dof模式切换
        SFS_ENABLE_3DOF_CONTROLLER_TRACKING = 63,

        //手柄震动反馈 PUI560开始支持
        SFS_SYSTEM_VIBRATION_ENABLED = 64,

        //蓝牙开关
        SFS_BLUE_TOOTH = 65,

        //视频画质增强 PUI580开始支持
        SFS_ENHANCED_VIDEO_QUALITY = 66,

        //手势识别（追踪） PUI560开始支持
        SFS_GESTURE_RECOGNITION = 67,

        //亮度自适应 PUI560开始支持
        SFS_BRIGHTNESS_AUTO_ADJUST = 68,

        //大电流OTG模式 PUI580开始支持
        SFS_HIGH_CURRENT_OTG_MODE = 69,

        //禁止后台应用播放音频 PUI560开始支持
        SFS_BACKGROUND_APP_PLAY_AUDIO = 70,

        //勿扰模式 PUI560开始支持
        SFS_NO_DISTURB_MODE = 71,

        //单目投屏 PUI570开始支持
        SFS_MONOCULAR_SCREENCAST = 72,

        //单目截录屏 PUI570开始支持
        SFS_MONOCULAR_SCREEN_CAPTURE = 73,

        //减少录屏画面抖动 PUI570开始支持
        SFS_STABILIZATION_FOR_RECORDING = 74,

        //主屏幕应用为VR应用时，回launcher收起2D应用开关接口
        SFS_HIDE_2D_APP_WHEN_GO_TO_HOME = 75,

        //手柄振动开关
        SFS_CONTROLLER_VIBRATE = 76,

        //刷新模式开关
        SFS_REFRESH_MODE = 77,

        //智能音效开关
        SFS_SMART_AUDIO = 78,

        //视线追踪开关
        SFS_EYE_TRACK = 79,

        //表情模拟开关
        SFS_FACE_SIMULATE = 80,

        //录屏时启用麦克风开关
        SFS_ENABLE_MIC_WHEN_RECORD = 81,

        //熄屏后继续录屏开关
        SFS_KEEP_RECORD_WHEN_SCREEN_OFF = 82,

        //安全边界中，手柄振动提醒开关
        SFS_CONTROLLER_TIP_VIBRATE = 83,

        //安全边界中，手柄触发透视开关
        SFS_CONTROLLER_SEE_THROUGH = 84,

        //安全边界中，原地边界降低高度开关
        SFS_LOW_BORDER_HEIGHT = 85,

        //安全边界中，快速移动安全提示开关
        SFS_FAST_MOVE_TIP = 86,

        //开启无线USB调试开关
        SFS_WIRELESS_USB_ADB = 87,

        //系统自动更新
        SFS_SYSTEM_AUTO_UPDATE = 88,

        //usb共享网络开关
        SFS_USB_TETHERING = 89,

        /**
         * VR应用中实时响应头戴返回键
         * 开关打开：头戴返回键按下发出DOWN事件，抬起发出UP事件
         * 开关关闭：头戴返回键按下不发出DOWN事件，抬起同时发出DOWN/UP事件
         */
        SFS_REAL_TIME_RESPONSE_HMD_BACK_KEY_IN_VR_APP = 90,

        //优先使用Marker点找回地图
        SFS_RETRIEVE_MAP_BY_MARKER_FIRST = 91
    }
}