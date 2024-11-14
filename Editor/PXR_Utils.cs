using System;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Editor;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.XR.CoreUtils.XROrigin;

namespace Unity.XR.PXR
{
    [InitializeOnLoad]
    internal static class PXR_Utils
    {
        public static string BuildingBlock = "Building Block";
        public static string sdkPackageName = "Packages/com.unity.xr.picoxr/";

        public static string xriPackageName = "com.unity.xr.interaction.toolkit";
        public static string xriVersion = "2.5.4";
        public static PackageVersion xriPackageVersion250 = new PackageVersion("2.5.0");
        public static string xriCategory = "XR Interaction Toolkit";
        public static string xriSamplesPath = "Assets/Samples/XR Interaction Toolkit";
        public static string xriStarterAssetsSampleName = "Starter Assets";
        public static string xriHandsInteractionDemoSampleName = "Hands Interaction Demo";
        public static string XRIDefaultInputActions
        {
            get
            {
                return $"{xriSamplesPath}/{xriVersion}/Starter Assets/XRI Default Input Actions.inputactions";
            }
        }

        public static string XRIDefaultLeftControllerPreset
        {
            get
            {
                var xriPackageVersion = new PackageVersion(xriVersion);
                if (xriPackageVersion >= xriPackageVersion250)
                {
                    return $"{xriSamplesPath}/{xriVersion}/Starter Assets/Presets/XRI Default Left Controller.preset";
                }
                else
                {
                    return $"{xriSamplesPath}/{xriVersion}/Starter Assets/XRI Default Left Controller.preset";
                }
            }
        }

        public static string XRIDefaultRightControllerPreset
        {
            get
            {
                var xriPackageVersion = new PackageVersion(xriVersion);
                if (xriPackageVersion >= xriPackageVersion250)
                {
                    return $"{xriSamplesPath}/{xriVersion}/Starter Assets/Presets/XRI Default Right Controller.preset";
                }
                else
                {
                    return $"{xriSamplesPath}/{xriVersion}/Starter Assets/XRI Default Right Controller.preset";
                }
            }
        }

        public static string XRInteractionHandsSetupPath
        {
            get
            {
                var xriPackageVersion = new PackageVersion(xriVersion);
                if (xriPackageVersion >= xriPackageVersion250)
                {
                    return $"{xriSamplesPath}/{xriVersion}/{xriHandsInteractionDemoSampleName}/Prefabs/XR Interaction Hands Setup.prefab";
                }
                else
                {
                    return $"{xriSamplesPath}/{xriVersion}/{xriHandsInteractionDemoSampleName}/Runtime/Prefabs/XR Interaction Hands Setup.prefab";
                }
            }
        }
        public static string XRInteractionPokeButtonPath
        {
            get
            {
                var xriPackageVersion = new PackageVersion(xriVersion);
                if (xriPackageVersion >= xriPackageVersion250)
                {
                    return $"{xriSamplesPath}/{xriVersion}/{xriHandsInteractionDemoSampleName}/HandsDemoSceneAssets/Prefabs/PokeButton.prefab";
                }
                else
                {
                    return $"{xriSamplesPath}/{xriVersion}/{xriHandsInteractionDemoSampleName}/Runtime/Prefabs/PokeButton.prefab";
                }
            }
        }

        public static string xrHandPackageName = "com.unity.xr.hands";
        public static string xrHandVersion = "1.4.1";
        public static PackageVersion xrHandRecommendedPackageVersion = new PackageVersion("1.3.0");
        public static string xrHandSamplesPath = "Assets/Samples/XR Hands";
        public static string xrHandGesturesSampleName = "Gestures";
        public static string xrHandVisualizerSampleName = "HandVisualizer";

        public static string XRHandLeftHandPrefabPath
        {
            get
            {
                return $"{xrHandSamplesPath}/{xrHandVersion}/HandVisualizer/Prefabs/Left Hand Tracking.prefab";
            }
        }

