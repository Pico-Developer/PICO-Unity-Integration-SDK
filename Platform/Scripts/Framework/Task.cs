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

using System.Threading.Tasks;
using UnityEngine;

namespace Pico.Platform
{
    public class Task
    {
        public readonly ulong TaskId;
        public bool HasSetCallback = false;

        public Task(ulong taskId)
        {
            this.TaskId = taskId;
        }

        public Task OnComplete(Message.Handler handler)
        {
            if (handler == null)
            {
                throw new UnityException("call Task.Oncomplete with null handler.");
            }

            if (HasSetCallback)
            {
                throw new UnityException("OnComplete() or Async() can call only once time.");
            }

            HasSetCallback = true;
            Looper.RegisterTaskHandler(TaskId, handler);
            return this;
        }

        public System.Threading.Tasks.Task<Message> Async()
        {
            if (HasSetCallback)
            {
                throw new UnityException("OnComplete() or Async() can call only once time.");
            }

            HasSetCallback = true;
            TaskCompletionSource<Message> x = new TaskCompletionSource<Message>();
            Message.Handler fun = msg => { x.SetResult(msg); };
            Looper.RegisterTaskHandler(this.TaskId, fun);
            return x.Task;
        }
    }

    public class Task<T> : Task
    {
        public Task(ulong taskId) : base(taskId)
        {
        }

        public Task<T> OnComplete(Message<T>.Handler handler)
        {
            if (handler == null)
            {
                throw new UnityException("call Task.Oncomplete with null handler.");
            }

            if (HasSetCallback)
            {
                throw new UnityException("OnComplete() or Async() can call only once time.");
            }

            HasSetCallback = true;
            Looper.RegisterTaskHandler(TaskId, handler);
            return this;
        }

        public new System.Threading.Tasks.Task<Message<T>> Async()
        {
            if (HasSetCallback)
            {
                throw new UnityException("OnComplete() or Async() can call only once time.");
            }

            HasSetCallback = true;
            TaskCompletionSource<Message<T>> x = new TaskCompletionSource<Message<T>>();
            Message<T>.Handler fun = msg => { x.SetResult(msg); };
            Looper.RegisterTaskHandler(this.TaskId, fun);
            return x.Task;
        }
    }
}