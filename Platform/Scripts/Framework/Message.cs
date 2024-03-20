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

namespace Pico.Platform.Models
{
    public class Error
    {
        public readonly int Code;
        public readonly string Message;

        public Error(int code, string msg)
        {
            this.Code = code;
            this.Message = msg;
        }

        public Error(IntPtr handle)
        {
            this.Code = CLIB.ppf_Error_GetCode(handle);
            this.Message = CLIB.ppf_Error_GetMessage(handle);
        }

        public override string ToString()
        {
            return $"Error(Code={this.Code},Message={this.Message})";
        }
    }

    public class MessageArray<T> : List<T>
    {
        /**@brief The next page parameter. It's empty when it doesn't has next page.*/
        public string NextPageParam;

        /**@brief The previous page parameter. It's empty when it doesn't has previous page.*/
        public string PreviousPageParam;

        public bool HasNextPage => !String.IsNullOrEmpty(NextPageParam);

        public bool HasPreviousPage => !String.IsNullOrEmpty(PreviousPageParam);
    }
}

namespace Pico.Platform
{
    public class Message
    {
        public delegate void Handler(Message message);

        public readonly MessageType Type;
        public readonly ulong RequestID;
        public readonly Error Error;

        public Message(IntPtr msgPointer)
        {
            Type = CLIB.ppf_Message_GetType(msgPointer);
            RequestID = CLIB.ppf_Message_GetRequestID(msgPointer);
            if (CLIB.ppf_Message_IsError(msgPointer))
            {
                Error = new Error(CLIB.ppf_Message_GetError(msgPointer));
            }
        }

        public bool IsError => Error != null && Error.Code != 0;

        [Obsolete("Use Error instead")]
        public Error GetError()
        {
            return Error;
        }
    }

    public class Message<T> : Message
    {
        public new delegate void Handler(Message<T> message);

        public readonly T Data;

        public delegate T GetDataFromMessage(IntPtr msgPointer);

        public Message(IntPtr msgPointer, GetDataFromMessage getData) : base(msgPointer)
        {
            if (!IsError)
            {
                Data = getData(msgPointer);
            }
        }
    }


    public delegate Message MessageParser(IntPtr ptr);

    public static class CommonParsers
    {
        public static Message StringParser(IntPtr msgPointer)
        {
            return new Message<string>(msgPointer, ptr => { return CLIB.ppf_Message_GetString(ptr); });
        }

        public static Message VoidParser(IntPtr msgPointer)
        {
            return new Message(msgPointer);
        }
    }
}