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
    public class PXR_InspectorManager : MonoBehaviour, IPXR_PanelManager
    {
        public static PXR_InspectorManager Instance;
        private void Awake(){
            if(Instance == null){
                Instance = this;
            }
        }
        public GameObject inspectItem;
        public Transform content;
        public Text positionText;
        public Text rotationText;
        public Text scaleText;
        public GameObject transformInfoNode;
        private Transform target;
        void Update(){
            ShowTransformInfo();
        }
        private void ClearAllChildren(){
            int childCount = content.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(content.GetChild(i).gameObject);
            }
        }
        public void CreateInspector()
        {
            GenerateInspectorTree();
        }
        public void Init(){
            CreateInspector();
        }
        public void Reset(){
            for (int i = 0; i < content.childCount; i++)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetChild(i).GetComponent<RectTransform>());
            }
        }
        public void SetTransformInfo(GameObject target){
            this.target = target.transform;
            transformInfoNode.SetActive(true);
            ShowTransformInfo();
        }
        public void Refresh(){
            ClearAllChildren();
            GenerateInspectorTree();
        } 
        private void GenerateInspectorTree(){
            GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            // 遍历所有根GameObject
            foreach (GameObject obj in rootObjects)
            {
                if(!obj.TryGetComponent<PXR_UIController>(out _) && !obj.TryGetComponent<PXR_UIManager>(out _) && obj.activeSelf){
                    var go = Instantiate(inspectItem, content);
                    if(go.TryGetComponent(out PXR_InspectorItem item)){
                        item.Init(obj.transform);
                    }
                }
            }
            Reset();       
        }
        private void ShowTransformInfo(){
            if(target != null){
                positionText.text = $"x:{target.position.x} y:{target.position.y} z:{target.position.z}";
                rotationText.text = $"x:{target.eulerAngles.x} y:{target.eulerAngles.y} z:{target.eulerAngles.z} ";
                scaleText.text = $"x:{target.localScale.x} y:{target.localScale.y} z:{target.localScale.z}";
            }else{
                transformInfoNode.SetActive(false);
                Refresh();
            }
        }
    }
}
#endif