        public static string XRHandRightHandPrefabPath
        {
            get
            {
                return $"{xrHandSamplesPath}/{xrHandVersion}/HandVisualizer/Prefabs/Right Hand Tracking.prefab";
            }
        }


        static AddRequest xrHandsPackageAddRequest;

        public static List<T> FindComponentsInScene<T>() where T : Component
        {
            var activeScene = SceneManager.GetActiveScene();
            var foundComponents = new List<T>();

            var rootObjects = activeScene.GetRootGameObjects();
            foreach (var rootObject in rootObjects)
            {
                var components = rootObject.GetComponentsInChildren<T>(true);
                foundComponents.AddRange(components);
            }

            return foundComponents;
        }
        public static List<T> FindGameObjectsInScene<T>() where T : Component
        {
            var activeScene = SceneManager.GetActiveScene();
            var foundComponents = new List<T>();

            var rootObjects = activeScene.GetRootGameObjects();
            foreach (var rootObject in rootObjects)
            {
                var components = rootObject.GetComponentsInChildren<T>(true);
                foundComponents.AddRange(components);
            }

            return foundComponents;
        }

        public static void AddNewTag(string newTag)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tags = tagManager.FindProperty("tags");

            bool tagExists = false;
            for (int i = 0; i < tags.arraySize; i++)
            {
                if (tags.GetArrayElementAtIndex(i).stringValue == newTag)
                {
                    tagExists = true;
                    break;
                }
            }

            if (!tagExists)
            {
                tags.InsertArrayElementAtIndex(tags.arraySize);
                tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = newTag;
                tagManager.ApplyModifiedProperties();
                Debug.Log($"Tag '{newTag}' has been added.");
            }
            else
            {
                Debug.LogWarning($"Tag '{newTag}' already exists.");
            }
        }

        public static bool TryFindSample(string packageName, string packageVersion, string sampleDisplayName, out Sample sample)
        {
            sample = default;

            IEnumerable<Sample> packageSamples;
            try
            {
                packageSamples = Sample.FindByPackage(packageName, packageVersion);
            }
            catch (Exception e)
            {
                Debug.LogError($"Couldn't find samples of the {ToString(packageName, packageVersion)} package. Exception: {e}");
                return false;
            }
            if (packageSamples == null)
            {
                Debug.LogWarning($"Couldn't find samples of the {ToString(packageName, packageVersion)} package.");
                return false;
            }

            foreach (var packageSample in packageSamples)
            {
                if (packageSample.displayName == sampleDisplayName)
                {
                    Debug.Log($" TryFindSample   packageSample.displayName={packageSample.displayName}, sampleDisplayName={sampleDisplayName}");
                    sample = packageSample;
                    return true;
                }
            }

            Debug.LogWarning($"Couldn't find {sampleDisplayName} sample in the { packageName}:{ packageVersion}.");
            return false;
        }
        private static string ToString(string packageName, string packageVersion)
        {
            return string.IsNullOrEmpty(packageVersion) ? packageName : $"{packageName}@{packageVersion}";
        }

