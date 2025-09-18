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
#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace Unity.XR.PXR.Debugger
{
    public class PXR_ToolManager : MonoBehaviour
    {
        private GameObject currentTool;
        public PXR_ToolButton[] toolButtons;
        public void CreateTool(GameObject tool){
            ResetButtons();
            if(currentTool != null){
                Destroy(currentTool);
            }
            currentTool = Instantiate(tool,PXR_DeviceManager.Instance.RightHand);
            PXR_DeviceManager.Instance.ToggleRightController(false);
        }
        private void ResetButtons(){
            foreach (var button in toolButtons){
                button.Reset();
            }
            PXR_DeviceManager.Instance.ToggleRightController(true);
        }
        public void DeleteTool(){
            ResetButtons();
            Destroy(currentTool);
            currentTool = null;
        }
    }
}
#endif