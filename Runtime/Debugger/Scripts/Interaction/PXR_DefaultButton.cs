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
    public class PXR_DefaultButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        bool isButtonPress = false;
        bool isButtonHover = false;
        private float hoverSpeed = 0.8f;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color hoverColor;
        [SerializeField] private GameObject panelGO;
        [SerializeField] private GameObject[] panelList;
        [SerializeField] private PXR_DefaultButton[] buttonList;
        [SerializeField] private MeshRenderer bg;
        [SerializeField] private MeshRenderer border;
        [SerializeField] private Sprite sprite;
        private float borderAlpha;
        private void Start()
        {
            Reset();
        }
        private void OnValidate()
        {
            transform.GetChild(0).GetComponent<Image>().sprite = sprite;
        }
        // Called when the button is selected
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isButtonPress)
            {
                border.material.color = hoverColor;
                bg.material.color = defaultColor;
                isButtonHover = false;
                isButtonPress = true;
                OpenPanel();
            }
        }
        private void Update()
        {
            if (isButtonHover)
            {
                bg.material.color = Color.Lerp(bg.material.color, hoverColor, hoverSpeed * Time.deltaTime);
                var borderColor = Color.Lerp(border.material.color, hoverColor, hoverSpeed * Time.deltaTime);
                borderAlpha += hoverSpeed * Time.deltaTime;
                borderColor.a = borderAlpha;
                border.material.color = borderColor;
            }
        }
        private void OpenPanel(){
            for (var i = 0; i < panelList.Length; i++)
            {
                panelList[i].SetActive(false);
            }
            for (var i = 0; i<buttonList.Length; i++){
                buttonList[i].Reset();
            }
           panelGO.SetActive(true);
            if(panelGO.TryGetComponent(out IPXR_PanelManager manager)){
                manager.Init();
            } 
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isButtonPress)
            {
                border.material.color = new Color(0, 0, 0, 0);
                borderAlpha = 0f;
                isButtonHover = true;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isButtonPress)
            {
                isButtonHover = false;
                border.material.color = new Color(0, 0, 0, 0);
                bg.material.color = defaultColor;
            }
        }
        public void Reset()
        {
            bg.material.color = defaultColor;
            border.material.color = new Color(0, 0, 0, 0);
            isButtonPress = false;
            isButtonHover = false;
            panelGO.SetActive(false);
        }
    }
}
#endif