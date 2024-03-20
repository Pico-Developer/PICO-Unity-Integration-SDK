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
using UnityEngine;

namespace Unity.XR.PXR
{
    public class PXR_MixedReality
    {
        #region New API
        /// <summary>
        /// Creates an anchor entity in the app's memory. Should listen to the `PxrEventAnchorEntityCreated` event which returns the handle and UUID of the anchor.
        /// </summary>
        /// <param name="position">Sets the he position of the anchor entity.</param>
        /// <param name="rotation">Sets the orientation of the anchor entity.</param>
        /// <param name="taskId">Returns the ID of this task.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        public static PxrResult CreateAnchorEntity(Vector3 position, Quaternion rotation, out ulong taskId)
        {
            PXR_System.GetTrackingOrigin(out var originMode);
            PxrAnchorEntityCreateInfo info = new PxrAnchorEntityCreateInfo()
            {
                origin = originMode,
                pose = new PxrPosef()
                {
                    orientation = new PxrVector4f()
                    {
                        x = rotation.x,
                        y = rotation.y,
                        z = -rotation.z,
                        w = -rotation.w
                    },
                    position = new PxrVector3f()
                    {
                        x = position.x,
                        y = position.y,
                        z = -position.z
                    }
                },
                time = PXR_Plugin.System.UPxr_GetPredictedDisplayTime()
            };
            var result =  PXR_Plugin.MixedReality.UPxr_CreateAnchorEntity(ref info,out taskId);
            return result;
        }

        /// <summary>
        /// Destroys an anchor entity in the app's memory.
        /// </summary>
        /// <param name="handle">Specifies the handle of the to-be-destroyed anchor entity.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        public static PxrResult DestroyAnchorEntity(ulong handle)
        {
            PxrAnchorEntityDestroyInfo info = new PxrAnchorEntityDestroyInfo()
            {
                anchorHandle = handle
            };
            return PXR_Plugin.MixedReality.UPxr_DestroyAnchorEntity(ref info);
        }

        /// <summary>
        /// Gets the pose of an anchor entity.
        /// </summary>
        /// <param name="handle">Specifies the handle of the anchor entity to get pose for.</param>
        /// <param name="orientation">Returns the orientation of the anchor entity.</param>
        /// <param name="position">Returns the position of the anchor entity.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        public static PxrResult GetAnchorPose(ulong handle,out Quaternion orientation,out Vector3 position)
        {
            PXR_System.GetTrackingOrigin(out var originMode);
            var result = PXR_Plugin.MixedReality.UPxr_GetAnchorPose(handle, originMode, out var pose);
            orientation = new Quaternion(pose.orientation.x, pose.orientation.y, - pose.orientation.z, - pose.orientation.w);
            position = new Vector3(pose.position.x, pose.position.y, - pose.position.z);
            return result;
        }

