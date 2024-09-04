using System;
using PXR_Audio.Spatializer;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PXR_Audio_Spatializer_AudioSource))]
[CanEditMultipleObjects]
public class PXR_Audio_Spatializer_AudioSourceEditor : Editor
{
    private SerializedProperty sourceGainDBProperty;
    private SerializedProperty reflectionGainDBProperty;
    private SerializedProperty sourceSizeProperty;
    private SerializedProperty enableDopplerProperty;
    private SerializedProperty sourceAttenuationModeProperty;
    private SerializedProperty minAttenuationDistanceProperty;
    private SerializedProperty maxAttenuationDistanceProperty;
    private SerializedProperty directivityAlphaProperty;
    private SerializedProperty directivityOrderProperty;

    private bool showAdvancedOptions = false;
    private bool showDirectivityOptions = false;

    private void OnEnable()
    {
        sourceGainDBProperty = serializedObject.FindProperty("sourceGainDB");
        reflectionGainDBProperty = serializedObject.FindProperty("reflectionGainDB");
        sourceSizeProperty = serializedObject.FindProperty("sourceSize");
        enableDopplerProperty = serializedObject.FindProperty("enableDoppler");
        sourceAttenuationModeProperty = serializedObject.FindProperty("sourceAttenuationMode");
        minAttenuationDistanceProperty = serializedObject.FindProperty("minAttenuationDistance");
        maxAttenuationDistanceProperty = serializedObject.FindProperty("maxAttenuationDistance");
        directivityAlphaProperty = serializedObject.FindProperty("directivityAlpha");
        directivityOrderProperty = serializedObject.FindProperty("directivityOrder");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(sourceGainDBProperty,
            new GUIContent("Source Gain (dB)", "Master gain of this sound source in dBFS"));
        EditorGUILayout.PropertyField(reflectionGainDBProperty,
            new GUIContent("Reflection Gain (dB)", "Gain of the reflection sound of this sound source in dBFS"));
        EditorGUILayout.PropertyField(sourceSizeProperty,
            new GUIContent("Source Size (meters)", "Volumetric radius of this sound source in meters"));

        showAdvancedOptions = EditorGUILayout.Foldout(showAdvancedOptions, "Advanced Options");
        if (showAdvancedOptions)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(enableDopplerProperty);

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.PropertyField(sourceAttenuationModeProperty);
            EditorGUI.EndDisabledGroup();
            var attenuationMode = (SourceAttenuationMode)sourceAttenuationModeProperty.enumValueIndex;
            if (attenuationMode == SourceAttenuationMode.InverseSquare ||
                attenuationMode == SourceAttenuationMode.Customized)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(minAttenuationDistanceProperty);
                EditorGUILayout.PropertyField(maxAttenuationDistanceProperty);
                EditorGUI.indentLevel--;
            }

            showDirectivityOptions = EditorGUILayout.Foldout(showDirectivityOptions,
                new GUIContent("Directivity", "Setup radiation polar pattern of sound energy of this source."));
            if (showDirectivityOptions)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(directivityAlphaProperty, new GUIContent("Alpha"));
                EditorGUILayout.PropertyField(directivityOrderProperty, new GUIContent("Order"));
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}