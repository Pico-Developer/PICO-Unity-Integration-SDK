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
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Unity.XR.PXR.Debugger
{
    [InitializeOnLoad]
    public class PXR_PicoDebuggerSetup
    {
        static PXR_PicoDebuggerSetup()
        {
            EditorApplication.update += Init_PXR_PicoDebuggerSetup;
        }
        static void Init_PXR_PicoDebuggerSetup()
        {
            string currentPanelPath = $"{PXR_DebuggerConst.sdkPackageName}Assets/Debugger/Prefabs/DebuggerPanel.prefab";
            string targetPanelPath = "Assets/Resources/DebuggerPanel.prefab";
            string currentEntryPath = $"{PXR_DebuggerConst.sdkPackageName}Assets/Debugger/Prefabs/PICODebugger.prefab";
            string targetEntryPath = "Assets/Resources/PICODebugger.prefab";
            if(!File.Exists(targetEntryPath)){
                if (!Directory.Exists("Assets/Resources"))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }
                
                if (AssetDatabase.LoadAssetAtPath<GameObject>(currentEntryPath) == null)
                {
                    Debug.LogError("Prefab not found at path: " + currentEntryPath);
                }
                
                AssetDatabase.CopyAsset(currentEntryPath, targetEntryPath);
                AssetDatabase.SaveAssets();
            }

            if(!File.Exists(targetPanelPath)){
                if (!Directory.Exists("Assets/Resources"))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }
                
                if (AssetDatabase.LoadAssetAtPath<GameObject>(currentPanelPath) == null)
                {
                    Debug.LogError("Prefab not found at path: " + currentPanelPath);
                }
                
                AssetDatabase.CopyAsset(currentPanelPath, targetPanelPath);
                AssetDatabase.SaveAssets();
            }
            // EditorApplication.update -= Init_PXR_PicoDebuggerSetup;
        }
    }
}
#endif