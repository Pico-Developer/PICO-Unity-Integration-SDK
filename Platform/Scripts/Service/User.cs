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
     * Account & Friends service allows developers to access the info of a specified account, get the friends list of the currently logged-in users, send friend requests, and more.
     */
    public static class UserService
    {
        /// <summary>
        /// Returns an access token for this user. Starting from SDK V2.2.0,
        /// the system caches the access token upon the first request, allowing subsequent requests to use the cached token information stored locally.
        /// @note  User's permission is required if the user uses this app for the first time.
        /// </summary>
        /// <returns>The access token for the current user.</returns>
        public static Task<string> GetAccessToken()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<string>(CLIB.ppf_User_GetAccessToken());
        }

        /// <summary>
        /// Gets the organization ID for a specified user.
        /// Different users have different organization IDs.
        /// A user has only one fixed unique organization ID for all the apps created by the same organization.
        ///
        /// </summary>
        /// <param name="userID">The ID of the user. It's the user's openID in the current app.</param>
        /// <returns>The `OrgScopedID` structure that contains the user's organization ID.</returns>
        public static Task<OrgScopedID> GetOrgScopedID(string userID)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<OrgScopedID>(CLIB.ppf_User_GetOrgScopedID(userID));
        }

        /// <summary>
        /// Gets the information about the current logged-in user.
        /// </summary>
        /// <returns>The User structure that contains the details about the user.</returns>
        public static Task<User> GetLoggedInUser()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<User>(CLIB.ppf_User_GetLoggedInUser());
        }

        /// <summary>
        /// Gets the information about a specified user.
        /// @note  The same user has different user IDs for different apps.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The User structure that contains the details about the specified user.</returns>
        public static Task<User> Get(string userId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<User>(CLIB.ppf_User_Get(userId));
        }

        /// <summary>
        /// Gets the friend list of the current user.
        /// @note  Friends who don't use this app won't appear in this list.
        /// </summary>
        /// <returns>The friend list of the current user.</returns>
        public static Task<UserList> GetFriends()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<UserList>(CLIB.ppf_User_GetLoggedInUserFriends());
        }

        /// <summary>
        /// Gets the relationship between the current user and other users.
        /// </summary>
        /// <param name="userIds">The list of user IDs.
        /// The request queries the current user's relationship with specified users.
        /// A single request can pass no more than 20 user IDs.</param>
        /// <returns>`UserRelationResult` which is a dictionary of user relationships.</returns>
        public static Task<UserRelationResult> GetUserRelations(string[] userIds)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<UserRelationResult>(CLIB.ppf_User_GetRelations(userIds));
        }

        /// <summary>
        /// Launches the flow to apply for friendship with someone.
        /// </summary>
        /// <param name="userId">The ID of the user that the friend request is sent to.</param>
        /// <returns>`LaunchFriendRequest` that indicates whether the request is sent successfully.</returns>
        public static Task<LaunchFriendResult> LaunchFriendRequestFlow(string userId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<LaunchFriendResult>(CLIB.ppf_User_LaunchFriendRequestFlow(userId));
        }

        /// <summary>
        /// Gets the friends of the logged-in user and the rooms the friends might be in.
        /// </summary>
        /// <returns>`UserRoomList` that contains the friend and room data. If a friend is not in any room, the `room` field will be null.</returns>
        public static Task<UserRoomList> GetFriendsAndRooms()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<UserRoomList>(CLIB.ppf_User_GetLoggedInUserFriendsAndRooms());
        }

        /// <summary>
        /// Gets the next page of user and room list.
        /// </summary>
        /// <param name="list">The user and room list from the current page.</param>
        /// <returns>The user and room list from the next page.</returns>
        public static Task<UserRoomList> GetNextUserAndRoomListPage(UserRoomList list)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            if (!list.HasNextPage)
            {
                Debug.LogWarning("GetNextUserAndRoomListPage: List has no next page");
                return null;
            }

            if (String.IsNullOrEmpty(list.NextPageParam))
            {
                Debug.LogWarning("GetNextUserAndRoomListPage: list.NextPageParam is empty");
                return null;
            }

            return new Task<UserRoomList>(CLIB.ppf_User_GetNextUserAndRoomArrayPage(list.NextPageParam));
        }

        /// <summary>
        /// Gets the next page of user list.
        /// </summary>
        /// <param name="list">The user list from the current page.</param>
        /// <returns>The user list from the next page.</returns>
        public static Task<UserList> GetNextUserListPage(UserList list)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            if (!list.HasNextPage)
            {
                Debug.LogWarning("GetNextUserListPage: List has no next page");
                return null;
            }

            if (String.IsNullOrEmpty(list.NextPageParam))
            {
                Debug.LogWarning("GetNextUserListPage: list.NextPageParam is empty");
                return null;
            }

            return new Task<UserList>(CLIB.ppf_User_GetNextUserArrayPage(list.NextPageParam));
        }

        /// <summary>
        /// Gets authorized permissions.
        /// </summary>
        /// <returns>
        /// A struct containing the access token and permission list. The `UserID` field is empty so do NOT use it.
        /// </returns>
        public static Task<PermissionResult> GetAuthorizedPermissions()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<PermissionResult>(CLIB.ppf_User_GetAuthorizedPermissions());
        }

        /// <summary>
        /// Requests user permissions. The user will received a pop-up notification window.
        /// </summary>
        /// <param name="permissionList">The list of permissions to request. You can use constants in \ref Pico.Platform.Models.Permissions.
        /// </param>
        /// <returns>A struct containing the access token and permission list.</returns>
        public static Task<PermissionResult> RequestUserPermissions(params string[] permissionList)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<PermissionResult>(CLIB.ppf_User_RequestUserPermissions(permissionList));
        }

        /// <summary>
        /// Checks whether the current user is entitled to use the current app.
        ///
        /// If the user is not entitled, the system will close the app and show a dialog box to remind the user to buy the app from the PICO Store.
        /// For customizations, you can set param `killApp` to `false` and then customize the dialog.
        /// </summary>
        /// <param name="killApp">Determines whether the system closes the app if the user fails to pass the entitlement check.
        /// The default value is `true`.
        /// </param>
        /// <returns>The entitlement check result.</returns>
        public static Task<EntitlementCheckResult> EntitlementCheck(bool killApp = true)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<EntitlementCheckResult>(CLIB.ppf_User_EntitlementCheck(killApp));
        }

        /// <summary>
        /// Gets the ID token for the current user. The ID token is used for OIDC login.
        /// You can use \ref ApplicationService.GetSystemInfo to determine the region of user's device and then choose the proper OIDC identity provider.
        /// Read the following articles for more information:
        /// * [Unity Service signin with openID connect](https://docs.unity.com/authentication/en/manual/platform-signin-openid-connect)
        /// * [PICO SDK guide](http://developer-global.pico-interactive.com/document/unity/accounts-and-friends/)
        /// @note  User's permission is required if the user uses this app for the first time. Call \ref UserService.RequestUserPermissions to request desired permissions in a batch.
        /// </summary>
        /// <returns>The ID token for the current user.</returns>
        public static Task<string> GetIdToken()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<string>(CLIB.ppf_User_GetIdToken());
        }
    }
}