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

using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;
using System;

[ExecuteInEditMode]
public class PXR_HandPosePreview : MonoBehaviour
{
    [HideInInspector]public List<Transform> handJoints = new List<Transform>(new Transform[(int)HandJoint.JointMax]);
    [HideInInspector] public Vector3[] jointAngles = new Vector3[(int)HandJoint.JointMax];
    [HideInInspector] public Transform posePreviewX;
    [HideInInspector] public Transform posePreviewY;
    [HideInInspector] public Transform handModel;
    [HideInInspector] public SkinnedMeshRenderer handAxis;
    [HideInInspector] public Transform headModel;
    [HideInInspector] public Transform handShadow;

    [HideInInspector] public ModelFinger modelThumb = new ModelFinger(ModelFinger.FingerType.thumb);
    [HideInInspector] public ModelFinger modelIndex = new ModelFinger(ModelFinger.FingerType.index);
    [HideInInspector] public ModelFinger modelMiddle = new ModelFinger(ModelFinger.FingerType.middle);
    [HideInInspector] public ModelFinger modelRing = new ModelFinger(ModelFinger.FingerType.ring);
    [HideInInspector] public ModelFinger modelLittle = new ModelFinger(ModelFinger.FingerType.little);

    [HideInInspector] public Material openMaterial;
    [HideInInspector] public Material anyMaterial;
    [HideInInspector] public Material openFadeMaterial;
    [HideInInspector] public Material anyFadeMaterial;
    [HideInInspector] public Material highLightMaterial;

    private Vector4 highLightBlendPower;
    private int blendPower = Shader.PropertyToID("_BlendPower");