        public static void SetTrackingOriginMode(TrackingOriginMode trackingOriginMode = TrackingOriginMode.Device)
        {
            List<XROrigin> components = FindComponentsInScene<XROrigin>().Where(component => component.isActiveAndEnabled).ToList();

            foreach (XROrigin origin in components)
            {
                if (TrackingOriginMode.NotSpecified == origin.RequestedTrackingOriginMode)
                {
                    Debug.Log($"SetTrackingOriginMode {trackingOriginMode}");
                    origin.RequestedTrackingOriginMode = trackingOriginMode;
                    EditorUtility.SetDirty(origin);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        public static GameObject CheckAndCreateXROrigin()
        {
            GameObject cameraOrigin;
            List<XROrigin> components = FindComponentsInScene<XROrigin>().Where(component => component.isActiveAndEnabled).ToList();
            if (components.Count == 0)
            {
                if (!EditorApplication.ExecuteMenuItem("GameObject/XR/XR Origin (VR)"))
                {
                    EditorApplication.ExecuteMenuItem("GameObject/XR/XR Origin (Action-based)");
                }
                cameraOrigin = FindComponentsInScene<XROrigin>().Where(component => component.isActiveAndEnabled).ToList()[0].gameObject;
                cameraOrigin.name = PXR_Utils.BuildingBlock + " XR Origin (XR Rig)";
                Undo.RegisterCreatedObjectUndo(cameraOrigin, "Create XR Origin");
            }
            else
            {
                cameraOrigin = components[0].gameObject;
            }

            if (!cameraOrigin.GetComponent<PXR_Manager>())
            {
                cameraOrigin.AddComponent<PXR_Manager>();
            }

            return cameraOrigin;
        }

        public static GameObject GetMainCameraGOForXROrigin()
        {
            GameObject cameraGameObject = Camera.main.gameObject;
            List<Camera> components = FindComponentsInScene<Camera>().Where(component => (component.enabled && component.gameObject.CompareTag("MainCamera"))).ToList();
            for (int i = 0; i < components.Count; i++)
            {
                GameObject gameObject = components[i].transform.gameObject;
                if (gameObject.GetComponentsInParent<XROrigin>().Length == 1)
                {
                    gameObject.SetActive(true);
                    cameraGameObject = gameObject;
                }
            }

            return cameraGameObject;
        }

        public static Camera GetMainCameraForXROrigin()
        {
            Camera mainCamera = Camera.main;
            List<Camera> components = FindComponentsInScene<Camera>().Where(component => (component.enabled && component.gameObject.CompareTag("MainCamera"))).ToList();
            for (int i = 0; i < components.Count; i++)
            {
                Camera camera = components[i];
                if (camera.GetComponentsInParent<XROrigin>().Length == 1)
                {
                    camera.gameObject.SetActive(true);
                    mainCamera = camera;
                }
            }

            return mainCamera;
        }

        public static void UpdateSamples(string packageName, string sampleDisplayName)
        {
            Debug.LogError($"Need to import {sampleDisplayName} first!");
            bool result = EditorUtility.DisplayDialog($"{sampleDisplayName}", $"It's detected that {sampleDisplayName} has not been imported in the current project. You can choose OK to auto-import it, or Cancel and install it manually. ", "OK", "Cancel");
            if (result)
            {
                // Get XRI Interaction
                if (TryFindSample(packageName, string.Empty, sampleDisplayName, out var sample))
                {
                    sample.Import(Sample.ImportOptions.OverridePreviousImports);
                }
            }
        }

        public static void InstallOrUpdateHands()
        {
            var currentT = DateTime.Now;
            var endT = currentT + TimeSpan.FromSeconds(3);

            var request = Client.Search(xrHandPackageName);
            if (request.Status == StatusCode.InProgress)
            {
                Debug.Log($"Searching for ({xrHandPackageName}) in Unity Package Registry.");
                while (request.Status == StatusCode.InProgress && currentT < endT)
                {
                    currentT = DateTime.Now;
                }
            }

            var addRequest = xrHandPackageName;
            if (request.Status == StatusCode.Success && request.Result.Length > 0)
            {
                var versions = request.Result[0].versions;
#if UNITY_2022_2_OR_NEWER
                var recommendedVersion = new PackageVersion(versions.recommended);
#else
                var recommendedVersion = new PackageVersion(versions.verified);
#endif
                var latestCompatible = new PackageVersion(versions.latestCompatible);
                if (recommendedVersion < xrHandRecommendedPackageVersion && xrHandRecommendedPackageVersion <= latestCompatible)
                    addRequest = $"{xrHandPackageName}@{xrHandRecommendedPackageVersion}";
            }

            xrHandsPackageAddRequest = Client.Add(addRequest);
            if (xrHandsPackageAddRequest.Error != null)
            {
                Debug.LogError($"Package installation error: {xrHandsPackageAddRequest.Error}: {xrHandsPackageAddRequest.Error.message}");
            }
        }

    }
}