//  Copyright © 2015-2022 Pico Technology Co., Ltd. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using PXR_Audio.Spatializer;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(PXR_Audio_Spatializer_SceneMaterial))]
public class PXR_Audio_Spatializer_SceneGeometry : MonoBehaviour
{
    [SerializeField] private bool includeChildren = false;
    [SerializeField] private bool visualizeMeshInEditor = false;
    [SerializeField] private Mesh bakedStaticMesh;

    #region EDITOR-ONLY SerializedFields

#if UNITY_EDITOR
    [SerializeField] private LayerMask meshBakingLayerMask = ~0;
    [SerializeField, HideInInspector] private string currentBakedStaticMeshAssetPath = null;
#endif

    #endregion

    public bool isStaticMeshBaked => bakedStaticMesh != null;

    private int geometryId = -1;

    public int GeometryId
    {
        get => geometryId;
    }

    private int staticGeometryID = -1;

    public int StaticGeometryId => staticGeometryID;

    private PXR_Audio_Spatializer_SceneMaterial material;

    public PXR_Audio_Spatializer_SceneMaterial Material
    {
        get
        {
            if (material == null)
            {
                material = GetComponent<PXR_Audio_Spatializer_SceneMaterial>();
            }

            return material;
        }
    }

    private MeshConfig meshConfig;
    private uint propertyMask = 0;

    private int currentContextUuid = -2;

    private void OnEnable()
    {
        if (PXR_Audio_Spatializer_Context.Instance == null) return;

        //  If geometries are added after context is initialized
        if (PXR_Audio_Spatializer_Context.Instance.UUID != currentContextUuid)
        {
            var ret = SubmitMeshToContext();
            var staticRet = SubmitStaticMeshToContext();
        }
        else
        {
            meshConfig = new MeshConfig(true, Material, transform.localToWorldMatrix);
            if (geometryId >= 0)
                PXR_Audio_Spatializer_Context.Instance.SetMeshConfig(geometryId, ref meshConfig,
                    (uint)MeshProperty.All);
            if (staticGeometryID >= 0)
                PXR_Audio_Spatializer_Context.Instance.SetMeshConfig(staticGeometryID, ref meshConfig,
                    (uint)MeshProperty.All);
        }
    }

    private void OnDisable()
    {
        if (PXR_Audio_Spatializer_Context.Instance == null) return;
        if (PXR_Audio_Spatializer_Context.Instance.UUID != currentContextUuid) return;

        meshConfig.enabled = false;
        if (geometryId >= 0)
            PXR_Audio_Spatializer_Context.Instance.SetMeshConfig(geometryId, ref meshConfig,
                (uint)MeshProperty.Enabled);
        if (staticGeometryID >= 0)
            PXR_Audio_Spatializer_Context.Instance.SetMeshConfig(staticGeometryID, ref meshConfig,
                (uint)MeshProperty.Enabled);
    }

    private void OnDestroy()
    {
        if (PXR_Audio_Spatializer_Context.Instance == null) return;
        if (PXR_Audio_Spatializer_Context.Instance.UUID != currentContextUuid) return;
        if (geometryId >= 0)
            PXR_Audio_Spatializer_Context.Instance.RemoveMesh(geometryId);
        if (staticGeometryID >= 0)
            PXR_Audio_Spatializer_Context.Instance.RemoveMesh(staticGeometryID);
    }

    private void Update()
    {
        if (PXR_Audio_Spatializer_Context.Instance == null) return;

        // //  If geometries are added after context is initialized
        // if (PXR_Audio_Spatializer_Context.Instance.UUID != currentContextUuid)
        // {
        //     var ret = SubmitMeshToContext();
        //     var staticRet = SubmitStaticMeshToContext();
        // }

        if (transform.hasChanged)
        {
            meshConfig.SetTransformMatrix4x4(transform.localToWorldMatrix);
            propertyMask |= (uint)MeshProperty.ToWorldTransform;
            transform.hasChanged = false;
        }

        if (propertyMask > 0)
        {
            if (geometryId >= 0)
                PXR_Audio_Spatializer_Context.Instance.SetMeshConfig(geometryId, ref meshConfig,
                    propertyMask);
            if (staticGeometryID >= 0)
                PXR_Audio_Spatializer_Context.Instance.SetMeshConfig(staticGeometryID, ref meshConfig,
                    propertyMask);

            propertyMask = 0;
        }
    }

    public void UpdateAbsorptionMultiband(float[] absorptions)
    {
        meshConfig.materialType = AcousticsMaterial.Custom;
        meshConfig.absorption.v0 = Material.absorption[0] = absorptions[0];
        meshConfig.absorption.v1 = Material.absorption[1] = absorptions[1];
        meshConfig.absorption.v2 = Material.absorption[2] = absorptions[2];
        meshConfig.absorption.v3 = Material.absorption[3] = absorptions[3];
        propertyMask |= (uint)MeshProperty.Material | (uint)MeshProperty.Absorption;
    }

