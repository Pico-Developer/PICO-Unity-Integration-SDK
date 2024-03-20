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
using UnityEngine.Events;
using System;
using UnityEngine.XR;

namespace Unity.XR.PXR
{
    public class PXR_HandPose : MonoBehaviour
    {
        public TrackType trackType;
        public PXR_HandPoseConfig config;
        public UnityEvent handPoseStart;
        public UpdateEvent handPoseUpdate;
        public UnityEvent handPoseEnd;

        private List<Vector3> leftJointPos = new List<Vector3>(new Vector3[(int)HandJoint.JointMax]);
        private List<Vector3> rightJointPos = new List<Vector3>(new Vector3[(int)HandJoint.JointMax]);
        private HandJointLocations leftHandJointLocations = new HandJointLocations();
        private HandJointLocations rightHandJointLocations = new HandJointLocations();

        private bool poseStateHold;
        private bool poseStateActive;
        private float poseStateHoldTime;

        public enum TrackType
        {
            Any,
            Left,
            Right
        }

        private void HandPoseEventCheck()
        {
            switch (trackType)
            {
                case TrackType.Any:
                    poseStateActive = (leftShapesActive && leftBonesActive && leftTransActive) || (rightShapesActive && rightBonesActive && rightTransActive);
                    break;
                case TrackType.Left:
                    poseStateActive = leftShapesActive && leftBonesActive && leftTransActive;
                    break;
                case TrackType.Right:
                    poseStateActive = rightShapesActive && rightBonesActive && rightTransActive;
                    break;
                default:
                    break;
            }
            if (poseStateHold != poseStateActive)
            {
                poseStateHold = poseStateActive;
                if (poseStateHold)
                {
                    poseStateActive = true;
                    if (handPoseStart != null)
                    {
                        handPoseStart.Invoke();
                    }
                }
                else
                {
                    poseStateActive = false;
                    if (handPoseStart != null)
                    {
                        handPoseEnd.Invoke();
                    }
                }
                poseStateHoldTime = 0f;
            }
            else
            {
                if (poseStateHold)
                {
                    poseStateHoldTime += Time.deltaTime;
                    handPoseUpdate.Invoke(poseStateHoldTime);
                }
            }
        }

        private bool HoldCheck(bool holdState, float holdDuration, bool resultState, ref float holdTime)
        {
            if (resultState != holdState)
            {
                holdTime += Time.deltaTime;
                if (holdTime >= holdDuration)
                {
                    resultState = holdState;
                }
            }
            else
            {
                holdTime = 0;
            }
            return resultState;
        }

        private void Start()
        {
            shapesHoldDuration = config.shapesRecognizer.holdDuration;

            bones = config.bonesRecognizer.Bones;
            bonesHoldDuration = config.bonesRecognizer.holdDuration;

            transTrackAxis = config.transRecognizer.trackAxis;
            transSpaceType = config.transRecognizer.spaceType;
            transTrackTarget = config.transRecognizer.trackTarget;
            transHoldDuration = config.transRecognizer.holdDuration;
            transAngleThreshold = config.transRecognizer.angleThreshold;
            transThresholdWidth = config.transRecognizer.thresholdWidth;
        }

        private void Update()
        {
            if (config == null) return;

            InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.devicePosition, out HMDpose);

            if (trackType == TrackType.Right || trackType == TrackType.Any)
            {
                PXR_HandTracking.GetJointLocations(HandType.HandRight, ref rightHandJointLocations);

                for (int i = 0; i < rightJointPos.Count; ++i)
                {
                    if (rightHandJointLocations.jointLocations == null) break;

                    rightJointPos[i] = rightHandJointLocations.jointLocations[i].pose.Position.ToVector3();

                    if (i == (int)HandJoint.JointWrist)
                    {
                        rightWirstPos = rightHandJointLocations.jointLocations[i].pose.Position.ToVector3();
                        rightWirstRot = rightHandJointLocations.jointLocations[i].pose.Orientation.ToQuat();
                    }
                }
                rightShapesHold = ShapesRecognizerCheck(rightJointPos, rightWirstRot * Vector3.left, rightWirstRot * Vector3.back);
                rightShapesActive = HoldCheck(rightShapesHold, shapesHoldDuration, rightShapesActive, ref rightShapesHoldTime);

                rightBonesHold = BonesCheck(HandType.HandRight);
                rightBonesActive = HoldCheck(rightBonesHold, bonesHoldDuration, rightBonesActive, ref rightBonesHoldTime);

                rightTransHold = TransCheck(TrackType.Right, rightWirstPos, rightWirstRot, HMDpose, rightTransHold);
                rightTransActive = HoldCheck(rightTransHold, transHoldDuration, rightTransActive, ref rightTransHoldTime);
            }

