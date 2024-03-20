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
using Unity.XR.PXR;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.XR.Management;

namespace Pico.Platform.Editor
{
    public class PicoGs
    {
        public static string appId
        {
            get { return PXR_PlatformSetting.Instance.appID; }
            set
            {
                PXR_PlatformSetting.Instance.appID = value;
                EditorUtility.SetDirty(PXR_PlatformSetting.Instance);
            }
        }
        
        public static bool useHighlight
        {
            get { return PXR_PlatformSetting.Instance.useHighlight; }
            set
            {
                PXR_PlatformSetting.Instance.useHighlight = value;
                EditorUtility.SetDirty(PXR_PlatformSetting.Instance);
            }
        }

        public static bool enableEntitlementCheck
        {
            get { return PXR_PlatformSetting.Instance.entitlementCheckSimulation; }
            set
            {
                PXR_PlatformSetting.Instance.entitlementCheckSimulation = value;
                EditorUtility.SetDirty(PXR_PlatformSetting.Instance);
            }
        }

        public static List<string> entitlementCheckDeviceList
        {
            get { return PXR_PlatformSetting.Instance.deviceSN; }
            set { PXR_PlatformSetting.Instance.deviceSN = value; }
        }

#if USING_XR_SDK_PICO
        static XRManagerSettings GetXrSettings()
        {
            XRGeneralSettings generalSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
            if (generalSettings == null) return null;
            var assignedSettings = generalSettings.AssignedSettings;
            return assignedSettings;
        }

        static PXR_Loader GetPxrLoader()
        {
            var x = GetXrSettings();
            if (x == null) return null;
            foreach (var i in x.activeLoaders)
            {
                if (i is PXR_Loader)
                {
                    return i as PXR_Loader;
                }
            }

            return null;
        }

        public static bool UsePicoXr
        {
            get { return GetPxrLoader() != null; }
            set
            {
                var x = GetXrSettings();
                if (x == null) return;
                var loader = GetPxrLoader();
                if (value == false)
                {
                    if (loader == null)
                    {
                    }
                    else
                    {
                        x.TryRemoveLoader(loader);
                    }
                }
                else
                {
                    if (loader == null)
                    {
                        var res = XRPackageMetadataStore.AssignLoader(x, nameof(PXR_Loader), BuildTargetGroup.Android);
                        Debug.Log($"设置XR{res} {value}");
                    }
                    else
                    {
                    }
                }
            }
        }
#endif
    }
}