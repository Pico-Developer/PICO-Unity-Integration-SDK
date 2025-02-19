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
using UnityEditor;
using UnityEngine;

namespace Unity.XR.PXR
{
    [System.Serializable]
    public class PXR_SpatialMeshColorSetting : ScriptableObject
    {
        public List<Color> colorLists = new List<Color>();

        public static PXR_SpatialMeshColorSetting GetSpatialMeshColorSetting()
        {
            PXR_SpatialMeshColorSetting colorSetting = Resources.Load<PXR_SpatialMeshColorSetting>("PXR_SpatialMeshColorSetting");
#if UNITY_EDITOR
            if (colorSetting == null)
            {
                colorSetting = CreateInstance<PXR_SpatialMeshColorSetting>();
                colorSetting.colorLists = new List<Color>
                {
                    //Unknown
                    Color.white,
                    //Floor
                    Color.grey,
                    //Ceiling
                    Color.grey,
                    //Wall
                    Color.blue,
                    //Door
                    Color.cyan,
                    //Window
                    Color.magenta,
                    //Opening
                    Color.yellow,
                    //Table
                    Color.red,
                    //Sofa
                    Color.green,
                    //Chair
                    new Color(0.5f, 0f, 0f),

                    //Human
                    new Color(0f, 0.5f, 0f),
                    //Curtain
                    new Color(0f, 0f, 0.5f),
                    //Cabinet
                    new Color(1f, 0.5f, 0f),
                    //Bed
                    new Color(1f, 0.75f, 0.8f),
                    //Plant
                    new Color(0.5f, 0f, 0.5f),
                    //Screen
                    new Color(0.5f, 0.25f, 0f),
                    //VirtualWall
                    Color.white,
                    //Refrigerator
                    new Color(0.5f, 0.5f, 0f),
                    //WashingMachine
                    new Color(1f, 0.84f, 0f),
                    //AirConditioner
                    new Color(0.75f, 0.75f, 0.75f),
                    //Lamp
                    new Color(0.5f, 1f, 0.5f),
                    //WallArt
                    new Color(0.5f, 0f, 0.25f),

                };
                string path = Application.dataPath + "/Resources";
                if (!Directory.Exists(path))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                    AssetDatabase.CreateAsset(colorSetting, "Assets/Resources/PXR_SpatialMeshColorSetting.asset");
                }
                else
                {
                    AssetDatabase.CreateAsset(colorSetting, "Assets/Resources/PXR_SpatialMeshColorSetting.asset");
                }
            }


#endif
            return colorSetting;
        }
#if UNITY_EDITOR
        public static void SaveAssets()
        {
            EditorUtility.SetDirty(GetSpatialMeshColorSetting());
            AssetDatabase.SaveAssets();
        }
#endif
    }
}

