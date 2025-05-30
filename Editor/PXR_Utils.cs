using System;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager.UI;
using UnityEditor.SceneManagement;
using UnityEditor.XR.Management;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.XR.CoreUtils.XROrigin;
using UnityEngine.Rendering;
using UnityEditor.Build;
#if URP
using UnityEngine.Rendering.Universal;
#endif

namespace Unity.XR.PXR
{
    [InitializeOnLoad]
    internal static class PXR_Utils
    {
        public static string BuildingBlock = "[Building Block]";
        public const string BuildingBlockPathO = "GameObject/PICO Building Blocks/";
        public const string BuildingBlockPathP = "PICO/PICO Building Blocks/";
        public static string sdkPackageName = "Packages/com.unity.xr.picoxr/";

        public static string xriPackageName = "com.unity.xr.interaction.toolkit";
        public static string xriVersion = "2.5.4";
        public static PackageVersion xriPackageVersion250 = new PackageVersion("2.5.0");
        public static PackageVersion xriPackageVersion300 = new PackageVersion("3.0.0");
        public static string xriCategory = "XR Interaction Toolkit";
        public static string xriSamplesPath = "Assets/Samples/XR Interaction Toolkit";
        public static string xriStarterAssetsSampleName = "Starter Assets";
        public static string xriHandsInteractionDemoSampleName = "Hands Interaction Demo";
        public static string xri2HandsSetupPefabName = "XR Interaction Hands Setup";
        public static string xri3HandsSetupPefabName = "XR Origin Hands (XR Rig)";

        public static AndroidSdkVersions minSdkVersionInEditor = AndroidSdkVersions.AndroidApiLevel29;
#if UNITY_2021_2_OR_NEWER
        public static NamedBuildTarget recommendedBuildTarget = NamedBuildTarget.Android;
#else
        public static BuildTargetGroup recommendedBuildTarget = BuildTargetGroup.Android;
#endif

