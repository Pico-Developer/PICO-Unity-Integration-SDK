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
using System.Xml;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEngine.XR.Management;

namespace Unity.XR.PXR.Editor
{
    public class PXR_BuildProcessor : XRBuildHelper<PXR_Settings>
    {
        public override string BuildSettingsKey { get { return "Unity.XR.PXR.Settings"; } }
        public static bool IsLoaderExists()
        {
            XRGeneralSettings generalSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
            if (generalSettings == null) return false;
            var assignedSettings = generalSettings.AssignedSettings;
            if (assignedSettings == null) return false;
#if UNITY_2021_1_OR_NEWER
            foreach (XRLoader loader in assignedSettings.activeLoaders)
            {
                if (loader is PXR_Loader) return true;
            }
#else
            foreach (XRLoader loader in assignedSettings.loaders)
            {
                if (loader is PXR_Loader) return true;
            }
#endif

            return false;
        }
        private PXR_Settings PxrSettings
        {
            get
            {
                return SettingsForBuildTargetGroup(BuildTargetGroup.Android) as PXR_Settings;
            }
        }
        private void SetRequiredPluginInBuild()
        {
            PluginImporter[] plugins = PluginImporter.GetAllImporters();
            foreach (PluginImporter plugin in plugins)
            {
                if (plugin.assetPath.Contains("PxrPlatform.aar") || plugin.assetPath.Contains("pxr_api-release.aar"))
                {
                    plugin.SetIncludeInBuildDelegate((path) =>
                    {
                        return IsLoaderExists();
                    });
                }
            }
        }

        public override void OnPreprocessBuild(BuildReport report)
        {
            SetRequiredPluginInBuild();
            if (report.summary.platformGroup != BuildTargetGroup.Android)
                return;
            if (!IsLoaderExists())
                return;
            GraphicsDeviceType firstGfxType =
                PlayerSettings.GetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget)[0];
            if (firstGfxType != GraphicsDeviceType.OpenGLES3 && firstGfxType != GraphicsDeviceType.Vulkan && firstGfxType != GraphicsDeviceType.OpenGLES2)
                throw new BuildFailedException($"PICO Plugin on mobile platforms nonsupport the {firstGfxType}");
            if (PxrSettings.stereoRenderingModeAndroid == PXR_Settings.StereoRenderingModeAndroid.Multiview && firstGfxType == GraphicsDeviceType.OpenGLES2)
                PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new GraphicsDeviceType[] { GraphicsDeviceType.OpenGLES3 });
            if (PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel26)
                throw new BuildFailedException("Android Minimum API must be set to 26 or higher for PICO Plugin.");
            base.OnPreprocessBuild(report);
        }
    }

#if UNITY_2021_3_OR_NEWER
    internal class PXR_BuildHooks : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder { get; }

        private static readonly Dictionary<string, string> AndroidBootConfigVars = new Dictionary<string, string>()
        {
        };

        public void OnPreprocessBuild(BuildReport report)
        {

            if (report.summary.platformGroup == BuildTargetGroup.Android)
            {

                var bootConfig = new BootConfig(report);
                bootConfig.ReadBootConfig();

                foreach (var entry in AndroidBootConfigVars)
                {
                    bootConfig.SetValueForKey(entry.Key, entry.Value);
                }

                bootConfig.WriteBootConfig();
            }

        }

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platformGroup == BuildTargetGroup.Android)
            {
                BootConfig bootConfig = new BootConfig(report);
                bootConfig.ReadBootConfig();

                foreach (KeyValuePair<string, string> entry in AndroidBootConfigVars)
                {
                    bootConfig.ClearEntryForKeyAndValue(entry.Key, entry.Value);
                }

                bootConfig.WriteBootConfig();

            }

        }
    }

    /// <summary>
    /// Small utility class for reading, updating and writing boot config.
    /// </summary>
    internal class BootConfig
    {
        private const string XrBootSettingsKey = "xr-boot-settings";

        private readonly Dictionary<string, string> bootConfigSettings;
        private readonly string buildTargetName;

        public BootConfig(BuildReport report)
        {
            bootConfigSettings = new Dictionary<string, string>();
            buildTargetName = BuildPipeline.GetBuildTargetName(report.summary.platform);
        }

        public void ReadBootConfig()
        {
            bootConfigSettings.Clear();

            string xrBootSettings = EditorUserBuildSettings.GetPlatformSettings(buildTargetName, XrBootSettingsKey);
            if (!string.IsNullOrEmpty(xrBootSettings))
            {
                var bootSettings = xrBootSettings.Split(';');
                foreach (var bootSetting in bootSettings)
                {
                    var setting = bootSetting.Split(':');
                    if (setting.Length == 2 && !string.IsNullOrEmpty(setting[0]) && !string.IsNullOrEmpty(setting[1]))
                    {
                        bootConfigSettings.Add(setting[0], setting[1]);
                    }
                }
            }
        }

        public void SetValueForKey(string key, string value) => bootConfigSettings[key] = value;

        public bool TryGetValue(string key, out string value) => bootConfigSettings.TryGetValue(key, out value);

        public void ClearEntryForKeyAndValue(string key, string value)
        {
            if (bootConfigSettings.TryGetValue(key, out string dictValue) && dictValue == value)
            {
                bootConfigSettings.Remove(key);
            }
        }

        public void WriteBootConfig()
        {
            bool firstEntry = true;
            var sb = new System.Text.StringBuilder();
            foreach (var kvp in bootConfigSettings)
            {
                if (!firstEntry)
                {
                    sb.Append(";");
                }

                sb.Append($"{kvp.Key}:{kvp.Value}");
                firstEntry = false;
            }

            EditorUserBuildSettings.SetPlatformSettings(buildTargetName, XrBootSettingsKey, sb.ToString());
        }
    }

