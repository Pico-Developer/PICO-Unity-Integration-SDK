#if PICO_OPENXR_SDK
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
// using Unity.XR.CoreUtils;
using Unity.XR.PXR;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;
#endif


namespace Unity.XR.OpenXR.Features.PICOSupport
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "PICO Spatial Mesh",
        Hidden = false,
        BuildTargetGroups = new[] { UnityEditor.BuildTargetGroup.Android },
        Company = "PICO",
        OpenxrExtensionStrings = extensionString,
        Version = "1.0.0",
        FeatureId = featureId)]
#endif
    public class PICOSpatialMesh: OpenXRFeature
    {
        public const string featureId = "com.pico.openxr.feature.spatialmesh";
        public const string extensionString = "XR_PICO_spatial_mesh XR_PICO_spatial_sensing XR_EXT_future";
        private static List<XRMeshSubsystemDescriptor> meshSubsystemDescriptors = new List<XRMeshSubsystemDescriptor>();
   
        public PxrMeshLod LOD;

        private XRMeshSubsystem subsystem;
        public static bool isEnable => OpenXRRuntime.IsExtensionEnabled("XR_PICO_spatial_mesh");
        protected override void OnSubsystemCreate()
        {
            base.OnSubsystemCreate();
            PXR_Plugin.Pxr_SetMeshLOD(Convert.ToUInt16(LOD));
           
        }

        protected override void OnSessionCreate(ulong xrSession)
        {
            base.OnSessionCreate(xrSession);
            CreateSubsystem<XRMeshSubsystemDescriptor, XRMeshSubsystem>(meshSubsystemDescriptors, "PICO Mesh");
        }
        
        protected override void OnSubsystemStop()
        {
            base.OnSubsystemStop();
            StopSubsystem<XRMeshSubsystem>();
           
        }

        protected override void OnSubsystemDestroy()
        {
            base.OnSubsystemDestroy();
            PXR_Plugin.MixedReality.UPxr_DisposeMesh();
            DestroySubsystem<XRMeshSubsystem>();
        }
     
      
    }
}
#endif