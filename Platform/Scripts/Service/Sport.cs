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
     * SportService provides multiple APIs for you to access
     * users' exercise data from the built-in PICO app — PICO Fitness.

     * When users are working out with PICO VR headsets, the app records
     * their exercise data, including exercise duration, calories burned,
     * exercise plan, preferences, and more.

     * With the APIs provided by the service, you can gather data to
     * understand the exercise habits of individuals, thereby providing
     * users with a better exercise experience.
     */
    public static class SportService
    {
        /// <summary>
        /// Gets a user's basic information and exercise plan.
        /// </summary>
        /// <returns>The \ref Pico.Platform.Models.SportUserInfo class containing the following:
        /// * `Gender`
        /// * `Birthday`
        /// * `Stature`: The natural height in centimeters.
        /// * `Weight`: The weight in kilograms.
        /// * `SportLevel`: `1`-low; `2`-medium; `3`-high.
        /// * `DailyDurationInMinutes`: The planned daily exercise duration (in minutes).
        /// * `DaysPerWeek`: The planned days for exercise per week.
        /// * `SportTarget`: "lose weight" or "stay healthy".
        /// </returns>
        public static Task<SportUserInfo> GetUserInfo()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<SportUserInfo>(CLIB.ppf_Sport_GetUserInfo());
        }

        /// <summary>
        /// Gets a summary of the user's daily exercise data for a specified period within the recent 90 days.
        /// For example, if the period you set is between 2022/08/16 and 2022/08/18, the exercise data generated on 08/16, 08/17, and 08/18 will be returned.
        /// </summary>
        /// <param name="beginTime">A DateTime struct defining the begin time of the period. The begin time should be no earlier than 90 days before the current time.</param>
        /// <param name="endTime">A DateTime struct defining the end time of the period, .</param>
        /// <returns>The \ref Pico.Platform.Models.SportDailySummaryList class containing the exercise data generated on each day within the specified period, including:
        /// * `Id`: Summary ID.
        /// * `Date`: The date when the data was generated.
        /// * `DurationInSeconds`: The actual daily exercise duration in seconds.
        /// * `PlanDurationInMinutes`: The planned daily exercise duration in minutes.
        /// * `Calorie`: The actual daily calorie burned.
        /// * `PlanCalorie`: The planned daily calorie to burn.
        /// </returns>
        public static Task<SportDailySummaryList> GetDailySummary(DateTime beginTime, DateTime endTime)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<SportDailySummaryList>(CLIB.ppf_Sport_GetDailySummary(TimeUtil.DateTimeToMilliSeconds(beginTime), TimeUtil.DateTimeToMilliSeconds(endTime)));
        }

        /// <summary>
        /// Get a summary of the user's exercise data for a specified period within
        /// the recent 24 hours. The period should not exceed 24 hours.
        /// </summary>
        /// <param name="beginTime">A DateTime struct defining the begin time of the period. The begin time should be no earlier than 24 hours before the current time.</param>
        /// <param name="endTime">A DateTime struct defining the end time of the period.</param>
        /// <returns>The \ref Pico.Platform.Models.SportSummary class containing the following:
        /// * `DurationInSeconds`: The actual exercise duration.
        /// * `Calorie`: The actual calorie burned.
        /// * `StartTime`: The start time you defined.
        /// * `EndTime`: The end time you defined.
        /// </returns>
        public static Task<SportSummary> GetSummary(DateTime beginTime, DateTime endTime)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<SportSummary>(CLIB.ppf_Sport_GetSummary(TimeUtil.DateTimeToMilliSeconds(beginTime), TimeUtil.DateTimeToMilliSeconds(endTime)));
        }

    }
}