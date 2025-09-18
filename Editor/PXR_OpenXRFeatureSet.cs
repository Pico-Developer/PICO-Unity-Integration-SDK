#if PICO_OPENXR_SDK
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;

namespace Unity.XR.OpenXR.Features.PICOSupport
{
    [OpenXRFeatureSet(
        FeatureIds = new string[] {
            LayerSecureContentFeature.featureId,
            DisplayRefreshRateFeature.featureId,
            PassthroughFeature.featureId,
            FoveationFeature.featureId,
            BodyTrackingFeature.featureId,
            PICOSceneCapture.featureId,
            PICOSpatialMesh.featureId,
            PICOSpatialAnchor.featureId,
            PICOFeature.featureId,
            OpenXRExtensions.featureId,
            PICO4ControllerProfile.featureId,
            PICO4UltraControllerProfile.featureId,
            PICONeo3ControllerProfile.featureId,
            PICOG3ControllerProfile.featureId,
        },
        DefaultFeatureIds = new string[] {
            LayerSecureContentFeature.featureId,
            DisplayRefreshRateFeature.featureId,
            PassthroughFeature.featureId,
            FoveationFeature.featureId,
            BodyTrackingFeature.featureId,
            PICOSceneCapture.featureId,
            PICOSpatialMesh.featureId,
            PICOSpatialAnchor.featureId,
            PICOFeature.featureId,
            OpenXRExtensions.featureId,
            PICO4ControllerProfile.featureId,
            PICO4UltraControllerProfile.featureId,
            PICONeo3ControllerProfile.featureId,
            PICOG3ControllerProfile.featureId,
        },
        UiName = "PICO XR",
        Description = "Feature set for using PICO XR Features",
        FeatureSetId = featureSetId,
        SupportedBuildTargets = new BuildTargetGroup[] { BuildTargetGroup.Android},
        RequiredFeatureIds = new string[]
        {
            PICOFeature.featureId,
            OpenXRExtensions.featureId,
            PICO4ControllerProfile.featureId,
            PICO4UltraControllerProfile.featureId,
            PICONeo3ControllerProfile.featureId,
            PICOG3ControllerProfile.featureId,
        }
    )]
    public class PXR_OpenXRFeatureSet
    {
        public const string featureSetId = "com.picoxr.openxr.features";
    }
}
#endif