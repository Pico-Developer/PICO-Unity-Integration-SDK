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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace Unity.XR.PXR.Debugger
{
    public class PXR_ToolButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private bool isButtonPress = false;
        private bool isButtonHover = false;
        // private float hoverSpeed = 0.8f;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color hoverColor;
        [SerializeField] private Image icon;
        public UnityEvent onButtonPressed;
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isButtonPress)
            {
                onButtonPressed?.Invoke();
                icon.color = hoverColor;
                isButtonHover = false;
                isButtonPress = true;
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isButtonPress)
            {
                icon.color = hoverColor;
                isButtonHover = true;
            }
        }
        public void Reset(){
            icon.color = defaultColor;
            isButtonHover = false;
            isButtonPress = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isButtonPress)
            {
                isButtonHover = false;
                icon.color = defaultColor;
            }
        }
        public void CreateTool(){

        }
    }
}
#endif