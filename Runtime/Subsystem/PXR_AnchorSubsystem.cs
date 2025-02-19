#if AR_FOUNDATION_5 || AR_FOUNDATION_6
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace Unity.XR.PXR
{
    public class PXR_AnchorSubsystem : XRAnchorSubsystem
    {
        internal const string k_SubsystemId = "PXR_AnchorSubsystem";

        class PXR_AnchorProvider : Provider
        {
            private Dictionary<TrackableId, ulong> trackableIdToHandleMap;
            private Dictionary<ulong, XRAnchor> handleToXRAnchorMap;
            private HashSet<ulong> managedAnchorHandles;
            private Dictionary<Guid, ulong> lastAnchorToTime;
            private bool isInit = false;

            public override void Start()
            {
                StartSpatialAnchorProvider();
            }

            private async void StartSpatialAnchorProvider()
            {
                var result = await PXR_MixedReality.StartSenseDataProvider(PxrSenseDataProviderType.SpatialAnchor);
                if (result == PxrResult.SUCCESS)
                {
                    if (!isInit)
                    {
                        trackableIdToHandleMap = new Dictionary<TrackableId, ulong>();
                        handleToXRAnchorMap = new Dictionary<ulong, XRAnchor>();
                        managedAnchorHandles = new HashSet<ulong>();
                        isInit = true;
                    }
                }
                else
                {
                    Debug.LogError("Spatial Anchor Provider Start Failed:" + result);
                }
            }

            public override void Stop()
            {
                var result = PXR_MixedReality.StopSenseDataProvider(PxrSenseDataProviderType.SpatialAnchor);
                if (result == PxrResult.SUCCESS)
                {

                }
                else
                {
                    Debug.LogError("Spatial Anchor Provider Stop Failed:" + result);
                }
            }

            public override void Destroy()
            {
                
            }

            public override TrackableChanges<XRAnchor> GetChanges(XRAnchor defaultAnchor, Allocator allocator)
            {
                return new TrackableChanges<XRAnchor>();
            }

#if AR_FOUNDATION_5
            public override bool TryAddAnchor(Pose pose, out XRAnchor anchor)
            {
                var tcs = new TaskCompletionSource<(PxrResult result, ulong anchorHandle, Guid uuid)>();
                var tcs2 = new TaskCompletionSource<PxrResult>();
                Task.Run(() =>
                {
                    var (pxrResult, handle, guid) = PXR_MixedReality.CreateSpatialAnchorAsync(pose.position, pose.rotation).Result;

                    tcs.SetResult((pxrResult, handle, guid));
                });
                var (result, anchorHandle, uuid) = tcs.Task.Result;
                if (result == PxrResult.SUCCESS)
                {
                    Task.Run(() =>
                    {
                        var pxrResult = PXR_MixedReality.PersistSpatialAnchorAsync(anchorHandle).Result;

                        tcs2.SetResult(pxrResult);
                    });

                    var result2 = tcs2.Task.Result;
                    if (result2 == PxrResult.SUCCESS)
                    {
                        var bytes = uuid.ToByteArray();
                        var trackabledId = new TrackableId(BitConverter.ToUInt64(bytes, 0), BitConverter.ToUInt64(bytes, 8));
                        var nativePtr = new IntPtr((long)anchorHandle);
                        anchor = new XRAnchor(trackabledId, pose, TrackingState.Tracking, nativePtr);
                        trackableIdToHandleMap[trackabledId] = anchorHandle;
                        handleToXRAnchorMap[anchorHandle] = anchor;
                        return true;
                    }
                    else
                    {
                        anchor = XRAnchor.defaultValue;
                        return false;
                    }
                }
                else
                {
                    anchor = XRAnchor.defaultValue;
                    return false;
                }
            }

            public override bool TryRemoveAnchor(TrackableId anchorId)
            {
                if (trackableIdToHandleMap.TryGetValue(anchorId, out var anchorHandle))
                {
                    var result = PXR_MixedReality.DestroyAnchor(anchorHandle);
                    if (result == PxrResult.SUCCESS)
                    {
                        var tcs = new TaskCompletionSource<PxrResult>();
                        Task.Run(() =>
                        {
                            var pxrResult = PXR_MixedReality.UnPersistSpatialAnchorAsync(anchorHandle).Result;

                            tcs.SetResult(pxrResult);
                        });
                        var result1 = tcs.Task.Result;
                        if (result1 == PxrResult.SUCCESS)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
#endif

#if AR_FOUNDATION_6
            public override Awaitable<Result<XRAnchor>> TryAddAnchorAsync(Pose pose)
            {
                var synchronousResultStatus = new XRResultStatus(XRResultStatus.StatusCode.UnknownError);
                var awaitable = new AwaitableCompletionSource<Result<XRAnchor>>();
                var anchor = XRAnchor.defaultValue;

                var tcs = new TaskCompletionSource<(PxrResult result, ulong anchorHandle, Guid uuid)>();
                Task.Run(() =>
                {
                    var (pxrResult, handle, guid) = PXR_MixedReality.CreateSpatialAnchorAsync(pose.position, pose.rotation).Result;

                    tcs.SetResult((pxrResult, handle, guid));
                });
                var (result, anchorHandle, uuid) = tcs.Task.Result;
                if (result == PxrResult.SUCCESS)
                {
                    var bytes = uuid.ToByteArray();
                    var trackabledId = new TrackableId(BitConverter.ToUInt64(bytes, 0), BitConverter.ToUInt64(bytes, 8));
                    var nativePtr = new IntPtr((long)anchorHandle);
                    synchronousResultStatus = new XRResultStatus(XRResultStatus.StatusCode.UnqualifiedSuccess);
                    anchor = new XRAnchor(trackabledId, pose, TrackingState.Tracking, nativePtr);
                    trackableIdToHandleMap[trackabledId] = anchorHandle;
                    handleToXRAnchorMap[anchorHandle] = anchor;
                }

                var returnResult = new Result<XRAnchor>(synchronousResultStatus, anchor);
                awaitable.SetResult(returnResult);
                return awaitable.Awaitable;
            }

            public override Awaitable<Result<SerializableGuid>> TrySaveAnchorAsync(TrackableId anchorId, CancellationToken cancellationToken = default)
            {
                var tcs2 = new TaskCompletionSource<PxrResult>();
                var synchronousResultStatus = new XRResultStatus(XRResultStatus.StatusCode.UnknownError);
                var awaitable = new AwaitableCompletionSource<Result<SerializableGuid>>();
                var returnResult = new Result<SerializableGuid>(synchronousResultStatus, default);

                if (trackableIdToHandleMap.TryGetValue(anchorId, out var anchorHandle))
                {
                    Task.Run(() =>
                    {
                        var pxrResult = PXR_MixedReality.PersistSpatialAnchorAsync(anchorHandle, cancellationToken).Result;

                        tcs2.SetResult(pxrResult);
                    });

                    var result2 = tcs2.Task.Result;
                    if (result2 == PxrResult.SUCCESS)
                    {
                        synchronousResultStatus = new XRResultStatus(XRResultStatus.StatusCode.UnqualifiedSuccess);
                        returnResult = new Result<SerializableGuid>(synchronousResultStatus, anchorId);
                    }
                    else
                    {
                        synchronousResultStatus = new XRResultStatus(XRResultStatus.StatusCode.PlatformError, (int)result2);
                        returnResult = new Result<SerializableGuid>(synchronousResultStatus, default);
                    }

                }
                awaitable.SetResult(returnResult);
                return awaitable.Awaitable;
            }

            public override Awaitable<XRResultStatus> TryEraseAnchorAsync(SerializableGuid savedAnchorGuid, CancellationToken cancellationToken = default)
            {
                var tcs = new TaskCompletionSource<PxrResult>();
                var synchronousResultStatus = new XRResultStatus(XRResultStatus.StatusCode.UnknownError);
                var awaitable = new AwaitableCompletionSource<XRResultStatus>();

                if (trackableIdToHandleMap.TryGetValue(savedAnchorGuid, out var anchorHandle))
                {
                    Task.Run(() =>
                    {
                        var pxrResult = PXR_MixedReality.UnPersistSpatialAnchorAsync(anchorHandle, cancellationToken).Result;

                        tcs.SetResult(pxrResult);
                    });
                    var result1 = tcs.Task.Result;
                    if (result1 == PxrResult.SUCCESS)
                    {
                        synchronousResultStatus = new XRResultStatus(XRResultStatus.StatusCode.UnqualifiedSuccess);
                    }
                    else
                    {
                        synchronousResultStatus = new XRResultStatus(XRResultStatus.StatusCode.PlatformError, (int)result1);
                    }
                }
                awaitable.SetResult(synchronousResultStatus);
                return awaitable.Awaitable;
            }

            public override bool TryRemoveAnchor(TrackableId anchorId)
            {
                if (trackableIdToHandleMap.TryGetValue(anchorId, out var anchorHandle))
                {
                    var result = PXR_MixedReality.DestroyAnchor(anchorHandle);
                    if (result == PxrResult.SUCCESS)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            public override Awaitable<Result<XRAnchor>> TryLoadAnchorAsync(SerializableGuid savedAnchorGuid, CancellationToken cancellationToken = default)
            {
                var tcs = new TaskCompletionSource<(PxrResult result, List<ulong> anchorHandleList)>();
                var synchronousResultStatus = new XRResultStatus(XRResultStatus.StatusCode.UnknownError);
                var awaitable = new AwaitableCompletionSource<Result<XRAnchor>>();
                var anchor = XRAnchor.defaultValue;
                var guid = savedAnchorGuid.guid;
                Guid[] guids = { guid };
                Task.Run(() =>
                {
                    var pxrResult = PXR_MixedReality.QuerySpatialAnchorAsync(guids).Result;

                    tcs.SetResult(pxrResult);
                });
                var result1 = tcs.Task.Result;
                if (result1.result == PxrResult.SUCCESS)
                {
                    for (int i = 0; i < result1.anchorHandleList.Count; i++)
                    {
                        var nativePtr = new IntPtr((long)result1.anchorHandleList[i]);
                        synchronousResultStatus = new XRResultStatus(XRResultStatus.StatusCode.UnqualifiedSuccess);
                        PXR_MixedReality.LocateAnchor(result1.anchorHandleList[i], out var position, out var quaternion);
                        anchor = new XRAnchor(savedAnchorGuid, new Pose(position,quaternion), TrackingState.Tracking, nativePtr);

                        trackableIdToHandleMap[savedAnchorGuid] = result1.anchorHandleList[i];
                        handleToXRAnchorMap[result1.anchorHandleList[i]] = anchor;
                    }
                }
                var returnResult = new Result<XRAnchor>(synchronousResultStatus, anchor);
                awaitable.SetResult(returnResult);
                return awaitable.Awaitable;
            }
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {

#if AR_FOUNDATION_5
            var cInfo = new XRAnchorSubsystemDescriptor.Cinfo()
            {
                id = k_SubsystemId,
                providerType = typeof(PXR_AnchorProvider),
                subsystemTypeOverride = typeof(PXR_AnchorSubsystem),
                supportsTrackableAttachments = false
            };
            XRAnchorSubsystemDescriptor.Create(cInfo);
#endif

#if AR_FOUNDATION_6
            var cInfo = new XRAnchorSubsystemDescriptor.Cinfo()
            {
                id = k_SubsystemId,
                providerType = typeof(PXR_AnchorProvider),
                subsystemTypeOverride = typeof(PXR_AnchorSubsystem),
                supportsTrackableAttachments = false,
                supportsSynchronousAdd = false,
                supportsSaveAnchor = true,
                supportsLoadAnchor = true,
                supportsEraseAnchor = true,
                supportsGetSavedAnchorIds = false,
                supportsAsyncCancellation = false
            };
            XRAnchorSubsystemDescriptor.Register(cInfo);
#endif
        }
    }
}
#endif
