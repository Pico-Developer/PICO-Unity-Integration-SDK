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
#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace Unity.XR.PXR.Debugger
{
    public class PXR_UIController : MonoBehaviour
    {
        public static PXR_UIController Instance { get; private set; }
        public PXR_PicoDebuggerSO config;
        [HideInInspector] public Vector3 origin;
        private Transform _camera;
        private float distance;
        private StartPosiion state;
        public void Awake()
        {
            if (config == null)
            {
                config = Resources.Load<PXR_PicoDebuggerSO>("PXR_PicoDebuggerSO");
            }

            if (Instance == null)
            {
                Instance = this;
            }
            Init();
        }
        private void Init()
        {
            _camera = Camera.main.transform;
            state = config.startPosition;
            distance = GetDistance();
        }
        public float GetDistance()
        {
            return state switch
            {
                StartPosiion.Far => 3f,
                StartPosiion.Medium => 2f,
                StartPosiion.Near => 1f,
                _ => 2f,
            };
        }
        private void OnEnable()
        {
            ResetTransform();
        }
        // Update is called once per frame
        private void ResetTransform()
        {
            origin = _camera.position;
            transform.position = origin + distance * _camera.transform.forward ;
            transform.forward = transform.position - origin;
        }
    }
}
#endif