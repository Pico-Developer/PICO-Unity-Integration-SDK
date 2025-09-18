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
    public class PXR_LogManager : MonoBehaviour, IPXR_PanelManager
    {
        public List<GameObject> infoList = new();
        public List<GameObject> warningList = new();
        public List<GameObject> errorList = new();
        public GameObject errorMessage;
        public GameObject warningMessage;
        public GameObject infoMessage;
        public Text infoText;
        public Text warningText;
        public Text errorText;
        public Transform messageContainer;
        private int ListCount => infoList.Count + warningList.Count + errorList.Count;
        private void AddMessage(string title, string content, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    CreateMessage(title, content, type, errorList, errorMessage);
                    break;
                case LogType.Assert:
                    break;
                case LogType.Warning:
                    CreateMessage(title, content, type, warningList, warningMessage);
                    break;
                case LogType.Log:
                    CreateMessage(title, content, type, infoList, infoMessage);
                    break;
                case LogType.Exception:
                    break;
            }
            RecaculateLogCount();
        }
        private void CreateMessage(string title, string content, in LogType type, in List<GameObject> list, in GameObject template)
        {
            var msg = Instantiate(template, messageContainer).GetComponent<PXR_MessageController>();
            msg.Init(title, content);
            list.Add(msg.gameObject);
        }
        private void RecaculateLogCount()
        {
            infoText.text = infoList.Count.ToString();
            warningText.text = warningList.Count.ToString();
            errorText.text = errorList.Count.ToString();
        }
        public void FilterLogs(LogType type, bool isFilter)
        {
            switch (type)
            {
                case LogType.Error:
                    ToggleLogs(errorList, isFilter);
                    break;
                case LogType.Assert:
                    break;
                case LogType.Warning:
                    ToggleLogs(warningList, isFilter);
                    break;
                case LogType.Log:
                    ToggleLogs(infoList, isFilter);
                    break;
                case LogType.Exception:
                    break;
            }
        }
        private void ToggleLogs(in List<GameObject> list, bool status)
        {
            foreach (var item in list)
            {
                item.SetActive(status);
            }
        }
        void Start()
        {
            Application.logMessageReceived += OnLogMessageReceived;
        }
        private void OnLogMessageReceived(string logString, string stackTrace, LogType type)
        {
            if (PXR_UIController.Instance.config.maxInfoCount > ListCount)
            {
                AddMessage(logString, stackTrace, type);
            }
        }
        void OnEnable()
        {
            foreach (var item in infoList)
            {
                item.GetComponent<PXR_MessageController>().Reset();
            }
            foreach (var item in warningList)
            {
                item.GetComponent<PXR_MessageController>().Reset();
            }
            foreach (var item in errorList)
            {
                item.GetComponent<PXR_MessageController>().Reset();
            }
        }
        public void Init(){
            
        }
        void OnDestroy()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
        }
    }
}
#endif