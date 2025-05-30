using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Editor.BuildingBlocks;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.Presets;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.UI;
#if XR_HAND
using UnityEngine.XR.Hands;
#endif

namespace Unity.XR.PXR
{
    #region PICO Controller
    [BuildingBlockItem(Priority = k_SectionPriority)]
    class PXR_ControllerSection : IBuildingBlockSection
    {
        const string k_SectionId = "PICO Controller";
        public string SectionId => k_SectionId;

        const string k_SectionIconPath = "Building/Block/Section/Icon/Path";
        public string SectionIconPath => k_SectionIconPath;
        const int k_SectionPriority = 1;

        readonly IBuildingBlock[] m_BBlocksElementIds = new IBuildingBlock[]
        {
            new PXR_BuildingBlocksControllerTracking(),
            new PXR_BuildingBlocksControllerTrackingCanvas(),
        };

        public IEnumerable<IBuildingBlock> GetBuildingBlocks()
        {
            var elements = m_BBlocksElementIds.ToList();
            return elements;
        }
    }

    class PXR_BuildingBlocksControllerTracking : IBuildingBlock
    {
        const string k_Id = "PICO Controller Tracking";
        const string k_BuildingBlockPath = PXR_Utils.BuildingBlockPathO + k_Id;
        const string k_IconPath = "buildingblockIcon";
        const string k_Tooltip = k_Id + " : Configure the controller model provided by PICO SDK in the scene and configure the controller interaction events. ";
        const int k_SectionPriority = 1;

        public string Id => k_Id;
        public string IconPath => k_IconPath;
        public bool IsEnabled => true;
        public string Tooltip => k_Tooltip;

        static string controllerLeftPath = PXR_Utils.sdkPackageName + "Assets/Resources/Prefabs/LeftControllerModel.prefab";
        static string controllerRightPath = PXR_Utils.sdkPackageName + "Assets/Resources/Prefabs/RightControllerModel.prefab";
        static string xrOriginName = $"{PXR_Utils.BuildingBlock} {k_Id} XR Origin (XR Rig)";
        static string controllerLeftName = "Left Controller";
        static string controllerRightName = "Right Controller";
        static string controllerModelLeftName = $"{PXR_Utils.BuildingBlock} Left Controller";
        static string controllerModelRightName = $"{PXR_Utils.BuildingBlock} Right Controller";

        static void DoInterestingStuff()
        {
            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strBuildingBlocks, PXR_AppLog.strBuildingBlocks_PICOControllerTracking);
            // Get XRI Interaction
            var xriPackage = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(XRInteractionManager).Assembly);
            if (xriPackage == null)
            {
                Debug.LogError($"Failed, please install {PXR_Utils.xriPackageName} first!");
                return;
            }
            PXR_Utils.xriVersion = xriPackage.version;
            Debug.Log($"XRI Toolkit version = {xriPackage.version}");

