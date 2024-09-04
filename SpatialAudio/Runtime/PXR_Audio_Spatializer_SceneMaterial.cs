//  Copyright © 2015-2022 Pico Technology Co., Ltd. All Rights Reserved.

using UnityEngine;

public class PXR_Audio_Spatializer_SceneMaterial : MonoBehaviour
{
    [Tooltip("Acoustic material preset corresponding to real-world material types. " +
             "Absorption, scattering, and transmission will be automatically set based on the physical nature of the selected material.\n" +
             "Deviating acoustic parameters away from preset values will automatically set this field to 'Custom'")]
    [SerializeField]
    public PXR_Audio.Spatializer.AcousticsMaterial
        materialPreset = PXR_Audio.Spatializer.AcousticsMaterial.AcousticTile;

    private PXR_Audio.Spatializer.AcousticsMaterial lastMaterialPreset =
        PXR_Audio.Spatializer.AcousticsMaterial.AcousticTile;

    [SerializeField] [Range(0.0f, 1.0f)]
    public float[] absorption = new float[4];

    [Tooltip("Ratio of sound energy get scattered by each reflection.\n" +
             "  - Low scattering will result in a more echoic sound\n" +
             "  - high scattering will result in a more reverberant sound")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    public float scattering = 0.0f;

    [Tooltip("Ratio of sound energy get transmitted through this material.")] [SerializeField] [Range(0.0f, 1.0f)]
    public float transmission = 0.0f;

    private float[] absorptionForValidation = new float[4];
    private float scatteringForValidation = 0.0f;
    private float transmissionForValidation = 0.0f;

    private static PXR_Audio_Spatializer_Context spatialAudioContextRef;

    private void OnValidate()
    {
        if (spatialAudioContextRef == null)
            spatialAudioContextRef = FindObjectOfType<PXR_Audio_Spatializer_Context>();
        if (lastMaterialPreset != materialPreset) // material_preset is changed
        {
            if (materialPreset != PXR_Audio.Spatializer.AcousticsMaterial.Custom)
            {
                if (spatialAudioContextRef != null)
                {
                    spatialAudioContextRef.PXR_Audio_Spatializer_Api.GetAbsorptionFactor(materialPreset,
                        absorption);
                    spatialAudioContextRef.PXR_Audio_Spatializer_Api.GetScatteringFactor(materialPreset,
                        ref scattering);
                    spatialAudioContextRef.PXR_Audio_Spatializer_Api.GetTransmissionFactor(
                        materialPreset, ref transmission);
                    lastMaterialPreset = materialPreset;
                }
            }
            else
            {
                lastMaterialPreset = materialPreset;
            }
        }
        else if (materialPreset != PXR_Audio.Spatializer.AcousticsMaterial.Custom &&
                 spatialAudioContextRef !=
                 null) // material_preset is not changed, but acoustic properties are changed manually
        {
            //  Check if actual material parameters are different from current materialPreset
            spatialAudioContextRef.PXR_Audio_Spatializer_Api.GetAbsorptionFactor(materialPreset,
                absorptionForValidation);
            spatialAudioContextRef.PXR_Audio_Spatializer_Api.GetScatteringFactor(materialPreset,
                ref scatteringForValidation);
            spatialAudioContextRef.PXR_Audio_Spatializer_Api.GetTransmissionFactor(materialPreset,
                ref transmissionForValidation);

            if (Mathf.Abs(absorption[0] - absorptionForValidation[0]) > float.Epsilon ||
                Mathf.Abs(absorption[1] - absorptionForValidation[1]) > float.Epsilon ||
                Mathf.Abs(absorption[2] - absorptionForValidation[2]) > float.Epsilon ||
                Mathf.Abs(absorption[3] - absorptionForValidation[3]) > float.Epsilon ||
                Mathf.Abs(scattering - scatteringForValidation) > float.Epsilon ||
                Mathf.Abs(transmission - transmissionForValidation) > float.Epsilon)
            {
                materialPreset = PXR_Audio.Spatializer.AcousticsMaterial.Custom;
                lastMaterialPreset = PXR_Audio.Spatializer.AcousticsMaterial.Custom;
            }
        }
    }
}