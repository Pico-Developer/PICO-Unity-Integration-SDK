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

namespace Pico.Platform.Models
{
    public static class DownloadStatus
    {
        public const string Downloaded = "downloaded";
        public const string Available = "available";
        public const string InProgress = "in-progress";
    }

    /// <summary>
    /// Constants indicates whether the user purchased the in-app product.
    /// </summary>
    public static class IapStatus
    {
        /// Purchased 
        public const string Entitled = "entitled";

        /// Not purchased. 
        public const string NotEntitled = "not-entitled";
    }

    /// <summary> Indicates where the DLC file is displayed.</summary>
    public static class AssetType
    {
        /// The DLC file is displayed in the PICO Store and the app. 
        public const string Store = "store";

        /// The DLC file is displayed in the app only. 
        public const string Default = "default";
    }

    public class AssetDetails
    {
        /// The unique identifier of DLC file.
        public ulong AssetId;

        /** Some DLC files can be displayed in the PICO Store. Now it has two values: `default` or `store`.
         * You can refer to \ref AssetType for details.
         */
        public string AssetType;

        /// One of `downloaded`, `available`, and `in-progress`. You can refer to \ref DownloadStatus for details.
        public string DownloadStatus;

        /// The path to the downloaded DLC file. For a non-downloaded DLC file, this field will be empty.
        public string Filepath;

        /// The meta info of the DLC file.
        public string Metadata;

        /// The name of the DLC file.
        public string Filename;

        /// The version of the DLC file.
        public int Version;

        /// One of `entitled`, `not-entitled`. You can refer to \ref IapStatus for details.
        public string IapStatus;

        /// The SKU of the in-app product that the DLC file associated with.
        public string IapSku;

        /// The name of the in-app product that the DLC fiel associated with.
        public string IapName;

        /// The price of this DLC file.
        public string IapPrice;

        /// The currency required for purchasing the DLC file.
        public string IapCurrency;

        /// The description of the in-app product that the DLC file associated with.
        public string IapDescription;

        /// The icon of the in-app product that the DLC file associated with.
        public string IapIcon;

        public AssetDetails(IntPtr o)
        {
            AssetId = CLIB.ppf_AssetDetails_GetAssetId(o);
            AssetType = CLIB.ppf_AssetDetails_GetAssetType(o);
            DownloadStatus = CLIB.ppf_AssetDetails_GetDownloadStatus(o);
            IapStatus = CLIB.ppf_AssetDetails_GetIapStatus(o);
            Filepath = CLIB.ppf_AssetDetails_GetFilepath(o);
            Metadata = CLIB.ppf_AssetDetails_GetMetadata(o);
            Filename = CLIB.ppf_AssetDetails_GetFilename(o);
            Version = CLIB.ppf_AssetDetails_GetVersion(o);
            IapSku = CLIB.ppf_AssetDetails_GetIapSku(o);
            IapName = CLIB.ppf_AssetDetails_GetIapName(o);
            IapPrice = CLIB.ppf_AssetDetails_GetIapPrice(o);
            IapCurrency = CLIB.ppf_AssetDetails_GetIapCurrency(o);
            IapDescription = CLIB.ppf_AssetDetails_GetIapDescription(o);
            IapIcon = CLIB.ppf_AssetDetails_GetIapIcon(o);
        }
    }

    /// <summary>
    /// Each element is \ref AssetDetails
    /// </summary>
    public class AssetDetailsList : MessageArray<AssetDetails>
    {
        public AssetDetailsList(IntPtr a)
        {
            var count = (int) CLIB.ppf_AssetDetailsArray_GetSize(a);
            this.Capacity = count;
            for (int i = 0; i < count; i++)
            {
                this.Add(new AssetDetails(CLIB.ppf_AssetDetailsArray_GetElement(a, (UIntPtr) i)));
            }

            NextPageParam = CLIB.ppf_AssetDetailsArray_GetNextPageParam(a);
        }
    }

    /// <summary>
    /// If the downloaded DLC file is different from the original one, 
    /// the DLC file will be automatically removed, and the app will receive a notification.
    /// </summary>
    public class AssetFileDeleteForSafety
    {
        /// The ID of the DLC file. 
        public readonly ulong AssetId;

        /// The description for why this asset file is deleted. 
        public readonly string Reason;

