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

namespace Pico.Platform.Models
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The information about the message packet.
    /// </summary>
    public sealed class Packet : IDisposable
    {
        /// The size of the message packet. 
        private readonly ulong size;
        /// The handler of the message packet. 
        private readonly IntPtr handler;

        public Packet(IntPtr handler)
        {
            this.handler = handler;
            this.size = (ulong) CLIB.ppf_Packet_GetSize(handler);
        }

        /// <summary>Get message content.</summary>
        public ulong GetBytes(byte[] dest)
        {
            if ((ulong) dest.LongLength >= size)
            {
                Marshal.Copy(CLIB.ppf_Packet_GetBytes(handler), dest, 0, (int) size);
                return size;
            }
            else
            {
                throw new ArgumentException($"Dest array can't  hold {size} bytes");
            }
        }

        /// <summary>Get message content.</summary>
        public string GetBytes()
        {
            if (size > 0)
            {
                byte[] bytes = new byte[size];
                Marshal.Copy(CLIB.ppf_Packet_GetBytes(handler), bytes, 0, (int) size);
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>Get the ID of the message sender.</summary>
        public string SenderId
        {
            get { return CLIB.ppf_Packet_GetSenderID(handler); }
        }
        /// <summary>Get message size.</summary>
        public ulong Size
        {
            get { return size; }
        }


        #region IDisposable

        ~Packet()
        {
            Dispose();
        }


        public void Dispose()
        {
            CLIB.ppf_Packet_Free(handler);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}