using System;
using UnityEngine;

namespace Unity.XR.PICO.TOBSupport
{
    public class FileCopyCallback: AndroidJavaProxy
    {
        public Action mOnCopyStart;
        public Action<double> mOnCopyProgress;
        public Action<int> mOnCopyFinish;
  
        public FileCopyCallback(Action onCopyStart,Action<double> onCopyProgress,Action<int> onCopyFinish) : base("com.picoxr.tobservice.interfaces.FileCopyCallback")
        {
            mOnCopyStart = onCopyStart;
            mOnCopyProgress = onCopyProgress;
            mOnCopyFinish = onCopyFinish;
        }

        public void OnCopyStart()
        {
            PXR_EnterpriseTools.QueueOnMainThread(() =>
            {
                if (mOnCopyStart!=null)
                {
                    mOnCopyStart();
                }
            });
        }
        public void OnCopyProgress(double var1)
        {
            PXR_EnterpriseTools.QueueOnMainThread(() =>
            {
                if (mOnCopyProgress!=null)
                {
                    mOnCopyProgress(var1);
                }
            });
        }
        public void OnCopyFinish(int var1)
        {
            PXR_EnterpriseTools.QueueOnMainThread(() =>
            {
                if (mOnCopyFinish!=null)
                {
                    mOnCopyFinish(var1);
                }
            });
        }
    }
}