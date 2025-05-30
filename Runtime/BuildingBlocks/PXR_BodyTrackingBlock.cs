using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;
using UnityEngine.UI;
using System;

public class PXR_BodyTrackingBlock : MonoBehaviour
{
    public Transform skeletonJoints;
    public bool showCube = true;
    public float zDistance = 0;

    private bool supportedBT = false;
    private bool updateBT = true;

    private BodyTrackingGetDataInfo bdi = new BodyTrackingGetDataInfo();
    private BodyTrackingData bd = new BodyTrackingData();
    private Transform[] boneMapping = new Transform[(int)BodyTrackerRole.ROLE_NUM];
    BodyTrackingStatus bs = new BodyTrackingStatus();
    bool istracking = false;
    // Start is called before the first frame update
    void Start()
    {
        skeletonJoints.transform.localPosition += new Vector3(0, 0, zDistance);
        InitializeSkeletonJoints();
        StartBodyTracking();
    }

    // Update is called once per frame
    void Update()
    {
      

#if UNITY_ANDROID
        // Update bodytracking pose.
        if (updateBT )
        {
            PXR_MotionTracking.GetBodyTrackingState(ref istracking, ref bs);

            // If not calibrated, invoked system motion tracker app for calibration.
            if (bs.stateCode!=BodyTrackingStatusCode.BT_VALID)
            {
                return;
            }
            // Get the position and orientation data of each body node.
            int ret = PXR_MotionTracking.GetBodyTrackingData(ref bdi, ref bd);

            // if the return is successful
            if (ret == 0)
            {
                for (int i = 0; i < (int)BodyTrackerRole.ROLE_NUM; i++)
                {
                    var bone = boneMapping[i];
                    if (bone != null)
                    {
                        bone.transform.localPosition = new Vector3((float)bd.roleDatas[i].localPose.PosX, (float)bd.roleDatas[i].localPose.PosY, (float)bd.roleDatas[i].localPose.PosZ);
                        bone.transform.localRotation = new Quaternion((float)bd.roleDatas[i].localPose.RotQx, (float)bd.roleDatas[i].localPose.RotQy, (float)bd.roleDatas[i].localPose.RotQz, (float)bd.roleDatas[i].localPose.RotQw);
                    }
                }
            }
        }

#endif
    }

    public void StartBodyTracking()
    {
        // Query whether the current device supports human body tracking.
        PXR_MotionTracking.GetBodyTrackingSupported(ref supportedBT);
        if (!supportedBT)
        {
            return;
        }
        BodyTrackingBoneLength bones = new BodyTrackingBoneLength();

        // Start BodyTracking
        PXR_MotionTracking.StartBodyTracking(BodyJointSet.BODY_JOINT_SET_BODY_FULL_START, bones);
        
        PXR_MotionTracking.GetBodyTrackingState(ref istracking, ref bs);

        // If not calibrated, invoked system motion tracker app for calibration.
        if (bs.stateCode!=BodyTrackingStatusCode.BT_VALID)
        {
            if (bs.message==BodyTrackingMessage.BT_MESSAGE_TRACKER_NOT_CALIBRATED||bs.message==BodyTrackingMessage.BT_MESSAGE_UNKNOWN)
            {
                PXR_MotionTracking.StartMotionTrackerCalibApp();
            }
        }
        
        skeletonJoints.gameObject.SetActive(true);
        updateBT = true;
    }

    private void OnDestroy()
    {
        int ret = PXR_MotionTracking.StopBodyTracking();
        updateBT = false;
    }

    public void InitializeSkeletonJoints()
    {
        Queue<Transform> nodes = new Queue<Transform>();
        nodes.Enqueue(skeletonJoints);
        while (nodes.Count > 0)
        {
            Transform next = nodes.Dequeue();
            for (int i = 0; i < next.childCount; ++i)
            {
                nodes.Enqueue(next.GetChild(i));
            }

            ProcessJoint(next);
        }
    }

    void ProcessJoint(Transform joint)
    {
        int index = GetJointIndex(joint.name);
        if (index >= 0 && index < (int)BodyTrackerRole.ROLE_NUM)
        {
            boneMapping[index] = joint;
            Transform cubeT = joint.Find("Cube");
            if (cubeT)
            {
                cubeT.gameObject.SetActive(showCube);
            }
        }
        else
        {
            Debug.LogWarning($"{joint.name} was not found.");
        }
    }

    // Returns the integer value corresponding to the JointIndices enum value
    // passed in as a string.
    int GetJointIndex(string jointName)
    {
        BodyTrackerRole val;
        if (Enum.TryParse(jointName, out val))
        {
            return (int)val;
        }
        return -1;
    }
}
