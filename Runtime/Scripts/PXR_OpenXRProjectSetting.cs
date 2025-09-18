#if PICO_OPENXR_SDK
using System.IO;
using Unity.XR.PXR;
using UnityEditor;
using UnityEngine;

namespace Unity.XR.OpenXR.Features.PICOSupport
{
    [System.Serializable]
    public class PXR_OpenXRProjectSetting: ScriptableObject
    {
        public bool useContentProtect;
        public bool isEyeTracking;
        public bool MRSafeguard;
        public bool isHandTracking;
        public bool isEyeTrackingCalibration;
        public bool highFrequencyHand;
        public SystemDisplayFrequency displayFrequency;
        public SecureContentFlag contentProtectFlags ;
        public  bool foveationEnable;
        public FoveationFeature.FoveatedRenderingMode foveatedRenderingMode;
        public  FoveationFeature.FoveatedRenderingLevel foveatedRenderingLevel;
        public bool isSubsampledEnabled;
        public HandTrackingSupport handTrackingSupportType;
        [SerializeField, Tooltip("Set the system splash screen picture in PNG format.")]
        public Texture2D systemSplashScreen;
        private string splashPath = string.Empty;

        public static PXR_OpenXRProjectSetting GetProjectConfig()
        {
            PXR_OpenXRProjectSetting projectConfig = Resources.Load<PXR_OpenXRProjectSetting>("PICOProjectSetting");
#if UNITY_EDITOR
            if (projectConfig == null)
            {
                projectConfig = CreateInstance<PXR_OpenXRProjectSetting>();
                projectConfig.useContentProtect = false;
                projectConfig.contentProtectFlags = SecureContentFlag.SECURE_CONTENT_OFF;
                projectConfig.isEyeTracking = false;
                projectConfig.isEyeTrackingCalibration = false;
                projectConfig.handTrackingSupportType = HandTrackingSupport.ControllersAndHands;
                projectConfig.isHandTracking = false;
                projectConfig.MRSafeguard = false;
                projectConfig.highFrequencyHand = false;
                projectConfig.displayFrequency = SystemDisplayFrequency.Default;
                projectConfig.foveationEnable = false;
                projectConfig.foveatedRenderingMode = FoveationFeature.FoveatedRenderingMode.FixedFoveatedRendering;
                projectConfig.foveatedRenderingLevel = FoveationFeature.FoveatedRenderingLevel.Off;
                projectConfig.isSubsampledEnabled = false;
                string path = Application.dataPath + "/Resources";
                if (!Directory.Exists(path))
                {
                    UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
                    UnityEditor.AssetDatabase.CreateAsset(projectConfig, "Assets/Resources/PICOProjectSetting.asset");
                }
                else
                {
                    UnityEditor.AssetDatabase.CreateAsset(projectConfig, "Assets/Resources/PICOProjectSetting.asset");
                }
            }
#endif
            return projectConfig;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (systemSplashScreen != null)
            {
                splashPath = AssetDatabase.GetAssetPath(systemSplashScreen);
                if (Path.GetExtension(splashPath).ToLower() != ".png")
                {
                    systemSplashScreen = null;
                    Debug.LogError("Invalid file format of System Splash Screen, only PNG format is supported. The asset path: " + splashPath);
                    splashPath = string.Empty;
                }
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
#endif