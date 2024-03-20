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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Pico.Platform
{
    public class UTF8Marshaller : ICustomMarshaler
    {
        public void CleanUpManagedData(object ManagedObj)
        {
        }

        public void CleanUpNativeData(IntPtr pNativeData)
            => Marshal.FreeHGlobal(pNativeData);

        public int GetNativeDataSize() => -1;

        public IntPtr MarshalManagedToNative(object managedObj)
        {
            if (managedObj == null)
                return IntPtr.Zero;
            if (!(managedObj is string))
                throw new MarshalDirectiveException("UTF8Marshaler must be used on a string.");

            return MarshalUtil.StringToPtr((string) managedObj);
        }

        public object MarshalNativeToManaged(IntPtr str)
        {
            if (str == IntPtr.Zero)
                return null;
            return MarshalUtil.PtrToString(str);
        }

        public static ICustomMarshaler GetInstance(string pstrCookie)
        {
            if (marshaler == null)
                marshaler = new UTF8Marshaller();
            return marshaler;
        }

        private static UTF8Marshaller marshaler;
    }

    public class PtrManager
    {
        public IntPtr ptr;
        private bool freed = false;

        public PtrManager(byte[] a)
        {
            this.ptr = MarshalUtil.ByteArrayToNative(a);
        }

        public void Free()
        {
            if (freed) return;
            freed = true;
            Marshal.FreeHGlobal(ptr);
        }

        ~PtrManager()
        {
            this.Free();
        }
    }

    class PtrArray
    {
        public IntPtr[] a;
        private bool freed = false;

        public PtrArray(string[] a)
        {
            if (a == null)
            {
                a = Array.Empty<string>();
            }

            this.a = a.Select(x => MarshalUtil.StringToPtr(x)).ToArray();
        }

        public void Free()
        {
            if (freed) return;
            freed = true;
            foreach (var i in a)
            {
                Marshal.FreeHGlobal(i);
            }
        }

        ~PtrArray()
        {
            this.Free();
        }
    }

    public static class MarshalUtil
    {
        public static IntPtr StringToPtr(string s)
        {
            if (s == null) return IntPtr.Zero;
            // not null terminated
            byte[] strbuf = Encoding.UTF8.GetBytes(s);
            IntPtr buffer = Marshal.AllocHGlobal(strbuf.Length + 1);
            Marshal.Copy(strbuf, 0, buffer, strbuf.Length);

            // write the terminating null
            Marshal.WriteByte(buffer + strbuf.Length, 0);
            return buffer;
        }

        public static string PtrToString(IntPtr p)
        {
            return GetString(Encoding.UTF8, p);
        }

        public static string GetString(Encoding encoding, IntPtr str)
        {
            if (str == IntPtr.Zero)
                return null;

            int byteCount = 0;

            if (Equals(encoding, Encoding.UTF32))
            {
                while (Marshal.ReadInt32(str, byteCount) != 0) byteCount += sizeof(int);
            }
            else if (Equals(encoding, Encoding.Unicode) || Equals(encoding, Encoding.BigEndianUnicode))
            {
                while (Marshal.ReadInt16(str, byteCount) != 0) byteCount += sizeof(short);
            }
            else
            {
                while (Marshal.ReadByte(str, byteCount) != 0) byteCount += sizeof(byte);
            }

            var bytes = new byte[byteCount];
            Marshal.Copy(str, bytes, 0, byteCount);
            return encoding.GetString(bytes);
        }

        public static byte[] ByteArrayFromNative(IntPtr ptr, uint length)
        {
            var ans = new byte[length];
            Marshal.Copy(ptr, ans, 0, (int) length);
            return ans;
        }

        public static IntPtr ByteArrayToNative(byte[] a)
        {
            var ptr = Marshal.AllocHGlobal(a.Length);
            Marshal.Copy(a, 0, ptr, a.Length);
            return ptr;
        }
    }
}