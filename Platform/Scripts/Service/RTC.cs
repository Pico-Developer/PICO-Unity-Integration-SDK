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
using AOT;
using Pico.Platform.Models;
using UnityEngine;
using UnityEngine.Android;

namespace Pico.Platform
{
    /**
     * \ingroup Platform
     *
     * Real-time communications (RTC) technology enables users in the same room to communicate with each other through voice chat.
     *
     * RTC service uses a centralized communication structure instead of an end-to-end one. After users have joined a room and enabled voice chat, the microphone keeps capturing audio data from users and uploading the data to the RTC server. Then, the RTC server transmits the audio data to each client in the room, and the client broadcasts the audio data received.
     */
    public static class RtcService
    {
        /// <summary>
        /// Initializes the RTC engine.
        /// @note  You should call this method before using the RTC service.
        /// </summary>
        /// <returns>The status that indicates whether the initialization is successful.</returns>
        public static RtcEngineInitResult InitRtcEngine()
        {
            if (Application.platform == RuntimePlatform.Android && !Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }

            return CLIB.ppf_Rtc_InitRtcEngine();
        }

        /// <summary>
        /// Gets the token required by \ref JoinRoom.
        ///
        /// </summary>
        /// <param name="roomId">The ID of the room that the token is for.</param>
        /// <param name="userId">The ID of the user that the token is for.</param>
        /// <param name="ttl">The time-to-live (ttl) of the token. The unit is seconds.
        /// The user will be kicked out from the room after ttl seconds.
        /// </param>
        /// <param name="privileges">The dictionary that maps privilege to ttl. The unit is seconds. </param>
        public static Task<string> GetToken(string roomId, string userId, int ttl, Dictionary<RtcPrivilege, int> privileges)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            var tokenOption = new RtcGetTokenOptions();
            tokenOption.SetRoomId(roomId);
            tokenOption.SetUserId(userId);
            tokenOption.SetTtl(ttl);
            if (privileges != null)
            {
                foreach (var i in privileges)
                {
                    tokenOption.SetPrivileges(i.Key, i.Value);
                }
            }

            return new Task<string>(CLIB.ppf_Rtc_GetToken((IntPtr) tokenOption));
        }

        /// <summary>
        /// Joins a user to a specified room.
        ///
        /// @note 
        /// * If code `0` is returned, you should use \ref SetOnJoinRoomResultCallback to handle the
        /// final join room result.
        /// * If a non-zero code is returned, you should call \ref LeaveRoom firstly to join the room in the next time.
        /// </summary>
        /// <param name="roomId">The ID of the room to join.</param>
        /// <param name="userId">The ID of user.</param>
        /// <param name="token">The token required for joining the room. You can get the token by calling \ref GetToken.</param>
        /// <param name="roomProfileType">Room type:
        ///    * `0`: communication room
        ///    * `1`: live broadcasting room
        ///    * `2`: game room
        ///    * `3`: cloud game room
        ///    * `4`: low-latency room
        /// </param>
        /// <param name="isAutoSubscribeAudio">Whether to automatically subscribe to the audio in the room:
        /// * `true`: subscribe
        /// * `false`: do not subscribe
        /// </param>
        /// <returns>`0` indicates success, and other codes indicate failure.
        /// | Code| Description |
        /// |---|---|
        /// |0|Success.|
        /// |-1|Invalid `roomID` or `userId`.|
        /// |-2|The user is already in this room.|
        /// |-3|The RTC engine is null. You should initialize the RTC engine before joining a room.|
        /// |-4|Creating the room failed.|
        /// </returns>
        public static int JoinRoom(string roomId, string userId, string token, RtcRoomProfileType roomProfileType, bool isAutoSubscribeAudio)
        {
            var roomOption = new RtcRoomOptions();
            roomOption.SetRoomId(roomId);
            roomOption.SetUserId(userId);
            roomOption.SetToken(token);
            roomOption.SetRoomProfileType(roomProfileType);
            roomOption.SetIsAutoSubscribeAudio(isAutoSubscribeAudio);
            return CLIB.ppf_Rtc_JoinRoom((IntPtr) roomOption);
        }

