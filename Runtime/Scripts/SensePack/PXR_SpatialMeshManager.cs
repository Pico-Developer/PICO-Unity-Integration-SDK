using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace Unity.XR.PXR
{
    [DisallowMultipleComponent]
    public class PXR_SpatialMeshManager : MonoBehaviour
    {
        public GameObject meshPrefab;
        private Dictionary<Guid, GameObject> meshIDToGameobject;
        private Dictionary<Guid, PxrSpatialMeshInfo> spatialMeshNeedingDraw;
        private Mesh mesh;
        private XRMeshSubsystem subsystem;
        private int objectPoolMaxSize = 200;
        private Queue<GameObject> meshObjectsPool;

        /// <summary>
        /// The drawing of the new spatial mesh is complete.
        /// </summary>
        public static Action<Guid, GameObject> MeshAdded;
        
        /// <summary>
        /// The drawing the updated spatial mesh is complete.
        /// </summary>
        public static Action<Guid, GameObject> MeshUpdated;
        
        /// <summary>
        /// The deletion of the disappeared spatial mesh is complete.
        /// </summary>
        public static Action<Guid> MeshRemoved;

        void Start()
        {
            spatialMeshNeedingDraw = new Dictionary<Guid, PxrSpatialMeshInfo>();
            meshIDToGameobject = new Dictionary<Guid, GameObject>();
            meshObjectsPool = new Queue<GameObject>();

            PXR_Manager.EnableVideoSeeThrough = true;
            InitializePool();
        }

        void Update()
        {
            DrawMesh();
        }

        void OnEnable()
        {
            if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
            {
                var pxrLoader = XRGeneralSettings.Instance.Manager.ActiveLoaderAs<PXR_Loader>();
                if (pxrLoader != null)
                {
                    subsystem = pxrLoader.meshSubsystem;
                    if (subsystem != null)
                    {
                        subsystem.Start();

                        if (subsystem.running)
                        {
                            PXR_Manager.SpatialMeshDataUpdated += SpatialMeshDataUpdated;
                        }
                    }
                    else
                    {
                        enabled = false;
                    }
                }
            }
        }

        void OnDisable()
        {
            if (subsystem != null && subsystem.running)
                subsystem.Stop();
        }

        private void InitializePool()
        {
            if (meshPrefab != null)
            {
                while (meshObjectsPool.Count < objectPoolMaxSize)
                {
                    GameObject obj = Instantiate(meshPrefab);
                    obj.transform.SetParent(this.transform);
                    obj.SetActive(false);
                    meshObjectsPool.Enqueue(obj);
                }
            }
        }

        private void DrawMesh()
        {
            if (meshPrefab != null)
            {
                StartCoroutine(ForeachLoopCoroutine());
            }
        }

        private IEnumerator ForeachLoopCoroutine()
        {
            int totalWork = spatialMeshNeedingDraw.Count;
            if (totalWork > 0 )
            {
                var meshList = spatialMeshNeedingDraw.Values.ToList();
                int workPerFrame = Mathf.CeilToInt(totalWork / 15f);
                int currentIndex = 0;

                while (currentIndex < totalWork)
                {
                    int workThisFrame = 0;
                    while (workThisFrame < workPerFrame && currentIndex < totalWork)
                    {
                        CreateMeshRoutine(meshList[currentIndex]);
                        currentIndex++;
                        workThisFrame++;
                    }

                    yield return null;
                }
            }
        }

        void SpatialMeshDataUpdated(List<PxrSpatialMeshInfo> meshInfos)
        {
            for (int i = 0; i < meshInfos.Count; i++)
            {
                switch (meshInfos[i].state)
                {
                    case MeshChangeState.Added:
                        {
                            spatialMeshNeedingDraw.Add(meshInfos[i].uuid, meshInfos[i]);
                        }
                        break;
                    case MeshChangeState.Updated:
                        {
                            if (!spatialMeshNeedingDraw.ContainsKey(meshInfos[i].uuid))
                            {
                                spatialMeshNeedingDraw.Add(meshInfos[i].uuid, meshInfos[i]);
                            }
                            else
                            {
                                spatialMeshNeedingDraw[meshInfos[i].uuid] = meshInfos[i];
                            }
                        }
                        break;
                    case MeshChangeState.Removed:
                        {
                            MeshRemoved?.Invoke(meshInfos[i].uuid);

                            spatialMeshNeedingDraw.Remove(meshInfos[i].uuid);
                            GameObject removedGo;
                            if (meshIDToGameobject.TryGetValue(meshInfos[i].uuid, out removedGo))
                            {
                                if (meshObjectsPool.Count < objectPoolMaxSize)
                                {
                                    removedGo.SetActive(false);
                                    meshObjectsPool.Enqueue(removedGo);
                                }
                                else
                                {
                                    Destroy(removedGo);
                                }
                                meshIDToGameobject.Remove(meshInfos[i].uuid);
                            }
                        }
                        break;
                    case MeshChangeState.Unchanged:
                        {
                            spatialMeshNeedingDraw.Remove(meshInfos[i].uuid);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void CreateMeshRoutine(PxrSpatialMeshInfo block)
        {
            GameObject meshGameObject = GetOrCreateGameObject(block.uuid);
            var meshFilter = meshGameObject.GetComponentInChildren<MeshFilter>();
            var meshCollider = meshGameObject.GetComponentInChildren<MeshCollider>();

            if (meshFilter.mesh == null)
            {
                mesh = new Mesh();
            }
            else
            {
                mesh = meshFilter.mesh;
                mesh.Clear();
            }

            Color[] normalizedColors = new Color[block.vertices.Length];
            for (int i = 0; i < block.vertices.Length; i++)
            {
                int flag = (int)block.labels[i];
                normalizedColors[i] = MeshColor[flag];
            }

            mesh.SetVertices(block.vertices);
            mesh.SetColors(normalizedColors);
            mesh.SetTriangles(block.indices, 0);
            meshFilter.mesh = mesh;
            if (meshCollider != null)
            {
                meshCollider.sharedMesh = mesh;
            }
            meshGameObject.transform.position = block.position;
            meshGameObject.transform.rotation = block.rotation;

            switch (block.state)
            {
                case MeshChangeState.Added:
                    {
                        MeshAdded?.Invoke(block.uuid, meshGameObject);
                    }
                    break;
                case MeshChangeState.Updated:
                    {
                        MeshUpdated?.Invoke(block.uuid, meshGameObject);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        GameObject CreateGameObject(Guid meshId)
        {
            GameObject meshObject = meshObjectsPool.Dequeue();
            meshObject.name = $"Mesh {meshId}";
            meshObject.SetActive(true);
            return meshObject;
        }

        GameObject GetOrCreateGameObject(Guid meshId)
        {
            GameObject go = null;
            if (!meshIDToGameobject.TryGetValue(meshId, out go))
            {
                go = CreateGameObject(meshId);
                meshIDToGameobject[meshId] = go;
            }

            return go;
        }

        private readonly Color[] MeshColor = {
        Color.black,
        Color.red,
        Color.green,
        Color.blue,
        Color.white,
        Color.yellow,
        Color.cyan,
        Color.magenta,
        Color.gray,
        Color.grey,
        new Color(0.8f,0.2f,0.6f)
        };
    }

}


