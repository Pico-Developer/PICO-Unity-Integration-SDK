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

using System.Collections;
using System.IO;
using LitJson;
using UnityEngine;

namespace Unity.XR.PXR
{
    public class PXR_ControllerLoader : MonoBehaviour
    {
        [SerializeField]
        private PXR_Input.Controller hand;

        public GameObject neo3L;
        public GameObject neo3R;
        public GameObject PICO_4L;
        public GameObject PICO_4R;
        public GameObject G3;

        public Material legacyMaterial;
        private Texture2D modelTexture2D;

        private int controllerType = -1;

        private JsonData curControllerData = null;
        private int systemOrLocal = 0;
        private bool loadModelSuccess = false;
        private string modelName = "";
        private string texFormat = "";
        private string prePath = "";
        private string modelFilePath = "/system/media/pxrRes/controller/";

        private bool leftControllerState = false;
        private bool rightControllerState = false;

        private enum ControllerSimulationType
        {
            None,
            Neo3,
            PICO4,
            G3
        }
#if UNITY_EDITOR
        [SerializeField]
        private ControllerSimulationType controllerSimulation = ControllerSimulationType.None;
#endif
        public PXR_ControllerLoader(PXR_Input.Controller controller)
        {
            hand = controller;
        }

        void Awake()
        {
#if UNITY_EDITOR
            switch (controllerSimulation)
            {
                case ControllerSimulationType.Neo3:
                    {
                        Instantiate(hand == PXR_Input.Controller.LeftController ? neo3L : neo3R, transform, false);
                        break; ;
                    }
                case ControllerSimulationType.PICO4:
                    {
                        Instantiate(hand == PXR_Input.Controller.LeftController ? PICO_4L : PICO_4R, transform, false);
                        break; ;
                    }
                case ControllerSimulationType.G3:
                    {
                        Instantiate(G3, transform, false);
                        break; ;
                    }
            }
#endif
        }

        void Start()
        {
            controllerType = PXR_Plugin.Controller.UPxr_GetControllerType();
#if UNITY_ANDROID && !UNITY_EDITOR
                LoadResFromJson();
#endif
            leftControllerState = PXR_Input.IsControllerConnected(PXR_Input.Controller.LeftController);
            rightControllerState = PXR_Input.IsControllerConnected(PXR_Input.Controller.RightController);
            if (hand == PXR_Input.Controller.LeftController)
                RefreshController(PXR_Input.Controller.LeftController);
            if (hand == PXR_Input.Controller.RightController)
                RefreshController(PXR_Input.Controller.RightController);
        }

        void Update()
        {
            if (hand == PXR_Input.Controller.LeftController)
            {
                if (PXR_Input.IsControllerConnected(PXR_Input.Controller.LeftController))
                {
                    if (!leftControllerState)
                    {
                        controllerType = PXR_Plugin.Controller.UPxr_GetControllerType();
                        RefreshController(PXR_Input.Controller.LeftController);
                        leftControllerState = true;
                    }
                }
                else
                {
                    if (leftControllerState)
                    {
                        DestroyLocalController();
                        leftControllerState = false;
                    }
                }
            }

            if (hand == PXR_Input.Controller.RightController)
            {
                if (PXR_Input.IsControllerConnected(PXR_Input.Controller.RightController))
                {
                    if (!rightControllerState)
                    {
                        controllerType = PXR_Plugin.Controller.UPxr_GetControllerType();
                        RefreshController(PXR_Input.Controller.RightController);
                        rightControllerState = true;
                    }
                }
                else
                {
                    if (rightControllerState)
                    {
                        DestroyLocalController();
                        rightControllerState = false;
                    }
                }
            }
        }

        private void RefreshController(PXR_Input.Controller hand)
        {
            if (PXR_Input.IsControllerConnected(hand))
            {
                if (systemOrLocal == 0)
                {
                    LoadControllerFromPrefab(hand);
                    if (!loadModelSuccess)
                    {
                        LoadControllerFromSystem((int)hand);
                    }
                }
                else
                {
                    var isControllerExist = false;
                    foreach (Transform t in transform)
                    {
                        if (t.name == modelName)
                        {
                            isControllerExist = true;
                        }
                    }
                    if (!isControllerExist)
                    {
                        LoadControllerFromSystem((int)hand);
                        if (!loadModelSuccess)
                        {
                            LoadControllerFromPrefab(hand);
                        }
                    }
                    else
                    {
                        var currentController = transform.Find(modelName);
                        currentController.gameObject.SetActive(true);
                    }
                }
            }
        }