            var inputActionAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(PXR_Utils.XRIDefaultInputActions);
#if XRI_TOOLKIT_3
            if (inputActionAsset == null)
            {
                PXR_Utils.UpdateSamples(PXR_Utils.xriPackageName, PXR_Utils.xriStarterAssetsSampleName);
            }
            else
            {
                // Get XROrigin
                GameObject cameraOrigin = PXR_Utils.CheckAndCreateXROriginXRI300();
                Transform cameraOffset = cameraOrigin.transform.Find("Camera Offset");
                if (cameraOffset != null)
                {
                    Transform leftController = cameraOffset.transform.Find("Left Controller");
                    Transform rightController = cameraOffset.transform.Find("Right Controller");

                    if (leftController != null)
                    {
                        GameObject oldLeftC = leftController.Find("Left Controller Visual")?.gameObject;
                        oldLeftC.SetActive(false);

                        GameObject ob = leftController.Find(controllerModelLeftName)?.gameObject;
                        if (!ob)
                        {
                            ob = PrefabUtility.LoadPrefabContents(controllerLeftPath);
                            Undo.RegisterCreatedObjectUndo(ob, "Create controllerLeftPath.");
                            Undo.SetTransformParent(ob.transform, leftController, true, "Parent to leftController.");
                            ob.transform.localPosition = Vector3.zero;
                            ob.transform.localRotation = Quaternion.identity;
                            ob.transform.localScale = Vector3.one;
                            ob.name = controllerModelLeftName;
                        }
                        ob.SetActive(true);
                    }

                    if (rightController != null)
                    {
                        GameObject oldRightC = rightController.Find("Right Controller Visual")?.gameObject;
                        oldRightC.SetActive(false);

                        GameObject ob = rightController.Find(controllerModelRightName)?.gameObject;
                        if (!ob)
                        {
                            ob = PrefabUtility.LoadPrefabContents(controllerRightPath);
                            Undo.RegisterCreatedObjectUndo(ob, "Create controllerRightPath.");
                            Undo.SetTransformParent(ob.transform, rightController, true, "Parent to rightController.");
                            ob.transform.localPosition = Vector3.zero;
                            ob.transform.localRotation = Quaternion.identity;
                            ob.transform.localScale = Vector3.one;
                            ob.name = controllerModelRightName;
                        }
                        ob.SetActive(true);
                    }
                }

                EditorSceneManager.SaveScene(cameraOrigin.gameObject.scene);
            }
#else
            var presetLC = AssetDatabase.LoadAssetAtPath<Preset>(PXR_Utils.XRIDefaultLeftControllerPreset);
                var presetRC = AssetDatabase.LoadAssetAtPath<Preset>(PXR_Utils.XRIDefaultRightControllerPreset);
                if (presetLC == null || presetRC == null || inputActionAsset == null)
                {
                    PXR_Utils.UpdateSamples(PXR_Utils.xriPackageName, PXR_Utils.xriStarterAssetsSampleName);
                }
                else
                {
                    // Get XROrigin
                    GameObject cameraOrigin = PXR_Utils.CheckAndCreateXROrigin();

                    Transform leftControllerTransform = cameraOrigin.transform.Find("Camera Offset").Find("Left Controller");
                    Transform rightControllerTransform = cameraOrigin.transform.Find("Camera Offset").Find("Right Controller");

                    if (leftControllerTransform == null || rightControllerTransform == null)
                    {
                        List<ActionBasedController> controllersComponents = PXR_Utils.FindComponentsInScene<ActionBasedController>().Where(component => component.isActiveAndEnabled).ToList();
                        if (controllersComponents.Count > 1)
                        {
                            leftControllerTransform = controllersComponents[0].transform;
                            rightControllerTransform = controllersComponents[1].transform;
                        }
                        else
                        {
                            cameraOrigin.SetActive(false);
                            if (!EditorApplication.ExecuteMenuItem("GameObject/XR/XR Origin (VR)"))
                            {
                                EditorApplication.ExecuteMenuItem("GameObject/XR/XR Origin (Action-based)");
                            }
                            cameraOrigin = PXR_Utils.FindComponentsInScene<XROrigin>().Where(component => component.isActiveAndEnabled).ToList()[0].gameObject;
                            leftControllerTransform = cameraOrigin.transform.Find("Camera Offset").Find(controllerLeftName);
                            rightControllerTransform = cameraOrigin.transform.Find("Camera Offset").Find(controllerRightName);
                        }
                    }

                    if (leftControllerTransform != null)
                    {
                        ActionBasedController leftController = leftControllerTransform.GetComponent<ActionBasedController>();

                        if (presetLC != null)
                        {
                            presetLC.ApplyTo(leftController);
                            Debug.Log("XRI Default Left Controller preset applied successfully.");
                        }
                        else
                        {
                            Debug.LogError("Failed to load XRI Default Left Controller preset.");
                        }

                        leftController.enableInputActions = true;
                        leftController.modelPrefab = AssetDatabase.LoadAssetAtPath<Transform>(controllerLeftPath);
                    }

                    if (rightControllerTransform != null)
                    {
                        ActionBasedController rightController = rightControllerTransform.GetComponent<ActionBasedController>();

                        if (presetRC != null)
                        {
                            presetRC.ApplyTo(rightController);
                            Debug.Log("XRI Default Right Controller preset applied successfully.");
                        }
                        else
                        {
                            Debug.LogError("Failed to load XRI Default Right Controller preset.");
                        }

                        rightController.enableInputActions = true;
                        rightController.modelPrefab = AssetDatabase.LoadAssetAtPath<Transform>(controllerRightPath);
                    }

                    List<InputActionAsset> inputActions = new List<InputActionAsset>();
                    inputActions.Add(inputActionAsset);

                    List<InputActionManager> iamComponents = PXR_Utils.FindComponentsInScene<InputActionManager>().Where(component => component.isActiveAndEnabled).ToList();
                    if (iamComponents.Count == 0)
                    {
                        InputActionManager inputActionManager = cameraOrigin.transform.GetComponent<InputActionManager>();
                        if (!inputActionManager)
                        {
                            inputActionManager = cameraOrigin.AddComponent<InputActionManager>();
                        }

                        inputActionManager.enabled = true;
                        iamComponents.Add(inputActionManager);
                    }
                    foreach (var component in iamComponents)
                    {
                        component.actionAssets = inputActions;
                    }

                    cameraOrigin.name = xrOriginName;
                    leftControllerTransform.name = controllerLeftName;
                    rightControllerTransform.name = controllerRightName;

                    EditorSceneManager.SaveScene(cameraOrigin.gameObject.scene);
                }
#endif
            AssetDatabase.SaveAssets();
        }

        public void ExecuteBuildingBlock() => DoInterestingStuff();

        // Each building block should have an accompanying MenuItem as a good practice, we add them here.
        [MenuItem(k_BuildingBlockPath, false, k_SectionPriority)]
        public static void ExecuteMenuItem(MenuCommand command) => DoInterestingStuff();

        [MenuItem(PXR_Utils.BuildingBlockPathP + k_Id, false, k_SectionPriority)]
        public static void ExecuteMenuItemHierarchy(MenuCommand command) => DoInterestingStuff();
    }

    class PXR_BuildingBlocksControllerTrackingCanvas : IBuildingBlock
    {
        const string k_Id = "Controller Canvas Interaction";
        const string k_BuildingBlockPath = PXR_Utils.BuildingBlockPathO + k_Id;
        const string k_IconPath = "buildingblockIcon";
        const string k_Tooltip = k_Id + " : Add Controller Ray Interaction to Canvas.";
        const int k_SectionPriority = 2;

        public string Id => k_Id;
        public string IconPath => k_IconPath;
        public bool IsEnabled => true;
        public string Tooltip => k_Tooltip;

        static string xrOriginName = $"{PXR_Utils.BuildingBlock} {k_Id} XR Origin (XR Rig)";
        static string canvasName = $"{PXR_Utils.BuildingBlock} {k_Id} Canvas";


        static void DoInterestingStuff()
        {
            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strBuildingBlocks, PXR_AppLog.strBuildingBlocks_ControllerCanvasInteraction);
            // Get XROrigin
            GameObject cameraOrigin = PXR_Utils.CheckAndCreateXROrigin();
            Undo.RegisterCreatedObjectUndo(cameraOrigin, "Create XROrigin");
            PXR_Utils.SetTrackingOriginMode();

            Canvas canvas;
            List<Canvas> canvasComponents = PXR_Utils.FindComponentsInScene<Canvas>().ToList();
            if (canvasComponents.Count == 0)
            {
                if (!EditorApplication.ExecuteMenuItem("GameObject/UI/Canvas"))
                {
                    EditorApplication.ExecuteMenuItem("GameObject/UI/Canvas");
                }
                canvas = PXR_Utils.FindComponentsInScene<Canvas>()[0];
                Undo.RegisterCreatedObjectUndo(canvas.gameObject, "Create Canvas");
            }
            else
            {
                canvas = canvasComponents[0];
            }

            if (canvas)
            {
                TrackedDeviceGraphicRaycaster trackedDeviceGraphicRaycaster = canvas.transform.GetComponent<TrackedDeviceGraphicRaycaster>();
                if (trackedDeviceGraphicRaycaster == null)
                {
                    trackedDeviceGraphicRaycaster = Undo.AddComponent<TrackedDeviceGraphicRaycaster>(canvas.gameObject);
                }
                else
                {
                    Undo.RecordObject(trackedDeviceGraphicRaycaster, "Enable TrackedDeviceGraphicRaycaster");
                    trackedDeviceGraphicRaycaster.enabled = true;
                }

                Camera mainCam = PXR_Utils.GetMainCameraForXROrigin();
                Undo.RecordObject(canvas, "Set Canvas World Camera");
                canvas.worldCamera = mainCam;

                if (canvas.renderMode != RenderMode.WorldSpace)
                {
                    Vector2 canvasDimensionsScaled;
                    Vector2 canvasDimensionsInMeters = new Vector2(1.0f, 1.0f);
                    const float canvasWorldSpaceScale = 0.001f;
                    canvasDimensionsScaled = canvasDimensionsInMeters / canvasWorldSpaceScale;

                    RectTransform rectTransform = canvas.GetComponent<RectTransform>();
                    Undo.RecordObject(rectTransform, "Change Canvas Size Delta");
                    rectTransform.sizeDelta = canvasDimensionsScaled;

                    canvas.renderMode = RenderMode.WorldSpace;
                    canvas.transform.localScale = Vector3.one * canvasWorldSpaceScale;
                    canvas.transform.position = mainCam.transform.position + new Vector3(0, 0, 1);
                    Undo.RecordObject(canvas.transform, "Change Canvas Rotation");
                    canvas.transform.rotation = mainCam.transform.rotation;
                }

                Undo.RecordObject(canvas, "Change Canvas Name");
                canvas.name = canvasName;
            }

            GameObject eventSystemGO;
            List<EventSystem> esComponents = PXR_Utils.FindComponentsInScene<EventSystem>().ToList();

#if !XRI_TOOLKIT_3
            if (esComponents.Count == 0)
            {
                if (!EditorApplication.ExecuteMenuItem("GameObject/UI/Event System"))
                {
                    EditorApplication.ExecuteMenuItem("GameObject/UI/Event System");
                }
                eventSystemGO = PXR_Utils.FindComponentsInScene<EventSystem>()[0].gameObject;
            }
            else
            {
                esComponents = PXR_Utils.FindComponentsInScene<EventSystem>().ToList();
                eventSystemGO = esComponents[0].gameObject;
                eventSystemGO.SetActive(true);
            }

            EventSystem system = eventSystemGO.transform.GetComponent<EventSystem>();
            if (system != null)
            {
                system.enabled = true;
            }

            StandaloneInputModule standalone = eventSystemGO.transform.GetComponent<StandaloneInputModule>();
            if (standalone != null)
            {
                standalone.enabled = false;
            }

            XRUIInputModule xRUIInputModule = eventSystemGO.transform.GetComponent<XRUIInputModule>();
            if (xRUIInputModule == null)
            {
                eventSystemGO.AddComponent<XRUIInputModule>();
            }
            else
            {
                xRUIInputModule.enabled = true;
            }
#else
            if (esComponents.Count > 0)
            {
                eventSystemGO = PXR_Utils.FindComponentsInScene<EventSystem>()[0].gameObject;
                Undo.RecordObject(eventSystemGO, "Disable Event System");
                eventSystemGO.SetActive(false);
            }
#endif

            Undo.RecordObject(cameraOrigin, "Change XROrigin Name");
            cameraOrigin.name = xrOriginName;

            EditorSceneManager.MarkSceneDirty(cameraOrigin.scene);
            EditorSceneManager.SaveScene(cameraOrigin.scene);
        }
        public void ExecuteBuildingBlock() => DoInterestingStuff();

        public static void ExecuteBuildingBlockStatic()
        {
            DoInterestingStuff();
        }

        // Each building block should have an accompanying MenuItem as a good practice, we add them here.
        [MenuItem(k_BuildingBlockPath, false, k_SectionPriority)]
        public static void ExecuteMenuItem(MenuCommand command) => DoInterestingStuff();

        [MenuItem(PXR_Utils.BuildingBlockPathP + k_Id, false, k_SectionPriority)]
        public static void ExecuteMenuItemHierarchy(MenuCommand command) => DoInterestingStuff();
    }

    #endregion

    #region PICO Hand
    [BuildingBlockItem(Priority = k_SectionPriority)]
    class PXR_HandSection : IBuildingBlockSection
    {
        const string k_SectionId = "PICO Hand";
        public string SectionId => k_SectionId;

        const string k_SectionIconPath = "Building/Block/Section/Icon/Path";
        public string SectionIconPath => k_SectionIconPath;
        const int k_SectionPriority = 2;

        readonly IBuildingBlock[] m_BBlocksElementIds = new IBuildingBlock[]
        {
            new PXR_BuildingBlocksPICOHandTracking(),
            new PXR_BuildingBlocksXRHandTracking(),
            new PXR_BuildingBlocksXRIHandInteraction(),
            new PXR_BuildingBlocksXRIGrabInteraction(),
            new PXR_BuildingBlocksXRIPokeInteraction(),
        };

        public IEnumerable<IBuildingBlock> GetBuildingBlocks()
        {
            var elements = m_BBlocksElementIds.ToList();
            return elements;
        }
    }
    class PXR_BuildingBlocksPICOHandTracking : IBuildingBlock
    {
        const string k_Id = "PICO Hand Tracking";
        const string k_BuildingBlockPath = PXR_Utils.BuildingBlockPathO + k_Id;
        const string k_IconPath = "buildingblockIcon";
        const string k_Tooltip = k_Id + " : Add the gesture model from PICO to the scene.";
        const int k_SectionPriority = 3;

        public string Id => k_Id;
        public string IconPath => k_IconPath;
        public bool IsEnabled => true;
        public string Tooltip => k_Tooltip;
        static string handLeftPath = PXR_Utils.sdkPackageName + "Assets/Resources/Prefabs/HandLeft.prefab";
        static string handRightPath = PXR_Utils.sdkPackageName + "Assets/Resources/Prefabs/HandRight.prefab";
        static string xrOriginName = $"{PXR_Utils.BuildingBlock} {k_Id} XR Origin (XR Rig)";
        static string handLeftName = $"{PXR_Utils.BuildingBlock} {k_Id} Left";
        static string handRightName = $"{PXR_Utils.BuildingBlock} {k_Id} Right";

        static void DoInterestingStuff()
        {
            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strBuildingBlocks, PXR_AppLog.strBuildingBlocks_PICOHandTracking);
            // Get XROrigin
            GameObject cameraOrigin = PXR_Utils.CheckAndCreateXROrigin();
            PXR_ProjectSetting.GetProjectConfig().handTracking = true;
            PXR_ProjectSetting.SaveAssets();

            // Add Left Hand
            List<PXR_Hand> leftList = PXR_Utils.FindComponentsInScene<PXR_Hand>().Where(component => component.transform.name == handLeftName).ToList();
            if (leftList.Count == 0)
            {
                GameObject leftHand = PrefabUtility.LoadPrefabContents(handLeftPath);
                if (leftHand != null)
                {
                    if (cameraOrigin != null)
                    {
                        Undo.SetTransformParent(leftHand.transform, cameraOrigin.transform.Find("Camera Offset"), true, "Parent to camera rig.");
                        leftHand.transform.localPosition = Vector3.zero;
                        leftHand.transform.localRotation = Quaternion.identity;
                        leftHand.transform.localScale = Vector3.one;
                        leftHand.SetActive(true);
                        leftHand.name = handLeftName;
                    }
                }
            }

            // Add Right Hand
            List<PXR_Hand> rightList = PXR_Utils.FindComponentsInScene<PXR_Hand>().Where(component => component.transform.name == handRightName).ToList();
            if (rightList.Count == 0)
            {
                GameObject rightHand = PrefabUtility.LoadPrefabContents(handRightPath);
                if (rightHand != null)
                {
                    if (cameraOrigin != null)
                    {
                        Undo.SetTransformParent(rightHand.transform, cameraOrigin.transform.Find("Camera Offset"), true, "Parent to camera rig.");
                        rightHand.transform.localPosition = Vector3.zero;
                        rightHand.transform.localRotation = Quaternion.identity;
                        rightHand.transform.localScale = Vector3.one;
                        rightHand.SetActive(true);
                        rightHand.name = handRightName;
                    }
                }
            }

            cameraOrigin.name = xrOriginName;

            EditorSceneManager.MarkSceneDirty(cameraOrigin.scene);
            EditorSceneManager.SaveScene(cameraOrigin.scene);
        }

        public void ExecuteBuildingBlock() => DoInterestingStuff();

        // Each building block should have an accompanying MenuItem as a good practice, we add them here.
        [MenuItem(k_BuildingBlockPath, false, k_SectionPriority)]
        public static void ExecuteMenuItem(MenuCommand command) => DoInterestingStuff();

        [MenuItem(PXR_Utils.BuildingBlockPathP + k_Id, false, k_SectionPriority)]
        public static void ExecuteMenuItemHierarchy(MenuCommand command) => DoInterestingStuff();
    }

    class PXR_BuildingBlocksXRHandTracking : IBuildingBlock
    {
        const string k_Id = "XR Hand Tracking";
        const string k_BuildingBlockPath = PXR_Utils.BuildingBlockPathO + k_Id;
        const string k_IconPath = "buildingblockIcon";
        const string k_Tooltip = k_Id + " : Add the gesture model from XRHands to the scene.";
        const int k_SectionPriority = 4;

        public string Id => k_Id;
        public string IconPath => k_IconPath;
        public bool IsEnabled => true;
        public string Tooltip => k_Tooltip;
        static string xrOriginName = $"{PXR_Utils.BuildingBlock} {k_Id} XR Origin (XR Rig)";
        static string handLeftName = $"{PXR_Utils.BuildingBlock} {k_Id} Left";
        static string handRightName = $"{PXR_Utils.BuildingBlock} {k_Id} Right";

        private static bool isExecuting = false;

        static void DoInterestingStuff()
        {
            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strBuildingBlocks, PXR_AppLog.strBuildingBlocks_XRHandTracking);
#if !XR_HAND
            if (isExecuting)
            {
                Debug.Log("DoInterestingStuff is already executing. Skipping operation.");
                return;
            }
            Debug.LogError($"Need to install {PXR_Utils.xrHandPackageName} first!");
            bool result = EditorUtility.DisplayDialog($"{PXR_Utils.xrHandPackageName}", $"It's detected that xrhand isn't installed in the current project. You can choose OK to auto-install XRHand, or Cancel and install it manually. ", "OK", "Cancel");
            if (result)
            {
                isExecuting = true;
                PXR_Utils.InstallOrUpdateHands();
            }
#else
            var xrHandPackage = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(UnityEngine.XR.Hands.XRHand).Assembly);
            if (xrHandPackage != null)
            {
                PXR_Utils.xrHandVersion = xrHandPackage.version;
                Debug.Log($"XRHand version = {PXR_Utils.xrHandVersion}");
                // if no samples, add.
                if (PXR_Utils.TryFindSample(PXR_Utils.xrHandPackageName, PXR_Utils.xrHandVersion, PXR_Utils.xrHandVisualizerSampleName, out var visualizerSample))
                {
                    visualizerSample.Import(Sample.ImportOptions.OverridePreviousImports);
                }
            }

            // Get XROrigin
            GameObject cameraOrigin = PXR_Utils.CheckAndCreateXROrigin();
            PXR_ProjectSetting.GetProjectConfig().handTracking = true;
            PXR_ProjectSetting.SaveAssets();

            // Add Left Hand
            List<XRHandSkeletonDriver> leftList = PXR_Utils.FindComponentsInScene<XRHandSkeletonDriver>().Where(component => component.transform.name == handLeftName).ToList();
            if (leftList.Count == 0)
            {
                GameObject leftHand = PrefabUtility.LoadPrefabContents(PXR_Utils.XRHandLeftHandPrefabPath);
                if (leftHand != null)
                {
                    if (cameraOrigin != null)
                    {
                        Undo.RegisterCreatedObjectUndo(leftHand, "Create left hand.");
                        Undo.SetTransformParent(leftHand.transform, cameraOrigin.transform.Find("Camera Offset"), true, "Parent to camera rig.");
                        leftHand.transform.localPosition = Vector3.zero;
                        leftHand.transform.localRotation = Quaternion.identity;
                        leftHand.transform.localScale = Vector3.one;
                        leftHand.SetActive(true);
                        leftHand.name = handLeftName;
                    }
                }
            }

            // Add Right Hand
            List<XRHandSkeletonDriver> rightList = PXR_Utils.FindComponentsInScene<XRHandSkeletonDriver>().Where(component => component.transform.name == handRightName).ToList();
            if (rightList.Count == 0)
            {
                GameObject rightHand = PrefabUtility.LoadPrefabContents(PXR_Utils.XRHandRightHandPrefabPath);
                if (rightHand != null)
                {
                    if (cameraOrigin != null)
                    {
                        Undo.RegisterCreatedObjectUndo(rightHand, "Create right hand.");
                        Undo.SetTransformParent(rightHand.transform, cameraOrigin.transform.Find("Camera Offset"), true, "Parent to camera rig.");
                        rightHand.transform.localPosition = Vector3.zero;
                        rightHand.transform.localRotation = Quaternion.identity;
                        rightHand.transform.localScale = Vector3.one;
                        rightHand.SetActive(true);
                        rightHand.name = handRightName;
                    }
                }
            }

            cameraOrigin.name = xrOriginName;

            EditorSceneManager.MarkSceneDirty(cameraOrigin.scene);
            EditorSceneManager.SaveScene(cameraOrigin.scene);
            isExecuting = false;
