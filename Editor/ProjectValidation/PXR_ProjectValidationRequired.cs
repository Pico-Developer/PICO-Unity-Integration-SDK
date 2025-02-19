using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils.Editor;
using Unity.XR.PXR;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;
using Unity.XR.CoreUtils;
using UnityEditor.PackageManager.UI;
#if URP
using UnityEngine.Rendering.Universal;
#endif
#if AR_FOUNDATION_5 || AR_FOUNDATION_6
using UnityEngine.XR.ARFoundation;
#endif
namespace Unity.XR.PXR
{
    static class PXR_ProjectValidationRequired
    {
        const string k_Catergory = "PICO XR Required";

        [InitializeOnLoadMethod]
        static void AddRequiredRules()
        {
#if UNITY_2021_1_OR_NEWER
            NamedBuildTarget recommendedBuildTarget = NamedBuildTarget.Android;
#else
        BuildTargetGroup recommendedBuildTarget = BuildTargetGroup.Android;
#endif
            const AndroidSdkVersions minSdkVersionInEditor = AndroidSdkVersions.AndroidApiLevel29;
            const AndroidSdkVersions maxSdkVersionInEditor = (AndroidSdkVersions)32;
            const string minSdkNameInEditor = "Android 10.0";

            var androidGlobalRules = new[]
            {
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = $"PICO XR SDK targeting minimum Android 10.0 is required or {minSdkNameInEditor} API Level.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return PlayerSettings.Android.minSdkVersion >= minSdkVersionInEditor;
                    },
                    FixItMessage = "Open Project Settings > Player Settings > Player> Other Settings > Android tab to set PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29.",
                    FixIt = () =>
                    {
                        PlayerSettings.Android.minSdkVersion = minSdkVersionInEditor;
                    },
                    Error = true
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = $"When setting 'Write Permission' to 'External(SDCard)', the Android API level needs to be <= 32.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        if (PlayerSettings.Android.forceSDCardPermission)
                        {
                            if(PlayerSettings.Android.minSdkVersion > maxSdkVersionInEditor)
                            {
                                return false;
                            }

                            if(PlayerSettings.Android.targetSdkVersion > maxSdkVersionInEditor)
                            {
                                return false;
                            }

                            if (PlayerSettings.Android.targetSdkVersion == AndroidSdkVersions.AndroidApiLevelAuto)
                            {
                                return false;
                            }
                            return true;
                        }
                        return true;
                    },
                    FixItMessage = "You can click 'Fix' to navigate to the designated developer documentation page and follow the instructions to set it. ",
                    FixIt = () =>
                    {
                         if(PlayerSettings.Android.minSdkVersion > maxSdkVersionInEditor)
                         {
                            PlayerSettings.Android.minSdkVersion = minSdkVersionInEditor;
                         }
                         string url = "https://developer.picoxr.com/zh/document/unity/set-up-read-and-write-permission-for-pico-4-ultra/";
                         Application.OpenURL(url);
                    },
                    Error = true
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "'Target Architectures' and 'Scripting Backend' must be matched!  Recommended use ARM64 architecture and IL2CPP scripting.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        if ((PlayerSettings.Android.targetArchitectures & AndroidArchitecture.ARM64) != AndroidArchitecture.None)
                        {
                            return PlayerSettings.GetScriptingBackend(recommendedBuildTarget) == ScriptingImplementation.IL2CPP;
                        }else if((PlayerSettings.Android.targetArchitectures & AndroidArchitecture.ARMv7) != AndroidArchitecture.None)
                        {
                            return PlayerSettings.GetScriptingBackend(recommendedBuildTarget) == ScriptingImplementation.Mono2x;
                        }
                        return false;
                    },
                    FixItMessage = "Open Project Settings > Player Settings > Player> Other Settings > Android tab and ensure 'Scripting Backend'" +
                        " is set to 'IL2CPP'. Then under 'Target Architectures' enable 'ARM64'.",
                    FixIt = () =>
                    {
                        PlayerSettings.SetScriptingBackend(recommendedBuildTarget, ScriptingImplementation.IL2CPP);
                        PlayerSettings.Android.targetArchitectures |= AndroidArchitecture.ARM64;
                    },
                    Error = true
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Using 'UIOrientation.LandscapeLeft'.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return PlayerSettings.defaultInterfaceOrientation == UIOrientation.LandscapeLeft;
                    },
                    FixItMessage = "Open Project Settings > Player Settings > Player> Resolution and Presentation > 'Default Orientation' set 'LandscapeLeft'.",
                    FixIt = () =>
                    {
                        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
                    },
                    Error = true
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = $"When using FaceTracking, it is necessary to allow unsafe codes!",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        if (PXR_ProjectSetting.GetProjectConfig().faceTracking)
                        {
                            return PlayerSettings.allowUnsafeCode;
                        }
                        return true;
                    },
                    FixItMessage = "Open Project Settings > Player Settings > Player> Other Settings > Allow 'unsafe' Code",
                    FixIt = () =>
                    {
                        if (PXR_ProjectSetting.GetProjectConfig().faceTracking)
                        {
                            PlayerSettings.allowUnsafeCode = true;
                        }
                    },
                    Error = true
                },