#endif

#if UNITY_ANDROID
    internal class PXR_Manifest : IPostGenerateGradleAndroidProject
    {
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            if (!PXR_BuildProcessor.IsLoaderExists())
                return;
            string originManifestPath = path + "/src/main/AndroidManifest.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(originManifestPath);
            string manifestTagPath = "/manifest";
            string applicationTagPath = manifestTagPath + "/application";
            string metaDataTagPath = applicationTagPath + "/meta-data";
            string usesPermissionTagName = "uses-permission";
            var settings = PXR_XmlTools.GetSettings();
            doc.InsertAttributeInTargetTag(applicationTagPath,null, new Dictionary<string, string>() {{"requestLegacyExternalStorage", "true"}});
            doc.InsertAttributeInTargetTag(metaDataTagPath,new Dictionary<string, string>{{"name","pvr.app.type"}},new Dictionary<string, string>{{"value","vr"}});
            doc.InsertAttributeInTargetTag(metaDataTagPath,new Dictionary<string, string>{{"name","pvr.sdk.version"}},new Dictionary<string, string>{{"value","XR Platform_2.5.3"}});
            doc.InsertAttributeInTargetTag(metaDataTagPath,new Dictionary<string, string>{{"name","pxr.sdk.version_code"}},new Dictionary<string, string>{{"value", "5900"}});
            doc.InsertAttributeInTargetTag(metaDataTagPath,new Dictionary<string, string>{{"name","enable_cpt"}},new Dictionary<string, string>{{"value",PXR_ProjectSetting.GetProjectConfig().useContentProtect ? "1" : "0"}});
            doc.InsertAttributeInTargetTag(metaDataTagPath,new Dictionary<string, string>{{"name","handtracking"}},new Dictionary<string, string> {{"value",PXR_ProjectSetting.GetProjectConfig().handTracking ? "1" : "0" }});
            doc.InsertAttributeInTargetTag(metaDataTagPath,new Dictionary<string, string>{{"name","rendering_mode"}},new Dictionary<string, string>{{"value",((int)settings.stereoRenderingModeAndroid).ToString()}});
            doc.InsertAttributeInTargetTag(metaDataTagPath,new Dictionary<string, string>{{"name","display_rate"}},new Dictionary<string, string>{{"value",((int)settings.systemDisplayFrequency).ToString()}});
            doc.InsertAttributeInTargetTag(metaDataTagPath,new Dictionary<string, string>{{"name","color_Space"}},new Dictionary<string, string>{{"value",QualitySettings.activeColorSpace == ColorSpace.Linear ? "1" : "0"}});
            doc.InsertAttributeInTargetTag(metaDataTagPath,new Dictionary<string, string>{{"name","MRCsupport"}},new Dictionary<string, string>{{"value",PXR_ProjectSetting.GetProjectConfig().openMRC ? "1" : "0" }});
            doc.InsertAttributeInTargetTag(metaDataTagPath,new Dictionary<string, string>{{"name","pvr.LateLatching"}}, new Dictionary<string, string> {{"value",PXR_ProjectSetting.GetProjectConfig().latelatching ? "1" : "0" } });
            doc.InsertAttributeInTargetTag(metaDataTagPath,new Dictionary<string, string>{{"name","pvr.LateLatchingDebug"}}, new Dictionary<string, string> {{"value", PXR_ProjectSetting.GetProjectConfig().latelatching && PXR_ProjectSetting.GetProjectConfig().latelatchingDebug ? "1" : "0" } });
            doc.InsertAttributeInTargetTag(metaDataTagPath,new Dictionary<string, string>{{"name","pvr.app.splash"} },new Dictionary<string, string>{{"value",settings.GetSystemSplashScreen(path)}});
            doc.InsertAttributeInTargetTag(metaDataTagPath,new Dictionary<string, string>{{"name","PICO.swift.feature"}},new Dictionary<string, string>{{"value",PXR_ProjectSetting.GetProjectConfig().bodyTracking ? "1" : "0" }});
            doc.InsertAttributeInTargetTag(metaDataTagPath,new Dictionary<string, string>{{"name","adaptive_resolution"}},new Dictionary<string, string>{{"value",PXR_ProjectSetting.GetProjectConfig().adaptiveResolution ? "1" : "0" }});
            doc.InsertAttributeInTargetTag(metaDataTagPath, new Dictionary<string, string> { { "name", "mr_map_require" } }, new Dictionary<string, string> { { "value", PXR_ProjectSetting.GetProjectConfig().spatialAnchor ? "1" : "0" } });
            doc.InsertAttributeInTargetTag(metaDataTagPath, new Dictionary<string, string> { { "name", "enable_mr" } }, new Dictionary<string, string> { { "value", PXR_ProjectSetting.GetProjectConfig().spatialAnchor ? "1" : "0" } });
            doc.InsertAttributeInTargetTag(metaDataTagPath, new Dictionary<string, string> { { "name", "enable_vst" } }, new Dictionary<string, string> { { "value", PXR_ProjectSetting.GetProjectConfig().videoSeeThrough ? "1" : "0" } });
            doc.InsertAttributeInTargetTag(metaDataTagPath, new Dictionary<string, string> { { "name", "enable_anchor" } }, new Dictionary<string, string> { { "value", PXR_ProjectSetting.GetProjectConfig().spatialAnchor ? "1" : "0" } });
            doc.InsertAttributeInTargetTag(metaDataTagPath, new Dictionary<string, string> { { "name", "mr_map_mgr_auto_start" } }, new Dictionary<string, string> { { "value", PXR_ProjectSetting.GetProjectConfig().spatialAnchor ? "1" : "0" } });
            doc.InsertAttributeInTargetTag(metaDataTagPath, new Dictionary<string, string> { { "name", "pvr.SuperResolution" } }, new Dictionary<string, string> { { "value", PXR_ProjectSetting.GetProjectConfig().superResolution ? "1" : "0" } });
            doc.InsertAttributeInTargetTag(metaDataTagPath, new Dictionary<string, string> { { "name", "pvr.NormalSharpening" } }, new Dictionary<string, string> { { "value", PXR_ProjectSetting.GetProjectConfig().normalSharpening ? "1" : "0" } });
            doc.InsertAttributeInTargetTag(metaDataTagPath, new Dictionary<string, string> { { "name", "pvr.QualitySharpening" } }, new Dictionary<string, string> { { "value", PXR_ProjectSetting.GetProjectConfig().qualitySharpening ? "1" : "0" } });
            doc.InsertAttributeInTargetTag(metaDataTagPath, new Dictionary<string, string> { { "name", "pvr.FixedFoveatedSharpening" } }, new Dictionary<string, string> { { "value", PXR_ProjectSetting.GetProjectConfig().fixedFoveatedSharpening ? "1" : "0" } });
            doc.InsertAttributeInTargetTag(metaDataTagPath, new Dictionary<string, string> { { "name", "pvr.SelfAdaptiveSharpening" } }, new Dictionary<string, string> { { "value", PXR_ProjectSetting.GetProjectConfig().selfAdaptiveSharpening ? "1" : "0" } });
            doc.CreateElementInTag(manifestTagPath,usesPermissionTagName,new Dictionary<string, string>{{"name","android.permission.WRITE_SETTINGS"}});

            if (PXR_ProjectSetting.GetProjectConfig().eyeTracking || PXR_ProjectSetting.GetProjectConfig().enableETFR)
            {
                doc.CreateElementInTag(manifestTagPath, usesPermissionTagName, new Dictionary<string, string> { { "name", "com.picovr.permission.EYE_TRACKING" } });
                doc.InsertAttributeInTargetTag(metaDataTagPath, new Dictionary<string, string> { { "name", "picovr.software.eye_tracking" } }, new Dictionary<string, string> { { "value", "1" } });
                doc.InsertAttributeInTargetTag(metaDataTagPath, new Dictionary<string, string> { { "name", "eyetracking_calibration" } }, new Dictionary<string, string> { { "value", PXR_ProjectSetting.GetProjectConfig().eyetrackingCalibration ? "true" : "false" } });
            }

            if (PXR_ProjectSetting.GetProjectConfig().faceTracking) { doc.CreateElementInTag(manifestTagPath, usesPermissionTagName, new Dictionary<string, string> { { "name", "com.picovr.permission.FACE_TRACKING" } }); }
            if (PXR_ProjectSetting.GetProjectConfig().lipsyncTracking) { doc.CreateElementInTag(manifestTagPath, usesPermissionTagName, new Dictionary<string, string> { { "name", "android.permission.RECORD_AUDIO" } }); }
            if (PXR_ProjectSetting.GetProjectConfig().faceTracking) { doc.InsertAttributeInTargetTag(metaDataTagPath, new Dictionary<string, string> { { "name", "picovr.software.face_tracking" } }, new Dictionary<string, string> { { "value", "false/true" } }); }
            doc.Save(originManifestPath);
        }
        public int callbackOrder { get { return 10000; } }
    }
