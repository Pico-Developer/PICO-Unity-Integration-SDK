using LitJson;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace Unity.XR.PXR
{
    public class PXR_AppLog
    {
        public static string APPName = "PICO_UnitySDK";
        public static int APPID = 722442;
        public static int productLineID = 843;


        public static string strXRSDK = "unity_xr_sdk";
        #region Portal
        public static string strPortal = "portal"; // param
        public static string strPortal_Enter = "enter";

        public static string strPortal_Configs_Open = "configs_open";
        public static string strPortal_Configs_RequiredPICOXRPluginApplied = "configs_required_picoxr_plugin_applied";
        public static string strPortal_Configs_RequiredBuildTargetAndroidApplied = "configs_required_build_target_android_applied";
        public static string strPortal_Configs_RequiredAndroidSdkVersionsApplied = "configs_required_android_api_level29_applied";
        public static string strPortal_Configs_ToApplyAllApplied = "configs_to_apply_all_applied";
        public static string strPortal_Configs_ProjectValidation = "configs_project_validation";
        public static string strPortal_Configs_OpenPICOXRProjectSetting = "configs_open_picoxr_project_setting";

        public static string strPortal_Tools_Open = "tools_open";
        public static string strPortal_Tools_ProjectValidation_Documentation = "tools_project_validation_documentation";
        public static string strPortal_Tools_ProjectValidation_Open = "tools_project_validation_open";
        public static string strPortal_Tools_BuildingBlocks = "tools_building_blocks_documentation";
        public static string strPortal_Tools_PICOXRToolkitMR = "tools_picoxr_toolkit_mr_documentation";
        public static string strPortal_Tools_XRProfilingToolkit = "tools_xr_profiling_toolkit_documentation";
        public static string strPortal_Tools_PICODeveloperCenter = "tools_pico_developer_center_documentation";
        public static string strPortal_Tools_Emulator = "tools_emulator_documentation";
        public static string strPortal_Tools_MoreDeveloperTools = "tools_more_developer_tools_documentation";

        public static string strPortal_Sample_Open = "samples_open";
        public static string strPortal_Samples_MixedRealitySample_Documentation = "samples_mixed_reality_sample_documentation";
        public static string strPortal_Samples_MixedRealitySample_GitHub = "samples_mixed_reality_sample_github";

        public static string strPortal_Samples_InteractionSample_Documentation = "samples_interaction_sample_documentation";
        public static string strPortal_Samples_InteractionSample_GitHub = "samples_interaction_sample_github";

        public static string strPortal_Samples_MotionTrackerSample_Documentation = "samples_motion_tracker_sample_documentation";
        public static string strPortal_Samples_MotionTrackerSample_GitHub = "samples_motion_tracker_sample_github";

        public static string strPortal_Samples_PlatformServicesSample_Documentation = "samples_platform_services_sample_documentation";
        public static string strPortal_Samples_PlatformServicesSample_GitHub = "samples_platform_services_sample_github";

        public static string strPortal_Samples_SpatialAudioSample_Documentation = "samples_spatial_audio_sample_documentation";
        public static string strPortal_Samples_SpatialAudioSample_GitHub = "samples_spatial_audio_sample_github";

        public static string strPortal_Samples_ARFoundationSample_Documentation = "samples_arfoundation_sample_documentation";
        public static string strPortal_Samples_ARFoundationSample_GitHub = "samples_arfoundation_sample_github";

        public static string strPortal_Samples_AdaptiveResolutionSample_Documentation = "samples_adaptive_resolution_sample_documentation";
        public static string strPortal_Samples_AdaptiveResolutionSample_GitHub = "samples_adaptive_resolution_sample_github";

        public static string strPortal_Samples_ToonWorldSample_Documentation = "samples_toon_world_sample_documentation";
        public static string strPortal_Samples_ToonWorldSample_GitHub = "samples_toon_world_sample_github";

        public static string strPortal_Samples_MicroWarSample_Documentation = "samples_micro_war_sample_documentation";
        public static string strPortal_Samples_MicroWarSample_GitHub = "samples_micro_war_sample_github";

        public static string strPortal_Samples_PICOAvatarSample_Documentation = "samples_pico_avatar_sample_documentation";
        public static string strPortal_Samples_PICOAvatarSample_GitHub = "samples_pico_avatar_sample_github";

        public static string strPortal_Samples_URPFork_Documentation = "samples_urp_fork_documentation";
        public static string strPortal_Samples_URPFork_GitHub = "samples_urp_fork_github";


        public static string strPortal_About_Open = "about_open";
        public static string strPortal_About_Documentation = "about_documentation";
        public static string strPortal_About_Installation = "about_installation";
        #endregion

        #region ProjectValidation
        public static string strProjectValidation = "project_validation"; // param
        public static string strProjectValidation_AndroidAPIMinSdkVersion = "android_api_min_sdk_version";
        public static string strProjectValidation_ARM64 = "arm64";
        public static string strProjectValidation_OneMainCamera = "one_main_camera";
        public static string strProjectValidation_OneAudioListener = "one_audio_listener";
        public static string strProjectValidation_BuildTargetPlatformAndroid = "build_target_platform_android";
        public static string strProjectValidation_PICOXRPlugin = "picoxr_plugin";
        public static string strProjectValidation_GraphicsAPIOrderForAndroid = "graphics_api_order_for_android";
        public static string strProjectValidation_Unity2022NoDevelopmentBuild = "unity2022_no_development_build";
        public static string strProjectValidation_Unity2022114URPLinearMSAA4OpenglesCrash = "unity2022114_urp_linear_msaa4_opengles_crash";
        public static string strProjectValidation_AddPXRManager = "add_pxr_manager";
        public static string strProjectValidation_ETFRUseOpenGLES3 = "etfr_use_opengles3";
        public static string strProjectValidation_FTUnsafeCode = "ft_unsafe_code";
        public static string strProjectValidation_URPNoHDR = "urp_no_hdr";
        public static string strProjectValidation_URPGraphicsQuality = "urp_graphics_quality";
        public static string strProjectValidation_MRARM64 = "mr_arm64";
        public static string strProjectValidation_OneXROrigin = "one_xr_origin";
        public static string strProjectValidation_ProjectKeystore = "project_keystore";
        public static string strProjectValidation_ProjectKey = "project_key";
        public static string strProjectValidation_UIOrientationLandscapeLeft = "ui_orientation_landscape_left";
        public static string strProjectValidation_UseActivity = "use_activity";
        public static string strProjectValidation_TargetAPILevelAuto = "target_api_level_auto";
        public static string strProjectValidation_InstallLocationAuto = "install_location_auto";
        public static string strProjectValidation_ContactOffset001 = "context_offset_001";
        public static string strProjectValidation_SleepThreshold0005 = "sleep_threshold_0005";
        public static string strProjectValidation_SolverIteration8 = "solver_iteration8";
        public static string strProjectValidation_MaximumPixelLights = "maximum_pixel_lights";
        public static string strProjectValidation_TextureQualitytoFullRes = "texture_quality_to_full_res";
        public static string strProjectValidation_AnisotropicFiltering = "anisotropic_filtering";
        public static string strProjectValidation_ETC2 = "etc2";
        public static string strProjectValidation_ColorSpaceLinear = "color_space_linear";
        public static string strProjectValidation_DisableGraphicsJobs = "disable_graphics_jobs";
        public static string strProjectValidation_Multithreaded = "multithreaded";
        public static string strProjectValidation_DisplayBufferFormat = "display_buffer_format";
        public static string strProjectValidation_RenderingPathToForward = "rendering_path_to_forward";
        public static string strProjectValidation_Multiview = "multiview";
        public static string strProjectValidation_URPIntermediatetexturetoAuto = "urp_intermediate_texture_to_auto";
        public static string strProjectValidation_URPDisableSSAO = "urp_disable_ssao";
        public static string strProjectValidation_FFRSubsampling = "ffr_subsampling";
        public static string strProjectValidation_MSAA = "msaa";
        public static string strProjectValidation_APPSWNoContentProtect = "appsw_no_content_protect";
        public static string strProjectValidation_TrackingOriginModeDevice = "tracking_origin_mode_device";
        public static string strProjectValidation_DisableRealtimeGI = "disable_realtime_gi";
        public static string strProjectValidation_GPUSkinning = "gpu_skinning";
        public static string strProjectValidation_EyeTrackingCalibration = "eye_tracking_calibration";
        public static string strProjectValidation_WritePermissionAndroid14 = "write_permission_android14";
        public static string strProjectValidation_Unity2020321Unity6 = "unity2020321_unity6";
        public static string strProjectValidation_URPNoUseToDelete = "urp_no_use_to_delete";
        public static string strProjectValidation_URPVSTNoPostProcessing = "urp_vst_no_post_processing";
        public static string strProjectValidation_URPNoETFRAndFFR = "urp_no_etfr_and_ffr";
        public static string strProjectValidation_APPSWNeed = "appsw_need";
        public static string strProjectValidation_LateLatchingNeed = "late_latching_need";
        public static string strProjectValidation_LateLatchingOrOverlay = "late_latching_or_overlay";
        public static string strProjectValidation_Overlay7 = "overlay7";
        public static string strProjectValidation_SuperResolutionOrSubsampling = "super_resolution_or_subsampling";
        public static string strProjectValidation_SharpeningOrSubsampling = "sharpening_or_subsampling";
        public static string strProjectValidation_Unity6URPOpenGLESMultiPassNoMSAA = "unity6_urp_opengles_multi_pass_no_msaa";
        public static string strProjectValidation_Overlay4 = "overlay4";
        public static string strProjectValidation_Unity6RunInBackground = "unity6_run_in_background";
        public static string strProjectValidation_MRC = "mrc";
        public static string strProjectValidation_DisplayRefreshRatesDefault = "display_refresh_rates_default";
        public static string strProjectValidation_VKOptimizeBufferDiscards = "vk_optimize_buffer_discards";
        #endregion

        #region BuildingBlocks
        public static string strBuildingBlocks = "building_blocks"; // param
        public static string strBuildingBlocks_PICOControllerTracking = "pico_controller_tracking";
        public static string strBuildingBlocks_ControllerCanvasInteraction = "controller_canvas_interaction";
        public static string strBuildingBlocks_PICOHandTracking = "pico_hand_tracking";
        public static string strBuildingBlocks_XRHandTracking = "xr_hand_tracking";
        public static string strBuildingBlocks_XRIHandInteraction = "xri_hand_interaction";
        public static string strBuildingBlocks_XRIGrabInteraction = "xri_grab_interaction";
        public static string strBuildingBlocks_XRIPokeInteraction = "xri_poke_interaction";
        public static string strBuildingBlocks_PICOVideoSeethrough = "pico_video_seethrough";
        public static string strBuildingBlocks_PICOVideoSeethroughEffect = "pico_video_seethrough_effect";
        public static string strBuildingBlocks_PICOBodyTracking = "pico_body_tracking";
        public static string strBuildingBlocks_PICOBodyTrackingDebug = "pico_body_tracking_debug";
        public static string strBuildingBlocks_PICOObjectTracking = "pico_object_tracking";
        public static string strBuildingBlocks_PICOSpatialAudioFreeField = "pico_spatial_audio_free_field";
        public static string strBuildingBlocks_PICOSpatialAudioAmbisonics = "pico_spatial_audio_ambisonics";
        #endregion

 
        private static bool isInited = false;
        private static void TryInitAppLog()
        {
#if UNITY_EDITOR_WIN
            if (isInited)
            {
                return;
            }
            isInited = true;

            Debug.Log($"TryInitAppLog ");
            AppLog_init("722442", "PICO_UnitySDK");
#endif
        }

        /// <summary>
        /// Call the buried point collection wherever it is required, and pass in the name and parameters of the buried point (in a dictionary structure). 
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="properties"></param>
        public static void PXR_OnEvent(string param, string value)
        {
#if UNITY_EDITOR_WIN
            // Debug.Log($"PXR_OnEvent eventName:{strXRSDK}, param:{param}, value:{value}");
            var contentData = new JsonData()
            {
                [param] = value,

            };
            TryInitAppLog();
            AppLog_onEvent(strXRSDK, contentData.ToJson());
#endif
        }


        /// <summary>
        /// Call the buried point collection wherever it is required, and pass in the name and parameters of the buried point (in a dictionary structure). 
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="properties"></param>
        public static void PXR_OnEvent(string eventName, string param, string value = "1")
        {
#if UNITY_EDITOR_WIN
            //Debug.Log($"PXR_OnEvent eventName:{eventName}, param:{param}, value:{value}");
            var contentData = new JsonData()
            {
                [param] = value,

            };
            TryInitAppLog();
            AppLog_onEvent(eventName, contentData.ToJson());
#endif
        }

        public static void PXR_SetLogEnabled(bool enable)
        {
#if UNITY_EDITOR_WIN
            Debug.Log($"PXR_SetLogEnabled start enable={enable}");
            if (enable)
            {
                AppLog_setLogEnabled(1);
            }
            else
            {
                AppLog_setLogEnabled(0);
            }
#endif
        }

        public static void PXR_AppLogDestroy(DestroyCallback observer)
        {
#if UNITY_EDITOR_WIN
            AppLog_destroy(observer);
#endif
        }

        private const string DllName = "applogrs";

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void AppLog_init(string appid, string channel);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void AppLog_setLogEnabled(uint enabled);
        public delegate void LoggerCallback(string message);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void AppLog_setLogger(LoggerCallback observer);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void AppLog_onEvent(string eventName, string param);
        public delegate void DestroyCallback();
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void AppLog_destroy(DestroyCallback destory_callback);
    }

}