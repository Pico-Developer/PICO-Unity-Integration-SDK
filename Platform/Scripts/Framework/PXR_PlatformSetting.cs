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
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Unity.XR.PXR
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public sealed class PXR_PlatformSetting : ScriptableObject
    {
        public enum simulationType
        {
            Null,
            Invalid,
            Valid
        }

        [SerializeField] public bool entitlementCheckSimulation;
        [SerializeField] public bool startTimeEntitlementCheck;
        [SerializeField] public string appID;
        [SerializeField] public bool useHighlight = true;
        
        public List<string> deviceSN = new List<string>();

        private static PXR_PlatformSetting instance;

        public static PXR_PlatformSetting Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<PXR_PlatformSetting>("PXR_PlatformSetting");
#if UNITY_EDITOR
                    string path = Application.dataPath + "/Resources";
                    if (!Directory.Exists(path))
                    {
                        AssetDatabase.CreateFolder("Assets", "Resources");
                        if (instance == null)
                        {
                            instance = CreateInstance<PXR_PlatformSetting>();
                            AssetDatabase.CreateAsset(instance, "Assets/Resources/PXR_PlatformSetting.asset");
                        }
                    }
                    else
                    {
                        if (instance == null)
                        {
                            instance = CreateInstance<PXR_PlatformSetting>();
                            AssetDatabase.CreateAsset(instance, "Assets/Resources/PXR_PlatformSetting.asset");
                        }
                    }

#endif
                }

                return instance;
            }

            set { instance = value; }
        }
    }
}