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
using System.Xml;
using UnityEditor.Android;
using UnityEngine;

namespace Pico.Platform.Editor
{
    public class PlatformManifestRewrite : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 9999;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            var appId = PicoGs.appId;
            if (string.IsNullOrWhiteSpace(appId))
            {
                Debug.Log("appId is ignored");
                return;
            }

            XmlDocument doc = new XmlDocument();
            const string androidUri = "http://schemas.android.com/apk/res/android";
            var manifestPath = Path.Combine(path, "src/main/AndroidManifest.xml");
            doc.Load(manifestPath);
            var app = doc.SelectSingleNode("//application");
            if (app == null) return;
            
            var appIdNode = doc.CreateElement("meta-data");
            appIdNode.SetAttribute("name", androidUri, "pvr.app.id");
            appIdNode.SetAttribute("value", androidUri, appId);
            app.AppendChild(appIdNode);
            
            var highlightNode = doc.CreateElement("meta-data");
            highlightNode.SetAttribute("name", androidUri, "use_record_highlight_feature");
            highlightNode.SetAttribute("value", androidUri, PicoGs.useHighlight.ToString());
            app.AppendChild(highlightNode);
            
            doc.Save(manifestPath);
        }
    }
}