#endif
        }

        public void ExecuteBuildingBlock() => DoInterestingStuff();

        // Each building block should have an accompanying MenuItem as a good practice, we add them here.
        [MenuItem(k_BuildingBlockPath, false, k_SectionPriority)]
        public static void ExecuteMenuItem(MenuCommand command) => DoInterestingStuff();

        [MenuItem(PXR_Utils.BuildingBlockPathP + k_Id, false, k_SectionPriority)]
        public static void ExecuteMenuItemHierarchy(MenuCommand command) => DoInterestingStuff();
    }

    class PXR_BuildingBlocksXRIHandInteraction : IBuildingBlock
    {
        const string k_Id = "XRI Hand Interaction";
        const string k_BuildingBlockPath = PXR_Utils.BuildingBlockPathO + k_Id;
        const string k_IconPath = "buildingblockIcon";
        const string k_Tooltip = k_Id + " : This button allows one-click configuration of the gesture interaction method in XRInteraction Toolkit to enable interaction between the hand and 3D objects.";
        static string k_BuildingBlocksXROriginName = $"{PXR_Utils.BuildingBlock} XRI Hand Interaction";
        static string k_BuildingBlocksGrabName = $"{PXR_Utils.BuildingBlock} XRI Hand Grab Interactable";
        const int k_SectionPriority = 5;

        public string Id => k_Id;
        public string IconPath => k_IconPath;
        public bool IsEnabled => true;
        public string Tooltip => k_Tooltip;

        static string handLeftPath = PXR_Utils.sdkPackageName + "Assets/Resources/Hand/Models/Hand_L.fbx";
        static string handRightPath = PXR_Utils.sdkPackageName + "Assets/Resources/Hand/Models/Hand_R.fbx";

        static string aimPositionLeftHandPath = "<PicoAimHand>{LeftHand}/devicePosition";
        static string aimRotationLeftHandPath = "<PicoAimHand>{LeftHand}/deviceRotation";
        static string aimFlagsLeftHandPath = "<PicoAimHand>{LeftHand}/aimFlags";
        static string indexPressedLeftHandPath = "<PicoAimHand>{LeftHand}/indexPressed";
        static string pinchStrengthIndexLeftHandPath = "<PicoAimHand>{LeftHand}/pinchStrengthIndex";

        static string aimPositionRightHandPath = "<PicoAimHand>{RightHand}/devicePosition";
        static string aimRotationRightHandPath = "<PicoAimHand>{RightHand}/deviceRotation";
        static string aimFlagsRightHandPath = "<PicoAimHand>{RightHand}/aimFlags";
        static string indexPressedRightHandPath = "<PicoAimHand>{RightHand}/indexPressed";
        static string pinchStrengthIndexRightHandPath = "<PicoAimHand>{RightHand}/pinchStrengthIndex";

        private static bool isExecuting = false;
        static void DoInterestingStuff()
        {
            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strBuildingBlocks, PXR_AppLog.strBuildingBlocks_XRIHandInteraction);
#if !XR_HAND
            if (isExecuting)
            {
                Debug.Log("DoInterestingStuff is already executing. Skipping operation.");
                return;
            }
            Debug.LogError($"Need to install {PXR_Utils.xrHandPackageName} first!");
            bool result = EditorUtility.DisplayDialog($"{PXR_Utils.xrHandPackageName}", $"It's detected that xrhand isn't installed in the current project. You can choose OK to auto-install XRHand, or Cancel and install it manually. ", "OK", "Cancel");
            if (result)
            {
                isExecuting = true;
                PXR_Utils.InstallOrUpdateHands();
            }
#else
            var xrHandPackage = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(UnityEngine.XR.Hands.XRHand).Assembly);
            if (xrHandPackage != null)
            {
                PXR_Utils.xrHandVersion = xrHandPackage.version;
                Debug.Log($"XRHand version = {PXR_Utils.xrHandVersion}");
                // if no samples, add.
                if (PXR_Utils.TryFindSample(PXR_Utils.xrHandPackageName, PXR_Utils.xrHandVersion, PXR_Utils.xrHandVisualizerSampleName, out var visualizerSample))
                {
                    visualizerSample.Import(Sample.ImportOptions.OverridePreviousImports);
                }
            }

            // Get left controller and right controller
            // Get XRI Interaction
            var xriPackage = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(XRInteractionManager).Assembly);
            if (xriPackage != null)
            {
                PXR_Utils.xriVersion = xriPackage.version;
                Debug.Log($"XRI Toolkit version = {PXR_Utils.xriVersion}");

                // if no samples, add.
                if (PXR_Utils.TryFindSample(PXR_Utils.xriPackageName, PXR_Utils.xriVersion, PXR_Utils.xriHandsInteractionDemoSampleName, out var sampleXRHand))
                {
                    sampleXRHand.Import(Sample.ImportOptions.OverridePreviousImports);
                }

                var inputActionAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(PXR_Utils.XRIDefaultInputActions);
                if (inputActionAsset == null)
                {
                    // add Samples
                    Debug.LogError($"Failed to load XRI Default Left Controller preset. Now load the {PXR_Utils.xriStarterAssetsSampleName} sample.");
                    if (PXR_Utils.TryFindSample(PXR_Utils.xriPackageName, PXR_Utils.xriVersion, PXR_Utils.xriStarterAssetsSampleName, out var sampleXRI))
                    {
                        sampleXRI.Import(Sample.ImportOptions.OverridePreviousImports);
                        inputActionAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(PXR_Utils.XRIDefaultInputActions);
                    }
                }

                // XRI LeftHand
#if XRI_TOOLKIT_3
                InputActionMap actionMapLeftHand = inputActionAsset.FindActionMap("XRI Left");
#else
                InputActionMap actionMapLeftHand = inputActionAsset.FindActionMap("XRI LeftHand");
#endif
                if (actionMapLeftHand != null)
                {
                    InputAction aimPositionAction = actionMapLeftHand.FindAction("Aim Position");
                    if (aimPositionAction != null)
                    {
                        bool aimPositionAdded = false;
                        foreach (var b in aimPositionAction.bindings)
                        {
                            if (aimPositionLeftHandPath == b.path)
                            {
                                aimPositionAdded = true;
                            }
                        }
                        if (!aimPositionAdded)
                        {
                            Debug.Log($"{k_Id} {actionMapLeftHand.name} {aimPositionAction.name} {aimPositionLeftHandPath}");
                            aimPositionAction.AddBinding(aimPositionLeftHandPath);
                        }
                    }

                    InputAction aimRotationAction = actionMapLeftHand.FindAction("Aim Rotation");
                    if (aimRotationAction != null)
                    {
                        bool aimRotationAdded = false;
                        foreach (var b in aimRotationAction.bindings)
                        {
                            if (aimRotationLeftHandPath == b.path)
                            {
                                aimRotationAdded = true;
                            }
                        }
                        if (!aimRotationAdded)
                        {
                            aimRotationAction.AddBinding(aimRotationLeftHandPath);
                        }
                    }

                    InputAction aimFlagsAction = actionMapLeftHand.FindAction("Aim Flags");
                    if(aimFlagsAction == null)
                    {
                        aimFlagsAction = actionMapLeftHand.FindAction("Meta Aim Flags");
                    }

                    if (aimFlagsAction != null)
                    {
                        bool aimFlagsAdded = false;
                        foreach (var b in aimFlagsAction.bindings)
                        {
                            if (aimFlagsLeftHandPath == b.path)
                            {
                                aimFlagsAdded = true;
                            }
                        }
                        if (!aimFlagsAdded)
                        {
                            aimFlagsAction.AddBinding(aimFlagsLeftHandPath);
                        }
                    }
                }

                // XRI RightHand
#if XRI_TOOLKIT_3
                InputActionMap actionMapRightHand = inputActionAsset.FindActionMap("XRI Right");
#else
                InputActionMap actionMapRightHand = inputActionAsset.FindActionMap("XRI RightHand");
#endif
                if (actionMapRightHand != null)
                {
                    InputAction aimPositionAction = actionMapRightHand.FindAction("Aim Position");
                    if (aimPositionAction != null)
                    {
                        bool aimPositionAdded = false;
                        foreach (var b in aimPositionAction.bindings)
                        {
                            if (aimPositionRightHandPath == b.path)
                            {
                                aimPositionAdded = true;
                            }
                        }
                        if (!aimPositionAdded)
                        {
                            aimPositionAction.AddBinding(aimPositionRightHandPath);
                        }
                    }

                    InputAction aimRotationAction = actionMapRightHand.FindAction("Aim Rotation");
                    if (aimRotationAction != null)
                    {
                        bool aimRotationAdded = false;
                        foreach (var b in aimRotationAction.bindings)
                        {
                            if (aimRotationRightHandPath == b.path)
                            {
                                aimRotationAdded = true;
                            }
                        }
                        if (!aimRotationAdded)
                        {
                            aimRotationAction.AddBinding(aimRotationRightHandPath);
                        }
                    }

                    InputAction aimFlagsAction = actionMapRightHand.FindAction("Aim Flags");
                    if (aimFlagsAction == null)
                    {
                        aimFlagsAction = actionMapRightHand.FindAction("Meta Aim Flags");
                    }

                    if (aimFlagsAction != null)
                    {
                        bool aimFlagsAdded = false;
                        foreach (var b in aimFlagsAction.bindings)
                        {
                            if (aimFlagsRightHandPath == b.path)
                            {
                                aimFlagsAdded = true;
                            }
                        }
                        if (!aimFlagsAdded)
                        {
                            aimFlagsAction.AddBinding(aimFlagsRightHandPath);
                        }
                    }
                }

                // XRI LeftHand Interaction
#if XRI_TOOLKIT_3
                InputActionMap actionMapLeftHandI = inputActionAsset.FindActionMap("XRI Left Interaction");
#else
                InputActionMap actionMapLeftHandI = inputActionAsset.FindActionMap("XRI LeftHand Interaction");
#endif
                if (actionMapLeftHandI != null)
                {
                    InputAction selectAction = actionMapLeftHandI.FindAction("Select");
                    if (selectAction != null)
                    {
                        bool selectAdded = false;
                        foreach (var b in selectAction.bindings)
                        {
                            if (indexPressedLeftHandPath == b.path)
                            {
                                selectAdded = true;
                            }
                        }
                        if (!selectAdded)
                        {
                            selectAction.AddBinding(indexPressedLeftHandPath);
                        }
                    }

                    InputAction selectValueAction = actionMapLeftHandI.FindAction("Select Value");
                    if (selectValueAction != null)
                    {
                        bool selectValueAdded = false;
                        foreach (var b in selectValueAction.bindings)
                        {
                            if (pinchStrengthIndexLeftHandPath == b.path)
                            {
                                selectValueAdded = true;
                            }
                        }
                        if (!selectValueAdded)
                        {
                            selectValueAction.AddBinding(pinchStrengthIndexLeftHandPath);
                        }
                    }

                    InputAction uiPressAction = actionMapLeftHandI.FindAction("UI Press");
                    if (uiPressAction != null)
                    {
                        bool uiPressAdded = false;
                        foreach (var b in uiPressAction.bindings)
                        {
                            if (indexPressedLeftHandPath == b.path)
                            {
                                uiPressAdded = true;
                            }
                        }
                        if (!uiPressAdded)
                        {
                            uiPressAction.AddBinding(indexPressedLeftHandPath);
                        }
                    }

                    InputAction uiPressValueAction = actionMapLeftHandI.FindAction("UI Press Value");
                    if (uiPressValueAction != null)
                    {
                        bool uiPressValueAdded = false;
                        foreach (var b in uiPressValueAction.bindings)
                        {
                            if (pinchStrengthIndexLeftHandPath == b.path)
                            {
                                uiPressValueAdded = true;
                            }
                        }
                        if (!uiPressValueAdded)
                        {
                            uiPressValueAction.AddBinding(pinchStrengthIndexLeftHandPath);
                        }
                    }
                }

                // XRI RightHand Interaction
#if XRI_TOOLKIT_3
                InputActionMap actionMapRightHandI = inputActionAsset.FindActionMap("XRI Right Interaction");
#else
                InputActionMap actionMapRightHandI = inputActionAsset.FindActionMap("XRI RightHand Interaction");
#endif
                if (actionMapRightHandI != null)
                {
                    InputAction selectAction = actionMapRightHandI.FindAction("Select");
                    if (selectAction != null)
                    {
                        bool selectAdded = false;
                        foreach (var b in selectAction.bindings)
                        {
                            if (indexPressedRightHandPath == b.path)
                            {
                                selectAdded = true;
                            }
                        }
                        if (!selectAdded)
                        {
                            selectAction.AddBinding(indexPressedRightHandPath);
                        }
                    }

                    InputAction selectValueAction = actionMapRightHandI.FindAction("Select Value");
                    if (selectValueAction != null)
                    {
                        bool selectValueAdded = false;
                        foreach (var b in selectValueAction.bindings)
                        {
                            if (pinchStrengthIndexRightHandPath == b.path)
                            {
                                selectValueAdded = true;
                            }
                        }
                        if (!selectValueAdded)
                        {
                            selectValueAction.AddBinding(pinchStrengthIndexRightHandPath);
                        }
                    }

                    InputAction uiPressAction = actionMapRightHandI.FindAction("UI Press");
                    if (uiPressAction != null)
                    {
                        bool uiPressAdded = false;
                        foreach (var b in uiPressAction.bindings)
                        {
                            if (indexPressedRightHandPath == b.path)
                            {
                                uiPressAdded = true;
                            }
                        }
                        if (!uiPressAdded)
                        {
                            uiPressAction.AddBinding(indexPressedRightHandPath);
                        }
                    }

                    InputAction uiPressValueAction = actionMapRightHandI.FindAction("UI Press Value");
                    if (uiPressValueAction != null)
                    {
                        bool uiPressValueAdded = false;
                        foreach (var b in uiPressValueAction.bindings)
                        {
                            if (pinchStrengthIndexRightHandPath == b.path)
                            {
                                uiPressValueAdded = true;
                            }
                        }
                        if (!uiPressValueAdded)
                        {
                            uiPressValueAction.AddBinding(pinchStrengthIndexRightHandPath);
                        }
                    }
                }

                EditorUtility.SetDirty(inputActionAsset);
                AssetDatabase.SaveAssets();
            }

            AssetDatabase.SaveAssets();
            isExecuting = false;
