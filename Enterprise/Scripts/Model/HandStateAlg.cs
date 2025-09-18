using System;
using UnityEngine;

namespace Unity.XR.PICO.TOBSupport
{
    [Serializable]
    public class HandStateAlg
    {
        public const int Hand_MaxBones = 26;
        public const int Hand_MaxPinch = 5;
        public const int Hand_MaxFingers = 5;
        public const int Hand_Pinch_Num = 4;
        public short status;
        public HandJointsLocation rootPose;
        public HandJointsLocation[] bonePose ;
        public short pinches;
        public float[] pinchStrength ;
        public float clickStrength;
        public Pose pointerPose;
        public float handScale;
        public int handConfidence;
        public int[] fingerConfidence ;
        public long requestedTimeStamp;
        public long sampleTimeStamp;
        public long outputTimeStamp;
        public short poseType = 0;
        public int handType ;
        public Pose rootPoseVel;
        public Pose[] bonePoseVel ;
        public Pose rootPoseAcc;
        public Pose[] bonePoseAcc;
        public short clickType ;
        public bool[] pinchActionReadyOpenxr ;
        public bool aimActionReadyOpenxr;
        public bool gripActionReadyOpenxr;
        public float[] pinchActionValueOpenxr ;
        public float aimActionValueOpenxr;
        public float gripActionValueOpenxr;
        public Pose[] pinchPoseOpenxr ;
        public Pose aimPoseOpenxr;
        public Pose pokePoseOpenxr;
        public Pose gripPoseOpenxr;
        public Pose gripSurfacePoseOpenxr;

        public HandStateAlg()
        {
            status = 0;
            rootPose = new HandJointsLocation();
            bonePose = new HandJointsLocation[Hand_MaxBones];
            for (int i = 0; i < Hand_MaxBones; i++) {
                bonePose[i] = new HandJointsLocation();
            }
            pinches = 0;
            pinchStrength = new float[Hand_MaxPinch];
            clickStrength = 0f;
            pointerPose = new Pose();
            handScale = 1f;
            handConfidence = 0;
            fingerConfidence = new int[Hand_MaxFingers];
            requestedTimeStamp = 0;
            sampleTimeStamp = 0;
            outputTimeStamp = 0;
            poseType = 0;
            handType = -1;
            rootPoseVel = new Pose();
            bonePoseVel = new Pose[Hand_MaxBones];
            rootPoseAcc = new Pose();
            bonePoseAcc = new Pose[Hand_MaxBones];
            clickType = -1;
            pinchActionReadyOpenxr = new bool[Hand_Pinch_Num];
            aimActionReadyOpenxr = false;
            gripActionReadyOpenxr = false;
            pinchActionValueOpenxr = new float[Hand_Pinch_Num];
            aimActionValueOpenxr = 0f;
            gripActionValueOpenxr = 0f;
            pinchPoseOpenxr = new Pose[Hand_Pinch_Num];
            aimPoseOpenxr = new Pose();
            pokePoseOpenxr = new Pose();
            gripPoseOpenxr = new Pose();
            gripSurfacePoseOpenxr = new Pose();
        }


        public static string ToJson(HandStateAlg data)
        {
            // 注意：JsonUtility 序列化数组时需包裹在对象中，这里直接序列化根对象即可
            return JsonUtility.ToJson(data, true); // 第二个参数为 true 时输出格式化的 JSON
        }
    }
    [Serializable]
    public class HandJointsLocation
    {
        public long locationFloags;
        public Pose pose;
        public float radius;
    }
}