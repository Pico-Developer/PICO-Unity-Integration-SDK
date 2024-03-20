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
using System.Collections.Concurrent;
using UnityEngine;

namespace Pico.Platform
{
    public class Looper
    {
        private static readonly ConcurrentDictionary<ulong, Delegate> TaskMap = new ConcurrentDictionary<ulong, Delegate>();
        private static readonly ConcurrentDictionary<MessageType, Delegate> NotifyMap = new ConcurrentDictionary<MessageType, Delegate>();
        public static readonly ConcurrentDictionary<MessageType, MessageParser> MessageParserMap = new ConcurrentDictionary<MessageType, MessageParser>();

        public static void ProcessMessages(uint limit = 0)
        {
            if (limit == 0)
            {
                while (true)
                {
                    var msg = PopMessage();
                    if (msg == null)
                    {
                        break;
                    }

                    dispatchMessage(msg);
                }
            }
            else
            {
                for (var i = 0; i < limit; ++i)
                {
                    var msg = PopMessage();
                    if (msg == null)
                    {
                        break;
                    }

                    dispatchMessage(msg);
                }
            }
        }

        public static Message PopMessage()
        {
            if (!CoreService.Initialized)
            {
                return null;
            }

            var handle = CLIB.ppf_PopMessage();
            if (handle == IntPtr.Zero)
            {
                return null;
            }

            MessageType messageType = CLIB.ppf_Message_GetType(handle);
            Message msg = MessageQueue.ParseMessage(handle);
            if (msg == null)
            {
                if (MessageParserMap.TryGetValue(messageType, out MessageParser parser))
                {
                    msg = parser(handle);
                }
            }

            if (msg == null)
            {
                Debug.LogError($"Cannot parse message type {messageType}");
            }

            CLIB.ppf_FreeMessage(handle);
            return msg;
        }

        private static void dispatchMessage(Message msg)
        {
            if (msg.RequestID != 0)
            {
                // handle task
                if (TaskMap.TryGetValue(msg.RequestID, out var handler))
                {
                    try
                    {
                        handler.DynamicInvoke(msg);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"dispatchMessage failed {e}");
                    }
                    finally
                    {
                        TaskMap.TryRemove(msg.RequestID, out handler);
                    }
                }
                else
                {
                    Debug.LogError($"No handler for task: requestId={msg.RequestID}, msg.Type = {msg.Type}. You should call `OnComplete()` when use request API.");
                }
            }
            else
            {
                // handle notification
                if (NotifyMap.TryGetValue(msg.Type, out var handler))
                {
                    handler.DynamicInvoke(msg);
                }
                else
                {
                    //Debug.LogError($"No handler for notification: msg.Type = {msg.Type}");
                }
            }
        }

        public static void RegisterTaskHandler(ulong taskId, Delegate handler)
        {
            if (taskId == 0)
            {
                Debug.LogError("The task is invalid.");
                return;
            }

            TaskMap[taskId] = handler;
        }

        public static void RegisterNotifyHandler(MessageType type, Delegate handler)
        {
            if (handler == null)
            {
                Debug.LogError("Cannot register null notification handler.");
                return;
            }

            NotifyMap[type] = handler;
        }

        public static void RegisterMessageParser(MessageType messageType, MessageParser messageParser)
        {
            if (messageParser == null)
            {
                Debug.LogError($"invalid message parser for {messageType}");
                return;
            }

            if (MessageParserMap.ContainsKey(messageType))
            {
                Debug.LogWarning($"Duplicate register of {messageType}");
            }

            MessageParserMap.TryAdd(messageType, messageParser);
        }

        public static void Clear()
        {
            TaskMap.Clear();
            NotifyMap.Clear();
        }
    }
}