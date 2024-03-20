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
using System.Runtime.InteropServices;
using Pico.Platform.Models;
using UnityEngine;

namespace Pico.Platform
{
    /**
     * \ingroup Platform
     * Leaderboard is one of the basic and important features of an app.
     * By displaying users' rankings in a multi-dimensional approach, leaderboards can give rise to a competitive atmosphere among users in specific scenarios such as gaming, drive users to improve their skills, and therefore increase app engagement. You can also use leaderboards to promote the app and attract new users.
     * Currently, Leaderboard service offers the following key features:
     * * Create leaderboards
     * * Get leaderboard data
     * * Update leaderboard data
     */
    public static class LeaderboardService
    {
        /// <summary>Gets the information for a specified leaderboard.</summary>
        ///
        /// <param name="leaderboardName">The name of the leaderboard to get information for.</param>
        /// <returns>Request information of type `Task`, including the request ID, and its response message will contain data of type `LeaderboardList`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |10701|request server failed|
        /// |10703|checking parameter failed|
        /// |10704|leaderboard is not exist|
        ///
        /// A message of type `MessageType.Leaderboard_Get` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `LeaderboardList`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<LeaderboardList> Get(string leaderboardName)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<LeaderboardList>(CLIB.ppf_Leaderboard_Get(leaderboardName));
        }

