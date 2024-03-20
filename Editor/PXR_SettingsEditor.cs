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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.XR.PXR;

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
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndBuildTargetSelectionGrouping();

            serializedObject.ApplyModifiedProperties();
            EditorGUIUtility.labelWidth = 0f;
        }
    }
}
