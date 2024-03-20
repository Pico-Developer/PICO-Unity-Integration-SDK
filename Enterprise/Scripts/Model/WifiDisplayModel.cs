namespace Unity.XR.PICO.TOBSupport
{
    public class WifiDisplayModel
    {
        public static int STATUS_NOT_CONNECT = -1;
        public static int STATUS_NONE = 0;
        public static int STATUS_SCANNING = 1;
        public static int STATUS_CONNECTING = 2;
        public static int STATUS_AVAILABLE = 3;
        public static int STATUS_NOT_AVAILABLE = 4;
        public static int STATUS_IN_USE = 5;
        public static int STATUS_CONNECTED = 6;

        public string deviceAddress;
        public string deviceName;
        public bool isAvailable;
        public bool canConnect;
        public bool isRemembered;
        public int statusCode;
        public string status;
        public string description;
    }
}