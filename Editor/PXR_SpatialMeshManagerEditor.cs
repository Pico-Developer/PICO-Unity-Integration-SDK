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
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.XR.PXR.Editor
{
    [CustomEditor(typeof(PXR_SpatialMeshManager))]
    public class PXR_SpatialMeshManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PXR_SpatialMeshColorSetting colorSetting = PXR_SpatialMeshColorSetting.GetSpatialMeshColorSetting();

            EditorGUILayout.BeginVertical("framebox");
            GUILayout.Label("Custom Mesh Color", EditorStyles.boldLabel);
            GUILayout.Space(5);

            if (colorSetting.colorLists != null)
            {
                var labels = Enum.GetNames(typeof(PxrSemanticLabel));
                for (int i = 0; i < colorSetting.colorLists.Count; i++)
                {
                    colorSetting.colorLists[i] = EditorGUILayout.ColorField(labels[i], colorSetting.colorLists[i]);
                }
            }
            
            EditorGUILayout.EndVertical();
            if (GUI.changed)
            {
                PXR_SpatialMeshColorSetting.SaveAssets();
            }
        }

    }
}

