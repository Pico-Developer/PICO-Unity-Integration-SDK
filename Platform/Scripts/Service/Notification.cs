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
     */
    public static class NotificationService
    {
        /// <summary>
        /// Gets a list of all pending room invites for your app. For example, notifications that may have been sent before the user launches your app.
        /// </summary>
        /// <param name="pageIdx">Defines which page of pending room invites to return. The first page index is `0`.</param>
        /// <param name="pageSize">Defines the number of pending room invites returned on each page.</param>
        /// <returns>Request information of type `Task`, including the request id, and its response message will contain data of type `RoomInviteNotificationList`.
        /// 
        /// A message of type `MessageType.Notification_GetRoomInvites` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `RoomInviteNotificationList`.
        /// Extract the payload from the message handle with `message.Data`.</returns>
        public static Task<RoomInviteNotificationList> GetRoomInviteNotifications(int pageIdx, int pageSize)
        {
            if (CoreService.IsInitialized())
            {
                return new Task<RoomInviteNotificationList>(CLIB.ppf_Notification_GetRoomInvites(pageIdx, pageSize));
            }

            Debug.LogError(CoreService.NotInitializedError);
            return null;
        }

        /// <summary>
        /// Marks a notification as read.
        /// </summary>
        /// <param name="notificationID">The ID of the notificaiton to mark.</param>
        /// <returns>Request information of type `Task`, including the request id, and its response message does not contain data.
        /// A message of type `MessageType.Notification_MarkAsRead` will be generated in response. Call `message.IsError()` to check if any error has occurred.
        /// </returns>
        public static Task MarkAsRead(UInt64 notificationID)
        {
            if (CoreService.IsInitialized())
            {
                return new Task(CLIB.ppf_Notification_MarkAsRead(notificationID));
            }

            Debug.LogError(CoreService.NotInitializedError);
            return null;
        }
    }
}