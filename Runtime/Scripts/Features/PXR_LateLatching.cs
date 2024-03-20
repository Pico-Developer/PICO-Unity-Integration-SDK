using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;


namespace Unity.XR.PXR
{
    [Serializable]
    public class PXR_LateLatching : MonoBehaviour
    {
#if UNITY_2020_3_OR_NEWER
        private Camera m_LateLatchingCamera;

        static XRDisplaySubsystem s_DisplaySubsystem = null;

        static List<XRDisplaySubsystem> s_DisplaySubsystems = new List<XRDisplaySubsystem>();

        private void Awake()
        {
            m_LateLatchingCamera = GetComponent<Camera>();
        }

        private void OnEnable()
        {

            List<XRDisplaySubsystem> displaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances(displaySubsystems);
            Debug.Log("PXR_U OnEnable() displaySubsystems.Count = " + displaySubsystems.Count);
            for (int i = 0; i < displaySubsystems.Count; i++)
            {
                s_DisplaySubsystem = displaySubsystems[i];

            }
        }

        private void OnDisable()
        {

        }

        void Update()
        {
            if (s_DisplaySubsystem == null)
            {
                List<XRDisplaySubsystem> displaySubsystems = new List<XRDisplaySubsystem>();
                SubsystemManager.GetInstances(displaySubsystems);

                if (displaySubsystems.Count > 0)
                {
                    s_DisplaySubsystem = displaySubsystems[0];
                }
            }

            if (null == s_DisplaySubsystem)
                return;


            s_DisplaySubsystem.MarkTransformLateLatched(m_LateLatchingCamera.transform, XRDisplaySubsystem.LateLatchNode.Head);

        }

        private void OnPreRender()
        {
            s_DisplaySubsystem.BeginRecordingIfLateLatched(m_LateLatchingCamera);
        }

        private void OnPostRender()
        {
            s_DisplaySubsystem.EndRecordingIfLateLatched(m_LateLatchingCamera);
        }

        private void FixedUpdate()
        {
        }

        private void LateUpdate()
        {
        }
#endif
    }
}
