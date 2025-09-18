#if PICO_OPENXR_SDK
using System;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

namespace Unity.XR.OpenXR.Features.PICOSupport
{
    public abstract class OpenXRFeatureBase : OpenXRFeature
    {
        protected static ulong xrInstance = 0ul;
        protected static ulong xrSession = 0ul;
        protected override bool OnInstanceCreate(ulong instance)
        {
            xrInstance = instance;
            xrSession = 0ul;
            InstanceCreate(instance);
            return true;
        }
        protected override void OnSessionCreate(ulong xrSessionId)
        {
            xrSession = xrSessionId;
            base.OnSessionCreate(xrSessionId);
            SessionCreate(xrSessionId);
        }
        public bool isExtensionEnabled(string extensionUrl)
        {
            string[] exts = extensionUrl.Split(' ');
            if (exts.Length > 0)
            {
                foreach (var _ext in exts)
                {
                    if (!string.IsNullOrEmpty(_ext) && !OpenXRRuntime.IsExtensionEnabled(_ext))
                    {
                        PLog.e("OpenXRFeatureBase", _ext + " is not enabled");
                        return false;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(extensionUrl) && !OpenXRRuntime.IsExtensionEnabled(extensionUrl))
                {
                    PLog.e("OpenXRFeatureBase", extensionUrl + " is not enabled");
                    return false;
                }
            }
            return true;
        }

        public virtual void InstanceCreate(ulong instance) {}

        public virtual void SessionCreate(ulong xrSessionId) {}
        public abstract string GetExtensionString();
#if UNITY_EDITOR
        protected override void GetValidationChecks(List<ValidationRule> rules, BuildTargetGroup targetGroup)
        {
            var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
            rules.Add(new ValidationRule(this)
            {
                message = "No PICO OpenXR Features selected.",
                checkPredicate = () =>
                {
                    if (null == settings)
                        return false;
                    
                    foreach (var feature in settings.GetFeatures<OpenXRFeature>())
                    {
                        if (feature is OpenXRExtensions)
                        {
                            return feature.enabled;
                        }
                    }

                    return false;
                },
                fixIt = () =>
                {
                    if (null == settings)
                        return ;
                    var openXRExtensions = settings.GetFeature<OpenXRExtensions>();
                    if (openXRExtensions != null)
                    {
                        openXRExtensions.enabled = true;
                    }
                },
                error = true
            });
        }
#endif
    }
}
#endif