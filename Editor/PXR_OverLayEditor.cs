/*******************************************************************************
Copyright © 2015-2022 PICO Technology Co., Ltd.All rights reserved.  

NOTICE：All information contained herein is, and remains the property of 
PICO Technology Co., Ltd. The intellectual and technical concepts 
contained herein are proprietary to PICO Technology Co., Ltd. and may be 
covered by patents, patents in process, and are protected by trade secret or 
copyright law. Dissemination of this information or reproduction of this 
material is strictly forbidden unless prior written permission is obtained from
PICO Technology Co., Ltd. 
*******************************************************************************/

using UnityEditor;
using UnityEngine;


namespace Unity.XR.PXR.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PXR_OverLay))]
    public class PXR_OverLayEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var guiContent = new GUIContent();
            foreach (PXR_OverLay overlayTarget in targets)
            {
                EditorGUILayout.LabelField("Overlay Settings", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical("frameBox");
                guiContent.text = "Type";
                overlayTarget.overlayType = (PXR_OverLay.OverlayType)EditorGUILayout.EnumPopup(guiContent, overlayTarget.overlayType);
                guiContent.text = "Shape";
                overlayTarget.overlayShape = (PXR_OverLay.OverlayShape)EditorGUILayout.EnumPopup(guiContent, overlayTarget.overlayShape);
                guiContent.text = "Depth";
                overlayTarget.layerDepth = EditorGUILayout.IntField(guiContent, overlayTarget.layerDepth);

                EditorGUILayout.EndVertical();

                guiContent.text = "Clones";
                overlayTarget.isClones = EditorGUILayout.Toggle(guiContent, overlayTarget.isClones);
                if (overlayTarget.isClones)
                {
                    overlayTarget.originalOverLay = EditorGUILayout.ObjectField("Original OverLay", overlayTarget.originalOverLay, typeof(PXR_OverLay), true) as PXR_OverLay;

                    GUIStyle firstLevelStyle = new GUIStyle(GUI.skin.label);
                    firstLevelStyle.alignment = TextAnchor.UpperLeft;
                    firstLevelStyle.fontStyle = FontStyle.Bold;
                    firstLevelStyle.fontSize = 12;
                    firstLevelStyle.wordWrap = true;
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField("Note:", firstLevelStyle);
                    EditorGUILayout.LabelField("Original OverLay cannot be empty or itself!");
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("Overlay Textures", EditorStyles.boldLabel);
                    guiContent.text = "Texture Type";
                    overlayTarget.textureType = (PXR_OverLay.TextureType)EditorGUILayout.EnumPopup(guiContent, overlayTarget.textureType);
                    EditorGUILayout.Separator();

                    if (overlayTarget.overlayShape == PXR_OverLay.OverlayShape.BlurredQuad)
                    {
                        overlayTarget.textureType = PXR_OverLay.TextureType.ExternalSurface;
                    }

                    if (overlayTarget.textureType == PXR_OverLay.TextureType.ExternalSurface)
                    {
                        overlayTarget.isExternalAndroidSurface = true;
                        overlayTarget.isDynamic = false;
                    }
                    else if (overlayTarget.textureType == PXR_OverLay.TextureType.DynamicTexture)
                    {
                        overlayTarget.isExternalAndroidSurface = false;
                        overlayTarget.isDynamic = true;
                    }
                    else
                    {
                        overlayTarget.isExternalAndroidSurface = false;
                        overlayTarget.isDynamic = false;
                    }

                    if (overlayTarget.isExternalAndroidSurface)
                    {
                        EditorGUILayout.BeginVertical("frameBox");
                        guiContent.text = "DRM";
                        overlayTarget.isExternalAndroidSurfaceDRM = EditorGUILayout.Toggle(guiContent, overlayTarget.isExternalAndroidSurfaceDRM);

                        guiContent.text = "3D Surface Type";
                        guiContent.tooltip = "The functions of '3D Surface Type' and 'Source Rects' are similar, and only one of them can be used. ";
                        overlayTarget.externalAndroidSurface3DType = (PXR_OverLay.Surface3DType)EditorGUILayout.EnumPopup(guiContent, overlayTarget.externalAndroidSurface3DType);
                        EditorGUILayout.EndVertical();

                        if (overlayTarget.overlayShape == PXR_OverLay.OverlayShape.BlurredQuad)
                        {
                            EditorGUILayout.LabelField("Blurred Quad");
                            EditorGUILayout.BeginVertical("frameBox");
                            guiContent.text = "Mode";
                            overlayTarget.blurredQuadMode = (PXR_OverLay.BlurredQuadMode)EditorGUILayout.EnumPopup(guiContent, overlayTarget.blurredQuadMode);

                            guiContent.text = "Scale";
                            overlayTarget.blurredQuadScale = EditorGUILayout.FloatField(guiContent, Mathf.Abs(overlayTarget.blurredQuadScale));

                            guiContent.text = "Shift";
                            overlayTarget.blurredQuadShift = EditorGUILayout.Slider(guiContent, overlayTarget.blurredQuadShift, -1, 1);

                            guiContent.text = "FOV";
                            overlayTarget.blurredQuadFOV = EditorGUILayout.Slider(guiContent, overlayTarget.blurredQuadFOV, 0, 180f);

                            guiContent.text = "IPD";
                            overlayTarget.blurredQuadIPD = EditorGUILayout.Slider(guiContent, overlayTarget.blurredQuadIPD, 0.01f, 1.0f);

                            EditorGUILayout.EndVertical();
                        }
                        guiContent.tooltip = "";
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Texture");
                        EditorGUILayout.BeginVertical("frameBox");

                        var labelControlRect = EditorGUILayout.GetControlRect();
                        EditorGUI.LabelField(new Rect(labelControlRect.x, labelControlRect.y, labelControlRect.width / 2, labelControlRect.height), new GUIContent("Left", "Texture used for the left eye"));
                        EditorGUI.LabelField(new Rect(labelControlRect.x + labelControlRect.width / 2, labelControlRect.y, labelControlRect.width / 2, labelControlRect.height), new GUIContent("Right", "Texture used for the right eye"));

                        var textureControlRect = EditorGUILayout.GetControlRect(GUILayout.Height(64));
                        overlayTarget.layerTextures[0] = (Texture)EditorGUI.ObjectField(new Rect(textureControlRect.x, textureControlRect.y, 64, textureControlRect.height), overlayTarget.layerTextures[0], typeof(Texture), false);
                        overlayTarget.layerTextures[1] = (Texture)EditorGUI.ObjectField(new Rect(textureControlRect.x + textureControlRect.width / 2, textureControlRect.y, 64, textureControlRect.height), overlayTarget.layerTextures[1] != null ? overlayTarget.layerTextures[1] : overlayTarget.layerTextures[0], typeof(Texture), false);

                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.Separator();

                    if (overlayTarget.overlayShape == PXR_OverLay.OverlayShape.Equirect ||
                        overlayTarget.overlayShape == PXR_OverLay.OverlayShape.Fisheye)
                    {
                        guiContent.text = "Radius";
                        overlayTarget.radius = EditorGUILayout.FloatField(guiContent, Mathf.Abs(overlayTarget.radius));
                    }
                }

                if (overlayTarget.overlayShape == PXR_OverLay.OverlayShape.Quad ||
                    overlayTarget.overlayShape == PXR_OverLay.OverlayShape.Cylinder ||
                    overlayTarget.overlayShape == PXR_OverLay.OverlayShape.Equirect ||
                    overlayTarget.overlayShape == PXR_OverLay.OverlayShape.Eac ||
                    overlayTarget.overlayShape == PXR_OverLay.OverlayShape.Fisheye)
                {
                    guiContent.text = "Texture Rects";
                    overlayTarget.useImageRect = EditorGUILayout.Toggle(guiContent, overlayTarget.useImageRect);
                    if (overlayTarget.useImageRect)
                    {
                        EditorGUI.indentLevel++;
                        if (PXR_OverLay.Surface3DType.Single != overlayTarget.externalAndroidSurface3DType)
                        {
                            GUI.enabled = false;
                        }
                        guiContent.text = "Source Rects";
                        guiContent.tooltip = "The functions of '3D Surface Type' and 'Source Rects' are similar, and only one of them can be used. ";
                        overlayTarget.textureRect = (PXR_OverLay.TextureRect)EditorGUILayout.EnumPopup(guiContent, overlayTarget.textureRect);

                        if (PXR_OverLay.Surface3DType.Single == overlayTarget.externalAndroidSurface3DType)
                        {
                            if (overlayTarget.textureRect == PXR_OverLay.TextureRect.Custom)
                            {
                                EditorGUILayout.BeginVertical("frameBox");

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Left Rect");
                                EditorGUILayout.LabelField("Right Rect");
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                overlayTarget.srcRectLeft = ClampRect(EditorGUILayout.RectField(overlayTarget.srcRectLeft));
                                EditorGUILayout.Space(15);
                                guiContent.text = "Right";
                                overlayTarget.srcRectRight = ClampRect(EditorGUILayout.RectField(overlayTarget.srcRectRight));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.EndVertical();
                                EditorGUILayout.Space();
                            }
                            else if (overlayTarget.textureRect == PXR_OverLay.TextureRect.MonoScopic)
                            {
                                overlayTarget.srcRectLeft = new Rect(0, 0, 1, 1);
                                overlayTarget.srcRectRight = new Rect(0, 0, 1, 1);
                            }
                            else if (overlayTarget.textureRect == PXR_OverLay.TextureRect.StereoScopic)
                            {
                                overlayTarget.srcRectLeft = new Rect(0, 0, 0.5f, 1);
                                overlayTarget.srcRectRight = new Rect(0.5f, 0, 0.5f, 1);
                            }
                        }
                        else
                        {
                            overlayTarget.textureRect = PXR_OverLay.TextureRect.MonoScopic;
                            overlayTarget.srcRectLeft = new Rect(0, 0, 1, 1);
                            overlayTarget.srcRectRight = new Rect(0, 0, 1, 1);
                        }

                        guiContent.tooltip = "";
                        GUI.enabled = true;
                        if (overlayTarget.overlayShape == PXR_OverLay.OverlayShape.Quad ||
                            overlayTarget.overlayShape == PXR_OverLay.OverlayShape.Equirect ||
                            overlayTarget.overlayShape == PXR_OverLay.OverlayShape.Fisheye)
                        {
                            guiContent.text = "Destination Rects";
                            overlayTarget.destinationRect = (PXR_OverLay.DestinationRect)EditorGUILayout.EnumPopup(guiContent, overlayTarget.destinationRect);

                            if (overlayTarget.destinationRect == PXR_OverLay.DestinationRect.Custom)
                            {
                                EditorGUILayout.BeginVertical("frameBox");

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Left Rect");
                                EditorGUILayout.LabelField("Right Rect");
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                overlayTarget.dstRectLeft = ClampRect(EditorGUILayout.RectField(overlayTarget.dstRectLeft));
                                EditorGUILayout.Space(15);
                                guiContent.text = "Right";
                                overlayTarget.dstRectRight = ClampRect(EditorGUILayout.RectField(overlayTarget.dstRectRight));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.EndVertical();
                                EditorGUILayout.Space();
                            }
                            else
                            {
                                GUI.enabled = false;
                                overlayTarget.dstRectLeft = new Rect(0, 0, 1, 1);
                                overlayTarget.dstRectRight = new Rect(0, 0, 1, 1);
                                GUI.enabled = true;
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                }

                guiContent.text = "Layer Blend";
                overlayTarget.useLayerBlend = EditorGUILayout.Toggle(guiContent, overlayTarget.useLayerBlend);
                if (overlayTarget.useLayerBlend)
                {
                    EditorGUILayout.BeginVertical("frameBox");
                    guiContent.text = "Src Color";
                    overlayTarget.srcColor = (PxrBlendFactor)EditorGUILayout.EnumPopup(guiContent, overlayTarget.srcColor);
                    guiContent.text = "Dst Color";
                    overlayTarget.dstColor = (PxrBlendFactor)EditorGUILayout.EnumPopup(guiContent, overlayTarget.dstColor);
                    guiContent.text = "Src Alpha";
                    overlayTarget.srcAlpha = (PxrBlendFactor)EditorGUILayout.EnumPopup(guiContent, overlayTarget.srcAlpha);
                    guiContent.text = "Dst Alpha";
                    overlayTarget.dstAlpha = (PxrBlendFactor)EditorGUILayout.EnumPopup(guiContent, overlayTarget.dstAlpha);

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Separator();
                if (overlayTarget.overlayShape == PXR_OverLay.OverlayShape.Eac)
                {
                    guiContent.text = "Model Type";
                    overlayTarget.eacModelType = (PXR_OverLay.EACModelType)EditorGUILayout.EnumPopup(guiContent, overlayTarget.eacModelType);

                    if (PXR_OverLay.EACModelType.Eac360ViewPort == overlayTarget.eacModelType ||
                        PXR_OverLay.EACModelType.Eac180ViewPort == overlayTarget.eacModelType)
                    {

                        guiContent.text = "Offset Pos Left";
                        Vector3 offsetPosLeft = EditorGUILayout.Vector3Field(guiContent, overlayTarget.offsetPosLeft);


                        guiContent.text = "Offset Pos Right";
                        Vector3 offsetPosRight = EditorGUILayout.Vector3Field(guiContent, overlayTarget.offsetPosRight);


                        guiContent.text = "Offset Rot Left";
                        Vector4 offsetRotLeft = EditorGUILayout.Vector4Field(guiContent, overlayTarget.offsetRotLeft);


                        guiContent.text = "Offset Rot Right";
                        Vector4 offsetRotRight = EditorGUILayout.Vector4Field(guiContent, overlayTarget.offsetRotRight);

                        overlayTarget.SetEACOffsetPosAndRot(offsetPosLeft, offsetPosRight, offsetRotLeft, offsetRotRight);
                    }

                    guiContent.text = "Overlap Factor";
                    overlayTarget.overlapFactor = EditorGUILayout.FloatField(guiContent, overlayTarget.overlapFactor);
                    //overlayTarget.SetEACFactor(overlapFactor);
                }

                guiContent.text = "Override Color Scale";
                overlayTarget.overrideColorScaleAndOffset = EditorGUILayout.Toggle(guiContent, overlayTarget.overrideColorScaleAndOffset);
                if (overlayTarget.overrideColorScaleAndOffset)
                {
                    EditorGUILayout.BeginVertical("frameBox");

                    guiContent.text = "Scale";
                    Vector4 colorScale = EditorGUILayout.Vector4Field(guiContent, overlayTarget.colorScale);

                    guiContent.text = "Offset";
                    Vector4 colorOffset = EditorGUILayout.Vector4Field(guiContent, overlayTarget.colorOffset);
                    overlayTarget.SetLayerColorScaleAndOffset(colorScale, colorOffset);

                    EditorGUILayout.EndVertical();
                }

                guiContent.text = "isAlphaPremultiplied";
                overlayTarget.isPremultipliedAlpha = EditorGUILayout.Toggle(guiContent, overlayTarget.isPremultipliedAlpha);

                //Super Resolution
                var superresolutionContent = new GUIContent();
                superresolutionContent.text = "Super Resolution";
                superresolutionContent.tooltip = "Single pass spatial aware upscaling technique.\n\nThis can't be used with Sharpening. \nAlso can't be used along with subsample feature due to unsupported texture format. \n\nThis effect won't work properly under low resolutions when Adaptive Resolution is also enabled.";
                overlayTarget.superResolution = EditorGUILayout.Toggle(superresolutionContent, overlayTarget.superResolution);

                //Supersampling
                var supersamplingContent = new GUIContent();
                supersamplingContent.text = "Supersampling Mode";
                supersamplingContent.tooltip = "Normal: Normal Quality \n\nQuality: Higher Quality, higher GPU usage\n\nThis effect won't work properly under low resolutions when Adaptive Resolution or Sharpening is also enabled.\n\nThis can't be used with Super Resolution or Sharpening. It will be automatically disabled when you enable Super Resolution or Sharpening. \nAlso can't be used along with subsample feature due to unsupported texture format";

                var supersamplingEnhanceContent = new GUIContent();
                supersamplingEnhanceContent.text = "Supersampling Enhance Mode";
                supersamplingEnhanceContent.tooltip = "None: Full screen will be super sampled\n\nFixed Foveated: Only the central fixation point will be sharpened\n\nSelf Adaptive: Only when contrast between the current pixel and the surrounding pixels exceeds a certain threshold will be sharpened.\n\nThis menu will be only enabled while Sharpening (either Normal or Quality) is enabled.";

                if (overlayTarget.superResolution)
                {
                    GUI.enabled = false;
                    overlayTarget.supersamplingMode = SuperSamplingMode.None;
                    overlayTarget.supersamplingEnhance = SuperSamplingEnhance.None;
                }
                else
                {
                    GUI.enabled = true;
                }

                overlayTarget.supersamplingMode = (SuperSamplingMode)EditorGUILayout.EnumPopup(supersamplingContent, overlayTarget.supersamplingMode);
                if (overlayTarget.supersamplingMode == SuperSamplingMode.None)
                {
                    overlayTarget.supersamplingEnhance = SuperSamplingEnhance.None;
                }
                else
                {
                    EditorGUI.indentLevel++;
                    overlayTarget.supersamplingEnhance = (SuperSamplingEnhance)EditorGUILayout.EnumPopup(supersamplingEnhanceContent, overlayTarget.supersamplingEnhance);
                    EditorGUI.indentLevel--;
                }

                if (overlayTarget.supersamplingMode != SuperSamplingMode.None)
                {
                    if (overlayTarget.supersamplingMode == SuperSamplingMode.Normal)
                    {
                        overlayTarget.normalSupersampling = true;
                        overlayTarget.qualitySupersampling = false;
                    }
                    else
                    {
                        overlayTarget.normalSupersampling = false;
                        overlayTarget.qualitySupersampling = true;
                    }

                    if (overlayTarget.supersamplingEnhance == SuperSamplingEnhance.FixedFoveated)
                    {
                        overlayTarget.fixedFoveatedSupersampling = true;
                    }
                    else
                    {
                        overlayTarget.fixedFoveatedSupersampling = false;
                    }
                }
                else
                {
                    overlayTarget.normalSupersampling = false;
                    overlayTarget.qualitySupersampling = false;
                    overlayTarget.fixedFoveatedSupersampling = false;
                }

                //Sharpening
                var sharpeningContent = new GUIContent();
                sharpeningContent.text = "Sharpening Mode";
                sharpeningContent.tooltip = "Normal: Normal Quality \n\nQuality: Higher Quality, higher GPU usage\n\nThis effect won't work properly under low resolutions when Adaptive Resolution is also enabled.\n\nThis can't be used with Super Resolution and Supersampling. It will be automatically disabled when you enable Super Resolution or Supersampling. \nAlso can't be used along with subsample feature due to unsupported texture format";
                var sharpeningEnhanceContent = new GUIContent();
                sharpeningEnhanceContent.text = "Sharpening Enhance Mode";
                sharpeningEnhanceContent.tooltip = "None: Full screen will be sharpened\n\nFixed Foveated: Only the central fixation point will be sharpened\n\nSelf Adaptive: Only when contrast between the current pixel and the surrounding pixels exceeds a certain threshold will be sharpened.\n\nThis menu will be only enabled while Sharpening (either Normal or Quality) is enabled.";

                if (overlayTarget.superResolution || overlayTarget.normalSupersampling || overlayTarget.qualitySupersampling || overlayTarget.fixedFoveatedSupersampling)
                {
                    GUI.enabled = false;
                    overlayTarget.sharpeningMode = SharpeningMode.None;
                    overlayTarget.sharpeningEnhance = SharpeningEnhance.None;
                }
                else
                {
                    GUI.enabled = true;
                }

                overlayTarget.sharpeningMode = (SharpeningMode)EditorGUILayout.EnumPopup(sharpeningContent, overlayTarget.sharpeningMode);
                if (overlayTarget.sharpeningMode == SharpeningMode.None)
                {
                    overlayTarget.sharpeningEnhance = SharpeningEnhance.None;
                }
                else
                {
                    EditorGUI.indentLevel++;
                    overlayTarget.sharpeningEnhance = (SharpeningEnhance)EditorGUILayout.EnumPopup(sharpeningEnhanceContent, overlayTarget.sharpeningEnhance);
                    EditorGUI.indentLevel--;
                }

                if (overlayTarget.sharpeningMode != SharpeningMode.None)
                {
                    if (overlayTarget.sharpeningMode == SharpeningMode.Normal)
                    {
                        overlayTarget.normalSharpening = true;
                        overlayTarget.qualitySharpening = false;
                    }
                    else
                    {
                        overlayTarget.normalSharpening = false;
                        overlayTarget.qualitySharpening = true;
                    }

                    if (overlayTarget.sharpeningEnhance == SharpeningEnhance.Both)
                    {
                        overlayTarget.fixedFoveatedSharpening = true;
                        overlayTarget.selfAdaptiveSharpening = true;
                    }
                    else if (overlayTarget.sharpeningEnhance == SharpeningEnhance.FixedFoveated)
                    {
                        overlayTarget.fixedFoveatedSharpening = true;
                        overlayTarget.selfAdaptiveSharpening = false;
                    }
                    else if (overlayTarget.sharpeningEnhance == SharpeningEnhance.SelfAdaptive)
                    {
                        overlayTarget.fixedFoveatedSharpening = false;
                        overlayTarget.selfAdaptiveSharpening = true;
                    }
                    else
                    {
                        overlayTarget.fixedFoveatedSharpening = false;
                        overlayTarget.selfAdaptiveSharpening = false;
                    }
                }
                else
                {
                    overlayTarget.normalSharpening = false;
                    overlayTarget.qualitySharpening = false;
                    overlayTarget.fixedFoveatedSharpening = false;
                    overlayTarget.selfAdaptiveSharpening = false;
                }

                if (GUI.changed)
                {
                    EditorUtility.SetDirty(overlayTarget);
                    EditorUtility.SetDirty(overlayTarget);
                }
                serializedObject.ApplyModifiedProperties();
            }

            if (GUI.changed)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
        }
        private Rect ClampRect(Rect rect)
        {
            rect.x = Mathf.Clamp01(rect.x);
            rect.y = Mathf.Clamp01(rect.y);
            rect.width = Mathf.Clamp01(rect.width);
            rect.height = Mathf.Clamp01(rect.height);
            return rect;
        }
    }
}