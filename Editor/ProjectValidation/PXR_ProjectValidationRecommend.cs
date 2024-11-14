
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Editor;
using Unity.XR.PXR;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEditor.XR.Management;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using static Unity.XR.CoreUtils.XROrigin;
#if URP
using UnityEngine.Rendering.Universal;
#endif

static class PXR_ProjectValidationRecommend
{
    const string k_Catergory = "PICO XR Recommend";

    [InitializeOnLoadMethod]
    static void AddRecommendRules()
    {
#if UNITY_2021_1_OR_NEWER
        NamedBuildTarget recommendedBuildTarget = NamedBuildTarget.Android;
#else
        const BuildTargetGroup recommendedBuildTarget = BuildTargetGroup.Android;
#endif
        var androidGlobalRules = new[]
        {                
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Set 'Target API Level' to automatic.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return PlayerSettings.Android.targetSdkVersion == AndroidSdkVersions.AndroidApiLevelAuto;
                    },
                    FixItMessage = "Open Project Settings > Player Settings > Player> Other Settings > 'Target API Level' to set automatic.",
                    FixIt = () =>
                    {
                        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
                    },
                    Error = false
                },
                 new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Set 'Install Location' to automatic.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return PlayerSettings.Android.preferredInstallLocation == AndroidPreferredInstallLocation.Auto;
                    },
                    FixItMessage = "Open Project Settings > Player Settings > Player> Other Settings > 'Install Location' to set automatic.",
                    FixIt = () =>
                    {
                        PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;
                    },
                    Error = false
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Use ARM64 architecture and IL2CPP scripting.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        if ((PlayerSettings.Android.targetArchitectures & AndroidArchitecture.ARM64) != AndroidArchitecture.None)
                        {
                            return PlayerSettings.GetScriptingBackend(recommendedBuildTarget) == ScriptingImplementation.IL2CPP;
                        }
                        return false;
                    },
                    FixItMessage = "Open Project Settings > Player Settings > Player> Other Settings > Android tab and ensure 'Scripting Backend'" +
                        " is set to 'IL2CPP'. Then under 'Target Architectures' enable 'ARM64'.",
                    FixIt = () =>
                    {
                        PlayerSettings.SetScriptingBackend(recommendedBuildTarget, ScriptingImplementation.IL2CPP);
                        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
                    },
                    Error = false
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "'Color Space' using Linear.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return PlayerSettings.colorSpace == ColorSpace.Linear;
                    },
                    FixItMessage = "Open Project Settings > Player Settings > Player> Other Settings > 'Color Space' set to 'Linear'.",
                    FixIt = () =>
                    {
                        PlayerSettings.colorSpace = ColorSpace.Linear;
                    },
                    Error = false
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "'Graphics Jobs' using disable.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return !PlayerSettings.graphicsJobs;
                    },
                    FixItMessage = "Open Project Settings > Player Settings > Player> Other Settings > 'Graphics Jobs' set to disable.",
                    FixIt = () =>
                    {
                        PlayerSettings.graphicsJobs = false;
                    },
                    Error = false
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "When using ETFR/FFR, it is recommended to enable subsampling to improve performance.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {                        
                        if (PXR_ProjectSetting.GetProjectConfig().recommendSubsamping)
                        {
                            return PXR_ProjectSetting.GetProjectConfig().enableSubsampled;
                        }
                        return true;
                    },
                    FixItMessage = "PXR_Manager > Subsamping set to enable.",
                    FixIt = () =>
                    {
                        PXR_ProjectSetting.GetProjectConfig().enableSubsampled = true;
                    },
                    Error = false
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Using recommended MSAA.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        if (PXR_ProjectSetting.GetProjectConfig().recommendMSAA)
                        {
                            return PXR_ProjectSetting.GetProjectConfig().enableRecommendMSAA;
                        }
                        return true;
                    },
                    FixItMessage = "PXR_Manager > 'Use Recommended MSAA' set to enable.",
                    FixIt = () =>
                    {
                        PXR_ProjectSetting.GetProjectConfig().enableRecommendMSAA = true;
                    },
                    Error = false
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Using tracking origin mode : Device or Floor.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        List<XROrigin> components = FindComponentsInScene<XROrigin>().Where(component => component.isActiveAndEnabled).ToList();

                        foreach(XROrigin origin in components)
                        {
                            if (TrackingOriginMode.NotSpecified == origin.RequestedTrackingOriginMode)
                            {
                                return false;
                            }
                        }
                        return true;
                    },
                    FixItMessage = "XROrigin > TrackingOriginMode.Device.",
                    FixIt = () =>
                    {
                        List<XROrigin> components = FindComponentsInScene<XROrigin>().Where(component => component.isActiveAndEnabled).ToList();
                        foreach(XROrigin origin in components)
                        {
                            origin.RequestedTrackingOriginMode = TrackingOriginMode.Device;
                        }
                    },
                    Error = false
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Not recommended to use both 'Application SpaceWarp' and 'Content Protect' simultaneously.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return !(PXR_ProjectSetting.GetProjectConfig().useContentProtect && GetSettings().enableAppSpaceWarp);
                    },
                    FixItMessage = "Open Project Settings > Player Settings> PICO > Application SpaceWarp: disabled.",
                    FixIt = () =>
                    {
                        GetSettings().enableAppSpaceWarp = false;
                    },
                    Error = false
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Using recommended 'Texture compression': ETC2 or ASTC.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return EditorUserBuildSettings.androidBuildSubtarget == MobileTextureSubtarget.ASTC ||
                             EditorUserBuildSettings.androidBuildSubtarget == MobileTextureSubtarget.ETC2;
                    },
                    FixItMessage = "Open Project Settings > 'Texture compression'.",
                    FixIt = () =>
                    {
                        EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ETC2;
                    },
                    Error = false
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Using '32-bit Display Buffer*'.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return PlayerSettings.use32BitDisplayBuffer;
                    },
                    FixItMessage = "Open Project Settings > Player Settings > Player> Resolution and Presentation > 'Use 32-bit Display Buffer*' set enable.",
                    FixIt = () =>
                    {
                        PlayerSettings.use32BitDisplayBuffer = true;
                    },
                    Error = false
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Using Multithreaded Rendering.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return PlayerSettings.GetMobileMTRendering(BuildTargetGroup.Android);
                    },
                    FixItMessage = "Open Project Settings > Player Settings > Player> Other Settings > 'Multithreaded Rendering' set to enable.",
                    FixIt = () =>
                    {
                        PlayerSettings.SetMobileMTRendering(BuildTargetGroup.Android, true);
                    },
                    Error = false
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Using recommended 'Pixel Light Count'.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                        {
                            return QualitySettings.pixelLightCount <= 1;
                        }
                        return true;
                    },
                    FixItMessage = "Open Project Settings > Player Settings> Quality> 'Pixel Light Count' set to 1.",
                    FixIt = () =>
                    {
                        QualitySettings.pixelLightCount = 1;
                    },
                    Error = false
                },

