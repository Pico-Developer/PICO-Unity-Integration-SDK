
using System;
using System.Linq;
using Unity.XR.CoreUtils.Editor;
using Unity.XR.PXR;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEditor.XR.Management;
using UnityEngine;

static class PXR_ProjectValidationOptional
{
    const string k_Catergory = "PICO XR Optional";

    [InitializeOnLoadMethod]
    static void AddOptionalRules()
    {
        var androidGlobalRules = new[]
        {
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "When enabling ET or ETFR, option 'Eye Tracking Calibration' can be used.",
                    IsRuleEnabled = PXR_Utils.IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        if (PXR_ProjectSetting.GetProjectConfig().eyeTracking || PXR_ProjectSetting.GetProjectConfig().enableETFR)
                        {
                            return PXR_ProjectSetting.GetProjectConfig().eyetrackingCalibration;
                        }
                        return true;
                    },
                    FixItMessage = "PXR_Manager > 'Eye Tracking Calibration' set to enable.",
                    FixIt = () =>
                    {
                        PXR_ProjectSetting.GetProjectConfig().eyetrackingCalibration = true;
                        PXR_AppLog.PXR_OnEvent(PXR_AppLog.strProjectValidation, PXR_AppLog.strProjectValidation_EyeTrackingCalibration);
                    },
                    Error = false
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Disable Realtime GI.",
                    IsRuleEnabled = PXR_Utils.IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return !Lightmapping.realtimeGI;
                    },
                    FixItMessage = "Open Window > Rendering > Lighting > Realtime Lighting > Realtime Global lllumination: disabled.",
                    FixIt = () =>
                    {
                        Lightmapping.realtimeGI = false;
                        PXR_AppLog.PXR_OnEvent(PXR_AppLog.strProjectValidation, PXR_AppLog.strProjectValidation_DisableRealtimeGI);
                    },
                    Error = false
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Enable GPU Skinning.",
                    IsRuleEnabled = PXR_Utils.IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return PlayerSettings.gpuSkinning;
                    },
                    FixItMessage = "Open Project Settings > Player Settings > Player> Other Settings > GPU Skinning :enabled",
                    FixIt = () =>
                    {
                        PlayerSettings.gpuSkinning = true;
                        PXR_AppLog.PXR_OnEvent(PXR_AppLog.strProjectValidation, PXR_AppLog.strProjectValidation_GPUSkinning);
                    },
                    Error = false
                },
        };
        BuildValidator.AddRules(BuildTargetGroup.Android, androidGlobalRules);
    }
}