            if (trackType == TrackType.Left || trackType == TrackType.Any)
            {
                PXR_HandTracking.GetJointLocations(HandType.HandLeft, ref leftHandJointLocations);

                for (int i = 0; i < leftJointPos.Count; ++i)
                {
                    if (leftHandJointLocations.jointLocations == null) break;

                    leftJointPos[i] = leftHandJointLocations.jointLocations[i].pose.Position.ToVector3();

                    if (i == (int)HandJoint.JointWrist)
                    {
                        leftWirstPos = leftHandJointLocations.jointLocations[i].pose.Position.ToVector3();
                        leftWirstRot = leftHandJointLocations.jointLocations[i].pose.Orientation.ToQuat();
                    }
                }
                leftShapesHold = ShapesRecognizerCheck(leftJointPos, leftWirstRot * Vector3.right, leftWirstRot * Vector3.forward, -1);
                leftShapesActive = HoldCheck(leftShapesHold, shapesHoldDuration, leftShapesActive, ref leftShapesHoldTime);

                leftBonesHold = BonesCheck(HandType.HandLeft);
                leftBonesActive = HoldCheck(leftBonesHold, bonesHoldDuration, leftBonesActive, ref leftBonesHoldTime);

                leftTransHold = TransCheck(TrackType.Left, leftWirstPos, leftWirstRot, HMDpose, leftTransHold);
                leftTransActive = HoldCheck(leftTransHold, transHoldDuration, leftTransActive, ref leftTransHoldTime);
            }

            HandPoseEventCheck();
        }

        #region ShapesRecognizer
        private float shapesHoldDuration = 0.09f;

        private bool leftShapesHold;
        private bool leftShapesActive;
        private float leftShapesHoldTime;

        private bool rightShapesActive;
        private bool rightShapesHold;
        private float rightShapesHoldTime;

        private bool angleCheckValid = false;
        private bool abducCheckOpen = false;

        private Vector3 leftWirstPos;
        private Vector3 rightWirstPos;
        private Quaternion leftWirstRot;
        private Quaternion rightWirstRot;

        private Vector3 thumb0, thumb1, thumb2, thumb3;
        private Vector3 index0, index1, index2, index3;
        private Vector3 middle0, middle1, middle2, middle3;
        private Vector3 ring0, ring1, ring2, ring3;
        private Vector3 pinky0, pinky1, pinky2, pinky3;

        private bool thumbFlex, indexFlex, middleFlex, ringFlex, pinkyFlex;
        private bool thumbCurl, indexCurl, middleCurl, ringCurl, pinkyCurl;
        private bool thumbAbduc, indexAbduc, middleAbduc, ringAbduc, pinkyAbduc;

