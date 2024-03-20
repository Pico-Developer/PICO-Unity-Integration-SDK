using System;
using UnityEngine;

namespace Unity.XR.PICO.TOBSupport
{
    public class BindCallback : AndroidJavaProxy
    {
        public  Action<bool> mCallback;

        public BindCallback(Action<bool> callback) : base("com.picoxr.tobservice.interfaces.BoolCallback")
        {
            mCallback = callback;
        }

        public void CallBack(bool var1)
        {
            Debug.Log("ToBService bindCallBack 回调:" + var1);
            PXR_EnterprisePlugin.GetServiceBinder();
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