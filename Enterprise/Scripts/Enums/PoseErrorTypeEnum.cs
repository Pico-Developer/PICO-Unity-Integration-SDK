using System;
using System.Runtime.InteropServices;

namespace Unity.XR.PICO.TOBSupport
{
    public enum PoseErrorType
    {
        BRIGHT_LIGHT_ERROR = (1 << 0),
        LOW_LIGHT_ERROR = (1 << 1),
        LOW_FEATURE_COUNT_ERROR = (1 << 2),
        CAMERA_CALIBRATION_ERROR = (1 << 3),
        RELOCATION_IN_PROGRESS = (1 << 4),
        INITILIZATION_IN_PROGRESS = (1 << 5),
        NO_CAMERA_ERROR = (1 << 6),
        NO_IMU_ERROR = (1 << 7),
        IMU_JITTER_ERROR = (1 << 8),
        UNKNOWN_ERROR = (1 << 9)
    };

}