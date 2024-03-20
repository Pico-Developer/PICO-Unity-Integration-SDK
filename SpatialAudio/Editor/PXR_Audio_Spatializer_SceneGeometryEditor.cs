using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PXR_Audio_Spatializer_SceneGeometry))]
[CanEditMultipleObjects]
public class PXR_Audio_Spatializer_SceneGeometryEditor : Editor
{
    private SerializedProperty includeChildren;
    private SerializedProperty visualizeMeshInEditor;
    private SerializedProperty bakedStaticMesh;
    private SerializedProperty meshBakingLayerMask;
    private bool showMeshBakingUtilsFlag = true;
    private string showMeshBakingUtilities = "Static mesh baking utilities";

    void OnEnable()
    {
        includeChildren = serializedObject.FindProperty("includeChildren");
        visualizeMeshInEditor = serializedObject.FindProperty("visualizeMeshInEditor");
        bakedStaticMesh = serializedObject.FindProperty("bakedStaticMesh");
        meshBakingLayerMask = serializedObject.FindProperty("meshBakingLayerMask");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //  Boolean flags
        EditorGUILayout.PropertyField(includeChildren);
        EditorGUILayout.PropertyField(visualizeMeshInEditor);

        //  Static mesh baking utilities
        showMeshBakingUtilsFlag = EditorGUILayout.Foldout(showMeshBakingUtilsFlag, showMeshBakingUtilities);
        if (showMeshBakingUtilsFlag)
        {
            EditorGUI.indentLevel++;
            
            EditorGUILayout.PropertyField(meshBakingLayerMask);
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * 15);
            if (GUILayout.Button("Bake"))
            {
                var start = Time.realtimeSinceStartup;

                Undo.IncrementCurrentGroup();
                var undoGroupIndex = Undo.GetCurrentGroup();

                string bakedObjectNames = "";
                int meshCount = 0;
                foreach (var t in targets)
                {
                    PXR_Audio_Spatializer_SceneGeometry geometry = (PXR_Audio_Spatializer_SceneGeometry)t;
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

            if (GUILayout.Button("Clear"))
            {
                Undo.IncrementCurrentGroup();
                var undoGroupIndex = Undo.GetCurrentGroup();
                string bakedObjectNames = "";
                foreach (var t in targets)
                {
                    PXR_Audio_Spatializer_SceneGeometry geometry = (PXR_Audio_Spatializer_SceneGeometry)t;
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

            serializedObject.Update();
            EditorGUILayout.PropertyField(bakedStaticMesh);
            serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel--;
        }
        else
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}