using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.XR.PXR;
using UnityEngine;

public class PXR_MRLegacyPlugin
{
    [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern PxrResult Pxr_CreateAnchorEntity(ref PxrAnchorEntityCreateInfo info, out ulong anchorHandle);

    [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern PxrResult Pxr_DestroyAnchorEntity(ref PxrAnchorEntityDestroyInfo info);

    [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern PxrResult Pxr_GetAnchorPose(ulong anchorHandle,  out PxrPosef pose);

    [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern PxrResult Pxr_GetAnchorEntityUuid(ulong anchorHandle, out PxrUuid uuid);

    [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern PxrResult Pxr_GetAnchorComponentFlags(ulong anchorHandle,
        out ulong flag);

    [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern PxrResult Pxr_GetAnchorSceneLabel(ulong anchorHandle, out PxrSceneLabel label);

    [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern PxrResult Pxr_GetAnchorPlaneBoundaryInfo(ulong anchorHandle,
        ref PxrAnchorPlaneBoundaryInfo info);

    [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern PxrResult Pxr_GetAnchorPlanePolygonInfo(ulong anchorHandle,
        ref PxrAnchorPlanePolygonInfo info);

    [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern PxrResult Pxr_GetAnchorBoxInfo(ulong anchorHandle, ref PxrAnchorVolumeInfo info);

    [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern PxrResult Pxr_PersistAnchorEntity(ref PxrAnchorEntityPersistInfo info,
        out ulong taskId);

    [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern PxrResult Pxr_UnpersistAnchorEntity(ref PxrAnchorEntityUnPersistInfo info,
        out ulong taskId);

    [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern PxrResult Pxr_ClearPersistedAnchorEntity(ref PxrAnchorEntityClearInfo info,
        out ulong taskId);

    [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern PxrResult Pxr_LoadAnchorEntity(ref PxrAnchorEntityLoadInfo info, out ulong taskId);

    [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern PxrResult
        Pxr_GetAnchorEntityLoadResults(ulong taskId, ref PxrAnchorEntityLoadResults results);

    [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern PxrResult Pxr_StartSpatialSceneCapture(out ulong taskId);


    public static PxrResult UPxr_LoadAnchorEntity(ref PxrAnchorEntityLoadInfo info, out ulong taskId)
    {
        taskId = ulong.MinValue;
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_LoadAnchorEntity(ref info, out taskId);
#else
        return PxrResult.TIMEOUT_EXPIRED;
#endif
    }

    public static PxrResult UPxr_GetAnchorEntityLoadResults(ulong taskId, ref PxrAnchorEntityLoadResults result)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetAnchorEntityLoadResults(taskId, ref result);
#else
        return PxrResult.TIMEOUT_EXPIRED;
#endif
    }

    public static PxrResult UPxr_CreateAnchorEntity(ref PxrAnchorEntityCreateInfo info, out ulong anchorHandle)
    {
        anchorHandle = ulong.MinValue;
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_CreateAnchorEntity(ref info,out anchorHandle);
#else
        return PxrResult.TIMEOUT_EXPIRED;
#endif
    }

    public static PxrResult UPxr_DestroyAnchorEntity(ref PxrAnchorEntityDestroyInfo info)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_DestroyAnchorEntity(ref info);
#else
        return PxrResult.TIMEOUT_EXPIRED;
#endif
    }

    public static PxrResult UPxr_GetAnchorPose(ulong anchorHandle,  out PxrPosef pose)
    {
        pose = new PxrPosef();
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetAnchorPose(anchorHandle, out pose);
#else
        return PxrResult.TIMEOUT_EXPIRED;
#endif
    }

    public static PxrResult UPxr_GetAnchorEntityUuid(ulong anchorHandle, out PxrUuid uuid)
    {
        uuid = new PxrUuid();
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetAnchorEntityUuid(anchorHandle, out uuid);
#else
        return PxrResult.TIMEOUT_EXPIRED;
#endif
    }

    public static PxrResult UPxr_PersistAnchorEntity(ref PxrAnchorEntityPersistInfo info, out ulong taskId)
    {
        taskId = ulong.MinValue;
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_PersistAnchorEntity(ref info, out taskId);
#else
        return PxrResult.TIMEOUT_EXPIRED;
#endif
    }

    public static PxrResult UPxr_GetAnchorPlanePolygonInfo(ulong anchorHandle, ref PxrAnchorPlanePolygonInfo info)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetAnchorPlanePolygonInfo(anchorHandle, ref info);
#else
        return PxrResult.TIMEOUT_EXPIRED;
#endif
    }

    public static PxrResult UPxr_GetAnchorVolumeInfo(ulong anchorHandle, ref PxrAnchorVolumeInfo info)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetAnchorBoxInfo(anchorHandle, ref info);
#else
        return PxrResult.TIMEOUT_EXPIRED;
#endif
    }

    public static PxrResult UPxr_StartSpatialSceneCapture(out ulong taskId)
    {
        taskId = ulong.MinValue;
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_StartSpatialSceneCapture(out taskId);
#else
        return PxrResult.TIMEOUT_EXPIRED;
#endif
    }

    public static PxrResult UPxr_ClearPersistedAnchorEntity(ref PxrAnchorEntityClearInfo info, out ulong taskId)
    {
        taskId = ulong.MinValue;
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_ClearPersistedAnchorEntity(ref info, out taskId);
#else
        return PxrResult.TIMEOUT_EXPIRED;
#endif
    }


    public static PxrResult UPxr_GetAnchorComponentFlags(ulong anchorHandle, out ulong flag)
    {
        flag = UInt64.MinValue;
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetAnchorComponentFlags(anchorHandle, out flag);
#else
        return PxrResult.TIMEOUT_EXPIRED;
#endif
    }

    public static PxrResult UPxr_UnpersistAnchorEntity(ref PxrAnchorEntityUnPersistInfo info, out ulong taskId)
    {
        taskId = ulong.MinValue;
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_UnpersistAnchorEntity(ref info, out taskId);
#else
        return PxrResult.TIMEOUT_EXPIRED;
#endif
    }

    public static PxrResult UPxr_GetAnchorPlaneBoundaryInfo(ulong anchorHandle, ref PxrAnchorPlaneBoundaryInfo info)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetAnchorPlaneBoundaryInfo(anchorHandle, ref info);
#else
        return PxrResult.TIMEOUT_EXPIRED;
#endif
    }

    public static PxrResult UPxr_GetAnchorSceneLabel(ulong anchorHandle, out PxrSceneLabel label)
    {
        label = PxrSceneLabel.UnKnown;
#if UNITY_ANDROID && !UNITY_EDITOR
                return Pxr_GetAnchorSceneLabel(anchorHandle, out label);
#else
        return PxrResult.TIMEOUT_EXPIRED;
#endif
    }
    public static PxrSceneComponentType UPxr_ConvertAnchorCTypeToSceneCType(PxrAnchorComponentTypeFlags flag)
    {
        switch (flag)
        {
            case PxrAnchorComponentTypeFlags.Pose:
            case PxrAnchorComponentTypeFlags.Persistence:
                return PxrSceneComponentType.Location;
            case PxrAnchorComponentTypeFlags.SceneLabel:
                return PxrSceneComponentType.Semantic;
            case PxrAnchorComponentTypeFlags.Plane:
                return PxrSceneComponentType.Box2D;
            case PxrAnchorComponentTypeFlags.Volume:
                return PxrSceneComponentType.Box3D;
            default:
                throw new ArgumentOutOfRangeException(nameof(flag), flag, null);
        }
    }

}

public struct PxrAnchorEntityLoadUuidFilter
{
    public PxrStructureType type;
    public uint uuidCount;
    public IntPtr uuidList; //=>PxrUuid[]
}

public struct PxrAnchorEntityLoadInfo
{
    public uint maxResult;
    public ulong timeout;
    public PxrPersistLocation location;
    public IntPtr include; //=>PxrAnchorEntityLoadFilterBaseHeader
    public IntPtr exclude; //=>PxrAnchorEntityLoadFilterBaseHeader
}

public struct PxrAnchorEntityLoadResults
{
    public uint inputCount;
    public uint outputCount;
    public IntPtr loadResults; //=>PxrAnchorEntityLoadResult[]
}

public struct PxrAnchorEntityLoadResult
{
    public ulong anchor;
    public PxrUuid uuid;
}
