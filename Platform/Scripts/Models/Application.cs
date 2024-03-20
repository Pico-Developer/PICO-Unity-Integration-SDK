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
    /// <summary>App launch details.</summary>
    public class LaunchDetails
    {
        /// How the app was launched: 
        /// * `Normal`: launched by clicking the app's icon
        /// * `RoomInvite`: launched by clicking the room invitation message card
        /// * `Deeplink`: launched by clicking the presence invitation message card or calling \ref ApplicationService.LaunchApp
        /// * `ChallengeInvite`: launched by clicking the challenge invitation message card
        ///
        public readonly LaunchType LaunchType;

        /// Deeplink message. You can pass a deeplink when you call \ref ApplicationService.LaunchApp,
        /// and the other app will receive the deeplink.This field will have a value only when `LaunchType` is `LaunchApp`.
        public readonly string DeeplinkMessage;

        /// Destination API name configured on the PICO Developer Platform.For a presence invitation, the inviters'
        /// presence data will contain this field which will be passed when the invitee clicks on the message card.
        public readonly string DestinationApiName;

        /// The lobby session ID that identifies a group or team.
        /// For a presence invitation, the inviters' presence data will contain this field which will be passed
        /// when the invitee clicks on the message card.
        public readonly string LobbySessionID;

        /// The match session ID that identifies a competition.
        /// For a presence invitation, the inviters' presence data will contain this field which will be passed when the invitee clicks on the message card.
        public readonly string MatchSessionID;

        /** The customized extra presence info.
         * For a presence invitation, the inviters' presence data will contain this field which will be passed when the invitee clicks on the message card.
         * You can use this field to add self-defined presence data. The data size cannot exceed 2MB.
         */
        public readonly string Extra;

        /// Room ID.For a room invitation, after calling \ref RoomService.InviteUser, this field will be passed when the invitee clicks on the message card.
        public readonly UInt64 RoomID;

        /// For a challenge invitation, after calling \ref ChallengesService.Invite, this field will be passed when the invitee clicks on the message card.
        public readonly UInt64 ChallengeID;

        /// Tracking ID. 
        public readonly string TrackingID;

        public LaunchDetails(IntPtr o)
        {
            DeeplinkMessage = CLIB.ppf_LaunchDetails_GetDeeplinkMessage(o);
            DestinationApiName = CLIB.ppf_LaunchDetails_GetDestinationApiName(o);
            LobbySessionID = CLIB.ppf_LaunchDetails_GetLobbySessionID(o);
            MatchSessionID = CLIB.ppf_LaunchDetails_GetMatchSessionID(o);
            Extra = CLIB.ppf_LaunchDetails_GetExtra(o);
            RoomID = CLIB.ppf_LaunchDetails_GetRoomID(o);
            ChallengeID = CLIB.ppf_LaunchDetails_GetChallengeID(o);
            TrackingID = CLIB.ppf_LaunchDetails_GetTrackingID(o);
            LaunchType = CLIB.ppf_LaunchDetails_GetLaunchType(o);
        }
    }

    /// <summary>
    /// The system information of the device.
    /// </summary>
    public class SystemInfo
    {
        /** The current ROM version (i.e., system version) of the device, such as "5.5.0" and "5.6.0".*/
        public readonly string ROMVersion;

        /** The locale of the device. Locale is combined with language and country code. Such as "zh-CN" and "en-US".*/
        public readonly string Locale;

        /** The product name of the device, such as "PICO 4".*/
        public readonly string ProductName;

        /** Whether the device's ROM is CN version. PICO provides different ROM versions in different countries/regions.*/
        public readonly bool IsCnDevice;

        /** The Matrix's version name. Matrix is a system app which provides system functions for platform services.*/
        public readonly string MatrixVersionName;
        
        /** The Matrix's version code. */
        public readonly long MatrixVersionCode;

        public SystemInfo(IntPtr o)
        {
            ROMVersion = CLIB.ppf_SystemInfo_GetROMVersion(o);
            Locale = CLIB.ppf_SystemInfo_GetLocale(o);
            ProductName = CLIB.ppf_SystemInfo_GetProductName(o);
            IsCnDevice = CLIB.ppf_SystemInfo_GetIsCnDevice(o);
            MatrixVersionName = CLIB.ppf_SystemInfo_GetMatrixVersionName(o);
            MatrixVersionCode = CLIB.ppf_SystemInfo_GetMatrixVersionCode(o);
        }
    }

    /// <summary>
    /// App's version info.
    /// </summary>
    public class ApplicationVersion
    {
        /// The current version code of the installed app. 
        public readonly long CurrentCode;

        /// The current version name of the installed app. 
        public readonly string CurrentName;

        /// The latest version code of the app in the PICO Store. 
        public readonly long LatestCode;

        /// The latest version name of the app in the PICO Store. 
        public readonly string LatestName;

        public ApplicationVersion(IntPtr o)
        {
            CurrentCode = CLIB.ppf_ApplicationVersion_GetCurrentCode(o);
            CurrentName = CLIB.ppf_ApplicationVersion_GetCurrentName(o);
            LatestCode = CLIB.ppf_ApplicationVersion_GetLatestCode(o);
            LatestName = CLIB.ppf_ApplicationVersion_GetLatestName(o);
        }
    }
}