        private bool ShapesRecognizerCheck(List<Vector3> jointPos, Vector3 wirstRight, Vector3 wirstForward, int wirstDirect = 1)
        {
            thumb0 = jointPos[(int)HandJoint.JointThumbTip];
            thumb1 = jointPos[(int)HandJoint.JointThumbDistal];
            thumb2 = jointPos[(int)HandJoint.JointThumbProximal];
            thumb3 = jointPos[(int)HandJoint.JointThumbMetacarpal];

            index0 = jointPos[(int)HandJoint.JointIndexTip];
            index1 = jointPos[(int)HandJoint.JointIndexDistal];
            index2 = jointPos[(int)HandJoint.JointIndexIntermediate];
            index3 = jointPos[(int)HandJoint.JointIndexProximal];

            middle0 = jointPos[(int)HandJoint.JointMiddleTip];
            middle1 = jointPos[(int)HandJoint.JointMiddleDistal];
            middle2 = jointPos[(int)HandJoint.JointMiddleIntermediate];
            middle3 = jointPos[(int)HandJoint.JointMiddleProximal];

            ring0 = jointPos[(int)HandJoint.JointRingTip];
            ring1 = jointPos[(int)HandJoint.JointRingDistal];
            ring2 = jointPos[(int)HandJoint.JointRingIntermediate];
            ring3 = jointPos[(int)HandJoint.JointRingProximal];

            pinky0 = jointPos[(int)HandJoint.JointLittleTip];
            pinky1 = jointPos[(int)HandJoint.JointLittleDistal];
            pinky2 = jointPos[(int)HandJoint.JointLittleIntermediate];
            pinky3 = jointPos[(int)HandJoint.JointLittleProximal];

            thumbFlex = FlexionCheck(config.shapesRecognizer.thumb, wirstDirect * wirstRight, wirstDirect * wirstForward);
            indexFlex = FlexionCheck(config.shapesRecognizer.index, wirstRight, wirstForward);
            middleFlex = FlexionCheck(config.shapesRecognizer.middle, wirstRight, wirstForward);
            ringFlex = FlexionCheck(config.shapesRecognizer.ring, wirstRight, wirstForward);
            pinkyFlex = FlexionCheck(config.shapesRecognizer.pinky, wirstRight, wirstForward);

            thumbCurl = CurlCheck(config.shapesRecognizer.thumb);
            indexCurl = CurlCheck(config.shapesRecognizer.index);
            middleCurl = CurlCheck(config.shapesRecognizer.middle);
            ringCurl = CurlCheck(config.shapesRecognizer.ring);
            pinkyCurl = CurlCheck(config.shapesRecognizer.pinky);

            thumbAbduc = AbductionCheck(config.shapesRecognizer.thumb);
            indexAbduc = AbductionCheck(config.shapesRecognizer.index);
            middleAbduc = AbductionCheck(config.shapesRecognizer.middle);
            ringAbduc = AbductionCheck(config.shapesRecognizer.ring);
            pinkyAbduc = AbductionCheck(config.shapesRecognizer.pinky);

            return thumbFlex && indexFlex && middleFlex && ringFlex && pinkyFlex
                   && thumbCurl && indexCurl && middleCurl && ringCurl && pinkyCurl
                   && thumbAbduc && indexAbduc && middleAbduc && ringAbduc && pinkyAbduc;
        }
        private bool FlexionCheck(ShapesRecognizer.Finger finger, Vector3 wirstRight, Vector3 wirstForward)
        {
            if (finger.flexion == ShapesRecognizer.Flexion.Any) return true;
            else
            {
                float flexAngle = 0;
                switch (finger.handFinger)
                {
                    case HandFinger.Thumb:
                        Vector3 thumb23 = (thumb2 - thumb3);
                        Vector3 thumb23_project = Vector3.ProjectOnPlane(thumb23, wirstRight);
                        flexAngle = Vector3.Angle(thumb23_project, wirstForward);
                        break;
                    case HandFinger.Index:
                        Vector3 index23 = (index2 - index3);
                        Vector3 index_project = Vector3.ProjectOnPlane(index23, wirstForward);
                        flexAngle = Vector3.Angle(index_project, wirstRight);
                        break;
                    case HandFinger.Middle:
                        Vector3 middle23 = (middle2 - middle3);
                        Vector3 middle_project = Vector3.ProjectOnPlane(middle23, wirstForward);
                        flexAngle = Vector3.Angle(middle_project, wirstRight);
                        break;
                    case HandFinger.Ring:
                        Vector3 ring23 = (ring2 - ring3);
                        Vector3 ring_project = Vector3.ProjectOnPlane(ring23, wirstForward);
                        flexAngle = Vector3.Angle(ring_project, wirstRight);
                        break;
                    case HandFinger.Pinky:
                        Vector3 pinky23 = (pinky2 - pinky3);
                        Vector3 pinky_project = Vector3.ProjectOnPlane(pinky23, wirstForward);
                        flexAngle = Vector3.Angle(pinky_project, wirstRight);
                        break;
                    default:
                        break;
                }
                return AngleCheck(flexAngle, finger.fingerConfigs.flexionConfigs.min, finger.fingerConfigs.flexionConfigs.max, finger.fingerConfigs.flexionConfigs.width,
                    ShapesRecognizer.flexionMin, ShapesRecognizer.flexionMax);
            }

        }
        private bool CurlCheck(ShapesRecognizer.Finger finger)
        {
            if (finger.curl == ShapesRecognizer.Curl.Any) return true;
            else
            {
                float curlAngle = 0;
                switch (finger.handFinger)
                {
                    case HandFinger.Thumb:
                        Vector3 thumb01 = (thumb0 - thumb1);
                        Vector3 thumb32 = (thumb3 - thumb2);
                        curlAngle = Vector3.Angle(thumb01, thumb32);
                        break;
                    case HandFinger.Index:
                        Vector3 index01 = (index0 - index1);
                        Vector3 index32 = (index3 - index2);
                        curlAngle = Vector3.Angle(index32, index01);
                        break;
                    case HandFinger.Middle:
                        Vector3 middle01 = (middle0 - middle1);
                        Vector3 middle32 = (middle3 - middle2);
                        curlAngle = Vector3.Angle(middle32, middle01);
                        break;
                    case HandFinger.Ring:
                        Vector3 ring01 = (ring0 - ring1);
                        Vector3 ring32 = (ring3 - ring2);
                        curlAngle = Vector3.Angle(ring32, ring01);
                        break;
                    case HandFinger.Pinky:
                        Vector3 pinky01 = (pinky0 - pinky1);
                        Vector3 pinky32 = (pinky3 - pinky2);
                        curlAngle = Vector3.Angle(pinky32, pinky01);
                        break;
                    default:
                        break;
                }
                return AngleCheck(curlAngle, finger.fingerConfigs.curlConfigs.min, finger.fingerConfigs.curlConfigs.max, finger.fingerConfigs.curlConfigs.width,
                    ShapesRecognizer.curlMin, ShapesRecognizer.curlMax);
            }
        }
        private bool AbductionCheck(ShapesRecognizer.Finger finger)
        {
            if (finger.abduction == ShapesRecognizer.Abduction.Any) return true;
            else
            {
                float abducAngle = 0;
                Vector3 thumb12 = (thumb1 - thumb2);
                Vector3 index23 = (index2 - index3);
                Vector3 middle23 = (middle2 - middle3);
                Vector3 ring23 = (ring2 - ring3);
                Vector3 pinky23 = (pinky2 - pinky3);
                switch (finger.handFinger)
                {
                    case HandFinger.Thumb:
                        abducAngle = Vector3.Angle(thumb12, index23);
                        break;
                    case HandFinger.Index:
                        abducAngle = Vector3.Angle(index23, middle23);
                        break;
                    case HandFinger.Middle:
                        abducAngle = Vector3.Angle(middle23, ring23);
                        break;
                    case HandFinger.Ring:
                        abducAngle = Vector3.Angle(ring23, pinky23);
                        break;
                    case HandFinger.Pinky:
                        abducAngle = Vector3.Angle(pinky23, ring23);
                        break;
                    default:
                        break;
                }
                bool result = false;
                if (finger.abduction == ShapesRecognizer.Abduction.Open)
                {
                    result = AbducCheck(abducAngle, finger.fingerConfigs.abductionConfigs.mid, finger.fingerConfigs.abductionConfigs.width); ;
                }
                else if (finger.abduction == ShapesRecognizer.Abduction.Close)
                {
                    result = !AbducCheck(abducAngle, finger.fingerConfigs.abductionConfigs.mid, finger.fingerConfigs.abductionConfigs.width); ;
                }
                return result;
            }
        }
        private bool AngleCheck(float angle, float min, float max, float width, float rangeMin, float rangeMax)
        {
            if (angle > min && angle < max)
            {
                angleCheckValid = true;
            }
            if (min - rangeMin <= 1f)
            {
                angleCheckValid = angle < max;
            }
            else if (angle < (min - width))
            {
                angleCheckValid = false;
            }

            if (rangeMax - max <= 1f)
            {
                angleCheckValid = angle > min;
            }
            else if ((angle > (max + width)))
            {
                angleCheckValid = false;
            }

            return angleCheckValid;
        }
        private bool AbducCheck(float angle, float mid, float width)
        {
            if (angle > mid + width / 2)
            {
                abducCheckOpen = true;
            }
            if (angle < mid - width / 2)
            {
                abducCheckOpen = false;
            }
            return abducCheckOpen;
        }

