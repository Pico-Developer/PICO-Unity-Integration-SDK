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
        private const float frameCount = 15.0f;
        

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
            {
                subsystem.Stop();
                PXR_Manager.SpatialMeshDataUpdated -= SpatialMeshDataUpdated;
            }
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

        void SpatialMeshDataUpdated(List<PxrSpatialMeshInfo> meshInfos)
        {
            if (meshPrefab != null)
            {
                for (int i = 0; i < meshInfos.Count; i++)
                {
                    switch (meshInfos[i].state)
                    {
                        case MeshChangeState.Added:
                            {
                                CreateMeshRoutine(meshInfos[i]);
                            }
                            break;
                        case MeshChangeState.Updated:
                            {
                                CreateMeshRoutine(meshInfos[i]);
                            }
                            break;
                        case MeshChangeState.Removed:
                            {
                                MeshRemoved?.Invoke(meshInfos[i].uuid);

                                if (meshIDToGameobject.TryGetValue(meshInfos[i].uuid, out var go))
                                {
                                    if (meshObjectsPool.Count < objectPoolMaxSize)
                                    {
                                        go.SetActive(false);
                                        meshObjectsPool.Enqueue(go);
                                    }
                                    else
                                    {
                                        Destroy(go);
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
                normalizedColors[i] = GetMeshColorBySemanticLabel(block.labels[i]);
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

        private Color GetMeshColorBySemanticLabel(PxrSemanticLabel label)
        {
            return label switch
            {
                PxrSemanticLabel.Unknown => Color.white,
                PxrSemanticLabel.Floor => Color.red,
                PxrSemanticLabel.Ceiling => Color.green,
                PxrSemanticLabel.Wall => Color.blue,
                PxrSemanticLabel.Door => Color.cyan,
                PxrSemanticLabel.Window => Color.magenta,
                PxrSemanticLabel.Opening => Color.yellow,
                PxrSemanticLabel.Table => Color.magenta,
                PxrSemanticLabel.Sofa => Color.gray,
                //Dark Red
                PxrSemanticLabel.Chair => new Color(0.5f, 0f, 0f),
                //Dark Green
                PxrSemanticLabel.Human => new Color(0f, 0.5f, 0f),
                //Dark Blue
                PxrSemanticLabel.Curtain => new Color(0f, 0f, 0.5f),
                //Orange
                PxrSemanticLabel.Cabinet => new Color(1f, 0.5f, 0f),
                //Pink
                PxrSemanticLabel.Bed => new Color(1f, 0.75f, 0.8f),
                //Purple
                PxrSemanticLabel.Plant => new Color(0.5f, 0f, 0.5f),
                //Brown
                PxrSemanticLabel.Screen => new Color(0.5f, 0.25f, 0f),
                //Olive Green
                PxrSemanticLabel.Refrigerator => new Color(0.5f, 0.5f, 0f),
                //Gold
                PxrSemanticLabel.WashingMachine => new Color(1f, 0.84f, 0f),
                //Silver
                PxrSemanticLabel.AirConditioner => new Color(0.75f, 0.75f, 0.75f),
                //Mint Green
                PxrSemanticLabel.Lamp => new Color(0.5f, 1f, 0.5f),
                //Dark Purple
                PxrSemanticLabel.WallArt => new Color(0.5f, 0f, 0.25f),
                _ => Color.white,
            };
        }
    }

}


