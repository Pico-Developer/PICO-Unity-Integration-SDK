/*******************************************************************************
Copyright ? 2015-2022 PICO Technology Co., Ltd.All rights reserved.

NOTICE：All information contained herein is, and remains the property of 
PICO Technology Co., Ltd. The intellectual and technical concepts 
contained herein are proprietary to PICO Technology Co., Ltd. and may be 
covered by patents, patents in process, and are protected by trade secret or 
copyright law. Dissemination of this information or reproduction of this 
material is strictly forbidden unless prior written permission is obtained from
PICO Technology Co., Ltd. 
*******************************************************************************/

using UnityEngine;
using UnityEngine.XR;

namespace Unity.XR.PXR
{
    public class PXR_ControllerG3Animator : MonoBehaviour
    {
        public Transform triggerTran;
        public Transform menuTran;
        public Transform touchPadTran;
        public PXR_ControllerPower controllerPower;


        private bool primaryAxisState = false;
        private bool menuButtonState;
        private float trigger;

        private Vector3 menu;
        private Vector3 originTrigger;
        private Vector3 touchPadPos;


        private InputDevice currentController;
        private int handness;

        void Start()
        {
            PXR_Input.GetControllerHandness(ref handness);
            XRNode hand = handness == 0? XRNode.RightHand : XRNode.LeftHand;
            if (controllerPower != null)
            {
                controllerPower.hand = handness == 0 ? PXR_Input.Controller.RightController : PXR_Input.Controller.LeftController;
            }
            currentController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            menu = menuTran.localPosition;
            originTrigger = triggerTran.localEulerAngles;
            touchPadPos = touchPadTran.localPosition;
        }

        void Update()
        {
            currentController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out primaryAxisState);
            currentController.TryGetFeatureValue(CommonUsages.menuButton, out menuButtonState);
            currentController.TryGetFeatureValue(CommonUsages.trigger, out trigger);

            if (triggerTran != null)
            {
                trigger *= -9.0f;
                triggerTran.localEulerAngles = new Vector3(0f, 0f, Mathf.Clamp(trigger, -9f, 0f)) + originTrigger;
            }

            if (touchPadTran != null)
            {
                if (primaryAxisState)
                    touchPadTran.localPosition = touchPadPos + new Vector3(0f, -0.0005f, 0f);
                else
                    touchPadTran.localPosition = touchPadPos;
            }

            if (menuButtonState)
                menuTran.localPosition = new Vector3(0f, -0.00021f, 0f) + menu;
            else
                menuTran.localPosition = menu;
        }
    }
}