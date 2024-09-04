#if AR_FOUNDATION
using Unity.Collections;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public class PXR_HumanBodySubsystem : XRHumanBodySubsystem
{
    internal const string k_SubsystemId = "PXR_HumanBodySubsystem";

    class HumanBodyProvider : Provider
    {
        bool isBodyTrackingSupported = false;
        bool init = false;

        private BodyTrackingGetDataInfo bdi = new BodyTrackingGetDataInfo();
        private BodyTrackingData bd = new BodyTrackingData();

        public override void Start()
        {
            PLog.i(k_SubsystemId, "Start");
            init = true;
        }

        public override void Stop()
        {
            PLog.i(k_SubsystemId, "Stop");
            int ret = PXR_MotionTracking.StopBodyTracking();
        }

        public override void Destroy()
        {
            PLog.i(k_SubsystemId, "Destroy");
        }

        public override void GetSkeleton(TrackableId trackableId, Allocator allocator, ref NativeArray<XRHumanBodyJoint> skeleton)
        {
            PLog.d(k_SubsystemId, $"GetSkeleton isBodyTrackingSupported={isBodyTrackingSupported}");
#if UNITY_ANDROID
            if (isBodyTrackingSupported)
            {
                // Get the current tracking mode, either bodytracking or motiontracking.
                MotionTrackerMode trackingMode = PXR_MotionTracking.GetMotionTrackerMode();

                // Update bodytracking pose.
                if (trackingMode == MotionTrackerMode.BodyTracking)
                {
                    // Get the position and orientation data of each body node.
                    int ret = PXR_MotionTracking.GetBodyTrackingData(ref bdi, ref bd);

                    // if the return is successful
                    if (ret == 0)
                    {
                        skeleton = new NativeArray<XRHumanBodyJoint>((int)BodyTrackerRole.ROLE_NUM, allocator);
                        for (int i = 0; i < (int)BodyTrackerRole.ROLE_NUM; i++)
                        {
                            BodyTrackerTransPose localPose = bd.roleDatas[i].localPose;
                            Vector3 pos = new Vector3((float)bd.roleDatas[i].localPose.PosX, (float)bd.roleDatas[i].localPose.PosY, (float)bd.roleDatas[i].localPose.PosZ);
                            Quaternion qu = new Quaternion((float)bd.roleDatas[i].localPose.RotQx, (float)bd.roleDatas[i].localPose.RotQy, (float)bd.roleDatas[i].localPose.RotQz, (float)bd.roleDatas[i].localPose.RotQw);

                            if (i == 0)
                            {
                                qu *= Quaternion.Euler(new Vector3(0, 180, 0));
                            }
                            else if (i == 1)
                            {
                                qu *= Quaternion.Euler(new Vector3(0, 0, -95));
                            }
                            else if (i == 2)
                            {
                                qu *= Quaternion.Euler(new Vector3(0, 0, 95));
                            }
                            else if (i == 4)
                            {
                                qu *= Quaternion.Euler(new Vector3(0, 0, -90));
                            }
                            else if (i == 3 || i == 5 || i == 12)
                            {
                                qu *= Quaternion.Euler(new Vector3(0, 0, 90));
                            }
                            else if (i == 7)
                            {
                                qu *= Quaternion.Euler(new Vector3(180, -90, 0));
                            }
                            else if (i == 6 || i == 9 || i == 15)
                            {
                                qu *= Quaternion.Euler(new Vector3(0, 90, 90));
                            }
                            else if (i == 8 || i == 10 || i == 11)
                            {
                                qu *= Quaternion.Euler(new Vector3(0, 90, 0));
                            }
                            else if (i == 13 || i == 14 || i == 16 || i == 17 || i == 18 || i == 19 || i == 20)
                            {
                                qu *= Quaternion.Euler(new Vector3(0, 0, 180));
                            }
                            else if (i == 21)
                            {
                                qu *= Quaternion.Euler(new Vector3(180, 0, 180));
                            }
                            Pose pose = new Pose(pos, qu);
                            XRHumanBodyJoint mXRHumanBodyJoint = new XRHumanBodyJoint(i, 0, Vector3.one, pose, Vector3.one, pose, true);
                            skeleton[i] = mXRHumanBodyJoint;
                        }
                    }
                }
            }
#endif
        }

        public override TrackableChanges<XRHumanBody> GetChanges(XRHumanBody defaultHumanBody, Allocator allocator)
        {
            PLog.d(k_SubsystemId, $"GetChanges init={init}, bodyTracking={PXR_ProjectSetting.GetProjectConfig().bodyTracking} ");
            if (init)
            {
                if (PXR_ProjectSetting.GetProjectConfig().bodyTracking)
                {
                    PXR_Plugin.MotionTracking.UPxr_WantBodyTrackingService();

                    // Query whether the current device supports human body tracking.
                    PXR_MotionTracking.GetBodyTrackingSupported(ref isBodyTrackingSupported);

                    if (isBodyTrackingSupported)
                    {
                        BodyTrackingBoneLength bones = new BodyTrackingBoneLength();

                        // Start BodyTracking
                        PXR_MotionTracking.StartBodyTracking(BodyTrackingMode.BTM_FULL_BODY_HIGH, bones);

                        // Has Pico motion tracker completed calibration (0: not completed; 1: completed)?
                        int calibrated = 0;
                        PXR_Input.GetMotionTrackerCalibState(ref calibrated);

                        // If not calibrated, invoked system motion tracker app for calibration.
                        if (calibrated == 0)
                        {
                            PXR_MotionTracking.StartMotionTrackerCalibApp();
                        }
                    }
                }
                init = false;
                return new TrackableChanges<XRHumanBody>(1, 0, 0, allocator);
            }
            else
            {
                return new TrackableChanges<XRHumanBody>(0, 1, 0, allocator);
            }
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void RegisterDescriptor()
    {
        PLog.i(k_SubsystemId, "RegisterDescriptor");
        var mXRHumanBodySubsystemCinfo = new XRHumanBodySubsystemCinfo
        {
            id = k_SubsystemId,
            providerType = typeof(HumanBodyProvider),
            subsystemTypeOverride = typeof(PXR_HumanBodySubsystem),
            supportsHumanBody2D = false,
            supportsHumanBody3D = true,
            supportsHumanBody3DScaleEstimation = true,
        };

        if (!Register(mXRHumanBodySubsystemCinfo))
        {
            PLog.e(k_SubsystemId, $"Failed to register the {k_SubsystemId} subsystem.");
        }
        else
        {
            PLog.i(k_SubsystemId, $"success to register the {k_SubsystemId} subsystem.");
        }
    }

}
#endif