#if UNITY_2022
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = $"On Unity2022, it is not allowed to check 'Development Build' when using Vulkan!",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return !(GraphicsDeviceType.Vulkan == PlayerSettings.GetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget)[0] &&  EditorUserBuildSettings.development);
                    },
                    FixItMessage = "Build Settings > uncheck 'Development Build'",
                    FixIt = () =>
                    {
                        EditorUserBuildSettings.development = false;
                    },
                    Error = true
                },
#endif
#if UNITY_2023_1_OR_NEWER                
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = $"Please use Activity instead of GameActivity!",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return PlayerSettings.Android.applicationEntry == AndroidApplicationEntry.Activity;
                    },
                    FixItMessage = "Open Project Settings > Player Settings > Player> Other Settings > Application Entry Point: Activity",
                    FixIt = () =>
                    {
                        PlayerSettings.Android.applicationEntry = AndroidApplicationEntry.Activity;
                    },
                    Error = true
                },
#endif
            new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = $"Build target platform needs to be modified to Android!",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;
                    },
                    FixItMessage = "Open Project Settings > Platform> Android",
                    FixIt = () =>
                    {
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                    },
                    Error = true
                },

            new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = $"'PXR_Manager' needs to be added in the scene!",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {   
#if AR_FOUNDATION_5 || AR_FOUNDATION_6
                        if (PXR_ProjectSetting.GetProjectConfig().arFoundation)
                        {
                            List<ARCameraManager> components = FindComponentsInScene<ARCameraManager>().Where(component => (component.enabled && component.gameObject.CompareTag("MainCamera"))).ToList();
                            if (components.Count > 0)
                            {
                                return true;
                            }
                        }
#endif
                        return FindComponentsInScene<PXR_Manager>().Where(component => component.isActiveAndEnabled).ToList().Count >= 1;
                    },
                    FixItMessage = "Add 'PXR_Manager' on 'MainCamera''s root parent transform",
                    FixIt = () =>
                    {
                        List<Camera> components = FindComponentsInScene<Camera>().Where(component => (component.enabled && component.gameObject.CompareTag("MainCamera"))).ToList();
                        Debug.LogFormat($"components.Count = {components.Count}");
                        for (int i = 0; i < components.Count; i++)
                        {
                            GameObject gameObject = components[i].transform.gameObject;
                            XROrigin[] xROrigins = gameObject.GetComponentsInParent<XROrigin>();
                            if(xROrigins.Length > 0)
                            {
                                Transform rootTransform = xROrigins[0].transform;
                                if(!rootTransform.GetComponent<PXR_Manager>())
                                {
                                    rootTransform.gameObject.AddComponent<PXR_Manager>();
                                }
                                else
                                {
                                    rootTransform.GetComponent<PXR_Manager>().enabled = true;
                                }
                            }
                        }
                    },
                    Error = true
                },
        new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Only one 'XROrigin' is allowed in the scene!",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return FindComponentsInScene<XROrigin>().Where(component => component.isActiveAndEnabled).ToList().Count ==1;
                    },
                    FixItMessage = "XROrigin > Disable.",
                    FixIt = () =>
                    {
                        List<XROrigin> components = FindComponentsInScene<XROrigin>().Where(component => component.isActiveAndEnabled).ToList();
                        if (components.Count == 0)
                        {
                            if(!EditorApplication.ExecuteMenuItem("GameObject/XR/XR Origin (VR)"))
                            {
                                EditorApplication.ExecuteMenuItem("GameObject/XR/XR Origin (Action-based)");
                            }
                            return;
                        }
                        for(int i=1; i < components.Count; i++)
                        {
                            components[i].transform.gameObject.SetActive(false);
                        }
                    },
                    Error = true
                },
                 new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = $"Only one 'MainCamera' is allowed in the scene!",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        List<Camera> components = FindComponentsInScene<Camera>().Where(component => (component.isActiveAndEnabled && component.gameObject.CompareTag("MainCamera"))).ToList();
                        if (components.Count == 1)
                        {
                            GameObject gameObject = components[0].transform.gameObject;
                            return gameObject.GetComponentsInParent<XROrigin>().Length == 1;
                        }
                        return false;
                    },
                    FixItMessage = "Scene > MainCamera > Disable.",
                    FixIt = () =>
                    {
                        List<Camera> components = FindComponentsInScene<Camera>().Where(component => (component.enabled && component.gameObject.CompareTag("MainCamera"))).ToList();
                        for(int i=0; i < components.Count; i++)
                        {
                            GameObject gameObject = components[i].transform.gameObject;
                            if(gameObject.GetComponentsInParent<XROrigin>().Length == 1)
                            {
                                gameObject.SetActive(true);
                            }
                            else
                            {
                                string newTag = $"Camera{i}";
                                PXR_Utils.AddNewTag(newTag);
                                gameObject.tag = newTag;
                                gameObject.SetActive(false);
                            }
                        }
                    },
                    Error = true
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = $"Only one 'AudioListener' is allowed in the scene!",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return FindComponentsInScene<AudioListener>().Where(component => component.isActiveAndEnabled).ToList().Count <= 1;
                    },
                    FixItMessage = "Disable 'AudioListener' on non 'MainCamera'",
                    FixIt = () =>
                    {
                        List<AudioListener> components = FindComponentsInScene<AudioListener>().Where(component => component.isActiveAndEnabled).ToList();
                        foreach (var component in components)
                        {
                            component.enabled = component.gameObject.CompareTag("MainCamera");
                            EditorSceneManager.MarkSceneDirty(component.gameObject.scene);
                        }
                    },
                    Error = true
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Set the Graphics API order (Vulkan or OpenGLES3) for Android.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        var buildTarget = BuildTarget.Android;
                        if (PlayerSettings.GetUseDefaultGraphicsAPIs(buildTarget))
                        {
                            return true;
                        }

                        return PlayerSettings.GetGraphicsAPIs(buildTarget).Any(item => item == GraphicsDeviceType.OpenGLES3 || item == GraphicsDeviceType.Vulkan);
                    },
                    FixItMessage = "Open Project Settings > Player Settings > Player> Other Settings > 'Graphics API' set Vulkan for Android.",
                    FixIt = () =>
                    {
                        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
                        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.Vulkan });
                    },
                    Error = true
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "When using ETFR, need to set Graphics API: 'OpenGLES3'.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        if (PXR_ProjectSetting.GetProjectConfig().enableETFR)
                        {
                            var buildTarget = BuildTarget.Android;
                            if (PlayerSettings.GetUseDefaultGraphicsAPIs(buildTarget))
                            {
                                return false;
                            }
                            return GraphicsDeviceType.OpenGLES3 == PlayerSettings.GetGraphicsAPIs(buildTarget)[0];
                        }
                        return true;
                    },
                    FixItMessage = "Open Project Settings > Player Settings > Player> Other Settings > 'Graphics API' set OpenGLES3 for Android.",
                    FixIt = () =>
                    {
                        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
                        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
                    },
                    Error = true
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "When using Sharpening, Subsampling needs to be disabled.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        if (PXR_ProjectSetting.GetProjectConfig().normalSharpening||PXR_ProjectSetting.GetProjectConfig().qualitySharpening)
                        {
                            return !PXR_ProjectSetting.GetProjectConfig().enableSubsampled;
                        }
                        return true;
                    },
                    FixItMessage = "Open PXR_Manager > Subsampling: disabled.",
                    FixIt = () =>
                    {
                        PXR_ProjectSetting.GetProjectConfig().enableSubsampled = false;
                    },
                    Error = true
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "When using Super Resolution, Subsampling needs to be disabled.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        if (PXR_ProjectSetting.GetProjectConfig().superResolution)
                        {
                            return !PXR_ProjectSetting.GetProjectConfig().enableSubsampled;
                        }
                        return true;
                    },
                    FixItMessage = "Open PXR_Manager > Subsampling: disabled.",
                    FixIt = () =>
                    {
                        PXR_ProjectSetting.GetProjectConfig().enableSubsampled = false;
                    },
                    Error = true
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "When using MR features, need to set ARM64 architecture and IL2CPP scripting.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        if (PXR_ProjectSetting.GetProjectConfig().spatialAnchor || PXR_ProjectSetting.GetProjectConfig().sceneCapture || PXR_ProjectSetting.GetProjectConfig().spatialMesh || PXR_ProjectSetting.GetProjectConfig().sharedAnchor)
                        {
                            return (PlayerSettings.Android.targetArchitectures & AndroidArchitecture.ARM64) != AndroidArchitecture.None && PlayerSettings.GetScriptingBackend(recommendedBuildTarget) == ScriptingImplementation.IL2CPP;
                        }
                        return true;
                    },
                    FixItMessage = "Open Project Settings > Player Settings > Player> Other Settings > Android tab and ensure 'Scripting Backend'" +
                        " is set to 'IL2CPP'. Then under 'Target Architectures' enable 'ARM64'.",
                    FixIt = () =>
                    {
                        PlayerSettings.SetScriptingBackend(recommendedBuildTarget, ScriptingImplementation.IL2CPP);
                        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
                    },
                    Error = true
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "PICO XR plugin needs to be enabled and unique.",
                    CheckPredicate = () =>
                    {
                        var generalSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
                        if (!generalSettings)
                        {
                            return false;
                        }
                        IReadOnlyList<XRLoader> list = generalSettings.Manager.activeLoaders;

                        if (list.Count == 0)
                        {
                            return false;
                        }else if (list.Count > 1)
                        {
                            return false;
                        }
                        else
                        {
                            return IsPXRPluginEnabled();
                        }
                    },
                    FixItMessage = "Open Project Settings > Player Settings > XR Plug-in Management>  enable 'PICO'.",
                    FixIt = () =>
                    {
                        var generalSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
                        if (generalSettings)
                        {
                            IReadOnlyList<XRLoader> list = generalSettings.Manager.activeLoaders;
                            while (list.Count > 0)
                            {
                                  string nameTemp = list[0].GetType().FullName;
                                  XRPackageMetadataStore.RemoveLoader(generalSettings.Manager, nameTemp, BuildTargetGroup.Android);
                            }
                            XRPackageMetadataStore.AssignLoader(generalSettings.Manager, "PXR_Loader", BuildTargetGroup.Android);
                        }
                    },
                    Error = true
                },
