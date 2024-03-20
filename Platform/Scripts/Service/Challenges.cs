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
     * Challenges create fun-to-join competitions among users, which
     * can therefore provide users with more opportunities to interact
     * with others. Challenges are asynchronous events, so users do not
     * have to be online and do challenges at the same time.

     * Both you and your app's users are able to create challenges,
     * configure challenge settings (including name, visibility, start
     * time, and end time), and invite friends to join challenges to
     * have fun together. Users can also join the challenges created
     * by PICO.
     */
    public static class ChallengesService
    {
        /// <summary>Invites specified user(s).</summary>
        /// <param name="challengeID">The ID of the challenge to which user(s) are invited.</param>
        /// <param name="userID">The ID(s) of the user(s) to invite.</param>
        /// <returns>Returns the `Challenge` struct that contains the information about the challenge,
        /// such as challenge ID, the leaderboard the challenge belongs to, the challenge's end date and start date, etc.</returns>
        public static Task<Challenge> Invite(UInt64 challengeID, string[] userID)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<Challenge>(CLIB.ppf_Challenges_Invites(challengeID, userID));
        }


        /// <summary>Gets the information for a specified challenge.</summary>
        /// <param name="challengeID">The ID of the challenge to get information for.</param>
        /// <returns>Returns the `Challenge` struct that contains the information about the challenge,
        /// such as challenge ID, the leaderboard the challenge belongs to, the challenge's end date and start date, etc.</returns>
        public static Task<Challenge> Get(UInt64 challengeID)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<Challenge>(CLIB.ppf_Challenges_Get(challengeID));
        }

        /// <summary>Gets a list of challenge entries.</summary>
        /// <param name="challengeID">The ID of the challenge whose entries are to be returned.</param>
        /// <param name="filter">Restricts the scope of entries to return:
        /// * `0`: None (returns all entries of the specified leaderboard)
        /// * `1`: Friends (returns the entries of the friends of the current logged-in user)
        /// * `2`: Unknown (returns no entry)
        /// * `3`: UserIds (returns the entries of specified users)
        /// </param>
        /// <param name="startAt">Defines where to start returning challenge entries, the enumerations are:
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
        /// <param name="pageIdx">Defines which page of entries to return. The first page index is `0`.
        /// For example, if you want to get the first page of entries, pass `0`; if you want to get the second page of entries, pass `1`.
        /// </param>
        /// <param name="pageSize">Defines the number of entries to return on the page.</param>
        /// <returns>Returns a list of matching entries.</returns>
        public static Task<ChallengeEntryList> GetEntries(UInt64 challengeID,
            LeaderboardFilterType filter, LeaderboardStartAt startAt, int pageIdx, int pageSize)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<ChallengeEntryList>(
                CLIB.ppf_Challenges_GetEntries(challengeID, filter, startAt, pageIdx, pageSize));
        }

        /// <summary>Gets a list of challenge entries after a specified rank.</summary>
        /// <param name="challengeID">The ID of the challenge whose entries are to be returned.</param>
        /// <param name="afterRank">Defines the rank after which the entries are to be returned.</param>
        /// <param name="pageIdx">Defines which page of entries to return. The first page index is `0`.
        /// For example, if you want to get the first page of entries, pass `0`; if you want to get the second page of entries, pass `1`.
        /// </param>
        /// <param name="pageSize">Defines the number of entries to return on each page.</param>
        /// <returns>Returns a list of matching entries.</returns>
        public static Task<ChallengeEntryList> GetEntriesAfterRank(UInt64 challengeID,
            ulong afterRank, int pageIdx, int pageSize)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<ChallengeEntryList>(
                CLIB.ppf_Challenges_GetEntriesAfterRank(challengeID, afterRank, pageIdx, pageSize));
        }

        /// <summary>Gets a list of challenge entries for specified users.</summary>
        /// <param name="challengeID">The ID of the challenge whose entries are to be returned.</param>
        /// <param name="startAt">Defines where to start returning challenge entries, the enumerations are:
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
        /// <param name="userIDs">Defines a list of user IDs to get entries for.</param>
        /// <param name="pageIdx">Defines which page of entries to return. The first page index is `0`.
        /// For example, if you want to get the first page of entries, pass `0`; if you want to get the second page of entries, pass `1`.
        /// </param>
        /// <param name="pageSize">Defines the number of entries to return on each page.</param>
        /// <returns>Returns a list of matching entries.</returns>
        public static Task<ChallengeEntryList> GetEntriesByIds(UInt64 challengeID,
            LeaderboardStartAt startAt, string[] userIDs, int pageIdx, int pageSize)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<ChallengeEntryList>(CLIB.ppf_Challenges_GetEntriesByIds(challengeID, startAt, userIDs, pageIdx, pageSize));
        }

        /// <summary>Gets a list of challenges.</summary>
        /// <param name="challengeOptions">Restricts the scope of challenges to return. You can define the start date and
        /// end date of challenges, the leaderboard the challenges belong to, etc.
        /// </param>
        /// <param name="pageIdx">Defines which page of challenges to return. The first page index is `0`.
        /// For example, if you want to get the first page of entries, pass `0`; if you want to get the second page of entries, pass `1`.
        /// </param>
        /// <param name="pageSize">Defines the number of challenges to return on each page.</param>
        /// <returns>Returns a list of matching challenges.</returns>
        public static Task<ChallengeList> GetList(ChallengeOptions challengeOptions, int pageIdx, int pageSize)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<ChallengeList>(CLIB.ppf_Challenges_GetList((IntPtr) challengeOptions, pageIdx, pageSize));
        }

        /// <summary>Lets the current user join a challenge.</summary>
        /// <param name="challengeID">The ID of the challenge to join.</param>
        /// <returns>Returns the `Challenge` struct that contains the information about the challenge,
        /// such as challenge ID, the leaderboard the challenge belongs to, the challenge's end date and start date, etc.</returns>
        public static Task<Challenge> Join(UInt64 challengeID)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<Challenge>(CLIB.ppf_Challenges_Join(challengeID));
        }

        /// <summary>Lets the current user leave a challenge.</summary>
        /// <param name="challengeID">The ID of the challenge to leave.</param>
        /// <returns>Returns the `Challenge` struct that contains the information about the challenge,
        /// such as challenge ID, the leaderboard the challenge belongs to, the challenge's end date and start date, etc.</returns>
        public static Task<Challenge> Leave(UInt64 challengeID)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<Challenge>(CLIB.ppf_Challenges_Leave(challengeID));
        }

        /// <summary>Launches the invitation flow to let the current user invite friends to a specified challenge.
        /// This launches the system default invite UI where all of the user's friends are displayed.
        /// This is intended to be a shortcut for developers not wanting to build their own invite-friends UI.</summary>
        /// <param name="challengeID">The ID of the challenge.</param>
        public static Task LaunchInvitableUserFlow(UInt64 challengeID)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task(CLIB.ppf_Challenges_LaunchInvitableUserFlow(challengeID));
        }
        
        /// <summary>Sets the callback to get notified when the user has accepted an invitation.
        /// @note You can get the ChallengeID by 'Message.Data'. 
        /// </summary>
        /// <param name="handler">The callback function will be called when receiving the `Notification_Challenge_LaunchByInvite` message.</param>
        public static void SetChallengeInviteAcceptedOrLaunchAppNotificationCallback(Message<string>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Challenge_LaunchByInvite, handler);
        }
    }
}