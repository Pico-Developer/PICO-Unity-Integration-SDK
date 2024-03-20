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
using UnityEngine;

namespace Pico.Platform.Models
{
    /// <summary>Challenge setting options.</summary>
    public class ChallengeOptions
    {
        /// For creating challenge options 
        public ChallengeOptions()
        {
            Handle = CLIB.ppf_ChallengeOptions_Create();
        }

        /// Set the end date. Currently, not used. 
        public void SetEndDate(DateTime value)
        {
            CLIB.ppf_ChallengeOptions_SetEndDate(Handle, Convert.ToUInt64(TimeUtil.DateTimeToSeconds(value)));
        }

        /// Set whether to get active challenges. 
        public void SetIncludeActiveChallenges(bool value)
        {
            CLIB.ppf_ChallengeOptions_SetIncludeActiveChallenges(Handle, value);
        }

        /// Set whether to get future challenges whose start dates are latter than the current time. 
        public void SetIncludeFutureChallenges(bool value)
        {
            CLIB.ppf_ChallengeOptions_SetIncludeFutureChallenges(Handle, value);
        }

        /// Set whether to get past challenges whose end dates are earlier than the current time. 
        public void SetIncludePastChallenges(bool value)
        {
            CLIB.ppf_ChallengeOptions_SetIncludePastChallenges(Handle, value);
        }

        /// (Optional) Set the name of the leaderboard that the challenges associated with. 
        public void SetLeaderboardName(string value)
        {
            CLIB.ppf_ChallengeOptions_SetLeaderboardName(Handle, value);
        }

        /// Set the start date. Currently, not used. 
        public void SetStartDate(DateTime value)
        {
            CLIB.ppf_ChallengeOptions_SetStartDate(Handle, Convert.ToUInt64(TimeUtil.DateTimeToSeconds(value)));
        }

        /// Set the challenge title. Currently, not used. 
        public void SetTitle(string value)
        {
            CLIB.ppf_ChallengeOptions_SetTitle(Handle, value);
        }

        /// Set the filter for quering specified challenges. 
        public void SetViewerFilter(ChallengeViewerFilter value)
        {
            CLIB.ppf_ChallengeOptions_SetViewerFilter(Handle, value);
        }

        /// Set to get the challenges of a specific visibility type. 
        public void SetVisibility(ChallengeVisibility value)
        {
            CLIB.ppf_ChallengeOptions_SetVisibility(Handle, value);
        }

        public static explicit operator IntPtr(ChallengeOptions options)
        {
            return options != null ? options.Handle : IntPtr.Zero;
        }

        ~ChallengeOptions()
        {
            CLIB.ppf_ChallengeOptions_Destroy(Handle);
        }

        IntPtr Handle;

        public IntPtr GetHandle()
        {
            return Handle;
        }
    }

    /// <summary>Challenge info.</summary>
    public class Challenge
    {
        /// The creator of the challenge. 
        public readonly ChallengeCreationType CreationType;

        /// Challenge ID 
        public readonly UInt64 ID;

        /// Challenge's start date. 
        public readonly DateTime StartDate;

        /// Challenge's end date. 
        public readonly DateTime EndDate;

        /// Participants of the challenge, which might be null. Should check if it is null before use. 
        public readonly UserList ParticipantsOptional;

        /// Users invited to the challenge, which might be null. Should check if it is null before use. 
        public readonly UserList InvitedUsersOptional;

        /// The info about the leaderboard that the challenge associated with. 
        public readonly Leaderboard Leaderboard;

        /// Challenge's title. 
        public readonly string Title;

        /// Challenge's visibility. 
        public readonly ChallengeVisibility Visibility;