#if URP
#if UNITY_2021_3_OR_NEWER || UNITY_2022_3_OR_NEWER
            new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "When using URP, it is necessary to set Quality > Render Pipeline Asset.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        if (GraphicsSettings.currentRenderPipeline!= null)
                        {
                            return QualitySettings.renderPipeline != null;
                        }

                        return true;
                    },
                    FixItMessage = "Open Project Settings > Player Settings > Quality> Render Pipeline Asset.",
                    FixIt = () =>
                    {
                        var pipelineAssets = new List<RenderPipelineAsset>();
                        QualitySettings.GetAllRenderPipelineAssetsForPlatform("Android", ref pipelineAssets);
                        RenderPipelineAsset renderPipeline = pipelineAssets[0];
                        if (QualitySettings.renderPipeline == null)
                        {
                            QualitySettings.renderPipeline = renderPipeline;
                        }
                    },
                    Error = true
                },
#endif
#if UNITY_2022
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = $"On Unity2022, it is not recommended msaa4 when using URP+Linear+OpenGLES3.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        if (QualitySettings.renderPipeline != null && GraphicsSettings.currentRenderPipeline!= null && PlayerSettings.colorSpace == ColorSpace.Linear
                        && GraphicsDeviceType.OpenGLES3 == PlayerSettings.GetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget)[0])
                        {
                            UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;
                            return universalRenderPipelineAsset.msaaSampleCount != 4;
                        }
                        return true;
                    },
                    FixItMessage = "Open Universal Render Pipeline Asset > Quality > Anti Aliasing(MSAA) > Disabled.",
                    FixIt = () =>
                    {
                            UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;
                            universalRenderPipelineAsset.msaaSampleCount = 1;
                    },
                    Error = true
                },