        private void LoadResFromJson()
        {
            string json = PXR_Plugin.System.UPxr_GetObjectOrArray("config.controller", (int)ResUtilsType.TypeObjectArray);
            if (json != null)
            {
                JsonData jdata = JsonMapper.ToObject(json);
                if (controllerType > 0)
                {
                    if (jdata.Count >= controllerType)
                    {
                        curControllerData = jdata[controllerType - 1];
                        if (curControllerData != null)
                        {
                            modelFilePath = (string)curControllerData["base_path"];
                            modelName = (string)curControllerData["model_name"] + "_sys";
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("PXRLog LoadJsonFromSystem Error");
            }
        }

        private void DestroyLocalController()
        {
            foreach (Transform t in transform)
            {
                Destroy(modelTexture2D);
                Destroy(t.gameObject);
                Resources.UnloadUnusedAssets();
                loadModelSuccess = false;
            }
        }

        private void LoadControllerFromPrefab(PXR_Input.Controller hand)
        {
            switch (controllerType)
            {
                case 5:
                    Instantiate(hand == PXR_Input.Controller.LeftController ? neo3L : neo3R, transform, false);
                    loadModelSuccess = true;
                    break;
                case 6:
                    Instantiate(hand == PXR_Input.Controller.LeftController ? PICO_4L : PICO_4R, transform, false);
                    loadModelSuccess = true;
                    break;
                case 7:
                    Instantiate(G3, transform, false);
                    loadModelSuccess = true;
                    break;
                default:
                    loadModelSuccess = false;
                    break;
            }
        }

        private void LoadControllerFromSystem(int id)
        {
            var sysControllerName = controllerType.ToString() + id.ToString() + ".obj";
            var fullFilePath = modelFilePath + sysControllerName;

            if (!File.Exists(fullFilePath))
            {
                Debug.Log("PXRLog Load Obj From Prefab");
            }
            else
            {
                GameObject go = new GameObject
                {
                    name = modelName
                };
                MeshFilter meshFilter = go.AddComponent<MeshFilter>();
                meshFilter.mesh = PXR_ObjImporter.Instance.ImportFile(fullFilePath);
                go.transform.SetParent(transform);
                go.transform.localPosition = Vector3.zero;

                MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
                meshRenderer.material = legacyMaterial;
                LoadTexture(meshRenderer, controllerType.ToString() + id.ToString(), false);
                go.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                go.transform.localScale = new Vector3(-0.01f, 0.01f, 0.01f);
                loadModelSuccess = true;
            }
        }


        private void LoadTexture(MeshRenderer  mr,string controllerName, bool fromRes)
        {
            if (fromRes)
            {
                texFormat = "";
                prePath = controllerName;
            }
            else
            {
                texFormat = "." + (string)curControllerData["tex_format"];
                prePath = modelFilePath + controllerName;
            }

            var texturePath = prePath + "_idle" + texFormat;
            mr.material.SetTexture("_MainTex", LoadOneTexture(texturePath, fromRes));
        }

        private Texture2D LoadOneTexture(string filepath, bool fromRes)
        {
            if (fromRes)
            {
                return Resources.Load<Texture2D>(filepath);
            }
            else
            {
                int tW = (int)curControllerData["tex_width"];
                int tH = (int)curControllerData["tex_height"];
                modelTexture2D = new Texture2D(tW, tH);
                modelTexture2D.LoadImage(ReadPNG(filepath));
                return modelTexture2D;
            }
        }

        private byte[] ReadPNG(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);
            byte[] binary = new byte[fileStream.Length];
            fileStream.Read(binary, 0, (int)fileStream.Length);
            fileStream.Close();
            fileStream.Dispose();
            return binary;
        }
    }
}

