#if PICO_OPENXR_SDK
using System.Collections.Generic;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using Object = System.Object;
using UnityEngine.XR.OpenXR.Features.Interactions;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;
using Unity.XR.PXR;


#if UNITY_EDITOR
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

#if AR_FOUNDATION_5||AR_FOUNDATION_6
using UnityEngine.XR.ARSubsystems;
#endif
namespace Unity.XR.OpenXR.Features.PICOSupport
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "PICO XR Support",
        Desc = "Necessary to deploy an PICO compatible app.",
        Company = "PICO",
        Version = PXR_Constants.SDKVersion,
        BuildTargetGroups = new[] { BuildTargetGroup.Android },
        CustomRuntimeLoaderBuildTargets = new[] { BuildTarget.Android },
        OpenxrExtensionStrings = OpenXrExtensionList,
        FeatureId = featureId
    )]
#endif
    public class PICOFeature : OpenXRFeature
    {
        /// <summary>
        /// The feature id string. This is used to give the feature a well known id for reference.
        /// </summary>
        public const string featureId = "com.unity.openxr.feature.pico";
        public const string OpenXrExtensionList = "XR_PICO_controller_interaction";
        public bool isPicoSupport = false;
        public static Action<bool> onAppFocusedAction;
        
        protected override void OnSessionStateChange(int oldState, int newState)
        {
            Debug.Log($"[PICOOpenXRExtensions] OnSessionStateChange: {oldState} -> {newState}");
            if (onAppFocusedAction != null)
            {
                onAppFocusedAction(newState == 5);
            }
        }
#if UNITY_EDITOR
    
        protected override void GetValidationChecks(List<ValidationRule> rules, BuildTargetGroup targetGroup)
        {
            OpenXRSettings settings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);


            var AdditionalRules = new ValidationRule[]
            {
                new ValidationRule(this)
                {
                    message = "Only the PICO Touch Interaction Profile is supported right now.",
                    checkPredicate = () =>
                    {
                        if (null == settings)
                            return false;

                        bool touchFeatureEnabled = false;
                        bool otherInteractionFeatureEnabled = false;

                        foreach (var feature in settings.GetFeatures<OpenXRInteractionFeature>())
                        {
                            if (feature.enabled)
                            {
                                if ((feature is PICONeo3ControllerProfile) ||
                                    (feature is PICO4UltraControllerProfile) || (feature is PICO4ControllerProfile) ||
                                    (feature is EyeGazeInteraction) || (feature is HandInteractionProfile) ||
                                    (feature is PalmPoseInteraction) || (feature is PICOG3ControllerProfile))
                                    touchFeatureEnabled = true;
                                else
                                    otherInteractionFeatureEnabled = true;
                            }
                        }

                        return touchFeatureEnabled && !otherInteractionFeatureEnabled;
                    },
                    fixIt = () =>
                    {
                        if (null == settings)
                            return;

                        foreach (var feature in settings.GetFeatures<OpenXRInteractionFeature>())
                        {
                            feature.enabled = ((feature is PICO4UltraControllerProfile) || (feature is PICO4ControllerProfile));
                        }
                    },
                    error = true,
                }
            };

            rules.AddRange(AdditionalRules);
        }
        
        internal class PICOFeatureEditorWindow : EditorWindow
        {
            private Object feature;
            private Editor featureEditor;

            public static EditorWindow Create(Object feature)
            {
                var window = EditorWindow.GetWindow<PICOFeatureEditorWindow>(true, "PICO Feature Configuration", true);
                window.feature = feature;
                window.featureEditor = Editor.CreateEditor((UnityEngine.Object)feature);
                return window;
            }

            private void OnGUI()
            {
                featureEditor.OnInspectorGUI();
            }
        }
#endif
    }
}
#endif