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
using Pico.Platform.Models;
using UnityEngine;

namespace Pico.Platform
{
    /**
     * \ingroup Platform
     *
     * Downloadable content (DLC) represents the contents/files such as expansion packs that users can purchase and download, which can help grow your revenue. Each DLC is associated with an add-on and has an individual SKU as its unique identifier. Users must purchase the app before purchasing the DLCs provided in it. DLCs are downloadable in apps only.
     *
     * DLC enables you to update your app in a more flexible and lightweight way. Once you want to update the content for a published app, you only need to upload new resources such as levels and cosmetics as DLCs on the PICO Developer Platform, but do not need to upload a new build. Users can thereby purchase, download, and experience the latest resources without having to update or reinstall your app.
     */
    public static class AssetFileService
    {
        /// <summary>
        /// Deletes an installed asset file by asset file ID. The corresponding
        /// asset file will be removed from the device. 
        /// </summary>
        /// <param name="assetFileId">The ID of the asset file to delete.</param>
        /// <returns>
        /// An object containing the asset file ID, asset file name, and a success flag.
        /// </returns>
        public static Task<AssetFileDeleteResult> DeleteById(ulong assetFileId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<AssetFileDeleteResult>(CLIB.ppf_AssetFile_DeleteById(assetFileId));
        }

        /// <summary>
        /// Deletes an installed asset file by asset file name. The corresponding
        /// asset file will be removed from the device. 
        /// </summary>
        /// <param name="assetFileName">The name of the asset file to delete.</param>
        /// <returns>
        /// An object containing the asset file ID, asset file name, and a success flag.
        /// </returns>
        public static Task<AssetFileDeleteResult> DeleteByName(string assetFileName)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<AssetFileDeleteResult>(CLIB.ppf_AssetFile_DeleteByName(assetFileName));
        }

        /// <summary>
        /// Downloads an asset file by asset file ID.
        /// </summary>
        /// <param name="assetFileId">The ID of the asset file to download.</param>
        /// <returns>
        /// An object containing the asset file ID and asset file name.
        ///
        /// If the response returns code `0`, the download will start and
        /// the system will periodically push information about the download progress.
        /// If the user has not purchased the asset file, a non-zero error code will be returned.
        /// </returns>
        public static Task<AssetFileDownloadResult> DownloadById(ulong assetFileId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<AssetFileDownloadResult>(CLIB.ppf_AssetFile_DownloadById(assetFileId));
        }

        /// <summary>
        /// Downloads an asset file by asset file name.
        /// </summary>
        /// <param name="assetFileName">The name of the asset file to download.</param>
        /// <returns>
        /// An object containing the asset file ID and asset file name.
        ///
        /// If the response returns code `0`, the download will start and
        /// the system will periodically push information about the download progress.
        /// If the user has not purchased the asset file, a non-zero error code will be returned.
        /// </returns>
        public static Task<AssetFileDownloadResult> DownloadByName(string assetFileName)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<AssetFileDownloadResult>(CLIB.ppf_AssetFile_DownloadByName(assetFileName));
        }

        /// <summary>
        /// Cancels the download of an asset file by asset file ID.
        ///
        /// </summary>
        /// <param name="assetFileId">The ID of the asset file to cancel download for.</param>
        /// <returns>
        /// An object contains the asset file ID, asset file name, and a success flag.
        /// </returns>
        public static Task<AssetFileDownloadCancelResult> DownloadCancelById(ulong assetFileId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<AssetFileDownloadCancelResult>(CLIB.ppf_AssetFile_DownloadCancelById(assetFileId));
        }

        /// <summary>
        /// Cancels the download of an asset file by asset file name.
        ///
        /// </summary>
        /// <param name="assetFileName">The name of the asset file to cancel download for.</param>
        /// <returns>
        /// An object contains the asset file ID, asset file name, and a success flag.
        /// </returns>
        public static Task<AssetFileDownloadCancelResult> DownloadCancelByName(string assetFileName)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<AssetFileDownloadCancelResult>(CLIB.ppf_AssetFile_DownloadCancelByName(assetFileName));
        }

        /// <summary>
        /// Gets the download status of an asset file by asset file ID.
        /// </summary>
        /// <param name="assetId">The ID of the asset file to get the download status for.</param>
        /// <returns>
        /// An object containing the asset file ID, asset file name, and whether the asset file is downloaded.
        /// </returns>
        public static Task<AssetStatus> StatusById(ulong assetId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<AssetStatus>(CLIB.ppf_AssetFile_StatusById(assetId));
        }

        /// <summary>
        /// Gets the download status of an asset file by asset file name.
        /// </summary>
        /// <param name="assetFileName">The name of the asset file to get the download status for.</param>
        /// <returns>
        /// An object containing the asset file ID, asset file name, and whether the asset file is downloaded.
        /// </returns>
        public static Task<AssetStatus> StatusByName(string assetFileName)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<AssetStatus>(CLIB.ppf_AssetFile_StatusByName(assetFileName));
        }

        /// <summary>
        /// Gets the asset file list.  
        /// </summary>
        /// <returns>
        /// An asset file list. Each `AssetDetails` contains fields indicating
        /// whether an asset file is purchased or downloaded.
        /// </returns>
        public static Task<AssetDetailsList> GetList()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<AssetDetailsList>(CLIB.ppf_AssetFile_GetList());
        }

        /// <summary>
        /// Gets the next page of the asset file list.
        /// </summary>
        /// <param name="list">The current page of the asset file list.</param>
        /// <returns>The next page of the asset file list.</returns>
        public static Task<AssetDetailsList> GetNextAssetDetailsListPage(AssetDetailsList list)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            if (!list.HasNextPage)
            {
                Debug.LogWarning("GetNextAssetDetailsListPage: List has no next page");
                return null;
            }

            if (String.IsNullOrEmpty(list.NextPageParam))
            {
                Debug.LogWarning("GetNextAssetDetailsListPage: list.NextPageParam is empty");
                return null;
            }

            return new Task<AssetDetailsList>(CLIB.ppf_AssetFile_GetNextAssetDetailsArrayPage(list.NextPageParam));
        }

        /// <summary>
        /// This notification is used to track the download progress of asset file.
        /// The `Transferred` field indicates the number of bytes downloaded.
        /// The `CompleteStatus` field indicates the download status.
        /// </summary>
        /// <param name="handler">The callback function.</param>
        public static void SetOnDownloadUpdateCallback(Message<AssetFileDownloadUpdate>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_AssetFile_DownloadUpdate, handler);
        }

        /// <summary>
        /// If the downloaded asset file is different from the original one, 
        /// the asset file will be automatically removed, and the app will receive a notification.
        /// </summary>
        /// <param name="handler">The callback function.</param>
        public static void SetOnDeleteForSafetyCallback(Message<AssetFileDeleteForSafety>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_AssetFile_DeleteForSafety, handler);
        }
    }
}