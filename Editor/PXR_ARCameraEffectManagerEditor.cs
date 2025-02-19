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
            var textureControlRect = EditorGUILayout.GetControlRect(GUILayout.Width(100), GUILayout.Height(100));
            manager.lutTex1 = (Texture2D)EditorGUI.ObjectField(new Rect(textureControlRect.x, textureControlRect.y, 100, textureControlRect.height), manager.lutTex1, typeof(Texture), false);
            ValidateTexture(manager.lutTex1);

            manager.lutTex2 = (Texture2D)EditorGUI.ObjectField(new Rect(textureControlRect.x + textureControlRect.width, textureControlRect.y, textureControlRect.width, textureControlRect.height), manager.lutTex2, typeof(Texture), false);
            ValidateTexture(manager.lutTex2);

            manager.lutTex3 = (Texture2D)EditorGUI.ObjectField(new Rect(textureControlRect.x + 2*textureControlRect.width, textureControlRect.y, textureControlRect.width, textureControlRect.height), manager.lutTex3, typeof(Texture), false);
            ValidateTexture(manager.lutTex3);

            manager.lutTex4 = (Texture2D)EditorGUI.ObjectField(new Rect(textureControlRect.x + 3 * textureControlRect.width, textureControlRect.y, textureControlRect.width, textureControlRect.height), manager.lutTex4, typeof(Texture), false);
            ValidateTexture(manager.lutTex4);

            manager.lutTex5 = (Texture2D)EditorGUI.ObjectField(new Rect(textureControlRect.x + 4 * textureControlRect.width, textureControlRect.y, textureControlRect.width, textureControlRect.height), manager.lutTex5, typeof(Texture), false);
            ValidateTexture(manager.lutTex5);
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

    private static void ValidateTexture(Texture2D lutTex)
    {
        if (lutTex != null)
        {
            // Validate texture format
            if (lutTex.format != TextureFormat.RGBA32)
            {
                Debug.LogError("Unsupported texture format! Please provide a texture in RGBA32 format.");
                lutTex = null; // Reset texture if format is incorrect
            }

            // Validate texture size
            if (lutTex.width > 512 || lutTex.height > 512)
            {
                Debug.LogError("The texture size must not exceed 512x512 pixels!");
                lutTex = null; // Reset texture if size is incorrect
            }

            // Set read/write flag
            if (!lutTex.isReadable)
            {
                string assetPath = AssetDatabase.GetAssetPath(lutTex);
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer != null)
                {
                    importer.isReadable = true;
                    AssetDatabase.ImportAsset(assetPath);
                }
            }
        }
    }
}