        /// <summary>
        /// Joins a user to a room.
        /// </summary>
        /// <param name="joinRoomOptions">The options to join a room.</param>
        /// <param name="leaveIfInRoom">Retry to join the room if the request returns error code `-2` (the user is already in the room).</param>
        /// <returns>`0` indicates success, and other codes indicate failure.
        /// | Code| Description |
        /// |---|---|
        /// |0|Success.|
        /// |-1|Invalid `roomID` or `userId`.|
        /// |-2|The user is already in this room.|
        /// |-3|The RTC engine is null. You should initialize the RTC engine before joining a room.|
        /// |-4|Creating the room failed.|
        /// </returns>
        public static int JoinRoom2(RtcRoomOptions joinRoomOptions, bool leaveIfInRoom = true)
        {
            var res = CLIB.ppf_Rtc_JoinRoom((IntPtr) joinRoomOptions);
            if (leaveIfInRoom && res == -2)
            {
                LeaveRoom(joinRoomOptions.RoomId);
                res = CLIB.ppf_Rtc_JoinRoom((IntPtr) joinRoomOptions);
            }

            return res;
        }

        /// <summary>
        /// Leaves a room and retries to join it when the previous request fails and returns error code `-2` (the user is already in this room).
        /// </summary>
        /// <param name="roomId">The ID of the room to join.</param>
        /// <param name="userId">The ID of user.</param>
        /// <param name="token">The token required for joining the room. You can get the token by calling `GetToken`.</param>
        /// <param name="roomProfileType">Room type:
        ///    * `0`: communication room
        ///    * `1`: live broadcasting room
        ///    * `2`: game room
        ///    * `3`: cloud game room
        ///    * `4`: low-latency room
        /// </param>
        /// <param name="isAutoSubscribeAudio">Whether to automatically subscribe to the audio in the room:
        /// * `true`: subscribe
        /// * `false`: do not subscribe
        /// </param>
        /// <returns>`0` indicates success, and other codes indicate failure.
        /// | Code| Description |
        /// |---|---|
        /// |0|Success.|
        /// |-1|Invalid `roomID` or `userId`.|
        /// |-2|The user is already in this room.|
        /// |-3|The RTC engine is null. You should initialize the RTC engine before joining a room.|
        /// |-4|Creating the room failed.|
        /// </returns>
        public static int JoinRoomWithRetry(string roomId, string userId, string token, RtcRoomProfileType roomProfileType, bool isAutoSubscribeAudio)
        {
            var roomOption = new RtcRoomOptions();
            roomOption.SetRoomId(roomId);
            roomOption.SetUserId(userId);
            roomOption.SetToken(token);
            roomOption.SetRoomProfileType(roomProfileType);
            roomOption.SetIsAutoSubscribeAudio(isAutoSubscribeAudio);
            var res = CLIB.ppf_Rtc_JoinRoom((IntPtr) roomOption);
            if (res == -2)
            {
                LeaveRoom(roomId);
                res = CLIB.ppf_Rtc_JoinRoom((IntPtr) roomOption);
            }

            return res;
        }

        /// <summary>
        /// Leaves a specified room.
        /// </summary>
        /// <param name="roomId">The ID of the room to leave.</param>
        /// <returns>`0` indicates success, and other codes indicate failure.
        ///
        /// | Code| Description |
        /// |---|---|
        /// |0|Success.|
        /// |-1|The RTC engine is not initialized.|
        /// |-2|The user is not in the room.|
        /// </returns>
        public static int LeaveRoom(string roomId)
        {
            return CLIB.ppf_Rtc_LeaveRoom(roomId);
        }

        /// <summary>
        /// Sets the audio playback device.
        /// </summary>
        /// <param name="device">The device ID.</param>
        public static void SetAudioPlaybackDevice(RtcAudioPlaybackDevice device)
        {
            CLIB.ppf_Rtc_SetAudioPlaybackDevice(device);
        }

        /// <summary>
        /// Unsubscribing from all the audio streams of a room, thereby making the local user unable to hear anything from the room.
        /// </summary>
        /// <param name="roomId">The ID of the room whose audio streams are to be unsubscribed from.</param>
        public static void RoomPauseAllSubscribedStream(string roomId)
        {
            CLIB.ppf_Rtc_RoomPauseAllSubscribedStream(roomId, RtcPauseResumeMediaType.Audio);
        }