#if UNITY_2022_2_OR_NEWER
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Using recommended Texture Quality.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return QualitySettings.globalTextureMipmapLimit == 0;
                    },
                    FixItMessage = "Open Project Settings > Player Settings> Quality> 'Global Mipmap Limit' set to '0: Full Resolution'.",
                    FixIt = () =>
                    {
                        QualitySettings.globalTextureMipmapLimit = 0;
                    },
                    Error = false
                },
#else
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Using recommended Texture Quality.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return QualitySettings.masterTextureLimit == 0;
                    },
                    FixItMessage = "Open Project Settings > Player Settings> Quality> 'Texture Quality' set to 'Full Res'.",
                    FixIt = () =>
                    {
                        QualitySettings.masterTextureLimit = 0;
                    },
                    Error = false
                },
#endif
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Using recommended 'Anisotropic Texture'.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return QualitySettings.anisotropicFiltering == AnisotropicFiltering.Enable;
                    },
                    FixItMessage = "Open Project Settings > Player Settings> Quality> 'Anisotropic Texture' set to 'Per Texture'.",
                    FixIt = () =>
                    {
                        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                    },
                    Error = false
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Using rendering path: forward.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Android, Graphics.activeTier).renderingPath == RenderingPath.Forward;
                    },
                    FixItMessage = "Open Project Settings > Player Settings> Graphics > RenderingPath.Forward.",
                    FixIt = () =>
                    {
                        var renderingTier = EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Android, Graphics.activeTier);
                        renderingTier.renderingPath = RenderingPath.Forward;
                        EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.Android, Graphics.activeTier, renderingTier);
                    },
                    Error = false
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Using stereo rendering mode: multiview.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return GetSettings().stereoRenderingModeAndroid == PXR_Settings.StereoRenderingModeAndroid.Multiview;
                    },
                    FixItMessage = "Open Project Settings > Player Settings> PICO > Stereo Rendering Mode: Multiview.",
                    FixIt = () =>
                    {
                        GetSettings().stereoRenderingModeAndroid = PXR_Settings.StereoRenderingModeAndroid.Multiview;
                    },
                    Error = false
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Using Default Contact Offset: 0.01f.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return Physics.defaultContactOffset >= 0.01f;
                    },
                    FixItMessage = "Open Project Settings > Player Settings> Physics > Default Contact Offset: 0.01f.",
                    FixIt = () =>
                    {
                        Physics.defaultContactOffset = 0.01f;
                    },
                    Error = false
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Using Sleep Threshold: 0.005f.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return Physics.sleepThreshold >= 0.005f;
                    },
                    FixItMessage = "Open Project Settings > Player Settings> Physics > Sleep Threshold: 0.005f.",
                    FixIt = () =>
                    {
                        Physics.sleepThreshold = 0.005f;
                    },
                    Error = false
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Using Default Solver Iterations: 8.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return Physics.defaultSolverIterations <= 8;
                    },
                    FixItMessage = "Open Project Settings > Player Settings> Physics > Default Solver Iterations: 8.",
                    FixIt = () =>
                    {
                        Physics.defaultSolverIterations = 8;
                    },
                    Error = false
                },
