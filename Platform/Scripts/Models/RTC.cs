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

namespace Pico.Platform.Models
{
    /// <summary>
    /// The binary message received in a RTC room.
    /// </summary>
    public class RtcBinaryMessageReceived
    {
        /// The message sender's user ID. 
        public readonly string UserId;

        /// The binary data of the message. 
        public readonly byte[] Data;

        /// The ID of the room that the message is sent to. 
        public readonly string RoomId;

        public RtcBinaryMessageReceived(IntPtr o)
        {
            UserId = CLIB.ppf_RtcBinaryMessageReceived_GetUserId(o);
            var ptr = CLIB.ppf_RtcBinaryMessageReceived_GetData(o);
            var sz = CLIB.ppf_RtcBinaryMessageReceived_GetLength(o);
            Data = MarshalUtil.ByteArrayFromNative(ptr, (uint) sz);
            RoomId = CLIB.ppf_RtcBinaryMessageReceived_GetRoomId(o);
        }
    }


    /// <summary>
    /// The audio frame is several frames of RTC recorded audio.
    /// </summary>
    public class RtcAudioFrame
    {
        /// The type of the audio channel for this audio frame.
        public readonly RtcAudioChannel Channel;

        /// The data pointer of the audio frame.
        public readonly IntPtr Data;

        /// The size of the data.
        public readonly long DataSize;

        /// The sample rate of the data.
        public readonly RtcAudioSampleRate SampleRate;

        /// The timestamp.Its value is always 0. So don't use it.
        public readonly long TimeStampInUs;

        public RtcAudioFrame(IntPtr o)
        {
            Channel = CLIB.ppf_RtcAudioFrame_GetChannel(o);
            DataSize = CLIB.ppf_RtcAudioFrame_GetDataSize(o);
            SampleRate = CLIB.ppf_RtcAudioFrame_GetSampleRate(o);
            TimeStampInUs = CLIB.ppf_RtcAudioFrame_GetTimeStampInUs(o);
            Data = CLIB.ppf_RtcAudioFrame_GetData(o);
        }

        public byte[] GetData()
        {
            return MarshalUtil.ByteArrayFromNative(this.Data, (uint) this.DataSize);
        }

        public void SetData(byte[] data)
        {
            Marshal.Copy(data, 0, this.Data, (int) this.DataSize);
        }
    }


    /// <summary>
    /// The message sending result that indicates whether the message is successfully sent.
    /// </summary>
    public class RtcMessageSendResult
    {
        /// The message ID. 
        public readonly long MessageId;

        /// The error code returned in the result. `200` means success.
        public readonly int Error;

        /// The ID of the room that the message is sent to. 
        public readonly string RoomId;

        public RtcMessageSendResult(IntPtr o)
        {
            MessageId = CLIB.ppf_RtcMessageSendResult_GetMessageId(o);
            Error = CLIB.ppf_RtcMessageSendResult_GetError(o);
            RoomId = CLIB.ppf_RtcMessageSendResult_GetRoomId(o);
        }
    }


    /// <summary>
    /// When the remote user canceled publshing stream to the room, you will receive a notification.
    /// </summary>
    public class RtcUserUnPublishInfo
    {
        /// The ID of the remote user.
        public readonly string UserId;

        /// The stream type.
        public readonly RtcMediaStreamType MediaStreamType;

        /// The reason why the remote user canceled publishing stream.
        public readonly RtcStreamRemoveReason Reason;

        /// The ID of the room that the remote user is in.
        public readonly string RoomId;

        public RtcUserUnPublishInfo(IntPtr o)
        {
            UserId = CLIB.ppf_RtcUserUnPublishInfo_GetUserId(o);
            MediaStreamType = CLIB.ppf_RtcUserUnPublishInfo_GetMediaStreamType(o);
            Reason = CLIB.ppf_RtcUserUnPublishInfo_GetReason(o);
            RoomId = CLIB.ppf_RtcUserUnPublishInfo_GetRoomId(o);
        }
    }


    /// <summary>
    /// The publish stream info.
    /// If the remote user publishes stream, you will receive a notification.
    /// </summary>
    public class RtcUserPublishInfo
    {
        /// The ID of the remote user.
        public readonly string UserId;

        /// The stream type.
        public readonly RtcMediaStreamType MediaStreamType;

        /// The ID of the room that the remote user is in.
        public readonly string RoomId;

        public RtcUserPublishInfo(IntPtr o)
        {
            UserId = CLIB.ppf_RtcUserPublishInfo_GetUserId(o);
            MediaStreamType = CLIB.ppf_RtcUserPublishInfo_GetMediaStreamType(o);
            RoomId = CLIB.ppf_RtcUserPublishInfo_GetRoomId(o);
        }
    }