#endif
            new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "When using URP, HDR needs to be disabled.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        bool isHDR = false;
                        if (QualitySettings.renderPipeline != null)
                        {
                            UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
                            isHDR = universalRenderPipelineAsset.supportsHDR;

                        }else if(GraphicsSettings.currentRenderPipeline!= null)
                        {
                            UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;
                            isHDR = universalRenderPipelineAsset.supportsHDR;
                        }
                        return !isHDR;
                    },
                    FixItMessage = "Open Universal Render Pipeline Asset > Quality > disable HDR.",
                    FixIt = () =>
                    {
                        if (QualitySettings.renderPipeline != null)
                        {
                            UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
                            universalRenderPipelineAsset.supportsHDR = false;

                        }else if(GraphicsSettings.currentRenderPipeline!= null)
                        {
                            UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;
                            universalRenderPipelineAsset.supportsHDR = false;
                        }
                    },
                    Error = true
                },
            new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "When using URP and VST, Post Processing needs to be disabled.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        if (QualitySettings.renderPipeline != null && GraphicsSettings.currentRenderPipeline!= null)
                        {
                            UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;

                            Camera mainCamera = PXR_Utils.GetMainCameraForXROrigin();
                            if(mainCamera.clearFlags == CameraClearFlags.SolidColor && mainCamera.backgroundColor == new Color(0, 0, 0, 0))
                            {
                                bool isPostProcessingEnabled = mainCamera.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing;
                                return !isPostProcessingEnabled;
                            }

                            return true;
                        }
                        return true;
                    },
                    FixItMessage = "Scene > MainCamera > Post Processing > Disable.",
                    FixIt = () =>
                    {
                        if (QualitySettings.renderPipeline != null && GraphicsSettings.currentRenderPipeline!= null)
                        {
                            UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;

                            Camera mainCamera = PXR_Utils.GetMainCameraForXROrigin();
                            if(mainCamera.clearFlags == CameraClearFlags.SolidColor && mainCamera.backgroundColor == new Color(0, 0, 0, 0))
                            {
                                mainCamera.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = false;
                            }
                        }
                    },
                    Error = true
                },
            new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "When using URP, The ETFR/FFR function will fail.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return !PXR_ProjectSetting.GetProjectConfig().validationFFREnabled && !PXR_ProjectSetting.GetProjectConfig().validationETFREnabled;
                    },
                    FixItMessage = "You can click 'Fix' to navigate to the designated developer documentation page and follow the instructions to set it. ",
                    FixIt = () =>
                    {
                         string url = "https://developer.picoxr.com/document/unity/fixed-foveated-rendering/";
                         Application.OpenURL(url);
                    },
                    Error = true
                },