        public static PackageVersion XRICurPackageVersion
        {
            get
            {
                return new PackageVersion(xriVersion);
            }
        }
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
                if (XRICurPackageVersion >= xriPackageVersion250)
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
                if (XRICurPackageVersion >= xriPackageVersion250)
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
                if (XRICurPackageVersion >= xriPackageVersion300)
                {
                    return $"{xriSamplesPath}/{xriVersion}/{xriHandsInteractionDemoSampleName}/Prefabs/{xri3HandsSetupPefabName}.prefab";
                }
                else if (XRICurPackageVersion >= xriPackageVersion250 && XRICurPackageVersion < xriPackageVersion300)
                {
                    return $"{xriSamplesPath}/{xriVersion}/{xriHandsInteractionDemoSampleName}/Prefabs/{xri2HandsSetupPefabName}.prefab";
                }
                else
                {
                    return $"{xriSamplesPath}/{xriVersion}/{xriHandsInteractionDemoSampleName}/Runtime/Prefabs/{xri2HandsSetupPefabName}.prefab";
                }
            }
        }
        public static string XRInteractionPokeButtonPath
        {
            get
            {
                if (XRICurPackageVersion >= xriPackageVersion250)
                {
                    return $"{xriSamplesPath}/{xriVersion}/{xriHandsInteractionDemoSampleName}/HandsDemoSceneAssets/Prefabs/PokeButton.prefab";
                }
                else
                {
                    return $"{xriSamplesPath}/{xriVersion}/{xriHandsInteractionDemoSampleName}/Runtime/Prefabs/PokeButton.prefab";
                }
            }
        }

        public static string XRInteractionXRI300OriginPath
        {
            get
            {
                if (XRICurPackageVersion >= xriPackageVersion250)
                {
                    return $"{xriSamplesPath}/{xriVersion}/{xriStarterAssetsSampleName}/Prefabs/XR Origin (XR Rig).prefab";
                }
                else
                {
                    return $"{xriSamplesPath}/{xriVersion}/{xriStarterAssetsSampleName}/Runtime/Prefabs/XR Origin (XR Rig).prefab";
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
#if XRI_TOOLKIT_3
        public static GameObject CheckAndCreateXROriginXRI300()
        {
            GameObject cameraOrigin;
            string k_BuildingBlocksXRI300OriginName = BuildingBlock + " XR Origin (XR Rig) XRI300";

            List<Transform> transforms = FindComponentsInScene<Transform>().Where(component => component.name == k_BuildingBlocksXRI300OriginName).ToList();
            if (transforms.Count == 0)
            {
                GameObject buildingBlockGO = new GameObject();
                Selection.activeGameObject = buildingBlockGO;

                List<XROrigin> components = FindComponentsInScene<XROrigin>().Where(component => component.isActiveAndEnabled).ToList();
                if (components.Count != 0)
                {
                    foreach (var c in components)
                    {
                        c.gameObject.SetActive(false);
                    }
                }

                GameObject ob = PrefabUtility.LoadPrefabContents(XRInteractionXRI300OriginPath);
                Undo.RegisterCreatedObjectUndo(ob, "Create XRInteractionXRI300OriginPath.");
                var activeScene = SceneManager.GetActiveScene();
                var rootObjects = activeScene.GetRootGameObjects();
                Undo.SetTransformParent(ob.transform, buildingBlockGO.transform, true, "Parent to buildingBlockGO.");
                ob.transform.localPosition = Vector3.zero;
                ob.transform.localRotation = Quaternion.identity;
                ob.transform.localScale = Vector3.one;
                ob.SetActive(true);
                cameraOrigin = ob;

                if (!cameraOrigin.GetComponent<PXR_Manager>())
                {
                    cameraOrigin.AddComponent<PXR_Manager>();
                }

                if (cameraOrigin.transform.Find("Locomotion/Move"))
                {
                    cameraOrigin.transform.Find("Locomotion/Move").gameObject.SetActive(false);
                }

                buildingBlockGO.name = k_BuildingBlocksXRI300OriginName;
                Undo.RegisterCreatedObjectUndo(buildingBlockGO, "Create buildingBlockGO.");

                EditorSceneManager.MarkSceneDirty(buildingBlockGO.scene);
                EditorSceneManager.SaveScene(buildingBlockGO.scene);

                SetTrackingOriginMode();
                PXR_ProjectSetting.SaveAssets();
            }
            else
            {
                cameraOrigin = transforms[0].GetChild(0).gameObject;
            }

            return cameraOrigin;
        }
#endif
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
                cameraOrigin.transform.localPosition = Vector3.zero;
                cameraOrigin.transform.localRotation = Quaternion.identity;
                cameraOrigin.transform.localScale = Vector3.one;
                cameraOrigin.SetActive(true);
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
            Debug.LogError($"Need to import {sampleDisplayName} first! Once completed, click this Block again.");
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

        public static string minUnityVersion = "2020.3.21f1";
        public static int CompareUnityVersions(string versionA, string versionB)
        {
            string[] partsA = versionA.Split(new char[] { '.', 'f' }, StringSplitOptions.RemoveEmptyEntries);
            string[] partsB = versionB.Split(new char[] { '.', 'f' }, StringSplitOptions.RemoveEmptyEntries);

            int maxLength = Math.Max(partsA.Length, partsB.Length);

            for (int i = 0; i < maxLength; i++)
            {
                int partA = i < partsA.Length ? int.Parse(partsA[i]) : 0;
                int partB = i < partsB.Length ? int.Parse(partsB[i]) : 0;

                if (partA > partB)
                    return 1; 
                if (partA < partB)
                    return -1;
            }

            return 0; 
        }

        public static bool IsPXRPluginEnabled()
        {
            var generalSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(
                BuildTargetGroup.Android);
            if (generalSettings == null)
                return false;

            var managerSettings = generalSettings.AssignedSettings;

            return managerSettings != null && managerSettings.activeLoaders.Any(loader => loader is PXR_Loader);
        }

        [DidReloadScripts]
        [InitializeOnLoadMethod]
        public static void IsPicoSpatializerAvailable()
        {
            string name = "PICO_SPATIALIZER";
#if UNITY_EDITOR
            string spatializerPath = sdkPackageName + "SpatialAudio/Pico.Spatializer.asmdef";
            var asmDef = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(spatializerPath);
            if (asmDef == null)
            {
                RemoveDefineSymbol(name);
            }
            else
            {
                SetDefineSymbols(name);
            }
#endif
        }

        [Obsolete]
        public static bool SetDefineSymbols(string name)
        {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            //Debug.Log($"SetDefineSymbols : {defines} , targetGroup={EditorUserBuildSettings.selectedBuildTargetGroup}");
            if (!defines.Contains(name))
            {
                defines += ";" + name;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
                //Debug.Log($"Added {name} to scripting define symbols.");
                return true;
            }
            else
            {
                //Debug.Log($"{name} already exists.");
                return false;
            }
        }

        [Obsolete]
        public static void RemoveDefineSymbol(string name)
        {
            string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            Debug.Log($"RemoveDefineSymbol : {currentDefines} , targetGroup={EditorUserBuildSettings.selectedBuildTargetGroup}");
            string[] definesArray = currentDefines.Split(';');
            List<string> definesList = new List<string>(definesArray);

            if (definesList.Contains(name))
            {
                definesList.Remove(name);
            }

            string newDefines = string.Join(";", definesList.ToArray());

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newDefines);
            Debug.Log($"Removed {name} from scripting define symbols.");
        }

#if URP
        public static UniversalRenderPipelineAsset GetCurrentURPAsset()
        {
            UniversalRenderPipelineAsset universalRenderPipelineAsset = null;
            if (QualitySettings.renderPipeline != null)
            {
                universalRenderPipelineAsset = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;

            }
            else if (GraphicsSettings.currentRenderPipeline != null)
            {
                universalRenderPipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;
            }
            return universalRenderPipelineAsset;
        }
#endif
    }
}