        #endregion

        #region BonesRecognizer
        private List<BonesRecognizer.BonesGroup> bones;
        private bool leftBonesHold;
        private bool leftBonesActive;
        private float leftBonesHoldTime;

        private bool rightBonesHold;
        private bool rightBonesActive;
        private float rightBonesHoldTime;

        private float bonesHoldDuration;
        private bool BonesCheck(HandType handType)
        {
            for (int i = 0; i < bones.Count; i++)
            {
                float distance = Vector3.Distance(GetHandJoint(handType, bones[i].bone1), GetHandJoint(handType, bones[i].bone2));
                if (distance < bones[i].distance - bones[i].thresholdWidth / 2)
                {
                    bones[i].activeState = true;
                }
                else if (distance > bones[i].distance + bones[i].thresholdWidth / 2)
                {
                    bones[i].activeState = false;
                }

                if (!bones[i].activeState)
                {
                    return false;
                }
            }
            return true;
        }
        private Vector3 GetHandJoint(HandType hand, BonesRecognizer.HandBones bone)
        {
            if (hand == HandType.HandLeft)
            {
                return leftHandJointLocations.jointLocations[(int)bone].pose.Position.ToVector3();
            }
            else
            {
                return rightHandJointLocations.jointLocations[(int)bone].pose.Position.ToVector3();
            }
        }
        #endregion

