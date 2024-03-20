using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PICO.TOBSupport
{
    public class PXR_EnterpriseTools : MonoBehaviour
    {
        public struct NoDelayedQueueItem
        {
            public Action action;
        }

        private List<NoDelayedQueueItem> _actions = new List<NoDelayedQueueItem>();
        List<NoDelayedQueueItem> _currentActions = new List<NoDelayedQueueItem>();
        private static PXR_EnterpriseTools instance;

        public void StartUp()
        {
            Debug.Log("ToBService PXR_EnterpriseTools StartUp");
        }
        public static PXR_EnterpriseTools Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PXR_EnterpriseTools>();
                }

                if (instance == null)
                {
                    GameObject obj = new GameObject("PXR_EnterpriseTools");
                    instance = obj.AddComponent<PXR_EnterpriseTools>();
                    DontDestroyOnLoad(obj);
                }

                return instance;
            }
        }
      

        public static  void QueueOnMainThread(Action taction)
        {
            lock (instance._actions)
            {
                instance._actions.Add(new NoDelayedQueueItem { action = taction });
            }
        }

        void Update()
        {
            if (_actions.Count > 0)
            {
                lock (_actions)
                {
                    _currentActions.Clear();
                    _currentActions.AddRange(_actions);
                    _actions.Clear();
                }

                for (int i = 0; i < _currentActions.Count; i++)
                {
                    _currentActions[i].action.Invoke();
                }
            }
        }
    }
}