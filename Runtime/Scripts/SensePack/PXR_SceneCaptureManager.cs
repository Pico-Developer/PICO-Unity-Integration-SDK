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

using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace Unity.XR.PXR
{
    public class PXR_SceneCaptureManager : MonoBehaviour
    {
        private const string TAG = "[PXR_SceneCaptureManager]";
        
        [SerializeField]
        private GameObject box2DPrefab;
        [SerializeField]
        private GameObject box3DPrefab;
        [SerializeField]
        private TextAsset sceneCaptureData;
        private List<Guid> sceneAnchorList;
        
        void Start()
        {

#if UNITY_EDITOR
            LoadSceneDataFromJson();
#else
            PXR_Manager.EnableVideoSeeThrough = true;
            sceneAnchorList = new List<Guid>();
            StartSceneCaptureProvider();
#endif
        }

        void OnEnable()
        {
            PXR_Manager.SceneAnchorDataUpdated += SceneAnchorDataUpdated;
        }

        void OnDisable()
        {
            PXR_Manager.SceneAnchorDataUpdated -= SceneAnchorDataUpdated;
        }

        private async void StartSceneCaptureProvider()
        {
            var result0 = await PXR_MixedReality.StartSenseDataProvider(PxrSenseDataProviderType.SceneCapture);
            if (result0 == PxrResult.SUCCESS)
            {
                LoadSceneData();
            }
            else
            {
                PLog.e(TAG, "SceneCaptureProvider start fail", false);
            }

        }

        private async void LoadSceneData()
        {
            var result = await PXR_MixedReality.QuerySceneAnchorAsync(default);

            if (result.result == PxrResult.SUCCESS)
            {
                if (result.anchorDictionary.Count > 0)
                {
                    foreach (var item in result.anchorDictionary)
                    {
                        if (!sceneAnchorList.Contains(item.Value))
                        {
                            var result1 = PXR_MixedReality.GetSceneSemanticLabel(item.Key, out var label);
                            if (result1 == PxrResult.SUCCESS)
                            {
                                DrawSceneModel(item.Key, label);
                            }
                            sceneAnchorList.Add(item.Value);
                        }
                    }
                }
                else
                {
                    var result2 = await PXR_MixedReality.StartSceneCaptureAsync(default);
                    if (result2 == PxrResult.SUCCESS)
                    {
                        LoadSceneData();
                    }
                    PLog.e(TAG, "Query scene anchor count is 0", false);
                }
            }
            else
            {
                PLog.e(TAG, "Query scene anchor fail" + result.result, false);
            }
        }

        private void SceneAnchorDataUpdated()
        {
            LoadSceneData();
        }

        private void DrawSceneModel(ulong anchorHandle, PxrSemanticLabel label)
        {
            /*
             *  UnKnown 0,
                Floor-------Polygon
                Ceiling,----Polygon
                Wall,-------Box2D
                Door,-------Box2D
                Window,-----Box2D
                Opening,----Box2D
                WallArt,----Box2D
                VirtualWall,----Box2D
                Table,------Box3D
                Sofa,-------Box3D
                Chair,------Box3D
                Chair,------Box3D
                Plant,------Box3D
                Refrigerator,------Box3D
                WashingMachine,------Box3D
                AirConditioner,------Box3D
                Lamp,------Box3D
             */

            switch (label)
            {
                case PxrSemanticLabel.Unknown:
                    break;
                case PxrSemanticLabel.Floor:
                case PxrSemanticLabel.Ceiling:
                    break;
                case PxrSemanticLabel.Wall:
                case PxrSemanticLabel.Door:
                case PxrSemanticLabel.Window:
                case PxrSemanticLabel.Opening:
                case PxrSemanticLabel.WallArt:
                    {
                        var result = PXR_MixedReality.GetSceneBox2DData(anchorHandle, out var offset, out var extent);
                        if (result == PxrResult.SUCCESS)
                        {
                            //currently,offset not support
                            if (box2DPrefab != null)
                            {
                                var sceneAnchor = new GameObject(anchorHandle.ToString());
                                var box2D = Instantiate(box2DPrefab);
                                box2D.transform.localScale = new Vector3(extent.x, extent.y, 0);
                                PXR_MixedReality.LocateAnchor(anchorHandle, out var anchorPosition, out var anchorRotation);
                                box2D.transform.SetParent(sceneAnchor.transform);
                                sceneAnchor.transform.rotation = anchorRotation;
                                sceneAnchor.transform.position = anchorPosition;
                            }
                            else
                            {
                                PLog.e(TAG, "box2D prefab is null", false);
                            }
                        }
                    }
                    break;
                case PxrSemanticLabel.Table:
                case PxrSemanticLabel.Sofa:
                case PxrSemanticLabel.Chair:
                case PxrSemanticLabel.Plant:
                case PxrSemanticLabel.Refrigerator:
                case PxrSemanticLabel.WashingMachine:
                case PxrSemanticLabel.AirConditioner:
                case PxrSemanticLabel.Lamp:
                    {
                        var result = PXR_MixedReality.GetSceneBox3DData(anchorHandle, out var position, out var rotation, out var extent);
                        if (result == PxrResult.SUCCESS)
                        {
                            if (box3DPrefab != null)
                            {
                                var sceneAnchor = new GameObject(anchorHandle.ToString());
                                var box3D = Instantiate(box3DPrefab);
                                //currently,rotation not support
                                box3D.transform.localPosition = position;
                                box3D.transform.localScale = extent;
                                PXR_MixedReality.LocateAnchor(anchorHandle, out var anchorPosition, out var anchorRotation);
                                box3D.transform.SetParent(sceneAnchor.transform);
                                sceneAnchor.transform.rotation = anchorRotation;
                                sceneAnchor.transform.position = anchorPosition;

                            }
                            else
                            {
                                PLog.e(TAG, "box3D prefab is null", false);
                            }
                        }
                    }
                    break;
            }
        }

#if UNITY_EDITOR
        private void LoadSceneDataFromJson()
        {
            if (sceneCaptureData != null)
            {
                JsonData jsonData = JsonMapper.ToObject(sceneCaptureData.ToString());
                for (int i = 0; i < jsonData.Count; i++)
                {
                    var sceneAnchorInfo = jsonData[i];

                    var uuid = sceneAnchorInfo["Guid"].ToString();
                    Enum.TryParse(jsonData[i]["SemanticLabel"].ToString(), out PxrSemanticLabel semantic);

                    var pX = Convert.ToSingle(jsonData[i]["Position"]["x"].ToString());
                    var pY = Convert.ToSingle(jsonData[i]["Position"]["y"].ToString());
                    var pZ = Convert.ToSingle(jsonData[i]["Position"]["z"].ToString());
                    var position = new Vector3(pX, pY, pZ);

                    var rX = Convert.ToSingle(jsonData[i]["Rotation"]["x"].ToString());
                    var rY = Convert.ToSingle(jsonData[i]["Rotation"]["y"].ToString());
                    var rZ = Convert.ToSingle(jsonData[i]["Rotation"]["z"].ToString());
                    var rW = Convert.ToSingle(jsonData[i]["Rotation"]["w"].ToString());
                    var rotation = new Quaternion(rX, rY, rZ, rW);

                    var box2DInfo = jsonData[i]["Box2DInfo"];
                    if (box2DInfo != null)
                    {
                        var oX = Convert.ToSingle(jsonData[i]["Box2DInfo"]["Offset"]["x"].ToString());
                        var oY = Convert.ToSingle(jsonData[i]["Box2DInfo"]["Offset"]["y"].ToString());
                        var offset = new Vector2(oX, oY);
                        var eX = Convert.ToSingle(jsonData[i]["Box2DInfo"]["Extent"]["x"].ToString());
                        var eY = Convert.ToSingle(jsonData[i]["Box2DInfo"]["Extent"]["y"].ToString());
                        var extent = new Vector2(eX, eY);
                        DrawSceneCaptureDataBox2D(uuid, semantic, position, rotation, offset, extent);
                    }

                    var box3DInfo = jsonData[i]["Box3DInfo"];
                    if (box3DInfo != null)
                    {
                        var oX = Convert.ToSingle(jsonData[i]["Box3DInfo"]["Offset"]["x"].ToString());
                        var oY = Convert.ToSingle(jsonData[i]["Box3DInfo"]["Offset"]["y"].ToString());
                        var oZ = Convert.ToSingle(jsonData[i]["Box3DInfo"]["Offset"]["z"].ToString());
                        var offset = new Vector3(oX, oY, oZ);
                        var eX = Convert.ToSingle(jsonData[i]["Box3DInfo"]["Extent"]["x"].ToString());
                        var eY = Convert.ToSingle(jsonData[i]["Box3DInfo"]["Extent"]["y"].ToString());
                        var eZ = Convert.ToSingle(jsonData[i]["Box3DInfo"]["Extent"]["z"].ToString());
                        var extent = new Vector3(eX, eY, eZ);
                        DrawSceneCaptureDataBox3D(uuid, semantic, position, rotation, offset, extent);
                    }
                }
            }
        }

        private void DrawSceneCaptureDataBox2D(string uuid, PxrSemanticLabel label, Vector3 position, Quaternion rotation, Vector2 offset, Vector2 extent)
        {
            if (box2DPrefab != null)
            {
                var sceneAnchor = new GameObject(uuid);
                var box2D = Instantiate(box2DPrefab);
                box2D.transform.localScale = new Vector3(extent.x, extent.y, 0);
                box2D.transform.SetParent(sceneAnchor.transform);
                sceneAnchor.transform.rotation = rotation;
                sceneAnchor.transform.position = position;
            }
        }

        private void DrawSceneCaptureDataBox3D(string uuid, PxrSemanticLabel label, Vector3 position, Quaternion rotation, Vector3 offset, Vector3 extent)
        {
            if (box3DPrefab != null)
            {
                var sceneAnchor = new GameObject(uuid);
                var box3D = Instantiate(box3DPrefab);
                //currently,rotation not support
                box3D.transform.localPosition = offset;
                box3D.transform.localScale = extent;
                box3D.transform.SetParent(sceneAnchor.transform);
                sceneAnchor.transform.rotation = rotation;
                sceneAnchor.transform.position = position;
            }
        }
#endif
    }
}

