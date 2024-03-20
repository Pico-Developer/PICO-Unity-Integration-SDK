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

    /// <summary>Leaderboard info.</summary>
    public class Leaderboard
    {
        /// The unique identifier of the leaderboard, which is configured on the PICO Developer Platform. 
        public readonly string ApiName;

        /// Leaderboard ID. 
        public readonly ulong ID;

        /** Associate a destination to the leaderboard so that users can be directed to a specific location in the app.
         * If the leaderboard for that challenge is associated with a destination, the app will be launched, and the user will be directed to the destination.
         * If the leaderboard for that challenge is not associated with any destination, the app will be launched, and the user will be directed to the Home page.
         */
        public readonly Destination DestinationOptional;

        public Leaderboard(IntPtr o)
        {
            ApiName = CLIB.ppf_Leaderboard_GetApiName(o);
            ID = CLIB.ppf_Leaderboard_GetID(o);
            var pointer = CLIB.ppf_Leaderboard_GetDestination(o);
            if (pointer == IntPtr.Zero)
                DestinationOptional = null;
            else
                DestinationOptional = new Destination(pointer);
        }
    }

    /// <summary>Leaderboard list. Each element is \ref Leaderboard.</summary>
    public class LeaderboardList : MessageArray<Leaderboard>
    {
        /// The total number of leaderboards in the list. 
        public readonly ulong TotalCount;
        public LeaderboardList(IntPtr a)
        
        {
            TotalCount = CLIB.ppf_LeaderboardArray_GetTotalCount(a);
            NextPageParam = CLIB.ppf_LeaderboardArray_HasNextPage(a) ? "true" : string.Empty;
            var count = (int) CLIB.ppf_LeaderboardArray_GetSize(a);
            this.Capacity = count;
            for (var i = 0; i < count; i++)
            {
                Add(new Leaderboard(CLIB.ppf_LeaderboardArray_GetElement(a, (UIntPtr) i)));
            }
        }
    }

    /// <summary>Supplementary metric.</summary>
    public class SupplementaryMetric
    {
        /// The ID of the supplementary metric. 
        public readonly UInt64 ID;
        /// The value of the supplementary metric. 
        public readonly long Metric;


        public SupplementaryMetric(IntPtr o)
        {
            ID = CLIB.ppf_SupplementaryMetric_GetID(o);
            Metric = CLIB.ppf_SupplementaryMetric_GetMetric(o);
        }
    }

    /// <summary>Leaderboard entry info.</summary> 
    public class LeaderboardEntry
    {
        /// The entry's display score. 
        public readonly string DisplayScore;
        /// Additional info, no more than 2KB. 
        public readonly byte[] ExtraData;
        /// Entry ID. 
        public readonly UInt64 ID;
        /// The entry's ranking on the leaderboard. For example, returns `1` for top1.
        public readonly int Rank;
        /// The score used to rank the entry. 
        public readonly long Score;
        /// The supplementary metric used for tiebreakers. This field can be null. Need to check whether it is null before use. 
        public readonly SupplementaryMetric SupplementaryMetricOptional;
        /// The time when the entry was written to the leaderboard. 
        public readonly DateTime Timestamp;
        /// The user the entry belongs to. 
        public readonly User User;


        public LeaderboardEntry(IntPtr o)
        {
            DisplayScore = CLIB.ppf_LeaderboardEntry_GetDisplayScore(o);
            var extraDataPtr = CLIB.ppf_LeaderboardEntry_GetExtraData(o);
            var extraDataSize = CLIB.ppf_LeaderboardEntry_GetExtraDataLength(o);
            ExtraData = MarshalUtil.ByteArrayFromNative(extraDataPtr, extraDataSize);
            ID = CLIB.ppf_LeaderboardEntry_GetID(o);
            Rank = CLIB.ppf_LeaderboardEntry_GetRank(o);
            Score = CLIB.ppf_LeaderboardEntry_GetScore(o);
            Timestamp = TimeUtil.SecondsToDateTime((long) CLIB.ppf_LeaderboardEntry_GetTimestamp(o));
            User = new User(CLIB.ppf_LeaderboardEntry_GetUser(o));
            {
                var pointer = CLIB.ppf_LeaderboardEntry_GetSupplementaryMetric(o);
                if (pointer == IntPtr.Zero)
                {
                    SupplementaryMetricOptional = null;
                }
                else
                {
                    SupplementaryMetricOptional = new SupplementaryMetric(pointer);
                }
            }
        }
    }

    /// <summary>Leaderboard entry list. Each element is \ref LeaderboardEntry.</summary>
    public class LeaderboardEntryList : MessageArray<LeaderboardEntry>
    {
        /// The total number of entries on the leaderboard. 
        public readonly ulong TotalCount;

        public LeaderboardEntryList(IntPtr a)
        {
            NextPageParam = CLIB.ppf_LeaderboardEntryArray_HasNextPage(a) ? "true" : string.Empty;
            PreviousPageParam = CLIB.ppf_LeaderboardEntryArray_HasPreviousPage(a) ? "true" : string.Empty;
            var count = (int) CLIB.ppf_LeaderboardEntryArray_GetSize(a); 
            this.Capacity = count;
            for (uint i = 0; i < count; i++)
            {
                this.Add(new LeaderboardEntry(CLIB.ppf_LeaderboardEntryArray_GetElement(a, (UIntPtr) i)));
            }

            TotalCount = CLIB.ppf_LeaderboardEntryArray_GetTotalCount(a);
        }
    }
}