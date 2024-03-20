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
using Unity.XR.PXR;
using UnityEditor;
using UnityEngine;

namespace Pico.Platform.Editor
{
    public class PicoSettings : EditorWindow
    {
        enum Language
        {
            English = 0,
            Chinese = 1,
        }

        private SerializedObject serObj;
        private SerializedProperty gosPty;
        static Language language = Language.English;

        static string[] strAppIdText = {"Paste your App ID here", "请粘贴你的AppID"};
        static string[] strAppIdHelpText = {"App ID is the unique identification ID of the PICO Application. Without AppID, you will not be able to use PICO platform feature.", "APP ID 是应用的唯一标识"};
        static string[] strBuildSettingText = {"Recommend Settings [?]", "推荐设置"};
        static string[] strBuildSettingHelpText = {"Recommended project settings for PXR SDK", "推荐项目设置"};
        static string[] strPlatformBuildText = {"Set Platform To Android", "设置目标平台为Android"};
        static string[] strUnityVersionLimit = {$"Unity Editor Version ≥ {EditorConf.minEditorVersion}", $"Unity Editor版本不小于{EditorConf.minEditorVersion}"};
        static string[] strOrientationBuildText = {"Set Orientation To LandscapeLeft", "设置屏幕方向为水平"};
        static string[] strMinApiLevel = {$"Android Min API Level ≥ {EditorConf.minSdkLevel}", $"Android最小API不低于{EditorConf.minSdkLevel}"};
        static string[] strIgnoreButtonText = {"Ask me later", "稍后询问"};
        static string[] strApplyButtonText = {"Apply", "应用"};
        static string[] strHighlightText = {"Use Highlight", "开启高光时刻"};

        private class Res
        {
            public readonly Texture PicoDeveloper;
            public string Correct = "✔️";
            public string Wrong = "×";
            public GUIStyle correctStyle;
            public GUIStyle wrongStyle;

            public Res()
            {
                this.PicoDeveloper = Resources.Load<Texture>("PICODeveloper");
                correctStyle = new GUIStyle(GUI.skin.label);
                correctStyle.normal.textColor = Color.green;
                wrongStyle = new GUIStyle();
                wrongStyle.normal.textColor = Color.red;
                wrongStyle.fontStyle = FontStyle.Bold;
            }
        }

        private Res _R;

        private Res R
        {
            get
            {
                if (_R != null) return _R;
                _R = new Res();
                return _R;
            }
        }

        internal enum ConfigStatus
        {
            Correct,
            Wrong,
            Fix,
            Hide,
        }

        internal abstract class ConfigField
        {
            public bool value = true;
            public abstract string[] GetText();
            public abstract ConfigStatus GetStatus();
            public abstract void Fix();
        }

        internal class ConfigIsAndroid : ConfigField
        {
            public override string[] GetText()
            {
                return strPlatformBuildText;
            }

            public override ConfigStatus GetStatus()
            {
                return Gs.buildTargetGroup == BuildTargetGroup.Android ? ConfigStatus.Correct : ConfigStatus.Fix;
            }

            public override void Fix()
            {
                Gs.buildTargetGroup = BuildTargetGroup.Android;
            }
        }

        internal class ConfigIsLandscapeLeft : ConfigField
        {
            public override string[] GetText()
            {
                return strOrientationBuildText;
            }

            public override ConfigStatus GetStatus()
            {
                return Gs.UIOrientation == UIOrientation.LandscapeLeft ? ConfigStatus.Correct : ConfigStatus.Fix;
            }

            public override void Fix()
            {
                Gs.UIOrientation = UIOrientation.LandscapeLeft;
            }
        }

        internal class ConfigMinApiLevel : ConfigField
        {
            public override string[] GetText()
            {
                return strMinApiLevel;
            }

            public override ConfigStatus GetStatus()
            {
                return Gs.minimumApiLevel >= (AndroidSdkVersions) EditorConf.minSdkLevel ? ConfigStatus.Correct : ConfigStatus.Fix;
            }

            public override void Fix()
            {
                Gs.minimumApiLevel = (AndroidSdkVersions) EditorConf.minSdkLevel;
            }
        }

        internal class ConfigUnityVersion : ConfigField
        {
            public override string[] GetText()
            {
                return strUnityVersionLimit;
            }

            public override ConfigStatus GetStatus()
            {
                return String.Compare(Application.unityVersion, EditorConf.minEditorVersion, StringComparison.Ordinal) >= 0 ? ConfigStatus.Hide : ConfigStatus.Wrong;
            }

            public override void Fix()
            {
                throw new NotImplementedException();
            }
        }

        public static string appId
        {
            get { return PicoGs.appId; }
            set { PicoGs.appId = value; }
        }
        
        public static bool useHighlight
        {
            get { return PicoGs.useHighlight; }
            set { PicoGs.useHighlight = value; }
        }

        bool enableEC
        {
            get { return PicoGs.enableEntitlementCheck; }
            set { PicoGs.enableEntitlementCheck = value; }
        }

        private ConfigField[] configFields;

        private void OnEnable()
        {
            configFields = new ConfigField[]
            {
                new ConfigUnityVersion(),
                new ConfigIsAndroid(),
                new ConfigIsLandscapeLeft(),
                new ConfigMinApiLevel(),
            };
            this.titleContent = new GUIContent("PICO Platform Settings");
            language = Language.English;
            if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified || Application.systemLanguage == SystemLanguage.ChineseTraditional)
            {
                language = Language.Chinese;
            }

