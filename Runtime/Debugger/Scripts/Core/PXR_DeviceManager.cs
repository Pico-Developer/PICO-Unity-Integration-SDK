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
using UnityEngine;
using UnityEngine.XR;
using System;
// using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace Unity.XR.PXR.Debugger
{
    public class PXR_DeviceManager : MonoBehaviour
    {
        public static PXR_DeviceManager Instance { get; private set; }
        private InputDevice rightHandDevice;
        private InputDevice leftHandDevice;
        private XRController rightHandController;
        private XRController leftHandController;
        public Transform RightHand => rightHandController.transform;
        public Transform LeftHand => leftHandController.transform;

        public Action OnAButtonPress;
        public Action OnBButtonPress;
        public Action OnXButtonPress;
        public Action OnYButtonPress;
        public Action OnAButtonRelease;
        public Action OnBButtonRelease;
        public Action OnXButtonRelease;
        public Action OnYButtonRelease;
        public Action OnLeftGripButtonPress;
        public Action OnLeftTriggerButtonPress;
        public Action OnRightGripButtonPress;
        public Action OnRightTriggerButtonPress;
        public Action OnLeftGripButtonRelease;
        public Action OnLeftTriggerButtonRelease;
        public Action OnRightGripButtonRelease;
        public Action OnRightTriggerButtonRelease;
        private bool previousRightPrimaryButtonPress;
        private bool previousLeftPrimaryButtonPress;
        private bool previousRightSecondaryButtonPress;
        private bool previousLeftSecondaryButtonPress;
        private bool previousRightGripButtonPress;
        private bool previousLeftGripButtonPress;
        private bool previousRightTriggerButtonPress;
        private bool previousLeftTriggerButtonPress;
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"The singleton has multiple instances！");
            }
            else
            {
                Destroy(Instance);
            }
            Instance = this;
        }
        void Start()
        {
            leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            XRController[] xrControllers = FindObjectsOfType<XRController>();
            foreach (XRController controller in xrControllers){
                if (controller.controllerNode == XRNode.LeftHand){
                    leftHandController = controller;
                }
                if (controller.controllerNode == XRNode.RightHand){
                    rightHandController = controller;
                }
            }
        }
        
        public void ToggleRightController(bool state){
            rightHandController.modelParent.gameObject.SetActive(state);
        }
        private void ButtonHandler(bool currentState,ref bool previousState,in Action OnPressed,in Action OnReleased)
        {
            if (currentState && !previousState)
            {
                OnPressed?.Invoke();
            }
            if (!currentState && previousState)
            {
                OnReleased?.Invoke();
            }
            previousState = currentState;
        }
        void Update()
        {

            rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool isRightPrimaryButtonPress);
            ButtonHandler(isRightPrimaryButtonPress,ref previousRightPrimaryButtonPress, OnAButtonPress, OnAButtonRelease);

            rightHandDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isRightSecondaryButtonPress);
            ButtonHandler(isRightSecondaryButtonPress,ref previousRightSecondaryButtonPress, OnBButtonPress, OnBButtonRelease);
 
            rightHandDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool isRightGripButtonPress);
            ButtonHandler(isRightGripButtonPress,ref previousRightGripButtonPress, OnRightGripButtonPress, OnRightGripButtonRelease);

            rightHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool isRightTriggerButtonPress);
            ButtonHandler(isRightTriggerButtonPress,ref previousRightTriggerButtonPress, OnRightTriggerButtonPress, OnRightTriggerButtonRelease);

            leftHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool isLeftPrimaryButtonPress);
            ButtonHandler(isLeftPrimaryButtonPress,ref previousLeftPrimaryButtonPress, OnXButtonPress, OnXButtonRelease);

            leftHandDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isLeftSecondaryButtonPress);
            ButtonHandler(isLeftSecondaryButtonPress,ref previousLeftSecondaryButtonPress, OnYButtonPress, OnYButtonRelease);
 
            leftHandDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool isLeftGripButtonPress);
            ButtonHandler(isLeftGripButtonPress,ref previousLeftGripButtonPress, OnLeftGripButtonPress, OnLeftGripButtonRelease);

            leftHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool isLeftTriggerButtonPress);
            ButtonHandler(isLeftTriggerButtonPress,ref previousLeftTriggerButtonPress, OnLeftTriggerButtonPress, OnLeftTriggerButtonRelease);
        }
    }
}
#endif