    public void UpdateShapeState(ShapesRecognizer shapesConfig)
    {
        var thumb = shapesConfig.thumb;
        var index = shapesConfig.index;
        var middle = shapesConfig.middle;
        var ring = shapesConfig.ring;
        var little = shapesConfig.pinky;

        int joint = 0;
        Vector3 angle = Vector3.zero;
        //thumb
        joint = (int)HandJoint.JointThumbProximal;
        angle =
            thumb.flexion == ShapesRecognizer.Flexion.Close ? new Vector3(52f, -37, -8) :
            thumb.abduction == ShapesRecognizer.Abduction.Close ? new Vector3(58f, 16, 1) : jointAngles[joint];
        handJoints[joint].localEulerAngles = angle;

        joint = (int)HandJoint.JointThumbDistal;
        angle =
            thumb.curl == ShapesRecognizer.Curl.Close ? new Vector3(36, jointAngles[joint].y, jointAngles[joint].z) : jointAngles[joint];
        handJoints[joint].localEulerAngles = angle;

        modelThumb.HighlightModelJoints(this,thumb.flexion, thumb.curl);

        //index
        joint = (int)HandJoint.JointIndexProximal;
        angle =
            index.flexion == ShapesRecognizer.Flexion.Close ? new Vector3(jointAngles[joint].x + 68, jointAngles[joint].y, jointAngles[joint].z) :
            index.abduction == ShapesRecognizer.Abduction.Close ? new Vector3(jointAngles[joint].x, 18, jointAngles[joint].z) : jointAngles[joint];
        handJoints[joint].localEulerAngles = angle;

        joint = (int)HandJoint.JointIndexIntermediate;
        angle = index.curl == ShapesRecognizer.Curl.Close ? new Vector3(jointAngles[joint].x + 60, jointAngles[joint].y, jointAngles[joint].z) : jointAngles[joint];
        handJoints[joint].localEulerAngles = angle;

        joint = (int)HandJoint.JointIndexDistal;
        angle = index.curl == ShapesRecognizer.Curl.Close ? new Vector3(jointAngles[joint].x + 65, jointAngles[joint].y, jointAngles[joint].z) : jointAngles[joint];
        handJoints[joint].localEulerAngles = angle;

        modelIndex.HighlightModelJoints(this, index.flexion, index.curl);

        //middle
        joint = (int)HandJoint.JointMiddleProximal;
        angle =
            middle.flexion == ShapesRecognizer.Flexion.Close ? new Vector3(jointAngles[joint].x + 68, jointAngles[joint].y, jointAngles[joint].z) : jointAngles[joint];
        handJoints[joint].localEulerAngles = angle;

        joint = (int)HandJoint.JointMiddleIntermediate;
        angle = middle.curl == ShapesRecognizer.Curl.Close ? new Vector3(jointAngles[joint].x + 60, jointAngles[joint].y, jointAngles[joint].z) : jointAngles[joint];
        handJoints[joint].localEulerAngles = angle;

        joint = (int)HandJoint.JointMiddleDistal;
        angle = middle.curl == ShapesRecognizer.Curl.Close ? new Vector3(jointAngles[joint].x + 65, jointAngles[joint].y, jointAngles[joint].z) : jointAngles[joint];
        handJoints[joint].localEulerAngles = angle;

        modelMiddle.HighlightModelJoints(this, middle.flexion, middle.curl);

        //ring
        joint = (int)HandJoint.JointRingProximal;
        angle =
            ring.flexion == ShapesRecognizer.Flexion.Close ? new Vector3(jointAngles[joint].x + 68, jointAngles[joint].y, jointAngles[joint].z) :
            middle.abduction == ShapesRecognizer.Abduction.Close ? new Vector3(jointAngles[joint].x, -18, jointAngles[joint].z) : jointAngles[joint];
        handJoints[joint].localEulerAngles = angle;

        joint = (int)HandJoint.JointRingIntermediate;
        angle = ring.curl == ShapesRecognizer.Curl.Close ? new Vector3(jointAngles[joint].x + 60, jointAngles[joint].y, jointAngles[joint].z) : jointAngles[joint];
        handJoints[joint].localEulerAngles = angle;

        joint = (int)HandJoint.JointRingDistal;
        angle = ring.curl == ShapesRecognizer.Curl.Close ? new Vector3(jointAngles[joint].x + 65, jointAngles[joint].y, jointAngles[joint].z) : jointAngles[joint];
        handJoints[joint].localEulerAngles = angle;

        modelRing.HighlightModelJoints(this, ring.flexion, ring.curl);

        //little
        joint = (int)HandJoint.JointLittleProximal;
        angle =
            little.flexion == ShapesRecognizer.Flexion.Close ? new Vector3(jointAngles[joint].x + 68, jointAngles[joint].y, jointAngles[joint].z) :
            ring.abduction == ShapesRecognizer.Abduction.Close ? new Vector3(jointAngles[joint].x, -18, jointAngles[joint].z) : jointAngles[joint];
        handJoints[joint].localEulerAngles = angle;

        joint = (int)HandJoint.JointLittleIntermediate;
        angle = little.curl == ShapesRecognizer.Curl.Close ? new Vector3(jointAngles[joint].x + 60, jointAngles[joint].y, jointAngles[joint].z) : jointAngles[joint];
        handJoints[joint].localEulerAngles = angle;

        joint = (int)HandJoint.JointLittleDistal;
        angle = little.curl == ShapesRecognizer.Curl.Close ? new Vector3(jointAngles[joint].x + 65, jointAngles[joint].y, jointAngles[joint].z) : jointAngles[joint];
        handJoints[joint].localEulerAngles = angle;

        modelLittle.HighlightModelJoints(this, little.flexion, little.curl);

        //abduction highlight
        highLightBlendPower.w = thumb.abduction == ShapesRecognizer.Abduction.Any ? 0 : 1;
        highLightBlendPower.x = index.abduction == ShapesRecognizer.Abduction.Any ? 0 : 1;
        highLightBlendPower.y = middle.abduction == ShapesRecognizer.Abduction.Any ? 0 : 1;
        highLightBlendPower.z = ring.abduction == ShapesRecognizer.Abduction.Any ? 0 : 1;

        highLightMaterial.SetVector(blendPower, highLightBlendPower);
    }