        /// <summary>Gets a list of entries.</summary>
        ///
        /// <param name="leaderboardName">The name of the leaderboard whose entries are to be returned.</param>
        /// <param name="pageSize">The number of entries to return on each page.</param>
        /// <param name="pageIdx">Defines which page of entries to return. The first page index is `0`.
        /// For example, if you want to get the first page of entries, pass `0`; if you want to get the second page of entries, pass `1`.
        /// </param>
        /// <param name="filter">Restricts the scope of entries to return:
        /// * `0`: None (returns all entries of the specified leaderboard)
        /// * `1`: Friends (returns the entries of the friends of the current logged-in user)
        /// * `2`: Unknown (returns no entry)
        /// * `3`: UserIds (returns the entries of specified users)
        /// </param>
        /// <param name="startAt">Defines where to start returning leaderboard entries, the enumerations are:
        /// * `0`: Top (return entries from top 1)
        /// * `1`: CenteredOnViewer (place the current logged-in user's entry in the middle of the list on the first page.
        /// For example, if the total number of entries is 10, `pageSize` is set to `5`, and the user's rank is top 5, the ranks displayed
        /// on the first page will be top 3, 4, 5, 6, and 7. Top 1 and 2 will not be displayed, and top 8, 9, and 10 will be
        /// displayed on the second page)
        /// * `2`: CenteredOnViewerOrTop (place the current logged-in user's entry on the top of the list on the first page.
        /// For example, if the total number of entries is 10, `pageSize` is set to `5`, and the user's rank is top 5,
        /// the ranks displayed on the first page will be top 5, 6, 7, 8, and 9. Top 1, 2, 3, and 4 will not be displayed,
        /// and top 10 will be displayed on the second page)
        /// * `3`: Unknown (returns an empty list)
        /// </param>
        /// <returns>Request information of type `Task`, including the request ID, and its response message will contain data of type `LeaderboardEntryList`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |3006501|request server failed|
        /// |3006503|checking parameter failed|
        /// |3006504|leaderboard is not exist|
        /// |3006506|load leaderboard data failed|
        /// |3006509|get friend failed|
        /// |3006510|get user account failed|
        ///
        /// A message of type `MessageType.Leaderboard_GetEntries` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `LeaderboardEntryList`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<LeaderboardEntryList> GetEntries(string leaderboardName, int pageSize, int pageIdx, LeaderboardFilterType filter, LeaderboardStartAt startAt)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<LeaderboardEntryList>(CLIB.ppf_Leaderboard_GetEntries(leaderboardName, pageSize, pageIdx, filter, startAt));
        }

        /// <summary>Gets a list of entries after a specified rank.</summary>
        ///
        /// <param name="leaderboardName">The name of the leaderboard whose entries are to be returned.</param>
        /// <param name="pageSize">The number of entries to return on each page.</param>
        /// <param name="pageIdx">Defines which page of entries to return. The first page index is `0`.
        /// For example, if you want to get the first page of entries, pass `0`; if you want to get the second page of entries, pass `1`.
        /// </param>
        /// <param name="afterRank">Defines after which rank to return entries.</param>
        /// <returns>Request information of type `Task`, including the request ID, and its response message will contain data of type `LeaderboardEntryList`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |10701|request server failed|
        /// |10703|checking parameter failed|
        /// |10704|leaderboard is not exist|
        /// |10706|load leaderboard data failed|
        /// |10709|get friend failed|
        /// |10710|get user account failed|
        ///
        /// A message of type `MessageType.Leaderboard_GetEntriesAfterRank` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `LeaderboardEntryList`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<LeaderboardEntryList> GetEntriesAfterRank(string leaderboardName, int pageSize, int pageIdx,
            ulong afterRank)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<LeaderboardEntryList>(
                CLIB.ppf_Leaderboard_GetEntriesAfterRank(leaderboardName, pageSize, pageIdx, afterRank));
        }

        /// <summary>Gets a list of entries for specified users.</summary>
        ///
        /// <param name="leaderboardName">The name of the leaderboard whose entries are to be returned.</param>
        /// <param name="pageSize">The number of entries to return on each page.</param>
        /// <param name="pageIdx">Defines which page of entries to return. The first page index is `0`.
        /// For example, if you want to get the first page of entries, pass `0`; if you want to get the second page of entries, pass `1`.
        /// </param>
        /// <param name="startAt">Defines where to start returning leaderboard entries, the enumerations are:
        /// * `0`: Top (return entries from top 1)
        /// * `1`: CenteredOnViewer (place the current logged-in user's entry in the middle of the list on the first page.
        /// For example, if the total number of entries is 10, `pageSize` is set to `5`, and the user's rank is top 5, the ranks displayed
        /// on the first page will be top 3, 4, 5, 6, and 7. Top 1 and 2 will not be displayed, and top 8, 9, and 10 will be
        /// displayed on the second page)
        /// * `2`: CenteredOnViewerOrTop (place the current logged-in user's entry on the top of the list on the first page.
        /// For example, if the total number of entries is 10, `pageSize` is set to `5`, and the user's rank is top 5,
        /// the ranks displayed on the first page will be top 5, 6, 7, 8, and 9. Top 1, 2, 3, and 4 will not be displayed,
        /// and top 10 will be displayed on the second page)
        /// * `3`: Unknown (returns an empty list)
        /// </param>
        /// <param name="userIDs">The ID list of the users to get entries for.</param>
        /// <returns>Request information of type `Task`, including the request ID, and its response message will contain data of type `LeaderboardEntryList`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |10701|request server failed|
        /// |10703|checking parameter failed|
        /// |10704|leaderboard is not exist|
        /// |10706|load leaderboard data failed|
        /// |10709|get friend failed|
        /// |10710|get user account failed|
        ///
        /// A message of type `MessageType.Leaderboard_GetEntriesByIds` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `LeaderboardEntryList`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<LeaderboardEntryList> GetEntriesByIds(string leaderboardName, int pageSize, int pageIdx,
            LeaderboardStartAt startAt, string[] userIDs)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<LeaderboardEntryList>(CLIB.ppf_Leaderboard_GetEntriesByIds(leaderboardName,
                pageSize, pageIdx, startAt, userIDs));
        }

        /// <summary>Writes an entry to a leaderboard.</summary>
        ///
        /// <param name="leaderboardName">The name of the leaderboard to write an entry to.</param>
        /// <param name="score">The score to write.</param>
        /// <param name="extraData">A 2KB custom data field that is associated with the leaderboard entry. This can be a game replay or anything that provides more details about the entry to the viewer.</param>
        /// <param name="forceUpdate">Defines whether to force update the score. If set to `true`, the score always updates even if it is not the user's best score.</param>
        /// <returns>Request information of type `Task`, including the request ID, and its response message will contain data of type `bool`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |10701|request server failed|
        /// |10703|checking parameter failed|
        /// |10704|leaderboard is not exist|
        /// |10705|no write permission|
        /// |10706|load leaderboard data failed|
        /// |10707|save leaderboard data failed|
        /// |10708|extra data too long|
        /// |10714|out of write time limit|
        ///
        /// A message of type `MessageType.Leaderboard_WriteEntry` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `bool`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<bool> WriteEntry(string leaderboardName, long score, byte[] extraData = null,
            bool forceUpdate = false)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            GCHandle hobj = GCHandle.Alloc(extraData, GCHandleType.Pinned);
            IntPtr pobj = hobj.AddrOfPinnedObject();
            var result = new Task<bool>(CLIB.ppf_Leaderboard_WriteEntry(leaderboardName, score, pobj,
                (uint) (extraData != null ? extraData.Length : 0), forceUpdate));
            if (hobj.IsAllocated)
                hobj.Free();
            return result;
        }

        /// <summary>Writes an entry to a leaderboard. The entry can include the supplementary metric for tiebreakers.</summary>
        ///
        /// <param name="leaderboardName">The name of the leaderboard to write an entry to.</param>
        /// <param name="score">The score to write.</param>
        /// <param name="supplementaryMetric">The metric that can be used for tiebreakers.</param>
        /// <param name="extraData">A 2KB custom data field that is associated with the leaderboard entry. This can be a game replay or anything that provides more details about the entry to the viewer.</param>
        /// <param name="forceUpdate">Defines whether to force update the score. If set to `true`, the score always updates even if it is not the user's best score.</param>
        /// <returns>Request information of type `Task`, including the request ID, and its response message will contain data of type `bool`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |10701|request server failed|
        /// |10703|checking parameter failed|
        /// |10704|leaderboard is not exist|
        /// |10705|no write permission|
        /// |10706|load leaderboard data failed|
        /// |10707|save leaderboard data failed|
        /// |10708|extra data too long|
        /// |10714|out of write time limit|
        ///
        /// A message of type `MessageType.Leaderboard_WriteEntryWithSupplementaryMetric` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `bool`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<bool> WriteEntryWithSupplementaryMetric(string leaderboardName, long score,
            long supplementaryMetric, byte[] extraData = null, bool forceUpdate = false)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            GCHandle hobj = GCHandle.Alloc(extraData, GCHandleType.Pinned);
            IntPtr pobj = hobj.AddrOfPinnedObject();
            var result = new Task<bool>(CLIB.ppf_Leaderboard_WriteEntryWithSupplementaryMetric(leaderboardName, score,
                supplementaryMetric, pobj, (uint) (extraData != null ? extraData.Length : 0), forceUpdate));
            if (hobj.IsAllocated)
                hobj.Free();
            return result;
        }
    }
}