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

    /// <summary>
    /// The summary of daily sport info.
    /// Users' daily sports info is recorded in the local database. This structure indicates the sports info generated someday.
    /// </summary>
    public class SportDailySummary
    {
        /// The ID of the summary.
        public readonly long Id;

        /// The date when the summary was generated.
        public readonly DateTime Date;

        /// The sport duration (in seconds).
        public readonly int DurationInSeconds;

        /// The planned sport duration (in seconds).
        public readonly int PlanDurationInMinutes;

        /// The actual calorie burnt (in kilo calorie).
        public readonly double Calorie;

        /// The planned calorie to burn. 
        public readonly double PlanCalorie;

        public SportDailySummary(IntPtr o)
        {
            Id = CLIB.ppf_SportDailySummary_GetId(o);
            Date = TimeUtil.MilliSecondsToDateTime(CLIB.ppf_SportDailySummary_GetDate(o));
            DurationInSeconds = CLIB.ppf_SportDailySummary_GetDurationInSeconds(o);
            PlanDurationInMinutes = CLIB.ppf_SportDailySummary_GetPlanDurationInMinutes(o);
            Calorie = CLIB.ppf_SportDailySummary_GetCalorie(o);
            PlanCalorie = CLIB.ppf_SportDailySummary_GetPlanCalorie(o);
        }
    }

    /// <summary>
    /// Each element is \ref SportDailySummary
    /// </summary>
    public class SportDailySummaryList : MessageArray<SportDailySummary>
    {
        public SportDailySummaryList(IntPtr a)
        {
            var count = (int) CLIB.ppf_SportDailySummaryArray_GetSize(a);
            this.Capacity = count;
            for (int i = 0; i < count; i++)
            {
                this.Add(new SportDailySummary(CLIB.ppf_SportDailySummaryArray_GetElement(a, (UIntPtr) i)));
            }
        }
    }

    /// <summary>
    /// User's sport summary of today.
    /// </summary>
    public class SportSummary
    {
        /// The sport duration (in seconds).
        public readonly int DurationInSeconds;

        /// The calorie burnt (in kilo calorie).
        public readonly double Calorie;

        /// The time when the user started playing sport.
        public readonly DateTime StartTime;

        /// The time when the user stopped playing sport.
        public readonly DateTime EndTime;

        public SportSummary(IntPtr o)
        {
            DurationInSeconds = (int) CLIB.ppf_SportSummary_GetDurationInSeconds(o);
            Calorie = CLIB.ppf_SportSummary_GetCalorie(o);
            StartTime = TimeUtil.MilliSecondsToDateTime(CLIB.ppf_SportSummary_GetStartTime(o));
            EndTime = TimeUtil.MilliSecondsToDateTime(CLIB.ppf_SportSummary_GetEndTime(o));
        }
    }

    /// <summary>
    /// The user's sport info.
    /// User can set sport goal in the Sport Center app.
    /// </summary>
    public class SportUserInfo
    {
        public readonly Gender Gender;
        public readonly DateTime Birthday;

        /// The height of the user (in cm). 
        public readonly int Stature;

        /// The weight of the user (in kg). 
        public readonly int Weight;

        /// The sport level that indicates the intensity of the sport.
        public readonly int SportLevel;

        /// The planned daily sport duration (in minutes).
        public readonly int DailyDurationInMinutes;

        /// The planned weekly sport days.
        public readonly int DaysPerWeek;

        /// The sport purpose, such as `keep fit` and `lose weight`. 
        public readonly SportTarget SportTarget;

        public SportUserInfo(IntPtr o)
        {
            Gender = CLIB.ppf_SportUserInfo_GetGender(o);
            Birthday = TimeUtil.MilliSecondsToDateTime(CLIB.ppf_SportUserInfo_GetBirthday(o));
            Stature = CLIB.ppf_SportUserInfo_GetStature(o);
            Weight = CLIB.ppf_SportUserInfo_GetWeight(o);
            SportLevel = CLIB.ppf_SportUserInfo_GetSportLevel(o);
            DailyDurationInMinutes = CLIB.ppf_SportUserInfo_GetDailyDurationInMinutes(o);
            DaysPerWeek = CLIB.ppf_SportUserInfo_GetDaysPerWeek(o);
            SportTarget = CLIB.ppf_SportUserInfo_GetSportTarget(o);
        }
    }
}