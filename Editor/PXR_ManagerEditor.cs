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
using System.IO;
using System.Linq;
using System.Xml;
using Unity.XR.PXR;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unity.XR.PXR.Editor
{
    [CustomEditor(typeof(PXR_Manager))]
    public class PXR_ManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.changed = false;
            DrawDefaultInspector();

            PXR_Manager manager = (PXR_Manager)target;
            PXR_ProjectSetting projectConfig = PXR_ProjectSetting.GetProjectConfig();

            //Screen Fade
            manager.screenFade = EditorGUILayout.Toggle("Open Screen Fade", manager.screenFade);
            if (Camera.main != null)
            {
                var head = Camera.main.transform;
                if (head)
                {
                    var fade = head.GetComponent<PXR_ScreenFade>();
                    if (manager.screenFade)
                    {
                        if (!fade)
                        {
                            head.gameObject.AddComponent<PXR_ScreenFade>();
                            Selection.activeObject = head;
                        }
                    }
                    else
                    {
                        if (fade) DestroyImmediate(fade);
                    }
                }
            }

            //ffr
            manager.foveatedRenderingMode = (FoveatedRenderingMode)EditorGUILayout.EnumPopup("Foveated Rendering Mode", manager.foveatedRenderingMode);
            if (FoveatedRenderingMode.FixedFoveatedRendering == manager.foveatedRenderingMode)
            {
                projectConfig.enableETFR = false;
                manager.foveationLevel = (FoveationLevel)EditorGUILayout.EnumPopup("Foveated Rendering Level", manager.foveationLevel);
                if (FoveationLevel.None != manager.foveationLevel)
                {
                    projectConfig.enableSubsampled = EditorGUILayout.Toggle("  Subsampling", projectConfig.enableSubsampled);
                }
            }
            else if (FoveatedRenderingMode.EyeTrackedFoveatedRendering == manager.foveatedRenderingMode) //etfr
            {
                projectConfig.enableETFR = true;
                manager.eyeFoveationLevel = (FoveationLevel)EditorGUILayout.EnumPopup("Foveated Rendering Level", manager.eyeFoveationLevel);
                if (FoveationLevel.None != manager.eyeFoveationLevel)
                {
                    projectConfig.enableSubsampled = EditorGUILayout.Toggle("  Subsampling", projectConfig.enableSubsampled);
                }
            }

            //eye tracking
            GUIStyle firstLevelStyle = new GUIStyle(GUI.skin.label);
            firstLevelStyle.alignment = TextAnchor.UpperLeft;
            firstLevelStyle.fontStyle = FontStyle.Bold;
            firstLevelStyle.fontSize = 12;
            firstLevelStyle.wordWrap = true;
            var guiContent = new GUIContent();
            guiContent.text = "Eye Tracking";
            guiContent.tooltip = "Before calling EyeTracking API, enable this option first, only for Neo3 Pro Eye , PICO 4 Pro device.";
            projectConfig.eyeTracking = EditorGUILayout.Toggle(guiContent, projectConfig.eyeTracking);
            manager.eyeTracking = projectConfig.eyeTracking;
            if (manager.eyeTracking || FoveatedRenderingMode.EyeTrackedFoveatedRendering == manager.foveatedRenderingMode)
            {
                projectConfig.eyetrackingCalibration = EditorGUILayout.Toggle(new GUIContent("Eye Tracking Calibration"), projectConfig.eyetrackingCalibration);
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Note:", firstLevelStyle);
                EditorGUILayout.LabelField("Eye Tracking is supported only on Neo 3 Pro Eye , PICO 4 Pro");
                EditorGUILayout.EndVertical();
            }

            //face tracking
            var FaceContent = new GUIContent();
            FaceContent.text = "Face Tracking Mode";
            manager.trackingMode = (FaceTrackingMode)EditorGUILayout.EnumPopup(FaceContent, manager.trackingMode);
            if (manager.trackingMode == FaceTrackingMode.None)
            {
                projectConfig.faceTracking = false;
                projectConfig.lipsyncTracking = false;
            }
            else if (manager.trackingMode == FaceTrackingMode.Hybrid)
            {
                projectConfig.faceTracking = true;
                projectConfig.lipsyncTracking = true;
            }
            else if (manager.trackingMode == FaceTrackingMode.FaceOnly)
            {
                projectConfig.faceTracking = true;
                projectConfig.lipsyncTracking = false;
            }
            else if (manager.trackingMode == FaceTrackingMode.LipsyncOnly)
            {
                projectConfig.faceTracking = false;
                projectConfig.lipsyncTracking = true;
            }
            manager.faceTracking = projectConfig.faceTracking;
            manager.lipsyncTracking = projectConfig.lipsyncTracking;

            //hand tracking
            var handContent = new GUIContent();
            handContent.text = "Hand Tracking";
            projectConfig.handTracking = EditorGUILayout.Toggle(handContent, projectConfig.handTracking);

            //body tracking
            var bodyContent = new GUIContent();
            bodyContent.text = "Body Tracking";
            projectConfig.bodyTracking = EditorGUILayout.Toggle(bodyContent, projectConfig.bodyTracking);
            manager.bodyTracking = projectConfig.bodyTracking;

            // content protect
            projectConfig.useContentProtect = EditorGUILayout.Toggle("Use Content Protect", projectConfig.useContentProtect);

            //MRC
            var mrcContent = new GUIContent();
            mrcContent.text = "MRC";
            projectConfig.openMRC = EditorGUILayout.Toggle(mrcContent, projectConfig.openMRC);
            manager.openMRC = projectConfig.openMRC;
            if (manager.openMRC == true)
            {
                EditorGUILayout.BeginVertical("frameBox");
                string[] layerNames = new string[32];
                for (int i = 0; i < 32; i++)
                {
                    layerNames[i] = LayerMask.LayerToName(i);
                    if (layerNames[i].Length == 0)
                    {
                        layerNames[i] = "LayerName " + i.ToString();
                    }
                }
                manager.foregroundLayerMask = EditorGUILayout.MaskField("foreground Layer Masks", manager.foregroundLayerMask, layerNames);
                manager.backLayerMask = EditorGUILayout.MaskField("back Layer Masks", manager.backLayerMask, layerNames);
                EditorGUILayout.EndVertical();
            }
            //Late Latching
            projectConfig.latelatching = EditorGUILayout.Toggle("Use Late Latching", projectConfig.latelatching);
            manager.lateLatching = projectConfig.latelatching;
            if (manager.lateLatching)
            {
                projectConfig.latelatchingDebug = EditorGUILayout.Toggle("  Late Latching Debug", projectConfig.latelatchingDebug);
                manager.latelatchingDebug = projectConfig.latelatchingDebug;
            }

            if (Camera.main != null)
            {
                var head = Camera.main.transform;
                if (head)
                {
                    var fade = head.GetComponent<PXR_LateLatching>();
                    if (manager.lateLatching)
                    {
                        if (!fade)
                        {
                            head.gameObject.AddComponent<PXR_LateLatching>();
                            Selection.activeObject = head;
                        }
                    }
                    else
                    {
                        if (fade) DestroyImmediate(fade);
                    }
                }
            }

            // msaa
            if (QualitySettings.renderPipeline != null)
            {
                EditorGUI.BeginDisabledGroup(true);
                manager.useRecommendedAntiAliasingLevel = EditorGUILayout.Toggle("Use Recommended MSAA", manager.useRecommendedAntiAliasingLevel);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.HelpBox("A Scriptable Render Pipeline is in use,the 'Use Recommended MSAA' will not be used. ", MessageType.Info, true);
            }
            else
            {
                manager.useRecommendedAntiAliasingLevel = EditorGUILayout.Toggle("Use Recommended MSAA", manager.useRecommendedAntiAliasingLevel);
            }

            //Adaptive Resolution
            guiContent = new GUIContent();
            guiContent.text = "Adaptive Resolution";
            guiContent.tooltip = "Adaptively change resolution based on GPU performance using renderViewportScale. Render buffer will be allocated to max adaptive resolution scale size. Currently, FFR should be disabled with this feature.";
            projectConfig.adaptiveResolution = EditorGUILayout.Toggle(guiContent, projectConfig.adaptiveResolution);
            manager.adaptiveResolution = projectConfig.adaptiveResolution;
            if (manager.adaptiveResolution)
            {
                EditorGUILayout.LabelField("Min Adaptive Resolution Scale:");
                manager.minEyeTextureScale = EditorGUILayout.Slider(manager.minEyeTextureScale, 0.7f, 1.3f);
                EditorGUILayout.LabelField("Max Adaptive Resolution Scale:");
                manager.maxEyeTextureScale = EditorGUILayout.Slider(manager.maxEyeTextureScale, 0.7f, 1.3f);
                manager.adaptiveResolutionPowerSetting = (AdaptiveResolutionPowerSetting)EditorGUILayout.EnumPopup(" Power Setting", manager.adaptiveResolutionPowerSetting);

            }

            projectConfig.stageMode = EditorGUILayout.Toggle("Stage Mode", projectConfig.stageMode);

            //mr
            if (projectConfig.spatialAnchor)
            {
                using (new EditorGUI.DisabledGroupScope(true))
                {
                    projectConfig.videoSeeThrough = true;
                    EditorGUILayout.Toggle("Video Seethrough", projectConfig.videoSeeThrough);
                }
            }
            else
            {
                projectConfig.videoSeeThrough = EditorGUILayout.Toggle("Video Seethrough", projectConfig.videoSeeThrough);
            }
            projectConfig.spatialAnchor = EditorGUILayout.Toggle("Anchor", projectConfig.spatialAnchor);

            //Super Resolution
            var superresolutionContent = new GUIContent();
            superresolutionContent.text = "Super Resolution";
            superresolutionContent.tooltip = "Single pass spatial aware upscaling technique.\n\nThis can't be used with Sharpening. \nAlso can't be used along with subsample feature due to unsupported texture format. \n\nThis effect won't work properly under low resolutions when Adaptive Resolution is also enabled.";
            projectConfig.superResolution = EditorGUILayout.Toggle(superresolutionContent, projectConfig.superResolution);
            manager.enableSuperResolution = projectConfig.superResolution;

            //Sharpening

            var sharpeningContent = new GUIContent();
            sharpeningContent.text = "Sharpening Mode";
            sharpeningContent.tooltip = "Normal: Normal Quality \n\nQuality: Higher Quality, higher GPU usage\n\nThis effect won't work properly under low resolutions when Adaptive Resolution is also enabled.\n\nThis can't be used with Super Resolution. It will be automatically disabled when you enable Super Resolution. \nAlso can't be used along with subsample feature due to unsupported texture format";
            var sharpeningEnhanceContent = new GUIContent();
            sharpeningEnhanceContent.text = "Sharpening Enhance Mode";
            sharpeningEnhanceContent.tooltip = "None: Full screen will be sharpened\n\nFixed Foveated: Only the central fixation point will be sharpened\n\nSelf Adaptive: Only when contrast between the current pixel and the surrounding pixels exceeds a certain threshold will be sharpened.\n\nThis menu will be only enabled while Sharpening (either Normal or Quality) is enabled.";

            if (projectConfig.superResolution)
            {
                GUI.enabled = false;
                manager.sharpeningMode = SharpeningMode.None;
                manager.sharpeningEnhance = SharpeningEnhance.None;
            }
            else 
            {
                GUI.enabled = true;
            }

            manager.sharpeningMode = (SharpeningMode)EditorGUILayout.EnumPopup(sharpeningContent, manager.sharpeningMode);
            if (manager.sharpeningMode == SharpeningMode.None)
            {
                manager.sharpeningEnhance = SharpeningEnhance.None;
            }
            else
            {
                manager.sharpeningEnhance = (SharpeningEnhance)EditorGUILayout.EnumPopup(sharpeningEnhanceContent, manager.sharpeningEnhance);
            }

            if (manager.sharpeningMode != SharpeningMode.None)
            {
                if (manager.sharpeningMode == SharpeningMode.Normal)
                {
                    projectConfig.normalSharpening = true;
                    projectConfig.qualitySharpening = false;
                }
                else
                {
                    projectConfig.normalSharpening = false;
                    projectConfig.qualitySharpening = true;
                }

                if (manager.sharpeningEnhance == SharpeningEnhance.Both)
                {
                    projectConfig.fixedFoveatedSharpening = true;
                    projectConfig.selfAdaptiveSharpening = true;
                }
                else if (manager.sharpeningEnhance == SharpeningEnhance.FixedFoveated)
                {
                    projectConfig.fixedFoveatedSharpening = true;
                    projectConfig.selfAdaptiveSharpening = false;
                }
                else if (manager.sharpeningEnhance == SharpeningEnhance.SelfAdaptive)
                {
                    projectConfig.fixedFoveatedSharpening = false;
                    projectConfig.selfAdaptiveSharpening = true;
                }
                else
                {
                    projectConfig.fixedFoveatedSharpening = false;
                    projectConfig.selfAdaptiveSharpening = false;
                }
            }
            else
            {
                projectConfig.normalSharpening = false;
                projectConfig.qualitySharpening = false;
                projectConfig.fixedFoveatedSharpening = false;
                projectConfig.selfAdaptiveSharpening = false;
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(projectConfig);
                EditorUtility.SetDirty(manager);
            }
            serializedObject.ApplyModifiedProperties();
        }

    }
}