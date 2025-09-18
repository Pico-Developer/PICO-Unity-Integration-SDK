
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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace Unity.XR.PXR.Debugger
{
    public class PXR_Tool_Ruler : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] Material mal;
        [SerializeField] private GameObject rulerPrefab;
        [SerializeField] private Transform exitPosition;
        private Vector3 startPosition;
        private float size = 0.02f;
        private bool isStart = false;
        private GameObject ruler;
        private Text text;
        private Transform container;
        private List<GameObject> rulers = new();

        void Start()
        {
            PXR_DeviceManager.Instance.OnRightGripButtonPress += StartMeasure;
            PXR_DeviceManager.Instance.OnRightGripButtonRelease += StopMeasure;
            var config = Resources.Load<PXR_PicoDebuggerSO>("PXR_PicoDebuggerSO");
            if(config.isOpen)
            {
                switch (config.rulerClearButton)
                {
                    case LauncherButton.PressA:
                        PXR_DeviceManager.Instance.OnAButtonPress += OnClearButtonPress;
                        break;
                    case LauncherButton.PressB:
                        PXR_DeviceManager.Instance.OnBButtonPress += OnClearButtonPress;
                        break;
                    case LauncherButton.PressX:
                        PXR_DeviceManager.Instance.OnXButtonPress += OnClearButtonPress;
                        break;
                    case LauncherButton.PressY:
                        PXR_DeviceManager.Instance.OnYButtonPress += OnClearButtonPress;
                        break;
                    default:
                        break;
                }
            }
        }

        void Update()
        {
            if (isStart)
            {
                GenerateRoundedRectMesh();
            }
        }
        void OnDestroy() {
            PXR_DeviceManager.Instance.OnRightGripButtonPress -= StartMeasure;
            PXR_DeviceManager.Instance.OnRightGripButtonRelease -= StopMeasure;
            var config = Resources.Load<PXR_PicoDebuggerSO>("PXR_PicoDebuggerSO");
            if(config.isOpen)
            {
                switch (config.rulerClearButton)
                {
                    case LauncherButton.PressA:
                        PXR_DeviceManager.Instance.OnAButtonPress -= OnClearButtonPress;
                        break;
                    case LauncherButton.PressB:
                        PXR_DeviceManager.Instance.OnBButtonPress -= OnClearButtonPress;
                        break;
                    case LauncherButton.PressX:
                        PXR_DeviceManager.Instance.OnXButtonPress -= OnClearButtonPress;
                        break;
                    case LauncherButton.PressY:
                        PXR_DeviceManager.Instance.OnYButtonPress -= OnClearButtonPress;
                        break;
                    default:
                        break;
                }
            }
        }
        private void StartMeasure()
        {
            startPosition = exitPosition.position;
            ruler = Instantiate(rulerPrefab);
            text = ruler.GetComponentInChildren<Text>();
            container = ruler.transform.Find("Container");
            ruler.SetActive(true);
            isStart = true;
            rulers.Add(ruler);
        }
        private void StopMeasure()
        {
            isStart = false;
        }
        private void OnClearButtonPress(){
            StopMeasure();
            for (var i = 0; i < rulers.Count; i++)
            {
                Destroy(rulers[i]);
            }
        }
        private void GenerateRoundedRectMesh()
        {
            var meshFilter = ruler.GetComponent<MeshFilter>();
            meshFilter.GetComponent<MeshRenderer>().material = mal;
            Mesh mesh = new();
            meshFilter.mesh = mesh;
            List<Vector3> vertices = new();
            List<Vector2> uvs = new();
            List<int> triangles = new();

            var dir = exitPosition.position-startPosition;
            var p0 = startPosition + exitPosition.forward * size;
            var p2 = startPosition - exitPosition.forward * size;
            var p1 = exitPosition.position + exitPosition.forward * size;
            var p3 = exitPosition.position - exitPosition.forward * size;


            vertices.Add(p0);
            uvs.Add(new Vector2(0, 0));
            vertices.Add(p2);
            uvs.Add(new Vector2(0, 1));
            triangles.Add(0);
            triangles.Add(2);
            triangles.Add(1);
            triangles.Add(0);
            triangles.Add(1);
            triangles.Add(2);

            vertices.Add(p1);
            uvs.Add(new Vector2(1, 0));
            vertices.Add(p3);
            uvs.Add(new Vector2(1, 1));
            triangles.Add(1);
            triangles.Add(2);
            triangles.Add(3);
            triangles.Add(2);
            triangles.Add(1);
            triangles.Add(3);
            text.text = string.Format("{0:0.000}", dir.magnitude);


            mesh.SetVertices(vertices);
            mesh.SetUVs(0,uvs);
            mesh.SetIndices(triangles, MeshTopology.Triangles, 0);

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            var normal = Vector3.Cross(p2 - p0, p1 - p0);

            container.transform.position = startPosition+ dir*0.5f +(exitPosition.forward - normal) * size;
            container.transform.forward = normal;
            meshFilter.GetComponent<MeshRenderer>().material.SetFloat("_MeshLength", dir.magnitude);
        }
    }
}
#endif