#endif
        }

        public void ExecuteBuildingBlock() => DoInterestingStuff();

        public static void ExecuteBuildingBlockStatic()
        {
            DoInterestingStuff();
        }

        // Each building block should have an accompanying MenuItem as a good practice, we add them here.
        [MenuItem(k_BuildingBlockPath, false, k_SectionPriority)]
        public static void ExecuteMenuItem(MenuCommand command) => DoInterestingStuff();

        [MenuItem(PXR_Utils.BuildingBlockPathP + k_Id, false, k_SectionPriority)]
        public static void ExecuteMenuItemHierarchy(MenuCommand command) => DoInterestingStuff();
    }

    class PXR_BuildingBlocksXRIGrabInteraction : IBuildingBlock
    {
        const string k_Id = "XRI Grab Interaction";
        const string k_BuildingBlockPath = PXR_Utils.BuildingBlockPathO + k_Id;
        const string k_IconPath = "buildingblockIcon";
        const string k_Tooltip = k_Id + " : Grab objects with hands or controllers.";
        static string k_BuildingBlocksXROriginName = $"{PXR_Utils.BuildingBlock} XRI Hand Interaction";
        static string k_BuildingBlocksGrabName = $"{PXR_Utils.BuildingBlock} XRI Hand Grab Interactable";
        const int k_SectionPriority = 6;

        public string Id => k_Id;
        public string IconPath => k_IconPath;
        public bool IsEnabled => true;
        public string Tooltip => k_Tooltip;

        private static bool isExecuting = false;

        static void DoInterestingStuff()
        {
            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strBuildingBlocks, PXR_AppLog.strBuildingBlocks_XRIGrabInteraction);
#if !XR_HAND
            if (isExecuting)
            {
                Debug.Log("DoInterestingStuff is already executing. Skipping operation.");
                return;
            }
            Debug.LogError($"Need to install {PXR_Utils.xrHandPackageName} first!");
            bool result = EditorUtility.DisplayDialog($"{PXR_Utils.xrHandPackageName}", $"It's detected that xrhand isn't installed in the current project. You can choose OK to auto-install XRHand, or Cancel and install it manually. ", "OK", "Cancel");
            if (result)
            {
                isExecuting = true;
                PXR_Utils.InstallOrUpdateHands();
            }
#else
            // Get XRI Interaction
            var xriPackage = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(XRInteractionManager).Assembly);
            if (xriPackage == null)
            {
                Debug.LogError($"Failed, please install {PXR_Utils.xriPackageName} first!");
                return;
            }
            PXR_Utils.xriVersion = xriPackage.version;

            // if no samples, add.
            if (PXR_Utils.TryFindSample(PXR_Utils.xriPackageName, PXR_Utils.xriVersion, PXR_Utils.xriStarterAssetsSampleName, out var sampleXRIStarter))
            {
                sampleXRIStarter.Import(Sample.ImportOptions.OverridePreviousImports);
            }
            if (PXR_Utils.TryFindSample(PXR_Utils.xriPackageName, PXR_Utils.xriVersion, PXR_Utils.xriHandsInteractionDemoSampleName, out var sampleXRHand))
            {
                sampleXRHand.Import(Sample.ImportOptions.OverridePreviousImports);
            }

            Debug.Log($"XRI Toolkit version = {xriPackage.version}");

            if (PXR_Utils.FindComponentsInScene<Transform>().Where(component => component.name == k_BuildingBlocksXROriginName).ToList().Count == 0)
            {
                GameObject buildingBlockGO = new GameObject();
                Selection.activeGameObject = buildingBlockGO;

                // Get XROrigin
                GameObject cameraOrigin;
                List<XROrigin> components = PXR_Utils.FindComponentsInScene<XROrigin>().Where(component => component.isActiveAndEnabled).ToList();
                if (components.Count == 0)
                {
                    GameObject ob = PrefabUtility.LoadPrefabContents(PXR_Utils.XRInteractionHandsSetupPath);
                    var activeScene = SceneManager.GetActiveScene();
                    var rootObjects = activeScene.GetRootGameObjects();
                    Undo.SetTransformParent(ob.transform, buildingBlockGO.transform, true, "Parent to camera rig.");
                    ob.transform.localPosition = Vector3.zero;
                    ob.transform.localRotation = Quaternion.identity;
                    ob.transform.localScale = Vector3.one;
                    ob.SetActive(true);
                    cameraOrigin = PXR_Utils.FindComponentsInScene<XROrigin>().Where(component => component.isActiveAndEnabled).ToList()[0].gameObject;
                }
                else
                {
                    cameraOrigin = components[0].gameObject;
                }

                if (cameraOrigin)
                {
                    Transform parentT = cameraOrigin.transform.parent;
#if XRI_TOOLKIT_3
                    if (parentT == null || cameraOrigin.name != PXR_Utils.xri3HandsSetupPefabName)
#else
                    if (parentT == null || parentT.name != PXR_Utils.xri2HandsSetupPefabName)
#endif
                    {
                        cameraOrigin.SetActive(false);
                        GameObject ob = PrefabUtility.LoadPrefabContents(PXR_Utils.XRInteractionHandsSetupPath);
                        var activeScene = SceneManager.GetActiveScene();
                        var rootObjects = activeScene.GetRootGameObjects();
                        Undo.SetTransformParent(ob.transform, buildingBlockGO.transform, true, "Parent to camera rig.");
                        ob.transform.localPosition = Vector3.zero;
                        ob.transform.localRotation = Quaternion.identity;
                        ob.transform.localScale = Vector3.one;
                        ob.SetActive(true);
                        if (ob.transform.Find("XR Origin (XR Rig)"))
                        {
                            cameraOrigin = ob.transform.Find("XR Origin (XR Rig)").gameObject;
                        }
                    }

                    if (!cameraOrigin.GetComponent<PXR_Manager>())
                    {
                        cameraOrigin.gameObject.AddComponent<PXR_Manager>();
                    }
                }

                PXR_ProjectSetting.GetProjectConfig().handTracking = true;

                buildingBlockGO.name = k_BuildingBlocksXROriginName;
                Undo.RegisterCreatedObjectUndo(buildingBlockGO, k_Id);

                EditorSceneManager.MarkSceneDirty(buildingBlockGO.scene);
                EditorSceneManager.SaveScene(buildingBlockGO.scene);

                PXR_Utils.SetTrackingOriginMode();
                PXR_ProjectSetting.SaveAssets();
            }

            if (PXR_Utils.FindComponentsInScene<Transform>().Where(component => component.name == k_BuildingBlocksGrabName).ToList().Count == 0)
            {
                GameObject buildingBlockGO = new GameObject();
                Selection.activeGameObject = buildingBlockGO;

                Camera mainCamera = PXR_Utils.GetMainCameraForXROrigin();
                buildingBlockGO.transform.position = mainCamera.transform.position + new Vector3(0, 0, 0.5f);
                buildingBlockGO.transform.rotation = mainCamera.transform.rotation;
                buildingBlockGO.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

                if (!EditorApplication.ExecuteMenuItem("GameObject/XR/Grab Interactable"))
                {
                    EditorApplication.ExecuteMenuItem("GameObject/XR/Grab Interactable");
                }

                GameObject grabInteractableGO = GameObject.Find("Grab Interactable");

                if (grabInteractableGO != null)
                {
                    grabInteractableGO.transform.parent = buildingBlockGO.transform;
                    grabInteractableGO.transform.localPosition = new Vector3(0, 0, 0.5f);
                    grabInteractableGO.transform.localRotation = Quaternion.identity;
                    grabInteractableGO.transform.localScale = Vector3.one;
                    grabInteractableGO.SetActive(true);

                    Selection.activeGameObject = buildingBlockGO;

                    Rigidbody rigidbody = grabInteractableGO.GetComponent<Rigidbody>();
                    if (rigidbody)
                    {
                        grabInteractableGO.GetComponent<Rigidbody>().useGravity = false;
                        grabInteractableGO.GetComponent<Rigidbody>().mass = 0;
#if UNITY_6000_0_OR_NEWER
                        grabInteractableGO.GetComponent<Rigidbody>().linearDamping = 2f;
#else
                        grabInteractableGO.GetComponent<Rigidbody>().drag = 2f;
#endif
                    }
                }

                buildingBlockGO.name = k_BuildingBlocksGrabName;
                Undo.RegisterCreatedObjectUndo(buildingBlockGO, k_Id);

                EditorSceneManager.MarkSceneDirty(buildingBlockGO.scene);
                EditorSceneManager.SaveScene(buildingBlockGO.scene);
            }
            AssetDatabase.SaveAssets();
            isExecuting = false;