        /// <summary>
        /// Resubscribing to all the audio streams of a room, thereby making the local user hear the voice from every in-room user.
        /// </summary>
        /// <param name="roomId">The ID of the room whose audio streams are to be resubscribed to.</param>
        public static void RoomResumeAllSubscribedStream(string roomId)
        {
            CLIB.ppf_Rtc_RoomResumeAllSubscribedStream(roomId, RtcPauseResumeMediaType.Audio);
        }

        public delegate int ProcessAudioFrameFunction(RtcAudioFrame frame);

        static ProcessAudioFrameFunction audioProcessor = null;

        [MonoPInvokeCallback(typeof(CLIB.RtcProcessAudioFrameFunction))]
        static int InnerAudioProcessor(IntPtr ptr)
        {
            if (audioProcessor != null)
            {
                return audioProcessor(new RtcAudioFrame(ptr));
            }

            return 0;
        }

        /// <summary>
        /// Register local audio processor. You can use this function to modify the recorded audio.
        /// </summary>
        /// <param name="processor">The processor function.
        /// If it returns 0,means the function didn't change the data.
        /// If its return value is negative integer, means the function encounters error.
        /// If its return value is positive integer, means the function handled successfully.
        /// </param>
        /// <param name="channel">The channel of the audio you want to process.</param>
        /// <param name="sampleRate">The sample rate of the audio you want to process.</param>
        public static void RegisterLocalAudioProcessor(ProcessAudioFrameFunction processor, RtcAudioChannel channel, RtcAudioSampleRate sampleRate)
        {
            // here should hold the processor to avoid GC delete the processor and `InnerAudioProcessor` will call this. 
            audioProcessor = processor;
            CLIB.ppf_Rtc_RegisterLocalAudioProcessor(InnerAudioProcessor, channel, sampleRate);
        }

        /// <summary>
        /// Enables audio properties report. Once enabled, you will regularly receive audio report data.
        /// </summary>
        /// <param name="interval">
        /// The interval (in milliseconds) between one report and the next. You can set this parameter to `0` or any negative integer to stop receiving audio properties report.
        /// For any integer between (0, 100), the SDK will regard it as invalid and automatically set this parameter to `100`; any integer equal to or greater than `100` is valid.
        /// </param>
        public static void EnableAudioPropertiesReport(int interval)
        {
            var conf = new RtcAudioPropertyOptions();
            conf.SetInterval(interval);
            CLIB.ppf_Rtc_EnableAudioPropertiesReport((IntPtr) conf);
        }

        /// <summary>
        /// Publishes the local audio stream to a room, thereby making the local user's voice heard by other in-room users.
        /// @note
        /// * A user can only publish the local audio stream to one room at the same time.
        /// * If a user wants to publish the local audio stream to another room, 
        /// `UnPublishRoom(oldRoomId)` should be called first to stop publishing the local audio stream to the current room and then `Publish(newRoomId)` should be called.
        /// </summary>
        /// <param name="roomId">The ID of the room that the local audio stream is published to.</param>
        public static void PublishRoom(string roomId)
        {
            CLIB.ppf_Rtc_RoomPublishStream(roomId, RtcMediaStreamType.Audio);
        }

        /// <summary>
        /// Stops publishing the local audio stream to a room, so other in-room users are unable to hear the local user's voice.
        /// </summary>
        /// <param name="roomId">The ID of the room to stop publishing the local audio stream to.</param>
        public static void UnPublishRoom(string roomId)
        {
            CLIB.ppf_Rtc_RoomUnPublishStream(roomId, RtcMediaStreamType.Audio);
        }

        /// <summary>
        /// Destroys a specified room. The resources occupied by the room will be released after destruction.
        /// </summary>
        /// <param name="roomId">The ID of the room to destroy.</param>
        public static void DestroyRoom(string roomId)
        {
            CLIB.ppf_Rtc_DestroyRoom(roomId);
        }

        /// <summary>
        /// Starts audio capture via the microphone.
        /// </summary>
        public static void StartAudioCapture()
        {
            CLIB.ppf_Rtc_StartAudioCapture();
        }

        /// <summary>
        /// Stops audio capture.
        /// </summary>
        public static void StopAudioCapture()
        {
            CLIB.ppf_Rtc_StopAudioCapture();
        }