#if UNITY_6000_0_OR_NEWER
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "When using URP+OpenGLES+MultiPass, The MSAA needs to be disabled.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        var buildTarget = BuildTarget.Android;
                        if (PlayerSettings.GetUseDefaultGraphicsAPIs(buildTarget))
                        {
                            return true;
                        }

                        if (PlayerSettings.GetGraphicsAPIs(buildTarget)[0] == GraphicsDeviceType.Vulkan)
                        {
                            return true;
                        }

                        if(GetSettings().stereoRenderingModeAndroid == PXR_Settings.StereoRenderingModeAndroid.Multiview)
                        {
                            return true;
                        }

                        int msaaSampleCount = 1;
                        if (QualitySettings.renderPipeline != null)
                        {
                            UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
                            msaaSampleCount = universalRenderPipelineAsset.msaaSampleCount;

                        }else if(GraphicsSettings.currentRenderPipeline!= null)
                        {
                            UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;
                            msaaSampleCount = universalRenderPipelineAsset.msaaSampleCount;
                        }

                        return msaaSampleCount==1;
                    },
                    FixItMessage = "Open Universal Render Pipeline Asset > Quality/Graphics > Anti Aliasing(MSAA) > Disabled.",
                    FixIt = () =>
                    {
                        if (QualitySettings.renderPipeline != null)
                        {
                            UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
                            universalRenderPipelineAsset.msaaSampleCount = 1;

                        }else if(GraphicsSettings.currentRenderPipeline!= null)
                        {
                            UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;
                            universalRenderPipelineAsset.msaaSampleCount = 1;
                        }
                    },
                    Error = true
                },
