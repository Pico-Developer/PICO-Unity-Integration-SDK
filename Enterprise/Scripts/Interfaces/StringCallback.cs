using System;
using UnityEngine;

namespace Unity.XR.PICO.TOBSupport
{
    public class StringCallback : AndroidJavaProxy
    {
        public Action<string> mCallback;

        public StringCallback(Action<string> callback) : base("com.picoxr.tobservice.interfaces.StringCallback")
        {
            mCallback = callback;
        }

        public void CallBack(string var1)
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