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

using UnityEditor;
using UnityEngine;

namespace Pico.Platform.Editor
{
    public class Menu
    {
        [MenuItem("PXR_SDK/Platform Settings")]
        public static void ShowNewConfig()
        {
            PicoSettings window = ScriptableObject.CreateInstance(typeof(PicoSettings)) as PicoSettings;
            window.minSize = new Vector2(400, 450);
            window.maxSize = new Vector2(400, 450);
            window.ShowUtility();
        }

        [MenuItem("PXR_SDK/PC Debug Settings")]
        public static void EditPcConfig()
        {
            var obj = PcConfigEditor.load();
            obj.name = "PC Debug Configuration";
            Selection.activeObject = obj;
        }
    }
}