#if URP
            new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "When using URP, set IntermediateTextureMode.Auto.",
                    CheckPredicate = () =>
                    {
                        if (QualitySettings.renderPipeline != null && GraphicsSettings.currentRenderPipeline!= null)
                        {
                            UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;
                            var path = AssetDatabase.GetAssetPath(universalRenderPipelineAsset);
                            var dependency = AssetDatabase.GetDependencies(path);
                            for (int i = 0; i < dependency.Length; i++)
                            {
                                if (AssetDatabase.GetMainAssetTypeAtPath(dependency[i]) != typeof(UniversalRendererData))
                                    continue;

                                UniversalRendererData renderData = (UniversalRendererData)AssetDatabase.LoadAssetAtPath(dependency[i], typeof(UniversalRendererData));
                                return renderData.intermediateTextureMode == IntermediateTextureMode.Auto;
                            }
                        }
                        return true;
                    },
                    FixItMessage = "Open Universal Render Pipeline Asset_Renderer > set IntermediateTextureMode.Auto.",
                    FixIt = () =>
                    {
                        if (QualitySettings.renderPipeline != null && GraphicsSettings.currentRenderPipeline!= null)
                        {
                            UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;
                            var path = AssetDatabase.GetAssetPath(universalRenderPipelineAsset);
                            var dependency = AssetDatabase.GetDependencies(path);
                            for (int i = 0; i < dependency.Length; i++)
                            {
                                if (AssetDatabase.GetMainAssetTypeAtPath(dependency[i]) != typeof(UniversalRendererData))
                                    continue;

                                UniversalRendererData renderData = (UniversalRendererData)AssetDatabase.LoadAssetAtPath(dependency[i], typeof(UniversalRendererData));
                                renderData.intermediateTextureMode = IntermediateTextureMode.Auto;
                            }
                        }
                    },
                    Error = false
                },
            new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "When using URP, set disable SSAO.",
                    CheckPredicate = () =>
                    {
                        if (QualitySettings.renderPipeline != null && GraphicsSettings.currentRenderPipeline!= null)
                        {
                            UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;
                            var path = AssetDatabase.GetAssetPath(universalRenderPipelineAsset);
                            var dependency = AssetDatabase.GetDependencies(path);
                            for (int i = 0; i < dependency.Length; i++)
                            {
                                if (AssetDatabase.GetMainAssetTypeAtPath(dependency[i]) != typeof(UniversalRendererData))
                                    continue;

                                UniversalRendererData renderData = (UniversalRendererData)AssetDatabase.LoadAssetAtPath(dependency[i], typeof(UniversalRendererData));

                                return renderData.rendererFeatures.Count == 0 || !renderData.rendererFeatures.Any(feature => feature != null && (feature.isActive && feature.GetType().Name == "ScreenSpaceAmbientOcclusion"));
                            }
                        }
                        return true;
                    },
                    FixItMessage = "Open Universal Render Pipeline Asset_Renderer > disable ScreenSpaceAmbientOcclusion.",
                    FixIt = () =>
                    {
                        if (QualitySettings.renderPipeline != null && GraphicsSettings.currentRenderPipeline!= null)
                        {
                            UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;
                            var path = AssetDatabase.GetAssetPath(universalRenderPipelineAsset);
                            var dependency = AssetDatabase.GetDependencies(path);
                            for (int i = 0; i < dependency.Length; i++)
                            {
                                if (AssetDatabase.GetMainAssetTypeAtPath(dependency[i]) != typeof(UniversalRendererData))
                                    continue;

                                UniversalRendererData renderData = (UniversalRendererData)AssetDatabase.LoadAssetAtPath(dependency[i], typeof(UniversalRendererData));
                                foreach( var feature in renderData.rendererFeatures)
                                {
                                    if (feature != null && feature.GetType().Name == "ScreenSpaceAmbientOcclusion")
                                    feature.SetActive(false);
                                }
                            }
                        }
                    },
                    Error = false
                },
            new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "When the URP package is installed but not set up and used, it is recommended to use or delete it.",
                    CheckPredicate = () =>
                    {
                        if (QualitySettings.renderPipeline == null && GraphicsSettings.currentRenderPipeline == null)
                        {
                            return false;
                        }
                        return true;
                    },
                    FixItMessage = "If you are not clear about how to set it, you can click 'Fix' to navigate to the designated developer documentation page and follow the instructions to set it.",
                    FixIt = () =>
                    {
                        string url = "https://developer-cn.picoxr.com/document/unity/universal-render-pipeline/";
                        Application.OpenURL(url);
                    },
                    Error = false
                },
#endif

        };
        BuildValidator.AddRules(BuildTargetGroup.Android, androidGlobalRules); 
    }

    static bool IsPXRPluginEnabled()
    {
        var generalSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(
            BuildTargetGroup.Android);
        if (generalSettings == null)
            return false;

        var managerSettings = generalSettings.AssignedSettings;

        return managerSettings != null && managerSettings.activeLoaders.Any(loader => loader is PXR_Loader);
    }

    static PXR_Settings GetSettings()
    {
        PXR_Settings settings = null;
#if UNITY_EDITOR
        UnityEditor.EditorBuildSettings.TryGetConfigObject<PXR_Settings>("Unity.XR.PXR.Settings", out settings);
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
            settings = PXR_Settings.settings;
#endif
        return settings;
    }

    public static List<T> FindComponentsInScene<T>() where T : Component
    {
        var activeScene = SceneManager.GetActiveScene();
        var foundComponents = new List<T>();

        var rootObjects = activeScene.GetRootGameObjects();
        foreach (var rootObject in rootObjects)
        {
            var components = rootObject.GetComponentsInChildren<T>(true);
            foundComponents.AddRange(components);
        }

        return foundComponents;
    }
}