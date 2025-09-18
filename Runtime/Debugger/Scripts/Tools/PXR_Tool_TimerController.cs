
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
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace Unity.XR.PXR.Debugger
{
    public class PXR_Tool_TimerController : MonoBehaviour
    {
        private readonly float delayTime = 0.17f;
        [SerializeField] private Animator anim;
        private bool isTurnOn = false;
        private bool isLock = true;

        void Start()
        {
            PXR_DeviceManager.Instance.OnRightGripButtonPress += CutDown;
            StartCoroutine(Delay(Open));
        }
        void OnDestroy()
        {
            PXR_DeviceManager.Instance.OnRightGripButtonPress -= CutDown;
        }
        private void CutDown()
        {
            if (isLock) return;
            isTurnOn = !isTurnOn;
            anim.SetBool("TurnOn", isTurnOn);
            StartCoroutine(Delay(Open));
            if (isTurnOn)
            {
                StartCoroutine(Delay(TimePause));
            }
            else
            {
                Time.timeScale = 1f;
            }
            isLock = true;
        }
        private IEnumerator Delay(Action action)
        {
            yield return new WaitForSeconds(delayTime);
            action();
        }
        private void Open()
        {
            isLock = false;
        }
        private void TimePause()
        {
            Time.timeScale = 0f;
        }
    }
}
#endif