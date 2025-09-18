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
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
namespace Unity.XR.PXR.Debugger
{
// Generate a setting item in the editor project settings screen
    static class SettingToolEditor
    {
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            var config = PXR_PicoDebuggerSO.Instance;

            var provider = new SettingsProvider("Project/PICO Debugger", SettingsScope.Project)
            {
                label = "PICO Debugger",
                activateHandler = (obj, rootElement) =>
                {
                    var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{PXR_Utils.sdkPackageName}Assets/Debugger/UI/PICODebuggerPanel.uxml");
                    var rootVisualElement = visualTree.Instantiate();
                    rootElement.Add(rootVisualElement);

                    var isOpenToggle = rootVisualElement.Q<Toggle>("IsOpen");
                    var debuggerlaucherButtonDropdown = rootVisualElement.Q<DropdownField>("DebuggerLaucherButton");
                    var startPositionDropdown = rootVisualElement.Q<DropdownField>("StartPosition");
                    var maxInfoCountSlider = rootVisualElement.Q<Slider>("MaxInfoCount");
                    var rulerClearButtonDropdown = rootVisualElement.Q<DropdownField>("RulerClearButton");

                    isOpenToggle.value = config.isOpen;
                    isOpenToggle.RegisterValueChangedCallback(evt =>
                    {
                        config.isOpen = evt.newValue;
                        EditorUtility.SetDirty(config); // Mark as dirty to save the changes
                        if(config.isOpen){
                            PXR_AppLog.PXR_OnEvent($"{PXR_AppLog.strPICODebugger}", PXR_AppLog.strPICODebugger_Enable,"enable");
                        }
                    });

                    debuggerlaucherButtonDropdown.choices = Enum.GetNames(typeof(LauncherButton)).ToList();
                    debuggerlaucherButtonDropdown.choices = Enum.GetNames(typeof(LauncherButton)).Where(name => name != config.rulerClearButton.ToString()).ToList();
                    debuggerlaucherButtonDropdown.index = (int)config.debuggerLauncherButton;
                    debuggerlaucherButtonDropdown.RegisterValueChangedCallback(evt =>
                    {
                        config.debuggerLauncherButton = (LauncherButton)Enum.Parse(typeof(LauncherButton), evt.newValue);
                        rulerClearButtonDropdown.choices = Enum.GetNames(typeof(LauncherButton)).Where(name => name != config.debuggerLauncherButton.ToString()).ToList();
                        EditorUtility.SetDirty(config); 
                        PXR_AppLog.PXR_OnEvent($"{PXR_AppLog.strPICODebugger}", PXR_AppLog.strPICODebugger_LauncherButton,$"{config.debuggerLauncherButton}");
                    });
                    
                    
                    startPositionDropdown.choices = Enum.GetNames(typeof(StartPosiion)).ToList();
                    startPositionDropdown.index = (int)config.startPosition;
                    startPositionDropdown.RegisterValueChangedCallback(evt =>
                    {
                        config.startPosition = (StartPosiion)Enum.Parse(typeof(StartPosiion), evt.newValue);
                        EditorUtility.SetDirty(config); 
                        PXR_AppLog.PXR_OnEvent($"{PXR_AppLog.strPICODebugger}",$"{PXR_AppLog.strPICODebugger_InitialPosition}",$"{config.startPosition}");
                    });

                    // var isFollowingToggle = rootVisualElement.Q<Toggle>("isFollowing");
                    // var isFollowingProperty = settings.FindProperty("isFollowing");

                    // var isLookAtCameraToggle = rootVisualElement.Q<Toggle>("isLookAtCamera");
                    // var isLookAtCameraProperty = settings.FindProperty("isLookAtCamera");

                    
                    maxInfoCountSlider.value = config.maxInfoCount;
                    maxInfoCountSlider.RegisterValueChangedCallback(evt =>
                    {
                        config.maxInfoCount = Mathf.RoundToInt(evt.newValue);
                        EditorUtility.SetDirty(config);
                        PXR_AppLog.PXR_OnEvent($"{PXR_AppLog.strPICODebugger}",$"{PXR_AppLog.strPICODebugger_MaxLogCount}", $"{config.maxInfoCount}");
                    });

                    
                    rulerClearButtonDropdown.choices = Enum.GetNames(typeof(LauncherButton)).Where(name => name != config.debuggerLauncherButton.ToString()).ToList();
                    rulerClearButtonDropdown.index = (int)config.rulerClearButton;
                    rulerClearButtonDropdown.RegisterValueChangedCallback(evt =>
                    {
                        config.rulerClearButton = (LauncherButton)Enum.Parse(typeof(LauncherButton), evt.newValue);
                        debuggerlaucherButtonDropdown.choices = Enum.GetNames(typeof(LauncherButton)).Where(name => name != config.rulerClearButton.ToString()).ToList();
                        EditorUtility.SetDirty(config); 
                        PXR_AppLog.PXR_OnEvent($"{PXR_AppLog.strPICODebugger}",$"{PXR_AppLog.strPICODebugger_RulerResetButton}", $"{config.rulerClearButton}");
                    });
                    
                    AssetDatabase.Refresh();
                },
                keywords = new HashSet<string>(new[] { "PICO", "Debugger Tool" })
            };
            return provider;
        }
    }
}
#endif