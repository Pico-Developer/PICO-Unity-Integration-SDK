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
    public class PXR_BGWithTouch : MonoBehaviour, IPointerMoveHandler, IPointerEnterHandler, IPointerExitHandler
    {  
        private Material material;
        private Vector4 v;
        private void Start()
        {
            var rect = GetComponent<RectTransform>().rect;
            material = new Material(GetComponent<Image>().material);
            v.z = rect.width;
            v.w = rect.height;
            material.SetVector("_TouchPos",v);
            GetComponent<Image>().material = material;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
        }
        private void Update()
        {
            
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            Vector3 offset = eventData.pointerCurrentRaycast.worldPosition-transform.position;
            v.x= offset.x/v.z;
            v.y= offset.y/v.w;
            material.SetVector("_TouchPos", v);
        }
    }
}
#endif