    public void ResetShapeState()
    {
        for (int i = 0; i < handJoints.Count; i++)
        {
            handJoints[i].localEulerAngles = jointAngles[i];
        }

        modelThumb.HighlightModelJoints(this, ShapesRecognizer.Flexion.Any, ShapesRecognizer.Curl.Any);
        modelIndex.HighlightModelJoints(this, ShapesRecognizer.Flexion.Any, ShapesRecognizer.Curl.Any);
        modelMiddle.HighlightModelJoints(this, ShapesRecognizer.Flexion.Any, ShapesRecognizer.Curl.Any);
        modelRing.HighlightModelJoints(this, ShapesRecognizer.Flexion.Any, ShapesRecognizer.Curl.Any);
        modelLittle.HighlightModelJoints(this, ShapesRecognizer.Flexion.Any, ShapesRecognizer.Curl.Any);

        highLightMaterial.SetVector(blendPower, Vector4.zero);
    }

    public void ResetTransformState()
    {
        headModel.gameObject.SetActive(false);
        handAxis.gameObject.SetActive(false);
        handModel.localEulerAngles = new Vector3(-90, 180, 0);
    }

    public void UpdateTransformState(TransRecognizer transRecognizer)
    {
        handAxis.gameObject.SetActive(true);

        switch (transRecognizer.trackAxis)
        {
            case TransRecognizer.TrackAxis.Fingers:
                handAxis.SetBlendShapeWeight(0, 100);
                handAxis.SetBlendShapeWeight(1, 0);
                handAxis.SetBlendShapeWeight(2, 0);
                break;
            case TransRecognizer.TrackAxis.Palm:
                handAxis.SetBlendShapeWeight(0, 0);
                handAxis.SetBlendShapeWeight(1, 0);
                handAxis.SetBlendShapeWeight(2, 100);
                break;
            case TransRecognizer.TrackAxis.Thumb:
                handAxis.SetBlendShapeWeight(0, 0);
                handAxis.SetBlendShapeWeight(1, 100);
                handAxis.SetBlendShapeWeight(2, 0);
                break;
            default:
                break;
        }

        switch (transRecognizer.trackTarget)
        {
            case TransRecognizer.TrackTarget.TowardsFace:
                headModel.gameObject.SetActive(true);
                headModel.localPosition = new Vector3(0, 0.05f, -0.24f);
                headModel.localEulerAngles = Vector3.zero;

                handModel.localEulerAngles =
                    transRecognizer.trackAxis == TransRecognizer.TrackAxis.Fingers ? new Vector3(0, 180, 0) :
                    transRecognizer.trackAxis == TransRecognizer.TrackAxis.Palm ? new Vector3(-90, 180, 0) : new Vector3(-90, 0, -90);
                break;
            case TransRecognizer.TrackTarget.AwayFromFace:
                headModel.gameObject.SetActive(true);
                headModel.localPosition = new Vector3(0, 0.05f, 0.24f);
                headModel.localEulerAngles = new Vector3(0, 180, 0);

                handModel.localEulerAngles =
                    transRecognizer.trackAxis == TransRecognizer.TrackAxis.Fingers ? new Vector3(0, 180, 0) :
                    transRecognizer.trackAxis == TransRecognizer.TrackAxis.Palm ? new Vector3(-90, 180, 0) : new Vector3(-90, 0, -90);
                break;
            case TransRecognizer.TrackTarget.WorldUp:
                headModel.gameObject.SetActive(false);

                handModel.localEulerAngles =
                    transRecognizer.trackAxis == TransRecognizer.TrackAxis.Fingers ? new Vector3(-90, 0, 0) :
                    transRecognizer.trackAxis == TransRecognizer.TrackAxis.Palm ? new Vector3(0, 0, 180) : new Vector3(0, 0, -90);
                break;
            case TransRecognizer.TrackTarget.WorldDown:
                headModel.gameObject.SetActive(false);

                handModel.localEulerAngles =
                    transRecognizer.trackAxis == TransRecognizer.TrackAxis.Fingers ? new Vector3(90, 0, 0) :
                    transRecognizer.trackAxis == TransRecognizer.TrackAxis.Palm ? Vector3.zero : new Vector3(0, 0, 90);
                break;
            default:
                break;
        }

        if (handModel.localEulerAngles.x == 0)
        {
            handShadow.GetChild(0).gameObject.SetActive(false);
            handShadow.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            handShadow.GetChild(0).gameObject.SetActive(true);
            handShadow.GetChild(1).gameObject.SetActive(false);
        }
    }

    [Serializable]
    public class ModelFinger
    {
        public FingerType Type;