        #region TransRecognizer
        private bool leftTransHold;
        private bool leftTransActive;
        private float leftTransHoldTime;

        private bool rightTransHold;
        private bool rightTransActive;
        private float rightTransHoldTime;

        private TransRecognizer.TrackAxis transTrackAxis;
        private TransRecognizer.SpaceType transSpaceType;
        private TransRecognizer.TrackTarget transTrackTarget;

        private float transAngleThreshold;
        private float transThresholdWidth;
        private float transHoldDuration;

        private Vector3 HMDpose;
        private Vector3 palmPos;
        private Vector3 palmAxis;
        private Vector3 targetPos;
        private bool TransCheck(TrackType trackType, Vector3 wristPos, Quaternion wristRot, Vector3 headPose, bool holdState)
        {
            GetTrackAxis(trackType, wristRot);
            GetProjectedTarget(headPose, wristRot, wristPos);

            float errorAngle = Vector3.Angle(palmAxis, targetPos);

            if (errorAngle < transAngleThreshold - transThresholdWidth / 2)
            {
                holdState = true;
            }
            if (errorAngle > transAngleThreshold + transThresholdWidth / 2)
            {
                holdState = false;
            }
            return holdState;
        }
        private Vector3 GetTrackAxis(TrackType trackType, Quaternion wristRot)
        {
            switch (transTrackAxis)
            {
                case TransRecognizer.TrackAxis.Fingers:
                    palmAxis = wristRot * Vector3.forward;
                    break;
                case TransRecognizer.TrackAxis.Palm:
                    palmAxis = wristRot * Vector3.down;
                    break;
                case TransRecognizer.TrackAxis.Thumb:
                    palmAxis = trackType == TrackType.Right ? wristRot * Vector3.left : wristRot * Vector3.right;
                    break;
            }

            return palmAxis;
        }
        private Vector3 GetProjectedTarget(Vector3 headPose, Quaternion wristRot, Vector3 wristPos)
        {
            palmPos = wristRot * (trackType == TrackType.Right ? new Vector3(0.08f, 0, 0) : new Vector3(-0.08f, 0, 0)) + wristPos;
            switch (transTrackTarget)
            {
                case TransRecognizer.TrackTarget.TowardsFace:
                    targetPos = headPose;
                    break;
                case TransRecognizer.TrackTarget.AwayFromFace:
                    targetPos = palmPos * 2 - headPose;
                    break;
                case TransRecognizer.TrackTarget.WorldUp:
                    targetPos = palmPos + Vector3.up;
                    break;
                case TransRecognizer.TrackTarget.WorldDown:
                    targetPos = palmPos + Vector3.down;
                    break;
            }
            targetPos -= palmPos;
            switch (transSpaceType)
            {
                case TransRecognizer.SpaceType.WorldSpace:
                    break;
                case TransRecognizer.SpaceType.LocalXY:
                    targetPos = Vector3.ProjectOnPlane(targetPos, wristRot * Vector3.forward);
                    break;
                case TransRecognizer.SpaceType.LocalXZ:
                    targetPos = Vector3.ProjectOnPlane(targetPos, wristRot * Vector3.up);
                    break;
                case TransRecognizer.SpaceType.LocalYZ:
                    targetPos = Vector3.ProjectOnPlane(targetPos, wristRot * Vector3.right);
                    break;
            }
            return targetPos;
        }
        #endregion

        [Serializable]
        public class UpdateEvent : UnityEvent<float> { }
    }
}