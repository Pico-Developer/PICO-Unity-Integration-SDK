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

namespace Pico.Platform.Framework
{
    public class Runner : MonoBehaviour
    {
        public static void RegisterGameObject()
        {
            var name = "Pico.Platform.Runner";
            GameObject g = GameObject.Find(name);
            if (g == null)
            {
                g = new GameObject(name);
            }

            if (g.GetComponent<Runner>() == null)
            {
                g.AddComponent<Runner>();
            }
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            Looper.ProcessMessages();
        }

        void OnApplicationQuit()
        {
            Looper.Clear();
            if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {
                CLIB.ppf_PcUnInitialize();
            }
        }
    }
}