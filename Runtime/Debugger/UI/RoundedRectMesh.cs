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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace Unity.XR.PXR.Debugger
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class RoundedRectMesh : MonoBehaviour
    {
        [SerializeField]public float width = 1f;
        [SerializeField]public float height = 1f;
        [SerializeField]public float depth = 1f;
        [SerializeField]public float cornerRadius = 0.2f;
        [SerializeField]public bool[] corner = { true, true, true, true };

        [SerializeField]public int cornerSegments = 10; // The number of subdivisions of rounded corners
        [SerializeField]public string saveName = "CustomMesh"; // Customize the save name
        private MeshFilter meshFilter;
        private bool isInit = false;
        void OnValidate()
        {
            Init();
            GenerateRoundedRectMesh();
        }
        private void Init(){
            if(!isInit){
                meshFilter = GetComponent<MeshFilter>();
                isInit = true;
            }
        }
        public static List<Vector3> CreateRoundedRectPath(Vector3 center, float width, float height, float radius, bool[] corners, int cornerSegments)
        {
            List<Vector3> path = new();
            var halfWidth = width *0.5f;
            var halfHeight = height *0.5f;
            // Check whether the radius of the fillet is reasonable
            radius = Mathf.Min(radius, width *0.5f, height *0.5f);

            // Define the fillet subdivision Angle
            float angleStep = 90f / (float)cornerSegments;

            Vector2[] cornerCentersOffset = new Vector2[]
            {
            new(-1,1), // 左上 Left Top
            new (1, 1),  // 右上 Right Top
            new (1, -1), // 右下 Right Bottom
            new (-1, -1) // 左下 Left Bottom
            };

            // Generate the path of the rounded rectangle
            for (int i = 0; i < 4; i++)
            {
                bool isCorner = corners[i];
                var delta = isCorner ? radius : 0;
                Vector3 currentCornerCenter = center + new Vector3((halfWidth - delta) * cornerCentersOffset[i].x, (halfHeight - delta) * cornerCentersOffset[i].y, 0);

                if (isCorner)
                {
                    // Add arcs
                    for (int j = 0; j <= cornerSegments; j++)
                    {
                        float angle = (i * 90 + angleStep * j) * Mathf.Deg2Rad;
                        path.Add(currentCornerCenter + new Vector3(-Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0));
                    }
                }
                else
                {
                    // Add a right Angle
                    path.Add(currentCornerCenter);
                }
            }
            return path;
        }

        private void GenerateRoundedRectMesh()
        {
            Mesh mesh = new();
            meshFilter.mesh = mesh;
            cornerRadius = Mathf.Clamp(cornerRadius, 0, Mathf.Min(width, height) *0.5f);

            List<Vector3> path = CreateRoundedRectPath(transform.position, width, height, cornerRadius, corner, cornerSegments);
            List<Vector3> vertices = new();
            List<int> triangles = new();
            var count = path.Count;
            var doubleCount = count * 2;
            var firstIndex = 0;
            for (var j = 0; j < count; j++)
            {
                vertices.Add(path[j]);
                vertices.Add(path[j] + Vector3.forward * depth);
                triangles.Add(firstIndex);
                triangles.Add((firstIndex + 2) % doubleCount );
                triangles.Add(doubleCount);

                triangles.Add((firstIndex + 1) % doubleCount);
                triangles.Add(doubleCount + 1);
                triangles.Add((firstIndex + 3) % doubleCount );
                firstIndex += 2;
            }
            vertices.Add(transform.position);
            vertices.Add(transform.position + Vector3.forward * depth);
            var currentCount = vertices.Count;
            firstIndex = 0;
            for (var i = 0; i < count; i++)
            {
                vertices.Add(path[i]);
                vertices.Add(path[i] + Vector3.forward * depth);
                triangles.Add(firstIndex + currentCount);
                triangles.Add((firstIndex + 1) % doubleCount + currentCount);
                triangles.Add((firstIndex + 3) % doubleCount + currentCount);
                triangles.Add(firstIndex + currentCount);
                triangles.Add((firstIndex + 3) % doubleCount + currentCount);
                triangles.Add((firstIndex + 2) % doubleCount + currentCount);
                firstIndex += 2;
            }


            mesh.SetVertices(vertices);
            mesh.SetIndices(triangles, MeshTopology.Triangles, 0);

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }
    }
}
#endif