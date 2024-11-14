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

namespace Unity.XR.PXR
{
    [System.Serializable]
    public class PXR_ProjectSetting : ScriptableObject
    {
        public bool useContentProtect;
        public bool handTracking;
        public bool adaptiveHand;
        public bool highFrequencyHand;
        public bool openMRC;
        public bool faceTracking;
        public bool lipsyncTracking;
        public bool eyeTracking;
        public bool eyetrackingCalibration;
        public bool enableETFR;
        public bool latelatching;
        public bool latelatchingDebug;
        public bool enableSubsampled;
        public bool bodyTracking;
        public bool adaptiveResolution;
        public bool stageMode;
        public bool videoSeeThrough;
        public bool spatialAnchor;
        public bool sceneCapture;
        public bool sharedAnchor;
        public bool spatialMesh;
        public PxrMeshLod meshLod;
        public bool superResolution;
        public bool normalSharpening;
        public bool qualitySharpening;
        public bool fixedFoveatedSharpening;
        public bool selfAdaptiveSharpening;

        public bool arFoundation;
        public bool mrSafeguard;
        public bool enableRecommendMSAA;
        public bool recommendSubsamping;
        public bool recommendMSAA;

        public static PXR_ProjectSetting GetProjectConfig()
        {
            PXR_ProjectSetting projectConfig = Resources.Load<PXR_ProjectSetting>("PXR_ProjectSetting");
#if UNITY_EDITOR
            if (projectConfig == null)
            {
                projectConfig = CreateInstance<PXR_ProjectSetting>();
                projectConfig.useContentProtect = false;
                projectConfig.handTracking = false;
                projectConfig.adaptiveHand = false;
                projectConfig.highFrequencyHand = false;
                projectConfig.openMRC = true;
                projectConfig.faceTracking = false;
                projectConfig.lipsyncTracking = false;
                projectConfig.eyeTracking = false;
                projectConfig.eyetrackingCalibration = false;
                projectConfig.enableETFR = false;
                projectConfig.latelatching = false;
                projectConfig.latelatchingDebug = false;
                projectConfig.enableSubsampled = false;
                projectConfig.bodyTracking = false;
                projectConfig.adaptiveResolution = false;
                projectConfig.stageMode = false;
                projectConfig.videoSeeThrough = false;
                projectConfig.spatialAnchor = false;
                projectConfig.sceneCapture = false;
                projectConfig.sharedAnchor = false;
                projectConfig.spatialMesh = false;
                projectConfig.superResolution = false;
                projectConfig.normalSharpening = false;
                projectConfig.qualitySharpening = false;
                projectConfig.fixedFoveatedSharpening = false;
                projectConfig.selfAdaptiveSharpening = false;
                projectConfig.arFoundation = false;
                projectConfig.mrSafeguard = false;
                projectConfig.enableRecommendMSAA = false;
                projectConfig.recommendSubsamping = false;
                projectConfig.recommendMSAA = false;
                projectConfig.meshLod = PxrMeshLod.Low;

                string path = Application.dataPath + "/Resources";
                if (!Directory.Exists(path))
                {
                    UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
                    UnityEditor.AssetDatabase.CreateAsset(projectConfig, "Assets/Resources/PXR_ProjectSetting.asset");
                }
                else
                {
                    UnityEditor.AssetDatabase.CreateAsset(projectConfig, "Assets/Resources/PXR_ProjectSetting.asset");
                }
            }
#endif
            return projectConfig;
        }

#if UNITY_EDITOR
        public static void SaveAssets()
        {
            EditorUtility.SetDirty(GetProjectConfig());
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}