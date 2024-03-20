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

namespace Pico.Platform
{
    public class MessageQueue
    {
        public static Message ParseMessage(IntPtr msgPointer)
        {
            Message msg = null;
            MessageType messageType = CLIB.ppf_Message_GetType(msgPointer);
            switch (messageType)
            {
                case MessageType.PlatformInitializeAndroidAsynchronous:
                {
                    msg = new Message<PlatformInitializeResult>(msgPointer, ptr => { return (PlatformInitializeResult) CLIB.ppf_Message_GetInt32(ptr); });
                    break;
                }

                case MessageType.CloudStorage_StartNewBackup:
                    {
                        msg = new Message(msgPointer);
                        break;
                    }

                #region speech

                case MessageType.Notification_Speech_OnAsrResult:
                {
                    msg = new Message<AsrResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetAsrResult(ptr);
                        return new AsrResult(obj);
                    });
                    break;
                }
                case MessageType.Notification_Speech_OnSpeechError:
                {
                    msg = new Message<SpeechError>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetSpeechError(ptr);
                        return new SpeechError(obj);
                    });
                    break;
                }

                #endregion

                #region Highlight

                case MessageType.Highlight_StartSession:
                {
                    msg = new Message<string>(msgPointer, ptr => { return CLIB.ppf_Message_GetString(ptr); });
                    break;
                }
                case MessageType.Highlight_CaptureScreen:
                {
                    msg = new Message<CaptureInfo>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetCaptureInfo(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new CaptureInfo(obj);
                    });
                    break;
                }
                case MessageType.Highlight_ListMedia:
                {
                    msg = new Message<SessionMedia>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetSessionMedia(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new SessionMedia(obj);
                    });
                    break;
                }
                case MessageType.Highlight_SaveMedia:
                case MessageType.Highlight_ShareMedia:
                case MessageType.Highlight_StartRecord:

                {
                    msg = new Message(msgPointer);
                    break;
                }
                case MessageType.Highlight_StopRecord:
                case MessageType.Notification_Highlight_OnRecordStop:
                {
                    msg = new Message<RecordInfo>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRecordInfo(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new RecordInfo(obj);
                    });
                    break;
                }

                #endregion

                #region compliance

                case MessageType.Compliance_DetectSensitive:
                {
                    msg = new Message<DetectSensitiveResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetDetectSensitiveResult(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new DetectSensitiveResult(obj);
                    });
                    break;
                }

                #endregion

                #region Sport

                case MessageType.Sport_GetSummary:
                {
                    msg = new Message<SportSummary>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetSportSummary(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new SportSummary(obj);
                    });
                    break;
                }
                case MessageType.Sport_GetDailySummary:
                {
                    msg = new Message<SportDailySummaryList>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetSportDailySummaryArray(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new SportDailySummaryList(obj);
                    });
                    break;
                }
                case MessageType.Sport_GetUserInfo:
                {
                    msg = new Message<SportUserInfo>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetSportUserInfo(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new SportUserInfo(obj);
                    });
                    break;
                }

                #endregion

                #region User

                case MessageType.User_EntitlementCheck:
                {
                    msg = new Message<EntitlementCheckResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetEntitlementCheckResult(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new EntitlementCheckResult(obj);
                    });
                    break;
                }
                case MessageType.User_GetAuthorizedPermissions:
                case MessageType.User_RequestUserPermissions:
                {
                    msg = new Message<PermissionResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetPermissionResult(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new PermissionResult(obj);
                    });
                    break;
                }


                case MessageType.User_GetLoggedInUserFriendsAndRooms:
                {
                    msg = new Message<UserRoomList>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetUserAndRoomArray(ptr);
                        if (obj == IntPtr.Zero) return null;
                        var data = new UserRoomList(obj);
                        return data;
                    });
                    break;
                }
                case MessageType.Presence_GetSentInvites:
                {
                    msg = new Message<ApplicationInviteList>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetApplicationInviteArray(ptr);
                        if (obj == IntPtr.Zero) return null;
                        var data = new ApplicationInviteList(obj);
                        return data;
                    });
                    break;
                }
                case MessageType.Presence_SendInvites:
                {
                    msg = new Message<SendInvitesResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetSendInvitesResult(ptr);
                        if (obj == IntPtr.Zero) return null;
                        var data = new SendInvitesResult(obj);
                        return data;
                    });
                    break;
                }
                case MessageType.Presence_GetDestinations:
                {
                    msg = new Message<DestinationList>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetDestinationArray(ptr);
                        if (obj == IntPtr.Zero) return null;
                        var data = new DestinationList(obj);
                        return data;
                    });
                    break;
                }
                case MessageType.User_GetAccessToken:
                case MessageType.User_GetIdToken:
                case MessageType.Rtc_GetToken:
                case MessageType.Notification_Rtc_OnTokenWillExpire:
                case MessageType.Notification_Rtc_OnUserStartAudioCapture:
                case MessageType.Notification_Rtc_OnUserStopAudioCapture:
                case MessageType.Application_LaunchOtherApp:
                case MessageType.Application_LaunchStore:
                case MessageType.Notification_Room_InviteAccepted:
                case MessageType.Notification_Challenge_LaunchByInvite:
                case MessageType.Notification_ApplicationLifecycle_LaunchIntentChanged:
                {
                    msg = new Message<string>(msgPointer, ptr => { return CLIB.ppf_Message_GetString(ptr); });
                    break;
                }
                case MessageType.Notification_Presence_JoinIntentReceived:
                {
                    msg = new Message<PresenceJoinIntent>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetPresenceJoinIntent(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new PresenceJoinIntent(obj);
                    });
                    break;
                }
                case MessageType.Application_GetVersion:
                {
                    msg = new Message<ApplicationVersion>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetApplicationVersion(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new ApplicationVersion(obj);
                    });
                    break;
                }

                case MessageType.User_GetLoggedInUser:
                case MessageType.User_Get:
                {
                    msg = new Message<User>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetUser(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new User(obj);
                    });
                    break;
                }
                case MessageType.User_GetOrgScopedID:
                {
                    msg = new Message<OrgScopedID>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetOrgScopedID(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new OrgScopedID(obj);
                    });
                    break;
                }
                case MessageType.User_LaunchFriendRequestFlow:
                {
                    msg = new Message<LaunchFriendResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetLaunchFriendRequestFlowResult(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new LaunchFriendResult(obj);
                    });
                    break;
                }
                case MessageType.User_GetLoggedInUserFriends:
                case MessageType.Room_GetInvitableUsers2:
                case MessageType.Presence_GetInvitableUsers:
                {
                    msg = new Message<UserList>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetUserArray(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new UserList(obj);
                    });
                    break;
                }

                case MessageType.User_GetRelations:
                {
                    msg = new Message<UserRelationResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetUserRelationResult(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new UserRelationResult(obj);
                    });
                    break;
                }

                #endregion

                #region RTC

                case MessageType.Notification_Rtc_OnRoomMessageReceived:
                {
                    msg = new Message<RtcRoomMessageReceived>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcRoomMessageReceived(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new RtcRoomMessageReceived(obj);
                    });
                    break;
                }
                case MessageType.Notification_Rtc_OnUserMessageReceived:
                {
                    msg = new Message<RtcUserMessageReceived>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcUserMessageReceived(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new RtcUserMessageReceived(obj);
                    });
                    break;
                }
                case MessageType.Notification_Rtc_OnRoomMessageSendResult:
                case MessageType.Notification_Rtc_OnUserMessageSendResult:
                {
                    msg = new Message<RtcMessageSendResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcMessageSendResult(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new RtcMessageSendResult(obj);
                    });
                    break;
                }
                case MessageType.Notification_Rtc_OnRoomBinaryMessageReceived:
                case MessageType.Notification_Rtc_OnUserBinaryMessageReceived:
                {
                    msg = new Message<RtcBinaryMessageReceived>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcBinaryMessageReceived(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new RtcBinaryMessageReceived(obj);
                    });
                    break;
                }
                case MessageType.Notification_Rtc_OnUserPublishScreen:
                case MessageType.Notification_Rtc_OnUserPublishStream:
                {
                    msg = new Message<RtcUserPublishInfo>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcUserPublishInfo(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new RtcUserPublishInfo(ptr);
                    });
                    break;
                }
                case MessageType.Notification_Rtc_OnUserUnPublishScreen:
                case MessageType.Notification_Rtc_OnUserUnPublishStream:
                {
                    msg = new Message<RtcUserUnPublishInfo>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcUserUnPublishInfo(ptr);
                        if (obj == IntPtr.Zero)
                        {
                            return null;
                        }

                        return new RtcUserUnPublishInfo(obj);
                    });
                    break;
                }

                case MessageType.Notification_Rtc_OnStreamSyncInfoReceived:
                {
                    msg = new Message<RtcStreamSyncInfo>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcStreamSyncInfo(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new RtcStreamSyncInfo(obj);
                    });
                    break;
                }
                case MessageType.Notification_Rtc_OnVideoDeviceStateChanged:
                {
                    break;
                }
                case MessageType.Notification_Rtc_OnRoomError:
                {
                    msg = new Message<RtcRoomError>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcRoomError(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new RtcRoomError(obj);
                    });
                    break;
                }
                case MessageType.Notification_Rtc_OnRoomWarn:
                {
                    msg = new Message<RtcRoomWarn>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcRoomWarn(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new RtcRoomWarn(obj);
                    });
                    break;
                }
                case MessageType.Notification_Rtc_OnConnectionStateChange:
                {
                    msg = new Message<RtcConnectionState>(msgPointer, ptr => { return (RtcConnectionState) CLIB.ppf_Message_GetInt32(ptr); });
                    break;
                }
                case MessageType.Notification_Rtc_OnError:
                case MessageType.Notification_Rtc_OnWarn:
                {
                    msg = new Message<Int32>(msgPointer, ptr => { return CLIB.ppf_Message_GetInt32(ptr); });
                    break;
                }
                case MessageType.Notification_Rtc_OnRoomStats:
                {
                    msg = new Message<RtcRoomStats>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcRoomStats(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new RtcRoomStats(obj);
                    });
                    break;
                }
                case MessageType.Notification_Rtc_OnJoinRoom:
                {
                    msg = new Message<RtcJoinRoomResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcJoinRoomResult(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new RtcJoinRoomResult(obj);
                    });
                    break;
                }
                case MessageType.Notification_Rtc_OnLeaveRoom:
                {
                    msg = new Message<RtcLeaveRoomResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcLeaveRoomResult(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new RtcLeaveRoomResult(obj);
                    });
                    break;
                }

                case MessageType.Notification_Rtc_OnUserLeaveRoom:
                {
                    msg = new Message<RtcUserLeaveInfo>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcUserLeaveInfo(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new RtcUserLeaveInfo(obj);
                    });
                    break;
                }

                case MessageType.Notification_Rtc_OnUserJoinRoom:
                {
                    msg = new Message<RtcUserJoinInfo>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcUserJoinInfo(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new RtcUserJoinInfo(obj);
                    });
                    break;
                }

                case MessageType.Notification_Rtc_OnAudioPlaybackDeviceChanged:
                {
                    msg = new Message<RtcAudioPlaybackDevice>(msgPointer, ptr => { return (RtcAudioPlaybackDevice) CLIB.ppf_Message_GetInt32(ptr); });
                    break;
                }

                case MessageType.Notification_Rtc_OnMediaDeviceStateChanged:
                {
                    msg = new Message<RtcMediaDeviceChangeInfo>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcMediaDeviceChangeInfo(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new RtcMediaDeviceChangeInfo(obj);
                    });
                    break;
                }

                case MessageType.Notification_Rtc_OnLocalAudioPropertiesReport:
                {
                    msg = new Message<RtcLocalAudioPropertiesReport>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcLocalAudioPropertiesReport(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new RtcLocalAudioPropertiesReport(obj);
                    });
                    break;
                }

                case MessageType.Notification_Rtc_OnRemoteAudioPropertiesReport:
                {
                    msg = new Message<RtcRemoteAudioPropertiesReport>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcRemoteAudioPropertiesReport(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new RtcRemoteAudioPropertiesReport(obj);
                    });
                    break;
                }

                case MessageType.Notification_Rtc_OnUserMuteAudio:
                {
                    msg = new Message<RtcMuteInfo>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcMuteInfo(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new RtcMuteInfo(obj);
                    });
                    break;
                }

                #endregion

                #region IAP

                case MessageType.IAP_GetViewerPurchases:
                {
                    msg = new Message<PurchaseList>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetPurchaseArray(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new PurchaseList(obj);
                    });
                    break;
                }
                case MessageType.IAP_GetSubscriptionStatus:
                {
                    msg = new Message<SubscriptionStatus>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetSubscriptionStatus(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new SubscriptionStatus(obj);
                    });
                    break;
                }
                case MessageType.IAP_LaunchCheckoutFlow:
                {
                    msg = new Message<Purchase>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetPurchase(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new Purchase(obj);
                    });
                    break;
                }
                case MessageType.IAP_GetProductsBySKU:
                {
                    msg = new Message<ProductList>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetProductArray(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new ProductList(obj);
                    });
                    break;
                }

                #endregion

                #region DLC

                case MessageType.AssetFile_DeleteById:
                case MessageType.AssetFile_DeleteByName:
                {
                    msg = new Message<AssetFileDeleteResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetAssetFileDeleteResult(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new AssetFileDeleteResult(obj);
                    });
                    break;
                }
                case MessageType.AssetFile_DownloadById:
                case MessageType.AssetFile_DownloadByName:
                {
                    msg = new Message<AssetFileDownloadResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetAssetFileDownloadResult(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new AssetFileDownloadResult(obj);
                    });
                    break;
                }
                case MessageType.AssetFile_DownloadCancelById:
                case MessageType.AssetFile_DownloadCancelByName:
                {
                    msg = new Message<AssetFileDownloadCancelResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetAssetFileDownloadCancelResult(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new AssetFileDownloadCancelResult(obj);
                    });
                    break;
                }
                case MessageType.AssetFile_GetList:
                case MessageType.AssetFile_GetNextAssetDetailsArrayPage:
                {
                    msg = new Message<AssetDetailsList>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetAssetDetailsArray(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new AssetDetailsList(obj);
                    });
                    break;
                }
                case MessageType.AssetFile_StatusById:
                case MessageType.AssetFile_StatusByName:
                {
                    msg = new Message<AssetStatus>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetAssetStatus(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new AssetStatus(obj);
                    });
                    break;
                }
                case MessageType.Notification_AssetFile_DownloadUpdate:
                {
                    msg = new Message<AssetFileDownloadUpdate>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetAssetFileDownloadUpdate(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new AssetFileDownloadUpdate(obj);
                    });
                    break;
                }
                case MessageType.Notification_AssetFile_DeleteForSafety:
                {
                    msg = new Message<AssetFileDeleteForSafety>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetAssetFileDeleteForSafety(ptr);
                        if (obj == IntPtr.Zero) return null;
                        return new AssetFileDeleteForSafety(obj);
                    });
                    break;
                }

                #endregion

                #region stark game

                case MessageType.Matchmaking_Cancel2:
                case MessageType.Matchmaking_ReportResultInsecure:
                case MessageType.Matchmaking_StartMatch:
                case MessageType.Room_LaunchInvitableUserFlow:
                case MessageType.Challenges_LaunchInvitableUserFlow:
                case MessageType.Room_UpdateOwner:
                case MessageType.Notification_MarkAsRead:
                case MessageType.Notification_Game_StateReset:
                case MessageType.Presence_Clear:
                case MessageType.Presence_Set:
                case MessageType.IAP_ConsumePurchase:
                case MessageType.Presence_LaunchInvitePanel:
                case MessageType.Presence_ShareMedia:
                {
                    msg = new Message(msgPointer);
                    break;
                }
                case MessageType.Matchmaking_GetAdminSnapshot:
                {
                    msg = new Message<MatchmakingAdminSnapshot>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetMatchmakingAdminSnapshot(ptr);
                        return new MatchmakingAdminSnapshot(obj);
                    });
                    break;
                }
                case MessageType.Matchmaking_Browse2:
                {
                    msg = new Message<MatchmakingBrowseResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetMatchmakingBrowseResult(ptr);
                        return new MatchmakingBrowseResult(obj);
                    });
                    break;
                }
                case MessageType.Matchmaking_Browse2CustomPage:
                {
                    msg = new Message<MatchmakingBrowseResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetMatchmakingBrowseCustomPageResult(ptr);
                        return new MatchmakingBrowseResult(obj);
                    });
                    break;
                }
                case MessageType.Matchmaking_Enqueue2:
                case MessageType.Matchmaking_EnqueueRoom2:
                {
                    msg = new Message<MatchmakingEnqueueResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetMatchmakingEnqueueResult(ptr);
                        return new MatchmakingEnqueueResult(obj);
                    });
                    break;
                }
                case MessageType.Matchmaking_CreateAndEnqueueRoom2:
                {
                    msg = new Message<MatchmakingEnqueueResultAndRoom>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetMatchmakingEnqueueResultAndRoom(ptr);
                        return new MatchmakingEnqueueResultAndRoom(obj);
                    });
                    break;
                }

                case MessageType.Matchmaking_GetStats:
                {
                    msg = new Message<MatchmakingStats>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetMatchmakingStats(ptr);
                        return new MatchmakingStats(obj);
                    });
                    break;
                }
                case MessageType.Room_GetCurrent:
                case MessageType.Room_GetCurrentForUser:
                case MessageType.Notification_Room_RoomUpdate:
                case MessageType.Room_CreateAndJoinPrivate:
                case MessageType.Room_CreateAndJoinPrivate2:
                case MessageType.Room_InviteUser:
                case MessageType.Room_Join:
                case MessageType.Room_Join2:
                case MessageType.Room_JoinNamed:
                case MessageType.Room_KickUser:
                case MessageType.Room_Leave:
                case MessageType.Room_SetDescription:
                case MessageType.Room_UpdateDataStore:
                case MessageType.Room_UpdateMembershipLockStatus:
                case MessageType.Room_UpdatePrivateRoomJoinPolicy:
                case MessageType.Notification_Matchmaking_MatchFound:
                case MessageType.Room_Get:
                {
                    msg = new Message<Room>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRoom(ptr);
                        return new Room(obj);
                    });
                    break;
                }
                case MessageType.Room_GetModeratedRooms:
                case MessageType.Room_GetNamedRooms:
                case MessageType.Room_GetNextRoomArrayPage:
                {
                    msg = new Message<RoomList>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRoomArray(ptr);
                        return new RoomList(obj);
                    });
                    break;
                }
                case MessageType.PlatformGameInitializeAsynchronous:
                {
                    msg = new Message<GameInitializeResult>(msgPointer, ptr =>
                    {
                        var objHandle = CLIB.ppf_Message_GetPlatformGameInitialize(ptr);
                        var obj = CLIB.ppf_PlatformGameInitialize_GetResult(objHandle);
                        return obj;
                    });
                    break;
                }
                case MessageType.Notification_Game_ConnectionEvent:
                {
                    msg = new Message<GameConnectionEvent>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetGameConnectionEvent(ptr);
                        return obj;
                    });
                    break;
                }
                case MessageType.Notification_Game_RequestFailed:
                {
                    msg = new Message<GameRequestFailedReason>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetGameRequestFailedReason(ptr);
                        return obj;
                    });
                    break;
                }
                case MessageType.Leaderboard_Get:
                case MessageType.Leaderboard_GetNextLeaderboardArrayPage:
                {
                    msg = new Message<LeaderboardList>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetLeaderboardArray(ptr);
                        return new LeaderboardList(obj);
                    });
                    break;
                }

                case MessageType.Leaderboard_GetEntries:
                case MessageType.Leaderboard_GetEntriesAfterRank:
                case MessageType.Leaderboard_GetEntriesByIds:
                case MessageType.Leaderboard_GetNextEntries:
                case MessageType.Leaderboard_GetPreviousEntries:
                {
                    msg = new Message<LeaderboardEntryList>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetLeaderboardEntryArray(ptr);
                        return new LeaderboardEntryList(obj);
                    });
                    break;
                }
                case MessageType.Leaderboard_WriteEntry:
                case MessageType.Leaderboard_WriteEntryWithSupplementaryMetric:
                {
                    msg = new Message<bool>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetLeaderboardUpdateStatus(ptr);
                        return CLIB.ppf_LeaderboardUpdateStatus_GetDidUpdate(obj);
                    });
                    break;
                }

                case MessageType.Achievements_GetAllDefinitions:
                case MessageType.Achievements_GetDefinitionsByName:
                case MessageType.Achievements_GetNextAchievementDefinitionArrayPage:
                    msg = new Message<AchievementDefinitionList>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetAchievementDefinitionArray(ptr);
                        return new AchievementDefinitionList(obj);
                    });
                    break;

                case MessageType.Achievements_GetAllProgress:
                case MessageType.Achievements_GetNextAchievementProgressArrayPage:
                case MessageType.Achievements_GetProgressByName:
                    msg = new Message<AchievementProgressList>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetAchievementProgressArray(ptr);
                        return new AchievementProgressList(obj);
                    });
                    break;

                case MessageType.Achievements_AddCount:
                case MessageType.Achievements_AddFields:
                case MessageType.Achievements_Unlock:
                    msg = new Message<AchievementUpdate>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetAchievementUpdate(ptr);
                        return new AchievementUpdate(obj);
                    });
                    break;
                case MessageType.Notification_GetNextRoomInviteNotificationArrayPage:
                case MessageType.Notification_GetRoomInvites:
                {
                    msg = new Message<RoomInviteNotificationList>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRoomInviteNotificationArray(ptr);
                        return new RoomInviteNotificationList(obj);
                    });
                    break;
                }
                case MessageType.Challenges_Invite:
                case MessageType.Challenges_Get:
                case MessageType.Challenges_Join:
                case MessageType.Challenges_Leave:
                {
                    msg = new Message<Challenge>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetChallenge(ptr);
                        return new Challenge(obj);
                    });
                    break;
                }
                case MessageType.Challenges_GetList:
                {
                    msg = new Message<ChallengeList>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetChallengeArray(ptr);
                        return new ChallengeList(obj);
                    });
                    break;
                }
                case MessageType.Challenges_GetEntries:
                case MessageType.Challenges_GetEntriesAfterRank:
                case MessageType.Challenges_GetEntriesByIds:
                {
                    msg = new Message<ChallengeEntryList>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetChallengeEntryArray(ptr);
                        return new ChallengeEntryList(obj);
                    });
                    break;
                }

                #endregion stark game

                default:
                    break;
            }

            return msg;
        }
    }
}