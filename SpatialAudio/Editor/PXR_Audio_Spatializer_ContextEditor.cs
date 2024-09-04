using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PXR_Audio_Spatializer_Context))]
[CanEditMultipleObjects]
public class PXR_Audio_Spatializer_ContextEditor : Editor
{
    private SerializedProperty meshBakingLayerMask;
    private bool showMeshBakingUtilsFlag = true;
    private string meshBakingUtilitiesTitle = "Static mesh baking utilities";

    private void OnEnable()
    {
        meshBakingLayerMask = serializedObject.FindProperty("meshBakingLayerMask");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //  Static mesh baking utilities
        serializedObject.Update();
        showMeshBakingUtilsFlag = EditorGUILayout.Foldout(showMeshBakingUtilsFlag, meshBakingUtilitiesTitle);
        if (showMeshBakingUtilsFlag)
        {
            EditorGUI.indentLevel++;
            
            EditorGUILayout.PropertyField(meshBakingLayerMask, new GUIContent("Layer", "Layers of game objects that will trigger mesh baking."));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * 15);
            if (GUILayout.Button(new GUIContent("Bake all", "Bake all acoustic geometries in this scene. Affected by 'Layer'")))
            {
                var start = Time.realtimeSinceStartup;

                Undo.IncrementCurrentGroup();
                var undoGroupIndex = Undo.GetCurrentGroup();

                string bakedObjectNames = "";
                int meshCount = 0;
                var sceneGeometries = FindObjectsOfType<PXR_Audio_Spatializer_SceneGeometry>();
                foreach (PXR_Audio_Spatializer_SceneGeometry geometry in sceneGeometries)
                {
                    bakedObjectNames += geometry.name + ", ";

                    Undo.RecordObject(geometry, "");
                    meshCount += geometry.BakeStaticMesh(meshBakingLayerMask.intValue);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(geometry);
                }

                Undo.SetCurrentGroupName("Bake static meshes for gameObject: " + bakedObjectNames);
                Undo.CollapseUndoOperations(undoGroupIndex);

                var durationMs = (Time.realtimeSinceStartup - start) * 1000;
                Debug.LogFormat("Baked static {0} meshes for gameObject: {1}in {2:f4} ms", meshCount, bakedObjectNames,
                    durationMs);
            }

            if (GUILayout.Button(new GUIContent("Clear all", "Clear all baked acoustic geometries in this scene.")))
            {
                Undo.IncrementCurrentGroup();
                var undoGroupIndex = Undo.GetCurrentGroup();
                string bakedObjectNames = "";

                var sceneGeometries = FindObjectsOfType<PXR_Audio_Spatializer_SceneGeometry>();
                foreach (PXR_Audio_Spatializer_SceneGeometry geometry in sceneGeometries)
                {
                    bakedObjectNames += geometry.name + ", ";

                    Undo.RecordObject(geometry, "");
                    geometry.ClearBakeStaticMesh();
                    PrefabUtility.RecordPrefabInstancePropertyModifications(geometry);
                }

                Undo.SetCurrentGroupName("Clear baked static meshes for gameObject: " + bakedObjectNames);
                Undo.CollapseUndoOperations(undoGroupIndex);

                Debug.LogFormat("Cleared baked static meshes for gameObject: {0}", bakedObjectNames);
            }
            
            GUILayout.Space(EditorGUI.indentLevel * 15 - 15);
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }
        else
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}