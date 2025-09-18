namespace Unity.XR.PICO.TOBSupport
{
    public class PXRCapture
    {
        public const string VALUE_TRUE = "1";
        public const string VALUE_FALSE = "0";
        public const string KEY_MCTF = "enable-mctf";
        public const string KEY_EIS = "enable-eis";
        public const string KEY_MFNR = "enable-mfnr";
        public const string KEY_ENABLE_MVHEVC = "enable-mvhevc";
        public const string KEY_OUTPUT_CAMERA_RAW_DATA = "output-camera-raw-data";
        //与视频录制有关，目前暂不支持
        public const string KEY_VIDEO_FPS = "video-fps";
        public const string KEY_VIDEO_WIDTH = "video-width";
        public const string KEY_VIDEO_HEIGHT = "video-height";
        public const string KEY_VIDEO_BITRATE = "video-bit-rate";
        public const string KEY_WRITE_DEPTH_DATA = "write-depth-data";
        public const string KEY_WRITE_POSE_DATA = "write-pose-data";
        public const string KEY_WRITE_CAMERA_PARAMS_DATA = "write-camera-params-data";
        
        
        public const int CAPTURE_STATUS_STREAM_TIME_OUT = -100;
        public const int CAPTURE_STATUS_PREPROCESS_ERROR = -99;
        public const int CAPTURE_STATUS_RECORD_ERROR = -98;
        public const int CAPTURE_STATUS_NO_PERMISSION = -97;
        public const int CAPTURE_STATUS_SERVER_DIED = -96;
        public const int CAPTURE_STATUS_UNKNOWN = -95;
        public const int CAPTURE_STATUS_OK = 0;
        
        

    }
}