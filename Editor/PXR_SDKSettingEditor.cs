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

using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor.XR.Management;
using UnityEngine.XR.Management;
using UnityEditor.XR.Management.Metadata;

namespace Unity.XR.PXR.Editor
{
    [InitializeOnLoad]
    public class PXR_SDKSettingEditor : EditorWindow
    {
        public static string assetPath = "Assets/Resources/";
        private const string titleName = "PICO Integration SDK";
        private const string windowName = titleName + "Window";
        private static PXR_SDKSettingEditor instance;
        private static PXR_EditorStyles _styles;
        public event Action<Response> WhenResponded = delegate { };
        private const string PICO_ICON_NAME = "PICO developer.png";
        private Vector2 scrollPosition = Vector2.zero;
        private const BuildTarget recommendedBuildTarget = BuildTarget.Android;

        public enum Response
        {
            Configs,
            Tools,
            Samples,
            About,
        }

        private Dictionary<Response, bool> buttonClickedStates = new Dictionary<Response, bool>()
        {
            { Response.Configs, false },
            { Response.Tools, false },
            { Response.Samples, false },
            { Response.About, false }
        };

        Action openProjectValidationAction = () =>
        {
            SettingsService.OpenProjectSettings("Project/XR Plug-in Management/Project Validation");

            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Tools_ProjectValidation_Open);
        };
        Action applyMinAndroidAPIAction = () =>
        {
            PlayerSettings.Android.minSdkVersion = PXR_Utils.minSdkVersionInEditor;

            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Configs_RequiredAndroidSdkVersionsApplied);
        };

        Action applyPICOXRPluginAction = () =>
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

            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Configs_RequiredPICOXRPluginApplied);
        };
        Action applyBuildTargetAction = () =>
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, recommendedBuildTarget);
            EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Android;

            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Configs_RequiredBuildTargetAndroidApplied);
        };


        [MenuItem("PICO/Portal", false, 0)]
        public static void ShowWindow()
        {
            if (instance == null)
            {
                instance = GetWindow<PXR_SDKSettingEditor>();
                instance.Show();
            }
            else
            {
                instance.Focus();
            }
            string version = "_UnityXR_" + PXR_Plugin.System.UPxr_GetSDKVersion() + "_" + Application.unityVersion;
            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Enter + version);
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            if (!PXR_ProjectSetting.GetProjectConfig().portalInited)
            {
                EditorApplication.delayCall += () =>
                {
                    EditorApplication.update += UpdateOnce;
                };

            }
        }

        static void UpdateOnce()
        {
            EditorApplication.update -= UpdateOnce;
            ShowWindow();
            PXR_ProjectSetting.GetProjectConfig().portalInited = true;
            PXR_ProjectSetting.SaveAssets();
        }

        private void Awake()
        {
            _styles ??= new PXR_EditorStyles();
            titleContent = new GUIContent(titleName);
            minSize = new Vector2(1080, 640);
            maxSize = minSize + new Vector2(2, 2);
            EditorApplication.delayCall += () => maxSize = new Vector2(4000, 4000);

            buttonClickedStates[Response.Configs] = true;
        }

        private void OnDestroy()
        {
            instance = null;
        }


        private void OnGUI()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                CloseWindow();
            }

            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.Space(20);
                DrawTitle(titleName);
                EditorGUILayout.Space(10);

                EditorGUILayout.Separator();

                DrawHorizontalLine(_styles.colorLine, 2);
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(30);
                    DrawLeftButton();
                    GUILayout.Space(30);
                    DrawVerticalLine(_styles.colorLine, 2);
                    GUILayout.Space(30);

                    Rect windowRect = position;
                    float xOffset = 30 + 200 + 30;
                    float width = windowRect.width - xOffset;
                    float topSpaceUsed = 30 + _styles.HeaderText.fixedHeight + 30;
                    float height = windowRect.height - topSpaceUsed - 30 - 2;

                    _styles.BackgroundColor.fixedWidth = width;
                    _styles.BackgroundColor.fixedHeight = height;
                    using (new GUILayout.VerticalScope(_styles.BackgroundColor))
                    {
                        if (buttonClickedStates[Response.Configs])
                        {
                            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
                            GUILayout.Space(30);
                            using (new EditorGUILayout.VerticalScope())
                            {
                                string title = "Information";
                                GUILayout.Label(title, _styles.BigWhiteTitleStyle, GUILayout.ExpandWidth(true));
                                string bodyContent = "Supported Unity Version: Unity 2020.3.21 and above.";
                                EditorGUILayout.LabelField(bodyContent, _styles.ContentText);

                                GUILayout.Space(30);
                                title = "Configuration";
                                GUILayout.Label(title, _styles.BigWhiteTitleStyle, GUILayout.ExpandWidth(true));

                                string strinfo = "Required: PICO XR plugin needs to be enabled and unique.";
                                EditorConfigurations(strinfo, PXR_Utils.IsPXRPluginEnabled(), applyPICOXRPluginAction);

                                strinfo = $"Required: Build Target = {recommendedBuildTarget}";
                                bool appliedBuildTarget = EditorUserBuildSettings.activeBuildTarget == recommendedBuildTarget;
                                EditorConfigurations(strinfo, appliedBuildTarget, applyBuildTargetAction);

                                strinfo = $"Required: AndroidSdkVersions = {PXR_Utils.minSdkVersionInEditor}";
                                bool appliedAdroidSdkVersions = PlayerSettings.Android.minSdkVersion == PXR_Utils.minSdkVersionInEditor;
                                EditorConfigurations(strinfo, appliedAdroidSdkVersions, applyMinAndroidAPIAction);

                                bool applied = PXR_Utils.IsPXRPluginEnabled() && appliedBuildTarget && appliedAdroidSdkVersions;
                                if (!applied)
                                {
                                    using (new EditorGUILayout.VerticalScope())
                                    {
                                        GUILayout.Space(10);
                                        bodyContent = "For one-click configuration, you can click 'To Apply' one by one or use 'To Apply All'.";
                                        EditorGUILayout.LabelField(bodyContent, _styles.ContentText);

                                        GUILayout.Space(10);

                                        using (new EditorGUILayout.HorizontalScope())
                                        {
                                            GUILayout.Space(8);
                                            bodyContent = "To Apply All";
                                            if (GUILayout.Button(bodyContent, _styles.ButtonToOpen, GUILayout.ExpandWidth(false)))
                                            {
                                                applyPICOXRPluginAction.Invoke();
                                                applyBuildTargetAction.Invoke();
                                                applyMinAndroidAPIAction.Invoke();

                                                PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Configs_ToApplyAllApplied);
                                            }

                                            var buttonRectToApplyAll = GUILayoutUtility.GetLastRect();
                                            if (Event.current.type == EventType.Repaint)
                                            {
                                                EditorGUIUtility.AddCursorRect(buttonRectToApplyAll, MouseCursor.Link);
                                            }
                                        }
                                    }
                                }

                                GUILayout.Space(20);

                                using (new EditorGUILayout.VerticalScope())
                                {
                                    bodyContent = "For more configuration items, open Project Validation.";
                                    EditorGUILayout.LabelField(bodyContent, _styles.ContentText);
                                    GUILayout.Space(10);

                                    using (new EditorGUILayout.HorizontalScope())
                                    {
                                        GUILayout.Space(8);
                                        bodyContent = "Project Validation";
                                        if (GUILayout.Button(bodyContent, _styles.ButtonToOpen, GUILayout.ExpandWidth(false)))
                                        {
                                            SettingsService.OpenProjectSettings("Project/XR Plug-in Management/Project Validation");
                                            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Configs_ProjectValidation);
                                        }
                                        var buttonRectProjectValidation = GUILayoutUtility.GetLastRect();
                                        if (Event.current.type == EventType.Repaint)
                                        {
                                            EditorGUIUtility.AddCursorRect(buttonRectProjectValidation, MouseCursor.Link);
                                        }
                                    }
                                }

                                GUILayout.Space(30);
                                title = "PICO XR Project Setting";
                                GUILayout.Label(title, _styles.BigWhiteTitleStyle, GUILayout.ExpandWidth(true));
                                bodyContent = $"SDK Settings for turning on and off features. You can locate it at this filepath: {assetPath}PXR_ProjectSetting.asset."; ;
                                EditorGUILayout.LabelField(bodyContent, _styles.ContentText);

                                GUILayout.Space(10);

                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    GUILayout.Space(8);
                                    bodyContent = "Open " + title;
                                    if (GUILayout.Button(bodyContent, _styles.ButtonToOpen, GUILayout.ExpandWidth(false)))
                                    {
                                        PXR_ProjectSetting asset;
                                        string path = assetPath + "PXR_ProjectSetting.asset";
                                        if (!File.Exists(path))
                                        {
                                            asset = new PXR_ProjectSetting();
                                            ScriptableObjectUtility.CreateAsset<PXR_ProjectSetting>(asset, PXR_SDKSettingEditor.assetPath);
                                        }

                                        asset = AssetDatabase.LoadAssetAtPath<PXR_ProjectSetting>(path);
                                        if (asset != null)
                                        {
                                            AssetDatabase.OpenAsset(asset);
                                        }
                                        else
                                        {
                                            Debug.LogError("Asset not found at path: " + assetPath);
                                        }

                                        PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Configs_OpenPICOXRProjectSetting);
                                    }
                                    var buttonRectProjectSetting = GUILayoutUtility.GetLastRect();
                                    if (Event.current.type == EventType.Repaint)
                                    {
                                        EditorGUIUtility.AddCursorRect(buttonRectProjectSetting, MouseCursor.Link);
                                    }
                                }
                            }
                            GUILayout.EndScrollView();
                        }
                        else if (buttonClickedStates[Response.Tools])
                        {
                            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
                            GUILayout.Space(30);
                            using (new EditorGUILayout.VerticalScope())
                            {
                                string title = "Unity Editor Tools and Developer Tools";
                                GUILayout.Label(title, _styles.BigWhiteTitleStyle, GUILayout.ExpandWidth(true));

                                title = "Project Validation";
                                string links = "https://developer.picoxr.com/document/unity/project-validation/";
                                GUIContent bodyContent = new GUIContent("Project Validation can display the validation rules requiredby the installed XR package. For any validation rules that are not properly set up, you can use thhis feature to automatically fix them with a single click.");
                                DrawTwoRowLayout(title, bodyContent, links, openProjectValidationAction);

                                title = "PICO Building Blocks";
                                links = "https://developer.picoxr.com/document/unity/pico-building-blocks/";
                                bodyContent = new GUIContent("The PICO Building Block system allows you to set up features, including those in the SDK and Unity Engine, with a single click.");
                                DrawTwoRowLayout(title, bodyContent, links);

                                title = "PICO XR Toolkit-MR";
                                links = "https://developer.picoxr.com/document/unity/sense-pack-overview/";
                                bodyContent = new GUIContent("The PICO XR Toolkit-MR part is a set of tools included in the SensePack on top of the Mixed Reality API. It is used to perform common operations when building spatial perception applications.");
                                DrawTwoRowLayout(title, bodyContent, links);

                                title = "XR Profiling Toolkit";
                                links = "https://github.com/Pico-Developer/XR-Profiling-Toolkit";
                                bodyContent = new GUIContent("An automated and customizable graphics profiling tool for evaluating the performance of XR applications on cross-vendor headsets.");
                                DrawTwoRowLayout(title, bodyContent, links);

                                title = "PICO Developer Center";
                                links = "https://developer.picoxr.com/resources/#pdc";
                                bodyContent = new GUIContent("PICO Developer Center (referred to as PDC tools below) is a developer service platform that integrates essential tools like the ADB command debugging tool and real-time preview tool. You can efficiently manage, develop, and debug your apps using the PDC tool.");
                                DrawTwoRowLayout(title, bodyContent, links);

                                title = "Emulator";
                                links = "https://developer.picoxr.com/resources/#emulator";
                                bodyContent = new GUIContent("You can install your app on PICO Emulator and run it, so as to preview how your app performs.");
                                DrawTwoRowLayout(title, bodyContent, links);

                                title = "More Developer Tools";
                                links = "https://developer.picoxr.com/document/unity/developer-tools-overview/";
                                bodyContent = new GUIContent("PICO provides a range of developer tools covering app debugging, performance monitoring, haptic editing, and more.See the Developer Tools Documentationpage to learn more details.");
                                DrawTwoRowLayout(title, bodyContent, links);
                            }
                            GUILayout.EndScrollView();
                        }
                        else if (buttonClickedStates[Response.Samples])
                        {
                            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
                            GUILayout.Space(30);
                            using (new EditorGUILayout.VerticalScope())
                            {
                                string title = "PICO Unity Integration SDK Samples";
                                GUILayout.Label(title, _styles.BigWhiteTitleStyle, GUILayout.ExpandWidth(true));

                                GUILayout.Space(30);
                                string bodyContent = "Besides the Samples you can import through the Unity Paackage Manager interface, PICO provides comprehensive sample projects that coverthe core features of the Unity Integration SDK on GitHub.";
                                EditorGUILayout.LabelField(bodyContent, _styles.ContentText);

                                title = "Mixed Reality Sample";
                                string gitHubLink = "https://github.com/Pico-Developer/MRSample-Unity";
                                string documentationLink = "https://developer.picoxr.com/document/unity/mixed-reality-sample/";
                                DrawSDKSampleLayout(title, documentationLink, gitHubLink);

                                title = "Interaction Sample";
                                gitHubLink = "https://github.com/Pico-Developer/InteractionSample-Unity";
                                documentationLink = "https://developer.picoxr.com/document/unity/y3lpmdhw/";
                                DrawSDKSampleLayout(title, documentationLink, gitHubLink);

                                title = "Motion Tracker Sample";
                                gitHubLink = "https://github.com/Pico-Developer/PICOMotionTrackerSample-Unity";
                                documentationLink = "https://developer.picoxr.com/document/unity/6bona7fv/";
                                DrawSDKSampleLayout(title, documentationLink, gitHubLink);

                                title = "Platform Services Sample";
                                gitHubLink = "https://github.com/Pico-Developer/PlatformSample-Unity";
                                documentationLink = "https://developer.picoxr.com/document/unity/simple-demo/";
                                DrawSDKSampleLayout(title, documentationLink, gitHubLink);

                                title = "Spatial Audio Sample";
                                gitHubLink = "https://github.com/Pico-Developer/SpatialAudioSample-Unity";
                                documentationLink = "https://developer.picoxr.com/document/unity/spatial-audio-sample/";
                                DrawSDKSampleLayout(title, documentationLink, gitHubLink);

                                title = "AR Foundation Sample";
                                gitHubLink = "https://github.com/Pico-Developer/PICOARFoundationSamples-Unity";
                                documentationLink = "https://developer.picoxr.com/document/unity/ar-foundation-for-pico-unity-integration-sdk/";
                                DrawSDKSampleLayout(title, documentationLink, gitHubLink);

                                title = "Adaptive Resolution Sample";
                                gitHubLink = "https://github.com/Pico-Developer/AdaptiveResolutionSample-Unity";
                                documentationLink = "https://developer.picoxr.com/document/unity/adaptive-resolution-demo/";
                                DrawSDKSampleLayout(title, documentationLink, gitHubLink);

                                title = "Toon World";
                                gitHubLink = "https://github.com/Pico-Developer/ToonSample-Unity";
                                documentationLink = "https://developer.picoxr.com/document/unity/toon-world/";
                                DrawSDKSampleLayout(title, documentationLink, gitHubLink);

                                title = "MicroWar";
                                gitHubLink = "https://github.com/picoxr/MicroWar";
                                documentationLink = "https://developer.picoxr.com/document/unity/micro-war/";
                                DrawSDKSampleLayout(title, documentationLink, gitHubLink);

                                title = "PICO Avatar Sample";
                                gitHubLink = "https://github.com/Pico-Developer/PICO-Avatar-SDK-Unity";
                                DrawSDKSampleLayout(title, null, gitHubLink);

                                title = "URP Fork";
                                gitHubLink = "https://github.com/Pico-Developer/PICO-URP-Fork";
                                DrawSDKSampleLayout(title, null, gitHubLink);
                            }
                            GUILayout.EndScrollView();
                        }
                        else if (buttonClickedStates[Response.About])
                        {
                            string title = "About the SDK";
                            GUIContent bodyContent = new GUIContent("PICO's official Unity package for developing applications for PICO XR devices.");
                            DrawTwoRowLayout(title, bodyContent);

                            title = "Features";
                            bodyContent = new GUIContent("The SDK provides features covering rendering, input and interaction, mixed reality, spatial audio, motion tracker, platform services, and enterprise features, etc.");
                            DrawTwoRowLayout(title, bodyContent);

                            title = "Documentation";
                            bodyContent = new GUIContent("Please visit the following page on PICO Developer Website for the latest documentation and samples:");
                            DrawTwoRowLayout(title, bodyContent);
                            string link = "https://developer.picoxr.com/document/unity";
                            if (GUILayout.Button(link, _styles.SmallBlueLinkStyle, GUILayout.ExpandWidth(false)))
                            {
                                Application.OpenURL(link);
                                PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_About_Documentation);
                            }
                            var buttonRectDocumentation = GUILayoutUtility.GetLastRect();
                            if (Event.current.type == EventType.Repaint)
                            {
                                EditorGUIUtility.AddCursorRect(buttonRectDocumentation, MouseCursor.Link);
                            }

                            title = "Installation";
                            bodyContent = new GUIContent("We recommend using 'add package from git URL' to add the SDK from the PICO Developer GitHub:");
                            DrawTwoRowLayout(title, bodyContent);

                            link = "https://github.com/Pico-Developer/PICO-Unity-Integration-SDK";
                            if (GUILayout.Button(link, _styles.SmallBlueLinkStyle, GUILayout.ExpandWidth(false)))
                            {
                                Application.OpenURL(link);
                                PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_About_Installation);
                            }

                            var buttonRectInstallation = GUILayoutUtility.GetLastRect();
                            if (Event.current.type == EventType.Repaint)
                            {
                                EditorGUIUtility.AddCursorRect(buttonRectInstallation, MouseCursor.Link);
                            }
                        }
                        GUILayout.FlexibleSpace();
                    }
                }
            }
        }

        private void DrawTitle(string title)
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(title, _styles.HeaderText, GUILayout.ExpandWidth(true));

                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("Version " + PXR_Plugin.System.UPxr_GetSDKVersion(), _styles.VersionText, GUILayout.ExpandWidth(true));

                string iconPath = Path.Combine(PXR_Utils.sdkPackageName, assetPath, PICO_ICON_NAME);
                var content = EditorGUIUtility.TrIconContent(iconPath, "PICO Logo");
                EditorGUILayout.LabelField(content, _styles.IconStyle,
                    GUILayout.Width(_styles.IconStyle.fixedWidth),
                    GUILayout.Height(_styles.IconStyle.fixedHeight), GUILayout.ExpandWidth(true));
            }
        }

        public void DrawTwoRowLayout(string title, GUIContent bodyContent, string link = null, System.Action buttonAction = null, string button = null)
        {
            GUILayout.Space(30);
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label(title, _styles.BigWhiteTitleStyle, GUILayout.ExpandWidth(true));
                    if (link != null)
                    {
                        if (GUILayout.Button("Documentation", _styles.SmallBlueLinkStyle, GUILayout.Width(200)))
                        {
                            Application.OpenURL(link);

                            if (title == "Project Validation")
                            {
                                PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Tools_ProjectValidation_Documentation);
                            }
                            else if (title == "PICO Building Blocks")
                            {
                                PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Tools_BuildingBlocks);
                            }
                            else if (title == "PICO XR Toolkit-MR")
                            {
                                PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Tools_PICOXRToolkitMR);
                            }
                            else if (title == "XR Profiling Toolkit")
                            {
                                PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Tools_XRProfilingToolkit);
                            }
                            else if (title == "PICO Developer Center")
                            {
                                PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Tools_PICODeveloperCenter);
                            }
                            else if (title == "Emulator")
                            {
                                PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Tools_Emulator);
                            }
                            else if (title == "More Developer Tools")
                            {
                                PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Tools_MoreDeveloperTools);
                            }
                        }

                        var buttonRect = GUILayoutUtility.GetLastRect();
                        if (Event.current.type == EventType.Repaint)
                        {
                            EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Link);
                        }
                    }
                    if (buttonAction != null)
                    {
                        string buttonText = button != null ? button : "Open " + title;
                        if (GUILayout.Button(buttonText, _styles.ButtonToOpen, GUILayout.ExpandWidth(false)))
                        {
                            buttonAction?.Invoke();
                        }

                        var buttonRect = GUILayoutUtility.GetLastRect();
                        if (Event.current.type == EventType.Repaint)
                        {
                            EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Link);
                        }
                    }
                    else
                    {
                        GUIStyle Box = new GUIStyle()
                        {
                            fixedWidth = 250,
                        };
                        GUILayout.Box("", Box, GUILayout.ExpandWidth(false));
                    }

                    GUILayout.Space(30);
                }
                EditorGUILayout.Space(10);
                if (bodyContent != null)
                {
                    EditorGUILayout.LabelField(bodyContent, _styles.ContentText);
                }
            }
        }

        private void DrawHorizontalLine(Color color, int thickness)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, thickness);
            EditorGUI.DrawRect(rect, color);
        }
        private void DrawVerticalLine(Color color, int thickness)
        {
            Rect rect = new Rect(220, 122, thickness, Screen.height);
            EditorGUI.DrawRect(rect, color);
        }

        private void DrawLeftButton()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.Space(30);

                var buttons = new[] {
                    ("<size=18>Configs</size>", Response.Configs),
                    ("<size=18>Tools</size>", Response.Tools),
                    ("<size=18>Samples</size>", Response.Samples),
                    ("<size=18>About</size>", Response.About)
                 };

                foreach (var (btnText, response) in buttons)
                {
                    bool isClicked = GUILayout.Button(btnText,
                        buttonClickedStates[response] ? _styles.ButtonSelected : _styles.Button,
                        GUILayout.ExpandHeight(false));

                    var rect = GUILayoutUtility.GetLastRect();
                    if (Event.current.type == EventType.Repaint)
                    {
                        EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
                    }

                    if (isClicked)
                    {
                        ClickedButton(response);
                    }
                    EditorGUILayout.Space(30);
                }
            }
        }

        private void ClickedButton(Response responseT)
        {
            var keys = buttonClickedStates.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                var response = keys[i];
                buttonClickedStates[response] = responseT == response;
            }
            WhenResponded.Invoke(responseT);
            switch (responseT)
            {
                case Response.Configs:
                    PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Configs_Open);
                    break;
                case Response.Tools:
                    PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Tools_Open);
                    break;
                case Response.Samples:
                    PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Sample_Open);
                    break;
                case Response.About:
                    PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_About_Open);
                    break;
                default:
                    break;
            }
        }

        private void DrawSDKSampleLayout(string title, string documentationLink, string gitHubLink)
        {
            GUILayout.Space(20);
            using (new EditorGUILayout.HorizontalScope())
            {
                _styles.BigWhiteTitleStyle.fontStyle = FontStyle.Bold;
                GUILayout.Label(title, _styles.BigWhiteTitleStyle, GUILayout.ExpandWidth(false));

                if (documentationLink != null)
                {
                    GUILayout.Label(" | ", _styles.BigWhiteTitleStyle, GUILayout.Width(20));
                    if (GUILayout.Button("Documentation", _styles.SmallBlueLinkStyle, GUILayout.ExpandWidth(false)))
                    {
                        Application.OpenURL(documentationLink);

                        if (title == "Mixed Reality Sample")
                        {
                            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_MixedRealitySample_Documentation);
                        }
                        else if (title == "Interaction Sample")
                        {
                            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_InteractionSample_Documentation);
                        }
                        else if (title == "Motion Tracker Sample")
                        {
                            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_MotionTrackerSample_Documentation);
                        }
                        else if (title == "Platform Services Sample")
                        {
                            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_PlatformServicesSample_Documentation);
                        }
                        else if (title == "Spatial Audio Sample")
                        {
                            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_SpatialAudioSample_Documentation);
                        }
                        else if (title == "AR Foundation Sample")
                        {
                            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_ARFoundationSample_Documentation);
                        }
                        else if (title == "Adaptive Resolution Sample")
                        {
                            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_AdaptiveResolutionSample_Documentation);
                        }
                        else if (title == "Toon World")
                        {
                            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_ToonWorldSample_Documentation);
                        }
                        else if (title == "MicroWar")
                        {
                            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_MicroWarSample_Documentation);
                        }
                        else if (title == "PICO Avatar Sample")
                        {
                            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_PICOAvatarSample_Documentation);
                        }
                        else if (title == "URP Fork")
                        {
                            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_URPFork_Documentation);
                        }
                    }

                    var buttonRectDoc = GUILayoutUtility.GetLastRect();
                    if (Event.current.type == EventType.Repaint)
                    {
                        EditorGUIUtility.AddCursorRect(buttonRectDoc, MouseCursor.Link);
                    }
                }

                GUILayout.Label(" | ", _styles.BigWhiteTitleStyle, GUILayout.Width(20));
                if (GUILayout.Button("GitHub", _styles.SmallBlueLinkStyle, GUILayout.ExpandWidth(false)))
                {
                    Application.OpenURL(gitHubLink);

                    if (title == "Mixed Reality Sample")
                    {
                        PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_MixedRealitySample_GitHub);
                    }
                    else if (title == "Interaction Sample")
                    {
                        PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_InteractionSample_GitHub);
                    }
                    else if (title == "Motion Tracker Sample")
                    {
                        PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_MotionTrackerSample_GitHub);
                    }
                    else if (title == "Platform Services Sample")
                    {
                        PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_PlatformServicesSample_GitHub);
                    }
                    else if (title == "Spatial Audio Sample")
                    {
                        PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_SpatialAudioSample_GitHub);
                    }
                    else if (title == "AR Foundation Sample")
                    {
                        PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_ARFoundationSample_GitHub);
                    }
                    else if (title == "Adaptive Resolution Sample")
                    {
                        PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_AdaptiveResolutionSample_GitHub);
                    }
                    else if (title == "Toon World")
                    {
                        PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_ToonWorldSample_GitHub);
                    }
                    else if (title == "MicroWar")
                    {
                        PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_MicroWarSample_GitHub);
                    }
                    else if (title == "PICO Avatar Sample")
                    {
                        PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_PICOAvatarSample_GitHub);
                    }
                    else if (title == "URP Fork")
                    {
                        PXR_AppLog.PXR_OnEvent(PXR_AppLog.strPortal, PXR_AppLog.strPortal_Samples_URPFork_GitHub);
                    }
                }

                var buttonRectGitHub = GUILayoutUtility.GetLastRect();
                if (Event.current.type == EventType.Repaint)
                {
                    EditorGUIUtility.AddCursorRect(buttonRectGitHub, MouseCursor.Link);
                }
            }
        }

        void EditorConfigurations(string strConfiguration, bool enable, Action buttonAction)
        {
            EditorGUILayout.BeginHorizontal();
            var iconStyle = new GUIStyle
            {
                fixedWidth = 30,
                stretchHeight = true,
                alignment = TextAnchor.MiddleCenter,
            };
            if (enable)
            {
                GUI.color = Color.green;
                GUILayout.Label(EditorGUIUtility.IconContent("FilterSelectedOnly"), iconStyle);
            }
            else
            {
                GUI.color = Color.yellow;
                GUILayout.Label(EditorGUIUtility.IconContent("console.warnicon"), iconStyle);
            }
            GUI.color = Color.white;
            GUILayout.Label(strConfiguration, _styles.ContentText, GUILayout.Width(480));
            _styles.ContentText.normal.textColor = Color.white;

            GUIStyle styleApplied = new GUIStyle();
            styleApplied.fontSize = 16;
            styleApplied.fixedWidth = 100;
            styleApplied.padding = new RectOffset(4, 4, 4, 4);
            styleApplied.alignment = TextAnchor.MiddleCenter;
            if (enable)
            {
                styleApplied.normal.textColor = Color.green;
                GUILayout.Label("Applied", styleApplied);
            }
            else
            {
                styleApplied.normal.textColor = Color.white;
                styleApplied.normal.background = _styles.MakeTexture(2, 2, _styles.colorSelected);
                if (GUILayout.Button("To Apply", styleApplied))
                {
                    buttonAction?.Invoke();
                }

                var buttonRectToApply = GUILayoutUtility.GetLastRect();
                if (Event.current.type == EventType.Repaint)
                {
                    EditorGUIUtility.AddCursorRect(buttonRectToApply, MouseCursor.Link);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void CloseWindow()
        {
            Close();
        }
    }

    public static class ScriptableObjectUtility
    {
        public static void CreateAsset<T>(T classdata, string path) where T : ScriptableObject
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + typeof(T).ToString() + ".asset");

            AssetDatabase.CreateAsset(classdata, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}