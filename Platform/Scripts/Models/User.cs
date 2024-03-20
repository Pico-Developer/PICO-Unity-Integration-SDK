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

namespace Pico.Platform.Models
{
    /// <summary>
    /// The User info structure.
    /// Basic fields, such as `DisplayName` and `ImageUrl`, are always valid. 
    /// Some fields, such as presence-related fields, are valid only when you call presence-related APIs.
    /// See also: \ref UserService.GetLoggedInUser 
    /// </summary>
    public class User
    {
        /// User's display name.
        public readonly string DisplayName;

        ///The URL of user's profile photo. The image size is 300x300.
        public readonly string ImageUrl;

        /// The URL of the user's small profile photo. The image size is 128x128. 
        public readonly string SmallImageUrl;

        /// User's openID. The same user has different openIDs in different apps.
        public readonly string ID;

        /// User's presence status which indicates whether the user is online.
        public readonly UserPresenceStatus PresenceStatus;

        /// User's gender.
        public readonly Gender Gender;

        /// User's presence information.
        public readonly string Presence;

        /// The deeplink message. 
        public readonly string PresenceDeeplinkMessage;

        /// The destination's API name. 
        public readonly string PresenceDestinationApiName;

        /// The lobby session ID which identifies a group or team. 
        public readonly string PresenceLobbySessionId;

        /// The match session ID which identifies a competition. 
        public readonly string PresenceMatchSessionId;

        /// User's extra presence information. 
        public readonly string PresenceExtra;

        /// Whether the user can be joined by others.
        public readonly bool PresenceIsJoinable;

        /// The user's invite token. 
        public readonly string InviteToken;

        /// The user's registration country/region. Returns a country/region code. 
        public readonly string StoreRegion;

        public User(IntPtr obj)
        {
            DisplayName = CLIB.ppf_User_GetDisplayName(obj);
            ImageUrl = CLIB.ppf_User_GetImageUrl(obj);
            ID = CLIB.ppf_User_GetID(obj);
            InviteToken = CLIB.ppf_User_GetInviteToken(obj);
            PresenceStatus = CLIB.ppf_User_GetPresenceStatus(obj);
            Gender = CLIB.ppf_User_GetGender(obj);
            Presence = CLIB.ppf_User_GetPresence(obj);
            PresenceDeeplinkMessage = CLIB.ppf_User_GetPresenceDeeplinkMessage(obj);
            PresenceDestinationApiName = CLIB.ppf_User_GetPresenceDestinationApiName(obj);
            PresenceLobbySessionId = CLIB.ppf_User_GetPresenceLobbySessionId(obj);
            PresenceMatchSessionId = CLIB.ppf_User_GetPresenceMatchSessionId(obj);
            PresenceExtra = CLIB.ppf_User_GetPresenceExtra(obj);
            PresenceIsJoinable = CLIB.ppf_User_GetPresenceIsJoinable(obj);
            SmallImageUrl = CLIB.ppf_User_GetSmallImageUrl(obj);
            InviteToken = CLIB.ppf_User_GetInviteToken(obj);
            StoreRegion = CLIB.ppf_User_GetStoreRegion(obj);
        }
    }

    /// <summary>
    /// Each element is \ref User.
    /// </summary>
    public class UserList : MessageArray<User>
    {
        public UserList(IntPtr a)
        {
            var count = (int) CLIB.ppf_UserArray_GetSize(a);
            this.Capacity = count;
            for (int i = 0; i < count; i++)
            {
                this.Add(new User(CLIB.ppf_UserArray_GetElement(a, (UIntPtr) i)));
            }

            NextPageParam = CLIB.ppf_UserArray_GetNextPageParam(a);
        }
    }

    /// <summary>
    /// The user's organization ID.
    /// </summary>
    public class OrgScopedID
    {
        /// <summary>
        /// The organization ID.
        /// </summary>
        public readonly string ID;

        public OrgScopedID(IntPtr o)
        {
            ID = CLIB.ppf_OrgScopedID_GetID(o);
        }
    }

    /// <summary>
    /// Indicates whether the friend request is canceled or successfully sent.
    /// </summary>
    public class LaunchFriendResult
    {
        /// Whether the request is canceled by the user.
        public readonly bool DidCancel;

        /// Whether the request is successfully sent. 
        public readonly bool DidSendRequest;

        public LaunchFriendResult(IntPtr obj)
        {
            DidCancel = CLIB.ppf_LaunchFriendRequestFlowResult_GetDidCancel(obj);
            DidSendRequest = CLIB.ppf_LaunchFriendRequestFlowResult_GetDidSendRequest(obj);
        }
    }


