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
    public class PXR_FolderButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private Image image;
        public GameObject content;
        private bool isShowContent = false;
        private float hoverSpeed = 0.5f;
        [SerializeField] private Color defaultColor = new(184, 235, 255);
        bool isButtonHover = false;
        private void Start()
        {
            image = GetComponent<Image>();
            image.color = defaultColor;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            // text.color = Color.red;
            isShowContent = !isShowContent;
            content.SetActive(isShowContent);
            image.transform.Rotate(0, 0, 180);
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.transform.parent.gameObject.GetComponent<RectTransform>());
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            isButtonHover = true;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            isButtonHover = false;
            image.color = defaultColor;
        }
        private void Update()
        {
            if (isButtonHover)
            {
                image.color = Color.Lerp(image.color, Color.white, hoverSpeed * Time.deltaTime);
            }
        }
    }
}
#endif