#endif
        }

        public void ExecuteBuildingBlock() => DoInterestingStuff();

        public static void ExecuteBuildingBlockStatic()
        {
            DoInterestingStuff();
        }

        // Each building block should have an accompanying MenuItem as a good practice, we add them here.
        [MenuItem(k_BuildingBlockPath, false, k_SectionPriority)]
        public static void ExecuteMenuItem(MenuCommand command) => DoInterestingStuff();

        [MenuItem(PXR_Utils.BuildingBlockPathP + k_Id, false, k_SectionPriority)]
        public static void ExecuteMenuItemHierarchy(MenuCommand command) => DoInterestingStuff();
    }

    class PXR_BuildingBlocksXRIPokeInteraction : IBuildingBlock
    {
        const string k_Id = "XRI Poke Interaction";
        const string k_BuildingBlockPath = PXR_Utils.BuildingBlockPathO + k_Id;
        const string k_IconPath = "buildingblockIcon";
        const string k_Tooltip = k_Id + " : Poke objects with hands or controllers.";
        static string k_BuildingBlocksXROriginName = $"{PXR_Utils.BuildingBlock} XRI Hand Interaction";
        static string k_BuildingBlocksGrabName = $"{PXR_Utils.BuildingBlock} XRI Hand Poke Interactable";
        const int k_SectionPriority = 7;

        public string Id => k_Id;
        public string IconPath => k_IconPath;
        public bool IsEnabled => true;
        public string Tooltip => k_Tooltip;

        private static bool isExecuting = false;

        static void DoInterestingStuff()
        {
            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strBuildingBlocks, PXR_AppLog.strBuildingBlocks_XRIPokeInteraction);
#if !XR_HAND
            if (isExecuting)
            {
                Debug.Log("DoInterestingStuff is already executing. Skipping operation.");
                return;
            }
            Debug.LogError($"Need to install {PXR_Utils.xrHandPackageName} first!");
            bool result = EditorUtility.DisplayDialog($"{PXR_Utils.xrHandPackageName}", $"It's detected that xrhand isn't installed in the current project. You can choose OK to auto-install XRHand, or Cancel and install it manually. ", "OK", "Cancel");
            if (result)
            {
                isExecuting = true;
                PXR_Utils.InstallOrUpdateHands();
            }
#else
            // Get XRI Interaction
            var xriPackage = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(XRInteractionManager).Assembly);
            if (xriPackage == null)
            {
                Debug.LogError($"Failed, please install {PXR_Utils.xriPackageName} first!");
                return;
            }
            PXR_Utils.xriVersion = xriPackage.version;

            // if no samples, add.
            if (PXR_Utils.TryFindSample(PXR_Utils.xriPackageName, PXR_Utils.xriVersion, PXR_Utils.xriStarterAssetsSampleName, out var sampleXRIStarter))
            {
                sampleXRIStarter.Import(Sample.ImportOptions.OverridePreviousImports);
            }
            if (PXR_Utils.TryFindSample(PXR_Utils.xriPackageName, PXR_Utils.xriVersion, PXR_Utils.xriHandsInteractionDemoSampleName, out var sampleXRHand))
            {
                sampleXRHand.Import(Sample.ImportOptions.OverridePreviousImports);
            }

            Debug.Log($"XRI Toolkit version = {xriPackage.version}");

            if (PXR_Utils.FindComponentsInScene<Transform>().Where(component => component.name == k_BuildingBlocksXROriginName).ToList().Count == 0)
            {
                GameObject buildingBlockGO = new GameObject();
                Selection.activeGameObject = buildingBlockGO;

                // Get XROrigin
                GameObject cameraOrigin;
                List<XROrigin> components = PXR_Utils.FindComponentsInScene<XROrigin>().Where(component => component.isActiveAndEnabled).ToList();
                if (components.Count == 0)
                {
                    GameObject ob = PrefabUtility.LoadPrefabContents(PXR_Utils.XRInteractionHandsSetupPath);
                    Undo.RegisterCreatedObjectUndo(ob, "Create XRInteractionHandsSetupPath.");
                    var activeScene = SceneManager.GetActiveScene();
                    var rootObjects = activeScene.GetRootGameObjects();
                    Undo.SetTransformParent(ob.transform, buildingBlockGO.transform, true, "Parent to camera rig.");
                    ob.transform.localPosition = Vector3.zero;
                    ob.transform.localRotation = Quaternion.identity;
                    ob.transform.localScale = Vector3.one;
                    ob.SetActive(true);
                    cameraOrigin = PXR_Utils.FindComponentsInScene<XROrigin>().Where(component => component.isActiveAndEnabled).ToList()[0].gameObject;
                }
                else
                {
                    cameraOrigin = components[0].gameObject;
                }

                if (cameraOrigin)
                {
                    Transform parentT = cameraOrigin.transform.parent;
#if XRI_TOOLKIT_3
                    if (parentT == null || cameraOrigin.name != PXR_Utils.xri3HandsSetupPefabName)
#else
                    if (parentT == null || parentT.name != PXR_Utils.xri2HandsSetupPefabName)
#endif
                    {
                        cameraOrigin.SetActive(false);

                        GameObject ob = PrefabUtility.LoadPrefabContents(PXR_Utils.XRInteractionHandsSetupPath);
                        Undo.RegisterCreatedObjectUndo(ob, "Create XRInteractionHandsSetupPath.");
                        var activeScene = SceneManager.GetActiveScene();
                        var rootObjects = activeScene.GetRootGameObjects();
                        Undo.SetTransformParent(ob.transform, buildingBlockGO.transform, true, "Parent to camera rig.");
                        ob.transform.localPosition = Vector3.zero;
                        ob.transform.localRotation = Quaternion.identity;
                        ob.transform.localScale = Vector3.one;
                        ob.SetActive(true);
                        if (ob.transform.Find("XR Origin (XR Rig)"))
                        {
                            cameraOrigin = ob.transform.Find("XR Origin (XR Rig)").gameObject;
                        }
                    }

                    if (!cameraOrigin.GetComponent<PXR_Manager>())
                    {
                        cameraOrigin.gameObject.AddComponent<PXR_Manager>();
                    }
                }

                PXR_ProjectSetting.GetProjectConfig().handTracking = true;

                buildingBlockGO.name = k_BuildingBlocksXROriginName;
                Undo.RegisterCreatedObjectUndo(buildingBlockGO, k_Id);

                EditorSceneManager.MarkSceneDirty(buildingBlockGO.scene);
                EditorSceneManager.SaveScene(buildingBlockGO.scene);

                PXR_Utils.SetTrackingOriginMode();
                PXR_ProjectSetting.SaveAssets();
            }

            if (PXR_Utils.FindComponentsInScene<Transform>().Where(component => component.name == k_BuildingBlocksGrabName).ToList().Count == 0)
            {
                GameObject buildingBlockGO = new GameObject();
                Selection.activeGameObject = buildingBlockGO;
                buildingBlockGO.transform.position = PXR_Utils.GetMainCameraGOForXROrigin().transform.position;
                buildingBlockGO.transform.rotation = Quaternion.identity;

                GameObject ob = PrefabUtility.LoadPrefabContents(PXR_Utils.XRInteractionPokeButtonPath);
                Undo.RegisterCreatedObjectUndo(ob, "Create XRInteractionPokeButtonPath.");
                var activeScene = SceneManager.GetActiveScene();
                var rootObjects = activeScene.GetRootGameObjects();
                Undo.SetTransformParent(ob.transform, buildingBlockGO.transform, true, "Parent to camera rig.");
                ob.transform.localPosition = new Vector3(0, 0, 0.5f);
                ob.transform.localRotation = Quaternion.identity;
                ob.transform.localScale = Vector3.one;
                ob.SetActive(true);

                buildingBlockGO.name = k_BuildingBlocksGrabName;
                Undo.RegisterCreatedObjectUndo(buildingBlockGO, k_Id);

                EditorSceneManager.MarkSceneDirty(buildingBlockGO.scene);
                EditorSceneManager.SaveScene(buildingBlockGO.scene);
            }
            AssetDatabase.SaveAssets();
            isExecuting = false;
