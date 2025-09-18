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
using UnityEditor;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace Unity.XR.PXR.Debugger
{
    public class PXR_PicoDebuggerManager : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod()
        {
            var config = Resources.Load<PXR_PicoDebuggerSO>("PXR_PicoDebuggerSO");
            if(config.isOpen){
                AddPrefab();
            }
        }

        private static void AddPrefab()
        {
            GameObject prefab = Resources.Load<GameObject>("PICODebugger");
            if (prefab != null)
            {
                Instantiate(prefab, Vector3.zero, Quaternion.identity);
            }
            else
            {
                Debug.LogError("Prefab not found in Resources folder.");
            }
        }
    }
}
#endif