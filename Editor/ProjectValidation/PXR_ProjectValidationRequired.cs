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
#if AR_FOUNDATION
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
#if AR_FOUNDATION
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
                    CheckPredicate = () =>
                    {
                        if (QualitySettings.renderPipeline != null && GraphicsSettings.currentRenderPipeline!= null)
                        {
                            UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;
                            return !universalRenderPipelineAsset.supportsHDR;
                        }
                        return true;
                    },
                    FixItMessage = "Open Universal Render Pipeline Asset > Quality > disable HDR.",
                    FixIt = () =>
                    {
                        UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;
                        universalRenderPipelineAsset.supportsHDR = false;
                    },
                    Error = true
                },
#endif                
                new BuildValidationRule
                {
                    Category = k_Catergory,
                    Message = "Project Keystore needs to be set up.",
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