    /// <summary>
    /// The message received by a certain room.
    /// The remote users can send messages to the room and you will receive this message.
    /// </summary>
    public class RtcRoomMessageReceived
    {
        /// The ID of the message sender.
        public readonly string UserId;

        /// The message.
        public readonly string Message;

        /// The ID of the room that the message was sent to. 
        public readonly string RoomId;

        public RtcRoomMessageReceived(IntPtr o)
        {
            UserId = CLIB.ppf_RtcRoomMessageReceived_GetUserId(o);
            Message = CLIB.ppf_RtcRoomMessageReceived_GetMessage(o);
            RoomId = CLIB.ppf_RtcRoomMessageReceived_GetRoomId(o);
        }
    }


    /// <summary>
    /// The message sent to you by a certain user. You will receive a notification.
    /// </summary>
    public class RtcUserMessageReceived
    {
        /// The ID of the message sender.
        public readonly string UserId;

        /// The message.
        public readonly string Message;

        /// The ID of the room that the message sender and recipient are in. 
        public readonly string RoomId;

        public RtcUserMessageReceived(IntPtr o)
        {
            UserId = CLIB.ppf_RtcUserMessageReceived_GetUserId(o);
            Message = CLIB.ppf_RtcUserMessageReceived_GetMessage(o);
            RoomId = CLIB.ppf_RtcUserMessageReceived_GetRoomId(o);
        }
    }


    /// <summary>
    /// The stream sync info sent to your room. You will receive a notification,
    /// </summary>
    public class RtcStreamSyncInfo
    {
        /// The key of the stream. 
        public readonly RtcRemoteStreamKey StreamKey;

        /// The type of the stream. 
        public readonly RtcSyncInfoStreamType StreamType;

        /// The stream sync info 
        public readonly byte[] Data;

        public RtcStreamSyncInfo(IntPtr o)
        {
            StreamKey = new RtcRemoteStreamKey(CLIB.ppf_RtcStreamSyncInfo_GetStreamKey(o));
            StreamType = CLIB.ppf_RtcStreamSyncInfo_GetStreamType(o);
            var ptr = CLIB.ppf_RtcStreamSyncInfo_GetData(o);
            var sz = CLIB.ppf_RtcStreamSyncInfo_GetLength(o);
            Data = MarshalUtil.ByteArrayFromNative(ptr, (uint) sz);
        }
    }


    /// <summary>
    /// If you enable audio properties report, you will periodically receive audio property info.
    /// </summary>
    public class RtcAudioPropertyInfo
    {
        /// The volume detected. It's a value between `0` and `255`.
        public readonly int Volume;

        public RtcAudioPropertyInfo(IntPtr o)
        {
            Volume = CLIB.ppf_RtcAudioPropertyInfo_GetVolume(o);
        }
    }


    /// <summary>
    /// You will receive this message after you call \ref RtcService.JoinRoom.
    /// </summary>
    public class RtcJoinRoomResult
    {
        /// The ID of the room that the user joined.
        public readonly string RoomId;

        /// The ID of the user.
        public readonly string UserId;

        /// The error code. `0` indicates success.
        public readonly int ErrorCode;

        /// The time from calling \ref RtcService.JoinRoom to receiving the result. 
        public readonly int Elapsed;

        /// Whether it is the first time that the user has joined the room or if the user is reconnected to the room.
        public readonly RtcJoinRoomType JoinType;

        public RtcJoinRoomResult(IntPtr o)
        {
            RoomId = CLIB.ppf_RtcJoinRoomResult_GetRoomId(o);
            UserId = CLIB.ppf_RtcJoinRoomResult_GetUserId(o);
            ErrorCode = CLIB.ppf_RtcJoinRoomResult_GetErrorCode(o);
            Elapsed = CLIB.ppf_RtcJoinRoomResult_GetElapsed(o);
            JoinType = CLIB.ppf_RtcJoinRoomResult_GetJoinType(o);
        }
    }


    /// <summary>
    /// You will receive this message after you call \ref RtcService.LeaveRoom.
    /// </summary>
    public class RtcLeaveRoomResult
    {
        /// The ID of the room that the user left. 
        public readonly string RoomId;

        public RtcLeaveRoomResult(IntPtr o)
        {
            RoomId = CLIB.ppf_RtcLeaveRoomResult_GetRoomId(o);
        }
    }


    /// <summary>
    /// The local audio properties info.
    /// You will periodically receive this message after you
    /// call \ref RtcService.EnableAudioPropertiesReport.
    /// </summary>
    public class RtcLocalAudioPropertiesInfo
    {
        /// The stream index info.
        public readonly RtcStreamIndex StreamIndex;

