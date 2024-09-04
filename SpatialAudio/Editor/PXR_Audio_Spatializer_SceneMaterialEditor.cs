using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PXR_Audio_Spatializer_SceneMaterial))]
[CanEditMultipleObjects]
public class PXR_Audio_Spatializer_SceneMaterialEditor : Editor
{
    private SerializedProperty materialPresetProperty;
    private SerializedProperty absorptionProperty;
    private SerializedProperty scatteringProperty;
    private SerializedProperty transmissionProperty;

    private void OnEnable()
    {
        materialPresetProperty = serializedObject.FindProperty("materialPreset");
        absorptionProperty = serializedObject.FindProperty("absorption");
        scatteringProperty = serializedObject.FindProperty("scattering");
        transmissionProperty = serializedObject.FindProperty("transmission");
    }

    private static int[] bandCenters = { 1000, 2000, 4000, 8000 };

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(materialPresetProperty);
        
        for (int i = 0; i < absorptionProperty.arraySize; i++)
        {
            SerializedProperty elementProperty = absorptionProperty.GetArrayElementAtIndex(i);
            string elementName = $"Absorption band {bandCenters[i]} Hz";
            string tooltips = $"Ratio of sound energy absorbed by each reflection for band {bandCenters[i]} Hz";
            EditorGUILayout.PropertyField(elementProperty, new GUIContent(elementName, tooltips));
        }

        EditorGUILayout.PropertyField(scatteringProperty);
        EditorGUILayout.PropertyField(transmissionProperty);
        
        serializedObject.ApplyModifiedProperties();
    }
}