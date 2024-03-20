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
using UnityEngine;
using UnityEngine.XR.Management;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace Unity.XR.PXR
{
    [Serializable]
    [XRConfigurationData("PICO", "Unity.XR.PXR.Settings")]
    public class PXR_Settings : ScriptableObject
    {
        public enum StereoRenderingModeAndroid
        {
            MultiPass,
            Multiview
        }

        public enum SystemDisplayFrequency
        {
            Default,
            RefreshRate72,
            RefreshRate90,
            RefreshRate120,
        }

        [SerializeField, Tooltip("Set the Stereo Rendering Method")]
        public StereoRenderingModeAndroid stereoRenderingModeAndroid;

        [SerializeField, Tooltip("Set the system display frequency")]
        public SystemDisplayFrequency systemDisplayFrequency;

        [SerializeField, Tooltip("if enabled,will always discarding depth and resolving MSAA color to improve performance on tile-based architectures. This only affects Vulkan. Note that this may break user content")]
        public bool optimizeBufferDiscards = true;
        
        [SerializeField, Tooltip("Enable Application SpaceWarp")]
        public bool enableAppSpaceWarp;

        [SerializeField, Tooltip("Set the system splash screen picture in PNG format")]
        public Texture2D systemSplashScreen;

        private string splashPath = string.Empty;


        public ushort GetStereoRenderingMode()
        {
            return (ushort)stereoRenderingModeAndroid;
        }
        public ushort GetSystemDisplayFrequency()
        {
            return (ushort)systemDisplayFrequency;
        }

        public ushort GetOptimizeBufferDiscards()
        {
            return optimizeBufferDiscards ? (ushort)1 : (ushort)0;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
		public static PXR_Settings settings;
		public void Awake()
		{
            settings = this;
		}
        
#elif UNITY_EDITOR
        private void OnValidate()
        {
            if (systemSplashScreen == null)
            {
                return;
            }

            splashPath = AssetDatabase.GetAssetPath(systemSplashScreen);
            if (!string.Equals(Path.GetExtension(splashPath), ".png", StringComparison.OrdinalIgnoreCase))
            {
                systemSplashScreen = null;
                Debug.LogError("Invalid file format of System Splash Screen, only PNG format is supported. The asset path: " + splashPath); 
                 splashPath = string.Empty;
            }
        }

        public string GetSystemSplashScreen(string path)
        {
            if (systemSplashScreen == null || splashPath == string.Empty)
            {
                return "0";
            }

            string targetPath = Path.Combine(path, "src/main/assets/pico_splash.png");
            FileUtil.ReplaceFile(splashPath, targetPath);
            return "1";
        }
#endif
    }
}