        /// <summary>
        /// Sets the volume of the captured audio.
        /// </summary>
        /// <param name="volume">The target volume. The valid value ranges from `0` to `400`. `100` indicates keeping the original volume.</param>
        public static void SetCaptureVolume(int volume)
        {
            CLIB.ppf_Rtc_SetCaptureVolume(RtcStreamIndex.Main, volume);
        }

        /// <summary>
        /// Sets the playback volume.
        /// </summary>
        /// <param name="volume">The target volume. The valid value ranges from `0` to `400`. `100` indicates keeping the original volume.</param>
        public static void SetPlaybackVolume(int volume)
        {
            CLIB.ppf_Rtc_SetPlaybackVolume(volume);
        }

        /// <summary>
        /// Switches the in-ear monitoring mode on/off. Once the in-ear monitoring mode is enabled, one can hear their own voice.
        /// </summary>
        /// <param name="mode">Whether to switch the in-ear monitoring mode on/off:
        /// * `0`: off
        /// * `1`: on
        /// </param>
        public static void SetEarMonitorMode(RtcEarMonitorMode mode)
        {
            CLIB.ppf_Rtc_SetEarMonitorMode(mode);
        }

        /// <summary>
        /// Sets the volume for in-ear monitoring.
        /// </summary>
        /// <param name="volume">The target volume. The valid value range from `0` to `400`.</param>
        public static void SetEarMonitorVolume(int volume)
        {
            CLIB.ppf_Rtc_SetEarMonitorVolume(volume);
        }

        /// @deprecated MuteLocalAudio() can be replaced by \ref UnPublishRoom(string roomId)
        /// <summary>
        /// Mutes local audio to make one's voice unable to be heard by other in-room users.
        /// </summary>
        /// <param name="rtcMuteState">The state of local audio:
        /// * `0`: off
        /// * `1`: on
        /// </param>
        [Obsolete("MuteLocalAudio can be replaced by UnPublishRoom(roomId)", true)]
        public static void MuteLocalAudio(RtcMuteState rtcMuteState)
        {
            CLIB.ppf_Rtc_MuteLocalAudio(rtcMuteState);
        }

        /// <summary>
        /// Updates the token in a room.
        ///
        /// When a token's ttl is about to expire, you will receive a notification
        /// through \ref SetOnTokenWillExpire. If you still want to stay in the room,
        /// you should call \ref GetToken to get a new token and call \ref UpdateToken
        /// with the new token. If you don't update token timely,you will be kicked
        /// out from the room. 
        /// </summary>
        /// <param name="roomId">The ID of the room you are in.</param>
        /// <param name="token">The token to update.</param>
        public static void UpdateToken(string roomId, string token)
        {
            CLIB.ppf_Rtc_UpdateToken(roomId, token);
        }

        /// <summary>
        /// Sets the audio scenario.
        /// @note  Different audio scenarios can impact the voice quality and how the earphones work.
        /// </summary>
        /// <param name="scenarioType">The audio scenario type:
        /// * `0`: Music
        /// * `1`: HighQualityCommunication
        /// * `2`: Communication
        /// * `3`: Media
        /// * `4`: GameStreaming
        /// </param>
        public static void SetAudioScenario(RtcAudioScenarioType scenarioType)
        {
            CLIB.ppf_Rtc_SetAudioScenario(scenarioType);
        }

        /// <summary>
        /// Sets the volume for a remote user in a room.
        /// </summary>
        /// <param name="roomId">The ID of the room.</param>
        /// <param name="userId">The ID of the remote user.</param>
        /// <param name="volume">The volume to set for the remote user, which ranges from `0` to `400` and `100` indicates the default volume.</param>
        public static void RoomSetRemoteAudioPlaybackVolume(string roomId, string userId, int volume)
        {
            CLIB.ppf_Rtc_RoomSetRemoteAudioPlaybackVolume(roomId, userId, volume);
        }

        /// <summary>
        /// Subscribes to the audio stream of a specific user in a room.
        /// </summary>
        /// <param name="roomId">The ID of the room.</param>
        /// <param name="userId">The ID of the user in the room.</param>
        public static void RoomSubscribeStream(string roomId, string userId)
        {
            CLIB.ppf_Rtc_RoomSubscribeStream(roomId, userId, RtcMediaStreamType.Audio);
        }

