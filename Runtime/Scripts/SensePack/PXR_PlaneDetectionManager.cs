using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.XR.PXR
{
    public class PXR_PlaneDetectionManager : MonoBehaviour
    {
        public GameObject planePrefab;
        private Dictionary<Guid, GameObject> meshIDToGameobject;
        private Dictionary<Guid, PxrPlaneData> spatialMeshNeedingDraw;
        private Mesh mesh;
        private int objectPoolMaxSize = 100;
        private Queue<GameObject> meshObjectsPool;

        private const float frameCount = 15.0f;

        // Start is called before the first frame update
        void Start()
        {
            StartPlaneDetection();
            spatialMeshNeedingDraw = new Dictionary<Guid, PxrPlaneData>();
            meshIDToGameobject = new Dictionary<Guid, GameObject>();
            meshObjectsPool = new Queue<GameObject>();

            InitializePool();
        }

        private async void StartPlaneDetection()
        {
            await PXR_MixedReality.StartSenseDataProvider(PxrSenseDataProviderType.PlaneDetection);
        }
        
        void OnEnable()
        {
            PXR_Manager.PlaneDetectionDataUpdated += PlaneDetectionDataUpdated;
        }

        private void InitializePool()
        {
            if (planePrefab != null)
            {
                while (meshObjectsPool.Count < objectPoolMaxSize)
                {
                    GameObject obj = Instantiate(planePrefab);
                    obj.transform.SetParent(this.transform);
                    obj.SetActive(false);
                    meshObjectsPool.Enqueue(obj);
                }
            }
        }

        void PlaneDetectionDataUpdated(List<PxrPlaneData> planeDatas)
        {
            if (planePrefab != null)
            {
                for (int i = 0; i < planeDatas.Count; i++)
                {
                    switch (planeDatas[i].state)
                    {
                        case MeshChangeState.Added:
                        {
                            CreateMeshRoutine(planeDatas[i]);
                        }
                            break;
                        case MeshChangeState.Updated:
                        {
                            CreateMeshRoutine(planeDatas[i]);
                        }
                            break;
                        case MeshChangeState.Removed:
                        {
                            if (meshIDToGameobject.TryGetValue(planeDatas[i].uuid, out var go))
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

                                meshIDToGameobject.Remove(planeDatas[i].uuid);
                            }
                        }
                            break;
                        case MeshChangeState.Unchanged:
                        {
                            spatialMeshNeedingDraw.Remove(planeDatas[i].uuid);
                        }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private void CreateMeshRoutine(PxrPlaneData block)
        {
            GameObject meshGameObject = GetOrCreateGameObject(block.uuid);
            var meshFilter = meshGameObject.GetComponentInChildren<MeshFilter>();
            var meshCollider = meshGameObject.GetComponentInChildren<MeshCollider>();
            var material = meshGameObject.GetComponent<MeshRenderer>().sharedMaterial;

            if (meshFilter.mesh == null)
            {
                mesh = new Mesh();
            }
            else
            {
                mesh = meshFilter.mesh;
                mesh.Clear();
            }

            var color = GetMeshColorBySemanticLabel(block.label);
            var colorA = new Color(color.r, color.g, color.b, 0.5f);
            material.color = colorA;
            mesh.SetVertices(block.vertices);
            mesh.SetTriangles(block.indices, 0);
            meshFilter.mesh = mesh;
            if (meshCollider != null)
            {
                meshCollider.sharedMesh = mesh;
            }

            meshGameObject.transform.position = block.position;
            meshGameObject.transform.rotation = block.rotation;
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
                PxrSemanticLabel.Floor => Color.green,
                PxrSemanticLabel.Ceiling => Color.green,
                PxrSemanticLabel.Wall => Color.blue,
                PxrSemanticLabel.Door => Color.cyan,
                PxrSemanticLabel.Window => Color.magenta,
                PxrSemanticLabel.Opening => Color.yellow,
                PxrSemanticLabel.Table => Color.red,
                PxrSemanticLabel.Sofa => Color.grey,
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
                PxrSemanticLabel.Stairway => new Color(0.25f, 0f, 0.25f),
                _ => Color.white,
            };
        }
    }
}