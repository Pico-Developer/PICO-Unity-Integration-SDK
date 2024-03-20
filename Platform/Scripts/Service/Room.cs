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
    public static class RoomService
    {
        /// <summary>Gets the room options for create a private room. You can use it when you use \ref RoomService.CreateAndJoinPrivate2.</summary>
        /// <param name="dataStore">The key/value pairs to add.</param>
        /// <returns>The room options for create a private room.</returns>
        public static RoomOptions GetCreatePrivateRoomOptions(Dictionary<string, string> dataStore)
        {
            RoomOptions options = new RoomOptions();
            foreach (var data in dataStore)
            {
                options.SetDataStore(data.Key, data.Value);
            }

            return options;
        }

        /// <summary>Gets the room options for joining or creating a named room. You can use it when you use \ref RoomService.JoinOrCreateNamedRoom.</summary>
        /// <param name="dataStore">The key/value pairs to add.</param>
        /// <param name="name">The name of the named room.</param>
        /// <param name="password">The password of the named room.</param>
        /// <returns>The room options for joining or creating a named room.</returns>
        public static RoomOptions GetJoinOrCreateNamedRoomOptions(Dictionary<string, string> dataStore, string name, string password)
        {
            RoomOptions options = new RoomOptions();
            foreach (var data in dataStore)
            {
                options.SetDataStore(data.Key, data.Value);
            }

            options.SetRoomName(name);
            options.SetPassword(password);

            return options;
        }

        /// <summary>Gets the list of named rooms created for the app.</summary>
        /// <param name="pageIndex">Defines which page of entries to return. The index for the first page is `0`.</param>
        /// <param name="pageSize">The number of entries returned on each page. Value range: [5,20].</param>
        /// <returns>The request ID of this async function.
        ///
        /// A message of type `MessageType.Room_GetNamedRooms` will be generated in response.
        /// First call `Message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `RoomList`.
        /// Extract the payload from the message handle with `Message.Data`.
        /// </returns>
        public static Task<RoomList> GetNamedRooms(int pageIndex, int pageSize)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<RoomList>(CLIB.ppf_Room_GetNamedRooms(pageIndex, pageSize));
        }

        /// <summary>Join or create a named room.</summary>
        /// <param name="joinPolicy">The join policy of the room. Currently only support 'RoomJoinPolicy Everyone'.</param>
        /// <param name="createIfNotExist">Determines whether to create a new room if the named room does not exist:
        /// * `true`: create
        /// * `false`: do not create
        /// </param>
        /// <param name="maxUsers">The maximum number of users allowed in the room, including the creator.</param>
        /// <param name="options">Additional room configuration for this request.</param>
        /// <returns>The request ID of this async function.
        ///
        /// A message of type `MessageType.Room_JoinNamed` will be generated in response.
        /// First call `Message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`.
        /// Extract the payload from the message handle with `Message.Data`.
        /// </returns>
        public static Task<Room> JoinOrCreateNamedRoom(RoomJoinPolicy joinPolicy, bool createIfNotExist, uint maxUsers, RoomOptions options)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_JoinNamed(joinPolicy, createIfNotExist, maxUsers, options.GetHandle()));
        }

        /// <summary>Launches the invitation flow to let the current user invite friends to a specified room.
        /// This launches the system default invite UI where all of the user's friends are displayed.
        /// This is intended to be a shortcut for developers not wanting to build their own invite-friends UI.
        /// </summary>
        /// <param name="roomID">The ID of the room.</param>
        /// <returns>The request ID of this async function.
        /// A message of type `MessageType.Room_LaunchInvitableUserFlow` will be generated in response.
        /// Call `message.IsError()` to check if any error has occurred.
        /// </returns>
        public static Task LaunchInvitableUserFlow(UInt64 roomID)
        {
            if (!CoreService.IsInitialized())
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task(CLIB.ppf_Room_LaunchInvitableUserFlow(roomID));
        }

        /// <summary>Updates the data store of the current room (the caller should be the room owner).
        /// @note Room data stores only allow string values. The maximum key length is 32 bytes and the maximum value length is 64 bytes.
        /// If you provide illegal values, this method will return an error.</summary>
        /// <param name="roomId">The ID of the room that you currently own (call `Room.OwnerOptional` to check).</param>
        /// <param name="data">The key/value pairs to add or update. Null value will clear a given key.</param>
        /// <returns>The request ID of this async function.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |3006004|change datastore failed: need room owner|
        ///
        /// A message of type `MessageType.Room_UpdateDataStore` will be generated in response.
        /// First call `Message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`.
        /// Extract the payload from the message handle with `Message.Data`.
        /// </returns>
        public static Task<Room> UpdateDataStore(UInt64 roomId, Dictionary<string, string> data)
        {
            KVPairArray kvarray = new KVPairArray((uint) data.Count);
            uint n = 0;
            foreach (var d in data)
            {
                var item = kvarray.GetElement(n);
                item.SetKey(d.Key);
                item.SetStringValue(d.Value);
                n++;
            }

            return new Task<Room>(CLIB.ppf_Room_UpdateDataStore(roomId, kvarray.GetHandle(), kvarray.Size));
        }

        /// <summary>Creates a new private room and joins it.
        /// @note This type of room can be obtained by querying the room where
        /// a friend is, so it is suitable for playing with friends.</summary>
        ///
        /// <param name="policy">Specifies who can join the room:
        /// * `0`: nobody
        /// * `1`: everybody
        /// * `2`: friends of members
        /// * `3`: friends of the room owner
        /// * `4`: invited users
        /// * `5`: unknown
        /// </param>
        /// <param name="maxUsers">The maximum number of members allowed in the room, including the room creator.</param>
        /// <param name="roomOptions">Room configuration for this request.</param>
        /// <returns>Request information of type Task, including the request id, and its response message will contain data of type `Room`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |3006101|room create: unknown error|
        /// |3006114|setting of 'room max user' is too large|
        ///
        /// A message of type `MessageType.Room_CreateAndJoinPrivate2` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<Room> CreateAndJoinPrivate2(RoomJoinPolicy policy, uint maxUsers, RoomOptions roomOptions)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_CreateAndJoinPrivate2(policy, maxUsers, roomOptions.GetHandle()));
        }

        /// <summary>Gets the information about a specified room.</summary>
        /// <param name="roomId">The ID of the room to get information for.</param>
        /// <returns>Request information of type `Task`, including the request id, and its response message will contain data of type `Room`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |3006103|invalid room|
        /// |3006301|server error: unknown|
        ///
        /// A message of type `MessageType.Room_Get` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<Room> Get(UInt64 roomId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_Get(roomId));
        }

        /// <summary>Gets the data of the room you are currently in.</summary>
        /// <returns>Request information of type `Task`, including the request id, and its response message will contain data of type `Room`.
        /// |Error Code| Error Message |
        /// |---|---|
        /// |3006104|not in room|
        ///
        /// A message of type `MessageType.Room_GetCurrent` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload with of type `Room`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<Room> GetCurrent()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_GetCurrent());
        }

        /// <summary>Gets the current room of the specified user.
        /// @note The user's privacy settings may not allow you to access their room.
        /// </summary>
        ///
        /// <param name="userId">The ID of the user.</param>
        /// <returns>Request information of type `Task`, including the request id, and its response message will contain data of type `Room`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |3006104|not in room|
        /// |3006009|tgt player is not in game now|
        /// |3006301|server error: unknown|
        ///
        /// A message of type `MessageType.Room_GetCurrentForUser` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<Room> GetCurrentForUser(string userId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_GetCurrentForUser(userId));
        }

        /// <summary>Gets a list of members the user can invite to the room.
        /// These members are drawn from the user's friends list and recently
        /// encountered list, and filtered based on relevance and interests.
        /// @note: Only applicable to private rooms and named rooms.
        /// </summary>
        ///
        /// <param name="roomOptions">Additional configuration for this request.
        /// If you pass `null`, the response will return code `0`.</param>
        /// <returns>Request information of type `Task`, including the request id, and its response message will contain data of type `UserList`.
        ///
        /// A message of type `MessageType.Room_GetInvitableUsers2` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `UserList`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<UserList> GetInvitableUsers2(RoomOptions roomOptions = null)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            if (roomOptions == null)
            {
                return new Task<UserList>(CLIB.ppf_Room_GetInvitableUsers2(IntPtr.Zero));
            }
            else
            {
                return new Task<UserList>(CLIB.ppf_Room_GetInvitableUsers2(roomOptions.GetHandle()));
            }
        }

        /// <summary>Gets the list of moderated rooms created for the application.</summary>
        ///
        /// <param name="index">Start page index.</param>
        /// <param name="size">Page entry number in response (should range from `5` to `20`).</param>
        /// <returns>Request information of type `Task`, including the request id, and its response message will contain data of type `RoomList`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |3006301|server error: unknown|
        ///
        /// A message of type `MessageType.Room_GetModeratedRooms` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `RoomList`, the room info does not contain the `UserList` info.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<RoomList> GetModeratedRooms(int index, int size)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<RoomList>(CLIB.ppf_Room_GetModeratedRooms(index, size));
        }

        /// <summary>Invites a user to the current room.
        /// @note  The user invited will receive a notification of type `MessageType.Notification_Room_InviteReceived`.
        /// </summary>
        ///
        /// <param name="roomId">The ID of the room.</param>
        /// <param name="token">The user's invitation token, which is returned by `RoomService.GetInvitableUsers2()`.</param>
        /// <returns>Request information of type `Task`, including the request id, and its response message will contain data of type `Room`.
        ///
        /// A message of type `MessageType.Room_InviteUser` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<Room> InviteUser(UInt64 roomId, string token)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_InviteUser(roomId, token));
        }

        /// <summary>Joins the target room and meanwhile leaves the current room.</summary>
        ///
        /// <param name="roomId">The ID of the room to join.</param>
        /// <param name="options">(Optional) Additional room configuration for this request.</param>
        /// <returns>Request information of type `Task`, including the request id, and its response message will contain data of type `Room`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |3006401|logic state checking failed|
        /// |3006103|invalid room|
        /// |3006102|duplicate join room(regarded as normal entry)|
        /// |3006106|exceed max room player number|
        /// |3006105|illegal enter request(Players outside the legal list enter)| 
        /// |3006108|room is locked|
        ///
        /// A message of type `MessageType.Room_Join2` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<Room> Join2(UInt64 roomId, RoomOptions options)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_Join2(roomId, options.GetHandle()));
        }

        /// <summary>Kicks a user out of a room. For use by homeowners only.</summary>
        ///
        /// <param name="roomId">The ID of the room.</param>
        /// <param name="userId">The ID of the user to be kicked (cannot be yourself).</param>
        /// <param name="kickDuration">The Length of the ban (in seconds).</param>
        /// <returns>Request information of type `Task`, including the request id, and its response message will contain data of type `Room`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |3006006|kick user failed: need room owner|
        /// |3006007|kick user failed: tgt user is not in the room|
        /// |3006008|kick user failed: can not kick self|
        ///
        /// A message of type `MessageType.Room_KickUser` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<Room> KickUser(UInt64 roomId, string userId, int kickDuration)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_KickUser(roomId, userId, kickDuration));
        }

        /// <summary>Leaves the current room.
        /// @note  The room you are now in will be returned if the request succeeds.</summary>
        ///
        /// <param name="roomId">The ID of the room.</param>
        /// <returns>Request information of type `Task`, including the request id, and its response message will contain data of type `Room`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |3006401|logic state checking failed(e.g. not in the room)| 
        /// |3006301|server error: unknown|
        ///
        /// A message of type `MessageType.Room_Leave` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<Room> Leave(UInt64 roomId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_Leave(roomId));
        }

        /// <summary>Sets the description of a room.  For use by homeowners only.</summary>
        ///
        /// <param name="roomId">The ID of the room to set description for.</param>
        /// <param name="description">The new description of the room.</param>
        /// <returns>Request information of type `Task`, including the request id, and its response message will contain data of type `Room`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |3006005|set description failed: need room owner|
        ///
        /// A message of type `MessageType.Room_SetDescription` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<Room> SetDescription(UInt64 roomId, string description)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_SetDescription(roomId, description));
        }

        /// <summary>Locks/unlocks the membership of a room (the caller should be the room owner) to allow/disallow new members from being able to join the room.
        /// @note  Locking membership will prevent other users from joining the room through `Join2()`, invitations, etc. Users that are in the room at the time of lock will be able to rejoin.</summary>
        ///
        /// <param name="roomId">The ID of the room to lock/unlock membership for.</param>
        /// <param name="membershipLockStatus">The new membership status to set for the room:
        /// * `0`: Unknown
        /// * `1`: lock
        /// * `2`: unlock
        /// </param>
        /// <returns>Request information of type `Task`, including the request id, and its response message will contain data of type `Room`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |3006104|not in room |
        /// |3006109|update membership lock: need room owner|
        ///
        /// A message of type `MessageType.Room_UpdateMembershipLockStatus` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<Room> UpdateMembershipLockStatus(UInt64 roomId, RoomMembershipLockStatus membershipLockStatus)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_UpdateMembershipLockStatus(roomId, membershipLockStatus));
        }

        /// <summary>Modifies the owner of the room, this person needs to be the person in this room.</summary>
        ///
        /// <param name="roomId">The ID of the room to change ownership for.</param>
        /// <param name="userId">The ID of the new user to own the room. The new user must be in the same room.</param>
        /// <returns>Request information of type `Task`, including the request id, and its response message does not contain data.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |3006001|change owner failed: need room owner| 
        /// |3006003|change owner failed: duplicate setting|
        /// |3006002|change owner failed: new owner not in this room|
        ///
        /// A message of type `MessageType.Room_UpdateOwner` will be generated in response.
        /// Call `message.IsError()` to check if any error has occurred.
        /// </returns>
        public static Task UpdateOwner(UInt64 roomId, string userId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task(CLIB.ppf_Room_UpdateOwner(roomId, userId));
        }

        /// <summary>Sets the join policy for a specified private room.</summary>
        ///
        /// <param name="roomId">The ID of the room you want to set join policy for.</param>
        /// <param name="policy">Specifies who can join the room:
        /// * `0`: nobody
        /// * `1`: everybody
        /// * `2`: friends of members
        /// * `3`: friends of the room owner
        /// * `4`: invited users
        /// * `5`: unknown
        /// </param>
        /// <returns>Request information of type `Task`, including the request id, and its response message will contain data of type `Room`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |3006104|not in room |
        /// |3006112|update room join policy: need room owner|
        ///
        /// A message of type `MessageType.Room_UpdatePrivateRoomJoinPolicy` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<Room> UpdatePrivateRoomJoinPolicy(UInt64 roomId, RoomJoinPolicy policy)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_UpdatePrivateRoomJoinPolicy(roomId, policy));
        }

        /// <summary>Sets the callback to get notified when the user has accepted an invitation.
        /// @note You can get the RoomID by 'Message.Data'. Then you can call 'RoomService.Join2' to join it.
        /// </summary>
        /// <param name="handler">The callback function will be called when receiving the `Notification_Room_InviteAccepted` message.</param>
        public static void SetRoomInviteAcceptedNotificationCallback(Message<string>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Room_InviteAccepted, handler);
        }

        /// <summary>Sets the callback to get notified when the current room has been updated. Use `Message.Data` to extract the room.</summary>
        ///
        /// <param name="handler">The callback function will be called when receiving the `Notification_Room_RoomUpdate` message.</param>
        public static void SetUpdateNotificationCallback(Message<Room>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Room_RoomUpdate, handler);
        }

        /// <summary>Sets the callback to get notified when the user has been kicked out of a room.
        /// Listen to this event to receive a relevant message. Use `Message.Data` to extract the room.</summary>
        ///
        /// <param name="handler">The callback function will be called when receiving the `Room_KickUser` message and the value of `requestID` is `0`.</param>
        public static void SetKickUserNotificationCallback(Message<Room>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Room_KickUser, handler);
        }

        /// <summary>Sets the callback to get notified when the room description has been updated.
        /// Listen to this event to receive a relevant message. Use `Message.Data` to extract the room.</summary>
        ///
        /// <param name="handler">The callback function will be called when receiving the `Room_SetDescription` message and the value of `requestID` is `0`.</param>
        public static void SetSetDescriptionNotificationCallback(Message<Room>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Room_SetDescription, handler);
        }

        /// <summary>Sets the callback to get notified when the room data has been modified.
        /// Listen to this event to receive a relevant message. Use `Message.Data` to extract the room.</summary>
        ///
        /// <param name="handler">The callback function will be called when receiving the `Room_UpdateDataStore` message and the value of `requestID` is `0`.</param>
        public static void SetUpdateDataStoreNotificationCallback(Message<Room>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Room_UpdateDataStore, handler);
        }

        /// <summary>If a player is passively removed from a room (for example, if they initiate another match within the room and are subsequently removed by the system or if they are kicked out of the room), they will receive a notification.
        /// Listen to this event to receive a relevant message.
        /// Use `Message.Data` to extract the room.</summary>
        ///
        /// <param name="handler">The callback function will be called when receiving the `Room_Leave` message and the value of `requestID` is `0`.</param>
        public static void SetLeaveNotificationCallback(Message<Room>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Room_Leave, handler);
        }

        /// <summary>If a player comes across network or disaster recovery problems after joining a room, they may not receive a notification confirming that they've successfully entered the room.
        /// In such cases, the server will resend the notification to ensure that the user receives it.
        /// Use `Message.Data` to extract the room.</summary>
        ///
        /// <param name="handler">The callback function will be called when receiving the `Room_Join2` message and the value of `requestID` is `0`.</param>
        public static void SetJoin2NotificationCallback(Message<Room>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Room_Join2, handler);
        }

        /// <summary>When there is a change in the room owner, the new owner will receive a notification.
        /// Listen to this event to receive a relevant message.
        /// Use `Message.Data` to extract the room.</summary>
        ///
        /// <param name="handler">The callback function will be called when receiving the `Room_UpdateOwner` message and the value of `requestID` is `0`.</param>
        public static void SetUpdateOwnerNotificationCallback(Message.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Room_UpdateOwner, handler);
        }

        /// <summary>Sets the callback to get notified when the membership status of a room has been changed.
        /// Listen to this event to receive a relevant message. Use `Message.Data` to extract the room.</summary>
        ///
        /// <param name="handler">The callback function will be called when receiving the "Room_UpdateMembershipLockStatus" message and the value of `requestID` is `0`.</param>
        public static void SetUpdateMembershipLockStatusNotificationCallback(Message<Room>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Room_UpdateMembershipLockStatus, handler);
        }
    }

    public class RoomOptions
    {
        public RoomOptions()
        {
            Handle = CLIB.ppf_RoomOptions_Create();
        }

        /// <summary>
        /// Sets the data store for a room.
        /// </summary>
        /// <param name="key">A unique identifier that maps to a value.</param>
        /// <param name="value">The data.</param>
        public void SetDataStore(string key, string value)
        {
            CLIB.ppf_RoomOptions_SetDataStoreString(Handle, key, value);
        }

        /// <summary>
        /// Clears the data store for a room.
        /// </summary>
        public void ClearDataStore()
        {
            CLIB.ppf_RoomOptions_ClearDataStore(Handle);
        }

        /// <summary>
        /// Sets whether to exclude recently-met users.
        /// </summary>
        /// <param name="value">
        /// * `true`: exclude
        /// * `false`: not exclude
        /// </param>
        public void SetExcludeRecentlyMet(bool value)
        {
            CLIB.ppf_RoomOptions_SetExcludeRecentlyMet(Handle, value);
        }

        /// <summary>
        /// Sets the maximum number of users to return.
        /// </summary>
        /// <param name="value">The maximum number of users to return.</param>
        public void SetMaxUserResults(uint value)
        {
            CLIB.ppf_RoomOptions_SetMaxUserResults(Handle, value);
        }

        /// <summary>
        /// Sets a room ID.
        /// @note Only available to \ref RoomService.GetInvitableUsers2.
        /// </summary>
        /// <param name="value">The room ID.</param>
        public void SetRoomId(UInt64 value)
        {
            CLIB.ppf_RoomOptions_SetRoomId(Handle, value);
        }

        /// <summary>
        /// Sets a name for the room.
        /// @note Only available to \ref RoomService.JoinOrCreateNamedRoom and \ref RoomService.GetNamedRooms.
        /// </summary>
        /// <param name="roomName">The room's name. The maximum length is 64 characters.</param>
        public void SetRoomName(string roomName)
        {
            CLIB.ppf_RoomOptions_SetName(Handle, roomName);
        }

        /// <summary>
        /// Sets a password for the room.
        /// @note Only available to \ref RoomService.JoinOrCreateNamedRoom and \ref RoomService.GetNamedRooms.
        /// </summary>
        /// <param name="password">The room's password. The maximum length is 64 characters.</param>
        public void SetPassword(string password)
        {
            CLIB.ppf_RoomOptions_SetPassword(Handle, password);
        }

        /// <summary>
        /// Enables/Disables the update of room data.
        /// </summary>
        /// <param name="value">
        /// * `true`: enable
        /// * `false`: disable
        /// </param>
        public void SetTurnOffUpdates(bool value)
        {
            CLIB.ppf_RoomOptions_SetTurnOffUpdates(Handle, value);
        }


        /// For passing to native C
        public static explicit operator IntPtr(RoomOptions roomOptions)
        {
            return roomOptions != null ? roomOptions.Handle : IntPtr.Zero;
        }

        ~RoomOptions()
        {
            CLIB.ppf_RoomOptions_Destroy(Handle);
        }

        IntPtr Handle;

        public IntPtr GetHandle()
        {
            return Handle;
        }
    }
}