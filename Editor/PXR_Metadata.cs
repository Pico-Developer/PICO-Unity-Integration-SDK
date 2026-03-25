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

#if XR_MGMT_GTE_320

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;

namespace Unity.XR.PXR.Editor
{
    internal class PXR_Metadata : IXRPackage
    {
        private class PXR_PackageMetadata : IXRPackageMetadata
        {
            public string packageName => "PICO Plugin";
            public string packageId => "com.unity.xr.picoxr";
            public string settingsType => "Unity.XR.PXR.PXR_Settings";
            public List<IXRLoaderMetadata> loaderMetadata => lLoaderMetadata;

            private static readonly List<IXRLoaderMetadata> lLoaderMetadata = new List<IXRLoaderMetadata>() { new PXR_LoaderMetadata() };
        }

        private class PXR_LoaderMetadata : IXRLoaderMetadata
        {
            public string loaderName => "PICO";
            public string loaderType => "Unity.XR.PXR.PXR_Loader";
            public List<BuildTargetGroup> supportedBuildTargets => SupportedBuildTargets;

            private static readonly List<BuildTargetGroup> SupportedBuildTargets = new List<BuildTargetGroup>()
            {
                BuildTargetGroup.Android
            };
        }

        private static IXRPackageMetadata Metadata = new PXR_PackageMetadata();
        public IXRPackageMetadata metadata => Metadata;

        public bool PopulateNewSettingsInstance(ScriptableObject obj)
        {
            var settings = obj as PXR_Settings;
            if (settings != null)
            {
                settings.stereoRenderingModeAndroid = PXR_Settings.StereoRenderingModeAndroid.MultiPass;

                return true;
            }
            return false;
        }
    }
}

#endif
