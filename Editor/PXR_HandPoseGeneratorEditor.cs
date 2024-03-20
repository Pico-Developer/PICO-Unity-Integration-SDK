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

using UnityEngine;
using UnityEditor;
using System;
using UnityEditorInternal;
using System.Collections.Generic;

namespace Unity.XR.PXR.Editor
{
    [CustomEditor(typeof(PXR_HandPoseGenerator))]
    public class PXR_HandPoseGeneratorEditor : UnityEditor.Editor
    {
        private static bool shapesRecognizer = true;
        private static bool bonesRecognizer = false;
        private static bool transRecognizer = false;
        private static bool thumb = true;
        private static bool index = true;
        private static bool middle = true;
        private static bool ring = true;
        private static bool pinky = true;

        private PXR_HandPoseGenerator m_Target;
        private PXR_HandPoseConfig config;
        private PXR_HandPosePreview preview;
        private ReorderableList bonesArray;

        private void OnEnable()
        {
            m_Target = (PXR_HandPoseGenerator)target;
            InitHandPosePreview();
            InitBonesGroup();
        }
        private void OnDisable()
        {
            DestroyHandPosePreview();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Hand Pose Config");
            m_Target.config = (PXR_HandPoseConfig)EditorGUILayout.ObjectField(m_Target.config, typeof(PXR_HandPoseConfig), false);

            if (GUILayout.Button("New"))
            {
                m_Target.config = CreateInstance<PXR_HandPoseConfig>();
                AssetDatabase.CreateAsset(m_Target.config, string.Format("Assets/{0}.asset", typeof(PXR_HandPoseConfig).Name + "_" + DateTime.Now.ToString("MMddhhmmss")));
            }

            //if (GUILayout.Button("Play"))
            //{

            //}

            GUILayout.EndHorizontal();

            if (m_Target.config != null)
            {
                if (config != m_Target.config)
                {
                    config = m_Target.config;
                    ConfigRead();
                }

                shapesRecognizer = EditorGUILayout.Foldout(shapesRecognizer, "Shapes");
                if (shapesRecognizer)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    using (new GUILayout.VerticalScope())
                    {
                        thumb = EditorGUILayout.Foldout(thumb, "Thumb");
                        if (thumb)
                        {
                            FingerConfig(m_Target.thumb);
                        }
                        index = EditorGUILayout.Foldout(index, "Index");
                        if (index)
                        {
                            FingerConfig(m_Target.index);
                        }
                        middle = EditorGUILayout.Foldout(middle, "Middle");
                        if (middle)
                        {
                            FingerConfig(m_Target.middle);
                        }
                        ring = EditorGUILayout.Foldout(ring, "Ring");
                        if (ring)
                        {
                            FingerConfig(m_Target.ring);
                        }
                        pinky = EditorGUILayout.Foldout(pinky, "Pinky");
                        if (pinky)
                        {
                            FingerConfig(m_Target.pinky);
                        }
                        EditorGUILayout.Space(5);
                        serializedObject.FindProperty("shapesholdDuration").floatValue = EditorGUILayout.FloatField("Hold Duration", Mathf.Max(0, serializedObject.FindProperty("shapesholdDuration").floatValue));
                    }
                    EditorGUILayout.EndHorizontal();

                }

                bonesRecognizer = EditorGUILayout.Foldout(bonesRecognizer, "Bones");
                if (bonesRecognizer)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    using (new GUILayout.VerticalScope())
                    {
                        BonesConfig(m_Target.Bones);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                transRecognizer = EditorGUILayout.Foldout(transRecognizer, "Transform");
                if (transRecognizer)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    using (new GUILayout.VerticalScope())
                    {
                        TransConfig();
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (GUI.changed)
                {
                    ConfigSave();
                    UpdatePosePreview();
                    EditorUtility.SetDirty(m_Target.config);
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void ConfigSave()
        {
            try
            {
                m_Target.config.shapesRecognizer.holdDuration = m_Target.shapesholdDuration;
                m_Target.config.shapesRecognizer.thumb = m_Target.thumb;
                m_Target.config.shapesRecognizer.index = m_Target.index;
                m_Target.config.shapesRecognizer.middle = m_Target.middle;
                m_Target.config.shapesRecognizer.ring = m_Target.ring;
                m_Target.config.shapesRecognizer.pinky = m_Target.pinky;

                m_Target.config.bonesRecognizer.holdDuration = m_Target.bonesHoldDuration;
                m_Target.config.bonesRecognizer.Bones = m_Target.Bones;

                m_Target.config.transRecognizer.holdDuration = m_Target.transHoldDuration;
                m_Target.config.transRecognizer.trackAxis = m_Target.trackAxis;
                m_Target.config.transRecognizer.spaceType = m_Target.spaceType;
                m_Target.config.transRecognizer.trackTarget = m_Target.trackTarget;
                m_Target.config.transRecognizer.angleThreshold = m_Target.angleThreshold;
                m_Target.config.transRecognizer.thresholdWidth = m_Target.thresholdWidth;
            }
            catch (Exception e)
            {
                Debug.LogError("Save Error " + e.ToString());
            }
            AssetDatabase.Refresh();
        }

        private void UpdatePosePreview()
        {
            if (shapesRecognizer)
            {
                preview.UpdateShapeState(m_Target.config.shapesRecognizer);
            }
            else
            {
                preview.ResetShapeState();
            }
            if (transRecognizer)
            {
                preview.UpdateTransformState(m_Target.config.transRecognizer);
            }
            else
            {
                preview.ResetTransformState();
            }
        }

        private void ConfigRead()
        {
            m_Target.shapesholdDuration = m_Target.config.shapesRecognizer.holdDuration;
            m_Target.thumb = m_Target.config.shapesRecognizer.thumb;
            m_Target.index = m_Target.config.shapesRecognizer.index;
            m_Target.middle = m_Target.config.shapesRecognizer.middle;
            m_Target.ring = m_Target.config.shapesRecognizer.ring;
            m_Target.pinky = m_Target.config.shapesRecognizer.pinky;

            m_Target.bonesHoldDuration = m_Target.config.bonesRecognizer.holdDuration;
            m_Target.Bones = m_Target.config.bonesRecognizer.Bones;

            m_Target.transHoldDuration = m_Target.config.transRecognizer.holdDuration;
            m_Target.trackAxis = m_Target.config.transRecognizer.trackAxis;
            m_Target.spaceType = m_Target.config.transRecognizer.spaceType;
            m_Target.trackTarget = m_Target.config.transRecognizer.trackTarget;
            m_Target.angleThreshold = m_Target.config.transRecognizer.angleThreshold;
            m_Target.thresholdWidth = m_Target.config.transRecognizer.thresholdWidth;
        }

        private void TransConfig()
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.Space(5);
            using (new GUILayout.VerticalScope())
            {
                EditorGUILayout.Space(5);
                m_Target.trackAxis = (TransRecognizer.TrackAxis)EditorGUILayout.EnumPopup("Track Axis", m_Target.trackAxis);
                m_Target.trackTarget = (TransRecognizer.TrackTarget)EditorGUILayout.EnumPopup("Track Target", m_Target.trackTarget);
                m_Target.angleThreshold = EditorGUILayout.FloatField("Angle Threshold", m_Target.angleThreshold);
                m_Target.thresholdWidth = EditorGUILayout.FloatField("Margin", m_Target.thresholdWidth);
                EditorGUILayout.Space(5);
            }
            EditorGUILayout.Space(5);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
            m_Target.transHoldDuration = EditorGUILayout.FloatField("Hold Duration", m_Target.transHoldDuration);
        }

        private void BonesConfig(List<BonesRecognizer.BonesGroup> listBones)
        {
            using (new GUILayout.VerticalScope())
            {
                bonesArray.DoLayoutList();
            }
            serializedObject.FindProperty("bonesHoldDuration").floatValue = EditorGUILayout.FloatField("Hold Duration", Mathf.Max(0, serializedObject.FindProperty("bonesHoldDuration").floatValue));
        }

        private void InitBonesGroup()
        {
            bonesArray = new ReorderableList(serializedObject, serializedObject.FindProperty("Bones"), true, true, true, true);

            bonesArray.drawHeaderCallback = (Rect rect) =>
            {
                GUI.Label(rect, "Bones Groups");
            };

            bonesArray.elementHeightCallback = (index) =>
            {
                var element = bonesArray.serializedProperty.GetArrayElementAtIndex(index);
                var h = EditorGUIUtility.singleLineHeight;
                if (element.isExpanded)
                    h += EditorGUI.GetPropertyHeight(element) + EditorGUIUtility.singleLineHeight;
                return h;
            };

            bonesArray.drawElementCallback = (Rect rect, int index, bool selected, bool focused) =>
            {
                SerializedProperty item = bonesArray.serializedProperty.GetArrayElementAtIndex(index);

                var posRect_label = new Rect(rect)
                {
                    x = rect.x + 14,
                    width = rect.width - 18,
                    height = EditorGUIUtility.singleLineHeight
                };
                item.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(posRect_label, item.isExpanded, item.isExpanded ? "" : $"{index}");
                if (item.isExpanded)
                {
                    rect.height -= 8 + EditorGUIUtility.singleLineHeight;
                    rect.y += 18;

                    GUIStyle style = new GUIStyle(EditorStyles.label);
                    style.fontSize = 20;
                    style.fontStyle = FontStyle.Bold;
                    EditorGUI.LabelField(rect, " " + index.ToString(), style);
                    EditorGUI.DrawRect(rect, new Color(0, 0, 0, 0.2f));

                    rect.y += 6;
                    EditorGUI.PropertyField(rect, item, new GUIContent());
                }
                EditorGUI.EndFoldoutHeaderGroup();

            };

            bonesArray.onAddCallback = (ReorderableList list) =>
            {
                ReorderableList.defaultBehaviours.DoAddButton(list);
                list.serializedProperty.GetArrayElementAtIndex(list.count - 1).FindPropertyRelative("distance").floatValue = 0.03f;
                list.serializedProperty.GetArrayElementAtIndex(list.count - 1).FindPropertyRelative("thresholdWidth").floatValue = 0.01f;
            };

            bonesArray.onRemoveCallback = (ReorderableList list) =>
            {
                {
                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                }
            };
        }

        private void FingerConfig(ShapesRecognizer.Finger finger)
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.Space(5);
            using (new GUILayout.VerticalScope())
            {
                EditorGUILayout.Space(5);
                FlexionConfig(finger, finger.fingerConfigs.flexionConfigs);
                CurlConfig(finger, finger.fingerConfigs.curlConfigs);
                AbductionConfig(finger, finger.fingerConfigs.abductionConfigs);
                EditorGUILayout.Space(5);
            }
            EditorGUILayout.Space(5);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }

        private void FlexionConfig(ShapesRecognizer.Finger finger, ShapesRecognizer.RangeConfigs flexionConfigs)
        {
            finger.flexion = (ShapesRecognizer.Flexion)EditorGUILayout.EnumPopup("Flexion", finger.flexion);
            Vector2 defaultVal = new Vector2();
            switch (finger.flexion)
            {
                case ShapesRecognizer.Flexion.Any:
                    return;
                case ShapesRecognizer.Flexion.Open:
                    defaultVal = GetDefaultShapeVal(finger.handFinger, ShapesRecognizer.ShapeType.flexion, true);
                    flexionConfigs.min = defaultVal.x;
                    flexionConfigs.max = defaultVal.y;
                    break;
                case ShapesRecognizer.Flexion.Close:
                    defaultVal = GetDefaultShapeVal(finger.handFinger, ShapesRecognizer.ShapeType.flexion, false);
                    flexionConfigs.min = defaultVal.x;
                    flexionConfigs.max = defaultVal.y;
                    break;
                //case ShapesRecognizer.Flexion.Custom:
                //    EditorGUILayout.MinMaxSlider("Custom Range",
                //        ref flexionConfigs.min,
                //        ref flexionConfigs.max,
                //        ShapesRecognizer.flexionMin,
                //        ShapesRecognizer.flexionMax);
                //   break;
                default:
                    break;
            }
            flexionConfigs.width = EditorGUILayout.Slider("Margin", flexionConfigs.width, 0,
                ShapesRecognizer.flexionMax - ShapesRecognizer.flexionMin);
            EditorGUILayout.LabelField(new GUIContent("Flexion Range"),
                new GUIContent($"[{flexionConfigs.min + " - " + flexionConfigs.width}, {flexionConfigs.max + " + " + flexionConfigs.width}]"));
        }

        private void CurlConfig(ShapesRecognizer.Finger finger, ShapesRecognizer.RangeConfigs curlConfigs)
        {
            finger.curl = (ShapesRecognizer.Curl)EditorGUILayout.EnumPopup("Curl", finger.curl);
            Vector2 defaultVal;
            switch (finger.curl)
            {
                case ShapesRecognizer.Curl.Any:
                    return;
                case ShapesRecognizer.Curl.Open:
                    defaultVal = GetDefaultShapeVal(finger.handFinger, ShapesRecognizer.ShapeType.curl, true);
                    curlConfigs.min = defaultVal.x;
                    curlConfigs.max = defaultVal.y;
                    break;
                case ShapesRecognizer.Curl.Close:
                    defaultVal = GetDefaultShapeVal(finger.handFinger, ShapesRecognizer.ShapeType.curl, false);
                    curlConfigs.min = defaultVal.x;
                    curlConfigs.max = defaultVal.y;
                    break;
                //case ShapesRecognizer.Curl.Custom:
                //    EditorGUILayout.MinMaxSlider("Custom Range",
                //        ref curlConfigs.min,
                //        ref curlConfigs.max,
                //        finger.handFinger == HandFinger.Thumb ? ShapesRecognizer.curlThumbMin : ShapesRecognizer.curlMin,
                //        finger.handFinger == HandFinger.Thumb ? ShapesRecognizer.curlThumbMax : ShapesRecognizer.curlMax);
                //   break;
                default:
                    break;
            }
            curlConfigs.width = EditorGUILayout.Slider("Margin", curlConfigs.width, 0,
                 ShapesRecognizer.curlMax - ShapesRecognizer.curlMin);
            EditorGUILayout.LabelField(new GUIContent("Curl Range"),
                new GUIContent($"[{curlConfigs.min + " - " + curlConfigs.width}, {curlConfigs.max + " + " + curlConfigs.width}]"));
        }

        private void AbductionConfig(ShapesRecognizer.Finger finger, ShapesRecognizer.RangeConfigsAbduction abductionConfigs)
        {
            if (finger.handFinger == HandFinger.Pinky) return;

            finger.abduction = (ShapesRecognizer.Abduction)EditorGUILayout.EnumPopup("Abduction", finger.abduction);
            Vector2 defaultVal = GetDefaultShapeVal(finger.handFinger, ShapesRecognizer.ShapeType.abduction);
            abductionConfigs.mid = defaultVal.x;
            switch (finger.abduction)
            {
                case ShapesRecognizer.Abduction.Any:
                    return;
                case ShapesRecognizer.Abduction.Open:
                    break;
                case ShapesRecognizer.Abduction.Close:
                    break;
                default:
                    break;
            }
            abductionConfigs.width = EditorGUILayout.Slider("Margin", abductionConfigs.width, 0,
                ShapesRecognizer.abductionMax - ShapesRecognizer.abductionMin);
            EditorGUILayout.LabelField(new GUIContent("Abduction Range"),
                new GUIContent($"[{abductionConfigs.mid + " ± " + abductionConfigs.width + "/2"}]"));
        }

        private Vector2 GetDefaultShapeVal(HandFinger finger, ShapesRecognizer.ShapeType shapeType, bool isOpen = true)
        {
            Vector2 val = new Vector2();
            switch (shapeType)
            {
                case ShapesRecognizer.ShapeType.flexion:
                    val.x = finger == HandFinger.Thumb ? (isOpen ? ShapesRecognizer.flexionThumbOpenMin : ShapesRecognizer.flexionThumbCloseMin) :
                        (isOpen ? ShapesRecognizer.flexionOpenMin : ShapesRecognizer.flexionCloseMin);
                    val.y = finger == HandFinger.Thumb ? (isOpen ? ShapesRecognizer.flexionThumbOpenMax : ShapesRecognizer.flexionThumbCloseMax) :
                        (isOpen ? ShapesRecognizer.flexionOpenMax : ShapesRecognizer.flexionCloseMax);
                    break;
                case ShapesRecognizer.ShapeType.curl:
                    val.x = finger == HandFinger.Thumb ? (isOpen ? ShapesRecognizer.curlThumbOpenMin : ShapesRecognizer.curlThumbCloseMin) :
                        (isOpen ? ShapesRecognizer.curlOpenMin : ShapesRecognizer.curlCloseMin);
                    val.y = finger == HandFinger.Thumb ? (isOpen ? ShapesRecognizer.curlThumbOpenMax : ShapesRecognizer.curlThumbCloseMax) :
                        (isOpen ? ShapesRecognizer.curlOpenMax : ShapesRecognizer.curlCloseMax);
                    break;
                case ShapesRecognizer.ShapeType.abduction:
                    val.x = finger == HandFinger.Thumb ? ShapesRecognizer.abductionThumbMid : ShapesRecognizer.abductionMid;
                    val.y = finger == HandFinger.Thumb ? ShapesRecognizer.abductionThumbWidth : ShapesRecognizer.abductionWidth;
                    break;
            }
            return val;
        }

        public override bool HasPreviewGUI()
        {
            return true;
        }

        private void DestroyHandPosePreview()
        {
            if (previewInstance)
            {
                DestroyImmediate(previewInstance);
            }
            previewInstance = null;

            if (previewRenderUtility != null)
            {
                previewRenderUtility.Cleanup();
                previewRenderUtility = null;
            }
        }

        public override GUIContent GetPreviewTitle()
        {
            return new GUIContent("Hand Pose");
        }

        public override void OnPreviewSettings()
        {
            if (GUILayout.Button("Reset", "preButton"))
            {
                dragPos = Vector2.zero;
            }
        }

        private PreviewRenderUtility previewRenderUtility;
        private GameObject previewInstance;
        private Vector2 dragPos;

        private void InitHandPosePreview()
        {
            if (previewRenderUtility == null)
            {
                previewRenderUtility = new PreviewRenderUtility(true);
                previewRenderUtility.cameraFieldOfView = 60f;

                previewInstance = Instantiate(m_Target.preview.gameObject);
                previewInstance.SetActive(true);
                preview = previewInstance.GetComponent<PXR_HandPosePreview>();
                previewRenderUtility.AddSingleGO(previewInstance);
            }

        }

        private static Vector2 Drag2D(Vector2 scrollPosition, Rect position)
        {
            int controlID = GUIUtility.GetControlID("Slider".GetHashCode(), FocusType.Passive);
            Event current = Event.current;

            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    {
                        bool flag = position.Contains(current.mousePosition) && position.width > 50f;
                        if (flag)
                        {
                            GUIUtility.hotControl = controlID;
                            current.Use();
                            EditorGUIUtility.SetWantsMouseJumping(1);
                        }

                        break;
                    }
                case EventType.MouseUp:
                    {
                        bool flag2 = GUIUtility.hotControl == controlID;
                        if (flag2)
                        {
                            GUIUtility.hotControl = 0;
                        }

                        EditorGUIUtility.SetWantsMouseJumping(0);
                        break;
                    }
                case EventType.MouseDrag:
                    {
                        bool flag3 = GUIUtility.hotControl == controlID;
                        if (flag3)
                        {
                            scrollPosition -= current.delta / Mathf.Min(position.width, position.height) * 140f;
                            current.Use();
                            GUI.changed = true;
                        }
                        break;
                    }
            }

            return scrollPosition;
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            dragPos = Drag2D(dragPos, rect);

            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            if (previewRenderUtility != null)
            {
                previewRenderUtility.BeginPreview(rect, background);

                Camera camera = previewRenderUtility.camera;
                camera.clearFlags = CameraClearFlags.Depth;
                camera.nearClipPlane = 0.01f;
                camera.farClipPlane = 100f;
                camera.transform.position = camera.transform.forward * -2f;

                preview.posePreviewX.localEulerAngles = new Vector3(0, dragPos.x, 0);
                preview.posePreviewY.localEulerAngles = new Vector3(Mathf.Clamp(dragPos.y, -60f, 0f), 0f, 0f);

                camera.Render();
                previewRenderUtility.EndAndDrawPreview(rect);
            }
        }
    }