        /// <summary>
        /// Gets the universally unique identifier (UUID) of an anchor entity.
        /// </summary>
        /// <param name="handle">Specifies the handle of the anchor entity to get UUID for.</param>
        /// <param name="uuid">Returns the UUID of the anchor entity.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        public static PxrResult GetAnchorEntityUuid(ulong handle, out Guid uuid)
        {
            var result = PXR_Plugin.MixedReality.UPxr_GetAnchorEntityUuid(handle, out var pUid);
            byte[] byteArray = new byte[16];
            BitConverter.GetBytes(pUid.value0).CopyTo(byteArray,0);
            BitConverter.GetBytes(pUid.value1).CopyTo(byteArray, 8);
            uuid = new Guid(byteArray);
            return result;
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
        public static PxrResult PersistAnchorEntity(ulong[] anchorHandles, PxrPersistLocation location,out ulong taskId)
        {
            PxrAnchorEntityPersistInfo info = new PxrAnchorEntityPersistInfo()
            {
                anchorList = new PxrAnchorEntityList()
                {
                    count = (uint)anchorHandles.Length,
                    anchorHandles = new IntPtr(0)
                },
                location = location
            };
            
            info.anchorList.anchorHandles = Marshal.AllocHGlobal(anchorHandles.Length * Marshal.SizeOf(typeof(ulong)));
            long[] tmpHandles = Array.ConvertAll(anchorHandles,x=>(long)x);
            Marshal.Copy(tmpHandles, 0, info.anchorList.anchorHandles, anchorHandles.Length);
            var result =  PXR_Plugin.MixedReality.UPxr_PersistAnchorEntity(ref info,out taskId);
            Marshal.FreeHGlobal(info.anchorList.anchorHandles);
            return result;
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
        public static PxrResult UnPersistAnchorEntity(ulong[] anchorHandles, PxrPersistLocation location, out ulong taskId)
        {
            PxrAnchorEntityUnPersistInfo info = new PxrAnchorEntityUnPersistInfo()
            {
                anchorList = new PxrAnchorEntityList()
                {
                    count = (uint)anchorHandles.Length,
                    anchorHandles = new IntPtr(0)
                },
                location = location
            };
            info.anchorList.anchorHandles = Marshal.AllocHGlobal(anchorHandles.Length * Marshal.SizeOf(typeof(ulong)));
            long[] tmpHandles = Array.ConvertAll(anchorHandles, x => (long)x);
            Marshal.Copy(tmpHandles, 0, info.anchorList.anchorHandles, anchorHandles.Length);
            var result = PXR_Plugin.MixedReality.UPxr_UnpersistAnchorEntity(ref info, out taskId);
            Marshal.FreeHGlobal(info.anchorList.anchorHandles);
            return result;
        }

        /// <summary>
        /// Clears all anchor entities saved in a specified location.
        /// Currently, only supports deleting all anchor entities saved in the device's local storage.
        /// Should listen to the `PxrEventAnchorEntityCleared` event.
        /// </summary>
        /// <param name="location">Specifies the location where the to-be-cleared anchor entities are saved. Currently, only supports passing `Local` to clear the anchor entities stored in the device's local storage.</param>
        /// <param name="taskId">Returns the ID of the task.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        public static PxrResult ClearPersistedAnchorEntity(PxrPersistLocation location, out ulong taskId)
        {
            PxrAnchorEntityClearInfo info = new PxrAnchorEntityClearInfo()
            {
                location = location,
            };
            return PXR_Plugin.MixedReality.UPxr_ClearPersistedAnchorEntity(ref info, out taskId);
        }

        /// <summary>
        /// Gets the components supported by an anchor entity.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the anchor entity to get supported components for.</param>
        /// <param name="flags">Returns the flags of the supported components.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        public static PxrResult GetAnchorComponentFlags(ulong anchorHandle,out PxrAnchorComponentTypeFlags[] flags)
        {
            List<PxrAnchorComponentTypeFlags> flagList = new List<PxrAnchorComponentTypeFlags>();
            var result = PXR_Plugin.MixedReality.UPxr_GetAnchorComponentFlags(anchorHandle, out var flag);

            foreach (PxrAnchorComponentTypeFlags value in Enum.GetValues(typeof(PxrAnchorComponentTypeFlags)))
            {
                if ((flag & (ulong)value) != 0)
                    flagList.Add(value);
            }

            flags = flagList.ToArray();
            return result;
        }

        /// <summary>
        /// Loads anchor entities by UUIDs. If no UUID is passed, all anchor entities will be loaded.
        /// Before calling this method, call `GetAnchorEntityUuid` first to get the UUIDs of anchor entities.
        /// Should listen to the `PxrEventAnchorEntityLoaded` event.
        /// </summary>
        /// <param name="taskId">Returns the ID of the task.</param>
        /// <param name="uuids">Specifies The UUIDs of the anchor entities to load.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        public static PxrResult LoadAnchorEntityByUuidFilter(out ulong taskId, Guid[] uuids = null)
        {
            PxrAnchorEntityLoadInfo info = new PxrAnchorEntityLoadInfo()
            {
                maxResult = 1024,
                timeout = 0,
                location = PxrPersistLocation.Local,
                include = IntPtr.Zero,
                exclude = IntPtr.Zero
            };

            PxrAnchorEntityLoadUuidFilter filter = new PxrAnchorEntityLoadUuidFilter()
            {
                type = PxrStructureType.AnchorEntityLoadUuidFilter,
                uuidCount = 0,
                uuidList = IntPtr.Zero
            };
            if (uuids != null)
            {
                filter.uuidCount = (uint)uuids.Length;
                filter.uuidList = Marshal.AllocHGlobal(uuids.Length * Marshal.SizeOf(typeof(Guid)));
                byte[] bytes = uuids.SelectMany(g => g.ToByteArray()).ToArray();
                Marshal.Copy(bytes, 0,filter.uuidList, uuids.Length * Marshal.SizeOf(typeof(Guid)));
            }
            
            int size = Marshal.SizeOf<PxrAnchorEntityLoadUuidFilter>();
            info.include = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(filter, info.include, false);
            var result = PXR_Plugin.MixedReality.UPxr_LoadAnchorEntity(ref info, out taskId);
            Marshal.FreeHGlobal(filter.uuidList);
            return result;
        }

        /// <summary>
        /// Loads anchor entities by scene data types. As one anchor entity can only have one scene date type, this method loads the anchor entities that supports one of the scene data types you specify.
        /// For example, if you pass `Floor` and `Ceiling` in the request, anchor entities supporting the `Floor` or `Ceiling` scene data type will be loaded.
        /// </summary>
        /// <param name="flags">Specifies the flags of scene data types.</param>
        /// <param name="taskId">Returns the ID of the task.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        public static PxrResult LoadAnchorEntityBySceneFilter(PxrSpatialSceneDataTypeFlags[] flags, out ulong taskId)
        {
            PxrAnchorEntityLoadInfo info = new PxrAnchorEntityLoadInfo()
            {
                maxResult = 1024,
                timeout = 0,
                location = PxrPersistLocation.Local,
                include = IntPtr.Zero,
                exclude = IntPtr.Zero
            };
            ulong mask = 0;
            foreach (var flag in flags)
            {
                mask |= (ulong)flag;
            }

            PxrAnchorEntityLoadSpatialSceneFilter filter = new PxrAnchorEntityLoadSpatialSceneFilter()
            {
                type = PxrStructureType.AnchorEntityLoadSpatialSceneFilter,
                typeFlags = mask
            };
            int size = Marshal.SizeOf<PxrAnchorEntityLoadSpatialSceneFilter>();
            info.include = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(filter, info.include, false);
            return PXR_Plugin.MixedReality.UPxr_LoadAnchorEntity(ref info, out taskId);
        }

        /// <summary>
        /// Gets the result of the task of loading anchor entities.
        /// </summary>
        /// <param name="taskId">Specifies the ID of the task to get result for. You can get the task ID from the `PxrEventAnchorEntityLoaded` struct.</param>
        /// <param name="count">Returns the number of anchor entities successfully loaded.</param>
        /// <param name="loadedAnchors">Returns the handles and UUIDs of the anchor entities loaded.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        public static PxrResult GetAnchorEntityLoadResults(ulong taskId, uint count, out Dictionary<ulong,Guid> loadedAnchors)
        {
            if (count == 0)
            {
                loadedAnchors = new Dictionary<ulong, Guid>();
                return PxrResult.SUCCESS;
            }

            PxrAnchorEntityLoadResults results = new PxrAnchorEntityLoadResults()
            {
                inputCount = count,
                outputCount = count,
                loadResults = new IntPtr(0)
            }; 
            loadedAnchors = new Dictionary<ulong, Guid>();
            int resultSize = Marshal.SizeOf(typeof(PxrAnchorEntityLoadResult));
            int resultBytesSize = (int)count * resultSize;
            results.loadResults = Marshal.AllocHGlobal(resultBytesSize);
            var result = PXR_Plugin.MixedReality.UPxr_GetAnchorEntityLoadResults(taskId, ref results);
            for (int i = 0; i < count; i++)
            {
                PxrAnchorEntityLoadResult t = (PxrAnchorEntityLoadResult)Marshal.PtrToStructure(results.loadResults + i * resultSize, typeof(PxrAnchorEntityLoadResult));
                byte[] byteArray = new byte[16];
                BitConverter.GetBytes(t.uuid.value0).CopyTo(byteArray, 0);
                BitConverter.GetBytes(t.uuid.value1).CopyTo(byteArray, 8);
                var uuid = new Guid(byteArray);
                loadedAnchors.Add(t.anchor, uuid);
            }
            Marshal.FreeHGlobal(results.loadResults);
            return result;
        }

        /// <summary>
        /// Launches the Room Capture app to calibrate the room.
        /// </summary>
        /// <param name="taskId">Returns the ID of the task.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        public static PxrResult StartSpatialSceneCapture(out ulong taskId)
        {
            return PXR_Plugin.MixedReality.UPxr_StartSpatialSceneCapture(out taskId);
        }

        /// <summary>
        /// Gets the information about the volume for an anchor entity.
        /// Before calling this method, you need to load anchor entities and get the anchor entity load result first. The result contains the handles of anchor entities loaded.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the anchor entity.</param>
        /// <param name="center">Returns the offset of the volume's position relative to the anchor entity's position.</param>
        /// <param name="extent">Returns the length, width, and height of the volume.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        public static PxrResult GetAnchorVolumeInfo(ulong anchorHandle, out Vector3 center,out Vector3 extent)
        {
            PxrAnchorVolumeInfo info = new PxrAnchorVolumeInfo();
            var result = PXR_Plugin.MixedReality.UPxr_GetAnchorVolumeInfo(anchorHandle, ref info);
            center = new Vector3(info.center.x, info.center.y, info.center.z);
            extent = new Vector3(info.extent.x, info.extent.y, info.extent.z);
            return result;
        }

        /// <summary>
        /// Gets the information about the polygon (irregular plane) for an anchor entity.
        /// Before calling this method, you need to load anchor entities and get the anchor entity load result first. The result contains the handles of anchor entities loaded.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the anchor entity.</param>
        /// <param name="vertices">Returns the positions of the polygon's vertices on the X, Y, and Z axis.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        public static PxrResult GetAnchorPlanePolygonInfo(ulong anchorHandle, out Vector3[] vertices)
        {
            PxrAnchorPlanePolygonInfo info = new PxrAnchorPlanePolygonInfo()
            {
                inputCount = 0,
                outputCount = 0
            };
            PXR_Plugin.MixedReality.UPxr_GetAnchorPlanePolygonInfo(anchorHandle, ref info);
            info.inputCount = info.outputCount;
            info.vertices = Marshal.AllocHGlobal((int)info.outputCount * Marshal.SizeOf(typeof(Vector3)));
            var result = PXR_Plugin.MixedReality.UPxr_GetAnchorPlanePolygonInfo(anchorHandle, ref info);
            vertices = new Vector3[info.outputCount];
            IntPtr longPtr = info.vertices;
            for (int i = 0; i < info.outputCount; i++)
            {
                IntPtr tempPtr = new IntPtr(Marshal.SizeOf(typeof(Vector3)));
                tempPtr = longPtr;
                longPtr += Marshal.SizeOf(typeof(Vector3));
                vertices[i] = Marshal.PtrToStructure<Vector3>(tempPtr);
            }

            Marshal.FreeHGlobal(info.vertices);
            return result;
        }

        /// <summary>
        /// Gets the information about the boundary (rectangle) for an anchor entity.
        /// Before calling this method, you need to load anchor entities and get the anchor entity load result first. The result contains the handles and UUIDs of anchor entities loaded.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the anchor entity.</param>
        /// <param name="center">Returns the offset of the boundary's position relative to the anchor entity's position.</param>
        /// <param name="extent">Returns the width and height of the boundary.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        public static PxrResult GetAnchorPlaneBoundaryInfo(ulong anchorHandle, out Vector3 center,out Vector2 extent)
        {
            PxrAnchorPlaneBoundaryInfo info = new PxrAnchorPlaneBoundaryInfo();
            var result = PXR_Plugin.MixedReality.UPxr_GetAnchorPlaneBoundaryInfo(anchorHandle, ref info);
            center = new Vector3(info.center.x, info.center.y, info.center.z);
            extent = new Vector2(info.extent.width, info.extent.height);
            return result;
        }

        /// <summary>
        /// Gets the scene label of an anchor entity.
        /// </summary>
        /// <param name="anchorHandle">Specifies the handle of the anchor entity.</param>
        /// <param name="label">Returns the anchor entity's scene label.</param>
        /// <returns>Returns `0` for success and other values for failure. For failure reasons, refer to the `PxrResult` enum.</returns>
        public static PxrResult GetAnchorSceneLabel(ulong anchorHandle, out PxrSceneLabel label)
        {
            return PXR_Plugin.MixedReality.UPxr_GetAnchorSceneLabel(anchorHandle, out label);
        }

        #endregion

        /// <summary>
        /// Enables/disables video seethrough.
        /// </summary>
        /// <param name="state">Determines the state of video seethrough:
        /// * `true`: enable
        /// * `false`: disable
        /// </param>
        /// <returns>Returns `0` for success and other values for failure.</returns>
        public static int EnableVideoSeeThrough(bool state)
        {
            return PXR_Plugin.Boundary.UPxr_SetSeeThroughBackground(state);
        }

        public static int EnableVideoSeeThroughEffect(bool value)
        {
            return PXR_Plugin.MixedReality.UPxr_EnableVideoSeeThroughEffect(value);
        }

        public static int SetVideoSeeThroughEffect(PxrLayerEffect type,float value,float duration)
        {
            return PXR_Plugin.MixedReality.UPxr_SetVideoSeeThroughEffect(type, value, duration);
        }

        public static int SetVideoSeeThroughLut(Texture2D texture, int row, int col)
        {
            var data = texture.GetRawTextureData();
            return PXR_Plugin.MixedReality.UPxr_SetVideoSeeThroughLUT(ref data, texture.width, texture.height, row, col);
        }

        [System.Obsolete("Deprecated. Please use CreateAnchorEntity", false)]
        public static int CreateSpatialAnchor(Vector3 position, Quaternion rotation, PxrReferenceType type, ref ulong handle)
        {
            PxrSpatialAnchorCreateInfo info = new PxrSpatialAnchorCreateInfo()
            {
                referenceType = type,
                pose = new PxrPosef()
                {
                    orientation = new PxrVector4f()
                    {
                        x = rotation.x,
                        y = rotation.y,
                        z = -rotation.z,
                        w = -rotation.w
                    },
                    position = new PxrVector3f()
                    {
                        x = position.x,
                        y = position.y,
                        z = -position.z
                    }
                },
                time = PXR_Plugin.System.UPxr_GetPredictedDisplayTime()
            };
            return PXR_Plugin.MixedReality.UPxr_CreateSpatialAnchor(ref info, ref handle);
        }
        
        [System.Obsolete("Deprecated. Please use DestroyAnchorEntity", false)]
        public static int DestroySpatialAnchor(ulong handle)
        {
            return PXR_Plugin.MixedReality.UPxr_DestroySpatialAnchor(handle);
        }
        
        [System.Obsolete("Deprecated. Please use PersistAnchorEntity", false)]
        public static int SaveSpatialAnchor(PxrSpatialAnchorSaveInfo info, ref ulong requestId)
        {
            return PXR_Plugin.MixedReality.UPxr_SaveSpatialAnchor(ref info, ref requestId);
        }
        
        [System.Obsolete("Deprecated. Please use UnPersistAnchorEntity", false)]
        public static int DeleteSpatialAnchor(PxrSpatialAnchorDeleteInfo info, ref ulong requestId)
        {
            return PXR_Plugin.MixedReality.UPxr_DeleteSpatialAnchor(ref info, ref requestId);
        }
        
        [System.Obsolete("Deprecated. Please use LoadAnchorEntity", false)]
        public static int LoadSpatialAnchorById(PxrSpatialInstanceLoadByIdInfo info, ref ulong requestId)
        {
            return PXR_Plugin.MixedReality.UPxr_LoadSpatialAnchorById(ref info, ref requestId);
        }
        
        [System.Obsolete("Deprecated. Please use GetAnchorEntityLoadResults", false)]
        public static int GetSpatialAnchorLoadResults(UInt64 requestId, out PxrSpatialAnchorLoadResult[] results)
        {
            results = null;

            PxrSpatialAnchorLoadResults resultsCount = new PxrSpatialAnchorLoadResults()
            {
                resultCapacityInput = 0,
                resultCountOutput = 0,
                results = new IntPtr(0),
            };
            PXR_Plugin.MixedReality.UPxr_GetSpatialAnchorLoadResults(requestId, ref resultsCount);
            int resultSize = Marshal.SizeOf(typeof(PxrSpatialAnchorLoadResult));
            int resultBytesSize = resultsCount.resultCountOutput * resultSize;
            PxrSpatialAnchorLoadResults anchorResults = new PxrSpatialAnchorLoadResults()
            {
                resultCapacityInput = resultsCount.resultCountOutput,
                resultCountOutput = 0,
                results = Marshal.AllocHGlobal(resultBytesSize),
            };
            int state = PXR_Plugin.MixedReality.UPxr_GetSpatialAnchorLoadResults(requestId, ref anchorResults);

            results = new PxrSpatialAnchorLoadResult[resultsCount.resultCountOutput];
            for (int i = 0; i < resultsCount.resultCountOutput; i++)
            {
                PxrSpatialAnchorLoadResult t = (PxrSpatialAnchorLoadResult)Marshal.PtrToStructure(anchorResults.results + (i * resultSize), typeof(PxrSpatialAnchorLoadResult));
                results[i] = t;
            }
            Marshal.FreeHGlobal(anchorResults.results);

            return state;
        }
        
        [System.Obsolete("Deprecated. Please use GetAnchorPose", false)]
        public static int GetSpatialAnchorPose(ulong handle, PxrReferenceType type, ref PxrPosef pose)
        {
            double predictDisplayTime = PXR_Plugin.System.UPxr_GetPredictedDisplayTime();
            int result = PXR_Plugin.MixedReality.UPxr_GetSpatialAnchorPose(handle, predictDisplayTime, type, ref pose);
            pose.position.z = -pose.position.z;
            pose.orientation.z = -pose.orientation.z;
            pose.orientation.w = -pose.orientation.w;
            return result;
        }
        
        [System.Obsolete("Deprecated. Please use GetAnchorEntityUuid", false)]
        public static int GetSpatialAnchorUuid(ulong handle, ref PxrSpatialInstanceUuid uuid)
        {
            return PXR_Plugin.MixedReality.UPxr_GetSpatialAnchorUuid(handle, ref uuid);
        }
        
        [System.Obsolete("Deprecated.", false)]
        public static int LoadRoomScene(PxrRoomSceneLoadInfo loadInfo, ref ulong requestId)
        {
            return PXR_Plugin.MixedReality.UPxr_LoadRoomScene(ref loadInfo, ref requestId);
        }
        
        [System.Obsolete("Deprecated.", false)]
        public static int GetRoomSceneLoadResults(ulong requestId, out PxrRoomSceneLoadResult[] results)
        {
            results = null;
            PxrRoomSceneLoadResults resultsCount = new PxrRoomSceneLoadResults()
            {
                resultCapacityInput = 0,
                resultCountOutput = 0,
                results = new IntPtr(0),
            };
            PXR_Plugin.MixedReality.UPxr_GetRoomSceneLoadResults(requestId, ref resultsCount);

            int resultSize = Marshal.SizeOf(typeof(PxrRoomSceneLoadResult));
            int resultBytesSize = resultsCount.resultCountOutput * resultSize;
            PxrRoomSceneLoadResults sceneResults = new PxrRoomSceneLoadResults()
            {
                resultCapacityInput = resultsCount.resultCountOutput,
                resultCountOutput = 0,
                results = Marshal.AllocHGlobal(resultBytesSize),
            };
            var state = PXR_Plugin.MixedReality.UPxr_GetRoomSceneLoadResults(requestId, ref sceneResults);

            results = new PxrRoomSceneLoadResult[resultsCount.resultCountOutput];
            for (int i = 0; i < resultsCount.resultCountOutput; i++)
            {
                PxrRoomSceneLoadResult t = (PxrRoomSceneLoadResult)Marshal.PtrToStructure(sceneResults.results + (i * resultSize), typeof(PxrRoomSceneLoadResult));
                results[i] = t;
            }
            Marshal.FreeHGlobal(sceneResults.results);
            return state;
        }
        
        [System.Obsolete("Deprecated.Please Use StartSpatialSceneCapture", false)]
        public static int StartRoomCapture()
        {
            return PXR_Plugin.MixedReality.UPxr_StartRoomCapture();
        }

        [System.Obsolete]
        public static int CreateRoomSceneData(PxrSpatialInstanceUuid anchorUuid, IntPtr roomSceneData, int dataLen,
            ref ulong roomSceneDataHandle)
        {
            return PXR_Plugin.MixedReality.UPxr_CreateRoomSceneData(anchorUuid, roomSceneData, dataLen,
                ref roomSceneDataHandle);
        }
        
        [System.Obsolete]
        public static int DestroyRoomSceneData(ulong roomSceneDataHandle)
        {
            return PXR_Plugin.MixedReality.UPxr_DestroyRoomSceneData(roomSceneDataHandle);
        }
        
        [System.Obsolete]
        public static int SaveRoomSceneData(PxrRoomSceneDataSaveInfo saveInfo, ref ulong requestId)
        {
            return PXR_Plugin.MixedReality.UPxr_SaveRoomSceneData(ref saveInfo, ref requestId);
        }
        
        [System.Obsolete]
        public static int DeleteRoomSceneData(PxrRoomSceneDataDeleteInfo deleteInfo, ref ulong requestId)
        {
            return PXR_Plugin.MixedReality.UPxr_DeleteRoomSceneData(ref deleteInfo, ref requestId);
        }
    }
}

