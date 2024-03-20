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

using Unity.XR.PXR;
using UnityEditor;
using UnityEngine;

namespace Pico.Platform.Editor
{
    [CustomEditor(typeof(PXR_PlatformSetting))]
    public class PXR_PlatformSettingEditor : UnityEditor.Editor
    {
        private SerializedProperty deviceSNList;

        private void OnEnable()
        {
            deviceSNList = serializedObject.FindProperty("deviceSN");
        }

        public override void OnInspectorGUI()
        {
            var startEntitleCheckTip = "If selected, you will need to enter the APPID that is obtained from" +
                                       " PICO Developer Platform after uploading the app for an entitlement check upon the app launch.";
            var startEntitleCheckLabel = new GUIContent("User Entitlement Check[?]", startEntitleCheckTip);

            PXR_PlatformSetting.Instance.startTimeEntitlementCheck =
                EditorGUILayout.Toggle(startEntitleCheckLabel, PXR_PlatformSetting.Instance.startTimeEntitlementCheck);
            if (PXR_PlatformSetting.Instance.startTimeEntitlementCheck)
            {
                serializedObject.Update();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("App ID ", GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                PXR_PlatformSetting.Instance.appID =
                    EditorGUILayout.TextField(PXR_PlatformSetting.Instance.appID, GUILayout.Width(350.0f));
                EditorGUILayout.EndHorizontal();

                if (PXR_PlatformSetting.Instance.appID == "")
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(300));
                    EditorGUILayout.HelpBox("APPID is required for Entitlement Check", UnityEditor.MessageType.Error, true);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("The APPID is required to run an Entitlement Check. Create / Find your APPID Here:", GUILayout.Width(500));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUIStyle style = new GUIStyle();
                style.normal.textColor = new Color(0, 122f / 255f, 204f / 255f);
                if (GUILayout.Button("" + "https://developer.pico-interactive.com/developer/overview", style,
                        GUILayout.Width(200)))
                {
                    Application.OpenURL("https://developer.pico-interactive.com/developer/overview");
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("If you do not need user Entitlement Check, please uncheck it.", GUILayout.Width(500));
                EditorGUILayout.EndHorizontal();
                serializedObject.ApplyModifiedProperties();

                var simulationTip = "If true,Development devices will simulate Entitlement Check," +
                                    "you should enter a valid device SN codes list." +
                                    "The SN code can be obtain in Settings-General-Device serial number or input  \"adb devices\" in cmd";
                var simulationLabel = new GUIContent("Entitlement Check Simulation [?]", simulationTip);

                PXR_PlatformSetting.Instance.entitlementCheckSimulation = EditorGUILayout.Toggle(simulationLabel, PXR_PlatformSetting.Instance.entitlementCheckSimulation);
                if (PXR_PlatformSetting.Instance.entitlementCheckSimulation)
                {
                    serializedObject.Update();
                    EditorGUILayout.PropertyField(deviceSNList, true);
                    serializedObject.ApplyModifiedProperties();
                }

                if (GUI.changed)
                {
                    EditorUtility.SetDirty(PXR_PlatformSetting.Instance);
                    GUI.changed = false;
                }
            }
        }
    }
}