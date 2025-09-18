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
#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace Unity.XR.PXR.Debugger
{
    public class PXR_MessageController : MonoBehaviour
    {
        // Start is called before the first frame update
        public Text title;
        public Text content;
        private readonly string widthMark = "------------------------------------------------------------------";
        private readonly int maxLength = 47;
        public void Init(string title, string content)
        {
            string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string finalTitle = $"{timestamp}: {title}";

            TextGenerator generator = new();
            var settings = this.title.GetGenerationSettings(this.title.gameObject.GetComponent<RectTransform>().rect.size);
            var targetWidth = generator.GetPreferredWidth(widthMark, settings);
            var currentWidth = generator.GetPreferredWidth(finalTitle, settings);
            if (targetWidth < currentWidth)
            {
                finalTitle = title[..(maxLength - 3)];
                currentWidth = generator.GetPreferredWidth(finalTitle, settings);
                while (targetWidth < currentWidth)
                {
                    finalTitle = title[..(title.Length - 1)];
                    currentWidth = generator.GetPreferredWidth(finalTitle, settings);
                }
                finalTitle += "...";
            }

            this.title.text = finalTitle;
            this.content.text = $"{title}\n{content}";
            Reset();
        }
        public void Reset()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
        }
        public void ToggleContent()
        {
            content.gameObject.SetActive(!content.gameObject.activeSelf);
        }
    }
}
#endif