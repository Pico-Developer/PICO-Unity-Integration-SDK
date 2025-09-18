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
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace Unity.XR.PXR.Debugger
{
    public class PXR_LogButton : MonoBehaviour, IPointerDownHandler
    {
        public LogType type;
        [SerializeField]private Color defaultColor;
        [SerializeField]private PXR_LogManager logManager;
        [SerializeField]private Image icon;
        [SerializeField]private Text text;
        private bool isFilter = true;
        public void OnPointerDown(PointerEventData eventData)
        {
            isFilter = !isFilter;
            logManager.FilterLogs(type,isFilter);
            icon.color = isFilter?defaultColor:Color.gray;
            text.color = isFilter?defaultColor:Color.gray;
        }
    }
}
#endif