    [CustomPropertyDrawer(typeof(DisplayOnly))]
    public class DisplayOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }

    [CustomPropertyDrawer(typeof(LabelAttribute))]
    public class LabelAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LabelAttribute labelAttribute = this.attribute as LabelAttribute;
            EditorGUI.PropertyField(position, property, new GUIContent(labelAttribute.name));
        }
    }

    [CustomPropertyDrawer(typeof(BonesRecognizer.BonesGroup))]
    public class PXR_BonesGroupPropertyDrawer : PropertyDrawer
    {
        private float propertyHeight = EditorGUIUtility.singleLineHeight;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var Space_Height = 2;

            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.width = position.width - position.width / 6;
            rect.y += Space_Height;

            var boneRect = rect;

            boneRect.position = new Vector2(rect.position.x + rect.width / 15 * 2, rect.position.y);
            var handAProperty = property.FindPropertyRelative("bone1");
            EditorGUI.PropertyField(boneRect, handAProperty);


            rect.y += EditorGUIUtility.singleLineHeight + Space_Height;

            boneRect.position = new Vector2(rect.position.x + rect.width / 15 * 2, rect.position.y);
            var handBProperty = property.FindPropertyRelative("bone2");
            EditorGUI.PropertyField(boneRect, handBProperty);


            rect.y += EditorGUIUtility.singleLineHeight + Space_Height;

            boneRect.position = new Vector2(rect.position.x + rect.width / 15 * 2, rect.position.y);
            var disProperty = property.FindPropertyRelative("distance");
            EditorGUI.PropertyField(boneRect, disProperty);

            rect.y += EditorGUIUtility.singleLineHeight + Space_Height;

            boneRect.position = new Vector2(rect.position.x + rect.width / 15 * 2, rect.position.y);
            var thresProperty = property.FindPropertyRelative("thresholdWidth");
            EditorGUI.PropertyField(boneRect, thresProperty);

            propertyHeight = rect.y - position.y + EditorGUIUtility.singleLineHeight;

            EditorGUI.EndProperty();

        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return propertyHeight;
        }
    }

}