    public void UpdateScattering(float scattering)
    {
        meshConfig.materialType = AcousticsMaterial.Custom;
        meshConfig.scattering = Material.scattering = scattering;
        propertyMask |= (uint)MeshProperty.Material | (uint)MeshProperty.Scattering;
    }

    public void UpdateTransmission(float transmission)
    {
        meshConfig.materialType = AcousticsMaterial.Custom;
        meshConfig.transmission = Material.transmission = transmission;
        propertyMask |= (uint)MeshProperty.Material | (uint)MeshProperty.Transmission;
    }

    public void UpdateMaterialType(PXR_Audio.Spatializer.AcousticsMaterial materialType)
    {
        meshConfig.materialType = materialType;
        propertyMask |= (uint)MeshProperty.Material;
    }

    private void GetAllMeshFilter(Transform transform, bool includeChildren, List<MeshFilter> meshFilterList,
        bool isStatic, LayerMask layerMask)
    {
        if (includeChildren)
        {
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var childTransform = transform.GetChild(i);
                if (childTransform.GetComponent<PXR_Audio_Spatializer_SceneGeometry>() == null)
                {
                    GetAllMeshFilter(childTransform.transform, includeChildren, meshFilterList, isStatic, layerMask);
                }
            }
        }

        //  Gather this mesh only when
        //  1. Its isStatic flag is equal to our requirement
        //  2. Its layer belongs to layerMask set 
        if (((1 << transform.gameObject.layer) & layerMask) != 0)
        {
            var meshFilterArray = transform.GetComponents<MeshFilter>();
            //  cases we don't add to mesh filter list
            //   1. meshFilter.sharedmesh == null
            //   2. meshFilter.sharedmesh.isReadable == false
            if (meshFilterArray != null)
            {
                foreach (var meshFilter in meshFilterArray)
                {
                    if (meshFilter != null && meshFilter.sharedMesh != null &&
                        (
                            (isStatic && (transform.gameObject.isStatic || !meshFilter.sharedMesh.isReadable)) ||
                            (!isStatic && (!transform.gameObject.isStatic && meshFilter.sharedMesh.isReadable))
                        ))
                    {
                        meshFilterList.Add(meshFilter);
                    }
                }
            }
        }
    }

    private static Mesh CombineMeshes(List<MeshFilter> meshFilterList, Transform rootTransform)
    {
        Mesh combinedMesh = new Mesh
        {
            name = "combined meshes",
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };

        var combinedVertices = Array.Empty<Vector3>();
        var combinedIndices = Array.Empty<int>();
        //  Accumulate combined vertices buffer size
        foreach (var meshFilter in meshFilterList)
        {
            int vertexOffset = combinedVertices.Length;
            combinedVertices = combinedVertices.Concat(meshFilter.sharedMesh.vertices).ToArray();
            int vertexSegmentEnd = combinedVertices.Length;
            var toWorld = rootTransform.worldToLocalMatrix *
                          meshFilter.transform.localToWorldMatrix;
            for (int i = vertexOffset; i < vertexSegmentEnd; ++i)
            {
                combinedVertices[i] = toWorld.MultiplyPoint3x4(combinedVertices[i]);
            }

            var trianglesStartIdx = combinedIndices.Length;
            combinedIndices = combinedIndices.Concat(meshFilter.sharedMesh.triangles).ToArray();
            var trianglesEndIdx = combinedIndices.Length;
            for (var i = trianglesStartIdx; i < trianglesEndIdx; ++i)
            {
                combinedIndices[i] += vertexOffset;
            }
        }

        combinedMesh.vertices = combinedVertices;
        combinedMesh.triangles = combinedIndices;
        combinedMesh.RecalculateNormals();

        return combinedMesh;
    }

    private static float[] FlattenVerticesBuffer(Vector3[] verticesBuffer)
    {
        float[] vertices = new float[verticesBuffer.Length * 3];
        int index = 0;
        foreach (Vector3 vertex in verticesBuffer)
        {
            vertices[index++] = vertex.x;
            vertices[index++] = vertex.y;
            vertices[index++] = vertex.z;
        }

        return vertices;
    }

    /// <summary>
    /// Submit non-static mesh of this geometry and its material into spatializer engine context
    /// </summary>
    /// <returns>Result of static mesh submission</returns>
    public PXR_Audio.Spatializer.Result SubmitMeshToContext()
    {
        // find all meshes
        var meshFilterList = new List<MeshFilter>();
        GetAllMeshFilter(transform, includeChildren, meshFilterList, false, ~0);

        //  Combine all meshes
        Mesh combinedMesh = CombineMeshes(meshFilterList, transform);

        //  flatten vertices buffer into a float array
        float[] vertices = FlattenVerticesBuffer(combinedMesh.vertices);

        meshConfig = new MeshConfig(enabled, Material, transform.localToWorldMatrix);

        //  Submit all meshes
        PXR_Audio.Spatializer.Result result = PXR_Audio_Spatializer_Context.Instance.SubmitMeshWithConfig(
            vertices, vertices.Length / 3,
            combinedMesh.triangles, combinedMesh.triangles.Length / 3,
            ref meshConfig, ref geometryId);

        if (result != Result.Success)
            Debug.LogError("Failed to submit audio mesh: " + gameObject.name + ", Error code is: " + result);
        else
            Debug.LogFormat("Submitted geometry #{0}, gameObject name is {1}", geometryId.ToString(),
                name);

        if (result == Result.Success)
            currentContextUuid = PXR_Audio_Spatializer_Context.Instance.UUID;

        return result;
    }

    /// <summary>
    /// Submit static mesh of this geometry and its material into spatializer engine context
    /// </summary>
    /// <returns>Result of static mesh submission</returns>
    public PXR_Audio.Spatializer.Result SubmitStaticMeshToContext()
    {
        PXR_Audio.Spatializer.Result result = Result.Success;
        if (bakedStaticMesh != null)
        {
            float[] tempVertices = FlattenVerticesBuffer(bakedStaticMesh.vertices);

            meshConfig = new MeshConfig(enabled, Material, transform.localToWorldMatrix);

            result = PXR_Audio_Spatializer_Context.Instance.SubmitMeshWithConfig(tempVertices,
                bakedStaticMesh.vertices.Length, bakedStaticMesh.triangles,
                bakedStaticMesh.triangles.Length / 3, ref meshConfig,
                ref staticGeometryID);

            if (result != Result.Success)
                Debug.LogError("Failed to submit static audio mesh: " + gameObject.name + ", Error code is: " + result);
            else
                Debug.LogFormat("Submitted static geometry #{0}, gameObject name is {1}", staticGeometryID.ToString(),
                    name);
        }

        if (result == Result.Success)
            currentContextUuid = PXR_Audio_Spatializer_Context.Instance.UUID;

        return result;
    }


