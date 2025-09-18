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
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace Unity.XR.PXR.Debugger
{
    public class PXR_InspectorItem : MonoBehaviour, IPointerDownHandler
    {
        public Text text;
        private bool isShowChildren = false;
        private bool hasChild = false;
        private string icon => hasChild?(isShowChildren?"- ":"+ "):"";
        private string nodeName;
        private GameObject target;
        public void SetTitle(){
            text.text = icon + nodeName;
        }
        public void Init(Transform item)
        {
            target = item.gameObject;
            nodeName = item.name;
            if(item.childCount > 0){
                hasChild = true;
                TraverseChild(item);
            }
            SetTitle();
        }
        public void AddItem(Transform item)
        {
            var go = Instantiate(PXR_InspectorManager.Instance.inspectItem, transform);
            if (go.TryGetComponent(out PXR_InspectorItem inspectItem))
            {
                inspectItem.Init(item);
                inspectItem.gameObject.SetActive(false);
            }
        }
        public void TraverseChild(Transform current)
        {
            for (int i = 0; i < current.childCount; i++)
            {
                // Debug.Log($"TraverseChild: {current.GetChild(i).name}");
                AddItem(current.GetChild(i));
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isShowChildren = !isShowChildren;
            // LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
            for (int i = 0; i < transform.childCount; i++)
            {
                // Debug.Log($"TraverseChild: {transform.GetChild(i).name}");
                transform.GetChild(i).gameObject.SetActive(isShowChildren);
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetChild(i).GetComponent<RectTransform>());
            }
            var root = transform;
            while(root.TryGetComponent(out PXR_InspectorItem _)){
                LayoutRebuilder.ForceRebuildLayoutImmediate(root.GetComponent<RectTransform>());
                root = root.parent;
            }
            SetTitle();
            PXR_InspectorManager.Instance.SetTransformInfo(target);
        }
    }
}
#endif