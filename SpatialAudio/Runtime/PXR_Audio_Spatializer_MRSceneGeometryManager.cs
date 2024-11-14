using System;
using PXR_Audio.Spatializer;
using UnityEngine;
using Unity.XR.PXR;
using UnityEngine.XR;

public class PXR_Audio_Spatializer_MRSceneGeometryManager : MonoBehaviour
{
    public bool meshUpdate = true;
    public bool ignoreMeshLabel = false;
    private static PXR_Audio_Spatializer_MRSceneGeometryManager _instance = null;
    
    void OnEnable()
    {
        PXR_SpatialMeshManager.MeshAdded += AddAcousticSceneGeometries;
        if (meshUpdate)
            PXR_SpatialMeshManager.MeshUpdated += UpdateAcousticSceneGeometries;
        Debug.Log("PXR_Audio_Spatializer_MRSceneGeometryManager attached");
    }

    void OnDisable()
    {
        PXR_SpatialMeshManager.MeshAdded -= AddAcousticSceneGeometries;
        if (meshUpdate)
            PXR_SpatialMeshManager.MeshUpdated -= UpdateAcousticSceneGeometries;
        Debug.Log("PXR_Audio_Spatializer_MRSceneGeometryManager detached");
    }

    private void AddAcousticSceneGeometries(Guid guid, GameObject o)
    {
        //  Add acoustic mesh
        var acousticMesh = o.GetComponent<PXR_Audio_Spatializer_SceneGeometry>();
        var acousticMaterial = o.GetComponent<PXR_Audio_Spatializer_SceneMaterial>();

        if (acousticMesh && acousticMaterial)
        {
            UpdateAcousticSceneGeometries(guid, o);
        }
        else if (PXR_Plugin.MixedReality.SpatialMeshData.TryGetValue(guid, out var spatialMesh))
        {
            var acousticMeshNew = o.AddComponent<PXR_Audio_Spatializer_SceneGeometry>();
            var acousticMaterialNew = o.GetComponent<PXR_Audio_Spatializer_SceneMaterial>();
            UpdateMaterialBasedOnLabel(spatialMesh.labels[0], ref acousticMaterialNew);
            acousticMeshNew.UpdateMaterialType(acousticMaterialNew.materialPreset);
            acousticMeshNew.UpdateAbsorptionMultiband(acousticMaterialNew.absorption);
            acousticMeshNew.UpdateScattering(acousticMaterialNew.scattering);
            acousticMeshNew.UpdateTransmission(acousticMaterialNew.transmission);
        }
    }

    private void UpdateAcousticSceneGeometries(Guid guid, GameObject o)
    {
        var acousticMesh = o.GetComponent<PXR_Audio_Spatializer_SceneGeometry>();
        var acousticMaterial = o.GetComponent<PXR_Audio_Spatializer_SceneMaterial>();
        if (PXR_Plugin.MixedReality.SpatialMeshData.TryGetValue(guid, out var spatialMesh) 
            && acousticMesh && acousticMaterial)
        {
            UpdateMaterialBasedOnLabel(spatialMesh.labels[0], ref acousticMaterial);
            acousticMesh.UpdateMeshInContext();
        }
    }

    private void UpdateMaterialBasedOnLabel(PxrSemanticLabel label, ref PXR_Audio_Spatializer_SceneMaterial material)
    {
        if (ignoreMeshLabel)
        {
            material.materialPreset = AcousticsMaterial.Custom;
            material.absorption[0] = 0;
            material.absorption[1] = 0;
            material.absorption[2] = 0;
            material.absorption[3] = 0;
            material.scattering = 0.2f;
            material.transmission = 0.5f;
        }
        else
        {
            AcousticsMaterial acousticsMaterial = AcousticsMaterial.AcousticTile;
            switch (label)
            {
                case PxrSemanticLabel.Floor:
                    acousticsMaterial = AcousticsMaterial.WoodFloor;
                    break;
                case PxrSemanticLabel.Ceiling:
                case PxrSemanticLabel.Wall:
                    acousticsMaterial = AcousticsMaterial.PlasterOnConcreteBlock;
                    break;
                case PxrSemanticLabel.Door:
                    acousticsMaterial = AcousticsMaterial.WoodThin;
                    break;
                case PxrSemanticLabel.Window:
                    acousticsMaterial = AcousticsMaterial.Glass;
                    break;
                case PxrSemanticLabel.Opening:
                    material.materialPreset = AcousticsMaterial.Custom;
                    material.absorption[0] = 1;
                    material.absorption[1] = 1;
                    material.absorption[2] = 1;
                    material.absorption[3] = 1;
                    material.scattering = 0;
                    material.transmission = 1;
                    break;
                case PxrSemanticLabel.Table:
                    acousticsMaterial = AcousticsMaterial.WoodThick;
                    break;
                case PxrSemanticLabel.Sofa:
                    acousticsMaterial = AcousticsMaterial.AcousticTile;
                    break;
                case PxrSemanticLabel.Chair:
                    acousticsMaterial = AcousticsMaterial.WoodThin;
                    break;
                case PxrSemanticLabel.Human:
                    acousticsMaterial = AcousticsMaterial.AcousticTile;
                    break;
                case PxrSemanticLabel.VirtualWall:
                    acousticsMaterial = AcousticsMaterial.AcousticTile;
                    break;
                case PxrSemanticLabel.Curtain:
                    acousticsMaterial = AcousticsMaterial.Curtain;
                    break;
                case PxrSemanticLabel.Cabinet:
                    acousticsMaterial = AcousticsMaterial.WoodThick;
                    break;
                case PxrSemanticLabel.Bed:
                    acousticsMaterial = AcousticsMaterial.AcousticTile;
                    break;
                case PxrSemanticLabel.Plant:
                    acousticsMaterial = AcousticsMaterial.Foliage;
                    break;
                case PxrSemanticLabel.Screen:
                    acousticsMaterial = AcousticsMaterial.Glass;
                    break;
                case PxrSemanticLabel.Refrigerator:
                case PxrSemanticLabel.WashingMachine:
                case PxrSemanticLabel.AirConditioner:
                    acousticsMaterial = AcousticsMaterial.PlasterOnConcreteBlock;
                    break;
                case PxrSemanticLabel.Lamp:
                    acousticsMaterial = AcousticsMaterial.WoodThin;
                    break;
                case PxrSemanticLabel.WallArt:
                    acousticsMaterial = AcousticsMaterial.PlasterOnConcreteBlock;
                    break;
                default:
                    acousticsMaterial = AcousticsMaterial.AcousticTile;
                    break;
            }
            
            UpdateMaterialBasedOnAcousticLabel(acousticsMaterial, ref material);
        }
    }

    private void UpdateMaterialBasedOnAcousticLabel(AcousticsMaterial acousticLabel,
        ref PXR_Audio_Spatializer_SceneMaterial material)
    {
        if (acousticLabel == AcousticsMaterial.Custom)
            return;
        if (PXR_Audio_Spatializer_Context.Instance == null)
            return;
        material.materialPreset = AcousticsMaterial.Custom;
        PXR_Audio_Spatializer_Context.Instance.GetAbsorptionFactors(acousticLabel, material.absorption);
        PXR_Audio_Spatializer_Context.Instance.GetScatteringFactors(acousticLabel, ref material.scattering);
        PXR_Audio_Spatializer_Context.Instance.GetTransmissionFactors(acousticLabel, ref material.transmission);
        material.transmission = Math.Min(material.transmission + 0.5f, 1.0f);
    }
}