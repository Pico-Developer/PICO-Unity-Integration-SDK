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
using System.Runtime.InteropServices;
using Pico.Platform.Models;
using UnityEngine;

namespace Pico.Platform
{
    /**
     * \ingroup Platform
     */
    public static class NetworkService
    {
        /// <summary>
        /// Reads the messages from other users in the room.
        /// </summary>
        public static Packet ReadPacket()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }

            var handle = CLIB.ppf_Net_ReadPacket();
            if (handle == IntPtr.Zero)
                return null;
            return new Packet(handle);
        }

        /// <summary>
        /// Sends messages to a specified user. The maximum messaging frequency is 1000/s.
        /// </summary>
        /// <param name="userId">The ID of the user to send messages to.</param>
        /// <param name="bytes">The message length (in bytes). The maximum bytes allowed is 512.</param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool SendPacket(string userId, byte[] bytes)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return false;
            }

            if (string.IsNullOrEmpty(userId))
            {
                Debug.LogError("User ID is null or empty!");
                return false;
            }

            GCHandle hobj = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            IntPtr pobj = hobj.AddrOfPinnedObject();
            var ok = CLIB.ppf_Net_SendPacket(userId, (UIntPtr) bytes.Length, pobj);
            if (hobj.IsAllocated)
                hobj.Free();
            return ok;
        }

        /// <summary>
        /// Sends messages to a specified user. The maximum messaging frequency is 1000/s.
        /// </summary>
        /// <param name="userId">The ID of the user to send messages to.</param>
        /// <param name="bytes">The message length (in bytes). The maximum bytes allowed is 512.</param>
        /// <param name="reliable">When `reliable` is set to `true`, messages between lost and resume will not be lost.
        /// The retention time is determined by the `reserve_period` parameter configured for the matchmaking pool, with a maximum of 1 minute.
        /// When `reliable` is set to `false`, this function works the same as the other `SendPacket` function.</param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool SendPacket(string userId, byte[] bytes, bool reliable)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return false;
            }

            if (string.IsNullOrEmpty(userId))
            {
                Debug.LogError("User ID is null or empty!");
                return false;
            }

            GCHandle hobj = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            IntPtr pobj = hobj.AddrOfPinnedObject();
            var ok = CLIB.ppf_Net_SendPacket2(userId, (UIntPtr) bytes.Length, pobj, reliable);
            if (hobj.IsAllocated)
            {
                hobj.Free();
            }

            return ok;
        }

        /// <summary>
        /// Sends messages to other users in the room. The maximum messaging frequency is 1000/s.
        /// </summary>
        /// <param name="bytes">The message length (in bytes). The maximum bytes allowed is 512.</param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool SendPacketToCurrentRoom(byte[] bytes)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return false;
            }

            GCHandle hobj = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            IntPtr pobj = hobj.AddrOfPinnedObject();
            var ok = CLIB.ppf_Net_SendPacketToCurrentRoom((UIntPtr) bytes.Length, pobj);
            if (hobj.IsAllocated)
            {
                hobj.Free();
            }

            return ok;
        }

        /// <summary>
        /// Sends messages to other users in the room. The maximum messaging frequency is 1000/s.
        /// </summary>
        /// <param name="bytes">The message length (in bytes). The maximum bytes allowed is 512.</param>
        /// <param name="reliable">When `reliable` is set to `true`, messages between lost and resume will not be lost.
        /// The retention time is determined by the `reserve_period` parameter configured for the matchmaking pool, with a maximum of 1 minute.
        /// When `reliable` is set to `false`, this function works the same as the other `SendPacketToCurrentRoom` function.</param>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool SendPacketToCurrentRoom(byte[] bytes, bool reliable)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return false;
            }

            GCHandle hobj = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            IntPtr pobj = hobj.AddrOfPinnedObject();
            var ok = CLIB.ppf_Net_SendPacketToCurrentRoom2((UIntPtr) bytes.Length, pobj, reliable);
            if (hobj.IsAllocated)
                hobj.Free();
            return ok;
        }

        public static void SetPlatformGameInitializeAsynchronousCallback(Message<GameInitializeResult>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.PlatformGameInitializeAsynchronous, handler);
        }

        /// <summary>Sets the callback to get notified when the game network fluctuates.
        /// Listen to this event to receive a relevant message. Use `Message.Data` to get the network situation in the game.</summary>
        ///
        /// <param name="handler">Callback handler. The callback function will be called when receiving the `Notification_Game_ConnectionEvent` message and the value of `requestID` is `0`.</param>
        public static void SetNotification_Game_ConnectionEventCallback(Message<GameConnectionEvent>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Game_ConnectionEvent, handler);
        }

        public static void SetNotification_Game_Request_FailedCallback(Message<GameRequestFailedReason>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Game_RequestFailed, handler);
        }

        /// <summary>Sets the callback to get notified when the game state needs to be reset.
        /// Listen to this event to receive a relevant message. If you receive this message, you will need to reset your gaming state. For example,
        /// * If you are in a room before receiving this message, you will need to check and reset your room state after receving this message.
        /// * If you are in a matchmaking queue before receiving this message, you will need to check and reset your matchmaking state after receiving this message.</summary>
        ///
        /// <param name="handler">Callback handler. The callback function will be called when receiving the "Notification_Game_StateReset" message and the value of `requestID` is `0`.</param>
        public static void SetNotification_Game_StateResetCallback(Message.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Game_StateReset, handler);
        }
    }
}