            serObj = new SerializedObject(PXR_PlatformSetting.Instance);
            gosPty = serObj.FindProperty(nameof(PXR_PlatformSetting.deviceSN));
        }


        Vector2 scrollPos;

        void OnGUI()
        {
            var frameWidth = 380;
            //顶部图片
            {
                GUIStyle style = new GUIStyle();
                style.stretchWidth = true;
                style.fixedWidth = 400;
                GUILayout.Label(R.PicoDeveloper, style);
            }


            //顶部中英文选择
            {
                GUIStyle activeStyle = new GUIStyle();
                activeStyle.alignment = TextAnchor.MiddleCenter;
                activeStyle.normal.textColor = new Color(0, 122f / 255f, 204f / 255f);
                GUIStyle normalStyle = new GUIStyle();
                normalStyle.alignment = TextAnchor.MiddleCenter;
                normalStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("ENGLISH", language == Language.English ? activeStyle : normalStyle, GUILayout.Width(80)))
                {
                    language = Language.English;
                }

                GUILayout.Label("|", normalStyle, GUILayout.Width(5));
                if (GUILayout.Button("中文", language == Language.Chinese ? activeStyle : normalStyle, GUILayout.Width(80)))
                {
                    language = Language.Chinese;
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            {
                GUIStyle style = new GUIStyle();
                style.margin = new RectOffset(5, 5, 5, 5);
                GUILayout.BeginVertical(style, GUILayout.Width(360));
            }
            //AppID设置
            {
                GUILayout.Space(15);
                GUILayout.Label(strAppIdText[(int) language]);
                appId = EditorGUILayout.TextField(appId, GUILayout.Width(frameWidth));
                if (string.IsNullOrWhiteSpace(appId))
                {
                    EditorGUILayout.HelpBox(strAppIdHelpText[(int) language], UnityEditor.MessageType.Warning);
                }

                GUILayout.Space(20);
                if (appId == "")
                {
                    GUI.enabled = false;
                    enableEC = false;
                }
                else
                {
                    GUI.enabled = true;
                }
            }
            //Highlight设置
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(strHighlightText[(int) language]);
                useHighlight = EditorGUILayout.Toggle(useHighlight, GUILayout.Width(frameWidth));
                EditorGUILayout.EndHorizontal();
            }
            //Recommend Settings
            {
                GUILayout.Space(5);
                GUILayout.Label(new GUIContent(strBuildSettingText[(int) language], strBuildSettingHelpText[(int) language]));

                GUIStyle style = "frameBox";
                style.fixedWidth = frameWidth;
                EditorGUILayout.BeginVertical(style);

                foreach (var field in configFields)
                {
                    var txt = field.GetText()[(int) language];
                    switch (field.GetStatus())
                    {
                        case ConfigStatus.Correct:
                        {
                            EditorGUILayout.BeginHorizontal(GUILayout.Width(frameWidth));
                            EditorGUILayout.LabelField(txt);
                            EditorGUILayout.LabelField(R.Correct, R.correctStyle);
                            GUI.enabled = true;
                            EditorGUILayout.EndHorizontal();
                            break;
                        }
                        case ConfigStatus.Wrong:
                        {
                            EditorGUILayout.BeginHorizontal(GUILayout.Width(frameWidth));
                            EditorGUILayout.LabelField(txt);
                            EditorGUILayout.LabelField(R.Wrong, R.wrongStyle);
                            EditorGUILayout.EndHorizontal();
                            break;
                        }
                        case ConfigStatus.Hide:
                        {
                            break;
                        }
                        case ConfigStatus.Fix:
                        {
                            EditorGUILayout.BeginHorizontal(GUILayout.Width(frameWidth));
                            EditorGUILayout.LabelField(txt);
                            float originalValue = EditorGUIUtility.labelWidth;
                            EditorGUIUtility.labelWidth = 250;
                            field.value = EditorGUILayout.Toggle(field.value);
                            EditorGUIUtility.labelWidth = originalValue;
                            EditorGUILayout.EndHorizontal();
                            break;
                        }
                        default:
                        {
                            Debug.LogWarning($"unhandled ConfigStatus {txt} {field.GetStatus()}");
                            break;
                        }
                    }
                }

                EditorGUILayout.EndVertical();
            }
            //按钮区域
            {
                var hasSomethingToFix = false;
                foreach (var field in configFields)
                {
                    if (field.GetStatus() == ConfigStatus.Fix && field.value)
                    {
                        hasSomethingToFix = true;
                        break;
                    }
                }

                if (hasSomethingToFix)
                {
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(strIgnoreButtonText[(int) language], GUILayout.Width(130)))
                    {
                        this.Close();
                    }

                    GUI.enabled = hasSomethingToFix;
                    if (GUILayout.Button(strApplyButtonText[(int) language], GUILayout.Width(130)))
                    {
                        this.ApplyRecommendConfig();
                    }

                    GUI.enabled = true;

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.FlexibleSpace();
                }
            }

            GUILayout.EndVertical();
        }

        private void ApplyRecommendConfig()
        {
            foreach (var field in configFields)
            {
                if (field.GetStatus() == ConfigStatus.Fix && field.value)
                {
                    field.Fix();
                }
            }
        }
    }
}