        /// The audio property details.
        public readonly RtcAudioPropertyInfo AudioPropertyInfo;

        public RtcLocalAudioPropertiesInfo(IntPtr o)
        {
            StreamIndex = CLIB.ppf_RtcLocalAudioPropertiesInfo_GetStreamIndex(o);
            AudioPropertyInfo = new RtcAudioPropertyInfo(CLIB.ppf_RtcLocalAudioPropertiesInfo_GetAudioPropertyInfo(o));
        }
    }


    /// <summary>
    /// The local audio properties report.
    /// You will periodically receive this message after you
    /// call \ref RtcService.EnableAudioPropertiesReport.
    /// </summary>
    public class RtcLocalAudioPropertiesReport
    {
        public readonly RtcLocalAudioPropertiesInfo[] AudioPropertiesInfos;

        public RtcLocalAudioPropertiesReport(IntPtr o)
        {
            ulong total = (ulong) CLIB.ppf_RtcLocalAudioPropertiesReport_GetAudioPropertiesInfosSize(o);
            AudioPropertiesInfos = new RtcLocalAudioPropertiesInfo[total];
            for (uint i = 0; i < total; i++)
            {
                AudioPropertiesInfos[i] = new RtcLocalAudioPropertiesInfo(CLIB.ppf_RtcLocalAudioPropertiesReport_GetAudioPropertiesInfos(o, (UIntPtr) i));
            }
        }
    }


    /// <summary>
    /// The media device change info.
    /// RTC engine will send this message if media device change is detected.
    /// </summary>
    public class RtcMediaDeviceChangeInfo
    {
        /// <summary>
        /// Device ID.
        /// </summary>
        public readonly string DeviceId;
        /// <summary>
        /// Device type.
        /// </summary>
        public readonly RtcMediaDeviceType DeviceType;
        /// <summary>
        /// Device state.
        /// </summary>
        public readonly RtcMediaDeviceState DeviceState;
        /// <summary>
        /// Device error.
        /// </summary>
        public readonly RtcMediaDeviceError DeviceError;

        public RtcMediaDeviceChangeInfo(IntPtr o)
        {
            DeviceId = CLIB.ppf_RtcMediaDeviceChangeInfo_GetDeviceId(o);
            DeviceType = CLIB.ppf_RtcMediaDeviceChangeInfo_GetDeviceType(o);
            DeviceState = CLIB.ppf_RtcMediaDeviceChangeInfo_GetDeviceState(o);
            DeviceError = CLIB.ppf_RtcMediaDeviceChangeInfo_GetDeviceError(o);
        }
    }


    /// <summary>
    /// You will receive this notification if the remote user call \ref RtcService.MuteLocalAudio.
    /// </summary>
    public class RtcMuteInfo
    {
        /// The ID of the remote user who muted audio. 
        public readonly string UserId;

        /// The state of audio muting: muted or canceled. 
        public readonly RtcMuteState MuteState;

        public RtcMuteInfo(IntPtr o)
        {
            UserId = CLIB.ppf_RtcMuteInfo_GetUserId(o);
            MuteState = CLIB.ppf_RtcMuteInfo_GetMuteState(o);
        }
    }


    /// <summary>
    /// The remote audio properties info.
    /// You can check who is speaking by this method.
    /// </summary>
    public class RtcRemoteAudioPropertiesInfo
    {
        public readonly RtcRemoteStreamKey StreamKey;
        public readonly RtcAudioPropertyInfo AudioPropertiesInfo;

        public RtcRemoteAudioPropertiesInfo(IntPtr o)
        {
            StreamKey = new RtcRemoteStreamKey(CLIB.ppf_RtcRemoteAudioPropertiesInfo_GetStreamKey(o));
            AudioPropertiesInfo = new RtcAudioPropertyInfo(CLIB.ppf_RtcRemoteAudioPropertiesInfo_GetAudioPropertiesInfo(o));
        }
    }


    /// <summary>
    /// You will receive remote user's audio info if you call \ref RtcService.EnableAudioPropertiesReport.
    /// </summary>
    public class RtcRemoteAudioPropertiesReport
    {
        public readonly RtcRemoteAudioPropertiesInfo[] AudioPropertiesInfos;

        /// The total volume of remote users in the room. 
        public readonly int TotalRemoteVolume;

