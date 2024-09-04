using Unity.XR.PXR;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PXR_ARCameraEffectManager))]
public class PXR_ARCameraEffectManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PXR_ARCameraEffectManager manager = (PXR_ARCameraEffectManager)target;
        PXR_ProjectSetting projectConfig = PXR_ProjectSetting.GetProjectConfig();
        var guiContent = new GUIContent();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        // camera effect
        guiContent.text = "Camera Effect";
        manager.enableCameraEffect = EditorGUILayout.Toggle(guiContent, manager.enableCameraEffect);

        EditorGUILayout.EndHorizontal();
        if (manager.enableCameraEffect)
        {
            EditorGUI.indentLevel++;
            guiContent.text = "Colortemp";
            manager.colortempValue = EditorGUILayout.Slider(guiContent, manager.colortempValue, -50, 50);

            guiContent.text = "Brightness";
            manager.brightnessValue = EditorGUILayout.Slider(guiContent, manager.brightnessValue, -50, 50);

            guiContent.text = "Saturation";
            manager.saturationValue = EditorGUILayout.Slider(guiContent, manager.saturationValue, -50, 50);

            guiContent.text = "Contrast";
            manager.contrastValue = EditorGUILayout.Slider(guiContent, manager.contrastValue, -50, 50);

            EditorGUILayout.LabelField("LUT");
            var textureControlRect = EditorGUILayout.GetControlRect(GUILayout.Height(100));
            manager.lutTex = (Texture2D)EditorGUI.ObjectField(new Rect(textureControlRect.x, textureControlRect.y, 100, textureControlRect.height), manager.lutTex, typeof(Texture), false);

            if (manager.lutTex != null)
            {
                // Validate texture format
                if (manager.lutTex.format != TextureFormat.RGBA32)
                {
                    Debug.LogError("Unsupported texture format! Please provide a texture in RGBA32 format.");
                    manager.lutTex = null; // Reset texture if format is incorrect
                }

                // Validate texture size
                if (manager.lutTex.width > 512 || manager.lutTex.height > 512)
                {
                    Debug.LogError("The texture size must not exceed 512x512 pixels!");
                    manager.lutTex = null; // Reset texture if size is incorrect
                }

                // Set read/write flag
                if (!manager.lutTex.isReadable)
                {
                    string assetPath = AssetDatabase.GetAssetPath(manager.lutTex);
                    TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                    if (importer != null)
                    {
                        importer.isReadable = true;
                        AssetDatabase.ImportAsset(assetPath);
                    }
                }
            }

            guiContent.text = "Lut Row";
            manager.lutRowValue = EditorGUILayout.Slider(guiContent, manager.lutRowValue, 0, 100);

            guiContent.text = "Lut Col";
            manager.lutColValue = EditorGUILayout.Slider(guiContent, manager.lutColValue, 0, 100);

            EditorGUI.indentLevel--;
        }
        
        Camera camera = manager.gameObject.GetComponent<Camera>();
        if (camera)
        {
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0, 0, 0, 0);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(projectConfig);
            EditorUtility.SetDirty(manager);
        }
        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }
    }
}