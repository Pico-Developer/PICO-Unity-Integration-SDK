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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR
{
    public class PXR_ScreenFade : MonoBehaviour
    {
        [Tooltip("The gradient of time.")]
        public float gradientTime = 5.0f;
        [Tooltip("Basic color.")]
        public Color fadeColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        [Tooltip("The default value is 4000.")]
        private int renderQueue = 4000;
        private MeshRenderer gradientMeshRenderer;
        private MeshFilter gradientMeshFilter;
        private Material gradientMaterial = null;
        private bool isGradient = false;
        private float currentAlpha;
        private float nowFadeAlpha;
        private List<Vector3> verts;
        private List<int> indices;
        private int N = 5;

        void Awake()
        {
            CreateFadeMesh();
            SetCurrentAlpha(0);
        }
        void OnEnable()
        {
            StartCoroutine(ScreenFade());
        }
        void OnDestroy()
        {
            DestoryGradientMesh();
        }

        private void CreateFadeMesh()
        {
            verts = new List<Vector3>();
            indices = new List<int>();
            gradientMaterial = new Material(Shader.Find("PXR_SDK/PXR_Fade"));
            gradientMeshFilter = gameObject.AddComponent<MeshFilter>();
            gradientMeshRenderer = gameObject.AddComponent<MeshRenderer>();

            CreateModel();
        }

        

        public void SetCurrentAlpha(float alpha)
        {
            currentAlpha = alpha;
            SetAlpha();
        }

        IEnumerator ScreenFade()
        {
            float nowTime = 0.0f;
            while (nowTime < gradientTime)
            {
                nowTime += Time.deltaTime;
                nowFadeAlpha = Mathf.Lerp(1.0f, 0, Mathf.Clamp01(nowTime / gradientTime));
                SetAlpha();
                yield return null;
            }
        }

        private void SetAlpha()
        {
            Color color = fadeColor;
            color.a = Mathf.Max(currentAlpha, nowFadeAlpha);
            isGradient = color.a > 0;
            if (gradientMaterial != null)
            {
                gradientMaterial.color = color;
                gradientMaterial.renderQueue = renderQueue;
                gradientMeshRenderer.material = gradientMaterial;
                gradientMeshRenderer.enabled = isGradient;
            }
        }

        void CreateModel()
        {
            for (float i = -N / 2f; i <= N / 2f; i++)
            {
                for (float j = -N / 2f; j <= N / 2f; j++)
                {
                    verts.Add(new Vector3(i, j, -N / 2f));
                }
            }
            for (float i = -N / 2f; i <= N / 2f; i++)
            {
                for (float j = -N / 2f; j <= N / 2f; j++)
                {
                    verts.Add(new Vector3(N / 2f, j, i));
                }
            }
            for (float i = -N / 2f; i <= N / 2f; i++)
            {
                for (float j = -N / 2f; j <= N / 2f; j++)
                {
                    verts.Add(new Vector3(i, N / 2f, j));
                }
            }
            for (float i = -N / 2f; i <= N / 2f; i++)
            {
                for (float j = -N / 2f; j <= N / 2f; j++)
                {
                    verts.Add(new Vector3(-N / 2f, j, i));
                }
            }
            for (float i = -N / 2f; i <= N / 2f; i++)
            {
                for (float j = -N / 2f; j <= N / 2f; j++)
                {
                    verts.Add(new Vector3(i, j, N / 2f));
                }
            }
            for (float i = -N / 2f; i <= N / 2f; i++)
            {
                for (float j = -N / 2f; j <= N / 2f; j++)
                {
                    verts.Add(new Vector3(i, -N / 2f, j));
                }
            }

            for (int i = 0; i < verts.Count; i++)
            {
                verts[i] = verts[i].normalized * 0.7f;
            }

            CreateMakePos(0);
            CreateMakePos(1);
            CreateMakePos(2);
            OtherMakePos(3);
            OtherMakePos(4);
            OtherMakePos(5);
            Mesh mesh = new Mesh();
            mesh.vertices = verts.ToArray();
            mesh.triangles = indices.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = -normals[i];
            }
            mesh.normals = normals;
            int[] triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int t = triangles[i];
                triangles[i] = triangles[i + 2];
                triangles[i + 2] = t;
            }
            mesh.triangles = triangles;
            gradientMeshFilter.mesh = mesh;
        }
        public void CreateMakePos(int num)
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    int index = j * (N + 1) + (N + 1) * (N + 1) * num + i;
                    int up = (j + 1) * (N + 1) + (N + 1) * (N + 1) * num + i;
                    indices.AddRange(new int[] { index, index + 1, up + 1 });
                    indices.AddRange(new int[] { index, up + 1, up });
                }
            }
        }
        public void OtherMakePos(int num)
        {
            for (int i = 0; i < N + 1; i++)
            {
                for (int j = 0; j < N + 1; j++)
                {
                    if (i != N && j != N)
                    {
                        int index = j * (N + 1) + (N + 1) * (N + 1) * num + i;
                        int up = (j + 1) * (N + 1) + (N + 1) * (N + 1) * num + i;
                        indices.AddRange(new int[] { index, up + 1, index + 1 });
                        indices.AddRange(new int[] { index, up, up + 1 });
                    }
                }
            }
        }
        private void DestoryGradientMesh()
        {
            if (gradientMeshRenderer != null)
                Destroy(gradientMeshRenderer);

            if (gradientMaterial != null)
                Destroy(gradientMaterial);

            if (gradientMeshFilter != null)
                Destroy(gradientMeshFilter);
        }
    }
}