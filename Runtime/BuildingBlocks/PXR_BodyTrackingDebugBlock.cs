using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;
using UnityEngine.UI;
using System;
using static UnityEngine.UI.Dropdown;

public class PXR_BodyTrackingDebugBlock : MonoBehaviour
{
    public Transform skeletonJoints;
    public bool showCube = true;
    public float zDistance = 0;

    public Dropdown dropdown;
    public Text changeJointTittle;


    private bool supportedBT = false;
    private bool updateBT = true;
    private Transform changeJointT;

    private BodyTrackingGetDataInfo bdi = new BodyTrackingGetDataInfo();
    private BodyTrackingData bd = new BodyTrackingData();
    private Transform[] boneMapping = new Transform[(int)BodyTrackerRole.ROLE_NUM];
    private Transform[] targetMapping = new Transform[(int)BodyTrackerRole.ROLE_NUM];
    BodyTrackingStatus bs = new BodyTrackingStatus();
    bool istracking = false;

    // Start is called before the first frame update
    void Start()
    {
        dropdown.ClearOptions();
        skeletonJoints.transform.localPosition += new Vector3(0, 0, zDistance);
        InitializeSkeletonJoints();
        StartBodyTracking();
        dropdown.RefreshShownValue();
        dropdown.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(dropdown);
        });
    }

    // Update is called once per frame
    void Update()
    {

#if UNITY_ANDROID
        // Update bodytracking pose.
        if (updateBT)
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
            OptionData optionData = new OptionData();
            optionData.text = joint.name;
            dropdown.options.Add(optionData);

            if (index == 0)
            {
                changeJointT = cubeT;
                var cubeRenderer = changeJointT.GetComponent<Renderer>();
                cubeRenderer.material.SetColor("_Color", Color.green);
                if (changeJointTittle)
                {
                    changeJointTittle.text = "Joint Rotation : " + joint.name;
                }
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

    void DropdownValueChanged(Dropdown change)
    {
        if (changeJointTittle)
        {
            changeJointTittle.text = "Joint Rotation : " + change.options[change.value].text;
        }

        foreach (var b in boneMapping)
        {
            changeJointT = b.Find("Cube");
            var cubeRenderer = changeJointT.GetComponent<Renderer>();
            cubeRenderer.material.SetColor("_Color", Color.white);
        }

        var bone = boneMapping[change.value];
        if (bone)
        {
            changeJointT = bone.Find("Cube");
            var cubeRenderer = changeJointT.GetComponent<Renderer>();
            cubeRenderer.material.SetColor("_Color", Color.green);
        }
    }

    public void SetRotationX(float x)
    {
        if (changeJointT)
        {
            changeJointT.localRotation = Quaternion.AngleAxis(x, Vector3.right);
        }
    }

    public void SetRotationY(float y)
    {
        if (changeJointT)
        {
            changeJointT.localRotation = Quaternion.AngleAxis(y, Vector3.up);
        }
    }

    public void SetRotationZ(float z)
    {
        if (changeJointT)
        {
            changeJointT.localRotation = Quaternion.AngleAxis(z, Vector3.forward);
        }
    }
}