#endif
#endif
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Project Keystore needs to be set up.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        string keystorePath = PlayerSettings.Android.keystoreName;
                        string keystorePass = PlayerSettings.Android.keystorePass;

                        if (string.IsNullOrEmpty(keystorePath) || string.IsNullOrEmpty(keystorePass))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }

                    },
                    FixItMessage = "You can refer to the following path: Open Project Settings > Player Settings > Player > Publishing Settings > to set 'Project Keystore'. \nIf you are not clear about how to set it, you can click 'Fix' to navigate to the designated developer documentation page and follow the instructions to set it. ",
                    FixIt = () =>
                    {
                        string url = "https://developer-cn.picoxr.com/document/unity/number-of-apks-associated-with-a-key-exceeds-limit/";
                        Application.OpenURL(url);
                    },
                    Error = true
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Project Key needs to be set up.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        string keyaliasName = PlayerSettings.Android.keyaliasName;
                        string keyaliasPass = PlayerSettings.Android.keyaliasPass;

                        if (string.IsNullOrEmpty(keyaliasName) || string.IsNullOrEmpty(keyaliasPass))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }

                    },
                    FixItMessage = "You can refer to the following path: Open Project Settings > Player Settings > Player > Publishing Settings > to set 'Project Key'. \nIf you are not clear about how to set it, you can click 'Fix' to navigate to the designated developer documentation page and follow the instructions to set it. ",
                    FixIt = () =>
                    {
                        string url = "https://developer-cn.picoxr.com/document/unity/number-of-apks-associated-with-a-key-exceeds-limit/";
                        Application.OpenURL(url);
                    },
                    Error = true
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "The range of official Unity versions supported by PICO SDK is from 2020.3.21 to Unity 6. ",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
#if UNITY_2020_3_OR_NEWER
                        string curVersion = Application.unityVersion;
                        string minVersion = PXR_Utils.minUnityVersion;
                        int comparisonResult = PXR_Utils.CompareUnityVersions(curVersion, minVersion);

                        if (comparisonResult > 0)
                        {
                            return true;
                        }
                        else if (comparisonResult < 0)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
#else
                        return false;
#endif
        },
                    FixItMessage = "You can Use Unity 2020.3.21 - Unity 6. ",
                    FixIt = () =>
                    {
                        string url = "https://developer.picoxr.com/resources/";
                        Application.OpenURL(url);
                    },
                    Error = true
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
                    Error = true
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Use ' Late Latching' need Unity 2021.3.19f1+ LTS.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        if (PXR_ProjectSetting.GetProjectConfig().latelatching)
                        {
                            string curVersion = Application.unityVersion;
                            string minVersion = "2021.3.19f1";
                            int comparisonResult = PXR_Utils.CompareUnityVersions(curVersion, minVersion);

                            if (comparisonResult > 0)
                            {
                                return true;
                            }
                            else if (comparisonResult < 0)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        return true;
                    },
                    FixItMessage = "Open PXR_Manager > Late Latching: disabled.",
                    FixIt = () =>
                    {
                        PXR_ProjectSetting.GetProjectConfig().latelatching = false;
                    },
                    Error = true
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = $"Late latching and composite layers cannot be used simultaneously as they can cause jitter in the composite layer! ",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        if (PXR_ProjectSetting.GetProjectConfig().latelatching)
                        {
                            return FindComponentsInScene<PXR_OverLay>().Where(component => component.isActiveAndEnabled).ToList().Count == 0;
                        }
                        return true;
                    },
                    FixItMessage = "Open PXR_Manager > Late Latching: disabled.",
                    FixIt = () =>
                    {
                        PXR_ProjectSetting.GetProjectConfig().latelatching = false;
                    },
                    Error = true
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = $"A single scene supports up to 7 compositor layers!",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        return FindComponentsInScene<PXR_OverLay>().Where(component => component.isActiveAndEnabled).ToList().Count <= 7;
                    },
                    FixItMessage = "You can click 'Fix' to navigate to the designated developer documentation page and follow the instructions to set it. ",
                    FixIt = () =>
                    {
                        string url = "https://developer.picoxr.com/en/document/unity/vr-compositor-layers/";
                        Application.OpenURL(url);
                    },
                    Error = true
                },
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "When using MRC in Vulkan, ColorSpace should be set to Linear.",
                    IsRuleEnabled = IsPXRPluginEnabled,
                    CheckPredicate = () =>
                    {
                        if (PXR_ProjectSetting.GetProjectConfig().openMRC)
                        {
                            var buildTarget = BuildTarget.Android;
                            if (PlayerSettings.GetUseDefaultGraphicsAPIs(buildTarget) || GraphicsDeviceType.Vulkan == PlayerSettings.GetGraphicsAPIs(buildTarget)[0])
                            {
                                return PlayerSettings.colorSpace == ColorSpace.Linear;
                            }
                        }
                        return true;
                    },
                    FixItMessage = "Open Project Settings > Player Settings > Player> Other Settings > 'Color Space' set to 'Linear'.",
                    FixIt = () =>
                    {
                        PlayerSettings.colorSpace = ColorSpace.Linear;
                    },
                    Error = true
                },
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
}