#if UNITY_EDITOR
    public int BakeStaticMesh(LayerMask layerMask)
    {
        List<MeshFilter> meshList = new List<MeshFilter>();
        GetAllMeshFilter(transform, includeChildren, meshList, true, meshBakingLayerMask);

        SerializedObject serializedObject = new SerializedObject(this);
        if (meshList.Count == 0)
        {
            bakedStaticMesh = null;
        }
        else
        {
            bakedStaticMesh = CombineMeshes(meshList, transform);
            bakedStaticMesh.name = "baked mesh for ygg";
        }

        serializedObject.FindProperty("bakedStaticMesh").objectReferenceValue = bakedStaticMesh;

        if (bakedStaticMesh != null)
        {
            System.IO.Directory.CreateDirectory("Assets/Resources/PxrAudioSpatializerBakedSceneMeshes/");
            if (!string.IsNullOrEmpty(currentBakedStaticMeshAssetPath))
            {
                AssetDatabase.DeleteAsset(currentBakedStaticMeshAssetPath);
            }

            currentBakedStaticMeshAssetPath = "Assets/Resources/PxrAudioSpatializerBakedSceneMeshes/" + name + "_" +
                                              GetInstanceID() + "_" +
                                              System.DateTime.UtcNow.ToBinary() + ".yggmesh";
            serializedObject.FindProperty("currentBakedStaticMeshAssetPath").stringValue =
                currentBakedStaticMeshAssetPath;
            AssetDatabase.CreateAsset(bakedStaticMesh, currentBakedStaticMeshAssetPath);
            AssetDatabase.SaveAssets();
        }

        serializedObject.ApplyModifiedProperties();
        return meshList.Count;
    }

    public void ClearBakeStaticMesh()
    {
        SerializedObject serializedObject = new SerializedObject(this);
        bakedStaticMesh = null;
        serializedObject.FindProperty("bakedStaticMesh").objectReferenceValue = null;
        if (!string.IsNullOrEmpty(currentBakedStaticMeshAssetPath))
        {
            AssetDatabase.DeleteAsset(currentBakedStaticMeshAssetPath);
            currentBakedStaticMeshAssetPath = null;
            serializedObject.FindProperty("currentBakedStaticMeshAssetPath").stringValue =
                currentBakedStaticMeshAssetPath;
        }

        serializedObject.ApplyModifiedProperties();
    }
#endif

    public void OnDrawGizmos()
    {
        if (visualizeMeshInEditor)
        {
            //  Visualize non-static meshes
            // find all MeshFilter
            var meshFilterList = new List<MeshFilter>();
            GetAllMeshFilter(transform, includeChildren, meshFilterList, false, ~0);

            for (int i = 0; i < meshFilterList.Count; i++)
            {
                var mesh = meshFilterList[i].sharedMesh;
                var transform = meshFilterList[i].transform;
                Gizmos.DrawWireMesh(mesh,
                    transform.position, transform.rotation, transform.localScale);
            }

            //  Visualize baked static meshes
            if (isStaticMeshBaked)
            {
                Color colorBackUp = Gizmos.color;
                Color c;
                c.r = 0.0f;
                c.g = 0.7f;
                c.b = 0.0f;
                c.a = 1.0f;
                Gizmos.color = c;
                var gizmosMatrixBackup = Gizmos.matrix;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireMesh(bakedStaticMesh);
                Gizmos.color = colorBackUp;
                Gizmos.matrix = gizmosMatrixBackup;
            }
        }
    }
}