        /// <summary>
        /// Unsubscribes from the audio stream of a specific user in a room.
        /// </summary>
        /// <param name="roomId">The ID of the room.</param>
        /// <param name="userId">The ID of the user in the room.</param>
        public static void RoomUnSubscribeStream(string roomId, string userId)
        {
            CLIB.ppf_Rtc_RoomUnsubscribeStream(roomId, userId, RtcMediaStreamType.Audio);
        }

        /// <summary>
        /// Sends a binary message to a room. All in-room users will receive this message.
        ///
        /// The message's bytes size shouldn't be greater than 64kB.
        /// </summary>
        /// <param name="roomId">The ID of the room.</param>
        /// <param name="message">The binary message to be sent.</param>
        /// <returns>A room message ID of the int64 type, which is automatically generated and incremented.</returns>
        public static long SendRoomBinaryMessage(string roomId, byte[] message)
        {
            var ptr = new PtrManager(message);
            var ans = CLIB.ppf_Rtc_SendRoomBinaryMessage(roomId, ptr.ptr, message.Length);
            ptr.Free();
            return ans;
        }

        /// <summary>
        /// Sends a text message to a room. All in-room users will receive this message.
        ///
        /// The message's bytes size shouldn't be greater than 64kB.
        /// </summary>
        /// <param name="roomId">The ID of the room.</param>
        /// <param name="message">The message to be sent.</param>
        /// <returns>A room message ID of the int64 type, which is automatically generated and incremented.</returns>
        public static long SendRoomMessage(string roomId, string message)
        {
            return CLIB.ppf_Rtc_SendRoomMessage(roomId, message);
        }

        /// <summary>
        /// Sends a binary message to a user. Only the user can receive this message.
        ///
        /// The message's bytes size shouldn't be greater than 64kB.
        /// </summary>
        /// <param name="roomId">The ID of the room the user is in.</param>
        /// <param name="userId">The ID of the user the message is sent to.</param>
        /// <param name="message">The message to be sent.</param>
        /// <returns>A user message ID of the int64 type, which is automatically generated and incremented.</returns>
        public static long SendUserBinaryMessage(string roomId, string userId, byte[] message)
        {
            var ptr = new PtrManager(message);
            var ans = CLIB.ppf_Rtc_SendUserBinaryMessage(roomId, userId, ptr.ptr, message.Length);
            ptr.Free();
            return ans;
        }

        /// <summary>
        /// Sends a text message to a user. Only the user can receive this message.
        /// The message's bytes size shouldn't be greater than 64kB.
        /// </summary>
        /// <param name="roomId">The ID of the room the user is in.</param>
        /// <param name="userId">The ID of the user the message is sent to.</param>
        /// <param name="message">The message to be sent.</param>
        /// <returns>A user message ID of the int64 type, which is automatically generated and incremented.</returns>
        public static long SendUserMessage(string roomId, string userId, string message)
        {
            return CLIB.ppf_Rtc_SendUserMessage(roomId, userId, message);
        }

        /// <summary>
        /// Sends stream sync info. The sync info data will be sent in the same packet with the audio data. Users who subscribe to this audio stream will receive the stream sync info message.
        /// </summary>
        /// <param name="data">The stream sync info.</param>
        /// <param name="repeatCount">The stream sync info will be sent repeatedly for the times set in `repeatCount`.
        /// It's designed to avoid losing package and ensuring that the sync info can be sent successfully.
        /// However, if `repeatCount` is too large, it will cause the sync info to pile up in the queue.
        /// Setting this parameter to `0` is recommended.
        /// </param>
        /// <returns>Any code equal to or below `0` indicates success, and others codes indicate failure. 
        /// | Code | Description|
        /// |---|---|
        /// |>=0|Send successfully.Indicates the times sent successfully.|
        /// |-1|Send Failed. Message length exceeded 255B|
        /// |-2|Send Failed. The data is empty.|
        /// |-3|Send Failed. Send sync info with a un-publish screen stream.|
        /// |-4|Send Failed. Send sync info with a un-publish audio stream.|
        /// </returns>
        public static int SendStreamSyncInfo(byte[] data, int repeatCount)
        {
            var config = new RtcStreamSyncInfoOptions();
            config.SetRepeatCount(repeatCount);
            config.SetStreamIndex(RtcStreamIndex.Main);
            config.SetStreamType(RtcSyncInfoStreamType.Audio);
            var ptr = new PtrManager(data);
            var ans = CLIB.ppf_Rtc_SendStreamSyncInfo(ptr.ptr, data.Length, (IntPtr) config);
            ptr.Free();
            return ans;
        }