        public List<Transform> flexionTransforms = new List<Transform>();
        public List<MeshRenderer> flexionMeshRenderers = new List<MeshRenderer>();

        public List<Transform> curlTransforms = new List<Transform>();
        public List<MeshRenderer> curlMeshRenderers = new List<MeshRenderer>();

        public enum ModelJoint
        {
            metacarpal = 0,
            proximal = 1,
            intermediate = 2,
            distal = 3,
            tip = 4
        }
        public enum FingerType
        {
            thumb,
            index,
            middle,
            ring,
            little
        }

        public ModelFinger(FingerType type)
        {
            Type = type;
        }

        public void RefreshModelJoints(Transform transform)
        {
            if (flexionTransforms.Count == 0 || curlTransforms.Count == 0)
            {
                flexionTransforms.Clear();
                curlTransforms.Clear();

                flexionMeshRenderers.Clear();
                curlMeshRenderers.Clear();

                var baseTransform = transform.GetChild(1);
                for (int i = 0; i < baseTransform.childCount; i++)
                {
                    if (baseTransform.GetChild(i).name.EndsWith($"{Type}_{ModelJoint.metacarpal}"))
                    {
                        baseTransform = baseTransform.GetChild(i);
                        break;
                    }
                }

                flexionTransforms.Add(GetModelJoint(baseTransform, ModelJoint.proximal));

                curlTransforms.Add(GetModelJoint(baseTransform, ModelJoint.intermediate));

                if (Type != FingerType.thumb)
                {
                    curlTransforms.Add(GetModelJoint(baseTransform, ModelJoint.distal));
                }

                foreach (var flexionTransform in flexionTransforms)
                {
                    flexionMeshRenderers.Add(flexionTransform.Find("Bone").GetComponent<MeshRenderer>());
                    flexionMeshRenderers.Add(flexionTransform.Find("Pointer").GetComponent<MeshRenderer>());
                    flexionMeshRenderers.Add(flexionTransform.parent.Find("Bone").GetComponent<MeshRenderer>());
                }

                foreach (var curlTransform in curlTransforms)
                {
                    var mesh = curlTransform.Find("Bone").GetComponent<MeshRenderer>();
                    if (!curlMeshRenderers.Contains(mesh)) curlMeshRenderers.Add(mesh);

                    mesh = curlTransform.Find("Pointer").GetComponent<MeshRenderer>();
                    if (!curlMeshRenderers.Contains(mesh)) curlMeshRenderers.Add(mesh);

                    mesh = curlTransform.parent.Find("Bone").GetComponent<MeshRenderer>();
                    if (!curlMeshRenderers.Contains(mesh)) curlMeshRenderers.Add(mesh);
                }

                if (Type != FingerType.thumb)
                {
                    var m = GetModelJoint(baseTransform, ModelJoint.tip).Find("Pointer")
                        .GetComponent<MeshRenderer>();
                    if (!curlMeshRenderers.Contains(m)) curlMeshRenderers.Add(m);
                }
                else
                {
                    var m = GetModelJoint(baseTransform, ModelJoint.distal).Find("Pointer")
                        .GetComponent<MeshRenderer>();
                    if (!curlMeshRenderers.Contains(m)) curlMeshRenderers.Add(m);
                }
            }
        }

        public void HighlightModelJoints(PXR_HandPosePreview handPosePreview, ShapesRecognizer.Flexion flexion, ShapesRecognizer.Curl curl)
        {
            foreach (var mesh in flexionMeshRenderers)
            {
                mesh.material = flexion != ShapesRecognizer.Flexion.Any ? handPosePreview.openMaterial : handPosePreview.anyMaterial;
            }
            foreach (var mesh in curlMeshRenderers)
            {
                mesh.material = curl != ShapesRecognizer.Curl.Any ? handPosePreview.openMaterial : handPosePreview.anyMaterial;
            }
            flexionMeshRenderers[2].material = flexion != ShapesRecognizer.Flexion.Any ? handPosePreview.openFadeMaterial : handPosePreview.anyFadeMaterial;
        }

        private Transform GetModelJoint(Transform tran, ModelJoint type)
        {
            for (int i = 0; i < (int)type; i++)
            {
                tran = tran.GetChild(2);
            }
            return tran;
        }
    }
}
