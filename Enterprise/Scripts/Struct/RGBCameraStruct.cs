using System;
using System.Runtime.InteropServices;

namespace Unity.XR.PICO.TOBSupport
{
    // pico slam results
    [StructLayout(LayoutKind.Sequential)]
    public struct SixDof
    {
        public Int64 timestamp;     // nanoseconds
        public double x;            // position X
        public double y;            // position Y
        public double z;            // position Z
        public double rw;           // rotation W
        public double rx;           // rotation X
        public double ry;           // rotation Y
        public double rz;           // rotation Z
        public byte type;           //1:6DOF 0:3DOF 
        public byte confidence;     //1:good 0:bad
        public PoseErrorType error;
        public double plane_height;
        public byte plane_status;
        public byte relocation_status;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AlgoResult
    {
        public SixDof pose;
        public SixDof relocation_pose;
        public double vx, vy, vz;        // linear velocity
        public double ax, ay, az;        // linear acceleration
        public double wx, wy, wz;        // angular velocity
        public double w_ax, w_ay, w_az;  // angular acceleration
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FrameItem
    {
        public byte camera_id;
        public UInt32 width;                // width
        public UInt32 height;               // height
        public UInt32 format;               // format - rgb24
        public UInt32 exposure_duration;    // exposure duration:ns
        public UInt64 timestamp;            // start of exposure time:ns (BOOTTIME)
        public UInt64 qtimer_timestamp;     // nanoseconds in qtimer
        public UInt64 framenumber;          // frame number
        public UInt32 datasize;             // datasize
        public IntPtr data;                 // image data. 
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FrameItemExt
    {
        public FrameItem frame;
        public bool is_rgb;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public double[] rgb_tsw_matrix;
        public bool is_anti_distortion;
        public AlgoResult six_dof_pose;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Frame
    {
        public UInt32 width;          // width
        public UInt32 height;         // height
        public UInt64 timestamp;      // start of exposure time:ns (BOOTTIME)
        public UInt32 datasize;       // datasize
        public IntPtr data;           // image data 
        public UnityEngine.Pose pose; // The head Pose at the time of image production.（Right-handed coordinate system: X right, Y up, Z in）
        public int status;            // sensor status(1:good 0:bad)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SensorState
    {
        public UnityEngine.Pose pose; // Predict the head Pose at the screen up time.（Right-handed coordinate system: X right, Y up, Z in）
        public int status;            // sensor status(1:good 0:bad)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RGBCameraParams
    {
        // Intrinsics
        public double fx;
        public double fy;
        public double cx;
        public double cy;
        // Extrinsics
        public double x;
        public double y;
        public double z;
        public double rw;
        public double rx;
        public double ry;
        public double rz;
    }
}