        /// <summary>
        /// Sets the callback to get notified when the token is about to expire.
        /// @note The token will expire 30 seconds after you receive this notification.
        /// * If you still want to stay in the room, you can get a new token by calling `UpdateToken`.
        /// * If you do not update the token after receiving this notification, you will be kicked out of the room in 30 seconds.
        /// </summary>
        /// <param name="handler">The callback function, the string in the message indicates the room ID.</param>
        public static void SetOnTokenWillExpire(Message<string>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnTokenWillExpire, handler);
        }

        /// <summary>
        /// Sets the callback to get notified when a to-room message is received.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnRoomMessageReceived(Message<RtcRoomMessageReceived>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnRoomMessageReceived, handler);
        }

        /// <summary>
        /// Sets the callback to get notified when a to-room binary message is received.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnRoomBinaryMessageReceived(Message<RtcBinaryMessageReceived>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnRoomBinaryMessageReceived, handler);
        }

        /// <summary>
        /// Sets the callback to get notified when a to-user message is received.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnUserMessageReceived(Message<RtcUserMessageReceived>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnUserMessageReceived, handler);
        }

        /// <summary>
        /// Sets the callback to get notified when a to-user binary message is received.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnUserBinaryMessageReceived(Message<RtcBinaryMessageReceived>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnUserBinaryMessageReceived, handler);
        }

        /// <summary>
        /// Sets the callback to get whether the to-room message is sent successfully.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnRoomMessageSendResult(Message<RtcMessageSendResult>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnRoomMessageSendResult, handler);
        }

        /// <summary>
        /// Sets the callback to get whether the to-user message is sent successfully.
        /// </summary>
        /// <param name="handler"></param>
        public static void SetOnUserMessageSendResult(Message<RtcMessageSendResult>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnUserMessageSendResult, handler);
        }

        /// <summary>
        /// Sets the callback to get notified when a remote user publishes audio stream.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnUserPublishStream(Message<RtcUserPublishInfo>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnUserPublishStream, handler);
        }

        /// <summary>
        /// Sets the callback to get notified when a remote user cancels publishing audio stream.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnUserUnPublishStream(Message<RtcUserUnPublishInfo>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnUserUnPublishStream, handler);
        }

        /// <summary>
        /// Sets the callback to get notified when the stream sync info is received.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnStreamSyncInfoReceived(Message<RtcStreamSyncInfo>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnStreamSyncInfoReceived, handler);
        }

        /// <summary>
        /// Sets the callback of `JoinRoom` to get `RtcJoinRoomResult`.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnJoinRoomResultCallback(Message<RtcJoinRoomResult>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnJoinRoom, handler);
        }

        /// <summary>
        /// Sets the callback of `LeaveRoom` to get `RtcLeaveRoomResult`.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnLeaveRoomResultCallback(Message<RtcLeaveRoomResult>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnLeaveRoom, handler);
        }

        /// <summary>
        /// Sets the callback to get notified when someone has joined the room.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnUserJoinRoomResultCallback(Message<RtcUserJoinInfo>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnUserJoinRoom, handler);
        }

        /// <summary>
        /// Sets the callback to get notified when someone has left the room.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnUserLeaveRoomResultCallback(Message<RtcUserLeaveInfo>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnUserLeaveRoom, handler);
        }

        /// <summary>
        /// Sets the callback to regularly get room statistics after joining a room.
        ///
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnRoomStatsCallback(Message<RtcRoomStats>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnRoomStats, handler);
        }

        /// <summary>
        /// Sets the callback to get warning messages from the RTC engine.
        /// The warning codes and descriptions are given below.
        ///
        /// |Warning Code|Description|
        /// |---|---|
        /// |-2001|Joining the room failed.|
        /// |-2002|Publishing audio stream failed.|
        /// |-2003|Subscribing to the audio stream failed because the stream cannot be found.|
        /// |-2004|Subscribing to the audio stream failed due to server error.|
        /// |-2013|When the people count in the room exceeds 500, the client will not be informed of user join and leave info anymore.|
        /// |-5001|The camera permission is missing.|
        /// |-5002|The microphone permission is missing.|
        /// |-5003|Starting the audio capture device failed.|
        /// |-5004|Starting the audio playback device failed.|
        /// |-5005|No available audio capture device.|
        /// |-5006|No available audio playback device.|
        /// |-5007|The audio capture device failed to capture valid audio data.|
        /// |-5008|Invalid media device operation.|
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnWarnCallback(Message<int>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnWarn, handler);
        }

        /// <summary>
        /// Sets the callback to get error messages from the RTC engine.
        /// The error codes and descriptions are given below.
        ///
        /// |Error Code|Description|
        /// |---|---|
        /// |-1000|Invalid token.|
        /// |-1001|Unknown error.|
        /// |-1002|No permission to publish audio stream.|
        /// |-1003|No permission to subscribe audio stream.|
        /// |-1004|A user with the same user Id joined this room. You are kicked out of the room.|
        /// |-1005|Incorrect configuration on the Developer Platform.|
        /// |-1007|Invalid room id.|
        /// |-1009|Token expired. You should get a new token and join the room.|
        /// |-1010|Token is invalid when you call `UpdateToken`|
        /// |-1011|The room is dismissed and all user is moved out from the room.|
        /// |-1070|Subscribing to audio stream failed. Perhaps the number of subscribed audio streams has exceeded the limit.|
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnErrorCallback(Message<int>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnError, handler);
        }

        /// <summary>
        /// Sets the callback to get warning messages from the room.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnRoomWarnCallback(Message<RtcRoomWarn>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnRoomWarn, handler);
        }

        /// <summary>
        /// Sets the callback to get error messages from the room.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnRoomErrorCallback(Message<RtcRoomError>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnRoomError, handler);
        }

        /// <summary>
        /// Sets the callback to get notified when the state of the connection to the RTC server has changed.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnConnectionStateChangeCallback(Message<RtcConnectionState>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnConnectionStateChange, handler);
        }

        /// <summary>
        /// Sets the callback to get notified when the user has muted local audio.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        [Obsolete("SetOnUserMuteAudio is deprecated,please use SetOnUserPublishStream/SetOnUserUnPublishStream", true)]
        public static void SetOnUserMuteAudio(Message<RtcMuteInfo>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnUserMuteAudio, handler);
        }

        /// <summary>
        /// Sets the callback to get notified when the user has started audio capture.
        ///
        /// When a remote user called \ref StartAudioCapture,RTC engine will call this callback.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnUserStartAudioCapture(Message<string>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnUserStartAudioCapture, handler);
        }

        /// <summary>
        /// Sets the callback to get notified when the user has stopped audio capture.
        ///
        /// When a remote user called \ref StopAudioCapture,RTC engine will call this callback.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnUserStopAudioCapture(Message<string>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnUserStopAudioCapture, handler);
        }

        /// <summary>
        /// Sets the callback to get notified when the audio playback device has been changed.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnAudioPlaybackDeviceChange(Message<RtcAudioPlaybackDevice>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnAudioPlaybackDeviceChanged, handler);
        }

        /// <summary>
        /// Sets the callback to receive local audio report.
        /// Rtc engine will call this callback periodically once you call \ref EnableAudioPropertiesReport.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnLocalAudioPropertiesReport(Message<RtcLocalAudioPropertiesReport>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnLocalAudioPropertiesReport, handler);
        }

        /// <summary>
        /// Sets the callback to receive remote audio report.
        /// Rtc engine will call this callback periodically once you call \ref EnableAudioPropertiesReport.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnRemoteAudioPropertiesReport(Message<RtcRemoteAudioPropertiesReport>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnRemoteAudioPropertiesReport, handler);
        }
    }

    public class RtcStreamSyncInfoOptions
    {
        private IntPtr Handle;

        public RtcStreamSyncInfoOptions()
        {
            Handle = CLIB.ppf_RtcStreamSyncInfoOptions_Create();
        }

        public void SetRepeatCount(int value)
        {
            CLIB.ppf_RtcStreamSyncInfoOptions_SetRepeatCount(Handle, value);
        }

        public void SetStreamIndex(RtcStreamIndex value)
        {
            CLIB.ppf_RtcStreamSyncInfoOptions_SetStreamIndex(Handle, value);
        }

        public void SetStreamType(RtcSyncInfoStreamType value)
        {
            CLIB.ppf_RtcStreamSyncInfoOptions_SetStreamType(Handle, value);
        }

        public static explicit operator IntPtr(RtcStreamSyncInfoOptions options)
        {
            return options != null ? options.Handle : IntPtr.Zero;
        }

        ~RtcStreamSyncInfoOptions()
        {
            CLIB.ppf_RtcStreamSyncInfoOptions_Destroy(Handle);
        }
    }

    public class RtcRoomOptions
    {
        public string RoomId;

        public RtcRoomOptions()
        {
            Handle = CLIB.ppf_RtcRoomOptions_Create();
        }


        public void SetRoomProfileType(RtcRoomProfileType value)
        {
            CLIB.ppf_RtcRoomOptions_SetRoomProfileType(Handle, value);
        }


        public void SetIsAutoSubscribeAudio(bool value)
        {
            CLIB.ppf_RtcRoomOptions_SetIsAutoSubscribeAudio(Handle, value);
        }

        public void SetRoomId(string value)
        {
            this.RoomId = value;
            CLIB.ppf_RtcRoomOptions_SetRoomId(Handle, value);
        }


        public void SetUserId(string value)
        {
            CLIB.ppf_RtcRoomOptions_SetUserId(Handle, value);
        }


        public void SetUserExtra(string value)
        {
            CLIB.ppf_RtcRoomOptions_SetUserExtra(Handle, value);
        }


        public void SetToken(string value)
        {
            CLIB.ppf_RtcRoomOptions_SetToken(Handle, value);
        }

        /// For passing to native C
        public static explicit operator IntPtr(RtcRoomOptions options)
        {
            return options != null ? options.Handle : IntPtr.Zero;
        }

        ~RtcRoomOptions()
        {
            CLIB.ppf_RtcRoomOptions_Destroy(Handle);
        }

        private IntPtr Handle;
    }

    public class RtcGetTokenOptions
    {
        public RtcGetTokenOptions()
        {
            Handle = CLIB.ppf_RtcGetTokenOptions_Create();
        }


        public void SetUserId(string value)
        {
            CLIB.ppf_RtcGetTokenOptions_SetUserId(Handle, value);
        }


        public void SetRoomId(string value)
        {
            CLIB.ppf_RtcGetTokenOptions_SetRoomId(Handle, value);
        }

        public void SetTtl(int value)
        {
            CLIB.ppf_RtcGetTokenOptions_SetTtl(Handle, value);
        }

        public void SetPrivileges(RtcPrivilege key, int value)
        {
            CLIB.ppf_RtcGetTokenOptions_SetPrivileges(Handle, key, value);
        }

        public void ClearPrivileges()
        {
            CLIB.ppf_RtcGetTokenOptions_ClearPrivileges(Handle);
        }

        /// For passing to native C
        public static explicit operator IntPtr(RtcGetTokenOptions options)
        {
            return options != null ? options.Handle : IntPtr.Zero;
        }

        ~RtcGetTokenOptions()
        {
            CLIB.ppf_RtcGetTokenOptions_Destroy(Handle);
        }

        private IntPtr Handle;
    }

    public class RtcAudioPropertyOptions
    {
        public IntPtr Handle;

        public RtcAudioPropertyOptions()
        {
            Handle = CLIB.ppf_RtcAudioPropertyOptions_Create();
        }

        public void SetInterval(int value)
        {
            CLIB.ppf_RtcAudioPropertyOptions_SetInterval(Handle, value);
        }

        ~RtcAudioPropertyOptions()
        {
            CLIB.ppf_RtcAudioPropertyOptions_Destroy(Handle);
        }

        /// For passing to native C
        public static explicit operator IntPtr(RtcAudioPropertyOptions options)
        {
            return options != null ? options.Handle : IntPtr.Zero;
        }
    }
}