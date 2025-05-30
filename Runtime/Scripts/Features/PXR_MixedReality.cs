/*******************************************************************************
Copyright ? 2015-2022 Pico Technology Co., Ltd.All rights reserved.  

NOTICE��All information contained herein is, and remains the property of 
Pico Technology Co., Ltd. The intellectual and technical concepts 
contained hererin are proprietary to Pico Technology Co., Ltd. and may be 
covered by patents, patents in process, and are protected by trade secret or 
copyright law. Dissemination of this information or reproduction of this 
material is strictly forbidden unless prior written permission is obtained from
Pico Technology Co., Ltd. 
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.XR;

namespace Unity.XR.PXR
{
    public class PXR_MixedReality
    {
        private const string TAG = "[PXR_MixedReality]";
        /// <summary>
        /// Starts the Spatial Anchor or Scene Capture feature by starting the corresponding sense data provider.
        /// </summary>
        /// <param name="type">Specifies the type of sense data provider to start: `SpatialAnchor` or `SceneCapture`.</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details.</returns>
        public static async Task<PxrResult> StartSenseDataProvider(PxrSenseDataProviderType type, CancellationToken token = default)
        {
            int isPermissionMR = -1;
            PXR_PermissionRequest.RequestUserPermissionMR(d =>
            {
                isPermissionMR = -2;
                Debug.LogError($"request permission result is {d}");
            }, async g => { isPermissionMR = 0; }, dda =>
            {
                isPermissionMR = -2;
                Debug.LogError($"request permission result is {dda}");
            });

            while (isPermissionMR == -1)
            {
                await Task.Delay(200);
            }
            if (isPermissionMR != 0)
            {
                return await Task.Run(() => { return PxrResult.ERROR_PERMISSION_INSUFFICIENT; }, token);
            }

            return await Task.Run(async () =>
            {
                var providerHandle = PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(type);
                var startResult = PXR_Plugin.MixedReality.UPxr_StartSenseDataProviderAsync(providerHandle, out var future);
                if (startResult == PxrResult.SUCCESS)
                {
                    while (true)
                    {
                        var pollResult = PXR_Plugin.MixedReality.UPxr_PollFuture(future, out var futureState);
                        if (pollResult == PxrResult.SUCCESS)
                        {
                            if (futureState == PxrFutureState.Ready)
                            {
                                var completeResult = PXR_Plugin.MixedReality.UPxr_StartSenseDataProviderComplete(future, out var completion);
                                if (completeResult == PxrResult.SUCCESS)
                                {
                                    return completion.futureResult;
                                }
                                else
                                {
                                    return completeResult;
                                }
                            }
                        }
                        else
                        {
                            return pollResult;
                        }
                        await Task.Delay(11, token);
                    }
                }
                else
                {
                    return startResult;
                }

            }, token);
        }

        /// <summary>
        /// Gets the state of the sense data provider.
        /// </summary>
        /// <param name="type">Specifies the type of sense data provider to get state for: `SpatialAnchor` or `SceneCapture`.</param>
        /// <param name="state">Returns the state of the specified sense data provider.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details.</returns>
        public static PxrResult GetSenseDataProviderState(PxrSenseDataProviderType type, out PxrSenseDataProviderState state)
        {
            var providerHandle = PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(type);
            return PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderState(providerHandle, out state);
        }

        /// <summary>
        /// Stops the Spatial Anchor or Scene Capture feature by stopping the corresponding sense data provider.
        /// </summary>
        /// <param name="type">Specifies the sense data provider to stop: `SpatialAnchor` or `SceneCapture`.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details.</returns>
        public static PxrResult StopSenseDataProvider(PxrSenseDataProviderType type)
        {
            if (type == PxrSenseDataProviderType.SceneCapture)
            {
                PXR_Plugin.MixedReality.SceneAnchorData.Clear();
            }
            var providerHandle = PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(type);
            var stopResult = PXR_Plugin.MixedReality.UPxr_StopSenseDataProvider(providerHandle);
            return stopResult;
        }

        /// <summary>
        /// Creates a spatial anchor in the app's memory.
        /// </summary>
        /// <param name="position">Specifies the position of the spatial anchor.</param>
        /// <param name="rotation">Specifies the rotation of the spatial anchor.</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details. In addition, the handle and UUID of the spatial anchor created are returned.</returns>
        public static async Task<(PxrResult result, ulong anchorHandle, Guid uuid)> CreateSpatialAnchorAsync(Vector3 position, Quaternion rotation, CancellationToken token = default)
        {
            return await Task.Run(async () =>
            {
                var createResult = PXR_Plugin.MixedReality.UPxr_CreateSpatialAnchorAsync(PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SpatialAnchor), position, rotation, out var future);
                if (createResult == PxrResult.SUCCESS)
                {
                    while (true)
                    {
                        var pollResult = PXR_Plugin.MixedReality.UPxr_PollFuture(future, out var futureState);
                        if (pollResult == PxrResult.SUCCESS)
                        {
                            if (futureState == PxrFutureState.Ready)
                            {
                                var completeResult = PXR_Plugin.MixedReality.UPxr_CreateSpatialAnchorComplete(PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SpatialAnchor), future, out var completion);
                                if (completeResult == PxrResult.SUCCESS)
                                {
                                    byte[] byteArray = new byte[16];
                                    BitConverter.GetBytes(completion.uuid.value0).CopyTo(byteArray, 0);
                                    BitConverter.GetBytes(completion.uuid.value1).CopyTo(byteArray, 8);
                                    var uuid = new Guid(byteArray);
                                    return (completion.futureResult, completion.anchorHandle, uuid);
                                }
                                else
                                {
                                    return (completeResult, ulong.MinValue, Guid.Empty);
                                }
                            }
                        }
                        else
                        {
                            return (pollResult, ulong.MinValue, Guid.Empty);
                        }

                        await Task.Delay(11, token);
                    }
                }
                else
                {
                    return (createResult, ulong.MinValue, Guid.Empty);
                }
            }, token);
        }

        /// <summary>
        /// Persists a spatial anchor to the PICO device's local disk.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the spatial anchor to persist.</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details.</returns>
        public static async Task<PxrResult> PersistSpatialAnchorAsync(ulong anchorHandle, CancellationToken token = default)
        {
            return await Task.Run(async () =>
            {
                var persistResult = PXR_Plugin.MixedReality.UPxr_PersistSpatialAnchorAsync(PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SpatialAnchor), anchorHandle, out var future);
                if (persistResult == PxrResult.SUCCESS)
                {
                    while (true)
                    {
                        var pollResult = PXR_Plugin.MixedReality.UPxr_PollFuture(future, out var futureState);
                        if (pollResult == PxrResult.SUCCESS)
                        {
                            if (futureState == PxrFutureState.Ready)
                            {
                                var completeResult = PXR_Plugin.MixedReality.UPxr_PersistSpatialAnchorComplete(PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SpatialAnchor), future, out var completion);
                                if (completeResult == PxrResult.SUCCESS)
                                {
                                    return completion.futureResult;
                                }
                                else
                                {
                                    return completeResult;
                                }
                            }
                        }
                        else
                        {
                            return pollResult;
                        }
                        await Task.Delay(11, token);
                    }
                }
                else
                {
                    return persistResult;
                }
            }, token);
        }

        /// <summary>
        /// Unpersists a spatial anchor from the PICO device's local disk.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the spatial anchor to unpersist.</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details.</returns>
        public static async Task<PxrResult> UnPersistSpatialAnchorAsync(ulong anchorHandle, CancellationToken token = default)
        {
            return await Task.Run(async () =>
            {
                var unPersistResult = PXR_Plugin.MixedReality.UPxr_UnPersistSpatialAnchorAsync(PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SpatialAnchor), anchorHandle, out var future);
                if (unPersistResult == PxrResult.SUCCESS)
                {
                    while (true)
                    {
                        var pollResult = PXR_Plugin.MixedReality.UPxr_PollFuture(future, out var futureState);
                        if (pollResult == PxrResult.SUCCESS)
                        {
                            if (futureState == PxrFutureState.Ready)
                            {
                                var completeResult = PXR_Plugin.MixedReality.UPxr_UnPersistSpatialAnchorComplete(PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SpatialAnchor), future, out var completion);
                                if (completeResult == PxrResult.SUCCESS)
                                {
                                    return completion.futureResult;
                                }
                                else
                                {
                                    return completeResult;
                                }
                            }
                        }
                        else
                        {
                            return pollResult;
                        }
                        await Task.Delay(11, token);
                    }
                }
                else
                {
                    return unPersistResult;
                }
            }, token);
        }

        /// <summary>
        /// Destroys an anchor in the app's memory.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the anchor to destroy. If you specify the handle of a scene anchor, the "Invalid handle" prompt will appear.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details.</returns>
        public static PxrResult DestroyAnchor(ulong anchorHandle)
        {
            if (PXR_Plugin.MixedReality.SceneAnchorData.ContainsKey(anchorHandle))
            {
                return PxrResult.ERROR_HANDLE_INVALID;
            }
            else
            {
                return PXR_Plugin.MixedReality.UPxr_DestroyAnchor(anchorHandle);
            }
        }

        /// <summary>
        /// Gets the UUID of an anchor.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the anchor to get UUID for.</param>
        /// <param name="uuid">Returns the UUID of the specified anchor.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details.</returns>
        public static PxrResult GetAnchorUuid(ulong anchorHandle, out Guid uuid)
        {
            if (PXR_Plugin.MixedReality.SceneAnchorData.ContainsKey(anchorHandle))
            {
                if (PXR_Plugin.MixedReality.SceneAnchorData.TryGetValue(anchorHandle, out var data))
                {
                    uuid = data.uuid;
                    return PxrResult.SUCCESS;
                }
                else
                {
                    uuid = Guid.Empty;
                    return PxrResult.ERROR_HANDLE_INVALID;
                }
            }
            else
            {
                return PXR_Plugin.MixedReality.UPxr_GetAnchorUuid(anchorHandle, out uuid);
            }
        }

        /// <summary>
        /// Locates an anchor by getting its real-time position and rotation.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the anchor to locate.</param>
        /// <param name="position">Returns the position of the anchor.</param>
        /// <param name="rotation">Returns the rotation of the anchor.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details.</returns>
        public static PxrResult LocateAnchor(ulong anchorHandle, out Vector3 position, out Quaternion rotation)
        {
            if (PXR_Plugin.MixedReality.SceneAnchorData.ContainsKey(anchorHandle))
            {
                if (PXR_Plugin.MixedReality.SceneAnchorData.TryGetValue(anchorHandle, out var data))
                {
                    position = data.position;
                    rotation = data.rotation;
                    return PxrResult.SUCCESS;
                }
                else
                {
                    position = Vector3.zero;
                    rotation = Quaternion.identity;
                    return PxrResult.ERROR_HANDLE_INVALID;
                }
            }
            else
            {
                return PXR_Plugin.MixedReality.UPxr_LocateAnchor(anchorHandle, out position, out rotation);
            }
        }

        /// <summary>
        /// Loads spatial anchor(s) from the device's local storage and the app's memory.
        /// </summary>
        /// <param name="uuids">Specifies the UUID(s) of the spatial anchor(s) you want to load. If you do not pass any UUID, all spatial anchors will be loaded.</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details. In addition, a list of the handles of the loaded anchors is returned.</returns>
        public static async Task<(PxrResult result, List<ulong> anchorHandleList)> QuerySpatialAnchorAsync(Guid[] uuids = null, CancellationToken token = default)
        {
            return await Task.Run(async () =>
            {
                if (uuids == null)
                {
                    uuids = Array.Empty<Guid>();
                }
                var queryResult = PXR_Plugin.MixedReality.UPxr_QuerySenseDataByUuidAsync(uuids, out var future);
                if (queryResult == PxrResult.SUCCESS)
                {
                    while (true)
                    {
                        var pollResult = PXR_Plugin.MixedReality.UPxr_PollFuture(future, out var futureState);
                        if (pollResult == PxrResult.SUCCESS)
                        {
                            if (futureState == PxrFutureState.Ready)
                            {
                                var completeResult = PXR_Plugin.MixedReality.UPxr_QuerySenseDataComplete(PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SpatialAnchor), future, out var completion);
                                if (completeResult == PxrResult.SUCCESS)
                                {
                                    if (completion.futureResult == PxrResult.SUCCESS)
                                    {
                                        var getResult = PXR_Plugin.MixedReality.UPxr_GetQueriedSenseData(PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SpatialAnchor), completion.snapshotHandle, out var entityInfos);
                                        if (getResult == PxrResult.SUCCESS)
                                        {
                                            var anchorHandleList = new List<ulong>();
                                            foreach (var e in entityInfos)
                                            {
                                                var retrieveResult = PXR_Plugin.MixedReality.UPxr_RetrieveSpatialEntityAnchor(completion.snapshotHandle, e.spatialEntity, out var anchorHandle);
                                                if (retrieveResult == PxrResult.SUCCESS)
                                                {
                                                    anchorHandleList.Add(anchorHandle);
                                                }
                                            }
                                            PXR_Plugin.MixedReality.UPxr_DestroySenseDataQueryResult(completion.snapshotHandle);
                                            return (getResult, anchorHandleList);
                                        }
                                        else
                                        {
                                            return (getResult, new List<ulong>());
                                        }
                                    }
                                    else
                                    {
                                        return (completion.futureResult, new List<ulong>());
                                    }
                                }
                                else
                                {
                                    return (completeResult, new List<ulong>());
                                }
                            }
                        }
                        else
                        {
                            return (pollResult, new List<ulong>());
                        }
                        await Task.Delay(11, token);
                    }
                }
                else
                {
                    return (queryResult, new List<ulong>());
                }
            }, token);
        }

        public static async Task<(PxrResult result, List<GameObject> spatialAnchorObjects)> QuerySpatialAnchorObjectsAsync(Guid[] uuids = null, CancellationToken token = default)
        {
            return await Task.Run(async () =>
            {
                if (uuids == null)
                {
                    uuids = Array.Empty<Guid>();
                }
                var queryResult = PXR_Plugin.MixedReality.UPxr_QuerySenseDataByUuidAsync(uuids, out var future);
                if (queryResult == PxrResult.SUCCESS)
                {
                    while (true)
                    {
                        var pollResult = PXR_Plugin.MixedReality.UPxr_PollFuture(future, out var futureState);
                        if (pollResult == PxrResult.SUCCESS)
                        {
                            if (futureState == PxrFutureState.Ready)
                            {
                                var completeResult = PXR_Plugin.MixedReality.UPxr_QuerySenseDataComplete(PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SpatialAnchor), future, out var completion);
                                if (completeResult == PxrResult.SUCCESS)
                                {
                                    if (completion.futureResult == PxrResult.SUCCESS)
                                    {
                                        var getResult = PXR_Plugin.MixedReality.UPxr_GetQueriedSenseData(PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SpatialAnchor), completion.snapshotHandle, out var entityInfos);
                                        if (getResult == PxrResult.SUCCESS)
                                        {
                                            var spatialAnchorList = new List<GameObject>();
                                            foreach (var e in entityInfos)
                                            {
                                                var retrieveResult = PXR_Plugin.MixedReality.UPxr_RetrieveSpatialEntityAnchor(completion.snapshotHandle, e.spatialEntity, out var anchorHandle);
                                                if (retrieveResult == PxrResult.SUCCESS)
                                                {
                                                    var anchorObject = new GameObject($"Anchor_{anchorHandle}");
                                                    PXR_Plugin.MixedReality.UPxr_GetAnchorUuid(anchorHandle, out var uuid);
                                                    var spatialAnchor = anchorObject.AddComponent<PXR_SpatialAnchor>();
                                                    spatialAnchor.Created = true;
                                                    spatialAnchor.anchorHandle = anchorHandle;
                                                    spatialAnchor.anchorUuid = uuid;
                                                    spatialAnchorList.Add(anchorObject);
                                                }
                                            }
                                            PXR_Plugin.MixedReality.UPxr_DestroySenseDataQueryResult(completion.snapshotHandle);
                                            return (getResult, spatialAnchorList);
                                        }
                                        else
                                        {
                                            return (getResult, new List<GameObject>());
                                        }
                                    }
                                    else
                                    {
                                        return (completion.futureResult, new List<GameObject>());
                                    }
                                }
                                else
                                {
                                    return (completeResult, new List<GameObject>());
                                }
                            }
                        }
                        else
                        {
                            return (pollResult, new List<GameObject>());
                        }
                        await Task.Delay(11, token);
                    }
                }
                else
                {
                    return (queryResult, new List<GameObject>());
                }
            }, token);
        }

        /// <summary>
        /// Launches the Room Capture app to capture the current real-world scene.
        /// </summary>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details.</returns>
        public static async Task<PxrResult> StartSceneCaptureAsync(CancellationToken token = default)
        {
            return await Task.Run(async () =>
            {
                var startResult = PXR_Plugin.MixedReality.UPxr_StartSceneCaptureAsync(out var future);
                if (startResult == PxrResult.SUCCESS)
                {
                    while (true)
                    {
                        var pollResult = PXR_Plugin.MixedReality.UPxr_PollFuture(future, out var futureState);
                        if (pollResult == PxrResult.SUCCESS)
                        {
                            if (futureState == PxrFutureState.Ready)
                            {
                                var completeResult = PXR_Plugin.MixedReality.UPxr_StartSceneCaptureComplete(future, out var completion);
                                if (completeResult == PxrResult.SUCCESS)
                                {
                                    return completion.futureResult;
                                }
                                else
                                {
                                    return completeResult;
                                }
                            }
                        }
                        else
                        {
                            return pollResult;
                        }
                        await Task.Delay(11, token);
                    }
                }
                else
                {
                    return startResult;
                }
            }, token);
        }

        /// <summary>
        /// Loads scene anchors with specified semantic label(s).
        /// </summary>
        /// <param name="labels">Specifies the semantic label(s). If not specified, all scene anchors will be returned.</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details. In addition, a list of the handles of loaded anchors is returned.</returns>
        public static async Task<(PxrResult result, List<ulong> anchorHandleList)> QuerySceneAnchorAsync(PxrSemanticLabel[] labels = null, CancellationToken token = default)
        {
            return await Task.Run(async () =>
            {
                if (labels == null)
                {
                    labels = Array.Empty<PxrSemanticLabel>();
                }

                var queryResult = PXR_Plugin.MixedReality.UPxr_QuerySenseDataBySemanticAsync(labels, out var future);
                if (queryResult == PxrResult.SUCCESS)
                {
                    while (true)
                    {
                        var pollResult = PXR_Plugin.MixedReality.UPxr_PollFuture(future, out var futureState);
                        if (pollResult == PxrResult.SUCCESS)
                        {
                            if (futureState == PxrFutureState.Ready)
                            {
                                var completeResult = PXR_Plugin.MixedReality.UPxr_QuerySenseDataComplete(PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SceneCapture), future, out var completion);
                                if (completeResult == PxrResult.SUCCESS)
                                {
                                    if (completion.futureResult == PxrResult.SUCCESS)
                                    {
                                        var getResult = PXR_Plugin.MixedReality.UPxr_GetQueriedSenseData(PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SceneCapture), completion.snapshotHandle, out var entityInfos);
                                        if (getResult == PxrResult.SUCCESS)
                                        {
                                            var anchorHandleList = new List<ulong>();
                                            foreach (var e in entityInfos)
                                            {
                                                byte[] byteArray = new byte[16];
                                                BitConverter.GetBytes(e.uuid.value0).CopyTo(byteArray, 0);
                                                BitConverter.GetBytes(e.uuid.value1).CopyTo(byteArray, 8);
                                                Guid guid = new Guid(byteArray);
                                                anchorHandleList.Add(e.spatialEntity);
                                                var sceneAnchor = new PxrSceneComponentData
                                                {
                                                    uuid = guid
                                                };
                                                var result = PXR_Plugin.MixedReality.UPxr_GetSpatialEntitySemanticInfo(completion.snapshotHandle, e.spatialEntity, out var label);
                                                if (result == PxrResult.SUCCESS)
                                                {
                                                    sceneAnchor.label = label;
                                                }
                                                result = PXR_Plugin.MixedReality.UPxr_GetSpatialEntityLocationInfo(completion.snapshotHandle, e.spatialEntity, out var position, out var rotation);
                                                if (result == PxrResult.SUCCESS)
                                                {
                                                    sceneAnchor.position = position;
                                                    sceneAnchor.rotation = rotation;
                                                }
                                                result = PXR_Plugin.MixedReality.UPxr_EnumerateSpatialEntityComponentTypes(completion.snapshotHandle, e.spatialEntity, out var types);
                                                if (result == PxrResult.SUCCESS)
                                                {
                                                    sceneAnchor.types = types;
                                                }

                                                foreach (var t in types)
                                                {
                                                    switch (t)
                                                    {
                                                        case PxrSceneComponentType.Box3D:
                                                            {
                                                                result = PXR_Plugin.MixedReality.UPxr_GetSpatialEntityBox3DInfo(completion.snapshotHandle, e.spatialEntity, out var cPosition, out var cRotation, out var extent);
                                                                if (result == PxrResult.SUCCESS)
                                                                {
                                                                    sceneAnchor.box3D = new PxrSceneBox3D()
                                                                    {
                                                                        position = cPosition,
                                                                        rotation = cRotation,
                                                                        extent = extent
                                                                    };
                                                                }
                                                                break;
                                                            }
                                                        case PxrSceneComponentType.Box2D:
                                                            {
                                                                result = PXR_Plugin.MixedReality.UPxr_GetSpatialEntityBox2DInfo(completion.snapshotHandle, e.spatialEntity, out var offset, out var extent);
                                                                if (result == PxrResult.SUCCESS)
                                                                {
                                                                    sceneAnchor.box2D = new PxrSceneBox2D()
                                                                    {
                                                                        offset = offset,
                                                                        extent = new XrExtent2Df(extent)
                                                                    };
                                                                }
                                                                break;
                                                            }
                                                        case PxrSceneComponentType.Polygon:
                                                            {
                                                                result = PXR_Plugin.MixedReality.UPxr_GetSpatialEntityPolygonInfo(completion.snapshotHandle, e.spatialEntity, out var vertices);
                                                                if (result == PxrResult.SUCCESS)
                                                                {
                                                                    sceneAnchor.polygon = new PxrScenePolygon()
                                                                    {
                                                                        vertices = vertices
                                                                    };
                                                                }
                                                                break;
                                                            }
                                                    }
                                                }
                                                if (PXR_Plugin.MixedReality.SceneAnchorData.ContainsKey(e.spatialEntity))
                                                {
                                                    PXR_Plugin.MixedReality.SceneAnchorData[e.spatialEntity] = sceneAnchor;
                                                }
                                                else
                                                {
#if UNITY_2021_2_OR_NEWER
                                                    PXR_Plugin.MixedReality.SceneAnchorData.TryAdd(e.spatialEntity, sceneAnchor);
#else
                                                    PXR_Plugin.MixedReality.SceneAnchorData.Add(e.spatialEntity, sceneAnchor);
#endif
                                                }

                                            }
                                            PXR_Plugin.MixedReality.UPxr_DestroySenseDataQueryResult(completion.snapshotHandle);
                                            return (getResult, anchorHandleList);
                                        }
                                        else
                                        {
                                            return (getResult, new List<ulong>());
                                        }
                                    }
                                    else
                                    {
                                        return (completion.futureResult, new List<ulong>());
                                    }
                                }
                                else
                                {
                                    return (completeResult, new List<ulong>());
                                }
                            }
                        }
                        else
                        {
                            return (pollResult, new List<ulong>());
                        }

                        await Task.Delay(11, token);
                    }
                }
                else
                {
                    return (queryResult, new List<ulong>());
                }
            }, token);
        }

        /// <summary>
        /// Loads all scene anchors.
        /// </summary>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details. In addition, a list of the handles and UUIDs of all scene anchors is returned.</returns>
        public static async Task<(PxrResult result, Dictionary<ulong, Guid> anchorDictionary)> QuerySceneAnchorAsync(CancellationToken token = default)
        {
            return await Task.Run(async () =>
            {
                var queryResult = PXR_Plugin.MixedReality.UPxr_QuerySenseDataBySemanticAsync(Array.Empty<PxrSemanticLabel>(), out var future);
                if (queryResult == PxrResult.SUCCESS)
                {
                    while (true)
                    {
                        var pollResult = PXR_Plugin.MixedReality.UPxr_PollFuture(future, out var futureState);
                        if (pollResult == PxrResult.SUCCESS)
                        {
                            if (futureState == PxrFutureState.Ready)
                            {
                                var completeResult = PXR_Plugin.MixedReality.UPxr_QuerySenseDataComplete(PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SceneCapture), future, out var completion);
                                if (completeResult == PxrResult.SUCCESS)
                                {
                                    if (completion.futureResult == PxrResult.SUCCESS)
                                    {
                                        var getResult = PXR_Plugin.MixedReality.UPxr_GetQueriedSenseData(PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SceneCapture), completion.snapshotHandle, out var entityInfos);
                                        if (getResult == PxrResult.SUCCESS)
                                        {
                                            var anchorDictionary = new Dictionary<ulong, Guid>();
                                            PXR_Plugin.MixedReality.SceneAnchorData.Clear();
                                            PXR_Plugin.MixedReality.SceneAnchorData = new Dictionary<ulong, PxrSceneComponentData>();
                                            foreach (var e in entityInfos)
                                            {
                                                byte[] byteArray = new byte[16];
                                                BitConverter.GetBytes(e.uuid.value0).CopyTo(byteArray, 0);
                                                BitConverter.GetBytes(e.uuid.value1).CopyTo(byteArray, 8);
                                                Guid guid = new Guid(byteArray);
                                                anchorDictionary.Add(e.spatialEntity, guid);
                                                var sceneAnchor = new PxrSceneComponentData();
                                                var result = PXR_Plugin.MixedReality.UPxr_GetSpatialEntitySemanticInfo(completion.snapshotHandle, e.spatialEntity, out var label);
                                                if (result == PxrResult.SUCCESS)
                                                {
                                                    sceneAnchor.label = label;
                                                }
                                                result = PXR_Plugin.MixedReality.UPxr_GetSpatialEntityLocationInfo(completion.snapshotHandle, e.spatialEntity, out var position, out var rotation);
                                                if (result == PxrResult.SUCCESS)
                                                {
                                                    sceneAnchor.position = position;
                                                    sceneAnchor.rotation = rotation;
                                                }
                                                result = PXR_Plugin.MixedReality.UPxr_EnumerateSpatialEntityComponentTypes(completion.snapshotHandle, e.spatialEntity, out var types);
                                                if (result == PxrResult.SUCCESS)
                                                {
                                                    sceneAnchor.types = types;
                                                }

                                                foreach (var t in types)
                                                {
                                                    switch (t)
                                                    {
                                                        case PxrSceneComponentType.Box3D:
                                                            {
                                                                result = PXR_Plugin.MixedReality.UPxr_GetSpatialEntityBox3DInfo(completion.snapshotHandle, e.spatialEntity, out var cPosition, out var cRotation, out var extent);
                                                                if (result == PxrResult.SUCCESS)
                                                                {
                                                                    sceneAnchor.box3D = new PxrSceneBox3D()
                                                                    {
                                                                        position = cPosition,
                                                                        rotation = cRotation,
                                                                        extent = extent
                                                                    };
                                                                }
                                                                break;
                                                            }
                                                        case PxrSceneComponentType.Box2D:
                                                            {
                                                                result = PXR_Plugin.MixedReality.UPxr_GetSpatialEntityBox2DInfo(completion.snapshotHandle, e.spatialEntity, out var offset, out var extent);
                                                                if (result == PxrResult.SUCCESS)
                                                                {
                                                                    sceneAnchor.box2D = new PxrSceneBox2D()
                                                                    {
                                                                        offset = offset,
                                                                        extent = new XrExtent2Df(extent)
                                                                    };
                                                                }
                                                                break;
                                                            }
                                                        case PxrSceneComponentType.Polygon:
                                                            {
                                                                result = PXR_Plugin.MixedReality.UPxr_GetSpatialEntityPolygonInfo(completion.snapshotHandle, e.spatialEntity, out var vertices);
                                                                if (result == PxrResult.SUCCESS)
                                                                {
                                                                    sceneAnchor.polygon = new PxrScenePolygon()
                                                                    {
                                                                        vertices = vertices
                                                                    };
                                                                }
                                                                break;
                                                            }
                                                    }
                                                }
                                                if (PXR_Plugin.MixedReality.SceneAnchorData.ContainsKey(e.spatialEntity))
                                                {
                                                    PXR_Plugin.MixedReality.SceneAnchorData[e.spatialEntity] = sceneAnchor;
                                                }
                                                else
                                                {
#if UNITY_2021_2_OR_NEWER
                                                    PXR_Plugin.MixedReality.SceneAnchorData.TryAdd(e.spatialEntity, sceneAnchor);
#else
                                                    PXR_Plugin.MixedReality.SceneAnchorData.Add(e.spatialEntity, sceneAnchor);
#endif
                                                }
                                            }
                                            PXR_Plugin.MixedReality.UPxr_DestroySenseDataQueryResult(completion.snapshotHandle);
                                            return (getResult, anchorDictionary);
                                        }
                                        else
                                        {
                                            return (getResult, new Dictionary<ulong, Guid>());
                                        }
                                    }
                                    else
                                    {
                                        return (completion.futureResult, new Dictionary<ulong, Guid>());
                                    }
                                }
                                else
                                {
                                    return (completeResult, new Dictionary<ulong, Guid>());
                                }
                            }
                        }
                        else
                        {
                            return (pollResult, new Dictionary<ulong, Guid>());
                        }

                        await Task.Delay(11, token);
                    }
                }
                else
                {
                    return (queryResult, new Dictionary<ulong, Guid>());
                }
            }, token);

        }

        /// <summary>
        /// Gets the component type of a scene anchor.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the anchor to get component type for.</param>
        /// <param name="types">Returns the component type of the specified anchor.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details.</returns>
        public static PxrResult GetSceneAnchorComponentTypes(ulong anchorHandle, out PxrSceneComponentType[] types)
        {
            if (PXR_Plugin.MixedReality.SceneAnchorData.TryGetValue(anchorHandle, out var data))
            {
                types = data.types;
                return PxrResult.SUCCESS;
            }
            else
            {
                types = Array.Empty<PxrSceneComponentType>();
                return PxrResult.ERROR_HANDLE_INVALID;
            }
        }

        /// <summary>
        /// Gets the semantic label of a scene anchor.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the anchor to get semantic label for.</param>
        /// <param name="label">Returns the semantic label of the specified anchor.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details.</returns>
        public static PxrResult GetSceneSemanticLabel(ulong anchorHandle, out PxrSemanticLabel label)
        {
            if (PXR_Plugin.MixedReality.SceneAnchorData.TryGetValue(anchorHandle, out var data))
            {
                label = data.label;
                return PxrResult.SUCCESS;
            }
            else
            {
                label = PxrSemanticLabel.Unknown;
                return PxrResult.ERROR_HANDLE_INVALID;
            }
        }

        /// <summary>
        /// Gets the information of a 3D box object.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the scene anchor that the 3D box object is associated with.</param>
        /// <param name="position">Returns the position of the 3D box object relative to the center of the scene anchor.</param>
        /// <param name="rotation">Returns the rotation of the 3D box object relative to the center of the scene anchor.</param>
        /// <param name="extent">Returns the length, width, and height of the 3D box object.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details.</returns>
        public static PxrResult GetSceneBox3DData(ulong anchorHandle, out Vector3 position, out Quaternion rotation, out Vector3 extent)
        {
            if (PXR_Plugin.MixedReality.SceneAnchorData.TryGetValue(anchorHandle, out var data))
            {
                if (data.types.Contains(PxrSceneComponentType.Box3D))
                {
                    position = data.box3D.position;
                    rotation = data.box3D.rotation;
                    extent = data.box3D.extent;
                    return PxrResult.SUCCESS;
                }
                else
                {
                    position = Vector3.zero;
                    rotation = Quaternion.identity;
                    extent = Vector3.zero;
                    return PxrResult.ERROR_HANDLE_INVALID;
                }

            }
            else
            {
                position = Vector3.zero;
                rotation = Quaternion.identity;
                extent = Vector3.zero;
                return PxrResult.ERROR_HANDLE_INVALID;
            }
        }

        /// <summary>
        /// Gets the information of a 2D box object.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the scene anchor that the 2D box object is associated with.</param>
        /// <param name="offset">Returns the offset of the 2D box object relative to the center of the scene anchor.</param>
        /// <param name="extent">Returns the length and width of the 2D box object.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details.</returns>
        public static PxrResult GetSceneBox2DData(ulong anchorHandle, out Vector2 offset, out Vector2 extent)
        {
            if (PXR_Plugin.MixedReality.SceneAnchorData.TryGetValue(anchorHandle, out var data))
            {
                if (data.types.Contains(PxrSceneComponentType.Box2D))
                {
                    offset = data.box2D.offset;
                    extent = data.box2D.extent.ToVector2();
                    return PxrResult.SUCCESS;
                }
                else
                {
                    offset = Vector2.zero;
                    extent = Vector2.zero;
                    return PxrResult.ERROR_HANDLE_INVALID;
                }
            }
            else
            {
                offset = Vector2.zero;
                extent = Vector2.zero;
                return PxrResult.ERROR_HANDLE_INVALID;
            }
        }

        /// <summary>
        /// Gets the information of a polygon object.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the scene anchor that the polygon object is associated with.</param>
        /// <param name="vertices">Returns the array of vertices of the polygon object.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details.</returns>
        public static PxrResult GetScenePolygonData(ulong anchorHandle, out Vector2[] vertices)
        {
            if (PXR_Plugin.MixedReality.SceneAnchorData.TryGetValue(anchorHandle, out var data))
            {
                if (data.types.Contains(PxrSceneComponentType.Polygon))
                {
                    vertices = data.polygon.vertices;
                    return PxrResult.SUCCESS;
                }
                else
                {
                    vertices = Array.Empty<Vector2>();
                    return PxrResult.ERROR_HANDLE_INVALID;
                }
            }
            else
            {
                vertices = Array.Empty<Vector2>();
                return PxrResult.ERROR_HANDLE_INVALID;
            }
        }

        public static async Task<(PxrResult result, List<PxrSpatialMeshInfo> meshInfos)> QueryMeshAnchorAsync(CancellationToken token = default)
        {
            return await Task.Run(async () =>
            {
                XrSenseDataQueryInfo info = new XrSenseDataQueryInfo()
                {
                    type = XrStructureType.XR_TYPE_SENSE_DATA_QUERY_INFO,
                    filter = IntPtr.Zero
                };
                var pxrResult = PXR_Plugin.MixedReality.UPxr_QuerySenseDataAsync(PXR_Plugin.MixedReality.UPxr_GetSpatialMeshProviderHandle(), ref info, out var future);
                if (pxrResult == PxrResult.SUCCESS)
                {
                    while (true)
                    {
                        var pollResult = PXR_Plugin.MixedReality.UPxr_PollFuture(future, out var futureState);
                        if (pollResult == PxrResult.SUCCESS)
                        {
                            if (futureState == PxrFutureState.Ready)
                            {
                                var completeResult = PXR_Plugin.MixedReality.UPxr_QuerySenseDataComplete(PXR_Plugin.MixedReality.UPxr_GetSpatialMeshProviderHandle(), future, out var completion);
                                if (completeResult == PxrResult.SUCCESS)
                                {
                                    if (completion.futureResult == PxrResult.SUCCESS)
                                    {
                                        var getResult = PXR_Plugin.MixedReality.UPxr_GetQueriedSenseData(PXR_Plugin.MixedReality.UPxr_GetSpatialMeshProviderHandle(), completion.snapshotHandle, out var entityHandles);
                                        if (getResult == PxrResult.SUCCESS)
                                        {
                                            var keysToRemove = PXR_Plugin.MixedReality.SpatialMeshData
                                                .Where(pair => pair.Value.state == MeshChangeState.Removed)
                                                .Select(pair => pair.Key)
                                                .ToList();
                                            foreach (var key in keysToRemove)
                                            {
                                                PXR_Plugin.MixedReality.SpatialMeshData.Remove(key);
                                            }
                                            var toBeRemove = new List<Guid>(PXR_Plugin.MixedReality.meshAnchorLastData.Keys);
                                            foreach (var e in entityHandles)
                                            {
                                                byte[] byteArray = new byte[16];
                                                BitConverter.GetBytes(e.uuid.value0).CopyTo(byteArray, 0);
                                                BitConverter.GetBytes(e.uuid.value1).CopyTo(byteArray, 8);
                                                Guid guid = new Guid(byteArray);
                                                var item = new PxrSpatialMeshInfo()
                                                {
                                                    uuid = guid,
                                                };
                                                toBeRemove.Remove(guid);

                                                if (PXR_Plugin.MixedReality.meshAnchorLastData.TryGetValue(guid, out var lastTime))
                                                {
                                                    if (lastTime < e.time)
                                                    {
                                                        var result = PXR_Plugin.MixedReality.UPxr_GetSpatialMesh(completion.snapshotHandle, e.spatialEntity, ref item);
                                                        if (result == PxrResult.SUCCESS)
                                                        {
                                                            item.state = MeshChangeState.Updated;
                                                            PXR_Plugin.MixedReality.SpatialMeshData[guid] = item;
                                                        }
                                                        PXR_Plugin.MixedReality.meshAnchorLastData[guid] = e.time;
                                                    }
                                                    else
                                                    {
                                                        var tempMesh = PXR_Plugin.MixedReality.SpatialMeshData[guid];
                                                        tempMesh.state = MeshChangeState.Unchanged;
                                                        PXR_Plugin.MixedReality.SpatialMeshData[guid] = tempMesh;
                                                    }
                                                }
                                                else
                                                {
                                                    var result = PXR_Plugin.MixedReality.UPxr_GetSpatialMesh(completion.snapshotHandle, e.spatialEntity, ref item);
                                                    if (result == PxrResult.SUCCESS)
                                                    {
                                                        item.state = MeshChangeState.Added;
#if UNITY_2021_2_OR_NEWER
                                                        PXR_Plugin.MixedReality.SpatialMeshData.TryAdd(guid, item);
#else
                                                        PXR_Plugin.MixedReality.SpatialMeshData.Add(guid, item);
#endif
                                                    }
#if UNITY_2021_2_OR_NEWER
                                                    PXR_Plugin.MixedReality.meshAnchorLastData.TryAdd(guid, e.time);
#else
                                                    PXR_Plugin.MixedReality.meshAnchorLastData.Add(guid, e.time);
#endif
                                                }
                                            }

                                            foreach (var m in toBeRemove)
                                            {
                                                PXR_Plugin.MixedReality.meshAnchorLastData.Remove(m);
                                                PXR_Plugin.MixedReality.SpatialMeshData.Remove(m);
                                                var removedMesh = new PxrSpatialMeshInfo()
                                                {
                                                    uuid = m,
                                                    state = MeshChangeState.Removed
                                                };
#if UNITY_2021_2_OR_NEWER
                                                PXR_Plugin.MixedReality.SpatialMeshData.TryAdd(m, removedMesh);
#else
                                                PXR_Plugin.MixedReality.SpatialMeshData.Add(m, removedMesh);
#endif
                                            }

                                            PXR_Plugin.MixedReality.UPxr_DestroySenseDataQueryResult(completion.snapshotHandle);
                                            return (getResult, PXR_Plugin.MixedReality.SpatialMeshData.Values.ToList());
                                        }
                                        else
                                        {
                                            return (getResult, new List<PxrSpatialMeshInfo>());
                                        }

                                    }
                                    else
                                    {
                                        return (completion.futureResult, new List<PxrSpatialMeshInfo>());
                                    }
                                }
                                else
                                {
                                    return (completeResult, new List<PxrSpatialMeshInfo>());
                                }
                            }
                        }
                        else
                        {
                            return (pollResult, new List<PxrSpatialMeshInfo>());
                        }

                        await Task.Delay(11, token);
                    }
                }
                else
                {
                    return (pxrResult, new List<PxrSpatialMeshInfo>());
                }
            }, token);
        }

        /// <summary>
        /// Uploads a spatial anchor to the cloud. The spatial anchor then becomes a shared spatial anchor, which can be downloaded and used by others.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the anchor to upload.</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details. In addition, the handle and UUID of the shared spatial anchor are returned.</returns>
        public static async Task<(PxrResult result, Guid uuid)> UploadSpatialAnchorAsync(ulong anchorHandle, CancellationToken token = default)
        {
            return await Task.Run(async () =>
            {
                var startResult = PXR_Plugin.MixedReality.UPxr_ShareSpatialAnchorAsync(anchorHandle, out var future);
                if (startResult == PxrResult.SUCCESS)
                {
                    while (true)
                    {
                        var pollResult = PXR_Plugin.MixedReality.UPxr_PollFuture(future, out var futureState);
                        if (pollResult == PxrResult.SUCCESS)
                        {
                            if (futureState == PxrFutureState.Ready)
                            {
                                var completeResult = PXR_Plugin.MixedReality.UPxr_ShareSpatialAnchorComplete(future, out var completion);
                                if (completeResult == PxrResult.SUCCESS)
                                {
                                    if (completion.futureResult == PxrResult.SUCCESS)
                                    {
                                        var getResult = GetAnchorUuid(anchorHandle, out var uuid);
                                        if (getResult == PxrResult.SUCCESS)
                                        {
                                            return (PxrResult.SUCCESS, uuid);
                                        }
                                        else
                                        {
                                            return (getResult, Guid.Empty);
                                        }
                                    }
                                    else
                                    {
                                        return (completion.futureResult, Guid.Empty);
                                    }
                                }
                                else
                                {
                                    return (completeResult, Guid.Empty);
                                }
                            }
                        }
                        else
                        {
                            return (pollResult, Guid.Empty);
                        }

                        await Task.Delay(11, token);
                    }
                }
                else
                {
                    return (startResult, Guid.Empty);
                }
            }, token);
        }

        /// <summary>
        /// Uploads a spatial anchor to the cloud with progress reporting. The spatial anchor then becomes a shared spatial anchor, which can be downloaded and used by others.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the local spatial anchor to upload.</param>
        /// <param name="progressUpdated">Callback for progress updates (0-100). Pass an action to receive the progress updates.</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details. In addition, the handle and UUID of the shared spatial anchor are returned.</returns>
        public static async Task<(PxrResult result, Guid uuid)> UploadSpatialAnchorWithProgressAsync(ulong anchorHandle, Action<int> progressUpdated, CancellationToken token = default)
        {
            return await Task.Run(async () =>
            {
                var startResult = PXR_Plugin.MixedReality.UPxr_ShareSpatialAnchorAsync(anchorHandle, out var future);
                if (startResult == PxrResult.SUCCESS)
                {
                    while (true)
                    {
                        var pollResult = PXR_Plugin.MixedReality.UPxr_PollFutureWithProgress(future, out var futureState, out var progressValue);
                        if (pollResult == PxrResult.SUCCESS)
                        {
                            progressUpdated?.Invoke(progressValue);
                            if (futureState == PxrFutureState.Ready)
                            {
                                var completeResult = PXR_Plugin.MixedReality.UPxr_ShareSpatialAnchorComplete(future, out var completion);
                                if (completeResult == PxrResult.SUCCESS)
                                {
                                    if (completion.futureResult == PxrResult.SUCCESS)
                                    {
                                        var getResult = GetAnchorUuid(anchorHandle, out var uuid);
                                        if (getResult == PxrResult.SUCCESS)
                                        {
                                            return (PxrResult.SUCCESS, uuid);
                                        }
                                        else
                                        {
                                            return (getResult, Guid.Empty);
                                        }
                                    }
                                    else
                                    {
                                        return (completion.futureResult, Guid.Empty);
                                    }
                                }
                                else
                                {
                                    return (completeResult, Guid.Empty);
                                }
                            }
                        }
                        else
                        {
                            return (pollResult, Guid.Empty);
                        }
                        await Task.Delay(11, token);
                    }
                }
                else
                {
                    return (startResult, Guid.Empty);
                }
            }, token);
        }


        /// <summary>
        /// Downloads a shared spatial anchor from cloud.
        /// </summary>
        /// <param name="uuid">Specifies the UUID of the shared spatial anchor to download.</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details. In addition, the handle and UUID of the downloaded shared spatial anchor are returned.</returns>
        public static async Task<PxrResult> DownloadSharedSpatialAnchorAsync(Guid uuid, CancellationToken token = default)
        {
            return await Task.Run(async () =>
            {
                var startResult = PXR_Plugin.MixedReality.UPxr_DownloadSharedSpatialAnchorsAsync(uuid, out var future);
                if (startResult == PxrResult.SUCCESS)
                {
                    while (true)
                    {
                        var pollResult = PXR_Plugin.MixedReality.UPxr_PollFuture(future, out var futureState);
                        if (pollResult == PxrResult.SUCCESS)
                        {
                            if (futureState == PxrFutureState.Ready)
                            {
                                var completeResult = PXR_Plugin.MixedReality.UPxr_DownloadSharedSpatialAnchorsComplete(future, out var completion);
                                if (completeResult == PxrResult.SUCCESS)
                                {
                                    return completion.futureResult;
                                }
                                else
                                {
                                    return completeResult;
                                }
                            }
                        }
                        else
                        {
                            return pollResult;
                        }
                        await Task.Delay(11, token);
                    }
                }
                else
                {
                    return startResult;
                }
            }, token);
        }

        /// <summary>
        /// Downloads a shared spatial anchor with progress reporting.
        /// </summary>
        /// <param name="uuid">Specifies the unique identifier of the shared spatial anchor to download.</param>
        /// <param name="progressUpdated">Callback for progress updates (0-100). Pass an action to receive the progress updates.</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        /// <returns>Refer to the `PxrResult` enumeration for details. In addition, the handle and UUID of the downloaded shared spatial anchor are returned.</returns>
        public static async Task<PxrResult> DownloadSharedSpatialAnchorWithProgressAsync(Guid uuid, Action<int> progressUpdated, CancellationToken token = default)
        {
            return await Task.Run(async () =>
            {
                var startResult = PXR_Plugin.MixedReality.UPxr_DownloadSharedSpatialAnchorsAsync(uuid, out var future);
                if (startResult == PxrResult.SUCCESS)
                {
                    while (true)
                    {
                        var pollResult = PXR_Plugin.MixedReality.UPxr_PollFutureWithProgress(future, out var futureState, out var progressValue);
                        if (pollResult == PxrResult.SUCCESS)
                        {
                            progressUpdated?.Invoke(progressValue);
                            if (futureState == PxrFutureState.Ready)
                            {
                                var completeResult = PXR_Plugin.MixedReality.UPxr_DownloadSharedSpatialAnchorsComplete(future, out var completion);
                                if (completeResult == PxrResult.SUCCESS)
                                {
                                    return completion.futureResult;
                                }
                                else
                                {
                                    return completeResult;
                                }
                            }
                        }
                        else
                        {
                            return pollResult;
                        }
                        await Task.Delay(11, token);
                    }
                }
                else
                {
                    return startResult;
                }
            }, token);
        }

        [Obsolete("Please use UploadSpatialAnchorAsync")]
        public static async Task<(PxrResult result, Guid uuid)> ShareSpatialAnchorAsync(ulong anchorHandle)
        {
            return await UploadSpatialAnchorAsync(anchorHandle);
        }

        #region 2.0 API Only Support PICO 4
        /// <summary>
        /// Creates an anchor entity in the app's memory. Should listen to the `PxrEventAnchorEntityCreated` event which returns the handle and UUID of the anchor.
        /// </summary>
        /// <param name="position">Sets the he position of the anchor entity.</param>
        /// <param name="rotation">Sets the orientation of the anchor entity.</param>
        /// <param name="taskId">Returns the ID of this task.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static PxrResult CreateAnchorEntity(Vector3 position, Quaternion rotation, out ulong taskId)
        {

            taskId = 0;
            return PxrResult.ERROR_RUNTIME_FAILURE;
        }

        /// <summary>
        /// Destroys an anchor entity in the app's memory.
        /// </summary>
        /// <param name="handle">Specifies the handle of the to-be-destroyed anchor entity.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static PxrResult DestroyAnchorEntity(ulong handle)
        {
            return PxrResult.ERROR_RUNTIME_FAILURE;
        }

        /// <summary>
        /// Gets the pose of an anchor entity.
        /// </summary>
        /// <param name="handle">Specifies the handle of the anchor entity to get pose for.</param>
        /// <param name="orientation">Returns the orientation of the anchor entity.</param>
        /// <param name="position">Returns the position of the anchor entity.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static PxrResult GetAnchorPose(ulong handle, out Quaternion orientation, out Vector3 position)
        {
            orientation = Quaternion.identity;
            position = Vector3.zero;
            return PxrResult.ERROR_RUNTIME_FAILURE;
        }

        /// <summary>
        /// Gets the universally unique identifier (UUID) of an anchor entity.
        /// </summary>
        /// <param name="handle">Specifies the handle of the anchor entity to get UUID for.</param>
        /// <param name="uuid">Returns the UUID of the anchor entity.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static PxrResult GetAnchorEntityUuid(ulong handle, out Guid uuid)
        {
            uuid = new Guid();
            return PxrResult.ERROR_RUNTIME_FAILURE;
        }

        /// <summary>
        /// Persists specified anchor entities, which means saving anchor entities to a specified location.
        /// Currently, only supports saving anchor entities to the device's local storage.
        /// </summary>
        /// <param name="anchorHandles">Specifies the handles of the to-be-persisted anchor entities.</param>
        /// <param name="location">The location that the anchor entities are saved to:
        /// * `Local`: device's local storage
        /// * `Remote` (not supported)
        /// </param>
        /// <param name="taskId">Returns the ID of the task.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static PxrResult PersistAnchorEntity(ulong[] anchorHandles, PxrPersistLocation location, out ulong taskId)
        {
            taskId = 0;
            return PxrResult.ERROR_RUNTIME_FAILURE;
        }

        /// <summary>
        /// Unpersists specified anchor entities, which means deleting anchor entities from the location where they are saved.
        /// Currently, only supports deleting anchor entities saved in the device's local storage.
        /// Should listen to the `PxrEventAnchorEntityUnPersisted` event.
        /// </summary>
        /// <param name="anchorHandles">Specifies the handles of the to-be-unpersisted anchor entities.</param>
        /// <param name="location">Specifies the location where the anchor entities are saved:
        /// * `Local`: device's local storage
        /// * `Remote`: (not supported)
        /// </param>
        /// <param name="taskId">Returns the ID of the task.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static PxrResult UnPersistAnchorEntity(ulong[] anchorHandles, PxrPersistLocation location, out ulong taskId)
        {
            taskId = 0;
            return PxrResult.ERROR_RUNTIME_FAILURE;
        }

        /// <summary>
        /// Clears all anchor entities saved in a specified location.
        /// Currently, only supports deleting all anchor entities saved in the device's local storage.
        /// Should listen to the `PxrEventAnchorEntityCleared` event.
        /// </summary>
        /// <param name="location">Specifies the location where the to-be-cleared anchor entities are saved. Currently, only supports passing `Local` to clear the anchor entities stored in the device's local storage.</param>
        /// <param name="taskId">Returns the ID of the task.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static PxrResult ClearPersistedAnchorEntity(PxrPersistLocation location, out ulong taskId)
        {
            taskId = 0;
            return PxrResult.ERROR_RUNTIME_FAILURE;
        }

        /// <summary>
        /// Gets the components supported by an anchor entity.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the anchor entity to get supported components for.</param>
        /// <param name="flags">Returns the flags of the supported components.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static PxrResult GetAnchorComponentFlags(ulong anchorHandle, out PxrAnchorComponentTypeFlags[] flags)
        {
            List<PxrAnchorComponentTypeFlags> flagList = new List<PxrAnchorComponentTypeFlags>();
            flags = flagList.ToArray();
            return PxrResult.ERROR_RUNTIME_FAILURE;
        }

        /// <summary>
        /// Loads anchor entities by UUIDs. If no UUID is passed, all anchor entities will be loaded.
        /// Before calling this method, call `GetAnchorEntityUuid` first to get the UUIDs of anchor entities.
        /// Should listen to the `PxrEventAnchorEntityLoaded` event.
        /// </summary>
        /// <param name="taskId">Returns the ID of the task.</param>
        /// <param name="uuids">Specifies The UUIDs of the anchor entities to load.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static PxrResult LoadAnchorEntityByUuidFilter(out ulong taskId, Guid[] uuids = null)
        {
            taskId = 0;
            return PxrResult.ERROR_RUNTIME_FAILURE;
        }

        /// <summary>
        /// Loads anchor entities by scene data types. As one anchor entity can only have one scene date type, this method loads the anchor entities that supports one of the scene data types you specify.
        /// For example, if you pass `Floor` and `Ceiling` in the request, anchor entities supporting the `Floor` or `Ceiling` scene data type will be loaded.
        /// </summary>
        /// <param name="flags">Specifies the flags of scene data types.</param>
        /// <param name="taskId">Returns the ID of the task.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static PxrResult LoadAnchorEntityBySceneFilter(PxrSpatialSceneDataTypeFlags[] flags, out ulong taskId)
        {
            taskId = 0;
            return PxrResult.ERROR_RUNTIME_FAILURE;
        }

        /// <summary>
        /// Gets the result of the task of loading anchor entities.
        /// </summary>
        /// <param name="taskId">Specifies the ID of the task to get result for. You can get the task ID from the `PxrEventAnchorEntityLoaded` struct.</param>
        /// <param name="count">Returns the number of anchor entities successfully loaded.</param>
        /// <param name="loadedAnchors">Returns the handles and UUIDs of the anchor entities loaded.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static PxrResult GetAnchorEntityLoadResults(ulong taskId, uint count, out Dictionary<ulong, Guid> loadedAnchors)
        {
            loadedAnchors = new Dictionary<ulong, Guid>();
            return PxrResult.ERROR_RUNTIME_FAILURE;
        }

        /// <summary>
        /// Launches the Room Capture app to calibrate the room.
        /// </summary>
        /// <param name="taskId">Returns the ID of the task.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static PxrResult StartSpatialSceneCapture(out ulong taskId)
        {
            taskId = 0;
            return PxrResult.ERROR_RUNTIME_FAILURE;
        }

        /// <summary>
        /// Gets the information about the volume for an anchor entity.
        /// Before calling this method, you need to load anchor entities and get the anchor entity load result first. The result contains the handles of anchor entities loaded.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the anchor entity.</param>
        /// <param name="center">Returns the offset of the volume's position relative to the anchor entity's position.</param>
        /// <param name="extent">Returns the length, width, and height of the volume.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static PxrResult GetAnchorVolumeInfo(ulong anchorHandle, out Vector3 center, out Vector3 extent)
        {
            center = Vector3.zero;
            extent = Vector3.zero;
            return PxrResult.ERROR_RUNTIME_FAILURE;
        }

        /// <summary>
        /// Gets the information about the polygon (irregular plane) for an anchor entity.
        /// Before calling this method, you need to load anchor entities and get the anchor entity load result first. The result contains the handles of anchor entities loaded.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the anchor entity.</param>
        /// <param name="vertices">Returns the positions of the polygon's vertices on the X, Y, and Z axis.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static PxrResult GetAnchorPlanePolygonInfo(ulong anchorHandle, out Vector3[] vertices)
        {
            vertices = new Vector3[0];
            return PxrResult.ERROR_RUNTIME_FAILURE;
        }

        /// <summary>
        /// Gets the information about the boundary (rectangle) for an anchor entity.
        /// Before calling this method, you need to load anchor entities and get the anchor entity load result first. The result contains the handles and UUIDs of anchor entities loaded.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the anchor entity.</param>
        /// <param name="center">Returns the offset of the boundary's position relative to the anchor entity's position.</param>
        /// <param name="extent">Returns the width and height of the boundary.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static PxrResult GetAnchorPlaneBoundaryInfo(ulong anchorHandle, out Vector3 center, out Vector2 extent)
        {
            center = Vector3.zero;
            extent = Vector2.zero;
            return PxrResult.ERROR_RUNTIME_FAILURE;
        }

        /// <summary>
        /// Gets the scene label of an anchor entity.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the anchor entity.</param>
        /// <param name="label">Returns the anchor entity's scene label.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        [Obsolete("Deprecated.Only Support PICO 4.")]
        public static PxrResult GetAnchorSceneLabel(ulong anchorHandle, out PxrSceneLabel label)
        {
            label = PxrSceneLabel.UnKnown;
            return PxrResult.ERROR_RUNTIME_FAILURE;
        }

        #endregion

        /// <summary>
        /// Enables/disables video seethrough.
        /// </summary>
        /// <param name="state">Specifies whether to enable or disable video seethrough:
        /// * `true`: enable
        /// * `false`: disable
        /// </param>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        /// <see cref="PXR_Manager.EnableVideoSeeThrough"/> is preferred over this method.
        [Obsolete("Deprecated.Please use PXR_Manager.EnableVideoSeeThrough instead", true)]
        public static int EnableVideoSeeThrough(bool state)
        {
            return -1;
        }

        /// <summary>
        /// Enables/disables video seethrough effect.
        /// </summary>
        /// <param name="value">Specifies whether to enable or disable video seethrough effect:
        /// * `true`: enable
        /// * `false`: disable
        /// </param>
        /// <returns>`0` indicates a success and other values indicate a failure.</returns>
        public static int EnableVideoSeeThroughEffect(bool value)
        {
            return PXR_Plugin.MixedReality.UPxr_EnableVideoSeeThroughEffect(value);
        }

        /// <summary>
        /// Sets video seethrough effect-related parameters.
        /// </summary>
        /// <param name="type">Specifies the parameter to set. You can set contrast, saturation, brightness, and colormap.</param>
        /// <param name="value">Specifies the final value that the current value of the parameter changes to. Value range: [-50,50]. The default value is `0`.</param>
        /// <param name="duration">Specifies the duration during which the current value of the specified parameter gradually changes to the specified value. If the duration is set to `0`, the current value of the specified parameter immediately changes to the specified value.</param>
        /// <returns>`0` indicates a success and other values indicate a failure.</returns>
        public static int SetVideoSeeThroughEffect(PxrLayerEffect type, float value, float duration)
        {
            return PXR_Plugin.MixedReality.UPxr_SetVideoSeeThroughEffect(type, value, duration);
        }

        /// <summary>
        /// Sets a LUT texture for video seethrough.
        /// </summary>
        /// <param name="texture">Passes a standard LUT texture. The size of the texture should not exceed 512*512 pixels. The imported LUT texture needs to be converted to the RGBA32 format in order to obtain the corresponding RGBA values correctly.</param>
        /// <param name="row">Specifies the number of rows in the LUT texture.</param>
        /// <param name="col">Specifies the number of columns in the LUT texture.</param>
        /// <returns>`0` indicates a success and other values indicate a failure.</returns>
        public static int SetVideoSeeThroughLut(Texture2D texture, int row, int col)
        {
            if (texture.format != TextureFormat.RGBA32)
            {
                PLog.e(TAG, "Unsupported texture format! Please provide a texture in RGBA32 format!");
                return -1;
            }

            if (texture.width > 512 || texture.height > 512)
            {
                PLog.e(TAG, "The texture size must not exceed 512x512 pixels!");
                return -1;
            }

            var data = texture.GetRawTextureData();
            return PXR_Plugin.MixedReality.UPxr_SetVideoSeeThroughLUT(ref data, texture.width, texture.height, row, col);
        }
    }
}