#endif
        }

        public void ExecuteBuildingBlock() => DoInterestingStuff();

        public static void ExecuteBuildingBlockStatic()
        {
            DoInterestingStuff();
        }

        // Each building block should have an accompanying MenuItem as a good practice, we add them here.
        [MenuItem(k_BuildingBlockPath, false, k_SectionPriority)]
        public static void ExecuteMenuItem(MenuCommand command) => DoInterestingStuff();

        [MenuItem(PXR_Utils.BuildingBlockPathP + k_Id, false, k_SectionPriority)]
        public static void ExecuteMenuItemHierarchy(MenuCommand command) => DoInterestingStuff();
    }


    #endregion

    #region PICO Video Seethrough (VST)
    [BuildingBlockItem(Priority = k_SectionPriority)]
    class PXR_VideoSeethroughSection : IBuildingBlockSection
    {
        const string k_SectionId = "PICO Video Seethrough";
        public string SectionId => k_SectionId;

        const string k_SectionIconPath = "Building/Block/Section/Icon/Path";
        public string SectionIconPath => k_SectionIconPath;
        const int k_SectionPriority = 3;

        readonly IBuildingBlock[] m_BBlocksElementIds = new IBuildingBlock[]
        {
            new PXR_BuildingBlocksVideoSeethrough(),
            new PXR_BuildingBlocksVideoSeethroughEffect(),
        };

        public IEnumerable<IBuildingBlock> GetBuildingBlocks()
        {
            var elements = m_BBlocksElementIds.ToList();
            return elements;
        }
    }

    class PXR_BuildingBlocksVideoSeethrough : IBuildingBlock
    {
        const string k_Id = "PICO Video Seethrough";
        const string k_BuildingBlockPath = PXR_Utils.BuildingBlockPathO + k_Id;
        const string k_IconPath = "buildingblockIcon";
        const string k_Tooltip = k_Id + " : Video seethrought can be set up and enabled with one click.";
        const int k_SectionPriority = 8;
        static string xrOriginName = $"{PXR_Utils.BuildingBlock} {k_Id} XR Origin (XR Rig)";

        public string Id => k_Id;
        public string IconPath => k_IconPath;
        public bool IsEnabled => true;
        public string Tooltip => k_Tooltip;

        static void DoInterestingStuff()
        {
            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strBuildingBlocks, PXR_AppLog.strBuildingBlocks_PICOVideoSeethrough);
            // Get XROrigin
            GameObject cameraOrigin = PXR_Utils.CheckAndCreateXROrigin();
            if (!cameraOrigin.GetComponent<PXR_CameraEffectBlock>())
            {
                cameraOrigin.AddComponent<PXR_CameraEffectBlock>();
            }

            Camera mainCamera = PXR_Utils.GetMainCameraForXROrigin();
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = new Color(0, 0, 0, 0);

            cameraOrigin.name = xrOriginName;
            PXR_ProjectSetting.GetProjectConfig().videoSeeThrough = true;
            PXR_ProjectSetting.SaveAssets();

            EditorSceneManager.SaveScene(cameraOrigin.gameObject.scene);
        }

        public void ExecuteBuildingBlock() => DoInterestingStuff();

        // Each building block should have an accompanying MenuItem as a good practice, we add them here.
        [MenuItem(k_BuildingBlockPath, false, k_SectionPriority)]
        public static void ExecuteMenuItem(MenuCommand command) => DoInterestingStuff();

        [MenuItem(PXR_Utils.BuildingBlockPathP + k_Id, false, k_SectionPriority)]
        public static void ExecuteMenuItemHierarchy(MenuCommand command) => DoInterestingStuff();
    }

    class PXR_BuildingBlocksVideoSeethroughEffect : IBuildingBlock
    {
        const string k_Id = "PICO Video Seethrough Effect";
        const string k_BuildingBlockPath = PXR_Utils.BuildingBlockPathO + k_Id;
        const string k_IconPath = "buildingblockIcon";
        const string k_Tooltip = k_Id + " : The parameters of Video Seethrough Effect can be set and debugged. After recording the values, they can be used. ";
        const int k_SectionPriority = 9;
        static string cameraEffectPath = PXR_Utils.sdkPackageName + "Assets/BuildingBlocks/Prefabs/CameraEffect.prefab";
        static string cameraEffectName = $"{PXR_Utils.BuildingBlock} {k_Id}";
        static string xrOriginName = $"{PXR_Utils.BuildingBlock} {k_Id} XR Origin (XR Rig)";

        public string Id => k_Id;
        public string IconPath => k_IconPath;
        public bool IsEnabled => true;
        public string Tooltip => k_Tooltip;

        static void DoInterestingStuff()
        {
            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strBuildingBlocks, PXR_AppLog.strBuildingBlocks_PICOVideoSeethroughEffect);
            PXR_BuildingBlocksControllerTracking pXR_BuildingBlocksControllerTracking = new PXR_BuildingBlocksControllerTracking();
            pXR_BuildingBlocksControllerTracking.ExecuteBuildingBlock();

            // Get XROrigin
            GameObject cameraOrigin = PXR_Utils.CheckAndCreateXROrigin();
            Camera mainCamera = PXR_Utils.GetMainCameraForXROrigin();
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = new Color(0, 0, 0, 0);

            PXR_ProjectSetting.GetProjectConfig().videoSeeThrough = true;
            PXR_ProjectSetting.SaveAssets();

            Canvas canvas;
            List<Canvas> canvasComponents = PXR_Utils.FindComponentsInScene<Canvas>().ToList();
            if (canvasComponents.Count == 0)
            {
                if (!EditorApplication.ExecuteMenuItem("GameObject/UI/Canvas"))
                {
                    EditorApplication.ExecuteMenuItem("GameObject/UI/Canvas");
                }
                canvas = PXR_Utils.FindComponentsInScene<Canvas>()[0];
            }
            else
            {
                canvas = canvasComponents[0];
            }

            if (canvas)
            {
                TrackedDeviceGraphicRaycaster trackedDeviceGraphicRaycaster = canvas.transform.GetComponent<TrackedDeviceGraphicRaycaster>();
                if (trackedDeviceGraphicRaycaster == null)
                {
                    canvas.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
                }
                else
                {
                    trackedDeviceGraphicRaycaster.enabled = true;
                }
                Camera mainCam = PXR_Utils.GetMainCameraForXROrigin();
                canvas.worldCamera = mainCam;
                if (canvas.renderMode != RenderMode.WorldSpace)
                {
                    Vector2 canvasDimensionsScaled;
                    Vector2 canvasDimensionsInMeters = new Vector2(1.0f, 1.0f);
                    const float canvasWorldSpaceScale = 0.001f;
                    canvasDimensionsScaled = canvasDimensionsInMeters / canvasWorldSpaceScale;
                    canvas.GetComponent<RectTransform>().sizeDelta = canvasDimensionsScaled;
                    canvas.renderMode = RenderMode.WorldSpace;
                    canvas.transform.localScale = Vector3.one * canvasWorldSpaceScale;
                    canvas.transform.position = mainCam.transform.position + new Vector3(0, 0, 1);
                    canvas.transform.rotation = mainCam.transform.rotation;
                }

                if (!canvas.transform.Find(cameraEffectName))
                {
                    GameObject cameraEffectPrefabs = PrefabUtility.LoadPrefabContents(cameraEffectPath);
                    if (cameraEffectPrefabs != null)
                    {
                        if (cameraOrigin != null)
                        {
                            Undo.RegisterCreatedObjectUndo(cameraEffectPrefabs, "Create camera effect.");
                            Undo.SetTransformParent(cameraEffectPrefabs.transform, canvas.transform, true, "Parent to canvas.");
                            cameraEffectPrefabs.transform.localPosition = Vector3.zero;
                            cameraEffectPrefabs.transform.localRotation = Quaternion.identity;
                            cameraEffectPrefabs.transform.localScale = Vector3.one * 2;
                            cameraEffectPrefabs.SetActive(true);
                            cameraEffectPrefabs.name = cameraEffectName;
                        }
                    }
                }
            }

#if XRI_TOOLKIT_3
            GameObject eventSystemGO;
            List<EventSystem> esComponents = PXR_Utils.FindComponentsInScene<EventSystem>().ToList();

            if (esComponents.Count > 0)
            {
                eventSystemGO = PXR_Utils.FindComponentsInScene<EventSystem>()[0].gameObject;
                eventSystemGO.SetActive(false);
            }
#endif
            PXR_Utils.SetTrackingOriginMode();
            cameraOrigin.name = xrOriginName;
            Undo.RegisterCreatedObjectUndo(canvas, k_Id);
            EditorSceneManager.MarkSceneDirty(cameraOrigin.scene);
            EditorSceneManager.SaveScene(cameraOrigin.scene);
        }

        public void ExecuteBuildingBlock() => DoInterestingStuff();

        // Each building block should have an accompanying MenuItem as a good practice, we add them here.
        [MenuItem(k_BuildingBlockPath, false, k_SectionPriority)]
        public static void ExecuteMenuItem(MenuCommand command) => DoInterestingStuff();

        [MenuItem(PXR_Utils.BuildingBlockPathP + k_Id, false, k_SectionPriority)]
        public static void ExecuteMenuItemHierarchy(MenuCommand command) => DoInterestingStuff();
    }

    #endregion

    #region PICO Motion Tracking
    [BuildingBlockItem(Priority = k_SectionPriority)]
    class PXR_MotionTrackingSection : IBuildingBlockSection
    {
        const string k_SectionId = "PICO Motion Tracking";
        public string SectionId => k_SectionId;

        const string k_SectionIconPath = "Building/Block/Section/Icon/Path";
        public string SectionIconPath => k_SectionIconPath;
        const int k_SectionPriority = 4;

        readonly IBuildingBlock[] m_BBlocksElementIds = new IBuildingBlock[]
        {
            new PXR_BuildingBlocksBodyTracking(),
            new PXR_BuildingBlocksBodyTrackingDebug(),
            new PXR_BuildingBlocksObjectTracking(),
        };

        public IEnumerable<IBuildingBlock> GetBuildingBlocks()
        {
            var elements = m_BBlocksElementIds.ToList();
            return elements;
        }
    }

    class PXR_BuildingBlocksBodyTracking : IBuildingBlock
    {
        const string k_Id = "PICO Body Tracking";
        const string k_BuildingBlockPath = PXR_Utils.BuildingBlockPathO + k_Id;
        const string k_IconPath = "buildingblockIcon";
        const string k_Tooltip = k_Id + " : Body Tracking can be set with one click through this block, and 24 cubes will be used to display the tracking status of 24 human body joints in real time. ";
        const int k_SectionPriority = 10;
        static string bodyTrackingPath = PXR_Utils.sdkPackageName + "Assets/BuildingBlocks/Prefabs/BodyTracking.prefab";
        static string k_BuildingBlocksGOName = $"{PXR_Utils.BuildingBlock} {k_Id}";

        public string Id => k_Id;
        public string IconPath => k_IconPath;
        public bool IsEnabled => true;
        public string Tooltip => k_Tooltip;

        static void DoInterestingStuff()
        {
            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strBuildingBlocks, PXR_AppLog.strBuildingBlocks_PICOBodyTracking);
            // Get XROrigin
            GameObject cameraOrigin = PXR_Utils.CheckAndCreateXROrigin();

            PXR_ProjectSetting.GetProjectConfig().bodyTracking = true;
            PXR_ProjectSetting.SaveAssets();

            if (PXR_Utils.FindComponentsInScene<Transform>().Where(component => component.name == k_BuildingBlocksGOName).ToList().Count == 0)
            {
                GameObject buildingBlockGO = new GameObject();
                Selection.activeGameObject = buildingBlockGO;

                Camera mainCamera = PXR_Utils.GetMainCameraForXROrigin();
                buildingBlockGO.transform.position = mainCamera.transform.position;
                buildingBlockGO.transform.rotation = mainCamera.transform.rotation;

                GameObject ob = PrefabUtility.LoadPrefabContents(bodyTrackingPath);
                Undo.RegisterCreatedObjectUndo(ob, "Create bodyTrackingPath.");
                var activeScene = SceneManager.GetActiveScene();
                var rootObjects = activeScene.GetRootGameObjects();
                Undo.SetTransformParent(ob.transform, buildingBlockGO.transform, true, "Parent to ob.");
                ob.transform.localPosition = Vector3.zero;
                ob.transform.localRotation = Quaternion.identity;
                ob.transform.localScale = Vector3.one;
                ob.SetActive(true);

                buildingBlockGO.name = k_BuildingBlocksGOName;
                Undo.RegisterCreatedObjectUndo(buildingBlockGO, k_Id);

                PXR_Utils.SetTrackingOriginMode();
                EditorSceneManager.MarkSceneDirty(buildingBlockGO.scene);
                EditorSceneManager.SaveScene(buildingBlockGO.scene);
            }
            AssetDatabase.SaveAssets();
        }

        public void ExecuteBuildingBlock() => DoInterestingStuff();

        // Each building block should have an accompanying MenuItem as a good practice, we add them here.
        [MenuItem(k_BuildingBlockPath, false, k_SectionPriority)]
        public static void ExecuteMenuItem(MenuCommand command) => DoInterestingStuff();

        [MenuItem(PXR_Utils.BuildingBlockPathP + k_Id, false, k_SectionPriority)]
        public static void ExecuteMenuItemHierarchy(MenuCommand command) => DoInterestingStuff();
    }

    class PXR_BuildingBlocksBodyTrackingDebug : IBuildingBlock
    {
        const string k_Id = "PICO Body Tracking Debug";
        const string k_BuildingBlockPath = PXR_Utils.BuildingBlockPathO + k_Id;
        const string k_IconPath = "buildingblockIcon";
        const string k_Tooltip = k_Id + " : If the Avatar model you are using does not match the 24-joint data direction of PICO, you can adapt it by rotating the X, Y, and Z axes of the specified joint data. ";
        const int k_SectionPriority = 11;
        static string bodyTrackingPath = PXR_Utils.sdkPackageName + "Assets/BuildingBlocks/Prefabs/BodyTrackingDebug.prefab";
        static string k_BuildingBlocksGOName = $"{PXR_Utils.BuildingBlock} {k_Id}";

        public string Id => k_Id;
        public string IconPath => k_IconPath;
        public bool IsEnabled => true;
        public string Tooltip => k_Tooltip;

        static void DoInterestingStuff()
        {
            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strBuildingBlocks, PXR_AppLog.strBuildingBlocks_PICOBodyTrackingDebug);
            PXR_BuildingBlocksControllerTracking pXR_BuildingBlocksControllerTracking = new PXR_BuildingBlocksControllerTracking();
            pXR_BuildingBlocksControllerTracking.ExecuteBuildingBlock();
            // Get XROrigin
            GameObject cameraOrigin = PXR_Utils.CheckAndCreateXROrigin();

            PXR_ProjectSetting.GetProjectConfig().bodyTracking = true;
            PXR_ProjectSetting.SaveAssets();

            if (PXR_Utils.FindComponentsInScene<Transform>().Where(component => component.name == k_BuildingBlocksGOName).ToList().Count == 0)
            {
                GameObject buildingBlockGO = new GameObject();
                Selection.activeGameObject = buildingBlockGO;

                Camera mainCamera = PXR_Utils.GetMainCameraForXROrigin();
                buildingBlockGO.transform.position = mainCamera.transform.position;
                buildingBlockGO.transform.rotation = mainCamera.transform.rotation;

                GameObject ob = PrefabUtility.LoadPrefabContents(bodyTrackingPath);
                Undo.RegisterCreatedObjectUndo(ob, "Create bodyTrackingPath.");
                var activeScene = SceneManager.GetActiveScene();
                var rootObjects = activeScene.GetRootGameObjects();
                Undo.SetTransformParent(ob.transform, buildingBlockGO.transform, true, "Parent to ob.");
                ob.transform.localPosition = Vector3.zero + new Vector3(0, 0, 1);
                ob.transform.localRotation = Quaternion.identity;
                ob.transform.localScale = Vector3.one;
                ob.SetActive(true);

                buildingBlockGO.name = k_BuildingBlocksGOName;
                Undo.RegisterCreatedObjectUndo(buildingBlockGO, k_Id);

                PXR_Utils.SetTrackingOriginMode();
                EditorSceneManager.MarkSceneDirty(buildingBlockGO.scene);
                EditorSceneManager.SaveScene(buildingBlockGO.scene);
            }
            AssetDatabase.SaveAssets();
        }

        public void ExecuteBuildingBlock() => DoInterestingStuff();

        // Each building block should have an accompanying MenuItem as a good practice, we add them here.
        [MenuItem(k_BuildingBlockPath, false, k_SectionPriority)]
        public static void ExecuteMenuItem(MenuCommand command) => DoInterestingStuff();

        [MenuItem(PXR_Utils.BuildingBlockPathP + k_Id, false, k_SectionPriority)]
        public static void ExecuteMenuItemHierarchy(MenuCommand command) => DoInterestingStuff();
    }

    class PXR_BuildingBlocksObjectTracking : IBuildingBlock
    {
        const string k_Id = "PICO Object Tracking";
        const string k_BuildingBlockPath = PXR_Utils.BuildingBlockPathO + k_Id;
        const string k_IconPath = "buildingblockIcon";
        const string k_Tooltip = k_Id + " : Object Tracking can be set with one click through this block. ";
        const int k_SectionPriority = 12;
        static string k_BuildingBlocksGOName = $"{PXR_Utils.BuildingBlock} {k_Id}";

        public string Id => k_Id;
        public string IconPath => k_IconPath;
        public bool IsEnabled => true;
        public string Tooltip => k_Tooltip;

        static void DoInterestingStuff()
        {
            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strBuildingBlocks, PXR_AppLog.strBuildingBlocks_PICOObjectTracking);
            // Get XROrigin
            GameObject cameraOrigin = PXR_Utils.CheckAndCreateXROrigin();

            PXR_ProjectSetting.GetProjectConfig().bodyTracking = true;
            PXR_ProjectSetting.SaveAssets();

            if (PXR_Utils.FindComponentsInScene<Transform>().Where(component => component.name == k_BuildingBlocksGOName).ToList().Count == 0)
            {
                GameObject buildingBlockGO = new GameObject();
                Selection.activeGameObject = buildingBlockGO;

                Camera mainCamera = PXR_Utils.GetMainCameraForXROrigin();

                if (!buildingBlockGO.GetComponent<PXR_ObjectTrackingBlock>())
                {
                    buildingBlockGO.AddComponent<PXR_ObjectTrackingBlock>();
                }

                buildingBlockGO.name = k_BuildingBlocksGOName;
                Undo.RegisterCreatedObjectUndo(buildingBlockGO, k_Id);
                Undo.SetTransformParent(buildingBlockGO.transform, mainCamera.transform.parent, true, "Parent to camera offset.");
                buildingBlockGO.transform.localPosition = Vector3.zero;
                buildingBlockGO.transform.localRotation = Quaternion.identity;
                buildingBlockGO.transform.localScale = Vector3.one;

                PXR_Utils.SetTrackingOriginMode();
                EditorSceneManager.MarkSceneDirty(buildingBlockGO.scene);
                EditorSceneManager.SaveScene(buildingBlockGO.scene);
            }
            AssetDatabase.SaveAssets();
        }

        public void ExecuteBuildingBlock() => DoInterestingStuff();

        // Each building block should have an accompanying MenuItem as a good practice, we add them here.
        [MenuItem(k_BuildingBlockPath, false, k_SectionPriority)]
        public static void ExecuteMenuItem(MenuCommand command) => DoInterestingStuff();

        [MenuItem(PXR_Utils.BuildingBlockPathP + k_Id, false, k_SectionPriority)]
        public static void ExecuteMenuItemHierarchy(MenuCommand command) => DoInterestingStuff();
    }

    #endregion

