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
using Debug = UnityEngine.Debug;

namespace Unity.XR.PXR.Editor
{
    [InitializeOnLoad]
    public static class PXR_SDKBuildCheck
    {
        private static bool doNotShowAgain = false;

        static PXR_SDKBuildCheck()
        {
            ObjectFactory.componentWasAdded += ComponentWasAdded;
            BuildPlayerWindow.RegisterBuildPlayerHandler(OnBuild);
            doNotShowAgain = GetDoNotShowBuildWarning();
            Debug.Log("PXRLog [Build Check]RegisterBuildPlayerHandler,Already Do not show: " + doNotShowAgain);
        }
        static void ComponentWasAdded(Component com)
        {
            if (com.name == "XR Rig")
            {
                if (!com.GetComponent<PXR_Manager>() && com.GetType() != typeof(Transform))
                {
                    com.gameObject.AddComponent<PXR_Manager>();
                }
            }
        }
        static bool GetDoNotShowBuildWarning()
        {
            string path = PXR_SDKSettingEditor.assetPath + typeof(PXR_SDKSettingAsset).ToString() + ".asset";
            if (File.Exists(path))
            {
                PXR_SDKSettingAsset asset = AssetDatabase.LoadAssetAtPath<PXR_SDKSettingAsset>(path);
                if (asset != null)
                {
                    return asset.doNotShowBuildWarning;
                }

            }
            return false;
        }

        public static void OnBuild(BuildPlayerOptions options)
        {
            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
        }
    }
}