    /// <summary>
    /// The info returned after calling \ref UserService.GetFriendsAndRooms.
    /// </summary>
    public class UserRoom
    {
        public readonly User User;
        public readonly Room Room;

        public UserRoom(IntPtr o)
        {
            User = new User(CLIB.ppf_UserAndRoom_GetUser(o));
            var ptr = CLIB.ppf_UserAndRoom_GetRoom(o);
            if (ptr != IntPtr.Zero)
            {
                Room = new Room(ptr);
            }
        }
    }

    /// <summary>
    /// Each element is \ref UserRoom.
    /// </summary>
    public class UserRoomList : MessageArray<UserRoom>
    {
        public UserRoomList(IntPtr a)
        {
            var count = (int) CLIB.ppf_UserAndRoomArray_GetSize(a);
            this.Capacity = count;
            for (int i = 0; i < count; i++)
            {
                this.Add(new UserRoom(CLIB.ppf_UserAndRoomArray_GetElement(a, (UIntPtr) i)));
            }

            NextPageParam = CLIB.ppf_UserAndRoomArray_GetNextPageParam(a);
        }
    }


    /// <summary>
    /// User permissions list.
    /// </summary>
    public static class Permissions
    {
        /// <summary>
        /// The permission to get the user's registration information, including the user's nickname, gender, profile photo, and more.
        /// </summary>
        public const string UserInfo = "user_info";
        /// <summary>
        /// The permission to get users' friend relations.
        /// </summary>
        public const string FriendRelation = "friend_relation";
        /// <summary>
        /// The permission to get the user's information, including the user's gender, birthday, stature, weight, and more, on the PICO Fitness app.
        /// </summary>
        public const string SportsUserInfo = "sports_userinfo";
        /// <summary>
        /// The permission to get users' exercise data from the PICO Fitness app.
        /// </summary>
        public const string SportsSummaryData = "sports_summarydata";
        /// <summary>
        /// The permission to capture or record the screen, which is required when using the highlight service.
        /// </summary>
        public const string RecordHighlight = "record_highlight";
    }


    /// <summary>
    /// The result returned after calling \ref UserService.RequestUserPermissions or \ref UserService.GetAuthorizedPermissions.
    /// </summary>
    public class PermissionResult
    {
        /// The authorized permissions.
        public readonly string[] AuthorizedPermissions;

        /// The access token. It has a value only after you call \ref UserService.RequestUserPermissions.
        public readonly string AccessToken;

        /// The current user's ID.
        public readonly string UserID;

        public PermissionResult(IntPtr o)
        {
            {
                int sz = (int) CLIB.ppf_PermissionResult_GetAuthorizedPermissionsSize(o);
                AuthorizedPermissions = new string[sz];
                for (int i = 0; i < sz; i++)
                {
                    AuthorizedPermissions[i] = CLIB.ppf_PermissionResult_GetAuthorizedPermissions(o, (UIntPtr) i);
                }
            }

            AccessToken = CLIB.ppf_PermissionResult_GetAccessToken(o);
            UserID = CLIB.ppf_PermissionResult_GetUserID(o);
        }
    }


    /// <summary>
    /// The result returned after calling \ref UserService.GetUserRelations.
    ///
    /// This class derives from Dictionary. The key is userId and value is
    /// \ref UserRelationType.
    /// </summary>
    public class UserRelationResult : Dictionary<string, UserRelationType>
    {
        public UserRelationResult(IntPtr o)
        {
            {
                int sz = (int) CLIB.ppf_UserRelationResult_GetRelationsSize(o);
                for (int i = 0; i < sz; i++)
                {
                    string userId = CLIB.ppf_UserRelationResult_GetRelationsKey(o, i);
                    UserRelationType relation = CLIB.ppf_UserRelationResult_GetRelationsValue(o, i);
                    Add(userId, relation);
                }
            }
        }
    }


    /// <summary>
    /// The result returned after calling \ref UserService.EntitlementCheck
    /// </summary>
    public class EntitlementCheckResult
    {
        /// Whether the user is entitled to use the current app.
        public readonly bool HasEntitlement;

        /// The status code for entitlement check. 
        public readonly int StatusCode;

        /// The status message for entitlement check. You can show this message to user if the user does not pass the entitlement check.
        public readonly string StatusMessage;

        public EntitlementCheckResult(IntPtr o)
        {
            HasEntitlement = CLIB.ppf_EntitlementCheckResult_GetHasEntitlement(o);
            StatusCode = CLIB.ppf_EntitlementCheckResult_GetStatusCode(o);
            StatusMessage = CLIB.ppf_EntitlementCheckResult_GetStatusMessage(o);
        }
    }
}