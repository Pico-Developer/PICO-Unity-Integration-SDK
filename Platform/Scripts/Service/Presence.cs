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
using System.Collections.Generic;
using Pico.Platform.Models;
using UnityEngine;

namespace Pico.Platform
{
    /**
     * \ingroup Platform
     */
    public static class PresenceService
    {
        /// <summary>
        /// Gets a list of invitable users for the current logged-in user.
        /// @note Currently, only invitable friends will be returned.
        /// </summary>
        /// <param name="options">Restricts the scope of friends returned. If no user ID is passed, all friends will
        /// be returned. If specific user IDs are passed, the information about specified friends will be returned.
        /// </param>
        /// <returns>
        /// A list of friends that can be invited to the current destination.
        /// </returns>
        public static Task<UserList> GetInvitableUsers(InviteOptions options)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<UserList>(CLIB.ppf_Presence_GetInvitableUsers((IntPtr) options));
        }

        /// <summary>
        /// Gets a list of invited users for the current logged-in user.
        /// You need set Presence before call this function.
        /// </summary>
        /// <returns>
        /// A list of users that have been invited.
        /// </returns>
        public static Task<ApplicationInviteList> GetSentInvites()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<ApplicationInviteList>(CLIB.ppf_Presence_GetSentInvites());
        }

        /// <summary>
        /// Get the next page of invited users.
        /// </summary>
        /// <param name="list">The current page of invited users.</param>
        /// <returns>The next page of invited users.</returns>
        public static Task<ApplicationInviteList> GetNextApplicationInviteListPage(ApplicationInviteList list)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            if (!list.HasNextPage)
            {
                Debug.LogWarning("GetNextApplicationInviteListPage: List has no next page");
                return null;
            }

            if (!String.IsNullOrEmpty(list.NextPageParam))
            {
                Debug.LogWarning("GetNextApplicationInviteListPage: list.NextPageParam is empty");
                return null;
            }

            return new Task<ApplicationInviteList>(CLIB.ppf_Presence_GetNextApplicationInviteArrayPage(list.NextPageParam));
        }

        /// <summary>
        /// Invites specified user(s) to the current destination.
        /// </summary>
        /// <param name="userIds">The ID(s) of the user(s) to invite.</param>
        public static Task<SendInvitesResult> SendInvites(string[] userIds)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            if (userIds == null)
                userIds = Array.Empty<string>();
            return new Task<SendInvitesResult>(CLIB.ppf_Presence_SendInvites(userIds));
        }

        /// <summary>Sets presence data for the current logged-in user.</summary>
        /// <param name="options">Presence-related options, including:
        /// * `DestinationApiName`: string, the API name of the destination.
        /// * `IsJoinable`: bool,
        ///   * `true`: joinable
        ///   * `false`: not joinable
        /// * `LobbySessionId`: string, a lobby session ID identifies a user group or team. Users with the same lobby session ID can play together or form a team in a game.
        /// * `MatchSessionId`: string, a match session ID identifies all users within a same destination, such as a map or a level. Users with different lobby session IDs will have the same match session ID when playing the same match.
        /// * `Extra`: string, extra presence data defined by the developer.
        /// </param>
        public static Task Set(PresenceOptions options)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task(CLIB.ppf_Presence_Set((IntPtr) options));
        }

        /// @deprecated SetDestination can be replaced by \ref Set()
        /// <summary>
        /// Replaces the current logged-in user's destination with the provided one.
        /// @note Other presence parameter settings will remain the same.
        /// </summary>
        /// <param name="apiName">The API name of the new destination.</param>
        [Obsolete("SetDestination can be replaced by Set()", false)]
        public static Task SetDestination(string apiName)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task(CLIB.ppf_Presence_SetDestination(apiName));
        }

        /// @deprecated SetIsJoinable can be replaced by \ref Set()
        /// <summary>Sets whether the current logged-in user is joinable.
        /// @note Other presence parameter settings will remain the same. If the user's destination or session
        /// ID has not been set, the user cannot be set as joinable.</summary>
        /// <param name="joinable">Defines whether the user is joinable:
        /// * `true`: joinable
        /// * `false`: not joinable
        /// </param>
        [Obsolete("SetIsJoinable can be replaced by Set()", false)]
        public static Task SetIsJoinable(bool joinable)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task(CLIB.ppf_Presence_SetIsJoinable(joinable));
        }

        /// @deprecated SetLobbySession can be replaced by \ref Set()
        /// <summary>
        /// Replaces the current logged-in user's lobby session ID with the provided one.
        /// @note Other presence parameter settings will remain the same.
        /// </summary>
        /// <param name="lobbySessionId">The new lobby session ID.</param>
        [Obsolete("SetLobbySession can be replaced by Set()", false)]
        public static Task SetLobbySession(string lobbySessionId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task(CLIB.ppf_Presence_SetLobbySession(lobbySessionId));
        }

        /// @deprecated SetMatchSession can be replaced by \ref Set()
        /// <summary>
        /// Replaces the current logged-in user's match session ID with the provided one.
        /// @note  Other presence parameter settings will remain the same.
        /// </summary>
        /// <param name="matchSessionId">The new match session ID.</param>
        [Obsolete("SetMatchSession can be replaced by Set()", false)]
        public static Task SetMatchSession(string matchSessionId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task(CLIB.ppf_Presence_SetMatchSession(matchSessionId));
        }

        /// @deprecated SetExtra can be replaced by \ref Set()
        /// <summary>
        /// Sets extra presence data for the current logged-in user.
        /// </summary>
        /// <param name="extra">The extra presence data, which is defined by the developer and will be returned in the user's presence information.</param>
        [Obsolete("SetExtra can be replaced by Set()", false)]
        public static Task SetExtra(string extra)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task(CLIB.ppf_Presence_SetExtra(extra));
        }

        /// <summary>
        /// Clears presence data for the current logged-in user.
        /// @note You need to clear a user's presence data when the user exits your app, leaves a specific destination within the app, or does not want others to see their destination and status.
        /// </summary>
        public static Task Clear()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task(CLIB.ppf_Presence_Clear());
        }

        /// <summary>
        /// Gets a list of destinations created on the PICO Developer Platform.
        /// </summary>
        /// <returns>The list of destinations.</returns>
        public static Task<DestinationList> GetDestinations()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<DestinationList>(CLIB.ppf_Presence_GetDestinations());
        }

        /// <summary>
        /// Gets the next page of destinations.
        /// </summary>
        /// <param name="destinationList">The current page of destinations.</param>
        /// <returns>The next page of destinations.</returns>
        public static Task<DestinationList> GetNextDestinationListPage(DestinationList destinationList)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<DestinationList>(CLIB.ppf_Presence_GetNextDestinationArrayPage(destinationList.NextPageParam));
        }

        /// <summary>
        /// Launches the invite panel provided in the PICO Friends app. Users can invite other people on the panel.
        /// @note Before calling this method, you should set presence data correctly.
        /// </summary>
        /// <returns>Returns a message. Check `Message.Error` to see whether the panel has been successfully launched.</returns>
        public static Task LaunchInvitePanel()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task(CLIB.ppf_Presence_LaunchInvitePanel());
        }

        /// <summary>
        /// Shares a video made of images to Douyin (a video app in Mainland China).
        /// @note Available in Mainland China only.
        /// </summary>
        /// <param name="imagePaths">The local path to images.</param>
        /// <returns>Returns a message. Check `Message.Error` to see whether the video has been successfully shared.</returns>
        public static Task ShareVideoByImages(List<string> imagePaths)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            var options = new ShareMediaOptions();
            foreach (var imagePath in imagePaths)
            {
                options.AddImagePath(imagePath);
            }

            options.SetShareMediaType(ShareMediaType.Image);
            return new Task(CLIB.ppf_Presence_ShareMedia((IntPtr) options));
        }

        /// <summary>
        /// Shares a video to Douyin (a video app in Mainland China).
        /// @note Available in Mainland China only.
        /// </summary>
        /// <param name="videoPath">The local path to the video.</param>
        /// <param name="videoThumbPath">The local path to the video thumbnail.
        /// If not defined, the first frame of the video will become the thumbnail.
        /// </param>
        /// <returns>Returns a message. Check `Message.Error` to see whether the video has been successfully shared.</returns>
        public static Task ShareVideo(string videoPath, string videoThumbPath)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            var options = new ShareMediaOptions();
            options.SetShareMediaType(ShareMediaType.Video);
            options.SetVideoPath(videoPath);
            options.SetVideoThumbPath(videoThumbPath);
            return new Task(CLIB.ppf_Presence_ShareMedia((IntPtr) options));
        }

        /// When the user clicks on the invitation message, the system will launch your app and
        /// the callback will be triggered. Read the fields of \ref Pico.Platform.Models.PresenceJoinIntent
        /// to figure out where the user wants to go. If the user is unable to go there,
        /// show the user the info about why they cannot go there. 
        public static void SetJoinIntentReceivedNotificationCallback(Message<PresenceJoinIntent>.Handler callback)
        {
            Looper.RegisterNotifyHandler(
                MessageType.Notification_Presence_JoinIntentReceived,
                callback
            );
        }
    }

    public class ShareMediaOptions
    {
        public ShareMediaOptions()
        {
            Handle = CLIB.ppf_ShareMediaOptions_Create();
        }


        public void SetShareMediaType(ShareMediaType value)
        {
            CLIB.ppf_ShareMediaOptions_SetShareMediaType(Handle, value);
        }


        public void SetVideoPath(string value)
        {
            CLIB.ppf_ShareMediaOptions_SetVideoPath(Handle, value);
        }


        public void SetVideoThumbPath(string value)
        {
            CLIB.ppf_ShareMediaOptions_SetVideoThumbPath(Handle, value);
        }


        public void AddImagePath(string ele)
        {
            CLIB.ppf_ShareMediaOptions_AddImagePath(Handle, ele);
        }

        public void ClearImagePaths()
        {
            CLIB.ppf_ShareMediaOptions_ClearImagePaths(Handle);
        }


        public void SetShareAppType(ShareAppType value)
        {
            CLIB.ppf_ShareMediaOptions_SetShareAppType(Handle, value);
        }

        /// For passing to native C
        public static explicit operator IntPtr(ShareMediaOptions options)
        {
            return options != null ? options.Handle : IntPtr.Zero;
        }

        ~ShareMediaOptions()
        {
            CLIB.ppf_ShareMediaOptions_Destroy(Handle);
        }

        IntPtr Handle;
    }

    public class PresenceOptions
    {
        public PresenceOptions()
        {
            Handle = CLIB.ppf_PresenceOptions_Create();
        }

        /// <summary>
        /// Sets a destination for the current logged-in user.
        /// </summary>
        /// <param name="value">The API name of the destination.</param>
        public void SetDestinationApiName(string value)
        {
            CLIB.ppf_PresenceOptions_SetDestinationApiName(Handle, value);
        }

        /// <summary>
        /// Sets whether the current logged-in user is joinable.
        /// </summary>
        /// <param name="value">
        /// * `true`: joinable
        /// * `false`: not joinable
        /// </param>
        public void SetIsJoinable(bool value)
        {
            CLIB.ppf_PresenceOptions_SetIsJoinable(Handle, value);
        }

        /// <summary>
        /// Sets a lobby session ID for the current logged-in user.
        /// </summary>
        /// <param name="value">The lobby session ID.</param>
        public void SetLobbySessionId(string value)
        {
            CLIB.ppf_PresenceOptions_SetLobbySessionId(Handle, value);
        }

        /// <summary>
        /// Sets a match session ID for the current logged-in user.
        /// </summary>
        /// <param name="value">The match session ID.</param>
        public void SetMatchSessionId(string value)
        {
            CLIB.ppf_PresenceOptions_SetMatchSessionId(Handle, value);
        }

        /// <summary>
        /// Sets extra presence data for the current logged-in user.
        /// </summary>
        /// <param name="value">Extra presence data defined by the developer.</param>
        public void SetExtra(string value)
        {
            CLIB.ppf_PresenceOptions_SetExtra(Handle, value);
        }


        /// For passing to native C
        public static explicit operator IntPtr(PresenceOptions options)
        {
            return options != null ? options.Handle : IntPtr.Zero;
        }

        ~PresenceOptions()
        {
            CLIB.ppf_PresenceOptions_Destroy(Handle);
        }

        IntPtr Handle;
    }

    public class InviteOptions
    {
        public InviteOptions()
        {
            Handle = CLIB.ppf_InviteOptions_Create();
        }


        public void AddSuggestedUser(string ele)
        {
            CLIB.ppf_InviteOptions_AddSuggestedUser(Handle, ele);
        }

        public void ClearSuggestedUsers()
        {
            CLIB.ppf_InviteOptions_ClearSuggestedUsers(Handle);
        }


        /// For passing to native C
        public static explicit operator IntPtr(InviteOptions options)
        {
            return options != null ? options.Handle : IntPtr.Zero;
        }

        ~InviteOptions()
        {
            CLIB.ppf_InviteOptions_Destroy(Handle);
        }

        IntPtr Handle;
    }
}