        public Challenge(IntPtr o)
        {
            CreationType = CLIB.ppf_Challenge_GetCreationType(o);

            try
            {
                EndDate = TimeUtil.SecondsToDateTime((long) CLIB.ppf_Challenge_GetEndDate(o));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Challenge Set EndDate: ppf_Challenge_GetEndDate(o) = {CLIB.ppf_Challenge_GetEndDate(o)}, Exception: {e}");
            }

            ID = CLIB.ppf_Challenge_GetID(o);
            {
                var pointer = CLIB.ppf_Challenge_GetInvitedUsers(o);
                if (pointer == IntPtr.Zero)
                {
                    InvitedUsersOptional = null;
                }
                else
                {
                    InvitedUsersOptional = new UserList(pointer);
                }
            }
            Leaderboard = new Leaderboard(CLIB.ppf_Challenge_GetLeaderboard(o));
            {
                var pointer = CLIB.ppf_Challenge_GetParticipants(o);
                if (pointer == IntPtr.Zero)
                {
                    ParticipantsOptional = null;
                }
                else
                {
                    ParticipantsOptional = new UserList(pointer);
                }
            }
            try
            {
                StartDate = TimeUtil.SecondsToDateTime((long) CLIB.ppf_Challenge_GetStartDate(o));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Challenge Set StartDate: ppf_Challenge_GetStartDate(o) = {CLIB.ppf_Challenge_GetStartDate(o)}, Exception: {e}");
            }

            Title = CLIB.ppf_Challenge_GetTitle(o);
            Visibility = CLIB.ppf_Challenge_GetVisibility(o);
        }
    }

    /// <summary>Challenge list. Each Element is \ref Challenge.</summary>
    public class ChallengeList : MessageArray<Challenge>
    {
        public ChallengeList(IntPtr a)
        {
            TotalCount = CLIB.ppf_ChallengeArray_GetTotalCount(a);
            NextPageParam = CLIB.ppf_ChallengeArray_HasNextPage(a) ? "true" : string.Empty;
            PreviousPageParam = CLIB.ppf_ChallengeArray_HasPreviousPage(a) ? "true" : String.Empty;
            int count = (int) CLIB.ppf_ChallengeArray_GetSize(a);
            this.Capacity = count;
            for (uint i = 0; i < count; i++)
            {
                this.Add(new Challenge(CLIB.ppf_ChallengeArray_GetElement(a, (UIntPtr) i)));
            }
        }

        /// The total number of challenges in the list. 
        public readonly ulong TotalCount;
    }

    /// <summary>Challenge entry info.</summary>
    public class ChallengeEntry
    {
        /// The entry's display score. 
        public readonly string DisplayScore;

        /// The entry's additional info, no more than 2KB. 
        public readonly byte[] ExtraData;

        /// The ID of the challenge that the entry belongs to. 
        public readonly UInt64 ID;

        /// The rank of the entry. 
        public readonly int Rank;

        /// The score of the entry. 
        public readonly long Score;

        /// The time when the entry was written. 
        public readonly DateTime Timestamp;

        /// The user the entry belongs to. 
        public readonly User User;


        public ChallengeEntry(IntPtr o)
        {
            DisplayScore = CLIB.ppf_ChallengeEntry_GetDisplayScore(o);
            var extraDataPtr = CLIB.ppf_ChallengeEntry_GetExtraData(o);
            var extraDataSize = CLIB.ppf_ChallengeEntry_GetExtraDataLength(o);
            ExtraData = MarshalUtil.ByteArrayFromNative(extraDataPtr, extraDataSize);
            ID = CLIB.ppf_ChallengeEntry_GetID(o);
            Rank = CLIB.ppf_ChallengeEntry_GetRank(o);
            Score = CLIB.ppf_ChallengeEntry_GetScore(o);

            try
            {
                Timestamp = TimeUtil.SecondsToDateTime((long) CLIB.ppf_ChallengeEntry_GetTimestamp(o));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"ChallengeEntry Set Timestamp: ppf_ChallengeEntry_GetTimestamp(o) = {CLIB.ppf_ChallengeEntry_GetTimestamp(o)}, Exception: {e}");
            }

            User = new User(CLIB.ppf_ChallengeEntry_GetUser(o));
        }
    }

    /// <summary>Challenge entry list. Each element is \ref ChallengeEntry.</summary>
    public class ChallengeEntryList : MessageArray<ChallengeEntry>
    {
        public ChallengeEntryList(IntPtr a)
        {
            TotalCount = CLIB.ppf_ChallengeEntryArray_GetTotalCount(a);
            NextPageParam = CLIB.ppf_ChallengeEntryArray_HasNextPage(a) ? "true" : string.Empty;
            PreviousPageParam = CLIB.ppf_ChallengeEntryArray_HasPreviousPage(a) ? "true" : string.Empty;
            int count = (int) CLIB.ppf_ChallengeEntryArray_GetSize(a);
            this.Capacity = count;
            for (uint i = 0; i < count; i++)
            {
                this.Add(new ChallengeEntry(CLIB.ppf_ChallengeEntryArray_GetElement(a, (UIntPtr) i)));
            }
        }

        /// The total number of entries in the list. 
        public readonly ulong TotalCount;
    }
}