#endif

    public static class PXR_XmlTools
    {
        static readonly string androidURI = "http://schemas.android.com/apk/res/android";
        public static void InsertAttributeInTargetTag(this XmlDocument doc, string tagPath, Dictionary<string, string> filterDic, Dictionary<string, string> attributeDic)
        {
            XmlElement targetElement = null;
            if (filterDic == null)
                targetElement = doc.SelectSingleNode(tagPath) as XmlElement;
            else
            {
                XmlNodeList nodeList = doc.SelectNodes(tagPath);
                if (nodeList != null)
                {
                    foreach (XmlNode node in nodeList)
                    {
                        if (FilterCheck(node as XmlElement, filterDic))
                        {
                            targetElement = node as XmlElement;
                            break;
                        }
                    }
                }
            }
            if (targetElement == null)
            {
                string parentPath = tagPath.Substring(0, tagPath.LastIndexOf("/"));
                string tagName = tagPath.Substring(tagPath.LastIndexOf("/") + 1);
                foreach (var item in attributeDic)
                    filterDic.Add(item.Key, item.Value);
                doc.CreateElementInTag(parentPath, tagName, filterDic);
            }
            else UpdateOrCreateAttribute(targetElement, attributeDic);
        }

        public static void CreateElementInTag(this XmlDocument doc, string parentPath, string tagName,
            Dictionary<string, string> attributeDic)
        {
            XmlNode parentNode = doc.SelectSingleNode(parentPath);
            if (parentNode == null)
            {
                return;
            }

            foreach (XmlNode childNode in parentNode.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    if (FilterCheck((XmlElement)childNode, attributeDic))
                        return;
                }
            }

            XmlElement newElement = doc.CreateElement(tagName);
            UpdateOrCreateAttribute(newElement, attributeDic);
            parentNode.AppendChild(newElement);
        }
        private static bool FilterCheck(XmlElement element, Dictionary<string, string> filterDic)
        {
            foreach (var filterCase in filterDic)
            {
                string caseValue = element.GetAttribute(filterCase.Key, androidURI);
                if (String.IsNullOrEmpty(caseValue) || caseValue != filterCase.Value)
                    return false;
            }
            return true;
        }
        private static void UpdateOrCreateAttribute(XmlElement element, Dictionary<string, string> attributeDic)
        {
            foreach (var attributeItem in attributeDic)
            {
                element.SetAttribute(attributeItem.Key, androidURI, attributeItem.Value);
            }
        }
        public static PXR_Settings GetSettings()
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
    }
}