        public AssetFileDeleteForSafety(IntPtr o)
        {
            AssetId = CLIB.ppf_AssetFileDeleteForSafety_GetAssetId(o);
            Reason = CLIB.ppf_AssetFileDeleteForSafety_GetReason(o);
        }
    }

    /// <summary>
    /// The callback for \ref AssetFileService.DeleteById and \ref AssetFileService.DeleteByName.
    /// </summary>
    public class AssetFileDeleteResult
    {
        /// The path to the DLC file.
        public readonly string Filepath;

        /// Whether the DLC file is deleted successfully.
        public readonly bool Success;

        /// The ID of the DLC file. 
        public readonly ulong AssetId;

        public AssetFileDeleteResult(IntPtr o)
        {
            Filepath = CLIB.ppf_AssetFileDeleteResult_GetFilepath(o);
            Success = CLIB.ppf_AssetFileDeleteResult_GetSuccess(o);
            AssetId = CLIB.ppf_AssetFileDeleteResult_GetAssetId(o);
        }
    }

    /// <summary>Indicates whether the download of the DLC file is successfully canceled.</summary>
    public class AssetFileDownloadCancelResult
    {
        /// The path to the DLC file. 
        public readonly string Filepath;

        /// Whether the download is successfully canceled. 
        public readonly bool Success;

        /// The ID of the DLC file. 
        public readonly ulong AssetId;

        public AssetFileDownloadCancelResult(IntPtr o)
        {
            Filepath = CLIB.ppf_AssetFileDownloadCancelResult_GetFilepath(o);
            Success = CLIB.ppf_AssetFileDownloadCancelResult_GetSuccess(o);
            AssetId = CLIB.ppf_AssetFileDownloadCancelResult_GetAssetId(o);
        }
    }

    /// <summary>The result returned after calling \ref AssetFileService.DownloadById or \ref AssetFileService.DownloadByName.</summary>
    public class AssetFileDownloadResult
    {
        /// The ID of the DLC file. 
        public readonly ulong AssetId;

        /// The path to the DLC file. 
        public readonly string Filepath;

        public AssetFileDownloadResult(IntPtr o)
        {
            AssetId = CLIB.ppf_AssetFileDownloadResult_GetAssetId(o);
            Filepath = CLIB.ppf_AssetFileDownloadResult_GetFilepath(o);
        }
    }

    /// <summary>
    /// You will receive this message periodically once you call \ref AssetFileService.DownloadById
    /// or \ref AssetFileService.DownloadByName.
    /// </summary>
    public class AssetFileDownloadUpdate
    {
        /// The ID of the DLC file. 
        public readonly ulong AssetId;

        /// The total bytes of the DLC file.
        public readonly ulong BytesTotal;

        /// The transferred bytes of the DLC file. 
        public readonly long BytesTransferred;

        /// The download status of the DLC file.
        public readonly AssetFileDownloadCompleteStatus CompleteStatus;

        public AssetFileDownloadUpdate(IntPtr o)
        {
            AssetId = CLIB.ppf_AssetFileDownloadUpdate_GetAssetId(o);
            BytesTotal = CLIB.ppf_AssetFileDownloadUpdate_GetBytesTotal(o);
            BytesTransferred = CLIB.ppf_AssetFileDownloadUpdate_GetBytesTransferred(o);
            CompleteStatus = CLIB.ppf_AssetFileDownloadUpdate_GetCompleteStatus(o);
        }
    }

    /// <summary>
    /// The callback for \ref AssetFileService.StatusById or \ref AssetFileService.StatusByName.
    /// </summary>
    public class AssetStatus
    {
        /// The ID of the DLC file. 
        public readonly ulong AssetId;

        /// The name of the DLC file. 
        public readonly string Filename;

        /// The path to the DLC file. 
        public readonly string Filepath;

        /// The download status of the DLC file. You can refer to \ref DownloadStatus for details.
        public readonly string DownloadStatus;

        public AssetStatus(IntPtr o)
        {
            AssetId = CLIB.ppf_AssetStatus_GetAssetId(o);
            Filename = CLIB.ppf_AssetStatus_GetFilename(o);
            Filepath = CLIB.ppf_AssetStatus_GetFilepath(o);
            DownloadStatus = CLIB.ppf_AssetStatus_GetDownloadStatus(o);
        }
    }
}