#if PICO_SPATIALIZER
    #region PICO Spatial Audio
    [BuildingBlockItem(Priority = k_SectionPriority)]
    class PXR_SpatialAudioSection : IBuildingBlockSection
    {
        const string k_SectionId = "PICO Spatial Audio";
        public string SectionId => k_SectionId;

        const string k_SectionIconPath = "Building/Block/Section/Icon/Path";
        public string SectionIconPath => k_SectionIconPath;
        const int k_SectionPriority = 5;

        readonly IBuildingBlock[] m_BBlocksElementIds = new IBuildingBlock[]
        {
            new PXR_BuildingBlocksSpatialAudioFreeField(),
            new PXR_BuildingBlocksSpatialAudioAmbisonics(),
        };

        public IEnumerable<IBuildingBlock> GetBuildingBlocks()
        {
            var elements = m_BBlocksElementIds.ToList();
            return elements;
        }
    }

    class PXR_BuildingBlocksSpatialAudioFreeField : IBuildingBlock
    {
        const string k_Id = "PICO Spatial Audio Free Field";
        const string k_BuildingBlockPath = PXR_Utils.BuildingBlockPathO + k_Id;
        const string k_IconPath = "buildingblockIcon";
        const string k_Tooltip = k_Id + " : A free field is a sound field that only simulates the location of the audio source while ignoring all environmental acoustic phenomena such as reflection sounds.";
        const int k_SectionPriority = 13;
        static string freeFieldPath = PXR_Utils.sdkPackageName + "Assets/BuildingBlocks/Prefabs/SpatialAudioFreeField.prefab";
        static string k_BuildingBlocksGOName = $"{PXR_Utils.BuildingBlock} {k_Id}";

        public string Id => k_Id;
        public string IconPath => k_IconPath;
        public bool IsEnabled => true;
        public string Tooltip => k_Tooltip;

        static void DoInterestingStuff()
        {
            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strBuildingBlocks, PXR_AppLog.strBuildingBlocks_PICOSpatialAudioFreeField);
            // Get XROrigin
            GameObject cameraOrigin = PXR_Utils.CheckAndCreateXROrigin();
            Camera mainCam = PXR_Utils.GetMainCameraForXROrigin();
            if (!mainCam.GetComponent<PXR_Audio_Spatializer_AudioListener>())
            {
                mainCam.gameObject.AddComponent<PXR_Audio_Spatializer_AudioListener>();
            }

            if (PXR_Utils.FindComponentsInScene<Transform>().Where(component => component.name == k_BuildingBlocksGOName).ToList().Count == 0)
            {
                GameObject buildingBlockGO = new GameObject();
                Selection.activeGameObject = buildingBlockGO;

                Camera mainCamera = PXR_Utils.GetMainCameraForXROrigin();
                buildingBlockGO.transform.position = mainCamera.transform.position;
                buildingBlockGO.transform.rotation = mainCamera.transform.rotation;

                GameObject ob = PrefabUtility.LoadPrefabContents(freeFieldPath);
                Undo.RegisterCreatedObjectUndo(ob, "Create freeFieldPath.");
                var activeScene = SceneManager.GetActiveScene();
                var rootObjects = activeScene.GetRootGameObjects();
                Undo.SetTransformParent(ob.transform, buildingBlockGO.transform, true, "Parent to ob.");
                ob.transform.localPosition = Vector3.forward;
                ob.transform.localRotation = Quaternion.identity;
                ob.transform.localScale = Vector3.one;
                ob.SetActive(true);

                buildingBlockGO.name = k_BuildingBlocksGOName;
                Undo.RegisterCreatedObjectUndo(buildingBlockGO, k_Id);

                PXR_Utils.SetTrackingOriginMode();
                EditorSceneManager.MarkSceneDirty(buildingBlockGO.scene);
                EditorSceneManager.SaveScene(buildingBlockGO.scene);
            }
            AssetDatabase.SaveAssets();
        }

        public void ExecuteBuildingBlock() => DoInterestingStuff();

        // Each building block should have an accompanying MenuItem as a good practice, we add them here.
        [MenuItem(k_BuildingBlockPath, false, k_SectionPriority)]
        public static void ExecuteMenuItem(MenuCommand command) => DoInterestingStuff();

        [MenuItem(PXR_Utils.BuildingBlockPathP + k_Id, false, k_SectionPriority)]
        public static void ExecuteMenuItemHierarchy(MenuCommand command) => DoInterestingStuff();
    }

    class PXR_BuildingBlocksSpatialAudioAmbisonics : IBuildingBlock
    {
        const string k_Id = "PICO Spatial Audio Ambisonics";
        const string k_BuildingBlockPath = PXR_Utils.BuildingBlockPathO + k_Id;
        const string k_IconPath = "buildingblockIcon";
        const string k_Tooltip = k_Id + " : Ambisonics is a full-sphere surround sound effect that covers audio sources on the horizontal plane and below and above the listener, thereby giving the listener a highly immersive audio experience.";
        const int k_SectionPriority = 14;
        static string patialAudioAmbisonicsPath = PXR_Utils.sdkPackageName + "Assets/BuildingBlocks/Prefabs/SpatialAudioAmbisonics.prefab";
        static string k_BuildingBlocksGOName = $"{PXR_Utils.BuildingBlock} {k_Id}";

        public string Id => k_Id;
        public string IconPath => k_IconPath;
        public bool IsEnabled => true;
        public string Tooltip => k_Tooltip;

        static void DoInterestingStuff()
        {
            PXR_AppLog.PXR_OnEvent(PXR_AppLog.strBuildingBlocks, PXR_AppLog.strBuildingBlocks_PICOSpatialAudioAmbisonics);
            // Get XROrigin
            GameObject cameraOrigin = PXR_Utils.CheckAndCreateXROrigin();
            Camera mainCam = PXR_Utils.GetMainCameraForXROrigin();
            if (!mainCam.GetComponent<PXR_Audio_Spatializer_AudioListener>())
            {
                mainCam.gameObject.AddComponent<PXR_Audio_Spatializer_AudioListener>();
            }

            if (PXR_Utils.FindComponentsInScene<Transform>().Where(component => component.name == k_BuildingBlocksGOName).ToList().Count == 0)
            {
                GameObject buildingBlockGO = new GameObject();
                Selection.activeGameObject = buildingBlockGO;

                Camera mainCamera = PXR_Utils.GetMainCameraForXROrigin();
                buildingBlockGO.transform.position = mainCamera.transform.position;
                buildingBlockGO.transform.rotation = mainCamera.transform.rotation;

                GameObject ob = PrefabUtility.LoadPrefabContents(patialAudioAmbisonicsPath);
                Undo.RegisterCreatedObjectUndo(ob, "Create patialAudioAmbisonicsPath.");
                var activeScene = SceneManager.GetActiveScene();
                var rootObjects = activeScene.GetRootGameObjects();
                Undo.SetTransformParent(ob.transform, buildingBlockGO.transform, true, "Parent to ob.");
                ob.transform.localPosition = Vector3.forward;
                ob.transform.localRotation = Quaternion.identity;
                ob.transform.localScale = Vector3.one;
                ob.SetActive(true);

                buildingBlockGO.name = k_BuildingBlocksGOName;
                Undo.RegisterCreatedObjectUndo(buildingBlockGO, k_Id);

                PXR_Utils.SetTrackingOriginMode();
                EditorSceneManager.MarkSceneDirty(buildingBlockGO.scene);
                EditorSceneManager.SaveScene(buildingBlockGO.scene);
            }

            const string audioSettingsPath = "ProjectSettings/AudioManager.asset";
            var audioSettingsAsset = AssetDatabase.LoadAssetAtPath<Object>(audioSettingsPath);

            if (audioSettingsAsset == null)
            {
                Debug.LogError("Could not load audio settings asset.");
                return;
            }

            var serializedObject = new SerializedObject(audioSettingsAsset);
            var decoderProperty = serializedObject.FindProperty("m_AmbisonicDecoderPlugin");

            if (decoderProperty == null)
            {
                Debug.LogError("Could not find the Ambisonic Decoder Plugin property. Please manually set Project Settings => Audio => Ambisonic Decoder Plugin => Pico Ambisonic Decoder");
                return;
            }

            decoderProperty.stringValue = "Pico Ambisonic Decoder";
            serializedObject.ApplyModifiedProperties();

            Debug.Log("Ambisonic Decoder Plugin has been set to Pico Ambisonic Decoder.");
            AssetDatabase.SaveAssets();
        }

        public void ExecuteBuildingBlock() => DoInterestingStuff();

        // Each building block should have an accompanying MenuItem as a good practice, we add them here.
        [MenuItem(k_BuildingBlockPath, false, k_SectionPriority)]
        public static void ExecuteMenuItem(MenuCommand command) => DoInterestingStuff();

        [MenuItem(PXR_Utils.BuildingBlockPathP + k_Id, false, k_SectionPriority)]
        public static void ExecuteMenuItemHierarchy(MenuCommand command) => DoInterestingStuff();
    }

    #endregion
#endif

}