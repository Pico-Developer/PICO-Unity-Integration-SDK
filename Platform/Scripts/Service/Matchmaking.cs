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
    public static class MatchmakingService
    {
        /// <summary>Reports the result of a skill-rating match.
        /// @note  Applicable to the following matchmaking modes: Quickmatch, Browse (+ Skill Pool)</summary>
        /// 
        /// <param name="roomId">The room ID.</param>
        /// <param name="data">The key-value pairs.</param>
        /// <returns>Request information of type `Task`, including the request ID, and its response message does not contain data.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |3006209|match result report: not in match|
        /// |3006210|match result report: error report data|
        /// |3006211|match result report: duplicate report|
        /// |3006212|match result report: conflict with other's report|
        ///
        /// Only for pools with skill-based matchmaking. 
        /// Call this method after calling `StartMatch()` to begin a skill-rating
        /// match. After the match finishes, the server will record the result and
        /// update the skill levels of all players involved based on the result. This
        /// method is insecure because, as a client API, it is susceptible to tampering
        /// and therefore cheating to manipulate skill ratings.
        ///
        /// A message of type `MessageType.Matchmaking_ReportResultInsecure` will be generated in response.
        /// First call `Message.IsError()` to check if any error has occurred.
        /// This response has no payload. If no error has occurred, the request is successful.
        /// </returns>
        public static Task ReportResultsInsecure(UInt64 roomId, Dictionary<string, int> data)
        {
            KVPairArray kvarray = new KVPairArray((uint) data.Count);
            uint n = 0;
            foreach (var d in data)
            {
                var item = kvarray.GetElement(n);
                item.SetKey(d.Key);
                item.SetIntValue(d.Value);
                n++;
            }

            return new Task(CLIB.ppf_Matchmaking_ReportResultInsecure(roomId, kvarray.GetHandle(), kvarray.Size));
        }

        /// <summary>Gets the matchmaking statistics for the current user.
        /// @note  Applicable to the following matchmaking modes: Quickmatch, Browse</summary>
        /// 
        /// <param name="pool">The pool to look in.</param>
        /// <param name="maxLevel">(beta feature, don't use it)</param>
        /// <param name="approach">(beta feature, don't use it)</param>
        /// <returns>Request information of type `Task`, including the request ID, and its response message will contain data of type `MatchmakingStats`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |3006201|match enqueue: invalid pool name|
        /// |3006208|match enqueue: no skill|
        ///
        ///
        /// When given a pool, the system will look up the current user's wins, losses, draws and skill
        /// level. The skill level returned will be between `1` and the maximum level. The approach
        /// will determine how should the skill level rise toward the maximum level.
        ///
        /// A message of type `MessageType.Matchmaking_GetStats` will be generated in response.
        /// First call `Message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `MatchmakingStats`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<MatchmakingStats> GetStats(string pool, uint maxLevel, MatchmakingStatApproach approach = MatchmakingStatApproach.Trailing)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<MatchmakingStats>(CLIB.ppf_Matchmaking_GetStats(pool, maxLevel, approach));
        }

        /// <summary>Gets rooms by matchmakinging pool name.
        /// The user can join the room with `RoomService.Join2 to`or cancel the retrieval with `MatchmakingService.Cancel`.
        /// @note  Applicable to the following matchmaking mode: Browse</summary>
        ///
        /// <param name="pool">The matchmaking pool name you want to browse.</param>
        /// <param name="matchmakingOptions">(Optional) The matchmaking configuration of the browse request.</param>
        /// <returns>Request information of type `Task`, including the request ID, and its response message will contain data of type `MatchmakingBrowseResult`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |3006201|match enqueue: invalid pool name|
        /// |3006205|match browse: access denied|
        /// |3006207|match enqueue: invalid query key|
        ///
        /// A message of type `MessageType.Matchmaking_Browse2` will be generated in response.
        /// First call `Message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `MatchmakingBrowseResult`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<MatchmakingBrowseResult> Browse2(string pool, MatchmakingOptions matchmakingOptions = null)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }
            
            if (matchmakingOptions == null)
            {
                return new Task<MatchmakingBrowseResult>(CLIB.ppf_Matchmaking_Browse2(pool, IntPtr.Zero));
            }
            else
            {
                return new Task<MatchmakingBrowseResult>(CLIB.ppf_Matchmaking_Browse2(pool, matchmakingOptions.GetHandle()));
            }
        }
        
        /// <summary>Gets rooms by matchmakinging pool name and specify the page number and the number of pages per page.</summary>
        ///
        /// <param name="pool">The matchmaking pool name you want to browse.</param>
        /// <param name="matchmakingOptions">(Optional) The matchmaking configuration of the browse request.</param>
        /// <param name="pageIndex">(Optional)Start page index.</param>
        /// <param name="pageSize">(Optional)the number of pages per page.</param>
        /// <returns>Request information of type `Task`, including the request ID, and its response message will contain data of type `MatchmakingBrowseResult`.
        ///
        /// A message of type `MessageType.Matchmaking_Browse2CustomPage` will be generated in response.
        /// First call `Message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `MatchmakingBrowseResult`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<MatchmakingBrowseResult> Browse2ForCustomPage(string pool, MatchmakingOptions matchmakingOptions = null, int pageIndex = 0, int pageSize = 5)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }
            
            if (matchmakingOptions == null)
            {
                return new Task<MatchmakingBrowseResult>(CLIB.ppf_Matchmaking_Browse2CustomPage(pool, IntPtr.Zero, pageIndex, pageSize));
            }
            else
            {
                return new Task<MatchmakingBrowseResult>(CLIB.ppf_Matchmaking_Browse2CustomPage(pool, matchmakingOptions.GetHandle(), pageIndex, pageSize));
            }
        }

        /// <summary>Cancels a matchmaking request. Call this function
        /// to cancel an enqueue request before a match
        /// is made. This is typically triggered when a user gives up waiting.
        /// If you do not cancel the request but the user goes offline, the user/room
        /// will be timed out according to the setting of reserved period on the PICO Developer Platform.
        /// @note  Applicable to the following matchmaking modes: Quickmatch, Browse</summary>
        /// 
        /// <returns>Request information of type `Task`, including the request ID, and its response message does not contain data.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |3006201|match enqueue: invalid pool name|
        /// |3006206|match cancel: not in match|
        /// |3006301|server error: unknown|
        /// 
        ///
        /// A message of type `MessageType.Matchmaking_Cancel2` will be generated in response.
        /// Call `Message.IsError()` to check if any error has occurred.
        /// This response has no payload. If no error has occurred, the request is successful.
        /// </returns>
        public static Task Cancel()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task(CLIB.ppf_Matchmaking_Cancel2());
        }

        /// <summary>Creates a matchmaking room, then enqueues and joins it.
        /// @note  Applicable to the following matchmaking modes: Quickmatch, Browse, Advanced (Can Users Create Rooms=`true`)</summary>
        /// 
        /// <param name="pool">The matchmaking pool to use, which is created on the PICO Developer Platform.</param>
        /// <param name="matchmakingOptions">(Optional) Additional matchmaking configuration for this request.</param>
        /// <returns>Request information of type `Task`, including the request ID, and its response message will contain data of type `MatchmakingEnqueueResultAndRoom`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |3006201|match enqueue: invalid pool name|
        /// |3006203|match create room: pool config not allow user create room|
        /// |3006207|match enqueue: invalid query key |
        /// |3006301|server error: unknown |
        /// |3006204|match enqueue: invalid room id(Assigned room id, present in this context, indicates an internal server error) |
        /// |3006103|invalid room(The room was found to be invalid when joining the room, which appears in this context, indicating an internal server error) |
        /// |3006102|duplicate join room(Duplicate joins are found when joining a room, which appears in this context, indicating an internal server error) |
        /// |3006106|exceed max room player number(Exceeding the maximum number of people when joining a room, appears in this context, indicating an internal server error) |
        /// |3006105|illegal enter request(Illegal incoming requests, such as not in the allowed whitelist, appear in this context, indicating an internal server error) |
        /// |3006108|room is locked(When joining a room, it is found that the room is locked, appears in this context, indicating an internal server error)|
        ///
        /// A message of type `MessageType.Matchmaking_CreateAndEnqueueRoom2` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `MatchmakingEnqueueResultAndRoom`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<MatchmakingEnqueueResultAndRoom> CreateAndEnqueueRoom2(string pool, MatchmakingOptions matchmakingOptions = null)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }
            
            if (matchmakingOptions == null)
            {
                return new Task<MatchmakingEnqueueResultAndRoom>(CLIB.ppf_Matchmaking_CreateAndEnqueueRoom2(pool, IntPtr.Zero));
            }
            else
            {
                return new Task<MatchmakingEnqueueResultAndRoom>(CLIB.ppf_Matchmaking_CreateAndEnqueueRoom2(pool, matchmakingOptions.GetHandle()));
            }
        }

        /// <summary>Enqueues for an available matchmaking room to join.
        /// When the server finds a match, it will return a message of
        /// type `MessageType.Notification_Matchmaking_MatchFound`. You
        /// can join found matching rooms by calling `RoomService.Join2`. 
        /// If you want to cancel the match early, you can use `MatchmakingService.Cancel`.
        /// @note  Applicable to the following matchmaking mode: Quickmatch</summary>
        /// 
        /// <param name="pool">The matchmaking pool to use, which is defined on the PICO Developer Platform.</param>
        /// <param name="matchmakingOptions">(Optional) Match configuration for Enqueue.</param>
        /// <returns>Request information of type `Task`, including the request ID, and its response message will contain data of type `MatchmakingEnqueueResult`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |3006201|match enqueue: invalid pool name|
        /// |3006401|logic state checking failed|
        /// |3006207|match enqueue: invalid query key|
        /// |3006301|server error: unknown|
        ///
        /// A message of type `MessageType.Matchmaking_Enqueue2` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `MatchmakingEnqueueResult`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<MatchmakingEnqueueResult> Enqueue2(string pool, MatchmakingOptions matchmakingOptions = null)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            if (matchmakingOptions == null)
            {
                return new Task<MatchmakingEnqueueResult>(CLIB.ppf_Matchmaking_Enqueue2(pool, IntPtr.Zero));
            }
            else
            {
                return new Task<MatchmakingEnqueueResult>(CLIB.ppf_Matchmaking_Enqueue2(pool, matchmakingOptions.GetHandle()));
            }
        }

        /// <summary>Debugs the state of the current matchmaking pool queue.
        /// @note 
        /// * This function should not be used in production.
        /// * Applicable to the following matchmaking modes: Quickmatch, Browse
        ///
        /// </summary>
        /// 
        /// <returns>Request information of type `Task`, including the request ID, and its response message will contain data of type `MatchmakingAdminSnapshot`.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |3006201|match enqueue: invalid pool name|
        /// |3006301|server error: unknown |
        ///
        /// A message of type `MessageType.Matchmaking_GetAdminSnapshot` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `MatchmakingAdminSnapshot`.
        /// Extract the payload from the message handle with `message.Data`.
        /// </returns>
        public static Task<MatchmakingAdminSnapshot> GetAdminSnapshot()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task<MatchmakingAdminSnapshot>(CLIB.ppf_Matchmaking_GetAdminSnapshot());
        }

        /// <summary>Reports that a skill-rating match has started.
        /// You can use this method after joining the room.
        /// @note 
        /// * This function is only for pools with skill-based matching.
        /// * Applicable to the following matchmaking modes: Quickmatch, Browse (+ Skill Pool)
        /// </summary>
        ///
        /// <param name="roomId">The ID of the room you want to match.</param>
        /// <returns>Request information of type `Task`, including the request ID, and its response message does not contain data.
        ///
        /// A message of type `MessageType.Matchmaking_StartMatch` will be generated in response.
        /// Call `message.IsError()` to check if any error has occurred.
        /// </returns>
        public static Task StartMatch(UInt64 roomId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            return new Task(CLIB.ppf_Matchmaking_StartMatch(roomId));
        }

        /// <summary>Sets the callback to get notified when a match has been found. For example,
        /// after calling `MatchmakingService.Enqueue`, when the match is successful, you will
        /// receive `Notification_Matchmaking_MatchFound`, and then execute the processing function
        /// set by this function.</summary>
        /// 
        /// <param name="handler">The callback function will be called when receiving the `Notification_Matchmaking_MatchFound` message.</param>
        public static void SetMatchFoundNotificationCallback(Message<Room>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Matchmaking_MatchFound, handler);
        }

        /// <summary>A notification will be sent to the player after they have been kicked out of the matchmaking pool.
        /// Listen to the event to receive a message.</summary> 
        /// 
        /// <param name="handler">The callback function will be called when receiving the `Matchmaking_Cancel2` message and the value of `requestID` is `0`.</param>
        public static void SetCancel2NotificationCallback(Message.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Matchmaking_Cancel2, handler);
        }
    }


    public class MatchmakingOptions
    {
        public MatchmakingOptions()
        {
            Handle = CLIB.ppf_MatchmakingOptions_Create();
        }

        /// <summary>
        /// Sets the data store for a room.
        /// </summary>
        /// <param name="key">A unique identifier that maps to a value.</param>
        /// <param name="value">The data.</param>
        public void SetCreateRoomDataStore(string key, string value)
        {
            CLIB.ppf_MatchmakingOptions_SetCreateRoomDataStoreString(Handle, key, value);
        }

        /// <summary>
        /// Clears the data store for a room.
        /// </summary>
        public void ClearCreateRoomDataStore()
        {
            CLIB.ppf_MatchmakingOptions_ClearCreateRoomDataStore(Handle);
        }

        /// <summary>
        /// Sets a join policy for a room.
        /// </summary>
        /// <param name="value">The enumerations of join policy:
        /// * `0`: None
        /// * `1`: Everyone
        /// * `2`: FriendsOfMembers
        /// * `3`: FriendsOfOwner
        /// * `4`: InvitedUsers
        /// * `5`: Unknown
        /// </param>
        public void SetCreateRoomJoinPolicy(RoomJoinPolicy value)
        {
            CLIB.ppf_MatchmakingOptions_SetCreateRoomJoinPolicy(Handle, value);
        }

        /// <summary>
        /// Sets the maximum number of users allowed for a room.
        /// </summary>
        /// <param name="value">The maximum number of users.</param>
        public void SetCreateRoomMaxUsers(uint value)
        {
            CLIB.ppf_MatchmakingOptions_SetCreateRoomMaxUsers(Handle, value);
        }

        /// <summary>
        /// Sets an integer data setting for a query of a matchmaking pool.
        /// </summary>
        /// <param name="key">A unique identifier that maps a value.</param>
        /// <param name="value">The data (integer).</param>
        public void SetEnqueueDataSettings(string key, int value)
        {
            CLIB.ppf_MatchmakingOptions_SetEnqueueDataSettingsInt(Handle, key, value);
        }

        /// <summary>
        /// Sets a float data setting for a query of a matchmaking pool.
        /// </summary>
        /// <param name="key">A unique identifier that maps a value.</param>
        /// <param name="value">The data.</param>
        public void SetEnqueueDataSettings(string key, double value)
        {
            CLIB.ppf_MatchmakingOptions_SetEnqueueDataSettingsDouble(Handle, key, value);
        }

        /// <summary>
        /// Sets a string data setting for a query of a matchmaking pool.
        /// </summary>
        /// <param name="key">A unique identifier that maps a value.</param>
        /// <param name="value">The data.</param>
        public void SetEnqueueDataSettings(string key, string value)
        {
            CLIB.ppf_MatchmakingOptions_SetEnqueueDataSettingsString(Handle, key, value);
        }

        /// <summary>
        /// Clears data settings for a query of a matchmaking pool.
        /// </summary>
        public void ClearEnqueueDataSettings()
        {
            CLIB.ppf_MatchmakingOptions_ClearEnqueueDataSettings(Handle);
        }

        /// <summary>
        /// Sets whether to return the debugging information.
        /// </summary>
        /// <param name="value">
        /// * `true`: return the debugging information with the response payload
        /// * `false`: do not return the debugging information
        /// </param>
        public void SetEnqueueIsDebug(bool value)
        {
            CLIB.ppf_MatchmakingOptions_SetEnqueueIsDebug(Handle, value);
        }

        /// <summary>
        /// Sets the query for a matchmaking.
        /// </summary>
        /// <param name="value">The key of the target query.
        /// @note One matchmaking pool can include multiple queries which are created on the PICO Developer Platform.
        /// You can choose which query to use before starting a matchmaking.
        /// </param>
        public void SetEnqueueQueryKey(string value)
        {
            CLIB.ppf_MatchmakingOptions_SetEnqueueQueryKey(Handle, value);
        }


        /// For passing to native C
        public static explicit operator IntPtr(MatchmakingOptions matchmakingOptions)
        {
            return matchmakingOptions != null ? matchmakingOptions.Handle : IntPtr.Zero;
        }

        ~MatchmakingOptions()
        {
            CLIB.ppf_MatchmakingOptions_Destroy(Handle);
        }

        IntPtr Handle;

        public IntPtr GetHandle()
        {
            return Handle;
        }
    }
}