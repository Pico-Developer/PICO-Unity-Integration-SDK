using System;
using UnityEngine;

namespace Unity.XR.PICO.TOBSupport
{
    public class LongCallback : AndroidJavaProxy
    {
        public Action<long> mCallback;

        public LongCallback(Action<long> callback) : base("com.picoxr.tobservice.interfaces.LongCallback")
        {
            mCallback = callback;
        }

        public void CallBack(long var1)
        {
            PXR_EnterpriseTools.QueueOnMainThread(() =>
            {
                if (mCallback != null)
                {
                    mCallback(var1);
                }
            });
        }
    }
}