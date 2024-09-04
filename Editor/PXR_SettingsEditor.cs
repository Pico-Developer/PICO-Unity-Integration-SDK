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

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.SceneManagement;
#if AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
#endif

namespace Unity.XR.PXR.Editor
{
    [CustomEditor(typeof(PXR_Settings))]
    public class PXR_SettingsEditor : UnityEditor.Editor
    {
        private const string StereoRenderingModeAndroid = "stereoRenderingModeAndroid";
        private const string SystemDisplayFrequency = "systemDisplayFrequency";
        private const string OptimizeBufferDiscards = "optimizeBufferDiscards";
        private const string SystemSplashScreen = "systemSplashScreen";

        static GUIContent guiStereoRenderingMode = EditorGUIUtility.TrTextContent("Stereo Rendering Mode");
        static GUIContent guiDisplayFrequency = EditorGUIUtility.TrTextContent("Display Refresh Rates");
        private static GUIContent guiOptimizeBuffer = EditorGUIUtility.TrTextContent("Optimize Buffer Discards(Vulkan)");
        static GUIContent guiSystemSplashScreen = EditorGUIUtility.TrTextContent("System Splash Screen");

        private SerializedProperty stereoRenderingModeAndroid;
        private SerializedProperty systemDisplayFrequency;
        private SerializedProperty optimizeBufferDiscards;
        private SerializedProperty systemSplashScreen;

        void OnEnable()
        {
            if (stereoRenderingModeAndroid == null)
                stereoRenderingModeAndroid = serializedObject.FindProperty(StereoRenderingModeAndroid);
            if (systemDisplayFrequency == null)
                systemDisplayFrequency = serializedObject.FindProperty(SystemDisplayFrequency);
            if (optimizeBufferDiscards == null)
                optimizeBufferDiscards = serializedObject.FindProperty(OptimizeBufferDiscards);
            if (systemSplashScreen == null)
                systemSplashScreen = serializedObject.FindProperty(SystemSplashScreen);
        }

        public override void OnInspectorGUI()
        {
            if (serializedObject == null || serializedObject.targetObject == null)
                return;

            serializedObject.Update();
            EditorGUIUtility.labelWidth = 200.0f;
            BuildTargetGroup selectedBuildTargetGroup = EditorGUILayout.BeginBuildTargetSelectionGrouping();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorGUILayout.HelpBox("PICO settings cannot be changed when the editor is in play mode.", MessageType.Info);
                EditorGUILayout.Space();
            }
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            if (selectedBuildTargetGroup == BuildTargetGroup.Android)
            {
                EditorGUILayout.PropertyField(stereoRenderingModeAndroid, guiStereoRenderingMode);
                EditorGUILayout.PropertyField(systemDisplayFrequency, guiDisplayFrequency);
                EditorGUILayout.PropertyField(optimizeBufferDiscards, guiOptimizeBuffer);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("enableAppSpaceWarp"), new GUIContent("Application SpaceWarp"));
                EditorGUILayout.PropertyField(systemSplashScreen, guiSystemSplashScreen);

#if AR_FOUNDATION
                PXR_ProjectSetting projectConfig = PXR_ProjectSetting.GetProjectConfig();
                var guiContent = new GUIContent();
                guiContent.text = "AR Foundation";
                projectConfig.arFoundation = EditorGUILayout.Toggle(guiContent, projectConfig.arFoundation);
                if (projectConfig.arFoundation)
                {
                    EditorGUI.indentLevel++;
                    // body tracking
                    guiContent.text = "Body Tracking";
                    projectConfig.bodyTracking = EditorGUILayout.Toggle(guiContent, projectConfig.bodyTracking);

                    // face tracking
                    guiContent.text = "Face Tracking";
                    projectConfig.faceTracking = EditorGUILayout.Toggle(guiContent, projectConfig.faceTracking);

                    // anchor
                    guiContent.text = "Anchor";
                    projectConfig.spatialAnchor = EditorGUILayout.Toggle(guiContent, projectConfig.spatialAnchor);

                    // anchor
                    guiContent.text = "Meshing";
                    projectConfig.spatialMesh = EditorGUILayout.Toggle(guiContent, projectConfig.spatialMesh);

                    List<ARCameraManager> components = FindComponentsInScene<ARCameraManager>().Where(component => (component.enabled && component.gameObject.CompareTag("MainCamera"))).ToList();
                    bool cameraEffect = false;
                    for (int i = 0; i < components.Count; i++)
                    {
                        ARCameraManager aRCamera = components[i];
                        if (aRCamera.gameObject.GetComponent<PXR_ARCameraEffectManager>())
                        {
                            cameraEffect = true;
                        }
                        Camera camera = aRCamera.gameObject.GetComponent<Camera>();
                        if (camera)
                        {
                            camera.clearFlags = CameraClearFlags.SolidColor;
                            camera.backgroundColor = new Color(0, 0, 0, 0);
                        }
                    }

                    if (!cameraEffect && components.Count > 0)
                    {
                        ARCameraManager aRCamera = components[0];
                        if (!aRCamera.gameObject.GetComponent<PXR_ARCameraEffectManager>())
                        {
                            aRCamera.gameObject.AddComponent<PXR_ARCameraEffectManager>();
                        }
                        cameraEffect = true;
                    }

                    EditorGUI.indentLevel--;
                }

                if (GUI.changed)
                {
                    EditorUtility.SetDirty(projectConfig);
                }
#endif
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndBuildTargetSelectionGrouping();

            serializedObject.ApplyModifiedProperties();
            EditorGUIUtility.labelWidth = 0f;
        }

        public static List<T> FindComponentsInScene<T>() where T : Component
        {
            var activeScene = SceneManager.GetActiveScene();
            var foundComponents = new List<T>();

            var rootObjects = activeScene.GetRootGameObjects();
            foreach (var rootObject in rootObjects)
            {
                var components = rootObject.GetComponentsInChildren<T>(true);
                foundComponents.AddRange(components);
            }

            return foundComponents;
        }
    }
}
