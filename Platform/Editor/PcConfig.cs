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
using System.IO;
using LitJson;
using UnityEditor;
using UnityEngine;

namespace Pico.Platform.Editor
{
    /// <summary>
    /// Unity Setting Getter and Setter
    /// </summary>
    public enum Region
    {
        cn = 0,
        i18n = 1,
    }

    public class PcConfig : ScriptableObject
    {
        public Region region = Region.cn;
        public string accessToken = "";
        internal bool hasError = false;
    }

    [CustomEditor(typeof(PcConfig))]
    public class PcConfigEditor : UnityEditor.Editor
    {
        static string filepath = "Assets/Resources/PicoSdkPCConfig.json";
        private static string i18nLink = "https://developer-global.pico-interactive.com/document/unity/pc-end-debugging-tool";
        private static string cnLink = "https://developer-cn.pico-interactive.com/document/unity/pc-end-debugging-tool";

        public override void OnInspectorGUI()
        {
            var x = Selection.activeObject as PcConfig;
            if (x.hasError)
            {
                EditorGUILayout.LabelField("Config file error,please check the file");
                return;
            }

            base.OnInspectorGUI();

            //Read the document
            {
                GUILayout.Space(5);
                var referenceStyle = new GUIStyle(EditorStyles.label);
                referenceStyle.normal.textColor = new Color(0, 122f / 255f, 204f / 255f);
                referenceStyle.focused.textColor = new Color(0, 122f / 255f, 204f / 255f);
                referenceStyle.hover.textColor = new Color(0, 122f / 255f, 204f / 255f);
                if (GUILayout.Button("Read the document", referenceStyle))
                {
                    var link = i18nLink;
                    if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified || Application.systemLanguage == SystemLanguage.ChineseTraditional)
                    {
                        link = cnLink;
                    }

                    Application.OpenURL(link);
                }
            }
            this.save();
        }

        public static PcConfig load()
        {
            var obj = CreateInstance<PcConfig>();
            obj.hasError = false;
            try
            {
                if (File.Exists(filepath))
                {
                    var jsonContent = File.ReadAllText(filepath);
                    var jsonConf = JsonMapper.ToObject(jsonContent);
                    obj.accessToken = jsonConf["account"]["access_token"].ToString();
                    if (!Region.TryParse(jsonConf["general"]["region"].ToString() ?? "", out obj.region))
                    {
                        obj.region = Region.cn;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                obj.hasError = true;
            }

            return obj;
        }

        public void save()
        {
            var obj = Selection.activeObject as PcConfig;
            if (obj.hasError)
            {
                return;
            }

            var conf = new JsonData();
            conf["general"] = new JsonData();
            conf["account"] = new JsonData();
            conf["package"] = new JsonData();
            conf["general"]["region"] = obj.region.ToString();
            conf["account"]["access_token"] = obj.accessToken.Trim();
            conf["package"]["package_name"] = Gs.packageName.Trim();
            conf["package"]["package_version_code"] = Gs.bundleVersionCode;
            conf["package"]["package_version_name"] = Gs.bundleVersion;
            File.WriteAllText(filepath, JsonMapper.ToJson(conf));
        }
    }
}