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

public class PXR_Hand : MonoBehaviour
{
    public HandType handType;
    [HideInInspector]
    public List<Transform> handJoints = new List<Transform>(new Transform[(int)HandJoint.JointMax]);

    public bool Computed { get; private set; }
    public Posef RayPose { get; private set; }
    public bool RayValid { get; private set; }
    public bool Pinch { get; private set; }
    public float PinchStrength { get; private set; }

    private HandJointLocations handJointLocations = new HandJointLocations();
    private HandAimState aimState = new HandAimState();
    [SerializeField]
    private Transform rayPose;
    [SerializeField]
    private GameObject defaultRay;
    private SkinnedMeshRenderer[] touchRenders;

    private void Start()
    {
        if (defaultRay != null)
        {
            touchRenders = defaultRay.GetComponentsInChildren<SkinnedMeshRenderer>();
        }
    }

    private void Update()
    {
        UpdateHandJoints();
        UpdateAimState();
        UpdateRayPose();
    }

    private void UpdateHandJoints()
    {
        if (PXR_HandTracking.GetJointLocations(handType, ref handJointLocations))
        {
            if (handJointLocations.isActive == 0) return;

            transform.localScale = Vector3.one*handJointLocations.handScale;

            for (int i = 0; i < handJoints.Count; ++i)
            {
                if (handJoints[i] == null) continue;

                if (i == (int)HandJoint.JointWrist)
                {
                    handJoints[i].localPosition = handJointLocations.jointLocations[i].pose.Position.ToVector3();
                    handJoints[i].localRotation = handJointLocations.jointLocations[i].pose.Orientation.ToQuat();
                }
                else
                {
                    Pose parentPose = Pose.identity;

                    if (i == (int)HandJoint.JointPalm ||
                        i == (int)HandJoint.JointThumbMetacarpal ||
                        i == (int)HandJoint.JointIndexMetacarpal ||
                        i == (int)HandJoint.JointMiddleMetacarpal ||
                        i == (int)HandJoint.JointRingMetacarpal ||
                        i == (int)HandJoint.JointLittleMetacarpal)
                    {
                        parentPose = new Pose(handJointLocations.jointLocations[1].pose.Position.ToVector3(), handJointLocations.jointLocations[1].pose.Orientation.ToQuat());
                    }
                    else
                    {
                        parentPose = new Pose(handJointLocations.jointLocations[i-1].pose.Position.ToVector3(), handJointLocations.jointLocations[i-1].pose.Orientation.ToQuat());
                    }
                    
                    var inverseParentRotation = Quaternion.Inverse(parentPose.rotation);
                    handJoints[i].localRotation = inverseParentRotation * handJointLocations.jointLocations[i].pose.Orientation.ToQuat();
                }
            }
        }
    }

    private void UpdateAimState()
    {
        if (PXR_HandTracking.GetAimState(handType, ref aimState))
        {
            Computed = (aimState.aimStatus&HandAimStatus.AimComputed) != 0;

            RayPose = aimState.aimRayPose;
            RayValid = (aimState.aimStatus&HandAimStatus.AimRayValid) != 0;
            Pinch = (aimState.aimStatus&HandAimStatus.AimRayTouched) != 0;
            PinchStrength = aimState.touchStrengthRay;
        }
    }

    private void UpdateRayPose()
    {
        if (rayPose == null) return;

        if (RayValid)
        {
            rayPose.gameObject.SetActive(true);
            rayPose.localPosition = RayPose.Position.ToVector3();
            rayPose.localRotation = RayPose.Orientation.ToQuat();

            if (defaultRay != null)
            {
                foreach (var touchRender in touchRenders)
                {
                    touchRender.SetBlendShapeWeight(0, aimState.touchStrengthRay*100);
                }
            }
        }
        else
        {
            rayPose.gameObject.SetActive(false);
        }
    }
}