        public RtcRemoteAudioPropertiesReport(IntPtr o)
        {
            AudioPropertiesInfos = new RtcRemoteAudioPropertiesInfo[(int) CLIB.ppf_RtcRemoteAudioPropertiesReport_GetAudioPropertiesInfosSize(o)];
            for (uint i = 0; i < AudioPropertiesInfos.Length; i++)
            {
                AudioPropertiesInfos[i] = new RtcRemoteAudioPropertiesInfo(CLIB.ppf_RtcRemoteAudioPropertiesReport_GetAudioPropertiesInfos(o, (UIntPtr) i));
            }

            TotalRemoteVolume = CLIB.ppf_RtcRemoteAudioPropertiesReport_GetTotalRemoteVolume(o);
        }
    }


    /// <summary>
    /// RtcRemoteStreamKey indicates the stream index of a remote user.
    /// </summary>
    public class RtcRemoteStreamKey
    {
        /// The ID of the room that the remote user is in. 
        public readonly string RoomId;

        /// The ID of the remote user. 
        public readonly string UserId;

        /// Indicates whether the stream is main stream or screen stream. 
        public readonly RtcStreamIndex RtcStreamIndex;

        public RtcRemoteStreamKey(IntPtr o)
        {
            RoomId = CLIB.ppf_RtcRemoteStreamKey_GetRoomId(o);
            UserId = CLIB.ppf_RtcRemoteStreamKey_GetUserId(o);
            RtcStreamIndex = CLIB.ppf_RtcRemoteStreamKey_GetStreamIndex(o);
        }
    }


    /// <summary>
    /// You will receive an error code when an error occurred in the room.
    /// </summary>
    public class RtcRoomError
    {
        /// The error code. 
        public readonly int Code;

        /// The ID of the room where the error occurred. 
        public readonly string RoomId;

        public RtcRoomError(IntPtr o)
        {
            Code = CLIB.ppf_RtcRoomError_GetCode(o);
            RoomId = CLIB.ppf_RtcRoomError_GetRoomId(o);
        }
    }


    /// <summary>
    /// You will periodically receive this message after you successfully join a room.
    /// </summary>
    public class RtcRoomStats
    {
        /// The time elapsed since you joined the room .
        public readonly int TotalDuration;

        /// The number of users in the room. 
        public readonly int UserCount;

        /// The ID of the room you joined. 
        public readonly string RoomId;

        public RtcRoomStats(IntPtr o)
        {
            TotalDuration = CLIB.ppf_RtcRoomStats_GetTotalDuration(o);
            UserCount = CLIB.ppf_RtcRoomStats_GetUserCount(o);
            RoomId = CLIB.ppf_RtcRoomStats_GetRoomId(o);
        }
    }


    /// <summary>
    /// The warning info of the room.
    /// </summary>
    public class RtcRoomWarn
    {
        /// The error code. 
        public readonly int Code;

        /// The ID of the room that the warning info comes from. 
        public readonly string RoomId;

        public RtcRoomWarn(IntPtr o)
        {
            Code = CLIB.ppf_RtcRoomWarn_GetCode(o);
            RoomId = CLIB.ppf_RtcRoomWarn_GetRoomId(o);
        }
    }


    /// <summary>
    /// You will receive this message after a remote user joins the room.
    /// </summary>
    public class RtcUserJoinInfo
    {
        /// The ID of the user. 
        public readonly string UserId;

        /// If the remote user set the `UserExtra` field when calling \ref RtcService.JoinRoom with extra info.
        public readonly string UserExtra;

        /// The time used for the remote user to join the room.
        public readonly int Elapsed;

        /// The ID of the room that the remote user joined. 
        public readonly string RoomId;

        public RtcUserJoinInfo(IntPtr o)
        {
            UserId = CLIB.ppf_RtcUserJoinInfo_GetUserId(o);
            UserExtra = CLIB.ppf_RtcUserJoinInfo_GetUserExtra(o);
            Elapsed = CLIB.ppf_RtcUserJoinInfo_GetElapsed(o);
            RoomId = CLIB.ppf_RtcUserJoinInfo_GetRoomId(o);
        }
    }


    /// <summary>
    /// You will receive this message when the remote user leaves the room.
    /// </summary>
    public class RtcUserLeaveInfo
    {
        /// The ID of the user. 
        public readonly string UserId;

        /// The reason why the user left the room, which can be network error or proactive quit. 
        public readonly RtcUserLeaveReasonType OfflineReason;

        /// The ID of the room that the user left. 
        public readonly string RoomId;

        public RtcUserLeaveInfo(IntPtr o)
        {
            UserId = CLIB.ppf_RtcUserLeaveInfo_GetUserId(o);
            OfflineReason = CLIB.ppf_RtcUserLeaveInfo_GetOfflineReason(o);
            RoomId = CLIB.ppf_RtcUserLeaveInfo_GetRoomId(o);
        }
    }
}