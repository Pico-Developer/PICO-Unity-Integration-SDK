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
    public class PXR_DragButton : MonoBehaviour, IBeginDragHandler, IDragHandler,IEndDragHandler
    {
        private Image image;
        private Transform _camera;
        public PXR_UIController uiController;
        private Vector3 origin; // Center of a sphere
        private float radius = 5f; // Radius of a sphere
        public Transform container;
        [SerializeField]private Color defaultColor;
        [SerializeField]private Color hoverColor;
        private void Start()
        {
            _camera = Camera.main.transform;
            image = GetComponent<Image>();
        }
        private void UpdateTransformPosition(PointerEventData eventData)
        {
            // Gets the position and direction of the controller
            Vector3 controllerPosition = eventData.pointerCurrentRaycast.worldPosition;

            // Calculate the point at which the ray intersects the sphere
            Vector3 sphereCenterToController = controllerPosition - origin;
            Vector3 intersectionPoint = origin + sphereCenterToController.normalized * radius;
            Vector3 intersectionDirection = (intersectionPoint - origin).normalized;
            float angle = Vector3.Angle(intersectionDirection, Vector3.up);
            if (angle < 45 || angle > 135)return;
            var forward = container.position - _camera.position;
            forward.y = 0;
            image.color = Color.Lerp(image.color,hoverColor,Time.deltaTime);
            container.forward = forward;
            container.position = intersectionPoint;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != gameObject) return;
            origin = uiController.origin;
            radius = uiController.GetDistance();
            // Update the position when you start dragging
            UpdateTransformPosition(eventData);
        }
         public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != gameObject) return;
            image.color = defaultColor;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != gameObject) return;
            // Update position while dragging
            UpdateTransformPosition(eventData);
        }
    }
}
#endif