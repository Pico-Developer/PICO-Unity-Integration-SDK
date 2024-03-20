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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Unity.XR.PXR
{
    public class PXR_ObjImporter : MonoBehaviour
    {
        private static PXR_ObjImporter instance;

        public static PXR_ObjImporter Instance
        {
            get { return instance ?? (instance = new PXR_ObjImporter()); }
        }

        private List<int> triangles;
        private List<Vector3> vertices;
        private List<Vector2> uv;
        private List<Vector3> normals;
        private List<PxrVector3Int> faceData;
        private List<int> intArray;

        private const int MinPow10 = -16;
        private const int MaxPow10 = 16;
        private const int NumPows10 = MaxPow10 - MinPow10 + 1;
        private static readonly float[] pow10 = GenerateLookupTable();

        public Mesh ImportFile(string filePath)
        {
            triangles = new List<int>();
            vertices = new List<Vector3>();
            uv = new List<Vector2>();
            normals = new List<Vector3>();
            faceData = new List<PxrVector3Int>();
            intArray = new List<int>();

            LoadMeshData(filePath);

            Vector3[] newVerts = new Vector3[faceData.Count];
            Vector2[] newUVs = new Vector2[faceData.Count];
            Vector3[] newNormals = new Vector3[faceData.Count];

            for (int i = 0; i < faceData.Count; i++)
            {
                newVerts[i] = vertices[faceData[i].x - 1];
                if (faceData[i].y >= 1)
                    newUVs[i] = uv[faceData[i].y - 1];

                if (faceData[i].z >= 1)
                    newNormals[i] = normals[faceData[i].z - 1];
            }

            Mesh mesh = new Mesh();
            mesh.vertices = newVerts;
            mesh.uv = newUVs;
            mesh.normals = newNormals;
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateBounds();

            return mesh;
        }

        private void LoadMeshData(string fileName)
        {
            StringBuilder sb = new StringBuilder();
            string text = File.ReadAllText(fileName);
            int start = 0;
            string objectName = null;
            int faceDataCount = 0;

            StringBuilder sbFloat = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    sb.Remove(0, sb.Length);

                    sb.Append(text, start + 1, i - start);
                    start = i;

                    if (sb[0] == 'o' && sb[1] == ' ')
                    {
                        sbFloat.Remove(0, sbFloat.Length);
                        int j = 2;
                        while (j < sb.Length)
                        {
                            objectName += sb[j];
                            j++;
                        }
                    }
                    else if (sb[0] == 'v' && sb[1] == ' ') // Vertices
                    {
                        int splitStart = 2;

                        vertices.Add(new Vector3(GetFloat(sb, ref splitStart, ref sbFloat),
                            GetFloat(sb, ref splitStart, ref sbFloat), GetFloat(sb, ref splitStart, ref sbFloat)));
                    }
                    else if (sb[0] == 'v' && sb[1] == 't' && sb[2] == ' ') // UV
                    {
                        int splitStart = 3;

                        uv.Add(new Vector2(GetFloat(sb, ref splitStart, ref sbFloat),
                            GetFloat(sb, ref splitStart, ref sbFloat)));
                    }
                    else if (sb[0] == 'v' && sb[1] == 'n' && sb[2] == ' ') // Normals
                    {
                        int splitStart = 3;

                        normals.Add(new Vector3(GetFloat(sb, ref splitStart, ref sbFloat),
                            GetFloat(sb, ref splitStart, ref sbFloat), GetFloat(sb, ref splitStart, ref sbFloat)));
                    }
                    else if (sb[0] == 'f' && sb[1] == ' ')
                    {
                        int splitStart = 2;

                        int j = 1;
                        intArray.Clear();
                        int info = 0;

                        while (splitStart < sb.Length && char.IsDigit(sb[splitStart]))
                        {
                            faceData.Add(new PxrVector3Int(GetInt(sb, ref splitStart, ref sbFloat),
                                GetInt(sb, ref splitStart, ref sbFloat), GetInt(sb, ref splitStart, ref sbFloat)));
                            j++;

                            intArray.Add(faceDataCount);
                            faceDataCount++;
                        }

                        info += j;
                        j = 1;
                        while (j + 2 < info)
                        {
                            triangles.Add(intArray[0]);
                            triangles.Add(intArray[j]);
                            triangles.Add(intArray[j + 1]);

                            j++;
                        }
                    }
                }
            }
        }

        private float GetFloat(StringBuilder sb, ref int start, ref StringBuilder sbFloat)
        {
            sbFloat.Remove(0, sbFloat.Length);
            while (start < sb.Length &&
                   (char.IsDigit(sb[start]) || sb[start] == '-' || sb[start] == '.'))
            {
                sbFloat.Append(sb[start]);
                start++;
            }
            start++;

            return ParseFloat(sbFloat);
        }

        private int GetInt(StringBuilder sb, ref int start, ref StringBuilder sbInt)
        {
            sbInt.Remove(0, sbInt.Length);
            while (start < sb.Length &&
                   (char.IsDigit(sb[start])))
            {
                sbInt.Append(sb[start]);
                start++;
            }
            start++;

            return IntParseFast(sbInt);
        }

        private static float[] GenerateLookupTable()
        {
            var result = new float[(-MinPow10 + MaxPow10) * 10];
            for (int i = 0; i < result.Length; i++)
                result[i] = (float)((i / NumPows10) *
                                    Mathf.Pow(10, i % NumPows10 + MinPow10));
            return result;
        }

        private float ParseFloat(StringBuilder value)
        {
            float result = 0;
            bool negate = false;
            int len = value.Length;
            int decimalIndex = value.Length;
            for (int i = len - 1; i >= 0; i--)
                if (value[i] == '.')
                { decimalIndex = i; break; }
            int offset = -MinPow10 + decimalIndex;
            for (int i = 0; i < decimalIndex; i++)
                if (i != decimalIndex && value[i] != '-')
                    result += pow10[(value[i] - '0') * NumPows10 + offset - i - 1];
                else if (value[i] == '-')
                    negate = true;
            for (int i = decimalIndex + 1; i < len; i++)
                if (i != decimalIndex)
                    result += pow10[(value[i] - '0') * NumPows10 + offset - i];
            if (negate)
                result = -result;
            return result;
        }

        private int IntParseFast(StringBuilder value)
        {
            int result = 0;
            for (int i = 0; i < value.Length; i++)
            {
                result = 10 * result + (value[i] - 48);
            }
            return result;
        }
    }

    public sealed class PxrVector3Int
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }

        public PxrVector3Int() { }

        public PxrVector3Int(int intX, int intY, int intZ)
        {
            x = intX;
            y = intY;
            z = intZ;
        }
    }
}

